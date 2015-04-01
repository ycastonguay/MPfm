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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sessions.Library.Objects;
using Sessions.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.Core;

namespace Sessions.iOS
{
    public partial class SyncViewController : BaseViewController, ISyncView
    {
        List<SyncDevice> _devices;
        string _cellIdentifier = "SyncDeviceCell";

        public SyncViewController()
            : base (UserInterfaceIdiomIsPhone ? "SyncViewController_iPhone" : "SyncViewController_iPad", null)
        {
            _devices = new List<SyncDevice>();
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            this.View.BackgroundColor = GlobalTheme.BackgroundColor;

            btnConnectDeviceManually.SetImage(UIImage.FromBundle("Images/Buttons/connect"));

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                NavigationController.InteractivePopGestureRecognizer.WeakDelegate = this;
                NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            }

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindSyncView(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SessionsNavigationController navCtrl = (SessionsNavigationController)this.NavigationController;
            navCtrl.SetTitle("Sync (Nearby Devices)");

            OnStartDiscovery();
            activityIndicator.StartAnimating();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            OnCancelDiscovery();
        }

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			Tracing.Log("!!!!>>> SyncViewCtrl - ViewDidLayoutSubviews - button.Frame: {0} button.Bounds: {1}", btnConnectDeviceManually.Frame, btnConnectDeviceManually.Bounds);
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
			OnOpenConnectDevice();
        }

        #region ISyncView implementation

        public Action OnStartDiscovery { get; set; }
        public Action OnCancelDiscovery { get; set; }
        public Action<SyncDevice> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }
		public Action OnOpenConnectDevice { get; set; }

		public Action<string> OnAddDeviceFromUrl { get; set; }
		public Action<SyncDevice> OnRemoveDevice { get; set; }
		public Action<SyncDevice> OnSyncLibrary { get; set; }
		public Action<SyncDevice> OnResumePlayback { get; set; }
		public Action OnOpenAddDeviceDialog { get; set; }

		public Action<SyncDevice> OnRemotePlayPause { get; set; }
		public Action<SyncDevice> OnRemotePrevious { get; set; }
		public Action<SyncDevice> OnRemoteNext { get; set; }
		public Action<SyncDevice> OnRemoteRepeat { get; set; }
		public Action<SyncDevice> OnRemoteShuffle { get; set; }

        public void SyncError(Exception ex)
        {
            ShowErrorDialog(ex);
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

		public void RefreshStatus(string status)
		{
		}

		public void NotifyAddedDevice(SyncDevice device)
		{
		}

		public void NotifyRemovedDevice(SyncDevice device)
		{
		}

		public void NotifyUpdatedDevice(SyncDevice device)
		{
		}

        public void SyncDevice(SyncDevice device)
        {
            InvokeOnMainThread(() => {

            });
        }

		public void NotifyUpdatedDevices(IEnumerable<SyncDevice> devices)
		{
		}

        #endregion

    }
}
