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

#if !WINDOWSSTORE && !WINDOWS_PHONE

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
using System.Diagnostics;
using System.ComponentModel;

namespace MPfm.Library.Services
{
    public class SyncClientService : ISyncClientService
    {
        readonly ILibraryService _libraryService;
        readonly IAudioFileCacheService _audioFileCacheService;
        readonly ISyncDeviceSpecifications _deviceSpecifications;

        Stopwatch _stopwatch;
        WebClientTimeout _webClient;
        long _bytesDownloaded = 0;
        int _filesDownloaded = 0;
        int _errorCount = 0;
        string _baseUrl = string.Empty;
        List<AudioFile> _audioFiles = new List<AudioFile>();
        
        long _lastBytesReceived = 0;

        public event ReceivedIndex OnReceivedIndex;
        public event DownloadIndexProgress OnDownloadIndexProgress;
        public event DownloadAudioFileStatus OnDownloadAudioFileStarted;
        public event DownloadAudioFileStatus OnDownloadAudioFileProgress;
        public event DownloadAudioFileStatus OnDownloadAudioFileCompleted;
        public event EventHandler OnDownloadAudioFilesCompleted;

        public SyncClientService(ILibraryService libraryService, IAudioFileCacheService audioFileCacheService, ISyncDeviceSpecifications deviceSpecifications)
        {
            _libraryService = libraryService;
            _audioFileCacheService = audioFileCacheService;
            _deviceSpecifications = deviceSpecifications;
            Initialize();
        }

        private void Initialize()
        {
            _stopwatch = new Stopwatch();
            _webClient = new WebClientTimeout(3000);
            _webClient.DownloadProgressChanged += HandleDownloadProgressChanged;
            _webClient.DownloadStringCompleted += HandleDownloadStringCompleted;
            _webClient.DownloadFileCompleted += HandleDownloadFileCompleted;
        }

        private string GetDownloadSpeed()
        {
            float speed = ((float)_bytesDownloaded / 1000) / ((float)_stopwatch.ElapsedMilliseconds / 1000);
            return string.Format("{0:0}kb/s", speed);
        }

        private void HandleDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //Console.WriteLine("SyncClientService - HandleDownloadProgressChanged - progressPercentage: {0} bytesReceived: {1} totalBytesToReceive: {2}", e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);

            string fileName = string.Empty;
            if(_audioFiles != null && _audioFiles.Count >= _filesDownloaded+1)
                fileName = Path.GetFileName(_audioFiles[_filesDownloaded].FilePath);

            _bytesDownloaded += e.BytesReceived - _lastBytesReceived;
            _lastBytesReceived = e.BytesReceived;

            if (OnDownloadIndexProgress != null)
                OnDownloadIndexProgress(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);

            if (OnDownloadAudioFileProgress != null)
                OnDownloadAudioFileProgress(new SyncClientDownloadAudioFileProgressEntity(){
                    Status = "Downloading files...",
                    PercentageDone = ((float)_filesDownloaded / (float)_audioFiles.Count()) * 100f, 
                    FilesDownloaded = _filesDownloaded, 
                    TotalFiles = _audioFiles.Count(),
                    DownloadBytesReceived = e.BytesReceived,
                    DownloadTotalBytesToReceive = e.TotalBytesToReceive,
                    DownloadPercentageDone = ((float)e.BytesReceived / (float)e.TotalBytesToReceive) * 100f,
                    DownloadSpeed = GetDownloadSpeed(),
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
                if (OnReceivedIndex != null)
                    OnReceivedIndex(e.Error);
                return;
            }

            string url = e.UserState.ToString();
            Console.WriteLine("SyncClientService - Finished downloading index. Deserializing XML...", url);
            _audioFiles = XmlSerialization.Deserialize<List<AudioFile>>(e.Result);
            Console.WriteLine("SyncClientService - XML deserialized!");
            if (OnReceivedIndex != null)
                OnReceivedIndex(null);
        }

        private void HandleDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled) return;

            _stopwatch.Stop();

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
                string localFilePath = GetLibraryLocalPath(audioFile);
                audioFile.FilePath = localFilePath;
                _libraryService.InsertAudioFile(audioFile);
            }

            _filesDownloaded++;
            var entity = new SyncClientDownloadAudioFileProgressEntity() {
                Status = "Downloading files...",
                PercentageDone = ((float)_filesDownloaded / (float)_audioFiles.Count()) * 100f, 
                FilesDownloaded = _filesDownloaded, 
                TotalFiles = _audioFiles.Count(), 
                DownloadSpeed = GetDownloadSpeed(),
                Errors = _errorCount, 
                Log = string.Empty
            };

            // Download the next file
            if(_filesDownloaded < _audioFiles.Count)
            {
                Console.WriteLine("SyncClientService - HandleDownloadFileCompleted - Downloading next file {0}...", _audioFiles[_filesDownloaded].FilePath);
                if (OnDownloadAudioFileCompleted != null) OnDownloadAudioFileCompleted(entity);
                DownloadAudioFile(_audioFiles[_filesDownloaded]);
                return;
            }

            // Process is over; refresh cache
            Console.WriteLine("SyncClientService - HandleDownloadFileCompleted - Process is over, refreshing audio file cache...");
            entity.Status = "Refreshing cache...";
            if (OnDownloadAudioFileCompleted != null) OnDownloadAudioFileCompleted(entity);
            _audioFileCacheService.RefreshCache();

            if (OnDownloadAudioFilesCompleted != null)
                OnDownloadAudioFilesCompleted(this, new EventArgs());
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

        public List<AudioFile> GetAudioFiles()
        {
            return _audioFiles;
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
            _bytesDownloaded = 0;
            _errorCount = 0;
            _baseUrl = baseUrl;
            _audioFiles = audioFiles.ToList();

            if (OnDownloadAudioFileProgress != null)
                OnDownloadAudioFileProgress(new SyncClientDownloadAudioFileProgressEntity(){
                    Status = "Downloading files...",
                    PercentageDone = 0,
                    FilesDownloaded = 0,
                    TotalFiles = _audioFiles.Count(), 
                    DownloadSpeed = GetDownloadSpeed(),
                    Errors = _errorCount, 
                    Log = string.Empty
                });

            var audioFile = _audioFiles[0];
            _stopwatch.Reset();
            DownloadAudioFile(audioFile);
        }

        private void DownloadAudioFile(AudioFile audioFile)
        {
            var url = new Uri(new Uri(_baseUrl), string.Format("/api/audiofile/{0}", audioFile.Id.ToString()));
            string localFilePath = GetLibraryLocalPath(audioFile);
            //Console.WriteLine("SyncClientService - DownloadAudioFile - folderPath: {0} fileName: {1} localFilePath: {2}", folderPath, fileName, localFilePath);
            _lastBytesReceived = 0;

            if (OnDownloadAudioFileStarted != null)
                OnDownloadAudioFileStarted(new SyncClientDownloadAudioFileProgressEntity(){
                    Status = "Downloading files...",
                    PercentageDone = ((float)_filesDownloaded / (float)_audioFiles.Count()) * 100f, 
                    FilesDownloaded = _filesDownloaded, 
                    DownloadFileName = Path.GetFileName(audioFile.FilePath),
                    TotalFiles = _audioFiles.Count(), 
                    DownloadSpeed = GetDownloadSpeed(),
                    Errors = _errorCount, 
                    Log = string.Empty
                });

            _stopwatch.Start();
            _webClient.DownloadFileAsync(url, localFilePath);
        }

        private string GetLibraryLocalPath(AudioFile audioFile)
        {
            string fileName = Path.GetFileName(audioFile.FilePath);            
            string folderPath = _deviceSpecifications.GetMusicFolderPath();

            // Add artist name to the path (create folder if necessary)
            folderPath = Path.Combine(folderPath, audioFile.ArtistName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Add album title to the path (create folder if necessary)
            folderPath = Path.Combine(folderPath, audioFile.AlbumTitle);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return Path.Combine(folderPath, fileName);
        }
    }
}
#endif