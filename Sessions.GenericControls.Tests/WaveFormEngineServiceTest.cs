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

        public int ControlWidth = 500;
        public int ControlHeight = 100;
        public int TileSize = 50;

        public void PrepareTests()
        {
            _mockRenderingService = new Mock<IWaveFormRenderingService>();
            _mockRequestService = new Mock<IWaveFormRequestService>();
            _cacheService = new TileCacheService();

            Service = new WaveFormEngineService(_mockRenderingService.Object, _cacheService, _mockRequestService.Object);
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

        public WaveFormBitmapRequest GenerateRequest(float zoom, float offsetX, BasicRectangle dirtyRect)
        {
            float deltaZoom = (float) (zoom/Math.Floor(zoom));
            int startDirtyTile = (int)Math.Floor((offsetX + dirtyRect.X) / ((float)TileSize * deltaZoom));
            int numberOfDirtyTilesToDraw = (int)Math.Ceiling(dirtyRect.Width / TileSize) + 1;
            var request = new WaveFormBitmapRequest()
            {
                StartTile = startDirtyTile,
                EndTile = startDirtyTile + numberOfDirtyTilesToDraw,
                TileSize = TileSize,
                BoundsWaveForm = new BasicRectangle(0, 0, ControlWidth, ControlHeight),
                Zoom = zoom,
                DisplayType = WaveFormDisplayType.Stereo
            };
            return request;
        }

        [TestFixture]
        public class GetTilesTest : WaveFormEngineServiceTest
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
                for (int a = 0; a < ControlWidth; a += TileSize)
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
                float zoom = 1;
                float offsetX = 0;
                var dirtyRect = new BasicRectangle(0, 0, ControlWidth, ControlHeight);
                var request = GenerateRequest(zoom, offsetX, dirtyRect);

                // Act
                var tiles = Service.GetTiles(request);

                foreach (var tile in tiles)
                    Console.WriteLine("---> Returned tile offsetX: {0} zoom: {1}", tile.ContentOffset.X, tile.Zoom);

                Assert.IsTrue(_tilesAt100Percent.Except(tiles).ToList().Count == 0);
            }

            [Test]
            public void ShouldReturnTiles_ForZoomAt120()
            {
                // Arrange
                PrepareAllTilesAt100Percent();
                float zoom = 1.2f;
                float offsetX = 0;
                var dirtyRect = new BasicRectangle(0, 0, ControlWidth, ControlHeight);
                var request = GenerateRequest(zoom, offsetX, dirtyRect);

                // Act
                var tiles = Service.GetTiles(request);

                foreach (var tile in tiles)
                    Console.WriteLine("---> Returned tile offsetX: {0} zoom: {1}", tile.ContentOffset.X, tile.Zoom);

                Assert.IsTrue(_tilesAt100Percent.Except(tiles).ToList().Count == 0);
                //Assert.AreEqual(0, tiles.Count);
            }
        }
	}
}
