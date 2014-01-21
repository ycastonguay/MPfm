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
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Basics;
using System.Collections.Generic;
using MPfm.iOS.Classes.Controls.Helpers;

namespace MPfm.iOS.Classes.Controls.Graphics
{
	public class GraphicsContextWrapper : IGraphicsContext
	{
		protected CGContext Context;

		public GraphicsContextWrapper(CGContext context, float boundsWidth, float boundsHeight)
		{
			Context = context;
			BoundsWidth = boundsWidth;
			BoundsHeight = boundsHeight;
		}

		public float BoundsWidth { get; private set; }
		public float BoundsHeight { get; private set; }

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
    }
}
