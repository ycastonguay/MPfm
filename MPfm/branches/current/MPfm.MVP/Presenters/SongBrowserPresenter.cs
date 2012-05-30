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
		
        /// <summary>
        /// Selects all the audio files from the cache.
        /// </summary>
        /// <returns>List of AudioFiles</returns>
        public IEnumerable<AudioFile> SelectAudioFiles()
        {
            return SelectAudioFiles(AudioFileFormat.All, string.Empty, true, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <returns>List of AudioFiles</returns>
        public IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat)
        {
            return SelectAudioFiles(audioFileFormat, string.Empty, true, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <param name="orderBy">Order by field name</param>
        /// <param name="orderByAscending">Order by (ascending) field name</param>
        /// <returns>List of AudioFiles</returns>
        public IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending)
        {
            return SelectAudioFiles(audioFileFormat, orderBy, orderByAscending, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <param name="orderBy">Order by field name</param>
        /// <param name="orderByAscending">Order by (ascending) field name</param>
        /// <param name="artistName">Artist name</param>
        /// <returns>List of AudioFiles</returns>
        public IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending, string artistName)
        {
            return SelectAudioFiles(audioFileFormat, orderBy, orderByAscending, artistName, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <param name="orderBy">Order by field name</param>
        /// <param name="orderByAscending">Order by (ascending) field name</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="albumTitle">Album title</param>        
        /// <returns>List of AudioFiles</returns>
        public IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending, string artistName, string albumTitle)
        {
            return SelectAudioFiles(audioFileFormat, orderBy, orderByAscending, artistName, albumTitle, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <param name="orderBy">Order by field name</param>
        /// <param name="orderByAscending">Order by (ascending) field name</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="albumTitle">Album title</param>
        /// <param name="searchTerms">Search terms</param>
        /// <returns>List of AudioFiles</returns>
        public IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending, string artistName, string albumTitle, string searchTerms)
        {			
			return audioFileCacheService.SelectAudioFiles(audioFileFormat, orderBy, orderByAscending, artistName, albumTitle, searchTerms);
        }
		
		#endregion

	}
}

