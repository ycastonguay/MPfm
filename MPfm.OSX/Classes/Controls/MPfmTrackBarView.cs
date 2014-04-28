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
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.GenericControls.Controls;
using MPfm.Mac.Classes.Controls.Graphics;
using MPfm.Mac.Classes.Controls.Helpers;

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmTrackBarView")]
    public class MPfmTrackBarView : NSView
    {
        private TrackBarControl _control;
        private bool _isMouseDown;

        public override bool IsOpaque { get { return true; } }
        public override bool IsFlipped { get { return true; } }

        public bool BlockValueChangeWhenDraggingMouse { get; set; }

        public int Minimum { get { return _control.Minimum; } set { _control.Minimum = value; } }
        public int Maximum { get { return _control.Maximum; } set { _control.Maximum = value; } }

        public int Value 
        { 
            get 
            { 
                return _control.Value; 
            } 
            set 
            { 
                if (BlockValueChangeWhenDraggingMouse && _isMouseDown)
                    return;

                _control.Value = value; 
            } 
        }
        public int ValueWithoutEvent 
        { 
            get 
            { 
                return _control.ValueWithoutEvent; 
            } 
            set 
            { 
                if (BlockValueChangeWhenDraggingMouse && _isMouseDown)
                    return;

                _control.ValueWithoutEvent = value; 
            } 
        }

        public event TrackBarControl.TrackBarValueChanged OnTrackBarValueChanged;

        [Export("init")]
        public MPfmTrackBarView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmTrackBarView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            // Add tracking area to receive mouse move and mouse dragged events
            var opts = NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.InVisibleRect | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.EnabledDuringMouseDrag;
            var trackingArea = new NSTrackingArea(Bounds, opts, this, new NSDictionary());
            AddTrackingArea(trackingArea);

            _control = new TrackBarControl();    
            _control.OnTrackBarValueChanged += () =>
            {
                if (OnTrackBarValueChanged != null)
                    OnTrackBarValueChanged();
            };
            _control.OnInvalidateVisual += () => InvokeOnMainThread(() => SetNeedsDisplayInRect(Bounds));
            _control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));
        }
        
        public override void DrawRect(RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);
            
            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height, GenericControlHelper.ToBasicRect(dirtyRect));
            _control.Render(wrapper);
        }
        
        public override void MouseUp(NSEvent theEvent)
        {
            _isMouseDown = false;
            base.MouseUp(theEvent);
            GenericControlHelper.MouseUp(this, _control, theEvent);
        }
        
        public override void MouseDown(NSEvent theEvent)
        {
            _isMouseDown = true;
            base.MouseDown(theEvent);
            GenericControlHelper.MouseDown(this, _control, theEvent);
        }
        
        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
            GenericControlHelper.MouseMove(this, _control, theEvent);
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            base.MouseDragged(theEvent);
            GenericControlHelper.MouseMove(this, _control, theEvent);
        }
        
        public override void ScrollWheel(NSEvent theEvent)
        {
            base.ScrollWheel(theEvent);

            if (theEvent.DeltaY > 0)
                _control.MouseWheel(2);
            else if (theEvent.DeltaY < 0)
                _control.MouseWheel(-2);
        }
    }
}
