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
using Sessions.MVP.Views;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;

namespace Sessions.iOS.Classes.Controllers
{
    public partial class AboutViewController : BaseViewController, IAboutView
    {
        public AboutViewController()
            : base (UserInterfaceIdiomIsPhone ? "AboutViewController_iPhone" : "AboutViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                NavigationController.InteractivePopGestureRecognizer.WeakDelegate = this;
                NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            }

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindAboutView(this);
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            SessionsNavigationController navCtrl = (SessionsNavigationController)this.NavigationController;
            navCtrl.SetTitle("About Sessions");
        }

        #region IAboutView implementation

		public void RefreshAboutContent(string version, string content)
        {
        }

        #endregion
    }
}
