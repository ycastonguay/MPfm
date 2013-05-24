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
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public class SyncListenerService : ISyncListenerService
    {
        private readonly IAudioFileCacheService _audioFileCacheService;
        private HttpListener _httpListener;

        public const string SyncVersionId = "sessions_app_sync_version_1";

#if IOS
        public const SyncDeviceType DeviceType = SyncDeviceType.iOS;
#elif ANDROID
        public const SyncDeviceType DeviceType = SyncDeviceType.Android;
#elif MACOSX
        public const SyncDeviceType DeviceType = SyncDeviceType.OSX;
#elif LINUX
        public const SyncDeviceType DeviceType = SyncDeviceType.Linux;
#else
        public const SyncDeviceType DeviceType = SyncDeviceType.Windows;
#endif

        public int Port { get; private set; }

        public SyncListenerService(IAudioFileCacheService audioFileCacheService)
        {
            _audioFileCacheService = audioFileCacheService;
            Port = 53551;
            Initialize();
        }

        private void Initialize()
        {
            Console.WriteLine("SyncListenerService - Initializing service...");
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://*:" + Port.ToString("0") + "/");
        }

        public void Start()
        {
            Console.WriteLine("SyncListenerService - Starting listener on port {0}...", Port);
            _httpListener.Start();
            Task.Factory.StartNew(() => {
                while (true)
                {
                    HttpListenerContext context = _httpListener.GetContext();
                    Task.Factory.StartNew((ctx) => {
                        var httpContext = (HttpListenerContext)ctx;
                        Console.WriteLine("SyncListenerService - command: {0} url: {1}", httpContext.Request.HttpMethod, httpContext.Request.Url.ToString());

                        string command = httpContext.Request.Url.PathAndQuery;

                        // /index           ==> Returns an XML file with the list of audio files in the library
                        // /audioFile/id/   ==> Returns the audio file in binary format

                        string agent = String.Format("MPfm: Music Player for Musicians version {0} running on {1}", Assembly.GetExecutingAssembly().GetName().Version.ToString(), DeviceType.ToString());
                        if(httpContext.Request.HttpMethod.ToUpper() == "GET")
                        {
                            if(command == "/")
                            {
                                // This returns information about this web service
                                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MPfm.Library.WebApp.index.html"))
                                    WriteFileResponse(httpContext, stream);
                            }
                            else if(command.ToUpper().StartsWith("/INDEX"))
                            {
                                // This returns the index of all audio files
                                // TODO: Add cache. Add AudioFileCacheUpdated message to flush cache.
                                // TODO: Add put audio file. this would enable a web interface to add audio files from a dir. ACTUALLY... /web could have a nice web-based interface for this
                                //
                                try
                                {
                                    string xml = XmlSerialization.Serialize(_audioFileCacheService.AudioFiles);
                                    WriteXMLResponse(httpContext, xml);
                                }
                                catch(Exception ex)
                                {
                                    WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while parsing the library.</h2><p>{0}</p>", ex, agent), HttpStatusCode.InternalServerError);
                                }
                            }
                            else if(command.ToUpper().StartsWith("/SESSIONSAPPVERSION"))
                            {
                                // This is used to know this is a Sessions web service
                                WriteHTMLResponse(httpContext, SyncVersionId);
                            }
                            else if(command.ToUpper().StartsWith("/AUDIOFILE"))
                            {
                                try
                                {
                                    // This returns audio files in binary format
                                    string[] split = command.Split('/');
                                    var audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == new Guid(split[2]));
                                    if(audioFile == null)
                                    {
                                        WriteHTMLResponse(httpContext, String.Format("<h2>Content could not be found.</h2><p>{0}</p>", agent), HttpStatusCode.NotFound);
                                        return;
                                    }
                                    WriteFileResponse(httpContext, audioFile.FilePath);
                                }
                                catch(Exception ex)
                                {
                                    WriteHTMLResponse(httpContext, String.Format("<h2>An error occured.</h2><p>{0}</p>", ex, agent), HttpStatusCode.InternalServerError);
                                }
                            }
                            else
                            {
                                // Try to serve file from WebApp (/WebApp/css/app.css ==> MPfm.Library.WebApp.css.app.css)
                                string resourceName = "MPfm.Library.WebApp" + command.Replace("/", ".");
                                var info = Assembly.GetExecutingAssembly().GetManifestResourceInfo(resourceName);
                                if(info != null)
                                {
                                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                                        WriteFileResponse(httpContext, stream);
                                }
                                else
                                {
                                    WriteHTMLResponse(httpContext, String.Format("<h2>Content could not be found.</h2><p>{0}</p>", agent), HttpStatusCode.NotFound);
                                }
                            }
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
                    }, context,TaskCreationOptions.LongRunning);
                }
            },TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            Console.WriteLine("SyncListenerService - Stopping listener...");
            _httpListener.Stop();
        }

        private void WriteHTMLResponse(HttpListenerContext context, string response)
        {
            WriteHTMLResponse(context, response, HttpStatusCode.OK);
        }

        private void WriteHTMLResponse(HttpListenerContext context, string response, HttpStatusCode statusCode)
        {
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentLength64 = responseBytes.Length;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();
        }

        private void WriteXMLResponse(HttpListenerContext context, string response)
        {
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
            context.Response.ContentType = "text/xml";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentLength64 = responseBytes.Length;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();
        }

        private void WriteFileResponse(HttpListenerContext context, string filePath)
        {
            var response = context.Response;
            using (FileStream fs = File.OpenRead(filePath))
            {
                response.ContentLength64 = fs.Length;
                response.SendChunked = false;
                response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
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

        private void WriteFileResponse(HttpListenerContext context, Stream stream)
        {
            var response = context.Response;
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

        private void ReadFileTextResponse(HttpListenerContext context)
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

        private void ReadFileBinaryResponse(HttpListenerContext context)
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
            BinaryReader reader = new BinaryReader(stream);
            using (FileStream writeStream = File.OpenWrite("/Users/ycastonguay/Desktop/hello.mp3"))
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

        // Good for desktop
        public static IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return null;

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
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
        
        private String GetBoundary(String ctype)
        {
            return "--" + ctype.Split(';')[1].Split('=')[1];
        }

        private void ReadFileMultiPartFormData(Encoding enc, String boundary, Stream input)
        {
            // http://stackoverflow.com/questions/8466703/httplistener-and-file-upload
            Byte[] boundaryBytes = enc.GetBytes(boundary);
            Int32 boundaryLen = boundaryBytes.Length;

            using (FileStream output = new FileStream("/Users/ycastonguay/Desktop/test.txt", FileMode.Create, FileAccess.Write))
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

        private Int32 IndexOf(Byte[] buffer, Int32 len, Byte[] boundaryBytes)
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
    }
}
