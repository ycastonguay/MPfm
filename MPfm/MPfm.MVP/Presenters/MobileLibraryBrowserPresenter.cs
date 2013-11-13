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
using MPfm.Core;
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
	    List<LibraryBrowserEntity> _items;
	    List<Tuple<MobileLibraryBrowserType, LibraryQuery>> _queryHistory;
        Task _currentTask;

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

            _messengerHub.Subscribe<AudioFileCacheUpdatedMessage>(AudioFileCacheUpdated);
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>(PlayerPlaylistIndexChanged);
            _messengerHub.Subscribe<PlaylistUpdatedMessage>(PlaylistUpdated);
            _messengerHub.Subscribe<MobileLibraryBrowserPopBackstackMessage>(PopBackstack);
            _messengerHub.Subscribe<MobileLibraryBrowserChangeQueryMessage>((m) => { ChangeQuery(m.BrowserType, m.Query); });

            RefreshLibraryBrowser();
        }

	    private void ChangeBrowserType(MobileLibraryBrowserType browserType)
	    {
	        _browserType = browserType;
            _query = new LibraryQuery();            
            _queryHistory.Clear();
            _queryHistory.Add(new Tuple<MobileLibraryBrowserType, LibraryQuery>(browserType, _query));
            RefreshLibraryBrowser();
	    }

        private void SetQuery(MobileLibraryBrowserType browserType, LibraryQuery query)
        {
            _queryHistory.Add(new Tuple<MobileLibraryBrowserType, LibraryQuery>(browserType, query));
            _browserType = browserType;
            _query = query;
            RefreshLibraryBrowser();
        }

        public void ChangeQuery(MobileLibraryBrowserType browserType, LibraryQuery query)
        {
            // Do not change query if the current query is the same
            if (_browserType == browserType &&
                _query.ArtistName == query.ArtistName &&
                _query.AlbumTitle == query.AlbumTitle &&
                _query.Format == query.Format)
                return;

            _queryHistory.Clear();
            SetQuery(browserType, query);
        }

        private void PopBackstack(MobileLibraryBrowserPopBackstackMessage message)
        {
            if (_queryHistory.Count == 0)
                return;

            _queryHistory.RemoveAt(_queryHistory.Count - 1);
            var history = _queryHistory[_queryHistory.Count - 1];
            _browserType = history.Item1;
            _query = history.Item2;
            
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

        private void PlaylistUpdated(PlaylistUpdatedMessage message)
        {
            View.NotifyNewPlaylistItems(string.Format("{0} songs were added to the playlist.", message.NewItemCount));
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
                Tracing.Log("MobileLibraryBrowserPresenter - AddItemToPlaylist - index: {0}", index);

                var view = _navigationManager.CreateSelectPlaylistView(_items[index]);
                _navigationManager.PushDialogView(MobileDialogPresentationType.Overlay, "Select Playlist", View, view);
            }
            catch (Exception ex)
            {
                Tracing.Log("MobileLibraryBrowserPresenter - AddItemToPlaylist - Exception: {0}", ex);
                View.MobileLibraryBrowserError(ex);
            }
        }

        private void PlayItem(int index)
        {
            try
            {
                Tracing.Log("MobileLibraryBrowserPresenter - PlayItem index: {0}", index);
                if (_items[index].EntityType == LibraryBrowserEntityType.Artist)
                {
                    Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
                    {
                        Query = _items[index].Query,
                        FilePath = _items[index].AudioFile != null ? _items[index].AudioFile.FilePath : string.Empty
                    });

                    _navigationManager.CreatePlayerView(_tabType);
                }
                else if (_items[index].EntityType == LibraryBrowserEntityType.Album)
                {
                    Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
                    {
                        Query = _items[index].Query,
                        FilePath = _items[index].AudioFile != null ? _items[index].AudioFile.FilePath : string.Empty
                    });

                    _navigationManager.CreatePlayerView(_tabType);
                }
                else if (_items[index].EntityType == LibraryBrowserEntityType.Song)
                {
                    Action<IBaseView> onViewBindedToPresenter = (theView) => _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
                    {
                        Query = _items[index].Query,
                        FilePath = _items[index].AudioFile != null ? _items[index].AudioFile.FilePath : string.Empty
                    });

                    _navigationManager.CreatePlayerView(_tabType);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MobileLibraryBrowserPresenter - PlayItem - Exception: {0}", ex);
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
                Tracing.Log("MobileLibraryBrowserPresenter - DeleteItem index: {0}", index);
                Task.Factory.StartNew(() => {
                    if(_items[index].EntityType == LibraryBrowserEntityType.Artist)
                    {
                        _libraryService.DeleteAudioFiles(_items[index].Query.ArtistName, string.Empty);
                        Tracing.Log("MobileLibraryBrowserPresenter - Deleting files from hard disk...");
                        var files = _audioFileCacheService.SelectAudioFiles(new LibraryQuery(){ ArtistName = _items[index].Query.ArtistName });
                        foreach(var file in files)
                        {
                            Tracing.Log("MobileLibraryBrowserPresenter - Deleting {0}...", file.FilePath);

#if WINDOWSSTORE || WINDOWS_PHONE
                            // TODO: Implement this
#else
                            File.Delete(file.FilePath);
#endif
                        }

                        Tracing.Log("MobileLibraryBrowserPresenter - Removing audio files from cache...");
                        _audioFileCacheService.RemoveAudioFiles(_items[index].Query.ArtistName, string.Empty);
                    }
                    else if(_items[index].EntityType == LibraryBrowserEntityType.Album)
                    {
                        _libraryService.DeleteAudioFiles(_items[index].Query.ArtistName, _items[index].Query.AlbumTitle);
                        _audioFileCacheService.RemoveAudioFiles(_items[index].Query.ArtistName, _items[index].Query.AlbumTitle);
                    }
                    else if(_items[index].EntityType == LibraryBrowserEntityType.Song)
                    {
                        _libraryService.DeleteAudioFile(_items[index].AudioFile.Id);
                        _audioFileCacheService.RemoveAudioFile(_items[index].AudioFile.Id);
                    }
                });
            }
            catch(Exception ex)
            {
                Tracing.Log("MobileLibraryBrowserPresenter - DeleteItem - Exception: {0}", ex);
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
                    _messengerHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
                    {
                        Query = _items[index].Query
                    });

                    // On Android, pushing new fragments on ViewPager is extremely buggy, so instead we refresh the same view with new queries. 
#if ANDROID
                    SetQuery(browserType, _items[index].Query);
#else
                    var newView = _navigationManager.CreateMobileLibraryBrowserView(_tabType, browserType, _items[index].Query);
                    //_navigationManager.PushTabView(_tabType, browserType, _items[index].Query, newView);
                    _navigationManager.PushTabView(_tabType, newView);
#endif

                    return;
                }

                // Start playback and start Player view
                var audioFiles = _audioFileCacheService.SelectAudioFiles(_query);
                _playerService.Play(audioFiles, _items[index].AudioFile != null ? _items[index].AudioFile.FilePath : string.Empty, 0, false, true);
                _navigationManager.CreatePlayerView(_tabType);
            }
            catch(Exception ex)
            {
                Tracing.Log("MobileLibraryBrowserPresenter - ItemClick - Exception: {0}", ex);
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
                Tracing.Log("MobileLibraryBrowserPresenter - RefreshLibraryBrowser - Breadcrumb - query history {0} - browserType: {1} - query.ArtistName: {2} - query.AlbumTitle {3}", a, history.Item1.ToString(), history.Item2.ArtistName, history.Item2.AlbumTitle);
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
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), "Playlists", breadcrumb, isPopBackstack, isBackstackEmpty);
                    break;
                case MobileLibraryBrowserType.Artists:
                    _items = GetArtists().ToList();
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), "Artists", breadcrumb, isPopBackstack, isBackstackEmpty);
                    break;
                case MobileLibraryBrowserType.Albums:
                    _items = GetAlbums(_query.ArtistName).ToList();
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), (String.IsNullOrEmpty(_query.ArtistName)) ? "Albums" : _query.ArtistName, breadcrumb, isPopBackstack, isBackstackEmpty);
                    break;
                case MobileLibraryBrowserType.Songs:
                    _items = GetSongs(_query.ArtistName, _query.AlbumTitle).ToList();
                    View.RefreshLibraryBrowser(_items, _browserType, _tabType.ToString(), (String.IsNullOrEmpty(_query.AlbumTitle)) ? "Songs" : _query.AlbumTitle, breadcrumb, isPopBackstack, isBackstackEmpty);
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
                    EntityType = LibraryBrowserEntityType.Artist,
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
					EntityType = LibraryBrowserEntityType.Album,
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
                    EntityType = LibraryBrowserEntityType.Song,
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
