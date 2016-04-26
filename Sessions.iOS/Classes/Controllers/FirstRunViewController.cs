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
using CoreGraphics;
using Foundation;
using UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.MVP.Views;
using Sessions.MVP.Navigation;
using Sessions.MVP.Bootstrap;

namespace Sessions.iOS.Classes.Controllers
{
    public partial class FirstRunViewController : BaseViewController, IFirstRunView
    {
        public FirstRunViewController()
			: base (UserInterfaceIdiomIsPhone ? "FirstRunViewController_iPhone" : "FirstRunViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            btnClose.SetImage(UIImage.FromBundle("Images/Buttons/select"));

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindFirstRunView(this);

        }

        partial void actionClose(NSObject sender)
        {
            OnCloseView();
            DismissViewController(true, null);

            //WillMoveToParentViewController(null);
//            UIView.Animate(0.2f, () => {
//                this.View.Alpha = 0;
//            }, () => {
//                View.RemoveFromSuperview();
//                RemoveFromParentViewController();
//            });
        }

        #region IFirstRunView implementation

        public Action OnCloseView { get; set; }

        public void FirstRunError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        #endregion
    }
}
