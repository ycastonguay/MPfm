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
		public AudioFileFormat Filter { get; private set; }
		private ILibraryBrowserView view = null;		
		private readonly ILibraryService libraryService = null;		
		private readonly IPlayerPresenter playerPresenter = null;
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.LibraryBrowserPresenter"/> class.
		/// </summary>
		public LibraryBrowserPresenter(IPlayerPresenter playerPresenter, ILibraryService libraryService)
		{
			// Validate parameters
			if(playerPresenter == null)			
				throw new ArgumentNullException("The playerPresenter parameter is null!");		
			if(libraryService == null)			
				throw new ArgumentNullException("The libraryService parameter is null!");
						
			// Set properties			
			this.libraryService = libraryService;
			this.playerPresenter = playerPresenter;
			
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
		
		public void SetAudioFileFormatFilter(AudioFileFormat format)
		{
			// Refresh view (first level nodes)
			view.RefreshLibraryBrowser(GetFirstLevelNodes());
		}
		
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
				view.RefreshLibraryBrowserNode(entity, GetArtistAlbumNodes(Filter, entity.Filter.ArtistName), userData);
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
					Filter = new SongBrowserFilterEntity(){
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
					Filter = new SongBrowserFilterEntity(){
						Format = format,
						ArtistName = artistName,
						AlbumTitle = album						
					}
				});
			}
			
			return list;
		}
		
//		public void DoubleClickItem(SongBrowserFilterEntity entity)
//		{
//			playerPresenter.Play();
//		}
		
		#endregion
	}
}

