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
using MPfm.Player.Objects;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;

namespace MPfm.MVP.Presenters
{
	public class EqualizerPresetsPresenter : BasePresenter<IEqualizerPresetsView>, IEqualizerPresetsPresenter
	{
        readonly MobileNavigationManager _navigationManager;
        readonly IPlayerService _playerService;
        readonly ILibraryService _libraryService;
        List<EQPreset> _presets;

        public EqualizerPresetsPresenter(MobileNavigationManager navigationManager, IPlayerService playerService, ILibraryService libraryService)
		{	
            _navigationManager = navigationManager;
            _playerService = playerService;
            _libraryService = libraryService;
		}

        public override void BindView(IEqualizerPresetsView view)
        {
            base.BindView(view);

            view.OnBypassEqualizer = BypassEqualizer;
            view.OnAddPreset = AddPreset;
            view.OnLoadPreset = LoadPreset;
            view.OnEditPreset = EditPreset;
            view.OnDeletePreset = DeletePreset;
            
            RefreshPresets();
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
                var view = _navigationManager.CreateEqualizerPresetDetailsView();
                _navigationManager.PushDialogSubview("EqualizerPresets", view);
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
                EQPreset preset = _presets.FirstOrDefault(x => x.EQPresetId == presetId);
                if(preset != null)
                {
                    _playerService.ApplyEQPreset(preset);
                }
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
                var view = _navigationManager.CreateEqualizerPresetDetailsView();
                _navigationManager.PushDialogSubview("EqualizerPresets", view);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while editing an equalizer preset: " + ex.Message);
                View.EqualizerPresetsError(ex);
            }
        }

        private void DeletePreset(Guid presetId)
        {
        }

        private void RefreshPresets()
        {
            try
            {
                _presets = _libraryService.SelectEQPresets().ToList();
                View.RefreshPresets(_presets);
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while refreshing equalizer presets: " + ex.Message);
                View.EqualizerPresetsError(ex);
            }
        }
	}
}

