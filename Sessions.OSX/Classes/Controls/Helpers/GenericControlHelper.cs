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
using System.Collections.Generic;

namespace MPfm.OSX.Classes.Controls.Helpers
{
    public static class GenericControlHelper
    {
        public static BasicRectangle ToBasicRect(RectangleF rectangle)
        {
            return new BasicRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

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

        public static CGPath ToCGPath(BasicPath path)
        {
            // TODO: We assume for now that only path lines can exist. 
            var linePoints = new List<PointF>();
            foreach (var item in path.Items)
            {
                if (item is BasicPathLine)
                {
                    var line = item as BasicPathLine;
                    linePoints.Add(ToPoint(line.PointA));
                    linePoints.Add(ToPoint(line.PointB));
                }
            }

            var cgPath = new CGPath();
            cgPath.AddLines(linePoints.ToArray());
            return cgPath;
        }

        public static void MouseUp(NSView view, IControlMouseInteraction control, NSEvent theEvent)
        {
            MouseUp(view, control, theEvent, false);
        }

        public static void MouseUp(NSView view, IControlMouseInteraction control, NSEvent theEvent, bool isRightButton)
        {
            var button = MouseButtonType.Right;
            if(!isRightButton)
                button = GetMouseButtonType(theEvent);

            var point = GetMouseLocation(view, theEvent);
            var keysHeld = GetKeysHeld(theEvent);
            //Console.WriteLine("GenericControlHelper - MouseUp - point: {0} button: {1} bounds: {2}", point, button, view.Bounds);
            control.MouseUp(point.X, point.Y, button, keysHeld);
        }    

        public static void MouseDown(NSView view, IControlMouseInteraction control, NSEvent theEvent)
        {
            MouseDown(view, control, theEvent, false);
        }

        public static void MouseDown(NSView view, IControlMouseInteraction control, NSEvent theEvent, bool isRightButton)
        {
            var button = MouseButtonType.Right;
            if(!isRightButton)
                button = GetMouseButtonType(theEvent);

            var point = GetMouseLocation(view, theEvent);
            var keysHeld = GetKeysHeld(theEvent);
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
            MouseClick(view, control, theEvent, false);
        }

        public static void MouseClick(NSView view, IControlMouseInteraction control, NSEvent theEvent, bool isRightButton)
        {
            var button = MouseButtonType.Right;
            if(!isRightButton)
                button = GetMouseButtonType(theEvent);

            var point = GetMouseLocation(view, theEvent);
            var keysHeld = GetKeysHeld(theEvent);
            //Console.WriteLine("GenericControlHelper - MouseClick - point: {0} bounds: {1}", point, view.Bounds);
            control.MouseClick(point.X, point.Y, button, keysHeld);
        }

        public static void MouseDoubleClick(NSView view, IControlMouseInteraction control, NSEvent theEvent)
        {
            var point = GetMouseLocation(view, theEvent);
            var button = GetMouseButtonType(theEvent);
            var keysHeld = GetKeysHeld(theEvent);
            //Console.WriteLine("GenericControlHelper - MouseClick - point: {0} bounds: {1}", point, view.Bounds);
            control.MouseDoubleClick(point.X, point.Y, button, keysHeld);
        }

        public static void KeyUp(IControlKeyboardInteraction control, NSEvent theEvent)
        {
            control.KeyUp(' ', GetSpecialKeys(theEvent), new ModifierKeys(), theEvent.IsARepeat);
        }

        public static void KeyDown(IControlKeyboardInteraction control, NSEvent theEvent)
        {
            control.KeyDown(' ', GetSpecialKeys(theEvent), new ModifierKeys(), theEvent.IsARepeat);
        }

        public static void ChangeMouseCursor(MouseCursorType mouseCursorType)
        {
            NSCursor cursor;
            switch (mouseCursorType)
            {
                case MouseCursorType.Default:
                    cursor = NSCursor.ArrowCursor;
                    break;
                case MouseCursorType.HSplit:
                    cursor = NSCursor.ResizeUpDownCursor;
                    break;
                case MouseCursorType.VSplit:
                    cursor = NSCursor.ResizeLeftRightCursor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            cursor.Set();
        }

        public static PointF GetMouseLocation(NSView view, NSEvent theEvent)
        {
            return view.ConvertPointFromView(theEvent.LocationInWindow, null);
        }    
        
        public static MouseButtonType GetMouseButtonType(NSEvent theEvent)
        {
            var button = MouseButtonType.Left;

            // On Mac, the Control key can be used for right click
            if (theEvent.ModifierFlags == NSEventModifierMask.ControlKeyMask)
                return MouseButtonType.Right;

            //Console.WriteLine("Event type: {0}", theEvent.Type);
            switch (theEvent.Type)
            {
                case NSEventType.LeftMouseUp:
                case NSEventType.LeftMouseDown:
                case NSEventType.LeftMouseDragged:
                    button = MouseButtonType.Left;
                    break;
                    // doesn't work
//                case NSEventType.RightMouseUp:
//                case NSEventType.RightMouseDown:
//                case NSEventType.RightMouseDragged:
//                    button = MouseButtonType.Right;
//                    break;
//                case NSEventType.OtherMouseUp:
//                case NSEventType.OtherMouseDown:
//                case NSEventType.OtherMouseDragged:
//                    button = MouseButtonType.Middle;
//                    break;
            }
            return button;
        }    

        public static KeysHeld GetKeysHeld(NSEvent theEvent)
        {
            var keysHeld = new KeysHeld();
            keysHeld.IsShiftKeyHeld = (theEvent.ModifierFlags & NSEventModifierMask.ShiftKeyMask) != 0;
            keysHeld.IsAltKeyHeld = (theEvent.ModifierFlags & NSEventModifierMask.AlternateKeyMask) != 0;
            keysHeld.IsCtrlKeyHeld = (theEvent.ModifierFlags & NSEventModifierMask.ControlKeyMask) != 0;
            return keysHeld;
        }

        public static SpecialKeys GetSpecialKeys(NSEvent theEvent)
        {
            string chars = theEvent.CharactersIgnoringModifiers;
            if (chars.Length == 0)
                return SpecialKeys.None;

            char key = chars[0];
            if (key == (char)NSKey.Return || key == (byte)NSKey.KeypadEnter || key == 13)
                return SpecialKeys.Enter;
            else if (key == (char)NSKey.UpArrow)
                return SpecialKeys.Up;
            else if (key == (char)NSKey.DownArrow)
                return SpecialKeys.Down;
            else if (key == (char)NSKey.LeftArrow)
                return SpecialKeys.Left;
            else if (key == (char)NSKey.RightArrow)
                return SpecialKeys.Right;
            else if (key == (char)NSKey.PageUp)
                return SpecialKeys.PageUp;
            else if (key == (char)NSKey.PageDown)
                return SpecialKeys.PageDown;
            else if (key == (char)NSKey.Home)
                return SpecialKeys.Home;
            else if (key == (char)NSKey.End)
                return SpecialKeys.End;
            else if (key == (char)NSKey.Space)
                return SpecialKeys.Space;

            return SpecialKeys.None;
        }
    }
}
