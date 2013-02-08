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
using System.Reflection;
using MPfm.MVP.Models;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Services
{	
	/// <summary>
	/// Service used for interacting with the audio file metadata cache.
	/// </summary>
	public class AudioFileCacheService : IAudioFileCacheService
	{
		private readonly ISongBrowserView view = null;
		private readonly ILibraryService libraryService = null;
		
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
		/// Initializes a new instance of the <see cref="AudioFileCacheService"/> class.
		/// </summary>
		public AudioFileCacheService(ILibraryService libraryService)
		{
			if(libraryService == null)			
				throw new ArgumentNullException("The libraryService parameter is null!");
						
			// Set properties
			this.libraryService = libraryService;
			
			// Refresh cache
			//RefreshCache();
		}

		#endregion		
		
		#region IAudioFileCacheService implementation
		
		/// <summary>
		/// Refreshes the audio file metadata cache.
		/// </summary>
		public void RefreshCache()
		{
			audioFiles = libraryService.SelectAudioFiles().ToList();
		}
        
		/// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="query">Song browser query</param>
        /// <returns>List of AudioFiles</returns>
        public IEnumerable<AudioFile> SelectAudioFiles(SongBrowserQueryEntity query)
        {
            try
            {
                IEnumerable<AudioFile> queryAudioFiles = null;

                // Check for default order by (ignore ascending)
                if (String.IsNullOrEmpty(query.OrderBy))
                {
                    // Set query
                    queryAudioFiles = from s in audioFiles
                                      orderby s.ArtistName, s.AlbumTitle, s.FileType, s.DiscNumber, s.TrackNumber
                                      select s;
                }
                else
                {
                    // Check for orderby ascending/descending
                    if (query.OrderByAscending)
                    {
                        // Set query
                        queryAudioFiles = from s in audioFiles
                                          orderby GetPropertyValue(s, query.OrderBy)
                                          select s;
                    }
                    else
                    {
                        // Set query
                        queryAudioFiles = from s in audioFiles
                                          orderby GetPropertyValue(s, query.OrderBy) descending
                                          select s;                        
                    }
                }

                // Check if artistName is null
                if (!String.IsNullOrEmpty(query.ArtistName))
                {
                    // Add the artist condition to the query
                    queryAudioFiles = queryAudioFiles.Where(s => s.ArtistName == query.ArtistName);                    
                }

                // Check if albumTitle is null
                if (!String.IsNullOrEmpty(query.AlbumTitle))
                {
                    // Add the artist condition to the query
                    queryAudioFiles = queryAudioFiles.Where(s => s.AlbumTitle == query.AlbumTitle);                    
                }

                // Check if searchTerms is null
                if (!String.IsNullOrEmpty(query.SearchTerms))
                {
                    // Split search terms
                    string[] searchTermsSplit = query.SearchTerms.Split(new string[] { " " }, StringSplitOptions.None);
                    
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
                if (query.Format == AudioFileFormat.All)
                {
                    // 
                }
                else
                {
                    // Set filter by file type
                    queryAudioFiles = queryAudioFiles.Where(s => s.FileType == query.Format);
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

