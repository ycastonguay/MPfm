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
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public class SyncListenerService : ISyncListenerService
    {
        private readonly ILibraryService _libraryService;
        private HttpListener _httpListener;

        public int Port { get; private set; }

        public SyncListenerService(int port, ILibraryService libraryService)
        {
            _libraryService = libraryService;
            Port = port;
            Initialize();
        }

        private void Initialize()
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://*:" + Port.ToString("0") + "/");
        }

        public void Start()
        {
            _httpListener.Start();
            Task.Factory.StartNew(() => {
                while (true)
                {
                    HttpListenerContext context = _httpListener.GetContext();
                    Task.Factory.StartNew((ctx) => {
                        var httpContext = (HttpListenerContext)ctx;
                        Console.WriteLine("HttpListener - WriteFile - url {0} {1}", httpContext.Request.Url.ToString(), httpContext.Request.Url.PathAndQuery);

                        string command = httpContext.Request.Url.PathAndQuery;

                        // /index           ==> Returns an XML file with the list of audio files in the library
                        // /audioFile/id/   ==> Returns the audio file in binary format

                        if(command.ToUpper().StartsWith("/INDEX"))
                        {

                        }
                        else if(command.ToUpper().StartsWith("/AUDIOFILE"))
                        {
                        }

                        WriteFile(httpContext);
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
            HttpListenerContext context = _httpListener.EndGetContext(result);
            //Start listening for the next request
            _httpListener.BeginGetContext(new AsyncCallback(HandleRequest), _httpListener);

//            //Update status on the UI thread
//            InvokeOnMainThread( delegate {
//                lblIPAddress.StringValue = context.Request.UserHostAddress;                
//            });

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

        // Good for desktop
        public static IPAddress LocalIPAddress()
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

        // Good for mobile
        public static string GetIPAddress()
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


    }
}
