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

using MPfm.GenericControls.Basics;

namespace MPfm.GenericControls.Services.Objects
{
    public class WaveFormBitmapRequest
    {
        public WaveFormDisplayType DisplayType { get; set; }
        public BasicRectangle BoundsBitmap { get; set; }
        public BasicRectangle BoundsWaveForm { get; set; }
        public bool IsScrollBar { get; set; }
        public float Zoom { get; set; }
        public int StartTile { get; set; }
        public int EndTile { get; set; }
        public int TileSize { get; set; }

        public WaveFormBitmapRequest()
        {
            BoundsBitmap = new BasicRectangle();
            BoundsWaveForm = new BasicRectangle();
            Zoom = 1;
        }
    }
}