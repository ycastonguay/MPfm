// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Objects;
using System.Collections.Generic;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Models;
using MPfm.iOS.Classes.Controls;

namespace MPfm.iOS
{
    public partial class SyncMenuViewController : BaseViewController, ISyncMenuView
    {
        string _cellIdentifier = "SyncMenuCell";
        List<SyncMenuItemEntity> _items = new List<SyncMenuItemEntity>();

        public SyncMenuViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "SyncMenuViewController_iPhone" : "SyncMenuViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            this.View.BackgroundColor = GlobalTheme.BackgroundColor;
            viewSync.BackgroundColor = GlobalTheme.MainColor;
            btnSync.BackgroundColor = GlobalTheme.SecondaryColor;
            btnSync.Layer.CornerRadius = 8;

            activityIndicator.StartAnimating();

            UILongPressGestureRecognizer longPress = new UILongPressGestureRecognizer(HandleLongPress);
            longPress.MinimumPressDuration = 0.7f;
            longPress.WeakDelegate = this;
            tableView.AddGestureRecognizer(longPress);

            base.ViewDidLoad();

            viewLoading.Hidden = false;
            viewSync.Hidden = true;
            tableView.Hidden = true;
        }

        private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
                return;

            PointF pt = gestureRecognizer.LocationInView(tableView);
            NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(pt);
            if (indexPath != null)
            {
                if(OnSelectItem != null)
                {
                    OnSelectItem(_items[indexPath.Row]);
                    RefreshCell(indexPath);
                }
            }
        }

        partial void actionSync(NSObject sender)
        {
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableView, int section)
        {
            return _items.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                cell = new MPfmTableViewCell(UITableViewCellStyle.Subtitle, _cellIdentifier);
                cell.OnRightButtonTap += HandleOnRightButtonTap;
            }

            cell.Tag = indexPath.Row;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
            cell.ImageView.Alpha = 0.7f;
            cell.RightButton.Alpha = 0.7f;
            cell.RightButton.Hidden = false;

            if(_items[indexPath.Row].Selection == StateSelectionType.Selected)
                cell.RightButton.SetImage(UIImage.FromBundle("Images/Icons/icon_checkbox_checked"), UIControlState.Normal);
            else if(_items[indexPath.Row].Selection == StateSelectionType.PartlySelected)
                cell.RightButton.SetImage(UIImage.FromBundle("Images/Icons/icon_checkbox_partial"), UIControlState.Normal);
            else
                cell.RightButton.SetImage(UIImage.FromBundle("Images/Icons/icon_checkbox_unchecked"), UIControlState.Normal);

            switch(_items[indexPath.Row].ItemType)
            {
                case SyncMenuItemEntityType.Artist:
                    cell.TextLabel.Text = _items[indexPath.Row].ArtistName;
                    cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 16);
                    cell.IndexTextLabel.Text = string.Empty;
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_user");
                    break;
                case SyncMenuItemEntityType.Album:
                    cell.TextLabel.Text = _items[indexPath.Row].AlbumTitle;
                    cell.TextLabel.Font = UIFont.FromName("HelveticaNeue", 16);
                    cell.IndexTextLabel.Text = string.Empty;
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_vinyl");
                    break;
                case SyncMenuItemEntityType.Song:
                    cell.TextLabel.Text = _items[indexPath.Row].Song.Title;
                    cell.TextLabel.Font = UIFont.FromName("HelveticaNeue", 14);
                    cell.IndexTextLabel.Text = _items[indexPath.Row].Song.TrackNumber.ToString();
                    cell.ImageView.Image = null;
                    break;
            }

            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;

            return cell;
        }

        private void HandleOnRightButtonTap(MPfmTableViewCell cell)
        {
            Console.WriteLine("SyncMenuViewController - HandleOnRightButtonTap");
            int row = cell.Tag;
            OnSelectItem(_items[row]);

            tableView.BeginUpdates();
            tableView.ReloadRows(new NSIndexPath[1] { NSIndexPath.FromRowSection(row, 0) }, UITableViewRowAnimation.None);
            tableView.EndUpdates();
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            switch(_items[indexPath.Row].ItemType)
            {
                case SyncMenuItemEntityType.Artist:
                    OnExpandItem(_items[indexPath.Row]);
                    break;
                case SyncMenuItemEntityType.Album:
                    OnExpandItem(_items[indexPath.Row]);
                    break;
                case SyncMenuItemEntityType.Song:
                    OnSelectItem(_items[indexPath.Row]);
                    RefreshCell(indexPath);
                    break;
            }

            tableView.DeselectRow(indexPath, true);
        }

        private void RefreshCell(NSIndexPath indexPath)
        {
            tableView.ReloadRows(new NSIndexPath[1] { indexPath }, UITableViewRowAnimation.None);
        }

        #region ISyncMenuView implementation

        public Action<SyncMenuItemEntity> OnExpandItem { get; set; }
        public Action<SyncMenuItemEntity> OnSelectItem { get; set; }

        public void SyncMenuError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Sync Menu Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            InvokeOnMainThread(() => {
                tableView.Hidden = isLoading;
                viewLoading.Hidden = !isLoading;
                viewSync.Hidden = isLoading;

                lblLoading.Text = String.Format("Loading index ({0}%)...", progressPercentage);
            });
        }

        public void RefreshItems(List<SyncMenuItemEntity> items)
        {
            InvokeOnMainThread(() => {
                _items = items;
                tableView.ReloadData();
            });
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
            InvokeOnMainThread(() => {
                lblTotal.Text = title;
                lblFreeSpace.Text = subtitle;

                if(enoughFreeSpace)
                    lblFreeSpace.TextColor = UIColor.White;
                else
                    lblFreeSpace.TextColor = UIColor.Red;
            });
        }

        public void InsertItems(int index, List<SyncMenuItemEntity> items)
        {
            InvokeOnMainThread(() => {
                _items.InsertRange(index, items);

                List<NSIndexPath> indexPaths = new List<NSIndexPath>();
                for(int a = index; a < index + items.Count; a++)
                    indexPaths.Add(NSIndexPath.FromRowSection(a, 0));

                tableView.InsertRows(indexPaths.ToArray(), UITableViewRowAnimation.Top);
            });
        }

        public void RemoveItems(int index, int count)
        {
            InvokeOnMainThread(() => {
                _items.RemoveRange(index, count);

                List<NSIndexPath> indexPaths = new List<NSIndexPath>();
                for(int a = index; a < index + count; a++)
                    indexPaths.Add(NSIndexPath.FromRowSection(a, 0));

                tableView.DeleteRows(indexPaths.ToArray(), UITableViewRowAnimation.Top);
            });
        }

        #endregion
    }
}
