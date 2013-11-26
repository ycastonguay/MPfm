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
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Webkit;
using Android.Widget;
using Com.Dropbox.Sync.Android;
using Java.IO;
using MPfm.Core;
using MPfm.Library.Services.Exceptions;
using MPfm.Library.Services.Interfaces;
using Console = System.Console;

namespace MPfm.Android.Classes.Services
{
    public class AndroidDropboxService : Java.Lang.Object, ICloudService, DbxDatastore.ISyncStatusListener, DbxAccountManager.IAccountListener, DbxFileSystem.IPathListener, DbxFileSystem.ISyncStatusListener, DbxFile.IListener
    {
        private DbxAccountManager _accountManager;
        private DbxAccount _account;
        //private DbxDatastore _store;
        private DbxFileSystem _fileSystem;
        private List<DbxFile> _watchedFiles;

        public event CloudAuthenticationStatusChanged OnCloudAuthenticationStatusChanged;
        public event CloudAuthenticationFailed OnCloudAuthenticationFailed;
        public event CloudFileDownloaded OnCloudFileDownloaded;
        public event CloudPathChanged OnCloudPathChanged;

        public bool HasLinkedAccount
        {
            get
            {
                return _accountManager != null && _accountManager.HasLinkedAccount;
            }
        }

        public AndroidDropboxService()
        {
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
                _watchedFiles = new List<DbxFile>();

                _accountManager = DbxAccountManager.GetInstance(MPfmApplication.GetApplicationContext(), appKey, appSecret);
                _accountManager.AddListener(this);

                if (_accountManager.HasLinkedAccount)
                    _account = _accountManager.LinkedAccount;

                if (_account != null)
                {
                    //_store = DbxDatastore.OpenDefault(_account);
                    //_store.AddSyncStatusListener(this);

                    _fileSystem = DbxFileSystem.ForAccount(_account);
                    _fileSystem.AddPathListener(this, new DbxPath("/"), DbxFileSystem.PathListenerMode.PathOrDescendant);
                    _fileSystem.AddSyncStatusListener(this);
                    //InitializeAppFolder();
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

            //if (_store != null)
            //{
            //    _store.RemoveSyncStatusListener(this);
            //    _store.Close();
            //}

            if (_fileSystem != null)
            {
                _fileSystem.RemovePathListenerForAll(this);
                _fileSystem.RemoveSyncStatusListener(this);
            }
        }

        public void LinkApp(object view)
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

        public void ContinueLinkApp()
        {
            if (OnCloudAuthenticationStatusChanged != null)
                OnCloudAuthenticationStatusChanged(CloudAuthenticationStatusType.ConnectedToDropbox);
        }

        public void UnlinkApp()
        {
            if (_accountManager.HasLinkedAccount)
                _accountManager.Unlink();
        }

        private void ThrowExceptionIfAppNotLinked()
        {
            if (_fileSystem == null)
                throw new CloudAppNotLinkedException();
        }

        public void CreateFolder(string path)
        {
            ThrowExceptionIfAppNotLinked();

            if(!_fileSystem.Exists(new DbxPath(path)))
                _fileSystem.CreateFolder(new DbxPath(path));
        }

        public bool FileExists(string path)
        {
            return _fileSystem.Exists(new DbxPath(path));
        }

        public List<string> ListFiles(string path, string extension)
        {
            ThrowExceptionIfAppNotLinked();

            var fileInfos = _fileSystem.ListFolder(new DbxPath(path));
            var files = fileInfos.Select(fileInfo => fileInfo.Path.Name).ToList();
            return files;
        }

        public void WatchFolder(string path)
        {
            ThrowExceptionIfAppNotLinked();

            _fileSystem.AddPathListener(this, new DbxPath(path), DbxFileSystem.PathListenerMode.PathOrChild);
        }

        public void StopWatchFolder(string path)
        {
            ThrowExceptionIfAppNotLinked();
            
            // TODO: Use path instead
            _fileSystem.RemovePathListenerForAll(this);
        }

        public void WatchFile(string path)
        {
            ThrowExceptionIfAppNotLinked();

            DbxFile file = _fileSystem.Open(new DbxPath(path));
            file.AddListener(this);
            _watchedFiles.Add(file);
        }

        public void StopWatchFile(string path)
        {
            ThrowExceptionIfAppNotLinked();

            var file = _watchedFiles.FirstOrDefault(x => x.Path.Name == path);
            if (file != null)
            {
                file.Close();
                _watchedFiles.Remove(file);
            }
        }

        public void CloseAllFiles()
        {
            ThrowExceptionIfAppNotLinked();

            foreach (var file in _watchedFiles)
            {
                file.RemoveListener(this);
                file.Close();                
            }
            _watchedFiles.Clear();
        }

        public void DownloadFile(string path)
        {
            ThrowExceptionIfAppNotLinked();

            DbxFile file = _fileSystem.Open(new DbxPath(path));
            Tracing.Log("AndroidDropboxService - DownloadFile - path: {0} isCached: {1} isLatest: {2}", path, file.SyncStatus.IsCached, file.SyncStatus.IsLatest);
            if (file.NewerStatus == null)
            {
                byte[] bytes = ReadFully(file.ReadStream);
                file.Close();

                if (OnCloudFileDownloaded != null)
                    OnCloudFileDownloaded(path, bytes);
            }
            else
            {
                Tracing.Log("AndroidDropboxService - DownloadFile - NewerStatus; adding listener - path: {0}", path);
                file.AddListener(this);
                _watchedFiles.Add(file);
                
                Tracing.Log("AndroidDropboxService - DownloadFile - NewerStatus; *AFTER* adding listener - path: {0} isLatest: {1}", path, file.SyncStatus.IsLatest);
            }            
        }

        public void UploadFile(string path, byte[] data)
        {
            ThrowExceptionIfAppNotLinked();

            DbxFile file = null;
            try
            {
                var dbxPath = new DbxPath(path);
                bool fileExists = _fileSystem.Exists(dbxPath);
                file = fileExists ? _fileSystem.Open(dbxPath) : _fileSystem.Create(dbxPath);
                file.WriteStream.Write(data, 0, data.Length);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        //private string PushNowPlaying_Datastore(AudioFile audioFile, long positionBytes, string position)
        //{
        //    try
        //    {
        //        DbxTable tableNowPlaying = _store.GetTable("nowPlaying");
        //        DbxFields queryParams = new DbxFields();
        //        queryParams.Set("deviceId", _deviceSpecifications.GetDeviceUniqueId());
        //        DbxTable.QueryResult results = tableNowPlaying.Query(queryParams);
        //        var list = results.AsList();

        //        // Edit existing item or insert new item
        //        DbxRecord record = list.Count > 0 ? list[0] : tableNowPlaying.Insert();
        //        if (record != null)
        //        {
        //            record.Set("audioFileId", audioFile.Id.ToString());
        //            record.Set("artistName", audioFile.ArtistName);
        //            record.Set("albumTitle", audioFile.AlbumTitle);
        //            record.Set("title", audioFile.Title);
        //            record.Set("position", "0:00.000");
        //            record.Set("positionBytes", 0);
        //            record.Set("deviceType", _deviceSpecifications.GetDeviceType().ToString());
        //            record.Set("deviceName", _deviceSpecifications.GetDeviceName());
        //            record.Set("deviceId", _deviceSpecifications.GetDeviceUniqueId());
        //            record.Set("ip", _deviceSpecifications.GetIPAddress());
        //            record.Set("timestamp", new Java.Util.Date());

        //            _store.Sync();
        //            return record.Id;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //    return string.Empty;
        //}

        //public string PullNowPlaying()
        //{
        //    string text = string.Empty;

        //    try
        //    {
        //        //DbxTable tableStuff = _store.GetTable("stuff");
        //        //DbxFields queryParams = new DbxFields();
        //        //queryParams.Set("test", true);
        //        //queryParams.Set("hello", "world");
        //        //DbxTable.QueryResult results = tableStuff.Query(queryParams);
        //        //var list = results.AsList();
        //        //if (list.Count == 0)
        //        //{
        //        //    //_lblValue.Text = "No value!";
        //        //    return text;
        //        //}

        //        //DbxRecord firstResult = list[0];
        //        //string timestamp = firstResult.GetString("timestamp");
        //        //string deviceType = firstResult.GetString("deviceType");
        //        //string deviceName = firstResult.GetString("deviceName");
        //        //text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //    return text;
        //}

        //public void DeleteNowPlaying()
        //{
        //    try
        //    {
        //        DbxTable table = _store.GetTable("nowPlaying");
        //        DbxTable.QueryResult results = table.Query();
        //        var list = results.AsList();
        //        foreach (var record in list)
        //        {
        //            record.DeleteRecord();
        //        }
        //        _store.Sync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public void OnDatastoreStatusChange(DbxDatastore store)
        {
        //    Tracing.Log("SyncCloudActivity - OnDatastoreStatusChange - hasIncoming: {0}", store.SyncStatus.HasIncoming);
        //    //if (OnDropboxDataChanged != null) OnDropboxDataChanged(string.Format("Data changed: {0} incoming: {1}", DateTime.Now.ToLongTimeString(), store.SyncStatus.HasIncoming));
        //    if (store.SyncStatus.HasIncoming)
        //    {
        //        try
        //        {
        //            var changes = store.Sync();
        //            if (!changes.ContainsKey("stuff"))
        //                return;

        //            var records = changes["stuff"];
        //            foreach (var record in records)
        //            {
        //                string timestamp = record.GetString("timestamp");
        //                string deviceType = record.GetString("deviceType");
        //                string deviceName = record.GetString("deviceName");
        //                string text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
        //                //if (OnCloudDataChanged != null) OnCloudDataChanged(text);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Tracing.Log("SyncCloudActivity - OnDatastoreStatusChange exception: {0}", ex);
        //            if (OnCloudDataChanged != null) OnCloudDataChanged(string.Format("Error: {0}", ex));
        //            //throw;
        //        }
        //    }
        }

        public void OnLinkedAccountChange(DbxAccountManager accountManager, DbxAccount account)
        {
            Tracing.Log("AndroidDropboxService - OnLinkedAccountChange");
            _account = account.IsLinked ? account : null;
        }

        public void OnSyncStatusChange(DbxFileSystem fileSystem)
        {
            Tracing.Log("AndroidDropboxService - OnSyncStatusChange - anyInProgress: {0}", fileSystem.SyncStatus.AnyInProgress());
        }

        public void OnPathChange(DbxFileSystem fileSystem, DbxPath registeredPath, DbxFileSystem.PathListenerMode registeredMode)
        {
            Tracing.Log("AndroidDropboxService - OnPathChange - path: {0}", registeredPath.Name);

            if (OnCloudPathChanged != null)
                OnCloudPathChanged(registeredPath.Name);

            //Task.Factory.StartNew(() =>
            //{
            //    var fileInfos = fileSystem.ListFolder(registeredPath);
            //    foreach(var fileInfo in fileInfos)
            //    {
            //        Tracing.Log("AndroidDropboxService - OnPathChange - path: {0} size: {1} modifiedTime: {2} isFolder: {3}", fileInfo.Path.Name, fileInfo.Size, fileInfo.ModifiedTime, fileInfo.IsFolder);
            //    }
            //});
        }

        public void OnFileChange(DbxFile file)
        {
            Tracing.Log("AndroidDropboxService - OnFileChange");
            if (file == null)
                return;

            Task.Factory.StartNew(() =>
            {
                ProcessWatchedFile(file);
            });
        }

        private void ProcessWatchedFile(DbxFile file)
        {
            try
            {
                Tracing.Log("AndroidDropboxService - OnFileChange - path: {0} syncStatus: {1} bytesTransfered: {2} bytesTotal: {3} isCached: {4} isLatest: {5}", file.Path.Name, file.SyncStatus.Pending, file.SyncStatus.BytesTransferred, file.SyncStatus.BytesTotal, file.SyncStatus.IsCached, file.SyncStatus.IsLatest);
                if (file.NewerStatus != null)
                {
                    Tracing.Log("AndroidDropboxService - OnFileChange - path: {0} newerStatus: {1} bytesTransfered: {2} bytesTotal: {3} isCached: {4} isLatest: {5}", file.Path.Name, file.NewerStatus.Pending, file.NewerStatus.BytesTransferred, file.NewerStatus.BytesTotal, file.NewerStatus.IsCached, file.NewerStatus.IsLatest);
                    if (file.NewerStatus.IsLatest)
                    {
                        Tracing.Log("AndroidDropboxService - OnFileChange - Finished downloading! Updating file...");
                        file.Update();

                        // TODO: Maybe add a lock so ReadFully isn't called twice for the same file
                        byte[] bytes = ReadFully(file.ReadStream);

                        Tracing.Log("AndroidDropboxService - OnFileChange - Finished downloading! (2)");
                        if (OnCloudFileDownloaded != null)
                        {
                            Tracing.Log("AndroidDropboxService - OnFileChange - Finished downloading! (3)");
                            OnCloudFileDownloaded(string.Format("/Devices/{0}", file.Path.Name), bytes);
                            Tracing.Log("AndroidDropboxService - OnFileChange - Finished downloading! (4)");
                        }
                        Tracing.Log("AndroidDropboxService - OnFileChange - Finished downloading! (5)");
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore file close exceptions
                Tracing.Log("AndroidDropboxService - OnFileChanged - Exception: {0}", ex);
            }
        }
    }
}