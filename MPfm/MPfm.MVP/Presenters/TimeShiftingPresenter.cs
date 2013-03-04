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
        IPlayerService _playerService;
        ITinyMessengerHub _messengerHub;

        public TimeShiftingPresenter(ITinyMessengerHub messengerHub, IPlayerService playerService)
		{
            _messengerHub = messengerHub;
            _playerService = playerService;
        }

        public override void BindView(ITimeShiftingView view)
        {            
            // Subscribe to view actions
            view.OnSetTimeShifting = SetTimeShifting;
            view.OnSetTimeShiftingMode = SetTimeShiftingMode;
            view.OnDetectTempo = DetectTempo;
            view.OnResetTimeShifting = ResetTimeShifting;

            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                // Notify view that time shifting has been reset
                View.RefreshTimeShifting(new PlayerTimeShiftingEntity(){
                    TimeShifting = 100,
                    TimeShiftingString = "100 %"
                });
            });

            base.BindView(view);
        }

        private void SetTimeShifting(float timeShifting)
        {
            try
            {
                // Convert scale from +50/+150 to -100/+100
                float ratio = (timeShifting - 50) / 100;
                float result = (ratio * 200) - 100;
                
                // Set time shifting and refresh UI
                _playerService.SetTimeShifting(result);
                View.RefreshTimeShifting(new PlayerTimeShiftingEntity(){
                    TimeShifting = timeShifting,
                    TimeShiftingString = timeShifting.ToString("0") + " %"
                });
            }
            catch(Exception ex)
            {
                View.TimeShiftingError(ex);
            }                
        }

        private void SetTimeShiftingMode(TimeShiftingMode mode)
        {
        }

        private void DetectTempo()
        {
        }

        private void ResetTimeShifting()
        {
            try
            {
                // Set time shifting and refresh UI
                _playerService.SetTimeShifting(0);
                View.RefreshTimeShifting(new PlayerTimeShiftingEntity(){
                    TimeShifting = 100,
                    TimeShiftingString = "100 %"
                });
            }
            catch(Exception ex)
            {
                View.TimeShiftingError(ex);
            }                
        }
    }

    public enum TimeShiftingMode
    {
        Percentage = 0,
        Tempo = 1
    }
}

