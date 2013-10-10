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
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmPlayerButton")]
    public class MPfmPlayerButton : UIButton
    {
        public UIImageView GlyphImageView { get; private set; }

        public delegate void ButtonClick();
        public event ButtonClick OnButtonClick;

		public MPfmPlayerButton() 
            : base()
        {
            Initialize();
        }

		public MPfmPlayerButton(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            BackgroundColor = GlobalTheme.BackgroundColor;
            //Layer.CornerRadius = 8;
            TintColor = UIColor.White;

            GlyphImageView = new UIImageView();
            GlyphImageView.BackgroundColor = UIColor.Clear;
            GlyphImageView.Layer.AnchorPoint = new PointF(0.5f, 0.5f);
            //GlyphImageView.Frame = new RectangleF(0, 0, 50, 50);
            GlyphImageView.Frame = new RectangleF((Frame.Width - 50) / 2, (Frame.Height - 50) / 2, 50, 50);
            GlyphImageView.Alpha = 0.7f;

            TitleLabel.Alpha = 0;
            TitleLabel.Hidden = true;
            TitleLabel.Text = string.Empty;

            AddSubview(GlyphImageView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            //GlyphImageView.Frame = new RectangleF((Frame.Width - 50) / 2, (Frame.Height - 50) / 2, 50, 50);
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
            if (!on)
            {
                UIView.Animate(0.1, () => {
                    BackgroundColor = GlobalTheme.BackgroundColor;
                    GlyphImageView.Transform = CGAffineTransform.MakeScale(1, 1);
                    GlyphImageView.Alpha = 0.7f;
                });
            }
            else
            {
                UIView.Animate(0.1, () => {
                    BackgroundColor = GlobalTheme.BackgroundColor;
                    GlyphImageView.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                    GlyphImageView.Alpha = 1f;
                });
            }
        }

        public override void Draw(RectangleF rect)
        {
            var context = UIGraphics.GetCurrentContext();
            var rectCircle = new RectangleF(2, 2, Bounds.Width - 4, Bounds.Height - 4);
            CoreGraphicsHelper.FillRect(context, Bounds, GlobalTheme.BackgroundColor.CGColor);
            CoreGraphicsHelper.DrawEllipsis(context, rectCircle, new CGColor(0.6f, 0.6f, 0.6f, 0.6f), 1f);
        }
    }
}
