using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using MPfm.Library.Sync;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace MPfm.Mac
{
    public partial class SyncWindowController : MonoMac.AppKit.NSWindowController
    {
        SyncTcpListener _syncTcpListener;

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

        HttpListener listener;
        partial void actionSyncLibraryWithDevice(NSObject sender)
        {
//            Task.Factory.StartNew(() => {
//                _syncTcpListener = new SyncTcpListener(10000);
//                _syncTcpListener.Start();
//            });

//            listener = new HttpListener();
//            listener.Prefixes.Add("http://*:8080/");
//            listener.Start();
//            listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            listener.Start();
            Task.Factory.StartNew(() =>
                                  {
                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    Task.Factory.StartNew((ctx) =>
                    {
                        WriteFile((HttpListenerContext)ctx);
                    }, context,TaskCreationOptions.LongRunning);
                }
            },TaskCreationOptions.LongRunning);
        }

        void WriteFile(HttpListenerContext ctx)
        {
            var response = ctx.Response;
            using (FileStream fs = File.OpenRead(@"/Users/ycastonguay/Documents/hello.mp3"))
            {

                //response is HttpListenerContext.Response...
                response.ContentLength64 = fs.Length;
                response.SendChunked = false;
                response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                response.AddHeader("Content-disposition", "attachment; filename=hello.mp3");

                byte[] buffer = new byte[64 * 1024];
                int read;
                using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
                {
                    while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, read);
                        bw.Flush(); //seems to have no effect
                    }

                    bw.Close();
                }

                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "OK";
                response.OutputStream.Close();
            }
        }

        private void HandleRequest(IAsyncResult result) {
            //if (!listenerRunning) return;

            //Get the listener context
            HttpListenerContext context = listener.EndGetContext(result);
            //Start listening for the next request
            listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);

            //Update status on the UI thread
            InvokeOnMainThread( delegate {
                lblIPAddress.StringValue = context.Request.UserHostAddress;                
            });

            //Here you can create any response that you want. You can serve text or a file or whatever else you need.
            string response = "<html><head><title>Sample Response</title></head><body>Response from mtouchwebserver.</body></html>";
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(response);

            //Set some response information
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentLength64 = responseBytes.Length;

            //Write the response.
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();
        }

        partial void actionRefreshDevices(NSObject sender)
        {
            lblIPAddress.StringValue = "My IP address is: " + LocalIPAddress().ToString();
            var client = new SyncTcpClient(4000);
            client.Connect("127.0.0.1", "Local is local");
        }

        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

    }
}

