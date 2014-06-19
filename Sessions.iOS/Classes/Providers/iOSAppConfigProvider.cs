// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using Sessions.MVP.Config;
using Sessions.MVP.Config.Providers;
using System.Reflection;
using System.Diagnostics;
using MonoTouch.Foundation;
using Sessions.MVP.Config.Models;
using System;

namespace Sessions.iOS.Classes.Providers
{
	public class iOSAppConfigProvider : IAppConfigProvider
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
            var keyStore = NSUbiquitousKeyValueStore.DefaultStore;
            var propertyInfos = config.GetType().GetTypeInfo().DeclaredProperties;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                string fullName = keyPreset + propertyInfo.Name;

                if (propertyType == typeof(int))
                {
                    propertyInfo.SetValue(config, (int)keyStore.GetLong(fullName));
                }
                else if (propertyType == typeof(bool))
                {
                    propertyInfo.SetValue(config, keyStore.GetBool(fullName));
                }
                else if (propertyType == typeof(double))
                {
                    propertyInfo.SetValue(config, keyStore.GetDouble(fullName));
                }
                else if (propertyType == typeof(float))
                {
                    propertyInfo.SetValue(config, (float)keyStore.GetDouble(fullName));
                }
                else if (propertyType == typeof(string))
                {
                    propertyInfo.SetValue(config, keyStore.GetString(fullName));
                }
                else if (propertyType == typeof(DateTime))
                {
                    long ticks = keyStore.GetLong(fullName);
                    DateTime dateTime = new DateTime(ticks);
                    propertyInfo.SetValue(config, dateTime);
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
            var keyStore = NSUbiquitousKeyValueStore.DefaultStore;
            var propertyInfos = config.GetType().GetTypeInfo().DeclaredProperties;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                string fullName = keyPreset + propertyInfo.Name;
                object value = propertyInfo.GetValue(config);

                if (propertyType == typeof(int))
                {
                    keyStore.SetLong(fullName, (long)(int)value);
                }
                else if (propertyType == typeof(bool))
                {
                    keyStore.SetBool(fullName, (bool)value);
                }
                else if (propertyType == typeof(double) || (propertyType == typeof(float)))
                {
                    keyStore.SetDouble(fullName, System.Convert.ToDouble(value));
                }
                else if (propertyType == typeof(string))
                {
                    string stringValue = value == null ? string.Empty : (string)value;
                    keyStore.SetString(fullName, stringValue);
                }
                else if (propertyType == typeof(DateTime))
                {
                    DateTime dateTime = (DateTime)value;
                    keyStore.SetLong(fullName, dateTime.Ticks);
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
