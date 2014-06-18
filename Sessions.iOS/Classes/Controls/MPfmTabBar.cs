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
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmTabBar")]
    public class MPfmTabBar : UITabBar
    {
        public MPfmTabBar() : base()
        {
            //TintColor = UIColor.FromRGBA(0.3f, 0.3f, 0.3f, 1);
            //SelectedImageTintColor = UIColor.White;
        }

        public MPfmTabBar(IntPtr handle) : base (handle)
        {
        }

        public override void Draw(RectangleF rect)
        {
            var screenSize = UIKitHelper.GetDeviceSize();
            var context = UIGraphics.GetCurrentContext();
			CoreGraphicsHelper.FillRect(context, rect, GlobalTheme.BackgroundColor.CGColor);
			//CoreGraphicsHelper.FillRect(context, new RectangleF(0, 0, screenSize.Width, 2), GlobalTheme.MainDarkColor.CGColor);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            SetNeedsDisplay();
        }
    }
}
