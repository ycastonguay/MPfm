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
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Delegates;
using MPfm.iOS.Classes.Objects;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace MPfm.iOS.Classes.Controllers
{
	public class MainViewController : UIViewController, IMobileMainView
    {
		private bool _isAnimating;
		private List<Tuple<UIViewController, MobileDialogPresentationType>> _viewControllers;

		public MPfmTabBarController TabBarController { get; private set; }

        public MainViewController() : base()
        {
			_viewControllers = new List<Tuple<UIViewController, MobileDialogPresentationType>>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			//View.BackgroundColor = GlobalTheme.BackgroundColor;
			View.BackgroundColor = GlobalTheme.MainColor;
			TabBarController = new MPfmTabBarController();
			View.AddSubview(TabBarController.View);
			// Is it needed to add ChildViewController? So far rotation works.

			var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            appDelegate.ShowMain(this);

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindMobileMainView(this);
        }

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			UpdateLayout();
		}

		private void UpdateLayout()
		{
			if (_isAnimating)
				return;

			var notificationViews = _viewControllers.Where(x => x.Item2 == MobileDialogPresentationType.NotificationBar).ToList();
			for (int i = 0; i < notificationViews.Count; i++)
			{
				var viewCtrl = notificationViews[i].Item1;
				float height = (i + 1) * 54;
				viewCtrl.View.Frame = new RectangleF(0, View.Bounds.Height - height, View.Bounds.Width, 54);
			}
			TabBarController.View.Frame = new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height - (notificationViews.Count * 54));
		}

		public void AddViewController(UIViewController viewController, MobileDialogPresentationType presentationType)
		{
			var itemExists = _viewControllers.FirstOrDefault(x => x.Item1 == viewController);
			if (itemExists != null)
				return;

			_viewControllers.Add(new Tuple<UIViewController, MobileDialogPresentationType>(viewController, presentationType));
			UIView view = viewController.View;		
			view.Alpha = 0;

			if (presentationType == MobileDialogPresentationType.TabBar)
			{
				// This one is a bit different; we are adding it as a child of the TabBar controller
				//TabBarController.AddChildViewController(viewController);
				TabBarController.View.AddSubview(view);
				//viewController.View.BringSubviewToFront(view);
				//viewController.DidMoveToParentViewController(TabBarController);
			}
			else
			{
				AddChildViewController(viewController);
				View.AddSubview(view);
				viewController.View.BringSubviewToFront(view);
				viewController.DidMoveToParentViewController(this);
			}

			switch (presentationType)
			{
				case MobileDialogPresentationType.Overlay:
					view.Frame = new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height);
					break;
				case MobileDialogPresentationType.NotificationBar:
					view.Frame = new RectangleF(0, View.Bounds.Height, View.Bounds.Width, 54);
					break;
				case MobileDialogPresentationType.TabBar:
					//view.Frame = new RectangleF(0, TabBarController.View.Bounds.Height, TabBarController.View.Bounds.Width, 54);
					float tabBarHeight = 49;
					view.Frame = new RectangleF(0, TabBarController.View.Bounds.Height - 54 - tabBarHeight, TabBarController.View.Bounds.Width, 54);
					view.Alpha = 1;
					break;
			}

			int notificationViewCount = _viewControllers.Count(x => x.Item2 == MobileDialogPresentationType.NotificationBar);
			switch (presentationType)
			{
				case MobileDialogPresentationType.Overlay:
					UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () => view.Alpha = 1, null);
					break;
				case MobileDialogPresentationType.NotificationBar:
					UIView.Animate(0.4, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
						{
							_isAnimating = true;
							view.Alpha = 1;
							view.Frame = new RectangleF(0, View.Bounds.Height - (notificationViewCount * 54), View.Bounds.Width, 54);
							TabBarController.View.Frame = new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height - (notificationViewCount * 54));
						}, () => _isAnimating = false);
					break;
				case MobileDialogPresentationType.TabBar:
							float tabBarHeight = 49;
							view.Alpha = 1;
							view.Frame = new RectangleF(0, TabBarController.View.Bounds.Height - 54 - tabBarHeight, TabBarController.View.Bounds.Width, 54);

//					UIView.Animate(0.4, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
//						{
//							float tabBarHeight = 49;
//							_isAnimating = true;
//							view.Alpha = 1;
//							view.Frame = new RectangleF(0, TabBarController.View.Bounds.Height - 54 - tabBarHeight, TabBarController.View.Bounds.Width, 54);
//							//TabBarController.View.Frame = new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height - 54);
//						}, () => _isAnimating = false);
					break;
			}
		}

		public void RemoveViewController(UIViewController viewController)
		{
			var item = _viewControllers.FirstOrDefault(x => x.Item1 == viewController);
			if (item == null)
				return;

			UIView.Animate(0.4, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				_isAnimating = true;
				//viewController.View.Alpha = 0;
				viewController.View.Frame = new RectangleF(0, View.Bounds.Height, View.Bounds.Width, 54);
				TabBarController.View.Frame = new RectangleF(0, 0, View.Bounds.Width, View.Bounds.Height);
			}, () => {
				_isAnimating = false;
				_viewControllers.Remove(item);
				viewController.WillMoveToParentViewController(null);
				viewController.View.RemoveFromSuperview();
				viewController.RemoveFromParentViewController();
			});
		}

		public override bool ShouldAutomaticallyForwardRotationMethods { get { return true; } }
		public override bool ShouldAutomaticallyForwardAppearanceMethods { get { return true; } }

        #region IMobileMainView implementation

        public void AddTab(MobileNavigationTabType type, string title, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view)
        {
			//Tracing.Log("MainViewController - AddTab - type: {0} title: {1} browserType: {2}", type, title, browserType);
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
