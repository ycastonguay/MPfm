//
// MPFMConfig.cs: This class manages the application configuration in typed values.
//
// Copyright © 2011 Yanick Castonguay
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
using System.Configuration;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;

namespace MPfm
{
    /// <summary>
    /// This class manages the application configuration in typed values.
    /// </summary>
    public class MPFMConfig
    {
        private Configuration m_config = null;
        public Configuration Config
        {
            get
            {
                return m_config;
            }
        }

        /// <summary>
        /// Constructor for the MPFMConfig class.
        /// </summary>
        public MPFMConfig()
        {
            m_config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);                                
        }

        /// <summary>
        /// Private method that converts strings into nullable integers.
        /// </summary>
        /// <param name="s">String to convert</param>
        /// <returns>Nullable Integer</returns>
        private static int? ToNullableInt32(string s)
        {
            int i;
            if (Int32.TryParse(s, out i)) return i;
            return null;
        }

        /// <summary>
        /// This index property retuns the value of a configuration item.
        /// </summary>
        /// <param name="key">Configuration Key</param>
        /// <returns>Configuration Value</returns>
        private string this[string key]
        {
            get
            {
                try
                {

                    if (m_config.AppSettings.Settings[key] != null)
                    {
                        return m_config.AppSettings.Settings[key].Value;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }

                return null;
            }
            set
            {
                try
                {
                    if (m_config.AppSettings.Settings[key] != null)
                    {
                        m_config.AppSettings.Settings.Remove(key);
                    }

                    m_config.AppSettings.Settings.Add(key, value);
                    m_config.Save();
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Validates if a key exists or not.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True if the key exists</returns>
        public bool KeyExists(string key)
        {
            if (m_config.AppSettings.Settings[key] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public FMOD.OUTPUTTYPE Driver
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_Driver];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    Driver = KeyDefaultValue_Driver;
                    return KeyDefaultValue_Driver;
                }

                // Return value
                FMOD.OUTPUTTYPE outputType = FMOD.OUTPUTTYPE.UNKNOWN;

                // Analyse string to get value
                string driverString = this[Key_Driver].ToUpper();

                if (driverString.Contains("DSOUND"))
                {
                    outputType = FMOD.OUTPUTTYPE.DSOUND;
                }
                else if (driverString.Contains("WASAPI"))
                {
                    outputType = FMOD.OUTPUTTYPE.WASAPI;
                }
                else if(driverString.Contains("ASIO"))
                {
                    outputType = FMOD.OUTPUTTYPE.ASIO;
                }
                else if (driverString.Contains("WINMM"))
                {
                    outputType = FMOD.OUTPUTTYPE.WINMM;
                }
                else if (driverString.Contains("NOSOUND"))
                {
                    outputType = FMOD.OUTPUTTYPE.NOSOUND;
                }

                return outputType;
            }
            set
            {
                // Set value in config
                this[Key_Driver] = value.ToString();
            }
        }

        public bool FirstRun
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_FirstRun];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    FirstRun = KeyDefaultValue_FirstRun;
                    return KeyDefaultValue_FirstRun;
                }

                // Return value from config (return default if convert fails)
                bool bValue = KeyDefaultValue_FirstRun;
                bool.TryParse(this[Key_FirstRun], out bValue);
                return bValue;
            }
            set
            {
                // Set value in config
                this[Key_FirstRun] = value.ToString();
            }
        }
        public bool ShowTray
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_ShowTray];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    ShowTray = KeyDefaultValue_ShowTray;
                    return KeyDefaultValue_ShowTray;
                }

                // Return value from config (return default if convert fails)
                bool bValue = KeyDefaultValue_ShowTray;
                bool.TryParse(this[Key_ShowTray], out bValue);
                return bValue;
            }
            set
            {
                // Set value in config
                this[Key_ShowTray] = value.ToString();
            }
        }
        public bool HideTray
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_HideTray];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    HideTray = KeyDefaultValue_HideTray;
                    return KeyDefaultValue_HideTray;
                }

                // Return value from config (return default if convert fails)
                bool bValue = KeyDefaultValue_HideTray;
                bool.TryParse(this[Key_HideTray], out bValue);
                return bValue;
            }
            set
            {
                // Set value in config
                this[Key_HideTray] = value.ToString();
            }
        }
        
        public bool EQOn
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_EQOn];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    EQOn = KeyDefaultValue_EQOn;
                    return KeyDefaultValue_EQOn;
                }

                // Return value from config (return default if convert fails)
                bool bValue = KeyDefaultValue_EQOn;
                bool.TryParse(this[Key_EQOn], out bValue);
                return bValue;
            }
            set
            {
                // Set value in config
                this[Key_EQOn] = value.ToString();
            }
        }
        public string EQPreset
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_EQPreset];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    EQPreset = KeyDefaultValue_EQPreset;
                    return KeyDefaultValue_EQPreset;
                }

                /// Return value
                return this[Key_EQPreset];
            }
            set
            {
                // Set value in config
                this[Key_EQPreset] = value;
            }
        }

        public string RepeatType
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_RepeatType];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    RepeatType = KeyDefaultValue_RepeatType;
                    return KeyDefaultValue_RepeatType;
                }

                /// Return value
                return this[Key_RepeatType];
            }
            set
            {
                // Set value in config
                this[Key_RepeatType] = value;
            }
        }
        public int Volume
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_Volume]);
                if (value == null)
                {
                    // Set value to default value
                    Volume = KeyDefaultValue_Volume;
                    return KeyDefaultValue_Volume;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_Volume] = value.ToString();
            }
        }
        public int TimeShifting
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_TimeShifting]);
                if (value == null)
                {
                    // Set value to default value
                    TimeShifting = KeyDefaultValue_TimeShifting;
                    return KeyDefaultValue_TimeShifting;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_TimeShifting] = value.ToString();
            }
        }
        public string OutputDevice
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_OutputDevice];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    OutputDevice = KeyDefaultValue_OutputDevice;
                    return KeyDefaultValue_OutputDevice;
                }

                /// Return value
                return this[Key_OutputDevice];
            }
            set
            {
                // Set value in config
                this[Key_OutputDevice] = value;
            }
        }

        public string FilterSoundFormat
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_FilterSoundFormat];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    FilterSoundFormat = KeyDefaultValue_FilterSoundFormat;
                    return KeyDefaultValue_FilterSoundFormat;
                }

                /// Return value
                return this[Key_FilterSoundFormat];
            }
            set
            {
                // Set value in config
                this[Key_FilterSoundFormat] = value;
            }
        }
        public string SongQueryArtistName
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_SongQueryArtistName];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    SongQueryArtistName = KeyDefaultValue_SongQueryArtistName;
                    return KeyDefaultValue_SongQueryArtistName;
                }

                /// Return value
                return this[Key_SongQueryArtistName];
            }
            set
            {
                // Set value in config
                this[Key_SongQueryArtistName] = value;
            }
        }
        public string SongQueryAlbumTitle
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_SongQueryAlbumTitle];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    SongQueryAlbumTitle = KeyDefaultValue_SongQueryAlbumTitle;
                    return KeyDefaultValue_SongQueryAlbumTitle;
                }

                /// Return value
                return this[Key_SongQueryAlbumTitle];
            }
            set
            {
                // Set value in config
                this[Key_SongQueryAlbumTitle] = value;
            }
        }
        public string SongQuerySongId
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_SongQuerySongId];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    SongQuerySongId = KeyDefaultValue_SongQuerySongId;
                    return KeyDefaultValue_SongQuerySongId;
                }

                /// Return value
                return this[Key_SongQuerySongId];
            }
            set
            {
                // Set value in config
                this[Key_SongQuerySongId] = value;
            }
        }
        public string SongQueryPlaylistId
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_SongQueryPlaylistId];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    SongQueryPlaylistId = KeyDefaultValue_SongQueryPlaylistId;
                    return KeyDefaultValue_SongQueryPlaylistId;
                }

                /// Return value
                return this[Key_SongQueryPlaylistId];
            }
            set
            {
                // Set value in config
                this[Key_SongQueryPlaylistId] = value;
            }
        }
        public string CurrentNodeType
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_CurrentNodeType];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    CurrentNodeType = KeyDefaultValue_CurrentNodeType;
                    return KeyDefaultValue_CurrentNodeType;
                }

                /// Return value
                return this[Key_CurrentNodeType];
            }
            set
            {
                // Set value in config
                this[Key_CurrentNodeType] = value;
            }
        }

        public int WindowX
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_WindowX]);
                if (value == null)
                {                        
                    // Set value to default value
                    WindowX = KeyDefaultValue_WindowX;
                    return KeyDefaultValue_WindowX;
                }

                // Return value
                return value.Value;                                               
            }
            set
            {
                // Set value in config
                this[Key_WindowX] = value.ToString();
            }
        }
        public int WindowY
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_WindowY]);
                if (value == null)
                {
                    // Set value to default value
                    WindowY = KeyDefaultValue_WindowY;
                    return KeyDefaultValue_WindowY;
                }

                // Return value
                return value.Value;  
            }
            set
            {
                // Set value in config
                this[Key_WindowY] = value.ToString();
            }
        }
        public int WindowWidth
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_WindowWidth]);
                if (value == null)
                {
                    // Set value to default value
                    WindowWidth = KeyDefaultValue_WindowWidth;
                    return KeyDefaultValue_WindowWidth;
                }

                // Return value
                return value.Value;  
            }
            set
            {
                // Set value in config
                this[Key_WindowWidth] = value.ToString();
            }
        }
        public int WindowHeight
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_WindowHeight]);
                if (value == null)
                {
                    // Set value to default value
                    WindowHeight = KeyDefaultValue_WindowHeight;
                    return KeyDefaultValue_WindowHeight;
                }

                // Return value
                return value.Value;  
            }
            set
            {
                // Set value in config
                this[Key_WindowHeight] = value.ToString();
            }
        }
        public bool WindowMaximized
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_WindowMaximized];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    WindowMaximized = KeyDefaultValue_WindowMaximized;
                    return KeyDefaultValue_WindowMaximized;
                }

                // Return value from config (return default if convert fails)
                bool bValue = KeyDefaultValue_WindowMaximized;
                bool.TryParse(this[Key_WindowMaximized], out bValue);
                return bValue;
            }
            set
            {
                // Set value in config
                this[Key_WindowMaximized] = value.ToString();
            }
        }
        public int WindowSplitterDistance
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_WindowSplitterDistance]);
                if (value == null)
                {
                    // Set value to default value
                    WindowSplitterDistance = KeyDefaultValue_WindowSplitterDistance;
                    return KeyDefaultValue_WindowSplitterDistance;
                }

                // Return value
                return value.Value;  
            }
            set
            {
                // Set value in config
                this[Key_WindowSplitterDistance] = value.ToString();
            }
        }

        public int SongBrowserCol1Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_SongBrowserCol1Width]);
                if (value == null)
                {
                    // Set value to default value
                    SongBrowserCol1Width = KeyDefaultValue_SongBrowserCol1Width;
                    return KeyDefaultValue_SongBrowserCol1Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_SongBrowserCol1Width] = value.ToString();
            }
        }
        public int SongBrowserCol2Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_SongBrowserCol2Width]);
                if (value == null)
                {
                    // Set value to default value
                    SongBrowserCol2Width = KeyDefaultValue_SongBrowserCol2Width;
                    return KeyDefaultValue_SongBrowserCol2Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_SongBrowserCol2Width] = value.ToString();
            }
        }
        public int SongBrowserCol3Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_SongBrowserCol3Width]);
                if (value == null)
                {
                    // Set value to default value
                    SongBrowserCol3Width = KeyDefaultValue_SongBrowserCol3Width;
                    return KeyDefaultValue_SongBrowserCol3Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_SongBrowserCol3Width] = value.ToString();
            }
        }
        public int SongBrowserCol4Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_SongBrowserCol4Width]);
                if (value == null)
                {
                    // Set value to default value
                    SongBrowserCol4Width = KeyDefaultValue_SongBrowserCol4Width;
                    return KeyDefaultValue_SongBrowserCol4Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_SongBrowserCol4Width] = value.ToString();
            }
        }
        public int SongBrowserCol5Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_SongBrowserCol5Width]);
                if (value == null)
                {
                    // Set value to default value
                    SongBrowserCol5Width = KeyDefaultValue_SongBrowserCol5Width;
                    return KeyDefaultValue_SongBrowserCol5Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_SongBrowserCol5Width] = value.ToString();
            }
        }
        public int SongBrowserCol6Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_SongBrowserCol6Width]);
                if (value == null)
                {
                    // Set value to default value
                    SongBrowserCol6Width = KeyDefaultValue_SongBrowserCol6Width;
                    return KeyDefaultValue_SongBrowserCol6Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_SongBrowserCol6Width] = value.ToString();
            }
        }
        public int SongBrowserCol7Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_SongBrowserCol7Width]);
                if (value == null)
                {
                    // Set value to default value
                    SongBrowserCol7Width = KeyDefaultValue_SongBrowserCol7Width;
                    return KeyDefaultValue_SongBrowserCol7Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_SongBrowserCol7Width] = value.ToString();
            }
        }
        public int SongBrowserCol8Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_SongBrowserCol8Width]);
                if (value == null)
                {
                    // Set value to default value
                    SongBrowserCol8Width = KeyDefaultValue_SongBrowserCol8Width;
                    return KeyDefaultValue_SongBrowserCol8Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_SongBrowserCol8Width] = value.ToString();
            }
        }       

        public int PlaylistX
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistX]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistX = KeyDefaultValue_PlaylistX;
                    return KeyDefaultValue_PlaylistX;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistX] = value.ToString();
            }
        }
        public int PlaylistY
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistY]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistY = KeyDefaultValue_PlaylistY;
                    return KeyDefaultValue_PlaylistY;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistY] = value.ToString();
            }
        }
        public int PlaylistWidth
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistWidth]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistWidth = KeyDefaultValue_PlaylistWidth;
                    return KeyDefaultValue_PlaylistWidth;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistWidth] = value.ToString();
            }
        }
        public int PlaylistHeight
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistHeight]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistHeight = KeyDefaultValue_PlaylistHeight;
                    return KeyDefaultValue_PlaylistHeight;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistHeight] = value.ToString();
            }
        }
        public bool PlaylistMaximized
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_PlaylistMaximized];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    PlaylistMaximized = KeyDefaultValue_PlaylistMaximized;
                    return KeyDefaultValue_PlaylistMaximized;
                }

                // Return value from config (return default if convert fails)
                bool bValue = KeyDefaultValue_PlaylistMaximized;
                bool.TryParse(this[Key_PlaylistMaximized], out bValue);
                return bValue;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistMaximized] = value.ToString();
            }
        }
        public bool PlaylistVisible
        {
            get
            {
                // Get value. Value will be null if the key doesn't exists.
                string value = this[Key_PlaylistVisible];

                // Check if string is null                
                if (value == null)
                {
                    // Set value to default value
                    PlaylistMaximized = KeyDefaultValue_PlaylistVisible;
                    return KeyDefaultValue_PlaylistVisible;
                }

                // Return value from config (return default if convert fails)
                bool bValue = KeyDefaultValue_PlaylistVisible;
                bool.TryParse(this[Key_PlaylistVisible], out bValue);
                return bValue;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistVisible] = value.ToString();
            }
        }
        
        public int PlaylistCol1Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistCol1Width]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistCol1Width = KeyDefaultValue_PlaylistCol1Width;
                    return KeyDefaultValue_PlaylistCol1Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistCol1Width] = value.ToString();
            }
        }
        public int PlaylistCol2Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistCol2Width]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistCol2Width = KeyDefaultValue_PlaylistCol2Width;
                    return KeyDefaultValue_PlaylistCol2Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistCol2Width] = value.ToString();
            }
        }
        public int PlaylistCol3Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistCol3Width]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistCol3Width = KeyDefaultValue_PlaylistCol3Width;
                    return KeyDefaultValue_PlaylistCol3Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistCol3Width] = value.ToString();
            }
        }
        public int PlaylistCol4Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistCol4Width]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistCol4Width = KeyDefaultValue_PlaylistCol4Width;
                    return KeyDefaultValue_PlaylistCol4Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistCol4Width] = value.ToString();
            }
        }
        public int PlaylistCol5Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistCol5Width]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistCol5Width = KeyDefaultValue_PlaylistCol5Width;
                    return KeyDefaultValue_PlaylistCol5Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistCol5Width] = value.ToString();
            }
        }
        public int PlaylistCol6Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistCol6Width]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistCol6Width = KeyDefaultValue_PlaylistCol6Width;
                    return KeyDefaultValue_PlaylistCol6Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistCol6Width] = value.ToString();
            }
        }
        public int PlaylistCol7Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistCol7Width]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistCol7Width = KeyDefaultValue_PlaylistCol7Width;
                    return KeyDefaultValue_PlaylistCol7Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistCol7Width] = value.ToString();
            }
        }
        public int PlaylistCol8Width
        {
            get
            {
                // Return value from config                    
                int? value = ToNullableInt32(this[Key_PlaylistCol8Width]);
                if (value == null)
                {
                    // Set value to default value
                    PlaylistCol8Width = KeyDefaultValue_PlaylistCol8Width;
                    return KeyDefaultValue_PlaylistCol8Width;
                }

                // Return value
                return value.Value;
            }
            set
            {
                // Set value in config
                this[Key_PlaylistCol8Width] = value.ToString();
            }
        }    

        public void Save()
        {
            m_config.Save();
        }

        #region Keys
        
        public const string Key_FirstRun = "FirstRun";
        public const string Key_OutputDevice = "OutputDevice";
        public const string Key_Driver = "Driver"; 

        public const string Key_HideForm = "HideForm";        
        public const string Key_ShowTray = "ShowTray";
        public const string Key_HideTray = "HideTray";     
        public const string Key_EQOn = "EQOn"; 
        public const string Key_EQPreset = "EQPreset";
        public const string Key_RepeatType = "RepeatType"; 
        public const string Key_Volume = "Volume"; 
        public const string Key_TimeShifting = "TimeShifting";         

        public const string Key_FilterSoundFormat = "FilterSoundFormat"; 
        public const string Key_SongQueryArtistName = "SongQueryArtistName"; 
        public const string Key_SongQueryAlbumTitle = "SongQueryAlbumTitle";
        public const string Key_SongQueryPlaylistId = "SongQueryPlaylistId";
        public const string Key_SongQuerySongId = "SongQuerySongId";       
        public const string Key_CurrentNodeType = "CurrentNodeType"; 
       
        public const string Key_IsSongPlaying = "IsSongPlaying";
        public const string Key_SongPosition = "SongPosition";

        public const string Key_WindowX = "WindowX";
        public const string Key_WindowY = "WindowY"; 
        public const string Key_WindowWidth = "WindowWidth"; 
        public const string Key_WindowHeight = "WindowHeight"; 
        public const string Key_WindowMaximized = "WindowMaximized"; 
        public const string Key_WindowSplitterDistance = "WindowSplitterDistance"; 

        public const string Key_SongBrowserCol1Width = "SongBrowserCol1Width";
        public const string Key_SongBrowserCol2Width = "SongBrowserCol2Width";
        public const string Key_SongBrowserCol3Width = "SongBrowserCol3Width";
        public const string Key_SongBrowserCol4Width = "SongBrowserCol4Width";
        public const string Key_SongBrowserCol5Width = "SongBrowserCol5Width";
        public const string Key_SongBrowserCol6Width = "SongBrowserCol6Width";
        public const string Key_SongBrowserCol7Width = "SongBrowserCol7Width";
        public const string Key_SongBrowserCol8Width = "SongBrowserCol8Width";

        public const string Key_PlaylistX = "PlaylistX";
        public const string Key_PlaylistY = "PlaylistY";
        public const string Key_PlaylistWidth = "PlaylistWidth";
        public const string Key_PlaylistHeight = "PlaylistHeight";
        public const string Key_PlaylistMaximized = "PlaylistMaximized";
        public const string Key_PlaylistVisible = "PlaylistVisible";

        public const string Key_PlaylistCol1Width = "PlaylistCol1Width";
        public const string Key_PlaylistCol2Width = "PlaylistCol2Width";
        public const string Key_PlaylistCol3Width = "PlaylistCol3Width";
        public const string Key_PlaylistCol4Width = "PlaylistCol4Width";
        public const string Key_PlaylistCol5Width = "PlaylistCol5Width";
        public const string Key_PlaylistCol6Width = "PlaylistCol6Width";
        public const string Key_PlaylistCol7Width = "PlaylistCol7Width";
        public const string Key_PlaylistCol8Width = "PlaylistCol8Width";

        #endregion

        #region Keys (default values)
        
        public const bool KeyDefaultValue_FirstRun = true;

        public const string KeyDefaultValue_OutputDevice = "";
        public const FMOD.OUTPUTTYPE KeyDefaultValue_Driver = FMOD.OUTPUTTYPE.UNKNOWN;

        public const bool KeyDefaultValue_HideForm = false;
        public const int KeyDefaultValue_Volume = 85;
        public const bool KeyDefaultValue_ShowTray = false;
        public const bool KeyDefaultValue_HideTray = false;
        public const string KeyDefaultValue_RepeatType = "Off";
        public const int KeyDefaultValue_TimeShifting = 100;
        

        public const bool KeyDefaultValue_EQOn = false;
        public const string KeyDefaultValue_EQPreset = "";

        public const string KeyDefaultValue_FilterSoundFormat = "MP3"; 
        public const string KeyDefaultValue_SongQueryArtistName = "";
        public const string KeyDefaultValue_SongQueryAlbumTitle = "";
        public const string KeyDefaultValue_SongQueryPlaylistId = "";
        public const string KeyDefaultValue_SongQuerySongId = "";
        public const string KeyDefaultValue_CurrentNodeType = "AllSongs";

        public const bool KeyDefaultValue_IsSongPlaying = false;
        public const int KeyDefaultValue_SongPosition = 0;

        public const int KeyDefaultValue_WindowX = 10;
        public const int KeyDefaultValue_WindowY = 10;
        public const int KeyDefaultValue_WindowWidth = 1020;
        public const int KeyDefaultValue_WindowHeight = 750;
        public const bool KeyDefaultValue_WindowMaximized = false;
        public const int KeyDefaultValue_WindowSplitterDistance = 206;

        public const int KeyDefaultValue_SongBrowserCol1Width = 21;
        public const int KeyDefaultValue_SongBrowserCol2Width = 35;
        public const int KeyDefaultValue_SongBrowserCol3Width = 234;
        public const int KeyDefaultValue_SongBrowserCol4Width = 80;
        public const int KeyDefaultValue_SongBrowserCol5Width = 198;
        public const int KeyDefaultValue_SongBrowserCol6Width = 216;
        public const int KeyDefaultValue_SongBrowserCol7Width = 68;
        public const int KeyDefaultValue_SongBrowserCol8Width = 161;

        public const int KeyDefaultValue_PlaylistX = 10;
        public const int KeyDefaultValue_PlaylistY = 10;
        public const int KeyDefaultValue_PlaylistWidth = 990;
        public const int KeyDefaultValue_PlaylistHeight = 550;
        public const bool KeyDefaultValue_PlaylistMaximized = false;
        public const bool KeyDefaultValue_PlaylistVisible = false;

        public const int KeyDefaultValue_PlaylistCol1Width = 21;
        public const int KeyDefaultValue_PlaylistCol2Width = 35;
        public const int KeyDefaultValue_PlaylistCol3Width = 234;
        public const int KeyDefaultValue_PlaylistCol4Width = 80;
        public const int KeyDefaultValue_PlaylistCol5Width = 198;
        public const int KeyDefaultValue_PlaylistCol6Width = 216;
        public const int KeyDefaultValue_PlaylistCol7Width = 68;
        public const int KeyDefaultValue_PlaylistCol8Width = 161;
        #endregion
    }    
}
