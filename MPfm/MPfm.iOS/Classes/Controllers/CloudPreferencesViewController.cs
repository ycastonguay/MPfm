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

namespace MPfm.iOS
{
    public partial class CloudPreferencesViewController : BaseViewController, ICloudPreferencesView
    {
        public CloudPreferencesViewController(Action<IBaseView> onViewReady)
			: base (onViewReady, UserInterfaceIdiomIsPhone ? "CloudPreferencesViewController_iPhone" : "CloudPreferencesViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        #region ICloudPreferencesView implementation

        public Action<CloudPreferencesEntity> OnSetCloudPreferences { get; set; }
        public Action OnDropboxLoginLogout { get; set; }

        public void CloudPreferencesError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("CloudPreferences error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshCloudPreferences(MPfm.MVP.Models.CloudPreferencesEntity entity)
        {
        }

        public void RefreshCloudPreferencesState(MPfm.MVP.Models.CloudPreferencesStateEntity entity)
        {
        }

        #endregion
    }
}
