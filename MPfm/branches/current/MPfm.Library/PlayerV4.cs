using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

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

        private Channel m_mainChannel = null;
        private List<PlayerV4Channel> m_subChannels = null;

        public PlayerV4()
        {
            // Initialize player using default driver (DirectSound)
            m_system = new Sound.BassNetWrapper.System(DriverType.DirectSound);
            
            // Load plugins
            m_system.LoadFlacPlugin();
            m_system.LoadFxPlugin();

            // Create lists
            m_subChannels = new List<PlayerV4Channel>();
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
            Tracing.Log("[PlayerV4.PlayFiles] Playing a list of " + filePaths.Count.ToString() + " files.");
            foreach (string filePath in filePaths)
            {
                // Check if the file exists                
                if (!File.Exists(filePath))
                {
                    // Throw exception
                    throw new Exception("The file at " + filePath + " doesn't exist!");
                }
            }

            // Reset channels (TODO: Check if channel is in use)
            m_subChannels = new List<PlayerV4Channel>();

            if (filePaths.Count == 1)
            {
                // TODO Something
            }

            // Loop through the first two file paths
            for (int a = 0; a < 2; a++)
            {
                // Create audio file and sound objects
                Tracing.Log("[PlayerV4.PlayFiles] Loading file " + (a + 1).ToString() + ": " + filePaths[a]);
                
                // Create channel for decoding
                PlayerV4Channel channel = new PlayerV4Channel();
                channel.Channel = MPfm.Sound.BassNetWrapper.Channel.CreateFileStreamForDecoding(filePaths[a]);
                channel.FileProperties = new AudioFile(filePaths[a]);

                // Add channel to list
                m_subChannels.Add(channel);
            }

            // Create the main channel
            STREAMPROC streamProc = new STREAMPROC(FileProc);
            m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStream(44100, 2, streamProc);

            // Start playback
            m_mainChannel.Play(false);
        }

        private int FileProc(int handle, IntPtr buffer, int length, IntPtr user)
        {
            // Loop through channels
            for (int a = 0; a < m_subChannels.Count; a++)
            {
                // Get active status
                BASSActive status = m_subChannels[a].Channel.IsActive();

                // Check if channel is playing
                if (status == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    // Return data
                    return m_subChannels[a].Channel.GetData(buffer, length);
                }
            }

            // Return end-of-channel
            return (int)BASSStreamProc.BASS_STREAMPROC_END;
        }
    }

    public class PlayerV4Channel
    {
        public AudioFile FileProperties { get; set; }
        public Channel Channel { get; set; }
    }
}
