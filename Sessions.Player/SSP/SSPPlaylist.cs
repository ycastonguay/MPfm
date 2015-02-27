// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using org.sessionsapp.player;
using Sessions.Sound.AudioFiles;
using System.Collections.Generic;

namespace Sessions.Player
{
    public class SSPPlaylist
    {
        private AudioFileDictionary _audioFiles;

        public int Count
        {
            get
            {
                return SSP.SSP_Playlist_GetCount();
            }
        }

        public int CurrentIndex
        {
            get
            {
                return SSP.SSP_Playlist_GetCurrentIndex();
            }
        }

        public object this[int i]
        {
            get
            {
                return GetItemAt(i);
            }
        }

        public SSPPlaylist()
        {
            _audioFiles = new AudioFileDictionary();
        }

        // Should this have a id associated with the file path, since a song can be multiple times in a playlist?
        // How do we match an TagLib AudioFile with a libssp AudioFile?
        // i.e. If I get the first item of a playlist, how do I compose a PlaylistItem?
        public void AddItem(string filePath)
        {
            SSP.SSP_Playlist_AddItem(filePath);
            var audioFile = _audioFiles.RequestItem(filePath); // preload metadata?
        }

        public void AddItems(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                AddItem(filePath);
            }
        }
            
        public void InsertItemAt(string filePath, int index)
        {
            SSP.SSP_Playlist_InsertItemAt(filePath, index);
            var audioFile = _audioFiles.RequestItem(filePath); // preload metadata?
        }

        public AudioFile GetItemAt(int index)
        {   
            var item = new SSPPlaylistItem();
            SSP.SSP_Playlist_GetItemAt(index, ref item.Struct);
            if (string.IsNullOrEmpty(item.FilePath))
                return null;

            var audioFile = _audioFiles.RequestItem(item);
            return audioFile;
        }

        public AudioFile GetCurrentItem()
        {   
            return GetItemAt(CurrentIndex);
        }

        public void RemoveItemAt(int index)
        {
            var item = new SSPPlaylistItem();
            SSP.SSP_Playlist_GetItemAt(index, ref item.Struct);
            if (string.IsNullOrEmpty(item.FilePath))
                return;

            _audioFiles.RemoveItem(item.FilePath); // is this really what we want?
            SSP.SSP_Playlist_RemoveItemAt(index);
        }

        public void Clear()
        {
            SSP.SSP_Playlist_Clear();
            _audioFiles.Clear(); // is this really what we want?
        }
    }
}
