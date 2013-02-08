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

namespace MPfm
{
    /// <summary>
    /// Interface for the MPfmConfiguration class.
    /// </summary>
    interface IMPfmConfiguration
    {
        AudioConfigurationSection Audio { get; }        
        ControlsConfigurationSection Controls { get; }
        GeneralConfigurationSection General { get; }
        MPfm.WindowsConfigurationSection Windows { get; }

        void Clear();
        string GetKeyValue(string name);
        T? GetKeyValueGeneric<T>(string name) where T : struct;
        void Load();
        void RefreshXML();
        void Save();
        void Save(string filePath);
        void SetKeyValue<T>(string name, T value);        
    }
}
