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
using Sessions.Core;
using Sessions.Library.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Config.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;

namespace org.sessionsapp.android
{
    public class LibraryPreferencesFragment : BasePreferenceFragment, ILibraryPreferencesView, ISharedPreferencesOnSharedPreferenceChangeListener
    {        
        // Leave an empty constructor or the application will crash at runtime
        public LibraryPreferencesFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = base.OnCreateView(inflater, container, savedInstanceState);
            view.SetBackgroundColor(Resources.GetColor(Resource.Color.background));
            return view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Xml.preferences_library);
            Activity.ActionBar.Title = "Library Preferences";

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindLibraryPreferencesView(this);
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

        public override bool OnPreferenceTreeClick(PreferenceScreen preferenceScreen, Preference preference)
        {
            //Tracing.Log("LibraryPrefs - OnPreferenceTreeClick - pref: {0}", preference.Title);
            if (preference.Key == "select_folders")
            {
                OnSelectFolders();
            }
            else if (preference.Key == "update_library")
            {
                OnUpdateLibrary();
            }
            else if (preference.Key == "reset_library")
            {
                AlertDialog ad = new AlertDialog.Builder(Activity)
                    .SetIconAttribute(global::Android.Resource.Attribute.AlertDialogIcon)
                    .SetTitle("Reset Library")
                    .SetMessage("Are you sure you wish to reset your library?")
                    .SetCancelable(true)
                    .SetPositiveButton("OK", (sender2, args) => OnResetLibrary())
                    .SetNegativeButton("Cancel", (sender2, args) => { })
                    .Create();
                ad.Show();
            }

            return base.OnPreferenceTreeClick(preferenceScreen, preference);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            Tracing.Log("GeneralPreferencesFragment - OnSharedPreferenceChanged - key: {0}", key);
        }

        #region ILibraryPreferencesView implementation

        public Action OnResetLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }
        public Action<Folder> OnRemoveFolder { get; set; }
        public Action<LibraryAppConfig> OnSetLibraryPreferences { get; set; }
        public Action OnSelectFolders { get; set; }
        public Action<bool> OnEnableSyncListener { get; set; }
        public Action<int> OnSetSyncListenerPort { get; set; }

        public void LibraryPreferencesError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshLibraryPreferences(LibraryAppConfig config, string librarySize)
        {
        }

        public void RefreshLibraryPreferences(LibraryAppConfig config)
        {
        }

        #endregion

    }
}
