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

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmTabButton")]
    public class MPfmTabButton : NSButton
    {
        bool _isMouseDown = false;
        bool _isMouseOver = false;

        public CGColor TextColor { get; set; }
        public CGColor BackgroundColor { get; set; }
        public CGColor BackgroundMouseDownColor { get; set; }
        public CGColor BackgroundMouseOverColor { get; set; }
        public CGColor BackgroundSelectedColor { get; set; }
        public CGColor BorderColor { get; set; }
        public bool ShowSelectedBackgroundColor { get; set; }
        public LineLocationEnum LineLocation { get; set; }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                SetNeedsDisplay();
            }
        }

        public delegate void TabButtonSelected(MPfmTabButton button);
        public event TabButtonSelected OnTabButtonSelected;

        [Export("init")]
        public MPfmTabButton() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmTabButton(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            TextColor = GlobalTheme.ButtonTextColor;
            //BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            BackgroundColor = GlobalTheme.PanelHeaderColor1;
            BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            BackgroundSelectedColor = GlobalTheme.SettingsTabSelectedColor;
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
            IsSelected = true;

            if(OnTabButtonSelected != null)
                OnTabButtonSelected(this);
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

            if (_isMouseDown)
                CoreGraphicsHelper.FillRect(context, Bounds, BackgroundMouseDownColor);
            else if (_isMouseOver)
                CoreGraphicsHelper.FillRect(context, Bounds, BackgroundMouseOverColor);
            else if (IsSelected && ShowSelectedBackgroundColor)
                CoreGraphicsHelper.FillRect(context, Bounds, BackgroundSelectedColor);
            else
                CoreGraphicsHelper.FillRect(context, Bounds, BackgroundColor);

            if(IsSelected && LineLocation == LineLocationEnum.Bottom)
                CoreGraphicsHelper.DrawLine(context, new PointF[2] { new PointF(0, 1), new PointF(Bounds.Width, 1) }, 0.5f, new CGColor(0.4f, 1, 1, 1));
            else if(IsSelected && LineLocation == LineLocationEnum.Top)
                CoreGraphicsHelper.DrawLine(context, new PointF[2] { new PointF(0, Bounds.Height - 1), new PointF(Bounds.Width, Bounds.Height - 1) }, 0.5f, new CGColor(0.4f, 1, 1, 1));

            //CocoaHelper.DrawRect(context, Bounds, BorderColor);
            //RectangleF rectTextSize = CocoaHelper.MeasureString(Bounds.Size, Title, "Junction", 11);
            RectangleF rectTextSize = CoreGraphicsHelper.MeasureString(Bounds.Size, Title, "Roboto", 12);
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

            // Fix for Roboto
            rectText.Y = rectText.Y - 2;

            context.RestoreState();
            CoreGraphicsHelper.DrawText(rectText, 0, 0, Title, "Roboto", 12, NSColor.White);
        }

        RectangleF Get1pxRect(RectangleF rect)
        {
            RectangleF newRect = new RectangleF(rect.X + 0.5f, rect.Y + 0.5f, rect.Width - 1, rect.Height - 1);
            return newRect;
        }

        public enum LineLocationEnum
        {
            Bottom = 0, Top = 1
        }
    }
}
