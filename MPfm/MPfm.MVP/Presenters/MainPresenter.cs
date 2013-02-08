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

using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Main window presenter.
	/// </summary>
	public class MainPresenter : BasePresenter<IMainView>, IMainPresenter
	{
        readonly NavigationManager navigationManager;
        
		#region Constructor and Dispose

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.MVP.EffectsPresenter"/> class.
        /// </summary>
        /// <param name='playerService'>
        /// Player service.
        /// </param>
		public MainPresenter(NavigationManager navigationManager)
		{
            this.navigationManager = navigationManager;          
		}

		#endregion
        
        public override void BindView(IMainView view)
        {            
            // Subscribe to view actions
            view.OnOpenPlaylistWindow = OpenPlaylistWindow;
            view.OnOpenEffectsWindow = OpenEffectsWindow;
            view.OnOpenPreferencesWindow = OpenPreferencesWindow;
            
            base.BindView(view);
        }
        
        void OpenPlaylistWindow()
        {
        }

        void OpenEffectsWindow()
        {
        }

        void OpenPreferencesWindow()
        {
            navigationManager.CreatePreferencesView();
        }
        
    }
}

