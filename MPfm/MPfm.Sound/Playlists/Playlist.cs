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
using MPfm.Core;
using MPfm.Core.Attributes;
using MPfm.Sound.AudioFiles;

namespace MPfm.Sound.Playlists
{
    /// <summary>
    /// Defines a playlist to be used with the Player.
    /// </summary>
    public class Playlist
    {
        private List<PlaylistItem> _items = null;
        /// <summary>
        /// List of playlist _items.
        /// </summary>
        [DatabaseField(false)]
        public List<PlaylistItem> Items
        {
            get
            {
                return _items;
            }
        }

        private int _currentItemIndex = 0;
        /// <summary>
        /// Returns the current playlist item index.
        /// </summary>
        [DatabaseField(false)]
        public int CurrentItemIndex
        {
            get
            {
                return _currentItemIndex;
            }
        }

        private PlaylistItem _currentItem = null;
        /// <summary>
        /// Returns the current item.
        /// </summary>
        [DatabaseField(false)]
        public PlaylistItem CurrentItem
        {
            get
            {
                return _currentItem;
            }
        }

        /// <summary>
        /// Playlist name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Playlist identifier.
        /// </summary>
        public Guid PlaylistId { get; set; }

        /// <summary>
        /// Playlist last modified.
        /// </summary>
        [DatabaseField(false)] // TODO: Save in database... causes an error in Android
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Playlist file path.
        /// </summary>
        [DatabaseField(false)]
        public string FilePath { get; set; }

        /// <summary>
        /// Playlist format.
        /// </summary>
        [DatabaseField(false)]
        public PlaylistFileFormat Format { get; set; }

        /// <summary>
        /// Default constructor for the Playlist class.
        /// </summary>
        public Playlist()
        {
            _items = new List<PlaylistItem>();
            PlaylistId = Guid.NewGuid();
            LastModified = DateTime.Now;
            Format = PlaylistFileFormat.Unknown;
        }

        /// <summary>
        /// Loads a playlist (from any of the following formats: M3U, M3U8, PLS and XSPF).
        /// Note: long playlists may take a while to load using this method!
        /// </summary>
        /// <param name="filePath">Playlist file path</param>
        /// <returns>Playlist</returns>
        public void LoadPlaylist(string filePath)
        {
            List<string> files = new List<string>();            

            if (filePath.ToUpper().Contains(".M3U"))
            {
                Format = PlaylistFileFormat.M3U;
                files = PlaylistTools.LoadM3UPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".M3U8"))
            {
                Format = PlaylistFileFormat.M3U8;
                files = PlaylistTools.LoadM3UPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".PLS"))
            {
                Format = PlaylistFileFormat.PLS;
                files = PlaylistTools.LoadPLSPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".XSPF"))
            {
                Format = PlaylistFileFormat.XSPF;
                files = PlaylistTools.LoadXSPFPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".ASX"))
            {
                Format = PlaylistFileFormat.ASX;
            }

            if (files == null || files.Count == 0)
                throw new Exception("Error: The playlist is empty or does not contain any valid audio file paths!");                

            Clear();
            FilePath = filePath;
            AddItems(files);
            First();
        }

        /// <summary>
        /// Clears the playlist.
        /// </summary>
        public void Clear()
        {            
            FilePath = string.Empty;
            Format = PlaylistFileFormat.Unknown;
            _items = new List<PlaylistItem>();
            _currentItemIndex = 0;
            _currentItem = null;            
        }

        /// <summary>
        /// Disposes channels and set them to null.
        /// </summary>
        public void DisposeChannels()
        {
            #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
            // Free current channel
            if (_currentItem.Channel != null)
            {
                _currentItem.Dispose();
                _currentItem = null;
            }
            #endif

            // Go through _items to set them load = false
            for (int a = 0; a < _items.Count; a++)
            {
                // Dispose channel, if not null (validation inside method)
                _items[a].Dispose();
            }
        }

        /// <summary>
        /// Updates the current item if the private value is null and
        /// there is more than one item in the list.
        /// </summary>
        private void UpdateCurrentItem()
        {
            if (_currentItem == null && _items.Count > 0)
                _currentItem = _items[0];
        }

        /// <summary>
        /// Adds an item at the end of the playlist.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        public void AddItem(string filePath)
        {
            AudioFile audioFile = new AudioFile(filePath);
            Items.Add(new PlaylistItem(this, audioFile));
            UpdateCurrentItem();
        }

        /// <summary>
        /// Adds an item at the end of the playlist.
        /// </summary>
        /// <param name="audioFile">Audio file metadata</param>
        public void AddItem(AudioFile audioFile)
        {
            Items.Add(new PlaylistItem(this, audioFile));
            UpdateCurrentItem();
        }

        /// <summary>
        /// Adds a list of _items at the end of the playlist.
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

            // Loop through _items
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
                    addItem = true;
                }
                catch
                {
                    // Skip this item
                    addItem = false;
                }                

                if (addItem)
                    Items.Add(new PlaylistItem(this, audioFile));
            }

            UpdateCurrentItem();
        }

        /// <summary>
        /// Adds a list of _items at the end of the playlist.
        /// </summary>
        /// <param name="audioFiles">List of AudioFile instances</param>
        public void AddItems(List<AudioFile> audioFiles)
        {
            foreach (AudioFile audioFile in audioFiles)
                AddItem(audioFile);
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
                _currentItemIndex++;

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
                _currentItemIndex++;

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
                throw new Exception("You cannot remove a playlist item which is currently playing.");

            Items[index].Dispose();
            Items.RemoveAt(index);

            // Decrement current item index if an item was removed before the current item
            if (index <= CurrentItemIndex)
                _currentItemIndex--;
        }

        /// <summary>
        /// Remove all playlist _items matching the ids in the list.
        /// </summary>
        /// <param name="playlistIds">List of playlist ids to remove</param>
        public void RemoveItems(List<Guid> playlistIds)
        {
            foreach (var playlistId in playlistIds)
            {
                var stuff = Items.Select(x => x.Id == playlistId).ToList();
                Items.RemoveAll(x => x.Id == playlistId);
            }
        }

        /// <summary>
        /// Sets the playlist to the first item.
        /// </summary>
        public void First()
        {
            _currentItemIndex = 0;
            _currentItem = _items[_currentItemIndex];
        }

        /// <summary>
        /// Go to a specific item using the playlist item index.
        /// </summary>
        /// <param name="index">Playlist item index</param>
        public void GoTo(int index)
        {
            _currentItemIndex = index;
            _currentItem = _items[_currentItemIndex];
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
                if (Items[a].Id == id || Items[a].AudioFile.Id == id)
                {
                    index = a;
                    break;
                }                
            }

            // Check if we have a valid item
            if (index >= 0)
                GoTo(index);
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
                if (Items[a].AudioFile.FilePath == filePath)
                {
                    index = a;
                    break;
                }
            }

            // Check if we have a valid item
            if (index >= 0)
                GoTo(index);
        }

        /// <summary>
        /// Go to the previous item.
        /// </summary>
        public void Previous()
        {
            if (_currentItemIndex > 0)
                _currentItemIndex--;
            _currentItem = _items[_currentItemIndex];
        }

        /// <summary>
        /// Go to the next item.
        /// </summary>
        public void Next()
        {
            if (_currentItemIndex < _items.Count - 1)
                _currentItemIndex++;                
            _currentItem = _items[_currentItemIndex];
        }
    }
}
