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
using MPfm.Library;
using MPfm.Library.Objects;
using MPfm.MVP.Config;
using MPfm.MVP.Services.Interfaces;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using System.Collections.Generic;
using System.Linq;
using Sessions.Core;
using Sessions.Sound.AudioFiles;

namespace MPfm.MVP.Services
{	
	/// <summary>
    /// Service used for managing playback resume on multiple platforms.
	/// </summary>
    public class ResumePlaybackService : IResumePlaybackService
	{
        public CloudDeviceInfo GetResumePlaybackInfo()
        {
            CloudDeviceInfo resumeCloudDeviceInfo = null;
            var playerService = Bootstrapper.GetContainer().Resolve<IPlayerService>();
            var cloudLibraryService = Bootstrapper.GetContainer().Resolve<ICloudLibraryService>();
            var audioFileCacheService = Bootstrapper.GetContainer().Resolve<IAudioFileCacheService>();
            var syncDeviceSpecs = Bootstrapper.GetContainer().Resolve<ISyncDeviceSpecifications>();

            // Compare timestamps from cloud vs local
            var infos = new List<CloudDeviceInfo>();
            try
            {
                infos = cloudLibraryService.GetDeviceInfos().OrderByDescending(x => x.Timestamp).ToList();
            }
            catch (Exception ex)
            {
                // TODO: When an exception is thrown, it is not shown to the user because this is done in a delegate.
                // Not being able to connect to the cloud is a minor error; keep on starting the app as if there was no info for resuming                    
            }

            CloudDeviceInfo cloudDeviceInfo = null;
            AudioFile audioFileCloud = null;
            AudioFile audioFileLocal = null;
            string localDeviceName = syncDeviceSpecs.GetDeviceName();
            //DateTime localTimestamp = AppConfigManager.Instance.Root.ResumePlayback.Timestamp;
            DateTime localTimestamp = DateTime.MinValue; // Temporary for testing purposes
            foreach (var deviceInfo in infos)
            {
                // Make sure the timestamp is earlier than local, and that this isn't actually the same device!
                if (deviceInfo.Timestamp > localTimestamp && deviceInfo.DeviceName != localDeviceName)
                {
                    // Check if the file can be found in the database
                    Tracing.Log("MobileNavigationManager - ContinueAfterSplash - Cloud device {0} has earlier timestamp {1} compared to local timestamp {2}", deviceInfo.DeviceName, deviceInfo.Timestamp, localTimestamp);
                    audioFileCloud = null;
                    audioFileCloud = audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == deviceInfo.AudioFileId);
                    if (audioFileCloud == null)
                    {
                        audioFileCloud = audioFileCacheService.AudioFiles.FirstOrDefault(x => x.ArtistName.ToUpper() == deviceInfo.ArtistName.ToUpper() &&
                            x.AlbumTitle.ToUpper() == deviceInfo.AlbumTitle.ToUpper() &&
                            x.Title.ToUpper() == deviceInfo.SongTitle.ToUpper());
                    }
                    if (audioFileCloud != null)
                    {
                        cloudDeviceInfo = deviceInfo;
                        Tracing.Log("MobileNavigationManager - ContinueAfterSplash - Found audioFile {0}/{1}/{2} in database!", audioFileCloud.ArtistName, audioFileCloud.AlbumTitle, audioFileCloud.Title);
                        break;
                    }
                }
            }

            // Try to get info to resume locally
            double positionPercentage = 0;
            Guid audioFileId = Guid.Empty;
            if(!string.IsNullOrEmpty(AppConfigManager.Instance.Root.ResumePlayback.AudioFileId))
                audioFileId = new Guid(AppConfigManager.Instance.Root.ResumePlayback.AudioFileId);
            audioFileLocal = audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == audioFileId);
            positionPercentage = AppConfigManager.Instance.Root.ResumePlayback.PositionPercentage;
            Tracing.Log("MobileNavigationManager - ContinueAfterSplash - Resuming from local device with audioFile {0} at position {1}", audioFileId, positionPercentage);

            // Try to resume playback
            try
            {
                if (audioFileLocal != null || audioFileCloud != null)
                {
                    AudioFile audioFile = null;
                    //List<AudioFile> audioFiles = new List<AudioFile>();
                    if (audioFileLocal == null)
                    {
                        // We can only resume from the cloud!
                        audioFile = audioFileCloud;
                    }
                    else if (audioFileCloud == null)
                    {
                        // We can only resume from the local device!
                        audioFile = audioFileLocal;
                    }
                    else
                    {
                        // By default, resume from local device before showing dialog, so that when the user clicks cancel, the playlist is already loaded.
                        audioFile = audioFileLocal;

                        // We can resume from both devices; compare timestamps to determine if the dialog must be shown
                        if (cloudDeviceInfo.Timestamp > localTimestamp)
                        {
                            // A cloud device has a timestamp that is earlier than the local device.
                            // Keep a flag to show the Start Resume Playback view when the Player view will be created. Or else the view will be pushed too soon!
                            resumeCloudDeviceInfo = cloudDeviceInfo;
                        }
                    }

                    // Limit the value in case we try to skip beyond 100%
                    if (positionPercentage > 1)
                        positionPercentage = 0.99;

                    Tracing.Log("MobileNavigationManager - ContinueAfterSplash - Resume playback is available; showing Player view...");
                    var audioFiles = audioFileCacheService.AudioFiles.Where(x => x.ArtistName == audioFile.ArtistName && x.AlbumTitle == audioFile.AlbumTitle).ToList();
                    playerService.Play(audioFiles, audioFile.FilePath, positionPercentage*100, true, true);
                }
                else
                {
                    // No info to resume; skip this step and go to IMobileMainView                    
                }
            }
            catch(Exception ex)
            {
                // If we cannot resume playback, this will simply go to IMobileMainView
                Tracing.Log("MobileNavigationManager - Failed to resume playback: {0}", ex);
            }

            return resumeCloudDeviceInfo;
        }
    }
}
