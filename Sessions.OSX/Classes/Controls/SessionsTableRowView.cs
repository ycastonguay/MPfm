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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Sessions.OSX.Classes.Helpers;

namespace Sessions.OSX.Classes.Controls
{
    /// <summary>
    /// Custom table row view for view-based NSTableView.
    /// </summary>
    [Register("SessionsTableRowView")]
    public class SessionsTableRowView : NSTableRowView
    {
        private bool isMouseDown = false;

        public CGColor GradientColor1 { get; set; }
        public CGColor GradientColor2 { get; set; }

        public bool IsBorderVisible { get; set; }

        [Export("init")]
        public SessionsTableRowView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsTableRowView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.BackgroundColor = NSColor.Blue;
            //this.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.None;          
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

        public override void DrawBackground(RectangleF dirtyRect)
        {
            //base.DrawBackground(dirtyRect);
            //Console.WriteLine("DrawBackground");

            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
            context.SaveState();
            CoreGraphicsHelper.FillRect(context, dirtyRect, new CGColor(1, 1, 1));

            if(IsBorderVisible)
                CoreGraphicsHelper.DrawLine(context, new PointF[2]{ new PointF(0, Bounds.Height), new PointF(Bounds.Width, Bounds.Height) }, 1, new CGColor(0.7f, 0.7f, 0.7f));

            context.RestoreState();
        }

        public override void DrawSelection(RectangleF dirtyRect)
        {
            //base.DrawSelection(dirtyRect);
            //Console.WriteLine("DrawSelection");
            if (SelectionHighlightStyle == NSTableViewSelectionHighlightStyle.None)
                return;

            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
            context.SaveState();
            CoreGraphicsHelper.FillRect(context, dirtyRect, new CGColor(1, 0, 0));
            context.RestoreState();
        }
    }
}
