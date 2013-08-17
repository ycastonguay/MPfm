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
using System;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Main window presenter.
	/// </summary>
	public class MainPresenter : BasePresenter<IMainView>, IMainPresenter
	{
        readonly NavigationManager _navigationManager;
        
		public MainPresenter(NavigationManager navigationManager)
		{
            _navigationManager = navigationManager;          
		}
        
        public override void BindView(IMainView view)
        {            
            view.OnOpenPlaylistWindow = OpenPlaylistWindow;
            view.OnOpenEffectsWindow = OpenEffectsWindow;
            view.OnOpenPreferencesWindow = OpenPreferencesWindow;
            view.OnOpenSyncWindow = OpenSyncWindow;

            base.BindView(view);
        }
        
        void OpenPlaylistWindow()
        {
            _navigationManager.CreatePlaylistView();
        }

        void OpenEffectsWindow()
        {
            _navigationManager.CreateEffectsView();
        }

        void OpenPreferencesWindow()
        {
            _navigationManager.CreatePreferencesView();
        }

        void OpenSyncWindow()
        {
            _navigationManager.CreateSyncView();
        }        
    }
}

