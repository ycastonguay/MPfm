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
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.Playlists;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Select Folders presenter.
	/// </summary>
    public class SelectFoldersPresenter : BasePresenter<ISelectFoldersView>, ISelectFoldersPresenter
	{
        private readonly NavigationManager _navigationManager;
        private readonly MobileNavigationManager _mobileNavigationManager;
	    private readonly ITinyMessengerHub _messengerHub;
	    private readonly ILibraryService _libraryService;

        public SelectFoldersPresenter(ITinyMessengerHub messengerHub, ILibraryService libraryService)
        {
	        _messengerHub = messengerHub;
	        _libraryService = libraryService;

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
        }

        public override void BindView(ISelectFoldersView view)
        {
            view.OnSelectFolder = SelectFolder;
            view.OnSaveFolders = SaveFolders;

            base.BindView(view);

            //_messengerHub.Subscribe<PlaylistListUpdatedMessage>(message => RefreshPlaylists());

            RefreshFolders();
        }

	    private void SelectFolder(Folder folder)
	    {
	    }

        private void RefreshFolders()
        {
        }

        private void SaveFolders()
        {
        }

	}
}
