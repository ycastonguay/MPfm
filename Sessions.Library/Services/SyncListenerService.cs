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
using System.Reflection;
using MPfm.Core;
using MPfm.Sound.AudioFiles;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
using MPfm.Player.Objects;

namespace MPfm.Library.Services
{
    public class SyncListenerService : SyncListenerServiceBase
    {
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly ILibraryService _libraryService;
        private readonly ISyncDeviceManagerService _syncDeviceManagerService;
        private readonly ISyncDeviceSpecifications _syncDeviceSpecifications;

        public const string SyncVersionId = "sessions_app_sync_version_1";
        private static string _authenticationCode = GetRandomNumber(10000, 99999).ToString();
        public static string AuthenticationCode { get { return _authenticationCode; } }

        public override string ServerAgent
        {
            get
            {
                var device = GetInternalSyncDevice();
                return string.Format("Sessions version {0} running on {1}", Assembly.GetExecutingAssembly().GetName().Version, device.DeviceType);
            }
        }

        public SyncListenerService(IAudioFileCacheService audioFileCacheService, ILibraryService libraryService, ISyncDeviceManagerService syncDeviceManagerService, ISyncDeviceSpecifications syncDeviceSpecifications) 
            : base()
        {
            Console.WriteLine("SyncListenerService - AuthenticationCode: {0}", AuthenticationCode);
            _audioFileCacheService = audioFileCacheService;
            _libraryService = libraryService;
            _syncDeviceManagerService = syncDeviceManagerService;
            _syncDeviceSpecifications = syncDeviceSpecifications;
            _syncDeviceSpecifications.OnNetworkStateChanged += delegate(NetworkState networkState) {
                Console.WriteLine("SyncListenerService - NetworkStateChanged isNetworkAvailable: {0} isWifiAvailable: {1} isCellularAvailable: {2}", networkState.IsNetworkAvailable, networkState.IsWifiAvailable, networkState.IsCellularAvailable);
                if (networkState.IsWifiAvailable && !IsRunning)
                {
                    Console.WriteLine("SyncListenerService - NetworkStateChanged - Wifi is now available; restarting HTTP service...");
                    Start();
                }
                else if (!networkState.IsWifiAvailable && IsRunning)
                {
                    Console.WriteLine("SyncListenerService - NetworkStateChanged - Wifi is no longer available; stopping HTTP service...");
                    Stop();
                }
            };
        }

        private static int GetRandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        public override void ProcessCommand(HttpListenerContext httpContext, string command)
        {
            // List of API commands:
            // /api/index           ==> Returns an XML file with the list of audio files in the library
            // /api/audioFile/id/   ==> Returns the audio file in binary format
            // /api/player          ==> Returns the current player status and metadata
            // /api/playlist        ==> Returns the current player playlist
            // /api/remote/*        ==> Remote commands for controlling the player

            if(command == "/")
            {
                // TODO: Get query string code for authentication. Maybe redirect to login.html? Make sure you have the name of the file
                //       in the url. i.e. login.html?authenticationCode=AAAA
                // This returns information about this web service

                // Check the cookies (or local storage) for code. If not found, redirect to login.
                WriteRedirect(httpContext, "login.html"); 
            }
            else if(command.ToUpper().StartsWith("/SESSIONSAPP.VERSION"))
            {
                ProcessGetVersionCommand(httpContext);
            }
            else if (command.ToUpper() == "/API/DEVICES")
            {
                ProcessGetDevicesCommand(httpContext);
            }
            else if(command.ToUpper() == "/API/INDEX/XML")
            {
                ProcessGetIndexXmlCommand(httpContext);
            }
            else if(command.ToUpper() == "/API/INDEX/JSON")
            {
                ProcessGetIndexJsonCommand(httpContext);
            }
            else if(command.ToUpper().StartsWith("/API/AUDIOFILE"))
            {
                ProcessGetAudioFileCommand(httpContext, command);
            }
            else if(command.ToUpper().StartsWith("/API/ALBUMART"))
            {
                ProcessGetAlbumArtCommand(httpContext, command);
            }
            else if(command.ToUpper().StartsWith("/API/PLAYER"))
            {
                ProcessGetPlayerCommand(httpContext);
            }
            else if(command.ToUpper().StartsWith("/API/PLAYLIST"))
            {
                ProcessGetPlaylistCommand(httpContext);
            }
            else if (command.ToUpper().StartsWith("/API/EQPRESETS"))
            {
                ProcessGetEQPresetsCommand(httpContext);
            }
            else if (command.ToUpper().StartsWith("/API/MARKERS"))
            {
                ProcessGetMarkersCommand(httpContext, command);
            }
            else if (command.ToUpper().StartsWith("/API/LOOPS"))
            {
                ProcessGetLoopsCommand(httpContext, command);
            }
            else if (command.ToUpper().StartsWith("/API/REMOTE"))
            {
                ProcessRemoteCommand(httpContext, command);
            }
            else
            {
                ProcessGetResourceCommand(httpContext, command);
            }
        }

        private void ProcessGetVersionCommand(HttpListenerContext httpContext)
        {
            try
            {
                string xml = XmlSerialization.Serialize(GetInternalSyncDevice());
                WriteXMLResponse(httpContext, xml);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while parsing the library.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetDevicesCommand(HttpListenerContext httpContext)
        {
            try
            {
                var devices = _syncDeviceManagerService.GetDeviceList();
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(devices);
                WriteJSONResponse(httpContext, json);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while parsing the list of devices.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetIndexXmlCommand(HttpListenerContext httpContext)
        {
            // This returns the index of all audio files
            // TODO: Add cache. Add AudioFileCacheUpdated message to flush cache.
            // TODO: Add put audio file. this would enable a web interface to add audio files from a dir. ACTUALLY... /web could have a nice web-based interface for this
            //
            try
            {
                var audioFiles = _audioFileCacheService.AudioFiles.OrderBy(x => x.ArtistName).ThenBy(x => x.AlbumTitle).ThenBy(x => x.DiscNumber).ThenBy(x => x.TrackNumber).ToList();
                string xml = XmlSerialization.Serialize(audioFiles);
                WriteXMLResponse(httpContext, xml);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while parsing the library.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetIndexJsonCommand(HttpListenerContext httpContext)
        {
            // This returns the index of all audio files
            // TODO: Add cache. Add AudioFileCacheUpdated message to flush cache.
            // TODO: Add put audio file. this would enable a web interface to add audio files from a dir. ACTUALLY... /web could have a nice web-based interface for this
            //
            try
            {
                var audioFiles = _audioFileCacheService.AudioFiles.OrderBy(x => x.ArtistName).ThenBy(x => x.AlbumTitle).ThenBy(x => x.DiscNumber).ThenBy(x => x.TrackNumber).ToList();
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(audioFiles);
                WriteJSONResponse(httpContext, json);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while parsing the library.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessRemoteCommand(HttpListenerContext httpContext, string command)
        {
            try
            {
                var player = MPfm.Player.Player.CurrentPlayer;
                if (player == null)
                {
                    WriteHTMLResponse(httpContext, "<h2>Could not process remote command; the player isn't available.</h2>", HttpStatusCode.ServiceUnavailable);
                    return;
                }
                var split = command.Split(new char[1] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                string remoteCommand = split[2];
                switch (remoteCommand.ToUpper())
                {
                    case "PLAY":
                        if (player.IsPaused)
                            player.Pause();
                        else
                            player.Play();
                        break;
                    case "PAUSE":
                        player.Pause();
                        break;
                    case "STOP":
                        player.Stop();
                        break;
                    case "PREVIOUS":
                        player.Previous();
                        break;
                    case "NEXT":
                        player.Next();
                        break;
                    case "GOTO":
                        int gotoIndex = -1;
                        int.TryParse(split[3], out gotoIndex);
                        if (gotoIndex == -1 || gotoIndex > player.Playlist.Items.Count - 1)
                        {
                            WriteHTMLResponse(httpContext, "<h2>Playlist item index is out of bounds.</h2>", HttpStatusCode.BadRequest);
                            return;
                        }
                        player.GoTo(gotoIndex);
                        break;
                    default:
                        WriteHTMLResponse(httpContext, "<h2>Remote command could not be found.</h2>", HttpStatusCode.NotImplemented);
                        return;
                        break;
                }
                WriteHTMLResponse(httpContext, "<h2>Remote command processed successfully.</h2>");
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while processing a remote command.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetPlaylistCommand(HttpListenerContext httpContext)
        {
            try
            {
                var player = MPfm.Player.Player.CurrentPlayer;
                if (player == null)
                {
                    WriteHTMLResponse(httpContext, "<h2>Could not fetch player metadata; the player isn't available.</h2>", HttpStatusCode.ServiceUnavailable);
                    return;
                }
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(player.Playlist);
                WriteJSONResponse(httpContext, json);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while fetching player metadata.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetPlayerCommand(HttpListenerContext httpContext)
        {
            try
            {
                var player = MPfm.Player.Player.CurrentPlayer;
                if(player == null)
                {
                    WriteHTMLResponse(httpContext, "<h2>Could not fetch player metadata; the player isn't available.</h2>", HttpStatusCode.ServiceUnavailable);
                    return;
                }

                var metadata = new PlayerMetadata();
                metadata.CurrentAudioFile = player.Playlist.CurrentItem.AudioFile;
                metadata.Status = player.IsPlaying ? player.IsPaused ? PlayerMetadata.PlayerStatus.Paused : PlayerMetadata.PlayerStatus.Playing : PlayerMetadata.PlayerStatus.Stopped;
                metadata.Length = player.Playlist.CurrentItem.LengthString;
                metadata.PlaylistCount = player.Playlist.Items.Count;
                metadata.PlaylistIndex = player.Playlist.CurrentItemIndex;

                long bytes = player.GetPosition();
                long samples = ConvertAudio.ToPCM(bytes, (uint)player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
                long ms = (int)ConvertAudio.ToMS(samples, (uint)player.Playlist.CurrentItem.AudioFile.SampleRate);
                string position = Conversion.MillisecondsToTimeString((ulong)ms);
                metadata.Position = position;

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(metadata);
                WriteJSONResponse(httpContext, json);
            }
            catch(Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured while fetching player metadata.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetAudioFileCommand(HttpListenerContext httpContext, string command)
        {
            try
            {
                // This returns audio files in binary format
                string[] split = command.Split(new char[1] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                var audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == new Guid(split[2]));
                if (audioFile == null)
                {
                    WriteHTMLResponse(httpContext, String.Format("<h2>Content could not be found.</h2>"), HttpStatusCode.NotFound);
                    return;
                }
                WriteFileResponse(httpContext, audioFile.FilePath, true);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetAlbumArtCommand(HttpListenerContext httpContext, string command)
        {
            try
            {
                // This returns audio files in binary format
                string[] split = command.Split(new char[1] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                var audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == new Guid(split[2]));
                if (audioFile == null)
                {
                    WriteHTMLResponse(httpContext, String.Format("<h2>Content could not be found.</h2>"), HttpStatusCode.NotFound);
                    return;
                }

                // TODO: Cache! Also protect from spamming
                byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                WriteBinaryResponse(httpContext, bytesImage);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetEQPresetsCommand(HttpListenerContext httpContext)
        {
            try
            {
                var presets = _libraryService.SelectEQPresets();
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(presets);
                WriteJSONResponse(httpContext, json);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetMarkersCommand(HttpListenerContext httpContext, string command)
        {
            try
            {
                string[] split = command.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var audioFileId = new Guid(split[2]);
                var markers = _libraryService.SelectMarkers(audioFileId);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(markers);
                WriteJSONResponse(httpContext, json);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetLoopsCommand(HttpListenerContext httpContext, string command)
        {
            try
            {
                string[] split = command.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var audioFileId = new Guid(split[2]);
                var loops = _libraryService.SelectLoops(audioFileId);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(loops);
                WriteJSONResponse(httpContext, json);
            }
            catch (Exception ex)
            {
                WriteHTMLResponse(httpContext, String.Format("<h2>An error occured.</h2><p>{0}</p>", ex), HttpStatusCode.InternalServerError);
            }
        }

        private void ProcessGetResourceCommand(HttpListenerContext httpContext, string command)
        {
            // Try to serve file from WebApp (/WebApp/css/app.css ==> MPfm.Library.WebApp.css.app.css)
            bool isAuthenticated = IsAuthenticated(httpContext);
            // Remove query string
            string[] commandSplit = command.Split(new char[1] {
                '?'
            }, StringSplitOptions.RemoveEmptyEntries);
            string resourceName = "MPfm.Library.WebApp" + commandSplit[0].Replace("/", ".");
            if (commandSplit[0].ToUpper().StartsWith("/LOGIN.HTML"))
            {
                if (isAuthenticated)
                {
                    WriteRedirect(httpContext, "index.html");
                }
                else
                {
                    // Prevent doing a redirect loop.
                    string redirectFail = "/login.html?loginStatus=failed";
                    if (command != redirectFail)
                    {
                        WriteRedirect(httpContext, redirectFail);
                        return;
                    }
                }
            }
            // If trying to access index.html without authentication, redirect to login.html
            if (commandSplit[0].ToUpper().StartsWith("/INDEX.HTML") && !isAuthenticated)
            {
                WriteRedirect(httpContext, "/login.html?loginStatus=failed");
                return;
            }
            // if index.html and not authenticated; redirect to login.html.
            //                    if(commandSplit[0].ToUpper() == "/LOGIN.HTML" && commandSplit.Length > 1)
            //                    {
            //                        string queryString = commandSplit[1];
            //                        // authenticationCode=value&whatever=true
            //                        string[] queryStringItems = queryString.Split('&');
            //                        foreach(string queryStringItem in queryStringItems)
            //                        {
            //                            string[] queryStringEquals = queryStringItem.Split('=');
            //                            if(queryStringEquals[0].ToUpper() == "AUTHENTICATIONCODE" && queryStringEquals.Length > 1)
            //                            {
            //                                string authenticationCode = queryStringEquals[1];
            //                            }
            //                        }
            //                        return;
            //                    }
            var info = Assembly.GetExecutingAssembly().GetManifestResourceInfo(resourceName);
            if (info != null)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                    WriteStreamResponse(httpContext, stream);
            }
            else
            {
                WriteHTMLResponse(httpContext, "<h2>Content could not be found.</h2>", HttpStatusCode.NotFound);
            }
        }

        private bool IsAuthenticated(HttpListenerContext context)
        {
            var authenticationCodeCookie = context.Request.Cookies["authenticationCode"];
            if(authenticationCodeCookie != null)
            {
                string authenticationCode = authenticationCodeCookie.Value;
                if(AuthenticationCode.ToUpper() == authenticationCode.ToUpper())
                    return true;
                else
                    return false;
            }
            return false;
        }

        private SyncDevice GetInternalSyncDevice()
        {
            var device = new SyncDevice();
            device.SyncVersionId = SyncVersionId;
            device.Name = _syncDeviceSpecifications.GetDeviceName();
            device.DeviceType = _syncDeviceSpecifications.GetDeviceType();
            return device;
        }
    }
}
