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

using MonoMac.CoreGraphics;
using MonoMac.AppKit;
using Sessions.GenericControls.Basics;

namespace Sessions.OSX.Classes.Objects
{
    public static class GlobalTheme
    {
        //public static NSColor MainWindowColor = NSColor.FromDeviceRgba(62f/255f, 79f/255f, 91f/255f, 1);
        //public static NSColor MainWindowColor = NSColor.FromDeviceRgba(130f/255f, 158f/255f, 177f/255f, 1);
        //public static NSColor MainWindowColor = NSColor.FromDeviceRgba(168f/255f, 184f/255f, 195f/255f, 1);
        //public static NSColor MainWindowColor = NSColor.FromDeviceRgba(210f/255f, 220f/255f, 226f/255f, 1);
        public static NSColor MainWindowColor = NSColor.FromDeviceRgba(32f/255f, 40f/255f, 46f/255f, 1);
        //public static NSColor SecondaryWindowColor = NSColor.FromDeviceRgba(69f/255f, 88f/255f, 101f/255f, 1);
        public static NSColor SecondaryWindowColor = NSColor.FromDeviceRgba(83f/255f, 104f/255f, 119f/255f, 1);

        public static CGColor ButtonBackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
        public static CGColor ButtonDisabledBackgroundColor = new CGColor(105f/255f, 62f/255f, 58f/255f, 1);
        public static CGColor ButtonBackgroundMouseDownColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
        public static CGColor ButtonBackgroundMouseOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
        public static CGColor ButtonBorderColor = new CGColor(0.7529f, 0.2235f, 0.1686f, 1);
        public static CGColor ButtonTextColor = new CGColor(1, 1, 1, 1);

        public static CGColor ButtonMainToolbarBackgroundColor = new CGColor(32f/255f, 40f/255f, 46f/255f);
        public static CGColor ButtonMainToolbarBackgroundMouseDownColor = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);
        public static CGColor ButtonMainToolbarBackgroundMouseOverColor = new CGColor(50f/255f, 60f/255f, 67f/255f, 1);
        public static CGColor ButtonMainToolbarBorderColor = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);
        public static CGColor ButtonMainToolbarMouseDownBorderColor = new CGColor(95f/255f, 113f/255f, 126f/255f, 1);
        public static CGColor ButtonMainToolbarMouseOverBorderColor = new CGColor(88f/255f, 100f/255f, 110f/255f, 1);

        public static CGColor ButtonToolbarBackgroundColor = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);
        public static CGColor ButtonToolbarDisabledBackgroundColor = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);
        public static CGColor ButtonToolbarBackgroundMouseDownColor = new CGColor(80f/255f, 100f/255f, 114f/255f, 1);
        public static CGColor ButtonToolbarBackgroundMouseOverColor = new CGColor(83f/255f, 104f/255f, 119f/255f, 1);
        public static CGColor ButtonToolbarBorderColor = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);

        public static CGColor PanelBackgroundColor1 = new CGColor(32f/255f, 40f/255f, 46f/255f);
        public static CGColor PanelBackgroundColor2 = new CGColor(32f/255f, 40f/255f, 46f/255f);
        public static CGColor PanelHeaderColor1 = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);
        public static CGColor PanelHeaderColor2 = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);
        public static CGColor PanelHeader2Color1 = new CGColor(51f/255f, 67f/255f, 78f/255f, 1);
        public static CGColor PanelHeader2Color2 = new CGColor(51f/255f, 67f/255f, 78f/255f, 1);
        public static CGColor PanelBorderColor = new CGColor(0.2745f, 0.3490f, 0.4f, 1);

//        public static CGColor SettingsTabColor = new CGColor(32f/255f, 40f/255f, 46f/255f, 1);
//        public static CGColor SettingsTabSelectedColor = new CGColor(51f/255f, 67f/255f, 78f/255f, 1);
//        public static CGColor SettingsTabOverColor = new CGColor(50f/255f, 60f/255f, 67f/255f, 1);
        public static CGColor SettingsTabColor = new CGColor(32f/255f, 40f/255f, 46f/255f, 1);
        public static CGColor SettingsTabSelectedColor = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);
        public static CGColor SettingsTabOverColor = new CGColor(50f/255f, 60f/255f, 67f/255f, 1);
        public static CGColor SettingsHeaderColor = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);
        public static CGColor SettingsHeader2Color = new CGColor(117f/255f, 135f/255f, 148f/255f, 1);
        public static CGColor SettingsBackgroundColor = new CGColor(242f/255f, 242f/255f, 242f/255f, 1);
        public static BasicColor SettingsGenericBackgroundColor = new BasicColor(242, 242, 242);

        public static CGColor AlbumCoverBackgroundColor1 = new CGColor(0.1490f, 0.1843f, 0.2118f, 1);
        public static CGColor AlbumCoverBackgroundColor2 = new CGColor(0.1490f, 0.1843f, 0.2118f, 1);

        public static CGColor TableHeaderBackgroundColor = new CGColor(69f/255f, 88f/255f, 101f/255f, 1);
        public static CGColor TableHeaderBackgroundMouseDownColor = new CGColor(80f/255f, 100f/255f, 114f/255f, 1);
        public static CGColor TableHeaderBackgroundMouseOverColor = new CGColor(130f/255f, 158f/255f, 177f/255f, 1);
        public static CGColor TableHeaderBorderColor = new CGColor(0.1490f, 0.1843f, 0.2118f, 1);
        public static CGColor TableHeaderTextColor = new CGColor(1, 1, 1, 1);

        public static CGColor WaveFormColor { get { return new CGColor(1, 1, 0.25f, 1); } }

        //public static UIColor PlayerPanelButtonColor { get { return UIColor.FromRGBA(0.2118f, 0.2706f, 0.3098f, 1); } }
        public static CGColor PlayerPanelButtonColor { get { return new CGColor(0.5f, 0.5f, 0.5f, 1); } }
        public static CGColor PlayerPanelBackgroundColor { get { return new CGColor(0, 0, 0, 0.6f); } }
        public static float PlayerPanelButtonAlpha { get { return 0.8f; } }
    }
}
