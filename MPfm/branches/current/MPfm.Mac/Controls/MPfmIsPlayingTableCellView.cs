//
// MPfmIsPlayingTableCellView.cs: Custom table cell view for view-based NSTableView.
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
    /// Custom table cell view for view-based NSTableView. Displays an animation to indicate that the song is currently playing.
    /// </summary>
    [Register("MPfmIsPlayingTableCellView")]
    public class MPfmIsPlayingTableCellView : NSTableCellView
    {
        bool isPlaying = false;
        bool isMouseDown = false;

        public CGColor GradientColor1 { get; set; }
        public CGColor GradientColor2 { get; set; }

        [Export("init")]
        public MPfmIsPlayingTableCellView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmIsPlayingTableCellView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        void Initialize()
        {
            GradientColor1 = new CGColor(1.0f, 1.0f, 1.0f);
            GradientColor2 = new CGColor(0.8f, 0.8f, 0.8f);
        }

        public void SetIsPlaying(bool isPlaying)
        {
            this.isPlaying = isPlaying;
            SetNeedsDisplayInRect(Bounds);
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

            float padding = 6;

            if(!isPlaying)
                return;

            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;

            float size = (Bounds.Width > Bounds.Height) ? Bounds.Height : Bounds.Width;
            RectangleF rect = new RectangleF(padding / 2, padding / 2, size - padding, size - padding);

            CGPath path = new CGPath();
            path.MoveToPoint(new PointF(0, 0));
            float outerRadius = 16;
            float innerRadius = 4;
            //path.AddArc(rect.Width / 2, rect.Height / 2, 45, 0*3.142f/180, angle*3.142f/180, false);
            path.AddArc(outerRadius / 2, outerRadius / 2, outerRadius / 2, 0, 360, false);
            path.CloseSubpath();

//            path.AddArc(rect.Width / 4, rect.Height / 4, 45, 0*3.142f/180, angle*3.142f/180, false);
//            path.CloseSubpath();           

            CocoaHelper.EOFillPath(context, path, new CGColor(0.0f, 0.0f, 0.0f));

            //CocoaHelper.FillEllipsis(context, rect, new CGColor(0.2f, 0.75f, 0.2f));
            //CocoaHelper.DrawEllipsis(context, rect, new CGColor(0.2f, 0.8f, 0.2f), 2);
            //rect.Inflate(new SizeF(1.0f, 1.0f));
            //CocoaHelper.DrawEllipsis(context, rect, new CGColor(0.2f, 0.95f, 0.2f), 0.5f);
        }
    }
}
