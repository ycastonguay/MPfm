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

using System.Diagnostics;
using System.Reflection;
using Windows.Storage;
using MPfm.MVP.Config;
using MPfm.MVP.Config.Providers;

namespace MPfm.WindowsStore.Classes.Providers
{
    public class WindowsStoreAppConfigProvider : IAppConfigProvider
    {
        public RootAppConfig Load(string filePath)
        {
            var config = new RootAppConfig();
            LoadRecursive(config, "Root.");
            return config;
        }

        public void Save(string filePath, RootAppConfig config)
        {
            SaveRecursive(config, "Root.");
        }

        private void LoadRecursive(IAppConfig config, string keyPreset)
        {
            // Create map by scanning properties
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            var propertyInfos = config.GetType().GetTypeInfo().DeclaredProperties;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                string fullName = keyPreset + propertyInfo.Name;
                //object value = propertyInfo.GetValue(config);
                bool isAssignable = typeof(IAppConfig).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo());
                Debug.WriteLine("{0} - {1} - isAssignable: {2}", fullName, propertyInfo.PropertyType.Name, isAssignable);

                if (propertyType == typeof(int) ||
                    propertyType == typeof(bool) ||
                    propertyType == typeof(double) ||
                    propertyType == typeof(float) ||
                    propertyType == typeof(string))
                {
                    object value = roamingSettings.Values[fullName];
                    propertyInfo.SetValue(config, value);
                    Debug.WriteLine("Load setting: {0} - {1} - {2}", fullName, propertyInfo.PropertyType.Name, value);
                }
                else if (typeof(IAppConfig).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo()))
                {
                    var subConfig = (IAppConfig)propertyInfo.GetValue(config);
                    LoadRecursive(subConfig, keyPreset + propertyType.Name + ".");
                }
            }            
        }

        private void SaveRecursive(IAppConfig config, string keyPreset)
        {
            // Create map by scanning properties
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            var propertyInfos = config.GetType().GetTypeInfo().DeclaredProperties;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                string fullName = keyPreset + propertyInfo.Name;
                object value = propertyInfo.GetValue(config);
                bool isAssignable = typeof (IAppConfig).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo());
                Debug.WriteLine("{0} - {1} - isAssignable: {2}", fullName, propertyInfo.PropertyType.Name, isAssignable);

                if (propertyType == typeof (int) ||
                    propertyType == typeof (bool) ||
                    propertyType == typeof (double) ||
                    propertyType == typeof (float) ||
                    propertyType == typeof (string))
                {
                    Debug.WriteLine("Save setting: {0} - {1} - {2}", fullName, propertyInfo.PropertyType.Name, value);
                    roamingSettings.Values[fullName] = value;
                }
                else if (typeof (IAppConfig).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo()))
                {
                    var subConfig = (IAppConfig)propertyInfo.GetValue(config);
                    SaveRecursive(subConfig, keyPreset + propertyType.Name + ".");
                }
            }
        }
    }
}
