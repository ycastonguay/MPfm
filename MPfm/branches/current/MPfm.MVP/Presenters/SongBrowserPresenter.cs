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
		private ISongBrowserView view = null;
		private readonly IAudioFileCacheService audioFileCacheService = null;
		private readonly ILibraryService libraryService = null;
		private readonly IPlayerPresenter playerPresenter = null;
		
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
		}

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
		
		public void ChangeQuery(SongBrowserQueryEntity query)
		{
			// Set query
			this.Query = query;
			
			// Refresh view
			IEnumerable<AudioFile> audioFiles = audioFileCacheService.SelectAudioFiles(query);
			view.RefreshSongBrowser(audioFiles);
		}
		
		#endregion

	}

}

