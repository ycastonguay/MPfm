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
using System.IO;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Services.Interfaces;

namespace Sessions.Sound.Services
{
    public class AlbumArtExtractionService : IAlbumArtExtractionService
    {
        public byte[] GetAlbumArtFromAudioFileOrFolder(string audioFilePath)
        {
            byte[] bytes = new byte[0];

            #if WINDOWSSTORE
            #else
            // Check if the file exists
            if (!File.Exists(audioFilePath))
                return null;
            #endif

            // Check the file extension
            string extension = Path.GetExtension(audioFilePath).ToUpper();
            if (extension == ".MP3")
            {
                try
                {
                    using (TagLib.Mpeg.AudioFile file = new TagLib.Mpeg.AudioFile(audioFilePath))
                    {
                        if (file != null && file.Tag != null && file.Tag.Pictures != null && file.Tag.Pictures.Length > 0)
                        {
                            bytes = file.Tag.Pictures[0].Data.Data;
                        }
                    }
                }
                catch
                {
                    // Failed to recover album art. Do nothing.
                }
            }

            // If we can't get the album art from the tags, try to use FOLDER.JPG instead
            if(bytes.Length == 0 || bytes == null)
            {
                try
                {
                    string albumArtFilePath = Path.Combine(Path.GetDirectoryName(audioFilePath), "folder.jpg");
                    Console.WriteLine("AudioFile - ExtractImageByteArrayForAudioFile - Trying to extract album art from FOLDER.JPG - Path: {0}", albumArtFilePath);
                    if(File.Exists(albumArtFilePath))
                    {
                        Console.WriteLine("AudioFile - ExtractImageByteArrayForAudioFile - File exists");
                        bytes = File.ReadAllBytes(albumArtFilePath);
                        Console.WriteLine("AudioFile - ExtractImageByteArrayForAudioFile - Read bytes length: {0}", bytes.Length);
                    }
                }
                catch
                {
                    // Failed to recover album art. Do nothing.
                }
            }
            
            return bytes;
        }

        public void SetAlbumArtForAudioFile(string audioFilePath, byte[] imageData)
        {
            if (!File.Exists(audioFilePath))
                return;

            string extension = Path.GetExtension(audioFilePath).ToUpper();
            if (extension == ".MP3")
            {
                using (var file = new TagLib.Mpeg.AudioFile(audioFilePath))
                {
                    if (file != null && file.Tag != null)
                    {
                        file.Tag.Pictures = new TagLib.IPicture[1];
                        file.Tag.Pictures[0].Data = new TagLib.ByteVector(imageData, imageData.Length);
                    }
                }
            }
            else if (extension == ".FLAC")
            {
                // Check if it uses APE
                //var apeTag = APEMetadata.Read(filePath);
                //APEMetadata.Write(filePath, apeTag.Dictionary);

                // Or place it as folder.jpg in the same folder
                string albumArtFilePath = Path.Combine(Path.GetDirectoryName(audioFilePath), "folder.jpg");
                Console.WriteLine("AudioFile - SetAlbumArtForAudioFile - Writing folder.jpg... - Path: {0}", albumArtFilePath);
                File.WriteAllBytes(albumArtFilePath, imageData);
            }
        }
    }
}