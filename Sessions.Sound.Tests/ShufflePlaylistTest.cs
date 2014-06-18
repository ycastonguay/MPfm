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
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using NUnit.Framework;
using MPfm.Sound.BassNetWrapper;
using MPfm.Core.TestConfiguration;

namespace MPfm.Sound.Tests
{
    [TestFixture()]
    public class ShufflePlaylistTest : PlaylistTest
    {        
        // Context:
        // - The playlist can contain thousands of items (i.e. do not randomize all the list at once)        
        // - An audio file can be added several times to the same playlist
        // - The playlist contents can change until the playlist is complete

        // Restrictions:
        // - The same song must not play until the playlist is complete

        public ShufflePlaylist ShufflePlaylist { get; protected set; }
        public override Playlist Playlist { get { return ShufflePlaylist; } }

        public ShufflePlaylistTest()
        {            
        }

        protected override void PreparePlaylist()
        {
            ShufflePlaylist = new ShufflePlaylist();
            ShufflePlaylist.IsShuffled = true;
            for (int a = 0; a < Config.AudioFilePaths.Count; a++)
                ShufflePlaylist.AddItem(new PlaylistItem(new AudioFile(Config.AudioFilePaths[a])));
        }
        
        [TestFixture]
        public class ClearTest : ShufflePlaylistTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PreparePlaylist();
                Playlist.Clear();
            }
            
            [Test]
            public void PlayedItemListShouldBeEmpty()
            {
                Assert.IsEmpty(ShufflePlaylist.PlayedItemIds);
            }
        }        

        [TestFixture]
        public class NextTest : ShufflePlaylistTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PreparePlaylist();
                Playlist.Next();                
            }

            [Test]
            public void ShouldAddCurrentItemToPlayedList_WhenCallingNextASecondTime()
            {
                var id = ShufflePlaylist.CurrentItem.Id;
                Playlist.Next();
                Assert.Contains(id, ShufflePlaylist.PlayedItemIds);
            }

            // When list is complete, no more items to randomize

        }

        [TestFixture]
        public class RandomizeNextItemTest : ShufflePlaylistTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PreparePlaylist();
            }

            [Test]
            public void EachRandomizedItemShouldBeUnique()
            {
                // This is an integration test
                var items = new List<PlaylistItem>();
                for (int a = 0; a < Playlist.Items.Count; a++)
                {
                    //Console.WriteLine("a: {0} guid: {1} currentItemIndex: {2} count: {3}", a, Playlist.CurrentItem.Id, Playlist.CurrentItemIndex, Playlist.Items.Count);
                    var item = ShufflePlaylist.RandomizeNextItem();
                    Assert.False(items.Contains(item));
                }
            }

            [Test]
            public void ShouldThrowException_WhenAllItemsHaveBeenRandomizedAndRepeatIsOff()
            {
                // This is an integration test
                for (int a = 0; a < Playlist.Items.Count; a++)
                {
                    //Console.WriteLine("a: {0} guid: {1} currentItemIndex: {2} count: {3}", a, Playlist.CurrentItem.Id, Playlist.CurrentItemIndex, Playlist.Items.Count);
                    var item = ShufflePlaylist.RandomizeNextItem();
                    ShufflePlaylist.PlayedItemIds.Add(item.Id);
                }

                Assert.Throws<EndOfPlaylistException>(() => ShufflePlaylist.RandomizeNextItem());
            }
        }
    }
}
