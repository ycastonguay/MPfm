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

using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using System.Collections.Generic;
using Sessions.MVP.Navigation;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Preferences presenter.
	/// </summary>
    public class PreferencesPresenter : BasePresenter<IPreferencesView>, IPreferencesPresenter
	{
        readonly MobileNavigationManager _navigationManager;
        List<string> _items = new List<string>();

        public PreferencesPresenter(MobileNavigationManager navigationManager)
		{	
            _navigationManager = navigationManager;
		}

        public override void BindView(IPreferencesView view)
        {
            view.OnSelectItem = SelectItem;
            base.BindView(view);
            
            Initialize();
        }       
        
        private void Initialize()
        {
            _items = new List<string>(){
                "Audio Preferences",
                "General Preferences",
                "Library Preferences",
                "Cloud Preferences"
            };
            View.RefreshItems(_items);
        }

        private void SelectItem(string item)
        {
            if(item.ToUpper() == "AUDIO PREFERENCES")
                _navigationManager.CreateAudioPreferencesView();
            else if(item.ToUpper() == "GENERAL PREFERENCES")
                _navigationManager.CreateGeneralPreferencesView();
            else if(item.ToUpper() == "LIBRARY PREFERENCES")
                _navigationManager.CreateLibraryPreferencesView();
            else if(item.ToUpper() == "CLOUD PREFERENCES")
                _navigationManager.CreateCloudPreferencesView();
        }
	}
}
