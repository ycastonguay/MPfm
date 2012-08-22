//
// MPfmView.cs: Custom view based on NSView. 
//
// Copyright © 2011-2012 Yanick Castonguay
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

        public CGColor GradientColor1 { get; set; }
        public CGColor GradientColor2 { get; set; }

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

        private void Initialize()
        {
            GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f);
            GradientColor2 = new CGColor(0.4f, 0.4f, 0.4f);
        }

        [Export("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            // Set flag
            isMouseDown = true;

            base.MouseDown(theEvent);

            // Call mouse up 
            this.MouseUp(theEvent);
        }

        [Export("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            // Call super class
            base.MouseUp(theEvent);

            // Set flag
            isMouseDown = false;
        }

        public override void DrawRect(System.Drawing.RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);

            CGGradient gradient;
            CGColorSpace colorSpace;
            PointF ptStart;
            PointF ptEnd;

            float[] locationList = new float[] { 1.0f, 0.0f };
            List<float> colorList = new List<float>();
            colorList.AddRange(GradientColor1.Components);
            colorList.AddRange(GradientColor2.Components);

            colorSpace = CGColorSpace.CreateDeviceRGB();
            gradient = new CGGradient(colorSpace, colorList.ToArray(), locationList);
            ptStart = new PointF(0, 0);
            ptEnd = new PointF(0, Bounds.Height);

            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
            context.SaveState();
            context.AddRect(dirtyRect);
            context.Clip();
            context.DrawLinearGradient(gradient, ptStart, ptEnd, CGGradientDrawingOptions.DrawsBeforeStartLocation);
            context.RestoreState();
        }
    }
}
