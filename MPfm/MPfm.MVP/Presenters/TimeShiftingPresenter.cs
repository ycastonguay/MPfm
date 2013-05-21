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

using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Models;
using System;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Time shifting view presenter.
	/// </summary>
	public class TimeShiftingPresenter : BasePresenter<ITimeShiftingView>, ITimeShiftingPresenter
	{
        readonly IPlayerService _playerService;
        readonly ITinyMessengerHub _messengerHub;

        float _referenceTempo = 120;

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

            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                // Notify view that time shifting has been reset
                View.RefreshTimeShifting(new PlayerTimeShiftingEntity(){
                    TimeShiftingValue = 100,
                    ReferenceTempo = "120 bpm",
                    CurrentTempo = "120 bpm (100%)",
                });
            });

            _playerService.OnBPMDetected += HandleOnBPMDetected;

            base.BindView(view);
        }

        private void HandleOnBPMDetected(float bpm)
        {
            string bpmStr = bpm == 0 ? "Calculating..." : bpm.ToString("0.0").Replace(",", ".") + " bpm";
            View.RefreshBPM(bpm, bpmStr);
        }

        private void UseDetectedTempo()
        {
            try
            {
                _referenceTempo = 120;
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
                // Convert scale from +50/+150 to -100/+100
                float ratio = (timeShifting - 50) / 100;
                float result = (ratio * 200) - 100;
                
                _playerService.SetTimeShifting(result);
                View.RefreshTimeShifting(new PlayerTimeShiftingEntity(){
                    TimeShiftingValue = timeShifting,
                    ReferenceTempo = _referenceTempo.ToString("0.0").Replace(",",".") + " bpm", 
                    CurrentTempo = "120 bpm (" + timeShifting.ToString("0.0").Replace(",",".") + "%)"
                });
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
                _playerService.SetTimeShifting(0);
                View.RefreshTimeShifting(new PlayerTimeShiftingEntity(){
                    TimeShiftingValue = 100,
                    ReferenceTempo = _referenceTempo.ToString("0.0").Replace(",",".") + " bpm", 
                    CurrentTempo = "120 bpm (100%)"
                });
            }
            catch(Exception ex)
            {
                View.TimeShiftingError(ex);
            }                
        }
    }
}
