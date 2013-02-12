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
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Library browser presenter for mobile devices.
	/// </summary>
	public class MobileLibraryBrowserPresenter : BasePresenter<IMobileLibraryBrowserView>, IMobileLibraryBrowserPresenter
	{
        private readonly MobileNavigationManager _navigationManager;
	    private readonly MobileNavigationTabType _tabType;
	    private readonly ITinyMessengerHub _messageHub;
        private readonly ILibraryService _libraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
		
		public AudioFileFormat Filter { get; private set; }
		
        public MobileLibraryBrowserPresenter(MobileNavigationTabType tabType, ITinyMessengerHub messageHub, MobileNavigationManager navigationManager,
                                             ILibraryService libraryService, IAudioFileCacheService audioFileCacheService)
		{
            _tabType = tabType;
            _messageHub = messageHub;
            _navigationManager = navigationManager;
            _libraryService = libraryService;
			_audioFileCacheService = audioFileCacheService;			
			
			Filter = AudioFileFormat.All;
		}
		
        public override void BindView(IMobileLibraryBrowserView view)
        {
            base.BindView(view);

            view.OnItemClick = OnItemClick;
        }

	    private void OnItemClick(int i)
	    {
            // Make sure the view was binded to the presenter before publishing a message
	        Action<IBaseView> onViewBindedToPresenter = (theView) => _messageHub.PublishAsync<MobileLibraryBrowserItemClickedMessage>(new MobileLibraryBrowserItemClickedMessage(this)
	            {
	                Item = null,
	                Query = new SongBrowserQueryEntity()
	                    {
	                        ArtistName = ""
	                    }
	            });

            // Create player view
            var view = _navigationManager.CreatePlayerView(onViewBindedToPresenter);
            _navigationManager.PushTabView(_tabType, view);

	    }
	}
}

