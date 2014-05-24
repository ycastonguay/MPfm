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
using MPfm.OSX.Classes.Helpers;
using MPfm.OSX.Classes.Objects;

namespace MPfm.OSX.Classes.Controls
{
    /// <summary>
    /// Custom view based on NSView.
    /// </summary>
    [Register("MPfmView")]
    public class MPfmView : NSView
    {
        private bool isMouseDown = false;
        public bool IsHeaderVisible { get; set; }
        public float HeaderHeight { get; set; }

        public CGColor BackgroundColor1 { get; set; }
        public CGColor BackgroundColor2 { get; set; }
        public CGColor HeaderColor1 { get; set; }
        public CGColor HeaderColor2 { get; set; }
        public CGColor BorderColor { get; set; }

        private bool _isEnabled = false;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                SetSubviewsEnabled(_isEnabled);
            }
        }

        [Export("init")]
        public MPfmView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        public MPfmView(RectangleF frameRect) : base(frameRect)
        {
            Initialize();
        }

        public MPfmView(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public MPfmView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        private void Initialize()
        {
            //WantsLayer = true;
            BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;
            HeaderColor1 = GlobalTheme.PanelHeaderColor1;
            HeaderColor2 = GlobalTheme.PanelHeaderColor2;
            BorderColor = GlobalTheme.PanelBorderColor;
            HeaderHeight = 29;
        }

        [Export("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            isMouseDown = true;
            base.MouseDown(theEvent);
            this.MouseUp(theEvent);
        }

        [Export("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            isMouseDown = false;
        }

        private void SetSubviewsEnabled(bool enabled)
        {
            foreach(var currentView in Subviews)
            {
                var currentControl = currentView as NSControl;
                if(currentControl != null)
                {
                    currentControl.Enabled = enabled;
                    currentControl.Display();
                }
            }
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;

            if (CGColor.Equals(BackgroundColor1, BackgroundColor2))
                CoreGraphicsHelper.FillRect(context, Bounds, BackgroundColor1);
            else
                CoreGraphicsHelper.FillGradient(context, Bounds, BackgroundColor1, BackgroundColor2, false);

            if (IsHeaderVisible)
            {
                RectangleF rectHeader = new RectangleF(0, Bounds.Height - HeaderHeight, Bounds.Width, HeaderHeight);
                CoreGraphicsHelper.FillRect(context, rectHeader, HeaderColor1);
                //CocoaHelper.DrawLine(context, new PointF[2] { new PointF(0, Bounds.Height - 24), new PointF(Bounds.Width, Bounds.Height - 24) }, 0.5f, new CGColor(0.4f, 1, 1, 1));
            }
        }

        RectangleF Get1pxRect(RectangleF rect)
        {
            RectangleF newRect = new RectangleF(rect.X + 0.5f, rect.Y + 0.5f, rect.Width - 1, rect.Height - 1);
            return newRect;
        }
    }
}
