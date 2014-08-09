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
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Sessions.MVP.Views;
using System.Drawing;
using Sessions.OSX.Classes.Controls;

namespace Sessions.OSX
{
    public partial class SelectAlbumArtWindowController : BaseWindowController, ISelectAlbumArtView
    {
        public SelectAlbumArtWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public SelectAlbumArtWindowController(Action<IBaseView> onViewReady)
            : base ("SelectAlbumArtWindow", onViewReady)
        {
            Initialize();
        }

        private void Initialize()
        {
            ShowWindowCentered();
        }

        public override void AwakeFromNib()
        {
            collectionView.ItemPrototype = new SessionsAlbumArtCollectionViewItem();
            //collectionView.Content = new NSObject[2] { new NSString("hello"), new NSString("world")};
            collectionView.MinItemSize = new SizeF(100, 100);
            collectionView.MaxItemSize = new SizeF(100, 100);
            collectionView.WeakDelegate = this;

            OnViewReady(this);
        }

        partial void actionCancel(NSObject sender)
        {
            Close();
        }

        partial void actionApplyToThisSongOnly(NSObject sender)
        {

        }

        partial void actionApplyToAllSongs(NSObject sender)
        {

        }

        #region ISelectAlbumArtView implementation

        public Action<int> OnApplyToAllSongs { get; set; }
        public Action<int> OnApplyToThisSong { get; set; }

        #endregion
    }
}
