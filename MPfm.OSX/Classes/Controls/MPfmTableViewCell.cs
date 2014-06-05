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
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.OSX.Classes.Objects;

namespace MPfm.OSX.Classes.Controls
{
    [Register("MPfmTableCellView")]
    public class MPfmTableCellView : NSTableCellView
    {
        const float Padding = 6;
        public override bool IsFlipped { get { return true; } }

        [Export("init")]
        public MPfmTableCellView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmTableCellView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        void Initialize()
        {
            //TextField.BackgroundColor = NSColor.Green;
            //TextField.DrawsBackground = true;
            //TextField.TextColor = NSColor.Purple;
            //BackgroundStyle = NSBackgroundStyle.Dark;
//            float x = TextField.Font.FontName.Contains("Roboto") ? -2 : 0;
//            float y = TextField.Font.FontName.Contains("Roboto") ? -2 : 0;
            //TextField.Frame = new RectangleF(-2, -2, Frame.Width, Frame.Height);
        }

        public override void ResizeSubviewsWithOldSize(SizeF oldSize)
        {
            Console.WriteLine(">>>>>>>>>>>>> RESIZE CELL");
            base.ResizeSubviewsWithOldSize(oldSize);
            //TextField.Frame = new RectangleF(10, 10, Frame.Width, Frame.Height);

        }

    }
}
