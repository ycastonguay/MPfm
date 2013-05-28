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
using MPfm.Mac.Classes.Helpers;

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmPopUpButton")]
    public class MPfmPopUpButton : NSPopUpButton
    {
        bool _isMouseDown = false;
        bool _isMouseOver = false;

        public CGColor TextColor { get; set; }
        public CGColor BackgroundColor { get; set; }
        public CGColor BackgroundMouseDownColor { get; set; }
        public CGColor BackgroundMouseOverColor { get; set; }
        public CGColor BorderColor { get; set; }

        [Export("init")]
        public MPfmPopUpButton() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmPopUpButton(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            //BackgroundColor = new CGColor(60f/255f, 76f/255f, 88f/255f, 1);
            BackgroundColor = new CGColor(97f/255f, 122f/255f, 140f/255f, 1);
            BackgroundMouseDownColor = new CGColor(80f/255f, 100f/255f, 114f/255f, 1);
            BackgroundMouseOverColor = new CGColor(130f/255f, 158f/255f, 177f/255f, 1);
            BorderColor = new CGColor(83f/255f, 104f/255f, 119f/255f, 1);

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
            SetNeedsDisplay();
        }

        [Export("mouseEntered:")]
        public override void MouseEntered(NSEvent theEvent)
        {
            base.MouseEntered(theEvent);
            _isMouseOver = true;
            SetNeedsDisplay();
        }

        [Export("mouseExited:")]
        public override void MouseExited(NSEvent theEvent)
        {
            base.MouseExited(theEvent);
            _isMouseOver = false;
            SetNeedsDisplay();
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            float padding = 4;
            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;

            if (_isMouseDown)
                CocoaHelper.FillRect(context, Bounds, BackgroundMouseDownColor);
            else if (_isMouseOver)
                CocoaHelper.FillRect(context, Bounds, BackgroundMouseOverColor);
            else
                CocoaHelper.FillRect(context, Bounds, BackgroundColor);

            CocoaHelper.DrawRect(context, Bounds, BorderColor);
            RectangleF rectTextSize = CocoaHelper.MeasureString(Bounds.Size, Title, "Junction", 11);
            RectangleF rectText;
            if (Image != null)
            {
                float xImage = ((Bounds.Width - rectTextSize.Width - (padding * 2) - Image.Size.Width) / 2);
                RectangleF rectImage = new RectangleF(xImage, (Bounds.Height - Image.Size.Height) / 2, Image.Size.Width, Image.Size.Height);
                Image.DrawInRect(rectImage, new RectangleF(0, 0, Image.Size.Width, Image.Size.Height), NSCompositingOperation.SourceOver, 1.0f);

                float xText = xImage + padding + Image.Size.Width + padding;
                rectText = new RectangleF(xText, (Bounds.Height - rectTextSize.Height) / 2, rectTextSize.Width, rectTextSize.Height);
            } 
            else
            {
                rectText = new RectangleF((Bounds.Width - rectTextSize.Width) / 2, (Bounds.Height - rectTextSize.Height) / 2, rectTextSize.Width, rectTextSize.Height);
            }

            CocoaHelper.DrawText(rectText, 0, 0, Title, "Junction", 11, NSColor.White);
        }

        RectangleF Get1pxRect(RectangleF rect)
        {
            RectangleF newRect = new RectangleF(rect.X + 0.5f, rect.Y + 0.5f, rect.Width - 1, rect.Height - 1);
            return newRect;
        }
    }
}
