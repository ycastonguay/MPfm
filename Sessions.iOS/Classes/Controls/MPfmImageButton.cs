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
    [Register("MPfmImageButton")]
    public class MPfmImageButton : UIButton
    {
        public delegate void ButtonClick();
        public event ButtonClick OnButtonClick;

        public MPfmImageButton() 
            : base()
        {
            Initialize();
        }

        public MPfmImageButton(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        public MPfmImageButton(RectangleF frame)
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            BackgroundColor = UIColor.FromRGBA(80, 80, 80, 225);
            Layer.CornerRadius = 4;
            TintColor = UIColor.White;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("MPfmImageButton - TouchesBegan");
            AnimatePress(true);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("MPfmImageButton - TouchesEnded");
            AnimatePress(false);

            if (OnButtonClick != null)
                OnButtonClick();

            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("MPfmImageButton - TouchesCancelled");
            AnimatePress(false);
            base.TouchesCancelled(touches, evt);
        }

        private void AnimatePress(bool on)
        {
            if (!on)
            {
                UIView.Animate(0.1, () => {
                    BackgroundColor = UIColor.FromRGBA(80, 80, 80, 225);
                    //ImageView.Transform = CGAffineTransform.MakeScale(1, 1);
                });
            }
            else
            {
                UIView.Animate(0.1, () => {
                    BackgroundColor = UIColor.FromRGBA(50, 50, 50, 255);
                    //ImageView.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
                });
            }
        }
    }
}
