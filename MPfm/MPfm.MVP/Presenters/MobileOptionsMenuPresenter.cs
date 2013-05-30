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
        List<KeyValuePair<MobileOptionsMenuType, string>> _items;
        
		#region Constructor and Dispose

        public MobileOptionsMenuPresenter(MobileNavigationManager navigationManager)
		{
            _navigationManager = navigationManager;          
		}

		#endregion

        public override void BindView(IMobileOptionsMenuView view)
        {
            base.BindView(view);

            view.OnItemClick = OnItemClick;
            
            Initialize();
        }

	    private void Initialize()
	    {	        
            _items = new List<KeyValuePair<MobileOptionsMenuType, string>>();
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.UpdateLibrary, "Update Library"));
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibrary, "Sync Library"));
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.EqualizerPresets, "Equalizer Presets"));
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.Preferences, "Preferences"));
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.About, "About MPfm"));
            View.RefreshMenu(_items);
	    }

        private void OnItemClick(MobileOptionsMenuType menuType)
        {
            switch (menuType)
            {
                case MobileOptionsMenuType.About:
                {
                    break;
                }
                case MobileOptionsMenuType.UpdateLibrary:
                {
                    var view = _navigationManager.CreateUpdateLibraryView();
                    _navigationManager.PushDialogView("Update Library", view);
                    break;
                }
                case MobileOptionsMenuType.SyncLibrary:
                {
                    var view = _navigationManager.CreateSyncView();
                    _navigationManager.PushTabView(MobileNavigationTabType.More, view);
                    break;
                }
                case MobileOptionsMenuType.EqualizerPresets:
                {
                    var view = _navigationManager.CreateEqualizerPresetsView();
                    _navigationManager.PushDialogView("Equalizer Presets", view);
                    break;
                }
                case MobileOptionsMenuType.Preferences:
                {
                    break;
                }
            }
        }
    }
}

