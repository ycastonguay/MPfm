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
    [Register("SessionsFlatButton")]
    public class SessionsFlatButton : UIView
    {
        public UIImageView ImageChevron { get; private set; }
        public UILabel Label { get; private set; }

        public UIControlContentHorizontalAlignment LabelAlignment { get; set; }

        public delegate void ButtonClick();
        public event ButtonClick OnButtonClick;

        public SessionsFlatButton() 
            : base()
        {
            Initialize();
        }

        public SessionsFlatButton(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            LabelAlignment = UIControlContentHorizontalAlignment.Left;
            BackgroundColor = UIColor.Clear;

            Label = new UILabel(new RectangleF(26, 5, 80, 32));
            Label.BackgroundColor = UIColor.Clear;
            Label.Text = "Back";
            Label.TextColor = UIColor.FromRGB(182, 213, 233);
            Label.TextAlignment = UITextAlignment.Left;
            Label.Font = UIFont.FromName("HelveticaNeue", 14.0f);

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron_left_blue"));
            ImageChevron.BackgroundColor = UIColor.Clear;
            ImageChevron.Frame = new RectangleF(0, 0, 22, 44);

            AddSubview(Label);
            AddSubview(ImageChevron);
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
                    BackgroundColor = UIColor.Clear;
                    if (LabelAlignment == UIControlContentHorizontalAlignment.Left)
                        Label.Frame = new RectangleF(Label.Frame.X + 8, Label.Frame.Y, Label.Frame.Width, Label.Frame.Height);
                    else if (LabelAlignment == UIControlContentHorizontalAlignment.Right)
                        Label.Frame = new RectangleF(Label.Frame.X - 4, Label.Frame.Y, Label.Frame.Width, Label.Frame.Height);

                    Label.Transform = CGAffineTransform.MakeScale(1, 1);
                    ImageChevron.Transform = CGAffineTransform.MakeScale(1, 1);
                });
            }
            else
            {
                UIView.Animate(0.1, () => {
                    BackgroundColor = GlobalTheme.BackgroundColor;
                    Label.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                    ImageChevron.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);

                    if(LabelAlignment == UIControlContentHorizontalAlignment.Left)
                        Label.Frame = new RectangleF(Label.Frame.X - 8, Label.Frame.Y, Label.Frame.Width, Label.Frame.Height);
                    else if(LabelAlignment == UIControlContentHorizontalAlignment.Right)
                        Label.Frame = new RectangleF(Label.Frame.X + 4, Label.Frame.Y, Label.Frame.Width, Label.Frame.Height);
                });
            }
        }
    }
}
