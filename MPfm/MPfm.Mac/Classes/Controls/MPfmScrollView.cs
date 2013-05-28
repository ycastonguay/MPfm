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
    /// <summary>
    /// Custom button based on NSButton.
    /// </summary>
    [Register("MPfmScrollView")]
    public class MPfmScrollView : NSScrollView
    {
        bool isMouseDown = false;
        MPfmScrollView synchronizedScrollView;

        public bool IsHeaderVisible { get; set; }
        public CGColor GradientColor1 { get; set; }
        public CGColor GradientColor2 { get; set; }
        public CGColor HeaderGradientColor1 { get; set; }
        public CGColor HeaderGradientColor2 { get; set; }

        [Export("init")]
        public MPfmScrollView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmScrollView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            GradientColor1 = new CGColor(0.1f, 0.1f, 0.1f);
            GradientColor2 = new CGColor(0.3f, 0.3f, 0.3f);
            HeaderGradientColor1 = new CGColor(0.2f, 0.2f, 0.2f);
            HeaderGradientColor2 = new CGColor(0.4f, 0.4f, 0.4f);
            //GradientColor1 = new CGColor(0.0f, 1.0f, 0.0f);
            //GradientColor2 = new CGColor(1.0f, 0.0f, 1.0f);
            //HeaderGradientColor1 = new CGColor(1.0f, 0.0f, 0.0f);
            //HeaderGradientColor2 = new CGColor(0.0f, 0.0f, 1.0f);
        }

        [Export("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            // Set flag
            isMouseDown = true;

            base.MouseDown(theEvent);

            // Call mouse up 
            this.MouseUp(theEvent);
        }

        [Export("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            // Call super class
            base.MouseUp(theEvent);

            // Set flag
            isMouseDown = false;
        }

        public void SetSynchronizedScrollView(MPfmScrollView scrollView)
        {
            StopSynchronizing();

            synchronizedScrollView = scrollView;
            NSView synchronizedContentView = synchronizedScrollView.ContentView;

            // Add a notification to know when the scroll view is scrolled
            synchronizedContentView.PostsBoundsChangedNotifications = true;
            NSNotificationCenter.DefaultCenter.AddObserver(NSView.NSViewBoundsDidChangeNotification, (notification) => { 

                // Calculate the difference
                NSClipView changedContentView = (NSClipView)notification.Object;
                RectangleF rect = changedContentView.DocumentVisibleRect();
                PointF curOffset = ContentView.Bounds.Location;
                PointF newOffset = curOffset;
                newOffset.Y = rect.Location.Y;

                // Synchronize position if different
                if (curOffset != newOffset)
                {
                    ContentView.ScrollToPoint(newOffset);
                    ReflectScrolledClipView(ContentView);
                    ContentView.SetNeedsDisplayInRect(ContentView.Frame);
                }
            }, synchronizedContentView);
        }

        public void StopSynchronizing()
        {
            if (synchronizedScrollView == null)
                return;

            NSView synchronizedContentView = synchronizedScrollView.ContentView;

            // Remove notification
            NSNotificationCenter.DefaultCenter.RemoveObserver(this, "NSViewBoundsDidChangeNotification", synchronizedContentView);
            synchronizedScrollView = null;
        }           
    }
}
