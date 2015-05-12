// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;

namespace Sessions.iOS.Classes.Controls.Buttons
{
	[Register("SessionsSecondaryMenuButton")]
	public class SessionsSecondaryMenuButton : UIButton
    {
		private bool _isTextLabelAllowedToChangeFrame = true;

        public UIControlContentHorizontalAlignment LabelAlignment { get; set; }

        public delegate void ButtonClick();
        public event ButtonClick OnButtonClick;

		public SessionsSecondaryMenuButton() 
            : base()
        {
            Initialize();
        }

		public SessionsSecondaryMenuButton(RectangleF frame)
			: base(frame)
		{
			Initialize();
		}

		public SessionsSecondaryMenuButton(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            LabelAlignment = UIControlContentHorizontalAlignment.Left;
			BackgroundColor = GlobalTheme.BackgroundColor;
            Layer.CornerRadius = 8;
			Layer.BorderWidth = 1f;
			Layer.BorderColor = GlobalTheme.MainLightColor.CGColor;
            TintColor = UIColor.White;
            SetTitleColor(UIColor.White, UIControlState.Normal);

			TitleLabel.BackgroundColor = UIColor.Clear;
            TitleLabel.TextColor = UIColor.White;
			TitleLabel.HighlightedTextColor = UIColor.White;
			TitleLabel.TextAlignment = UITextAlignment.Center;
            TitleLabel.Font = UIFont.FromName("HelveticaNeue", 14.0f);
            TitleLabel.Frame = Bounds;
            TitleLabel.Text = Title(UIControlState.Normal);

			ImageView.BackgroundColor = UIColor.Clear;

            UpdateLayout();
        }

        public void UpdateLayout()
        {
			if (TitleLabel == null)
				return;

			if (_isTextLabelAllowedToChangeFrame)
			{
				ImageView.Frame = new RectangleF((Bounds.Width - 30f) / 2f, 8, 30, 30);
				TitleLabel.Frame = new RectangleF(0, Bounds.Height - 30, Bounds.Width, 30);
			}
        }

        public void SetImage(UIImage image)
        {
			ImageView.Image = image;
        }

        public override void LayoutSubviews()
        {
            // Do not call base.LayoutSubviews as this will change the position during an animation
			if (ImageView == null || ImageView.Image == null)
                base.LayoutSubviews();

			UpdateLayout();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            AnimatePress(true);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);

            if (OnButtonClick != null)
                OnButtonClick();

            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);
            base.TouchesCancelled(touches, evt);
        }

        private void AnimatePress(bool on)
        {
			_isTextLabelAllowedToChangeFrame = !on;

            if (!on)
            {
                UIView.Animate(0.1, () => {
					BackgroundColor = GlobalTheme.BackgroundColor;
                    TitleLabel.Transform = CGAffineTransform.MakeScale(1, 1);
					ImageView.Transform = CGAffineTransform.MakeScale(1, 1);
                });
            }
            else
            {
                UIView.Animate(0.1, () => {
					BackgroundColor = GlobalTheme.MainColor;
                    TitleLabel.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
					ImageView.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
                });
            }
        }
    }
}
