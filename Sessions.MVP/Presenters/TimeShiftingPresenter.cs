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

using System.Collections.Generic;
using Sessions.MVP.Messages;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Models;
using System;
using Sessions.Core;
using TinyMessenger;

namespace Sessions.MVP.Presenters
{
	/// <summary>
	/// Time shifting view presenter.
	/// </summary>
	public class TimeShiftingPresenter : BasePresenter<ITimeShiftingView>, ITimeShiftingPresenter
	{
        readonly IPlayerService _playerService;
        readonly ITinyMessengerHub _messengerHub;
	    private List<TinyMessageSubscriptionToken> _tokens = new List<TinyMessageSubscriptionToken>();

        float _timeShifting = 0;
        float _referenceTempo = 120;
        float _detectedTempo = 0;

	    public TimeShiftingPresenter(ITinyMessengerHub messengerHub, IPlayerService playerService)
		{
            _messengerHub = messengerHub;
            _playerService = playerService;
        }

        public override void BindView(ITimeShiftingView view)
        {            
            view.OnSetTimeShifting = SetTimeShifting;
            view.OnResetTimeShifting = ResetTimeShifting;
            view.OnUseDetectedTempo = UseDetectedTempo;
            view.OnIncrementTempo = IncrementTempo;
            view.OnDecrementTempo = DecrementTempo;

            base.BindView(view);

            _playerService.OnBPMDetected += HandleOnBPMDetected;
            _tokens.Add(_messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                _detectedTempo = 0;
                _referenceTempo = 120;
                _timeShifting = 0;
                RefreshTimeShiftingView();
            }));

            // Refresh view with initial data
            _timeShifting = _playerService.TimeShifting;
            RefreshTimeShiftingView();
        }

	    public override void ViewDestroyed()
	    {
            foreach (TinyMessageSubscriptionToken token in _tokens)
                token.Dispose();

	        base.ViewDestroyed();
	    }

	    private void RefreshTimeShiftingView()
        {
            try
            {
                // From -100/+100 to 50/150
                float timeShiftingRatio = (_timeShifting + 100) / 200;
                float timeShiftingValue = (timeShiftingRatio * 100) + 50;
                float currentTempo = _referenceTempo * (timeShiftingValue / 100);
//                Tracing.Log("TimeShiftingPresenter - RefreshTimeShiftingView - timeShifting: " + _timeShifting.ToString() + " timeShiftingRatio: " + timeShiftingRatio.ToString() + 
//                                  " timeShiftingValue: " + timeShiftingValue.ToString() + " detectedTempo: " + _detectedTempo.ToString() +
//                                  " currentTempo: " + currentTempo.ToString() + " referenceTempo: " + _referenceTempo.ToString());

                View.RefreshTimeShifting(new PlayerTimeShiftingEntity(){
                    TimeShiftingValue = timeShiftingValue,
                    ReferenceTempo = String.Format("{0:0.0} bpm", _referenceTempo),
                    CurrentTempo = String.Format("{0:0.0} bpm ({1:0.0}%)", currentTempo, timeShiftingValue),
                    DetectedTempo = (_detectedTempo == 0) ? "Calculating..." : String.Format("{0:0.0} bpm", _detectedTempo)
                });
            }
            catch(Exception ex)
            {
                Tracing.Log("TimeShiftingPresenter - RefreshTimeShiftingView - Exception: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private float GetTempoFromTimeShifting(float timeShifting)
        {
            float timeShiftingRatio = (timeShifting + 100) / 200;
            float timeShiftingValue = (timeShiftingRatio * 100) + 50;
            float tempo = _referenceTempo * (timeShiftingValue / 100);
            return tempo;
        }

        private float GetTimeShiftingFromTempo(float tempo)
        {
            float tempoRatio = tempo / _referenceTempo;
            return tempoRatio * 100;
        }

        private void HandleOnBPMDetected(float bpm)
        {
            try
            {
                if(_detectedTempo > 0 && bpm == 0)
                {
                    // Use the last detected tempo insteadd
                }
                else
                {
                    _detectedTempo = bpm;
                }
                //Tracing.Log("TimeShiftingPresenter - HandleOnBPMDetected - bpm: " + bpm.ToString() + " detectedTempo: " + _detectedTempo.ToString());
                RefreshTimeShiftingView();
            }
            catch(Exception ex)
            {
                Tracing.Log("TimeShiftingPresenter - HandleOnBPMDetected - Exception: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DecrementTempo()
        {
            try
            {
                float currentTempo = GetTempoFromTimeShifting(_timeShifting);
                currentTempo -= 1;
                float timeShifting = GetTimeShiftingFromTempo(currentTempo);
                SetTimeShifting(timeShifting);
            }
            catch(Exception ex)
            {
                View.TimeShiftingError(ex);
            }                
        }

        private void IncrementTempo()
        {
            try
            {
                float currentTempo = GetTempoFromTimeShifting(_timeShifting);
                currentTempo += 1;
                float timeShifting = GetTimeShiftingFromTempo(currentTempo);
                SetTimeShifting(timeShifting);
            }
            catch(Exception ex)
            {
                View.TimeShiftingError(ex);
            }                
        }

        private void UseDetectedTempo()
        {
            try
            {
                _referenceTempo = _detectedTempo;
                RefreshTimeShiftingView();
            }
            catch(Exception ex)
            {
                View.TimeShiftingError(ex);
            }                
        }

        private void SetTimeShifting(float timeShifting)
        {
            try
            {
                if (!_playerService.IsPlaying)
                    return;

                // Convert scale from +50/+150 to -100/+100
                float ratio = (timeShifting - 50) / 100;
                float result = (ratio * 200) - 100;
                _timeShifting = result;
                _playerService.SetTimeShifting(result);
                RefreshTimeShiftingView();
            }
            catch(Exception ex)
            {
                View.TimeShiftingError(ex);
            }
        }

        private void ResetTimeShifting()
        {
            try
            {
                _timeShifting = 0;
                _playerService.SetTimeShifting(0);
                RefreshTimeShiftingView();
            }
            catch(Exception ex)
            {
                View.TimeShiftingError(ex);
            }                
        }
    }
}
