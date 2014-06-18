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
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public abstract class SyncListenerServiceBase : ISyncListenerService
    {
        private HttpListener _httpListener;

        public int Port { get; protected set; }
        public bool IsRunning { get; protected set; }

        public abstract string ServerAgent { get; }
        public abstract void ProcessCommand(HttpListenerContext httpContext, string command);

        protected SyncListenerServiceBase()
        {
            Port = 53551;
            Initialize();
        }

        protected void Initialize()
        {
            string url = "http://*:" + Port.ToString("0") + "/";
            Console.WriteLine("SyncListenerServiceBase - Initializing service on url {0}...", url);
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(url);
        }

        public void Start()
        {
            Console.WriteLine("SyncListenerServiceBase - Starting listener on port {0}...", Port);
            _httpListener.Start();
            Task.Factory.StartNew(() => {
                while (true)
                {
                    IsRunning = true;
                    HttpListenerContext context = _httpListener.GetContext();
                    Task.Factory.StartNew((ctx) => {
                        var httpContext = (HttpListenerContext)ctx;
                        try
                        {
                            Console.WriteLine("SyncListenerServiceBase - command: {0} url: {1}", httpContext.Request.HttpMethod, httpContext.Request.Url.ToString());
                            ProcessCommand(httpContext);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("SyncListenerServiceBase - Failed to respond to {0} {1} - Exception: {2}", httpContext.Request.HttpMethod, httpContext.Request.Url.ToString(), ex);
                        }

                    }, context, TaskCreationOptions.LongRunning);
                }
            }, TaskCreationOptions.LongRunning);
        }

        protected void ProcessCommand(HttpListenerContext httpContext)
        {
            // List of API commands:
            // /api/index           ==> Returns an XML file with the list of audio files in the library
            // /api/audioFile/id/   ==> Returns the audio file in binary format
            // /api/player          ==> Returns the current player status and metadata
            // /api/playlist        ==> Returns the current player playlist
            // /api/remote/*        ==> Remote commands for controlling the player

            string command = httpContext.Request.Url.PathAndQuery;
            if(httpContext.Request.HttpMethod.ToUpper() == "GET")
            {
                ProcessCommand(httpContext, command);
            }
            else if(httpContext.Request.HttpMethod.ToUpper() == "PUT" || httpContext.Request.HttpMethod.ToUpper() == "OPTIONS" ||
                httpContext.Request.HttpMethod.ToUpper() == "POST")
            {
                if(httpContext.Request.ContentType.ToUpper().Contains("MULTIPART/FORM-DATA"))
                {
                    ReadFileMultiPartFormData(httpContext.Request.ContentEncoding, GetBoundary(httpContext.Request.ContentType), httpContext.Request.InputStream);
                    WriteHTMLResponse(httpContext, "Success");
                }
                else
                {
                    ReadFileBinaryResponse(httpContext);
                }
            }
        }

        public void Stop()
        {
            Console.WriteLine("SyncListenerServiceBase - Stopping listener...");
            _httpListener.Stop();
            IsRunning = false;
        }

        public void SetPort(int port)
        {
            if(IsRunning)
                Stop();

            Port = port;
            Initialize();
        }

        protected void WriteRedirect(HttpListenerContext context, string url)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes("redirect");
            context.Response.RedirectLocation = url;
            context.Response.StatusCode = 302;
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = responseBytes.Length;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();
        }

        protected void WriteHTMLResponse(HttpListenerContext context, string response)
        {
            WriteHTMLResponse(context, response, HttpStatusCode.OK);
        }

        protected void WriteHTMLResponse(HttpListenerContext context, string response, HttpStatusCode statusCode)
        {
            string responseWithAgent = string.Format("{0}<small>{1}</small>", response, ServerAgent);
            WriteResponse(context, responseWithAgent, statusCode, "text/html");
        }

        protected void WriteXMLResponse(HttpListenerContext context, string response)
        {
            WriteResponse(context, response, HttpStatusCode.OK, "text/html");        
        }

        protected void WriteJSONResponse(HttpListenerContext context, string response)
        {
            WriteResponse(context, response, HttpStatusCode.OK, "text/html");
        }

        protected void WriteResponse(HttpListenerContext context, string response, HttpStatusCode statusCode, string contentType)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            context.Response.ContentType = contentType;
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentLength64 = responseBytes.Length;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();
        }

        protected void WriteFileResponse(HttpListenerContext context, string filePath, bool isAttachment)
        {
            var response = context.Response;
            using (FileStream fs = File.OpenRead(filePath))
            {
                response.ContentLength64 = fs.Length;
                response.SendChunked = false;
                response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;

                if(isAttachment)
                    response.AddHeader("Content-disposition", "attachment; filename=" + Path.GetFileName(filePath));

                byte[] buffer = new byte[64 * 1024];
                int read;
                using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
                {
                    while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, read);
                        bw.Flush();
                    }
                    bw.Close();
                }

                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "OK";
                response.OutputStream.Close();
            }
        }

        protected void WriteBinaryResponse(HttpListenerContext context, byte[] data)
        {
            var stream = new MemoryStream(data);
            WriteStreamResponse(context, stream);
        }

        protected void WriteStreamResponse(HttpListenerContext context, Stream stream)
        {
            //Console.WriteLine("SyncListenerService - WriteFileResponse - fileName: {0}", fileName);
            var response = context.Response;
//            string extension = Path.GetExtension(fileName);
//            if (extension.ToUpper().Contains("CSS"))
//                response.ContentType = "text/css";
//            else if (extension.ToUpper().Contains("HTML"))
//                response.ContentType = "text/html";
//            else if (extension.ToUpper().Contains("JS"))
//                response.ContentType = "text/plain";
            response.ContentLength64 = stream.Length;
            response.SendChunked = false;

            byte[] buffer = new byte[64 * 1024];
            int read;
            using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
            {
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bw.Write(buffer, 0, read);
                    bw.Flush();
                }
                bw.Close();
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            response.StatusDescription = "OK";
            response.OutputStream.Close();
        }

        protected void ReadFileTextResponse(HttpListenerContext context)
        {
            var request = context.Request;
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return;
            }

            if (request.ContentType != null)
                Console.WriteLine("Client data content type {0}", request.ContentType);

            Stream stream = request.InputStream;
            StreamReader reader = new StreamReader(stream, request.ContentEncoding);
            Console.WriteLine("Client data content length {0}", request.ContentLength64);

            Console.WriteLine("[Start of client data]");
            string s = reader.ReadToEnd();
            Console.WriteLine(s);
            Console.WriteLine("[End of client data]");

            stream.Close();
            reader.Close();
        }

        protected void ReadFileBinaryResponse(HttpListenerContext context)
        {
            string fileName = "/Users/ycastonguay/Desktop/hello.mp3";
            var request = context.Request;
            Console.WriteLine("SyncListenerService - ReadFileBinaryResponse fileName: {0}", fileName);
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return;
            }

            if (request.ContentType != null)
                Console.WriteLine("Client data content type {0}", request.ContentType);

            Stream stream = request.InputStream;
            BinaryReader reader = new BinaryReader(stream);
            using (FileStream writeStream = File.OpenWrite(fileName))
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, 1024)) > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                }
            }
            stream.Close();
            reader.Close();
        }

        protected String GetBoundary(String ctype)
        {
            return "--" + ctype.Split(';')[1].Split('=')[1];
        }

        protected void ReadFileMultiPartFormData(Encoding enc, String boundary, Stream input)
        {
            // http://stackoverflow.com/questions/8466703/httplistener-and-file-upload

            string fileName = "/Users/ycastonguay/Desktop/test.txt";
            Console.WriteLine("SyncListenerService - ReadFileMultiPartFormData fileName: {0}", fileName);

            // TODO: Add multiple files...
            Byte[] boundaryBytes = enc.GetBytes(boundary);
            Int32 boundaryLen = boundaryBytes.Length;

            using (FileStream output = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                Byte[] buffer = new Byte[1024];
                Int32 len = input.Read(buffer, 0, 1024);
                Int32 startPos = -1;

                // Find start boundary
                while (true)
                {
                    if (len == 0)
                    {
                        throw new Exception("Start Boundaray Not Found");
                    }

                    startPos = IndexOf(buffer, len, boundaryBytes);
                    if (startPos >= 0)
                    {
                        break;
                    }
                    else
                    {
                        Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
                        len = input.Read(buffer, boundaryLen, 1024 - boundaryLen);
                    }
                }

                // Skip four lines (Boundary, Content-Disposition, Content-Type, and a blank)
                for (Int32 i = 0; i < 4; i++)
                {
                    while (true)
                    {
                        if (len == 0)
                        {
                            throw new Exception("Preamble not Found.");
                        }

                        startPos = Array.IndexOf(buffer, enc.GetBytes("\n")[0], startPos);
                        Console.WriteLine("Multipart - skip 4 lines - startPos: {0}", startPos);
                        if (startPos >= 0)
                        {
                            startPos++;
                            break;
                        }
                        else
                        {
                            len = input.Read(buffer, 0, 1024);
                        }
                    }
                }

                Array.Copy(buffer, startPos, buffer, 0, len - startPos);
                len = len - startPos;

                while (true)
                {
                    Int32 endPos = IndexOf(buffer, len, boundaryBytes);
                    if (endPos >= 0)
                    {
                        if (endPos > 0) output.Write(buffer, 0, endPos);
                        break;
                    }
                    else if (len <= boundaryLen)
                    {
                        throw new Exception("End Boundaray Not Found");
                    }
                    else
                    {
                        output.Write(buffer, 0, len - boundaryLen);
                        Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
                        len = input.Read(buffer, boundaryLen, 1024 - boundaryLen) + boundaryLen;
                    }
                }
            }
            input.Close();
        }

        protected Int32 IndexOf(Byte[] buffer, Int32 len, Byte[] boundaryBytes)
        {
            for (Int32 i = 0; i <= len - boundaryBytes.Length; i++)
            {
                Boolean match = true;
                for (Int32 j = 0; j < boundaryBytes.Length && match; j++)
                {
                    match = buffer[i + j] == boundaryBytes[j];
                }

                if (match)
                {
                    return i;
                }
            }

            return -1;
        }

        // Alternative that doesnt work under MonoTouch though!
//        public static IPAddress GetIPAddressForDesktop()
//        {
//            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
//                return null;
//
//            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
//            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
//        }

        public static string GetLocalIPAddress()
        {
            string address = "Not Connected";
            try
            {
                //Console.WriteLine("GetIPAddress - Detecting IP address...");
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    //Console.WriteLine("GetIPAddress - NetworkInterface: {0} {1}", ni.Name, ni.NetworkInterfaceType.ToString());
                    if((ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                    {                        
                        IPAddress ip = null;
                        foreach (var a in ni.GetIPProperties().UnicastAddresses)
                        {
                            //Console.WriteLine("GetIPAddress - Address: {0} {1}", a.Address, a.Address.AddressFamily.ToString());
                            if(a.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                //Console.WriteLine("GetIPAddress - Address **FOUND**: {0} {1}", a.Address, a.Address.AddressFamily.ToString());
                                ip = a.Address;
                                break;
                            }
                        }

                        if(ip != null)
                        {
                            address = ip.ToString();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetIPAddress - Exception: {0}", ex);
            }
            return address;
        }
    }
}
