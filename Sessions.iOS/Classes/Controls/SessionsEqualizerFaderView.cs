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
    [Register("SessionsEqualizerFaderView")]
    public class SessionsEqualizerFaderView : UIView
    {
        UISlider _slider;
        UILabel _lblValue;
        UILabel _lblFrequency;

        public delegate void ValueChangedEventHandler(object sender, SessionsEqualizerFaderValueChangedEventArgs e);

        public event ValueChangedEventHandler ValueChanged;

        public string Frequency
        {
            get
            {
                return _lblFrequency.Text;
            }
        }

        public SessionsEqualizerFaderView() 
            : base()
        {
            Initialize();
        }

        public SessionsEqualizerFaderView(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
			BackgroundColor = GlobalTheme.BackgroundColor;

            _lblFrequency = new UILabel(new RectangleF(12, 4, 60, 36));
            _lblFrequency.BackgroundColor = UIColor.Clear;
            _lblFrequency.TextColor = UIColor.White;
            _lblFrequency.Font = UIFont.FromName("HelveticaNeue-Light", 12.0f);

            _lblValue = new UILabel(new RectangleF(UIScreen.MainScreen.Bounds.Width - 60 - 14, 4, 60, 36));
            _lblValue.BackgroundColor = UIColor.Clear;
            _lblValue.Text = "0.0 dB";
            _lblValue.TextColor = UIColor.White;
            _lblValue.TextAlignment = UITextAlignment.Right;
            _lblValue.Font = UIFont.FromName("HelveticaNeue-Light", 12.0f);

            _slider = new UISlider(new RectangleF(62, 4, UIScreen.MainScreen.Bounds.Width - 120 - 14, 36));
            _slider.MinValue = -6;
            _slider.MaxValue = 6;
            _slider.Value = 0;
            _slider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            _slider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            _slider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            _slider.ValueChanged += HandleSliderValueChanged;

            AddSubview(_lblFrequency);
            AddSubview(_lblValue);
            AddSubview(_slider);
        }

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var screenSize = UIKitHelper.GetDeviceSize();
			_lblFrequency.Frame = new RectangleF(12, 4, 60, 36);
			_lblValue.Frame = new RectangleF(screenSize.Width - 60 - 14, 4, 60, 36);
			_slider.Frame = new RectangleF(62, 4, screenSize.Width - 120 - 14, 36);
		}

        protected virtual void OnValueChanged(SessionsEqualizerFaderValueChangedEventArgs e)
        {
            if(ValueChanged != null)
                ValueChanged(this, e);
        }

        private void HandleSliderValueChanged(object sender, EventArgs e)
        {
            if(_slider.Value > 0)
                _lblValue.Text = "+" + _slider.Value.ToString("0.0").Replace(",",".") + " dB";
            else
                _lblValue.Text = _slider.Value.ToString("0.0").Replace(",",".") + " dB";

            OnValueChanged(new SessionsEqualizerFaderValueChangedEventArgs(){
                Value = _slider.Value
            });
        }

        public void SetValue(string frequency, float value)
        {
            _lblFrequency.Text = frequency;
            _slider.Value = value;
            if(_slider.Value > 0)
                _lblValue.Text = "+" + _slider.Value.ToString("0.0").Replace(",",".") + " dB";
            else
                _lblValue.Text = _slider.Value.ToString("0.0").Replace(",",".") + " dB";

        }
    }
}
