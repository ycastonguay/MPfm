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
using CoreGraphics;
using CoreGraphics;
using Foundation;
using UIKit;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;
using Sessions.iOS.Classes.Controls.Buttons;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsPopupView")]
    public class SessionsPopupView : UIView
    {
        private const int Padding = 8;
        private ThemeType _themeType;    
        private UIActivityIndicatorView _activityIndicator;
        private UILabel _label;
        private SessionsSemiTransparentRoundButton _button;

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
        public event ButtonClick OnButtonClick;

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

        public SessionsPopupView(CGRect frame)
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
            _activityIndicator.Frame = new CGRect(0, 0, 22, 22);
            AddSubview(_activityIndicator);

            _label = new UILabel();
            _label.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
            _label.TextColor = UIColor.White;
            _label.Font = UIFont.FromName("HelveticaNeue", 12);
            _label.Lines = 2;
            AddSubview(_label);

            _button = new SessionsSemiTransparentRoundButton(new CGRect(0, 0, 44, 44));
            _button.Hidden = true;
            _button.GlyphImageView.Image = UIImage.FromBundle("Images/Player/more");
            _button.OnButtonClick += HandleButton1TouchUpInside;
            AddSubview(_button);
        }
            
        public void SetTheme(ThemeType themeType)
        {
            _themeType = themeType;
            SetNeedsLayout();
        }

        private void HandleButton1TouchUpInside()
        {
            if(OnButtonClick != null)
                OnButtonClick();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            // Dismiss
            AnimateOut();
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

        public void Animate(nfloat startY, nfloat endY, bool fadeIn, bool fadeOut, Action completed)
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
                    _label.Frame = new CGRect(Padding, 0, Frame.Width - (Padding * 2), Frame.Height);

                    _activityIndicator.StopAnimating();
                    _activityIndicator.Hidden = true;
                    _button.Hidden = true;
                    break;
                case ThemeType.LabelWithActivityIndicator:
                    float labelX = Padding + activityIndicatorSize + Padding;

                    _activityIndicator.Frame = new CGRect(Padding, (Frame.Height - activityIndicatorSize) / 2f, activityIndicatorSize, activityIndicatorSize);
                    _label.Frame = new CGRect(labelX, 0, Frame.Width - labelX - Padding, Frame.Height);

                    _activityIndicator.StartAnimating();
                    _activityIndicator.Hidden = false;
                    _button.Hidden = true;
                    break;
                case ThemeType.LabelWithButtons:
                    nfloat button1X = Frame.Width - buttonSize - Padding;
                    //float button2X = Frame.Width - ((buttonSize + Padding) * 2);
                    _button.Frame = new CGRect(button1X, (Frame.Height - buttonSize) / 2f, buttonSize, buttonSize);
                    _label.Frame = new CGRect(Padding, 0, Frame.Width - buttonSize - Padding - Padding, Frame.Height);

                    _activityIndicator.StopAnimating();
                    _activityIndicator.Hidden = true;
                    _button.Hidden = false;
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
