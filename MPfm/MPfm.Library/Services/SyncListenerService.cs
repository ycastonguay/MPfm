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
using MPfm.Core;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public class SyncListenerService : ISyncListenerService
    {
        private readonly IAudioFileCacheService _audioFileCacheService;
        private HttpListener _httpListener;

        public const string SyncVersionId = "sessions_app_sync_version_1";

        public int Port { get; private set; }

        public SyncListenerService(IAudioFileCacheService audioFileCacheService)
        {
            _audioFileCacheService = audioFileCacheService;
            Port = 8080;
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
                        Console.WriteLine("SyncListenerService - url: {0}", httpContext.Request.Url.ToString());

                        string command = httpContext.Request.Url.PathAndQuery;

                        // /index           ==> Returns an XML file with the list of audio files in the library
                        // /audioFile/id/   ==> Returns the audio file in binary format

                        string agent = String.Format("MPfm: Music Player for Musicians version {0} running on {1}", "0.7.0.0", OS.Type.ToString());
                        if(command == "/")
                        {
                            // This returns information about this web service
                            WriteHTML(httpContext, String.Format("<h2>Hello world!</h2><p>This web service is used for syncing content between devices.</p><p>{0}</p>", agent));
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
                                WriteXML(httpContext, xml);
                            }
                            catch(Exception ex)
                            {
                                WriteHTML(httpContext, String.Format("<h2>An error occured while parsing the library.</h2><p>{0}</p>", ex, agent), HttpStatusCode.InternalServerError);
                            }
                        }
                        else if(command.ToUpper().StartsWith("/SESSIONSAPPVERSION"))
                        {
                            // This is used to know this is a Sessions web service
                            WriteHTML(httpContext, SyncVersionId);
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
                                    WriteHTML(httpContext, String.Format("<h2>Content could not be found.</h2><p>{0}</p>", agent), HttpStatusCode.NotFound);
                                    return;
                                }
                                WriteFile(httpContext, audioFile.FilePath);
                            }
                            catch(Exception ex)
                            {
                                WriteHTML(httpContext, String.Format("<h2>An error occured.</h2><p>{0}</p>", ex, agent), HttpStatusCode.InternalServerError);
                            }
                        }
                        else
                        {
                            WriteHTML(httpContext, String.Format("<h2>Content could not be found.</h2><p>{0}</p>", agent), HttpStatusCode.NotFound);
                        }
                    }, context,TaskCreationOptions.LongRunning);
                }
            },TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _httpListener.Stop();
        }

        private void WriteHTML(HttpListenerContext context, string response)
        {
            WriteHTML(context, response, HttpStatusCode.OK);
        }

        private void WriteHTML(HttpListenerContext context, string response, HttpStatusCode statusCode)
        {
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentLength64 = responseBytes.Length;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();
        }

        private void WriteXML(HttpListenerContext context, string response)
        {
            byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
            context.Response.ContentType = "text/xml";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentLength64 = responseBytes.Length;
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();
        }

        private void WriteFile(HttpListenerContext context, string filePath)
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
    }
}
