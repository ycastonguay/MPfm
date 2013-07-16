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
        
        public MobileOptionsMenuPresenter(MobileNavigationManager navigationManager)
		{
            _navigationManager = navigationManager;          
		}

        public override void BindView(IMobileOptionsMenuView view)
        {
            base.BindView(view);

            view.OnItemClick = OnItemClick;
            Initialize();
        }

	    private void Initialize()
	    {
            Console.WriteLine("MobileOptionsMenuPresenter - Initialize");
            _items = new List<KeyValuePair<MobileOptionsMenuType, string>>();
            //_items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.UpdateLibrary, "Update Library"));
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibrary, "Sync Library (Other Devices)"));

#if IOS
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibraryFileSharing, "Sync Library (iTunes)"));
#elif ANDROID
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibraryFileSharing, "Sync Library with File Sharing"));
#endif

            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibraryCloud, "Sync Library (Cloud Services)"));
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibraryWebBrowser, "Sync Library (Web Browser)"));
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.EqualizerPresets, "Equalizer Presets"));
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.Preferences, "Preferences"));
            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.About, "About Sessions"));
            View.RefreshMenu(_items);
	    }

        private void OnItemClick(MobileOptionsMenuType menuType)
        {
            switch (menuType)
            {
                case MobileOptionsMenuType.About:
                {
                    var view = _navigationManager.CreateAboutView();
                    _navigationManager.PushTabView(MobileNavigationTabType.More, view);
                    break;
                }
                case MobileOptionsMenuType.UpdateLibrary:
                {
                    var view = _navigationManager.CreateUpdateLibraryView();
                    _navigationManager.PushDialogView("Update Library", View, view);
                    break;
                }
                case MobileOptionsMenuType.SyncLibrary:
                {
                    _navigationManager.CreateSyncView();                    
                    break;
                }
                case MobileOptionsMenuType.SyncLibraryWebBrowser:
                {
                    _navigationManager.CreateSyncWebBrowserView();
                    break;
                }
                case MobileOptionsMenuType.EqualizerPresets:
                {
                    _navigationManager.CreateEqualizerPresetsView(View);
                    break;
                }
                case MobileOptionsMenuType.Preferences:
                {
                    _navigationManager.CreatePreferencesView();                    
                    break;
                }
            }
        }
    }
}

