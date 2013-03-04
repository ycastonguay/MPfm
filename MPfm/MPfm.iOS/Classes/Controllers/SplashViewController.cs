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

namespace MPfm.iOS.Classes.Controllers
{
    public partial class SplashViewController : BaseViewController, ISplashView
    {
        public SplashViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "SplashViewController_iPhone" : "SplashViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            // Check for iPhone 5 resolution (1136x640)
            if (UIScreen.MainScreen.Bounds.Height == 568)
                imageView.Image = UIImage.FromBundle("Images/Splash/Default-568h");
            else
                imageView.Image = UIImage.FromBundle("Images/Splash/Default");

            lblStatus.Font = UIFont.FromName("OstrichSans-Black", 16);

            base.ViewDidLoad();
        }

        #region ISplashView implementation
        
        public void RefreshStatus(string message)
        {
            InvokeOnMainThread(() => {
                lblStatus.Text = message;
            });
        }
        
        public void InitDone()
        {
        }
        
        #endregion    
    }
}
