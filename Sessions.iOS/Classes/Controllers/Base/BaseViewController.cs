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
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.MVP;
using Sessions.MVP.Views;

namespace MPfm.iOS.Classes.Controllers.Base
{
	public abstract class BaseViewController : UIViewController, IBaseView
	{
        #region IBaseView implementation

        public void ShowView(bool shown)
        {
        }

        public Action<IBaseView> OnViewDestroy { get; set; }

        public bool ConfirmBackButton = false;
        public string ConfirmBackButtonTitle = string.Empty;
        public string ConfirmBackButtonMessage = string.Empty;

        #endregion

        public BaseViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }       

        public virtual void ConfirmedBackButton()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            OnViewDestroy(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //OnViewReady(this);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        public override void ViewDidLoad()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                EdgesForExtendedLayout = UIRectEdge.None;
            }

            base.ViewDidLoad();
            this.NavigationItem.SetHidesBackButton(true, true);
        }

        protected void ShowErrorDialog(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("An error has occured", string.Format("{0}", ex), null, "OK", null);
                alertView.Show();
            });
        }

        public static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }
	}
}

