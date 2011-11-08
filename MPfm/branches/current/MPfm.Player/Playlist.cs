//
// Playlist.cs: This file contains the class defining a playlist to be used with PlayerV4.
//
// Copyright © 2011 Yanick Castonguay
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
using MPfm.Library;

namespace MPfm.Player.PlayerV4
{
    /// <summary>
    /// Defines a playlist to be used with PlayerV4
    /// </summary>
    public class Playlist
    {
        /// <summary>
        /// Private value for the Items property.
        /// </summary>
        private List<PlaylistItem> m_items = null;
        /// <summary>
        /// List of playlist items.
        /// </summary>
        public List<PlaylistItem> Items
        {
            get
            {
                return m_items;
            }
        }

        /// <summary>
        /// Private value for the CurrentItemIndex property.
        /// </summary>
        private int m_currentItemIndex = 0;
        /// <summary>
        /// Returns the current playlist item index.
        /// </summary>
        public int CurrentItemIndex
        {
            get
            {
                return m_currentItemIndex;
            }
        }

        private PlaylistItem m_currentItem = null;
        /// <summary>
        /// Returns the current item.
        /// </summary>
        public PlaylistItem CurrentItem
        {
            get
            {
                return m_currentItem;
            }
        }

        /// <summary>
        /// Default constructor for the Playlist class.
        /// </summary>
        public Playlist()
        {
            m_items = new List<PlaylistItem>();
            m_currentItemIndex = 0;
        }

        /// <summary>
        /// Clears the playlist.
        /// </summary>
        public void Clear()
        {
            m_items = new List<PlaylistItem>();
            m_currentItemIndex = 0;
            m_currentItem = null;
        }

        /// <summary>
        /// Disposes channels and set them to null.
        /// </summary>
        public void DisposeChannels()
        {
            // Free current channel
            if (m_currentItem.Channel != null)
            {
                // Stop and free channel
                m_currentItem.Dispose();
                m_currentItem = null;
            }

            // Go through items to set them load = false
            for (int a = 0; a < m_items.Count; a++)
            {
                // Dispose channel, if not null (validation inside method)
                m_items[a].Dispose();
            }
        }

        /// <summary>
        /// Adds an item at the end of the playlist.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        public void AddItem(string filePath)
        {
            // Add new playlist item at the end
            Items.Add(new PlaylistItem(this, filePath));
        }

        /// <summary>
        /// Adds an item at the end of the playlist.
        /// </summary>
        /// <param name="song">SongDTO instance</param>
        public void AddItem(SongDTO song)
        {
            // Add new playlist item at the end
            Items.Add(new PlaylistItem(this, song.FilePath) { Song = song });
        }

        /// <summary>
        /// Adds a list of items at the end of the playlist.
        /// </summary>
        /// <param name="filePaths">List of audio file paths</param>
        public void AddItems(List<string> filePaths)
        {
            // Loop through file paths
            foreach (string filePath in filePaths)
            {
                // Add item
                AddItem(filePath);
            }
        }

        /// <summary>
        /// Adds a list of items at the end of the playlist.
        /// </summary>
        /// <param name="songs">List of SongDTO instances</param>
        public void AddItems(List<SongDTO> songs)
        {
            // Loop through file paths
            foreach (SongDTO song in songs)
            {
                // Add item
                AddItem(song);
            }
        }

        /// <summary>
        /// Inserts an item at a specific location in the playlist.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        /// <param name="index">The item will be inserted before this index</param>
        public void InsertItem(string filePath, int index)
        {
            // Add new playlist item at the specified index
            Items.Insert(index, new PlaylistItem(this, filePath));

            // Increment current item index if an item was inserted before the current item
            if (index <= CurrentItemIndex)
            {
                // Increment index
                m_currentItemIndex++;
            }
        }

        /// <summary>
        /// Inserts an item at a specific location in the playlist.
        /// </summary>
        /// <param name="song">SongDTO instance</param>
        /// <param name="index">The item will be inserted before this index</param>
        public void InsertItem(SongDTO song, int index)
        {
            // Add new playlist item at the specified index
            Items.Insert(index, new PlaylistItem(this, song.FilePath) { Song = song });

            // Increment current item index if an item was inserted before the current item
            if (index <= CurrentItemIndex)
            {
                // Increment index
                m_currentItemIndex++;
            }
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
                m_currentItemIndex--;
            }
        }

        /// <summary>
        /// Sets the playlist to the first item.
        /// </summary>
        public void First()
        {
            // Set first index
            m_currentItemIndex = 0;
            m_currentItem = m_items[m_currentItemIndex];
        }

        /// <summary>
        /// Go to a specific item index in the playlist.
        /// </summary>
        /// <param name="index">Item index</param>
        public void GoTo(int index)
        {
            // Set index
            m_currentItemIndex = index;
            m_currentItem = m_items[m_currentItemIndex];
        }

        /// <summary>
        /// Go to a specific item index in the playlist.
        /// </summary>
        /// <param name="index">Item index</param>
        public void GoTo(Guid songId)
        {
            int index = -1;
            for (int a = 0; a < Items.Count; a++)
            {
                if (Items[a].Song != null)
                {
                    if (Items[a].Song.SongId == songId)
                    {
                        index = a;
                    }
                }
            }

            if (index >= 0)
            {
                GoTo(index);
            }
        }

        /// <summary>
        /// Go to the previous item.
        /// </summary>
        public void Previous()
        {
            // Check if the previous channel needs to be loaded
            if (m_currentItemIndex > 0)
            {
                // Increment item
                m_currentItemIndex--;
            }
            m_currentItem = m_items[m_currentItemIndex];
        }

        /// <summary>
        /// Go to the next item.
        /// </summary>
        public void Next()
        {
            // Check if the next channel needs to be loaded
            if (m_currentItemIndex < m_items.Count - 1)
            {
                // Increment item
                m_currentItemIndex++;                
            }
            m_currentItem = m_items[m_currentItemIndex];
        }
    }
}
