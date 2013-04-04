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
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;

namespace MPfm.iOS
{
    public partial class EffectsViewController : BaseViewController, IEffectsView
    {
        UIBarButtonItem _btnAdd;
        UIBarButtonItem _btnDone;         

        public EffectsViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "EffectsViewController_iPhone" : "EffectsViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            // Add navigation controller buttons
            _btnDone = new UIBarButtonItem(UIBarButtonSystemItem.Done);
            _btnDone.Clicked += (sender, e) => {
                this.DismissViewController(true, null);
            };
            _btnAdd = new UIBarButtonItem(UIBarButtonSystemItem.Add);

            NavigationItem.SetLeftBarButtonItem(_btnDone, true);
            NavigationItem.SetRightBarButtonItem(_btnAdd, true);

            var navCtrl = (MPfmNavigationController)NavigationController;
            navCtrl.SetBackButtonVisible(false);
            navCtrl.SetTitle("Effects", "Equalizer Presets");

            base.ViewDidLoad();
        }

        #region IEffectsView implementation

        public void UpdateFader(int index, float value)
        {
        }

        public void UpdatePresetList(System.Collections.Generic.IEnumerable<string> presets)
        {
        }

        #endregion
    }
}
