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

using Android.Graphics;
using MPfm.GenericControls.Basics;

namespace MPfm.Android.Classes.Controls.Helpers
{
    public static class GenericControlHelper
    {
        public static Rect ToRect(BasicRectangle rectangle)
        {
            return new Rect((int) rectangle.X, (int) rectangle.Y, (int) (rectangle.X + rectangle.Width), (int) (rectangle.Y + rectangle.Height));
        }

        public static RectF ToRectF(BasicRectangle rectangle)
        {
            return new RectF(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
        }

        public static Point ToPoint(BasicPoint point)
        {
            return new Point((int) point.X, (int) point.Y);
        }

        public static PointF ToPointF(BasicPoint point)
        {
            return new PointF(point.X, point.Y);
        }

        public static Color ToColor(BasicColor color)
        {
            return Color.Argb(color.A, color.R, color.G, color.B);
        }

        // No brushes/pens on Android
    }
}