//
// MPfmAlbumCoverView.cs: View for displaying album covers in a view-based NSTableView.
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
    /// View for displaying album covers in a view-based NSTableView.
    /// </summary>
    [Register("MPfmAlbumCoverView")]
    public class MPfmAlbumCoverView : NSView
    {
        private bool isMouseDown = false;

        public CGColor GradientColor1 { get; set; }
        public CGColor GradientColor2 { get; set; }

        [Export("init")]
        public MPfmAlbumCoverView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmAlbumCoverView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            GradientColor1 = new CGColor(0.1f, 0.1f, 0.1f);
            GradientColor2 = new CGColor(0.3f, 0.3f, 0.3f);

            this.FocusRingType = NSFocusRingType.None;
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
            
            CGGradient gradientBackground;
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            
            float[] locationListBackground = new float[] { 1.0f, 0.0f };
            List<float> colorListBackground = new List<float>();
            colorListBackground.AddRange(GradientColor1.Components);
            colorListBackground.AddRange(GradientColor2.Components);
            gradientBackground = new CGGradient(colorSpace, colorListBackground.ToArray(), locationListBackground);
            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
            
            RectangleF rectBackground = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
            context.SaveState();
            context.AddRect(rectBackground);
            context.Clip();
            context.DrawLinearGradient(gradientBackground, new PointF(0, 0), new PointF(0, Bounds.Height), CGGradientDrawingOptions.DrawsBeforeStartLocation);
            context.RestoreState();

//            context.SaveState();
//            context.SetStrokeColor(new CGColor(0.35f, 1.0f));
//            context.StrokeRect(Get1pxRect(Bounds));
//            context.RestoreState();
            
        }
        
        RectangleF Get1pxRect(RectangleF rect)
        {
            RectangleF newRect = new RectangleF(rect.X + 0.5f, rect.Y + 0.5f, rect.Width - 1, rect.Height - 1);
            return newRect;
        }
    }
}
