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
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MonoTouch.CoreGraphics;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmSemiTransparentButton")]
    public class MPfmSemiTransparentButton : UIButton
    {
        private bool _isTouchDown = false;

        public Action OnTouchesBegan;
        public Action OnTouchesMoved;
        public Action OnTouchesEnded;

        public MPfmSemiTransparentButton(IntPtr handle) : base(handle)
        {
            // Add custom background to button
            SetTitleColor(UIColor.White, UIControlState.Normal);
            //SetTitleColor(UIColor.DarkGray, UIControlState.Highlighted);
            Layer.CornerRadius = 8;
            Layer.BackgroundColor = new CGColor(0.6f, 0.6f, 0.6f, 1);
            Alpha = 0.8f;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            if (OnTouchesBegan != null)
                OnTouchesBegan();

            UIView.Animate(0.125f, () => {
                Layer.BackgroundColor = new CGColor(0.5f, 0.5f, 0.5f, 1);
                Alpha = 1.0f;
            });

            _isTouchDown = true;
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            if (OnTouchesMoved != null)
                OnTouchesMoved();

            base.TouchesMoved(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (OnTouchesEnded != null)
                OnTouchesEnded();

            UIView.Animate(0.125f, () => {
                Layer.BackgroundColor = new CGColor(0.6f, 0.6f, 0.6f, 1);
                Alpha = 0.8f;
            });

            _isTouchDown = false;
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
        }
    }
}
