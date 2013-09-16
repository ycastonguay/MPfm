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
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Player status view presenter.
	/// </summary>
	public class PlayerStatusPresenter : BasePresenter<IPlayerStatusView>, IPlayerStatusPresenter
	{
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly ITinyMessengerHub _messageHub;
        readonly IPlayerService _playerService;
	    private readonly ILibraryService _libraryService;

	    public PlayerStatusPresenter(ITinyMessengerHub messageHub, MobileNavigationManager mobileNavigationManager, IPlayerService playerService, ILibraryService libraryService)
        {
            _messageHub = messageHub;
            _mobileNavigationManager = mobileNavigationManager;
            _playerService = playerService;
            _libraryService = libraryService;
        }

        public override void BindView(IPlayerStatusView view)
        {            
            // Subscribe to view actions
            base.BindView(view);

            view.OnPlayerPlayPause = PlayerPlayPause;
            view.OnPlayerPrevious = PlayerPrevious;
            view.OnPlayerNext = PlayerNext;
            view.OnPlayerRepeat = PlayerRepeat;
            view.OnPlayerShuffle = PlayerShuffle;
            view.OnOpenPlaylist = OpenPlaylist;

            _messageHub.Subscribe<PlayerStatusMessage>(message => View.RefreshPlayerStatus(message.Status));
            _messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>(message => View.RefreshAudioFile(message.Data.AudioFileStarted));
            _messageHub.Subscribe<PlayerPlaylistUpdatedMessage>(message =>
            {
                // Refresh current song as the first song in the playlist if the player was never started
                if (_playerService.Status == PlayerStatusType.Initialized && _playerService.CurrentPlaylist.Items.Count > 0)
                    View.RefreshAudioFile(_playerService.CurrentPlaylist.Items[0].AudioFile);

                View.RefreshPlaylist(_playerService.CurrentPlaylist);
            });
            _messageHub.Subscribe<PlaylistUpdatedMessage>(message =>
            {
                var playlist = _libraryService.SelectPlaylist(message.PlaylistId);
                View.RefreshPlaylist(playlist);
                RefreshPlaylists(playlist.PlaylistId);
            });
            _messageHub.Subscribe<PlaylistListUpdatedMessage>(message => RefreshPlaylists(Guid.Empty));

            if (!_playerService.IsInitialized)
                return;

            // Refresh initial data
            View.RefreshPlayerStatus(_playerService.Status);
            View.RefreshPlaylist(_playerService.CurrentPlaylist);
            RefreshPlaylists(Guid.Empty);
            if (!_playerService.IsPlaying || _playerService.CurrentPlaylistItem == null)
                View.RefreshAudioFile(null);
            else                
                View.RefreshAudioFile(_playerService.CurrentPlaylistItem.AudioFile);            
        }

	    private void RefreshPlaylists(Guid selectedPlaylistId)
	    {
            var playlists = _libraryService.SelectPlaylists();
            var items = playlists.Select(playlist => new PlaylistEntity()
            {
                PlaylistId = playlist.PlaylistId,
                Name = playlist.Name,
                LastModified = playlist.LastModified
            }).ToList();

	        View.RefreshPlaylists(items, selectedPlaylistId);
	    }

        private void OpenPlaylist()
	    {
            _mobileNavigationManager.CreatePlaylistView(View);
	    }

	    private void PlayerPlayPause()
	    {
            if(_playerService.Status == PlayerStatusType.Initialized)
                _playerService.Play();
            else
	            _playerService.Pause();
	    }

        private void PlayerPrevious()
        {
            _playerService.Previous();
        }

        private void PlayerNext()
	    {
            _playerService.Next();
	    }
 
        private void PlayerShuffle()
        {
            
        }

        private void PlayerRepeat()
        {
        }

	}
}
