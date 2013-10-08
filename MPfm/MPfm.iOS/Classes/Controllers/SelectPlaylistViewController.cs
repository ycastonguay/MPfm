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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Views;
using MPfm.MVP.Models;
using System.Linq;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS
{
    public partial class SelectPlaylistViewController : BaseViewController, ISelectPlaylistView
    {
        List<PlaylistEntity> _items;

        string _cellIdentifier = "SelectPlaylistCell";

        public SelectPlaylistViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "SelectPlaylistViewController_iPhone" : "SelectPlaylistViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            _items = new List<PlaylistEntity>();
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Select a playlist", "");
        }

        partial void actionAddNewPlaylist(NSObject sender)
        {

        }

        partial void actionSelect(NSObject sender)
        {

        }

        partial void actionCancel(NSObject sender)
        {
            WillMoveToParentViewController(null);
            UIView.Animate(0.2f, () => {
                this.View.Alpha = 0;
            }, () => {
                View.RemoveFromSuperview();
                RemoveFromParentViewController();
            });
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _items.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new MPfmTableViewCell(cellStyle, _cellIdentifier);
            }

            //cell.ImageView.Alpha = 0.7f;
            cell.Accessory = UITableViewCellAccessory.None;
            //cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
            //cell.ImageChevron.Hidden = false;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            cell.TextLabel.Text = _items[indexPath.Row].Name;

            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //OnItemClick(_items[indexPath.Row].Key);
        }

//        [Export ("tableView:didHighlightRowAtIndexPath:")]
//        public void DidHighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
//        {
//            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
//            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_white");
//        }
//
//        [Export ("tableView:didUnhighlightRowAtIndexPath:")]
//        public void DidUnhighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
//        {
//            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
//            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
//        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }

        #region ISelectPlaylistView implementation

        public Action OnAddNewPlaylist { get; set; }
        public Action<PlaylistEntity> OnSelectPlaylist { get; set; }

        public void SelectPlaylistError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("SelectPlaylist Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshPlaylists(List<PlaylistEntity> playlists)
        {
            InvokeOnMainThread(() => {
                _items = playlists.ToList();
                tableView.ReloadData();
            });
        }

        #endregion
    }
}

