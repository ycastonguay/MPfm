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

using MPfm.Core.Helpers;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Config.Models;
using MPfm.MVP.Config.Providers;
using MPfm.MVP.Helpers;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Config
{
    /// <summary>
    /// Singleton containing all application settings.
    /// </summary>
    public class AppConfigManager
    {
        private readonly IAppConfigProvider _provider;
        public RootAppConfig Root { get; private set; }

        #region Singleton

        private static AppConfigManager instance;
        /// <summary>
        /// AppConfigManager instance.
        /// </summary>
        public static AppConfigManager Instance
        {
            get
            {
                if(instance == null)
                    instance = new AppConfigManager();
                return instance;
            }
        }

        #endregion

        public AppConfigManager()
        {
            _provider = Bootstrapper.GetContainer().Resolve<IAppConfigProvider>();
            Root = new RootAppConfig();
        }

        public void Load()
        {
            Root = _provider.Load(PathHelper.ConfigurationFilePath);

            // Fix any important values that could have been corrupted for some reason
            if (Root.Library.SyncServicePort < 80 || Root.Library.SyncServicePort > 65535)
                Root.Library.SyncServicePort = 53551;
            if (Root.General.SongPositionUpdateFrequency < 10 || Root.General.SongPositionUpdateFrequency > 100)
                Root.General.SongPositionUpdateFrequency = 20;
            if (Root.General.OutputMeterUpdateFrequency < 10 || Root.General.OutputMeterUpdateFrequency > 100)
                Root.General.OutputMeterUpdateFrequency = 20;
            if (Root.General.MaximumPeakFolderSize < 10 || Root.General.OutputMeterUpdateFrequency > 1000)
                Root.General.MaximumPeakFolderSize = 100;
       }

        public void Save()
        {
            _provider.Save(PathHelper.ConfigurationFilePath, Root);
        }
    }
}
