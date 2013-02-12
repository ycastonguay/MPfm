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
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Main window presenter.
	/// </summary>
	public class MobileOptionsMenuPresenter : BasePresenter<IMobileOptionsMenuView>, IMobileOptionsMenuPresenter
	{
        readonly MobileNavigationManager _navigationManager;
        
		#region Constructor and Dispose

        public MobileOptionsMenuPresenter(MobileNavigationManager navigationManager)
		{
            this._navigationManager = navigationManager;          
		}

		#endregion

        public override void BindView(IMobileOptionsMenuView view)
        {
            base.BindView(view);

            view.OnClickAbout = OnClickAbout;
            view.OnClickEffects = OnClickEffects;
            view.OnClickPreferences = OnClickPreferences;
            
            Initialize();
        }

	    private void Initialize()
	    {	        
            Dictionary<MobileOptionsMenuType, string> dictionary = new Dictionary<MobileOptionsMenuType, string>();
            dictionary.Add(MobileOptionsMenuType.UpdateLibrary, "Update Library");
            dictionary.Add(MobileOptionsMenuType.Effects, "Effects");
            dictionary.Add(MobileOptionsMenuType.Preferences, "Preferences");
            dictionary.Add(MobileOptionsMenuType.About, "About MPfm");
            View.RefreshMenu(dictionary);
	    }

	    private void OnClickPreferences()
	    {
	    }

	    private void OnClickEffects()
	    {
	    }

	    private void OnClickAbout()
	    {
	    }
    }
}

