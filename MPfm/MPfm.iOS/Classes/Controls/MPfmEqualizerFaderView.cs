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
    [Register("MPfmEqualizerFaderView")]
    public class MPfmEqualizerFaderView : UIView
    {
        UISlider _slider;
        UILabel _lblValue;
        UILabel _lblFrequency;

        public MPfmEqualizerFaderView() 
            : base()
        {
            Initialize();
        }

        public MPfmEqualizerFaderView(IntPtr handle) 
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
            _lblFrequency.Font = UIFont.FromName("HelveticaNeue", 12.0f);

            _lblValue = new UILabel(new RectangleF(256, 4, 60, 36));
            _lblValue.BackgroundColor = UIColor.Clear;
            _lblValue.Text = "+6.0 dB";
            _lblValue.TextColor = UIColor.White;
            _lblValue.Font = UIFont.FromName("HelveticaNeue", 12.0f);

            _slider = new UISlider(new RectangleF(62, 4, 186, 36));
            _slider.MinValue = -6;
            _slider.MaxValue = 6;
            _slider.Value = 0;
            _slider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            _slider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").StretchableImage(8, 0), UIControlState.Normal);
            _slider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").StretchableImage(8, 0), UIControlState.Normal);

            AddSubview(_lblFrequency);
            AddSubview(_lblValue);
            AddSubview(_slider);
        }

        public void SetValue(string frequency, float value)
        {
            _lblFrequency.Text = frequency;
            _slider.Value = value;
        }
    }
}