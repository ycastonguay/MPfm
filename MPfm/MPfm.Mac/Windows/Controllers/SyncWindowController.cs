using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Core.Network;
using MPfm.Library.Objects;
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.Mac.Classes.Objects;
using MPfm.Mac.Classes.Helpers;

namespace MPfm.Mac
{
    public partial class SyncWindowController : BaseWindowController, ISyncView
    {
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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            progressIndicator.StartAnimation(this);

            lblTitle.Font = NSFont.FromFontName("TitilliumText25L-800wt", 18);
            //lblLibraryUrl.Font = NSFont.FromFontName("Junction", 12);

            btnAddDevice.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_plus");
            btnRefreshDevices.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_refresh");
            btnSyncLibraryWithDevice.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_cabinet");
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            tableViewDevices.WeakDelegate = this;
            tableViewDevices.WeakDataSource = this;

            OnViewReady.Invoke(this);
        }

        partial void actionSyncLibraryWithDevice(NSObject sender)
        {
        }

        partial void actionRefreshDevices(NSObject sender)
        {
            progressIndicator.Hidden = false;
            btnRefreshDevices.Enabled = false;
            btnRefreshDevices.StringValue = "Cancel refresh";
            OnRefreshDevices();
        }

        [Export ("numberOfRowsInTableView:")]
        public int GetRowCount(NSTableView tableView)
        {
            return _items.Count;
        }

        [Export ("tableView:heightOfRow:")]
        public float GetRowHeight(NSTableView tableView, int row)
        {
            return 20;
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
            //view.TextField.Font = NSFont.FromFontName("Junction", 11);

            if (view.ImageView != null)
            {
                string iconName = string.Empty;
                switch (_items[row].DeviceType)
                {
                    case SyncDeviceType.iOS:
                        iconName = "16_icomoon_apple";
                        break;
                    case SyncDeviceType.Android:
                        iconName = "16_icomoon_android";
                        break;
                    default:
                        iconName = "16_icomoon_laptop";
                        break;
                }
                view.ImageView.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == iconName);
            }
            return view;
        }

        [Export ("tableViewSelectionDidChange:")]
        public void SelectionDidChange(NSNotification notification)
        {         
            btnSyncLibraryWithDevice.Enabled = (tableViewDevices.SelectedRow == -1) ? false : true;
        }

        #region ISyncView implementation

        public Action<string> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }
        public Action OnRefreshDevices { get; set; }

        public void SyncError(Exception ex)
        {
            InvokeOnMainThread(() => {
                CocoaHelper.ShowCriticalAlert(ex.Message + "\n" + ex.StackTrace);
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
                btnRefreshDevices.StringValue = "Refresh devices";
                progressIndicator.Hidden = true;
                btnRefreshDevices.Enabled = true;
            });
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion
    }
}

