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
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Library browser presenter for mobile devices.
	/// </summary>
	public class MobileLibraryBrowserPresenter : BasePresenter<IMobileLibraryBrowserView>, IMobileLibraryBrowserPresenter
	{
        private readonly MobileNavigationManager _navigationManager;
	    private readonly MobileNavigationTabType _tabType;
	    private readonly MobileLibraryBrowserType _browserType;
	    private readonly ITinyMessengerHub _messengerHub;
        private readonly ILibraryService _libraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
	    private List<LibraryBrowserEntity> _items;

	    public AudioFileFormat Filter { get; private set; }
		
        public MobileLibraryBrowserPresenter(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, ITinyMessengerHub messengerHub, MobileNavigationManager navigationManager,
                                             ILibraryService libraryService, IAudioFileCacheService audioFileCacheService)
		{
            _tabType = tabType;
            _browserType = browserType;
            _messengerHub = messengerHub;
            _navigationManager = navigationManager;
            _libraryService = libraryService;
			_audioFileCacheService = audioFileCacheService;			
			
			Filter = AudioFileFormat.All;
		}
		
        public override void BindView(IMobileLibraryBrowserView view)
        {
            base.BindView(view);

            view.OnItemClick = OnItemClick;

            // Subscribe to any audio file cache update so we can update this screen
            _messengerHub.Subscribe<AudioFileCacheUpdatedMessage>(AudioFileCacheUpdated);
            RefreshLibraryBrowser();
        }

	    private void AudioFileCacheUpdated(AudioFileCacheUpdatedMessage audioFileCacheUpdatedMessage)
	    {
            // Refresh browser with new data
            RefreshLibraryBrowser();
	    }

	    private void OnItemClick(int i)
	    {
            // PLAYLIST TAB: Playlists --> Player
            // ARTISTS TAB: Artists --> Albums --> Songs --> Player
            // ALBUMS TAB: Albums --> Songs --> Player
            // SONGS TAB: Songs --> Player

            // Check if another MobileLibraryBrowser view needs to be pushed
            if (_tabType == MobileNavigationTabType.Artists || _tabType == MobileNavigationTabType.Albums)
            {
                var browserType = (_browserType == MobileLibraryBrowserType.Artists) ? MobileLibraryBrowserType.Albums : MobileLibraryBrowserType.Songs;
                var newView = _navigationManager.CreateMobileLibraryBrowserView(_tabType, browserType);
                _navigationManager.PushTabView(_tabType, newView);
                return;
            }

            // Make sure the view was binded to the presenter before publishing a message
	        Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
	            {
	                Query = _items[i].Query
	            });

            // Create player view
            var view = _navigationManager.CreatePlayerView(onViewBindedToPresenter);
            _navigationManager.PushTabView(_tabType, view);
	    }

        private void RefreshLibraryBrowser()
        {
            _items = new List<LibraryBrowserEntity>();
            if (_tabType == MobileNavigationTabType.Artists)
                _items = GetArtists().ToList();
            else if (_tabType == MobileNavigationTabType.Albums)
                _items = GetAlbums().ToList();
            else if (_tabType == MobileNavigationTabType.Songs)
                _items = GetSongs().ToList();
            View.RefreshLibraryBrowser(_items);
        }

        private IEnumerable<LibraryBrowserEntity> GetArtists()
        {
            var format = AudioFileFormat.All;
            var list = new List<LibraryBrowserEntity>();
            List<string> artists = _libraryService.SelectDistinctArtistNames(format);
            foreach (string artist in artists)
            {
                list.Add(new LibraryBrowserEntity()
                {
                    Title = artist,
                    Type = LibraryBrowserEntityType.Artist,
                    Query = new SongBrowserQueryEntity()
                    {
                        Format = format,
                        ArtistName = artist
                    }
                });
            }
            return list;
        }

        private IEnumerable<LibraryBrowserEntity> GetAlbums()
        {
            return GetArtistAlbums(string.Empty);
        }

        private IEnumerable<LibraryBrowserEntity> GetArtistAlbums(string artistName)
        {
            var format = AudioFileFormat.All;
            var list = new List<LibraryBrowserEntity>();
            var albumTitles = _libraryService.SelectDistinctAlbumTitles(format, artistName);
            var albums = new List<string>();

            // Extract album titles from dictionary
            foreach (var keyValue in albumTitles)
                foreach (string albumTitle in keyValue.Value)
                    albums.Add(albumTitle);

            // Order the albums by title
            albums = albums.OrderBy(x => x).ToList();
			
			// Convert to entities
			foreach(string album in albums)
			{
				list.Add(new LibraryBrowserEntity(){
					Title = album,
					Type = LibraryBrowserEntityType.Album,
					Query = new SongBrowserQueryEntity(){
						Format = format,
						ArtistName = artistName,
						AlbumTitle = album						
					}
				});
			}

            return list;
        }

        private IEnumerable<LibraryBrowserEntity> GetSongs()
        {
            return GetSongs(string.Empty, string.Empty);
        }

        private IEnumerable<LibraryBrowserEntity> GetSongs(string artistName, string albumTitle)
        {
            var format = AudioFileFormat.All;
            var list = new List<LibraryBrowserEntity>();
            var audioFiles = _libraryService.SelectAudioFiles(format, artistName, albumTitle, string.Empty);
            var songs = audioFiles.Select(x => x.Title).OrderBy(x => x).ToList();

            // Convert to entities
            foreach (var song in songs)
            {
                list.Add(new LibraryBrowserEntity()
                {
                    Title = song,
                    Type = LibraryBrowserEntityType.Song,
                    Query = new SongBrowserQueryEntity()
                    {
                        Format = format,                        
                        ArtistName = artistName,
                        AlbumTitle = albumTitle,
                        SearchTerms = song
                    }
                });
            }

            return list;
        }
    
    }
}

