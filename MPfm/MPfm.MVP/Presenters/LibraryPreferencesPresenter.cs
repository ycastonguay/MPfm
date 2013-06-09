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

using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.Services.Interfaces;
using System;
using MPfm.MVP.Navigation;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Library preferences presenter.
	/// </summary>
    public class LibraryPreferencesPresenter : BasePresenter<ILibraryPreferencesView>, ILibraryPreferencesPresenter
	{
        readonly MobileNavigationManager _navigationManager;
        readonly ISyncListenerService _syncListenerService;
        readonly ILibraryService _libraryService;
        readonly IAudioFileCacheService _audioFileCacheService;

        public LibraryPreferencesPresenter(ISyncListenerService syncListenerService, ILibraryService libraryService, 
                                           IAudioFileCacheService audioFileCacheService, MobileNavigationManager navigationManager)
		{	
            _syncListenerService = syncListenerService;
            _libraryService = libraryService;
            _audioFileCacheService = audioFileCacheService;
            _navigationManager = navigationManager;
		}

        public override void BindView(ILibraryPreferencesView view)
        {
            view.OnResetLibrary = ResetLibrary;
            view.OnUpdateLibrary = UpdateLibrary;
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
                Console.WriteLine("LibraryPreferencesPresenter - ResetLibrary - Failed to reset library: {0}", ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void UpdateLibrary()
        {
            try
            {
                var view = _navigationManager.CreateUpdateLibraryView();
                _navigationManager.PushDialogView("Update Library", view);
            }
            catch(Exception ex)
            {
                Console.WriteLine("LibraryPreferencesPresenter - UpdateLibrary - Failed to reset library: {0}", ex);
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
                Console.WriteLine("LibraryPreferencesPresenter - EnableSyncListener - Failed to enable/disable sync listener: {0}", ex);
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
                Console.WriteLine("LibraryPreferencesPresenter - SetSyncListenerPort - Failed to set sync listener port: {0}", ex);
                View.LibraryPreferencesError(ex);
            }
        }
	}
}
