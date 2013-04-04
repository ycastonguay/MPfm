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
	/// Player status view presenter.
	/// </summary>
	public class PlayerStatusPresenter : BasePresenter<IPlayerStatusView>, IPlayerStatusPresenter
	{
        ITinyMessengerHub _messageHub;
        IPlayerService _playerService;

        public PlayerStatusPresenter(ITinyMessengerHub messageHub, IPlayerService playerService)
		{
            this._playerService = playerService;
            this._messageHub = messageHub;
        }

        private void OnPlaylistIndexChanged(PlayerPlaylistIndexChangedMessage message)
        {
            //View.RefreshAudioFile(message.Data.AudioFileStarted);
        }

        public override void BindView(IPlayerMetadataView view)
        {            
            // Subscribe to view actions
            base.BindView(view);

            _messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>(OnPlaylistIndexChanged);
        }
    }
}

