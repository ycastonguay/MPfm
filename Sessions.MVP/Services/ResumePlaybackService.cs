// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Sessions.Core;
using Sessions.Library;
using Sessions.Library.Objects;
using Sessions.Library.Services.Interfaces;
using Sessions.MVP.Config;
using Sessions.MVP.Services.Interfaces;
using Sessions.Sound.AudioFiles;
using System.IO;
using Sessions.MVP.Models;

namespace Sessions.MVP.Services
{	
	/// <summary>
    /// Service used for managing playback resume on multiple platforms.
	/// </summary>
    public class ResumePlaybackService : IResumePlaybackService
	{
        private readonly IPlayerService _playerService;
        private readonly ICloudLibraryService _cloudLibraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly ISyncDeviceSpecifications _syncDeviceSpecifications;

        public ResumePlaybackService(IPlayerService playerService, ICloudLibraryService cloudLibraryService, 
                                     IAudioFileCacheService audioFileCacheService, ISyncDeviceSpecifications syncDeviceSpecifications)
        {
            _playerService = playerService;
            _cloudLibraryService = cloudLibraryService;
            _audioFileCacheService = audioFileCacheService;
            _syncDeviceSpecifications = syncDeviceSpecifications;
        }

        private IEnumerable<CloudDeviceInfo> GetDeviceInfosOrderedByTimestamp()
        {
            var infos = _cloudLibraryService.GetDeviceInfos();
            if (infos != null)
            {
                var orderedInfos = infos.OrderByDescending(x => x.Timestamp).ToList();
                return orderedInfos;
            }

            return infos;
        }

        public ResumePlaybackInfoCloud FindResumableCloudDevice()
        {
            CloudDeviceInfo cloudDeviceInfo = null;
            AudioFile audioFileCloud = null;
            string localDeviceName = _syncDeviceSpecifications.GetDeviceName();

            var infos = GetDeviceInfosOrderedByTimestamp();
            if(infos == null)
                return null;

            foreach (var deviceInfo in infos)
            {
                // Make sure we aren't trying to resume from the local device
                if (deviceInfo.DeviceName != localDeviceName)
                {
                    // Check if the file can be found in the database; try to find it by its identifier first
                    audioFileCloud = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == deviceInfo.AudioFileId);
                    if (audioFileCloud == null)
                    {
                        // Try to find it by its metadata (a little less likely to work)
                        audioFileCloud = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.ArtistName.ToUpper() == deviceInfo.ArtistName.ToUpper() &&
                        x.AlbumTitle.ToUpper() == deviceInfo.AlbumTitle.ToUpper() &&
                        x.Title.ToUpper() == deviceInfo.SongTitle.ToUpper());
                    }
                    if (audioFileCloud != null)
                    {
                        // We found the album on this device!
                        return new ResumePlaybackInfoCloud() { DeviceInfo = deviceInfo, AudioFile = audioFileCloud };
                    }
                }
            }

            return null;
        }

        private AudioFile GetAudioFileFromLocalResumeInfo()
        {
            AudioFile audioFileLocal = null;
            if (!string.IsNullOrEmpty(AppConfigManager.Instance.Root.ResumePlayback.AudioFileId))
            {
                var audioFileId = new Guid(AppConfigManager.Instance.Root.ResumePlayback.AudioFileId);
                audioFileLocal = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == audioFileId);
                double positionPercentage = AppConfigManager.Instance.Root.ResumePlayback.PositionPercentage;
                Tracing.Log("ResumePlaybackService - GetAudioFileFromLocalResumeInfo - Got audioFile {0} at position {1}", audioFileId, positionPercentage);
            }
            return audioFileLocal;
        }

        //private ResumePlaybackSourceType SelectResumePlaybackSource(DateTime cloudTimestamp, AudioFile audioFileCloud, DateTime localTimestamp, AudioFile audioFileLocal)
        private ResumePlaybackSourceType SelectResumePlaybackSource(ResumePlaybackInfo info)
        {
            if (info.Local.AudioFile == null && info.Cloud != null)
            {
                return ResumePlaybackSourceType.Cloud;
            } 
            else if (info.Cloud == null && info.Local.AudioFile != null)
            {
                return ResumePlaybackSourceType.Local;
            } 
            else if (info.Cloud != null && info.Local.AudioFile != null)
            {
                if (info.Local.Timestamp > info.Cloud.DeviceInfo.Timestamp)
                {
                    // If the time stamp from the local device is more recent than the cloud timestamp, we select the local device automatically
                    return ResumePlaybackSourceType.Local;
                }
                else
                {
                    // Display a dialog for the user to select between local or cloud
                    return ResumePlaybackSourceType.LocalOrCloud;
                }
            }

            return ResumePlaybackSourceType.None;
        }

        public ResumePlaybackInfo TryToResumePlaybackFromLocalOrCloud()
        {
            AudioFile audioFileToResumeFrom = null;
            var info = new ResumePlaybackInfo();

            // Extract information for resuming playback from a previous local session
            info.Local = new ResumePlaybackInfoLocal();
            info.Local.Timestamp = DateTime.MinValue; // Force older timestamp for testing purposes // AppConfigManager.Instance.Root.ResumePlayback.Timestamp;
            info.Local.AudioFile = GetAudioFileFromLocalResumeInfo();

            // Extract information for resuming playback from another device connected to the cloud
            info.Cloud = FindResumableCloudDevice();

            // Call a method that will help us select the source from which we should resume the playback
            info.Source = SelectResumePlaybackSource(info);
            switch (info.Source)
            {
                case ResumePlaybackSourceType.Local:
                case ResumePlaybackSourceType.LocalOrCloud:
                    // By default, we take the local resume, but we will display a popup and let the user choose between the two.
                    audioFileToResumeFrom = info.Local.AudioFile;
                    break;
                case ResumePlaybackSourceType.Cloud:
                    audioFileToResumeFrom = info.Cloud.AudioFile;
                    break;
            }

            if (audioFileToResumeFrom == null)
                return info;

            // Check if the files we want to play still exist (TODO: Shouldn't this be done in PlayerService instead?)
            Tracing.Log("ResumePlaybackService - TryToResumePlaybackFromLocalOrCloud - Resuming playback from {0}...", info.Source);
            var audioFilesToResumeFrom = _audioFileCacheService.AudioFiles.Where(x => x.ArtistName == audioFileToResumeFrom.ArtistName && x.AlbumTitle == audioFileToResumeFrom.AlbumTitle).ToList();
            var audioFilesToPlay = new List<AudioFile>();
            foreach (var audioFile in audioFilesToResumeFrom)
            {
                if (File.Exists(audioFile.FilePath))
                    audioFilesToPlay.Add(audioFile);
            }

            if (audioFilesToPlay.Count > 0)
            {
                // Start playback (paused); only the position information is available from a local resume
                double positionPercentage = info.Source == ResumePlaybackSourceType.Local ? Math.Min(1, AppConfigManager.Instance.Root.ResumePlayback.PositionPercentage) : 0;
                _playerService.Play(audioFilesToResumeFrom, audioFileToResumeFrom.FilePath, positionPercentage * 100, true, true);
            }

            return info;
        }
    }
}
