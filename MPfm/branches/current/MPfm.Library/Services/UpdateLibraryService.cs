//
// UpdateLibraryService.cs: This service is used for updating audio files to 
//                          the library database.
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
using MPfm.Sound;

namespace MPfm.Library
{
    /// <summary>
    /// Interface for the UpdateLibraryService class.
    /// </summary>
    public class UpdateLibraryService : IUpdateLibraryService
    {
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
		public UpdateLibraryService(MPfmGateway gateway)
		{
			// Check for null
			if(gateway == null)
				throw new ArgumentNullException("The gateway parameter cannot be null!");
				
			// Set gateway
			this.gateway = gateway;
		}
		
		#region IUpdateLibraryService implementation

		public void AddFiles(List<string> filePaths)
		{
			//gateway.InsertAudioFile(
		}
	
		public void AddFolder(string folderPath, bool recursive)
		{		
            // Check if the folder is already part of a configured folder
            bool folderFound = false;

            // Get the list of folders from the database                
            List<Folder> folders = gateway.SelectFolders();

            // Search through folders if the base found can be found
            foreach (Folder folder in folders)
            {
                // Check if the base path is found in the configured path
                if (folderPath.Contains(folder.FolderPath))
                {
                    // Set flag
                    folderFound = true;
                    break;
                }
            }

            // Check if the user has entered a folder deeper than those configured
            // i.e. The user enters F:\FLAC when F:\FLAC\Brian Eno is configured
            foreach (Folder folder in folders)
            {
                // Check if the configured path is part of the specified path
                if (folder.FolderPath.Contains(folderPath))
                {
                    // Delete this configured folder                        
                    gateway.DeleteFolder(folder.FolderId);
                }
            }

            // Add the folder to the list of configured folders
            if (!folderFound)
            {
                // Add folder to database                    
                gateway.InsertFolder(folderPath, true);
			}			
			
			// Update library
			UpdateLibrary();
		}
	
		public void UpdateLibrary()
		{
			// Cycle through 
		}
		
		#endregion
		
    }
}
