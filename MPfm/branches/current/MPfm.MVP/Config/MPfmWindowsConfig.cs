//
// MPfmWindowsConfig.cs: Class containing settings for all windows for MPfm.
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
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.MVP
{
    /// <summary>
    /// Class containing settings for all windows for MPfm.
    /// </summary>
    public class MPfmWindowsConfig
    {
        public MPfmWindowConfig MainWindow { get; private set; }
        public MPfmWindowConfig PlaylistWindow { get; private set; }
        public MPfmWindowConfig EffectsWindow { get; private set; }
        public MPfmWindowConfig PreferencesWindow { get; private set; }

        public MPfmWindowsConfig()
        {
            // Set defaults
            MainWindow = new MPfmWindowConfig();
            PlaylistWindow = new MPfmWindowConfig();
            EffectsWindow = new MPfm.MVP.MPfmWindowConfig();
            PreferencesWindow = new MPfm.MVP.MPfmWindowConfig();
        }
    }
}
