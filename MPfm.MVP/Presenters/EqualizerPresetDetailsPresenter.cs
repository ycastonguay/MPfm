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
using MPfm.Core;
using MPfm.Player.Objects;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;
using MPfm.MVP.Messages;
using MPfm.Library.Services.Interfaces;

namespace MPfm.MVP.Presenters
{
	public class EqualizerPresetDetailsPresenter : BasePresenter<IEqualizerPresetDetailsView>, IEqualizerPresetDetailsPresenter
	{
        private readonly IPlayerService _playerService;
        private readonly ILibraryService _libraryService;
        private readonly ITinyMessengerHub _messageHub;
        private EQPreset _preset;
        private List<EQPresetBand> _originalPresetBands;

        public EqualizerPresetDetailsPresenter(Guid presetId, ITinyMessengerHub messageHub, IPlayerService playerService, ILibraryService libraryService)
		{	
            _messageHub = messageHub;
            _playerService = playerService;
            _libraryService = libraryService;

            if (presetId == Guid.Empty)
                ChangePreset(new EQPreset());
            else
                ChangePreset(presetId);
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

        public void ChangePreset(Guid equalizerPresetId)
        {
            // Only used on desktop devices where the EqualizerPreset and EqualizerPresetDetails are merged into the same view
            var preset = _libraryService.SelectEQPreset(equalizerPresetId);
            if (preset == null)
                return;

            ChangePreset(preset);
            View.RefreshPreset(preset);
        }

        public void ChangePreset(EQPreset preset)
        {
            _preset = preset;

            // Clone band values to make sure we're not dealing with the same instance
            _originalPresetBands = new List<EQPresetBand>();
            foreach (var band in preset.Bands)
                _originalPresetBands.Add(new EQPresetBand(){
                    Bandwidth = band.Bandwidth,
                    Center = band.Center,
                    CenterString = band.CenterString,
                    //FXChannel = band.FXChannel,
                    Gain = band.Gain,
                    Q = band.Q
                });
        }

        public void RevertPreset()
        {
            try
            {
                for(int a = 0; a < _preset.Bands.Count; a++)
                {
                    _preset.Bands[a].Bandwidth = _originalPresetBands[a].Bandwidth;
                    _preset.Bands[a].Center = _originalPresetBands[a].Center;
                    _preset.Bands[a].CenterString = _originalPresetBands[a].CenterString;
                    //_preset.Bands[a].FXChannel = _originalPresetBands[a].FXChannel;
                    _preset.Bands[a].Gain = _originalPresetBands[a].Gain;
                    _preset.Bands[a].Q = _originalPresetBands[a].Q;
                }
                _playerService.ApplyEQPreset(_preset);
                View.RefreshPreset(_preset);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while reverting the equalizer preset: " + ex.Message);
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
                for (int a = 0; a < _preset.Bands.Count; a++)
                {
                    var band = _preset.Bands[a];
                    value = _preset.Bands[a].Gain;
                    Tracing.Log("NormalizePreset - band {0} value {1}", band, value);
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
                Tracing.Log("An error occured while normalizing the equalizer preset: " + ex.Message);
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
                Tracing.Log("An error occured while reseting the equalizer preset: " + ex.Message);
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

                View.ShowMessage("Equalizer Preset", "Preset saved successfully.");
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while saving the equalizer preset: " + ex.Message);
                View.EqualizerPresetDetailsError(ex);
            }
        }

        public void SetFaderGain(string frequency, float gain)
        {
            try
            {
                //Tracing.Log("EqualizerPresetDetailsPresenter - SetFaderGain - frequency: {0} gain: {1}", frequency, gain);
                var band = _preset.Bands.FirstOrDefault(x => x.CenterString == frequency);
                band.Gain = gain;
                int index = _preset.Bands.IndexOf(band);
                _playerService.UpdateEQBand(index, gain, true);
            }
            catch(Exception ex)
            {
                Tracing.Log("An error occured while setting the equalizer preset fader value: " + ex.Message);
                View.EqualizerPresetDetailsError(ex);
            }
        }       
	}
}

