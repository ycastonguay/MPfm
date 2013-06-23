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

using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;
using MPfm.MVP.Messages;
using System;
using MPfm.MVP.Services.Interfaces;

namespace MPfm.MVP.Presenters
{
	public class PlaylistPresenter : BasePresenter<IPlaylistView>, IPlaylistPresenter
	{
        readonly ITinyMessengerHub _messageHub;
        readonly IPlayerService _playerService;

        public PlaylistPresenter(ITinyMessengerHub messageHub, IPlayerService playerService)
		{
            _messageHub = messageHub;
            _playerService = playerService;

            _messageHub.Subscribe<PlayerPlaylistUpdatedMessage>((PlayerPlaylistUpdatedMessage m) => {
                Console.WriteLine("PlaylistPresenter - PlayerPlaylistUpdated - Refreshing items...");
                RefreshItems();
            });
            _messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>(PlayerPlaylistIndexChanged);
		}
 
        public override void BindView(IPlaylistView view)
        {
            view.OnNewPlaylist = NewPlaylist;
            view.OnLoadPlaylist = LoadPlaylist;
            view.OnSavePlaylist = SavePlaylist;
            view.OnShufflePlaylist = ShufflePlaylist;
            view.OnChangePlaylistItemOrder = ChangePlaylistItemOrder;
            view.OnRemovePlaylistItem = RemovePlaylistItem;
            view.OnSelectPlaylistItem = SelectPlaylistItem;

            base.BindView(view);
            Initialize();
        }

        private void Initialize()
        {
            RefreshItems();

            if(_playerService.IsPlaying)
                View.RefreshCurrentlyPlayingSong(_playerService.CurrentPlaylist.CurrentItemIndex, _playerService.CurrentPlaylist.CurrentItem.AudioFile);
        }

        private void PlayerPlaylistIndexChanged(PlayerPlaylistIndexChangedMessage message)
        {
            View.RefreshCurrentlyPlayingSong(_playerService.CurrentPlaylist.CurrentItemIndex, _playerService.CurrentPlaylist.CurrentItem.AudioFile);
        }

        private void RefreshItems()
        {
            try
            {
                View.RefreshPlaylist(_playerService.CurrentPlaylist);
            }
            catch(Exception ex)
            {
                Console.WriteLine("PlaylistPresenter - RefreshItems - Exception: {0}", ex);
                View.PlaylistError(ex);
            }        
        }

        private void NewPlaylist()
        {
            try
            {
                if(_playerService.IsPlaying)
                    _playerService.Stop();

                _playerService.CurrentPlaylist.Clear();
            }
            catch(Exception ex)
            {
                Console.WriteLine("PlaylistPresenter - NewPlaylist - Exception: {0}", ex);
                View.PlaylistError(ex);
            }        
        }

        private void LoadPlaylist(string filePath)
        {
        }

        private void SavePlaylist()
        {
        }

        private void ShufflePlaylist()
        {
        }

        private void ChangePlaylistItemOrder(Guid playlistItemId, int newIndex)
        {
        }

        private void SelectPlaylistItem(Guid playlistItemId)
        {
            try
            {
                _playerService.GoTo(playlistItemId);
            }
            catch(Exception ex)
            {
                Console.WriteLine("PlaylistPresenter - SelectPlaylistItem - Exception: {0}", ex);
                View.PlaylistError(ex);
            }
        }

        private void RemovePlaylistItem(Guid playlistItemId)
        {
        }
	}
}
