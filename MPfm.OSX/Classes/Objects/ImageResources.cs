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
using System.IO;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace MPfm.OSX.Classes.Objects
{
    /// <summary>
    /// Singleton containing all image resources for the application.
    /// </summary>
    public static class ImageResources
    {
        public static List<NSImage> Images { get; private set; }

        static ImageResources()
        {
            Images = new List<NSImage>() {
                NSImage.ImageNamed("icon_button_add"),
                NSImage.ImageNamed("icon_button_cancel"),
                NSImage.ImageNamed("icon_button_cloud"),
                NSImage.ImageNamed("icon_button_connect"),
                NSImage.ImageNamed("icon_button_delete"),
                NSImage.ImageNamed("icon_button_download"),
                NSImage.ImageNamed("icon_button_dropbox"),
                NSImage.ImageNamed("icon_button_edit"),
                NSImage.ImageNamed("icon_button_general"),
                NSImage.ImageNamed("icon_button_goto"),
                NSImage.ImageNamed("icon_button_library"),
                NSImage.ImageNamed("icon_button_ok"),
                NSImage.ImageNamed("icon_button_open"),
                NSImage.ImageNamed("icon_button_play"),
                NSImage.ImageNamed("icon_button_refresh"),
                NSImage.ImageNamed("icon_button_reset"),
                NSImage.ImageNamed("icon_button_save"),
                NSImage.ImageNamed("icon_button_speaker"),
                NSImage.ImageNamed("icon_button_stop"),
                NSImage.ImageNamed("icon_button_test"),
                NSImage.ImageNamed("icon_roundbutton_add"),
                NSImage.ImageNamed("icon_roundbutton_minus"),
                NSImage.ImageNamed("icon_roundbutton_reset"),
                NSImage.ImageNamed("icon_android"),
                NSImage.ImageNamed("icon_artists"),
                NSImage.ImageNamed("icon_linux"),
                NSImage.ImageNamed("icon_osx"),
                NSImage.ImageNamed("icon_phone"),
                NSImage.ImageNamed("icon_tablet"),
                NSImage.ImageNamed("icon_user"),
                NSImage.ImageNamed("icon_song"),
                NSImage.ImageNamed("icon_vinyl"),
                NSImage.ImageNamed("icon_windows"),
                NSImage.ImageNamed("toolbar_cloud"),
                NSImage.ImageNamed("toolbar_devices"),
                NSImage.ImageNamed("toolbar_equalizer"),
                NSImage.ImageNamed("toolbar_next"),
                NSImage.ImageNamed("toolbar_new"),
                NSImage.ImageNamed("toolbar_open"),
                NSImage.ImageNamed("toolbar_pause"),
                NSImage.ImageNamed("toolbar_play"),
                NSImage.ImageNamed("toolbar_playlist"),
                NSImage.ImageNamed("toolbar_preferences"),
                NSImage.ImageNamed("toolbar_previous"),
                NSImage.ImageNamed("toolbar_repeat"),
                NSImage.ImageNamed("toolbar_save"),
                NSImage.ImageNamed("toolbar_shuffle"),
                NSImage.ImageNamed("pc_android"),
                NSImage.ImageNamed("pc_android_large"),
                NSImage.ImageNamed("pc_linux"),
                NSImage.ImageNamed("pc_linux_large"),
                NSImage.ImageNamed("pc_mac"),
                NSImage.ImageNamed("pc_mac_large"),
                NSImage.ImageNamed("pc_windows"),
                NSImage.ImageNamed("pc_windows_large"),
                NSImage.ImageNamed("phone_android"),
                NSImage.ImageNamed("phone_android_large"),
                NSImage.ImageNamed("phone_iphone"),
                NSImage.ImageNamed("phone_iphone_large"),
                NSImage.ImageNamed("phone_windows"),
                NSImage.ImageNamed("phone_windows_large"),
                NSImage.ImageNamed("tablet_android"),
                NSImage.ImageNamed("tablet_android_large"),
                NSImage.ImageNamed("tablet_ipad"),
                NSImage.ImageNamed("tablet_ipad_large"),
                NSImage.ImageNamed("tablet_windows"),
                NSImage.ImageNamed("tablet_windows_large")
            };
        }
    }
}
