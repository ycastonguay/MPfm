//
// EffectsPresenter.cs: Effects presenter.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using MPfm.Core;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using AutoMapper;

namespace MPfm.MVP
{
	/// <summary>
	/// Effects presenter.
	/// </summary>
	public class EffectsPresenter : BasePresenter<IEffectsView>, IEffectsPresenter
	{
		// Private variables
		//IEffectsView view = null;
        IPlayerService playerService;

		#region Constructor and Dispose

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.MVP.EffectsPresenter"/> class.
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
		
//		/// <summary>
//		/// Binds the view to its implementation.
//		/// </summary>
//		/// <param name='view'>Effects view implementation</param>		
//		public void BindView(IEffectsView view)
//		{
//			// Validate parameters
//			if(view == null)			
//				throw new ArgumentNullException("The view parameter is null!");			
//						
//			// Set properties
//			this.view = view;	
//		}

        public void SetEQParam(int index, float value)
        {
            // Set EQ and update UI
            playerService.Player.UpdateEQBand(index, value, true);
            View.UpdateFader(index, value);
        }

        public void BypassEQ()
        {
            playerService.Player.BypassEQ();
        }

        public void AutoLevel()
        {
        }

        public void Reset()
        {
            playerService.Player.ResetEQ();
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

