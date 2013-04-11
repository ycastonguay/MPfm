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

        public EqualizerPresetsPresenter(MobileNavigationManager navigationManager, IPlayerService playerService)
		{	
            _navigationManager = navigationManager;
            _playerService = playerService;
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
            _playerService.BypassEQ();            
        }

        private void AddPreset()
        {
            var view = _navigationManager.CreateEqualizerPresetDetailsView();
            _navigationManager.PushDialogSubview("EqualizerPresets", view);
        }

        private void LoadPreset(string presetName)
        {
        }

        private void EditPreset(string presetName)
        {
        }

        private void DeletePreset(string presetName)
        {
        }

        private void RefreshPresets()
        {
        }
	}
}

