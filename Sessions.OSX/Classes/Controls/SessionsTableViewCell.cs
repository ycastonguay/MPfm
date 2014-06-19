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
    [Register("SessionsTableCellView")]
    public class SessionsTableCellView : NSTableCellView
    {
        const float Padding = 6;
        private CellTheme _theme;
        public override bool IsFlipped { get { return true; } }

        public NSTextField IndexLabel { get; private set; }
        public SessionsView IndexBackground { get; private set; }

//        public override NSBackgroundStyle BackgroundStyle
//        {
//            get
//            {
//                return base.BackgroundStyle;
//            }
//            set
//            {
//                base.BackgroundStyle = value;
//                TextField.DrawsBackground = value != NSBackgroundStyle.Light;
//                TextField.BackgroundColor = value == NSBackgroundStyle.Light ? NSColor.White : NSColor.Green;
//            }
//        }

        [Export("init")]
        public SessionsTableCellView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsTableCellView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            IndexBackground = new SessionsView(new RectangleF(0, 0, 16, 24));
            IndexBackground.BackgroundColor1 = new CGColor(1, 0, 0);
            IndexBackground.BackgroundColor2 = new CGColor(1, 0, 0);
            IndexBackground.Hidden = true;
            AddSubview(IndexBackground);

            IndexLabel = new NSTextField(new RectangleF(0, -2, 14, 24));
            IndexLabel.Bezeled = false;
            IndexLabel.Hidden = true;
            IndexLabel.DrawsBackground = false;
            IndexLabel.Alignment = NSTextAlignment.Center;
            IndexLabel.TextColor = NSColor.White;
            IndexLabel.Font = NSFont.FromFontName("Roboto", 11);
            AddSubview(IndexLabel);
        }

        public void SetTheme(CellTheme theme)
        {
            _theme = theme;
            switch (theme)
            {
                case CellTheme.Normal:
                    IndexLabel.Hidden = true;
                    IndexBackground.Hidden = true;
                    TextField.Hidden = false;
                    break;
                case CellTheme.Index:
                    IndexLabel.Hidden = false;
                    IndexBackground.Hidden = false;
                    TextField.Hidden = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void ResizeSubviewsWithOldSize(SizeF oldSize)
        {
            //Console.WriteLine("Resize Table View Cell");
            base.ResizeSubviewsWithOldSize(oldSize);
            IndexBackground.Frame = new RectangleF(0, 0, 24, 24);
            IndexLabel.Frame = new RectangleF(0, -2, 24, 24);
        }

        public enum CellTheme
        {
            Normal = 0,
            Index = 1
        }
    }
}
