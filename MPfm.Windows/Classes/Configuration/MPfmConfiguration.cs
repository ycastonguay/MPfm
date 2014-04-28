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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;
using MPfm.Core.Helpers;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.Windows.Classes.Forms;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Custom XML configuration framework for MPfm.
    /// </summary>
    public class MPfmConfiguration : IMPfmConfiguration
    {
        #region Properties
        
        /// <summary>
        /// Configuration file path.
        /// </summary>
        private string filePath = string.Empty;

        /// <summary>
        /// Private value for the General property.
        /// </summary>
        private GeneralConfigurationSection generalSection = null;
        /// <summary>
        /// General configuration section (key/value pairs)
        /// </summary>
        public GeneralConfigurationSection General
        {
            get
            {
                return generalSection;
            }
        }

        /// <summary>
        /// Private value for the Audio property.
        /// </summary>
        private AudioConfigurationSection audioSection = null;
        /// <summary>
        /// Audio configuration section.
        /// </summary>
        public AudioConfigurationSection Audio
        {
            get
            {
                return audioSection;
            }
        }

        /// <summary>
        /// Private value for the Controls property.
        /// </summary>
        private ControlsConfigurationSection controlsSection = null;
        /// <summary>
        /// Controls configuration section.
        /// </summary>
        public ControlsConfigurationSection Controls
        {
            get
            {
                return controlsSection;
            }
        }

        /// <summary>
        /// Private value for the Windows property.
        /// </summary>
        private WindowsConfigurationSection windowsSection = null;
        /// <summary>
        /// Windows configuration section.
        /// </summary>
        public WindowsConfigurationSection Windows
        {
            get
            {
                return windowsSection;
            }
        }

        /// <summary>
        /// XML document used for loading and saving configuration to file.
        /// </summary>
        private XDocument document = null;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Default constructor for the MPfmConfiguration class.
        /// Requires the file path to the configuration file.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        public MPfmConfiguration(string filePath)
        {
            // Set private values
            this.filePath = filePath;

            // Set default values
            Clear();
        }

        #endregion

        #region Load/Save methods
        
        /// <summary>
        /// Loads the MPfm configuration from an XML file.
        /// </summary>
        public void Load()
        {
            // Create sections
            Clear();

            // Load XML file
            document = XDocument.Load(filePath);

            // Read elements
            XElement elementConfig = document.Element("configuration");

            // Check if the configuration node was found
            if(elementConfig == null)
            {
                throw new Exception("Error reading configuration file: The configuration node was not found!");
            }

            // *************************************************************************
            // GENERAL SECTION

            // Read General node
            XElement elementGeneral = elementConfig.Element("general");

            // Check if node was found
            if (elementGeneral != null)
            {
                // Get key/value pairs
                List<XElement> elementsKeyValue = elementGeneral.Elements("key").ToList();

                // Loop through key/value pairs
                foreach(XElement elementKeyValue in elementsKeyValue)
                {
                    // Add key/value
                    GeneralConfigurationKeyValue keyValue = new GeneralConfigurationKeyValue();                    
                    keyValue.ValueType = Type.GetType(XMLHelper.GetAttributeValue(elementKeyValue, "type"));
                    keyValue.Name = XMLHelper.GetAttributeValue(elementKeyValue, "name");
                    keyValue.Value = XMLHelper.GetAttributeValue(elementKeyValue, "value");
                    generalSection.KeyValues.Add(keyValue);
                }
            }

            // *************************************************************************
            // AUDIO SECTION
            
            // Read Audio node
            XElement elementAudio = elementConfig.Element("audio");

            // Check if node was found
            if (elementAudio != null)
            {
                // Read subnodes
                XElement elementAudioDriver = elementAudio.Element("driver");
                XElement elementAudioDevice = elementAudio.Element("device");
                XElement elementAudioMixer = elementAudio.Element("mixer");
                XElement elementAudioEQ = elementAudio.Element("eq");

                // Check if this XML element was found
                if (elementAudioDriver != null)
                {
                    DriverType driverType;
                    Enum.TryParse<DriverType>(XMLHelper.GetAttributeValue(elementAudioDriver, "type"), out driverType);
                    audioSection.DriverType = driverType;
                }
                // Check if this XML element was found
                if (elementAudioDevice != null)
                {
                    // Get values
                    audioSection.Device.Name = XMLHelper.GetAttributeValue(elementAudioDevice, "name");
                    audioSection.Device.Id = XMLHelper.GetAttributeValueGeneric<int>(elementAudioDevice, "id");
                }
                // Check if this XML element was found
                if (elementAudioMixer != null)
                {
                    // Get values
                    audioSection.Mixer.Frequency = XMLHelper.GetAttributeValueGeneric<int>(elementAudioMixer, "frequency");
                    audioSection.Mixer.Volume = XMLHelper.GetAttributeValueGeneric<int>(elementAudioMixer, "volume");                    
                    audioSection.Mixer.BufferSize = XMLHelper.GetAttributeValueGeneric<int>(elementAudioMixer, "bufferSize");
                    audioSection.Mixer.UpdatePeriod = XMLHelper.GetAttributeValueGeneric<int>(elementAudioMixer, "updatePeriod");
                }
                // Check if this XML element was found
                if (elementAudioEQ != null)
                {
                    // Get values
                    audioSection.EQ.Enabled = XMLHelper.GetAttributeValueGeneric<bool>(elementAudioEQ, "enabled");
                    audioSection.EQ.Preset = XMLHelper.GetAttributeValue(elementAudioEQ, "preset");
                }
            }

            // *************************************************************************
            // CONTROLS SECTION

            // Read Controls node
            XElement elementControls = elementConfig.Element("controls");

            // Check if node was found
            if (elementControls != null)
            {
                // Read subnodes
                XElement elementSongGridView = elementControls.Element("songGridView");
                XElement elementPlaylistGridView = elementControls.Element("playlistGridView");

                // Check if this XML element was found
                if (elementSongGridView != null)
                {
                    // Read subnodes
                    XElement elementSongGridViewQuery = elementSongGridView.Element("query");
                    XElement elementSongGridViewColumns = elementSongGridView.Element("columns");

                    // Check if this XML element was found
                    if (elementSongGridViewQuery != null)
                    {
                        //// Get node type
                        //TreeLibraryNodeType nodeType;
                        //Enum.TryParse<TreeLibraryNodeType>(XMLHelper.GetAttributeValue(elementSongGridViewQuery, "type"), out nodeType);
                        //controlsSection.SongGridView.Query.NodeType = nodeType;

                        // Set other query properties
                        controlsSection.SongGridView.Query.AudioFileId = XMLHelper.GetAttributeValueGeneric<Guid>(elementSongGridViewQuery, "audioFileId");
                        controlsSection.SongGridView.Query.PlaylistId = XMLHelper.GetAttributeValueGeneric<Guid>(elementSongGridViewQuery, "playlistId");
                        controlsSection.SongGridView.Query.ArtistName = XMLHelper.GetAttributeValue(elementSongGridViewQuery, "artistName");
                        controlsSection.SongGridView.Query.AlbumTitle = XMLHelper.GetAttributeValue(elementSongGridViewQuery, "albumTitle");                        
                    }
                    // Check if this XML element was found
                    if (elementSongGridViewColumns != null)
                    {
                        // Get column list                                
                        List<XElement> elementsColumns = elementSongGridViewColumns.Elements("column").ToList();

                        // Loop through columns
                        foreach (XElement elementColumn in elementsColumns)
                        {
                            // Create column and add to list
                            SongGridViewColumn column = new SongGridViewColumn(string.Empty, XMLHelper.GetAttributeValue(elementColumn, "fieldName"), true, 0);
                            column.Title = XMLHelper.GetAttributeValue(elementColumn, "title");
                            column.Order = XMLHelper.GetAttributeValueGeneric<int>(elementColumn, "order");
                            column.Width = XMLHelper.GetAttributeValueGeneric<int>(elementColumn, "width");
                            column.Visible = XMLHelper.GetAttributeValueGeneric<bool>(elementColumn, "visible");
                            controlsSection.SongGridView.Columns.Add(column);
                        }
                    }
                }
                // Check if this XML element was found
                if (elementPlaylistGridView != null)
                {
                    // Read subnodes                    
                    XElement elementPlaylistGridViewColumns = elementPlaylistGridView.Element("columns");

                    // Check if this XML element was found
                    if (elementPlaylistGridViewColumns != null)
                    {
                        // Clear existing columns
                        controlsSection.PlaylistGridView.Columns.Clear();

                        // Get column list                                
                        List<XElement> elementsColumns = elementPlaylistGridViewColumns.Elements("column").ToList();

                        // Loop through columns
                        foreach (XElement elementColumn in elementsColumns)
                        {
                            // Create column and add to list                                                        
                            SongGridViewColumn column = new SongGridViewColumn(string.Empty, XMLHelper.GetAttributeValue(elementColumn, "fieldName"), true, 0);
                            column.Title = XMLHelper.GetAttributeValue(elementColumn, "title");
                            column.Order = XMLHelper.GetAttributeValueGeneric<int>(elementColumn, "order");
                            column.Width = XMLHelper.GetAttributeValueGeneric<int>(elementColumn, "width");
                            column.Visible = XMLHelper.GetAttributeValueGeneric<bool>(elementColumn, "visible");
                            controlsSection.PlaylistGridView.Columns.Add(column);
                        }
                    }
                }
            }

            // *************************************************************************
            // WINDOWS SECTION

            // Read Windows node
            XElement elementWindows = elementConfig.Element("windows");

            // Check if node was found
            if (elementWindows != null)
            {
                // Reset window list
                windowsSection.Windows.Clear();

                // Get window list                                
                List<XElement> elementsWindows = elementWindows.Elements("window").ToList();

                // Loop through key/value pairs
                foreach (XElement elementWindow in elementsWindows)
                {
                    // Create new window configuration
                    WindowConfiguration window = new WindowConfiguration();
                    window.Name = XMLHelper.GetAttributeValue(elementWindow, "name");
                    window.UseDefaultPosition = XMLHelper.GetAttributeValueGeneric<bool>(elementWindow, "useDefaultPosition");
                    window.X = XMLHelper.GetAttributeValueGeneric<int>(elementWindow, "x");
                    window.Y = XMLHelper.GetAttributeValueGeneric<int>(elementWindow, "y");
                    window.Width = XMLHelper.GetAttributeValueGeneric<int>(elementWindow, "width");
                    window.Height = XMLHelper.GetAttributeValueGeneric<int>(elementWindow, "height");
                    window.Maximized = XMLHelper.GetAttributeValueGeneric<bool>(elementWindow, "maximized");
                    window.Visible = XMLHelper.GetAttributeValueGeneric<bool>(elementWindow, "visible");
                    windowsSection.Windows.Add(window);
                }
            }
        }

        /// <summary>
        /// Saves configuration to file.
        /// </summary>        
        public void Save()
        {
            // Save to default path
            Save(filePath);
        }

        /// <summary>
        /// Saves configuration to file to a specific location.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        public void Save(string filePath)
        {
            // Refresh XML document from configuration values
            RefreshXML();            

            // Save configuration to file
            document.Save(filePath);            
        }

        #endregion

        #region Other methods
        
        /// <summary>
        /// Clears the configuration and sets default values.
        /// </summary>
        public void Clear()
        {
            // Create sections
            generalSection = new GeneralConfigurationSection();
            audioSection = new AudioConfigurationSection();
            controlsSection = new ControlsConfigurationSection();
            windowsSection = new WindowsConfigurationSection();
        }

        /// <summary>
        /// Refeshes the XML document from the configuration values.
        /// </summary>
        public void RefreshXML()
        {
            // Create new document
            document = new XDocument();

            // Create main node
            XElement elementConfiguration = new XElement("configuration");

            // *************************************************************************
            // GENERAL SECTION

            //<general>
            //    <key type="boolean" name="FirstRun" value="true" />
            //</general>

            // Create nodes
            XElement elementGeneral = new XElement("general");

            // Loop through key/value pairs
            foreach (GeneralConfigurationKeyValue keyValue in generalSection.KeyValues)
            {
                // Create key/value pair
                XElement elementKeyValue = new XElement("key");
                elementKeyValue.Add(XMLHelper.NewAttribute("type", keyValue.ValueType.FullName));
                elementKeyValue.Add(XMLHelper.NewAttribute("name", keyValue.Name));
                elementKeyValue.Add(XMLHelper.NewAttribute("value", keyValue.Value));                

                // Add to General node
                elementGeneral.Add(elementKeyValue);
            }

            // *************************************************************************
            // AUDIO SECTION

            //<audio>
            //    <driver type="DirectSound" />
            //    <device name="M-Audio" driver="DriverInfo" id="1" />
            //    <mixer frequency="96000" volume="100" />
            //</audio>

            // Create nodes
            XElement elementAudio = new XElement("audio");
            XElement elementAudioDriver = new XElement("driver");
            XElement elementAudioDevice = new XElement("device");
            XElement elementAudioMixer = new XElement("mixer");
            XElement elementAudioEQ = new XElement("eq");

            // Set node values and attributes
            elementAudioDriver.Add(XMLHelper.NewAttribute("type", audioSection.DriverType.ToString()));
            elementAudioDevice.Add(XMLHelper.NewAttribute("name", audioSection.Device.Name));
            elementAudioDevice.Add(XMLHelper.NewAttribute("driver", ""));
            elementAudioDevice.Add(XMLHelper.NewAttribute("id", audioSection.Device.Id));
            elementAudioMixer.Add(XMLHelper.NewAttribute("frequency", audioSection.Mixer.Frequency));
            elementAudioMixer.Add(XMLHelper.NewAttribute("volume", audioSection.Mixer.Volume));
            elementAudioMixer.Add(XMLHelper.NewAttribute("bufferSize", audioSection.Mixer.BufferSize));
            elementAudioMixer.Add(XMLHelper.NewAttribute("updatePeriod", audioSection.Mixer.UpdatePeriod));
            elementAudioEQ.Add(XMLHelper.NewAttribute("enabled", audioSection.EQ.Enabled));
            elementAudioEQ.Add(XMLHelper.NewAttribute("preset", audioSection.EQ.Preset));

            // Add nodes           
            elementAudio.Add(elementAudioDriver);
            elementAudio.Add(elementAudioDevice);
            elementAudio.Add(elementAudioMixer);
            elementAudio.Add(elementAudioEQ);

            // *************************************************************************
            // CONTROLS SECTION

            //<controls>
            //  <songGridView>
            //    <columns>
            //      <column fieldName="Test" order="0" width="0" visible="true" />
            //    </columns>
            //    <query nodeType="all" audioFileId="" playlistId="" artistName="" albumTitle="" />      
            //  </songGridView>
            //  <playlistGridView>
            //    <columns>
            //      <column fieldName="Test" order="0" width="0" visible="true" />
            //    </columns>      
            //  </playlistGridView>
            //</controls>

            // Create nodes
            XElement elementControls = new XElement("controls");            
            XElement elementControlsSongGridView = new XElement("songGridView");
            XElement elementControlsSongGridViewColumns = new XElement("columns");
            XElement elementControlsSongGridViewQuery = new XElement("query");
            XElement elementControlsPlaylistGridView = new XElement("playlistGridView");
            XElement elementControlsPlaylistGridViewColumns = new XElement("columns");

            // Song browser query
            //elementControlsSongGridViewQuery.Add(XMLHelper.NewAttribute("type", controlsSection.SongGridView.Query..ToString()));
            elementControlsSongGridViewQuery.Add(XMLHelper.NewAttribute("audioFileId", controlsSection.SongGridView.Query.AudioFileId.ToString()));
            elementControlsSongGridViewQuery.Add(XMLHelper.NewAttribute("playlistId", controlsSection.SongGridView.Query.PlaylistId.ToString()));
            elementControlsSongGridViewQuery.Add(XMLHelper.NewAttribute("artistName", controlsSection.SongGridView.Query.ArtistName));
            elementControlsSongGridViewQuery.Add(XMLHelper.NewAttribute("albumTitle", controlsSection.SongGridView.Query.AlbumTitle));

            // Loop through song browser columns            
            foreach (SongGridViewColumn column in controlsSection.SongGridView.Columns.OrderBy(x => x.Order))
            {
                // Create node
                XElement elementColumn = new XElement("column");                
                elementColumn.Add(XMLHelper.NewAttribute("fieldName", column.FieldName));
                elementColumn.Add(XMLHelper.NewAttribute("title", column.Title));
                elementColumn.Add(XMLHelper.NewAttribute("order", column.Order));
                elementColumn.Add(XMLHelper.NewAttribute("width", column.Width));
                elementColumn.Add(XMLHelper.NewAttribute("visible", column.Visible));
                elementControlsSongGridViewColumns.Add(elementColumn);
            }

            // Loop through playlist browser columns            
            foreach (SongGridViewColumn column in controlsSection.PlaylistGridView.Columns.OrderBy(x => x.Order))
            {
                // Create node
                XElement elementColumn = new XElement("column");                
                elementColumn.Add(XMLHelper.NewAttribute("fieldName", column.FieldName));
                elementColumn.Add(XMLHelper.NewAttribute("title", column.Title));
                elementColumn.Add(XMLHelper.NewAttribute("order", column.Order));
                elementColumn.Add(XMLHelper.NewAttribute("width", column.Width));
                elementColumn.Add(XMLHelper.NewAttribute("visible", column.Visible));
                elementControlsPlaylistGridViewColumns.Add(elementColumn);
            }
            
            // Add nodes            
            elementControlsSongGridView.Add(elementControlsSongGridViewQuery);
            elementControlsSongGridView.Add(elementControlsSongGridViewColumns);
            elementControlsPlaylistGridView.Add(elementControlsPlaylistGridViewColumns);
            elementControls.Add(elementControlsSongGridView);
            elementControls.Add(elementControlsPlaylistGridView);

            // *************************************************************************
            // WINDOWS SECTION

            //<windows>            
            //    <window name="Playlist" x="0" y="0" width="0" height="0" maximized="true" visible="true" />
            //</windows>

            // Create nodes
            XElement elementWindows = new XElement("windows");

            // Loop through windows
            foreach (WindowConfiguration window in windowsSection.Windows)
            {
                // Add node
                XElement elementWindow = new XElement("window");
                elementWindow.Add(XMLHelper.NewAttribute("name", window.Name));
                elementWindow.Add(XMLHelper.NewAttribute("useDefaultPosition", window.UseDefaultPosition));
                elementWindow.Add(XMLHelper.NewAttribute("x", window.X));
                elementWindow.Add(XMLHelper.NewAttribute("y", window.Y));
                elementWindow.Add(XMLHelper.NewAttribute("width", window.Width));
                elementWindow.Add(XMLHelper.NewAttribute("height", window.Height));
                elementWindow.Add(XMLHelper.NewAttribute("maximized", window.Maximized));
                elementWindow.Add(XMLHelper.NewAttribute("visible", window.Visible));
                elementWindows.Add(elementWindow);
            }

            // Add nodes to main node
            elementConfiguration.Add(elementGeneral);
            elementConfiguration.Add(elementAudio);
            elementConfiguration.Add(elementControls);
            elementConfiguration.Add(elementWindows);
            document.Add(elementConfiguration);
        }

        /// <summary>
        /// Returns the value of a key in the General configuration section.
        /// </summary>        
        /// <param name="name">Key name</param>
        /// <returns>Key value</returns>
        public string GetKeyValue(string name)
        {
            // Try to get the key/value pair
            GeneralConfigurationKeyValue keyValue = generalSection.KeyValues.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());

            // Check if the key/value pair is valid
            if (keyValue != null)
            {
                // Get value
                return keyValue.Value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Returns the value of a key in the General configuration section.
        /// </summary>
        /// <typeparam name="T">Generic type to convert the value to</typeparam>
        /// <param name="name">Key name</param>
        /// <returns>Key value</returns>
        public T? GetKeyValueGeneric<T>(string name) where T : struct
        {
            // Try to get the key/value pair
            GeneralConfigurationKeyValue keyValue = generalSection.KeyValues.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());

            // Check if the key/value pair is valid
            if (keyValue != null)
            {
                // Get value
                return keyValue.GetValue<T>();
            }

            return null;           
        }

        /// <summary>
        /// Sets the value of a key in the General configuration section.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="value">Key value</param>
        public void SetKeyValue<T>(string name, T value)
        {
            // Try to get the key/value pair
            GeneralConfigurationKeyValue keyValue = generalSection.KeyValues.FirstOrDefault(x => x.Name.ToUpper() == name.ToUpper());

            // Check if the key/value pair is valid
            if (keyValue != null)
            {
                // Set value
                keyValue.Value = value;
            }
            else
            {
                // Create key
                keyValue = new GeneralConfigurationKeyValue();
                keyValue.Name = name;
                keyValue.ValueType = typeof(T);
                keyValue.Value = value;
                generalSection.KeyValues.Add(keyValue);
            }
        }

        #endregion
    }
}
