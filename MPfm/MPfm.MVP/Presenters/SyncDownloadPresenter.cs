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
using MPfm.Library.Objects;
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.MVP.Models;
using MPfm.Library;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Sync download view presenter.
	/// </summary>
    public class SyncDownloadPresenter : BasePresenter<ISyncDownloadView>, ISyncDownloadPresenter
	{
        readonly ISyncClientService _syncClientService;
        string _url;
        List<AudioFile> _audioFiles = new List<AudioFile>();

        public SyncDownloadPresenter(ISyncClientService syncClientService)
		{
            _syncClientService = syncClientService;
            _syncClientService.OnDownloadAudioFileStarted += HandleOnDownloadAudioFileStarted;
            _syncClientService.OnDownloadAudioFileProgress += HandleOnDownloadAudioFileProgress;
            _syncClientService.OnDownloadAudioFileCompleted += HandleOnDownloadAudioFileCompleted;
		}

        public override void BindView(ISyncDownloadView view)
        {
            view.OnButtonPressed = ButtonPressed;
            base.BindView(view);

            Initialize();
        }       

        private void Initialize()
        {

        }
                
        private void HandleOnDownloadAudioFileStarted(SyncClientDownloadAudioFileProgressEntity entity)
        {
            View.RefreshStatus(entity);
        }

        private void HandleOnDownloadAudioFileProgress(SyncClientDownloadAudioFileProgressEntity entity)
        {
            View.RefreshStatus(entity);
        }

        private void HandleOnDownloadAudioFileCompleted(SyncClientDownloadAudioFileProgressEntity entity)
        {
            View.RefreshStatus(entity);
        }

        private void ButtonPressed()
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

        public void StartSync(string url, IEnumerable<AudioFile> audioFiles)
        {
            try
            {
                _url = url;
                _audioFiles = audioFiles.ToList();
                _syncClientService.DownloadAudioFiles(url, audioFiles);
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncDownloadPresenter - SetAudioFiles - Exception: {0}", ex);
            }
        }
    }
}
