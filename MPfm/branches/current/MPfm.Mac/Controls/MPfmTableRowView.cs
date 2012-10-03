//
// MPfmTableRowView.cs: Custom table row view for view-based NSTableView.
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
    /// Custom table row view for view-based NSTableView.
    /// </summary>
    [Register("MPfmTableRowView")]
    public class MPfmTableRowView : NSTableRowView
    {
        private bool isMouseDown = false;

        public CGColor GradientColor1 { get; set; }
        public CGColor GradientColor2 { get; set; }

        [Export("init")]
        public MPfmTableRowView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmTableRowView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.BackgroundColor = NSColor.Blue;
            this.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.None;          
            GradientColor1 = new CGColor(1.0f, 1.0f, 1.0f);
            GradientColor2 = new CGColor(0.8f, 0.8f, 0.8f);
        }

//        [Export("mouseDown:")]
//        public override void MouseDown(NSEvent theEvent)
//        {
//            // Set flag
//            isMouseDown = true;
//
//            base.MouseDown(theEvent);
//
//            // Call mouse up 
//            this.MouseUp(theEvent);
//        }
//
//        [Export("mouseUp:")]
//        public override void MouseUp(NSEvent theEvent)
//        {
//            // Call super class
//            base.MouseUp(theEvent);
//
//            // Set flag
//            isMouseDown = false;
//        }        

        //[Export ("drawBackgroundInRect:")]
        public override void DrawBackgrounn(RectangleF dirtyRect)
        {
            base.DrawBackgrounn(dirtyRect); // WTF Backgrounn?

//            NSGradient gradient = new NSGradient(new NSColor[]{
//                NSColor.White, NSColor.Black
//            });
//            gradient.DrawInRect(dirtyRect, 90);

            // Draw background
            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
            RectangleF rectBackground = new RectangleF(0, 0, dirtyRect.Width, dirtyRect.Height);
            //CocoaHelper.FillRect(context, rectBackground, new CGColor(1.0f, 0, 0, 1.0f));
            CocoaHelper.FillGradient(context, dirtyRect, GradientColor1, GradientColor2);

        }



//
//
//        public override void DrawRect(System.Drawing.RectangleF dirtyRect)
//        {
//            base.DrawRect(dirtyRect);
//
//            NSGradient gradient = new NSGradient(new NSColor[]{
//                NSColor.White, NSColor.Black
//            });
//            gradient.DrawInRect(dirtyRect, 90);
////            // Draw background
////            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
////            RectangleF rectBackground = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
////            //CocoaHelper.FillRect(context, rectBackground, new CGColor(1.0f, 0, 0, 1.0f));
////            CocoaHelper.DrawGradient(context, rectBackground, GradientColor1, GradientColor2);
//        }
    }
}
