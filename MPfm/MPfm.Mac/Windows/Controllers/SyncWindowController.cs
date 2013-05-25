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

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            //lblIPAddress.StringValue = "My IP address is: " + SyncListenerService.().ToString();
            //lblLibraryUrl.attr
            progressIndicator.StartAnimation(this);
            //progressIndicator.Hidden = true;
            //lblStatus.Hidden = true;
            tableViewDevices.WeakDelegate = this;
            tableViewDevices.WeakDataSource = this;

            OnViewReady.Invoke(this);
        }

        partial void actionSyncLibraryWithDevice(NSObject sender)
        {
//            var libraryService = Bootstrapper.GetContainer().Resolve<ILibraryService>();
//            _syncService = new SyncListenerService(libraryService);
//            _syncService.Start();
        }

        partial void actionRefreshDevices(NSObject sender)
        {
            progressIndicator.Hidden = false;
            btnRefreshDevices.Enabled = false;
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

        public Action OnRefreshDevices { get; set; }

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
                btnRefreshDevices.Enabled = true;
            });
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion
    }
}

