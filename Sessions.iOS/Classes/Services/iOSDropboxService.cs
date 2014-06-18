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
using MPfm.Core;
using MPfm.Library;
using MPfm.Library.Objects;
using MPfm.Library.Services.Exceptions;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace MPfm.iOS.Classes.Services
{
	public class iOSDropboxService : NSObject, ICloudService
	{
        // Of course, those are temp keys and will be replaced when pushing to the App Store.
        //private string _dropboxAppKey = "6tc6565743i743n";
        //private string _dropboxAppSecret = "fbkt3neevjjl0l2";
        private string _dropboxAppKey = "m1bcpax276elhfi";
        private string _dropboxAppSecret = "2azbuj2eelkranm";

		//private DBDatastore _store;
        private DBFilesystem _fileSystem;
		private List<DBFile> _watchedFiles;

        public event CloudAuthenticationStatusChanged OnCloudAuthenticationStatusChanged;
        public event CloudAuthenticationFailed OnCloudAuthenticationFailed;
		public event CloudFileDownloaded OnCloudFileDownloaded;
		public event CloudPathChanged OnCloudPathChanged;

        public bool HasLinkedAccount
        {
            get
            {
                return DBAccountManager.SharedManager.LinkedAccount != null;
            }
        }

        public iOSDropboxService()
        {
            Initialize();
        }

        private void Initialize()
        {
			_watchedFiles = new List<DBFile>();

            var manager = new DBAccountManager(_dropboxAppKey, _dropboxAppSecret);
            DBAccountManager.SharedManager = manager;

            // Check if user is logged in 
            var account = DBAccountManager.SharedManager.LinkedAccount;
            if (account == null)
                return;

//            if (_store != null && _store.Open)
//                return;
//
//            DBError error = null;
//            _store = DBDatastore.OpenDefaultStoreForAccount(account, out error);
//            if(error != null)
//                throw new Exception(error.Description);

            _fileSystem = new DBFilesystem(account);
            DBFilesystem.SharedFilesystem = _fileSystem;

//			_fileSystem.AddObserverForPathAndChildren(_fileSystem, new DBPath("/Devices"), () => {
//				Console.WriteLine("iOSDropboxService - FileSystem - Data changed!");
////                if(OnCloudDataChanged != null)
////                    OnCloudDataChanged(string.Empty);
//            });

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

        public static DateTime NSDateToDateTime(MonoTouch.Foundation.NSDate date)
        {
            return (new DateTime(2001,1,1,0,0,0)).AddSeconds(date.SecondsSinceReferenceDate);
        }

		public void CreateFolder(string path)
		{
			ThrowExceptionIfAppNotLinked();

			DBError error = null;
			var dbPath = new DBPath(path);
			_fileSystem.CreateFolder(dbPath, out error);
			if(error != null)
				throw new Exception(error.Description);
		}

		public bool FileExists(string path)
		{
			ThrowExceptionIfAppNotLinked();

			// Unlike the Android SDK, there is no method to check if a file exists... 
			DBError error = null;
			DBPath dbPath = new DBPath(path);
			var fileInfo = _fileSystem.FileInfoForPath(dbPath, out error);
			if(error != null)
				throw new Exception(error.Description);

			return fileInfo != null;
		}

		public List<string> ListFiles(string path, string extension)
		{
			ThrowExceptionIfAppNotLinked();

			DBError error = null;
			var fileInfos = _fileSystem.ListFolder(new DBPath("/Devices"), out error);
			if(error != null)
				throw new Exception(error.Description);

			var files = fileInfos.Select(x => x.Path.Name).ToList();
			return files;
		}

		public void WatchFolder(string path)
		{
			ThrowExceptionIfAppNotLinked();

			//Console.WriteLine("iOSDropboxService - Watching folder {0}", path);
			_fileSystem.AddObserverForPathAndChildren(this, new DBPath(path), () => {
				//Console.WriteLine("iOSDropboxService - FileSystem - Data changed on {0}", path);
				if(OnCloudPathChanged != null)
					OnCloudPathChanged(path);
			});
		}

		public void StopWatchFolder(string path)
		{
			ThrowExceptionIfAppNotLinked();

			//Console.WriteLine("iOSDropboxService - Removing watch on folder {0}", path);
			//_fileSystem.RemoveObserver(_fileSystem, new NSString(path));
			_fileSystem.RemoveObserver(this);
		}

		public void WatchFile(string path)
		{
			ThrowExceptionIfAppNotLinked();

			DBError error = null;
			var dbPath = new DBPath(path);
			var file = _fileSystem.OpenFile(dbPath, out error);
			if (error != null)
				throw new Exception(error.Description);
			if (file == null)
				return;

			file.AddObserver(this, () => {
				ProcessWatchedFile(file, path);
			});
			_watchedFiles.Add(file);

		}

		public void StopWatchFile(string path)
		{
		}

		public void DownloadFile(string path)
		{
			ThrowExceptionIfAppNotLinked();

			try
			{
				Task.Factory.StartNew(() =>
				{
					DBError error = null;
					DBFile file = null;
					var dbPath = new DBPath(path);

					file = _fileSystem.OpenFile(dbPath, out error);
					if (error != null)
						throw new Exception(error.Description);

						if(file.NewerStatus == null)
						{
							byte[] bytes = null;
							try
							{
								//Tracing.Log("iOSDropboxService - DownloadFile - File is already latest version; getting {0}", path);
								var data = file.ReadData(out error);
								if (error != null)
									throw new Exception(error.Description);

								bytes = new byte[data.Length];
								System.Runtime.InteropServices.Marshal.Copy(data.Bytes, bytes, 0, Convert.ToInt32(data.Length));
							}
							finally
							{
								if (file != null)
									file.Close();
							}

							if (OnCloudFileDownloaded != null)
								OnCloudFileDownloaded(path, bytes);
						}
						else
						{
							//Tracing.Log("iOSDropboxService - DownloadFile - File needs to be updated; adding observer to {0}", path);
							_watchedFiles.Add(file);
							file.AddObserver(this, () => {
								ProcessWatchedFile(file, path);
							});
						}
				});
			}
			catch(AggregateException ae)
			{
				ae.Handle((ex) =>
				{
					return false;
				});
			}
		}

		private void ProcessWatchedFile(DBFile file, string filePath)
		{
			//Tracing.Log("iOSDropboxService - DownloadFile - File changed - {0}", filePath);
			DBError error = null;
			DBFileStatus status = file.NewerStatus;

			// If file.NewerStatus is null, the file hasn't changed.
			if (status == null) return;

			if (status.Cached) 
			{
				//Tracing.Log("iOSDropboxService - DownloadFile - File changed - File is cached; updating {0}", filePath);
				file.Update(out error);
				if (error != null)
					throw new Exception(error.Description);

				byte[] bytes = null;
				try
				{
					var data = file.ReadData(out error);
					if (error != null)
						throw new Exception(error.Description);

					bytes = new byte[data.Length];
					System.Runtime.InteropServices.Marshal.Copy(data.Bytes, bytes, 0, Convert.ToInt32(data.Length));
				}
				finally
				{
					// Do we really need to close here? Or CloseAllFiles?
					if (file != null)
						file.Close();
				}

				if (OnCloudFileDownloaded != null)
					OnCloudFileDownloaded(filePath, bytes);
			} 
			else
			{
				// The file is still downloading
				//Tracing.Log("iOSDropboxService - DownloadFile - File changed - File is still downloading {0}", filePath);
			}
		}

		public void UploadFile(string path, byte[] data)
		{
			ThrowExceptionIfAppNotLinked();

			try
			{
				Task.Factory.StartNew(() =>
				{
					DBError error = null;
					DBFile file = null;
					var dbPath = new DBPath(path);

					try
					{
						// Unlike the Android SDK, there is no method to check if a file exists... 
						var fileInfo = _fileSystem.FileInfoForPath(dbPath, out error);
//						if (error != null) // Ignore error; API always returns 2002 error when the file doesn't exist
//							throw new Exception(error.Description);

						if (fileInfo == null)
							file = _fileSystem.CreateFile(dbPath, out error);
						else
							file = _fileSystem.OpenFile(dbPath, out error);
						if (error != null)
							throw new Exception(error.Description);

						NSData nsData = NSData.FromArray(data);
						file.WriteData(nsData, out error);
						if (error != null)
							throw new Exception(error.Description);
					}
					finally
					{
						if (file != null)
							file.Close();
					}
				});
			}
			catch(AggregateException ae)
			{
				ae.Handle((ex) =>
				{
					return false;
				});
			}

		}

		public void CloseAllFiles()
		{
			foreach (var file in _watchedFiles)
			{
				file.RemoveObserver(this);
				file.Close();                
			}
			_watchedFiles.Clear();
		}

		private void ThrowExceptionIfAppNotLinked()
		{
			if(_fileSystem == null)
				throw new CloudAppNotLinkedException();
		}

//        public IEnumerable<T> SyncFolder<T>(string path)
//        {
//            List<CloudDeviceInfo> devices = new List<CloudDeviceInfo>();
//            DBError error = null;
//            DBFile file = null;
//
//            try
//            {
//                if(_fileSystem == null)
//                    throw new CloudAppNotLinkedException();
//
//                Tracing.Log("iOSDropboxService - SyncFolder - ListFolder");
//                var fileInfos = _fileSystem.ListFolder(new DBPath(path), out error);
//                if(error != null)
//                    throw new Exception(error.Description);
//
//                foreach(var fileInfo in fileInfos)
//                {
//                    try
//                    {
//                        SyncFile(fileInfo.Path);
//                    }
//                    catch(Exception ex)
//                    {
//                        Tracing.Log("iOSDropboxService - SyncFolder - Failed to download file {0} - ex: {1}", fileInfo.Path.Name, ex);
//                    }
//                    finally
//                    {
//                        if(file != null)
//                            file.Close();                            
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Tracing.Log("iOSDropboxService - SyncFolder - Exception: {0}", ex);
//                throw;
//            }
//
//            return devices;
//        }
//
//        private void SyncFile(DBPath filePath)
//        {
//            DBError error = null;
//            DBFile file = null;
//
//            Tracing.Log("iOSDropboxService - SyncFile - OpenFile - fileInfo.Path: {0} - timestamp: {1}", fileInfo.Path, NSDateToDateTime(fileInfo.ModifiedTime));
//            file = _fileSystem.OpenFile(filePath, out error);
//            if(error != null)
//                throw new Exception(error.Description);
//
//            //                        file.Update(out error);
//            //                        if(error != null)
//            //                            throw new Exception(error.Description);
//
//            // Check if a newer file is available
//            if(file.NewerStatus != null)
//            {
//                file.AddObserver(this, () => {
//                    Tracing.Log("iOSDropboxService - PullDevicesInfos - ReadString - fileInfo.Path: {0} - timestamp: {1} - fileStatus.cached: {2} fileStatus.state: {3} fileStatus.progress: {4} fileStatus.error: {5} newerStatus: {6}", fileInfo.Path, NSDateToDateTime(fileInfo.ModifiedTime), file.Status.Cached, file.Status.State, file.Status.Progress, file.Status.Error != null, file.NewerStatus != null);
//                });
//            }
//
//            Tracing.Log("iOSDropboxService - SyncFile - ReadString - fileInfo.Path: {0} - timestamp: {1} - fileStatus.cached: {2} fileStatus.state: {3} fileStatus.progress: {4} fileStatus.error: {5} newerStatus: {6}", fileInfo.Path, NSDateToDateTime(fileInfo.ModifiedTime), file.Status.Cached, file.Status.State, file.Status.Progress, file.Status.Error != null, file.NewerStatus != null);
//            string json = file.ReadString(out error);
//            if(error != null)
//                throw new Exception(error.Description);
//
//            CloudDeviceInfo device = null;
//            try
//            {
//                Tracing.Log("iOSDropboxService - SyncFile - Deserialize - fileInfo.Path: {0} - timestamp: {1} - fileStatus.cached: {2} fileStatus.state: {3} fileStatus.progress: {4} fileStatus.error: {5} newerStatus: {6}", fileInfo.Path, NSDateToDateTime(fileInfo.ModifiedTime), file.Status.Cached, file.Status.State, file.Status.Progress, file.Status.Error != null, file.NewerStatus != null);
//                device = JsonConvert.DeserializeObject<CloudDeviceInfo>(json);
//                devices.Add(device);
//            }
//            catch(Exception ex)
//            {
//                Tracing.Log("iOSDropboxService - PullDeviceInfos - Failed to deserialize JSON for path {0} - ex: {1}", fileInfo.Path.Name, ex);
//            }
//        }

	}
}
