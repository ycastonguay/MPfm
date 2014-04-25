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

namespace MPfm.Mac.Classes.Objects
{
    /// <summary>
    /// Singleton containing all image resources for the application.
    /// </summary>
    public static class ImageResources
    {
        public static List<NSImage> images16x16 { get; private set; }
        public static List<NSImage> images32x32 { get; private set; } // TODO: Remove 16x16/32x32 lists to merge in one list with appropriate Retina image loading
        public static List<NSImage> ButtonImages { get; private set; }
        public static List<NSImage> ToolbarImages { get; private set; }
        public static List<NSImage> Images { get; private set; }

        static ImageResources()
        {
            ButtonImages = new List<NSImage>()
            {
                new NSImage(NSBundle.MainBundle.PathForResource("add", "png", "Resources/Buttons", string.Empty)) { Name = "add" },
                new NSImage(NSBundle.MainBundle.PathForResource("minus", "png", "Resources/Buttons", string.Empty)) { Name = "minus" },
                new NSImage(NSBundle.MainBundle.PathForResource("reset", "png", "Resources/Buttons", string.Empty)) { Name = "reset" }
            };

            ToolbarImages = new List<NSImage>()
            {
                new NSImage(NSBundle.MainBundle.PathForResource("play", "png", "Resources/Toolbar", string.Empty)) { Name = "play" },
                new NSImage(NSBundle.MainBundle.PathForResource("pause", "png", "Resources/Toolbar", string.Empty)) { Name = "pause" },
                new NSImage(NSBundle.MainBundle.PathForResource("previous", "png", "Resources/Toolbar", string.Empty)) { Name = "previous" },
                new NSImage(NSBundle.MainBundle.PathForResource("next", "png", "Resources/Toolbar", string.Empty)) { Name = "next" },
                new NSImage(NSBundle.MainBundle.PathForResource("repeat", "png", "Resources/Toolbar", string.Empty)) { Name = "repeat" },
                new NSImage(NSBundle.MainBundle.PathForResource("shuffle", "png", "Resources/Toolbar", string.Empty)) { Name = "shuffle" },
                new NSImage(NSBundle.MainBundle.PathForResource("playlist", "png", "Resources/Toolbar", string.Empty)) { Name = "playlist" },
                new NSImage(NSBundle.MainBundle.PathForResource("effects", "png", "Resources/Toolbar", string.Empty)) { Name = "effects" },
                new NSImage(NSBundle.MainBundle.PathForResource("sync", "png", "Resources/Toolbar", string.Empty)) { Name = "sync" },
                new NSImage(NSBundle.MainBundle.PathForResource("cloud", "png", "Resources/Toolbar", string.Empty)) { Name = "cloud" },
                new NSImage(NSBundle.MainBundle.PathForResource("resume", "png", "Resources/Toolbar", string.Empty)) { Name = "resume" },
                new NSImage(NSBundle.MainBundle.PathForResource("preferences", "png", "Resources/Toolbar", string.Empty)) { Name = "preferences" }
            };

            images16x16 = new List<NSImage>() {
                new NSImage(NSBundle.MainBundle.PathForResource("list-add", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_list-add" },
                new NSImage(NSBundle.MainBundle.PathForResource("accessories-text-editor", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_accessories-text-editor" },
                new NSImage(NSBundle.MainBundle.PathForResource("list-remove", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_list-remove" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-start", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_media-playback-start" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-stop", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_media-playback-stop" },
                new NSImage(NSBundle.MainBundle.PathForResource("go-last", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_go-last" },
                new NSImage(NSBundle.MainBundle.PathForResource("cd", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_cd" },
                new NSImage(NSBundle.MainBundle.PathForResource("database", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_database" },
                new NSImage(NSBundle.MainBundle.PathForResource("group", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_group" },
                new NSImage(NSBundle.MainBundle.PathForResource("user", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_user" },
                new NSImage(NSBundle.MainBundle.PathForResource("document-save", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_document-save" },
                new NSImage(NSBundle.MainBundle.PathForResource("emblem-important", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_emblem-important" },
                new NSImage(NSBundle.MainBundle.PathForResource("network-wireless", "png", "Resources/16x16/tango", string.Empty)) { Name = "16_tango_network-wireless" },

                new NSImage(NSBundle.MainBundle.PathForResource("cancel", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_cancel" },
                new NSImage(NSBundle.MainBundle.PathForResource("delete", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_delete" },
                new NSImage(NSBundle.MainBundle.PathForResource("exclamation", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_exclamation" },
                new NSImage(NSBundle.MainBundle.PathForResource("information", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_information" },
                new NSImage(NSBundle.MainBundle.PathForResource("shape_align_middle", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_shape_align_middle" },
                new NSImage(NSBundle.MainBundle.PathForResource("tick", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_tick" },
                new NSImage(NSBundle.MainBundle.PathForResource("time", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_time" },
                new NSImage(NSBundle.MainBundle.PathForResource("pencil", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_pencil" },
                new NSImage(NSBundle.MainBundle.PathForResource("accept", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_accept" },
                new NSImage(NSBundle.MainBundle.PathForResource("add", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_add" },

                new NSImage(NSBundle.MainBundle.PathForResource("android", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_android" },
                new NSImage(NSBundle.MainBundle.PathForResource("apple", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_apple" },
                new NSImage(NSBundle.MainBundle.PathForResource("laptop", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_laptop" },
                new NSImage(NSBundle.MainBundle.PathForResource("play", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_play" },
                new NSImage(NSBundle.MainBundle.PathForResource("stop", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_stop" },
                new NSImage(NSBundle.MainBundle.PathForResource("plus", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_plus" },
                new NSImage(NSBundle.MainBundle.PathForResource("pencil", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_pencil" },
                new NSImage(NSBundle.MainBundle.PathForResource("quill", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_quill" },
                new NSImage(NSBundle.MainBundle.PathForResource("delete", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_delete" },
                new NSImage(NSBundle.MainBundle.PathForResource("goto", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_goto" },
                new NSImage(NSBundle.MainBundle.PathForResource("close", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_close" },
                new NSImage(NSBundle.MainBundle.PathForResource("refresh", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_refresh" },
                new NSImage(NSBundle.MainBundle.PathForResource("drawer", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_drawer" },
                new NSImage(NSBundle.MainBundle.PathForResource("cabinet", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_cabinet" },
                new NSImage(NSBundle.MainBundle.PathForResource("user", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_user" },
                new NSImage(NSBundle.MainBundle.PathForResource("users", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_users" },
                new NSImage(NSBundle.MainBundle.PathForResource("arrow-right", "png", "Resources/16x16/icomoon", string.Empty)) { Name = "16_icomoon_arrow-right" },

                new NSImage(NSBundle.MainBundle.PathForResource("vinyl", "png", "Resources/16x16/custom", string.Empty)) { Name = "16_custom_vinyl" }
            };

            images32x32 = new List<NSImage>() {
                new NSImage(NSBundle.MainBundle.PathForResource("document-open", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_document-open" },
                new NSImage(NSBundle.MainBundle.PathForResource("drive-harddisk", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_drive-harddisk" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-start", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_media-playback-start" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-pause", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_media-playback-pause" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-stop", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_media-playback-stop" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-skip-backward", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_media-skip-backward" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-skip-forward", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_media-skip-forward" },
                new NSImage(NSBundle.MainBundle.PathForResource("view-refresh", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_view-refresh" },
                new NSImage(NSBundle.MainBundle.PathForResource("preferences-desktop", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_preferences-desktop" },
                new NSImage(NSBundle.MainBundle.PathForResource("audio-x-generic", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_audio-x-generic" },
                new NSImage(NSBundle.MainBundle.PathForResource("preferences-system", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_preferences-system" },
                new NSImage(NSBundle.MainBundle.PathForResource("document-new", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_document-new" },
                new NSImage(NSBundle.MainBundle.PathForResource("document-save", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_document-save" },
                new NSImage(NSBundle.MainBundle.PathForResource("document-save-as", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_document-save-as" },
                new NSImage(NSBundle.MainBundle.PathForResource("network-wireless", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_network-wireless" },

                new NSImage(NSBundle.MainBundle.PathForResource("equalizer", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_equalizer" },
                new NSImage(NSBundle.MainBundle.PathForResource("folder-open", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_folder-open" },
                new NSImage(NSBundle.MainBundle.PathForResource("next", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_next" },
                new NSImage(NSBundle.MainBundle.PathForResource("pause", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_pause" },
                new NSImage(NSBundle.MainBundle.PathForResource("play", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_play" },
                new NSImage(NSBundle.MainBundle.PathForResource("playlist", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_playlist" },
                new NSImage(NSBundle.MainBundle.PathForResource("previous", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_previous" },
                new NSImage(NSBundle.MainBundle.PathForResource("repeat", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_repeat" },
                new NSImage(NSBundle.MainBundle.PathForResource("settings", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_settings" },
                new NSImage(NSBundle.MainBundle.PathForResource("stop", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_stop" },
                new NSImage(NSBundle.MainBundle.PathForResource("sync", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_sync" },
                new NSImage(NSBundle.MainBundle.PathForResource("update", "png", "Resources/32x32/icomoon", string.Empty)) { Name = "32_icomoon_update" }
            };

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
                NSImage.ImageNamed("icon_button_play"),
                NSImage.ImageNamed("icon_button_refresh"),
                NSImage.ImageNamed("icon_button_reset"),
                NSImage.ImageNamed("icon_button_save"),
                NSImage.ImageNamed("icon_button_speaker"),
                NSImage.ImageNamed("icon_button_stop"),
                NSImage.ImageNamed("icon_button_test"),
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
                NSImage.ImageNamed("toolbar_next"),
                NSImage.ImageNamed("toolbar_pause"),
                NSImage.ImageNamed("toolbar_play"),
                NSImage.ImageNamed("toolbar_previous"),
                NSImage.ImageNamed("toolbar_repeat"),
                NSImage.ImageNamed("toolbar_shuffle")
            };
        }
    }
}
