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
using MPfm.MVP.Config.Models;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;

namespace MPfm.iOS
{
    public partial class LibraryPreferencesViewController : BaseViewController, ILibraryPreferencesView
    {
        public LibraryPreferencesViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "LibraryPreferencesViewController_iPhone" : "LibraryPreferencesViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            NavigationController.InteractivePopGestureRecognizer.WeakDelegate = this;
            NavigationController.InteractivePopGestureRecognizer.Enabled = true;

            btnResetLibrary.SetImage(UIImage.FromBundle("Images/Buttons/reset"));
            btnUpdateLibrary.SetImage(UIImage.FromBundle("Images/Buttons/refresh"));

            base.ViewDidLoad();
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Library Preferences", "Menu");
        }

        partial void actionResetLibrary(NSObject sender)
        {
            var alertView = new UIAlertView("Reset Library", "Are you sure you wish to reset your library?", null, "OK", new string[1]{"Cancel"});
            alertView.Clicked += (sender2, e) => {
                if(e.ButtonIndex == 0)
                    OnResetLibrary();
            };
            alertView.Show();
        }

        partial void actionUpdateLibrary(NSObject sender)
        {
            OnUpdateLibrary();
        }

        #region ILibraryPreferencesView implementation

        public Action<LibraryAppConfig> OnSetLibraryPreferences { get; set; }
        public Action OnResetLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }
        public Action OnSelectFolders { get; set; }
        public Action OnEnableSyncListener { get; set; }
        public Action<int> OnSetSyncListenerPort { get; set; }

        public void LibraryPreferencesError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Library Preferences error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshLibraryPreferences(LibraryAppConfig config)
        {
        }

        #endregion
    }
}
