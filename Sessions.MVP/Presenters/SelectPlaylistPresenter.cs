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
using System.Linq;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Messages;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using Sessions.Core;
using Sessions.Library.Objects;
using Sessions.Library.Services.Interfaces;
using TinyMessenger;

namespace Sessions.MVP.Presenters
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
	    private readonly IAudioFileCacheService _audioFileCacheService;
	    private LibraryBrowserEntity _item;
	    private List<PlaylistEntity> _items;

	    public SelectPlaylistPresenter(ITinyMessengerHub messengerHub, ILibraryService libraryService, IAudioFileCacheService audioFileCacheService, LibraryBrowserEntity item)
        {
	        _messengerHub = messengerHub;
	        _libraryService = libraryService;
	        _audioFileCacheService = audioFileCacheService;
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
            //var playlists = _libraryService.SelectPlaylists().ToList();
            //_items = new List<PlaylistEntity>();
            //foreach (var playlist in playlists)
            //{
            //    var items = _libraryService.SelectPlaylistItems(playlist.PlaylistId);
            //    _items.Add(new PlaylistEntity()
            //    {
            //        PlaylistId = playlist.PlaylistId,
            //        Name = playlist.Name,
            //        LastModified = playlist.LastModified,
            //        ItemCount = items.Count
            //    });
            //}

            //View.RefreshPlaylists(_items);
	    }

        private void SelectPlaylist(PlaylistEntity playlist)
	    {
            //try
            //{
            //    // Check if adding a song or an album
            //    if (_item.AudioFile != null)
            //    {
            //        //_playerService.CurrentPlaylist.AddItem(_items[index].AudioFile);
            //        //View.NotifyNewPlaylistItems(string.Format("'{0}' was added at the end of the current playlist.", _items[index].Title));
            //    }
            //    else
            //    {
            //        var items = _libraryService.SelectPlaylistItems(playlist.PlaylistId).ToList();
            //        var audioFileIds = _audioFileCacheService.SelectAudioFiles(_item.Query).Select(x => x.Id).ToList();
                    
            //        // Find a list of new items
            //        var newItems = new List<PlaylistAudioFile>();
            //        int position = items.Count;
            //        foreach (Guid audioFileId in audioFileIds)
            //        {
            //            if (!items.Any(x => x.AudioFileId == audioFileId))
            //            {
            //                newItems.Add(new PlaylistAudioFile(playlist.PlaylistId, audioFileId, position));
            //                position++;
            //            }
            //        }

            //        foreach(var item in newItems)
            //            _libraryService.InsertPlaylistItem(item);

            //        _messengerHub.PublishAsync<PlaylistUpdatedMessage>(new PlaylistUpdatedMessage(this, playlist.PlaylistId, newItems.Count));
            //    }                
            //}
            //catch (Exception ex)
            //{
            //    Tracing.Log("An error occured while selecting playlist: " + ex.Message);
            //    View.SelectPlaylistError(ex);
            //}
	    }

	    private void AddNewPlaylist()
	    {
            try
            {
                var view = _mobileNavigationManager.CreateAddPlaylistView();
                _mobileNavigationManager.PushDialogView(MobileDialogPresentationType.Overlay, "Add New Playlist", View, view);
            }
            catch (Exception ex)
            {
                Tracing.Log("An error occured while creating AddNewPlaylist view: " + ex.Message);
                View.SelectPlaylistError(ex);
            }
	    }
	}
}
