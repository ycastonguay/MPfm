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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Classes.Controls;
using MPfm.MVP.Views;
using MPfm.MVP.Navigation;
using MPfm.Library.Objects;
using MPfm.iOS.Classes.Delegates;
using MPfm.MVP.Bootstrap;
using MPfm.Core;

namespace MPfm.iOS.Classes.Controllers
{
    public class MainViewController : UITabBarController, IMobileMainView
    {
        public MainViewController() : base()
        {
            this.SetValueForKey(new MPfmTabBar(), new NSString("tabBar"));            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

//            RectangleF frame = new RectangleF(0, 0, View.Bounds.Size.Width, 48);
//            UIView view = new UIView(frame);
//            view.BackgroundColor = GlobalTheme.MainDarkColor;
//            TabBar.AddSubview(view);

            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            appDelegate.ShowMain(this);

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindMobileMainView(this);
        }

        #region IMobileMainView implementation

        public void AddTab(MobileNavigationTabType type, string title, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view)
        {
            Tracing.Log("MainViewController - AddTab - type: {0} title: {1} browserType: {2}", type, title, browserType);
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            appDelegate.AddTab(type, title, (UIViewController)view);
        }

        #endregion

        #region IBaseView implementation

        public Action<IBaseView> OnViewDestroy { get; set; }

        public void ShowView(bool shown)
        {
        }

        #endregion
    }
}