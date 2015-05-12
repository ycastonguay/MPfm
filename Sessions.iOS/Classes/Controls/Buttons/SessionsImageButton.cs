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

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsImageButton")]
    public class SessionsImageButton : UIButton
    {
        public delegate void ButtonClick();
        public event ButtonClick OnButtonClick;

        public SessionsImageButton() 
            : base()
        {
            Initialize();
        }

        public SessionsImageButton(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        public SessionsImageButton(RectangleF frame)
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
            Console.WriteLine("SessionsImageButton - TouchesBegan");
            AnimatePress(true);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("SessionsImageButton - TouchesEnded");
            AnimatePress(false);

            if (OnButtonClick != null)
                OnButtonClick();

            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("SessionsImageButton - TouchesCancelled");
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
