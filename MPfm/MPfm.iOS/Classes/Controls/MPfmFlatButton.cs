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
    [Register("MPfmFlatButton")]
    public class MPfmFlatButton : UIView
    {
        public UIImageView ImageChevron { get; private set; }
        public UILabel Label { get; private set; }

        public delegate void ButtonClick();
        public event ButtonClick OnButtonClick;

        public MPfmFlatButton() 
            : base()
        {
            Initialize();
        }

        public MPfmFlatButton(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            //BackgroundColor = GlobalTheme.BackgroundColor;
            BackgroundColor = UIColor.Clear;

            Label = new UILabel(new RectangleF(26, 5, 80, 32));
            Label.BackgroundColor = UIColor.Clear;
            Label.Text = "Back";
            Label.TextColor = UIColor.FromRGB(182, 213, 233);
            Label.TextAlignment = UITextAlignment.Left;
            Label.Font = UIFont.FromName("HelveticaNeue", 14.0f);

            ImageChevron = new UIImageView(UIImage.FromBundle("Images/Tables/chevron_left_blue"));
            ImageChevron.Frame = new RectangleF(0, 0, 22, 44);

            AddSubview(Label);
            AddSubview(ImageChevron);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("FlatButton - TouchesBegan");

            UIView.Animate(0.2, () => {
                BackgroundColor = GlobalTheme.BackgroundColor;
            });

            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("FlatButton - TouchesEnded");

            UIView.Animate(0.2, () => {
                BackgroundColor = UIColor.Clear;
            });

            if (OnButtonClick != null)
                OnButtonClick();

            base.TouchesEnded(touches, evt);
        }
    }
}
