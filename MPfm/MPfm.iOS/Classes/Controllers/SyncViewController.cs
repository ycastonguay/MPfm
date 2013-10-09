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
using System.Linq;
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS
{
    public partial class SyncViewController : BaseViewController, ISyncView
    {
        List<SyncDevice> _devices;
        string _cellIdentifier = "SyncDeviceCell";

        public SyncViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "SyncViewController_iPhone" : "SyncViewController_iPad", null)
        {
            _devices = new List<SyncDevice>();
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            this.View.BackgroundColor = GlobalTheme.BackgroundColor;
            //viewRefresh.BackgroundColor = GlobalTheme.MainLightColor;

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Sync Library", "Connect to a device");

            OnStartDiscovery();
            activityIndicator.StartAnimating();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            OnCancelDiscovery();
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableView, int section)
        {
            return _devices.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new UITableViewCell(cellStyle, _cellIdentifier);
            }

            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = _devices[indexPath.Row].Name;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);
            cell.TextLabel.TextColor = UIColor.Black;
            cell.TextLabel.HighlightedTextColor = UIColor.White;
            cell.Accessory = UITableViewCellAccessory.None;
            cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;

            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            OnCancelDiscovery();
            OnConnectDevice(_devices[indexPath.Row]);
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }

        partial void actionConnectDeviceManually(NSObject sender)
        {
            // TODO: Popup window asking for host and port.
            //OnConnectDeviceManually("");
        }

        #region ISyncView implementation

        public Action OnStartDiscovery { get; set; }
        public Action OnCancelDiscovery { get; set; }
        public Action<SyncDevice> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }

        public void SyncError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Sync error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshIPAddress(string address)
        {
            InvokeOnMainThread(() => {
                lblIPAddress.Text = address;
            });
        }

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
            InvokeOnMainThread(() => {
                lblStatus.Text = status;
            });
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
            InvokeOnMainThread(() => {
                _devices = devices.ToList();
                tableView.ReloadData();
            });
        }

        public void RefreshDevicesEnded()
        {
            InvokeOnMainThread(() => {
                activityIndicator.StopAnimating();
//                UIView.Animate(0.25, () => {
//                    viewRefresh.Alpha = 0;
//                });
            });
        }

        public void SyncDevice(SyncDevice device)
        {
            InvokeOnMainThread(() => {

            });
        }

        #endregion
    }
}
