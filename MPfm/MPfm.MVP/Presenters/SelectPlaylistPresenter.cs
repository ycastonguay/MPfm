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
using MPfm.Core;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.Playlists;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Select Playlist presenter.
	/// </summary>
    public class SelectPlaylistPresenter : BasePresenter<ISelectPlaylistView>, ISelectPlaylistPresenter
	{
        private readonly NavigationManager _navigationManager;
        private readonly MobileNavigationManager _mobileNavigationManager;
	    private readonly ITinyMessengerHub _messengerHub;
	    private readonly ILibraryService _libraryService;
	    private LibraryBrowserEntity _item;
	    private List<Playlist> _items;

	    public SelectPlaylistPresenter(ITinyMessengerHub messengerHub, ILibraryService libraryService, LibraryBrowserEntity item)
        {
	        _messengerHub = messengerHub;
	        _libraryService = libraryService;
	        _item = item;

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
        }

        public override void BindView(ISelectPlaylistView view)
        {
            view.OnAddNewPlaylist = AddNewPlaylist;
            view.OnSelectPlaylist = SelectPlaylist;

            base.BindView(view);

            _messengerHub.Subscribe<PlaylistListUpdatedMessage>(message => RefreshPlaylists());

            RefreshPlaylists();
        }

	    private void RefreshPlaylists()
	    {
	        _items = _libraryService.SelectPlaylists().ToList();
            View.RefreshPlaylists(_items);
	    }        

	    private void SelectPlaylist(Playlist playlist)
	    {
            try
            {
                //Task.Factory.StartNew(() =>
                //{
                //    // Check if adding a song or an album
                //    if (_items[index].AudioFile != null)
                //    {
                //        _playerService.CurrentPlaylist.AddItem(_items[index].AudioFile);
                //        View.NotifyNewPlaylistItems(string.Format("'{0}' was added at the end of the current playlist.", _items[index].Title));
                //    }
                //    else
                //    {
                //        var audioFiles = _libraryService.SelectAudioFiles(_items[index].Query).ToList();
                //        _playerService.CurrentPlaylist.AddItems(audioFiles);
                //        View.NotifyNewPlaylistItems(string.Format("'{0}' was added at the end of the current playlist ({1} songs).", _items[index].Title, audioFiles.Count));
                //    }
                //    _messengerHub.PublishAsync<PlayerPlaylistUpdatedMessage>(new PlayerPlaylistUpdatedMessage(this));
                //}, _cancellationToken);
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while selecting playlist: " + ex.Message);
                View.SelectPlaylistError(ex);
            }
	    }

	    private void AddNewPlaylist()
	    {
            try
            {
                var view = _mobileNavigationManager.CreateAddNewPlaylistView();
                _mobileNavigationManager.PushDialogView("Add New Playlist", View, view);
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while creating AddNewPlaylist view: " + ex.Message);
                View.SelectPlaylistError(ex);
            }
	    }
	}
}
