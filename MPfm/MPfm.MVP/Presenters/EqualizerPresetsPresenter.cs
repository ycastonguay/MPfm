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
using TinyMessenger;
using MPfm.MVP.Messages;

namespace MPfm.MVP.Presenters
{
	public class EqualizerPresetsPresenter : BasePresenter<IEqualizerPresetsView>, IEqualizerPresetsPresenter
	{
        readonly MobileNavigationManager _navigationManager;
        readonly ITinyMessengerHub _messageHub;
        readonly IPlayerService _playerService;
        readonly ILibraryService _libraryService;

        public EqualizerPresetsPresenter(MobileNavigationManager navigationManager, ITinyMessengerHub messageHub, IPlayerService playerService, ILibraryService libraryService)
		{	
            _navigationManager = navigationManager;
            _messageHub = messageHub;
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

            _messageHub.Subscribe<EqualizerPresetUpdatedMessage>((EqualizerPresetUpdatedMessage m) => {
                RefreshPresets();
            });
            
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

