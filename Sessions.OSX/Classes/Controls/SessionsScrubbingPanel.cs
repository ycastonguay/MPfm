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
using Sessions.OSX.Classes.Helpers;
using Sessions.OSX.Classes.Objects;
using MonoMac.CoreImage;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsScrubbingPanel")]
    public class SessionsScrubbingPanel : NSWindow
    {
        NSTextView _labelTitle;
        NSTextView _labelSubtitle;

        [Export("init")]
        public SessionsScrubbingPanel() 
            : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsScrubbingPanel(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        public SessionsScrubbingPanel(RectangleF contentRect, NSWindowStyle windowStyle, NSBackingStore backingStore, bool deferCreation)
            : base(contentRect, windowStyle, backingStore, deferCreation)
        {
            Initialize();
        }

        private void Initialize()
        {
            BackgroundColor = NSColor.FromDeviceRgba(32/255f, 40/255f, 46/255f, 255);
            AlphaValue = 0.9f;

            _labelTitle = new NSTextView();
            _labelTitle.BackgroundColor = NSColor.Clear;
            _labelTitle.Font = NSFont.FromFontName("Roboto", 11);
            _labelTitle.TextColor = NSColor.LightGray;
            _labelTitle.Alignment = NSTextAlignment.Center;
            _labelTitle.Value = "Move your mouse cursor down to adjust scrubbing speed";
            ContentView.AddSubview(_labelTitle);

            _labelSubtitle = new NSTextView();
            _labelSubtitle.BackgroundColor = NSColor.Clear;
            _labelSubtitle.Font = NSFont.FromFontName("Roboto", 11);
            _labelSubtitle.TextColor = NSColor.White;
            _labelSubtitle.Alignment = NSTextAlignment.Center;
            _labelSubtitle.Value = "High-speed scrubbing";
            ContentView.AddSubview(_labelSubtitle);

            SetViewFrame();
        }

        public void SetSubtitle(string text)
        {
            _labelSubtitle.Value = text;
        }

        public override void LayoutIfNeeded()
        {
            base.LayoutIfNeeded();
            SetViewFrame();
        }

        private void SetViewFrame()
        {
            _labelSubtitle.Frame = new RectangleF(0, 0, Frame.Width, 16);
            _labelTitle.Frame = new RectangleF(0, _labelSubtitle.Frame.Bottom, Frame.Width, 16);
        }
    }
}
