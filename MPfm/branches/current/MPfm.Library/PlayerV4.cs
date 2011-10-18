using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.Library
{   
    public class PlayerV4
    {
        private MPfm.Sound.BassNetWrapper.System m_system = null;
        public MPfm.Sound.BassNetWrapper.System System
        {
            get
            {
                return m_system;
            }
        }

        public PlayerV4()
        {
            // Initialize player using default driver (DirectSound)
            m_system = new Sound.BassNetWrapper.System(DriverType.DirectSound);
            
            // Load plugins
            m_system.LoadFlacPlugin();
            m_system.LoadFxPlugin();
        }

        public void Dispose()
        {
            // Dispose plugins
            m_system.FreeFxPlugin();
            m_system.FreeFlacPlugin();

            // Dispose system
            m_system.Free();
        }

        /// <summary>
        /// Plays the list of audio files specified in the filePaths parameter.
        /// </summary>
        /// <param name="filePaths">List of audio file paths</param>
        public void PlayFiles(List<string> filePaths)
        {
            // Check for null or empty list of file paths
            if (filePaths == null || filePaths.Count == 0)
            {
                throw new Exception("There must be at least one file in the filePaths parameter.");
            }

            // Check if all file paths exist
            Tracing.Log("[PlayerV3.PlayFiles] Playing a list of " + filePaths.Count.ToString() + " files.");
            foreach (string filePath in filePaths)
            {
                // Check if the file exists                
                if (!File.Exists(filePath))
                {
                    // Throw exception
                    throw new Exception("The file at " + filePath + " doesn't exist!");
                }
            }
        }
    }
}
