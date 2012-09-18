//
// SongBrowserPresenter.cs: Song browser presenter.
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
using Ninject;

namespace MPfm.MVP
{
	/// <summary>
	/// Song browser presenter.
	/// </summary>
	public class SongBrowserPresenter : ISongBrowserPresenter
	{
		ISongBrowserView view = null;
		readonly IAudioFileCacheService audioFileCacheService = null;
		readonly ILibraryService libraryService = null;
		readonly IPlayerPresenter playerPresenter = null;      
		
		public SongBrowserQueryEntity Query { get; private set; }
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.SongBrowserPresenter"/> class.
		/// </summary>
		public SongBrowserPresenter(IPlayerPresenter playerPresenter, 
		                            ILibraryService libraryService,
		                            IAudioFileCacheService audioFileCacheService)
		{
			// Validate parameters
			if(playerPresenter == null)
				throw new ArgumentNullException("The playerPresenter parameter is null!");
			if(libraryService == null)				
				throw new ArgumentNullException("The libraryService parameter is null!");
			if(audioFileCacheService == null)				
				throw new ArgumentNullException("The audioFileCacheService parameter is null!");
			
			// Set properties
			this.playerPresenter = playerPresenter;
			this.libraryService = libraryService;
			this.audioFileCacheService = audioFileCacheService;
            //this.playerPresenter.Player.OnPlaylistIndexChanged += HandleOnPlaylistIndexChanged;

            // Create default query
            Query = new SongBrowserQueryEntity();
		}

//        void HandleOnPlaylistIndexChanged(PlayerPlaylistIndexChangedData data)
//        {
//            if(data.IsPlaybackStopped)
//                return;
//
//            // Update view with new song            
//            view.RefreshCurrentlyPlayingSong(data.AudioFileEnded);
//        }

		#endregion		
		
		#region ISongBrowserPresenter implementation
		
		/// <summary>
		/// Binds the view to its implementation.
		/// </summary>
		/// <param name='view'>Song Browser view implementation</param>	
		public void BindView(ISongBrowserView view)
		{
			// Validate parameters 
			if(view == null)			
				throw new ArgumentNullException("The view parameter is null!");
			
			// Set property
			this.view = view;
		}
		
		/// <summary>
		/// Changes the Song Browser query and updates the Song Browser view.
		/// </summary>
		/// <param name='query'>New query</param>
		public void ChangeQuery(SongBrowserQueryEntity query)
		{
			// Set query
			this.Query = query;
			
			// Get audio files
            Tracing.Log("SongBrowserPresenter.ChangeQuery -- Getting audio files (Format: " + query.Format.ToString() + 
                        " | Artist: " + query.ArtistName + " | Album: " + query.AlbumTitle + " | OrderBy: " + query.OrderBy + 
                        " | OrderByAscending: " + query.OrderByAscending.ToString() + " | Search terms: " + query.SearchTerms + ")");
			IEnumerable<AudioFile> audioFiles = audioFileCacheService.SelectAudioFiles(query);

            // Refresh view
            Tracing.Log("SongBrowserPresenter.ChangeQuery -- Refreshing song browser...");
			view.RefreshSongBrowser(audioFiles);
		}
		
		/// <summary>
		/// Call this method when the table row has been double clicked.
		/// This will start a new playlist in the Player presenter.
		/// </summary>
		/// <param name='audioFile'>Audio file</param>
		public void TableRowDoubleClicked(AudioFile audioFile)
		{			
            Tracing.Log("SongBrowserPresenter.TableRowDoubleClicked -- Calling PlayerPresenter.Play with item " + audioFile.Title + "...");
			playerPresenter.Play(audioFileCacheService.SelectAudioFiles(Query), audioFile.FilePath);
		}
		
		#endregion

	}
}

