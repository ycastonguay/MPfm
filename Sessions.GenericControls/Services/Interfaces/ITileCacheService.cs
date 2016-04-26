// Copyright � 2011-2013 Yanick Castonguay
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

using System.Collections.Generic;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Services.Objects;
using Sessions.Sound.AudioFiles;

namespace Sessions.GenericControls.Services.Interfaces
{
    public interface ITileCacheService
    {
        int Count { get; }

        void Flush();
        void AddTile(WaveFormTile tile, bool isScrollBar);
        WaveFormTile GetTile(float contentOffsetX, float zoom, bool isScrollBar);

        List<WaveFormTile> GetTilesForPosition(float x, float zoom);
        List<WaveFormTile> GetTilesToFillBounds(float zoom, BasicRectangle bounds);
    }
}