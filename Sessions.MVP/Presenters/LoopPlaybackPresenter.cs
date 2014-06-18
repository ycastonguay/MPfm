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

using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.Services.Interfaces;
using MPfm.Player.Objects;
using Sessions.Core;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using MPfm.MVP.Messages;
using System;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Models;

namespace MPfm.MVP.Presenters
{
    public class LoopPlaybackPresenter : BasePresenter<ILoopPlaybackView>, ILoopPlaybackPresenter
	{
        Guid _loopId;
        Loop _loop;
        AudioFile _audioFile;
        long _lengthBytes;
        readonly ITinyMessengerHub _messageHub;
        readonly IPlayerService _playerService;
        
        public LoopPlaybackPresenter(ITinyMessengerHub messageHub, IPlayerService playerService)
		{
            _messageHub = messageHub;
            _playerService = playerService;
		}

        public override void BindView(ILoopPlaybackView view)
        {            
            view.OnPreviousLoop = PreviousLoop;
            view.OnNextLoop = NextLoop;
            view.OnPreviousSegment = PreviousSegment;
            view.OnNextSegment = NextSegment;
            base.BindView(view);

            RefreshEntity();
        }

        private void RefreshEntity()
        {
            var entity = new LoopPlaybackEntity();
            View.RefreshLoopPlayback(entity);
        }

        private void PreviousLoop()
        {
            try
            {

            } 
            catch (Exception ex)
            {
                Tracing.Log(string.Format("An error occured: {0}", ex));
                View.LoopPlaybackError(ex);
            }
        }

        private void NextLoop()
        {
            try
            {

            } 
            catch (Exception ex)
            {
                Tracing.Log(string.Format("An error occured: {0}", ex));
                View.LoopPlaybackError(ex);
            }
        }

        private void PreviousSegment()
        {
            try
            {

            } 
            catch (Exception ex)
            {
                Tracing.Log(string.Format("An error occured: {0}", ex));
                View.LoopPlaybackError(ex);
            }
        }

        private void NextSegment()
        {
            try
            {

            } 
            catch (Exception ex)
            {
                Tracing.Log(string.Format("An error occured: {0}", ex));
                View.LoopPlaybackError(ex);
            }
        }

    }
}
