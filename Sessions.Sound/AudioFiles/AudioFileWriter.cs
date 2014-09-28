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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Sessions.Sound.Tags;
using Sessions.Core;
using Sessions.Core.Attributes;

namespace Sessions.Sound.AudioFiles
{
    public static class AudioFileWriter
    {
        public static void ForMP3(AudioFile audioFile)
        {
            TagLib.Mpeg.AudioFile file = null;
            try
            {
                file = new TagLib.Mpeg.AudioFile(audioFile.FilePath);
                file = (TagLib.Mpeg.AudioFile)FillTags(file, audioFile);
                file.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading/writing the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void ForFLAC(AudioFile audioFile)
        {
            TagLib.Flac.File file = null;
            try
            {
                file = new TagLib.Flac.File(audioFile.FilePath);
                file = (TagLib.Flac.File)FillTags(file, audioFile);
                file.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading/writing the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void ForOGG(AudioFile audioFile)
        {
            TagLib.Ogg.File file = null;
            try
            {
                file = new TagLib.Ogg.File(audioFile.FilePath);
                file = (TagLib.Ogg.File)FillTags(file, audioFile);
                file.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading/writing the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void ForAPE(AudioFile audioFile)
        {
            TagLib.Ape.File file = null;
            try
            {
                file = new TagLib.Ape.File(audioFile.FilePath);
                file = (TagLib.Ape.File)FillTags(file, audioFile);
                file.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading/writing the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void ForWV(AudioFile audioFile)
        {
            TagLib.WavPack.File file = null;
            try
            {
                file = new TagLib.WavPack.File(audioFile.FilePath);
                file = (TagLib.WavPack.File)FillTags(file, audioFile);
                file.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading/writing the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        public static void ForWMA(AudioFile audioFile)
        {
            TagLib.Asf.File file = null;
            try
            {
                file = new TagLib.Asf.File(audioFile.FilePath);
                file = (TagLib.Asf.File)FillTags(file, audioFile);
                file.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while reading/writing the tags and properties of the file (" + audioFile.FilePath + ").", ex);
            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }

        /// <summary>
        /// Fills the Tag properties of a TagLib.File class (including its subclasses).
        /// </summary>
        /// <param name="file">TagLib.File class</param>
        /// <returns>TagLib.File</returns>
        public static TagLib.File FillTags(TagLib.File file, AudioFile audioFile)
        {
            file.Tag.AlbumArtists = new string[] { audioFile.ArtistName };
            file.Tag.Album = audioFile.AlbumTitle;
            file.Tag.Title = audioFile.Title;
            file.Tag.Genres = new string[] { audioFile.Genre };
            file.Tag.Disc = audioFile.DiscNumber;
            file.Tag.Track = audioFile.TrackNumber;
            file.Tag.TrackCount = audioFile.TrackCount;
            file.Tag.Lyrics = audioFile.Lyrics;
            file.Tag.Year = audioFile.Year;
            file.Tag.BeatsPerMinute = (uint)audioFile.Tempo;
            return file;
        }
    }
}
