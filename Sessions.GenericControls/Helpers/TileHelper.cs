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
            // Make zoom go in steps (1, 2, 3, etc.)
//            float zoomForTile = (float)(zoom/Math.Floor(zoom));
//            float zoomForX = zoom / tileZoom;
            //float zoomForTile = zoom;
            //float deltaZoom = zoom;
            //float zoomForTile = (float)Math.Floor(zoom);
            //int index = (int)Math.Floor((x * deltaZoom) / tileSize);
            //int index = (int)Math.Floor((x / deltaZoom) / tileSize);
            //int index = (int)Math.Floor(x / tileSize);
            //int index = (int)Math.Floor((x * zoomForX) / (tileSize * zoomForTile));

            // This one works with tilezoom == 1
            //int index = (int)Math.Floor(x / (tileSize * zoom));

            // The idea is to get a range from 1.0 to almost 2 for zoom.
            // i.e. 1.5 = 1.5, 2.5 = 1.5, 3.5 = 1.5, etc.
            float myzoom = (float)(zoom % Math.Floor(zoom)) + 1;
            int index = (int)Math.Floor(x / (tileSize * myzoom));

            //int index = (int)Math.Floor((x / tileZoom) / (tileSize * zoom));
            //int index = (int)Math.Floor(x / (tileSize * (zoom / tileZoom)));
            //Console.WriteLine("TileHelper - GetTileIndexAt - x: {0} zoom: {1} tileZoom: {2} tileSize: {3} --> myzoom: {4} index: {5}", x, zoom, tileZoom, tileSize, myzoom, index);
            //Console.WriteLine("TileHelper - GetTileIndexAt - x: {0} zoom: {1} tileZoom: {2} tileSize: {3} --> zoomForX: {4} zoomForTile: {5} index: {6}", x, zoom, tileZoom, tileSize, zoomForX, zoomForTile, index);
            return index;
        }

        public static int GetStartDirtyTile(float offsetX, float dirtyRectX, float zoom, int tileSize)
        {
            //return (int)Math.Floor((offsetX + dirtyRectX) / ((float)TileSize * deltaZoom));
            return GetTileIndexAt(offsetX + dirtyRectX, zoom, 1, tileSize);
        }

        public static int GetEndDirtyTile(float offsetX, float dirtyRectX, float dirtyRectWidth, float zoom, int tileSize)
        {
            //int numberOfDirtyTilesToDraw = (int)Math.Ceiling(context.DirtyRect.Width / tileSize) + 1;
            //return GetTileIndexAt(offsetX + dirtyRectX + dirtyRectWidth, zoom, 1, tileSize) + 1;
            int tileIndex = GetTileIndexAt(offsetX + dirtyRectX + dirtyRectWidth, zoom, 1, tileSize);
            float myzoom = (float)(zoom % Math.Floor(zoom)) + 1;
            return (int)(tileIndex * myzoom) + 1;
        }
    }
}
