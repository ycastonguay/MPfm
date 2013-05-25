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

namespace MPfm.Mac
{
    /// <summary>
    /// Custom view based on NSView.
    /// </summary>
    [Register("MPfmView")]
    public class MPfmView : NSView
    {
        private bool isMouseDown = false;

        public bool IsHeaderVisible { get; set; }
        public CGColor GradientColor1 { get; set; }
        public CGColor GradientColor2 { get; set; }
        public CGColor HeaderGradientColor1 { get; set; }
        public CGColor HeaderGradientColor2 { get; set; }
        public CGColor BorderColor { get; set; }

        [Export("init")]
        public MPfmView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        public MPfmView(RectangleF frameRect) : base(frameRect)
        {
            Initialize();
        }

        public MPfmView(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public MPfmView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        private void Initialize()
        {
            GradientColor1 = new CGColor(0.1490f, 0.1843f, 0.2118f, 1);
            GradientColor2 = new CGColor(0.1490f, 0.1843f, 0.2118f, 1);
            //GradientColor2 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
            //HeaderGradientColor1 = new CGColor(0.7529f, 0.2235f, 0.1686f, 1);
            HeaderGradientColor1 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
            HeaderGradientColor2 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
            BorderColor = new CGColor(0.1490f, 0.1843f, 0.2118f, 1);
      }

        [Export("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            isMouseDown = true;
            base.MouseDown(theEvent);
            this.MouseUp(theEvent);
        }

        [Export("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            isMouseDown = false;
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);

            CGGradient gradientBackground;
            CGGradient gradientHeader;
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();

            float[] locationListBackground = new float[] { 1.0f, 0.0f };
            List<float> colorListBackground = new List<float>();
            colorListBackground.AddRange(GradientColor1.Components);
            colorListBackground.AddRange(GradientColor2.Components);
            float[] locationListHeader = new float[] { 1.0f, 0.0f };
            List<float> colorListHeader = new List<float>();
            colorListHeader.AddRange(HeaderGradientColor1.Components);
            colorListHeader.AddRange(HeaderGradientColor2.Components);
            gradientBackground = new CGGradient(colorSpace, colorListBackground.ToArray(), locationListBackground);
            gradientHeader = new CGGradient(colorSpace, colorListHeader.ToArray(), locationListHeader);
            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;

            RectangleF rectBackground = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
            context.SaveState();
            context.AddRect(rectBackground);
            context.Clip();
            context.DrawLinearGradient(gradientBackground, new PointF(0, 0), new PointF(0, Bounds.Height), CGGradientDrawingOptions.DrawsBeforeStartLocation);
            context.RestoreState();

            if (IsHeaderVisible)
            {
                RectangleF rectHeader = new RectangleF(0, Bounds.Height - 24, Bounds.Width, 24);
                context.SaveState();
                context.AddRect(rectHeader);
                context.Clip();
                context.DrawLinearGradient(gradientHeader, new PointF(0, Bounds.Height - 24), new PointF(0, Bounds.Height), CGGradientDrawingOptions.DrawsBeforeStartLocation);
                context.RestoreState();
            }

            context.SaveState();
            context.SetStrokeColor(BorderColor);
            context.StrokeRect(Get1pxRect(Bounds));
            context.RestoreState();
        }

        RectangleF Get1pxRect(RectangleF rect)
        {
            RectangleF newRect = new RectangleF(rect.X + 0.5f, rect.Y + 0.5f, rect.Width - 1, rect.Height - 1);
            return newRect;
        }
    }
}
