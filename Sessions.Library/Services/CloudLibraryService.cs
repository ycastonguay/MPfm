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

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPfm.Library.Objects;
using MPfm.Library.Services.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using Sessions.Core;

namespace MPfm.Library.Services
{
    public class CloudLibraryService : ICloudLibraryService
    {
        private readonly object _locker = new object();
        private readonly ICloudService _cloudService;
        private readonly ILibraryService _libraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly ISyncDeviceSpecifications _deviceSpecifications;
        private readonly List<CloudDeviceInfo> _deviceInfos;

        private List<string> _deviceInfosLeftToDownload;

        public bool HasLinkedAccount { get { return _cloudService.HasLinkedAccount; } }
        
        public event DeviceInfosDownloadProgress OnDeviceInfosDownloadProgress;
        public event DeviceInfosAvailable OnDeviceInfosAvailable;
        public event DeviceInfoUpdated OnDeviceInfoUpdated;

        public CloudLibraryService(ICloudService cloudService, ILibraryService libraryService, IAudioFileCacheService audioFileCacheService,
            ISyncDeviceSpecifications deviceSpecifications)
        {
            _cloudService = cloudService;
            _libraryService = libraryService;
            _audioFileCacheService = audioFileCacheService;
            _deviceSpecifications = deviceSpecifications;

            _deviceInfos = new List<CloudDeviceInfo>();
            _deviceInfosLeftToDownload = new List<string>();

            Initialize();
        }

        private void Initialize()
        {
            _cloudService.OnCloudFileDownloaded += CloudServiceOnCloudFileDownloaded;
			_cloudService.OnCloudPathChanged += CloudServiceOnCloudPathChanged;
        }

		private void CloudServiceOnCloudPathChanged(string path)
        {
			// Unfortunately this only tells us something in the folder has changed, not the exact file
			//Tracing.Log("CloudLibraryService - CloudServiceOnCloudPathChanged - path: {0}", path);
			if (path.Contains("Devices")) // No slash on Android
				PullDeviceInfos();
        }

        private void CloudServiceOnCloudFileDownloaded(string path, byte[] data)
        {
            lock (_locker)
            {
				CloudDeviceInfo device = null;
                try
                {
					//Tracing.Log("CloudLibraryService - CloudServiceOnCloudFileDownloaded - path: {0}", path);
                    string json = Encoding.UTF8.GetString(data);
                    device = JsonConvert.DeserializeObject<CloudDeviceInfo>(json);
					//Tracing.Log("CloudLibraryService - CloudServiceOnCloudFileDownloaded - path: {0} device: {1} artist: {2} song: {3}", path, device.DeviceName, device.ArtistName, device.SongTitle);

                    // Try to update the list instead of adding/removing item
                    int itemIndex = _deviceInfos.FindIndex(x => x.DeviceId == device.DeviceId);
                    if (itemIndex == -1)
                        _deviceInfos.Add(device);
                    else
                        _deviceInfos[itemIndex] = device;
                }
                catch (Exception ex)
                {
					Tracing.Log("CloudLibraryService - Failed to deserialize JSON for path {0} - ex: {1}", path, ex);
                }

                if (OnDeviceInfosDownloadProgress != null)
                    OnDeviceInfosDownloadProgress((float)_deviceInfos.Count/(float)(_deviceInfos.Count + _deviceInfosLeftToDownload.Count));

                // Check if list is already empty; do not raise OnDeviceInfosAvailable multiple times
				if (_deviceInfosLeftToDownload.Count == 0)
				{
					if (OnDeviceInfoUpdated != null && device != null)
						OnDeviceInfoUpdated(device);
					return;
				}

                _deviceInfosLeftToDownload.Remove(path);
				//Tracing.Log("CloudLibraryService - CloudServiceOnCloudFileDownloaded - path: {0} deviceInfosLeftToDownload.Count: {1} deviceInfos.Count: {2}", path, _deviceInfosLeftToDownload.Count, _deviceInfos.Count);
                if (_deviceInfosLeftToDownload.Count == 0)
                {
					Tracing.Log("CloudLibraryService - CloudServiceOnCloudFileDownloaded - Finished downloading files!");
                    _cloudService.CloseAllFiles();
                    if (OnDeviceInfosAvailable != null)
                        OnDeviceInfosAvailable(_deviceInfos);
                }
            }
        }

        public void InitializeAppFolder()
        {
            Task.Factory.StartNew(() =>
            {
                if (!_cloudService.FileExists("/Devices"))
                    _cloudService.CreateFolder("/Devices");
                if (!_cloudService.FileExists("/Playlists"))
                    _cloudService.CreateFolder("/Playlists");
            });
        }

        public void PushDeviceInfo(AudioFile audioFile, long positionBytes, string position)
        {            
            if (!HasLinkedAccount)
                throw new CloudAppNotLinkedException();

            Task.Factory.StartNew(() =>
            {
                var device = new CloudDeviceInfo()
                {
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

                string json = JsonConvert.SerializeObject(device);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                _cloudService.UploadFile(string.Format("/Devices/{0}.json", device.DeviceId), bytes);
            });
        }

        public void PullDeviceInfos()
        {
			//Tracing.Log("CloudLibraryService - PullDeviceInfos");

            if (!HasLinkedAccount)
                throw new CloudAppNotLinkedException();

            Task.Factory.StartNew(() =>
            {
                const string folderPath = "/Devices";
                var filePaths = _cloudService.ListFiles(folderPath, ".json");
                if (filePaths.Count == 0)
                    return;

                _deviceInfosLeftToDownload = filePaths[0].Contains("/") ? filePaths.ToList() : filePaths.Select(x => string.Format("{0}/{1}", folderPath, x)).ToList();
                foreach (var filePath in filePaths)
                {
                    try
                    {
                        // On iOS and Android, the filePath is relative; on desktop devices it is absolute.
						//Tracing.Log("CloudLibraryService - PullDeviceInfos - filePath: {0}", filePath);
                        string downloadFilePath = filePath;
                        if (!filePath.Contains("/"))
                            downloadFilePath = string.Format("{0}/{1}", folderPath, filePath);

                        _cloudService.DownloadFile(downloadFilePath);
                    }
                    catch (Exception ex)
                    {
						Tracing.Log("CloudLibraryService - PullDeviceInfos - Failed to download file - ex: {0}", ex);
                    }
                }
            });
        }

		public void WatchDeviceInfos()
		{
			_cloudService.WatchFolder("/Devices");

//			Task.Factory.StartNew(() =>
//			{
//				const string folderPath = "/Devices";
//				var filePaths = _cloudService.ListFiles(folderPath, ".json");
//				if (filePaths.Count == 0)
//					return;
//
//				foreach(var filePath in filePaths)
//				{
//					string downloadFilePath = filePath;
//					if (!filePath.Contains("/"))
//						downloadFilePath = string.Format("{0}/{1}", folderPath, filePath);
//
//					_cloudService.WatchFile(downloadFilePath);
//				}
//			});
		}

		public void StopWatchingDeviceInfos()
		{
			_cloudService.CloseAllFiles();
			_cloudService.StopWatchFolder("/Devices");
		}

        public IEnumerable<CloudDeviceInfo> GetDeviceInfos()
        {
            return _deviceInfos.ToList();
        }
    }
}

