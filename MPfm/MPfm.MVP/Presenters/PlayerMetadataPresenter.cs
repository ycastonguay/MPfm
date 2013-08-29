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

using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Player metadata view presenter.
	/// </summary>
	public class PlayerMetadataPresenter : BasePresenter<IPlayerMetadataView>, IPlayerMetadataPresenter
	{
        MobileNavigationManager _navigationManager;
        ITinyMessengerHub _messageHub;
        IPlayerService _playerService;
        bool _isShuffle;

        public PlayerMetadataPresenter(ITinyMessengerHub messageHub, MobileNavigationManager navigationManager, IPlayerService playerService)
		{
            _messageHub = messageHub;
            _navigationManager = navigationManager;
            _playerService = playerService;
        }

        public override void BindView(IPlayerMetadataView view)
        {
            base.BindView(view);

            view.OnOpenPlaylist = OpenPlaylist;
            view.OnToggleRepeat = ToggleRepeat;
            view.OnToggleShuffle = ToggleShuffle;
            _messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>(OnPlaylistIndexChanged);

            // Refresh initial data if player is already playing
            if (_playerService.IsPlaying)
            {
                View.RefreshAudioFile(_playerService.CurrentPlaylistItem.AudioFile);
                View.RefreshRepeat(_playerService.RepeatType);
                View.RefreshShuffle(_isShuffle);
            }
        }

        private void OnPlaylistIndexChanged(PlayerPlaylistIndexChangedMessage message)
        {
            View.RefreshAudioFile(message.Data.AudioFileStarted);
        }

        private void OpenPlaylist()
        {
            _navigationManager.CreatePlaylistView();            
        }

        private void ToggleRepeat()
        {
            _playerService.ToggleRepeatType();
            View.RefreshRepeat(_playerService.RepeatType);
        }

        private void ToggleShuffle()
        {
            // TODO: Actually implement shuffling
            _isShuffle = !_isShuffle;
            View.RefreshShuffle(_isShuffle);
        }
    }
}
