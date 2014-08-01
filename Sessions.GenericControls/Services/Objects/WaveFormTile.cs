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
using Sessions.GenericControls.Basics;

namespace Sessions.GenericControls.Services.Objects
{
    public class WaveFormTile
    {
        public IBasicImage Image { get; set; }
        public BasicPoint ContentOffset { get; set; }
        public float Zoom { get; set; }

        /// <summary>
        /// Returns the content offset X for the tile covering the area at X at the specified zoom factor.
        /// </summary>
        /// <param name="x">Content offset x @ current zoom</param>
        /// <param name="tileSize">Tile size</param>
        /// <param name="zoom">Zoom factor (mathes zoom for content offset x)</param>
        /// <returns>Adjusted content offset x</returns>
        public float GetAdjustedContentOffsetForZoom(float x, int tileSize, float zoom)
        {
            // Adjust the content offset x so we take the tile that covers the area in a different zoom factor.
            // i.e. if we request a tile at position 100 for zoom 300%, and only a tile at zoom 100% is available, this means we need to use the tile at content offset x == 0.
            float deltaZoom = zoom / Zoom;
            float xAdj = zoom > Zoom ? x * (1 / deltaZoom) : x * deltaZoom; 
            float xFloor = (float) (Math.Floor(xAdj / tileSize) * tileSize);
            //Console.WriteLine("   WaveFormTile - GetAdjustedContentOffsetForZoom x: {0} tileSize: {1} zoom: {2} // ContentOffset.X: {3} Zoom: {4} deltaZoom: {5} xAdj: {6} xFloor: {7}", x, tileSize, zoom, ContentOffset.X, Zoom, deltaZoom, xAdj, xFloor);
            return xFloor; // returns the wrong kind of tiles sometimes
        }

        public bool CheckIfTileIsInBounds(int tileSize, float zoom, BasicRectangle bounds)
        {
            float deltaZoom = zoom / Zoom;
            float tileWidth = tileSize*deltaZoom;
            //float offsetX = GetAdjustedContentOffsetForZoom(x, tileSize, zoom);
            //float offsetX = x;
            float offsetX = ContentOffset.X*deltaZoom;
            //Console.WriteLine("WaveFormTile - CheckIfTileIsInBounds - offsetX: {0} tileWidth: {1} tileZoom: {2} bounds: {3} zoom: {4}", offsetX, tileWidth, Zoom, bounds, zoom);
            //return offsetX >= bounds.X && offsetX + tileWidth <= bounds.X + bounds.Width;
            return offsetX <= bounds.X && offsetX + tileWidth >= bounds.X + bounds.Width;
        }

        public WaveFormTile()
        {
            ContentOffset = new BasicPoint();
            Zoom = 1;
        }
    }
}