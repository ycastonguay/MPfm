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

namespace MPfm.Sound.Tests
{
    [TestFixture()]
    public class TestPlaylist
    {        
        // Context:
        // - The playlist can contain thousands of items (i.e. do not randomize all the list at once)        
        // - An audio file can be added several times to the same playlist
        // - The playlist contents can change until the playlist is complete

        // Restrictions:
        // - The same song must not play until the playlist is complete

        private const int NumberOfSongsToFillPlaylist = 10;
        private Playlist _playlist;

        public TestPlaylist()
        {
        }
        
        private void PreparePlaylistForTesting()
        {
            _playlist = new Playlist();    
            for (int a = 0; a < NumberOfSongsToFillPlaylist - 1; a++)
                _playlist.AddItem(new AudioFile());
        }

        [Test]
        public void EnsureThatEveryPlaylistItemIsPlayed()
        {
            EnsureThatEveryPlaylistItemIsPlayed(false);
        }

        [Test]
        public void EnsureThatEveryPlaylistItemIsPlayed_Shuffled()
        {
            EnsureThatEveryPlaylistItemIsPlayed(true);
        }

        private void EnsureThatEveryPlaylistItemIsPlayed(bool shuffle)
        {
            // Variant: add/remove songs from playlist inside the loop
            var guids = new List<Guid>();
            PreparePlaylistForTesting();
            _playlist.IsShuffled = shuffle;
            for (int a = 0; a < _playlist.Items.Count - 1; a++)
            {
                Console.WriteLine("a: {0} guid: {1} currentItemIndex: {2} count: {3}", a, _playlist.CurrentItem.Id, _playlist.CurrentItemIndex, _playlist.Items.Count);
                guids.Add(_playlist.CurrentItem.Id);
                _playlist.Next();
            }
            
            // Validate that every id is unique (i.e. the same playlist item hasn't been played more than once)
            Assert.True(guids.Distinct().Count() == guids.Count);
            
            // Validate that every id is part of the playlist
            foreach (var id in guids)
            {
                var item = _playlist.Items.FirstOrDefault(x => x.Id == id);
                Assert.IsNotNull(item);
            }
        }

        [Test]
        public void EnsureThatEveryPlaylistItemIsPlayed_WithInsertedItems()
        {
            var guids = new List<Guid>();
            PreparePlaylistForTesting();

            var insertedItem = new PlaylistItem(new AudioFile());
            var insertedItem2 = new PlaylistItem(new AudioFile());
            var insertedItem3 = new PlaylistItem(new AudioFile());

            for (int a = 0; a < _playlist.Items.Count - 1; a++)
            {
                Console.WriteLine("a: {0} guid: {1} currentItemIndex: {2} count: {3}", a, _playlist.CurrentItem.Id, _playlist.CurrentItemIndex, _playlist.Items.Count);

                // Insert an item at the current index; should push current item an index further and count as played 
                if (a == 1)
                    _playlist.InsertItem(insertedItem, a);
                // Insert an item before the current index; should NOT count as played
                else if(a == 2)
                    _playlist.InsertItem(insertedItem2, a - 1);
                // Insert an item after the current index; should count as played
                else if (a == 3)
                    _playlist.InsertItem(insertedItem3, a + 1);

                guids.Add(_playlist.CurrentItem.Id);
                _playlist.Next();
            }

            // Validate that every id is unique (i.e. the same playlist item hasn't been played more than once)
            Assert.True(guids.Distinct().Count() == guids.Count);

            // Validate that every id is part of the playlist
            foreach (var id in guids)
            {
                var item = _playlist.Items.FirstOrDefault(x => x.Id == id);
                Assert.IsNotNull(item);
            }
        }
    }
}
