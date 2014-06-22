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

using System;
using System.Reflection;
using Android.Content;
using Android.Preferences;
using Sessions.MVP.Config.Models;
using Sessions.MVP.Config.Providers;

namespace Sessions.Android.Classes.Providers
{
    public class AndroidAppConfigProvider : IAppConfigProvider
    {
        private Context _context;

        public AndroidAppConfigProvider()
        {
            _context = SessionsApplication.GetApplicationContext();
        }

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
            var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(_context);
            var propertyInfos = config.GetType().GetTypeInfo().DeclaredProperties;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                string fullName = keyPreset + propertyInfo.Name;
                bool isAssignable = typeof(IAppConfig).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo());
                //Debug.WriteLine("{0} - {1} - isAssignable: {2}", fullName, propertyInfo.PropertyType.Name, isAssignable);

                if (propertyType == typeof(int))
                {
                    propertyInfo.SetValue(config, sharedPreferences.GetInt(fullName, 0));
                }
                else if (propertyType == typeof(bool))
                {
                    propertyInfo.SetValue(config, sharedPreferences.GetBoolean(fullName, false));
                }
                else if (propertyType == typeof(double))
                {
                    propertyInfo.SetValue(config, sharedPreferences.GetFloat(fullName, 0));
                }
                else if (propertyType == typeof(float))
                {
                    propertyInfo.SetValue(config, sharedPreferences.GetFloat(fullName, 0));
                }
                else if (propertyType == typeof(string))
                {                    
                    propertyInfo.SetValue(config, sharedPreferences.GetString(fullName, string.Empty));
                }
                else if (propertyType == typeof(DateTime))
                {
                    long ticks = sharedPreferences.GetLong(fullName, 0);
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
            var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(_context);
            var editor = sharedPreferences.Edit();
            var propertyInfos = config.GetType().GetTypeInfo().DeclaredProperties;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                var propertyType = propertyInfo.PropertyType;
                string fullName = keyPreset + propertyInfo.Name;
                object value = propertyInfo.GetValue(config);
                bool isAssignable = typeof(IAppConfig).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo());
                //Debug.WriteLine("{0} - {1} - isAssignable: {2}", fullName, propertyInfo.PropertyType.Name, isAssignable);

                if (propertyType == typeof(int))
                {
                    editor.PutInt(fullName, (int) value);
                }
                else if (propertyType == typeof(bool))
                {
                    editor.PutBoolean(fullName, (bool)value);
                }
                else if (propertyType == typeof(double) || (propertyType == typeof(float)))
                {
                    editor.PutFloat(fullName, (float)value);
                }
                else if (propertyType == typeof(string))
                {
                    string stringValue = value == null ? string.Empty : (string)value;
                    editor.PutString(fullName, stringValue);
                }
                else if (propertyType == typeof(DateTime))
                {
                    DateTime dateTime = (DateTime) value;
                    editor.PutLong(fullName, dateTime.Ticks);
                }
                else if (typeof(IAppConfig).GetTypeInfo().IsAssignableFrom(propertyType.GetTypeInfo()))
                {
                    var subConfig = (IAppConfig)propertyInfo.GetValue(config);
                    SaveRecursive(subConfig, keyPreset + propertyType.Name + ".");
                }
            }

            editor.Commit();
        }
    }
}
