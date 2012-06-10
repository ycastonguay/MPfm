//
// AlbumCoverHelper.cs: Helper static class for fetching album covers.
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Mono.Unix;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MPfm.Sound;

namespace MPfm.Mac
{
    /// <summary>
    /// Helper static class for fetching album covers.
    /// </summary>
    public static class AlbumCoverHelper
    {
        public static NSImage GetAlbumCover(string audioFilePath)
        {
            // Try to extract image from tags
            System.Drawing.Image image = AudioFile.ExtractImageForAudioFile(audioFilePath);
            if(image == null)
            {
                // Try to find the cover in the current folder
                string folderPath = Path.GetDirectoryName(audioFilePath);
                UnixDirectoryInfo rootDirectoryInfo = new UnixDirectoryInfo(folderPath);

                UnixFileSystemInfo[] infos = rootDirectoryInfo.GetFileSystemEntries();
                foreach(UnixFileSystemInfo fileInfo in rootDirectoryInfo.GetFileSystemEntries())
                {
                    // Check if the file matches
                    string fileName = fileInfo.Name.ToUpper();
                    if((fileName.EndsWith(".JPG") ||
                        fileName.EndsWith(".JPEG") ||
                        fileName.EndsWith(".PNG") ||
                        fileName.EndsWith(".GIF")) &&
                       (fileName.StartsWith("FOLDER") ||
                        fileName.StartsWith("COVER")))
                    {
                        return new NSImage(fileInfo.FullName);
                    }
                }
            }

            // Convert image to NSImage if not null
            if(image != null)
                return ConvertToNSImage(image);

            return null;
        }

        /// <summary>
        /// Converts a System.Drawing.Image to NSImage.
        /// Taken from http://lists.ximian.com/pipermail/mono-osx/2011-July/004436.html.
        /// </summary>
        /// <returns>NSImage</returns>
        /// <param name='img'>System.Drawing.Image to convert</param>
        public static NSImage ConvertToNSImage(Image img)
        {
            MemoryStream s = new MemoryStream();
            img.Save(s, ImageFormat.Png);
            byte[] b = s.ToArray();
            CGDataProvider dp = new CGDataProvider(b,0,(int)s.Length);
            s.Flush();
            s.Close();
            CGImage img2 = CGImage.FromPNG(dp,null,false,CGColorRenderingIntent.Default);
            return new NSImage(img2, new SizeF(img2.Width,img2.Height));
        }
    }
}

