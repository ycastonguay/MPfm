//
// MPfmConfiguration.cs: Custom XML configuration framework for MPfm.
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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;
using MPfm.Sound;

namespace MPfm
{
    /// <summary>
    /// Custom XML configuration framework for MPfm.
    /// </summary>
    public class MPfmConfiguration
    {
        /// <summary>
        /// Configuration file path.
        /// </summary>
        private string m_filePath = string.Empty;

        /// <summary>
        /// Private value for the General property.
        /// </summary>
        private GeneralConfigurationSection m_generalSection = null;
        /// <summary>
        /// General configuration section (key/value pairs)
        /// </summary>
        public GeneralConfigurationSection General
        {
            get
            {
                return m_generalSection;
            }
        }

        /// <summary>
        /// Private value for the Audio property.
        /// </summary>
        private AudioConfigurationSection m_audioSection = null;
        /// <summary>
        /// Audio configuration section.
        /// </summary>
        public AudioConfigurationSection Audio
        {
            get
            {
                return m_audioSection;
            }
        }

        /// <summary>
        /// Private value for the Controls property.
        /// </summary>
        private ControlsConfigurationSection m_controlsSection = null;
        /// <summary>
        /// Controls configuration section.
        /// </summary>
        public ControlsConfigurationSection Controls
        {
            get
            {
                return m_controlsSection;
            }
        }

        /// <summary>
        /// XML document used for loading and saving configuration to file.
        /// </summary>
        private XDocument m_document = null;

        /// <summary>
        /// Default constructor for the MPfmConfiguration class.
        /// Requires the file path to the configuration file.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        public MPfmConfiguration(string filePath)
        {
            // Set private values
            m_filePath = filePath;

            // Set default values
            Clear();
        }

        /// <summary>
        /// Clears the configuration and sets default values.
        /// </summary>
        public void Clear()
        {
            // Create sections
            m_audioSection = new AudioConfigurationSection();
            m_controlsSection = new ControlsConfigurationSection();
            m_generalSection = new GeneralConfigurationSection();
        }

        /// <summary>
        /// Loads the MPfm configuration from an XML file.
        /// </summary>
        public void Load()
        {
            // Create sections
            Clear();

            // Load XML file
            m_document = XDocument.Load(m_filePath);

            // Read elements
            XElement elementConfig = m_document.Element("configuration");

            // Check if the configuration node was found
            if(elementConfig == null)
            {
                throw new Exception("Error reading configuration file: The configuration node was not found!");
            }

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
                    m_generalSection.KeyValues.Add(keyValue);
                }
            }            

            #region Audio
            
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
                    // Find the right type of driver
                    if (elementAudioDriver.Value.ToUpper() == "DIRECTSOUND")
                    {
                        // DirectSound
                        m_audioSection.DriverType = Sound.BassNetWrapper.DriverType.DirectSound;
                    }
                    else if (elementAudioDriver.Value.ToUpper() == "ASIO")
                    {
                        // ASIO
                        m_audioSection.DriverType = Sound.BassNetWrapper.DriverType.ASIO;
                    }
                    else if (elementAudioDriver.Value.ToUpper() == "WASAPI")
                    {
                        // WASAPI
                        m_audioSection.DriverType = Sound.BassNetWrapper.DriverType.WASAPI;
                    }
                }
                // Check if this XML element was found
                if (elementAudioDevice != null)
                {
                    // Get values
                    m_audioSection.Device.Name = XMLHelper.GetAttributeValue(elementAudioDevice, "name");
                    m_audioSection.Device.Id = XMLHelper.GetAttributeValueGeneric<int>(elementAudioDevice, "id");
                }
                // Check if this XML element was found
                if (elementAudioMixer != null)
                {
                    // Get values
                    m_audioSection.Mixer.Frequency = XMLHelper.GetAttributeValueGeneric<int>(elementAudioMixer, "frequency");
                    m_audioSection.Mixer.Volume = XMLHelper.GetAttributeValueGeneric<int>(elementAudioMixer, "volume");
                }
                // Check if this XML element was found
                if (elementAudioEQ != null)
                {
                    // Get values
                    m_audioSection.EQ.Enabled = XMLHelper.GetAttributeValueGeneric<bool>(elementAudioEQ, "enabled");
                    m_audioSection.EQ.Preset = XMLHelper.GetAttributeValue(elementAudioEQ, "preset");
                }
            }

            #endregion

        }

        /// <summary>
        /// Saves configuration to file.
        /// </summary>        
        public void Save()
        {
            // Save to default path
            Save(m_filePath);
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
            m_document.Save(filePath);            
        }

        /// <summary>
        /// Refeshes the XML document from the configuration values.
        /// </summary>
        public void RefreshXML()
        {
            // Create new document
            m_document = new XDocument();

            // Create main node
            XElement elementConfiguration = new XElement("configuration");

            #region General
            
            //<general>
            //    <key type="boolean" name="FirstRun" value="true" />
            //</general>

            // Create nodes
            XElement elementGeneral = new XElement("general");

            // Loop through key/value pairs
            foreach (GeneralConfigurationKeyValue keyValue in m_generalSection.KeyValues)
            {
                // Create key/value pair
                XElement elementKeyValue = new XElement("key");
                elementKeyValue.Add(new XAttribute("type", keyValue.ValueType.Name));
                elementKeyValue.Add(new XAttribute("name", keyValue.Name));
                elementKeyValue.Add(new XAttribute("value", keyValue.Value));                

                // Add to General node
                elementGeneral.Add(elementKeyValue);
            }

            #endregion

            #region Audio

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
            elementAudioDriver.Add(new XAttribute("type", m_audioSection.DriverType.ToString()));
            elementAudioDevice.Add(new XAttribute("name", m_audioSection.Device.Name));
            elementAudioDevice.Add(new XAttribute("driver", ""));
            elementAudioDevice.Add(new XAttribute("id", m_audioSection.Device.Id));
            elementAudioMixer.Add(new XAttribute("frequency", m_audioSection.Mixer.Frequency));
            elementAudioMixer.Add(new XAttribute("volume", m_audioSection.Mixer.Volume));
            elementAudioEQ.Add(new XAttribute("enabled", m_audioSection.EQ.Enabled));
            elementAudioEQ.Add(new XAttribute("preset", m_audioSection.EQ.Preset));

            // Add nodes           
            elementAudio.Add(elementAudioDriver);
            elementAudio.Add(elementAudioDevice);
            elementAudio.Add(elementAudioMixer);
            elementAudio.Add(elementAudioEQ);

            #endregion

            // Add nodes to main node
            elementConfiguration.Add(elementGeneral);
            elementConfiguration.Add(elementAudio);
            m_document.Add(elementConfiguration);
        }
    }


}
