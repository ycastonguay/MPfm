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
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.MVP.Views;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.iOS.Helpers;

namespace Sessions.iOS
{
    public partial class SyncConnectManualViewController : BaseViewController, ISyncConnectManualView
    {
        public SyncConnectManualViewController()
			: base (UserInterfaceIdiomIsPhone ? "SyncConnectManualViewController_iPhone" : "SyncConnectManualViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
			viewPanel.Layer.CornerRadius = 8;
			btnCancel.SetImage(UIImage.FromBundle("Images/Buttons/cancel"));
			btnConnect.SetImage(UIImage.FromBundle("Images/Buttons/select"));

			// Make sure the Done key closes the keyboard
			txtUrl.ShouldReturn = (a) => {
				txtUrl.ResignFirstResponder();
				return true;
			};

			base.ViewDidLoad();

			var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
			navigationManager.BindSyncConnectManualView(this);
        }

		public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			var screenSize = UIKitHelper.GetDeviceSize();
			View.Frame = new RectangleF(0, 0, screenSize.Width, screenSize.Height);
		}

		partial void actionCancel(NSObject sender)
		{
			Close();
		}

		partial void actionConnect(NSObject sender)
		{
			Close();
		}

		private void Close()
		{
			WillMoveToParentViewController(null);
			UIView.Animate(0.2f, () => {
				this.View.Alpha = 0;
			}, () => {
				View.RemoveFromSuperview();
				RemoveFromParentViewController();
			});
		}
    }
}
