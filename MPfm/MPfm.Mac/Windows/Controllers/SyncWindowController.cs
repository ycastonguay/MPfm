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
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.MVP.Views;
using MPfm.MVP.Models;

namespace MPfm.Mac
{
    public partial class SyncWindowController : BaseWindowController, ISyncView
    {
        SyncListenerService _syncService;

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

//            lblIPAddress.StringValue = "My IP address is: " + SyncListenerService.().ToString();
            progressIndicator.Hidden = true;
            lblStatus.Hidden = true;

            OnViewReady.Invoke(this);
        }

        partial void actionSyncLibraryWithDevice(NSObject sender)
        {
            var libraryService = Bootstrapper.GetContainer().Resolve<ILibraryService>();
            _syncService = new SyncListenerService(libraryService);
            _syncService.Start();
        }

        partial void actionRefreshDevices(NSObject sender)
        {
            lblStatus.StringValue = "Refreshing device list...";
            progressIndicator.StartAnimation(this);
            progressIndicator.Hidden = false;
            lblStatus.Hidden = false;

            OnRefreshDevices();
        }

        #region ISyncView implementation

        public Action OnRefreshDevices { get; set; }

        public void RefreshDevices(IEnumerable<SyncDeviceEntity> devices)
        {
            InvokeOnMainThread(() => {
                Console.WriteLine("SyncWindowCtrl - RefreshDevices");
            });
        }

        public void SyncDevice(SyncDeviceEntity device)
        {
        }

        #endregion
    }
}

