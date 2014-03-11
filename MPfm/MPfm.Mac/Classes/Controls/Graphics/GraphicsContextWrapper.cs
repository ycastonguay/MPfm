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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.MVP;
using MPfm.Mac.Classes.Objects;
using MPfm.Mac.Classes.Helpers;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Basics;
using MPfm.Mac.Classes.Controls.Helpers;

namespace MPfm.Mac.Classes.Controls.Graphics
{
    public class GraphicsContextWrapper : NSObject, IGraphicsContext
    {
        protected CGContext Context;
        private float _density;

        public GraphicsContextWrapper(CGContext context, float boundsWidth, float boundsHeight)
        {
            Context = context;
            BoundsWidth = boundsWidth;
            BoundsHeight = boundsHeight;
            _density = GetDisplayScale();
        }
        
        private float GetDisplayScale()
        {
            float scale = 1;
            foreach (var screen in NSScreen.Screens)
                if(screen.BackingScaleFactor > scale)
                    scale = screen.BackingScaleFactor;
            return scale;
        }

        public float BoundsWidth { get; private set; }
        public float BoundsHeight { get; private set; }
        public float Density { get { return _density; } }

        public void DrawImage(BasicRectangle rectangle, IDisposable image)
        {
            //Console.WriteLine("GraphicsContextWrapper - DrawImage - rectangle: {0}", rectangle);
            DrawImage(rectangle, rectangle, image);
        }

        public void DrawImage(BasicRectangle rectangleDestination, BasicRectangle rectangleSource, IDisposable image)
        {
            //Console.WriteLine("GraphicsContextWrapper - DrawImage - rectangleDestination: {0} rectangleSource: {1}", rectangleDestination, rectangleSource);
            var bitmap = image as NSImage;
            if (bitmap == null)
                return;

            bitmap.Draw(GenericControlHelper.ToRect(rectangleDestination), GenericControlHelper.ToRect(rectangleSource), NSCompositingOperation.SourceOver, 1, true, new NSDictionary());
        }

        public void DrawEllipsis(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
        {
            CoreGraphicsHelper.FillEllipsis(Context, GenericControlHelper.ToRect(rectangle), GenericControlHelper.ToCGColor(brush.Color));
            // TODO: Add outline
        }

        public void DrawRectangle(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
        {
            CoreGraphicsHelper.FillRect(Context, GenericControlHelper.ToRect(rectangle), GenericControlHelper.ToCGColor(brush.Color));
            // TODO: Add outline
        }

        public void DrawLine(BasicPoint point, BasicPoint point2, BasicPen pen)
        {
            CoreGraphicsHelper.DrawLine(Context, new List<PointF>(){ GenericControlHelper.ToPoint(point), GenericControlHelper.ToPoint(point2) }, GenericControlHelper.ToCGColor(pen.Brush.Color), pen.Thickness, true, false);
        }

        public void DrawText(string text, BasicPoint point, BasicColor color, string fontFace, float fontSize)
        {
            // Very ugly fix for Roboto which is rendered too low on OS X
            var newPt = new BasicPoint(point.X, point.Y);
            if(fontFace.ToUpper().Contains("ROBOTO"))
                newPt.Y -= 2;
            CoreGraphicsHelper.DrawTextAtPoint(Context, GenericControlHelper.ToPoint(newPt), text, fontFace, fontSize, GenericControlHelper.ToNSColor(color));
        }

        public void DrawText(string text, BasicRectangle rectangle, BasicColor color, string fontFace, float fontSize)
        {
            // Very ugly fix for Roboto which is rendered too low on OS X
            var newRect = new BasicRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            if(fontFace.ToUpper().Contains("ROBOTO"))
                newRect.Y -= 2;
            CoreGraphicsHelper.DrawTextInRect(Context, GenericControlHelper.ToRect(newRect), text, fontFace, fontSize, GenericControlHelper.ToNSColor(color));
        }

        public BasicRectangle MeasureText(string text, BasicRectangle rectangle, string fontFace, float fontSize)
        {
            var size = CoreGraphicsHelper.MeasureText(Context, text, fontFace, fontSize);
            return new BasicRectangle(0, 0, size.Width, size.Height);
        }

        public void SetPen(BasicPen pen)
        {
            Context.SetStrokeColor(GenericControlHelper.ToCGColor(pen.Brush.Color));
            Context.SetLineWidth(pen.Thickness);
        }

        public void StrokeLine(BasicPoint point, BasicPoint point2)
        {
            //SetViewportOrigin(false);
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
            // Not used on OS X
        }
    }
}
