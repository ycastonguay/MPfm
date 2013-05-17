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
using System.Linq;
using System.Timers;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	public class EqualizerPresetsPresenter : BasePresenter<IEqualizerPresetsView>, IEqualizerPresetsPresenter
	{
        readonly MobileNavigationManager _navigationManager;
        readonly ITinyMessengerHub _messageHub;
        readonly IPlayerService _playerService;
        readonly ILibraryService _libraryService;
        Timer _timerOutputMeter;

        public EqualizerPresetsPresenter(MobileNavigationManager navigationManager, ITinyMessengerHub messageHub, IPlayerService playerService, ILibraryService libraryService)
		{	
            _navigationManager = navigationManager;
            _messageHub = messageHub;
            _playerService = playerService;
            _libraryService = libraryService;
            _timerOutputMeter = new Timer();         
            _timerOutputMeter.Interval = 20;
            _timerOutputMeter.Elapsed += HandleOutputMeterTimerElapsed;
		}

        public override void BindView(IEqualizerPresetsView view)
        {
            base.BindView(view);

            view.OnBypassEqualizer = BypassEqualizer;
            view.OnAddPreset = AddPreset;
            view.OnLoadPreset = LoadPreset;
            view.OnEditPreset = EditPreset;
            view.OnDeletePreset = DeletePreset;

            _messageHub.Subscribe<EqualizerPresetUpdatedMessage>((EqualizerPresetUpdatedMessage m) => {
                RefreshPresets();
            });
            _messageHub.Subscribe<PlayerStatusMessage>((PlayerStatusMessage m) => {
                switch(m.Status)
                {
                    case PlayerStatusType.Playing:
                        _timerOutputMeter.Start();
                        break;
                    case PlayerStatusType.Paused:
                        _timerOutputMeter.Stop();
                        break;
                    case PlayerStatusType.Stopped:
                        _timerOutputMeter.Stop();
                        break;
                }
            });

            if (_playerService.IsPlaying)
                _timerOutputMeter.Start();

            RefreshPresets();
        }

        private void HandleOutputMeterTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //Tuple<float[], float[]> data = _playerService.GetMixerData(0.02);
                //View.RefreshOutputMeter(data.Item1, data.Item2);
            }
            catch(Exception ex)
            {
                // Log a soft error
                Console.WriteLine("EqualizerPresetsPresenter - Error fetching output meter data: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void BypassEqualizer()
        {
            try
            {
                _playerService.BypassEQ();            
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while bypassing the equalizer: " + ex.Message);
                View.EqualizerPresetsError(ex);
            }
        }

        private void AddPreset()
        {
            try
            {
                var view = _navigationManager.CreateEqualizerPresetDetailsView(new EQPreset());
                _navigationManager.PushDialogSubview("Equalizer Presets", view);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while adding an equalizer preset: " + ex.Message);
                View.EqualizerPresetsError(ex);
            }
        }

        private void LoadPreset(Guid presetId)
        {
            try
            {
                EQPreset preset = _libraryService.SelectEQPreset(presetId);
                if(preset != null)
                    _playerService.ApplyEQPreset(preset);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while loading an equalizer preset: " + ex.Message);
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

                var view = _navigationManager.CreateEqualizerPresetDetailsView(preset);
                _navigationManager.PushDialogSubview("Equalizer Presets", view);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while editing an equalizer preset: " + ex.Message);
                View.EqualizerPresetsError(ex);
            }
        }

        private void DeletePreset(Guid presetId)
        {
            try
            {
                _libraryService.DeleteEQPreset(presetId);
                if(_playerService.EQPreset.EQPresetId == presetId)
                    _playerService.ResetEQ();
                RefreshPresets();
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while deleting an equalizer preset: " + ex.Message);
                View.EqualizerPresetsError(ex);
            }
        }

        private void RefreshPresets()
        {
            try
            {
                var presets = _libraryService.SelectEQPresets().OrderBy(x => x.Name).ToList();
                Guid selectedPresetId = (_playerService.EQPreset != null) ? _playerService.EQPreset.EQPresetId : Guid.Empty;
                View.RefreshPresets(presets, selectedPresetId, _playerService.IsEQBypassed);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while refreshing equalizer presets: " + ex.Message);
                View.EqualizerPresetsError(ex);
            }
        }
	}
}

