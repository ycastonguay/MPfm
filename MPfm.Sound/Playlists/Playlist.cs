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
        [DatabaseField(false)]
        public PlaylistRepeatType RepeatType { get; set; }

        /// <summary>
        /// List of playlist _items.
        /// </summary>
        [DatabaseField(false)]
        public List<PlaylistItem> Items { get; protected set; }

        /// <summary>
        /// Returns the current playlist item index.
        /// </summary>
        [DatabaseField(false)]
        public int CurrentItemIndex { get; protected set; }

        /// <summary>
        /// Returns the current item.
        /// </summary>
        [DatabaseField(false)]
        public PlaylistItem CurrentItem { get; protected set; }

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

        public Playlist()
        {
            Items = new List<PlaylistItem>();
            PlaylistId = Guid.NewGuid();
            LastModified = DateTime.Now;
            Format = PlaylistFileFormat.Unknown;
        }

        private void PrepareCurrentItemForPlayback()
        {
            if (CurrentItem == null && Items.Count > 0)
                CurrentItem = Items[0];
        }

        public virtual void Clear()
        {            
            FilePath = string.Empty;
            Format = PlaylistFileFormat.Unknown;
            Items = new List<PlaylistItem>();
            CurrentItemIndex = 0;
            CurrentItem = null;            
        }
       
        public void DisposeChannels()
        {
            CurrentItem = null;
            for (int a = 0; a < Items.Count; a++)
                Items[a].Dispose();
        }

        public void AddItem(PlaylistItem playlistItem)
        {
            Items.Add(playlistItem);
            PrepareCurrentItemForPlayback();
        }

        public void AddItems(IEnumerable<string> filePaths)
        {
            AudioFile audioFile = null;
            for (int a = 0; a < filePaths.Count(); a++)
            {
                bool addItem = false;

                try
                {
                    audioFile = new AudioFile(filePaths.ElementAt(a));
                    addItem = true;
                }
                catch
                {
                    addItem = false;
                }                

                if (addItem)
                    Items.Add(new PlaylistItem(audioFile));
            }

            PrepareCurrentItemForPlayback();
        }

		public void AddItems(IEnumerable<PlaylistItem> playlistItems)
        {
            Items.AddRange(playlistItems);
			PrepareCurrentItemForPlayback();
        }

        public void InsertItem(string filePath, int index)
        {
            InsertItem(new AudioFile(filePath), index);
        }

        public void InsertItem(AudioFile audioFile, int index)
        {
            InsertItem(new PlaylistItem(audioFile), index);
        }

        public void InsertItem(PlaylistItem item, int index)
        {
            // Add new playlist item at the specified index
            Items.Insert(index, item);

            // Increment current item index if an item was inserted before the current item; set current item if index is the same
            if (index < CurrentItemIndex)
                CurrentItemIndex++;
            else if (index == CurrentItemIndex)
                CurrentItem = item;

            PrepareCurrentItemForPlayback();
        }

        public void RemoveItem(int index)
        {            
            Items[index].Dispose();
            Items.RemoveAt(index);

            // Decrement current item index if an item was removed before the current item
            //if (index <= CurrentItemIndex)
            if (index < CurrentItemIndex)
                CurrentItemIndex--;
            
            PrepareCurrentItemForPlayback();
        }

        public void RemoveItems(IEnumerable<Guid> playlistIds)
        {
            foreach (var playlistId in playlistIds)
                Items.RemoveAll(x => x.Id == playlistId);
            
            PrepareCurrentItemForPlayback();
        }

        /// <summary>
        /// Sets the playlist to the first item.
        /// </summary>
        public void First()
        {
            CurrentItemIndex = 0;
            CurrentItem = Items[CurrentItemIndex];
        }

        /// <summary>
        /// Go to a specific item using the playlist item index.
        /// </summary>
        /// <param name="index">Playlist item index</param>
        public void GoTo(int index)
        {
            CurrentItemIndex = index;
            CurrentItem = Items[CurrentItemIndex];
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

        public virtual void Previous()
        {
            if (CurrentItemIndex > 0)
            {
                CurrentItemIndex--;
                CurrentItem = Items[CurrentItemIndex];
            }
            else
            {
                // Leave the same index   
            }
        }

        public virtual void Next()
        {
            if (CurrentItemIndex < Items.Count - 1)
            {
                CurrentItemIndex++;
                CurrentItem = Items[CurrentItemIndex];
            }
            else
            {
                // Leave the same index
            }
        }
    }
}
