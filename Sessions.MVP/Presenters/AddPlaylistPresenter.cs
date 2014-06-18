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
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using Sessions.Core;
using Sessions.Sound.Playlists;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Add Playlist presenter.
	/// </summary>
    public class AddPlaylistPresenter : BasePresenter<IAddPlaylistView>, IAddPlaylistPresenter
	{
	    private readonly ITinyMessengerHub _messengerHub;
	    private readonly ILibraryService _libraryService;

	    public AddPlaylistPresenter(ITinyMessengerHub messengerHub, ILibraryService libraryService)
	    {
	        _messengerHub = messengerHub;
	        _libraryService = libraryService;
	    }

	    public override void BindView(IAddPlaylistView view)
        {
            view.OnSavePlaylist = SavePlaylist;

            base.BindView(view);
        }

	    private void SavePlaylist(string title)
	    {
            try
            {
                var playlist = new Playlist();
                playlist.Name = title;
                _libraryService.InsertPlaylist(playlist);
                _messengerHub.PublishAsync<PlaylistListUpdatedMessage>(new PlaylistListUpdatedMessage(this));
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while saving playlist: " + ex.Message);
                View.AddPlaylistError(ex);
            }
	    }
	}
}
