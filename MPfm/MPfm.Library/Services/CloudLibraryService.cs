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

            Initialize();
        }

        private void Initialize()
        {
            _cloudService.OnCloudDataChanged += CloudServiceOnCloudDataChanged;

            PullDeviceInfos();            
        }

        private void CloudServiceOnCloudDataChanged(string path, string data)
        {
            // Check if pull device infos has ended.

            // Keep a list of requested device infos, and once this is empty, notify consumer?
        }

        public void InitializeAppFolder()
        {
            _cloudService.CreateFolder("/Devices");
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
            //List<CloudDeviceInfo> devices = new List<CloudDeviceInfo>();

            //if (!HasLinkedAccount)
            //    throw new CloudAppNotLinkedException();

            //var filePaths = _cloudService.ListFiles("/Devices");
            //foreach (var filePath in filePaths)
            //{
            //    try
            //    {
            //        _cloudService.WatchFile(filePath);

            //        var bytes = _cloudService.DownloadFile(filePath);
            //        string json = Encoding.UTF8.GetString(bytes);

            //        CloudDeviceInfo device = null;
            //        try
            //        {
            //            device = JsonConvert.DeserializeObject<CloudDeviceInfo>(json);
            //            devices.Add(device);
            //        }
            //        catch (Exception ex)
            //        {
            //            Tracing.Log("AndroidDropboxService - PullDeviceInfos - Failed to deserialize JSON for path {0} - ex: {1}", filePath, ex);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Tracing.Log("AndroidDropboxService - PullDeviceInfos - Failed to download file - ex: {0}", ex);
            //    }
            //}

            //return devices;
        }

        public IEnumerable<CloudDeviceInfo> GetDeviceInfos()
        {
            return _deviceInfos.ToList();
        }
    }
}

