//
// Channel.cs: FMOD Wrapper Channel class.
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
using System.Text;
using MPfm.Core;

namespace MPfm.Sound
{
    /// <summary>
    /// This class wraps up the Channel class of the FMOD library.
    /// </summary>
    public class Channel
    {
        // Private variables
        private System system = null;
        public FMOD.Channel baseChannel = null;
        private FMOD.CHANNEL_CALLBACK callbackInternalSoundEnd;

        // Events
        public delegate void SoundEndHandler(EventArgs args);
        public event SoundEndHandler SoundEnd;

        #region Dynamic Properties

        /// <summary>
        /// Returns the current sound of the channel.
        /// </summary>
        public Sound CurrentSound
        {
            get
            {
                FMOD.Sound sound = null;
                FMOD.RESULT result;

                try
                {
                    result = baseChannel.getCurrentSound(ref sound);
                    Channel.CheckForError(result);

                    if (sound == null)
                    {
                        return null;
                    }

                    Sound currentSound = new Sound(sound, "unknown");
                    return currentSound;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns the current audibility of the channel.
        /// </summary>
        public float Audibility
        {
            get
            {
                FMOD.RESULT result;
                float value = 0;

                if (baseChannel != null)
                {
                    result = baseChannel.getAudibility(ref value);
                    Channel.CheckForError(result);
                }

                return value;
            }
        }

        /// <summary>
        /// Returns or sets the current frequency of the channel.
        /// </summary>
        public float Frequency
        {
            get
            {
                FMOD.RESULT result;
                float value = 0;

                if (baseChannel != null)
                {
                    result = baseChannel.getFrequency(ref value);
                    Channel.CheckForError(result);
                }

                return value;
            }
            set
            {
                FMOD.RESULT result;

                if (baseChannel != null)
                {
                    result = baseChannel.setFrequency(value);
                    Channel.CheckForError(result);
                }
            }
        }

        /// <summary>
        /// Returns or sets the current volume of the channel.
        /// </summary>
        public float Volume
        {
            get
            {
                FMOD.RESULT result;
                float value = 0;

                if (baseChannel != null)
                {
                    result = baseChannel.getVolume(ref value);
                    Channel.CheckForError(result);
                }

                return value;
            }
            set
            {
                FMOD.RESULT result;

                if (baseChannel != null)
                {
                    result = baseChannel.setVolume(value);
                    Channel.CheckForError(result);
                }
            }
        }

        /// <summary>
        /// Returns true if a sound is playing in the channel.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                FMOD.RESULT result;
                bool value = false;

                if (baseChannel != null && IsInitialized)
                {
                    result = baseChannel.isPlaying(ref value);
                    Channel.CheckForError(result);
                }

                return value;
            }
        }

        /// <summary>
        /// Returns true if the sound is paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                FMOD.RESULT result;
                bool value = false;

                if (baseChannel != null)
                {
                    result = baseChannel.getPaused(ref value);
                    Channel.CheckForError(result);
                }

                return value;
            }
        }

        /// <summary>
        /// Checks if the channel is properly initialized
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                FMOD.RESULT result = FMOD.RESULT.ERR_HTTP;
                bool value = false;

                if (baseChannel != null)
                {
                    result = baseChannel.isPlaying(ref value);
                    //Channel.CheckForError(result);
                }

                if (result == FMOD.RESULT.ERR_INVALID_PARAM || result == FMOD.RESULT.ERR_INVALID_HANDLE)
                {
                    return false;
                }

                return true;
            }
        }

        #endregion

        #region Position Properties

        #region Standard Position Properties
        
        /// <summary>
        /// Returns the current position of the sound in the channel, in PCM format.
        /// </summary>
        public uint PositionPCM
        {
            get
            {
                FMOD.RESULT result;
                uint value = 0;

                result = baseChannel.getPosition(ref value, FMOD.TIMEUNIT.PCM);
                Channel.CheckForError(result);

                return value;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in PCM bytes format.
        /// </summary>
        public uint PositionPCMBytes
        {
            get
            {
                FMOD.RESULT result;
                uint value = 0;

                result = baseChannel.getPosition(ref value, FMOD.TIMEUNIT.PCMBYTES);
                Channel.CheckForError(result);

                return value;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in absolute milliseconds.
        /// </summary>
        public uint PositionAbsoluteMilliseconds
        {
            get
            {
                // Declare variables
                FMOD.RESULT result;
                uint value = 0;

                try
                {
                    // Try to get position
                    result = baseChannel.getPosition(ref value, FMOD.TIMEUNIT.MS);
                    Channel.CheckForError(result);
                }
                catch
                {
                    // If the position fetch fails, return 0
                    return 0;
                }

                // Return value
                return value;
            }
            set
            {
                // Declare variables
                FMOD.RESULT result;

                // Check if current sound is null
                if (CurrentSound == null)
                {
                    // No current sound; return 0
                    value = 0;
                }
                else
                {
                    // If the position is the same as the length, it will return an error
                    if (value == CurrentSound.LengthAbsoluteMilliseconds)
                    {
                        value--;
                    }

                    // Set position
                    result = baseChannel.setPosition(value, FMOD.TIMEUNIT.MS);
                    Channel.CheckForError(result);
                }
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in minutes.
        /// </summary>
        public double PositionMinutes
        {
            get
            {
                return PositionAbsoluteMilliseconds / 1000 / 60;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in seconds.
        /// </summary>
        public double PositionSeconds
        {
            get
            {
                return PositionAbsoluteMilliseconds / 1000 % 60;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in milliseconds.
        /// </summary>
        public double PositionMilliseconds
        {
            get
            {
                return PositionAbsoluteMilliseconds % 1000;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in 0:00.000 format.
        /// </summary>
        public string Position
        {
            get
            {
                return PositionMinutes.ToString("0") + ":" + PositionSeconds.ToString("00") + "." + PositionMilliseconds.ToString("000");
            }
        }

        #endregion

        #region Sentence Position Properties

        /// <summary>
        /// Returns the current position of the sound in the channel, in PCM format.
        /// </summary>
        public uint PositionSentencePCM
        {
            get
            {
                FMOD.RESULT result;
                uint value = 0;

                result = baseChannel.getPosition(ref value, FMOD.TIMEUNIT.SENTENCE_PCM);
                Channel.CheckForError(result);

                return value;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in PCM bytes format.
        /// </summary>
        public uint PositionSentencePCMBytes
        {
            get
            {
                FMOD.RESULT result;
                uint value = 0;

                result = baseChannel.getPosition(ref value, FMOD.TIMEUNIT.SENTENCE_PCMBYTES);
                Channel.CheckForError(result);

                return value;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in absolute milliseconds.
        /// </summary>
        public uint PositionSentenceAbsoluteMilliseconds
        {
            get
            {
                // Declare variables
                FMOD.RESULT result;
                uint value = 0;

                try
                {
                    // Try to get position
                    result = baseChannel.getPosition(ref value, FMOD.TIMEUNIT.SENTENCE_MS);
                    Channel.CheckForError(result);
                }
                catch
                {
                    // If the position fetch fails, return 0
                    return 0;
                }

                // Return value
                return value;
            }
            set
            {
                // Declare variables
                FMOD.RESULT result;

                // Check if current sound is null
                if (CurrentSound == null)
                {
                    // No current sound; return 0
                    value = 0;
                }
                else
                {
                    // If the position is the same as the length, it will return an error
                    if (value == CurrentSound.LengthAbsoluteMilliseconds)
                    {
                        value--;
                    }

                    // Set position
                    result = baseChannel.setPosition(value, FMOD.TIMEUNIT.SENTENCE_MS);
                    Channel.CheckForError(result);
                }
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in minutes.
        /// </summary>
        public double PositionSentenceMinutes
        {
            get
            {
                return PositionSentenceAbsoluteMilliseconds / 1000 / 60;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in seconds.
        /// </summary>
        public double PositionSentenceSeconds
        {
            get
            {
                return PositionSentenceAbsoluteMilliseconds / 1000 % 60;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in milliseconds.
        /// </summary>
        public double PositionSentenceMilliseconds
        {
            get
            {
                return PositionSentenceAbsoluteMilliseconds % 1000;
            }
        }

        /// <summary>
        /// Returns the current position of the sound in the channel, in 0:00.000 format.
        /// </summary>
        public string PositionSentence
        {
            get
            {
                return PositionSentenceMinutes.ToString("0") + ":" + PositionSentenceSeconds.ToString("00") + "." + PositionSentenceMilliseconds.ToString("000");
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for Channel. Requires a hook to the System class.
        /// </summary>
        /// <param name="system">System</param>
        public Channel(System system)
        {
            Tracing.Log("[MPfm.Sound.Channel] Initialization...");
            this.system = system;

            Tracing.Log("[MPfm.Sound.Channel] Creating base channel...");
            baseChannel = new FMOD.Channel();

            Tracing.Log("[MPfm.Sound.Channel] Setting callbacks...");
            SetCallbacks();     
        }

        /// <summary>
        /// Constructor for Channel based on already created base channel.
        /// </summary>
        /// <param name="system">System</param>
        /// <param name="baseChannel">Base Channel</param>
        public Channel(System system, FMOD.Channel baseChannel)
        {
            this.system = system;
            this.baseChannel = baseChannel;
            SetCallbacks();
        }

        #endregion

        #region Callbacks / Events

        /// <summary>
        /// Set callbacks.
        /// </summary>
        public void SetCallbacks()
        {
            callbackInternalSoundEnd = new FMOD.CHANNEL_CALLBACK(InternalSoundEnd);

            //baseChannel.setCallback(FMOD.CHANNEL_CALLBACKTYPE.END, callbackInternalSoundEnd, 0);
            baseChannel.setCallback(callbackInternalSoundEnd);
        }

        /// <summary>
        /// Occurs when a sound has finished.
        /// </summary>
        /// <param name="args">Event Arguments</param>
        protected void OnSoundEnd(EventArgs args)
        {
            if (SoundEnd != null)
            {
                SoundEnd(args);
            }
        }           

        /// <summary>
        /// Occurs when FMOD has finished playing a sound file.
        /// </summary>
        /// <param name="channelraw">Channel Pointer</param>
        /// <param name="type">Callback Type</param>
        /// <param name="commanddata1">Data 1</param>
        /// <param name="commanddata2">Data 2</param>
        /// <returns>Result</returns>
        private FMOD.RESULT InternalSoundEnd(IntPtr channelraw, FMOD.CHANNEL_CALLBACKTYPE type, IntPtr commanddata1, IntPtr commanddata2)
        {
            // Raise event
            EventArgs args = new EventArgs();
            OnSoundEnd(args);

            // Return OK
            return FMOD.RESULT.OK;
        }

        #endregion

        #region Playback

        /// <summary>
        /// Pauses the channel 
        /// </summary>
        public void Pause()
        {
            FMOD.RESULT result;            

            if (baseChannel != null)
            {                
                result = baseChannel.setPaused(!IsPaused);
                Channel.CheckForError(result);
            }
        }

        /// <summary>
        /// Pauses the channel 
        /// </summary>
        public void Pause(bool pause)
        {
            FMOD.RESULT result;

            if (baseChannel != null)
            {
                result = baseChannel.setPaused(pause);
                Channel.CheckForError(result);
            }
        }

        /// <summary>
        /// Stops any sound playing.
        /// </summary>
        public void Stop()
        {
            FMOD.RESULT result;

            if (baseChannel != null)
            {
                result = baseChannel.stop();
                Channel.CheckForError(result);

                // Set channel to null (2009)
                baseChannel = null;
            }
        }

        #endregion

        #region Position Methods

        /// <summary>
        /// Returns the position of the sound in the channel, using the specified time unit
        /// passed in parameter. Can also return the buffered value (see FMOD documentation 
        /// for more information).
        /// </summary>
        /// <param name="timeUnit">FMOD Time Unit</param>
        /// <param name="buffered">Buffered Value</param>
        /// <returns>Position</returns>
        public uint GetPosition(FMOD.TIMEUNIT timeUnit, bool buffered)
        {
            // Create variables 
            FMOD.RESULT result;
            uint position = 0;

            // Get position
            if (buffered)
            {
                result = baseChannel.getPosition(ref position, timeUnit | FMOD.TIMEUNIT.BUFFERED);               
            }
            else
            {
                result = baseChannel.getPosition(ref position, timeUnit);
            }
                           
            Channel.CheckForError(result);

            return position;
        }

        /// <summary>
        /// Sets the position of the sound in the channel, using the specified time unit
        /// passed in parameter.
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="timeUnit">FMOD Time Unit</param>
        public void SetPosition(uint position, FMOD.TIMEUNIT timeUnit)
        {
            // Create variables 
            FMOD.RESULT result;            

            // Set position
            result = baseChannel.setPosition(position, timeUnit);
            Channel.CheckForError(result);
        }

        /// <summary>
        /// Sets the position of the current channel, in milliseconds.
        /// </summary>
        /// <param name="position">Position (in milliseconds)</param>
        public void SetPosition(uint position)
        {
            // Check if a current sound exists
            if (CurrentSound != null)
            {
                PositionAbsoluteMilliseconds = position;
            }
        }

        /// <summary>
        /// Sets the position of the current channel (sentence), in milliseconds.
        /// </summary>
        /// <param name="position">Position (in milliseconds)</param>
        public void SetPositionSentence(uint position)
        {
            // Check if a current sound exists
            if (CurrentSound != null)
            {
                PositionSentenceAbsoluteMilliseconds = position;
            }
        }

        /// <summary>
        /// Sets the position of the current channel, in percentage.
        /// </summary>
        /// <param name="percentage">Position (in percentage)</param>
        /// <returns>New position</returns>
        public uint SetPosition(double percentage)
        {
            uint newPosition = 0;

            // Check if a current sound exists
            if (CurrentSound != null)
            {
                // Get the position by the specified percentage and set position
                newPosition = Convert.ToUInt32(percentage * (double)CurrentSound.LengthAbsoluteMilliseconds);
                SetPosition(newPosition);                
            }

            return newPosition;
        }

        #endregion

        /// <summary>
        /// [FMOD documentation] Gets the currently set delay values.  
        /// </summary>
        /// <param name="delayType">Type of delay (FMOD_DELAYTYPE_END_MS, FMOD_DELAYTYPE_DSPCLOCK_START, 
        /// FMOD_DELAYTYPE_DSPCLOCK_END, FMOD_DELAYTYPE_DSPCLOCK_PAUSE, FMOD_DELAYTYPE_MAX)</param>
        /// <returns>64-bit word</returns>
        public Fmod64BitWord GetDelay(FMOD.DELAYTYPE delayType)
        {
            // Declare variables
            FMOD.RESULT result;            
            uint delayhi = 0;
            uint delaylo = 0;

            // Check if base channel exists
            if (baseChannel != null)
            {
                // Get delay
                result = baseChannel.getDelay(delayType, ref delayhi, ref delaylo);
                Channel.CheckForError(result);

                // Return delay value in 64-bit word
                return new Fmod64BitWord(delayhi, delaylo);
            }

            return new Fmod64BitWord();
        }

        /// <summary>
        /// [FMOD documentation] Sets an end delay for a sound (so that dsp can continue to process the finished 
        /// sound), set the start of the sound according to the global DSP clock value which represents the time 
        /// in the mixer timeline.  
        /// </summary>
        /// <param name="delayType">Type of delay (FMOD_DELAYTYPE_END_MS, FMOD_DELAYTYPE_DSPCLOCK_START, 
        /// FMOD_DELAYTYPE_DSPCLOCK_END, FMOD_DELAYTYPE_DSPCLOCK_PAUSE, FMOD_DELAYTYPE_MAX)</param>
        /// <param name="delayhi">Top (most significant) 32 bits of a 64bit number representing the time.</param>
        /// <param name="delaylo">Bottom (least significant) 32 bits of a 64bit number representing the time.</param>
        public void SetDelay(FMOD.DELAYTYPE delayType, uint delayhi, uint delaylo)
        {
            // Declare variables
            FMOD.RESULT result;

            // Check if base channel exists
            if (baseChannel != null)
            {
                // Set delay
                result = baseChannel.setDelay(delayType, delayhi, delaylo);
                Channel.CheckForError(result);
            }
        }

        public void SetDelayMs(uint delay)
        {
            FMOD.RESULT result;

            if (baseChannel != null)
            {
                result = baseChannel.setDelay(FMOD.DELAYTYPE.END_MS, delay, 0);
                Channel.CheckForError(result);
            }
        }

        #region Wave Data / Spectrum

        /// <summary>
        /// Gets the spectrum data from FMOD.
        /// </summary>
        /// <returns>Float array (wave data)</returns>
        public float[] GetSpectrum()
        {
            int spectrumSize = 512;
            float[] spectrum = new float[spectrumSize];

            if (baseChannel != null)
            {
                FMOD.RESULT result = baseChannel.getSpectrum(spectrum, spectrumSize, 0, FMOD.DSP_FFT_WINDOW.TRIANGLE);
                Channel.CheckForError(result);
            }

            return spectrum;
        }

        /// <summary>
        /// Returns the wave data from the left and right channel and averages the value
        /// between the two channels.
        /// </summary>
        /// <returns>Float array (wave data)</returns>
        public float[] GetWaveDataAverageLeftRight()
        {
            int waveDataSize = 256;
            float[] waveDataLeft = new float[waveDataSize];
            float[] waveDataRight = new float[waveDataSize];
            float[] waveData = new float[waveDataSize];

            if (baseChannel != null)
            {
                FMOD.RESULT resultLeft = baseChannel.getWaveData(waveDataLeft, waveDataSize, 0);
                Channel.CheckForError(resultLeft);
                FMOD.RESULT resultRight = baseChannel.getWaveData(waveDataRight, waveDataSize, 1);
                Channel.CheckForError(resultRight);

                for (int a = 0; a < waveDataSize; a++)
                {
                    waveData[a] = (waveDataLeft[a] + waveDataRight[a]) / 2;
                }

            }

            return waveData;
        }

        /// <summary>
        /// Returns the wave data from the left or mono channel.
        /// </summary>
        /// <returns>Float array (wave data)</returns>
        public float[] GetWaveDataLeft()
        {
            int waveDataSize = 256;
            float[] waveData = new float[waveDataSize];

            if (baseChannel != null)
            {
                FMOD.RESULT result = baseChannel.getWaveData(waveData, waveDataSize, 0);
                Channel.CheckForError(result);
            }

            return waveData;
        }

        /// <summary>
        /// Returns the wave data from the right channel.
        /// </summary>
        /// <returns>Float array (wave data)</returns>
        public float[] GetWaveDataRight()
        {
            int waveDataSize = 256;
            float[] waveData = new float[waveDataSize];

            if (baseChannel != null)
            {
                FMOD.RESULT result = baseChannel.getWaveData(waveData, waveDataSize, 1);
            }

            return waveData;
        }

        #endregion

        /// <summary>
        /// Checks the result of a FMOD channel operation. If an error is found, throw a new exception.
        /// Ignores the ERR_INVALID_HANDLE and ERR_CHANNEL_STOLEN results.
        /// </summary>
        /// <param name="result">FMOD result</param>
        private static void CheckForError(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK && result != FMOD.RESULT.ERR_INVALID_HANDLE && result != FMOD.RESULT.ERR_CHANNEL_STOLEN)
            {
                throw new Exception(result.ToString());
            }
        }
    }
}
