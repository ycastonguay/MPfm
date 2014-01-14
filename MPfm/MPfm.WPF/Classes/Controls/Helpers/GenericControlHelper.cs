﻿// Copyright © 2011-2013 Yanick Castonguay
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

using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MPfm.GenericControls.Basics;

namespace MPfm.WPF.Classes.Controls.Helpers
{
    public static class GenericControlHelper
    {
        public static Rect ToRect(BasicRectangle rectangle)
        {
            return new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Point ToPoint(BasicPoint point)
        {
            return new Point(point.X, point.Y);
        }

        public static Color ToColor(BasicColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static SolidColorBrush ToSolidColorBrush(BasicBrush brush)
        {
            return new SolidColorBrush(ToColor(brush.Color));
        }

        public static LinearGradientBrush ToLinearGradientBrush(BasicGradientBrush brush)
        {
            if(brush.StartPoint.X == 0 && brush.StartPoint.Y == 0 &&
               brush.EndPoint.X == 0 && brush.EndPoint.Y == 0)
                return new LinearGradientBrush(ToColor(brush.Color), ToColor(brush.Color2), brush.Angle);

            return new LinearGradientBrush(ToColor(brush.Color), ToColor(brush.Color2), ToPoint(brush.StartPoint), ToPoint(brush.EndPoint));
        }

        public static Pen ToPen(BasicPen pen)
        {
            return new Pen(ToSolidColorBrush(pen.Brush), pen.Thickness);
        }
    }
}
