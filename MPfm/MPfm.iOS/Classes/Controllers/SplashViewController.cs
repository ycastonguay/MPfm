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
            Console.WriteLine("SplashViewController - UIScreen width: {0} height: {1}", UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
            if (UIScreen.MainScreen.Bounds.Height == 568)
                imageView.Image = UIImage.FromBundle("Images/Splash/splash_default-568h");
            else if (UIScreen.MainScreen.Bounds.Height == 768)
                imageView.Image = UIImage.FromBundle("Images/Splash/splash_landscape");
            else if (UIScreen.MainScreen.Bounds.Height == 1024)
                imageView.Image = UIImage.FromBundle("Images/Splash/splash_portrait");
            else
                imageView.Image = UIImage.FromBundle("Images/Splash/splash_default");

            imageViewLogo.Image = UIImage.FromBundle("Images/Splash/app_badge");
            lblStatus.Font = UIFont.FromName("HelveticaNeue-Light", 14);

            imageViewLogo.Alpha = 0;
            lblStatus.Alpha = 0;
            activityIndicator.Alpha = 0;
            activityIndicator.StartAnimating();

            imageViewLogo.Frame = new RectangleF((UIScreen.MainScreen.Bounds.Width - imageViewLogo.Bounds.Width) / 2.0f, (UIScreen.MainScreen.Bounds.Height - imageViewLogo.Bounds.Height) / 2.0f, imageViewLogo.Bounds.Width, imageViewLogo.Bounds.Height);
            activityIndicator.Frame = new RectangleF(activityIndicator.Frame.X, imageViewLogo.Frame.Y + imageViewLogo.Frame.Height + 28, activityIndicator.Frame.Width, activityIndicator.Frame.Height);
            lblStatus.Frame = new RectangleF(lblStatus.Frame.X, activityIndicator.Frame.Y, lblStatus.Frame.Width, lblStatus.Frame.Height);

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            imageViewLogo.Frame = new RectangleF(imageViewLogo.Frame.X, imageViewLogo.Frame.Y + 50, imageViewLogo.Frame.Width, imageViewLogo.Frame.Height);
            UIView.Animate(0.5, 0, UIViewAnimationOptions.CurveEaseInOut, () => {

                imageViewLogo.Frame = new RectangleF(imageViewLogo.Frame.X, imageViewLogo.Frame.Y - 50, imageViewLogo.Frame.Width, imageViewLogo.Frame.Height);

                imageViewLogo.Alpha = 1;
                lblStatus.Alpha = 1;
                activityIndicator.Alpha = 1;
            }, null);
        }

        #region ISplashView implementation
        
        public void RefreshStatus(string message)
        {
            InvokeOnMainThread(() => {
                lblStatus.Text = message;
            });
        }
        
        public void InitDone(bool isAppFirstStart)
        {
        }
        
        #endregion    

    }
}
