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
using CoreGraphics;
using CoreGraphics;
using Foundation;
using UIKit;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsTabBar")]
    public class SessionsTabBar : UITabBar
    {
        public SessionsTabBar() : base()
        {
            //TintColor = UIColor.FromRGBA(0.3f, 0.3f, 0.3f, 1);
            //SelectedImageTintColor = UIColor.White;
        }

        public SessionsTabBar(IntPtr handle) : base (handle)
        {
        }

        public override void Draw(CGRect rect)
        {
            var screenSize = UIKitHelper.GetDeviceSize();
            var context = UIGraphics.GetCurrentContext();
            context.SaveState();
			CoreGraphicsHelper.FillRect(context, rect, GlobalTheme.BackgroundColor.CGColor);
			//CoreGraphicsHelper.FillRect(context, new RectangleF(0, 0, screenSize.Width, 2), GlobalTheme.MainDarkColor.CGColor);
            context.RestoreState();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            SetNeedsDisplay();
        }
    }
}
