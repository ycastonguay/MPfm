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
using MPfm.iOS.Classes.Objects;
using System.Collections.Generic;
using MPfm.MVP.Models;
using System.Linq;

namespace MPfm.iOS
{
    public partial class PitchShiftingViewController : BaseViewController, IPitchShiftingView
    {
        UIActionSheet _actionSheet;
        Tuple<int, string> _currentKey;
        List<Tuple<int, string>> _keys;

        public PitchShiftingViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "PitchShiftingViewController_iPhone" : "PitchShiftingViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            viewBackground.BackgroundColor = GlobalTheme.PlayerPanelBackgroundColor;
            btnReset.Layer.CornerRadius = 8;
            btnReset.Layer.BackgroundColor = GlobalTheme.PlayerPanelButtonColor.CGColor;
            btnReset.Alpha = GlobalTheme.PlayerPanelButtonAlpha;

            slider.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            slider.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            slider.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider_gray").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            slider.ValueChanged += HandleSliderValueChanged;

            base.ViewDidLoad();
        }

        void HandleSliderValueChanged(object sender, EventArgs e)
        {
            OnSetInterval((int)slider.Value);
        }

        partial void actionReset(NSObject sender)
        {
            OnResetInterval();
        }

        partial void actionDecrementInterval(NSObject sender)
        {
            OnDecrementInterval();
        }

        partial void actionIncrementInterval(NSObject sender)
        {
            OnIncrementInterval();
        }

        partial void actionChangeKey(NSObject sender)
        {
            _actionSheet = new UIActionSheet("Title", null, string.Empty, string.Empty, null);

            var pickerView = new UIPickerView(new RectangleF(0, 44, 320, 300));
            pickerView.ShowSelectionIndicator = true;
            pickerView.WeakDelegate = this;

            var toolbar = new UIToolbar(new RectangleF(0, 0, 320, 44));
            toolbar.BarStyle = UIBarStyle.Black;

            UIBarButtonItem flexSpace = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

            var button = new UIButton(UIButtonType.Custom);
            button.SetTitle("Done", UIControlState.Normal);
            button.Layer.CornerRadius = 8;
            button.Layer.BackgroundColor = GlobalTheme.SecondaryColor.CGColor;
            button.Font = UIFont.FromName("HelveticaNeue-Bold", 12.0f);
            button.Frame = new RectangleF(0, 20, 60, 30);
            button.TouchUpInside += (sender2, e) => {
                int key = _keys[pickerView.SelectedRowInComponent(0)].Item1;
                OnChangeKey(key);
                _actionSheet.DismissWithClickedButtonIndex(0, true);
            };
            var btnDone = new UIBarButtonItem(button);
            toolbar.SetItems(new UIBarButtonItem[2] { flexSpace, btnDone }, true);

            _actionSheet.AddSubview(toolbar);
            _actionSheet.AddSubview(pickerView);
            _actionSheet.ShowInView(UIApplication.SharedApplication.KeyWindow);
            _actionSheet.Bounds = new RectangleF(0, 0, 320, 344);

            int index = _keys.IndexOf(_currentKey);
            pickerView.Select(index, 0, false);
        }

        [Export("numberOfComponentsInPickerView:")]
        public int NumberOfComponentsInPickerView(UIPickerView pickerView)
        {
            return 1;
        }

        [Export("pickerView:numberOfRowsInComponent:")]
        public int NumberOfRowsInComponent(UIPickerView pickerView, int component)
        {
            return _keys.Count;
        }

        [Export("pickerView:titleForRow:forComponent:")]
        public string TitleForRow(UIPickerView pickerView, int row, int component)
        {
            return _keys[row].Item2;
        }

        [Export("pickerView:didSelectRow:inComponent:")]
        public void DidSelectRow(UIPickerView pickerView, int row, int component)
        {
            Console.WriteLine("Selected {0}", _keys[row].Item2);
            //lblKey.Text = _keys[row].Item2;
        }

        #region IPitchShiftingView implementation

        public Action<int> OnChangeKey { get; set; }
        public Action<int> OnSetInterval { get; set; }
        public Action OnResetInterval { get; set; }
        public Action OnIncrementInterval { get; set; }
        public Action OnDecrementInterval { get; set; }

        public void PitchShiftingError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Time shifting error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshKeys(List<Tuple<int, string>> keys)
        {
            _keys = keys;
        }

        public void RefreshPitchShifting(PlayerPitchShiftingEntity entity)
        {
            InvokeOnMainThread(() => {
                try
                {
                    _currentKey = entity.ReferenceKey;
                    lblKey.Text = entity.ReferenceKey.Item2;
                    lblNewKey.Text = entity.NewKey.Item2;
                    lblInterval.Text = entity.Interval;
                    slider.Value = entity.IntervalValue;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("PitchShiftingViewController - RefreshPitchShifting - Exception: {0}", ex);
                }
            });
        }

        #endregion
    }
}
