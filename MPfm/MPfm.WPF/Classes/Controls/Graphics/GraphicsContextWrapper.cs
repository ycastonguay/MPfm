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
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Graphics;
using MPfm.WPF.Classes.Controls.Helpers;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;

namespace MPfm.WPF.Classes.Controls.Graphics
{
    public class GraphicsContextWrapper : IGraphicsContext
    {
        private readonly DrawingContext _context;
        private Pen _currentPen;

        public GraphicsContextWrapper(DrawingContext context, float boundsWidth, float boundsHeight)
        {
            _context = context;
            BoundsWidth = boundsWidth;
            BoundsHeight = boundsHeight;
            using (var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
                Density = g.DpiX/96f;
        }

        public float BoundsWidth { get; private set; }
        public float BoundsHeight { get; private set; }
        public float Density { get; private set; }

        public void SetPen(BasicPen pen)
        {
            _currentPen = GenericControlHelper.ToPen(pen);
        }

        public void StrokeLine(BasicPoint point, BasicPoint point2)
        {
            //TryToCreatePen();
            _context.DrawLine(_currentPen, GenericControlHelper.ToPoint(point), GenericControlHelper.ToPoint(point2));
        }

        public void SaveState()
        {
            // Does not exist
        }

        public void RestoreState()
        {
            // Does not exist
        }

        public void DrawImage(BasicRectangle rectangleDestination, BasicRectangle rectangleSource, IDisposable image)
        {
            DrawImage(rectangleSource, image);
        }

        public void DrawImage(BasicRectangle rectangle, IDisposable image)
        {
            var disposableImage = (DisposableBitmap) image;
            _context.DrawImage(disposableImage.Bitmap, GenericControlHelper.ToRect(rectangle));
        }

        public void DrawEllipsis(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
        {
            _context.DrawEllipse(GenericControlHelper.ToSolidColorBrush(brush), GenericControlHelper.ToPen(pen), GenericControlHelper.ToPoint(rectangle.Center()), rectangle.Width / 2, rectangle.Height / 2);
        }

        public void DrawRectangle(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
        {
            if(brush is BasicGradientBrush)
                _context.DrawRectangle(GenericControlHelper.ToLinearGradientBrush((BasicGradientBrush)brush), GenericControlHelper.ToPen(pen), GenericControlHelper.ToRect(rectangle));

            _context.DrawRectangle(GenericControlHelper.ToSolidColorBrush(brush), GenericControlHelper.ToPen(pen), GenericControlHelper.ToRect(rectangle));
        }

        public void DrawLine(BasicPoint point, BasicPoint point2, BasicPen pen)
        {
            _context.DrawLine(GenericControlHelper.ToPen(pen), GenericControlHelper.ToPoint(point), GenericControlHelper.ToPoint(point2));
        }

        public void DrawText(string text, BasicPoint point, BasicColor color, string fontFace, float fontSize)
        {
            var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(fontFace), fontSize, new SolidColorBrush(GenericControlHelper.ToColor(color)));
            _context.DrawText(formattedText, GenericControlHelper.ToPoint(point));
        }

        public void DrawText(string text, BasicRectangle rectangle, BasicColor color, string fontFace, float fontSize)
        {
            throw new System.NotImplementedException();
        }

        public BasicRectangle MeasureText(string text, BasicRectangle rectangle, string fontFace, float fontSize)
        {
            var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(fontFace), fontSize, Brushes.Black);
            return new BasicRectangle(0, 0, (float) formattedText.Width, (float) formattedText.Height);
        }

        public void Close()
        {
            _context.Close();
        }
    }
}
