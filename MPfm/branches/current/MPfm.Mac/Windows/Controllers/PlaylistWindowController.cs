//
// PlaylistWindowController.cs: Playlist window controller.
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
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;

namespace MPfm.Mac
{
    public partial class PlaylistWindowController : MonoMac.AppKit.NSWindowController, IPlaylistView
    {
        readonly IPlaylistPresenter playlistPresenter;

        #region Constructors
        
        // Called when created from unmanaged code
        public PlaylistWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public PlaylistWindowController(NSCoder coder) : base (coder)
        {
            this.playlistPresenter = playlistPresenter;
            Initialize();
            this.playlistPresenter.BindView(this);
        }
        
        // Call to load from the XIB/NIB file
        public PlaylistWindowController(IPlaylistPresenter playlistPresenter)
            : base ("PlaylistWindow")
        {
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
        }
        
        #endregion
        
        //strongly typed window accessor
        public new PlaylistWindow Window
        {
            get
            {
                return (PlaylistWindow)base.Window;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            LoadImages();
        }

        private void LoadImages()
        {
            // Load images in toolbar
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarNewPlaylist").Image = ImageResources.images32x32[11];
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarLoadPlaylist").Image = ImageResources.images32x32[0];
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarSavePlaylist").Image = ImageResources.images32x32[12];
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarSaveAsPlaylist").Image = ImageResources.images32x32[13];
        }

        partial void actionNewPlaylist(NSObject sender)
        {
        }

        partial void actionLoadPlaylist(NSObject sender)
        {
        }

        partial void actionSavePlaylist(NSObject sender)
        {
        }

        partial void actionSaveAsPlaylist(NSObject sender)
        {
        }
    }
}
