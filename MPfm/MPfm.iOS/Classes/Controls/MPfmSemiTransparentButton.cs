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
using MPfm.iOS.Classes.Objects;
using MPfm.Core;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmSemiTransparentButton")]
    public class MPfmSemiTransparentButton : UIButton
    {
		private bool _isTextLabelAllowedToChangeFrame;
        private bool _isTouchDown;

        public Action OnTouchesBegan;
        public Action OnTouchesMoved;
        public Action OnTouchesEnded;

        public MPfmSemiTransparentButton(IntPtr handle) : base(handle)
        {
            // Add custom background to button
            SetTitleColor(UIColor.White, UIControlState.Normal);
            //SetTitleColor(UIColor.DarkGray, UIControlState.Highlighted);
			TitleLabel.BackgroundColor = UIColor.Clear;
			TitleLabel.TextColor = UIColor.White;
			TitleLabel.Text = CurrentTitle;
			TitleLabel.TextAlignment = UITextAlignment.Center;
			TitleLabel.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            Layer.CornerRadius = 8;
			Layer.BorderWidth = 1f;
			Layer.BorderColor = GlobalTheme.MainLightColor.CGColor;
			Layer.BackgroundColor = GlobalTheme.PlayerPanelButtonColor.CGColor;
            Alpha = 0.8f;
        }

		public override void LayoutSubviews()
		{
			//base.LayoutSubviews();

			//Tracing.Log("SemiTransparentButton - LayoutSubviews - Frame: {0} Bounds: {1} TitleLabel.Frame: {2}", Frame, Bounds, TitleLabel.Frame);

			if(_isTextLabelAllowedToChangeFrame)
				TitleLabel.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
		}

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            if (OnTouchesBegan != null)
                OnTouchesBegan();

			AnimatePress(true);

            _isTouchDown = true;
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            if (OnTouchesMoved != null)
                OnTouchesMoved();

			AnimatePress(false);
            base.TouchesMoved(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if (OnTouchesEnded != null)
                OnTouchesEnded();

			AnimatePress(false);

            _isTouchDown = false;
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
        }

		private void AnimatePress(bool on)
		{
			_isTextLabelAllowedToChangeFrame = !on;

			if (!on)
			{
				UIView.Animate(0.1, () => {
					//BackgroundColor = GlobalTheme.SecondaryColor;
					Alpha = 0.7f;
					//TitleLabel.TextColor = GlobalTheme.LightColor;
					TitleLabel.Transform = CGAffineTransform.MakeScale(1, 1);
				});
			}
			else
			{
				UIView.Animate(0.1, () => {
					//BackgroundColor = GlobalTheme.SecondaryDarkColor;
					Alpha = 1;
					//TitleLabel.TextColor = UIColor.LightGray;
					TitleLabel.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
				});
			}
		}
    }
}
