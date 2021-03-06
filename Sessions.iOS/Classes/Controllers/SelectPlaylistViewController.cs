// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.MVP.Views;
using Sessions.MVP.Models;
using System.Linq;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.Core;
using Sessions.iOS.Helpers;

namespace Sessions.iOS
{
    public partial class SelectPlaylistViewController : BaseViewController, ISelectPlaylistView
    {
        List<PlaylistEntity> _items;
        LibraryBrowserEntity _item;

        string _cellIdentifier = "SelectPlaylistCell";

        public SelectPlaylistViewController(LibraryBrowserEntity item)
			: base (UserInterfaceIdiomIsPhone ? "SelectPlaylistViewController_iPhone" : "SelectPlaylistViewController_iPad", null)
        {
            _item = item;
        }

        public override void ViewDidLoad()
        {
            viewPanel.Layer.CornerRadius = 8;
            _items = new List<PlaylistEntity>();
            tableView.BackgroundColor = UIColor.Clear;
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            btnAddNewPlaylist.SetImage(UIImage.FromBundle("Images/Buttons/add"));
            btnCancel.SetImage(UIImage.FromBundle("Images/Buttons/cancel"));
            btnSelect.SetImage(UIImage.FromBundle("Images/Buttons/select"));

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindSelectPlaylistView(this, _item);
        }

		public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			var screenSize = UIKitHelper.GetDeviceSize();
			View.Frame = new RectangleF(0, 0, screenSize.Width, screenSize.Height);
		}

        partial void actionAddNewPlaylist(NSObject sender)
        {
            OnAddNewPlaylist();
        }

        partial void actionSelect(NSObject sender)
        {
            //OnSelectPlaylist();
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
            SessionsPlaylistTableViewCell cell = (SessionsPlaylistTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new SessionsPlaylistTableViewCell(cellStyle, _cellIdentifier);
            }

            cell.Accessory = UITableViewCellAccessory.None;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            cell.TextLabel.Text = _items[indexPath.Row].Name;
            cell.TextLabel.TextColor = UIColor.White;
            cell.TextLabel.BackgroundColor = UIColor.Clear;
            cell.BackgroundColor = UIColor.Clear;

            UIView viewBackground = new UIView();
            viewBackground.BackgroundColor = UIColor.Clear;
            cell.BackgroundView = viewBackground;

            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
        }

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
