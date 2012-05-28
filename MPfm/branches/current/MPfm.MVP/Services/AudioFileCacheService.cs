//
// AudioFileCacheService.cs: Service used for interacting with the audio file metadata cache.
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
using MPfm.Library;
using MPfm.MVP;
using MPfm.Player;
using MPfm.Sound;
using AutoMapper;
using Ninject;

namespace MPfm.MVP
{	
	/// <summary>
	/// Service used for interacting with the audio file metadata cache.
	/// </summary>
	public class AudioFileCacheService : IAudioFileCacheService
	{
		private readonly ISongBrowserView view = null;
		private readonly ILibraryService service = null;
		
		private List<AudioFile> audioFiles = null;
		public List<AudioFile> AudioFiles
		{
			get
			{
				return audioFiles;
			}
		}
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.MVP.AudioFileCacheService"/> class.
		/// </summary>
		public AudioFileCacheService(ILibraryService service)
		{
			if(service == null)			
				throw new ArgumentNullException("The service parameter is null!");
						
			// Set properties
			this.service = service;
			
			// Load cache
			audioFiles = service.SelectAudioFiles().ToList();
		}

		#endregion		
		
		#region ISongBrowserPresenter implementation
		
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
            try
            {
                IEnumerable<AudioFile> queryAudioFiles = null;

                // Check for default order by (ignore ascending)
                if (String.IsNullOrEmpty(orderBy))
                {
                    // Set query
                    queryAudioFiles = from s in audioFiles
                                      orderby s.ArtistName, s.AlbumTitle, s.FileType, s.DiscNumber, s.TrackNumber
                                      select s;
                }
                else
                {
                    // Check for orderby ascending/descending
                    if (orderByAscending)
                    {
                        // Set query
                        queryAudioFiles = from s in audioFiles
                                          orderby GetPropertyValue(s, orderBy)
                                          select s;
                    }
                    else
                    {
                        // Set query
                        queryAudioFiles = from s in audioFiles
                                          orderby GetPropertyValue(s, orderBy) descending
                                          select s;                        
                    }
                }

                // Check if artistName is null
                if (!String.IsNullOrEmpty(artistName))
                {
                    // Add the artist condition to the query
                    queryAudioFiles = queryAudioFiles.Where(s => s.ArtistName == artistName);                    
                }

                // Check if albumTitle is null
                if (!String.IsNullOrEmpty(albumTitle))
                {
                    // Add the artist condition to the query
                    queryAudioFiles = queryAudioFiles.Where(s => s.AlbumTitle == albumTitle);                    
                }

                // Check if searchTerms is null
                if (!String.IsNullOrEmpty(searchTerms))
                {
                    // Split search terms
                    string[] searchTermsSplit = searchTerms.Split(new string[] { " " }, StringSplitOptions.None);
                    
                    // Loop through search terms
                    foreach (string searchTerm in searchTermsSplit)
                    {
                        // Add the artist condition to the query
                        queryAudioFiles = queryAudioFiles.Where(s => s.ArtistName.ToUpper().Contains(searchTerm.ToUpper()) ||
                                                                     s.AlbumTitle.ToUpper().Contains(searchTerm.ToUpper()) ||
                                                                     s.Title.ToUpper().Contains(searchTerm.ToUpper()));
                    }
                }

                // Check for audio file format filter
                if (audioFileFormat == AudioFileFormat.All)
                {
                    // 
                }
                else
                {
                    // Set filter by file type
                    queryAudioFiles = queryAudioFiles.Where(s => s.FileType == audioFileFormat);
                }

                //// Check for default order by
                //if (String.IsNullOrEmpty(orderBy))
                //{
                //    // Add order by
                //    querySongs = querySongs.OrderBy(s => s.ArtistName).ThenBy(s => s.AlbumTitle).ThenBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber);
                //}
                //else
                //{
                //    // Custom order by
                //    querySongs = querySongs.OrderBy("");
                //}

                // Execute query
                return queryAudioFiles.ToList();
            }
            catch (Exception ex)
            {
                //Tracing.Log(ex);
                throw;
            }
        }
		
		#endregion
					
		/// <summary>
        /// Fetches the property value of an object.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="property">Property</param>
        /// <returns>Value</returns>
        private static object GetPropertyValue(object obj, string property)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }
	}
	
}

