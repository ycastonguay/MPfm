//
// System.cs: FMOD Wrapper System class.
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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MPfm.Core;

namespace MPfm.Sound
{
    /// <summary>
    /// This class wraps up the System class of the FMOD library.
    /// </summary>
    public class System
    {
        #region Properties

        private FMOD.DSPConnection dspConnection = null;
        public FMOD.DSPConnection DspConnection
        {
            get
            {
                return dspConnection;
            }
        }

        private FMOD.System baseSystem = null;
        public FMOD.System BaseSystem
        {
            get
            {
                return baseSystem;
            }
        }

        /// <summary>
        /// [FMOD documentation] Returns the current version of FMOD Ex being used.  
        /// </summary>
        public uint Version
        {
            get
            {
                // Declare variables
                FMOD.RESULT result;
                uint value = 0;

                // Check if base system exists
                if (baseSystem != null)
                {
                    // Get version
                    result = baseSystem.getVersion(ref value);
                    CheckForError(result);
                }

                return value;
            }
        }

        /// <summary>
        /// [FMOD documentation] Retrieves the number of currently playing channels.  
        /// </summary>
        public int NumberOfChannelsPlaying
        {
            get
            {
                // Declare variables
                FMOD.RESULT result;
                int value = 0;

                // Check if base system exists
                if (baseSystem != null)
                {
                    // Get version
                    result = baseSystem.getChannelsPlaying(ref value);
                    CheckForError(result);
                }

                return value;
            }
        }

        public int NumDrivers
        {
            get
            {
                FMOD.RESULT result;
                int value = 0;

                result = baseSystem.getNumDrivers(ref value);
                System.CheckForError(result);

                return value;
            }
        }

        private List<OutputType> m_outputTypes = null;
        public List<OutputType> OutputTypes
        {
            get
            {
                return m_outputTypes;
            }
        }

        private List<string> m_drivers = null;
        public List<string> Drivers
        {
            get
            {
                return m_drivers;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the FMOD System (based on FMOD.System). Requires the 
        /// output type and output device name to start.
        /// </summary>
        /// <param name="outputType">Output Type</param>
        /// <param name="outputDeviceName">Output Device</param>
        public System(FMOD.OUTPUTTYPE outputType, string outputDeviceName)
        {
            try
            {
                // Create list of FMOD "output types" (in reality, those are audio drivers)
                m_outputTypes = GetOutputTypes();

                // Set default result
                FMOD.RESULT result = FMOD.RESULT.OK;

                // Create system                                
                Tracing.Log("[MPfm.Sound.System] Creating sound system...");
                result = FMOD.Factory.System_Create(ref baseSystem);
                CheckForError(result);

                // Check FMOD version
                Tracing.Log("[MPfm.Sound.System] Checking FMOD version...");
                uint version = 0;
                result = baseSystem.getVersion(ref version);
                CheckForError(result);
                Tracing.Log("[MPfm.Sound.System] FMOD Version: " + version.ToString());
                if (version < FMOD.VERSION.number)
                {
                    Tracing.Log("[MPfm.Sound.System]The FMOD version is not compatible (C#: " + version + " / FMOD: " + FMOD.VERSION.number + ")!");
                }

                // Getting drivers
                Tracing.Log("[MPfm.Sound.System] Getting drivers...");
                int numberOfDrivers = 0;
                result = baseSystem.getNumDrivers(ref numberOfDrivers);
                CheckForError(result);

                // Check if a driver is available
                int outputDeviceIndex = 0;
                if (numberOfDrivers == 0)
                {
                    Tracing.Log("[MPfm.Sound.System] No driver was found.");
                }
                else
                {
                    Tracing.Log("[MPfm.Sound.System] " + numberOfDrivers + " drivers found.");

                    // Create list of drivers
                    m_drivers = new List<string>();

                    // List drivers
                    for (int a = 0; a < numberOfDrivers; a++)
                    {
                        // Get driver name
                        string driverName = GetDriverName(a);                        

                        // Add to driver list                       
                        m_drivers.Add(driverName);

                        // If the name of the driver is the same than the one passed in parameter
                        if(driverName == outputDeviceName)
                        {
                            // We found the device
                            outputDeviceIndex = a;
                        }

                        // List in log file
                        Tracing.Log("[MPfm.Sound.System] Driver " + (a + 1).ToString() + ": " + driverName);
                    }
                }

                // Set output device
                result = baseSystem.setOutput(outputType);
                CheckForError(result);

                // Set sound card driver
                result = baseSystem.setDriver(outputDeviceIndex);
                CheckForError(result);

                // Check if the output type is WAV writer
                if (outputType == FMOD.OUTPUTTYPE.WAVWRITER)
                {
                    // Init system
                    Tracing.Log("[MPfm.Sound.System] Initializing sound system (WavWriter)...");
                    result = baseSystem.init(32, FMOD.INITFLAGS.NORMAL, Marshal.StringToHGlobalUni(@"test.wav"));
                    CheckForError(result);
                }
                else
                {
                    // Init system
                    Tracing.Log("[MPfm.Sound.System] Initializing sound system...");
                    result = baseSystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)null);
                    CheckForError(result);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("[MPfm.Sound.System] ERROR! " + ex.Message + " // " + ex.StackTrace);
                throw ex;
            }

        }

        /// <summary>
        /// Closes the sound system. Must use Release() after this method.
        /// </summary>
        public void Close()
        {
            FMOD.RESULT result = BaseSystem.close();
            System.CheckForError(result);
        }

        /// <summary>
        /// Releases the sound system from memory. Close() should be called right before this mehod.
        /// </summary>
        public void Release()
        {
            FMOD.RESULT result = BaseSystem.release();
            System.CheckForError(result);
        }

        /// <summary>
        /// [FMOD documentation] Returns the current internal buffersize settings for streamable sounds.  
        /// </summary>
        /// <param name="bufferSize">Address of a variable that receives the current stream file buffer size setting. 
        /// Default is 16384 (FMOD_TIMEUNIT_RAWBYTES). Optional. Specify 0 or NULL to ignore. </param>
        /// <param name="timeUnit">Address of a variable that receives the type of unit for the current stream file buffer 
        /// size setting. Can be FMOD_TIMEUNIT_MS, FMOD_TIMEUNIT_PCM, FMOD_TIMEUNIT_PCMBYTES or FMOD_TIMEUNIT_RAWBYTES. 
        /// Default is FMOD_TIMEUNIT_RAWBYTES. Optional. Specify 0 or NULL to ignore.</param>
        public void GetStreamBufferSize(ref uint bufferSize, ref FMOD.TIMEUNIT timeUnit)
        {
            FMOD.RESULT result = BaseSystem.getStreamBufferSize(ref bufferSize, ref timeUnit);            
            System.CheckForError(result);
        }

        /// <summary>
        /// [FMOD documentation] Sets the internal buffersize for streams opened after this call. Larger values will consume more memory 
        /// (see remarks), whereas smaller values may cause buffer under-run/starvation/stuttering caused by large delays 
        /// in disk access (ie CDROM or netstream), or cpu usage in slow machines, or by trying to play too many streams at once.
        /// </summary>
        /// <param name="bufferSize">Size of the stream file buffer</param>
        /// <param name="timeUnit">Time unit</param>
        public void SetStreamBufferSize(uint bufferSize, FMOD.TIMEUNIT timeUnit)
        {
            FMOD.RESULT result = BaseSystem.setStreamBufferSize(bufferSize, timeUnit);
            System.CheckForError(result);
        }

        /// <summary>
        /// [FMOD documentation] Retrieves the buffer size settings for the FMOD software mixing engine.  
        /// </summary>
        /// <param name="bufferLength">Address of a variable that receives the mixer engine block size in samples. Default = 1024. 
        /// (milliseconds = 1024 at 48khz = 1024 / 48000 * 1000 = 10.66ms). </param>
        /// <param name="numBuffers">Address of a variable that receives the mixer engine number of buffers used. Default = 4. 
        /// To get the total buffersize multiply the bufferlength by the numbuffers value. By default this would be 4*1024. </param>
        public void GetDSPBufferSize(ref uint bufferLength, ref int numBuffers)
        {
            FMOD.RESULT result = baseSystem.getDSPBufferSize(ref bufferLength, ref numBuffers);
            System.CheckForError(result);
        }

        /// <summary>
        /// [FMOD documentation] Sets the FMOD internal mixing buffer size. This function is used if you need to control mixer 
        /// latency or granularity.  Smaller buffersizes lead to smaller latency, but can lead to stuttering/skipping/instable sound on 
        /// slower machines or soundcards with bad drivers.  
        /// </summary>
        /// <param name="bufferLength">The mixer engine block size in samples. Use this to adjust mixer update granularity. 
        /// Default = 1024. (milliseconds = 1024 at 48khz = 1024 / 48000 * 1000 = 21.33ms). This means the mixer updates every 21.33ms. </param>
        /// <param name="numBuffers">The mixer engine number of buffers used. Use this to adjust mixer latency. Default = 4. To get the total 
        /// buffersize multiply the bufferlength by the numbuffers value. By default this would be 4*1024. </param>
        public void SetDSPBufferSize(uint bufferLength, int numBuffers)
        {
            FMOD.RESULT result = BaseSystem.setDSPBufferSize(bufferLength, numBuffers);
            System.CheckForError(result);
        }

        /// <summary>
        /// [FMOD documentation] Retrieves the buffer size settings for the FMOD software mixing engine.  
        /// </summary>
        /// <returns>DSPBufferSize data structure</returns>
        public DSPBufferSize GetDSPBufferSize()
        {
            DSPBufferSize data = new DSPBufferSize();
            FMOD.RESULT result = baseSystem.getDSPBufferSize(ref data.bufferLength, ref data.numBuffers);
            System.CheckForError(result);
            return data;
        }

        /// <summary>
        /// [FMOD documentation] Return the current 64bit DSP clock value which counts up by the number of samples per second 
        /// in the software mixer, every second.  Ie if the default sample rate is 48khz, the DSP clock increments by 48000 
        /// per second.  
        /// </summary>
        /// <returns>FMOD 64-bit word</returns>
        public Fmod64BitWord GetDSPClock()
        {
            Fmod64BitWord word = new Fmod64BitWord();
            FMOD.RESULT result = baseSystem.getDSPClock(ref word.hi, ref word.lo);
            System.CheckForError(result);
            return word;
        }

        /// <summary>
        /// [FMOD documentation] Mutual exclusion function to lock the FMOD DSP engine (which runs asynchronously in another thread), 
        /// so that it will not execute. If the FMOD DSP engine is already executing, this function will block until it has completed. 
        /// The function may be used to synchronize DSP network operations carried out by the user.
        /// </summary>
        public void LockDSP()
        {
            // Lock DSP
            FMOD.RESULT result = baseSystem.lockDSP();
            System.CheckForError(result);
        }

        /// <summary>
        /// [FMOD documentation] Mutual exclusion function to unlock the FMOD DSP engine (which runs asynchronously in another thread) 
        /// and let it continue executing.  
        /// </summary>
        public void UnlockDSP()
        {
            // Unlock DSP
            FMOD.RESULT result = baseSystem.unlockDSP();
            System.CheckForError(result);
        }
       
        /// <summary>
        /// Returns the list of supported output devices without starting the system
        /// (this is a static method).
        /// </summary>
        /// <returns>List of supported output devices</returns>
        public static List<string> GetOutputDevicesWithoutStartingSystem()
        {
            List<string> listOutputDevices = new List<string>();

            try
            {
                FMOD.RESULT result = FMOD.RESULT.OK;
                FMOD.System system = null;

                // Create system                                
                Tracing.Log("MPfm.Sound (System.GetOutputDevicesWithoutStartingSystem) -- Creating sound system...");
                result = FMOD.Factory.System_Create(ref system);
                CheckForError(result);

                // Check FMOD version
                Tracing.Log("MPfm.Sound (System.GetOutputDevicesWithoutStartingSystem) -- Checking FMOD version...");
                uint version = 0;
                system.getVersion(ref version);
                Tracing.Log("MPfm.Sound (System.GetOutputDevicesWithoutStartingSystem) -- FMOD Version: " + version.ToString());
                if (version < FMOD.VERSION.number)
                {
                    Tracing.Log("MPfm.Sound (System.GetOutputDevicesWithoutStartingSystem) -- The FMOD version is not compatible (C#: " + version + " / FMOD: " + FMOD.VERSION.number + ")!");
                }

                // Getting drivers
                Tracing.Log("MPfm.Sound (System.GetOutputDevicesWithoutStartingSystem) -- Getting drivers...");
                int numberOfDrivers = 0;
                system.getNumDrivers(ref numberOfDrivers);                

                // Check if a driver is available
                if (numberOfDrivers == 0)
                {
                    Tracing.Log("MPfm.Sound (System.GetOutputDevicesWithoutStartingSystem) -- No driver was found.");
                }
                else
                {
                    Tracing.Log("MPfm.Sound (System.GetOutputDevicesWithoutStartingSystem) -- " + numberOfDrivers + " drivers found.");

                    // List drivers
                    for (int a = 0; a < numberOfDrivers; a++)
                    {
                        // Get driver name
                        FMOD.GUID guid = new FMOD.GUID();
                        StringBuilder sb = new StringBuilder(500);

                        result = system.getDriverInfo(a, sb, 500, ref guid);                        
                        System.CheckForError(result);

                        string driverName = sb.ToString();
                        Tracing.Log("MPfm.Sound (System.GetOutputDevicesWithoutStartingSystem) -- Driver " + (a + 1).ToString() + ": " + driverName);

                        listOutputDevices.Add(driverName);
                    }
                }                              

                return listOutputDevices;
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Sound (System.GetOutputDevicesWithoutStartingSystem) -- ERROR: " + ex.Message + " // " + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Updates the FMOD system. Should be called once per frame in a application (or game tick).
        /// The standard is 10ms.
        /// </summary>
        /// <returns>Successful if true</returns>
        public bool Update()
        {
            FMOD.RESULT result;

            if (baseSystem != null)
            {
                result = baseSystem.update();
                CheckForError(result);
            }

            return true;
        }

        #region Output Type

        /// <summary>
        /// Returns the list of supported output types for MPfm.
        /// </summary>
        /// <returns>List of supported output types</returns>
        public static List<OutputType> GetOutputTypes()
        {
            // Create list of output types
            List<OutputType> outputTypes = new List<OutputType>();
            outputTypes.Add(new OutputType() { FMODOutputType = FMOD.OUTPUTTYPE.DSOUND, Description = "DirectSound (default, recommended for Windows XP)" });
            outputTypes.Add(new OutputType() { FMODOutputType = FMOD.OUTPUTTYPE.WASAPI, Description = "Windows Audio Session API (recommended/only for Windows Vista and Windows 7)" });
            outputTypes.Add(new OutputType() { FMODOutputType = FMOD.OUTPUTTYPE.ASIO, Description = "ASIO 2.0 (ASIO low latency driver required, mandatory for VST plugins)" });
            //outputTypes.Add(new OutputType() { FMODOutputType = FMOD.OUTPUTTYPE.OPENAL, Description = "OpenAL 1.1 (OpenAL driver required)" });
            outputTypes.Add(new OutputType() { FMODOutputType = FMOD.OUTPUTTYPE.WINMM, Description = "Windows Multimedia (for older hardware or virtual machines)" });
            outputTypes.Add(new OutputType() { FMODOutputType = FMOD.OUTPUTTYPE.NOSOUND, Description = "No sound (for debugging)" });

            return outputTypes;
        }

        /// <summary>
        /// Sets the current output type of the sound system.
        /// </summary>
        /// <param name="outputType">Output Type</param>
        public void SetOutputType(FMOD.OUTPUTTYPE outputType)
        {
            try
            {
                // Check if system is initialized
                if (baseSystem == null)
                {
                    // Throw exception
                    throw new Exception("The system is not properly initialized! baseSystem is null.");
                }

                // Set output
                FMOD.RESULT result = baseSystem.setOutput(outputType);

                // Check for errors
                CheckForError(result);
            }
            catch (Exception ex)
            {
                // Handle exception
                HandleException(ex);
            }
        }

        /// <summary>
        /// Returns the current output type of the sound system.
        /// </summary>
        /// <returns>Output Type</returns>
        public FMOD.OUTPUTTYPE GetOutputType()
        {
            // Create variables
            FMOD.OUTPUTTYPE outputType = FMOD.OUTPUTTYPE.UNKNOWN;

            try
            {
                // Check if system is initialized
                if (baseSystem == null)
                {
                    // Throw exception
                    throw new Exception("The system is not properly initialized! baseSystem is null.");
                }

                // Set output
                FMOD.RESULT result = baseSystem.getOutput(ref outputType);

                // Check for errors
                CheckForError(result);
            }
            catch (Exception ex)
            {
                // Handle exception
                HandleException(ex);
            }

            // Return value
            return outputType;
        }

        #endregion

        #region Drivers

        /// <summary>
        /// Gets the name of a driver.
        /// </summary>
        /// <param name="id">Driver Id (index)</param>
        /// <returns>Name of the driver</returns>
        private string GetDriverName(int id)
        {
            FMOD.RESULT result;
            FMOD.GUID guid = new FMOD.GUID();
            StringBuilder sb = new StringBuilder(500);

            result = baseSystem.getDriverInfo(id, sb, 500, ref guid);
            System.CheckForError(result);

            return sb.ToString();
        }

        public int GetDriver()
        {
            int driver = 0;
            FMOD.RESULT result = baseSystem.getDriver(ref driver);

            return driver;
        }

        /// <summary>
        /// Get a list of drivers.
        /// </summary>
        /// <returns>Array of drivers</returns>
        public Driver[] GetDrivers()
        {
            int a;
            Driver[] drivers = new Driver[NumDrivers];

            for (a = 0; a < NumDrivers; a++)
            {
                drivers[a] = new Driver(a, GetDriverName(a));
            }

            return drivers;
        }

        #endregion

        #region Sound

        /// <summary>
        /// Load a sound file into memory (FMOD createSound).
        /// </summary>
        /// <param name="filePath">Path of the sound file</param>
        /// <param name="buffer">Buffer sound</param>
        /// <returns></returns>
        public Sound CreateSound(string filePath, bool buffer)
        {
            FMOD.Sound sound = null;
            FMOD.RESULT result;

            if (baseSystem != null)
            {
                if (buffer)
                {
                    result = baseSystem.createSound(filePath, FMOD.MODE.SOFTWARE | FMOD.MODE.CREATESTREAM | FMOD.MODE.ACCURATETIME, ref sound);
                    //result = baseSystem.createSound(filePath, FMOD.MODE.HARDWARE | FMOD.MODE._2, ref sound);
                    //result = baseSystem.createSound(filePath, FMOD.MODE.SOFTWARE, ref sound);
                }
                else
                {
                    result = baseSystem.createSound(filePath, FMOD.MODE.SOFTWARE | FMOD.MODE.OPENONLY /*| FMOD.MODE.ACCURATETIME*/, ref sound);
                    //result = baseSystem.createSound(filePath, FMOD.MODE.SOFTWARE, ref sound);
                    //result = baseSystem.createSound(filePath, FMOD.MODE.HARDWARE | FMOD.MODE._2D /*| FMOD.MODE.ACCURATETIME*/, ref sound);
                }
                CheckForError(result);
            }

            return new Sound(sound, filePath);
        }

        /// <summary>
        /// Load a sound file into memory using a stream.
        /// </summary>
        /// <param name="filePath">Path of the sound file</param>        
        /// <returns></returns>
        public Sound CreateStream(string filePath)
        {
            FMOD.Sound sound = null;
            FMOD.RESULT result;

            if (baseSystem != null)
            {
                // Created extended info for FMOD real-time stiching
                FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
                exinfo.cbsize = Marshal.SizeOf(exinfo);
                //exinfo.defaultfrequency = 44100;
                ////exinfo.numsubsounds = filePaths.Count;
                //exinfo.numchannels = 2;
                //exinfo.format = FMOD.SOUND_FORMAT.MPEG;

                //result = baseSystem.createStream(filePath, FMOD.MODE.SOFTWARE | FMOD.MODE.OPENONLY, ref sound);
                result = baseSystem.createStream(filePath, FMOD.MODE.OPENONLY, ref sound);
                CheckForError(result);
            }

            return new Sound(sound, filePath);
        }

        /// <summary>
        /// Play a sound on a specific channel. The Sound object must have
        /// been created before using this method. A valid Channel object must be
        /// passed in parameter.
        /// </summary>
        /// <param name="sound">Sound object to player</param>
        /// <param name="paused">If true, the playback will start paused</param>
        /// <param name="channel">Target channel</param>
        /// <returns>Successful if true</returns>
        public bool PlaySound(Sound sound, bool paused, Channel channel)
        {
            FMOD.RESULT result;

            // Check if the base system is null
            if (baseSystem != null)
            {
                // Play sound on channel
                result = baseSystem.playSound(FMOD.CHANNELINDEX.FREE, sound.BaseSound, paused, ref channel.baseChannel);
                CheckForError(result);

                // Setup channel callbacks
                channel.SetCallbacks();
                //MainChannel.SetCallbacks();
            }

            return true;
        }

        /// <summary>
        /// Plays a gapless sequence of sound files on a specific channel.
        /// </summary>
        /// <param name="filePaths">File paths of sound files</param>
        /// <param name="soundFormat">Data structure containing the sound format of the first file</param>     
        /// <param name="channel">Target channel</param>
        /// <returns>Gapless Sequence Data Structure (null if failed)</returns>
        public GaplessSequenceData PlayGaplessSequence(List<string> filePaths, SoundFormat soundFormat, Channel channel)
        {
            // Create variables
            FMOD.RESULT result;

            // Check for nulls
            if (baseSystem == null)
            {
                return null;
            }

            // Created extended info for FMOD real-time stiching
            FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
            exinfo.cbsize = Marshal.SizeOf(exinfo);
            exinfo.defaultfrequency = soundFormat.Frequency;
            exinfo.numsubsounds = filePaths.Count;
            exinfo.numchannels = soundFormat.Channels;
            exinfo.format = soundFormat.Format;            

            // Create sound data
            GaplessSequenceData data = new GaplessSequenceData();
            data.sound = null;
            data.subSounds = new FMOD.Sound[filePaths.Count];

            // Set buffer size
            SetStreamBufferSize(256 * 1024, FMOD.TIMEUNIT.RAWBYTES);
            //SetDSPBufferSize()

            // Create parent stream which will contain the substreams. Loop through selection.
            //result = baseSystem.createStream(string.Empty, FMOD.MODE.LOOP_OFF | FMOD.MODE.OPENUSER, ref exinfo, ref data.sound);
            result = baseSystem.createStream(string.Empty, FMOD.MODE.LOOP_NORMAL | FMOD.MODE.OPENUSER, ref exinfo, ref data.sound);
            CheckForError(result);

            // Create streams for each file
            for (int a = 0; a < filePaths.Count; a++)
            {
                if (Path.GetExtension(filePaths[a]).ToUpper().Contains("MP3"))
                {
                    // Create stream
                    result = baseSystem.createStream(filePaths[a], FMOD.MODE.DEFAULT, ref data.subSounds[a]);
                }
                else
                { 
                    // Create stream
                    //result = baseSystem.createStream(filePaths[a], FMOD.MODE.DEFAULT, ref data.subSounds[a]);
                    result = baseSystem.createStream(filePaths[a], FMOD.MODE.DEFAULT, ref data.subSounds[a]);
                }
                //result = baseSystem.createStream(filePaths[a], FMOD.MODE.SOFTWARE | FMOD.MODE._2D, ref data.subSounds[a]);
                CheckForError(result);
            }

            // Set subsounds for each file
            for (int a = 0; a < filePaths.Count; a++)
            {
                // Set subsound
                result = data.sound.setSubSound(a, data.subSounds[a]);
                CheckForError(result);
            }

            // Setup gapless sentence            
            int[] soundlist = new int[filePaths.Count];
            for (int a = 0; a < filePaths.Count; a++)
            {
                soundlist[a] = a;
            }
            result = data.sound.setSubSoundSentence(soundlist, soundlist.Length);
            CheckForError(result);

            // Play the master sound            
            result = baseSystem.playSound(FMOD.CHANNELINDEX.FREE, data.sound, false, ref channel.baseChannel);
            CheckForError(result);

            return data;
        }

        #endregion

        #region Wave Data / Spectrum

        /// <summary>
        /// Gets the spectrum data from FMOD.
        /// </summary>
        /// <returns>Float array (wave data)</returns>
        public float[] GetSpectrum()
        {
            int spectrumSize = 512;
            float[] spectrum = new float[spectrumSize];

            if (baseSystem != null)
            {
                FMOD.RESULT result = baseSystem.getSpectrum(spectrum, spectrumSize, 0, FMOD.DSP_FFT_WINDOW.TRIANGLE);
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

            if (baseSystem != null)
            {
                FMOD.RESULT resultLeft = baseSystem.getWaveData(waveDataLeft, waveDataSize, 0);
                FMOD.RESULT resultRight = baseSystem.getWaveData(waveDataRight, waveDataSize, 1);

                for (int a = 0; a < waveDataSize; a++ )
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

            if (baseSystem != null)
            {
                FMOD.RESULT result = baseSystem.getWaveData(waveData, waveDataSize, 0);
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

            if (baseSystem != null)
            {
                FMOD.RESULT result = baseSystem.getWaveData(waveData, waveDataSize, 1);
            }

            return waveData;
        }

        #endregion

        #region DSP / VST Plugins

        /// <summary>
        /// Returns the number of supported DSP plugins on the current hardware.
        /// </summary>
        /// <returns>Number of supported DSP plugins</returns>
        public int GetNumPlugins()
        {
            // Set variables
            int pluginCount = 0;

            // Check if system is properly initialized
            if (baseSystem == null)
            {
                return -1;
            }

            // Get the number of DSP plugins
            FMOD.RESULT result = baseSystem.getNumPlugins(FMOD.PLUGINTYPE.OUTPUT, ref pluginCount);

            return pluginCount;
        }

        public void GetPluginHandle(int pluginIndex)
        {
            // Set variables
            uint handle = 0;
            uint version = 0;
            StringBuilder sb = new StringBuilder(500);

            // Check if system is properly initialized
            if (baseSystem == null)
            {
                return;
            }

            // Get the number of DSP plugins
            FMOD.RESULT result = baseSystem.getPluginHandle(FMOD.PLUGINTYPE.DSP, pluginIndex, ref handle);                                
                        //result = system.getDriverInfo(a, sb, 500, ref guid);                        

            FMOD.PLUGINTYPE pluginType = FMOD.PLUGINTYPE.DSP;

            result = baseSystem.getPluginInfo(handle, ref pluginType, sb, 500, ref version);
        }

        public List<string> GetVSTPluginList()
        {
            string vstPluginPath = @"c:\Program Files\Steinberg\VstPlugins\";

            string[] files = Directory.GetFiles(vstPluginPath, "*.dll");

            List<string> names = new List<string>();

            for (int a = 0; a < files.Length; a++)
            {

                try
                {
                    uint handle = 0;
                    string file = files[a];
                    FMOD.RESULT result = baseSystem.loadPlugin(files[a], ref handle, 0);

                    FMOD.DSP dsp = new FMOD.DSP();

                    IntPtr ptr = new IntPtr();
                    //unsafe
                    //{

                    //    fixed (int* p = &dsp)
                    //    {
                    //        *p = 1;
                    //    }

                    //    ptr = new IntPtr(&dsp);
                    //}

                    //FMOD.DSP dspTest = null;

                    result = baseSystem.createDSPByPlugin(handle, ref ptr);

                    if (result == FMOD.RESULT.OK)
                    {
                        dsp.setRaw(ptr);
                    }

                    IntPtr name = new IntPtr();
                    uint version = 0;
                    int channels = 0;
                    int configWidth = 0;
                    int configHeight = 0;

                    result = dsp.getInfo(ref name, ref version, ref channels, ref configWidth, ref configHeight);

                    string strName = Marshal.PtrToStringAnsi(name);

                    names.Add(strName);
                    //Marshal.ptr

                    //char[] array = new char[32];
                    //Marshal.Copy(name, array, 0, 32);



                    //StringBuilder sb = new StringBuilder(500);
                    //result = baseSystem.getPluginInfo(handle, ref pluginType, sb, 500, ref version);
                    //baseSystem.createDSP()
                }
                catch (Exception ex)
                {
                    // Skip file for now
                }

            }

            return names;
        }

        #endregion

        #region DSP

        /// <summary>
        /// Adds a DSP plugin to the sound system.
        /// </summary>
        /// <param name="dsp">DSP Plugin</param>
        public void AddDSP(DSP dsp)
        {
            baseSystem.addDSP(dsp.baseDSP, ref dspConnection);
        }

        /// <summary>
        /// Creates a pitch shift DSP plugin and adds it to the sound system.
        /// </summary>
        /// <param name="pitch">Pitch Shift Value</param>
        /// <returns>Pitch Shift DSP Plugin</returns>
        public PitchShiftDSP CreatePitchShiftDSP(double pitch)
        {
            FMOD.RESULT result;
            FMOD.DSP dsp = null;

            result = baseSystem.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT, ref dsp);
            CheckForError(result);

            PitchShiftDSP newDSP = new PitchShiftDSP(this, dsp, (float)pitch);

            AddDSP(newDSP);

            return newDSP;
        }

        /// <summary>
        /// Creates a parametric equalizer DSP plugin and adds it to the sound system.
        /// </summary>
        /// <returns>Parametric Equalizer DSP Plugin</returns>
        public ParamEQDSP CreateParamEQDSP()
        {
            FMOD.RESULT result;
            FMOD.DSP dsp = null;

            result = baseSystem.createDSPByType(FMOD.DSP_TYPE.PARAMEQ, ref dsp);
            CheckForError(result);

            ParamEQDSP newDSP = new ParamEQDSP(this, dsp);

            AddDSP(newDSP);

            return newDSP;
        }

        #endregion

        /// <summary>
        /// [FMOD documentation] Retrieves a sound's default attributes for when it is played on a channel with System::playSound.  
        /// </summary>
        /// <returns>OutputFormatMixer object</returns>
        public OutputFormatMixer GetSoftwareFormat()
        {
            // Declare variables
            FMOD.RESULT result;
            OutputFormatMixer data = new OutputFormatMixer();

            // Check if base system is null
            if (baseSystem != null)
            {
                result = baseSystem.getSoftwareFormat(ref data.sampleRate, ref data.format, ref data.numOutputChannels, ref data.maxInputChannels, ref data.resampleMethod, ref data.bits);
                System.CheckForError(result);                
            }

            return data;
        }


        #region Error handling

        /// <summary>
        /// Returns the full error description of a FMOD result.
        /// </summary>
        /// <param name="result">FMOD result</param>
        /// <returns>Full error description string</returns>
        public static string GetError(FMOD.RESULT result)
        {
            return FMOD.Error.String(result);
        }

        /// <summary>
        /// Checks the result of a FMOD operation. If an error is found, throw a new exception.
        /// </summary>
        /// <param name="result">FMOD result</param>
        public static void CheckForError(FMOD.RESULT result)
        {
            if (result != FMOD.RESULT.OK)
            {
                //throw new Exception(result.ToString() + " // " + FMOD.Error.String(result));
                HandleException(new Exception(result.ToString() + " // " + FMOD.Error.String(result)));
            }
        }

        /// <summary>
        /// Handles exceptions in the sound system.
        /// </summary>
        /// <param name="ex">Exception</param>
        public static void HandleException(Exception ex)
        {
            // Display message box
            //MessageBox.Show("Exception information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error in sound system!", MessageBoxButtons.OK, MessageBoxIcon.Error);            

            throw ex;
        }

        #endregion

        //public void GetAdvancedSettings()
        //{
        //    FMOD.ADVANCEDSETTINGS settings = new FMOD.ADVANCEDSETTINGS();
            
        //    FMOD.RESULT result = baseSystem.getAdvancedSettings(ref settings);
        //}

    }

    /// <summary>
    /// Class repesenting an output type, with its FMOD value and a description.
    /// </summary>
    public class OutputType
    {
        public FMOD.OUTPUTTYPE FMODOutputType { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Represents the data structure used in the return of the PlayGaplessSequence method.
    /// </summary>
    public class GaplessSequenceData
    {
        public FMOD.Sound[] subSounds;
        public FMOD.Sound sound;
    }

    /// <summary>
    /// Data structure used for the GetDSPBufferSize method.
    /// </summary>
    public class DSPBufferSize
    {
        /// <summary>
        /// [FMOD documentation] Address of a variable that receives the mixer engine block size in samples. 
        /// Default = 1024. (milliseconds = 1024 at 48khz = 1024 / 48000 * 1000 = 10.66ms). 
        /// This means the mixer updates every 21.3ms. 
        /// </summary>
        public uint bufferLength;

        /// <summary>
        /// [FMOD documentation] Address of a variable that receives the mixer engine number of buffers used. 
        /// Default = 4. To get the total buffersize multiply the bufferlength by the numbuffers value. 
        /// By default this would be 4*1024. 
        /// </summary>
        public int numBuffers;
    }

    /// <summary>
    /// Data structure used for the GetSoftwareFormat method.
    /// </summary>
    public class OutputFormatMixer
    {
        /// <summary>
        /// [FMOD documentation] Address of a variable that receives the mixer's output rate.
        /// </summary>
        public int sampleRate;
        /// <summary>
        /// [FMOD documentation] Address of a variable that receives the mixer's output format.
        /// </summary>
        public FMOD.SOUND_FORMAT format;
        /// <summary>
        /// [FMOD documentation] Address of a variable that receives the number of output channels to initialize the mixer to, 
        /// for example 1 = mono, 2 = stereo. 8 is the maximum for soundcards that can handle it. 
        /// </summary>
        public int numOutputChannels;
        /// <summary>
        /// [FMOD documentation] Address of a variable that receives the maximum channel depth on sounds that are loadable or creatable.
        /// </summary>
        public int maxInputChannels;
        /// <summary>
        /// [FMOD documentation] Address of a variable that receives the current resampling (frequency conversion) method for software mixed sounds. 
        /// </summary>
        public FMOD.DSP_RESAMPLER resampleMethod;
        /// <summary>
        /// [FMOD documentation] Address of a variable that receives the number of bits per sample. Useful for byte->sample conversions. 
        /// for example FMOD_SOUND_FORMAT_PCM16 is 16. Optional. Specify 0 or NULL to ignore. 
        /// </summary>
        public int bits;
    }
}
