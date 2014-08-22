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
	public class WaveFormRequestServiceTest   
	{
        public IWaveFormRequestService Service { get; protected set; }

        Mock<IWaveFormRenderingService> _mockRenderingService;
        Mock<ITileCacheService> _mockCacheService;

        public void PrepareTests()
        {
            _mockRenderingService = new Mock<IWaveFormRenderingService>();
            _mockRenderingService.Setup(m => m.FlushCache());
            _mockRenderingService.Setup(m => m.LoadPeakFile(It.IsAny<AudioFile>()));
            _mockRenderingService.Setup(m => m.RequestBitmap(It.IsAny<WaveFormBitmapRequest>()));
            _mockCacheService = new Mock<ITileCacheService>();
            Service = new WaveFormRequestService(_mockRenderingService.Object, _mockCacheService.Object);
        }

        [TestFixture]
        public class FlushTest : WaveFormRequestServiceTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }
            
            [Test]
            public void CountShouldBeZero()
            {
                var task = Service.RequestBitmap(new BasicRectangle(), new BasicRectangle(), 1, WaveFormDisplayType.Stereo);
                task.Wait();
                Service.Flush();

                Assert.AreEqual(Service.Count, 0);
            }  
        }
	}
}
