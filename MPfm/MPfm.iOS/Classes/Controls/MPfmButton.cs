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
        //public UILabel Label { get; private set; }

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
            //BackgroundColor = UIColor.Clear;
            BackgroundColor = GlobalTheme.SecondaryColor;
            Layer.CornerRadius = 8;
            TintColor = UIColor.White;

            TitleLabel.BackgroundColor = UIColor.Clear;
            TitleLabel.TextColor = UIColor.White;
            TitleLabel.TextAlignment = UITextAlignment.Center;
            TitleLabel.Font = UIFont.FromName("HelveticaNeue", 14.0f);

            Image = new UIImageView();
            Image.BackgroundColor = UIColor.Clear;
            Image.Frame = new RectangleF(0, 0, 22, 44);
            Image.Alpha = 0;

            //AddSubview(Label);
            AddSubview(Image);
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
//                    if (LabelAlignment == UIControlContentHorizontalAlignment.Left)
//                        TitleLabel.Frame = new RectangleF(TitleLabel.Frame.X + 8, TitleLabel.Frame.Y, TitleLabel.Frame.Width, TitleLabel.Frame.Height);
//                    else if (LabelAlignment == UIControlContentHorizontalAlignment.Right)
//                        TitleLabel.Frame = new RectangleF(TitleLabel.Frame.X - 4, TitleLabel.Frame.Y, TitleLabel.Frame.Width, TitleLabel.Frame.Height);

                    TitleLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                    Image.Transform = CGAffineTransform.MakeScale(1, 1);
                });
            }
            else
            {
                UIView.Animate(0.1, () => {
                    BackgroundColor = GlobalTheme.SecondaryDarkColor;
                    TitleLabel.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
                    Image.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);

//                    if(LabelAlignment == UIControlContentHorizontalAlignment.Left)
//                        TitleLabel.Frame = new RectangleF(TitleLabel.Frame.X - 8, TitleLabel.Frame.Y, TitleLabel.Frame.Width, TitleLabel.Frame.Height);
//                    else if(LabelAlignment == UIControlContentHorizontalAlignment.Right)
//                        TitleLabel.Frame = new RectangleF(TitleLabel.Frame.X + 4, TitleLabel.Frame.Y, TitleLabel.Frame.Width, TitleLabel.Frame.Height);
                });
            }
        }
    }
}
