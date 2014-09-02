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
using System.Collections.Specialized;
using System.Linq;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services.Objects;
using System.Collections;

namespace Sessions.GenericControls.Services
{
    public class AlbumArtCacheService : IAlbumArtCacheService
    {
        // TODO: Implement max cache size
        private readonly object _lockerCache = new object();
        private List<AlbumArtCacheItem> _cache;

        public int Count
        {
            get { return _cache.Count; }
        }

        public int MaximumCacheCount { get; set; }

        public AlbumArtCacheService()
        {
            _cache = new List<AlbumArtCacheItem>();
            MaximumCacheCount = 10;
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
                foreach (var cacheItem in _cache)
                {
                    var value = cacheItem.Image;
                    if (value != null && value.Image != null)
                        value.Image.Dispose();
                }

                _cache.Clear();
            }
        }

        public void FlushItemsExceedingMaximumCacheCount()
        {
            lock (_lockerCache)
            {
                var orderedItems = _cache.OrderBy(x => x.TimeStamp);
                var itemsToRemove = orderedItems.Take(Count - MaximumCacheCount);
                foreach (var itemToRemove in itemsToRemove)
                {
                    _cache.Remove(itemToRemove);
                }
            }
        }

        public void AddAlbumArt(IBasicImage image, string artistName, string albumTitle)
        {
            string cacheKey = GetCacheKey(artistName, albumTitle);
            lock (_lockerCache)
            {
                var cacheItem = _cache.FirstOrDefault(x => string.Compare(x.Key, cacheKey, StringComparison.OrdinalIgnoreCase) == 0);
                if (cacheItem == null)
                {
                    _cache.Add(new AlbumArtCacheItem(cacheKey, image));
                }
            }
        }

        public IBasicImage GetAlbumArt(string artistName, string albumTitle)
        {
            IBasicImage image = null;
            string cacheKey = GetCacheKey(artistName, albumTitle);
            lock (_lockerCache)
            {
                var cacheItem = _cache.FirstOrDefault(x => string.Compare(x.Key, cacheKey, StringComparison.OrdinalIgnoreCase) == 0);
                if (cacheItem != null)
                {
                    return cacheItem.Image;
                }
            }
            return image;
        }

        public class AlbumArtCacheItem
        {
            public string Key { get; set; }
            public IBasicImage Image { get; set; }
            public DateTime TimeStamp { get; set; }

            public AlbumArtCacheItem(string key, IBasicImage image)
            {
                Key = key;
                Image = image;
                TimeStamp = DateTime.Now;
            }
        }
    }
}
