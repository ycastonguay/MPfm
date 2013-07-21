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
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Sync download view presenter.
	/// </summary>
    public class SyncDownloadPresenter : BasePresenter<ISyncDownloadView>, ISyncDownloadPresenter
	{
        readonly ISyncClientService _syncClientService;
        List<AudioFile> _audioFiles = new List<AudioFile>();
        SyncDevice _device;
        Timer _timerUpdateProgress;
        SyncClientDownloadAudioFileProgressEntity _currentProgress;

	    public SyncDownloadPresenter(ISyncClientService syncClientService)
		{
            _syncClientService = syncClientService;
            _syncClientService.OnDownloadAudioFileStarted += HandleOnDownloadAudioFileStarted;
            _syncClientService.OnDownloadAudioFileProgress += HandleOnDownloadAudioFileProgress;
            _syncClientService.OnDownloadAudioFileCompleted += HandleOnDownloadAudioFileCompleted;
            _syncClientService.OnDownloadAudioFilesCompleted += HandleOnDownloadAudioFilesCompleted;

            // Limit how progress is reported to the UI, Android gets very slow when trying to update text views frequently
            _timerUpdateProgress = new Timer(200);
            _timerUpdateProgress.Elapsed += TimerUpdateProgressOnElapsed;
		}

        public override void BindView(ISyncDownloadView view)
        {
            view.OnCancelDownload = CancelDownload;
            base.BindView(view);

            Initialize();
        }       

        private void Initialize()
        {

        }

        private void TimerUpdateProgressOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_currentProgress == null)
                return;

            View.RefreshStatus(_currentProgress);
        }
                
        private void HandleOnDownloadAudioFileStarted(SyncClientDownloadAudioFileProgressEntity entity)
        {
            _currentProgress = entity;
        }

        private void HandleOnDownloadAudioFileProgress(SyncClientDownloadAudioFileProgressEntity entity)
        {
            _currentProgress = entity;
        }

        private void HandleOnDownloadAudioFileCompleted(SyncClientDownloadAudioFileProgressEntity entity)
        {
            _currentProgress = entity;
        }

        private void HandleOnDownloadAudioFilesCompleted(object sender, EventArgs e)
        {
            View.SyncCompleted();
            _timerUpdateProgress.Stop();
        }

        private void CancelDownload()
        {
            try
            {
                _syncClientService.Cancel();
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncDownloadPresenter - ButtonPressed - Exception: {0}", ex);
            }
        }

        public void StartSync(SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            try
            {
                _device = device;
                _audioFiles = audioFiles.ToList();
                Console.WriteLine("SyncDownloadPresenter - StartSync - url: {0} audioFiles.Count: {1}", _device.Url, _audioFiles.Count);
                View.RefreshDevice(_device);
                _timerUpdateProgress.Start();

                Task.Factory.StartNew(() => {
                    _syncClientService.DownloadAudioFiles(_device.Url, audioFiles);
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncDownloadPresenter - StartSync - Exception: {0}", ex);
            }
        }
    }
}
