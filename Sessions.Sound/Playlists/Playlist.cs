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
using System.Collections.Generic;
using org.sessionsapp.player;
using Sessions.Core.Attributes;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;

namespace Sessions.Sound.Player
{
    // It is a bit misleading that this Playlist class is actually tied to a singleton playlist in libssp_player.
    // This class cannot be used right now to build a playlist that's not connected to the player.
    public class Playlist
    {
        private AudioFileDictionary _audioFiles;

        public string Name { get; set; } // not actually tied to SSP_PLAYLIST
        public Guid PlaylistId { get; set; }

        [DatabaseField(false)]
        public SSPRepeatType RepeatType { get; set; } // this has to be removed... right?

        [DatabaseField(false)]
        public DateTime LastModified { get; set; }

        [DatabaseField(false)]
        public string FilePath { get; set; }

        [DatabaseField(false)]
        public PlaylistFileFormat Format { get; set; }

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

        public Playlist()
        {
            _audioFiles = new AudioFileDictionary();
        }

        public void AddItem(AudioFile audioFile)
        {
            SSP.SSP_Playlist_AddItem(audioFile.FilePath, audioFile.Id.ToString());
            //var audioFile = _audioFiles.RequestItem(filePath); // preload metadata?
        }

        public void AddItems(IEnumerable<AudioFile> audioFiles)
        {
            foreach (var audioFile in audioFiles)
            {
                AddItem(audioFile);
            }
        }

        public void InsertItemAt(AudioFile audioFile, int index)
        {
            SSP.SSP_Playlist_InsertItemAt(audioFile.FilePath, audioFile.Id.ToString(), index);
            //var audioFile = _audioFiles.RequestItem(filePath); // preload metadata?
        }

        public virtual PlaylistItem GetItemAt(int index)
        {
            var item = new PlaylistItem();
            SSP.SSP_Playlist_GetItemAt(index, ref item.Struct);
            if (string.IsNullOrEmpty(item.FilePath))
                return null;

            var audioFile = _audioFiles.RequestItem(item);
            item.AudioFile = audioFile;
            return item;
        }

        public virtual PlaylistItem GetItemFromId(int id)
        {
            var item = new PlaylistItem();
            SSP.SSP_Playlist_GetItemFromId(id, ref item.Struct);
            if (string.IsNullOrEmpty(item.FilePath))
                return null;

            var audioFile = _audioFiles.RequestItem(item);
            item.AudioFile = audioFile;
            return item;
        }

        public virtual int GetIndexFromId(int id)
        {
            return SSP.SSP_Playlist_GetIndexFromId(id);
        }

        public virtual PlaylistItem GetCurrentItem()
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
