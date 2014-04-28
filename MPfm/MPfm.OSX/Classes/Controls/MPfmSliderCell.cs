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

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmSliderCell")]
    public class MPfmSliderCell : NSSliderCell
    {
        [Export("init")]
        public MPfmSliderCell() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmSliderCell(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        public MPfmSliderCell(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public MPfmSliderCell(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        private void Initialize()
        {
        }
//
//        public override void DrawKnob(RectangleF knobRect)
//        {
//            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
//            CocoaHelper.FillRect(context, knobRect, new CGColor(0, 1, 0, 1));
//
////            //base.DrawKnob(knobRect);
////
////            RectangleF innerRect = knobRect;
////            innerRect.X += 1;
////            innerRect.Y += 1;
////            innerRect.Size.Width -= 2;
////            innerRect.Size.Height -= 2;
////
////            NSBezierPath clipShape = new NSBezierPath();
////            clipShape.AppendPathWithRoundedRect(knobRect, 20, 20);
////
////            NSBezierPath innerShape = new NSBezierPath();
////            innerShape.AppendPathWithRoundedRect(innerRect, 20, 20);
////
////            NSColor knobBorderLight = NSColor.White;
////            NSColor knobTopColor = NSColor.FromDeviceRgba(0.72f, 0.72f, 0.72f, 1);
////            NSColor knobBottomColor = NSColor.White;
////
////            NSGradient knobBorder = new NSGradient(new NSColor[1]{ knobBorderLight }, new float[1]{ 0 });
////            NSGradient knobGradient = new NSGradient(new NSColor[2]{ knobTopColor, knobBottomColor }, new float[2]{ 0, 1 });
////
////            knobBorder.DrawInBezierPath(clipShape, 0.0f);
////            knobGradient.DrawInBezierPath(innerShape, 90.0f);
//        }
//
//        public override void DrawBar(RectangleF rect, bool flipped)
//        {
//            //base.DrawBar(aRect, flipped);
//
//            //rect.Size.Height = 8;
//
////            RectangleF leftRect = rect;
////            leftRect.X = 0;
////            leftRect.Y = 2;
////            leftRect.Size.Width = 
//
//            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
//            CocoaHelper.FillRect(context, rect, new CGColor(1.0f, 0, 0, 1));
//        }
    }
}
