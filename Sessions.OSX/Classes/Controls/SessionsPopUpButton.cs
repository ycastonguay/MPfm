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
using Sessions.OSX.Classes.Objects;
using MonoMac.CoreImage;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsPopUpButton")]
    public class SessionsPopUpButton : NSPopUpButton
    {
        bool _isMouseDown = false;
        bool _isMouseOver = false;

        public int RoundedRadius { get; set; }
        public CGColor TextColor { get; set; }
        public CGColor BackgroundColor { get; set; }
        public CGColor DisabledBackgroundColor { get; set; }
        public CGColor BackgroundMouseDownColor { get; set; }
        public CGColor BackgroundMouseOverColor { get; set; }
        public CGColor BorderColor { get; set; }

        [Export("init")]
        public SessionsPopUpButton() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsPopUpButton(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            RoundedRadius = 6;
            TextColor = GlobalTheme.ButtonTextColor;
            BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            DisabledBackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            BorderColor = GlobalTheme.ButtonToolbarBorderColor;

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

            if (RoundedRadius == 0)
            {
                if (!Enabled)
                    CoreGraphicsHelper.FillRect(context, Bounds, DisabledBackgroundColor);
                else if (_isMouseDown)
                    CoreGraphicsHelper.FillRect(context, Bounds, BackgroundMouseDownColor);
                else if (_isMouseOver)
                    CoreGraphicsHelper.FillRect(context, Bounds, BackgroundMouseOverColor);
                else
                    CoreGraphicsHelper.FillRect(context, Bounds, BackgroundColor);

                CoreGraphicsHelper.DrawRect(context, Bounds, BorderColor, 2);
            } 
            else
            {
                var path = NSBezierPath.FromRoundedRect(Bounds, RoundedRadius, RoundedRadius);
                NSColor nsColor = null;
                if(!Enabled)
                    nsColor = NSColor.FromCIColor(CIColor.FromCGColor(DisabledBackgroundColor));
                else if (_isMouseDown)
                    nsColor = NSColor.FromCIColor(CIColor.FromCGColor(BackgroundMouseDownColor));
                else if (_isMouseOver)
                    nsColor = NSColor.FromCIColor(CIColor.FromCGColor(BackgroundMouseOverColor));
                else
                    nsColor = NSColor.FromCIColor(CIColor.FromCGColor(BackgroundColor));
                nsColor.SetFill();
                path.Fill();
            }

            //CoreGraphicsHelper.DrawRect(context, Bounds, BorderColor, 2);
            RectangleF rectTextSize = CoreGraphicsHelper.MeasureString(Bounds.Size, Title, "Roboto", 11);
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

            CoreGraphicsHelper.DrawText(rectText, 0, 0, Title, "Roboto", 11, NSColor.White);
        }

        RectangleF Get1pxRect(RectangleF rect)
        {
            RectangleF newRect = new RectangleF(rect.X + 0.5f, rect.Y + 0.5f, rect.Width - 1, rect.Height - 1);
            return newRect;
        }
    }
}
