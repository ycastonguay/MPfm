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
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Sessions.OSX.Classes.Objects;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsTextBoxTableCellView")]
    public class SessionsTextBoxTableCellView : NSTableCellView
    {
        const float Padding = 6;
        public override bool IsFlipped { get { return true; } }

        public NSTextField TextBox { get; private set; }

        [Export("init")]
        public SessionsTextBoxTableCellView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsTextBoxTableCellView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            TextBox = new NSTextField(new RectangleF(0, -2, 14, 24));
            TextBox.Bezeled = true;
            TextBox.Hidden = true;
            TextBox.DrawsBackground = false;
            TextBox.Alignment = NSTextAlignment.Left;
            TextBox.TextColor = NSColor.Black;
            TextBox.Font = NSFont.FromFontName("Roboto", 11);
            AddSubview(TextBox);
        }

        public override void ResizeSubviewsWithOldSize(SizeF oldSize)
        {
            //Console.WriteLine("Resize Table View Cell");
            base.ResizeSubviewsWithOldSize(oldSize);
            TextBox.Frame = new RectangleF(0, -2, 24, 24);
        }
    }
}
