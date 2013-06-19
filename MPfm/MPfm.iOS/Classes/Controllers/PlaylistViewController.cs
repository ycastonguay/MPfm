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
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Views;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Classes.Controls;

namespace MPfm.iOS
{
    public partial class PlaylistViewController : BaseViewController, IPlaylistView
    {
        UIBarButtonItem _btnDone;

        public PlaylistViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "PlaylistViewController_iPhone" : "PlaylistViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            var btnDone = new MPfmFlatButton();
            btnDone.Label.Text = "Done";
            btnDone.Frame = new RectangleF(0, 0, 70, 44);
            btnDone.OnButtonClick += () => {
                NavigationController.DismissViewController(true, null);
            };
            var btnDoneView = new UIView(new RectangleF(0, 0, 70, 44));
            var rect = new RectangleF(btnDoneView.Bounds.X + 5, btnDoneView.Bounds.Y, btnDoneView.Bounds.Width, btnDoneView.Bounds.Height);
            btnDoneView.Bounds = rect;
            btnDoneView.AddSubview(btnDone);
            _btnDone = new UIBarButtonItem(btnDoneView);
            NavigationItem.SetLeftBarButtonItem(_btnDone, true);

            base.ViewDidLoad();
        }

        partial void actionRepeat(NSObject sender)
        {
        }

        partial void actionShuffle(NSObject sender)
        {
        }
    }
}
