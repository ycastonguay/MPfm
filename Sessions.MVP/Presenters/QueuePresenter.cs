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
using Sessions.MVP.Messages;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Sound.AudioFiles;
using TinyMessenger;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Queue view presenter.
	/// </summary>
    public class QueuePresenter : BasePresenter<IQueueView>, IQueuePresenter
	{
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly ITinyMessengerHub _messageHub;
        readonly IPlayerService _playerService;
        readonly IAudioFileCacheService _audioFileCacheService;

        public QueuePresenter(ITinyMessengerHub messageHub, MobileNavigationManager mobileNavigationManager, IPlayerService playerService, IAudioFileCacheService audioFileCacheService)
        {
            _messageHub = messageHub;
            _mobileNavigationManager = mobileNavigationManager;
            _playerService = playerService;
            _audioFileCacheService = audioFileCacheService;

            _messageHub.Subscribe<QueueUpdatedMessage>((m) => RefreshQueue());
        }

        public override void BindView(IQueueView view)
        {            
            base.BindView(view);

            view.OnQueueStartPlayback = StartPlayback;
            view.OnQueueRemoveAll = RemoveAll;

            RefreshQueue();
        }

        private void RefreshQueue()
        {
            long ms = 0;
            var lengths = _playerService.CurrentQueue.Items.Select(x => x.AudioFile.Length);
            foreach (string length in lengths)
                ms += ConvertAudio.ToMS(length);
            string totalLength = ConvertAudio.ToTimeString(ms);
            string shortTotalLength = totalLength.Substring(0, totalLength.IndexOf(".", StringComparison.Ordinal));
            View.RefreshQueue(_playerService.CurrentQueue.Items.Count, shortTotalLength);
        }

        private void StartPlayback()
        {
            try
            {
                _playerService.PlayQueue();
                _mobileNavigationManager.CreatePlayerView(MobileNavigationTabType.Artists); // TODO: Damn tab type, must get rid of this
            }
            catch(Exception ex)
            {
                View.QueueError(ex);
            }
        }

        private void RemoveAll()
        {
            _playerService.CurrentQueue.Clear();
            _messageHub.PublishAsync<QueueUpdatedMessage>(new QueueUpdatedMessage(this));
        }
	}
}
