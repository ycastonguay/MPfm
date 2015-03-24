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
using System.Collections;
using System.Collections.Generic;
using org.sessionsapp.player;
using Sessions.Sound.AudioFiles;

namespace Sessions.Sound.AudioFiles
{
    public class AudioFileDictionary
    {
        private Dictionary<string, AudioFile> _dict;

        public AudioFileDictionary()
        {
            _dict = new Dictionary<string, AudioFile>();
        }

        public AudioFile RequestItem(SSPPlaylistItem item)
        {
            var audioFile = RequestItem(item.FilePath);
            audioFile.LengthBytes = item.Length;
            audioFile.Id = new Guid(item.AudioFileId);
            return audioFile;
        }

        private AudioFile RequestItem(string filePath)
        {
            if (_dict.ContainsKey(filePath))
            {
                return _dict[filePath];
            }

            var audioFile = new AudioFile(filePath);
            if (audioFile != null)
            {
                _dict.Add(filePath, audioFile);
            }
            return audioFile;
        }

        public void RemoveItem(string filePath)
        {
            if (_dict.ContainsKey(filePath))
            {
                _dict.Remove(filePath);
            }
        }

        public void Clear()
        {
            // TODO: Create a method to partially flush metadata... define a stategy for caching
            _dict.Clear();
        }
    }
}
