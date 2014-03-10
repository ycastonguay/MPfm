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
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Interaction;

namespace MPfm.Mac.Classes.Controls.Helpers
{
    public static class GenericControlHelper
    {
        public static RectangleF ToRect(BasicRectangle rectangle)
        {
            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static PointF ToPoint(BasicPoint point)
        {
            return new PointF(point.X, point.Y);
        }

        public static NSColor ToNSColor(BasicColor color)
        {
            return NSColor.FromDeviceRgba(color.R, color.G, color.B, color.A);
        }

        public static CGColor ToCGColor(BasicColor color)
        {
            return new CGColor(color.R/255f, color.G/255f, color.B/255f, color.A/255f);
        }

        public static void MouseUp(NSView view, IControlMouseInteraction control, NSEvent theEvent)
        {
            var point = GetMouseLocation(view, theEvent);
            var button = GetMouseButtonType(theEvent);
            var keysHeld = new KeysHeld();
            //Console.WriteLine("GenericControlHelper - MouseUp - point: {0} button: {1} bounds: {2}", point, button, view.Bounds);
            control.MouseUp(point.X, point.Y, button, keysHeld);
        }    

        public static void MouseDown(NSView view, IControlMouseInteraction control, NSEvent theEvent)
        {
            var point = GetMouseLocation(view, theEvent);
            var button = GetMouseButtonType(theEvent);
            var keysHeld = new KeysHeld();
            //Console.WriteLine("GenericControlHelper - MouseDown - point: {0} button: {1} bounds: {2}", point, button, view.Bounds);
            control.MouseDown(point.X, point.Y, button, keysHeld);
        }    

        public static void MouseMove(NSView view, IControlMouseInteraction control, NSEvent theEvent)
        {
            var point = GetMouseLocation(view, theEvent);
            var button = GetMouseButtonType(theEvent);
            //Console.WriteLine("GenericControlHelper - MouseMove - point: {0} bounds: {1}", point, view.Bounds);
            control.MouseMove(point.X, point.Y, button);
        } 

        public static void MouseClick(NSView view, IControlMouseInteraction control, NSEvent theEvent)
        {
            var point = GetMouseLocation(view, theEvent);
            var button = GetMouseButtonType(theEvent);
            var keysHeld = new KeysHeld();
            //Console.WriteLine("GenericControlHelper - MouseClick - point: {0} bounds: {1}", point, view.Bounds);
            control.MouseClick(point.X, point.Y, button, keysHeld);
        }

        public static void MouseDoubleClick(NSView view, IControlMouseInteraction control, NSEvent theEvent)
        {
            var point = GetMouseLocation(view, theEvent);
            var button = GetMouseButtonType(theEvent);
            var keysHeld = new KeysHeld();
            //Console.WriteLine("GenericControlHelper - MouseClick - point: {0} bounds: {1}", point, view.Bounds);
            control.MouseDoubleClick(point.X, point.Y, button, keysHeld);
        }

        private static PointF GetMouseLocation(NSView view, NSEvent theEvent)
        {
            // Invert point because origin Y is inversed in Cocoa
            //var point = view.ConvertPointFromBase(theEvent.LocationInWindow);
            // ConvertPointfromBase doesn't work in Retina
            //var point = view.ConvertPointFromBacking(theEvent.LocationInWindow);
            //var point = view.ConvertPointToBacking(theEvent.LocationInWindow);
            var point = view.ConvertPointFromView(theEvent.LocationInWindow, null);
            //return new PointF(point.X, view.Bounds.Height - point.Y);
            return point;
        }    
        
        private static MouseButtonType GetMouseButtonType(NSEvent theEvent)
        {
            var button = MouseButtonType.Left;
            switch (theEvent.Type)
            {
                case NSEventType.LeftMouseUp:
                    button = MouseButtonType.Left;
                    break;
                case NSEventType.RightMouseUp:
                    button = MouseButtonType.Right;
                    break;
                case NSEventType.OtherMouseUp:
                    button = MouseButtonType.Middle;
                    break;
            }
            return button;
        }    
    }
}
