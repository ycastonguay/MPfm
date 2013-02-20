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
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Events;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;

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
	    private readonly IAudioFileCacheService _audioFileCacheService;
		readonly ILibraryService _libraryService;		
		readonly IUpdateLibraryService _updateLibraryService;
		
		public UpdateLibraryPresenter(IAudioFileCacheService audioFileCacheService, ILibraryService libraryService, IUpdateLibraryService updateLibraryService)
		{
			// Set properties
		    _audioFileCacheService = audioFileCacheService;
		    _libraryService = libraryService;
			_updateLibraryService = updateLibraryService;
			
			// Set events
            _updateLibraryService.RaiseRefreshStatusEvent += new EventHandler<RefreshStatusEventArgs>(updateLibraryService_RaiseRefreshStatusEvent);
            _updateLibraryService.RaiseProcessEndedEvent += new EventHandler<ProcessEndedEventArgs>(updateLibraryService_RaiseProcessEndedEvent);
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
            // Refresh audio file cache service
            _audioFileCacheService.RefreshCache();
            View.ProcessEnded(e.Canceled);   
        }

        public override void BindView(IUpdateLibraryView view)
        {
            base.BindView(view);

            view.OnStartUpdateLibrary = UpdateLibrary;
            view.OnCancelUpdateLibrary = Cancel;
        }
		
		/// <summary>
		/// Starts the update library process in a background thread. 
		/// The Update Library view will be updated during progress.
		/// </summary>
		/// <param name='mode'>Update library mode</param>
		/// <param name='filePaths'>Audio file paths to add to the database</param>
		/// <param name='folderPath'>Folder path to add to the database</param>
		public void UpdateLibrary(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
		{
			_updateLibraryService.UpdateLibrary(mode, filePaths, folderPath);
		}
		
		/// <summary>
		/// Cancels the update library process.
		/// </summary>
		public void Cancel()
		{	
			_updateLibraryService.Cancel();
		}

		/// <summary>
		/// Saves the update library process log to the specified file path.
		/// </summary>
		/// <param name='filePath'>Log file path</param>
		public void SaveLog(string filePath)
		{			
			_updateLibraryService.SaveLog(filePath);
		}
	}
}
