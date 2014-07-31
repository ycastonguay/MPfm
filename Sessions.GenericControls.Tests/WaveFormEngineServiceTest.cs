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
using Sessions.GenericControls.Basics;
using System.Collections.Generic;
using System.Linq;

namespace Sessions.GenericControls.Tests
{
	[TestFixture]
	public class WaveFormEngineServiceTest   
	{
        Mock<IWaveFormRenderingService> _mockRenderingService;
        Mock<IWaveFormRequestService> _mockRequestService;
        ITileCacheService _cacheService;

        public IWaveFormEngineService Service { get; protected set; }
        public BasicRectangle ControlFrame { get; protected set; }

        public const int TileSize = 50;

        public void PrepareTests()
        {
            _mockRenderingService = new Mock<IWaveFormRenderingService>();
            _mockRequestService = new Mock<IWaveFormRequestService>();
            _cacheService = new TileCacheService();

            Service = new WaveFormEngineService(_mockRenderingService.Object, _cacheService, _mockRequestService.Object);
            ControlFrame = new BasicRectangle(0, 0, 500, 100);
        }    

        public WaveFormTile AddTile(float offsetX, float zoom)
        {
            var tile = new WaveFormTile()
            {
                ContentOffset = new BasicPoint(offsetX, 0),
                Zoom = zoom
            };
            _cacheService.AddTile(tile, false);
            return tile;
        }        

        [TestFixture]
        public class GetTilesTest : WaveFormEngineServiceTest // Rename for GettilesTestWithTilesAt100Pct?
        {
            private List<WaveFormTile> _tilesAt100Percent;

            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }                

            private void PrepareAllTilesAt100Percent()
            {
                // For this test, we only need to mock the data in the cache service.
                _tilesAt100Percent = new List<WaveFormTile>();
                for (int a = 0; a < ControlFrame.Width; a += TileSize)
                {
                    Console.WriteLine("Adding tile at {0}", a);
                    var tile = AddTile(a, 1);
                    _tilesAt100Percent.Add(tile);
                }
            }

            [Test]
            public void ShouldReturnTiles_ForZoomAt100()
            {
                // Arrange
                PrepareAllTilesAt100Percent();
                var dirtyRect = new BasicRectangle(0, 0, ControlFrame.Width, ControlFrame.Height);
                var request = Service.GetTilesRequest(0, 1, ControlFrame, dirtyRect, WaveFormDisplayType.Stereo);

                // Act
                var tiles = Service.GetTiles(request);

                //Assert.IsTrue(_tilesAt100Percent.Except(tiles).ToList().Count == 0);
                Assert.IsTrue(_tilesAt100Percent.SequenceEqual(tiles));
            }

            [Test]
            public void ShouldReturnTiles_ForZoomAt120()
            {
                // Arrange
                PrepareAllTilesAt100Percent();
                var dirtyRect = new BasicRectangle(0, 0, ControlFrame.Width, ControlFrame.Height);
                var request = Service.GetTilesRequest(0, 1.2f, ControlFrame, dirtyRect, WaveFormDisplayType.Stereo);

                // Act
                var tiles = Service.GetTiles(request);

                Assert.IsTrue(_tilesAt100Percent.SequenceEqual(tiles));
            }

            [Test]
            public void ShouldReturnTiles_ForZoomAt200()
            {
                // Arrange
                PrepareAllTilesAt100Percent();
                var dirtyRect = new BasicRectangle(0, 0, ControlFrame.Width - 50, ControlFrame.Height);
                var request = Service.GetTilesRequest(0, 2, ControlFrame, dirtyRect, WaveFormDisplayType.Stereo);

                // Act
                var tiles = Service.GetTiles(request);

//                foreach (var tile in tiles)
//                    Console.WriteLine("---> Returned tile offsetX: {0} zoom: {1}", tile.ContentOffset.X, tile.Zoom);

                // Should only return the first five tiles (i.e. we are seeing only half of the content)
                var expectedTiles = new List<WaveFormTile>();
                expectedTiles.Add(_tilesAt100Percent[0]);
                expectedTiles.Add(_tilesAt100Percent[1]);
                expectedTiles.Add(_tilesAt100Percent[2]);
                expectedTiles.Add(_tilesAt100Percent[3]);
                expectedTiles.Add(_tilesAt100Percent[4]);
                Assert.IsTrue(expectedTiles.SequenceEqual(tiles));
            }

//            [Test]
//            public void ShouldReturnTiles_ForZoomAt200AndOffsetAt200()
//            {
//                // Arrange
//                PrepareAllTilesAt100Percent();
//                float zoom = 2f;
//                float offsetX = 200;
//                var dirtyRect = new BasicRectangle(0, 0, ControlFrame.Width, ControlFrame.Height);
//
//                // Act
//                var request = Service.GetTilesRequest(offsetX, zoom, ControlFrame, dirtyRect, WaveFormDisplayType.Stereo);
//                var tiles = Service.GetTiles(request);
//
//                foreach (var tile in tiles)
//                    Console.WriteLine("---> Returned tile offsetX: {0} zoom: {1}", tile.ContentOffset.X, tile.Zoom);
//
//                // how do we assert?
//
//                // With all tiles generated at 100%:
//                // If the screen width is 200 and tile size is 50
//                // At 100%, offset 0   ---> Should return all tiles (count=4)
//                // At 200%, offset 0   ---> Should return first two tiles (count=2)
//                // At 200%, offset 100 ---> Should return tile 2 and tile 3 (count=2)
//                // 0 50 100 150
//                // * * * * * * *
//                // *   *   *   *
//
//                // 
//                // * * * * * * *
//                // *   *   *   *
//
//                Assert.IsTrue(_tilesAt100Percent.Except(tiles).ToList().Count == 0);
//            }
        }
	}
}
