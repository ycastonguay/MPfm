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
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Edit Song Metadata view presenter.
	/// </summary>
	public class EditSongMetadataPresenter : BasePresenter<IEditSongMetadataView>, IEditSongMetadataPresenter
	{
	    readonly IPlayerService _playerService;
	    AudioFile _audioFile;

	    public EditSongMetadataPresenter(IPlayerService playerService)
	    {
	        _playerService = playerService;
	    }

        public override void BindView(IEditSongMetadataView view)
        {            
            base.BindView(view);

            view.OnSaveAudioFile = SaveAudioFile;
        }

	    public void SetAudioFile(AudioFile audioFile)
	    {
	        _audioFile = audioFile;
            View.RefreshAudioFile(_audioFile);
	    }

        private void SaveAudioFile(AudioFile audioFile)
        {
            try
            {
                if (_playerService.IsPlaying &&
                    _playerService.CurrentPlaylistItem.AudioFile.FilePath == audioFile.FilePath)
                {
                    // TODO: Stop player and resume when editing the currently playing file
                }
                audioFile.SaveMetadata();
            }
            catch (Exception ex)
            {
                View.EditSongMetadataError(ex);
            }
        }
	}
}
