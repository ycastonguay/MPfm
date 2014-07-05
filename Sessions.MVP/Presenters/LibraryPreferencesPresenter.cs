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
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Config;
using Sessions.MVP.Config.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using System.Linq;
using Sessions.Core;
using Sessions.Library.Objects;
using Sessions.Library.Services.Interfaces;
using TinyMessenger;
using Sessions.MVP.Messages;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Library preferences presenter.
	/// </summary>
    public class LibraryPreferencesPresenter : BasePresenter<ILibraryPreferencesView>, ILibraryPreferencesPresenter
	{
        private readonly NavigationManager _navigationManager;
        private readonly MobileNavigationManager _mobileNavigationManager;
        private readonly ISyncListenerService _syncListenerService;
        private readonly ILibraryService _libraryService;
	    private readonly IUpdateLibraryService _updateLibraryService;
        private readonly IAudioFileCacheService _audioFileCacheService;
        private readonly ITinyMessengerHub _messageHub;

        public LibraryPreferencesPresenter(ISyncListenerService syncListenerService, ILibraryService libraryService, IUpdateLibraryService updateLibraryService,
                                           IAudioFileCacheService audioFileCacheService, ITinyMessengerHub messageHub)
		{	
            _syncListenerService = syncListenerService;
            _libraryService = libraryService;
            _updateLibraryService = updateLibraryService;
            _audioFileCacheService = audioFileCacheService;
            _messageHub = messageHub;

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
		}

        public override void BindView(ILibraryPreferencesView view)
        {
            view.OnSetLibraryPreferences = SetLibraryPreferences;
            view.OnResetLibrary = ResetLibrary;
            view.OnUpdateLibrary = UpdateLibrary;
            view.OnSelectFolders = SelectFolders;
            view.OnAddFolder = AddFolder;
            view.OnRemoveFolders = RemoveFolders;
            view.OnEnableSyncListener = EnableSyncService;
            view.OnSetSyncListenerPort = SetSyncListenerPort;
            base.BindView(view);

            RefreshPreferences();
        }

        private void SetLibraryPreferences(LibraryAppConfig config)
        {
            try
            {
                // Save config
                AppConfigManager.Instance.Root.Library = config;
                AppConfigManager.Instance.Save();

                // Update service configuration
                SetSyncListenerPort(config.SyncServicePort);
                EnableSyncService(config.IsSyncServiceEnabled);

                // Make sure preferences are in sync
                RefreshPreferences();
                _messageHub.PublishAsync<LibraryAppConfigChangedMessage>(new LibraryAppConfigChangedMessage(this, config));
            }
            catch (Exception ex)
            {
                Tracing.Log(ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void RefreshPreferences()
        {
            string size = string.Format("{0:0.00} GB", _audioFileCacheService.AudioFiles.Sum(x => x.FileSize) / 1000000000f);
            View.RefreshLibraryPreferences(AppConfigManager.Instance.Root.Library, size);
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
                Tracing.Log(ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void UpdateLibrary()
        {
            try
            {
#if IOS || ANDROID
                var view = _mobileNavigationManager.CreateUpdateLibraryView();
                _mobileNavigationManager.PushDialogView(MobileDialogPresentationType.NotificationBar, "Update Library", View, view);
#else
                //_navigationManager.CreateUpdateLibraryView(new List<string>(), AppConfigManager.Instance.Root.Library.Folders);

                if(!_updateLibraryService.IsUpdatingLibrary)
                    _updateLibraryService.UpdateLibrary(new List<string>(), AppConfigManager.Instance.Root.Library.Folders);
#endif
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void SelectFolders()
        {
            try
            {
                var view = _mobileNavigationManager.CreateSelectFoldersView();
                _mobileNavigationManager.PushDialogView(MobileDialogPresentationType.Standard, "Select Folders", View, view);
            }
            catch (Exception ex)
            {
                Tracing.Log(ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void AddFolder(string folderPath, bool isRecursive)
        {
            try
            {
                // TODO: Compare paths and 
                var folder = new Folder(folderPath, isRecursive);
                AppConfigManager.Instance.Root.Library.Folders.Add(folder);
                AppConfigManager.Instance.Save();
                RefreshPreferences();
                UpdateLibrary();
            }
            catch (Exception ex)
            {
                Tracing.Log(ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void RemoveFolders(IEnumerable<Folder> folders, bool removeAudioFilesFromLibrary)
        {
            try
            {
                foreach (var folder in folders)
                {
                    AppConfigManager.Instance.Root.Library.Folders.Remove(folder);
                    if (removeAudioFilesFromLibrary)
                        _libraryService.DeleteAudioFiles(folder.FolderPath);
                }
                AppConfigManager.Instance.Save();
                RefreshPreferences();
            }
            catch (Exception ex)
            {
                Tracing.Log(ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void EnableSyncService(bool enabled)
        {
            try
            {
                AppConfigManager.Instance.Root.Library.IsSyncServiceEnabled = enabled;
                AppConfigManager.Instance.Save();

                if(!enabled && _syncListenerService.IsRunning)
                    _syncListenerService.Stop();
                else if(enabled && !_syncListenerService.IsRunning)
                    _syncListenerService.Start();
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.LibraryPreferencesError(ex);
            }
        }

        private void SetSyncListenerPort(int port)
        {
            try
            {
                AppConfigManager.Instance.Root.Library.SyncServicePort = port;
                AppConfigManager.Instance.Save();

                if(_syncListenerService.Port != port)
                    _syncListenerService.SetPort(port);
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.LibraryPreferencesError(ex);
            }
        }
	}
}
