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

using System.Collections.Generic;
using System.Linq;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.Core;
using TinyMessenger;
using MPfm.Library.Services.Interfaces;
using MPfm.Library.Objects;
using MPfm.MVP.Config;
using MPfm.Library.Messages;
using System;
using System.IO;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Library browser presenter.
	/// </summary>
	public class LibraryBrowserPresenter : BasePresenter<ILibraryBrowserView>, ILibraryBrowserPresenter
	{
        private readonly ITinyMessengerHub _messageHub;
        private readonly ILibraryService _libraryService;
        private readonly IPlayerService _playerService;
		
		public AudioFileFormat Filter { get; private set; }
		
        public LibraryBrowserPresenter(ITinyMessengerHub messageHub, ILibraryService libraryService, IPlayerService playerService)
		{						
            _messageHub = messageHub;
			_libraryService = libraryService;
            _playerService = playerService;
		}
		
        public override void BindView(ILibraryBrowserView view)
        {
            base.BindView(view);
            view.OnAudioFileFormatFilterChanged = AudioFileFormatFilterChanged;
            view.OnTreeNodeSelected = TreeNodeSelected;
            view.OnTreeNodeExpanded = TreeNodeExpanded;
            view.OnTreeNodeExpandable = TreeNodeExpandable;
            view.OnTreeNodeDoubleClicked = TreeNodeDoubleClicked;
            view.OnAddToPlaylist = AddToPlaylist;
            view.OnRemoveFromLibrary = RemoveFromLibrary;
            view.OnDeleteFromHardDisk = DeleteFromHardDisk;

            _messageHub.Subscribe<AudioFileCacheUpdatedMessage>(AudioFileCacheUpdated);

            Initialize();
        }

        private void Initialize()
        {
            RefreshLibraryBrowser();
        }

        private void RefreshLibraryBrowser()
        {
            // Start by adding the first level nodes
            var firstLevelNodes = GetFirstLevelNodes();
            
            // Check if we need to restore the state from a previous session
            if (AppConfigManager.Instance.Root.LibraryBrowser.IsAvailable)
            {
                // Refresh/expand the nodes needed for restoring the state
                var entityType = AppConfigManager.Instance.Root.LibraryBrowser.EntityType;
                var query = AppConfigManager.Instance.Root.LibraryBrowser.Query;
                switch (entityType)
                {
                    case LibraryBrowserEntityType.AllSongs:
                    case LibraryBrowserEntityType.Dummy:
                    case LibraryBrowserEntityType.Artists:
                    case LibraryBrowserEntityType.Albums:
                        var node = firstLevelNodes.FirstOrDefault(x => x.EntityType == entityType);
                        View.RefreshLibraryBrowser(firstLevelNodes);
                        View.RefreshLibraryBrowserSelectedNode(node);
                        break;
                    case LibraryBrowserEntityType.Artist:
                    case LibraryBrowserEntityType.ArtistAlbum:
                        var artistsNode = firstLevelNodes.FirstOrDefault(x => x.EntityType == LibraryBrowserEntityType.Artists);
                        var artistNodes = GetArtistNodes(Filter);
                        artistsNode.SubItems.AddRange(artistNodes);
                        var artistNode = artistNodes.FirstOrDefault(x => string.Compare(x.Query.ArtistName, query.ArtistName, true) == 0);
                        
                        LibraryBrowserEntity nodeSelected = null;
                        if (entityType == LibraryBrowserEntityType.Artist)
                        {
                            nodeSelected = artistNode;
                        } 
                        else if (entityType == LibraryBrowserEntityType.ArtistAlbum)
                        {
                            if (artistNode != null)
                            {
                                var artistAlbums = GetArtistAlbumNodes(Filter, query.ArtistName);
                                artistNode.SubItems.AddRange(artistAlbums);
                                nodeSelected = artistAlbums.FirstOrDefault(
                                    x => string.Compare(x.Query.ArtistName, query.ArtistName, true) == 0 &&
                                    string.Compare(x.Query.AlbumTitle, query.AlbumTitle, true) == 0);
                            }
                        }
                        
                        View.RefreshLibraryBrowser(firstLevelNodes);
                        if(nodeSelected != null)
                            View.RefreshLibraryBrowserSelectedNode(nodeSelected);
                        break;
                    case LibraryBrowserEntityType.Album:
                        var albumsNode = firstLevelNodes.FirstOrDefault(x => x.EntityType == LibraryBrowserEntityType.Albums);
                        var albumNodes = GetAlbumNodes(Filter);
                        albumsNode.SubItems.AddRange(albumNodes);
                        var selectedAlbum = albumNodes.FirstOrDefault(x => string.Compare(x.Query.AlbumTitle, query.AlbumTitle, true) == 0);
                        
                        View.RefreshLibraryBrowser(firstLevelNodes);
                        if(selectedAlbum != null)
                            View.RefreshLibraryBrowserSelectedNode(selectedAlbum);
                        break;
                }
            } 
            else
            {
                View.RefreshLibraryBrowser(firstLevelNodes);
            }
        }

        private void AudioFileCacheUpdated(AudioFileCacheUpdatedMessage message)
        {
            RefreshLibraryBrowser();
        }

		/// <summary>
		/// Call this method when the Audio File Format combo box selected value has changed.
		/// </summary>
		/// <param name='format'>Audio file format</param>
		public void AudioFileFormatFilterChanged(AudioFileFormat format)
		{
            //Tracing.Log("LibraryBrowserPresenter.AudioFileFormatFilterChanged -- Getting first level nodes and refreshing view...");
			Filter = format;
			View.RefreshLibraryBrowser(GetFirstLevelNodes());
		}
		
		/// <summary>
		/// Call this method when a tree node has been selected to update the Song Browser.
		/// </summary>
		/// <param name='entity'>Library Browser entity</param>
		public void TreeNodeSelected(LibraryBrowserEntity entity)
		{
            //Tracing.Log("LibraryBrowserPresenter.TreeNodeSelected -- Broadcasting LibraryBrowserItemSelected message (" + entity.Title + ")...");
            
            // Store selection for reloading the Library Browser state later (i.e. startup)
            AppConfigManager.Instance.Root.LibraryBrowser.IsAvailable = true;
            AppConfigManager.Instance.Root.LibraryBrowser.EntityType = entity.EntityType;
            AppConfigManager.Instance.Root.LibraryBrowser.Query = entity.Query;
            AppConfigManager.Instance.Save();
            
            _messageHub.PublishAsync(new LibraryBrowserItemSelectedMessage(this){
                Item = entity
            });
		}
		
		/// <summary>
		/// Call this method when the tree node has expanded to fetch the additional tree nodes to add to the tree.
		/// The view is updated when the data has been extracted from the database.
		/// </summary>
		/// <param name='entity'>Library Browser entity</param>
		/// <param name='userData'>User data (i.e. tree node object)</param>
		public void TreeNodeExpanded(LibraryBrowserEntity entity, object userData)
        {
            if (entity.EntityType == LibraryBrowserEntityType.Artists)
            {
                //Tracing.Log("LibraryBrowserPresenter.TreeNodeExpanded -- Getting Artist nodes and refreshing view (RefreshLibraryBrowserNode)...");
                View.RefreshLibraryBrowserNode(
                    entity,
                    GetArtistNodes(Filter),
                    userData
                );
            } 
            else if (entity.EntityType == LibraryBrowserEntityType.Albums)
            {
                //Tracing.Log("LibraryBrowserPresenter.TreeNodeExpanded -- Getting Album nodes and refreshing view (RefreshLibraryBrowserNode)...");
                View.RefreshLibraryBrowserNode(
                    entity,
                    GetAlbumNodes(Filter),
                    userData
                );
            } 
            else if (entity.EntityType == LibraryBrowserEntityType.Artist)
            {
                //Tracing.Log("LibraryBrowserPresenter.TreeNodeExpanded -- Getting ArtistAlbum nodes and refreshing view (RefreshLibraryBrowserNode)...");
                View.RefreshLibraryBrowserNode(
                    entity,
                    GetArtistAlbumNodes(Filter, entity.Query.ArtistName),
                    userData
                );
            }
		}

        /// <summary>
        /// Call this method on Mac OS X when adding the subitems of the LibraryBrowserDataSource.
        /// This method returns the list of nodes to add to the NSOutlineView.
        /// </summary>
        /// <param name='entity'>Library Browser entity</param>
        /// <returns>List of nodes to ad to the NSOutlineView</returns>
        public IEnumerable<LibraryBrowserEntity> TreeNodeExpandable(LibraryBrowserEntity entity)
        {
            switch (entity.EntityType)
            {
                case LibraryBrowserEntityType.Artists:
                    return GetArtistNodes(Filter);
                case LibraryBrowserEntityType.Albums:
                    return GetAlbumNodes(Filter);
                case LibraryBrowserEntityType.Artist:
                    return GetArtistAlbumNodes(Filter, entity.Query.ArtistName);
            }

            return null;
        }

	    /// <summary>
		/// Call this method when the tree node has been double clicked. 
		/// This will start a new playlist in the Player presenter.
		/// </summary>
		/// <param name='entity'>Library Browser entity</param>
		public void TreeNodeDoubleClicked(LibraryBrowserEntity entity)
		{
            //Tracing.Log("LibraryBrowserPresenter.TreeNodeDoubleClicked -- Publishing LibraryBrowserItemDoubleClickedMessageay with item " + entity.Title);
            _messageHub.PublishAsync(new LibraryBrowserItemDoubleClickedMessage(this){
                Query = entity.Query
            });
		}

        private void AddToPlaylist(LibraryBrowserEntity entity)
        {
            try
            {
                var audioFiles = _libraryService.SelectAudioFiles(entity.Query);
                _playerService.CurrentPlaylist.AddItems(audioFiles);
            }
            catch(Exception ex)
            {
                View.LibraryBrowserError(ex);
            }
        }

        private void RemoveFromLibrary(LibraryBrowserEntity entity)
        {
            try
            {
                // TODO: Optimize this by creating a single SQL query instead of a query for each file
                var audioFiles = _libraryService.SelectAudioFiles(entity.Query);
                foreach(var audioFile in audioFiles)
                    _libraryService.DeleteAudioFile(audioFile.Id);
            }
            catch(Exception ex)
            {
                View.LibraryBrowserError(ex);
            }
        }

        private void DeleteFromHardDisk(LibraryBrowserEntity entity)
        {
            try
            {
                // TODO: Optimize this by creating a single SQL query instead of a query for each file
                var audioFiles = _libraryService.SelectAudioFiles(entity.Query);
                foreach(var audioFile in audioFiles)
                {
                    _libraryService.DeleteAudioFile(audioFile.Id);
                    File.Delete(audioFile.FilePath);
                }
            }
            catch(Exception ex)
            {
                View.LibraryBrowserError(ex);
            }
        }

		/// <summary>
		/// Returns the first level nodes of the library browser.
		/// </summary>
		/// <returns>
		/// First level nodes.
		/// </returns>
		private IEnumerable<LibraryBrowserEntity> GetFirstLevelNodes()
		{
			var list = new List<LibraryBrowserEntity>();
            list.Add(new LibraryBrowserEntity(){
                Title = "All Songs",
                EntityType = LibraryBrowserEntityType.AllSongs
            });           
			list.Add(new LibraryBrowserEntity(){
				Title = "Artists",
				EntityType = LibraryBrowserEntityType.Artists,
				SubItems = new List<LibraryBrowserEntity>(){ new LibraryBrowserEntity() { EntityType = LibraryBrowserEntityType.Dummy, Title = "dummy" }} // dummy node
			});
			list.Add(new LibraryBrowserEntity(){
				Title = "Albums",
				EntityType = LibraryBrowserEntityType.Albums,
				SubItems = new List<LibraryBrowserEntity>(){ new LibraryBrowserEntity() { EntityType = LibraryBrowserEntityType.Dummy, Title = "dummy" }} // dummy node
			});
			
			return list;
		}
	
		/// <summary>
		/// Returns a list of distinct artist names.
		/// </summary>
		/// <returns>
		/// List of artist names.
		/// </returns>
		private IEnumerable<LibraryBrowserEntity> GetArtistNodes(AudioFileFormat format)
		{
			var list = new List<LibraryBrowserEntity>();
			var artists = _libraryService.SelectDistinctArtistNames(format);
			foreach(string artist in artists)
			{
				list.Add(new LibraryBrowserEntity(){
					Title = artist,
					EntityType = LibraryBrowserEntityType.Artist,
                    Query = new LibraryQuery(){
						Format = format,
						ArtistName = artist
					},
					SubItems = new List<LibraryBrowserEntity>(){ new LibraryBrowserEntity() { EntityType = LibraryBrowserEntityType.Dummy, Title = "dummy" }} // dummy node					
				});
			}
			
			return list;
		}
	
		/// <summary>
		/// Returns a list of distinct album titles.
		/// </summary>
		/// <param name='format'>Audio file format</param>		
		/// <returns>List of album titles</returns>		
		private IEnumerable<LibraryBrowserEntity> GetAlbumNodes(AudioFileFormat format)
		{
			return GetArtistAlbumNodes(format, string.Empty);	
		}
	
		/// <summary>
		/// Returns a list of distinct album titles of a specific artist.
		/// </summary>
		/// <param name='format'>Audio file format</param>
		/// <param name='artistName'>Artist name</param>
		/// <returns>List of album titles</returns>		
		private IEnumerable<LibraryBrowserEntity> GetArtistAlbumNodes(AudioFileFormat format, string artistName)
		{
			var list = new List<LibraryBrowserEntity>();
		    var albumTitles = _libraryService.SelectDistinctAlbumTitles(format, artistName);
		    var albums = albumTitles.SelectMany(keyValue => keyValue.Value).OrderBy(x => x).ToList();
			foreach(string album in albums)
			{
				list.Add(new LibraryBrowserEntity(){
					Title = album,
                    EntityType = string.IsNullOrEmpty(artistName) ? LibraryBrowserEntityType.Album : LibraryBrowserEntityType.ArtistAlbum,
                    Query = new LibraryQuery(){
						Format = format,
						ArtistName = artistName,
						AlbumTitle = album						
					}
				});
			}
			
			return list;
		}
	}
}
