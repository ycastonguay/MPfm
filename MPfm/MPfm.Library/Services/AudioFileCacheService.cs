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
using MPfm.Sound.AudioFiles;
using MPfm.Library.Services.Interfaces;
using TinyMessenger;
using MPfm.Library.Objects;
using MPfm.Library.Messages;

namespace MPfm.Library.Services
{	
	/// <summary>
	/// Service used for interacting with the audio file metadata cache.
	/// </summary>
	public class AudioFileCacheService : IAudioFileCacheService
	{
	    private readonly ITinyMessengerHub _messengerHub;
	    private readonly ILibraryService _libraryService;

	    public List<AudioFile> AudioFiles { get; private set; }

	    public AudioFileCacheService(ITinyMessengerHub messengerHub, ILibraryService libraryService)
		{
		    _messengerHub = messengerHub;
		    _libraryService = libraryService;
		}

	    /// <summary>
		/// Refreshes the audio file metadata cache.
		/// </summary>
		public void RefreshCache()
		{
            // Warn any subscribers that the audio file cache has been updated (i.e. library/song browser presenters)
            Console.WriteLine("AudioFileCacheService - Refreshing cache...");
            AudioFiles = _libraryService.SelectAudioFiles().ToList();
            Console.WriteLine("AudioFileCacheService - Cache has {0} files.", AudioFiles.Count);
            _messengerHub.PublishAsync(new AudioFileCacheUpdatedMessage(this));
		}

        /// <summary>
        /// Removes the audio files of an artist and/or album from the cache. This does not delete the files from the database.
        /// </summary>
        /// <param name="artistName">Artist name</param>
        /// <param name="albumTitle">Album title</param>
        public void RemoveAudioFiles(string artistName, string albumTitle)
        {
            AudioFiles.RemoveAll(x => x.ArtistName.ToUpper() == artistName.ToUpper() && x.AlbumTitle.ToUpper() == albumTitle.ToUpper());
            _messengerHub.PublishAsync(new AudioFileCacheUpdatedMessage(this));
        }

        /// <summary>
        /// Removes a single audio file from the cache.
        /// </summary>
        /// <param name="audioFileId">Audio file identifier</param>
        public void RemoveAudioFile(Guid audioFileId)
        {
            AudioFiles.RemoveAll(x => x.Id == audioFileId);
            _messengerHub.PublishAsync(new AudioFileCacheUpdatedMessage(this));
        }

		/// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="query">Song browser query</param>
        /// <returns>List of AudioFiles</returns>
        public IEnumerable<AudioFile> SelectAudioFiles(LibraryQuery query)
        {
            IEnumerable<AudioFile> queryAudioFiles = null;

            if (String.IsNullOrEmpty(query.OrderBy))
            {
                queryAudioFiles = from s in AudioFiles
                                  orderby s.ArtistName, s.AlbumTitle, s.FileType, s.DiscNumber, s.TrackNumber
                                  select s;
            }
            else
            {
                if (query.OrderByAscending)
                    queryAudioFiles = from s in AudioFiles
                                      orderby GetPropertyValue(s, query.OrderBy)
                                      select s;
                else
                    queryAudioFiles = from s in AudioFiles
                                      orderby GetPropertyValue(s, query.OrderBy) descending
                                      select s;                        
            }

            if (!String.IsNullOrEmpty(query.ArtistName))
                queryAudioFiles = queryAudioFiles.Where(s => s.ArtistName == query.ArtistName);                    

            if (!String.IsNullOrEmpty(query.AlbumTitle))
                queryAudioFiles = queryAudioFiles.Where(s => s.AlbumTitle == query.AlbumTitle);                    

            if (!String.IsNullOrEmpty(query.SearchTerms))
            {
                string[] searchTermsSplit = query.SearchTerms.Split(new string[] { " " }, StringSplitOptions.None);
                foreach (string searchTerm in searchTermsSplit)
                    queryAudioFiles = queryAudioFiles.Where(s => s.ArtistName.ToUpper().Contains(searchTerm.ToUpper()) ||
                                                                 s.AlbumTitle.ToUpper().Contains(searchTerm.ToUpper()) ||
                                                                 s.Title.ToUpper().Contains(searchTerm.ToUpper()));
            }

            if (query.Format != AudioFileFormat.All)
                queryAudioFiles = queryAudioFiles.Where(s => s.FileType == query.Format);

            return queryAudioFiles.ToList();
        }
					
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

