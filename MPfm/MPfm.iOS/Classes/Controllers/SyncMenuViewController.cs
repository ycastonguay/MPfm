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
using MPfm.Library.Objects;

namespace MPfm.iOS
{
    public partial class SyncMenuViewController : BaseViewController, ISyncMenuView
    {
        string _cellIdentifier = "SyncMenuCell";
        UIBarButtonItem _btnSync;
        List<SyncMenuItemEntity> _items = new List<SyncMenuItemEntity>();
        float _nowPlayingButtonPreviousAlpha = 0;

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
            btnSelect.BackgroundColor = GlobalTheme.SecondaryColor;
            btnSelect.Layer.CornerRadius = 8;

            btnSelect.SetTitle("Select all", UIControlState.Normal);
            activityIndicator.StartAnimating();

            UILongPressGestureRecognizer longPress = new UILongPressGestureRecognizer(HandleLongPress);
            longPress.MinimumPressDuration = 0.7f;
            longPress.WeakDelegate = this;
            tableView.AddGestureRecognizer(longPress);

            var btnSync = new MPfmFlatButton();
            btnSync.LabelAlignment = UIControlContentHorizontalAlignment.Right;
            btnSync.Label.Text = "Sync";
            btnSync.Label.TextAlignment = UITextAlignment.Right;
            btnSync.Label.Frame = new RectangleF(0, 0, 44, 44);
            btnSync.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_blue");
            btnSync.ImageChevron.Frame = new RectangleF(70 - 22, 0, 22, 44);
            btnSync.Frame = new RectangleF(0, 0, 70, 44);
            btnSync.OnButtonClick += HandleButtonSyncTouchUpInside;
            var btnSyncView = new UIView(new RectangleF(UIScreen.MainScreen.Bounds.Width - 70, 0, 70, 44));
            var rect2 = new RectangleF(btnSyncView.Bounds.X - 5, btnSyncView.Bounds.Y, btnSyncView.Bounds.Width, btnSyncView.Bounds.Height);
            btnSyncView.Bounds = rect2;
            btnSyncView.AddSubview(btnSync);
            _btnSync = new UIBarButtonItem(btnSyncView);

            NavigationItem.SetRightBarButtonItem(_btnSync, true);

            viewLoading.Hidden = false;
            viewSync.Hidden = true;
            tableView.Hidden = true;

            base.ViewDidLoad();
        }       

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Sync Library", "Choose audio files to sync");

            _nowPlayingButtonPreviousAlpha = navCtrl.BtnNowPlaying.Alpha;
            navCtrl.BtnNowPlaying.Alpha = 0;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.BtnNowPlaying.Alpha = _nowPlayingButtonPreviousAlpha;
        }

        private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
                return;

            PointF pt = gestureRecognizer.LocationInView(tableView);
            NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(pt);
            if (indexPath != null)
            {
                if(OnSelectItems != null)
                {
                    OnSelectItems(new List<SyncMenuItemEntity>() { _items[indexPath.Row] });
                    RefreshCell(indexPath);
                }
            }
        }

        private void HandleButtonSyncTouchUpInside()
        {
            OnSync();
        }

        partial void actionSelect(NSObject sender)
        {
            OnSelectButtonClick();
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
                    cell.TextLabel.Font = UIFont.FromName("HelveticaNeue", 16);
                    cell.IndexTextLabel.Text = string.Empty;
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_user");
                    break;
                case SyncMenuItemEntityType.Album:
                    cell.TextLabel.Text = _items[indexPath.Row].AlbumTitle;
                    cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
                    cell.IndexTextLabel.Text = string.Empty;
                    cell.ImageView.Image = UIImage.FromBundle("Images/Icons/icon_vinyl");
                    break;
                case SyncMenuItemEntityType.Song:
                    cell.TextLabel.Text = _items[indexPath.Row].Song.Title;
                    cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 14);
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
            OnSelectItems(new List<SyncMenuItemEntity>() { _items[row] });

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
                    OnExpandItem(_items[indexPath.Row], null);
                    break;
                case SyncMenuItemEntityType.Album:
                    OnExpandItem(_items[indexPath.Row], null);
                    break;
                case SyncMenuItemEntityType.Song:
                    OnSelectItems(new List<SyncMenuItemEntity>() { _items[indexPath.Row] });
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

        public Action<SyncMenuItemEntity, object> OnExpandItem { get; set; }
        public Action<List<SyncMenuItemEntity>> OnSelectItems { get; set; }
        public Action OnSync { get; set; }
        public Action OnSelectButtonClick { get; set; }
        public Action<List<AudioFile>> OnRemoveItems { get; set; }
        public Action OnSelectAll { get; set; }
        public Action OnRemoveAll { get; set; }

        public void SyncMenuError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Sync Menu Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void SyncEmptyError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Sync Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshDevice(SyncDevice device)
        {
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            //Console.WriteLine("SyncMenuViewController - isLoading: {0} progressPercentage: {1}", isLoading, progressPercentage);
            InvokeOnMainThread(() => {
                tableView.Hidden = isLoading;
                viewLoading.Hidden = !isLoading;
                viewSync.Hidden = isLoading;

                if(isLoading)
                    NavigationItem.SetRightBarButtonItem(null, true);
                else
                    NavigationItem.SetRightBarButtonItem(_btnSync, true);                   

                if(progressPercentage < 100)
                    lblLoading.Text = String.Format("Loading index ({0}%)...", progressPercentage);
                else
                    lblLoading.Text = "Processing index...";
            });
        }

        public void RefreshSelectButton(string text)
        {
            InvokeOnMainThread(() => {
                btnSelect.SetTitle(text, UIControlState.Normal);
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

        public void InsertItems(int index, SyncMenuItemEntity parentItem, List<SyncMenuItemEntity> items, object userData)
        {
            InvokeOnMainThread(() => {
                _items.InsertRange(index, items);

                List<NSIndexPath> indexPaths = new List<NSIndexPath>();
                for(int a = index; a < index + items.Count; a++)
                    indexPaths.Add(NSIndexPath.FromRowSection(a, 0));

                tableView.InsertRows(indexPaths.ToArray(), UITableViewRowAnimation.Top);
            });
        }

        public void RemoveItems(int index, int count, object userData)
        {
            InvokeOnMainThread(() => {
                _items.RemoveRange(index, count);

                List<NSIndexPath> indexPaths = new List<NSIndexPath>();
                for(int a = index; a < index + count; a++)
                    indexPaths.Add(NSIndexPath.FromRowSection(a, 0));

                tableView.DeleteRows(indexPaths.ToArray(), UITableViewRowAnimation.Top);
            });
        }

        public void RefreshSelection(List<AudioFile> audioFiles)
        {
            // Not used on mobile devices.
        }

        #endregion

    }
}
