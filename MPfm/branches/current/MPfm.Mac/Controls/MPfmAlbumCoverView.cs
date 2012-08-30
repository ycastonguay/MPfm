//
// MPfmAlbumCoverView.cs: View for displaying album covers in a view-based NSTableView.
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
using System.Drawing;
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Ninject;
using MPfm.MVP;

namespace MPfm.Mac
{
    /// <summary>
    /// View for displaying album covers in a view-based NSTableView.
    /// </summary>
    [Register("MPfmAlbumCoverView")]
    public class MPfmAlbumCoverView : NSView
    {
        SongBrowserItem item;
        NSImage imageAlbumCover;
        bool isMouseDown = false;

        public CGColor GradientColor1 { get; set; }
        public CGColor GradientColor2 { get; set; }

        [Export("init")]
        public MPfmAlbumCoverView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmAlbumCoverView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            GradientColor1 = new CGColor(0.2f, 0.2f, 0.2f);
            GradientColor2 = new CGColor(0.3f, 0.3f, 0.3f);

            //this.FocusRingType = NSFocusRingType.None;
        }

        public void SetItem(SongBrowserItem item, NSImage imageAlbumCover)
        {
            this.item = item;
            this.imageAlbumCover = imageAlbumCover;
        }

        [Export("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            // Set flag
            isMouseDown = true;

            base.MouseDown(theEvent);

            // Call mouse up 
            this.MouseUp(theEvent);
        }

        [Export("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            // Call super class
            base.MouseUp(theEvent);

            // Set flag
            isMouseDown = false;
        }

        public override void DrawRect(System.Drawing.RectangleF dirtyRect)
        {
            base.DrawRect(dirtyRect);

            float padding = 6;

            // Draw background
            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
            RectangleF rectBackground = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
            CocoaHelper.DrawGradient(context, rectBackground, GradientColor1, GradientColor2);

            // Check if an album cover is available
            if (imageAlbumCover != null)
            {
                // Check which is the smallest value
                float bounds = Bounds.Width;
                if(Bounds.Height < Bounds.Width)
                    bounds = Bounds.Height;

                // Draw image
                float imageX = ((Bounds.Width - bounds) / 2) + 6;
                float imageY = Bounds.Height - bounds + 6;
                RectangleF rectImage = new RectangleF(imageX, imageY, bounds - 12, bounds - 12);
                imageAlbumCover.DrawInRect(rectImage, new RectangleF(0, 0, imageAlbumCover.Size.Width, imageAlbumCover.Size.Height), NSCompositingOperation.Copy, 1.0f);

                // Draw artist name
                RectangleF rectArtistNameSize = CocoaHelper.MeasureString(new SizeF(Bounds.Width - 16, Bounds.Height), item.AudioFile.ArtistName, "TitilliumText25L-800wt", 13);
                float artistNameX = ((Bounds.Width - rectArtistNameSize.Width) / 2) + 6;
                RectangleF rectArtistName = new RectangleF(artistNameX, rectBackground.Height - 32, rectArtistNameSize.Width, rectArtistNameSize.Height);
                rectArtistName.Inflate(4, 0);
                CocoaHelper.FillRect(context, rectArtistName, new CGColor(0, 0, 0, 0.6f));
                CocoaHelper.DrawText(rectArtistName, 4, 2, item.AudioFile.ArtistName, "TitilliumText25L-800wt", 13, NSColor.White);
                
                // Draw album title
                RectangleF rectAlbumTitleSize = CocoaHelper.MeasureString(new SizeF(Bounds.Width - 16, Bounds.Height), item.AudioFile.AlbumTitle, "TitilliumText25L-400wt", 13);
                float albumTitleX = ((Bounds.Width - rectAlbumTitleSize.Width) / 2) + 6;
                RectangleF rectAlbumTitle = new RectangleF(albumTitleX, imageY + 6, rectAlbumTitleSize.Width, rectAlbumTitleSize.Height);
                rectAlbumTitle.Inflate(4, 0);
                CocoaHelper.FillRect(context, rectAlbumTitle, new CGColor(0, 0, 0, 0.6f));
                CocoaHelper.DrawText(rectAlbumTitle, 4, 2, item.AudioFile.AlbumTitle, "TitilliumText25L-400wt", 13, NSColor.White);
            }
            // No album cover
            else
            {
                float y = 8;
                
                // Draw artist name
                RectangleF rectArtistNameSize = CocoaHelper.MeasureString(new SizeF(Bounds.Width - 16, Bounds.Height), item.AudioFile.ArtistName, "TitilliumText25L-800wt", 13);
                y += rectArtistNameSize.Height;
                RectangleF rectArtistName = new RectangleF(10, rectBackground.Height - y, rectArtistNameSize.Width, rectArtistNameSize.Height);
                rectArtistName.Inflate(4, 0); // 
                CocoaHelper.FillRect(context, rectArtistName, new CGColor(0, 0, 0, 0.35f));
                CocoaHelper.DrawText(rectArtistName, 4, 2, item.AudioFile.ArtistName, "TitilliumText25L-800wt", 13, NSColor.White);
                
                // Draw album title
                RectangleF rectAlbumTitleSize = CocoaHelper.MeasureString(new SizeF(Bounds.Width - 16, Bounds.Height), item.AudioFile.AlbumTitle, "TitilliumText25L-400wt", 13);
                y += rectAlbumTitleSize.Height;
                RectangleF rectAlbumTitle = new RectangleF(10, rectBackground.Height - y, rectAlbumTitleSize.Width, rectAlbumTitleSize.Height);
                rectAlbumTitle.Inflate(4, 0);
                CocoaHelper.FillRect(context, rectAlbumTitle, new CGColor(0, 0, 0, 0.35f));
                CocoaHelper.DrawText(rectAlbumTitle, 4, 2, item.AudioFile.AlbumTitle, "TitilliumText25L-400wt", 13, NSColor.White);
            }

        }        
    }
}
