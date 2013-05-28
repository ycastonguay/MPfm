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
    [Register("MPfmScrollView")]
    public class MPfmScrollView : NSScrollView
    {
        MPfmScrollView synchronizedScrollView;

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
