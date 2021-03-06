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
using System.Collections.Generic;
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
using Sessions.Sound.BassNetWrapper;

namespace org.sessionsapp.android
{
    public class AudioPreferencesFragment : BasePreferenceFragment, IAudioPreferencesView, ISharedPreferencesOnSharedPreferenceChangeListener
    {        
        // Leave an empty constructor or the application will crash at runtime
        public AudioPreferencesFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = base.OnCreateView(inflater, container, savedInstanceState);
            view.SetBackgroundColor(Resources.GetColor(Resource.Color.background));
            return view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Xml.preferences_audio);
            Activity.ActionBar.Title = "Audio Preferences";

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindAudioPreferencesView(this);
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

        #region IAudioPreferencesView implementation

        public Action<AudioAppConfig> OnSetAudioPreferences { get; set; }
        public Action<Device, int> OnSetOutputDeviceAndSampleRate { get; set; }
        public Action OnResetAudioSettings { get; set; }
        public Func<bool> OnCheckIfPlayerIsPlaying { get; set; }

        public void AudioPreferencesError(Exception ex)
        {
        }

        public void RefreshAudioPreferences(AudioAppConfig config)
        {
        }

        public void RefreshAudioDevices(IEnumerable<Device> devices)
        {
        }

        #endregion

    }
}
