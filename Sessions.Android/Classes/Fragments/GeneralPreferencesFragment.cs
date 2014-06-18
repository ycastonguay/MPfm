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
using System.Reflection.Emit;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android;
using MPfm.Android.Classes.Fragments.Base;
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
        }

        #region IGeneralPreferencesView implementation

        public Action<GeneralAppConfig> OnSetGeneralPreferences { get; set; }
        public Action OnDeletePeakFiles { get; set; }
        
        public void GeneralPreferencesError(Exception ex)
        {
        }

        public void RefreshGeneralPreferences(GeneralAppConfig config, string peakFolderSize)
        {
        }

        #endregion

    }
}
