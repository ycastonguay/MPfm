//
// Playlist.cs: This file contains the class defining a playlist to be used with the Player.
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
using System.Collections;
using System.Collections.Generic;
using MPfm.Sound;
using MPfm.Core;

namespace MPfm.Sound
{
    /// <summary>
    /// Defines a playlist to be used with the Player.
    /// </summary>
    public class Playlist
    {
        #region Properties
        
        /// <summary>
        /// Private value for the Items property.
        /// </summary>
        private List<PlaylistItem> items = null;
        /// <summary>
        /// List of playlist items.
        /// </summary>
        public List<PlaylistItem> Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Private value for the CurrentItemIndex property.
        /// </summary>
        private int currentItemIndex = 0;
        /// <summary>
        /// Returns the current playlist item index.
        /// </summary>
        public int CurrentItemIndex
        {
            get
            {
                return currentItemIndex;
            }
        }

        /// <summary>
        /// Private value for the CurrentItem property.
        /// </summary>
        private PlaylistItem currentItem = null;
        /// <summary>
        /// Returns the current item.
        /// </summary>
        public PlaylistItem CurrentItem
        {
            get
            {
                return currentItem;
            }
        }

        /// <summary>
        /// Playlist file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Playlist format.
        /// </summary>
        public PlaylistFileFormat Format { get; set; }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Default constructor for the Playlist class.
        /// </summary>
        public Playlist()
        {
            items = new List<PlaylistItem>();
            currentItemIndex = 0;            
            FilePath = string.Empty;
            Format = PlaylistFileFormat.Unknown;
        }

        #endregion

        /// <summary>
        /// Loads a playlist (from any of the following formats: M3U, M3U8, PLS and XSPF).
        /// Note: long playlists may take a while to load using this method!
        /// </summary>
        /// <param name="filePath">Playlist file path</param>
        /// <returns>Playlist</returns>
        public void LoadPlaylist(string filePath)
        {
            // Declare variables
            List<string> files = new List<string>();            

            // Determine the playlist format
            if (filePath.ToUpper().Contains(".M3U"))
            {
                // Set format
                Format = PlaylistFileFormat.M3U;

                // Load playlist file
                files = PlaylistTools.LoadM3UPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".M3U8"))
            {
                // Set format
                Format = PlaylistFileFormat.M3U8;

                // Load playlist file
                files = PlaylistTools.LoadM3UPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".PLS"))
            {
                // Set format
                Format = PlaylistFileFormat.PLS;

                // Load playlist file
                files = PlaylistTools.LoadPLSPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".XSPF"))
            {
                // Set format
                Format = PlaylistFileFormat.XSPF;

                // Load playlist file
                files = PlaylistTools.LoadXSPFPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".ASX"))
            {
                // Set format
                Format = PlaylistFileFormat.ASX;
            }

            // Check if the playlist is empty
            if (files == null || files.Count == 0)
            {
                // Display error
                throw new Exception("Error: The playlist is empty or does not contain any valid audio file paths!");                
            }

            // Clear current playlist
            Clear();

            // Set properties
            FilePath = filePath;

            // Add files to playlist
            AddItems(files);

            // Set first item
            First();
        }

        /// <summary>
        /// Clears the playlist.
        /// </summary>
        public void Clear()
        {            
            FilePath = string.Empty;
            Format = PlaylistFileFormat.Unknown;
            items = new List<PlaylistItem>();
            currentItemIndex = 0;
            currentItem = null;            
        }

        /// <summary>
        /// Disposes channels and set them to null.
        /// </summary>
        public void DisposeChannels()
        {
            // Free current channel
            if (currentItem.Channel != null)
            {
                // Stop and free channel                
                currentItem.Dispose();
                currentItem = null;
            }

            // Go through items to set them load = false
            for (int a = 0; a < items.Count; a++)
            {
                // Dispose channel, if not null (validation inside method)
                items[a].Dispose();
            }
        }

        /// <summary>
        /// Updates the current item if the private value is null and
        /// there is more than one item in the list.
        /// </summary>
        private void UpdateCurrentItem()
        {
            // Check if there is at least one item but no current item set
            if (currentItem == null && items.Count > 0)
            {
                // Set current item to the first in the list
                currentItem = items[0];
            }
        }

        /// <summary>
        /// Adds an item at the end of the playlist.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        public void AddItem(string filePath)
        {
            // Create audio file and read metadata
            AudioFile audioFile = new AudioFile(filePath);

            // Add new playlist item at the end
            Items.Add(new PlaylistItem(this, audioFile));

            // Update current item
            UpdateCurrentItem();
        }

        /// <summary>
        /// Adds an item at the end of the playlist.
        /// </summary>
        /// <param name="audioFile">Audio file metadata</param>
        public void AddItem(AudioFile audioFile)
        {
            // Add new playlist item at the end
            Tracing.Log("Playlist.cs -- Adding " + audioFile.FilePath); 
            Items.Add(new PlaylistItem(this, audioFile));

            // Update current item
            Tracing.Log("Playlist.cs -- Updating " + audioFile.FilePath);
            UpdateCurrentItem();
        }

        /// <summary>
        /// Adds a list of items at the end of the playlist.
        /// </summary>
        /// <param name="filePaths">List of audio file paths</param>
        public void AddItems(List<string> filePaths)
        {
            // Declare variables
            AudioFile audioFile = null;
            int numberOfFilesToReadMetadata = filePaths.Count;

            //// Limit the number of files to read metadata to 2
            //if (filePaths.Count > 2)
            //{
            //    numberOfFilesToReadMetadata = 2;
            //}
            //else
            //{
            //    numberOfFilesToReadMetadata = filePaths.Count;
            //}

            // Loop through items
            for (int a = 0; a < filePaths.Count; a++)
            {
                //// Check if metadata needs to be read
                //if (a < numberOfFilesToReadMetadata)
                //{
                //    // Create audio file and read metadata
                //    audioFile = new AudioFile(filePaths[a]);
                //}
                //else
                //{
                //    audioFile = null;
                //}

                bool addItem = false;

                try
                {
                    // Create audio file and read metadata
                    audioFile = new AudioFile(filePaths[a]);

                    // Set flag
                    addItem = true;
                }
                catch
                {
                    // Skip this item
                    addItem = false;
                }                

                // Make sure the item needs to be added
                if (addItem)
                {
                    // Add new playlist item at the end
                    Items.Add(new PlaylistItem(this, audioFile));
                }
            }

            // Update current item
            UpdateCurrentItem();
        }

        /// <summary>
        /// Adds a list of items at the end of the playlist.
        /// </summary>
        /// <param name="audioFiles">List of AudioFile instances</param>
        public void AddItems(List<AudioFile> audioFiles)
        {
            // Loop through file paths
            foreach (AudioFile audioFile in audioFiles)
            {
                // Add item
                AddItem(audioFile);
            }
        }

        /// <summary>
        /// Inserts an item at a specific location in the playlist.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        /// <param name="index">The item will be inserted before this index</param>
        public void InsertItem(string filePath, int index)
        {
            // Create audio file and read metadata
            AudioFile audioFile = new AudioFile(filePath);

            // Add new playlist item at the specified index
            Items.Insert(index, new PlaylistItem(this, audioFile));

            // Increment current item index if an item was inserted before the current item
            if (index <= CurrentItemIndex)
            {
                // Increment index
                currentItemIndex++;
            }

            // Update current item
            UpdateCurrentItem();
        }

        /// <summary>
        /// Inserts an item at a specific location in the playlist.
        /// </summary>
        /// <param name="audioFile">Audio file</param>
        /// <param name="index">The item will be inserted before this index</param>
        public void InsertItem(AudioFile audioFile, int index)
        {
            // Add new playlist item at the specified index
            Items.Insert(index, new PlaylistItem(this, audioFile));

            // Increment current item index if an item was inserted before the current item
            if (index <= CurrentItemIndex)
            {
                // Increment index
                currentItemIndex++;
            }

            // Update current item
            UpdateCurrentItem();
        }

        /// <summary>
        /// Removes the item at the location passed in the index parameter.
        /// </summary>
        /// <param name="index">Index of the item to remove</param>
        public void RemoveItem(int index)
        {            
            // Make sure the item is not playing
            if (CurrentItemIndex == index)
            {
                throw new Exception("You cannot remove a playlist item which is currently playing.");
            }

            // Dispose item
            Items[index].Dispose();

            // Remove playlist item
            Items.RemoveAt(index);

            // Decrement current item index if an item was removed before the current item
            if (index <= CurrentItemIndex)
            {
                // Decrement current item index
                currentItemIndex--;
            }
        }

        /// <summary>
        /// Sets the playlist to the first item.
        /// </summary>
        public void First()
        {
            // Set first index
            currentItemIndex = 0;
            currentItem = items[currentItemIndex];
        }

        /// <summary>
        /// Go to a specific item using the playlist item index.
        /// </summary>
        /// <param name="index">Playlist item index</param>
        public void GoTo(int index)
        {
            // Set index
            currentItemIndex = index;
            currentItem = items[currentItemIndex];
        }

        /// <summary>
        /// Go to a specific item using the playlist item identifier.
        /// </summary>
        /// <param name="id">Playlist item identifier</param>
        public void GoTo(Guid id)
        {
            // Search for the playlist item by its id
            int index = -1;
            for (int a = 0; a < Items.Count; a++)
            {
                // Does the id match?
                if (Items[a].Id == id || Items[a].AudioFile.Id == id)
                {
                    // The item has been found, exit loop
                    index = a;
                    break;
                }                
            }

            // Check if we have a valid item
            if (index >= 0)
            {
                // Skip to this item
                GoTo(index);
            }
        }

        /// <summary>
        /// Go to the first instance of the audio file path in the list.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        public void GoTo(string filePath)
        {
            // Search for the playlist item by its id
            int index = -1;
            for (int a = 0; a < Items.Count; a++)
            {
                // Does the id match?
                if (Items[a].AudioFile.FilePath == filePath)
                {
                    // The item has been found, exit loop
                    index = a;
                    break;
                }
            }

            // Check if we have a valid item
            if (index >= 0)
            {
                // Skip to this item
                GoTo(index);
            }
        }

        /// <summary>
        /// Go to the previous item.
        /// </summary>
        public void Previous()
        {
            // Check if the previous channel needs to be loaded
            if (currentItemIndex > 0)
            {
                // Increment item
                currentItemIndex--;
            }
            currentItem = items[currentItemIndex];
        }

        /// <summary>
        /// Go to the next item.
        /// </summary>
        public void Next()
        {
            // Check if the next channel needs to be loaded
            if (currentItemIndex < items.Count - 1)
            {
                // Increment item
                currentItemIndex++;                
            }
            currentItem = items[currentItemIndex];
        }
    }
}
