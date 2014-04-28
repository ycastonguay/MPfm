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

namespace MPfm.iOS.Helpers
{
    /// <summary>
    /// Helper static class for UIKit.
    /// </summary>
    public static class UIKitHelper
    {
        /// <summary>
        /// Returns the device size, depending on the device orientation (portrait/landscape).
        /// </summary>
        /// <returns>Device size (in pixels)</returns>
        public static SizeF GetDeviceSize()
        {
            float width = UIScreen.MainScreen.Bounds.Width;
            float height = UIScreen.MainScreen.Bounds.Height;
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft ||
                UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                width = UIScreen.MainScreen.Bounds.Height;
                height = UIScreen.MainScreen.Bounds.Width;
            }
            return new SizeF(width, height);
        }

		public static UIColor ColorWithBrightness(UIColor color, float brightnessMultiplier)
		{
			float h, s, b, a;
			color.GetHSBA(out h, out s, out b, out a);
			return UIColor.FromHSBA(h, s, Math.Min(b * brightnessMultiplier, 1), a);
		}
    }
}
