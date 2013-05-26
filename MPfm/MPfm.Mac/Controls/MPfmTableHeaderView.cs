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
    [Register("MPfmTableHeaderView")]
    public class MPfmTableHeaderView : NSTableHeaderView
    {
        bool _isMouseDown = false;
        bool _isMouseOver = false;

        public CGColor TextColor { get; set; }
        public CGColor BackgroundColor { get; set; }
        public CGColor BackgroundMouseDownColor { get; set; }
        public CGColor BackgroundMouseOverColor { get; set; }
        public CGColor BorderColor { get; set; }

        [Export("init")]
        public MPfmTableHeaderView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmTableHeaderView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            //BackgroundColor = new CGColor(60f/255f, 76f/255f, 88f/255f, 1);
            BackgroundColor = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            //BackgroundColor = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
            BackgroundMouseDownColor = new CGColor(80f/255f, 100f/255f, 114f/255f, 1);
            BackgroundMouseOverColor = new CGColor(130f/255f, 158f/255f, 177f/255f, 1);
            BorderColor =  new CGColor(0.1490f, 0.1843f, 0.2118f, 1);//new CGColor(83f/255f, 104f/255f, 119f/255f, 1);

            //            BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
            //            BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
            //            BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);

            TextColor = new CGColor(1, 1, 1, 1);

            // This allows MouseEntered and MouseExit to work
            AddTrackingRect(Bounds, this, IntPtr.Zero, false);
        }
                
        [Export("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            _isMouseDown = true;
            base.MouseDown(theEvent);
            this.MouseUp(theEvent);
        }

        [Export("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            _isMouseDown = false;
            SetNeedsDisplayInRect(Bounds);
        }

        [Export("mouseEntered:")]
        public override void MouseEntered(NSEvent theEvent)
        {
            base.MouseEntered(theEvent);
            _isMouseOver = true;
            SetNeedsDisplayInRect(Bounds);
        }

        [Export("mouseExited:")]
        public override void MouseExited(NSEvent theEvent)
        {
            base.MouseExited(theEvent);
            _isMouseOver = false;
            SetNeedsDisplayInRect(Bounds);
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;

            if (_isMouseDown)
                CocoaHelper.FillRect(context, Bounds, BackgroundMouseDownColor);
//            else if (_isMouseOver)
//                CocoaHelper.FillRect(context, Bounds, BackgroundMouseOverColor);
            else
                CocoaHelper.FillRect(context, Bounds, BackgroundColor);

            float x = 0;
            foreach (NSTableColumn column in TableView.TableColumns())
            {
                //Console.WriteLine("Column title: {0} width: {1} x: {2}", column.HeaderCell.Title, column.Width, x);
                //CocoaHelper.DrawRect(context, new RectangleF(x, 0, column.Width + 3, Bounds.Height), BorderColor);
                CocoaHelper.DrawText(new RectangleF(0, 0, column.Width, Bounds.Height), x + 2, 2, column.HeaderCell.Title, "Junction", 10, NSColor.FromDeviceRgba(0.9f, 0.9f, 0.9f, 1));
                x += column.Width;// + 3;
            }
        }
    }
}
