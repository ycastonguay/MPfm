// Copyright © 2011-2013 Yanick Castonguay
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
using System.Linq;
using MPfm.Core;
using MPfm.Core.Attributes;
using MPfm.Sound.AudioFiles;

namespace MPfm.Sound.Playlists
{
    /// <summary>
    /// Defines a shuffable playlist to be used with the Player.
    /// </summary>
    public class ShufflePlaylist : Playlist
    {
        private Random _random = null;

        [DatabaseField(false)]
        public bool IsShuffled { get; set; }
        
        [DatabaseField(false)]
        public List<Guid> PlayedItemIds { get; protected set; }

        public ShufflePlaylist()
        {
            _random = new Random();
            PlayedItemIds = new List<Guid>();
        }

        public override void Clear()
        {            
            _random = new Random();
            PlayedItemIds = new List<Guid>();
        }
       
        public override void Next()
        {
            if(!IsShuffled)
            {
                base.Next();
                return;
            }

            if (CurrentItem != null)
                PlayedItemIds.Add(CurrentItem.Id);

            var item = RandomizeNextItem();
            CurrentItem = item;
        }

        public PlaylistItem RandomizeNextItem()
        {
            while (true)
            {
                Console.WriteLine("RandomizeNextItem - items: {0} playedItems: {1}", Items.Count, PlayedItemIds.Count);
                var playedItems = Items.Where(x => PlayedItemIds.Contains(x.Id)).ToList();
                if (playedItems.Count == Items.Count)
                    throw new EndOfPlaylistException();

                // This will generate a random number based on the list count
                // (however, the count can change during a session...)
                int r = _random.Next(Items.Count);
                if (!PlayedItemIds.Contains(Items[r].Id))
                {
                    Console.WriteLine("RandomizeNextItem - Found item");
                    return Items[r];
                }
            }
        }
    }
}
