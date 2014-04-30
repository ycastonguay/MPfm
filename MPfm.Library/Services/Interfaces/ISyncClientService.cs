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
using MPfm.Library.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.Library.Services.Interfaces
{
    public delegate void ReceivedIndex(Exception ex);
    public delegate void DownloadIndexProgress(int progressPercentage, long bytesReceived, long totalBytesToReceive);
    public delegate void DownloadAudioFileStatus(SyncClientDownloadAudioFileProgressEntity entity);

    /// <summary>
    /// Interface for the SyncClientService class.
    /// </summary>
    public interface ISyncClientService
    {
        event ReceivedIndex OnReceivedIndex;
        event DownloadIndexProgress OnDownloadIndexProgress;
        event DownloadAudioFileStatus OnDownloadAudioFileStarted;
        event DownloadAudioFileStatus OnDownloadAudioFileProgress;
        event DownloadAudioFileStatus OnDownloadAudioFileCompleted;
        event EventHandler OnDownloadAudioFilesCompleted;
		SyncDevice CurrentDevice { get; }

        void Cancel();
		void DownloadIndex(SyncDevice device);
		void DownloadAudioFiles(SyncDevice device, IEnumerable<AudioFile> audioFiles);
        List<string> GetDistinctArtistNames();
        List<string> GetDistinctAlbumTitles(string artistName);
        List<AudioFile> GetAudioFiles();
        List<AudioFile> GetAudioFiles(string artistName);
        List<AudioFile> GetAudioFiles(string artistName, string albumTitle);
    }
}