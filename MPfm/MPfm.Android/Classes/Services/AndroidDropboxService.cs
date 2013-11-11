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
using System.Threading.Tasks;
using Android.App;
using Com.Dropbox.Sync.Android;
using MPfm.Core;
using MPfm.Library;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using Newtonsoft.Json;

namespace MPfm.Android.Classes.Services
{
    public class AndroidDropboxService : Java.Lang.Object, ICloudLibraryService, DbxDatastore.ISyncStatusListener, DbxAccountManager.IAccountListener, DbxFileSystem.IPathListener, DbxFileSystem.ISyncStatusListener, DbxFile.IListener
    {
        private readonly ILibraryService _libraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly ISyncDeviceSpecifications _deviceSpecifications;
        private DbxAccountManager _accountManager;
        private DbxAccount _account;
        private DbxDatastore _store;
        private DbxFileSystem _fileSystem;

        public event CloudAuthenticationStatusChanged OnCloudAuthenticationStatusChanged;
        public event CloudDataChanged OnCloudDataChanged;

        public bool HasLinkedAccount
        {
            get
            {
                return _accountManager != null && _accountManager.HasLinkedAccount;
            }
        }

        public AndroidDropboxService(ILibraryService libraryService, IAudioFileCacheService audioFileCacheService, ISyncDeviceSpecifications deviceSpecifications)
        {
            _libraryService = libraryService;
            _audioFileCacheService = audioFileCacheService;
            _deviceSpecifications = deviceSpecifications;
            Initialize();
        }

        private void Initialize()
        {
            // These keys will be replaced when the app is pushed to Google Play :-)
            // Test account for Audio Files
            //string appKey = "6tc6565743i743n";
            //string appSecret = "fbkt3neevjjl0l2";
            // Test account for App Folder
            string appKey = "m1bcpax276elhfi";
            string appSecret = "2azbuj2eelkranm";

            try
            {
                _accountManager = DbxAccountManager.GetInstance(MPfmApplication.GetApplicationContext(), appKey, appSecret);
                _accountManager.AddListener(this);

                if (_accountManager.HasLinkedAccount)
                    _account = _accountManager.LinkedAccount;

                if (_account != null)
                {
                    _store = DbxDatastore.OpenDefault(_account);
                    _store.AddSyncStatusListener(this);

                    _fileSystem = DbxFileSystem.ForAccount(_account);
                    _fileSystem.AddPathListener(this, new DbxPath("/"), DbxFileSystem.PathListenerMode.PathOrDescendant);
                    _fileSystem.AddSyncStatusListener(this);
                    InitializeAppFolder();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("AndroidDropboxService - Initialize - Failed to initialize: {0}", ex);
                throw;
            }
        }

        public void Dispose()
        {
            if (_accountManager != null)
            {
                _accountManager.RemoveListener(this);
            }

            if (_store != null)
            {
                _store.RemoveSyncStatusListener(this);
                _store.Close();
            }

            if (_fileSystem != null)
            {
                _fileSystem.RemovePathListenerForAll(this);
                _fileSystem.RemoveSyncStatusListener(this);
            }
        }

        public void LinkApp(object view)
        {
            try
            {
                if (_accountManager.HasLinkedAccount)
                {
                    _account = _accountManager.LinkedAccount;
                }
                else
                {
                    if (OnCloudAuthenticationStatusChanged != null)
                        OnCloudAuthenticationStatusChanged(CloudAuthenticationStatusType.ConnectToDropbox);

                    if (view is Fragment)
                    {
                        var fragment = (Fragment) view;
                        _accountManager.StartLink(fragment, 0);
                    }
                    else if (view is Activity)
                    {
                        var activity = (Activity)view;
                        _accountManager.StartLink(activity, 0);                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ContinueLinkApp()
        {
            if (OnCloudAuthenticationStatusChanged != null)
                OnCloudAuthenticationStatusChanged(CloudAuthenticationStatusType.ConnectedToDropbox);
        }

        public void UnlinkApp()
        {
            try
            {
                if (_accountManager.HasLinkedAccount)
                {
                    _accountManager.Unlink();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void InitializeAppFolder()
        {
            try
            {
                // Create base folders if they don't exist
                var pathPlaylists = new DbxPath("/Playlists");
                var pathDevices = new DbxPath("/Devices");
                if(!_fileSystem.Exists(pathPlaylists))
                    _fileSystem.CreateFolder(pathPlaylists);
                if (!_fileSystem.Exists(pathDevices))
                    _fileSystem.CreateFolder(pathDevices);
            }
            catch (Exception ex)
            {
                Tracing.Log("AndroidDropboxService - InitializeAppFolder - Exception: {0}", ex);
                throw;
            }
        }

        public string PushDeviceInfo(AudioFile audioFile, long positionBytes, string position)
        {
            return PushNowPlaying_File(audioFile, positionBytes, position);
        }

        public IEnumerable<CloudDeviceInfo> PullDeviceInfos()
        {
            List<CloudDeviceInfo> devices = new List<CloudDeviceInfo>();
            DbxFile file = null;

            try
            {
                var fileInfos = _fileSystem.ListFolder(new DbxPath("/Devices"));

                foreach (var fileInfo in fileInfos)
                {
                    try
                    {
                        file = _fileSystem.Open(fileInfo.Path);
                        //file.Update();
                        string json = file.ReadString();

                        CloudDeviceInfo device = null;
                        try
                        {
                            device = JsonConvert.DeserializeObject<CloudDeviceInfo>(json);
                            devices.Add(device);
                        }
                        catch (Exception ex)
                        {
                            Tracing.Log("AndroidDropboxService - PullDeviceInfos - Failed to deserialize JSON for path {0} - ex: {1}", fileInfo.Path.Name, ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Tracing.Log("AndroidDropboxService - PullDeviceInfos - Failed to download file {0} - ex: {1}", fileInfo.Path.Name, ex);
                    }
                    finally
                    {
                        if (file != null)
                            file.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("AndroidDropboxService - PullDeviceInfos - Exception: {0}", ex);
                throw;
            }

            return devices;
        }

        public string PushNowPlaying(AudioFile audioFile, long positionBytes, string position)
        {
            return PushNowPlaying_File(audioFile, positionBytes, position);
        }

        private string PushNowPlaying_Datastore(AudioFile audioFile, long positionBytes, string position)
        {
            try
            {
                DbxTable tableNowPlaying = _store.GetTable("nowPlaying");
                DbxFields queryParams = new DbxFields();
                queryParams.Set("deviceId", _deviceSpecifications.GetDeviceUniqueId());
                DbxTable.QueryResult results = tableNowPlaying.Query(queryParams);
                var list = results.AsList();

                // Edit existing item or insert new item
                DbxRecord record = list.Count > 0 ? list[0] : tableNowPlaying.Insert();
                if (record != null)
                {
                    record.Set("audioFileId", audioFile.Id.ToString());
                    record.Set("artistName", audioFile.ArtistName);
                    record.Set("albumTitle", audioFile.AlbumTitle);
                    record.Set("title", audioFile.Title);
                    record.Set("position", "0:00.000");
                    record.Set("positionBytes", 0);
                    record.Set("deviceType", _deviceSpecifications.GetDeviceType().ToString());
                    record.Set("deviceName", _deviceSpecifications.GetDeviceName());
                    record.Set("deviceId", _deviceSpecifications.GetDeviceUniqueId());
                    record.Set("ip", _deviceSpecifications.GetIPAddress());
                    record.Set("timestamp", new Java.Util.Date());

                    _store.Sync();
                    return record.Id;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return string.Empty;
        }

        private string PushNowPlaying_File(AudioFile audioFile, long positionBytes, string position)
        {
            DbxFile file = null;

            try
            {
                var nowPlaying = new SerializableNowPlaying(){
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
                var path = new DbxPath(string.Format("/Devices/{0}.json", nowPlaying.DeviceId));
                bool fileExists = _fileSystem.Exists(path);
                if (fileExists)
                    file = _fileSystem.Open(path);
                else
                    file = _fileSystem.Create(path);

                string json = JsonConvert.SerializeObject(nowPlaying);
                file.WriteString(json);
            }
            catch (Exception ex)
            {
                Tracing.Log("AndroidDropboxService - PushNowPlaying_File - Exception: {0}", ex);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
            return string.Empty;
        }

        public string PullNowPlaying()
        {
            string text = string.Empty;

            try
            {
                //DbxTable tableStuff = _store.GetTable("stuff");
                //DbxFields queryParams = new DbxFields();
                //queryParams.Set("test", true);
                //queryParams.Set("hello", "world");
                //DbxTable.QueryResult results = tableStuff.Query(queryParams);
                //var list = results.AsList();
                //if (list.Count == 0)
                //{
                //    //_lblValue.Text = "No value!";
                //    return text;
                //}

                //DbxRecord firstResult = list[0];
                //string timestamp = firstResult.GetString("timestamp");
                //string deviceType = firstResult.GetString("deviceType");
                //string deviceName = firstResult.GetString("deviceName");
                //text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
            }
            catch (Exception ex)
            {
                throw;
            }

            return text;
        }

        public void DeleteNowPlaying()
        {
            try
            {
                DbxTable table = _store.GetTable("nowPlaying");
                DbxTable.QueryResult results = table.Query();
                var list = results.AsList();
                foreach (var record in list)
                {
                    record.DeleteRecord();
                }
                _store.Sync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void PushHello()
        {
            DbxFile testFile = null;
            try
            {
                var path = new DbxPath("hello.txt");
                bool fileExists = _fileSystem.Exists(path);
                if (fileExists)
                    testFile = _fileSystem.Open(path);
                else
                    testFile = _fileSystem.Create(path);

                testFile.WriteString(string.Format("Hello from Captain Obvious in {0} Land! I am on the spaceship {1} (code name: {2}) in the area {3}. Things are looking quite obvious to me! This message was sent on {4}.", _deviceSpecifications.GetDeviceType(), _deviceSpecifications.GetDeviceName(), _deviceSpecifications.GetDeviceUniqueId(), _deviceSpecifications.GetIPAddress(), DateTime.Now));
            }
            catch (Exception ex)
            {
                Tracing.Log("AndroidDropboxService - PushPlaylist - Exception: {0}", ex);
                throw;
            }
            finally
            {
                if (testFile != null)
                    testFile.Close();
            }
        }

        public string PushPlaylist(Playlist playlist)
        {
            DbxFile testFile = null;

            try
            {
                // Do we really need to check folder existence before each call?
                var path = new DbxPath(string.Format("/Playlists/{0}.json", playlist.PlaylistId));
                bool fileExists = _fileSystem.Exists(path);
                if(fileExists)
                    testFile = _fileSystem.Open(path);
                else
                    testFile = _fileSystem.Create(path);

                string json = JsonConvert.SerializeObject(playlist);
                testFile.WriteString(json);
                //testFile.WriteString(string.Format("Hello from Captain Obvious in {0} Land! I am on the spaceship {1} (code name: {2}) in the area {3}. Things are looking quite obvious to me! This message was sent on {4}.", _deviceSpecifications.GetDeviceType(), _deviceSpecifications.GetDeviceName(), _deviceSpecifications.GetDeviceUniqueId(), _deviceSpecifications.GetIPAddress(), DateTime.Now));
            }
            catch (Exception ex)
            {
                Tracing.Log("AndroidDropboxService - PushPlaylist - Exception: {0}", ex);
            }
            finally
            {
                if (testFile != null)
                    testFile.Close();
            }
            return string.Empty;
        }

        public Playlist PullPlaylist(Guid playlistId)
        {
            return null;
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

        public void PushStuff()
        {
            try
            {
                DbxTable tableStuff = _store.GetTable("stuff");
                DbxRecord stuff = tableStuff.Insert();
                stuff.Set("hello", "world");
                stuff.Set("deviceType", _deviceSpecifications.GetDeviceType().ToString());
                stuff.Set("deviceName", _deviceSpecifications.GetDeviceName());
                stuff.Set("ip", _deviceSpecifications.GetIPAddress());
                stuff.Set("test", true);
                stuff.Set("timestamp", DateTime.Now.ToLongTimeString());
                _store.Sync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string PullStuff()
        {
            string text = string.Empty;

            try
            {
                DbxTable tableStuff = _store.GetTable("stuff");
                DbxFields queryParams = new DbxFields();
                queryParams.Set("test", true);
                queryParams.Set("hello", "world");
                DbxTable.QueryResult results = tableStuff.Query(queryParams);
                var list = results.AsList();
                if (list.Count == 0)
                {
                    //_lblValue.Text = "No value!";
                    return text;
                }

                DbxRecord firstResult = list[0];
                string timestamp = firstResult.GetString("timestamp");
                string deviceType = firstResult.GetString("deviceType");
                string deviceName = firstResult.GetString("deviceName");
                text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
            }
            catch (Exception ex)
            {
                throw;
            }

            return text;
        }

        public void DeleteStuff()
        {
            try
            {
                DbxTable tableStuff = _store.GetTable("stuff");
                DbxTable.QueryResult results = tableStuff.Query();
                var list = results.AsList();
                foreach (var record in list)
                {
                    record.DeleteRecord();
                }
                _store.Sync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void OnDatastoreStatusChange(DbxDatastore store)
        {
            Console.WriteLine("SyncCloudActivity - OnDatastoreStatusChange - hasIncoming: {0}", store.SyncStatus.HasIncoming);
            //if (OnDropboxDataChanged != null) OnDropboxDataChanged(string.Format("Data changed: {0} incoming: {1}", DateTime.Now.ToLongTimeString(), store.SyncStatus.HasIncoming));
            if (store.SyncStatus.HasIncoming)
            {
                try
                {
                    var changes = store.Sync();
                    if (!changes.ContainsKey("stuff"))
                        return;

                    var records = changes["stuff"];
                    foreach (var record in records)
                    {
                        string timestamp = record.GetString("timestamp");
                        string deviceType = record.GetString("deviceType");
                        string deviceName = record.GetString("deviceName");
                        string text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
                        if (OnCloudDataChanged != null) OnCloudDataChanged(text);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SyncCloudActivity - OnDatastoreStatusChange exception: {0}", ex);
                    if (OnCloudDataChanged != null) OnCloudDataChanged(string.Format("Error: {0}", ex));
                    //throw;
                }
            }
        }

        public void OnLinkedAccountChange(DbxAccountManager accountManager, DbxAccount account)
        {
            Console.WriteLine("AndroidDropboxService - OnLinkedAccountChange");
            _account = account.IsLinked ? account : null;
            //_lblConnected.Text = string.Format("Is Linked: {0} {1}", _accountManager.HasLinkedAccount, DateTime.Now.ToLongTimeString());
        }

        public void OnSyncStatusChange(DbxFileSystem fileSystem)
        {
            Console.WriteLine("AndroidDropboxService - OnSyncStatusChange - anyInProgress: {0}", fileSystem.SyncStatus.AnyInProgress());
        }

        public void OnPathChange(DbxFileSystem fileSystem, DbxPath registeredPath, DbxFileSystem.PathListenerMode registeredMode)
        {
            Console.WriteLine("AndroidDropboxService - OnPathChange - path: {0}", registeredPath.Name);

            Task.Factory.StartNew(() =>
            {
                var fileInfos = fileSystem.ListFolder(registeredPath);
                foreach(var fileInfo in fileInfos)
                {
                    Console.WriteLine("AndroidDropboxService - OnPathChange - path: {0} size: {1} modifiedTime: {2} isFolder: {3}", fileInfo.Path.Name, fileInfo.Size, fileInfo.ModifiedTime, fileInfo.IsFolder);
                }
            });
        }

        public void OnFileChange(DbxFile file)
        {
            if (file == null)
                return;

            Console.WriteLine("AndroidDropboxService - OnFileChange {0}", file.Path.ToString());
        }

    }

    public class SerializablePlaylist
    {
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public List<Guid> AudioFiles { get; set; }

        public SerializablePlaylist()
        {
            AudioFiles = new List<Guid>();
        }
    }

    public class SerializableNowPlaying
    {
        public Guid AudioFileId { get; set; }
        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }
        public string SongTitle { get; set; }
        public string Position { get; set; }
        public long PositionBytes { get; set; }
        public string DeviceType { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string IPAddress { get; set; }
        public DateTime Timestamp { get; set; }   
    }
}