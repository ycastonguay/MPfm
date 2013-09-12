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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Sound.AudioFiles;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public class SyncListenerService : ISyncListenerService
    {
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly ISyncDeviceSpecifications _syncDeviceSpecifications;
        //private HttpListener _httpListener;

        public const string SyncVersionId = "sessions_app_sync_version_1";
        private static string _authenticationCode = GetRandomNumber(10000, 99999).ToString();
        public static string AuthenticationCode { get { return _authenticationCode; } }

        private static int GetRandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public int Port { get; private set; }
        public bool IsRunning { get; private set; }

        public SyncListenerService(IAudioFileCacheService audioFileCacheService, ISyncDeviceSpecifications syncDeviceSpecifications)
        {
            Port = 53551;
            _audioFileCacheService = audioFileCacheService;
            _syncDeviceSpecifications = syncDeviceSpecifications;
            _syncDeviceSpecifications.OnNetworkStateChanged += delegate(NetworkState networkState) {
                //Console.WriteLine("SyncListenerService - NetworkStateChanged isNetworkAvailable: {0} isWifiAvailable: {1} isCellularAvailable: {2}", networkState.IsNetworkAvailable, networkState.IsWifiAvailable, networkState.IsCellularAvailable);
                //if (networkState.IsWifiAvailable && !IsRunning)
                //{
                //    Console.WriteLine("SyncListenerService - NetworkStateChanged - Wifi is now available; restarting HTTP service...");
                //    Start();
                //}
                //else if (!networkState.IsWifiAvailable && IsRunning)
                //{
                //    Console.WriteLine("SyncListenerService - NetworkStateChanged - Wifi is no longer available; stopping HTTP service...");
                //    Stop();
                //}
            };
            Initialize();
        }

        private void Initialize()
        {
            //string url = "http://*:" + Port.ToString("0") + "/";
            //Console.WriteLine("SyncListenerService - Initializing service on url {0} with authenticationCode {1}...", url, AuthenticationCode);
            //_httpListener = new HttpListener();
            //_httpListener.Prefixes.Add(url);
        }

        public void Start()
        {
    //        Console.WriteLine("SyncListenerService - Starting listener on port {0}...", Port);
    //        _httpListener.Start();
    //        Task.Factory.StartNew(() => {
    //            while (true)
    //            {
    //                IsRunning = true;
    //                HttpListenerContext context = _httpListener.GetContext();
    //                Task.Factory.StartNew((ctx) => {
    //                    var httpContext = (HttpListenerContext)ctx;
    //                    Console.WriteLine("SyncListenerService - command: {0} url: {1}", httpContext.Request.HttpMethod, httpContext.Request.Url.ToString());

    //                    try
    //                    {
    //                        string command = httpContext.Request.Url.PathAndQuery;

    //                        // /index           ==> Returns an XML file with the list of audio files in the library
    //                        // /audioFile/id/   ==> Returns the audio file in binary format

    //                        string agent = String.Format("MPfm: Music Player for Musicians version {0} running on {1}", Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetInternalSyncDevice().DeviceType.ToString());
    //                        if(httpContext.Request.HttpMethod.ToUpper() == "GET")
    //                        {
    //                            if(command == "/")
    //                            {
    //                                // TODO: Get query string code for authentication. Maybe redirect to login.html? Make sure you have the name of the file
    //                                //       in the url. i.e. login.html?authenticationCode=AAAA
    //                                // This returns information about this web service

    //                                // Check the cookies (or local storage) for code. If not found, redirect to login.
    //                                WriteRedirect(httpContext, "login.html"); 
    //                            }
    //                            else if(command.ToUpper().StartsWith("/SESSIONSAPP.VERSION"))
    //                            {
    //                                try
    //                                {
    //                                    string xml = XmlSerialization.Serialize(GetInternalSyncDevice());
    //                                    WriteXMLResponse(httpContext, xml);
    //                                }
    //                                catch(Exception ex)
    //                                {
    //                                    WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while parsing the library.</h2><p>{0}</p>", ex, agent), HttpStatusCode.InternalServerError);
    //                                }
    //                            }
    //                            else if(command.ToUpper() == "/API/INDEX/XML")
    //                            {
    //                                // This returns the index of all audio files
    //                                // TODO: Add cache. Add AudioFileCacheUpdated message to flush cache.
    //                                // TODO: Add put audio file. this would enable a web interface to add audio files from a dir. ACTUALLY... /web could have a nice web-based interface for this
    //                                //
    //                                try
    //                                {
    //                                    string xml = XmlSerialization.Serialize(_audioFileCacheService.AudioFiles.OrderBy(x => x.ArtistName).ThenBy(x => x.AlbumTitle).ThenBy(x => x.DiscNumber).ThenBy(x => x.TrackNumber).ToList());
    //                                    WriteXMLResponse(httpContext, xml);
    //                                }
    //                                catch(Exception ex)
    //                                {
    //                                    WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while parsing the library.</h2><p>{0}</p>", ex, agent), HttpStatusCode.InternalServerError);
    //                                }
    //                            }
    //                            else if(command.ToUpper() == "/API/INDEX/JSON")
    //                            {
    //                                // This returns the index of all audio files
    //                                // TODO: Add cache. Add AudioFileCacheUpdated message to flush cache.
    //                                // TODO: Add put audio file. this would enable a web interface to add audio files from a dir. ACTUALLY... /web could have a nice web-based interface for this
    //                                //
    //                                try
    //                                {
    //                                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(_audioFileCacheService.AudioFiles.OrderBy(x => x.ArtistName).ThenBy(x => x.AlbumTitle).ThenBy(x => x.DiscNumber).ThenBy(x => x.TrackNumber).ToList());
    //                                    WriteJSONResponse(httpContext, json);
    //                                }
    //                                catch(Exception ex)
    //                                {
    //                                    WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while parsing the library.</h2><p>{0}</p>", ex, agent), HttpStatusCode.InternalServerError);
    //                                }
    //                            }
    //                            else if(command.ToUpper().StartsWith("/API/AUDIOFILE"))
    //                            {
    //                                try
    //                                {
    //                                    // This returns audio files in binary format
    //                                    string[] split = command.Split(new char[1]{'/'}, StringSplitOptions.RemoveEmptyEntries);
    //                                    var audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == new Guid(split[2]));
    //                                    if(audioFile == null)
    //                                    {
    //                                        WriteHTMLResponse(httpContext, String.Format("<h2>Content could not be found.</h2><p>{0}</p>", agent), HttpStatusCode.NotFound);
    //                                        return;
    //                                    }
    //                                    WriteFileResponse(httpContext, audioFile.FilePath, true);
    //                                }
    //                                catch(Exception ex)
    //                                {
    //                                    WriteHTMLResponse(httpContext, String.Format("<h2>An error occured.</h2><p>{0}</p>", ex, agent), HttpStatusCode.InternalServerError);
    //                                }
    //                            }
    //                            else
    //                            {
    //                                // Try to serve file from WebApp (/WebApp/css/app.css ==> MPfm.Library.WebApp.css.app.css)

    //                                bool isAuthenticated = IsAuthenticated(httpContext);

    //                                // Remove query string
    //                                string[] commandSplit = command.Split(new char[1]{'?'}, StringSplitOptions.RemoveEmptyEntries);
    //                                string resourceName = "MPfm.Library.WebApp" + commandSplit[0].Replace("/", ".");

    //                                if(commandSplit[0].ToUpper().StartsWith("/LOGIN.HTML"))
    //                                {
    //                                    if(isAuthenticated)
    //                                    {
    //                                        WriteRedirect(httpContext, "index.html");
    //                                    }
    //                                    else
    //                                    {
    //                                        // Prevent doing a redirect loop.
    //                                        string redirectFail = "/login.html?loginStatus=failed";
    //                                        if(command != redirectFail)
    //                                        {
    //                                            WriteRedirect(httpContext, redirectFail);
    //                                            return;
    //                                        }
    //                                    }
    //                                }

    //                                // If trying to access index.html without authentication, redirect to login.html
    //                                if(commandSplit[0].ToUpper().StartsWith("/INDEX.HTML") && !isAuthenticated)
    //                                {
    //                                    WriteRedirect(httpContext, "/login.html?loginStatus=failed");
    //                                    return;
    //                                }


    //                                // if index.html and not authenticated; redirect to login.html.


    ////                                if(commandSplit[0].ToUpper() == "/LOGIN.HTML" && commandSplit.Length > 1)
    ////                                {
    ////                                    string queryString = commandSplit[1];
    ////                                    // authenticationCode=value&whatever=true
    ////                                    string[] queryStringItems = queryString.Split('&');
    ////                                    foreach(string queryStringItem in queryStringItems)
    ////                                    {
    ////                                        string[] queryStringEquals = queryStringItem.Split('=');
    ////                                        if(queryStringEquals[0].ToUpper() == "AUTHENTICATIONCODE" && queryStringEquals.Length > 1)
    ////                                        {
    ////                                            string authenticationCode = queryStringEquals[1];
    ////                                        }
    ////                                    }
    ////                                    return;
    ////                                }

    //                                var info = Assembly.GetExecutingAssembly().GetManifestResourceInfo(resourceName);
    //                                if(info != null)
    //                                {
    //                                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
    //                                        WriteFileResponse(httpContext, stream, info.FileName);
    //                                }
    //                                else
    //                                {
    //                                    WriteHTMLResponse(httpContext, String.Format("<h2>Content could not be found.</h2><p>{0}</p>", agent), HttpStatusCode.NotFound);
    //                                }
    //                            }
    //                        }
    //                        else if(httpContext.Request.HttpMethod.ToUpper() == "PUT" || httpContext.Request.HttpMethod.ToUpper() == "OPTIONS" ||
    //                                httpContext.Request.HttpMethod.ToUpper() == "POST")
    //                        {
    //                            if(httpContext.Request.ContentType.ToUpper().Contains("MULTIPART/FORM-DATA"))
    //                            {
    //                                ReadFileMultiPartFormData(httpContext.Request.ContentEncoding, GetBoundary(httpContext.Request.ContentType), httpContext.Request.InputStream);
    //                                WriteHTMLResponse(httpContext, "Success");
    //                            }
    //                            else
    //                            {
    //                                ReadFileBinaryResponse(httpContext);
    //                            }
    //                        }
    //                    }
    //                    catch(Exception ex)
    //                    {
    //                        Console.WriteLine("SyncListenerService - Failed to respond to {0} {1} - Exception: {2}", httpContext.Request.HttpMethod, httpContext.Request.Url.ToString(), ex);
    //                    }
    //                }, context,TaskCreationOptions.LongRunning);
    //            }
    //        },TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            //Console.WriteLine("SyncListenerService - Stopping listener...");
            //_httpListener.Stop();
            IsRunning = false;
        }

        public void SetPort(int port)
        {
            if(IsRunning)
                Stop();

            Port = port;
            Initialize();
        }

//        private bool IsAuthenticated(HttpListenerContext context)
//        {
//            var authenticationCodeCookie = context.Request.Cookies["authenticationCode"];
//            if(authenticationCodeCookie != null)
//            {
//                string authenticationCode = authenticationCodeCookie.Value;
//                if(AuthenticationCode.ToUpper() == authenticationCode.ToUpper())
//                    return true;
//                else
//                    return false;
//            }
//            return false;
//        }

//        private void WriteRedirect(HttpListenerContext context, string url)
//        {
//            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes("redirect");
//            context.Response.RedirectLocation = url;
//            context.Response.StatusCode = 302;
//            context.Response.ContentType = "text/html";
//            context.Response.ContentLength64 = responseBytes.Length;
//            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
//            context.Response.OutputStream.Close();
//        }

//        private void WriteHTMLResponse(HttpListenerContext context, string response)
//        {
//            WriteHTMLResponse(context, response, HttpStatusCode.OK);
//        }

//        private void WriteHTMLResponse(HttpListenerContext context, string response, HttpStatusCode statusCode)
//        {
//            WriteResponse(context, response, statusCode, "text/html");
//        }

//        private void WriteXMLResponse(HttpListenerContext context, string response)
//        {
//            WriteResponse(context, response, HttpStatusCode.OK, "text/html");        
//        }

//        private void WriteJSONResponse(HttpListenerContext context, string response)
//        {
//            WriteResponse(context, response, HttpStatusCode.OK, "text/html");
//        }

//        private void WriteResponse(HttpListenerContext context, string response, HttpStatusCode statusCode, string contentType)
//        {
//            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
//            context.Response.ContentType = contentType;
//            context.Response.StatusCode = (int)statusCode;
//            context.Response.ContentLength64 = responseBytes.Length;
//            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
//            context.Response.OutputStream.Close();
//        }

//        private void WriteFileResponse(HttpListenerContext context, string filePath, bool isAttachment)
//        {
//            var response = context.Response;
//            using (FileStream fs = File.OpenRead(filePath))
//            {
//                response.ContentLength64 = fs.Length;
//                response.SendChunked = false;
//                response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;

//                if(isAttachment)
//                    response.AddHeader("Content-disposition", "attachment; filename=" + Path.GetFileName(filePath));

//                byte[] buffer = new byte[64 * 1024];
//                int read;
//                using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
//                {
//                    while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
//                    {
//                        bw.Write(buffer, 0, read);
//                        bw.Flush();
//                    }
//                    bw.Close();
//                }

//                response.StatusCode = (int)HttpStatusCode.OK;
//                response.StatusDescription = "OK";
//                response.OutputStream.Close();
//            }
//        }

//        private void WriteFileResponse(HttpListenerContext context, Stream stream, string fileName)
//        {
//            Console.WriteLine("SyncListenerService - WriteFileResponse - fileName: {0}", fileName);
//            var response = context.Response;
////            string extension = Path.GetExtension(fileName);
////            if (extension.ToUpper().Contains("CSS"))
////                response.ContentType = "text/css";
////            else if (extension.ToUpper().Contains("HTML"))
////                response.ContentType = "text/html";
////            else if (extension.ToUpper().Contains("JS"))
////                response.ContentType = "text/plain";
//            response.ContentLength64 = stream.Length;
//            response.SendChunked = false;

//            byte[] buffer = new byte[64 * 1024];
//            int read;
//            using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
//            {
//                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
//                {
//                    bw.Write(buffer, 0, read);
//                    bw.Flush();
//                }
//                bw.Close();
//            }

//            response.StatusCode = (int)HttpStatusCode.OK;
//            response.StatusDescription = "OK";
//            response.OutputStream.Close();
//        }

//        private void ReadFileTextResponse(HttpListenerContext context)
//        {
//            var request = context.Request;
//            if (!request.HasEntityBody)
//            {
//                Console.WriteLine("No client data was sent with the request.");
//                return;
//            }

//            if (request.ContentType != null)
//                Console.WriteLine("Client data content type {0}", request.ContentType);

//            Stream stream = request.InputStream;
//            StreamReader reader = new StreamReader(stream, request.ContentEncoding);
//            Console.WriteLine("Client data content length {0}", request.ContentLength64);

//            Console.WriteLine("[Start of client data]");
//            string s = reader.ReadToEnd();
//            Console.WriteLine(s);
//            Console.WriteLine("[End of client data]");

//            stream.Close();
//            reader.Close();
//        }

//        private void ReadFileBinaryResponse(HttpListenerContext context)
//        {
//            string fileName = "/Users/ycastonguay/Desktop/hello.mp3";
//            var request = context.Request;
//            Console.WriteLine("SyncListenerService - ReadFileBinaryResponse fileName: {0}", fileName);
//            if (!request.HasEntityBody)
//            {
//                Console.WriteLine("No client data was sent with the request.");
//                return;
//            }

//            if (request.ContentType != null)
//                Console.WriteLine("Client data content type {0}", request.ContentType);

//            Stream stream = request.InputStream;
//            BinaryReader reader = new BinaryReader(stream);
//            using (FileStream writeStream = File.OpenWrite(fileName))
//            {
//                byte[] buffer = new byte[1024];
//                int bytesRead;

//                while ((bytesRead = stream.Read(buffer, 0, 1024)) > 0)
//                {
//                    writeStream.Write(buffer, 0, bytesRead);
//                }
//            }
//            stream.Close();
//            reader.Close();
//        }

        //private String GetBoundary(String ctype)
        //{
        //    return "--" + ctype.Split(';')[1].Split('=')[1];
        //}

        //private void ReadFileMultiPartFormData(Encoding enc, String boundary, Stream input)
        //{
        //    // http://stackoverflow.com/questions/8466703/httplistener-and-file-upload

        //    string fileName = "/Users/ycastonguay/Desktop/test.txt";
        //    Console.WriteLine("SyncListenerService - ReadFileMultiPartFormData fileName: {0}", fileName);

        //    // TODO: Add multiple files...
        //    Byte[] boundaryBytes = enc.GetBytes(boundary);
        //    Int32 boundaryLen = boundaryBytes.Length;

        //    using (FileStream output = new FileStream(fileName, FileMode.Create, FileAccess.Write))
        //    {
        //        Byte[] buffer = new Byte[1024];
        //        Int32 len = input.Read(buffer, 0, 1024);
        //        Int32 startPos = -1;

        //        // Find start boundary
        //        while (true)
        //        {
        //            if (len == 0)
        //            {
        //                throw new Exception("Start Boundaray Not Found");
        //            }

        //            startPos = IndexOf(buffer, len, boundaryBytes);
        //            if (startPos >= 0)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
        //                len = input.Read(buffer, boundaryLen, 1024 - boundaryLen);
        //            }
        //        }

        //        // Skip four lines (Boundary, Content-Disposition, Content-Type, and a blank)
        //        for (Int32 i = 0; i < 4; i++)
        //        {
        //            while (true)
        //            {
        //                if (len == 0)
        //                {
        //                    throw new Exception("Preamble not Found.");
        //                }

        //                startPos = Array.IndexOf(buffer, enc.GetBytes("\n")[0], startPos);
        //                Console.WriteLine("Multipart - skip 4 lines - startPos: {0}", startPos);
        //                if (startPos >= 0)
        //                {
        //                    startPos++;
        //                    break;
        //                }
        //                else
        //                {
        //                    len = input.Read(buffer, 0, 1024);
        //                }
        //            }
        //        }

        //        Array.Copy(buffer, startPos, buffer, 0, len - startPos);
        //        len = len - startPos;

        //        while (true)
        //        {
        //            Int32 endPos = IndexOf(buffer, len, boundaryBytes);
        //            if (endPos >= 0)
        //            {
        //                if (endPos > 0) output.Write(buffer, 0, endPos);
        //                break;
        //            }
        //            else if (len <= boundaryLen)
        //            {
        //                throw new Exception("End Boundaray Not Found");
        //            }
        //            else
        //            {
        //                output.Write(buffer, 0, len - boundaryLen);
        //                Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
        //                len = input.Read(buffer, boundaryLen, 1024 - boundaryLen) + boundaryLen;
        //            }
        //        }
        //    }
        //    input.Close();
        //}

        //private Int32 IndexOf(Byte[] buffer, Int32 len, Byte[] boundaryBytes)
        //{
        //    for (Int32 i = 0; i <= len - boundaryBytes.Length; i++)
        //    {
        //        Boolean match = true;
        //        for (Int32 j = 0; j < boundaryBytes.Length && match; j++)
        //        {
        //            match = buffer[i + j] == boundaryBytes[j];
        //        }

        //        if (match)
        //        {
        //            return i;
        //        }
        //    }

        //    return -1;
        //}

        // Alternative that doesnt work under MonoTouch though!
//        public static IPAddress GetIPAddressForDesktop()
//        {
//            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
//                return null;
//
//            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
//            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
//        }

        //public static string GetLocalIPAddress()
        //{
        //    string address = "Not Connected";
        //    try
        //    {
        //        //Console.WriteLine("GetIPAddress - Detecting IP address...");
        //        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        //        {
        //            //Console.WriteLine("GetIPAddress - NetworkInterface: {0} {1}", ni.Name, ni.NetworkInterfaceType.ToString());
        //            if(ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
        //               ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
        //            {
        //                IPAddress ip = null;
        //                foreach (var a in ni.GetIPProperties().UnicastAddresses)
        //                {
        //                    //Console.WriteLine("GetIPAddress - Address: {0} {1}", a.Address, a.Address.AddressFamily.ToString());
        //                    if(a.Address.AddressFamily == AddressFamily.InterNetwork)
        //                    {
        //                        //Console.WriteLine("GetIPAddress - Address **FOUND**: {0} {1}", a.Address, a.Address.AddressFamily.ToString());
        //                        ip = a.Address;
        //                        break;
        //                    }
        //                }

        //                if(ip != null)
        //                {
        //                    address = ip.ToString();
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("GetIPAddress - Exception: {0}", ex);
        //    }
        //    return address;
        //}

        public SyncDevice GetInternalSyncDevice()
        {
            var device = new SyncDevice();
            device.SyncVersionId = SyncVersionId;
            device.Name = _syncDeviceSpecifications.GetDeviceName();
            device.DeviceType = _syncDeviceSpecifications.GetDeviceType();
            return device;
        }
    }
}
