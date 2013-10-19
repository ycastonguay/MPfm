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
using Android.App;
using Com.Dropbox.Sync.Android;
using MPfm.Library;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;

namespace MPfm.Android.Classes.Services
{
    public class AndroidDropboxService : Java.Lang.Object, IDropboxService, DbxDatastore.ISyncStatusListener, DbxAccountManager.IAccountListener
    {
        private readonly ILibraryService _libraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly ISyncDeviceSpecifications _deviceSpecifications;
        private DbxAccountManager _accountManager;
        private DbxAccount _account;
        private DbxDatastore _store;

        public event DropboxDataChanged OnDropboxDataChanged;

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
            string appKey = "6tc6565743i743n";
            string appSecret = "fbkt3neevjjl0l2";
            _accountManager = DbxAccountManager.GetInstance(MPfmApplication.GetApplicationContext(), appKey, appSecret);
            _accountManager.AddListener(this);

            if (_accountManager.HasLinkedAccount)
                _account = _accountManager.LinkedAccount;

            if (_account != null)
            {
                _store = DbxDatastore.OpenDefault(_account);
                _store.AddSyncStatusListener(this);
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
                    var activity = (Activity) view;
                    _accountManager.StartLink(activity, 0);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ContinueLinkApp()
        {
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
        }

        public string PushNowPlaying(AudioFile audioFile, long positionBytes, string position)
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
                        if (OnDropboxDataChanged != null) OnDropboxDataChanged(text);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SyncCloudActivity - OnDatastoreStatusChange exception: {0}", ex);
                    if (OnDropboxDataChanged != null) OnDropboxDataChanged(string.Format("Error: {0}", ex));
                    //throw;
                }
            }
        }

        public void OnLinkedAccountChange(DbxAccountManager accountManager, DbxAccount account)
        {
            Console.WriteLine("SyncCloudActivity - OnLinkedAccountChange");
            _account = account.IsLinked ? account : null;
            //_lblConnected.Text = string.Format("Is Linked: {0} {1}", _accountManager.HasLinkedAccount, DateTime.Now.ToLongTimeString());
        }
    }
}