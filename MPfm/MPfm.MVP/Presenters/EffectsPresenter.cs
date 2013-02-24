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

using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Effects presenter.
	/// </summary>
	public class EffectsPresenter : BasePresenter<IEffectsView>, IEffectsPresenter
	{
		// Private variables
		//IEffectsView view = null;
        readonly IPlayerService playerService;

		#region Constructor and Dispose

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectsPresenter"/> class.
        /// </summary>
        /// <param name='playerService'>
        /// Player service.
        /// </param>
		public EffectsPresenter(IPlayerService playerService)
		{	
            // Set properties
            this.playerService = playerService;
		}

		#endregion

        public void SetEQParam(int index, float value)
        {
            // Set EQ and update UI
            playerService.UpdateEQBand(index, value, true);
            View.UpdateFader(index, value);
        }

        public void BypassEQ()
        {
            playerService.BypassEQ();
        }

        public void AutoLevel()
        {
        }

        public void Reset()
        {
            playerService.ResetEQ();
            for (int a = 0; a < 18; a++)
                View.UpdateFader(a, 0);
        }

        public void LoadPreset(string presetName)
        {
        }

        public void SavePreset(string presetName)
        {
        }

        public void DeletePreset(string presetName)
        {
        }
	}
}

