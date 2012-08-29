//
// CacheStore.cs: Cache store for storing any type of object.
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
using System.Linq;
using System.Reflection;

namespace MPfm.Core
{
    /// <summary>
    /// Cache store for storing any type of object.
    /// </summary>
    public class CacheStore<TObj, TId> //where TObj : struct where TId : struct
    {
        List<CacheStoreItem<TObj, TId>> listItems;

        /// <summary>
        /// Limit of items in the cache store.
        /// 0 = infinite (default).
        /// </summary>
        public int Limit { get; set; }

        public CacheStore()
        {
            listItems = new List<CacheStoreItem<TObj, TId>>();
        }

        void CheckForLimit()
        {
            if (listItems.Count <= Limit)
                return;

            // Order items by timestamp
            List<CacheStoreItem<TObj, TId>> itemsOrdered = listItems.OrderBy(x => x.Timestamp).ToList();

            // Remove items until it meets the limit
            while(true)
            {
                // Remove first item (i.e. oldest)
                listItems.Remove(itemsOrdered[0]);

                // Check for limit
                if(listItems.Count <= Limit)
                    break;
            }
        }

        public void Add(TObj obj, TId id)
        {
            listItems.Add(new CacheStoreItem<TObj, TId>(obj, id));
            CheckForLimit();
        }

        public void Clear()
        {
            listItems.Clear();
        }

        public TObj GetObjectById(TId id)
        {
            CacheStoreItem<TObj, TId> item = listItems.FirstOrDefault(x => x.Id.Equals(id));
            if(item == null)
                return default(TObj);
            else
                return item.Object;
        }
    }

    public class CacheStoreItem<TObj, TId>
    {
        public TId Id { get; set; }
        public TObj Object { get; set; }
        public DateTime Timestamp { get; set; }

        public CacheStoreItem(TObj obj, TId id)
        {
            Object = obj;
            Id = id;
            Timestamp = DateTime.Now;
        }
    }
}