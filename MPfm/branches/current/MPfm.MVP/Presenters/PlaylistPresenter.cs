//
// PlaylistPresenter.cs: Playlist presenter.
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

using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Playlist presenter.
	/// </summary>
	public class PlaylistPresenter : BasePresenter<IPlaylistView>, IPlaylistPresenter
	{
		// Private variables
		//IPlaylistView view = null;

		#region Constructor and Dispose

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistPresenter"/> class.
        /// </summary>
		public PlaylistPresenter()
		{	
		}

		#endregion
		
//		/// <summary>
//		/// Binds the view to its implementation.
//		/// </summary>
//		/// <param name='view'>Playlist view implementation</param>		
//		public void BindView(IPlaylistView view)
//		{
//			// Validate parameters
//			if(view == null)			
//				throw new ArgumentNullException("The view parameter is null!");			
//						
//			// Set properties
//			this.view = view;	
//		}
	}
}

