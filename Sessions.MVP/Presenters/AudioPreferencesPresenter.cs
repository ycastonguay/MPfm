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
using Sessions.MVP.Config.Models;
using Sessions.MVP.Config;
using System;
using Sessions.Core;
using TinyMessenger;
using Sessions.MVP.Messages;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Audio preferences presenter.
	/// </summary>
    public class AudioPreferencesPresenter : BasePresenter<IAudioPreferencesView>, IAudioPreferencesPresenter
	{
        private readonly ITinyMessengerHub _messageHub;

        public AudioPreferencesPresenter(ITinyMessengerHub messageHub)
        {
            _messageHub = messageHub;
        }

        public override void BindView(IAudioPreferencesView view)
        {
            view.OnSetAudioPreferences = SetAudioPreferences;
            base.BindView(view);

            RefreshPreferences();
        }

        private void SetAudioPreferences(AudioAppConfig config)
        {
            try
            {
                AppConfigManager.Instance.Root.Audio = config;
                AppConfigManager.Instance.Save();
                RefreshPreferences();
                _messageHub.PublishAsync<AudioAppConfigChangedMessage>(new AudioAppConfigChangedMessage(this, config));
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
