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
using MPfm.MVP.Views;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Controllers.Base;

namespace MPfm.iOS
{
    public partial class AboutViewController : BaseViewController, IAboutView
    {
        public AboutViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "AboutViewController_iPhone" : "AboutViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("About Sessions", "");
        }
    }
}
