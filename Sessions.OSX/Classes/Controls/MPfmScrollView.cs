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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsScrollView")]
    public class SessionsScrollView : NSScrollView
    {
        SessionsScrollView synchronizedScrollView;

        [Export("init")]
        public SessionsScrollView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsScrollView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        public void SetSynchronizedScrollView(SessionsScrollView scrollView)
        {
            StopSynchronizing();

            synchronizedScrollView = scrollView;
            NSView synchronizedContentView = synchronizedScrollView.ContentView;

            // Add a notification to know when the scroll view is scrolled
            synchronizedContentView.PostsBoundsChangedNotifications = true;
            NSNotificationCenter.DefaultCenter.AddObserver(NSView.BoundsChangedNotification, (notification) => { 

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
