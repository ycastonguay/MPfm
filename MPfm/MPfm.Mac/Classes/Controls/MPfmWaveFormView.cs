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

using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.GenericControls.Controls;
using MPfm.Mac.Classes.Controls.Graphics;
using MPfm.Mac.Classes.Controls.Helpers;
using System;

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmWaveFormView")]
    public class MPfmWaveFormView : NSView
    {
        private WaveFormControl _control;

        [Export("init")]
        public MPfmWaveFormView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmWaveFormView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            _control = new WaveFormControl();    
            // TODO: Could these be moved inside a generic helper or something?
            _control.OnInvalidateVisual += () => {
                SetNeedsDisplayInRect(Bounds);
            };
            _control.OnInvalidateVisualInRect += (rect) => {
                SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect));
            };
        }
        
        public override void DrawRect(RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);
            
            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height);
            _control.Render(wrapper);
        }
        
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            GenericControlHelper.MouseUp(this, _control, theEvent);
        }
        
        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);
            GenericControlHelper.MouseDown(this, _control, theEvent);
        }
        
        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
            GenericControlHelper.MouseMove(this, _control, theEvent);
        }
    }
}
