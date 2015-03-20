﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Linq;
using Sessions.MVP.Messages;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using TinyMessenger;
using Sessions.MVP.Bootstrap;
using System.IO;
using org.sessionsapp.player;

#if WINDOWSSTORE
using Windows.UI.Xaml;
#elif WINDOWS_PHONE
using System.Windows.Threading;
#endif

namespace Sessions.MVP.Presenters
{
	public class EqualizerPresetsPresenter : BasePresenter<IEqualizerPresetsView>, IEqualizerPresetsPresenter
	{
        private readonly NavigationManager _navigationManager;
        private readonly MobileNavigationManager _mobileNavigationManager;
        private readonly ITinyMessengerHub _messageHub;
        private readonly IPlayerService _playerService;
        private readonly ILibraryService _libraryService;
        private Guid _selectedPresetId = Guid.Empty;

#if WINDOWS_PHONE
        private System.Windows.Threading.DispatcherTimer _timerOutputMeter = null;
#elif WINDOWSSTORE
        private Windows.UI.Xaml.DispatcherTimer _timerOutputMeter = null;
#else
        private System.Timers.Timer _timerOutputMeter = null;
#endif

        public EqualizerPresetsPresenter(ITinyMessengerHub messageHub, IPlayerService playerService, ILibraryService libraryService)
		{	
            _messageHub = messageHub;
            _playerService = playerService;
            _libraryService = libraryService;

#if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
            _timerOutputMeter = new System.Timers.Timer();         
            _timerOutputMeter.Interval = 40;
            _timerOutputMeter.Elapsed += HandleOutputMeterTimerElapsed;
#else
            _timerOutputMeter = new DispatcherTimer();
            _timerOutputMeter.Interval = new TimeSpan(0, 0, 0, 0, 40);
            _timerOutputMeter.Tick += HandleOutputMeterTimerElapsed;
#endif

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
		}

        public override void BindView(IEqualizerPresetsView view)
        {
            base.BindView(view);

            view.OnBypassEqualizer = BypassEqualizer;
            view.OnSetVolume = SetVolume;
            view.OnAddPreset = AddPreset;
            view.OnLoadPreset = LoadPreset;
            view.OnEditPreset = EditPreset;
            view.OnDeletePreset = DeletePreset;
            view.OnDuplicatePreset = DuplicatePreset;
            view.OnExportPreset = ExportPreset;

            _messageHub.Subscribe<EqualizerPresetUpdatedMessage>((EqualizerPresetUpdatedMessage m) => {
                RefreshPresets();
            });
            _messageHub.Subscribe<PlayerStatusMessage>((PlayerStatusMessage m) => {
                switch(m.State)
                {
                    case SSPPlayerState.Playing:
                        _timerOutputMeter.Start();
                        break;
                    case SSPPlayerState.Paused:
                    case SSPPlayerState.Stopped:
                        _timerOutputMeter.Stop();
                        break;
                }
            });

            if (_playerService.State == SSPPlayerState.Playing)
                _timerOutputMeter.Start();

            _selectedPresetId = _playerService.EQPreset != null ? _playerService.EQPreset.EQPresetId : Guid.Empty;
            RefreshPresets();
            View.RefreshVolume(_playerService.Volume);
        }

        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        private void HandleOutputMeterTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        #else
        private void HandleOutputMeterTimerElapsed(object sender, object eventArgs)
        #endif
        {
            try
            {
                if (_playerService.Mixer.UseFloatingPoint)
                {
                    Tuple<float[], float[]> data = _playerService.GetFloatingPointMixerData(0.02);
                    View.RefreshOutputMeter(data.Item1, data.Item2);
                }
                else
                {
                    Tuple<short[], short[]> data = _playerService.GetMixerData(0.02);

                    // Convert to floats (TODO: Try to optimize this. I'm sure there's a clever way to do this faster.
                    float[] left = new float[data.Item1.Length];
                    float[] right = new float[data.Item1.Length];
                    for (int a = 0; a < data.Item1.Length; a++)
                    {
                        // The values are already negative to positive, it's just a matter of dividing the value by the max value to get it to -1/+1.
                        left[a] = (float)data.Item1[a] / (float)Int16.MaxValue;
                        right[a] = (float)data.Item2[a] / (float)Int16.MaxValue;
                        //Console.WriteLine("EQPresetPresenter - a: {0} value: {1} newValue: {2}", a, data.Item1[a], left[a]);
                    }

                    View.RefreshOutputMeter(left, right);
                }
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
            }
        }

        private void BypassEqualizer()
        {
            try
            {
                _playerService.EnableEQ(!_playerService.IsEQEnabled);
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetsError(ex);
            }
        }

        private void SetVolume(float volume)
        {
            try
            {
                _playerService.Volume = volume;
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetsError(ex);
            }
        }

        private void AddPreset()
        {
            try
            {
                var newPreset = new SSPEQPreset();
                _libraryService.InsertEQPreset(newPreset);
                _playerService.ApplyEQPreset(newPreset);
                RefreshPresets();

                if (_mobileNavigationManager != null)
                {
                    // Mobile devices
                    _mobileNavigationManager.CreateEqualizerPresetDetailsView(View, newPreset.EQPresetId);
                }
                else
                {
                    // Desktop devices
                    _messageHub.PublishAsync<EqualizerPresetSelectedMessage>(new EqualizerPresetSelectedMessage(this, newPreset.EQPresetId));
                }
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetsError(ex);
            }
        }

        private void LoadPreset(Guid presetId)
        {
            try
            {
                var preset = _libraryService.SelectEQPreset(presetId);
                if(preset != null)
                {
                    _selectedPresetId = preset.EQPresetId;
                    _playerService.ApplyEQPreset(preset);
                }
                else
                {
                    _selectedPresetId = Guid.Empty;
                }
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetsError(ex);
            }
        }

        private void EditPreset(Guid presetId)
        {
            try
            {
                var preset = _libraryService.SelectEQPreset(presetId);
                if(preset == null)
                    return;

                _messageHub.PublishAsync<EqualizerPresetSelectedMessage>(new EqualizerPresetSelectedMessage(this, preset.EQPresetId));

                if(_mobileNavigationManager != null)
                    _mobileNavigationManager.CreateEqualizerPresetDetailsView(View, preset.EQPresetId);
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetsError(ex);
            }
        }

        private void DeletePreset(Guid presetId)
        {
            try
            {
                _libraryService.DeleteEQPreset(presetId);
                if(_playerService.EQPreset != null && _playerService.EQPreset.EQPresetId == presetId)
                    _playerService.ResetEQ();
                RefreshPresets();
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetsError(ex);
            }
        }

        private void DuplicatePreset(Guid presetId)
        {
            try
            {
                var preset = _libraryService.SelectEQPreset(presetId);
                preset.EQPresetId = Guid.NewGuid();
                preset.Name = "Copy of " + preset.Name;
                _libraryService.InsertEQPreset(preset);
                RefreshPresets();
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetsError(ex);
            }
        }

        private void ExportPreset(Guid presetId, string filePath)
        {
            try
            {
                var preset = _libraryService.SelectEQPreset(presetId);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(preset);
                using (TextWriter textWriter = new StreamWriter(filePath))
                {
                    textWriter.Write(json);
                }
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetsError(ex);
            }
        }

        private void RefreshPresets()
        {
            try
            {
                var presets = _libraryService.SelectEQPresets().OrderBy(x => x.Name).ToList();
                View.RefreshPresets(presets, _selectedPresetId, _playerService.IsEQEnabled);
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetsError(ex);
            }
        }
	}
}

