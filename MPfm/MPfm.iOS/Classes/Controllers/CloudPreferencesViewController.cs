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
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Config.Models;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.iOS.Classes.Controls;

namespace MPfm.iOS
{
    public partial class CloudPreferencesViewController : BaseViewController, ICloudPreferencesView
    {
        public CloudPreferencesViewController()
			: base (UserInterfaceIdiomIsPhone ? "CloudPreferencesViewController_iPhone" : "CloudPreferencesViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            btnLoginDropbox.SetImage(UIImage.FromBundle("Images/Buttons/dropbox"));

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindCloudPreferencesView(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Cloud Preferences", "Menu");
        }

        partial void actionLoginDropbox(NSObject sender)
        {
            OnDropboxLoginLogout();
        }

        #region ICloudPreferencesView implementation

        public Action<CloudAppConfig> OnSetCloudPreferences { get; set; }
        public Action OnDropboxLoginLogout { get; set; }

        public void CloudPreferencesError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("CloudPreferences error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshCloudPreferences(CloudAppConfig config)
        {
        }

        public void RefreshCloudPreferencesState(CloudPreferencesStateEntity entity)
        {
            InvokeOnMainThread(() =>
            {
                btnLoginDropbox.TitleLabel.Text = entity.IsDropboxLinkedToApp ? "Logout from Dropbox" : "Login to Dropbox";
                btnLoginDropbox.UpdateLayout();
            });
        }

        #endregion
    }
}
