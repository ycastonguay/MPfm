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
using Sessions.GenericControls.Services;
using Sessions.GenericControls.Services.Objects;
using Sessions.GenericControls.Basics;
using System.Collections.Generic;
using System.Linq;

namespace Sessions.GenericControls.Tests
{
	[TestFixture]
	public class TileCacheServiceTest   
	{
        public ITileCacheService Service { get; protected set; }

        public void PrepareTests()
        {
            Service = new TileCacheService();
        }

        public WaveFormTile AddTile(float offsetX, float zoom)
        {
            var tile = new WaveFormTile(){
                ContentOffset = new BasicPoint(offsetX, 0),
                Zoom = zoom
            };
            Service.AddTile(tile, false);
            return tile;
        }

        [TestFixture]
        public class FlushTest : TileCacheServiceTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }
            
            [Test]
            public void CountShouldBeZero()
            {
                Service.AddTile(new WaveFormTile(), false);
                Service.Flush();

                Assert.AreEqual(Service.Count, 0);
            }  
        }

        [TestFixture]
        public class AddTileTest : TileCacheServiceTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }
            
            [Test]
            public void ShouldAddAndReturnTile()
            {                
                var offset = new BasicPoint(1, 0);
                var zoom = 2;
                var tile = new WaveFormTile(){
                    ContentOffset = offset,
                    Zoom = zoom
                };
                Service.AddTile(tile, false);
                var newTile = Service.GetTile(offset.X, zoom);

                Assert.AreEqual(tile, newTile);
            }  
        }

        [TestFixture]
        public class GetTileTest : TileCacheServiceTest
        {
            private BasicPoint _offset;
            private float _zoom;
            private WaveFormTile _tile;

            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();

                _offset = new BasicPoint(1, 0);
                _zoom = 2;
                _tile = new WaveFormTile(){
                    ContentOffset = _offset,
                    Zoom = _zoom
                };
                Service.AddTile(_tile, false);
            }
            
            [Test]
            public void ShouldAddAndReturnTile()
            {                
                var newTile = Service.GetTile(_offset.X, _zoom);

                Assert.AreEqual(_tile, newTile);
            }  

            [Test]
            public void ShouldReturnNull()
            {                
                var newTile = Service.GetTile(_offset.X, 1);

                Assert.IsNull(newTile);
            } 
        }

        [TestFixture]
        public class GetTilesForPositionTest : TileCacheServiceTest
        {
            private float _offsetX = 50;
            private float _startZoom = 1;
            private List<WaveFormTile> _tiles;

            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();

                _tiles = new List<WaveFormTile>();

                // Generate tiles for testing
                float x = _offsetX;
                float zoom = _startZoom;
                for (int a = 0; a < 4; a++)
                {
                    // Generate a tile for expected results
                    _tiles.Add(AddTile(x * zoom, zoom));

                    // Generate a tile that shouldn't be in the expected results
                    // Is it possible though that some tiles are in double when generating this way?
                    AddTile((x * 2) * zoom, zoom);

                    zoom += 1;
                }
            }

            [Test]
            public void ShouldReturnTiles()
            {                
                // Check which tests to do. tile at == 0, tile at == 50, depending on what bugs are found.            
                var tiles = Service.GetTilesForPosition(_offsetX, _startZoom);

                Assert.IsTrue(tiles.Except(_tiles).ToList().Count == 0);
            } 

            [Test]
            public void ShouldReturnEmptyList()
            {     
                // Fails           
                var tiles = Service.GetTilesForPosition(_offsetX + 10, _startZoom);

                Assert.AreEqual(tiles.Count, 0);
            } 
        }

        [TestFixture]
        public class GetTilesToFillBoundsTest : TileCacheServiceTest
        {
            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }
            
            [Test]
            public void ShouldIncrementCount()
            {                
            }  
        }
	}
}
