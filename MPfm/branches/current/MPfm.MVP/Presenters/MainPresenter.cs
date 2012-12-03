//
// MainPresenter.cs: Main window presenter.
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
	/// Main window presenter.
	/// </summary>
	public class MainPresenter : BasePresenter<IMainView>, IMainPresenter
	{
		#region Constructor and Dispose

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.MVP.EffectsPresenter"/> class.
        /// </summary>
        /// <param name='playerService'>
        /// Player service.
        /// </param>
		public MainPresenter()
		{
            
		}

		#endregion
        
        public override void BindView(IMainView view)
        {            
            // Subscribe to view actions
            view.OnOpenPlaylistWindow = OpenPlaylistWindow;
            view.OnOpenEffectsWindow = OpenEffectsWindow;
            view.OnOpenPreferencesWindow = OpenPreferencesWindow;
            
            base.BindView(view);
        }
        
        void OpenPlaylistWindow()
        {
        }

        void OpenEffectsWindow()
        {
        }

        void OpenPreferencesWindow()
        {
            NavigationManager.CreatePreferencesWindow();
        }
        
    }
}

