//
// LibraryService.cs: This service is used for interacting with the library database.
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
using System.IO;
using MPfm.Sound;

namespace MPfm.Library
{
    /// <summary>
    /// Interface for the LibraryService class.
    /// </summary>
    public class LibraryService : ILibraryService
    {
		// Private variables
		private readonly IMPfmGateway gateway = null;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.Library.UpdateLibraryService"/> class.
		/// </summary>
		/// <param name='gateway'>
		/// MPfm Gateway.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
		/// </exception>
		public LibraryService(MPfmGateway gateway)
		{
			// Check for null
			if(gateway == null)
				throw new ArgumentNullException("The gateway parameter cannot be null!");
				
			// Set gateway
			this.gateway = gateway;
		}
		
		/// <summary>
		/// Removes the audio files with broken file paths (i.e. that do not exist anymore on the hard drive).
		/// </summary>
		public void RemoveAudioFilesWithBrokenFilePaths()
		{
            // Get all audio files
            List<AudioFile> files = gateway.SelectAudioFiles();

            // For each audio file
            for (int a = 0; a < files.Count; a++)
            {
                // If the file doesn't exist, delete the audio file from the database
                if (!File.Exists(files[a].FilePath))
                {
                    gateway.DeleteAudioFile(files[a].Id);
                }
            }
		}	
    }
}
