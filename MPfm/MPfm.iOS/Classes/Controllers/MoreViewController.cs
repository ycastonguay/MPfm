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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;

namespace MPfm.iOS
{
    public partial class MoreViewController : BaseViewController, IMobileOptionsMenuView
    {
        SyncTcpClient _syncTcpClient;
        string _cellIdentifier = "MoreCell";
        List<KeyValuePair<MobileOptionsMenuType, string>> _items;

        public MoreViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "MoreViewController_iPhone" : "MoreViewController_iPad", null)
        {
        }
		
        public override void ViewDidLoad()
        {
            _items = new List<KeyValuePair<MobileOptionsMenuType, string>>();
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            base.ViewDidLoad();
        }

        public override void ViewDidDisappear(bool animated)
        {
            tableView.DeselectRow(tableView.IndexPathForSelectedRow, false);
            base.ViewDidDisappear(animated);
        }        
        
        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _items.Count;
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // Request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Default;
                cell = new UITableViewCell(cellStyle, _cellIdentifier);
            }
            
            cell.TextLabel.Text = _items[indexPath.Row].Value;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Medium", 16);
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

            UIView viewBackgroundSelected = new UIView();
            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            cell.SelectedBackgroundView = viewBackgroundSelected;

            
//            // Check this is the version cell (remove all user interaction)
//            if (viewModel.Items[indexPath.Row].ItemType == MoreItemType.Version)
//            {
//                cell.Accessory = UITableViewCellAccessory.None;
//                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
//                cell.TextLabel.TextColor = UIColor.Gray;
//                cell.TextLabel.TextAlignment = UITextAlignment.Center;
//                cell.TextLabel.Font = UIFont.FromName("Asap", 16);
//            }
            
            return cell;
        }
        
        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if(indexPath.Row == 3)
            {
                //            UIAlertView alertView = new UIAlertView("Your IP Address", GetIPAddress(), null, "OK", null);
                //            alertView.Show();

                //            listener = new HttpListener();
                //            listener.Prefixes.Add("http://*:8080/");
                //            listener.Start();
                //            listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);

                var remoteHostStatus = ReachabilityHelper.RemoteHostStatus ();
                var internetStatus = ReachabilityHelper.InternetConnectionStatus ();
                var localWifiStatus = ReachabilityHelper.LocalWifiConnectionStatus ();
                Console.WriteLine("remoteHostStatus: {0} - internetStatus: {1} - localWifiStatus: {2}", remoteHostStatus, internetStatus, localWifiStatus);

                var webClient = new WebClient();
                webClient.DownloadFile("http://192.168.1.101:8080/", Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "test.mp3"));
                return;
            }

            OnItemClick(_items[indexPath.Row].Key);
        }

        HttpListener listener;
        private void HandleRequest(IAsyncResult result) {
            //if (!listenerRunning) return;

            //Get the listener context
            HttpListenerContext context = listener.EndGetContext(result);
            //Start listening for the next request
            listener.BeginGetContext(new AsyncCallback(HandleRequest), listener);

            //Update status on the UI thread
//            InvokeOnMainThread( delegate {
//                //lblIPAddress.StringValue = context.Request.UserHostAddress;                
//            });
            Console.WriteLine("HttpListener - HandleRequest {0}", context.Request.UserHostAddress);

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

        public string GetIPAddress()
        {
            string address = "Not Connected";
            try
            {
                // For simulator: address = IPAddress.FileStyleUriParser("127.0.0.1"); 
                string str = Dns.GetHostName() + ".local";
                IPHostEntry hostEntry = Dns.GetHostEntry(str);
                address = (
                    from addr in hostEntry.AddressList
                    where addr.AddressFamily == AddressFamily.InterNetwork
                    select addr.ToString()
                    ).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetIPAddress Exception: {0}", ex);
            }
            return address;
        }

        #region IMobileOptionsMenuView implementation

        public Action<MobileOptionsMenuType> OnItemClick { get; set; }
        
        public void RefreshMenu(List<KeyValuePair<MobileOptionsMenuType, string>> options)
        {
            InvokeOnMainThread(() => {
                _items = options;
                tableView.ReloadData();
            });
        }
        
        #endregion
    }

    public class SyncTcpClient
    {
        public int Port { get; private set; }

        public SyncTcpClient(int port)
        {
            Port = port;
        }

        private void Callback(IAsyncResult result)
        {
            Console.WriteLine("CALLBACK");
        }

        public void Connect(string server, string message)
        {
            try 
            {
                // Create a TcpClient. 
                // Note, for this client to work you need to have a TcpServer  
                // connected to the same address as specified by the server, port 
                // combination.
                //TcpClient client = new TcpClient(server, Port);
                TcpClient client = new TcpClient();
                client.ReceiveTimeout = 5;
                client.SendTimeout = 5;
                client.BeginConnect(server, Port, Callback, null);

//                // Translate the passed message into ASCII and store it as a Byte array.
//                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);         
//
//                // Get a client stream for reading and writing. 
//                //  Stream stream = client.GetStream();
//                NetworkStream stream = client.GetStream();
//
//                // Send the message to the connected TcpServer. 
//                stream.Write(data, 0, data.Length);
//
//                Console.WriteLine("Sent: {0}", message);         
//
//                // Receive the TcpServer.response. 
//                // Buffer to store the response bytes.
//                data = new Byte[256];
//
//                // String to store the response ASCII representation.
//                String responseData = String.Empty;
//
//                // Read the first batch of the TcpServer response bytes.
//                Int32 bytes = stream.Read(data, 0, data.Length);
//                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
//                Console.WriteLine("Received: {0}", responseData);         
//
//                // Close everything.
//                stream.Close();         
//                client.Close();         
            } 
            catch (ArgumentNullException e) 
            {
                Console.WriteLine("SyncTcpClient - ArgumentNullException: {0}", e);
            } 
            catch (SocketException e) 
            {
                Console.WriteLine("SyncTcpClient - SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}

