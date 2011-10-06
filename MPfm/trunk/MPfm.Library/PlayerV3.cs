//
// PlayerV3.cs: This is the main Player class for MPfm (Playback Engine V3).
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
using System.ComponentModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MPfm.Core;
using MPfm.Sound;

namespace MPfm.Library
{
    /// <summary>
    /// MPfm Player (Playback Engine V3). Manages audio playback in multiple channels.     
    /// Supports gapless playback and can be re-used in other software.
    /// 
    /// This is the opposite of PEV2 (Playback Engine V2); the Library is decoupled from this class.
    /// </summary>
    public class PlayerV3
    {
        #region Properties

        #region Playback Properties

        private RepeatType m_repeatType = RepeatType.Off;
        /// <summary>
        /// Repeat type (Off, Playlist, Song)
        /// </summary>
        public RepeatType RepeatType
        {
            get
            {
                return m_repeatType;
            }
            set
            {
                m_repeatType = value;
            }
        }

        /// <summary>
        /// Number of channels currently playing (read-only).
        /// </summary>
        public int NumberOfChannelsPlaying
        {
            get
            {
                // Get info from sound system
                return m_soundSystem.NumberOfChannelsPlaying;
            }
        }

        /// <summary>
        /// Private value for the IsPaused property.
        /// </summary>
        private bool m_isPaused = false;
        /// <summary>
        /// Indicates if the playback is paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return m_isPaused;
            }
        }

        /// <summary>
        /// Private value for the IsPlaying property.
        /// </summary>
        private bool m_isPlaying = false;
        /// <summary>
        /// Indicates if at least one channel is playing.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return m_isPlaying;
            }
        }

        /// <summary>
        /// Private value for the CurrentPausePosition property.
        /// </summary>
        private uint m_currentPausePosition = 0;
        /// <summary>
        /// Defines the current channel position when the player was paused (in PCM samples).
        /// Only filled when the playback is paused.
        /// </summary>
        public uint CurrentPausePosition
        {
            get
            {
                return m_currentPausePosition;
            }
        }

        #endregion

        #region Sound System Properties
        
        /// <summary>
        /// Timer for the player.
        /// </summary>
        private System.Timers.Timer m_timerPlayer = null;
        //private bool m_isTimeToLoadNewSong = false;

        /// <summary>
        /// Private value for the SoundSystem property.
        /// </summary>
        private MPfm.Sound.System m_soundSystem;
        /// <summary>
        /// MPfm Sound System used for playback (property).
        /// </summary>
        public MPfm.Sound.System SoundSystem
        {
            get
            {
                return m_soundSystem;
            }
        }

        /// <summary>
        /// Private value for the AudioFiles property.
        /// </summary>
        private MPfm.Sound.AudioFile[] m_audioFiles = null;
        /// <summary>
        /// List of audio files to play. The AudioFile object exposes all the values
        /// coming from the tags and audio file properties and also contains the 
        /// Sound and Channel objects for audio playback.
        /// </summary>
        public MPfm.Sound.AudioFile[] AudioFiles
        {
            get
            {
                return m_audioFiles;
            }
        }

        /// <summary>
        /// Private value for the CurrentAudioFileIndex property.
        /// </summary>
        private int m_currentAudioFileIndex = 0;
        /// <summary>
        /// Returns the index of the currently playing audio file (channel).
        /// </summary>
        public int CurrentAudioFileIndex
        {
            get
            {
                return m_currentAudioFileIndex;
            }
        }

        /// <summary>
        /// Returns the currently playing AudioFile object from the AudioFiles array.
        /// </summary>
        public MPfm.Sound.AudioFile CurrentAudioFile
        {
            get
            {
                // Make sure we can get the current sound playing (check for nulls)
                if (m_audioFiles == null || m_audioFiles.Length == 0 || m_currentAudioFileIndex < 0)
                {
                    return null;
                }

                // Return the current sound
                return m_audioFiles[m_currentAudioFileIndex];
            }
        }

        /// <summary>
        /// Returns the currently playing Sound object from the AudioFiles array.
        /// </summary>
        public MPfm.Sound.Sound CurrentSound
        {
            get
            {
                // Make sure we can get the current sound playing (check for nulls)
                if (m_audioFiles == null || m_audioFiles.Length == 0 || m_currentAudioFileIndex < 0)
                {
                    return null;
                }

                // Return the current sound
                return m_audioFiles[m_currentAudioFileIndex].Sound;               
            }
        }

        /// <summary>
        /// Returns the currently playing Channel object from the AudioFiles array.
        /// </summary>
        public MPfm.Sound.Channel CurrentChannel
        {
            get
            {
                // Make sure we can get the current sound playing (check for nulls)
                if (m_audioFiles == null || m_audioFiles.Length == 0 || m_currentAudioFileIndex < 0)
                {
                    return null;
                }

                // Return the current sound
                return m_audioFiles[m_currentAudioFileIndex].Channel;
            }
        }

        /// <summary>
        /// Private value for the Volume property.
        /// </summary>
        private int m_volume = 90;
        /// <summary>
        /// Volume of the currently playing channel.
        /// </summary>
        public int Volume
        {
            get
            {
                return m_volume;
            }
            set
            {
                m_volume = value;

                // Check if the current channel is initialized and is playing
                if (CurrentChannel != null && CurrentChannel.IsInitialized && CurrentChannel.IsPlaying)
                {
                    // Set current channel
                    CurrentChannel.Volume = (float)Volume / 100;
                }
            }
        }

        /// <summary>
        /// Private value for the MinimumDelay property.
        /// </summary>
        private uint m_minimumDelay = 0;
        /// <summary>
        /// Minimum delay for starting the playback of an audio file
        /// (necessary for gapless playback).
        /// </summary>
        public uint MinimumDelay
        {
            get
            {
                return m_minimumDelay;
            }
        }
           
        /// <summary>
        /// Private value for the OutputFormatMixer property.
        /// </summary>
        private OutputFormatMixer m_outputFormatMixer = null;
        /// <summary>
        /// Data structure indicating what output format is used for in the FMOD mixer.
        /// </summary>
        public OutputFormatMixer OutputFormatMixer
        {
            get
            {
                return m_outputFormatMixer;
            }
        }

        /// <summary>
        /// Private value for the WavWriterFilePath property.
        /// </summary>
        private string m_wavWriterFilePath = string.Empty;
        /// <summary>
        /// Defines the file path where to write the Wave output
        /// (used with V3DriverType.WavWriter)
        /// </summary>
        public string WavWriterFilePath
        {
            get
            {
                return m_wavWriterFilePath;
            }
            set
            {
                m_wavWriterFilePath = value;
            }
        }

        #endregion

        #endregion

        #region Constructors / Initialize and Close Methods

        /// <summary>
        /// Default constructor for PlayerV3. The player will use DirectSound as default driver type
        /// and will use the default output device configured in Windows.
        /// </summary>
        public PlayerV3()
        {
            // Use initialize method with default values
            Initialize(V3DriverType.DirectSound, string.Empty);
        }

        /// <summary>
        /// Constructor for PlayerV3. The player will use the driver type specified in the parameter. It
        /// will raise an exception if the driver is not supported.
        /// </summary>
        /// <param name="driverType">Driver type</param>
        public PlayerV3(V3DriverType driverType)
        {
            // Use initialize method with specified driver but default output device
            Initialize(driverType, string.Empty);
        }

        /// <summary>
        /// Constructor for PlayerV3. The player will use the driver type and output device specified in the parameter. It
        /// will raise an exception if the driver is not supported.
        /// </summary>
        /// <param name="driverType">Driver type</param>
        /// <param name="outputDeviceName">Output device name</param>
        public PlayerV3(V3DriverType driverType, string outputDeviceName)
        {
            // Use initialize method with specified driver and specified output device
            Initialize(driverType, outputDeviceName);
        }

        /// <summary>
        /// Initialization method used in constructors.
        /// </summary>
        /// <param name="driverType">Driver type</param>
        /// <param name="outputDeviceName">Output device name</param>
        private void Initialize(V3DriverType driverType, string outputDeviceName)
        {
            // Initialize sound system            
            Tracing.Log("[PlayerV3.Initialize] Initializing sound system (Driver type: " + driverType.ToString() + " / Output device (blank is default): " + outputDeviceName + ")");

            // Initialize flags
            m_isPlaying = false;

            // Define the FMOD output type (what we call the "driver type")
            FMOD.OUTPUTTYPE fmodOutputType = ConvertDriverTypeToFMODOutputType(driverType);

            // Create sound system
            m_soundSystem = new MPfm.Sound.System(fmodOutputType, outputDeviceName);

            // Get DSP buffer size
            DSPBufferSize dspBufferSize = m_soundSystem.GetDSPBufferSize();

            uint buff = 0;
            FMOD.TIMEUNIT tu = FMOD.TIMEUNIT.MS;
            m_soundSystem.GetStreamBufferSize(ref buff, ref tu);
            m_soundSystem.SetStreamBufferSize(buff * 4, tu);

            // Set minimum delay
            m_minimumDelay = dspBufferSize.bufferLength * 2;
            Tracing.Log("[PlayerV3.Initialize] Minimum delay (DSP buffer size * 2): " + m_minimumDelay.ToString());
         
            // Get output mixer format
            m_outputFormatMixer = m_soundSystem.GetSoftwareFormat();
            Tracing.Log("[PlayerV3.Initialize] Output mixer frequency: " + m_outputFormatMixer.sampleRate.ToString() + " Hz");

            // Create timer
            Tracing.Log("[PlayerV3.Initialize] Creating timer...");
            m_timerPlayer = new System.Timers.Timer();
            m_timerPlayer.Elapsed += new System.Timers.ElapsedEventHandler(m_timerPlayer_Elapsed);
            m_timerPlayer.Interval = 1000;            
            m_timerPlayer.Enabled = false;
        }       

        /// <summary>
        /// Converts a PlayerV3 driver type to a FMOD output type (used for MPfm.Sound).
        /// </summary>
        /// <param name="driverType">PlayerV3 driver type</param>
        /// <returns>FMOD output type</returns>
        private FMOD.OUTPUTTYPE ConvertDriverTypeToFMODOutputType(V3DriverType driverType)
        {
            // Convert to FMOD output type            
            if (driverType == V3DriverType.ASIO)
            {
                return FMOD.OUTPUTTYPE.ASIO;
            }
            else if (driverType == V3DriverType.DirectSound)
            {
                return FMOD.OUTPUTTYPE.DSOUND;
            }
            else if (driverType == V3DriverType.WindowsMultimedia)
            {
                return FMOD.OUTPUTTYPE.DSOUND;
            }
            else if (driverType == V3DriverType.WASAPI)
            {
                return FMOD.OUTPUTTYPE.WASAPI;
            }
            else if (driverType == V3DriverType.WavWriter)
            {
                return FMOD.OUTPUTTYPE.WAVWRITER;
            }

            return FMOD.OUTPUTTYPE.NOSOUND;
        }

        /// <summary>
        /// Closes the player and releases the sound system from the memory.
        /// </summary>
        public void Close()
        {
            // Check if the sound system is cleared
            if (m_soundSystem != null)
            {
                // Close system
                m_soundSystem.Close();

                // Release system from memory
                m_soundSystem.Release();
            }
        }

        #endregion

        #region Callback / Timer Methods

        /// <summary>
        /// Occurs when a sound in a channel has finished playing.
        /// Increments the audio file index counter.
        /// </summary>
        /// <param name="args">Event arguments</param>
        protected void Channel_SoundEnd(EventArgs args)
        {
            // If the playback has stopped or the playback is paused, exit
            if (!m_isPlaying || m_isPaused)
            {
                return;
            }

            Tracing.Log("[PlayerV3.Channel_SoundEnd] Channel has stopped playing");

            // Make sure this is not the last audio file to play
            if (m_currentAudioFileIndex >= m_audioFiles.Length - 1)
            {
                Tracing.Log("[PlayerV3.Channel_SoundEnd] No need to increment m_currentAudioFileIndex since this is the last song (" + (m_currentAudioFileIndex + 1).ToString() + " / " + m_audioFiles.Length.ToString() + ")");
                return;
            }

            // Increment the audio file counter
            m_currentAudioFileIndex++;

            Tracing.Log("[PlayerV3.Channel_SoundEnd] Incremented the current channel to " + (m_currentAudioFileIndex + 1).ToString() + " / " + m_audioFiles.Length.ToString());

            // Make sure this is not the last audio file to play
            if (m_currentAudioFileIndex < m_audioFiles.Length - 1)
            {
                // Flag the timer it's time to load the next song                                
                Tracing.Log("[PlayerV3.Channel_SoundEnd] Load the next song in advance (start timer).");
                m_timerPlayer.Enabled = true;
            }
            else
            {
                Tracing.Log("[PlayerV3.Channel_SoundEnd] This is the last song, no more songs to load in advance.");
            }
        }

        /// <summary>
        /// Occurs when the timer for loading the next song in advance expires.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void m_timerPlayer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Reset timer
            m_timerPlayer.Enabled = false;

            // Load Sound object
            Tracing.Log("[PlayerV3.timerPlayer] Time to load a new song!");
            Tracing.Log("[PlayerV3.timerPlayer] (" + (m_currentAudioFileIndex + 1).ToString() + "/" + m_audioFiles.Length.ToString() + ") " + m_audioFiles[m_currentAudioFileIndex + 1].FilePath);
            //m_audioFiles[m_currentAudioFileIndex + 1].Sound = m_soundSystem.CreateSound(m_audioFiles[m_currentAudioFileIndex + 1].FilePath, true);

            // Create channel                
            //m_audioFiles[m_currentAudioFileIndex + 1].Channel = new Channel(m_soundSystem);
            //m_audioFiles[m_currentAudioFileIndex + 1].Channel.SoundEnd += new Channel.SoundEndHandler(Channel_SoundEnd);
            //m_audioFiles[m_currentAudioFileIndex+1].Channel.Volume = m_volume;

            uint length_pcm = (uint)((m_audioFiles[m_currentAudioFileIndex].Sound.LengthPCM * m_outputFormatMixer.sampleRate / m_audioFiles[m_currentAudioFileIndex].Channel.Frequency) + 0.5f);
            float frequency = m_audioFiles[m_currentAudioFileIndex].Channel.Frequency;
            Tracing.Log("[PlayerV3.timerPlayer] LengthPCM: " + length_pcm.ToString());

            // Lock the DSP mixer (make sure the delay values stay in sync)
            Tracing.Log("[PlayerV3.timerPlayer] Locking DSP engine...");
            m_soundSystem.LockDSP();

            // Start playback (in paused mode) 
            m_soundSystem.PlaySound(m_audioFiles[m_currentAudioFileIndex + 1].Sound, true, m_audioFiles[m_currentAudioFileIndex + 1].Channel);

            // Get existing delay
            Fmod64BitWord wordDelay = m_audioFiles[m_currentAudioFileIndex + 1].Channel.GetDelay(FMOD.DELAYTYPE.DSPCLOCK_START);
            
            // Calculate position (with frequency conversion)
            uint position = (uint)((m_audioFiles[m_currentAudioFileIndex].Channel.PositionPCM * m_outputFormatMixer.sampleRate / frequency) + 0.5f);

            // Substact the current position from the total length
            AudioTools.FMOD_64BIT_ADD(ref wordDelay.hi, ref wordDelay.lo, 0, length_pcm - position);

            // Set the new DSP clock start delay value            
            m_audioFiles[m_currentAudioFileIndex + 1].Channel.SetDelay(FMOD.DELAYTYPE.DSPCLOCK_START, wordDelay.hi, wordDelay.lo);                                                         

            // Start playback
            m_audioFiles[m_currentAudioFileIndex + 1].Channel.Pause(false);

            // Unlock the DSP mixer (the delays have been set)
            m_soundSystem.UnlockDSP();
            Tracing.Log("[PlayerV3.timerPlayer] Unlocked DSP engine.");
            Tracing.Log("[PlayerV3.timerPlayer] Set delay: " + wordDelay.lo.ToString());
        }

        #endregion

        #region Playback Methods

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

            // Reset index and audio file array
            m_currentAudioFileIndex = 0;
            m_isPlaying = false;
            m_isPaused = false;
            m_audioFiles = new AudioFile[filePaths.Count];

            // Loop through file paths
            for (int a = 0; a < filePaths.Count; a++)
            {
                // Create audio file and sound objects
                Tracing.Log("[PlayerV3.PlayFiles] Loading file " + (a + 1).ToString() + ": " + filePaths[a]);
                m_audioFiles[a] = new AudioFile(filePaths[a]);
                m_audioFiles[a].Sound = m_soundSystem.CreateSound(filePaths[a], true);
            }

            // Loop through file paths
            for (int a = 0; a < m_audioFiles.Length; a++)            
            {
                // Create channel                
                Tracing.Log("[PlayerV3.PlayFiles] Creating channel " + (a + 1).ToString() + ": " + filePaths[a]);
                m_audioFiles[a].Channel = new Channel(m_soundSystem);                
                m_audioFiles[a].Channel.SoundEnd += new Channel.SoundEndHandler(Channel_SoundEnd);
            }

            // Check if there are more than one songs
            if (m_audioFiles.Length > 1)
            {
                // Schedule the first two channels from start
                ScheduleChannel(0, 1500000);
            }
            else if (m_audioFiles.Length == 1)
            {
                // Start playback of the only channel
                m_audioFiles[0].Channel.Pause(false);
            }

            // Set playing flag
            m_isPlaying = true;
            m_isPaused = false;
        }

        /// <summary>
        /// Stops the playback of any channel currently playing.
        /// </summary>
        public void Stop()
        {
            // Check if the player is playing
            if (m_isPlaying)
            {
                // Reset flags
                m_isPlaying = false;
                m_isPaused = false;

                // Release channels from memory, if they exist
                if(m_audioFiles != null)
                {
                    // Loop through files                     
                    Tracing.Log("[PlayerV3.Stop] Stopping playback...");
                    for (int a = 0; a < m_audioFiles.Length; a++)
                    {
                        // Check if there is a valid channel
                        if (m_audioFiles[a].Channel != null && m_audioFiles[a].Channel.IsPlaying)
                        {
                            // Stop playback
                            Tracing.Log("[PlayerV3.Stop] Stopping channel " + (a + 1).ToString() + " of " + m_audioFiles.Length.ToString());
                            m_audioFiles[a].Channel.Stop();
                        }
                        // Check if there is a valid sound
                        if (m_audioFiles[a].Sound != null)
                        {
                            // Release sound from memory
                            Tracing.Log("[PlayerV3.Stop] Releasing sound " + (a + 1).ToString() + " of " + m_audioFiles.Length.ToString());
                            m_audioFiles[a].Sound.Release();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pauses or unpauses the playback of any channel currently playing.
        /// </summary>        
        public void Pause()
        {
            // Set pause flag
            m_isPaused = !m_isPaused;

            Pause(m_isPaused);
        }

        private Fmod64BitWord wordPauseTime = null;

        /// <summary>
        /// Pauses the playback of any channel currently playing.
        /// </summary>
        /// <param name="paused">If true, the playback will be paused</param>
        public void Pause(bool paused)
        {
            // Check if the player is playing an audio file
            if (!m_isPlaying)
            {
                return;
            }

            // Set pause value
            m_isPaused = paused;

            // Check if this is the last song of the playlist
            if (m_currentAudioFileIndex == m_audioFiles.Length - 1)
            {
                // Pause channels
                m_audioFiles[m_currentAudioFileIndex].Channel.Pause();
            }
            else
            {
                // Check if the playback is to be paused
                if (m_isPaused)
                {
                    m_soundSystem.LockDSP();

                    // Get DSP clock from sound system
                    wordPauseTime = m_soundSystem.GetDSPClock();

                    // Add minimum delay
                    AudioTools.FMOD_64BIT_ADD(ref wordPauseTime.hi, ref wordPauseTime.lo, 0, m_minimumDelay);

                    // Loop through channels
                    for (int a = 0; a < m_audioFiles.Length; a++)
                    {
                        // Check if there is a valid channel
                        if (m_audioFiles[a].Channel != null && m_audioFiles[a].Channel.IsPlaying)
                        {
                            // Set delay on the channel (DSPCLOCK_PAUSE). This will stop the channel at an exact position
                            m_audioFiles[a].Channel.SetDelay(FMOD.DELAYTYPE.DSPCLOCK_PAUSE, wordPauseTime.hi, wordPauseTime.lo);
                        }
                    }

                    m_soundSystem.UnlockDSP();
                    

                    //result = system.getDSPClock(ref hipause_time, ref lopause_time);
                    //ERRCHECK(result);

                    //FMOD.DELAYTYPE_UTILITY.FMOD_64BIT_ADD(ref hipause_time, ref lopause_time, 0, min_delay);

                    //for (int i = 0; i < numsounds; ++i)
                    //{
                    //    bool playing = false;
                    //    result = channels[i].isPlaying(ref playing);
                    //    ERRCHECK_CHANNEL(result);

                    //    if (playing)
                    //    {
                    //        // we use FMOD_DELAYTYPE_DSPCLOCK_PAUSE instead of setPaused to get
                    //        // sample-accurate pausing (so we know exactly when the channel will pause)                                        
                    //        result = channels[i].setDelay(FMOD.DELAYTYPE.DSPCLOCK_PAUSE, hipause_time, lopause_time);
                    //    }
                    //}

                    //// Unpause channels // lock the DSP mixer (make sure the delay values stay in sync)
                    //Tracing.Log("[PlayerV3.Pause] Pausing playback...");
                    ////Tracing.Log("[PlayerV3.Pause] Locking DSP engine...");
                    ////m_soundSystem.LockDSP();

                    //// Get position
                    //m_currentPausePosition = m_audioFiles[m_currentAudioFileIndex].Channel.PositionPCM;                    

                    //// Unlock the DSP mixer (the delays have been set)
                    ////m_soundSystem.UnlockDSP();
                    ////Tracing.Log("[PlayerV3.Pause] Unlocked DSP engine.");

                    //// Stop playback of all current channels                                          
                    //for (int a = 0; a < m_audioFiles.Length; a++)
                    //{
                    //    // Check if there is a valid channel
                    //    if (m_audioFiles[a].Channel != null && m_audioFiles[a].Channel.IsPlaying)
                    //    {
                    //        // Stop playback
                    //        Tracing.Log("[PlayerV3.Pause] Stopping channel " + (a + 1).ToString() + " of " + m_audioFiles.Length.ToString());
                    //        m_audioFiles[a].Channel.Stop();                            
                    //    }
                    //}
                }
                else
                {
                    m_soundSystem.LockDSP();

                    // Get DSP clock from sound system
                    Fmod64BitWord wordCurrentDelay = m_soundSystem.GetDSPClock();

                    // Add minimum delay
                    AudioTools.FMOD_64BIT_ADD(ref wordCurrentDelay.hi, ref wordCurrentDelay.lo, 0, m_minimumDelay);

                    // Get delta
                    Fmod64BitWord wordDelta = new Fmod64BitWord(wordCurrentDelay.hi, wordCurrentDelay.lo);

                    // Substact the pause time from the current time
                    AudioTools.FMOD_64BIT_SUB(ref wordDelta.hi, ref wordDelta.lo, wordPauseTime.hi, wordPauseTime.lo);
                    
                    // Loop through channels
                    for (int a = 0; a < m_audioFiles.Length; a++)
                    {
                        // Check if there is a valid channel
                        if (m_audioFiles[a].Channel != null && m_audioFiles[a].Channel.IsPlaying)
                        {
                            // Get unpause time
                            Fmod64BitWord wordUnpauseTime = m_audioFiles[a].Channel.GetDelay(FMOD.DELAYTYPE.DSPCLOCK_START);

                            // Get position
                            uint positionPcm = m_audioFiles[a].Channel.PositionPCM;

                            // Convert to output mixer sample rate
                            uint positionOutput = (uint)((positionPcm / m_audioFiles[a].SampleRate * m_outputFormatMixer.sampleRate) + 0.5f);

                            Fmod64BitWord wordDeltaPositionOutput = new Fmod64BitWord(wordDelta.hi, wordDelta.lo);
                            AudioTools.FMOD_64BIT_ADD(ref wordDeltaPositionOutput.hi, ref wordDeltaPositionOutput.lo, 0, positionOutput);

                            // BUG INSIDE SUB
                            AudioTools.FMOD_64BIT_SUB(ref wordUnpauseTime.hi, ref wordUnpauseTime.lo, wordDeltaPositionOutput.hi, wordDeltaPositionOutput.lo);
                            m_audioFiles[a].Channel.SetDelay(FMOD.DELAYTYPE.DSPCLOCK_START, wordUnpauseTime.hi, wordUnpauseTime.lo);
                        }
                    }

                    // Loop through channels
                    for (int a = 0; a < m_audioFiles.Length; a++)
                    {
                        // Check if there is a valid channel
                        if (m_audioFiles[a].Channel != null && m_audioFiles[a].Channel.IsPlaying)
                        {
                            m_audioFiles[a].Channel.SetDelay(FMOD.DELAYTYPE.DSPCLOCK_PAUSE, 0, 0);
                            m_audioFiles[a].Channel.Pause(false);
                        }
                    }

                    m_soundSystem.UnlockDSP();

                    //for (int i = 0; i < numsounds; ++i)
                    //{
                    //    result = channels[i].setDelay(FMOD.DELAYTYPE.DSPCLOCK_PAUSE, 0, 0);
                    //    ERRCHECK_CHANNEL(result);

                    //    result = channels[i].setPaused(false);
                    //    ERRCHECK_CHANNEL(result);
                    //}

                    //uint hiCurrentTime = 0;
                    //uint loCurrentTime = 0;
                    //result = system.getDSPClock(ref hiCurrentTime, ref loCurrentTime);

                    //FMOD.DELAYTYPE_UTILITY.FMOD_64BIT_ADD(ref hiCurrentTime, ref loCurrentTime, 0, min_delay);

                    //// calculate how long it's been since we paused; this is how much we need
                    //// to offset the delays of all the channels by
                    //uint hiDelta = hiCurrentTime;
                    //uint loDelta = loCurrentTime;
                    //FMOD.DELAYTYPE_UTILITY.FMOD_64BIT_SUB(ref hiDelta, ref loDelta, hipause_time, lopause_time);

                    //for (int i = 0; i < numsounds; ++i)
                    //{
                    //    bool playing = false;
                    //    result = channels[i].isPlaying(ref playing);
                    //    ERRCHECK_CHANNEL(result);

                    //    if (playing)
                    //    {
                    //        uint hiunpause_time = 0;
                    //        uint lounpause_time = 0;
                    //        result = channels[i].getDelay(FMOD.DELAYTYPE.DSPCLOCK_START, ref hiunpause_time, ref lounpause_time);
                    //        ERRCHECK(result);

                    //        uint position = 0;
                    //        float frequency = 0.0f;
                    //        float volume = 0.0f;
                    //        float pan = 0.0f;
                    //        int priority = 0;

                    //        result = channels[i].getPosition(ref position, FMOD.TIMEUNIT.PCM);
                    //        ERRCHECK(result);

                    //        result = sounds[i].getDefaults(ref frequency, ref volume, ref pan, ref priority);
                    //        ERRCHECK(result);

                    //        // get the channel's position in output samples; the channel will start
                    //        // playing from this position as soon as the start time is reached, so
                    //        // we need to offset the start time from the original by this amount
                    //        //ulong position_output = (ulong)((position / frequency * outputrate) + 0.5);
                    //        uint position_output = (uint)((position / frequency * outputrate) + 0.5);

                    //        uint hiDeltaPositionOutput = hiDelta;
                    //        uint loDeltaPositionOutput = loDelta;
                    //        FMOD.DELAYTYPE_UTILITY.FMOD_64BIT_ADD(ref hiDeltaPositionOutput, ref loDeltaPositionOutput, 0, position_output);

                    //        FMOD.DELAYTYPE_UTILITY.FMOD_64BIT_ADD(ref hiunpause_time, ref lounpause_time, hiDeltaPositionOutput, loDeltaPositionOutput);

                    //        result = channels[i].setDelay(FMOD.DELAYTYPE.DSPCLOCK_START, hiunpause_time, lounpause_time);
                    //    }
                    //}

                    //for (int i = 0; i < numsounds; ++i)
                    //{
                    //    result = channels[i].setDelay(FMOD.DELAYTYPE.DSPCLOCK_PAUSE, 0, 0);
                    //    ERRCHECK_CHANNEL(result);

                    //    result = channels[i].setPaused(false);
                    //    ERRCHECK_CHANNEL(result);
                    //}








                    //Tracing.Log("[PlayerV3.Pause] Unpausing playback...");

                    ////m_audioFiles[m_currentAudioFileIndex].Channel = null;
                    ////m_audioFiles[m_currentAudioFileIndex].Channel = new Channel(m_soundSystem);
                    ////m_audioFiles[m_currentAudioFileIndex].Channel.SoundEnd += new Channel.SoundEndHandler(Channel_SoundEnd);

                    ////m_audioFiles[m_currentAudioFileIndex+1].Channel = null;
                    ////m_audioFiles[m_currentAudioFileIndex+1].Channel = new Channel(m_soundSystem);
                    ////m_audioFiles[m_currentAudioFileIndex+1].Channel.SoundEnd += new Channel.SoundEndHandler(Channel_SoundEnd);

                    //// Schedule the channels and start playback at the paused position
                    //ScheduleChannel(m_currentAudioFileIndex, m_currentPausePosition);
                }
            }
        }
        
        /// <summary>
        /// Schedules the playback between the different channels for gapless playback.
        /// </summary>
        /// <param name="channelStartIndex">Index of the channel to schedule</param>
        /// <param name="startChannelPositionPCM">Start position of the start channel (in PCM samples)</param>
        private void ScheduleChannel(int channelStartIndex, uint startChannelPositionPCM)
        {
            // Check how many songs are left
            if (channelStartIndex + 1 > m_audioFiles.Length - 1)
            {
                // There must be at least one song
                return;
            }

            // Calculate the length in PCM            
            uint length_pcm = (uint)((m_audioFiles[channelStartIndex].Sound.LengthPCM * m_outputFormatMixer.sampleRate / m_audioFiles[channelStartIndex].SampleRate) + 0.5f);

            // Calculate the start position
            uint startPosition_pcm = 0;
            if (startChannelPositionPCM > 0)
            {
                // Set position
                startPosition_pcm = (uint)((startChannelPositionPCM * m_outputFormatMixer.sampleRate / m_audioFiles[channelStartIndex].SampleRate) + 0.5f);
            }

            // Check if the file is an MP3 file with a Xing/Info header and encoder delay information
            uint channel1EncoderPadding = 0;
            uint channel1EncoderDelay = 0;
            if (m_audioFiles[channelStartIndex].FileType == AudioFileType.MP3 && m_audioFiles[channelStartIndex].XingInfoHeader != null && m_audioFiles[channelStartIndex].XingInfoHeader.EncoderDelay != null)
            {
                // Get encoder padding and delay and convert to actual sample rate
                channel1EncoderPadding = (uint)((m_audioFiles[channelStartIndex].XingInfoHeader.EncoderPadding.Value * m_outputFormatMixer.sampleRate / m_audioFiles[channelStartIndex].SampleRate) + 0.5f);
                channel1EncoderDelay = (uint)((m_audioFiles[channelStartIndex].XingInfoHeader.EncoderDelay.Value * m_outputFormatMixer.sampleRate / m_audioFiles[channelStartIndex].SampleRate) + 0.5f);
            }

            // Check if the file is an MP3 file with a Xing/Info header and encoder delay information
            uint channel2EncoderPadding = 0;
            uint channel2EncoderDelay = 0;
            if (m_audioFiles[channelStartIndex + 1].FileType == AudioFileType.MP3 && m_audioFiles[channelStartIndex + 1].XingInfoHeader != null && m_audioFiles[channelStartIndex + 1].XingInfoHeader.EncoderDelay != null)
            {
                // Get encoder padding and delay and convert to actual sample rate
                channel2EncoderPadding = (uint)((m_audioFiles[channelStartIndex + 1].XingInfoHeader.EncoderPadding.Value * m_outputFormatMixer.sampleRate / m_audioFiles[channelStartIndex + 1].SampleRate) + 0.5f);
                channel2EncoderDelay = (uint)((m_audioFiles[channelStartIndex + 1].XingInfoHeader.EncoderDelay.Value * m_outputFormatMixer.sampleRate / m_audioFiles[channelStartIndex + 1].SampleRate) + 0.5f);
            }

            // Lock the DSP mixer (make sure the delay values stay in sync)
            Tracing.Log("[PlayerV3.PlayFiles] Locking DSP engine...");            
            m_soundSystem.LockDSP();

            // Loop through the first two songs
            for (int a = channelStartIndex; a < channelStartIndex + 2; a++)
            {
                // Start playback (in paused mode)                
                m_soundSystem.PlaySound(m_audioFiles[a].Sound, true, m_audioFiles[a].Channel);

                // Set initial volume
                //m_audioFiles[a].Channel.Volume = m_volume;
            }

            // ----------------------------------------------------------------
            // Set channel 1 delay (minimum delay)

            // Get DSP clock start delay            
            Fmod64BitWord wordDelayChannel1 = m_audioFiles[channelStartIndex].Channel.GetDelay(FMOD.DELAYTYPE.DSPCLOCK_START);

            // Added the minimum delay to this value
            uint originalChannel1Delay = wordDelayChannel1.lo;
            AudioTools.FMOD_64BIT_ADD(ref wordDelayChannel1.hi, ref wordDelayChannel1.lo, 0, m_minimumDelay);

            // Set the new DSP clock start delay value            
            m_audioFiles[channelStartIndex].Channel.SetDelay(FMOD.DELAYTYPE.DSPCLOCK_START, wordDelayChannel1.hi, wordDelayChannel1.lo);

            // ----------------------------------------------------------------
            // Set channel 2 delay (minimum delay + sound 1 length)

            // Get DSP clock start delay            
            Fmod64BitWord wordDelayChannel2 = m_audioFiles[channelStartIndex+1].Channel.GetDelay(FMOD.DELAYTYPE.DSPCLOCK_START);

            // Minimum delay + First audio file PCM length - Encoder delay - Encoder padding
            AudioTools.FMOD_64BIT_ADD(ref wordDelayChannel2.hi, ref wordDelayChannel2.lo, 0, m_minimumDelay + length_pcm - startPosition_pcm - channel1EncoderDelay - channel1EncoderPadding);

            // Set the new DSP clock start delay value            
            m_audioFiles[channelStartIndex+1].Channel.SetDelay(FMOD.DELAYTYPE.DSPCLOCK_START, wordDelayChannel2.hi, wordDelayChannel2.lo);

            // Check if a start position must be set on the start channel
            if (startChannelPositionPCM > 0)
            {
                // Set position for the starting channel
                // Note: this overrides the MP3 encoder delay because it is already in the position value
                m_audioFiles[channelStartIndex].Channel.SetPosition(startChannelPositionPCM, FMOD.TIMEUNIT.PCM);
            }
            // Check if the file is an MP3 file with a Xing/Info header and encoder delay information
            else if (m_audioFiles[channelStartIndex].FileType == AudioFileType.MP3 && m_audioFiles[channelStartIndex].XingInfoHeader != null && m_audioFiles[channelStartIndex].XingInfoHeader.EncoderDelay != null)
            {
                // Set position as encoder delay for both channels
                m_audioFiles[channelStartIndex].Channel.SetPosition(channel1EncoderDelay, FMOD.TIMEUNIT.PCM);
                m_audioFiles[channelStartIndex + 1].Channel.SetPosition(channel2EncoderDelay, FMOD.TIMEUNIT.PCM);                
            }

            // Unpause the first two channels            
            for (int a = channelStartIndex; a < channelStartIndex + 2; a++)
            {
                // Check if the channel exists
                if (m_audioFiles[a].Channel != null)
                {
                    // Start playback                        
                    m_audioFiles[a].Channel.Pause(false);
                }
            }

            // Unlock the DSP mixer (the delays have been set)                
            m_soundSystem.UnlockDSP();
            Tracing.Log("[PlayerV3.PlayFiles] Unlocked DSP engine.");
            Tracing.Log("[PlayerV3.PlayFiles] Original channel 1 delay: " + originalChannel1Delay.ToString());
            Tracing.Log("[PlayerV3.PlayFiles] Set channel 1 delay: " + wordDelayChannel1.lo.ToString());
            Tracing.Log("[PlayerV3.PlayFiles] Set channel 2 delay: " + wordDelayChannel2.lo.ToString());
        }

        /// <summary>
        /// This method updates the sound buffer; it needs to be called by a timer on the player client 
        /// every 10ms.
        /// </summary>
        public void Update()
        {
            // Update sound system (necessary to fill the audio stream)
            m_soundSystem.Update();

            // Check if the playback is paused
            if (IsPaused)
            {
                // Do nothing
                return;
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the types of drivers supported by the playback engine.
    /// </summary>
    public enum V3DriverType
    {
        /// <summary>
        /// DirectSound (default). Supported by most sound cards.
        /// </summary>
        DirectSound = 0, 
        /// <summary>
        /// ASIO (for low latency sound cards). This is required for VST plugins.
        /// </summary>
        ASIO = 1, 
        /// <summary>
        /// Windows Audio Session API. Only for Windows Vista and Windows 7. Requires WASAPI drivers.
        /// </summary>
        WASAPI = 2, 
        /// <summary>
        /// Old Windows Multimedia sound engine. Use this only if no other driver works!
        /// </summary>
        WindowsMultimedia = 3, 
        /// <summary>
        /// Writes a wave file on the hard disk instead of sending the stream to the sound card.
        /// </summary>
        WavWriter = 4,
        /// <summary>
        /// No sound. For debugging purposes.
        /// </summary>
        NoSound = 5
    }
}
