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
    [Register("MPfmButton")]
    public class MPfmButton : UIButton
    {
        public UIImageView Image { get; private set; }
        public UIControlContentHorizontalAlignment LabelAlignment { get; set; }

        public delegate void ButtonClick();
        public event ButtonClick OnButtonClick;

		public MPfmButton() 
            : base()
        {
            Initialize();
        }

		public MPfmButton(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            LabelAlignment = UIControlContentHorizontalAlignment.Left;
            BackgroundColor = GlobalTheme.SecondaryColor;
            Layer.CornerRadius = 8;
            TintColor = UIColor.White;

            TitleLabel.BackgroundColor = UIColor.Clear;
            TitleLabel.TextColor = UIColor.White;
            TitleLabel.TextAlignment = UITextAlignment.Left;
            TitleLabel.Font = UIFont.FromName("HelveticaNeue", 14.0f);
            TitleLabel.Frame = Bounds;
            TitleLabel.Text = Title(UIControlState.Normal);

            Image = new UIImageView();
            Image.BackgroundColor = UIColor.Clear;
            Image.Frame = new RectangleF(9, 9, 26, 26);
            AddSubview(Image);

            UpdateLayout();
        }

        private void UpdateLayout()
        {
            float padding = 10;
            string title = Title(UIControlState.Normal);

            UIGraphics.BeginImageContextWithOptions(Bounds.Size, true, 0);
            var context = UIGraphics.GetCurrentContext();
            if (context == null)
            {   // When added to a toolbar, the button returns a null context. Take full width for text and center horizontally.
                TitleLabel.Frame = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
                TitleLabel.TextAlignment = UITextAlignment.Center;
                return;
            }
            else
            {
                TitleLabel.TextAlignment = UITextAlignment.Left;
            }

            float width = CoreGraphicsHelper.MeasureStringWidth(context, title, TitleLabel.Font.Name, TitleLabel.Font.PointSize);
            UIGraphics.EndImageContext();

            float totalWidth = width;
            if (Image.Image != null)
                totalWidth += Image.Bounds.Width + padding;
            float imageX = (Bounds.Width - totalWidth) / 2;
            float textX = Image.Image == null ? imageX : imageX + Image.Bounds.Width + padding;

            Image.Frame = new RectangleF(imageX, 10, 24, 24);
            TitleLabel.Frame = new RectangleF(textX, 0, width, Bounds.Height);

            //Console.WriteLine("MPfmButton - UpdateLayout - title: {0} width: {1} totalWidth: {2} x: {3}", TitleLabel.Text, width, totalWidth, x);
        }

        public override void SetTitle(string title, UIControlState forState)
        {
            base.SetTitle(title, forState);
            UpdateLayout();
        }

        public void SetImage(UIImage image)
        {
            Image.Image = image;
            UpdateLayout();
        }

        public override void LayoutSubviews()
        {
            // Do not call base.LayoutSubviews as this will change the position during an animation
            if (Image == null || Image.Image == null)
                base.LayoutSubviews();
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
                    BackgroundColor = GlobalTheme.SecondaryColor;
                    TitleLabel.TextColor = GlobalTheme.LightColor;
                    TitleLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    Image.Transform = CGAffineTransform.MakeScale(1, 1);
                });
            }
            else
            {
                UIView.Animate(0.1, () => {
                    BackgroundColor = GlobalTheme.SecondaryDarkColor;
                    TitleLabel.TextColor = GlobalTheme.SecondaryColor;
                    TitleLabel.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
                    Image.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
                });
            }
        }
    }
}
