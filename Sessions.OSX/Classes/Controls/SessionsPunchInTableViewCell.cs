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
using System.Linq;

namespace Sessions.OSX.Classes.Controls
{
    [Register("SessionsPunchInTableCellView")]
    public class SessionsPunchInTableCellView : NSTableCellView
    {
        const float Padding = 6;
        public override bool IsFlipped { get { return true; } }

        public SessionsRoundButton PunchInButton { get; private set; }

        [Export("init")]
        public SessionsPunchInTableCellView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsPunchInTableCellView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            PunchInButton = new SessionsRoundButton();
            PunchInButton.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_punch_in");
            AddSubview(PunchInButton);
        }

        public override void ResizeSubviewsWithOldSize(SizeF oldSize)
        {
            //Console.WriteLine("Resize Table View Cell");
            base.ResizeSubviewsWithOldSize(oldSize);
            PunchInButton.Frame = new RectangleF(0, 0, 24, 24);
        }
    }
}
