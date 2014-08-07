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
    [Register("SessionsPopupView")]
    public class SessionsPopupView : UIView
    {
        private const int Padding = 8;
        private ThemeType _themeType;    
        private UIActivityIndicatorView _activityIndicator;
        private UILabel _label;
        private SessionsSemiTransparentRoundButton _button1;
        private SessionsSemiTransparentRoundButton _button2;

        public string LabelTitle
        {
            get
            {
                return _label.Text;
            }
            set
            {
                _label.Text = value;
            }
        }

        public delegate void ButtonClick();
        public event ButtonClick OnButton1Click;
        public event ButtonClick OnButton2Click;

        public SessionsPopupView() 
            : base()
        {
            Initialize();
        }

        public SessionsPopupView(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        public SessionsPopupView(RectangleF frame)
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            Layer.CornerRadius = 8;
            Layer.BackgroundColor = GlobalTheme.BackgroundDarkerColor.ColorWithAlpha(0.7f).CGColor;
            TintColor = UIColor.White;
            UserInteractionEnabled = true;

            _activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
            _activityIndicator.Hidden = true;
            _activityIndicator.Frame = new RectangleF(0, 0, 22, 22);
            AddSubview(_activityIndicator);

            _label = new UILabel();
            _label.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
            _label.TextColor = UIColor.White;
            _label.Font = UIFont.FromName("HelveticaNeue", 12);
            _label.Lines = 2;
            AddSubview(_label);

            _button1 = new SessionsSemiTransparentRoundButton(new RectangleF(0, 0, 44, 44));
            _button1.Hidden = true;
            _button1.GlyphImageView.Image = UIImage.FromBundle("Images/Player/next");
            _button1.OnButtonClick += HandleButton1TouchUpInside;
            AddSubview(_button1);

            _button2 = new SessionsSemiTransparentRoundButton(new RectangleF(0, 0, 44, 44));
            _button2.Hidden = true;
            _button2.GlyphImageView.Image = UIImage.FromBundle("Images/Player/next");
            _button2.OnButtonClick += HandleButton2TouchUpInside;
            AddSubview(_button2);
        }
            
        public void SetTheme(ThemeType themeType)
        {
            _themeType = themeType;
            SetNeedsLayout();
        }

        private void HandleButton1TouchUpInside()
        {
            if(OnButton1Click != null)
                OnButton1Click();
        }

        private void HandleButton2TouchUpInside()
        {
            if(OnButton2Click != null)
                OnButton2Click();            
        }

        public void AnimateIn()
        {
            AnimateIn(null);
        }

        public void AnimateIn(Action completed)
        {
            Animate(-Frame.Height, Padding, true, false, completed);
        }

        public void AnimateOut()
        {
            AnimateOut(null);
        }

        public void AnimateOut(Action completed)
        {
            Animate(Padding, -Frame.Height, false, true, completed);
        }

        public void Animate(float startY, float endY, bool fadeIn, bool fadeOut, Action completed)
        {
            var frame = this.Frame;
            frame.Y = startY;
            this.Frame = frame;

            if(fadeIn)
                this.Alpha = 0;
            else if(fadeOut)
                this.Alpha = 1;

            UIView.Animate(0.2, () => {
                var frame2 = this.Frame;
                frame2.Y = endY;
                this.Frame = frame2;

                if(fadeIn)
                    this.Alpha = 1;
                else if(fadeOut)
                    this.Alpha = 0;
            }, () => {
                if(completed != null)
                    completed();
            });
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        
            float activityIndicatorSize = 22;
            float buttonSize = 44;

            switch (_themeType)
            {
                case ThemeType.Label:
                    _label.Frame = new RectangleF(Padding, 0, Frame.Width - (Padding * 2), Frame.Height);

                    _activityIndicator.StopAnimating();
                    _activityIndicator.Hidden = true;
                    _button1.Hidden = true;
                    _button2.Hidden = true;
                    break;
                case ThemeType.LabelWithActivityIndicator:
                    float labelX = Padding + activityIndicatorSize + Padding;

                    _activityIndicator.Frame = new RectangleF(Padding, (Frame.Height - activityIndicatorSize) / 2f, activityIndicatorSize, activityIndicatorSize);
                    _label.Frame = new RectangleF(labelX, 0, Frame.Width - labelX - Padding, Frame.Height);

                    _activityIndicator.StartAnimating();
                    _activityIndicator.Hidden = false;
                    _button1.Hidden = true;
                    _button2.Hidden = true;
                    break;
                case ThemeType.LabelWithButtons:
                    float button1X = Frame.Width - buttonSize - Padding;
                    float button2X = Frame.Width - ((buttonSize + Padding) * 2);
                    _button1.Frame = new RectangleF(button1X, (Frame.Height - buttonSize) / 2f, buttonSize, buttonSize);
                    _button2.Frame = new RectangleF(button2X, (Frame.Height - buttonSize) / 2f, buttonSize, buttonSize);
                    _label.Frame = new RectangleF(Padding, 0, Frame.Width - ((buttonSize + Padding) * 2) - Padding, Frame.Height);

                    _activityIndicator.StopAnimating();
                    _activityIndicator.Hidden = true;
                    _button1.Hidden = false;
                    _button2.Hidden = false;
                    break;
            }
        }

        public enum ThemeType
        {
            Label = 0,
            LabelWithActivityIndicator = 1,
            LabelWithButtons = 2
        }
    }
}
