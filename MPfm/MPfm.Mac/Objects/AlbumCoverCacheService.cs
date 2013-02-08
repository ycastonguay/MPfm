//
// AlbumCoverCacheService.cs: Cache service for storing album covers.
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
using Mono.Unix;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MPfm.Core;

namespace MPfm.Mac
{
    /// <summary>
    /// Cache service for storing album covers.
    /// </summary>
    public class AlbumCoverCacheService
    {
        CacheStore<NSImage, string> cacheStore;

        public AlbumCoverCacheService()
        {
            cacheStore = new CacheStore<NSImage, string>(10);
        }

        public NSImage GetAlbumCover(string artistName, string albumTitle)
        {
            return cacheStore.GetObjectById(artistName.ToUpper() + "_" + albumTitle.ToUpper());
        }

        public void AddAlbumCover(string audioFilePath, string artistName, string albumTitle)
        {
            NSImage image = AlbumCoverHelper.GetAlbumCover(audioFilePath);
            if(image != null)
                cacheStore.Add(image, artistName.ToUpper() + "_" + albumTitle.ToUpper());
        }

        public NSImage TryGetAlbumCover(string audioFilePath, string artistName, string albumTitle)
        {
            // Check if image is found in cache
            NSImage image = GetAlbumCover(artistName, albumTitle);

            // If image is not in cache, try to fetch it
            if (image == null)
            {
                // Fetch image
                image = AlbumCoverHelper.GetAlbumCover(audioFilePath);

                // Add to cache only if not null
                if(image != null)
                    cacheStore.Add(image, artistName.ToUpper() + "_" + albumTitle.ToUpper());
            }

            return image;
        }
    }
}
