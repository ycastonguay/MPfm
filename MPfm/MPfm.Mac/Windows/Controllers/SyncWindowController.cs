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
using System.Linq;
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.Mac.Classes.Objects;
using MPfm.Mac.Classes.Helpers;

namespace MPfm.Mac
{
    public partial class SyncWindowController : BaseWindowController, ISyncView
    {
        bool _isDiscovering;
        List<SyncDevice> _items = new List<SyncDevice>();

        // Called when created from unmanaged code
        public SyncWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public SyncWindowController(Action<IBaseView> onViewReady)
            : base ("SyncWindow", onViewReady)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
            this.Window.Center();
            this.Window.MakeKeyAndOrderFront(this);
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            tableViewDevices.WeakDelegate = this;
            tableViewDevices.WeakDataSource = this;
            LoadFontsAndImages();
            OnViewReady.Invoke(this);
        }

        private void LoadFontsAndImages()
        {
            lblTitle.Font = NSFont.FromFontName("TitilliumText25L-800wt", 18);
            lblLibraryUrl.Font = NSFont.FromFontName("Junction", 12);
            btnRefreshDevices.StringValue = "Cancel refresh";

            btnConnect.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_connect");
            btnConnectManual.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_connect");
            btnRefreshDevices.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_refresh");
        }

        private void RefreshDeviceListButton()
        {
            if (_isDiscovering)
            {
                btnRefreshDevices.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_cancel");
                btnRefreshDevices.Title = "Cancel refresh";
            }
            else
            {
                btnRefreshDevices.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_refresh");
                btnRefreshDevices.Title = "Refresh devices";
            }
        }

        partial void actionConnect(NSObject sender)
        {
            if(tableViewDevices.SelectedRow == -1)
                return;

            OnConnectDevice(_items[tableViewDevices.SelectedRow]);
        }

        partial void actionConnectManual(NSObject sender)
        {
            // Show dialog box
            //OnConnectDeviceManually();
        }

        partial void actionRefreshDevices(NSObject sender)
        {
            if (_isDiscovering)
                OnCancelDiscovery();
            else
                OnStartDiscovery();
        }

        [Export ("numberOfRowsInTableView:")]
        public int GetRowCount(NSTableView tableView)
        {
            return _items.Count;
        }

        [Export ("tableView:dataCellForTableColumn:row:")]
        public NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            return new NSString();
        }

        [Export ("tableView:viewForTableColumn:row:")]
        public NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            NSTableCellView view;
            if(tableColumn.Identifier.ToString() == "columnDeviceName")
            {
                view = (NSTableCellView)tableView.MakeView("cellDeviceName", this);
                view.TextField.StringValue = _items[row].Name;
            }
            else
            {
                view = (NSTableCellView)tableView.MakeView("cellDeviceDescription", this);
                view.TextField.StringValue = _items[row].Url;
            }

            view.TextField.Font = NSFont.FromFontName("Junction", 11);
            if (view.ImageView != null)
            {
                string iconName = string.Empty;
                switch (_items[row].DeviceType)
                {
                    case SyncDeviceType.Linux:
                        iconName = "icon_linux";
                        break;
                    case SyncDeviceType.OSX:
                        iconName = "icon_osx";
                        break;
                    case SyncDeviceType.Windows:
                        iconName = "icon_windows";
                        break;
                    case SyncDeviceType.iOS:
                        iconName = "icon_phone";
                        break;
                    case SyncDeviceType.Android:
                        iconName = "icon_android";
                        break;
                }
                view.ImageView.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == iconName);
            }
            return view;
        }

        #region ISyncView implementation

        public Action<SyncDevice> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }
        public Action OnStartDiscovery { get; set; }
        public Action OnCancelDiscovery { get; set; }
        public Action OnOpenConnectDevice { get; set; }

        public void SyncError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in Sync: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshIPAddress(string address)
        {
            InvokeOnMainThread(() => {
                lblLibraryUrl.StringValue = address;
            });
        }

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
            InvokeOnMainThread(() => {
                progressIndicator.DoubleValue = (double)percentageDone;
                if (!_isDiscovering)
                {
                    _isDiscovering = true;
                    progressIndicator.Hidden = false;
                    RefreshDeviceListButton();
                }
            });
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
            InvokeOnMainThread(() => {
                Console.WriteLine("SyncWindowCtrl - RefreshDevices");
                _items = devices.ToList();
                tableViewDevices.ReloadData();
            });
        }

        public void RefreshDevicesEnded()
        {
            InvokeOnMainThread(() => {
                Console.WriteLine("SyncWindowCtrl - RefreshDevicesEnded");
                progressIndicator.Hidden = true;
                _isDiscovering = false;
                RefreshDeviceListButton();
            });
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion
    }
}

