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

using MonoTouch.UIKit;

namespace Sessions.iOS.Classes.Objects
{
	public static class GlobalTheme
	{
        public static UIColor MainColor { get { return UIColor.FromRGBA(0.2118f, 0.2706f, 0.3098f, 1); } }
        public static UIColor MainDarkColor { get { return UIColor.FromRGBA(0.1490f, 0.1843f, 0.2118f, 1); } }
        public static UIColor MainLightColor { get { return UIColor.FromRGBA(0.2745f, 0.3490f, 0.4f, 1); } }
        public static UIColor SecondaryColor { get { return UIColor.FromRGBA(0.9059f, 0.2980f, 0.2353f, 1); } }
		public static UIColor SecondaryLightColor { get { return UIColor.FromRGBA(234, 138, 128, 255); } }
        public static UIColor SecondaryDarkColor { get { return UIColor.FromRGBA(0.7529f, 0.2235f, 0.1686f, 1); } }
        public static UIColor LightColor { get { return UIColor.FromRGBA(1.0f, 1.0f, 1.0f, 1); } }

		//public static UIColor BackgroundColor { get { return UIColor.FromRGBA(0.1255f, 0.1569f, 0.1804f, 1); } } 32, 40, 46
		//public static UIColor BackgroundColor { get { return UIColor.FromRGBA(36, 47, 53, 255); } }
		public static UIColor BackgroundColor { get { return UIColor.FromRGBA(32, 40, 46, 255); } }
		public static UIColor BackgroundDarkColor { get { return UIColor.FromRGBA(21, 26, 30, 255); } }
		public static UIColor BackgroundDarkerColor { get { return UIColor.FromRGBA(16, 20, 23, 255); } }

        public static UIColor WaveFormColor { get { return UIColor.FromRGBA(1, 1, 0.25f, 1); } }

        //public static UIColor PlayerPanelButtonColor { get { return UIColor.FromRGBA(0.2118f, 0.2706f, 0.3098f, 1); } }
		public static UIColor PlayerPanelButtonColor { get { return UIColor.FromRGBA(0.6f, 0.6f, 0.6f, 0.2f); } }
        public static UIColor PlayerPanelBackgroundColor { get { return UIColor.FromRGBA(0, 0, 0, 0.6f); } }
        public static float PlayerPanelButtonAlpha { get { return 0.8f; } }

		public static UIColor AlternateColor1 { get { return UIColor.FromRGB(47, 129, 183); } }
    }
}
