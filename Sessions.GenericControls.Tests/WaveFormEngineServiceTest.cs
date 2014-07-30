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
using NUnit.Framework;
using Sessions.GenericControls.Services.Interfaces;
using Moq;
using Sessions.GenericControls.Services;
using Sessions.Sound.AudioFiles;
using Sessions.GenericControls.Services.Objects;

namespace Sessions.GenericControls.IntegrationTests
{
	[TestFixture]
	public class WaveFormEngineServiceTest   
	{
        Mock<IWaveFormRenderingService> _mockRenderingService;
        Mock<IWaveFormRequestService> _mockRequestService;
        Mock<ITileCacheService> _mockCacheService;

        public IWaveFormEngineService Service { get; protected set; }

        [SetUp]
        public void InitializeTests()
        {
            _mockRenderingService = new Mock<IWaveFormRenderingService>();
            _mockRenderingService.Setup(m => m.FlushCache());
            _mockRenderingService.Setup(m => m.LoadPeakFile(It.IsAny<AudioFile>()));
            _mockRenderingService.Setup(m => m.RequestBitmap(It.IsAny<WaveFormBitmapRequest>()));
            _mockCacheService = new Mock<ITileCacheService>();
            _mockRequestService = new Mock<IWaveFormRequestService>();
            Service = new WaveFormEngineService(_mockRenderingService.Object, _mockCacheService.Object, _mockRequestService.Object);

//            var memoryGraphicsContextMock = new Mock<IMemoryGraphicsContext>();
//            var memoryGraphicsContextFactoryMock = new Mock<IMemoryGraphicsContextFactory>();
//            memoryGraphicsContextFactoryMock.Setup(m => m.CreateMemoryGraphicsContext(It.IsAny<float>(), It.IsAny<float>()))
//                                            .Returns(() => memoryGraphicsContextMock.Object);
//
//            base.SetMemoryGraphicsContextFactory(memoryGraphicsContextFactoryMock.Object);
        }

        [Test]
        public void Test()
        {
            var audioFile = new AudioFile();
            audioFile.ArtistName = "ArtistName";
            audioFile.AlbumTitle = "AlbumTitle";
            audioFile.Title = "SongTitle";

            Service.LoadPeakFile(audioFile);
        }
	}
}
