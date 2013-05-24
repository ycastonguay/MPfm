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
using System.Threading;
using System.Threading.Tasks;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using TinyMessenger;
using MPfm.Library.Services.Interfaces;
using MPfm.Library.Messages;
using MPfm.Library.Objects;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Library browser presenter for mobile devices.
	/// </summary>
	public class MobileLibraryBrowserPresenter : BasePresenter<IMobileLibraryBrowserView>, IMobileLibraryBrowserPresenter
	{
        private readonly MobileNavigationManager _navigationManager;
	    private readonly ITinyMessengerHub _messengerHub;
        private readonly ILibraryService _libraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly MobileNavigationTabType _tabType;
        private readonly MobileLibraryBrowserType _browserType;
        private LibraryQuery _query;

        private Task _currentTask;
	    private List<LibraryBrowserEntity> _items;

	    public AudioFileFormat Filter { get; private set; }
		
        public MobileLibraryBrowserPresenter(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query,
                                             ITinyMessengerHub messengerHub, MobileNavigationManager navigationManager,
                                             ILibraryService libraryService, IAudioFileCacheService audioFileCacheService)
		{
            _query = query;
            _tabType = tabType;
            _browserType = browserType;
            _messengerHub = messengerHub;
            _navigationManager = navigationManager;
            _libraryService = libraryService;
			_audioFileCacheService = audioFileCacheService;			
			
			Filter = AudioFileFormat.All;
            _currentTask = Task.Factory.StartNew(() => { });
		}
		
        public override void BindView(IMobileLibraryBrowserView view)
        {
            base.BindView(view);

            view.OnItemClick = OnItemClick;
            view.OnRequestAlbumArt = RequestAlbumArt;

            // Subscribe to any audio file cache update so we can update this screen
            _messengerHub.Subscribe<AudioFileCacheUpdatedMessage>(AudioFileCacheUpdated);
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>(PlayerPlaylistIndexChanged);

            RefreshLibraryBrowser();
        }

	    private void AudioFileCacheUpdated(AudioFileCacheUpdatedMessage message)
	    {
            RefreshLibraryBrowser();
	    }

        private void PlayerPlaylistIndexChanged(PlayerPlaylistIndexChangedMessage message)
        {
            if (_browserType == MobileLibraryBrowserType.Songs)
            {
                // Find song index in list
                int index = -1;
                index = _items.FindIndex(x => x.AudioFile.FilePath == message.Data.AudioFileStarted.FilePath);
                if(index >= 0)
                {
                    View.RefreshCurrentlyPlayingSong(index, _items[index].AudioFile);
                }
            }
        }

        private void RequestAlbumArt(string artistName, string albumTitle)
        {
            // Only run one task at a time.
            _currentTask = _currentTask.ContinueWith(t => {
                // Get the file path of the first file in the album
                var audioFiles = _libraryService.SelectAudioFiles(AudioFileFormat.All, artistName, albumTitle, string.Empty);
                var audioFile = (audioFiles != null && audioFiles.Count() > 0) ? audioFiles.ElementAt(0) : null;

                if (audioFile != null)
                {
                    // Update with with album art byte array
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                    View.RefreshAlbumArtCell(artistName, albumTitle, bytesImage);
                }
            });
        }

	    private void OnItemClick(int i)
	    {
            // PLAYLIST TAB: Playlists --> Player
            // ARTISTS TAB: Artists --> Albums --> Songs --> Player
            // ALBUMS TAB: Albums --> Songs --> Player
            // SONGS TAB: Songs --> Player

            // Check if another MobileLibraryBrowser view needs to be pushed
            if ((_tabType == MobileNavigationTabType.Artists || _tabType == MobileNavigationTabType.Albums) &&
                _browserType != MobileLibraryBrowserType.Songs)
            {
                var browserType = (_browserType == MobileLibraryBrowserType.Artists) ? MobileLibraryBrowserType.Albums : MobileLibraryBrowserType.Songs;
                var newView = _navigationManager.CreateMobileLibraryBrowserView(_tabType, browserType, _items[i].Query);
                _navigationManager.PushTabView(_tabType, newView);
                return;
            }

            // Make sure the view was binded to the presenter before publishing a message
            // Why is an action used here? Because on desktop devices, IPlayerView is initialized right away. On mobile devices, IPlayerView is only initialized when playback is started.
	        Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
	            {
	                Query = _items[i].Query,
                    FilePath = _items[i].AudioFile.FilePath
	            });

            // Create player view
            var view = _navigationManager.CreatePlayerView(onViewBindedToPresenter);
            _navigationManager.PushTabView(_tabType, view);
	    }

        public void RefreshView(LibraryQuery query)
        {
            _query = query;
            RefreshLibraryBrowser();
        }

        private void RefreshLibraryBrowser()
        {
            _items = new List<LibraryBrowserEntity>();

            switch (_browserType)
            {
                case MobileLibraryBrowserType.Playlists:
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), "All Playlists");
                    break;
                case MobileLibraryBrowserType.Artists:
                    _items = GetArtists().ToList();
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), "All Artists");
                    break;
                case MobileLibraryBrowserType.Albums:
                    _items = GetAlbums(_query.ArtistName).ToList();
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), (String.IsNullOrEmpty(_query.ArtistName)) ? "All Albums" : _query.ArtistName);
                    break;
                case MobileLibraryBrowserType.Songs:
                    _items = GetSongs(_query.ArtistName, _query.AlbumTitle).ToList();
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), (String.IsNullOrEmpty(_query.AlbumTitle)) ? "All Songs" : _query.AlbumTitle);
                    break;
            }
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
                    Query = new LibraryQuery()
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
            return GetAlbums(string.Empty);
        }

        private IEnumerable<LibraryBrowserEntity> GetAlbums(string artistName)
        {
            var format = AudioFileFormat.All;
            var list = new List<LibraryBrowserEntity>();

            // Fetch album titles from library
            var albumTitles = _libraryService.SelectDistinctAlbumTitles(format, artistName);

            // Extract album titles from dictionary
            var albums = new List<KeyValuePair<string, string>>();
            foreach (var key in albumTitles)
                foreach (string albumTitle in key.Value)
                    albums.Add(new KeyValuePair<string, string>(key.Key, albumTitle));

            // Order the albums by title
            albums = albums.OrderBy(x => x.Value).ToList();
			
			// Convert to entities
			foreach(var album in albums)
			{
				list.Add(new LibraryBrowserEntity(){
					Title = album.Value,
                    Subtitle = album.Key,
					Type = LibraryBrowserEntityType.Album,
                    Query = new LibraryQuery(){
						Format = format,
						ArtistName = artistName,
						AlbumTitle = album.Value						
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

            // If a single album is specified, order songs by disc number/track number
            if (!String.IsNullOrEmpty(albumTitle))
                audioFiles = audioFiles.OrderBy(x => x.DiscNumber).ThenBy(x => x.TrackNumber).ToList();
            else
                audioFiles = audioFiles.OrderBy(x => x.Title).ToList();

            // Convert to entities
            foreach (var audioFile in audioFiles)
            {
                list.Add(new LibraryBrowserEntity()
                {
                    Title = audioFile.Title,
                    Subtitle = audioFile.Length,
                    AudioFile = audioFile,
                    Type = LibraryBrowserEntityType.Song,
                    Query = new LibraryQuery()
                    {
                        Format = format,                        
                        ArtistName = artistName,
                        AlbumTitle = albumTitle
                    }
                });
            }

            return list;
        }
    
    }
}

