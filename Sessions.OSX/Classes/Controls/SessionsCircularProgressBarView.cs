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
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Sessions.GenericControls.Controls;
using Sessions.OSX.Classes.Controls.Graphics;
using Sessions.OSX.Classes.Controls.Helpers;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsCircularProgressBarView")]
    public class SessionsCircularProgressBarView : NSView
    {
        private CircularProgressBarControl _control;

        public override bool IsOpaque { get { return true; } }
        public override bool IsFlipped { get { return true; } }

        public float Value 
        { 
            get 
            { 
                return _control.Value; 
            } 
            set 
            { 
                _control.Value = value; 
            } 
        }

        [Export("init")]
        public SessionsCircularProgressBarView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsCircularProgressBarView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            // Add tracking area to receive mouse move and mouse dragged events
            var opts = NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.InVisibleRect | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.EnabledDuringMouseDrag;
            var trackingArea = new NSTrackingArea(Bounds, opts, this, new NSDictionary());
            AddTrackingArea(trackingArea);

            _control = new CircularProgressBarControl();    
            _control.OnInvalidateVisual += () => InvokeOnMainThread(() => SetNeedsDisplayInRect(Bounds));
            _control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));
        }
        
        public override void DrawRect(RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);
            
            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
            context.SaveState();
            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height, GenericControlHelper.ToBasicRect(dirtyRect));
            _control.Render(wrapper);
            context.RestoreState();
        }        
    }
}
