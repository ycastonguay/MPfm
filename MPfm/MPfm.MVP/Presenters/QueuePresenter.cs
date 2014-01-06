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
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Queue view presenter.
	/// </summary>
    public class QueuePresenter : BasePresenter<IQueueView>, IQueuePresenter
	{
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly ITinyMessengerHub _messageHub;
        readonly IPlayerService _playerService;
	    private readonly ILibraryService _libraryService;

        public QueuePresenter(ITinyMessengerHub messageHub, MobileNavigationManager mobileNavigationManager, IPlayerService playerService, ILibraryService libraryService)
        {
            _messageHub = messageHub;
            _mobileNavigationManager = mobileNavigationManager;
            _playerService = playerService;
            _libraryService = libraryService;
        }

        public override void BindView(IQueueView view)
        {            
            base.BindView(view);

            view.OnQueueStartPlayback = StartPlayback;
            view.OnQueueRemoveAll = RemoveAll;
        }

        private void StartPlayback()
        {
        }

        private void RemoveAll()
        {
        }
	}
}
