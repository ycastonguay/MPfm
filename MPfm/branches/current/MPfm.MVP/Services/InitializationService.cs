//
// InitializationService.cs: Service used for creating/updating the database and log files.
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
using System.Linq;
using System.Diagnostics;
using MPfm.Core;

namespace MPfm.MVP
{	
	/// <summary>
	/// Service used for creating/updating the database and log files.
	/// </summary>
	public class InitializationService : IInitializationService
	{
		// Private variables		
		private Stream fileTracing = null;
        private TextWriterTraceListener textTraceListener = null;
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.MVP.InitializationService"/> class.
		/// </summary>
		public InitializationService()
		{
		}		
		
		/// <summary>
		/// Creates the configuration.
		/// </summary>
		public void CreateConfiguration()
		{
            // Check if trace file exists
            if (!File.Exists(ConfigurationHelper.LogFilePath))
            {
                // Create file
                fileTracing = File.Create(ConfigurationHelper.LogFilePath);
            }
            else
            {
                // Open file
                fileTracing = File.Open(ConfigurationHelper.LogFilePath, FileMode.Append);
            }
            textTraceListener = new TextWriterTraceListener(fileTracing);
            Trace.Listeners.Add(textTraceListener);

			// Check for configuration file
		}
		
		
		/// <summary>
		/// Creates and initializes the library.
		/// </summary>
		/// <exception cref='Exception'>
		/// Represents errors that occur during application execution.
		/// </exception>
		public void CreateLibrary()
		{
            try
            {
                // Check if the database file exists
                if (!File.Exists(ConfigurationHelper.DatabaseFilePath))
                {                    
                    // Create database file
                    //frmSplash.SetStatus("Creating database file...");
                    MPfm.Library.Library.CreateDatabaseFile(ConfigurationHelper.DatabaseFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing MPfm: Could not create database file!", ex);
            }
			
            try
            {
                // Check current database version
                string databaseVersion = MPfm.Library.Library.GetDatabaseVersion(ConfigurationHelper.DatabaseFilePath);

                // Extract major/minor
                string[] currentVersionSplit = databaseVersion.Split('.');

                // Check integrity of the setting value (should be split in 2)
                if (currentVersionSplit.Length != 2)
                {
                    throw new Exception("Error fetching database version; the setting value is invalid!");
                }

                int currentMajor = 0;
                int currentMinor = 0;
                int.TryParse(currentVersionSplit[0], out currentMajor);
                int.TryParse(currentVersionSplit[1], out currentMinor);

                // Is this earlier than 1.04?
                if (currentMajor == 1 && currentMinor < 4)
                {
                    // Set buffer size
                    //Config.Audio.Mixer.BufferSize = 1000;
                }

                // Check if the database needs to be updated
                MPfm.Library.Library.CheckIfDatabaseVersionNeedsToBeUpdated(ConfigurationHelper.DatabaseFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing MPfm: The MPfm database could not be updated!", ex);
            }
			
		    try
            {
                // Load library
                Tracing.Log("Main form init -- Loading library...");
                //frmSplash.SetStatus("Loading library...");                
                //library = new Library.Library(databaseFilePath);
				//library.OnUpdateLibraryFinished += HandleLibraryOnUpdateLibraryFinished;
				//library.OnUpdateLibraryProgress += HandleLibraryOnUpdateLibraryProgress;
            }
            catch
            {
				throw;
                // Set error in splash and hide splash
                //frmSplash.SetStatus("Error initializing library!");
                //frmSplash.HideSplash();

                // Display message box with error
                //this.TopMost = true;
                //MessageBox.Show("There was an error while initializing the library.\nYou can delete the MPfm.Database.db file in the MPfm application data folder (" + applicationDataFolderPath + ") to reset the library.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error initializing library!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Tracing.Log("Main form init -- Library init error: " + ex.Message + "\nStack trace: " + ex.StackTrace);
                
                // Exit application
                //Application.Exit();
                //return;
            }
		}

		#endregion
		
	}
	
}

