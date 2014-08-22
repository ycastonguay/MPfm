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
using Sessions.GenericControls.Graphics;
using System.Threading;

namespace Sessions.GenericControls.Tests
{
	[TestFixture]
	public class AlbumArtRequestServiceTest   
	{
        protected const string ArtistName = "ArtistName";
        protected const string AlbumTitle = "AlbumTitle";
        protected const string AudioFilePath = "/fake/audio.mp3";
        protected const int ImageSize = 300;
        protected const int UserData = 4;

        protected IAlbumArtRequestService Service { get; set; }
        protected AlbumArtRequest Request { get; set; }

        private Mock<IAlbumArtCacheService> _mockCacheService;
        private Mock<IDisposableImageFactory> _mockDisposableImageFactory;

        public void PrepareTests()
        {
            _mockCacheService = new Mock<IAlbumArtCacheService>();
            _mockDisposableImageFactory = new Mock<IDisposableImageFactory>();
            Service = new AlbumArtRequestService(_mockCacheService.Object, _mockDisposableImageFactory.Object);
            Request = new AlbumArtRequest(){
                ArtistName = ArtistName,
                AlbumTitle = AlbumTitle,
                AudioFilePath = AudioFilePath,
                Width = ImageSize,
                Height = ImageSize,
                UserData = UserData
            };
        }

        [TestFixture]
        public class FlushTest : AlbumArtRequestServiceTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }
            
            [Test]
            public void CountShouldBeZero()
            {
                Service.RequestAlbumArt(Request);
                Service.FlushRequests();

                Assert.AreEqual(0, Service.Count);
            }  
        }

        [TestFixture]
        public class RequestAlbumArtTest : AlbumArtRequestServiceTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }

            [Test]
            public void ShouldReturnAlbumArt()
            {
                var autoEvent = new AutoResetEvent(false);
                Service.OnAlbumArtExtracted += (image, request) => {
                    Assert.IsNotNull(image);
                    autoEvent.Set();
                };
                Service.RequestAlbumArt(Request);
                autoEvent.WaitOne();
            }  
        }
	}
}
