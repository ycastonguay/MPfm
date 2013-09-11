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
using System.IO;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Library browser presenter for mobile devices.
	/// </summary>
	public class MobileLibraryBrowserPresenter : BasePresenter<IMobileLibraryBrowserView>, IMobileLibraryBrowserPresenter
	{
        readonly MobileNavigationManager _navigationManager;
	    readonly IPlayerService _playerService;
	    readonly ITinyMessengerHub _messengerHub;
        readonly ILibraryService _libraryService;
        readonly IAudioFileCacheService _audioFileCacheService;
        readonly MobileNavigationTabType _tabType;
        MobileLibraryBrowserType _browserType;
        LibraryQuery _query;

        Task _currentTask;
	    List<LibraryBrowserEntity> _items;

	    List<Tuple<MobileLibraryBrowserType, LibraryQuery>> _queryHistory; 
        CancellationTokenSource _cancellationTokenSource = null;
        CancellationToken _cancellationToken;
	    List<Tuple<Task, string, string>> _tasks = new List<Tuple<Task, string, string>>();

	    public AudioFileFormat Filter { get; private set; }
		
        public MobileLibraryBrowserPresenter(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query,
                                             ITinyMessengerHub messengerHub, MobileNavigationManager navigationManager, IPlayerService playerService,
                                             ILibraryService libraryService, IAudioFileCacheService audioFileCacheService)
		{
            _query = query;
            _tabType = tabType;
            _browserType = browserType;
            _messengerHub = messengerHub;
            _navigationManager = navigationManager;
            _playerService = playerService;
            _libraryService = libraryService;
			_audioFileCacheService = audioFileCacheService;

            _queryHistory = new List<Tuple<MobileLibraryBrowserType, LibraryQuery>>();
            _queryHistory.Add(new Tuple<MobileLibraryBrowserType, LibraryQuery>(browserType, query));
			
			Filter = AudioFileFormat.All;
            _currentTask = Task.Factory.StartNew(() => { });
		}
		
        public override void BindView(IMobileLibraryBrowserView view)
        {
            base.BindView(view);

            view.OnChangeBrowserType = ChangeBrowserType;
            view.OnDeleteItem = DeleteItem;
            view.OnItemClick = ItemClick;
            view.OnPlayItem = PlayItem;
            view.OnAddItemToPlaylist = AddItemToPlaylist;
            view.OnRequestAlbumArt = RequestAlbumArt;
            view.OnRequestAlbumArtSynchronously = OnRequestAlbumArtSynchronously;

            // Subscribe to any audio file cache update so we can update this screen
            _messengerHub.Subscribe<AudioFileCacheUpdatedMessage>(AudioFileCacheUpdated);
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>(PlayerPlaylistIndexChanged);

            RefreshLibraryBrowser();
        }

	    public void ChangeBrowserType(MobileLibraryBrowserType browserType)
	    {
	        _browserType = browserType;
            _query = new LibraryQuery();            
            _queryHistory.Clear();
            _queryHistory.Add(new Tuple<MobileLibraryBrowserType, LibraryQuery>(browserType, _query));
            RefreshLibraryBrowser();
	    }

        public void SetQuery(MobileLibraryBrowserType browserType, LibraryQuery query)
        {
            _queryHistory.Add(new Tuple<MobileLibraryBrowserType, LibraryQuery>(browserType, query));
            _browserType = browserType;
            _query = query;
            RefreshLibraryBrowser();
        }

        public void PopBackstack(MobileLibraryBrowserType browserType, LibraryQuery query)
        {
            if (_queryHistory.Count == 0)
                return;

            _queryHistory.RemoveAt(_queryHistory.Count - 1);
            _browserType = browserType;
            _query = query;
            RefreshLibraryBrowser(true);
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

        private void RequestAlbumArt(string artistName, string albumTitle, object userData)
        {
            // TODO: Add canceling, add detection to not request the same album art multiple times
            // Only run one task at a time.
            _currentTask = _currentTask.ContinueWith(t => {
                // Get the file path of the first file in the album
                var audioFiles = _libraryService.SelectAudioFiles(AudioFileFormat.All, artistName, albumTitle, string.Empty);
                var audioFile = (audioFiles != null && audioFiles.Count() > 0) ? audioFiles.ElementAt(0) : null;

                if (audioFile != null)
                {
                    // Update with with album art byte array
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                    View.RefreshAlbumArtCell(artistName, albumTitle, bytesImage, userData);
                }
            });
        }

        private void AddItemToPlaylist(int index)
        {
            try
            {
                Console.WriteLine("MobileLibraryBrowserPresenter - AddItemToPlaylist - index: {0}", index);
                Task.Factory.StartNew(() =>
                {
                    // Check if adding a song or an album
                    if (_items[index].AudioFile != null)
                    {
                        _playerService.CurrentPlaylist.AddItem(_items[index].AudioFile);
                        View.NotifyNewPlaylistItems(string.Format("'{0}' was added at the end of the current playlist.", _items[index].Title));
                    }
                    else
                    {
                        var audioFiles = _libraryService.SelectAudioFiles(_items[index].Query).ToList();
                        _playerService.CurrentPlaylist.AddItems(audioFiles);
                        View.NotifyNewPlaylistItems(string.Format("'{0}' was added at the end of the current playlist ({1} songs).", _items[index].Title, audioFiles.Count));
                    }
                    _messengerHub.PublishAsync<PlayerPlaylistUpdatedMessage>(new PlayerPlaylistUpdatedMessage(this));
                }, _cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("MobileLibraryBrowserPresenter - AddItemToPlaylist - Exception: {0}", ex);
                View.MobileLibraryBrowserError(ex);
            }
        }

        private void PlayItem(int index)
        {
            try
            {
                Console.WriteLine("MobileLibraryBrowserPresenter - PlayItem index: {0}", index);
                if (_items[index].Type == LibraryBrowserEntityType.Artist)
                {
                    Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
                    {
                        Query = _items[index].Query,
                        FilePath = _items[index].AudioFile != null ? _items[index].AudioFile.FilePath : string.Empty
                    });

                    _navigationManager.CreatePlayerView(_tabType, onViewBindedToPresenter);
                }
                else if (_items[index].Type == LibraryBrowserEntityType.Album)
                {
                    Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
                    {
                        Query = _items[index].Query,
                        FilePath = _items[index].AudioFile != null ? _items[index].AudioFile.FilePath : string.Empty
                    });

                    _navigationManager.CreatePlayerView(_tabType, onViewBindedToPresenter);
                }
                else if (_items[index].Type == LibraryBrowserEntityType.Song)
                {
                    Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
                    {
                        Query = _items[index].Query,
                        FilePath = _items[index].AudioFile != null ? _items[index].AudioFile.FilePath : string.Empty
                    });

                    _navigationManager.CreatePlayerView(_tabType, onViewBindedToPresenter);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MobileLibraryBrowserPresenter - PlayItem - Exception: {0}", ex);
                View.MobileLibraryBrowserError(ex);
            }
        }

        private byte[] OnRequestAlbumArtSynchronously(string artistName, string albumTitle)
        {
            // Get the file path of the first file in the album
            byte[] bytesImage = null;
            var audioFiles = _audioFileCacheService.SelectAudioFiles(new LibraryQuery()
            {
                Format = AudioFileFormat.All,
                ArtistName = artistName,
                AlbumTitle = albumTitle
            });
            //var audioFiles = _libraryService.SelectAudioFiles(AudioFileFormat.All, artistName, albumTitle, string.Empty);
            var audioFile = (audioFiles != null && audioFiles.Count() > 0) ? audioFiles.ElementAt(0) : null;
            if (audioFile != null)
                bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);

            return bytesImage;
        }

        private void DeleteItem(int index)
        {
            try
            {
                Console.WriteLine("MobileLibraryBrowserPresenter - DeleteItem index: {0}", index);
                Task.Factory.StartNew(() => {
                    if(_items[index].Type == LibraryBrowserEntityType.Artist)
                    {
                        _libraryService.DeleteAudioFiles(_items[index].Query.ArtistName, string.Empty);
                        Console.WriteLine("MobileLibraryBrowserPresenter - Deleting files from hard disk...");
                        var files = _audioFileCacheService.SelectAudioFiles(new LibraryQuery(){ ArtistName = _items[index].Query.ArtistName });
                        foreach(var file in files)
                        {
                            Console.WriteLine("MobileLibraryBrowserPresenter - Deleting {0}...", file.FilePath);
                            File.Delete(file.FilePath);
                        }

                        Console.WriteLine("MobileLibraryBrowserPresenter - Removing audio files from cache...");
                        _audioFileCacheService.RemoveAudioFiles(_items[index].Query.ArtistName, string.Empty);
                    }
                    else if(_items[index].Type == LibraryBrowserEntityType.Album)
                    {
                        _libraryService.DeleteAudioFiles(_items[index].Query.ArtistName, _items[index].Query.AlbumTitle);
                        _audioFileCacheService.RemoveAudioFiles(_items[index].Query.ArtistName, _items[index].Query.AlbumTitle);
                    }
                    else if(_items[index].Type == LibraryBrowserEntityType.Song)
                    {
                        _libraryService.DeleteAudioFile(_items[index].AudioFile.Id);
                        _audioFileCacheService.RemoveAudioFile(_items[index].AudioFile.Id);
                    }
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine("MobileLibraryBrowserPresenter - DeleteItem - Exception: {0}", ex);
                View.MobileLibraryBrowserError(ex);
            }
        }

	    private void ItemClick(int index)
	    {
            try
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

                    // On Android, pushing new fragments on ViewPager is extremely buggy, so instead we refresh the same view with new queries. 
                    // AndroidNavigationManager manages the tab history backstack.
#if ANDROID
                    _navigationManager.NotifyMobileLibraryBrowserQueryChange(_tabType, browserType, _items[index].Query);
                    SetQuery(browserType, _items[index].Query);
#else
                    var newView = _navigationManager.CreateMobileLibraryBrowserView(_tabType, browserType, _items[index].Query);
                    _navigationManager.PushTabView(_tabType, browserType, _items[index].Query, newView);
#endif

                    return;
                }

                // Make sure the view was binded to the presenter before publishing a message
                // Why is an action used here? Because on desktop devices, IPlayerView is initialized right away. On mobile devices, IPlayerView is only initialized when playback is started.
                // TO DO: Replace this info on a service. i.e. when initializing presenter, refresh view with query information.
    	        Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
    	            {
    	                Query = _items[index].Query,
                        FilePath = _items[index].AudioFile.FilePath
    	            });
                _navigationManager.CreatePlayerView(_tabType, onViewBindedToPresenter);
            }
            catch(Exception ex)
            {
                Console.WriteLine("MobileLibraryBrowserPresenter - ItemClick - Exception: {0}", ex);
                View.MobileLibraryBrowserError(ex);
            }
	    }

        private void RefreshLibraryBrowser()
        {
            RefreshLibraryBrowser(false);
        }

        private void RefreshLibraryBrowser(bool isPopBackstack)
        {
            // Build breadcrumb
            string breadcrumb = string.Empty;
            for(int a = 0; a < _queryHistory.Count; a++)
            {
                var history = _queryHistory[a];
                Console.WriteLine("MobileLibraryBrowserPresenter - RefreshLibraryBrowser - Breadcrumb - query history {0} - browserType: {1} - query.ArtistName: {2} - query.AlbumTitle {3}", a, history.Item1.ToString(), history.Item2.ArtistName, history.Item2.AlbumTitle);
                switch (history.Item1)
                {
                    case MobileLibraryBrowserType.Playlists:
                        breadcrumb += "Playlists";
                        break;
                    case MobileLibraryBrowserType.Artists:
                        breadcrumb += "Artists";
                        break;
                    case MobileLibraryBrowserType.Albums:
                        if (!string.IsNullOrEmpty(history.Item2.ArtistName))
                            breadcrumb += "Albums by " + history.Item2.ArtistName;
                        else
                            breadcrumb += "Albums";
                        break;
                    case MobileLibraryBrowserType.Songs:
                        breadcrumb += "Songs";
                        //if (string.IsNullOrEmpty(history.Item2.ArtistName) &&
                        //    string.IsNullOrEmpty(history.Item2.AlbumTitle))
                        //    breadcrumb += "Songs";
                        //else
                        //    breadcrumb += "Songs on " + history.Item2.AlbumTitle;
                        break;
                }
                if (a < _queryHistory.Count - 1)
                    breadcrumb += " > ";
            }

            bool isBackstackEmpty = _queryHistory.Count <= 1;
            _items = new List<LibraryBrowserEntity>();
            switch (_browserType)
            {
                case MobileLibraryBrowserType.Playlists:
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), "All Playlists", breadcrumb, isPopBackstack, isBackstackEmpty);
                    break;
                case MobileLibraryBrowserType.Artists:
                    _items = GetArtists().ToList();
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), "All Artists", breadcrumb, isPopBackstack, isBackstackEmpty);
                    break;
                case MobileLibraryBrowserType.Albums:
                    _items = GetAlbums(_query.ArtistName).ToList();
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), (String.IsNullOrEmpty(_query.ArtistName)) ? "All Albums" : _query.ArtistName, breadcrumb, isPopBackstack, isBackstackEmpty);
                    break;
                case MobileLibraryBrowserType.Songs:
                    _items = GetSongs(_query.ArtistName, _query.AlbumTitle).ToList();
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), (String.IsNullOrEmpty(_query.AlbumTitle)) ? "All Songs" : _query.AlbumTitle, breadcrumb, isPopBackstack, isBackstackEmpty);
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
                Dictionary<string, List<string>> albums = _libraryService.SelectDistinctAlbumTitles(format, artist);
                list.Add(new LibraryBrowserEntity()
                {
                    Title = artist,
                    Type = LibraryBrowserEntityType.Artist,
                    AlbumTitles = albums[artist].ToList(),
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
