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
using org.sessionsapp.player;
using Sessions.Core;
using Sessions.MVP.Config;
using Sessions.MVP.Config.Models;
using Sessions.MVP.Messages;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Sound.PeakFiles;
using TinyMessenger;
using System.Collections.Generic;
using Sessions.Sound.PeakFiles.Interfaces;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Audio preferences presenter.
	/// </summary>
    public class AudioPreferencesPresenter : BasePresenter<IAudioPreferencesView>, IAudioPreferencesPresenter
	{
        private readonly ITinyMessengerHub _messageHub;
	    private readonly IPlayerService _playerService;
	    private readonly IPeakFileService _peakFileService;

	    public AudioPreferencesPresenter(ITinyMessengerHub messageHub, IPlayerService playerService, IPeakFileService peakFileService)
        {
            _messageHub = messageHub;
            _playerService = playerService;
	        _peakFileService = peakFileService;
        }

	    public override void BindView(IAudioPreferencesView view)
        {
            view.OnSetAudioPreferences = SetAudioPreferences;
            view.OnSetOutputDeviceAndSampleRate = SetOutputDeviceAndSampleRate;
            view.OnResetAudioSettings = ResetAudioSettings;
            view.OnCheckIfPlayerIsPlaying = CheckIfPlayerIsPlaying;
            base.BindView(view);

            RefreshAudioDevices();
            RefreshPreferences();
        }

	    private void SetAudioPreferences(AudioAppConfig config)
        {
            try
            {
                AppConfigManager.Instance.Root.Audio = config;
                AppConfigManager.Instance.Save();
                RefreshPreferences();

                _playerService.SetBufferSize(config.BufferSize);
                _playerService.SetUpdatePeriod(config.UpdatePeriod);

                _messageHub.PublishAsync<AudioAppConfigChangedMessage>(new AudioAppConfigChangedMessage(this, config));
            }
            catch (Exception ex)
            {
                Tracing.Log("AudioPreferencesPresenter - SetAudioPreferences - Failed to set preferences: {0}", ex);
                View.AudioPreferencesError(ex);
            }
        }

        private void SetOutputDeviceAndSampleRate(SSPDevice device, int sampleRate)
        {
            try
            {                
                if(_peakFileService.IsLoading)
                    _peakFileService.Cancel();

                if(_playerService.State == SSPPlayerState.Playing)
                    _playerService.Stop();

                // Keep original settings, or reset to default if this fails?
                _playerService.Dispose();
                _playerService.InitDevice(device, sampleRate, AppConfigManager.Instance.Root.Audio.BufferSize, AppConfigManager.Instance.Root.Audio.UpdatePeriod);

                // No exception; this audio config should work, save settings
                AppConfigManager.Instance.Root.Audio.AudioDevice = device;
                AppConfigManager.Instance.Root.Audio.SampleRate = sampleRate;
                AppConfigManager.Instance.Save();        
        
                _messageHub.PublishAsync(new PlayerReinitializedMessage(this));
            }
            catch (Exception ex)
            {
                Tracing.Log("AudioPreferencesPresenter - SetOutputDeviceAndSampleRate - Failed: {0}", ex);
                View.AudioPreferencesError(ex);
            }
        }

        private void ResetAudioSettings()
        {
            try
            {
                AppConfigManager.Instance.Root.Audio.BufferSize = 1000;
                AppConfigManager.Instance.Root.Audio.UpdatePeriod = 100;
                var defaultDevice = new SSPDevice();
                SetOutputDeviceAndSampleRate(defaultDevice, 44100);
                RefreshPreferences();
            }
            catch (Exception ex)
            {
                Tracing.Log("AudioPreferencesPresenter - ResetAudioSettings - Failed: {0}", ex);
                View.AudioPreferencesError(ex);
            }
        }

        private bool CheckIfPlayerIsPlaying()
        {
            return _playerService.State == SSPPlayerState.Playing;
        }

        private void RefreshPreferences()
        {
            try
            {
                View.RefreshAudioPreferences(AppConfigManager.Instance.Root.Audio);
            } 
            catch (Exception ex)
            {
                Tracing.Log("AudioPreferencesPresenter - SetSampleRate - Failed to set sample rate: {0}", ex);
                View.AudioPreferencesError(ex);
            }
        }

	    private void RefreshAudioDevices()
	    {
            try
            {
                var devices = _playerService.GetOutputDevices();
                View.RefreshAudioDevices(devices);
            }
            catch (Exception ex)
            {
                Tracing.Log("AudioPreferencesPresenter - RefreshAudioDevices - Exception: {0}", ex);
                View.AudioPreferencesError(ex);
            }
	    }
	}
}
