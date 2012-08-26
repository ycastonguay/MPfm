//
// ImageResources.cs: Singleton containing all image resources for the application.
//
// Copyright © 2011-2012 Yanick Castonguay
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
                new NSImage(NSBundle.MainBundle.PathForResource("list-add", "png", "Resources/16x16", string.Empty)) { Name = "list-add" },
                new NSImage(NSBundle.MainBundle.PathForResource("accessories-text-editor", "png", "Resources/16x16", string.Empty)) { Name = "accessories-text-editor" },
                new NSImage(NSBundle.MainBundle.PathForResource("list-remove", "png", "Resources/16x16", string.Empty)) { Name = "list-remove" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-start", "png", "Resources/16x16", string.Empty)) { Name = "media-playback-start" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-stop", "png", "Resources/16x16", string.Empty)) { Name = "media-playback-stop" },
                new NSImage(NSBundle.MainBundle.PathForResource("go-last", "png", "Resources/16x16", string.Empty)) { Name = "go-last" },
                new NSImage(NSBundle.MainBundle.PathForResource("cd", "png", "Resources/16x16", string.Empty)) { Name = "cd" },
                new NSImage(NSBundle.MainBundle.PathForResource("database", "png", "Resources/16x16", string.Empty)) { Name = "database" },
                new NSImage(NSBundle.MainBundle.PathForResource("group", "png", "Resources/16x16", string.Empty)) { Name = "group" },
                new NSImage(NSBundle.MainBundle.PathForResource("user", "png", "Resources/16x16", string.Empty)) { Name = "user" }
            };

            // Load 32x32px images
            images32x32 = new List<NSImage>() {
                new NSImage(NSBundle.MainBundle.PathForResource("document-open", "png", "Resources/32x32", string.Empty)) { Name = "document-open" },
                new NSImage(NSBundle.MainBundle.PathForResource("drive-harddisk", "png", "Resources/32x32", string.Empty)) { Name = "drive-harddisk" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-start", "png", "Resources/32x32", string.Empty)) { Name = "media-playback-start" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-pause", "png", "Resources/32x32", string.Empty)) { Name = "media-playback-pause" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-playback-stop", "png", "Resources/32x32", string.Empty)) { Name = "media-playback-stop" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-skip-backward", "png", "Resources/32x32", string.Empty)) { Name = "media-skip-backward" },
                new NSImage(NSBundle.MainBundle.PathForResource("media-skip-forward", "png", "Resources/32x32", string.Empty)) { Name = "media-skip-forward" },
                new NSImage(NSBundle.MainBundle.PathForResource("view-refresh", "png", "Resources/32x32", string.Empty)) { Name = "view-refresh" },
                new NSImage(NSBundle.MainBundle.PathForResource("preferences-desktop", "png", "Resources/32x32", string.Empty)) { Name = "preferences-desktop" },
                new NSImage(NSBundle.MainBundle.PathForResource("audio-x-generic", "png", "Resources/32x32", string.Empty)) { Name = "audio-x-generic" },
                new NSImage(NSBundle.MainBundle.PathForResource("preferences-system", "png", "Resources/32x32", string.Empty)) { Name = "preferences-system" },
                new NSImage(NSBundle.MainBundle.PathForResource("document-new", "png", "Resources/32x32", string.Empty)) { Name = "document-new" },
                new NSImage(NSBundle.MainBundle.PathForResource("document-save", "png", "Resources/32x32", string.Empty)) { Name = "document-save" },
                new NSImage(NSBundle.MainBundle.PathForResource("document-save-as", "png", "Resources/32x32", string.Empty)) { Name = "document-save-as" }
            };

            // Load splash image
            imageSplash = new NSImage(NSBundle.MainBundle.PathForResource("Splash", "png", "Resources", string.Empty));
        }
    }
}
