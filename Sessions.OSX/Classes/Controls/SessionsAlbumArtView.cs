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

using System;
using System.Collections.Generic;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Sessions.OSX.Classes.Helpers;
using Sessions.OSX.Classes.Objects;

namespace Sessions.OSX.Classes.Controls
{
    /// <summary>
    /// Custom view based on NSView for displaying album art and related UI.
    /// </summary>
    [Register("SessionsAlbumArtView")]
    public class SessionsAlbumArtView : NSView
    {
        private NSImageView _imageViewAlbumArt;
        private NSView _viewImageDownloading;
        private NSView _viewImageDownloaded;
        private NSView _viewImageDownloadError;

        [Export("init")]
        public SessionsAlbumArtView() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public SessionsAlbumArtView(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        public SessionsAlbumArtView(RectangleF frameRect) : base(frameRect)
        {
            Initialize();
        }

        public SessionsAlbumArtView(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public SessionsAlbumArtView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        private void Initialize()
        {
            _imageViewAlbumArt = new NSImageView(Bounds);
            _imageViewAlbumArt.WantsLayer = true;
            AddSubview(_imageViewAlbumArt);

            _viewImageDownloading = new NSView(Bounds);
            AddSubview(_viewImageDownloading);

            _viewImageDownloaded = new NSView(Bounds);
            AddSubview(_viewImageDownloaded);

            _viewImageDownloadError = new NSView(Bounds);
            AddSubview(_viewImageDownloadError);
        }

        public void SetImage(NSImage image)
        {
            try
            {
                //CocoaHelper.FadeOut(_imageViewAlbumArt, 0.2f);
                //_imageViewAlbumArt.AlphaValue = 0;
//                NSAnimationContext.BeginGrouping();
//                NSAnimationContext.CurrentContext.Duration = 0.5;
//                var stuff = (NSControl)_imageViewAlbumArt.Animator;
//                stuff.AlphaValue = 0;
//                stuff.SetNeedsDisplay();
//                NSAnimationContext.EndGrouping();

                _imageViewAlbumArt.Image = image;

//                NSAnimationContext.BeginGrouping();
//                NSAnimationContext.CurrentContext.Duration = 0.5;
//                var stuff2 = (NSControl)_imageViewAlbumArt.Animator;
//                stuff2.AlphaValue = 1;
//                stuff2.SetNeedsDisplay();
//                NSAnimationContext.EndGrouping();

                //CocoaHelper.FadeIn(_imageViewAlbumArt, 0.2f);

                //CocoaHelper.FadeOut(_imageViewAlbumArt, 0.2f, () => {
                    //_imageViewAlbumArt.Image = image;
                    //CocoaHelper.FadeIn(_imageViewAlbumArt, 0.2f, null);
                //});            
            }
            catch(Exception ex)
            {
                Console.WriteLine("neeeehh {0}", ex);
            }
        }
    }
}
