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
using System.Net;
using MPfm.Core.Network;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using MPfm.Core;
using System.Linq;
using System.IO;

namespace MPfm.Library.Services
{
    public class SyncClientService : ISyncClientService
    {
        WebClientTimeout _webClient;
        List<AudioFile> _audioFiles = new List<AudioFile>();

        public delegate void DownloadIndexProgress(int progressPercentage, long bytesReceived, long totalBytesToReceive);
        public delegate void DownloadAudioFilesProgress(float percentageDone, int filesDownloaded, int totalFiles, int errors, string log);
        public event EventHandler OnReceivedIndex;
        public event DownloadIndexProgress OnDownloadIndexProgress;
        public event DownloadAudioFilesProgress OnDownloadAudioFilesProgress;

        public SyncClientService()
        {
            Initialize();
        }

        private void Initialize()
        {
            _webClient = new WebClientTimeout(3000);
            _webClient.DownloadProgressChanged += HandleDownloadProgressChanged;
            _webClient.DownloadStringCompleted += HandleDownloadStringCompleted;
        }

        private void HandleDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (OnDownloadIndexProgress != null)
                OnDownloadIndexProgress(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
        }

        private void HandleDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            if (e.Error != null)
            {
                Console.WriteLine("SyncClientService - Error on DownloadStringAsync: {0}", e.Error);
                return;
            }

            string url = e.UserState.ToString();
            Console.WriteLine("SyncClientService - Finished downloading index from {0}.", url);
            _audioFiles = XmlSerialization.Deserialize<List<AudioFile>>(e.Result);

            if (OnReceivedIndex != null)
                OnReceivedIndex(this, new EventArgs());
        }

        public void Cancel()
        {
            Console.WriteLine("SyncClientService - Cancelling...");
            if (_webClient.IsBusy)
                _webClient.CancelAsync();
        }

        public void DownloadIndex(string baseUrl)
        {
            var url = new Uri(new Uri(baseUrl), "/api/index/xml");
            Cancel();

            Console.WriteLine("SyncClientService - Downloading index from {0}...", url);
            _webClient.DownloadStringAsync(url, url);
        }

        public List<string> GetDistinctArtistNames()
        {
            return _audioFiles.Select(x => x.ArtistName).Distinct().ToList();
        }

        public List<string> GetDistinctAlbumTitles(string artistName)
        {
            return _audioFiles.Where(x => x.ArtistName.ToUpper() == artistName.ToUpper()).Select(x => x.AlbumTitle).Distinct().ToList();
        }

        public List<AudioFile> GetAudioFiles(string artistName)
        {
            return _audioFiles.Where(x => x.ArtistName.ToUpper() == artistName.ToUpper()).ToList();
        }

        public List<AudioFile> GetAudioFiles(string artistName, string albumTitle)
        {
            return _audioFiles.Where(x => x.ArtistName.ToUpper() == artistName.ToUpper() && x.AlbumTitle.ToUpper() == albumTitle.ToUpper()).ToList();
        }

        public void DownloadAudioFiles(string baseUrl, IEnumerable<AudioFile> audioFiles)
        {
            if (_webClient.IsBusy)
                return;

            if (OnDownloadAudioFilesProgress != null)
                OnDownloadAudioFilesProgress(0, 0, audioFiles.Count(), 0, string.Empty);

            foreach (var audioFile in audioFiles)
            {
                var url = new Uri(new Uri(baseUrl), string.Format("/api/audiofile/{0}", audioFile.Id.ToString()));
                string fileName = Path.GetFileName(audioFile.FilePath);
                Console.WriteLine("SyncClientService - DownloadAudioFiles - url: {0} fileName: {1}", url.ToString(), fileName);
                //_webClient.DownloadFileAsync(url, fileName);
            }
        }
    }
}
