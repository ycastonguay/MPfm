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

using System.Linq;
using Sessions.Library.Objects;
using Sessions.Library.Services.Interfaces;
using Sessions.MVP.Models;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
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
        private ResumePlaybackInfo _resumePlaybackInfo;
        private AudioFile _audioFile;

        public StartResumePlaybackPresenter(ResumePlaybackInfo resumePlaybackInfo, IAudioFileCacheService audioFileCacheService, IPlayerService playerService)
		{
            _resumePlaybackInfo = resumePlaybackInfo;
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
            View.RefreshCloudDeviceInfo(_resumePlaybackInfo);
        }

        private void ResumePlayback()
        {
            var deviceInfo = _resumePlaybackInfo.Cloud.DeviceInfo;
            var audioFiles = _audioFileCacheService.SelectAudioFiles(new LibraryQuery() {
                ArtistName = deviceInfo.ArtistName,
                AlbumTitle = deviceInfo.AlbumTitle
            });
            _playerService.Play(audioFiles, _audioFile != null ? _audioFile.FilePath : string.Empty, 0, true, false);
        }
	}
}
