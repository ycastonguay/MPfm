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

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Interaction;
using MPfm.WindowsControls;

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
            var solidColorBrush = new SolidColorBrush(ToColor(brush.Color));
            solidColorBrush.Freeze();
            return solidColorBrush;
        }

        public static LinearGradientBrush ToLinearGradientBrush(BasicGradientBrush brush)
        {
            LinearGradientBrush linearGradientBrush = null;
            if (brush.StartPoint.X == 0 && brush.StartPoint.Y == 0 &&
                brush.EndPoint.X == 0 && brush.EndPoint.Y == 0)
                linearGradientBrush = new LinearGradientBrush(ToColor(brush.Color), ToColor(brush.Color2), brush.Angle);
            else
                linearGradientBrush = new LinearGradientBrush(ToColor(brush.Color), ToColor(brush.Color2), ToPoint(brush.StartPoint), ToPoint(brush.EndPoint));
            linearGradientBrush.Freeze();
            return linearGradientBrush;
        }

        public static Pen ToPen(BasicPen pen)
        {
            var basicPen = new Pen(ToSolidColorBrush(pen.Brush), pen.Thickness);
            basicPen.Freeze();
            return basicPen;
        }

        public static void MouseDown(MouseButtonEventArgs e, UIElement element, IControlMouseInteraction control)
        {
            var location = e.GetPosition(element);
            float x = (float)location.X;
            float y = (float)location.Y;
            element.CaptureMouse();
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    control.MouseDown(x, y, MouseButtonType.Left);
                    break;
                case MouseButton.Middle:
                    control.MouseDown(x, y, MouseButtonType.Middle);
                    break;
                case MouseButton.Right:
                    control.MouseDown(x, y, MouseButtonType.Right);
                    break;
            }
        }

        public static void MouseUp(MouseButtonEventArgs e, UIElement element, IControlMouseInteraction control)
        {
            var location = e.GetPosition(element);
            float x = (float)location.X;
            float y = (float)location.Y;
            element.ReleaseMouseCapture();
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    control.MouseUp(x, y, MouseButtonType.Left);
                    break;
                case MouseButton.Middle:
                    control.MouseUp(x, y, MouseButtonType.Middle);
                    break;
                case MouseButton.Right:
                    control.MouseUp(x, y, MouseButtonType.Right);
                    break;
            }
        }

        public static void MouseMove(MouseEventArgs e, UIElement element, IControlMouseInteraction control)
        {
            var location = e.GetPosition(element);
            float x = (float)location.X;
            float y = (float)location.Y;
            if (e.LeftButton == MouseButtonState.Pressed)
                control.MouseMove(x, y, MouseButtonType.Left);
            else if (e.MiddleButton == MouseButtonState.Pressed)
                control.MouseMove(x, y, MouseButtonType.Middle);
            else if (e.RightButton == MouseButtonState.Pressed)
                control.MouseMove(x, y, MouseButtonType.Right);
        }

        public static void MouseDoubleClick(MouseEventArgs e, UIElement element, IControlMouseInteraction control)
        {
            var location = e.GetPosition(element);
            float x = (float)location.X;
            float y = (float)location.Y;
            if (e.LeftButton == MouseButtonState.Pressed)
                control.MouseMove(x, y, MouseButtonType.Left);
            else if (e.MiddleButton == MouseButtonState.Pressed)
                control.MouseMove(x, y, MouseButtonType.Middle);
            else if (e.RightButton == MouseButtonState.Pressed)
                control.MouseMove(x, y, MouseButtonType.Right);
        }
    }
}
