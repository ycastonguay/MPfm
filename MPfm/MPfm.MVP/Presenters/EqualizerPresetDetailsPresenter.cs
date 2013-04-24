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
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.MVP.Presenters
{
	public class EqualizerPresetDetailsPresenter : BasePresenter<IEqualizerPresetDetailsView>, IEqualizerPresetDetailsPresenter
	{
        readonly IPlayerService _playerService;
        readonly ILibraryService _libraryService;

        public EqualizerPresetDetailsPresenter(IPlayerService playerService, ILibraryService libraryService)
		{	
            _playerService = playerService;
            _libraryService = libraryService;
		}

        public override void BindView(IEqualizerPresetDetailsView view)
        {
            base.BindView(view);

            view.OnNormalizePreset = NormalizePreset;
            view.OnResetPreset = ResetPreset;
            view.OnSavePreset = SavePreset;
            
            ResetPreset();
        }

        public void NormalizePreset()
        {
            try
            {
                // TODO: Add from Windows code
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while normalizing the equalizer preset: " + ex.Message);
                View.EqualizerPresetDetailsError(ex);
            }
        }

        public void ResetPreset()
        {
            try
            {
                _playerService.ResetEQ();
                View.RefreshFaders(new List<KeyValuePair<string, float>>() {
                    new KeyValuePair<string, float>("55 Hz", 0),
                    new KeyValuePair<string, float>("77 Hz", 0),
                    new KeyValuePair<string, float>("110 Hz", 0),
                    new KeyValuePair<string, float>("156 Hz", 0),
                    new KeyValuePair<string, float>("220 Hz", 0),
                    new KeyValuePair<string, float>("311 Hz", 0),
                    new KeyValuePair<string, float>("440 Hz", 0),
                    new KeyValuePair<string, float>("622 Hz", 0),
                    new KeyValuePair<string, float>("880 Hz", 0),
                    new KeyValuePair<string, float>("1.2 kHz", 0),
                    new KeyValuePair<string, float>("1.8 kHz", 0),
                    new KeyValuePair<string, float>("2.5 kHz", 0),
                    new KeyValuePair<string, float>("3.5 kHz", 0),
                    new KeyValuePair<string, float>("5 kHz", 0),
                    new KeyValuePair<string, float>("7 kHz", 0),
                    new KeyValuePair<string, float>("10 kHz", 0),
                    new KeyValuePair<string, float>("14 kHz", 0),
                    new KeyValuePair<string, float>("20 kHz", 0)
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while reseting the equalizer preset: " + ex.Message);
                View.EqualizerPresetDetailsError(ex);
            }
        }

        public void SavePreset(EQPreset preset)
        {
            try
            {
                _libraryService.UpdateEQPreset(preset);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while saving the equalizer preset: " + ex.Message);
                View.EqualizerPresetDetailsError(ex);
            }
        }
	}
}

