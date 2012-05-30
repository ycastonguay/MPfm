//
// UpdateLibraryPresenter.cs: Update Library window presenter.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
#if (MACOSX || LINUX)
using Mono.Unix;
using Mono.Unix.Native;
#endif

namespace MPfm.MVP
{
	/// <summary>
	/// Update Library window presenter.
	/// </summary>
	public class UpdateLibraryPresenter : IDisposable, IUpdateLibraryPresenter
	{
		// Private variables
		private IUpdateLibraryView view = null;
		private ILibraryService libraryService = null;		
		private IUpdateLibraryService updateLibraryService = null;
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.UpdateLibraryPresenter"/> class.
		/// </summary>
		public UpdateLibraryPresenter(ILibraryService libraryService, IUpdateLibraryService updateLibraryService)
		{
			// Check for null
			if(libraryService == null)
				throw new ArgumentNullException("The libraryService parameter cannot be null!");
			if(updateLibraryService == null)
				throw new ArgumentNullException("The updateLibraryService parameter cannot be null!");

			// Set properties
			this.libraryService = libraryService;
			this.updateLibraryService = updateLibraryService;
			
			// Set events
            this.updateLibraryService.RaiseRefreshStatusEvent += new EventHandler<RefreshStatusEventArgs>(updateLibraryService_RaiseRefreshStatusEvent);
            this.updateLibraryService.RaiseProcessEndedEvent += new EventHandler<ProcessEndedEventArgs>(updateLibraryService_RaiseProcessEndedEvent);
		}

        /// <summary>
        /// Raises when the Update Library service needs to update its consumer.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void updateLibraryService_RaiseRefreshStatusEvent(object sender, RefreshStatusEventArgs e)
        {            
            view.RefreshStatus(e.Entity);
        }

        /// <summary>
        /// Raises when the Update Library service needs to tell its consumer that the process has ended.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void updateLibraryService_RaiseProcessEndedEvent(object sender, ProcessEndedEventArgs e)
        {
            view.ProcessEnded(e.Canceled);   
        }

		/// <summary>
		/// Releases all resources used by the <see cref="MPfm.UI.UpdateLibraryPresenter"/> object.
		/// </summary>
		/// <remarks>
		/// Call <see cref="Dispose"/> when you are finished using the <see cref="MPfm.UI.UpdateLibraryPresenter"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="MPfm.UI.UpdateLibraryPresenter"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="MPfm.UI.UpdateLibraryPresenter"/>
		/// so the garbage collector can reclaim the memory that the <see cref="MPfm.UI.UpdateLibraryPresenter"/> was occupying.
		/// </remarks>
		public void Dispose()
		{
		}

		#endregion
		
		#region IUpdateLibraryPresenter implementation
			
		/// <summary>
		/// Binds the view to its implementation.
		/// </summary>
		/// <param name='view'>Update Library view implementation</param>	
		public void BindView(IUpdateLibraryView view)
		{
			// Validate parameters 
			if(view == null)			
				throw new ArgumentNullException("The view parameter is null!");
			
			// Set property
			this.view = view;
		}
		
		public void UpdateLibrary(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
		{
			updateLibraryService.UpdateLibrary(mode, filePaths, folderPath);
		}
		
		public void Cancel()
		{	
			updateLibraryService.Cancel();
		}
	
		public void SaveLog(string filePath)
		{			
			updateLibraryService.SaveLog(filePath);
		}
			
		#endregion
	}
}
