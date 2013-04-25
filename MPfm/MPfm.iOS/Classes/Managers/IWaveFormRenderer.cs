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

namespace MPfm.iOS.Managers
{
    public interface IWaveFormRenderer
    {
        void StartRender(RendererSizeF size);
        void EndRender();

        void RenderLine(RendererPointF pt1, RendererPointF pt2, float lineWidth, RendererColor color);
    }

    public class RendererColor
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }

    public class RendererPointF
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class RendererSizeF
    {
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
