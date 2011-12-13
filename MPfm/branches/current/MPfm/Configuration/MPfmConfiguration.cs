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

            // Create XML document
            m_document = new XDocument();
        }

        /// <summary>
        /// Clears the configuration and sets default values.
        /// </summary>
        public void Clear()
        {
            // Set default values            

            m_document.Add(
                new XElement("configuration",
                    new XElement("general", "")
                    )
                );
        }

        /// <summary>
        /// Loads the MPfm configuration from an XML file.
        /// </summary>
        public void Load()
        {
            // Load XML file
            m_document = XDocument.Load(m_filePath);

            // Read elements
            XElement elementConfig = m_document.Element("configuration");
            XElement elementAudio = elementConfig.Element("audio");
            XElement elementAudioDriver = elementAudio.Element("driver");
            XElement elementAudioDevice = elementAudio.Element("device");
            XElement elementAudioMixer = elementAudio.Element("mixer");            

            // Create sections
            m_audioSection = new AudioConfigurationSection();
            m_controlsSection = new ControlsConfigurationSection();

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
        }

        /// <summary>
        /// Saves configuration to file.
        /// </summary>
        public void Save()
        {

        }
    }


}
