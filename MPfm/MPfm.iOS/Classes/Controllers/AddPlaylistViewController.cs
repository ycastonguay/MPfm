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

namespace MPfm.iOS
{
    public partial class AddPlaylistViewController : BaseViewController, IAddPlaylistView
    {
        public AddPlaylistViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "AddPlaylistViewController_iPhone" : "AddPlaylistViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            // Make sure the Done key closes the keyboard
            txtPlaylistName.ShouldReturn = (a) => {
                txtPlaylistName.ResignFirstResponder();
                return true;
            };

            base.ViewDidLoad();
        }

        private void CloseDialog()
        {
            WillMoveToParentViewController(null);
            UIView.Animate(0.2f, () => {
                this.View.Alpha = 0;
            }, () => {
                View.RemoveFromSuperview();
                RemoveFromParentViewController();
            });
        }

        partial void actionCancel(NSObject sender)
        {
            CloseDialog();
        }

        partial void actionCreate(NSObject sender)
        {
            OnSavePlaylist(txtPlaylistName.Text);
            CloseDialog();
        }

        #region IAddPlaylistView implementation

        public Action<string> OnSavePlaylist { get; set; }

        public void AddPlaylistError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("AddPlaylist Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        #endregion
    }
}

