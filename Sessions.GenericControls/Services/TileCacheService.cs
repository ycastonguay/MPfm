// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services.Objects;

namespace Sessions.GenericControls.Services
{
    public class TileCacheService : ITileCacheService
    {
        private readonly object _lockerCache = new object();
        private List<WaveFormTile> _tileCache;
        private List<WaveFormTile> _tileCacheForScrollBar; // to be merged?

        public int Count
        {
            get { return _tileCache.Count; }
        }

        public TileCacheService()
        {
            _tileCache = new List<WaveFormTile>();
            _tileCacheForScrollBar = new List<WaveFormTile>();
        }

        public void Flush()
        {
            lock (_lockerCache)
            {
                foreach (var tile in _tileCache)
                    if(tile.Image != null && tile.Image.Image != null)
                        tile.Image.Image.Dispose();

                foreach (var tile in _tileCacheForScrollBar)
                    if(tile.Image != null && tile.Image.Image != null)
                        tile.Image.Image.Dispose();

                _tileCache.Clear();
                _tileCacheForScrollBar.Clear();
            }
        }

        public void AddTile(WaveFormTile tile, bool isScrollBar)
        {
            lock (_lockerCache)
            {
                _tileCache.Add(tile);
            }
        }

        public WaveFormTile GetTile(float contentOffsetX, bool isScrollBar)
        {
            WaveFormTile tile = null;
            lock (_lockerCache)
            {
                if(isScrollBar)
                    tile = _tileCacheForScrollBar.FirstOrDefault(x => x.ContentOffset.X == contentOffsetX);
                else
                    tile = _tileCache.FirstOrDefault(x => x.ContentOffset.X == contentOffsetX);
            }
            return tile;
        }

        public WaveFormTile GetTile(float contentOffsetX, float zoom)
        {
            WaveFormTile tile = null;
            lock (_lockerCache)
            {
                tile = _tileCache.FirstOrDefault(x => x.ContentOffset.X == contentOffsetX && x.Zoom == zoom);
            }
            return tile;
        }

        public List<WaveFormTile> GetTilesForPosition(float x, float zoom)
        {
            var tiles = new List<WaveFormTile>();
            float zoomThreshold = (float)Math.Floor(zoom);
            lock (_lockerCache)
            {
//                foreach (var tile in _tileCache)
//                    Console.WriteLine("TileCacheService - GetTilesForPosition - x: {0} zoom: {1} ## currentTile --> tileZoom: {2} tileOffsetX: {3} tileAdjustedOffsetX: {4}", x, zoom, tile.Zoom, tile.ContentOffset.X, tile.GetAdjustedContentOffsetForZoom(x, WaveFormCacheService.TileSize, zoomThreshold));

                // Try to get a bitmap tile that covers the area we're interested in. The content offset x must be adjusted depending on the zoom level because a tile with
                // a lower zoom might cover a very large area when adjusted to the new zoom; we need to draw the bitmap tile with a large offset (most of the bitmap is off screen)
                tiles = _tileCache.Where(obj => obj.ContentOffset.X == obj.GetAdjustedContentOffsetForZoom(x, WaveFormEngineService.TileSize, zoomThreshold)).ToList();
            }

            return tiles;
        }

        public List<WaveFormTile> GetTilesToFillBounds(float zoom, BasicRectangle bounds)
        {
            var tiles = new List<WaveFormTile>();
            float zoomThreshold = (float)Math.Floor(zoom);
            lock (_lockerCache)
            {
                tiles = _tileCache.Where(obj => obj.CheckIfTileIsInBounds(WaveFormEngineService.TileSize, zoomThreshold, bounds)).ToList();
            }

            return tiles;
        }
    }
}
