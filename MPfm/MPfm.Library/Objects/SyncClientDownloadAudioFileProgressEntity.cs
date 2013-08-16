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

namespace MPfm.Library.Objects
{
    /// <summary>
    /// Data structure repesenting the progress downloading audio files using the Sync client.
    /// </summary>
    public class SyncClientDownloadAudioFileProgressEntity
    {
        public string Status { get; set; }
        public float PercentageDone { get; set; }
        public int FilesDownloaded { get; set; }
        public int TotalFiles { get; set; }
        public string DownloadFileName { get; set; }
        public string DownloadSpeed { get; set; }
        public float DownloadPercentageDone { get; set; }
        public long DownloadBytesReceived { get; set; }
        public long DownloadTotalBytesToReceive { get; set; }
        public int Errors { get; set; }
        public string Log { get; set; }

        public SyncClientDownloadAudioFileProgressEntity()
        {
            DownloadSpeed = "0kb/sec";
            DownloadFileName = string.Empty;
        }
    }
}
