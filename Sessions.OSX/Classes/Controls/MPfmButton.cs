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
    [Register("SessionsButton")]
    public class SessionsButton : NSButton
    {
        bool _isMouseDown = false;
        bool _isMouseOver = false;

        private ButtonTheme _theme;
        public ButtonTheme Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
                SetTheme();
            }
        }

        public int RoundedRadius { get; set; }
        public CGColor TextColor { get; set; }
        public CGColor BackgroundColor { get; set; }
        public CGColor DisabledBackgroundColor { get; set; }
        public CGColor BackgroundMouseDownColor { get; set; }
        public CGColor BackgroundMouseOverColor { get; set; }
        public CGColor BorderColor { get; set; }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                SetNeedsDisplay();
            }
        }

        public delegate void ButtonSelected(SessionsButton button);
        public event ButtonSelected OnButtonSelected;

        [Export("init")]
        public SessionsButton() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsButton(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            // This allows MouseEntered and MouseExit to work
            AddTrackingRect(Bounds, this, IntPtr.Zero, false);
            SetTheme();
        }

        private void SetTheme()
        {
            switch (Theme)
            {
                case ButtonTheme.Main:
                    RoundedRadius = 6;
                    TextColor = GlobalTheme.ButtonTextColor;
                    BackgroundColor = GlobalTheme.ButtonBackgroundColor;
                    DisabledBackgroundColor = GlobalTheme.ButtonDisabledBackgroundColor;
                    BackgroundMouseDownColor = GlobalTheme.ButtonBackgroundMouseDownColor;
                    BackgroundMouseOverColor = GlobalTheme.ButtonBackgroundMouseOverColor;
                    BorderColor = GlobalTheme.ButtonBorderColor;
                    break;
                case ButtonTheme.Toolbar:
                    RoundedRadius = 0;
                    BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
                    DisabledBackgroundColor = GlobalTheme.ButtonToolbarDisabledBackgroundColor;
                    BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
                    BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
                    BorderColor = GlobalTheme.ButtonToolbarBorderColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

            if(OnButtonSelected != null && Enabled && _isMouseDown)
                OnButtonSelected(this);

            _isMouseDown = false;
            _isMouseOver = false;
            SetNeedsDisplay();
        }

        [Export("mouseMoved:")]
        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
            if (!_isMouseOver)
            {
                _isMouseOver = true;
                SetNeedsDisplay();
            }
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

            //CocoaHelper.DrawEllipsis(context, Bounds, BorderColor, 1);
            RectangleF rectTextSize = CoreGraphicsHelper.MeasureString(Bounds.Size, Title, "Roboto", 11);
            RectangleF rectText;
            if (Image != null)
            {
                float xImage = ((Bounds.Width - rectTextSize.Width - (padding * 2) - Image.Size.Width) / 2);
                RectangleF rectImage = new RectangleF(xImage, (Bounds.Height - Image.Size.Height) / 2, Image.Size.Width, Image.Size.Height);
                Image.DrawInRect(rectImage, new RectangleF(0, 0, Image.Size.Width, Image.Size.Height), NSCompositingOperation.SourceOver, Enabled ? 1.0f : 0.5f);

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
            CoreGraphicsHelper.DrawText(rectText, 0, 0, Title, "Roboto", 11, Enabled ? NSColor.White : NSColor.LightGray);
        }

        RectangleF Get1pxRect(RectangleF rect)
        {
            RectangleF newRect = new RectangleF(rect.X + 0.5f, rect.Y + 0.5f, rect.Width - 1, rect.Height - 1);
            return newRect;
        }

        public enum ButtonTheme
        {
            Main = 0,
            Toolbar = 1
        }
    }
}
