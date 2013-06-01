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
using System.Linq;
using TinyMessenger;
using TinyMessenger;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.MVP.Messages;
using MPfm.MVP.Services.Interfaces;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Pitch shifting view presenter.
	/// </summary>
	public class PitchShiftingPresenter : BasePresenter<IPitchShiftingView>, IPitchShiftingPresenter
	{
        readonly IPlayerService _playerService;
        readonly ITinyMessengerHub _messengerHub;

        List<Tuple<int, string>> _keys;
        int _referenceKey = 0;
        int _interval = 0;

        public PitchShiftingPresenter(ITinyMessengerHub messengerHub, IPlayerService playerService)
        {
            _messengerHub = messengerHub;
            _playerService = playerService;

            _keys = new List<Tuple<int, string>>(){
                new Tuple<int, string>(1, "C#/Db (minor: A#/Bb)"), 
                new Tuple<int, string>(8, "G#/Ab (minor: F)"), 
                new Tuple<int, string>(3, "D#/Eb (minor: C)"), 
                new Tuple<int, string>(10, "A#/Bb (minor: G)"), 
                new Tuple<int, string>(5, "F (minor: D)"), 
                new Tuple<int, string>(0, "C (minor: A)"), 
                new Tuple<int, string>(7, "G (minor: E)"), 
                new Tuple<int, string>(2, "D (minor: B)"), 
                new Tuple<int, string>(9, "A (minor: F#/Gb)"),
                new Tuple<int, string>(4, "E (minor: C#/Db)"), 
                new Tuple<int, string>(11, "B (minor: G#/Ab)"), 
                new Tuple<int, string>(6, "F#/Gb (minor: D#/Eb)")
            };
		}

        public override void BindView(IPitchShiftingView view)
        {            
            view.OnSetInterval = SetInterval;
            view.OnResetInterval = ResetInterval;
            view.OnChangeKey = ChangeKey;
            view.OnIncrementInterval = IncrementInterval;
            view.OnDecrementInterval = DecrementInterval;

            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                _interval = 0;
                _referenceKey = 0;
                RefreshPitchShiftingView();
            });

            base.BindView(view);

            View.RefreshKeys(_keys);
        }

        private void RefreshPitchShiftingView()
        {
            try
            {
                // ex: referenceKey = 10. interval=+12 k+i = 22.     referencekey - re

                // Remove 12 until < 12.

                // -12 = 0
                // -11 = 1
                // -10 = 2
                // -9 = 3
                // -8 = 4
                // -7 = 5 
                // -6 = 6
                // -5 = 7
                // -4 = 8
                // -3 = 9
                // -2 = 10
                // -1 = 11

                // 12 = 0
                // 13 = 1, etc.
                // ...
                // 24 = 0
                // 25 = 1, etc.

                int newKey = _referenceKey + _interval;

                // Make sure new key value is from 0/+12
                while(newKey > 11)
                    newKey -= 12;
                while(newKey < 0)
                    newKey += 12;

                var tupleReferenceKey = _keys.FirstOrDefault(x => x.Item1 == _referenceKey);
                var tupleNewKey = _keys.FirstOrDefault(x => x.Item1 == newKey);

                Console.WriteLine("PitchShiftingPresenter - RefreshPitchShiftingView");
                View.RefreshPitchShifting(new PlayerPitchShiftingEntity(){
                    Interval = _interval.ToString("+#;-#;0"),
                    IntervalValue = _interval,
                    NewKey = tupleNewKey,
                    ReferenceKey = tupleReferenceKey
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine("PitchShiftingPresenter - RefreshPitchShiftingView - Exception: {0}", ex);
            }
        }

        private void ChangeKey(int key)
        {
            try
            {
                _referenceKey = key;
                RefreshPitchShiftingView();
            }
            catch(Exception ex)
            {
                View.PitchShiftingError(ex);
            }
        }

        private void SetInterval(int interval)
        {
            try
            {
                _interval = interval;
                RefreshPitchShiftingView();
            }
            catch(Exception ex)
            {
                View.PitchShiftingError(ex);
            }
        }

        private void ResetInterval()
        {
            try
            {
                _interval = 0;
                RefreshPitchShiftingView();
            }
            catch(Exception ex)
            {
                View.PitchShiftingError(ex);
            }
        }

        private void IncrementInterval()
        {
            try
            {
                Console.WriteLine("PitchShiftingPresenter - IncrementInterval");
                if(_interval + 1 > 12)
                    return;
                _interval += 1;
                RefreshPitchShiftingView();
            }
            catch(Exception ex)
            {
                View.PitchShiftingError(ex);
            }
        }

        private void DecrementInterval()
        {
            try
            {
                Console.WriteLine("PitchShiftingPresenter - DecrementInterval");
                if(_interval - 1 < -12)
                    return;
                _interval -= 1;
                RefreshPitchShiftingView();
            }
            catch(Exception ex)
            {
                View.PitchShiftingError(ex);
            }
        }
    }
}
