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
using System.Reflection;
using Cairo;
using Sessions.GenericControls.Basics;

namespace MPfm.GTK.Helpers
{
    public static class GenericControlHelper
    {
        public static Rectangle ToRect(BasicRectangle rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public static Point ToPoint(BasicPoint point)
        {
            return new Point((int)point.X, (int)point.Y);
        }

        public static Color ToColor(BasicColor color)
        {
            return new Color(color.R, color.G, color.B);
        }
    }
}
