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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using MPfm.Android;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Config.Models;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;

namespace org.sessionsapp.android
{
    public class CloudPreferencesFragment : BasePreferenceFragment, ICloudPreferencesView, ISharedPreferencesOnSharedPreferenceChangeListener
    {        
        // Leave an empty constructor or the application will crash at runtime
        public CloudPreferencesFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = base.OnCreateView(inflater, container, savedInstanceState);
            view.SetBackgroundColor(Resources.GetColor(Resource.Color.background));
            return view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Xml.preferences_cloud);
            Activity.ActionBar.Title = "Cloud Preferences";

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindCloudPreferencesView(this);
        }

        public override void OnResume()
        {
            base.OnResume();
            PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnPause()
        {
            base.OnPause();
            PreferenceManager.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            Tracing.Log("CloudPreferencesFragment - OnSharedPreferenceChanged - key: {0}", key);
        }

        public override bool OnPreferenceTreeClick(PreferenceScreen preferenceScreen, Preference preference)
        {
            if (preference.Key == "dropbox_login")
            {
                OnDropboxLoginLogout();
            }
            else if (preference.Key == "cloud_resume_playback_enabled")
            {

            }
            else if (preference.Key == "cloud_sync_playlists")
            {
                
            }
            else if (preference.Key == "cloud_sync_presets")
            {
                
            }
            else if (preference.Key == "cloud_sync_wifi_only")
            {
                
            }

            return base.OnPreferenceTreeClick(preferenceScreen, preference);
        }

        #region ICloudPreferencesView implementation

        public Action<CloudAppConfig> OnSetCloudPreferences { get; set; }
        public Action OnDropboxLoginLogout { get; set; }

        public void CloudPreferencesError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshCloudPreferences(CloudAppConfig config)
        {
        }

        public void RefreshCloudPreferencesState(CloudPreferencesStateEntity entity)
        {
            Activity.RunOnUiThread(() =>
            {
                var preference = FindPreference("dropbox_login");
                preference.Title = entity.IsDropboxLinkedToApp ? "Logout from Dropbox" : "Login to Dropbox";
            });
        }

        #endregion

    }
}
