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

using System;
using System.Reflection.Emit;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Sessions.Android;
using Sessions.Android.Classes.Fragments.Base;
using Sessions.Core;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Config.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;

namespace org.sessionsapp.android
{    
    public class GeneralPreferencesFragment : BasePreferenceFragment, IGeneralPreferencesView, ISharedPreferencesOnSharedPreferenceChangeListener
    {        
        private View _view;
        private TextView _lblTitle;
        private GeneralAppConfig _config;

        // Leave an empty constructor or the application will crash at runtime
        public GeneralPreferencesFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = base.OnCreateView(inflater, container, savedInstanceState);
            view.SetBackgroundColor(Resources.GetColor(Resource.Color.background));
            return view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Xml.preferences_general);
            Activity.ActionBar.Title = "General Preferences";

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindGeneralPreferencesView(this);
        }

        public override void OnResume()
        {
            base.OnResume();
            Tracing.Log("GeneralPreferencesFragment - OnResume - Registering shared preferences event");
            PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnPause()
        {
            base.OnPause();
            PreferenceManager.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            Tracing.Log("GeneralPreferencesFragment - OnSharedPreferenceChanged - key: {0}", key);

            switch (key)
            {
                case "update_frequency_song_position":
                    _config.SongPositionUpdateFrequency = sharedPreferences.GetInt(key, 0);
                    break;
                case "update_frequency_output_meter":
                    _config.OutputMeterUpdateFrequency = sharedPreferences.GetInt(key, 0);
                    break;
            }

            OnSetGeneralPreferences(_config);
        }

        #region IGeneralPreferencesView implementation

        public Action<GeneralAppConfig> OnSetGeneralPreferences { get; set; }
        public Action OnDeletePeakFiles { get; set; }
        
        public void GeneralPreferencesError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshGeneralPreferences(GeneralAppConfig config, string peakFolderSize)
        {
            _config = config;
            Activity.RunOnUiThread(() => 
            { 
                var sharedPreferences = PreferenceManager.SharedPreferences;
                //var value = sharedPreferences.GetInt("update_frequency_song_position", -1);
                var editor = sharedPreferences.Edit();
                editor.PutInt("update_frequency_song_position", config.SongPositionUpdateFrequency);
                editor.PutInt("update_frequency_output_meter", config.OutputMeterUpdateFrequency);
            });
        }

        #endregion

    }
}
