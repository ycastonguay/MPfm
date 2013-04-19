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
        public MPfmEqualizerFaderView(string frequency) 
            : base()
        {
            Initialize(frequency);
        }

        public MPfmEqualizerFaderView(IntPtr handle) 
            : base(handle)
        {
            Initialize("14.4 kHz");
        }

        private void Initialize(string frequency)
        {
            BackgroundColor = GlobalTheme.BackgroundColor;

            var lblFrequency = new UILabel(new RectangleF(12, 4, 60, 36));
            lblFrequency.BackgroundColor = UIColor.Clear;
            lblFrequency.Text = frequency;
            lblFrequency.TextColor = UIColor.White;
            lblFrequency.Font = UIFont.FromName("HelveticaNeue", 12.0f);

            var lblValue = new UILabel(new RectangleF(256, 4, 60, 36));
            lblValue.BackgroundColor = UIColor.Clear;
            lblValue.Text = "+6.0 dB";
            lblValue.TextColor = UIColor.White;
            lblValue.Font = UIFont.FromName("HelveticaNeue", 12.0f);

            var slider = new UISlider(new RectangleF(62, 4, 186, 36));
            slider.MinValue = -6;
            slider.MaxValue = 6;
            slider.Value = 0;
            slider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            slider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").StretchableImage(8, 0), UIControlState.Normal);
            slider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").StretchableImage(8, 0), UIControlState.Normal);

            AddSubview(lblFrequency);
            AddSubview(lblValue);
            AddSubview(slider);
        }
    }
}
