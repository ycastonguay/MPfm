//
// CustomConfig.cs: Custom XML configuration framework for MPfm.
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
    //Configurator config = new Configurator(string.Empty);            
    // config.General.FirstRun
    // config.Audio.Device (Device object) -- need a ToXML() method

    public class CustomConfig
    {
        private string m_filePath = string.Empty;

        private AudioSection m_audioSection = null;
        public AudioSection Audio
        {
            get
            {
                return m_audioSection;
            }
        }

        private ControlsSection m_controlsSection = null;
        public ControlsSection Controls
        {
            get
            {
                return m_controlsSection;
            }
        }

        private XDocument m_document = null;

        public CustomConfig(string filePath)
        {
            // Set private values
            m_filePath = filePath;

            //MPfm.Core.XMLHelper.TryParse<int>("", int.TryParse);

            // Create XML document
            m_document = new XDocument();
        }

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

            // Create audio section
            m_audioSection = new AudioSection();

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

        public void Save()
        {

        }
    }

    public class AudioSection
    {
        public MPfm.Sound.BassNetWrapper.DriverType DriverType { get; set; }

        private AudioSectionDevice m_device = null;
        public AudioSectionDevice Device
        {
            get
            {
                return m_device;
            }
        }

        private AudioSectionMixer m_mixer = null;
        public AudioSectionMixer Mixer
        {
            get
            {
                return m_mixer;
            }
        }

        public AudioSection()
        {
            m_device = new AudioSectionDevice();
            m_mixer = new AudioSectionMixer();
        }
    }

    public class AudioSectionDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AudioSectionMixer
    {
        public int Frequency { get; set; }
        public int Volume { get; set; }
    }

    public class ControlsSection
    {
        private ControlsSectionSongGridView m_songGridView = null;
        public ControlsSectionSongGridView SongGridView
        {
            get
            {
                return m_songGridView;
            }
        }

        public ControlsSection()
        {
            m_songGridView = new ControlsSectionSongGridView();
        }
    }

    public class ControlsSectionSongGridView
    {
        private List<ControlsSectionSongGridViewColumn> m_columns = null;
        public List<ControlsSectionSongGridViewColumn> Columns
        {
            get
            {
                return m_columns;
            }
        }

        public ControlsSectionSongGridView()
        {
            m_columns = new List<ControlsSectionSongGridViewColumn>();
        }
    }

    public class ControlsSectionSongGridViewColumn
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public bool OrderBy { get; set; }
        public bool Visible { get; set; }
    }
}
