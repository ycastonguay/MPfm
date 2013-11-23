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
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.Library.Services.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;

namespace MPfm.Library.Services
{
    public class CloudLibraryService : ICloudLibraryService
    {
        private readonly ICloudService _cloudService;
        private readonly ILibraryService _libraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly ISyncDeviceSpecifications _deviceSpecifications;
        private readonly List<CloudDeviceInfo> _deviceInfos;

        private List<string> _deviceInfosLeftToDownload;

        public bool HasLinkedAccount { get { return _cloudService.HasLinkedAccount; } }
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
        }

        private void CloudServiceOnCloudFileDownloaded(string path, byte[] data)
        {
            // Check if pull device infos has ended.
            // Keep a list of requested device infos, and once this is empty, notify consumer?

            _deviceInfosLeftToDownload.Remove(path);
            Tracing.Log("CloudLibraryService - CloudServiceOnCloudFileDownloaded - path: {0} deviceInfosLeftToDownload.Count: {1}", path, _deviceInfosLeftToDownload.Count);
            if (_deviceInfosLeftToDownload.Count == 0)
            {
                Tracing.Log(">>>>> CloudLibraryService - CloudServiceOnCloudFileDownloaded - DONE");
            }

            //string json = Encoding.UTF8.GetString(bytes);

            //CloudDeviceInfo device = null;
            //try
            //{
            //    device = JsonConvert.DeserializeObject<CloudDeviceInfo>(json);
            //    devices.Add(device);
            //}
            //catch (Exception ex)
            //{
            //    Tracing.Log("AndroidDropboxService - PullDeviceInfos - Failed to deserialize JSON for path {0} - ex: {1}", filePath, ex);
            //}
        }

        public void InitializeAppFolder()
        {
            if(!_cloudService.FileExists("/Devices"))
                _cloudService.CreateFolder("/Devices");
            if (!_cloudService.FileExists("/Playlists"))
                _cloudService.CreateFolder("/Playlists");
        }

        public void PushDeviceInfo(AudioFile audioFile, long positionBytes, string position)
        {
            if (!HasLinkedAccount)
                throw new CloudAppNotLinkedException();

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
        }

        public void PullDeviceInfos()
        {
            Tracing.Log("AndroidDropboxService - PullDeviceInfos");

            if (!HasLinkedAccount)
                throw new CloudAppNotLinkedException();

            Task.Factory.StartNew(() =>
            {
                const string folderPath = "/Devices";
                var filePaths = _cloudService.ListFiles(folderPath);
                _deviceInfosLeftToDownload = filePaths.Select(x => string.Format("{0}/{1}", folderPath, x)).ToList();
                foreach (var filePath in filePaths)
                {
                    try
                    {
                        Tracing.Log("AndroidDropboxService - PullDeviceInfos - filePath: {0}", filePath);
                        //_cloudService.WatchFile(filePath);
                        _cloudService.DownloadFile(string.Format("{0}/{1}", folderPath, filePath));
                    }
                    catch (Exception ex)
                    {
                        Tracing.Log("AndroidDropboxService - PullDeviceInfos - Failed to download file - ex: {0}", ex);
                    }
                }

                // What do we do about file statuses? 
                // on iOS + Android, we cannot simply redownload the file. On desktop, we can. In fact, on desktop, deltas are not managed the same way.

                //_deviceInfos.Clear();
                //_deviceInfos.AddRange(devices);
                //if (OnDeviceInfoUpdated != null)
                //    OnDeviceInfoUpdated(_deviceInfos);
            });
        }

        public IEnumerable<CloudDeviceInfo> GetDeviceInfos()
        {
            return _deviceInfos.ToList();
        }
    }
}

