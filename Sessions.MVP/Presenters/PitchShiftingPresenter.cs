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
using System.Collections.Generic;
using System.Linq;
using Sessions.Core;
using Sessions.Player.Objects;
using TinyMessenger;
using Sessions.MVP.Messages;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using org.sessionsapp.player;

namespace Sessions.MVP.Presenters
{
	/// <summary>
	/// Pitch shifting view presenter.
	/// </summary>
	public class PitchShiftingPresenter : BasePresenter<IPitchShiftingView>, IPitchShiftingPresenter
	{
        readonly IPlayerService _playerService;
        readonly ITinyMessengerHub _messengerHub;
	    private List<TinyMessageSubscriptionToken> _tokens = new List<TinyMessageSubscriptionToken>();

        List<Tuple<int, string>> _keys;
        int _referenceKey = 0;
        int _interval = 0;

	    public PitchShiftingPresenter(ITinyMessengerHub messengerHub, IPlayerService playerService)
        {
            _messengerHub = messengerHub;
            _playerService = playerService;

            _keys = new List<Tuple<int, string>>(){
                new Tuple<int, string>(1, "C#/Db (A#m/Bbm)"), 
                new Tuple<int, string>(8, "G#/Ab (Fm)"), 
                new Tuple<int, string>(3, "D#/Eb (Cm)"), 
                new Tuple<int, string>(10, "A#/Bb (Gm)"), 
                new Tuple<int, string>(5, "F (Dm)"), 
                new Tuple<int, string>(0, "C (Am)"), 
                new Tuple<int, string>(7, "G (Em)"), 
                new Tuple<int, string>(2, "D (Bm)"), 
                new Tuple<int, string>(9, "A (F#m/Gbm)"),
                new Tuple<int, string>(4, "E (C#m/Dbm)"), 
                new Tuple<int, string>(11, "B (G#m/Abm)"), 
                new Tuple<int, string>(6, "F#/Gb (D#m/Ebm)")
            };
		}

        public override void BindView(IPitchShiftingView view)
        {            
            view.OnSetInterval = SetInterval;
            view.OnResetInterval = ResetInterval;
            view.OnChangeKey = ChangeKey;
            view.OnIncrementInterval = IncrementInterval;
            view.OnDecrementInterval = DecrementInterval;

            base.BindView(view);

            _tokens.Add(_messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                _interval = 0;
                _referenceKey = 0;
                RefreshPitchShiftingView();
            }));

            // Refresh view with initial data
            _interval = _playerService.PitchShifting;
            View.RefreshKeys(_keys);
            RefreshPitchShiftingView();
        }

        public override void ViewDestroyed()
        {
            foreach (TinyMessageSubscriptionToken token in _tokens)
                token.Dispose();

            base.ViewDestroyed();
        }

	    private void RefreshPitchShiftingView()
        {
            try
            {
                // Make sure new key value is from 0/+12
                int newKey = _referenceKey + _interval;
                while(newKey > 11)
                    newKey -= 12;
                while(newKey < 0)
                    newKey += 12;

                var tupleReferenceKey = _keys.FirstOrDefault(x => x.Item1 == _referenceKey);
                var tupleNewKey = _keys.FirstOrDefault(x => x.Item1 == newKey);

                string intervalName = string.Empty;
                switch(Math.Abs(_interval))
                {
                    case 1:
                        intervalName = " (minor 2nd)";
                        break;
                    case 2:
                        intervalName = " (major 2nd)";
                        break;
                    case 3:
                        intervalName = " (minor 3rd)";
                        break;
                    case 4:
                        intervalName = " (major 3rd)";
                        break;
                    case 5:
                        intervalName = " (perfect 4th)";
                        break;
                    case 6:
                        intervalName = " (diminished 5th)";
                        break;
                    case 7:
                        intervalName = " (perfect 5th)";
                        break;
                    case 8:
                        intervalName = " (minor 6th)";
                        break;
                    case 9:
                        intervalName = " (major 6th)";
                        break;
                    case 10:
                        intervalName = " (minor 7th)";
                        break;
                    case 11:
                        intervalName = " (major 7th)";
                        break;
                    case 12:
                        intervalName = " (octave)";
                        break;
                }

                View.RefreshPitchShifting(new PlayerPitchShifting(){
                    Interval = _interval.ToString("+#;-#;0") + intervalName,
                    IntervalValue = _interval,
                    NewKey = tupleNewKey,
                    ReferenceKey = tupleReferenceKey
                });
            }
            catch(Exception ex)
            {
                Tracing.Log("PitchShiftingPresenter - RefreshPitchShiftingView - Exception: {0}", ex);
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
                if (_playerService.State != SSPPlayerState.Playing)
                    return;

                _interval = interval;                
                _playerService.SetPitchShifting(_interval);
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
                _playerService.SetPitchShifting(_interval);
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
                if(_interval + 1 > 12)
                    return;
                _interval += 1;
                _playerService.SetPitchShifting(_interval);
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
                if(_interval - 1 < -12)
                    return;
                _interval -= 1;
                _playerService.SetPitchShifting(_interval);
                RefreshPitchShiftingView();
            }
            catch(Exception ex)
            {
                View.PitchShiftingError(ex);
            }
        }
    }
}
