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
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Objects;
using System.Linq;
using MPfm.iOS.Classes.Controls;

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
            viewRefresh.BackgroundColor = GlobalTheme.MainLightColor;
            btnAddDevice.BackgroundColor = GlobalTheme.SecondaryColor;
            btnAddDevice.Layer.CornerRadius = 8;
            btnRefreshDevices.BackgroundColor = GlobalTheme.SecondaryColor;
            btnRefreshDevices.Layer.CornerRadius = 8;

            activityIndicator.StartAnimating();

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Sync Library", "Connect to a device");
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableView, int section)
        {
            return _devices.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // Request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new UITableViewCell(cellStyle, _cellIdentifier);
            }

            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = _devices[indexPath.Row].Name;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 16);
            cell.TextLabel.TextColor = UIColor.Black;
            cell.Accessory = UITableViewCellAccessory.None;
            cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;

//            if (_presets[indexPath.Row].EQPresetId == _selectedPresetId)
//                cell.Accessory = UITableViewCellAccessory.Checkmark;

            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
//            SetCheckmarkCell(indexPath);
//            OnLoadPreset(_presets[indexPath.Row].EQPresetId);
            tableView.DeselectRow(indexPath, true);
        }

        partial void actionAddDevice(NSObject sender)
        {

        }

        partial void actionRefreshDevices(NSObject sender)
        {
            btnRefreshDevices.SetTitle("Cancel refresh", UIControlState.Normal);
            UIView.Animate(0.25, () => {
                viewRefresh.Alpha = 1;
            });
            OnRefreshDevices();
        }

        #region ISyncView implementation

        public Action OnRefreshDevices { get; set; }

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
                btnRefreshDevices.SetTitle("Refresh devices", UIControlState.Normal);
                UIView.Animate(0.25, () => {
                    viewRefresh.Alpha = 0;
                });
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