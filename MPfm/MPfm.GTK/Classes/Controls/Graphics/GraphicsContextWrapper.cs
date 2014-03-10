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
using Gdk;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Graphics;
using Cairo;
using MPfm.GTK.Helpers;

namespace MPfm.GTK.Classes.Controls.Graphics
{
    public class GraphicsContextWrapper : IGraphicsContext
    {
        private Context _context;

        public float BoundsWidth { get; private set; }
        public float BoundsHeight { get; private set; }
		public float Density { get { return 1; } }

        public GraphicsContextWrapper(Context context, float boundsWidth, float boundsHeight)
        {
            _context = context;
            BoundsWidth = boundsWidth;
            BoundsHeight = boundsHeight;
        }

		public void DrawImage(BasicRectangle rectangle, IDisposable image)
		{
		}

		public void DrawImage(BasicRectangle rectangleDestination, BasicRectangle rectangleSource, IDisposable image)
		{
		}

        public void DrawEllipsis(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
        {            
        }

        public void DrawRectangle(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
        {
            _context.SetSourceRGB(brush.Color.R / 255f, brush.Color.G / 255f, brush.Color.B / 255f);
            _context.Rectangle(GenericControlHelper.ToRect(rectangle));
            _context.Fill();
        }

        public void DrawLine(BasicPoint point, BasicPoint point2, BasicPen pen)
        {
            _context.SetSourceRGB(pen.Brush.Color.R / 255f, pen.Brush.Color.G / 255f, pen.Brush.Color.B / 255f);
            _context.LineTo(point.X, point.Y);
            _context.LineTo(point2.X, point2.Y);
            _context.LineWidth = pen.Thickness;
            _context.Stroke();
        }

        public void DrawText(string text, BasicPoint point, BasicColor color, string fontFace, float fontSize)
        {
            _context.SetSourceRGB(color.R / 255f, color.G / 255f, color.B / 255f);
            _context.SelectFontFace(fontFace, FontSlant.Normal, FontWeight.Normal);
            _context.SetFontSize(fontSize);
            _context.MoveTo(point.X, point.Y);
            _context.ShowText(text);
        }

        public void DrawText(string text, BasicRectangle rectangle, BasicColor color, string fontFace, float fontSize)
        {
        }

        public BasicRectangle MeasureText(string text, BasicRectangle rectangle, string fontFace, float fontSize)
        {
            return new BasicRectangle(0, 0, 30, 20);
        }

		public void SetPen(BasicPen pen)
		{
		}

		public void StrokeLine(BasicPoint point, BasicPoint point2)
		{
		}

		public void SaveState()
		{
		}

		public void RestoreState()
		{
		}	

		public void Close()
		{
		}
    }
}
