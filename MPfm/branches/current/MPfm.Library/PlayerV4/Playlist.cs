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

namespace MPfm.Library.PlayerV4
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

        /// <summary>
        /// Returns the current item.
        /// </summary>
        public PlaylistItem CurrentItem
        {
            get
            {
                return Items[CurrentItemIndex];
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
        }

        public void AddItem(string filePath)
        {
            // Add new playlist item at the end
            Items.Add(new PlaylistItem(this, filePath));
        }

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

        public void First()
        {
            // Set first index
            m_currentItemIndex = 0;
        }

        public void GoTo(int index)
        {
            // Set index
            m_currentItemIndex = index;
        }

        public void Previous()
        {
            // Check if the previous channel needs to be loaded
            if (m_currentItemIndex > 0)
            {
                // Increment item
                m_currentItemIndex--;
            }
        }

        public void Next()
        {
            // Check if the next channel needs to be loaded
            if (m_currentItemIndex < m_items.Count - 1)
            {
                // Increment item
                m_currentItemIndex++;                
            }
        }
    }
}
