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
using System.Diagnostics;
using System.Reflection;
using DropBoxSync.iOS;
using MPfm.Library;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using MonoTouch.Foundation;
using MPfm.Core;
using Newtonsoft.Json;
using MPfm.Library.Objects;
using MonoTouch.UIKit;

namespace MPfm.iOS.Classes.Services
{
	public class iOSDropboxService : ICloudLibraryService
	{
        // Of course, those are temp keys and will be replaced when pushing to the App Store.
        //private string _dropboxAppKey = "6tc6565743i743n";
        //private string _dropboxAppSecret = "fbkt3neevjjl0l2";
        private string _dropboxAppKey = "m1bcpax276elhfi";
        private string _dropboxAppSecret = "2azbuj2eelkranm";

        private ISyncDeviceSpecifications _deviceSpecifications;
        private DBDatastore _store;
        private DBFilesystem _fileSystem;

        public event CloudAuthenticationStatusChanged OnCloudAuthenticationStatusChanged;
        public event CloudAuthenticationFailed OnCloudAuthenticationFailed;
        public event CloudDataChanged OnCloudDataChanged;

        public bool HasLinkedAccount
        {
            get
            {
                return DBAccountManager.SharedManager.LinkedAccount != null;
            }
        }

        public iOSDropboxService()
        {
            _deviceSpecifications = Bootstrapper.GetContainer().Resolve<ISyncDeviceSpecifications>();
            Initialize();
        }

        private void Initialize()
        {
            var manager = new DBAccountManager(_dropboxAppKey, _dropboxAppSecret);
            DBAccountManager.SharedManager = manager;

            // Check if user is logged in 
            var account = DBAccountManager.SharedManager.LinkedAccount;
            if (account == null)
                return;

            if (_store != null && _store.Open)
                return;

            DBError error = null;
            _store = DBDatastore.OpenDefaultStoreForAccount(account, out error);
            if(error != null)
                throw new Exception(error.Description);

            _fileSystem = new DBFilesystem(account);
            DBFilesystem.SharedFilesystem = _fileSystem;

            _fileSystem.AddObserverForPathAndChildren(_fileSystem, new DBPath("/Devices"), () => {
                Console.WriteLine("SyncCloudViewController - FileSystem - Data changed!");
                if(OnCloudDataChanged != null)
                    OnCloudDataChanged(string.Empty);
            });

//            _store.AddObserver (_store, () => {
//                Console.WriteLine("SyncCloudViewController - DBDatastore - Data changed!");
//                if (_store.Status.HasFlag(DBDatastoreStatus.Incoming)) {
//                    // Handle the updated data
//                    Console.WriteLine("SyncCloudViewController - DBDatastore - Incoming data!");
//                    try
//                    {
//                        DBError error2 = null;
//                        var changes = _store.Sync(error2);
//                        if(error2 != null)
//                            throw new Exception(error2.Description);
//
//                        if (!changes.ContainsKey(new NSString("stuff")))
//                            return;
//
//                        var records = (NSSet)changes["stuff"];
//                        foreach(var record in records)
//                        {
//                            var theRecord = (DBRecord)record;
//                            var timestamp = theRecord.ObjectForKey("timestamp");
//                            var deviceType = theRecord.ObjectForKey("deviceType");
//                            var deviceName = theRecord.ObjectForKey("deviceName");
//                            //lblValue.Text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine("SyncCloudActivity - OnDatastoreStatusChange exception: {0}", ex);
//                        //lblValue.Text = string.Format("Error: {0}", ex);
//                    }
//                }
//            });
        }

        public void LinkApp(object view)
        {
            using (NSAutoreleasePool pool = new NSAutoreleasePool())
            {
                pool.InvokeOnMainThread(() =>
                {
                    var account = DBAccountManager.SharedManager.LinkedAccount;
                    if (account == null)
                    {
                        DBAccountManager.SharedManager.LinkFromController((UIViewController)view);
                    }
                });
            }
        }

        public void UnlinkApp()
        {
            var account = DBAccountManager.SharedManager.LinkedAccount;
            if (account.Linked)
                account.Unlink();
        }

        public void ContinueLinkApp()
        {
            if (HasLinkedAccount)
            {
                if (OnCloudAuthenticationStatusChanged != null)
                    OnCloudAuthenticationStatusChanged(CloudAuthenticationStatusType.ConnectedToDropbox);
            }
            else
            {
                if (OnCloudAuthenticationFailed != null)
                    OnCloudAuthenticationFailed();
            }
        }

        public void InitializeAppFolder()
        {
            // Ignore any errors; there is no way to check if the folder exists before (like on Android!)
            DBError error = null;
            _fileSystem.CreateFolder(new DBPath("/Devices"), out error);
            _fileSystem.CreateFolder(new DBPath("/Playlists"), out error);
        }

        public void PushHello()
        {
        }

        public void PushStuff()
        {
        }

        public string PullStuff()
        {
            return string.Empty;
        }

        public void DeleteStuff()
        {
        }

        public string PushDeviceInfo(AudioFile audioFile, long positionBytes, string position)
        {
            DBError error = null;
            DBFile file = null;

            try
            {
                var nowPlaying = new CloudDeviceInfo(){
                    AudioFileId = audioFile.Id,
                    ArtistName = audioFile.ArtistName,
                    AlbumTitle = audioFile.AlbumTitle,
                    SongTitle = audioFile.Title,
                    Position = position,
                    PositionBytes = positionBytes,
                    DeviceType = _deviceSpecifications.GetDeviceType().ToString(),
                    DeviceName = _deviceSpecifications.GetDeviceName(),
                    DeviceId = _deviceSpecifications.GetDeviceUniqueId(),
                    IPAddress = _deviceSpecifications.GetIPAddress(),
                    Timestamp = DateTime.Now
                };

                // Do we really need to check folder existence before each call?
                var path = new DBPath(string.Format("/Devices/{0}.json", nowPlaying.DeviceId));

                // Unlike the Android SDK, there is no method to check if a file exists... 
                var fileInfo = _fileSystem.FileInfoForPath(path, out error);
                if (fileInfo == null)
                    file = _fileSystem.CreateFile(path, out error);
                else
                    file = _fileSystem.OpenFile(path, out error);

                if(error != null)
                    throw new Exception(error.Description);

                string json = JsonConvert.SerializeObject(nowPlaying);
                file.WriteString(json, out error);

                if(error != null)
                    throw new Exception(error.Description);
            }
            catch (Exception ex)
            {
                Tracing.Log("iOSDropboxService - PushDeviceInfo - Exception: {0}", ex);
                throw;
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            return string.Empty;
        }

        public IEnumerable<CloudDeviceInfo> PullDeviceInfos()
        {
            List<CloudDeviceInfo> devices = new List<CloudDeviceInfo>();
            DBError error = null;
            DBFile file = null;

            try
            {
                var fileInfos = _fileSystem.ListFolder(new DBPath("/Devices"), out error);
                if(error != null)
                    throw new Exception(error.Description);

                foreach(var fileInfo in fileInfos)
                {
                    try
                    {
                        file = _fileSystem.OpenFile(fileInfo.Path, out error);
                        if(error != null)
                            throw new Exception(error.Description);

                        file.Update(out error);
                        if(error != null)
                            throw new Exception(error.Description);

                        string json = file.ReadString(out error);
                        if(error != null)
                            throw new Exception(error.Description);

                        CloudDeviceInfo device = null;
                        try
                        {
                            device = JsonConvert.DeserializeObject<CloudDeviceInfo>(json);
                            devices.Add(device);
                        }
                        catch(Exception ex)
                        {
                            Tracing.Log("iOSDropboxService - PullDeviceInfos - Failed to deserialize JSON for path {0} - ex: {1}", fileInfo.Path.Name, ex);
                        }
                    }
                    catch(Exception ex)
                    {
                        Tracing.Log("iOSDropboxService - PullDeviceInfos - Failed to download file {0} - ex: {1}", fileInfo.Path.Name, ex);
                    }
                    finally
                    {
                        if(file != null)
                            file.Close();                            
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("iOSDropboxService - PushNowPlaying - Exception: {0}", ex);
                throw;
            }

            return devices;
        }

        public void DeleteNowPlaying()
        {
        }

        public string PushPlaylist(Playlist playlist)
        {
            return string.Empty;
        }

        public Playlist PullPlaylist(Guid playlistId)
        {
            return new Playlist();
        }

        public IEnumerable<Playlist> PullPlaylists()
        {
            return new List<Playlist>();
        }

        public void DeletePlaylist(Guid playlistId)
        {
        }

        public void DeletePlaylists()
        {
        }
	}
}
