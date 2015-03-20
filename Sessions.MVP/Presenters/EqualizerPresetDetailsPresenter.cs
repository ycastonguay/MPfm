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
using System.Linq;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Services.Interfaces;
using Sessions.MVP.Views;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using TinyMessenger;
using Sessions.MVP.Messages;
using org.sessionsapp.player;

namespace Sessions.MVP.Presenters
{
	public class EqualizerPresetDetailsPresenter : BasePresenter<IEqualizerPresetDetailsView>, IEqualizerPresetDetailsPresenter
	{
        private readonly IPlayerService _playerService;
        private readonly ILibraryService _libraryService;
        private readonly ITinyMessengerHub _messageHub;
        private SSPEQPreset _preset;
        private SSPEQPreset _originalPreset;
        private Guid _presetId;

        public EqualizerPresetDetailsPresenter(Guid presetId, ITinyMessengerHub messageHub, IPlayerService playerService, ILibraryService libraryService)
        {
            _presetId = presetId;
            _messageHub = messageHub;
            _playerService = playerService;
            _libraryService = libraryService;
		}

        public override void BindView(IEqualizerPresetDetailsView view)
        {
            base.BindView(view);

            view.OnChangePreset = ChangePreset;
            view.OnNormalizePreset = NormalizePreset;
            view.OnResetPreset = ResetPreset;
            view.OnSavePreset = SavePreset;
            view.OnSetFaderGain = SetFaderGain;
            view.OnRevertPreset = RevertPreset;

            LoadInitialPreset();

            if(_preset != null)
            {
                _playerService.ApplyEQPreset(_preset);
                View.RefreshPreset(_preset);
            }

            _messageHub.Subscribe<EqualizerPresetSelectedMessage>((m) =>
            {
                ChangePreset(m.EQPresetId);
                View.RefreshPreset(_preset);
            });
        }

        private void LoadInitialPreset()
        {
            if (_presetId != Guid.Empty)
                ChangePreset(_presetId);
            else
                ChangePreset(_playerService.EQPreset);
        }

        public void ChangePreset(Guid equalizerPresetId)
        {
            var preset = _libraryService.SelectEQPreset(equalizerPresetId);
            if (preset == null)
                return;

            ChangePreset(preset);
            View.RefreshPreset(preset);
        }

        public void ChangePreset(SSPEQPreset preset)
        {
            _preset = preset;
            _originalPreset = preset;
        }

        public void RevertPreset()
        {
            try
            {
                _preset = _originalPreset;
                _playerService.ApplyEQPreset(_preset);
                View.RefreshPreset(_preset);
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetDetailsError(ex);
            }
        }

        public void NormalizePreset()
        {
            try
            {
                float highestValue = -6f;
                float value = 0;

                // Try to find the highest value in all bands
                for (int a = 0; a < _preset.Bands.Length; a++)
                {
                    var band = _preset.Bands[a];
                    value = _preset.Bands[a].Gain;
                    if (value > highestValue)
                        highestValue = value;
                }

                // Normalize bands
                foreach (var band in _preset.Bands)
                    band.Gain = band.Gain - highestValue;

                View.RefreshPreset(_preset);
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetDetailsError(ex);
            }
        }

        public void ResetPreset()
        {
            try
            {
                _preset.Reset();
                _playerService.ResetEQ();
                View.RefreshPreset(_preset);
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetDetailsError(ex);
            }
        }

        public void SavePreset(string presetName)
        {
            try
            {
                if(string.IsNullOrEmpty(presetName))
                {
                    View.EqualizerPresetDetailsError(new Exception("The preset name cannot be empty!"));
                    return;
                }

                _preset.Name = presetName;
                var preset = _libraryService.SelectEQPreset(_preset.EQPresetId);
                if(preset == null)
                    _libraryService.InsertEQPreset(_preset);
                else
                    _libraryService.UpdateEQPreset(_preset);

                _messageHub.PublishAsync<EqualizerPresetUpdatedMessage>(new EqualizerPresetUpdatedMessage(this){
                    EQPresetId = _preset.EQPresetId
                });

                //View.ShowMessage("Equalizer Preset", "Preset saved successfully.");
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetDetailsError(ex);
            }
        }

        public void SetFaderGain(string frequency, float gain)
        {
            try
            {
                var band = _preset.Bands.FirstOrDefault(x => x.Label == frequency);
                band.Gain = gain;
                int index = _preset.Bands.ToList().FindIndex(x => x.Label == frequency);

                if(index >= 0) 
                    _playerService.UpdateEQBand(index, gain, true);
            }
            catch(Exception ex)
            {
                Tracing.Log(ex);
                View.EqualizerPresetDetailsError(ex);
            }
        }       
	}
}

