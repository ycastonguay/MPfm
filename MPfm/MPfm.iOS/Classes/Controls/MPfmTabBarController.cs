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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmTabBarController")]
    public class MPfmTabBarController : UITabBarController
    {
        public MPfmTabBarController() : base()
        {
            this.SetValueForKey(new MPfmTabBar(), new NSString("tabBar"));            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

//            RectangleF frame = new RectangleF(0, 0, View.Bounds.Size.Width, 48);
//            UIView view = new UIView(frame);
//            view.BackgroundColor = GlobalTheme.MainDarkColor;
//            TabBar.AddSubview(view);
        }
    }
}
