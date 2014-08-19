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
using System.Collections.Generic;
using System.Linq;
using Sessions.GenericControls.Services.Objects;

namespace Sessions.GenericControls.Helpers
{
    public static class TileHelper
    {
        public static WaveFormTile GetOptimalTileAtZoom(IEnumerable<WaveFormTile> tiles, float zoom)
        {
            // We don't want to scale down bitmaps, it is more CPU intensive than scaling up
            var orderedTiles = tiles.OrderBy(obj => obj.Zoom).ToList();
            var tile = orderedTiles.Count > 0 ? orderedTiles[0] : null;
            foreach (var thisTile in orderedTiles)
            {
                if (thisTile.Zoom <= zoom)
                {
                    //if (tile != null && thisTile.Zoom > tile.Zoom || tile == null)
                    if (thisTile.Zoom > tile.Zoom)
                    {
                        tile = thisTile;
                    }
                }
            }
            return tile;
        }

        public static int GetTileIndexAt(float x, float zoom, float tileZoom, int tileSize)
        {
            float floorZoom = (float)(zoom / Math.Floor(zoom));
            float adjustedX = (x / floorZoom);
            int index = (int)Math.Floor(adjustedX / tileSize);
            return index;
        }

        public static int GetStartDirtyTile(float offsetX, float dirtyRectX, float zoom, int tileSize)
        {
            return GetStartDirtyTile(offsetX, dirtyRectX, zoom, 1, tileSize);
        }

        public static int GetStartDirtyTile(float offsetX, float dirtyRectX, float zoom, float tileZoom, int tileSize)
        {
            return GetTileIndexAt(offsetX + dirtyRectX, zoom, tileZoom, tileSize);
        }

        public static int GetEndDirtyTile(float offsetX, float dirtyRectX, float dirtyRectWidth, float zoom, int tileSize)
        {
            return GetEndDirtyTile(offsetX, dirtyRectX, dirtyRectWidth, zoom, 1, tileSize);
        }

        public static int GetEndDirtyTile(float offsetX, float dirtyRectX, float dirtyRectWidth, float zoom, float tileZoom, int tileSize)
        {
            //Console.WriteLine("TileHelper - GetEndDirtyTile - zoom: {0} tileZoom: {1} --- offsetX: {2} dirtyRectX: {3} dirtyRectWidth: {4} --- X: {5}", zoom, tileZoom, offsetX, dirtyRectX, dirtyRectWidth, offsetX + dirtyRectX + dirtyRectWidth);
            int tileIndex = GetTileIndexAt(offsetX + dirtyRectX + dirtyRectWidth, zoom, tileZoom, tileSize);
            return tileIndex;
        }
    }
}
