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

namespace MPfm.Mac
{
    /// <summary>
    /// Singleton containing all image resources for the application.
    /// </summary>
    public static class ImageResources
    {
        /// <summary>
        /// Static list of 16x16px images.
        /// </summary>
        public static List<NSImage> images16x16 { get; private set; }
        /// <summary>
        /// Static list of 32x32px images.
        /// </summary>
        public static List<NSImage> images32x32 { get; private set; }
        public static NSImage imageSplash { get; private set; }

        /// <summary>
        /// Constructor for the ImageResources static class.
        /// </summary>
        static ImageResources()
        {
            // Load 16x16px images
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

                new NSImage(NSBundle.MainBundle.PathForResource("cancel", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_cancel" },
                new NSImage(NSBundle.MainBundle.PathForResource("delete", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_delete" },
                new NSImage(NSBundle.MainBundle.PathForResource("exclamation", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_exclamation" },
                new NSImage(NSBundle.MainBundle.PathForResource("information", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_information" },
                new NSImage(NSBundle.MainBundle.PathForResource("shape_align_middle", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_shape_align_middle" },
                new NSImage(NSBundle.MainBundle.PathForResource("tick", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_tick" },
                new NSImage(NSBundle.MainBundle.PathForResource("time", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_time" },
                new NSImage(NSBundle.MainBundle.PathForResource("pencil", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_pencil" },
                new NSImage(NSBundle.MainBundle.PathForResource("accept", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_accept" },
                new NSImage(NSBundle.MainBundle.PathForResource("add", "png", "Resources/16x16/fam", string.Empty)) { Name = "16_fam_add" }
            };

            // Load 32x32px images
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
                new NSImage(NSBundle.MainBundle.PathForResource("document-save-as", "png", "Resources/32x32/tango", string.Empty)) { Name = "32_tango_document-save-as" }
            };

            // Load splash image
            imageSplash = new NSImage(NSBundle.MainBundle.PathForResource("Splash", "png", "Resources", string.Empty));
        }
    }
}
