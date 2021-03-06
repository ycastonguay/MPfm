// Copyright © 2011-2013 Yanick Castonguay
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

using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Basics;
using System.Collections.Generic;
using Sessions.iOS.Classes.Controls.Helpers;

namespace Sessions.iOS.Classes.Controls.Graphics
{
	public class GraphicsContextWrapper : IGraphicsContext
	{
		protected CGContext Context;

		public GraphicsContextWrapper(CGContext context, float boundsWidth, float boundsHeight, BasicRectangle dirtyRect)
		{
			Context = context;
			BoundsWidth = boundsWidth;
			BoundsHeight = boundsHeight;
			DirtyRect = dirtyRect;
		}

		public BasicRectangle DirtyRect { get; private set; }
		public float BoundsWidth { get; private set; }
		public float BoundsHeight { get; private set; }
		public float Density { get { return 1; } } // Always 1 on iOS because the Retina displays actually use fractions

		public void DrawImage(BasicRectangle rectangle, IDisposable image)
		{
			Context.DrawImage(GenericControlHelper.ToRect(rectangle), ((UIImage)image).CGImage);
		}

		public void DrawImage(BasicRectangle rectangleDestination, BasicRectangle rectangleSource, IDisposable image)
		{
			DrawImage(rectangleDestination, image);
		}

		public void DrawEllipsis(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
		{
			CoreGraphicsHelper.DrawEllipsis(Context, GenericControlHelper.ToRect(rectangle), GenericControlHelper.ToColor(pen.Brush.Color).CGColor, pen.Thickness);
			// TODO: Add fill
		}

		public void DrawRectangle(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
		{
			CoreGraphicsHelper.FillRect(Context, GenericControlHelper.ToRect(rectangle), GenericControlHelper.ToColor(brush.Color).CGColor);
			// TODO: Add outline
		}

		public void DrawLine(BasicPoint point, BasicPoint point2, BasicPen pen)
		{
			CoreGraphicsHelper.DrawLine(Context, new List<PointF>(){ GenericControlHelper.ToPoint(point), GenericControlHelper.ToPoint(point2) }, GenericControlHelper.ToColor(pen.Brush.Color).CGColor, pen.Thickness, true, false);
		}

		public void DrawPath(BasicPath path, BasicBrush brush, BasicPen pen)
		{
			throw new NotImplementedException();
		}

		public void DrawText(string text, BasicPoint point, BasicColor color, string fontFace, float fontSize)
		{
			CoreGraphicsHelper.DrawTextAtPoint(Context, GenericControlHelper.ToPoint(point), text, fontFace, fontSize, GenericControlHelper.ToColor(color).CGColor);
		}

		public void DrawText(string text, BasicRectangle rectangle, BasicColor color, string fontFace, float fontSize)
		{
			CoreGraphicsHelper.DrawTextInRect(Context, GenericControlHelper.ToRect(rectangle), text, fontFace, fontSize, GenericControlHelper.ToColor(color).CGColor, UILineBreakMode.TailTruncation, UITextAlignment.Left);
			// TODO: Add text alignment to IGraphicsContext
		}

		public BasicRectangle MeasureText(string text, BasicRectangle rectangle, string fontFace, float fontSize)
		{
			var size = CoreGraphicsHelper.MeasureText(Context, text, fontFace, fontSize);
			return new BasicRectangle(0, 0, size.Width, size.Height);
		}

		public void SetPen(BasicPen pen)
		{
			Context.SetStrokeColor(GenericControlHelper.ToColor(pen.Brush.Color).CGColor);
			Context.SetLineWidth(pen.Thickness);
		}

		public void StrokeLine(BasicPoint point, BasicPoint point2)
		{
			Context.StrokeLineSegments(new PointF[2] { GenericControlHelper.ToPoint(point), GenericControlHelper.ToPoint(point2) });
		}

		public void SaveState()
		{
			Context.SaveState();
		}

		public void RestoreState()
		{
			Context.RestoreState();
		}

		public void Close()
		{
			// Not used on iOS
		}
    }
}
