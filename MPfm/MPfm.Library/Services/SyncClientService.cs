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
using MPfm.Library.Objects;

namespace MPfm.Library.Services
{
    public class SyncClientService : ISyncClientService
    {
        readonly ILibraryService _libraryService;
        readonly IAudioFileCacheService _audioFileCacheService;

        WebClientTimeout _webClient;
        int _filesDownloaded = 0;
        int _errorCount = 0;
        string _baseUrl = string.Empty;
        List<AudioFile> _audioFiles = new List<AudioFile>();

        public delegate void DownloadIndexProgress(int progressPercentage, long bytesReceived, long totalBytesToReceive);
        public delegate void DownloadAudioFileStatus(SyncClientDownloadAudioFileProgressEntity entity);
        public event EventHandler OnReceivedIndex;
        public event DownloadIndexProgress OnDownloadIndexProgress;
        public event DownloadAudioFileStatus OnDownloadAudioFileStarted;
        public event DownloadAudioFileStatus OnDownloadAudioFileProgress;
        public event DownloadAudioFileStatus OnDownloadAudioFileCompleted;

        public SyncClientService(ILibraryService libraryService, IAudioFileCacheService audioFileCacheService)
        {
            _libraryService = libraryService;
            _audioFileCacheService = audioFileCacheService;
            Initialize();
        }

        private void Initialize()
        {
            _webClient = new WebClientTimeout(3000);
            _webClient.DownloadProgressChanged += HandleDownloadProgressChanged;
            _webClient.DownloadStringCompleted += HandleDownloadStringCompleted;
            _webClient.DownloadFileCompleted += HandleDownloadFileCompleted;
        }

        private void HandleDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //Console.WriteLine("SyncClientService - HandleDownloadProgressChanged - progressPercentage: {0} bytesReceived: {1} totalBytesToReceive: {2}", e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);

            string fileName = string.Empty;
            if(_audioFiles != null && _audioFiles.Count >= _filesDownloaded+1)
                fileName = Path.GetFileName(_audioFiles[_filesDownloaded].FilePath);

            if (OnDownloadIndexProgress != null)
                OnDownloadIndexProgress(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);

            if (OnDownloadAudioFileProgress != null)
                OnDownloadAudioFileProgress(new SyncClientDownloadAudioFileProgressEntity(){
                    PercentageDone = ((float)_filesDownloaded / (float)_audioFiles.Count()) * 100f, 
                    FilesDownloaded = _filesDownloaded, 
                    TotalFiles = _audioFiles.Count(),
                    DownloadBytesReceived = e.BytesReceived,
                    DownloadTotalBytesToReceive = e.TotalBytesToReceive,
                    DownloadPercentageDone = ((float)e.BytesReceived / (float)e.TotalBytesToReceive) * 100f,
                    Errors = _errorCount, 
                    DownloadFileName = fileName,
                    Log = string.Empty
                });
        }

        private void HandleDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            if (e.Error != null)
            {
                Console.WriteLine("SyncClientService - HandleDownloadStringCompleted - Error: {0}", e.Error);
                return;
            }

            string url = e.UserState.ToString();
            Console.WriteLine("SyncClientService - Finished downloading index from {0}.", url);
            _audioFiles = XmlSerialization.Deserialize<List<AudioFile>>(e.Result);

            if (OnReceivedIndex != null)
                OnReceivedIndex(this, new EventArgs());
        }

        private void HandleDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            if (e.Error != null)
            {
                Console.WriteLine("SyncClientService - HandleDownloadFileCompleted - Error: {0}", e.Error);
                _errorCount++;
            }
            else
            {
                // TODO: Check if the ID is already in the database! If yes, that means the insert statement will fail.
                Console.WriteLine("SyncClientService - HandleDownloadFileCompleted - File downloaded; inserting audio file into database...");
                var audioFile = _audioFiles[_filesDownloaded];
                string fileName = Path.GetFileName(audioFile.FilePath);
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string localFilePath = Path.Combine(folderPath, fileName);
                audioFile.FilePath = localFilePath;
                _libraryService.InsertAudioFile(audioFile);
            }

            _filesDownloaded++;

            if (OnDownloadAudioFileCompleted != null)
                OnDownloadAudioFileCompleted(new SyncClientDownloadAudioFileProgressEntity(){
                    PercentageDone = ((float)_filesDownloaded / (float)_audioFiles.Count()) * 100f, 
                    FilesDownloaded = _filesDownloaded, 
                    TotalFiles = _audioFiles.Count(), 
                    Errors = _errorCount, 
                    Log = string.Empty
                });

            // Download the next file
            if(_filesDownloaded < _audioFiles.Count)
            {
                Console.WriteLine("SyncClientService - HandleDownloadFileCompleted - Downloading next file {0}...", _audioFiles[_filesDownloaded].FilePath);
                DownloadAudioFile(_audioFiles[_filesDownloaded]);
                return;
            }

            // Process is over; refresh cache
            Console.WriteLine("SyncClientService - HandleDownloadFileCompleted - Process is over, refreshing audio file cache...");
            _audioFileCacheService.RefreshCache();
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

            _filesDownloaded = 0;
            _errorCount = 0;
            _baseUrl = baseUrl;
            _audioFiles = audioFiles.ToList();

            if (OnDownloadAudioFileProgress != null)
                OnDownloadAudioFileProgress(new SyncClientDownloadAudioFileProgressEntity(){
                    PercentageDone = 0,
                    FilesDownloaded = 0,
                    TotalFiles = _audioFiles.Count(), 
                    Errors = _errorCount, 
                    Log = string.Empty
                });

            var audioFile = _audioFiles[0];
            DownloadAudioFile(audioFile);
        }

        private void DownloadAudioFile(AudioFile audioFile)
        {
            var url = new Uri(new Uri(_baseUrl), string.Format("/api/audiofile/{0}", audioFile.Id.ToString()));
            string fileName = Path.GetFileName(audioFile.FilePath);
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string localFilePath = Path.Combine(folderPath, fileName);

            if (OnDownloadAudioFileStarted != null)
                OnDownloadAudioFileStarted(new SyncClientDownloadAudioFileProgressEntity(){
                    PercentageDone = ((float)_filesDownloaded / (float)_audioFiles.Count()) * 100f, 
                    FilesDownloaded = _filesDownloaded, 
                    DownloadFileName = fileName, 
                    TotalFiles = _audioFiles.Count(), 
                    Errors = _errorCount, 
                    Log = string.Empty
                });

            Console.WriteLine("SyncClientService - DownloadAudioFile - url: {0} fileName: {1} localFilePath: {2}", url.ToString(), fileName, localFilePath);
            _webClient.DownloadFileAsync(url, localFilePath);
        }
    }
}
