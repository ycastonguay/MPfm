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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Views;
using MPfm.MVP.Presenters;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS
{
    public partial class TimeShiftingViewController : BaseViewController, ITimeShiftingView
    {
        public TimeShiftingViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "TimeShiftingViewController_iPhone" : "TimeShiftingViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;
            btnReset.Layer.CornerRadius = 8;
            btnReset.Layer.BackgroundColor = GlobalTheme.PlayerPanelButtonColor.CGColor;
            btnReset.Alpha = GlobalTheme.PlayerPanelButtonAlpha;
            btnDetectTempo.Layer.CornerRadius = 8;
            btnDetectTempo.Layer.BackgroundColor = GlobalTheme.PlayerPanelButtonColor.CGColor;
            btnDetectTempo.Alpha = 0.8f;

            slider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            slider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").StretchableImage(8, 0), UIControlState.Normal);
            slider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").StretchableImage(8, 0), UIControlState.Normal);

            // Use Appearance API (iOS 5+) for segmented control
            UITextAttributes attr = new UITextAttributes();
            attr.Font = UIFont.FromName("HelveticaNeue-Bold", 12);
            attr.TextColor = UIColor.White;
            segmentedControl.SetTitleTextAttributes(attr, UIControlState.Normal);
            segmentedControl.TintColor = GlobalTheme.PlayerPanelButtonColor;
            segmentedControl.Alpha = GlobalTheme.PlayerPanelButtonAlpha;

            slider.ValueChanged += HandleSliderValueChanged;

            base.ViewDidLoad();
        }

        void HandleSliderValueChanged(object sender, EventArgs e)
        {
            OnSetTimeShifting(slider.Value);
        }

        partial void actionDetectTempo(NSObject sender)
        {
            OnDetectTempo();
        }

        partial void actionReset(NSObject sender)
        {
            OnResetTimeShifting();
        }

        partial void actionSegmentChanged(NSObject sender)
        {
            TimeShiftingMode mode = (TimeShiftingMode)segmentedControl.SelectedSegment;
            switch(mode)
            {
                case TimeShiftingMode.Percentage:
                    btnDetectTempo.Hidden = true;
                    lblOriginalTempo.Hidden = true;
                    break;
                case TimeShiftingMode.Tempo:
                    btnDetectTempo.Hidden = false;
                    lblOriginalTempo.Hidden = false;
                    break;
            }
            OnSetTimeShiftingMode(mode);
        }

        #region ITimeShiftingView implementation

        public Action<float> OnSetTimeShifting { get; set; }
        public Action<TimeShiftingMode> OnSetTimeShiftingMode { get; set; }
        public Action OnResetTimeShifting { get; set; }
        public Action OnDetectTempo { get; set; }

        public void RefreshTimeShifting(MPfm.MVP.Models.PlayerTimeShiftingEntity entity)
        {
            InvokeOnMainThread(() => {
                lblTempo.Text = entity.TimeShiftingString;
                slider.Value = entity.TimeShifting;
            });
        }

        public void TimeShiftingError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Time shifting error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        #endregion
    }
}
