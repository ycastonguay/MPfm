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
using MPfm.iOS.Classes.Objects;

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
            var btnDone = new UIButton(UIButtonType.Custom);
            btnDone.SetTitle("Done", UIControlState.Normal);
            btnDone.Layer.CornerRadius = 8;
            btnDone.Layer.BackgroundColor = GlobalTheme.SecondaryColor.CGColor;
            btnDone.Font = UIFont.FromName("HelveticaNeue-Bold", 12.0f);
            btnDone.Frame = new RectangleF(0, 20, 60, 30);
            btnDone.TouchUpInside += (sender, e) => {
                this.DismissViewController(true, null);
            };
            _btnDone = new UIBarButtonItem(btnDone);

            var btnAdd = new UIButton(UIButtonType.Custom);
            btnAdd.SetTitle("+", UIControlState.Normal);
            btnAdd.Layer.CornerRadius = 8;
            btnAdd.Layer.BackgroundColor = GlobalTheme.SecondaryColor.CGColor;
            btnAdd.Font = UIFont.FromName("HelveticaNeue-Bold", 18.0f);
            btnAdd.Frame = new RectangleF(0, 12, 60, 30);
            btnAdd.TouchUpInside += (sender, e) => {
                this.DismissViewController(true, null);
            };
            _btnAdd = new UIBarButtonItem(btnAdd);

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
