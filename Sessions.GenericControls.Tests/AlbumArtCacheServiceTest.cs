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
using Moq;
using NUnit.Framework;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services;
using Sessions.GenericControls.Services.Objects;
using Sessions.GenericControls.Basics;
using System.Collections.Generic;
using System.Linq;
using Sessions.Sound.AudioFiles;

namespace Sessions.GenericControls.Tests
{
	[TestFixture]
	public class AlbumArtCacheServiceTest   
	{
        protected const string ArtistName = "ArtistName";
        protected const string AlbumTitle = "AlbumTitle";

        protected IAlbumArtCacheService Service { get; set; }
        protected Mock<IBasicImage> TestImageMock { get; set; }

        public void PrepareTests()
        {
            Service = new AlbumArtCacheService();
            TestImageMock = new Mock<IBasicImage>();
        }

        [TestFixture]
        public class FlushTest : AlbumArtCacheServiceTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }
            
            [Test]
            public void CountShouldBeZero()
            {
                Service.AddAlbumArt(TestImageMock.Object, ArtistName, AlbumTitle);
                Service.Flush();

                Assert.AreEqual(0, Service.Count);
            }  
        }

        [TestFixture]
        public class AddAlbumArtTest : AlbumArtCacheServiceTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }
            
            [Test]
            public void ShouldAddAlbum()
            {
                Service.AddAlbumArt(TestImageMock.Object, ArtistName, AlbumTitle);

                Assert.AreEqual(1, Service.Count);
            }  
        }

        [TestFixture]
        public class GetAlbumArtTest : AlbumArtCacheServiceTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }
            
            [Test]
            public void ShouldGetAlbum()
            {
                Service.AddAlbumArt(TestImageMock.Object, ArtistName, AlbumTitle);
                var image = Service.GetAlbumArt(ArtistName, AlbumTitle);

                Assert.AreEqual(TestImageMock.Object, image);
            }  

            [Test]
            public void ShouldReturnNullIfAlbumNotFound()
            {
                var image = Service.GetAlbumArt(ArtistName, AlbumTitle);

                Assert.AreEqual(null, image);
            }  

        }
	}
}
