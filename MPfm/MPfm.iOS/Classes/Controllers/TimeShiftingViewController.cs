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
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.iOS.Helpers;

namespace MPfm.iOS
{
    public partial class TimeShiftingViewController : BaseViewController, ITimeShiftingView
    {
        public TimeShiftingViewController()
            : base (UserInterfaceIdiomIsPhone ? "TimeShiftingViewController_iPhone" : "TimeShiftingViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;
            btnReset.Layer.CornerRadius = 8;
            btnReset.Layer.BackgroundColor = GlobalTheme.PlayerPanelButtonColor.CGColor;
            btnReset.Alpha = GlobalTheme.PlayerPanelButtonAlpha;
            btnUseTempo.Layer.CornerRadius = 8;
            btnUseTempo.Layer.BackgroundColor = GlobalTheme.PlayerPanelButtonColor.CGColor;
            btnUseTempo.Alpha = GlobalTheme.PlayerPanelButtonAlpha;
            btnDecrementTempo.Layer.CornerRadius = 8;
            btnDecrementTempo.Layer.BackgroundColor = GlobalTheme.PlayerPanelButtonColor.CGColor;
            btnDecrementTempo.Alpha = GlobalTheme.PlayerPanelButtonAlpha;
            btnIncrementTempo.Layer.CornerRadius = 8;
            btnIncrementTempo.Layer.BackgroundColor = GlobalTheme.PlayerPanelButtonColor.CGColor;
            btnIncrementTempo.Alpha = GlobalTheme.PlayerPanelButtonAlpha;

            slider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            slider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            slider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider_gray").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            slider.ValueChanged += HandleSliderValueChanged;

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindTimeShiftingView(this);
        }

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			viewButtons.Frame = new RectangleF((View.Frame.Width - viewButtons.Frame.Width) / 2f, viewButtons.Frame.Y, viewButtons.Frame.Width, viewButtons.Frame.Height);
		}

        void HandleSliderValueChanged(object sender, EventArgs e)
        {
            OnSetTimeShifting(slider.Value);
        }

        partial void actionReset(NSObject sender)
        {
            OnResetTimeShifting();
        }

        partial void actionUseTempo(NSObject sender)
        {
            OnUseDetectedTempo();
        }

        partial void actionIncrementTempo(NSObject sender)
        {
            OnIncrementTempo();
        }

        partial void actionDecrementTempo(NSObject sender)
        {
            OnDecrementTempo();
        }

        #region ITimeShiftingView implementation

        public Action<float> OnSetTimeShifting { get; set; }
        public Action OnResetTimeShifting { get; set; }
        public Action OnUseDetectedTempo { get; set; }
        public Action OnIncrementTempo { get; set; }
        public Action OnDecrementTempo { get; set; }

        public void TimeShiftingError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Time shifting error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshTimeShifting(MPfm.MVP.Models.PlayerTimeShiftingEntity entity)
        {
            InvokeOnMainThread(() => {
                try
                {
                    lblCurrentTempoValue.Text = entity.CurrentTempo;
                    lblReferenceTempoValue.Text = entity.ReferenceTempo;
                    lblDetectedTempoValue.Text = entity.DetectedTempo;
                    slider.SetValue(entity.TimeShiftingValue, false);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("TimeShiftingViewController - RefreshTimeShifting - Exception: {0}", ex);
                }
            });
        }

        #endregion
    }
}
