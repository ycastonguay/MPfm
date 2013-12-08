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
using MPfm.Library.Objects;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.Services.Interfaces;
using MPfm.Library.Services.Events;
using MPfm.Library;
using System.Threading.Tasks;

#if (MACOSX || LINUX)
using Mono.Unix;
using Mono.Unix.Native;
#endif

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Update Library window presenter.
	/// </summary>
	public class UpdateLibraryPresenter : BasePresenter<IUpdateLibraryView>, IUpdateLibraryPresenter
	{
	    readonly IAudioFileCacheService _audioFileCacheService;
		readonly IUpdateLibraryService _updateLibraryService;
        readonly ISyncDeviceSpecifications _syncDeviceSpecifications;
		
        public UpdateLibraryPresenter(IAudioFileCacheService audioFileCacheService, IUpdateLibraryService updateLibraryService, 
                                      ISyncDeviceSpecifications syncDeviceSpecifications)
		{
		    _audioFileCacheService = audioFileCacheService;
			_updateLibraryService = updateLibraryService;
            _updateLibraryService.RaiseRefreshStatusEvent += new EventHandler<RefreshStatusEventArgs>(updateLibraryService_RaiseRefreshStatusEvent);
            _updateLibraryService.RaiseProcessEndedEvent += new EventHandler<ProcessEndedEventArgs>(updateLibraryService_RaiseProcessEndedEvent);
            _syncDeviceSpecifications = syncDeviceSpecifications;
		}

        /// <summary>
        /// Raises when the Update Library service needs to update its consumer.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void updateLibraryService_RaiseRefreshStatusEvent(object sender, RefreshStatusEventArgs e)
        {            
            View.RefreshStatus(e.Entity);
        }

        /// <summary>
        /// Raises when the Update Library service needs to tell its consumer that the process has ended.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void updateLibraryService_RaiseProcessEndedEvent(object sender, ProcessEndedEventArgs e)
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

            view.OnStartUpdateLibrary = StartUpdateLibrary;
            view.OnCancelUpdateLibrary = Cancel;
            view.OnSaveLog = SaveLog;
        }

        public void StartUpdateLibrary()
        {
            var folder = new Folder()
            {
                FolderPath = _syncDeviceSpecifications.GetMusicFolderPath(),
                IsRecursive = true
            };
            UpdateLibrary(new List<string>(), new List<Folder>(){ folder });
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
