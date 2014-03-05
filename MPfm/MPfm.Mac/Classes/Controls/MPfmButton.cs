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
using MPfm.Mac.Classes.Objects;
using MonoMac.CoreImage;

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmButton")]
    public class MPfmButton : NSButton
    {
        bool _isMouseDown = false;
        bool _isMouseOver = false;

        public int RoundedRadius { get; set; }
        public CGColor TextColor { get; set; }
        public CGColor BackgroundColor { get; set; }
        public CGColor BackgroundMouseDownColor { get; set; }
        public CGColor BackgroundMouseOverColor { get; set; }
        public CGColor BorderColor { get; set; }

        [Export("init")]
        public MPfmButton() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmButton(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            //Layer.CornerRadius = 8; // Crashes the app. Bug in MonoMac?
            //BezelStyle = NSBezelStyle.Rounded;
            //Cell.BezelStyle = NSBezelStyle.Rounded;
            RoundedRadius = 6;
            TextColor = GlobalTheme.ButtonTextColor;
            BackgroundColor = GlobalTheme.ButtonBackgroundColor;
            BackgroundMouseDownColor = GlobalTheme.ButtonBackgroundMouseDownColor;
            BackgroundMouseOverColor = GlobalTheme.ButtonBackgroundMouseOverColor;
            BorderColor = GlobalTheme.ButtonBorderColor;

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
            context.SaveState();
            context.TranslateCTM(0, Bounds.Height);
            context.ScaleCTM(1, -1);

            if (RoundedRadius == 0)
            {
                if (_isMouseDown)
                    CocoaHelper.FillRect(context, Bounds, BackgroundMouseDownColor);
                else if (_isMouseOver)
                    CocoaHelper.FillRect(context, Bounds, BackgroundMouseOverColor);
                else
                    CocoaHelper.FillRect(context, Bounds, BackgroundColor);

                CocoaHelper.DrawRect(context, Bounds, BorderColor);
            } 
            else
            {
                var path = NSBezierPath.FromRoundedRect(Bounds, RoundedRadius, RoundedRadius);
                NSColor nsColor = null;
                if (_isMouseDown)
                    nsColor = NSColor.FromCIColor(CIColor.FromCGColor(BackgroundMouseDownColor));
                else if (_isMouseOver)
                    nsColor = NSColor.FromCIColor(CIColor.FromCGColor(BackgroundMouseOverColor));
                else
                    nsColor = NSColor.FromCIColor(CIColor.FromCGColor(BackgroundColor));
                nsColor.SetFill();
                path.Fill();
            }

            //CocoaHelper.DrawEllipsis(context, Bounds, BorderColor, 1);
            RectangleF rectTextSize = CocoaHelper.MeasureString(Bounds.Size, Title, "Roboto", 11);
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

            // Fix for Roboto, not sure why the font is rendered a bit below
            rectText.Y = rectText.Y - 2;

            context.RestoreState();
            CocoaHelper.DrawText(rectText, 0, 0, Title, "Roboto", 11, NSColor.White);
        }

        RectangleF Get1pxRect(RectangleF rect)
        {
            RectangleF newRect = new RectangleF(rect.X + 0.5f, rect.Y + 0.5f, rect.Width - 1, rect.Height - 1);
            return newRect;
        }
    }
}
