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
using Sessions.MVP.Config;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using System.Threading.Tasks;
#if (MACOSX || LINUX)
using Mono.Unix;
using Mono.Unix.Native;
#endif
using Sessions.Library;
using Sessions.Library.Objects;
using Sessions.Library.Services.Events;
using Sessions.Library.Services.Interfaces;

namespace Sessions.MVP.Presenters
{
	/// <summary>
	/// Update Library window presenter.
	/// </summary>
	 public class UpdateLibraryPresenter : BasePresenter<IUpdateLibraryView>, IUpdateLibraryPresenter
	{
	    readonly IAudioFileCacheService _audioFileCacheService;
		readonly IUpdateLibraryService _updateLibraryService;
        readonly ILibraryService _libraryService;
        readonly ISyncDeviceSpecifications _syncDeviceSpecifications;
		
        public UpdateLibraryPresenter(IAudioFileCacheService audioFileCacheService, IUpdateLibraryService updateLibraryService, 
                                      ILibraryService libraryService, ISyncDeviceSpecifications syncDeviceSpecifications)
		{
		    _audioFileCacheService = audioFileCacheService;
            _libraryService = libraryService;
			_updateLibraryService = updateLibraryService;
            _updateLibraryService.RaiseRefreshStatusEvent += UpdateLibraryServiceOnRaiseRefreshStatusEvent;
            _updateLibraryService.RaiseProcessStartedEvent += UpdateLibraryServiceOnRaiseProcessStartedEvent;
            _updateLibraryService.RaiseProcessEndedEvent += UpdateLibraryServiceOnRaiseProcessEndedEvent;
            _syncDeviceSpecifications = syncDeviceSpecifications;
		}

        protected void UpdateLibraryServiceOnRaiseRefreshStatusEvent(object sender, RefreshStatusEventArgs e)
        {            
            View.RefreshStatus(e.Entity);
        }

        protected void UpdateLibraryServiceOnRaiseProcessStartedEvent(object sender, EventArgs eventArgs)
        {
            View.ProcessStarted();
        }

        protected void UpdateLibraryServiceOnRaiseProcessEndedEvent(object sender, ProcessEndedEventArgs e)
        {
            View.RefreshStatus(new UpdateLibraryEntity() {
                Title = "Refreshing cache",
                Subtitle = "Refreshing cache",
                PercentageDone = 100
            });
            Task.Factory.StartNew(() =>
            {
                _audioFileCacheService.RefreshCache();
                View.ProcessEnded(e.Canceled);  
            });
        }

        public override void BindView(IUpdateLibraryView view)
        {
            base.BindView(view);

            view.OnAddFilesToLibrary = AddFilesToLibrary;
            view.OnAddFolderToLibrary = AddFolderToLibrary;
            view.OnStartUpdateLibrary = StartUpdateLibrary;
            view.OnCancelUpdateLibrary = Cancel;
            view.OnSaveLog = SaveLog;
        }

	    private void AddFolderToLibrary(string folderPath)
	    {
            // Add to list of configured folders
            //_libraryService.AddFolder(folderPath, true);
            UpdateLibrary(new List<string>(), new List<Folder>() { new Folder(folderPath, true) });

            var foundFolder = AppConfigManager.Instance.Root.Library.Folders.FirstOrDefault(x => x.FolderPath == folderPath);
            if (foundFolder == null)
            {
                var folder = new Folder(folderPath, true);
                AppConfigManager.Instance.Root.Library.Folders.Add(folder);
                AppConfigManager.Instance.Save();
            }
	    }

	    private void AddFilesToLibrary(List<string> filePaths)
	    {
            UpdateLibrary(filePaths, new List<Folder>());
	    }

	    public void StartUpdateLibrary()
        {
            var folders = new List<Folder>();
            var paths = _syncDeviceSpecifications.GetMusicFolderPaths();
            foreach (var path in paths)
            {
                folders.Add(new Folder(path, true));
            }

            UpdateLibrary(new List<string>(), folders);
        }
		
		public void UpdateLibrary(List<string> filePaths, List<Folder> folderPaths)
		{
            if (_updateLibraryService.IsUpdatingLibrary)
                return;

            View.ProcessStarted();
	        _updateLibraryService.UpdateLibrary(filePaths, folderPaths);
		}
		
		public void Cancel()
		{	
			_updateLibraryService.Cancel();
		}

		public void SaveLog(string filePath)
		{			
			_updateLibraryService.SaveLog(filePath);
		}
	}
}
