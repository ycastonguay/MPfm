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
using MPfm.GenericControls.Basics;

namespace MPfm.GenericControls.Services.Objects
{
    public class WaveFormTile
    {
        public IDisposable Image { get; set; }
        public BasicPoint ContentOffset { get; set; }
        public float Zoom { get; set; }

        public float GetAdjustedContentOffsetForZoom(float x, float tileSize, float zoom)
        {
            //double x = tileSize * 5.0;
            //float zoomDiff = zoom - Zoom; // gives 0 for the same zoom... 
            //float xAdj = x / zoomDiff;
            float deltaZoom = zoom / Zoom;
            float xAdj = x * (1 / deltaZoom); 
            float xFloor = (float) (Math.Floor(xAdj / tileSize) * tileSize);
            //Console.WriteLine("x: {0} zoomDiff: {1} xAdj: {2} xFloor: {3}", x, zoomDiff, xAdj, xFloor);
            return xFloor;
        }

        public WaveFormTile()
        {
            ContentOffset = new BasicPoint();
            Zoom = 1;
        }
    }
}