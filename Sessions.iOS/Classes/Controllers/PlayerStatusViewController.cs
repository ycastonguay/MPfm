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

using System;
using System.Collections.Generic;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;
using Sessions.iOS.Classes.Controllers.Base;
using org.sessionsapp.player;
using Sessions.Sound.Player;

namespace Sessions.iOS
{
	public partial class PlayerStatusViewController : BaseViewController, IPlayerStatusView
    {
        public PlayerStatusViewController()
			: base(UserInterfaceIdiomIsPhone ? "PlayerStatusViewController_iPhone" : "PlayerStatusViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
			navigationManager.BindPlayerStatusView(this);
        }

		#region IPlayerStatusView implementation

		public Action OnPlayerPlayPause { get; set; }
		public Action OnPlayerPrevious { get; set; }
		public Action OnPlayerNext { get; set; }
		public Action OnPlayerShuffle { get; set; }
		public Action OnPlayerRepeat { get; set; }
		public Action OnOpenPlaylist { get; set; }

		public void RefreshPlayerState(SSPPlayerState state)
		{
		}

		public void RefreshAudioFile(AudioFile audioFile)
		{
		}

		public void RefreshPlaylist(SSPPlaylist playlist)
		{
		}

		public void RefreshPlaylists(List<PlaylistEntity> playlists, Guid selectedPlaylistId)
		{
		}

		#endregion
    }
}
