﻿// Copyright © 2011-2013 Yanick Castonguay
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

using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.MVP.Config.Models;
using MPfm.MVP.Config;
using System;
using MPfm.Core;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Audio preferences presenter.
	/// </summary>
    public class AudioPreferencesPresenter : BasePresenter<IAudioPreferencesView>, IAudioPreferencesPresenter
	{
        public AudioPreferencesPresenter()
		{	
		}

        public override void BindView(IAudioPreferencesView view)
        {
            view.OnSetAudioPreferences = SetAudioPreferences;
            base.BindView(view);

            RefreshPreferences();
        }

        private void SetAudioPreferences(AudioAppConfig audioAppConfig)
        {
            try
            {
                AppConfigManager.Instance.Root.Audio = audioAppConfig;
                AppConfigManager.Instance.Save();
                RefreshPreferences();
            }
            catch (Exception ex)
            {
                Tracing.Log("AudioPreferencesPresenter - SetAudioPreferences - Failed to set preferences: {0}", ex);
                View.AudioPreferencesError(ex);
            }
        }

        private void RefreshPreferences()
        {
            try
            {
                View.RefreshAudioPreferences(AppConfigManager.Instance.Root.Audio);
            } 
            catch (Exception ex)
            {
                Tracing.Log("AudioPreferencesPresenter - RefreshPreferences - Failed to refresh preferences: {0}", ex);
                View.AudioPreferencesError(ex);
            }
        }
	}
}