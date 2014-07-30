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
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Sessions.Android.Classes.Fragments.Base;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using Sessions.Player.Objects;

namespace Sessions.Android.Classes.Fragments
{
    public class TimeShiftingFragment : BaseFragment, ITimeShiftingView
    {        
        private View _view;
        private TextView _lblCurrentTempoValue;
        private TextView _lblReferenceTempoValue;
        private TextView _lblDetectedTempoValue; 
        private SeekBar _seekBar;
        private Button _btnReset;
        private Button _btnIncrement;
        private Button _btnDecrement;

        // Leave an empty constructor or the application will crash at runtime
        public TimeShiftingFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.TimeShifting, container, false);
            _lblCurrentTempoValue = _view.FindViewById<TextView>(Resource.Id.timeShifting_lblCurrentTempoValue);
            _lblReferenceTempoValue = _view.FindViewById<TextView>(Resource.Id.timeShifting_lblReferenceTempoValue);
            _lblDetectedTempoValue = _view.FindViewById<TextView>(Resource.Id.timeShifting_lblDetectedTempoValue);
            _seekBar = _view.FindViewById<SeekBar>(Resource.Id.timeShifting_seekBar);
            _btnReset = _view.FindViewById<Button>(Resource.Id.timeShifting_btnReset);
            _btnIncrement = _view.FindViewById<Button>(Resource.Id.timeShifting_btnIncrement);
            _btnDecrement = _view.FindViewById<Button>(Resource.Id.timeShifting_btnDecrement);

            _btnReset.Click += (sender, args) => OnResetTimeShifting();
            _btnIncrement.Click += (sender, args) => OnIncrementTempo();
            _btnDecrement.Click += (sender, args) => OnDecrementTempo();

            _seekBar.ProgressChanged += SeekBarOnProgressChanged;

            return _view;
        }

        public override void OnResume()
        {
            base.OnResume();
            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindTimeShiftingView(this);
        }

        private void SeekBarOnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs progressChangedEventArgs)
        {            
            // Time shifting range: 50% to 150%. Seek bar range: 0-1000
            float timeShiftingValue = (((float)_seekBar.Progress) / 10f) + 50f;
            //Console.WriteLine("SeekBarProgressChanged progress: {0} timeShiftingValue: {1}", _seekBar.Progress, timeShiftingValue);
            OnSetTimeShifting(timeShiftingValue);
        }

        #region ITimeShiftingView implementation

        public Action<float> OnSetTimeShifting { get; set; }
        public Action OnResetTimeShifting { get; set; }
        public Action OnUseDetectedTempo { get; set; }
        public Action OnIncrementTempo { get; set; }
        public Action OnDecrementTempo { get; set; }

        public void TimeShiftingError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshTimeShifting(PlayerTimeShifting entity)
        {
            try
            {
                Activity.RunOnUiThread(() => {
                    _lblCurrentTempoValue.Text = entity.CurrentTempo;
                    _lblReferenceTempoValue.Text = entity.ReferenceTempo;
                    _lblDetectedTempoValue.Text = entity.DetectedTempo;

                    // The seekbar in Android doesn't have a minimum value and doesn't support floats. Lazy Google! 
                    // Time shifting range: 50% to 150%. Seek bar range: 0-1000
                    int seekBarProgress = (int)Math.Ceiling(entity.TimeShiftingValue * 10f - 500f);
                    //Console.WriteLine("TimeShiftingFragment - RefreshTimeShifting - timeShiftingValue: {0} seekBarProgress: {1}", entity.TimeShiftingValue, seekBarProgress);
                    _seekBar.Progress = seekBarProgress;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("TimeShiftingFragment - RefreshTimeShifting - Exception: {0}", ex);
            }
        }   

        #endregion

    }
}
