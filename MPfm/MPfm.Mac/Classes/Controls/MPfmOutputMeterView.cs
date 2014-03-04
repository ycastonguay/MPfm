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
    [Register("MPfmOutputMeterView")]
    public class MPfmOutputMeterView : NSView
    {
        private OutputMeterControl _control;

        // https://developer.apple.com/library/mac/documentation/performance/conceptual/Drawing/Articles/CocoaDrawingTips.html
        //public override bool WantsDefaultClipping { get { return false; } }
        public override bool IsOpaque { get { return true; } }

        [Export("init")]
        public MPfmOutputMeterView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmOutputMeterView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            //WantsLayer = true;

            _control = new OutputMeterControl();    
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
            var context = NSGraphicsContext.CurrentContext.GraphicsPort;
            var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height);
            _control.Render(wrapper);
        }

        public void AddWaveDataBlock(float[] waveDataLeft, float[] waveDataRight)
        {
            _control.AddWaveDataBlock(waveDataLeft, waveDataRight);
        }
    }
}
