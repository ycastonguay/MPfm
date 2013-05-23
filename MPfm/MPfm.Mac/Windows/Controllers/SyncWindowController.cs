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

namespace MPfm.Mac
{
    public partial class SyncWindowController : MonoMac.AppKit.NSWindowController
    {
        SyncListenerService _syncService;

        #region Constructors
        
        // Called when created from unmanaged code
        public SyncWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public SyncWindowController(NSCoder coder) : base (coder)
        {
            Initialize();
        }
        // Call to load from the XIB/NIB file
        public SyncWindowController() : base ("SyncWindow")
        {
            Initialize();
        }
        // Shared initialization code
        void Initialize()
        {

        }
        #endregion
        
        //strongly typed window accessor
        public new SyncWindow Window
        {
            get
            {
                return (SyncWindow)base.Window;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            lblIPAddress.StringValue = "My IP address is: " + SyncListenerService.LocalIPAddress().ToString();
            progressIndicator.Hidden = true;
            lblStatus.Hidden = true;
        }

        partial void actionSyncLibraryWithDevice(NSObject sender)
        {
            var libraryService = Bootstrapper.GetContainer().Resolve<ILibraryService>();
            _syncService = new SyncListenerService(8080, libraryService);
            _syncService.Start();
        }

        partial void actionRefreshDevices(NSObject sender)
        {
            lblStatus.StringValue = "Refreshing device list...";
            progressIndicator.StartAnimation(this);
            progressIndicator.Hidden = false;
            lblStatus.Hidden = false;

            Task.Factory.StartNew(() => {
                var ips = IPAddressRangeFinder.GetIPRange(IPAddress.Parse("192.168.1.100"), IPAddress.Parse("192.168.1.255"));
                var validIps = new List<string>();
                foreach(var ip in ips)
                {
                    bool successful = false;
                    //lblStatus.StringValue = String.Format("Querying {0}...", ip);
                    Console.WriteLine("Querying {0}...", ip);
                    try
                    {
                        var ping = new Ping();
                        

//                        var client = new WebClientTimeout(3000);
//                        string data = client.DownloadString("http://" + ip + ":8080/hello");
//                        if(!String.IsNullOrEmpty(data))
//                        {
//                            successful = true;
//                            Console.WriteLine("Successfully connected to {0}", ip);
//                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Failed to connect to {0}: {1}", ip, ex);
                    }

                    if(successful)
                        validIps.Add(ip);
                }
            });
        }
    }
}

