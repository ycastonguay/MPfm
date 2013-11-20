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
using MPfm.Core;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.MVP.Models;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Main window presenter.
	/// </summary>
	public class MobileOptionsMenuPresenter : BasePresenter<IMobileOptionsMenuView>, IMobileOptionsMenuPresenter
	{
        readonly MobileNavigationManager _navigationManager;
        List<MobileOptionsMenuEntity> _items;
        
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
            Tracing.Log("MobileOptionsMenuPresenter - Initialize");
            _items = new List<MobileOptionsMenuEntity>();
            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "Resume Playback",
                HeaderTitle = "Player",
                MenuType = MobileOptionsMenuType.ResumePlayback
            });
            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "Equalizer Presets",
                HeaderTitle = "Player",
                MenuType = MobileOptionsMenuType.EqualizerPresets
            });

             _items.Add(new MobileOptionsMenuEntity() { 
                Title = "Nearby Devices",
                HeaderTitle = "Sync",
                MenuType = MobileOptionsMenuType.SyncLibrary
            });
            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "Cloud",
                HeaderTitle = "Sync",
                MenuType = MobileOptionsMenuType.SyncLibraryCloud
            });
            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "Web Browser",
                HeaderTitle = "Sync",
                MenuType = MobileOptionsMenuType.SyncLibraryWebBrowser
            });


#if IOS
            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "Audio",
                HeaderTitle = "Preferences",
                MenuType = MobileOptionsMenuType.AudioPreferences
            });
            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "Cloud",
                HeaderTitle = "Preferences",
                MenuType = MobileOptionsMenuType.CloudPreferences
            });
            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "General",
                HeaderTitle = "Preferences",
                MenuType = MobileOptionsMenuType.GeneralPreferences
            });
            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "Library",
                HeaderTitle = "Preferences",
                MenuType = MobileOptionsMenuType.LibraryPreferences
            });
#else
            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "Preferences",
                HeaderTitle = "Other",
                MenuType = MobileOptionsMenuType.Preferences
            });
#endif

            _items.Add(new MobileOptionsMenuEntity() { 
                Title = "About Sessions",
                HeaderTitle = "Other",
                MenuType = MobileOptionsMenuType.About
            });

//            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.ResumePlayback, "Resume Playback"));
//            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibrary, "Sync (Nearby Devices)"));
//
//#if IOS
//            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibraryFileSharing, "Sync (iTunes)"));
//#elif ANDROID
//            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibraryFileSharing, "Sync (File Share)"));
//#endif
//
//            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibraryCloud, "Sync (Cloud)"));
//            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.SyncLibraryWebBrowser, "Sync (Web Browser)"));
//            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.EqualizerPresets, "Equalizer Presets"));
//            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.Preferences, "Preferences"));
//            _items.Add(new KeyValuePair<MobileOptionsMenuType, string>(MobileOptionsMenuType.About, "About Sessions"));
            View.RefreshMenu(_items);
	    }

        private void OnItemClick(MobileOptionsMenuType menuType)
        {
            switch (menuType)
            {
                case MobileOptionsMenuType.About:
                {
                    _navigationManager.CreateAboutView();
                    break;
                }
                case MobileOptionsMenuType.ResumePlayback:
                {
                    _navigationManager.CreateResumePlaybackView();
                    break;
                }
                case MobileOptionsMenuType.UpdateLibrary:
                {
                    var view = _navigationManager.CreateUpdateLibraryView();
                    _navigationManager.PushDialogView(MobileDialogPresentationType.Standard, "Update Library", View, view);
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
                case MobileOptionsMenuType.SyncLibraryCloud:
                {
                    _navigationManager.CreateSyncCloudView();
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
                case MobileOptionsMenuType.AudioPreferences:
                {
                    _navigationManager.CreateAudioPreferencesView();
                    break;
                }
                case MobileOptionsMenuType.CloudPreferences:
                {
                    _navigationManager.CreateCloudPreferencesView();
                    break;
                }
                case MobileOptionsMenuType.GeneralPreferences:
                {
                    _navigationManager.CreateGeneralPreferencesView();
                    break;
                }
                case MobileOptionsMenuType.LibraryPreferences:
                {
                    _navigationManager.CreateLibraryPreferencesView();
                    break;
                }
            }
        }
    }
}

