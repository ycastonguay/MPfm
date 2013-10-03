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
using MPfm.Core;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Library preferences presenter.
	/// </summary>
    public class LibraryPreferencesPresenter : BasePresenter<ILibraryPreferencesView>, ILibraryPreferencesPresenter
	{
        readonly NavigationManager _navigationManager;
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly ISyncListenerService _syncListenerService;
        readonly ILibraryService _libraryService;
        readonly IAudioFileCacheService _audioFileCacheService;

        public LibraryPreferencesPresenter(ISyncListenerService syncListenerService, ILibraryService libraryService, 
                                           IAudioFileCacheService audioFileCacheService)
		{	
            _syncListenerService = syncListenerService;
            _libraryService = libraryService;
            _audioFileCacheService = audioFileCacheService;

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
		}

        public override void BindView(ILibraryPreferencesView view)
        {
            view.OnResetLibrary = ResetLibrary;
            view.OnUpdateLibrary = UpdateLibrary;
            view.OnSelectFolders = SelectFolders;
            
            view.OnEnableSyncListener = EnableSyncListener;
            view.OnSetSyncListenerPort = SetSyncListenerPort;
            base.BindView(view);
            
            Initialize();
        }

	    private void Initialize()
        {
        }

        private void ResetLibrary()
        {
            try
            {
                _libraryService.ResetLibrary();
                _audioFileCacheService.RefreshCache();               
            }
            catch(Exception ex)
            {
                Tracing.Log("LibraryPreferencesPresenter - ResetLibrary - Failed to reset library: {0}", ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void UpdateLibrary()
        {
            try
            {
                var view = _mobileNavigationManager.CreateUpdateLibraryView();
                _mobileNavigationManager.PushDialogView("Update Library", View, view);
            }
            catch(Exception ex)
            {
                Tracing.Log("LibraryPreferencesPresenter - UpdateLibrary - Failed to create and push update library view: {0}", ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void SelectFolders()
        {
            try
            {
                var view = _mobileNavigationManager.CreateSelectFoldersView();
                _mobileNavigationManager.PushDialogView("Select Folders", View, view);
            }
            catch (Exception ex)
            {
                Tracing.Log("LibraryPreferencesPresenter - SelectFolders - Failed to create SelectFolders view: {0}", ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void EnableSyncListener()
        {
            try
            {
                if(_syncListenerService.IsRunning)
                    _syncListenerService.Stop();
                else 
                    _syncListenerService.Start();
            }
            catch(Exception ex)
            {
                Tracing.Log("LibraryPreferencesPresenter - EnableSyncListener - Failed to enable/disable sync listener: {0}", ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void SetSyncListenerPort(int port)
        {
            try
            {
                _syncListenerService.SetPort(port);
            }
            catch(Exception ex)
            {
                Tracing.Log("LibraryPreferencesPresenter - SetSyncListenerPort - Failed to set sync listener port: {0}", ex);
                View.LibraryPreferencesError(ex);
            }
        }
	}
}
