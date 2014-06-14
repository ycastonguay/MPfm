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
    public class PlaylistTest
    {        
        // Context:
        // - The playlist can contain thousands of items (i.e. do not randomize all the list at once)        
        // - An audio file can be added several times to the same playlist
        // - The playlist contents can change until the playlist is complete

        // Restrictions:
        // - The same song must not play until the playlist is complete
        
        protected TestConfigurationEntity Config;
        protected TestDevice TestDevice;

        public virtual Playlist Playlist { get; protected set; }

        public PlaylistTest()
        {
            Config = TestConfigurationHelper.Load();            
        }
        
        protected void InitializeBass()
        {
            //Console.WriteLine("Initializing Bass...");
            Base.Register(BassNetKey.Email, BassNetKey.RegistrationKey);
            TestDevice = new TestDevice(DriverType.DirectSound, -1, 44100);
        }

        protected virtual void PreparePlaylist()
        {
            Playlist = new Playlist();
            for (int a = 0; a < Config.AudioFilePaths.Count; a++)
                Playlist.AddItem(new PlaylistItem(new AudioFile(Config.AudioFilePaths[a])));
        }

        [TestFixture]
        public class AddItemTest : PlaylistTest
        {
            private const int StartIndex = 1;
            
            private PlaylistItem PrepareTest()
            {
                PreparePlaylist();
                for(int a = 0; a < StartIndex; a++)
                    Playlist.Next();
                
                var item = new PlaylistItem(new AudioFile());
                Playlist.AddItem(item);
                return item;
            }
            
            [Test]
            public void ShouldIncrementCount()
            {
                PrepareTest();
                Assert.AreEqual(Playlist.Items.Count, Config.AudioFilePaths.Count + 1);
            }            
            
            [Test]
            public void ShouldKeepSameItemIndex()
            {
                PrepareTest();
                Assert.AreEqual(Playlist.CurrentItemIndex, 1);
            }            
            
            [Test]
            public void ListShouldContainItem()
            {
                var item = PrepareTest();
                Assert.Contains(item, Playlist.Items);
            }                        
        }

        [TestFixture]
        public class InsertItemTest : PlaylistTest
        {
            private const int StartIndex = 1;
            
            private PlaylistItem PrepareTest(int index)
            {
                PreparePlaylist();                
                for(int a = 0; a < StartIndex; a++)
                    Playlist.Next();
                
                var item = new PlaylistItem(new AudioFile());
                Playlist.InsertItem(item, index);
                return item;
            }

            [Test]
            public void AtCurrentIndex_ShouldIncrementCount()
            {
                PrepareTest(StartIndex);
                Assert.AreEqual(Playlist.Items.Count, Config.AudioFilePaths.Count + 1);
            }            

            [Test]
            public void AtCurrentIndex_ShouldKeepSameItemIndex()
            {
                PrepareTest(StartIndex);
                Assert.AreEqual(Playlist.CurrentItemIndex, 1);
            }            

            [Test]
            public void AtCurrentIndex_ListShouldContainItem()
            {
                var item = PrepareTest(StartIndex);
                Assert.Contains(item, Playlist.Items);
            }            
            
            [Test]
            public void BeforeCurrentIndex_ShouldIncrementCount()
            {               
                PrepareTest(StartIndex - 1);
                Assert.AreEqual(Playlist.Items.Count, Config.AudioFilePaths.Count + 1);
            }

            [Test]
            public void BeforeCurrentIndex_ShouldIncrementItemIndex()
            {   
                PrepareTest(StartIndex - 1);
                Assert.AreEqual(Playlist.CurrentItemIndex, 2);
            }

            [Test]
            public void BeforeCurrentIndex_ListShouldContainItem()
            {   
                var item = PrepareTest(StartIndex - 1);
                Assert.Contains(item, Playlist.Items);
            }     
            
            [Test]
            public void AfterCurrentIndex_ShouldIncrementCount()
            {
                PrepareTest(StartIndex + 1);
                Assert.AreEqual(Playlist.Items.Count, Config.AudioFilePaths.Count + 1);
            }
            
            [Test]
            public void AfterCurrentIndex_ShouldKeepSameItemIndex()
            {
                PrepareTest(StartIndex + 1);
                Assert.AreEqual(Playlist.CurrentItemIndex, 1);
            }
            
            [Test]
            public void AfterCurrentIndex_ListShouldContainItem()
            {
                var item = PrepareTest(StartIndex + 1);
                Assert.Contains(item, Playlist.Items);
            }            
        }
        
        [TestFixture]
        public class RemoveItemTest : PlaylistTest
        {        
            private const int StartIndex = 1;
            
            private PlaylistItem PrepareTest(int index)
            {
                PreparePlaylist();
                for(int a = 0; a < StartIndex; a++)
                    Playlist.Next();
                
                var item = Playlist.Items[index];
                Playlist.RemoveItem(index);
                return item;
            }
            
            [Test]
            public void AtCurrentIndex_ShouldDecrementCount()
            {
                PrepareTest(StartIndex);
                Assert.AreEqual(Playlist.Items.Count, Config.AudioFilePaths.Count - 1);
            }  

            [Test]
            public void AtCurrentIndex_ShouldKeepSameItemIndex()
            {
                PrepareTest(StartIndex);
                Assert.AreEqual(Playlist.CurrentItemIndex, 1);
            }  
            
            [Test]
            public void AtCurrentIndex_ListShouldNotContainItem()
            {
                var item = PrepareTest(StartIndex);
                Assert.False(Playlist.Items.Contains(item));
            }              
            
            [Test]
            public void BeforeCurrentIndex_ShouldDecrementCount()
            {
                PrepareTest(StartIndex - 1);
                Assert.AreEqual(Playlist.Items.Count, Config.AudioFilePaths.Count - 1);
            }  

            [Test]
            public void BeforeCurrentIndex_ShouldDecrementItemIndex()
            {
                PrepareTest(StartIndex - 1);
                Assert.AreEqual(Playlist.CurrentItemIndex, 0);
            }  
            
            [Test]
            public void BeforeCurrentIndex_ListShouldNotContainItem()
            {
                var item = PrepareTest(StartIndex - 1);
                Assert.False(Playlist.Items.Contains(item));
            }    
            
            [Test]
            public void AfterCurrentIndex_ShouldDecrementCount()
            {
                PrepareTest(StartIndex + 1);
                Assert.AreEqual(Playlist.Items.Count, Config.AudioFilePaths.Count - 1);
            }  

            [Test]
            public void AfterCurrentIndex_ShouldKeepSameItemIndex()
            {
                PrepareTest(StartIndex + 1);
                Assert.AreEqual(Playlist.CurrentItemIndex, 1);
            }  
            
            [Test]
            public void AfterCurrentIndex_ListShouldNotContainItem()
            {
                var item = PrepareTest(StartIndex + 1);
                Assert.False(Playlist.Items.Contains(item));
            }
        }
        
        [TestFixture]
        public class RemoveItemsTest : PlaylistTest
        {
            private const int StartIndex = 1;
            private const int NumberOfItemsToRemove = 1;
            
            private List<Guid> PrepareTest()
            {
                PreparePlaylist();
                
                var guids = new List<Guid>();
                for(int a = 1; a < NumberOfItemsToRemove; a++)
                    guids.Add(Playlist.Items[a].Id);
                
                for(int a = 0; a < StartIndex; a++)
                    Playlist.Next();
                Playlist.RemoveItems(guids);
                
                return guids;                
            }  
            
            [Test]
            public void ShouldDecrementCount()
            {
                var guids = PrepareTest();
                Assert.AreEqual(Playlist.Items.Count, Config.AudioFilePaths.Count - guids.Count);
            }
            
            [Test]
            public void ListShouldNotContainItems()
            {
                var guids = PrepareTest();
                foreach(var guid in guids)
                    Assert.IsNull(Playlist.Items.FirstOrDefault(x => x.Id == guid));
            }            
        }           
        
        [TestFixture]
        public class ClearTest : PlaylistTest
        {
            private void PrepareTest()
            {
                PreparePlaylist();
                Playlist.Clear();
            }
            
            [Test]
            public void ListShouldBeEmpty()
            {
                PrepareTest();
                Assert.IsEmpty(Playlist.Items);
                //Assert.IsEmpty(Playlist.PlayedItemIds);
            }
            
            [Test]
            public void ItemIndexShouldBeZero()
            {
                PrepareTest();
                Assert.AreEqual(Playlist.CurrentItemIndex, 0);
            }
        }
        
        [TestFixture]
        public class DisposeChannelsTest : PlaylistTest
        {
            private void PrepareTest()
            {
                InitializeBass();                
                PreparePlaylist();
                
                foreach (var item in Playlist.Items)
                    item.Load(false);
            }

            [Test]
            public void DisposeChannels_ShouldBeDisposed()
            {
                PrepareTest();

                Playlist.DisposeChannels();
                foreach (var item in Playlist.Items)
                    Assert.IsNull(item.Channel);
            }
        }

        [TestFixture]
        public class PreviousTest : PlaylistTest
        {
            private void PrepareTest()
            {
                PreparePlaylist();
            }

            [Test]
            public void ShouldDecrementCurrentItemIndex_UnlessThisIsTheFirstItem()
            {
                PrepareTest();
                Playlist.Next();                    

                int index = Playlist.CurrentItemIndex;
                Playlist.Previous();

                Assert.AreEqual(Playlist.CurrentItemIndex, index - 1);
            }

            [Test]
            public void ShouldSetCurrentItemToNextItem_UnlessThisIsTheFirstItem()
            {
                PrepareTest();
                Playlist.Next();

                int index = Playlist.CurrentItemIndex;
                Playlist.Previous();

                Assert.AreEqual(Playlist.CurrentItem, Playlist.Items[index - 1]);
            }

            [Test]
            public void ShouldKeepCurrentItemIndex_WhenThisIsTheFirstItem()
            {
                PrepareTest();

                int index = Playlist.CurrentItemIndex;
                Playlist.Previous();

                Assert.AreEqual(Playlist.CurrentItemIndex, index);
            }

            [Test]
            public void ShouldKeepCurrentItem_WhenThisIsTheFirstItem()
            {
                PrepareTest();

                int index = Playlist.CurrentItemIndex;
                Playlist.Previous();

                Assert.AreEqual(Playlist.CurrentItem, Playlist.Items[index]);
            }

            //[Test]
            //public void ShouldSetCurrentItemIndexToMinusOne_IfThisIsTheLastItemAndPlaylistRepeatIsDisabled()

            //[Test]
            //public void ShouldSetCurrentItemToNull_IfThisIsTheLastItemAndPlaylistRepeatIsDisabled()
        }
        
        [TestFixture]
        public class NextTest : PlaylistTest
        {
            private void PrepareTest()
            {
                PreparePlaylist();
            }

            [Test]
            public void ShouldIncrementCurrentItemIndex_UnlessThisIsTheLastItem()
            {
                PrepareTest();

                int index = Playlist.CurrentItemIndex;
                Playlist.Next();

                Assert.AreEqual(Playlist.CurrentItemIndex, index + 1);
            }

            [Test]
            public void ShouldSetCurrentItemToNextItem_UnlessThisIsTheLastItem()
            {
                PrepareTest();

                int index = Playlist.CurrentItemIndex;
                Playlist.Next();

                Assert.AreEqual(Playlist.CurrentItem, Playlist.Items[index + 1]);
            }

            [Test]
            public void ShouldKeepCurrentItemIndex_WhenThisIsTheLastItem()
            {
                PrepareTest();
                for (int a = 0; a < Playlist.Items.Count - 1; a++)
                    Playlist.Next();

                int index = Playlist.CurrentItemIndex;
                Playlist.Next();

                Assert.AreEqual(Playlist.CurrentItemIndex, index);
            }

            [Test]
            public void ShouldKeepCurrentItem_WhenThisIsTheLastItem()
            {
                PrepareTest();
                for (int a = 0; a < Playlist.Items.Count - 1; a++)
                    Playlist.Next();

                int index = Playlist.CurrentItemIndex;
                Playlist.Next();

                Assert.AreEqual(Playlist.CurrentItem, Playlist.Items[index]);
            }

            //[Test]
            //public void ShouldSetCurrentItemIndexToMinusOne_IfThisIsTheLastItemAndPlaylistRepeatIsDisabled()

            //[Test]
            //public void ShouldSetCurrentItemToNull_IfThisIsTheLastItemAndPlaylistRepeatIsDisabled()
        }
    }
}
