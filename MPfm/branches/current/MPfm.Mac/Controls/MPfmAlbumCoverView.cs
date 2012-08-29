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

namespace MPfm.Mac
{
    /// <summary>
    /// View for displaying album covers in a view-based NSTableView.
    /// </summary>
    [Register("MPfmAlbumCoverView")]
    public class MPfmAlbumCoverView : NSView
    {
        SongBrowserItem item;
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

        public void SetItem(SongBrowserItem item)
        {
            this.item = item;
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

            // Draw background
            CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
            RectangleF rectBackground = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
            CocoaHelper.DrawGradient(context, rectBackground, GradientColor1, GradientColor2);

            // Draw artist name
            RectangleF rectArtistNameSize = CocoaHelper.MeasureString(new SizeF(Bounds.Width, Bounds.Height), item.AudioFile.ArtistName, "TitilliumText25L-800wt", 13);
            RectangleF rectArtistName = new RectangleF(10, rectBackground.Height - 27, rectArtistNameSize.Width, rectArtistNameSize.Height);
            rectArtistName.Inflate(4, 0);
            CocoaHelper.FillRect(context, rectArtistName, new CGColor(0, 0, 0, 0.35f));
            rectArtistName.X += 4;
            rectArtistName.Y += 2;
            CocoaHelper.DrawText(rectArtistName, item.AudioFile.ArtistName, "TitilliumText25L-800wt", 13, NSColor.White);

            // Draw album title
            RectangleF rectAlbumTitleSize = CocoaHelper.MeasureString(new SizeF(Bounds.Width, Bounds.Height), item.AudioFile.AlbumTitle, "TitilliumText25L-400wt", 13);
            RectangleF rectAlbumTitle = new RectangleF(10, rectBackground.Height - 46, rectAlbumTitleSize.Width, rectAlbumTitleSize.Height);
            rectAlbumTitle.Inflate(4, 0);
            CocoaHelper.FillRect(context, rectAlbumTitle, new CGColor(0, 0, 0, 0.35f));
            rectAlbumTitle.X += 4;
            rectAlbumTitle.Y += 2;
            CocoaHelper.DrawText(rectAlbumTitle, item.AudioFile.AlbumTitle, "TitilliumText25L-400wt", 13, NSColor.White);

        }        
    }
}
