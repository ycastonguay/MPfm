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
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Select Playlist presenter.
	/// </summary>
    public class SelectPlaylistPresenter : BasePresenter<ISelectPlaylistView>, ISelectPlaylistPresenter
	{
        private readonly NavigationManager _navigationManager;
        private readonly MobileNavigationManager _mobileNavigationManager;
        private LibraryBrowserEntity _item;
	    private List<PlaylistEntity> _items;

	    public SelectPlaylistPresenter(LibraryBrowserEntity item)
        {
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

            RefreshPlaylists();
        }

	    private void RefreshPlaylists()
	    {
	        _items = new List<PlaylistEntity>();
            _items.Add(new PlaylistEntity() { Title = "Bacon" });
            _items.Add(new PlaylistEntity() { Title = "Cow" });
            _items.Add(new PlaylistEntity() { Title = "Tenderloin" });
            _items.Add(new PlaylistEntity() { Title = "Swine" });
            _items.Add(new PlaylistEntity() { Title = "Ground" });
            _items.Add(new PlaylistEntity() { Title = "Round" });
            _items.Add(new PlaylistEntity() { Title = "Tail" });
            _items.Add(new PlaylistEntity() { Title = "Pancetta" });
            _items.Add(new PlaylistEntity() { Title = "Pig" });
            _items.Add(new PlaylistEntity() { Title = "T-Bone" });
            _items.Add(new PlaylistEntity() { Title = "Pork" });
            _items.Add(new PlaylistEntity() { Title = "Chop" });
            _items.Add(new PlaylistEntity() { Title = "Tongue" });
            _items.Add(new PlaylistEntity() { Title = "Drumstick" });
            _items.Add(new PlaylistEntity() { Title = "Jerky" });
            _items.Add(new PlaylistEntity() { Title = "Steak" });
            View.RefreshPlaylists(_items);
	    }        

	    private void SelectPlaylist(PlaylistEntity playlistEntity)
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

	    private void AddNewPlaylist()
	    {
	        var view = _mobileNavigationManager.CreateAddNewPlaylistView();
            _mobileNavigationManager.PushDialogView("Add New Playlist", View, view);
	    }
	}
}
