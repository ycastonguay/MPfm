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
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Models;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class PitchShiftingFragment : BaseFragment, IPitchShiftingView
    {        
        private View _view;
        private TextView _lblCurrentKeyValue;
        private TextView _lblReferenceKeyValue;
        private TextView _lblNewKeyValue;
        private SeekBar _seekBar;
        private Button _btnReset;
        private Button _btnIncrement;
        private Button _btnDecrement;
        private Tuple<int, string> _currentKey;

        // Leave an empty constructor or the application will crash at runtime
        public PitchShiftingFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.PitchShifting, container, false);
            _lblCurrentKeyValue = _view.FindViewById<TextView>(Resource.Id.pitchShifting_lblCurrentIntervalValue);
            _lblReferenceKeyValue = _view.FindViewById<TextView>(Resource.Id.pitchShifting_lblReferenceKeyValue);
            _lblNewKeyValue = _view.FindViewById<TextView>(Resource.Id.pitchShifting_lblNewKeyValue);
            _seekBar = _view.FindViewById<SeekBar>(Resource.Id.pitchShifting_seekBar);
            _btnReset = _view.FindViewById<Button>(Resource.Id.pitchShifting_btnReset);
            _btnIncrement = _view.FindViewById<Button>(Resource.Id.pitchShifting_btnIncrement);
            _btnDecrement = _view.FindViewById<Button>(Resource.Id.pitchShifting_btnDecrement);

            _btnReset.Click += (sender, args) => OnResetInterval();
            _btnIncrement.Click += (sender, args) => OnIncrementInterval();
            _btnDecrement.Click += (sender, args) => OnDecrementInterval();

            _seekBar.ProgressChanged += SeekBarOnProgressChanged;
            return _view;
        }

        private void SeekBarOnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            // Pitch shifting range: -12 to +12. Seek bar range: 0-23
            int interval = _seekBar.Progress - 12;
            //Console.WriteLine("SeekBarProgressChanged progress: {0} interval: {1}", _seekBar.Progress, interval);
            OnSetInterval(interval);
        }

        #region IPitchShiftingView implementation

        public Action<int> OnChangeKey { get; set; }
        public Action<int> OnSetInterval { get; set; }
        public Action OnResetInterval { get; set; }
        public Action OnIncrementInterval { get; set; }
        public Action OnDecrementInterval { get; set; }

        public void PitchShiftingError(Exception ex)
        {
            Activity.RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(Activity).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in PitchShifting: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        public void RefreshKeys(List<Tuple<int, string>> keys)
        {
        }

        public void RefreshPitchShifting(PlayerPitchShiftingEntity entity)
        {
            try
            {
                Activity.RunOnUiThread(() => {
                    _currentKey = entity.ReferenceKey;
                    _lblReferenceKeyValue.Text = entity.ReferenceKey.Item2;
                    _lblNewKeyValue.Text = entity.NewKey.Item2;
                    _lblCurrentKeyValue.Text = entity.Interval;

                    // Pitch shifting range: -12 to +12. Seek bar range: 0-23
                    int seekBarProgress = entity.IntervalValue + 12;
                    //Console.WriteLine("PitchShiftingFragment - RefreshPitchShifting - interval: {0} seekBarProgress: {1}", entity.IntervalValue, seekBarProgress);
                    _seekBar.Progress = seekBarProgress;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("PitchShiftingFragment - RefreshTimeShifting - Exception: {0}", ex);
            }
        }

        #endregion
    }
}
