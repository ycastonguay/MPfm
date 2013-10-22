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
using System.Collections.Generic;
using System.Drawing;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Services;
using MPfm.Library.Objects;
using System.Linq;

namespace MPfm.iOS
{
    public partial class ResumePlaybackViewController : BaseViewController, IResumePlaybackView
    {
        string _cellIdentifier = "ResumePlaybackCell";
        List<CloudDeviceInfo> _devices = new List<CloudDeviceInfo>();

        public ResumePlaybackViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "ResumePlaybackViewController_iPhone" : "ResumePlaybackViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Resume Playback", "");
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _devices.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var device = _devices[indexPath.Row];
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new MPfmTableViewCell(cellStyle, _cellIdentifier);
            }

            cell.ImageView.Hidden = true;
            cell.TextLabel.Text = string.Format("{0}/{1}/{2}", device.DeviceType, device.DeviceName, device.DeviceId);
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue", 11);
            cell.DetailTextLabel.Text = string.Format("{0}/{1}/{2}", device.ArtistName, device.AlbumTitle, device.SongTitle);
            cell.DetailTextLabel.Font = UIFont.FromName("HelveticaNeue", 11);
            cell.Accessory = UITableViewCellAccessory.None;
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
            cell.ImageChevron.Hidden = false;

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //OnItemClick(_items[indexPath.Row].Key);
        }

        [Export ("tableView:didHighlightRowAtIndexPath:")]
        public void DidHighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_white");
        }

        [Export ("tableView:didUnhighlightRowAtIndexPath:")]
        public void DidUnhighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }

        partial void actionResumePlayback(NSObject sender)
        {

        }

        #region IResumePlaybackView implementation

        public Action<CloudDeviceInfo> OnResumePlayback { get; set; }

        public void ResumePlaybackError(Exception ex)
        {
            InvokeOnMainThread(() => {
                UIAlertView alertView = new UIAlertView("ResumePlayback Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshDevices(IEnumerable<CloudDeviceInfo> devices)
        {
            InvokeOnMainThread(() => {
                _devices = devices.ToList();
                tableView.ReloadData();
            });
        }

        #endregion
    }
}
