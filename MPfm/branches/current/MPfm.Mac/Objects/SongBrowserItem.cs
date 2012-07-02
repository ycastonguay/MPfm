//
// SongBrowserItem.cs: Song Browser item for the NSTableView.
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
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.MVP;
using MPfm.Sound;

namespace MPfm.Mac
{
    /// <summary>
    /// Song Browser item for the NSTableView.
    /// </summary>
    public class SongBrowserItem : NSObject
    {
        public AudioFile AudioFile { get; private set; }
        public NSObject Null { get; private set; }
        public Dictionary<string, NSString> KeyValues { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.Mac.SongBrowserItem"/> class.
        /// </summary>
        /// <param name='audioFile'>Audio file</param>
        public SongBrowserItem(AudioFile audioFile)
        {
            // Set entity and NSObject values
            this.AudioFile = audioFile;

            // Set null object
            Null = new NSObject();

            // Add key/values
            KeyValues = new Dictionary<string, NSString>();
            KeyValues.Add("TrackNumber", new NSString(AudioFile.TrackNumber.ToString()));
            KeyValues.Add("Title", new NSString(audioFile.Title));
            KeyValues.Add("ArtistName", new NSString(audioFile.ArtistName));
            KeyValues.Add("AlbumTitle", new NSString(audioFile.AlbumTitle));
            KeyValues.Add("Length", new NSString(AudioFile.Length));
        }
    }
}

