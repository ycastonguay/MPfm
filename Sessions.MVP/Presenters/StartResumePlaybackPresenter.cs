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

using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using System.Linq;
using System;
using Sessions.Library.Objects;
using Sessions.Library.Services.Interfaces;
using Sessions.Sound.AudioFiles;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Start Resume Playback view presenter.
	/// </summary>
    public class StartResumePlaybackPresenter : BasePresenter<IStartResumePlaybackView>, IStartResumePlaybackPresenter
	{
	    private readonly IPlayerService _playerService;
	    private readonly IAudioFileCacheService _audioFileCacheService;
        private CloudDeviceInfo _device;
        private AudioFile _audioFile;

	    public StartResumePlaybackPresenter(CloudDeviceInfo device, IAudioFileCacheService audioFileCacheService, IPlayerService playerService)
		{
	        _device = device;
            _audioFileCacheService = audioFileCacheService;
            _playerService = playerService;
		}

        public override void BindView(IStartResumePlaybackView view)
        {
            view.OnResumePlayback = ResumePlayback;
            base.BindView(view);
            RefreshDevice();
        }

	    private void RefreshDevice()
	    {
            _audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == _device.AudioFileId);
            if (_audioFile == null)
            {
                _audioFile = _audioFileCacheService.AudioFiles.FirstOrDefault(x => string.Compare(x.ArtistName, _device.ArtistName, true) == 0 &&
                    string.Compare(x.AlbumTitle, _device.AlbumTitle, true) == 0 &&
                    string.Compare(x.Title, _device.SongTitle, true) == 0);

            }
            View.RefreshCloudDeviceInfo(_device, _audioFile);
        }

        private void ResumePlayback()
        {
            var audioFiles = _audioFileCacheService.SelectAudioFiles(new LibraryQuery() {
                ArtistName = _device.ArtistName,
                AlbumTitle = _device.AlbumTitle
            });
            _playerService.Play(audioFiles, _audioFile != null ? _audioFile.FilePath : string.Empty, 0, true, false);
        }
	}
}
