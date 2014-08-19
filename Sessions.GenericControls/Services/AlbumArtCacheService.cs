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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services.Objects;

namespace Sessions.GenericControls.Services
{
    public class AlbumArtCacheService : IAlbumArtCacheService
    {
        private readonly object _lockerCache = new object();
        private Dictionary<string, IBasicImage> _cache;

        public int Count
        {
            get { return _cache.Count; }
        }

        public AlbumArtCacheService()
        {
            _cache = new Dictionary<string, IBasicImage>();
        }

        private string GetCacheKey(string artistName, string albumTitle)
        {
            // TODO: Try better matching
            return artistName.ToUpper() + "_" + albumTitle.ToUpper();
        }

        public void Flush()
        {
            lock (_lockerCache)
            {
                foreach (var item in _cache)
                    if(item.Value != null && item.Value.Image != null)
                        item.Value.Image.Dispose();

                _cache.Clear();
            }
        }

        public void AddAlbumArt(IBasicImage image, string artistName, string albumTitle)
        {
            lock (_lockerCache)
            {
                _cache.Add(GetCacheKey(artistName, albumTitle), image);
            }
        }

        public IBasicImage GetAlbumArt(string artistName, string albumTitle)
        {
            IBasicImage image;
            lock (_lockerCache)
            {
                image = _cache[GetCacheKey(artistName, albumTitle)];
            }
            return image;
        }
    }
}
