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
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Enc;

namespace Sessions.Sound.BassNetWrapper
{
    /// <summary>
    /// The Base class contains methods for casting in the Bassenc library.
    /// </summary>
    public static class BaseCast
    {
        public static void CastInit(int handle, string server, string password, string name, string url, int bitrate, bool isPublic)
        {
            string content = BassEnc.BASS_ENCODE_TYPE_AAC;
            string genre = string.Empty;
            string desc = string.Empty;
            string headers = string.Empty;
            bool result = BassEnc.BASS_Encode_CastInit(handle, server, password, content, name, url, genre, desc, headers, bitrate, isPublic);
            if (!result)
                Base.CheckForError();
        }

        public static void CastSetTitle(int handle, string title, string url)
        {
            bool result = BassEnc.BASS_Encode_CastSetTitle(handle, title, url);
            if (!result)
                Base.CheckForError();
        }

        public static void CastSendMeta(int handle, BASSEncodeMetaDataType metaDataType, byte[] data)
        {
            bool result = BassEnc.BASS_Encode_CastSendMeta(handle, metaDataType, data);
            if (!result)
                Base.CheckForError();
        }

        public static string CastGetStats(int handle, BASSEncodeStats statsType, string password)
        {
            string result = BassEnc.BASS_Encode_CastGetStats(handle, statsType, password);
            return result;
        }

    }
}
