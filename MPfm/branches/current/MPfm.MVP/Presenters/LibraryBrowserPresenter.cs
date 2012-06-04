//
// LibraryBrowserPresenter.cs: Library browser presenter.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using MPfm.Core;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using AutoMapper;

namespace MPfm.MVP
{
	/// <summary>
	/// Library browser presenter.
	/// </summary>
	public class LibraryBrowserPresenter : ILibraryBrowserPresenter
	{
		private ILibraryBrowserView view = null;		
		private readonly ILibraryService libraryService = null;
		private readonly IAudioFileCacheService audioFileCacheService = null;
		private readonly IPlayerPresenter playerPresenter = null;
		private readonly ISongBrowserPresenter songBrowserPresenter = null;
		
		public AudioFileFormat Filter { get; private set; }
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.MVP.LibraryBrowserPresenter"/> class.
		/// </summary>
		public LibraryBrowserPresenter(IPlayerPresenter playerPresenter, 
		                               ISongBrowserPresenter songBrowserPresenter, 
		                               ILibraryService libraryService,
		                               IAudioFileCacheService audioFileCacheService)
		{
			// Validate parameters
			if(playerPresenter == null)			
				throw new ArgumentNullException("The playerPresenter parameter is null!");
			if(songBrowserPresenter == null)			
				throw new ArgumentNullException("The songBrowserPresenter parameter is null!");
			if(libraryService == null)			
				throw new ArgumentNullException("The libraryService parameter is null!");
			if(audioFileCacheService == null)			
				throw new ArgumentNullException("The audioFileCacheService parameter is null!");			
						
			// Set properties
			this.playerPresenter = playerPresenter;
			this.songBrowserPresenter = songBrowserPresenter;
			this.libraryService = libraryService;
			this.audioFileCacheService = audioFileCacheService;			
			
			// Set default filter
			Filter = AudioFileFormat.All;
		}

		#endregion		
		
		#region ILibraryBrowserPresenter implementation
		
		/// <summary>
		/// Binds the view to its implementation.
		/// </summary>
		/// <param name='view'>Library Browser view implementation</param>	
		public void BindView(ILibraryBrowserView view)
		{
			// Validate parameters 
			if(view == null)			
				throw new ArgumentNullException("The view parameter is null!");
			
			// Set property
			this.view = view;
			
			// Refresh view (first level nodes)
			view.RefreshLibraryBrowser(GetFirstLevelNodes());
		}
		
		/// <summary>
		/// Call this method when the Audio File Format combo box selected value has changed.
		/// </summary>
		/// <param name='format'>Audio file format</param>
		public void AudioFileFormatFilterChanged(AudioFileFormat format)
		{
			// Refresh view (first level nodes)
			this.Filter = format;
			view.RefreshLibraryBrowser(GetFirstLevelNodes());
		}
		
		/// <summary>
		/// Call this method when a tree node has been selected to update the Song Browser.
		/// </summary>
		/// <param name='entity'>Library Browser entity</param>
		public void TreeNodeSelected(LibraryBrowserEntity entity)
		{
			songBrowserPresenter.ChangeQuery(entity.Query);			
		}
		
		/// <summary>
		/// Call this method when the tree node has expanded to fetch the additional tree nodes to add to the tree.
		/// The view is updated when the data has been extracted from the database.
		/// </summary>
		/// <param name='entity'>Library Browser entity</param>
		/// <param name='userData'>User data (i.e. tree node object)</param>
		public void TreeNodeExpanded(LibraryBrowserEntity entity, object userData)
		{
			// Check node type
			if(entity.Type == LibraryBrowserEntityType.Artists)
			{
				view.RefreshLibraryBrowserNode(entity, GetArtistNodes(Filter), userData);
			}
			else if(entity.Type == LibraryBrowserEntityType.Albums)
			{
				view.RefreshLibraryBrowserNode(entity, GetAlbumNodes(Filter), userData);
			}
			else if(entity.Type == LibraryBrowserEntityType.Artist)
			{
				view.RefreshLibraryBrowserNode(entity, GetArtistAlbumNodes(Filter, entity.Query.ArtistName), userData);
			}
		}
		
		/// <summary>
		/// Call this method when the tree node has been double clicked. 
		/// This will start a new playlist in the Player presenter.
		/// </summary>
		/// <param name='entity'>Library Browser entity</param>
		public void TreeNodeDoubleClicked(LibraryBrowserEntity entity)
		{
			playerPresenter.Play(audioFileCacheService.SelectAudioFiles(entity.Query));
		}
		
		#endregion
		
		#region Data Methods
		
		/// <summary>
		/// Returns the first level nodes of the library browser.
		/// </summary>
		/// <returns>
		/// First level nodes.
		/// </returns>
		private IEnumerable<LibraryBrowserEntity> GetFirstLevelNodes()
		{
			List<LibraryBrowserEntity> list = new List<LibraryBrowserEntity>();
			
			list.Add(new LibraryBrowserEntity(){
				Title = "Artists",
				Type = LibraryBrowserEntityType.Artists,
				SubItems = new List<LibraryBrowserEntity>(){ new LibraryBrowserEntity() { Type = LibraryBrowserEntityType.Dummy, Title = "dummy" }} // dummy node
			});
			
			list.Add(new LibraryBrowserEntity(){
				Title = "Albums",
				Type = LibraryBrowserEntityType.Albums,
				SubItems = new List<LibraryBrowserEntity>(){ new LibraryBrowserEntity() { Type = LibraryBrowserEntityType.Dummy, Title = "dummy" }} // dummy node
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
			List<LibraryBrowserEntity> list = new List<LibraryBrowserEntity>();
			
			List<string> artists = libraryService.SelectDistinctArtistNames(format);
			foreach(string artist in artists)
			{
				list.Add(new LibraryBrowserEntity(){
					Title = artist,
					Type = LibraryBrowserEntityType.Artist,
					Query = new SongBrowserQueryEntity(){
						Format = format,
						ArtistName = artist
					},
					SubItems = new List<LibraryBrowserEntity>(){ new LibraryBrowserEntity() { Type = LibraryBrowserEntityType.Dummy, Title = "dummy" }} // dummy node					
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
			// Declare variables
			List<LibraryBrowserEntity> list = new List<LibraryBrowserEntity>();
			List<string> albums = new List<string>();
			
			// Get distinct album titles
			Dictionary<string, List<string>> albumTitles = libraryService.SelectDistinctAlbumTitles(format, artistName);
				       
            // For each song                    
            foreach (KeyValuePair<string, List<string>> keyValue in albumTitles)
            {
                foreach (string albumTitle in keyValue.Value)
                {
                    albums.Add(albumTitle);
                }
            }

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
		
		#endregion
	}
}

