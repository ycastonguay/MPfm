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
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.Core;
using TinyMessenger;
using MPfm.Library.Services.Interfaces;
using MPfm.Library.Objects;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Song browser presenter.
	/// </summary>
	public class SongBrowserPresenter : BasePresenter<ISongBrowserView>, ISongBrowserPresenter
	{
        readonly ITinyMessengerHub _messageHub;
		readonly IAudioFileCacheService _audioFileCacheService;
		readonly ILibraryService _libraryService;
        readonly IPlayerService _playerService;
	    readonly NavigationManager _navigationManager;
        readonly MobileNavigationManager _mobileNavigationManager;
        LibraryQuery _query;

	    public SongBrowserPresenter(ITinyMessengerHub messageHub,                                     
		                            ILibraryService libraryService,
                                    IPlayerService playerService,
		                            IAudioFileCacheService audioFileCacheService)
		{
			_libraryService = libraryService;
            _playerService = playerService;
			_audioFileCacheService = audioFileCacheService;
            _messageHub = messageHub;
            _query = new LibraryQuery();

            messageHub.Subscribe<LibraryBrowserItemSelectedMessage>((LibraryBrowserItemSelectedMessage m) => {
                ChangeQuery(m.Item.Query);
            });

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
		}

        public override void BindView(ISongBrowserView view)
        {
            base.BindView(view);
            
            view.OnTableRowDoubleClicked = TableRowDoubleClicked;
            view.OnSongBrowserEditSongMetadata = SongBrowserEditSongMetadata;
            view.OnSongBrowserAddToPlaylist = AddToPlaylist;
            view.OnSearchTerms = SearchTerms;
        }

	    public void SongBrowserEditSongMetadata(AudioFile audioFile)
	    {
#if IOS || ANDROID
            // Not available on mobile devices yet.
#else
	        _navigationManager.CreateEditSongMetadataView(audioFile);
#endif
	    }

        private void AddToPlaylist(IEnumerable<AudioFile> audioFiles)
        {
            try
            {
                _playerService.CurrentPlaylist.AddItems(audioFiles);
            }
            catch(Exception ex)
            {
                View.SongBrowserError(ex);
            }
        }

	    public void ChangeQuery(LibraryQuery query)
		{
            try
            {
    			_query = query;
                Tracing.Log("SongBrowserPresenter.ChangeQuery -- Getting audio files (Format: " + query.Format.ToString() + 
                            " | Artist: " + query.ArtistName + " | Album: " + query.AlbumTitle + " | OrderBy: " + query.OrderBy + 
                            " | OrderByAscending: " + query.OrderByAscending.ToString() + " | Search terms: " + query.SearchTerms + ")");
    			IEnumerable<AudioFile> audioFiles = _audioFileCacheService.SelectAudioFiles(query);
    			View.RefreshSongBrowser(audioFiles);
            }
            catch(Exception ex)
            {
                View.SongBrowserError(ex);
            }
		}
		
		public void TableRowDoubleClicked(AudioFile audioFile)
		{			
            Tracing.Log("SongBrowserPresenter.TableRowDoubleClicked -- Publishing SongBrowserItemDoubleClickedMessage with item " + audioFile.Title + "...");
            _messageHub.PublishAsync(new SongBrowserItemDoubleClickedMessage(this){
                Item = audioFile,
                Query = _query
            });
		}

        private void SearchTerms(string searchTerms)
        {
            try
            {
                _query.SearchTerms = searchTerms;
                var audioFiles = _audioFileCacheService.SelectAudioFiles(_query);
                View.RefreshSongBrowser(audioFiles);
            }
            catch(Exception ex)
            {
                View.SongBrowserError(ex);
            }
        }
	}
}
