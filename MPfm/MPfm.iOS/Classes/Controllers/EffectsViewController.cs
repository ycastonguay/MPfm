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
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;

namespace MPfm.iOS
{
    public partial class EffectsViewController : BaseViewController, IEffectsView
    {
        public EffectsViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "EffectsViewController_iPhone" : "EffectsViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            CAGradientLayer gradient = new CAGradientLayer();
            gradient.Frame = this.View.Bounds;
            gradient.Colors = new MonoTouch.CoreGraphics.CGColor[2] { new CGColor(0.1f, 0.1f, 0.1f, 1), new CGColor(0.4f, 0.4f, 0.4f, 1) }; //[NSArray arrayWithObjects:(id)[[UIColor blackColor] CGColor], (id)[[UIColor whiteColor] CGColor], nil];
            this.View.Layer.InsertSublayer(gradient, 0);

            btnBarDone.Clicked += (sender, e) => {
                this.DismissViewController(true, null);
            };

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
