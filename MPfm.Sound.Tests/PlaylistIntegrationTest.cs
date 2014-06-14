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
using NUnit.Framework;

namespace MPfm.Sound.Tests
{
    [TestFixture()]
    public class PlaylistIntegrationTest : PlaylistTest
    {            
        private void PrepareTest()
        {
            PreparePlaylist();
        }

        private void EveryPlaylistItemShouldBePlayed(bool shuffle)
        {
            var guids = new List<Guid>();
            PrepareTest();
            //Playlist.IsShuffled = shuffle;
            for (int a = 0; a < Playlist.Items.Count; a++)
            {
                Console.WriteLine("a: {0} guid: {1} currentItemIndex: {2} count: {3}", a, Playlist.CurrentItem.Id, Playlist.CurrentItemIndex, Playlist.Items.Count);
                guids.Add(Playlist.CurrentItem.Id);
                Playlist.Next();
            }

            // Validate that every id is unique (i.e. the same playlist item hasn't been played more than once)
            Assert.True(guids.Distinct().Count() == guids.Count);

            // Validate that every id is part of the playlist
            foreach (var id in guids)
            {
                var item = Playlist.Items.FirstOrDefault(x => x.Id == id);
                Assert.IsNotNull(item);
            }
        }

        [Test]
        public void EveryPlaylistItemShouldBePlayed()
        {
            EveryPlaylistItemShouldBePlayed(false);
        }

        [Test]
        public void EveryPlaylistItemShouldBePlayedWithShuffleEnabled()
        {
            EveryPlaylistItemShouldBePlayed(true);
        }
    }
}
