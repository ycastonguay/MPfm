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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.MVP
{
	/// <summary>
	/// Update Library window presenter.
	/// </summary>
	public class UpdateLibraryPresenter : IDisposable, IUpdateLibraryPresenter
	{
		// Private variables
		private IUpdateLibraryView view = null;
		private IMainPresenter mainPresenter = null;
		private IUpdateLibraryService updateLibraryService = null;
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.UpdateLibraryPresenter"/> class.
		/// </summary>
		public UpdateLibraryPresenter(IUpdateLibraryView view, IMainPresenter mainPresenter, IUpdateLibraryService updateLibraryService)
		{
			// Check for null
			if(view == null)
				throw new ArgumentNullException("The view parameter cannot be null!");
			if(mainPresenter == null)
				throw new ArgumentNullException("The mainPresenter parameter cannot be null!");
			if(updateLibraryService == null)
				throw new ArgumentNullException("The updateLibraryService parameter cannot be null!");

			// Set properties
			this.view = view;
			this.mainPresenter = mainPresenter;
			this.updateLibraryService = updateLibraryService;			
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
			
		public void OK()
		{			
		}
	
		public void Cancel()
		{			
		}
	
		public void SaveLog(string filePath)
		{			
		}
			
		#endregion
		
		private void UpdateStatus()
		{
		}
	}
}

