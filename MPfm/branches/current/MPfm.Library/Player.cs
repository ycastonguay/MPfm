//
// Player.cs: This class is used for playing songs, sound files, gapless sequences
//            and more.
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
using MPfm.Sound.FMODWrapper;

namespace MPfm.Library
{
    /// <summary>
    /// This is the main Player class which manages audio playback and the audio library.
    /// This is the MPfm Playback Engine V2.
    /// </summary>
    public class Player
    {
        #region Properties

        private bool isGapless = false;
        private bool isNewGaplessSequenceLoading = false;
        private bool isLastSongPlaying = false;
        private int currentSentenceIndex = -1;
        private List<string> subSoundsFilePaths = null;
        private GaplessSequenceData gaplessSequenceData = null;
        public uint currentSongLength = 0;

        private PlaybackMode m_playbackMode = PlaybackMode.None;
        public PlaybackMode PlaybackMode
        {
            get
            {
                return m_playbackMode;
            }
        }

        // Delegates
        public delegate void TimerElapsed(TimerElapsedData data);
        public event TimerElapsed OnTimerElapsed;

        public delegate void SongFinished(SongFinishedData data);
        public event SongFinished OnSongFinished;

        // Sound system
        private MPfm.Sound.FMODWrapper.System soundSystem;
        public MPfm.Sound.FMODWrapper.System SoundSystem
        {
            get
            {
                return soundSystem;
            }
        }

        private Channel mainChannel = null;
        public Channel MainChannel
        {
            get
            {
                return mainChannel;
            }
        }

        private PitchShiftDSP m_pitchShift;
        public PitchShiftDSP PitchShift
        {
            get
            {
                return m_pitchShift;
            }
        }

        #region Equalizer properties

        // Equalizer (18 bands)
        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ55Hz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ55Hz
        {
            get
            {
                return m_paramEQ55Hz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ77Hz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ77Hz
        {
            get
            {
                return m_paramEQ77Hz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ110Hz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ110Hz
        {
            get
            {
                return m_paramEQ110Hz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ156Hz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ156Hz
        {
            get
            {
                return m_paramEQ156Hz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ220Hz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ220Hz
        {
            get
            {
                return m_paramEQ220Hz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ311Hz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ311Hz
        {
            get
            {
                return m_paramEQ311Hz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ440Hz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ440Hz
        {
            get
            {
                return m_paramEQ440Hz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ622Hz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ622Hz
        {
            get
            {
                return m_paramEQ622Hz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ880Hz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ880Hz
        {
            get
            {
                return m_paramEQ880Hz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ1_2kHz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ1_2kHz
        {
            get
            {
                return m_paramEQ1_2kHz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ1_8kHz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ1_8kHz
        {
            get
            {
                return m_paramEQ1_8kHz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ2_5kHz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ2_5kHz
        {
            get
            {
                return m_paramEQ2_5kHz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ3_5kHz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ3_5kHz
        {
            get
            {
                return m_paramEQ3_5kHz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ5kHz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ5kHz
        {
            get
            {
                return m_paramEQ5kHz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ7kHz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ7kHz
        {
            get
            {
                return m_paramEQ7kHz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ10kHz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ10kHz
        {
            get
            {
                return m_paramEQ10kHz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ14kHz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ14kHz
        {
            get
            {
                return m_paramEQ14kHz;
            }
        }

        private MPfm.Sound.FMODWrapper.ParamEQDSP m_paramEQ20kHz;
        public MPfm.Sound.FMODWrapper.ParamEQDSP ParamEQ20kHz
        {
            get
            {
                return m_paramEQ20kHz;
            }
        }

        private bool m_IsEQOn = true;
        public bool IsEQOn
        {
            get
            {
                return m_IsEQOn;
            }
            set
            {
                // Is there a value change?
                if (m_IsEQOn != value)
                {
                    // Set value
                    m_IsEQOn = value;

                    // By pass
                    ParamEQ55Hz.Bypassed = !value;
                    ParamEQ77Hz.Bypassed = !value;
                    ParamEQ110Hz.Bypassed = !value;
                    ParamEQ156Hz.Bypassed = !value;
                    ParamEQ220Hz.Bypassed = !value;
                    ParamEQ311Hz.Bypassed = !value;
                    ParamEQ440Hz.Bypassed = !value;
                    ParamEQ622Hz.Bypassed = !value;
                    ParamEQ880Hz.Bypassed = !value;
                    ParamEQ1_2kHz.Bypassed = !value;
                    ParamEQ1_8kHz.Bypassed = !value;
                    ParamEQ2_5kHz.Bypassed = !value;
                    ParamEQ3_5kHz.Bypassed = !value;
                    ParamEQ5kHz.Bypassed = !value;
                    ParamEQ7kHz.Bypassed = !value;
                    ParamEQ10kHz.Bypassed = !value;
                    ParamEQ14kHz.Bypassed = !value;
                    ParamEQ20kHz.Bypassed = !value;
                }
            }
        }

        #endregion

        private MPfm.Sound.FMODWrapper.Sound m_currentSound;
        /// <summary>
        /// The current sound structure.
        /// </summary>
        public MPfm.Sound.FMODWrapper.Sound CurrentSound
        {
            get
            {
                return m_currentSound;
            }
        }

        // Markers for current loop
        private MPfm.Library.Data.Marker m_currentLoopMarkerA = null;
        private MPfm.Library.Data.Marker m_currentLoopMarkerB = null;

        /// <summary>
        /// Private value for the CurrentLoop property.
        /// </summary>
        private MPfm.Library.Data.Loop m_currentLoop = null;
        /// <summary>
        /// Current loop. Must be related to the currently playing song.
        /// </summary>
        public MPfm.Library.Data.Loop CurrentLoop
        {
            get
            {
                return m_currentLoop;
            }
            set
            {
                m_currentLoop = value;

                // Fetch markers if necessary
                if (m_currentLoop != null)
                {
                    m_currentLoopMarkerA = DataAccess.SelectMarker(new Guid(m_currentLoop.MarkerAId));
                    m_currentLoopMarkerB = DataAccess.SelectMarker(new Guid(m_currentLoop.MarkerBId));
                }
            }
        }

        /// <summary>
        /// The current song playing (SongDTO). Comes from the CurrentSong
        /// property of the CurrentPlaylist.
        /// </summary>
        public SongDTO CurrentSong
        {
            get
            {
                // Check for null
                if (CurrentPlaylist != null && CurrentPlaylist.CurrentSong != null)
                {
                    // Return the current song in the current playlist
                    return CurrentPlaylist.CurrentSong.Song;
                }

                return null;
            }
        }

        private Library m_library = null;
        /// <summary>
        /// Library associated with the player.
        /// </summary>
        public Library Library
        {
            get
            {
                return m_library;
            }

        }

        private PlaylistDTO m_currentPlaylist = null;
        /// <summary>
        /// The current playlist.
        /// </summary>
        public PlaylistDTO CurrentPlaylist
        {
            get
            {
                return m_currentPlaylist;
            }
        }

        /// <summary>
        /// Timer for the player.
        /// </summary>
        private System.Timers.Timer timerPlayer = null;

        private bool IsPlaybackCancelled { get; set; }

        /// <summary>
        /// Volume of the main channel.
        /// </summary>
        private int m_Volume;
        public int Volume
        {
            get
            {
                return m_Volume;
            }
            set
            {
                m_Volume = value;

                if (MainChannel.IsInitialized && MainChannel.IsPlaying)
                {
                    MainChannel.Volume = (float)Volume / 100;
                }
            }
        }

        private int m_timeShifting = 100;
        /// <summary>
        /// Time shifting percentage.
        /// </summary>
        public int TimeShifting
        {
            get
            {
                return m_timeShifting;
            }
            set
            {
                m_timeShifting = value;

                try
                {
                    if (MainChannel.IsInitialized && MainChannel.IsPlaying)
                    {
                        double multiplier = 1 / ((double)value / 100);

                        if (value == 100)
                        {
                            PitchShift.Bypassed = true;
                        }
                        else
                        {
                            PitchShift.Bypassed = false;
                        }

                        MainChannel.Frequency = (float)CurrentSound.Frequency / (float)multiplier;
                        PitchShift.SetPitch((float)multiplier);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Repeat type (Off, Playlist, Song)
        /// </summary>
        public RepeatType RepeatType
        {
            get;
            set;
        }

        public bool IsPaused
        {
            get
            {
                if (MainChannel.IsInitialized && MainChannel.IsPlaying)
                {
                    return MainChannel.IsPaused;
                }

                return false;
            }

        }

        public bool IsPlaying
        {
            get
            {
                if (MainChannel.IsInitialized)
                {
                    return MainChannel.IsPlaying;
                }

                return false;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return MainChannel.IsInitialized;
            }
        }

        #endregion

        /// <summary>
        /// Play class constructor. Initializes library and sound system.
        /// </summary>
        public Player(FMOD.OUTPUTTYPE driver, string outputDeviceName, bool initializeLibrary)
        {
            try
            {
                // Initialize library
                if (initializeLibrary)
                {
                    Tracing.Log("Player -- Initializing library...");
                    m_library = new Library();
                }

                // Initialize sound system
                Tracing.Log("Player -- Initializing sound system...");
                soundSystem = new MPfm.Sound.FMODWrapper.System(driver, outputDeviceName);

                // Create main channel
                Tracing.Log("Player -- Creating main channel...");
                mainChannel = new Channel(soundSystem);
                mainChannel.SoundEnd += new Channel.SoundEndHandler(soundSystem_OnSoundEnd);

                // Create pitch shift (bypassed by default)
                Tracing.Log("Player -- Creating pitch shift DSP...");
                m_pitchShift = soundSystem.CreatePitchShiftDSP(1);
                m_pitchShift.SetFFTSize(4096);
                m_pitchShift.Bypassed = true;

                // Create equalizer bands
                Tracing.Log("Player -- Creating equalizer...");
                #region EQ
                // 55 Hz
                m_paramEQ55Hz = soundSystem.CreateParamEQDSP();
                m_paramEQ55Hz.SetCenter(55);
                m_paramEQ55Hz.SetBandwidth(1);
                m_paramEQ55Hz.SetGain(1);

                // 77 Hz
                m_paramEQ77Hz = soundSystem.CreateParamEQDSP();
                m_paramEQ77Hz.SetCenter(77);
                m_paramEQ77Hz.SetBandwidth(1);
                m_paramEQ77Hz.SetGain(1);

                // 110 Hz
                m_paramEQ110Hz = soundSystem.CreateParamEQDSP();
                m_paramEQ110Hz.SetCenter(110);
                m_paramEQ110Hz.SetBandwidth(1);
                m_paramEQ110Hz.SetGain(1);

                // 156 Hz
                m_paramEQ156Hz = soundSystem.CreateParamEQDSP();
                m_paramEQ156Hz.SetCenter(156);
                m_paramEQ156Hz.SetBandwidth(1);
                m_paramEQ156Hz.SetGain(1);

                // 220 Hz
                m_paramEQ220Hz = soundSystem.CreateParamEQDSP();
                m_paramEQ220Hz.SetCenter(220);
                m_paramEQ220Hz.SetBandwidth(1);
                m_paramEQ220Hz.SetGain(1);

                // 311 Hz
                m_paramEQ311Hz = soundSystem.CreateParamEQDSP();
                m_paramEQ311Hz.SetCenter(311);
                m_paramEQ311Hz.SetBandwidth(1);
                m_paramEQ311Hz.SetGain(1);

                // 440 Hz
                m_paramEQ440Hz = soundSystem.CreateParamEQDSP();
                m_paramEQ440Hz.SetCenter(440);
                m_paramEQ440Hz.SetBandwidth(1);
                m_paramEQ440Hz.SetGain(1);

                // 622 Hz
                m_paramEQ622Hz = soundSystem.CreateParamEQDSP();
                m_paramEQ622Hz.SetCenter(622);
                m_paramEQ622Hz.SetBandwidth(1);
                m_paramEQ622Hz.SetGain(1);

                // 880 Hz
                m_paramEQ880Hz = soundSystem.CreateParamEQDSP();
                m_paramEQ880Hz.SetCenter(880);
                m_paramEQ880Hz.SetBandwidth(1);
                m_paramEQ880Hz.SetGain(1);

                // 1.2 kHz
                m_paramEQ1_2kHz = soundSystem.CreateParamEQDSP();
                m_paramEQ1_2kHz.SetCenter(1200);
                m_paramEQ1_2kHz.SetBandwidth(1);
                m_paramEQ1_2kHz.SetGain(1);

                // 1.8 kHz
                m_paramEQ1_8kHz = soundSystem.CreateParamEQDSP();
                m_paramEQ1_8kHz.SetCenter(1800);
                m_paramEQ1_8kHz.SetBandwidth(1);
                m_paramEQ1_8kHz.SetGain(1);

                // 2.5 kHz
                m_paramEQ2_5kHz = soundSystem.CreateParamEQDSP();
                m_paramEQ2_5kHz.SetCenter(2500);
                m_paramEQ2_5kHz.SetBandwidth(1);
                m_paramEQ2_5kHz.SetGain(1);

                // 3.5 kHz
                m_paramEQ3_5kHz = soundSystem.CreateParamEQDSP();
                m_paramEQ3_5kHz.SetCenter(3500);
                m_paramEQ3_5kHz.SetBandwidth(1);
                m_paramEQ3_5kHz.SetGain(1);

                // 5 kHz
                m_paramEQ5kHz = soundSystem.CreateParamEQDSP();
                m_paramEQ5kHz.SetCenter(5000);
                m_paramEQ5kHz.SetBandwidth(1);
                m_paramEQ5kHz.SetGain(1);

                // 7 kHz
                m_paramEQ7kHz = soundSystem.CreateParamEQDSP();
                m_paramEQ7kHz.SetCenter(7000);
                m_paramEQ7kHz.SetBandwidth(1);
                m_paramEQ7kHz.SetGain(1);

                // 10 kHz
                m_paramEQ10kHz = soundSystem.CreateParamEQDSP();
                m_paramEQ10kHz.SetCenter(10000);
                m_paramEQ10kHz.SetBandwidth(1);
                m_paramEQ10kHz.SetGain(1);

                // 14 kHz
                m_paramEQ14kHz = soundSystem.CreateParamEQDSP();
                m_paramEQ14kHz.SetCenter(14000);
                m_paramEQ14kHz.SetBandwidth(1);
                m_paramEQ14kHz.SetGain(1);

                // 20 kHz
                m_paramEQ20kHz = soundSystem.CreateParamEQDSP();
                m_paramEQ20kHz.SetCenter(20000);
                m_paramEQ20kHz.SetBandwidth(1);
                m_paramEQ20kHz.SetGain(1);
                #endregion

                // Set default properties
                Volume = 100;
                TimeShifting = 100;
                RepeatType = RepeatType.Off;
                IsPlaybackCancelled = false;

                // Create and start timer
                timerPlayer = new System.Timers.Timer();
                timerPlayer.Interval = 10;
                timerPlayer.Elapsed += new System.Timers.ElapsedEventHandler(timerPlayer_Elapsed);
                //timerPlayer.SynchronizingObject = this;
                timerPlayer.Enabled = true;

                // Create default empty playlist
                m_currentPlaylist = new PlaylistDTO();
                m_currentPlaylist.PlaylistName = "Empty playlist";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Closes the player and releases the sound system from the memory.
        /// </summary>
        public void Close()
        {
            // Check if the sound system is cleared
            if (soundSystem != null)
            {
                // Close system
                soundSystem.Close();

                // Release system from memory
                soundSystem.Release();
            }
        }

        /// <summary>
        /// Returns the list of supported drivers on the system.
        /// </summary>
        /// <returns>List of drivers</returns>
        public List<string> GetDrivers()
        {
            Driver[] drivers = soundSystem.GetDrivers();

            List<string> driverNames = new List<string>();
            foreach (Driver driver in drivers)
            {
                driverNames.Add(driver.Name);
            }

            return driverNames;
        }

        #region Playback methods

        /// <summary>
        /// Sets the playback position, in percentage.
        /// </summary>
        /// <param name="percentage">Percantage position</param>
        /// <returns>Position in milliseconds</returns>
        public uint SetPosition(double percentage)
        {
            // Set position
            uint newPosition = Convert.ToUInt32(percentage * (double)currentSongLength);
            MainChannel.SetPosition(newPosition);
            return newPosition;
        }

        public uint SetPositionSentenceMS(double percentage)
        {
            // Set position
            uint newPosition = Convert.ToUInt32(percentage * (double)currentSongLength);
            MainChannel.SetPosition(newPosition, FMOD.TIMEUNIT.SENTENCE_MS);
            return newPosition;
        }

        /// <summary>
        /// Generates a playlist containing all the songs of the library, and starts playback from the first song in the playlist.
        /// </summary>
        /// <param name="soundFormat">Filter sound format</param>
        public void PlayAll(FilterSoundFormat soundFormat)
        {
            // Generate playlist and start from the first song
            Play(Library.GeneratePlaylistAll(soundFormat), Guid.Empty);
        }

        /// <summary>
        /// Generates a playlist containing all the songs of the library, and starts playback at the song specified in the
        /// startSongId parameter.
        /// </summary>
        /// <param name="soundFormat">Filter sound format</param>
        /// <param name="startSongId">Id of the first song to be played</param>
        public void PlayAll(FilterSoundFormat soundFormat, Guid startSongId)
        {
            // Generate playlist with all songs and start from a specific song.            
            Play(Library.GeneratePlaylistAll(soundFormat), startSongId);
        }

        /// <summary>
        /// Generates a playlist containing all songs from an artist, and starts playback from the first song in the playlist.
        /// </summary>
        /// <param name="soundFormat">Filter sound format</param>
        /// <param name="artistName">Artist name</param>
        public void PlayArtist(FilterSoundFormat soundFormat, string artistName)
        {
            PlayArtist(soundFormat, artistName, Guid.Empty);
        }

        /// <summary>
        /// Generates a playlist containing all songs from an artist, and starts playback at the song specified in the
        /// startSongId parameter.
        /// </summary>
        /// <param name="soundFormat">Filter sound format</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="startSongId">Id of the first song to be played</param>
        public void PlayArtist(FilterSoundFormat soundFormat, string artistName, Guid startSongId)
        {
            // Generate playlist and start from first song
            PlaylistDTO playlist = Library.GeneratePlaylistFromArtist(soundFormat, artistName);
            Play(playlist, startSongId);
        }

        /// <summary>
        /// Generates a playlist containing all songs from an artist's album, and starts playback from the first song in the playlist.
        /// </summary>
        /// <param name="soundFormat">Filter sound format</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="albumTitle">Album title</param>
        public void PlayAlbum(FilterSoundFormat soundFormat, string artistName, string albumTitle)
        {
            // Generate playlist and start from the first song
            PlayAlbum(soundFormat, artistName, albumTitle, Guid.Empty);
        }

        /// <summary>
        /// Generates a playlist containing all songs from an artist's album, and starts playback at the song specified in the
        /// startSongId parameter.
        /// </summary>
        /// <param name="soundFormat">Filter sound format</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="albumTitle">Album title</param>
        /// <param name="startSongId">Id of the first song to be played</param>
        public void PlayAlbum(FilterSoundFormat soundFormat, string artistName, string albumTitle, Guid startSongId)
        {
            // Generate playlist and start from a specific song
            Play(Library.GeneratePlaylistFromAlbum(soundFormat, artistName, albumTitle), startSongId);
        }

        /// <summary>
        /// Selects a playlist from the database and starts playback from the first song in the playlist.
        /// </summary>
        /// <param name="playlistId">Playlist Identifier</param>        
        public void PlayPlaylist(Guid playlistId)
        {
            PlayPlaylist(playlistId, Guid.Empty);
        }

        /// <summary>
        /// Selects a playlist from the database and starts playback at the song specified in the startSongId parameter.
        /// </summary>
        /// <param name="playlistId">Playlist Identifier</param>
        /// <param name="startSongId">Start Song Identifier (SongId or PlaylistSongId)</param>
        public void PlayPlaylist(Guid playlistId, Guid startSongId)
        {
            // Get playlist from database
            PlaylistDTO playlist = Library.SelectPlaylist(playlistId);

            // If there is no start song, start from the first song
            if (startSongId == Guid.Empty && playlist.Songs.Count > 0)
            {
                playlist.CurrentSong = playlist.Songs[0];
            }
            else
            {
                playlist.CurrentSong = playlist.Songs.FirstOrDefault(x => x.PlaylistSongId == startSongId);
            }

            // Play playlist
            Play(playlist, startSongId, false, 0);
        }

        /// <summary>
        /// Plays a specific playlist and starts playback from the first song of the playlist.
        /// </summary>
        /// <param name="playlist">Playlist to be played</param>
        protected void Play(PlaylistDTO playlist)
        {
            Play(playlist, Guid.Empty, false, 0);
        }

        /// <summary>
        /// Plays a specific playlist and starts playback at the song specified in the
        /// startSongId parameter.
        /// </summary>
        /// <param name="playlist">Playlist to be played</param>
        /// <param name="startSongId">Id of the first song to be played  (SongId or PlaylistSongId)</param>
        protected void Play(PlaylistDTO playlist, Guid startSongId)
        {
            Play(playlist, startSongId, false, 0);
        }

        /// <summary>
        /// Plays a specific playlist and starts playback at the song specified in the
        /// startSongId parameter. Can be paused or seeked to a position before starting playback.
        /// </summary>
        /// <param name="playlist">Playlist to be played</param>
        /// <param name="startSongId">Id of the first song to be played (SongId or PlaylistSongId)</param>
        /// <param name="paused">True if the playback is to be started paused</param>
        /// <param name="position">Position to set before playback</param> 
        protected void Play(PlaylistDTO playlist, Guid startSongId, bool paused, int position)
        {
            // Check for nulls
            if (playlist == null)
            {
                return;
            }

            // Reset loop
            CurrentLoop = null;

            // Set flags
            isGapless = false;
            m_playbackMode = MPfm.Library.PlaybackMode.Playlist;

            // Set current playlist
            m_currentPlaylist = playlist;

            // Check if the start song is the default song or passed in parameter
            if (startSongId != Guid.Empty)
            {
                // Get song from playlist
                PlaylistSongDTO playlistSong = playlist.Songs.SingleOrDefault(x => x.Song.SongId == startSongId);

                // Is the song valid?
                if (playlistSong != null)
                {
                    // Set current song
                    m_currentPlaylist.CurrentSong = playlistSong;
                }
                else if (playlistSong == null)
                {
                    // Get song from playlist
                    playlistSong = playlist.Songs.SingleOrDefault(x => x.PlaylistSongId == startSongId);

                    // Is the song valid?
                    if (playlistSong != null)
                    {
                        // Set current song
                        m_currentPlaylist.CurrentSong = playlistSong;
                    }
                }
            }

            // Making a list of two sounds in a sound list sentence with different sound lengths doesn't work when trying to get current MS position.
            // (see realtimestiching example from FMOD, and replace the ogg files with sound files with different lengths.)
            // Thus we must analyze the playlist and group songs together for a gapless sequence.

            // First, group albums together

            // Find the first song to play
            //for (int a = 0; a < playlist.Songs.Count; a++)
            //{
            //    // Check if the playlist song id match
            //    if (playlist.Songs[a].PlaylistSongId == playlist.CurrentSong.PlaylistSongId)
            //    {
            //        // This is the first song!
            //        firstSongIndex = a;

            //        // Check how many songs match the same album title in the batch
            //        for (int b = a; b < playlist.Songs.Count; b++)
            //        {
            //            // Does the album title match?
            //            if (playlist.Songs[b].Song.AlbumTitle != playlist.Songs[a].Song.AlbumTitle)
            //            {
            //                // No match; this means the "streak" stops here!
            //                lastSongIndex = b - 1;
            //                break;
            //            }
            //        }

            //        // If the next album title hasn't been found yet, it's because there is no other album after this one.
            //        if (lastSongIndex == -1)
            //        {
            //            // Set the last song as the last song of the current album
            //            lastSongIndex = playlist.Songs.Count - 1;
            //        }                    

            //        // We have the first and last song index, get out of the loop!
            //        break;
            //    }
            //}            


            int firstSongIndex = -1;
            int lastSongIndex = -1;

            for (int a = 0; a < playlist.Songs.Count; a++)
            {
                if (playlist.Songs[a].Song.SongId == startSongId)
                {
                    firstSongIndex = a;
                    break;
                }
                else if (playlist.Songs[a].PlaylistSongId == startSongId)
                {
                    firstSongIndex = a;
                    break;
                }
            }

            playlist.GetRemainingSongsFromAlbum(ref firstSongIndex, ref lastSongIndex);
            //List<string> fPaths = playlist.GetRemainingSongFilePathsFromAlbum();

            // Build file path list
            subSoundsFilePaths = new List<string>();
            for (int a = firstSongIndex; a <= lastSongIndex; a++)
            {
                // Add file path
                subSoundsFilePaths.Add(playlist.Songs[a].Song.FilePath);
            }

            m_currentPlaylist.Position = firstSongIndex + 1;

            // Stop playback if necessary
            if (MainChannel.IsPlaying)
            {
                MainChannel.Stop();

                // RELEASE SOUNDS
                if (gaplessSequenceData != null)
                {
                    if (gaplessSequenceData.sound != null)
                    {
                        gaplessSequenceData.sound.release();
                    }
                    //foreach (FMOD.Sound sound in gaplessSequenceData.subSounds)
                    //{
                    //    if (sound != null)
                    //    {
                    //        sound.release();
                    //    }
                    //}
                }

            }

            // Play song
            //PlaySong(CurrentSong, paused, position);

            // Play gapless sequence            
            GaplessSequenceData data = PlayGaplessSequence(subSoundsFilePaths);
        }

        ///// <summary>
        ///// Plays a specific song in the song library. Can be paused or seeked to a position before starting playback.
        ///// </summary>
        ///// <param name="song">Song to play</param>
        ///// <param name="paused">True if the playback is to be started paused</param>
        ///// <param name="position">Position to set before playback</param>
        //protected void PlaySong(SongDTO song, bool paused, int position)
        //{
        //    // If the sound system is not initialized, return nothing
        //    if (soundSystem == null)
        //    {
        //        Tracing.Log("Player -- PlaySong error: Sound system is not initialized!");
        //        return;
        //    }

        //    // Check if the sound file exists
        //    if (!File.Exists(CurrentSong.FilePath))
        //    {
        //        Tracing.Log("Player -- PlaySong error: The file at " + CurrentSong.FilePath + " could not be found!");
        //        throw new Exception("The file at " + CurrentSong.FilePath + " could not be found!");
        //    }

        //    // Check if a current sound exists            
        //    if (CurrentSound != null)
        //    {
        //        // Is it the same sound file path?                
        //        if (IsPlaying)
        //        {
        //            // It is a different file; release current sound
        //            IsPlaybackCancelled = true;
        //            CurrentSound.Release();                    
        //        }
        //    }

        //    // Create sound
        //    m_currentSound = soundSystem.CreateSound(CurrentSong.FilePath, true);

        //    // Play sound file
        //    soundSystem.PlaySound(m_currentSound, true);

        //    //// Update the length of the audio file if different than in the database
        //    //if (CurrentSong.Time != m_currentSound.Length)
        //    //{
        //    //    // Update time of current song
        //    //    CurrentSong.Time = CurrentSound.Length;

        //    //    //db.UpdateSong(currentSong);
        //    //}

        //    // Volume
        //    soundSystem.MainChannel.Volume = (float)Volume / 100;
        //    soundSystem.MainChannel.PositionAbsoluteMilliseconds = (uint)position;

        //    // Check if the channel must be set to pause
        //    if (!paused)
        //    {
        //        // Pause channel
        //        soundSystem.MainChannel.Pause();
        //    }
        //}

        /// <summary>
        /// Creates a gapless sequence to be played.
        /// </summary>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        public GaplessSequenceData PlayGaplessSequence(List<string> filePaths)
        {
            // If the sound system is not initialized, return nothing
            if (soundSystem == null || filePaths == null)
            {
                Tracing.Log("Player -- PlaySong error: Sound system is not initialized or file list is null!");
                return null;
            }

            //// Make a list of the two first file paths
            //subSoundsFilePaths = new List<string>();
            //if (playlist.Songs.Count > 1)
            //{
            //    // Limit to 2 elements
            //    for (int a = 0; a < 2; a++)
            //    {
            //        subSoundsFilePaths.Add(playlist.Songs[a].Song.FilePath);
            //    }
            //}
            //else
            //{
            //    // Only one element... (need gapless?!)
            //    subSoundsFilePaths.Add(playlist.Songs[0].Song.FilePath);
            //}

            // Reset flags            
            currentSentenceIndex = 0;
            isGapless = true;
            isLastSongPlaying = false;
            //isNewGaplessSequenceLoading = false;

            // Check if file paths exist
            foreach (string filePath in filePaths)
            {
                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    // Throw exception
                    throw new Exception("The file at " + filePath + " doesn't exist!");
                }
            }

            // Detect the file information for the stream 
            MPfm.Sound.FMODWrapper.Sound tempSound = soundSystem.CreateSound(filePaths[0], false);
            SoundFormat soundFormat = tempSound.GetSoundFormat();
            tempSound.Release();

            // Play gapless sequence            
            gaplessSequenceData = soundSystem.PlayGaplessSequence(filePaths, soundFormat, MainChannel);

            // Set current sound as the first file
            //FMOD.Sound newSound = new FMOD.Sound();
            m_currentSound = new MPfm.Sound.FMODWrapper.Sound(gaplessSequenceData.subSounds[0], filePaths[0]);
            currentSongLength = m_currentSound.LengthAbsoluteMilliseconds;

            // Volume
            MainChannel.Volume = (float)Volume / 100;

            return gaplessSequenceData;
        }

        /// <summary>
        /// Plays the audio file specified in the filePath parameter. Does not update the audio library since
        /// the song is considered to be outside the library. 
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        public void PlayFile(string filePath)
        {
            // If the sound system is not initialized, return nothing
            if (soundSystem == null)
            {
                Tracing.Log("Player -- PlayFile error: Sound system is not initialized!");
                return;
            }

            // Check if the sound file exists
            if (!File.Exists(filePath))
            {
                Tracing.Log("Player -- PlayFile error: The file at " + CurrentSong.FilePath + " could not be found!");
                throw new Exception("The file at " + CurrentSong.FilePath + " could not be found!");
            }

            // Stop playback
            //Stop();

            // Set flags
            m_playbackMode = MPfm.Library.PlaybackMode.OpenFile;

            // Make a list of one file path
            subSoundsFilePaths = new List<string>() { filePath };

            // Play gapless "sequence"
            PlayGaplessSequence(subSoundsFilePaths);

            //// Check if a current sound exists            
            //if (CurrentSound != null)
            //{
            //    // Is it the same sound file path?                
            //    if (IsPlaying)
            //    {
            //        // It is a different file; release current sound
            //        IsPlaybackCancelled = true;
            //        CurrentSound.Release();
            //    }
            //}

            //// Create sound
            ////m_currentSound = soundSystem.CreateSound(filePath, true);

            //// Play sound file
            ////soundSystem.PlaySound(m_currentSound, false);
        }

        /// <summary>
        /// Pauses the playback. If the playback is running, the playback is paused. If the playback is paused, the playback starts again.
        /// </summary>
        public void Pause()
        {
            if (MainChannel.IsPlaying)
            {
                MainChannel.Pause();
            }
        }

        /// <summary>
        /// Stops the playback.
        /// </summary>
        public void Stop()
        {
            // Check if channel is playing
            if (MainChannel.IsPlaying)
            {
                // Raise the flag that we cancelled playback (for the soundend event)
                IsPlaybackCancelled = true;

                // Set playback mode
                m_playbackMode = MPfm.Library.PlaybackMode.None;

                // Stop channel
                MainChannel.Stop();

                // Check if the current sound is not null
                if (m_currentSound != null)
                {
                    // Release and dispose sound
                    m_currentSound.Release();
                    m_currentSound = null;
                }

                // Set current song to null                
                CurrentPlaylist.CurrentSong = null;

                // Reset current loop
                CurrentLoop = null;
            }
        }

        /// <summary>
        /// Skips the current song and go to the next song in the playlist (if the playlist exists).
        /// </summary>
        public void GoToNextSong()
        {
            // Check for nulls
            if (CurrentPlaylist == null || CurrentSong == null)
            {
                // Exit method
                return;
            }

            // Get the next song in the playlist
            PlaylistSongDTO nextSong = CurrentPlaylist.GetNextSong(CurrentPlaylist.CurrentSong.PlaylistSongId);

            // Check if the next song is null
            if (nextSong == null)
            {
                // Check if the repeat type is set to Playlist 
                if (RepeatType == MPfm.Library.RepeatType.Playlist)
                {
                    // Repeat playlist: return to the first song of the playlist
                    nextSong = CurrentPlaylist.Songs[0];
                }
            }

            // Did we find the next song to play?
            if (nextSong != null)
            {
                // Set the current song
                CurrentPlaylist.CurrentSong = nextSong;

                // Play song
                //PlaySong(CurrentSong, false, 0);
                Play(CurrentPlaylist, nextSong.Song.SongId);
            }
            else
            {
                // There is no next song; stop playback.
                Stop();
            }
        }

        /// <summary>
        /// Skips the current song and go to the previous song in the playlist (if the playlist exists).
        /// </summary>
        public void GoToPreviousSong()
        {
            // Check for nulls
            if (CurrentPlaylist == null || CurrentSong == null)
            {
                // Exit method
                return;
            }

            // Get the next song in the playlist
            PlaylistSongDTO prevSong = CurrentPlaylist.GetPreviousSong(CurrentPlaylist.CurrentSong.PlaylistSongId);

            // Check if the next song is null
            if (prevSong == null)
            {
                // Check if the repeat type is set to Playlist 
                if (RepeatType == MPfm.Library.RepeatType.Playlist)
                {
                    // Repeat playlist: return to the last song of the playlist
                    prevSong = CurrentPlaylist.Songs[CurrentPlaylist.Songs.Count - 1];
                }
            }

            // Did we find the next song to play?
            if (prevSong != null)
            {
                // Set the current song
                CurrentPlaylist.CurrentSong = prevSong;

                // Play song
                //PlaySong(CurrentSong, false, 0);
                Play(CurrentPlaylist, prevSong.Song.SongId);
            }
            else
            {
                // There is no next song; stop playback.
                Stop();
            }
        }

        /// <summary>
        /// Skips to a specific song in the playlist.
        /// </summary>
        /// <param name="playlistSongId">PlaylistSong Identifier</param>
        public void SkipToSong(Guid playlistSongId)
        {
            // Check for nulls
            if (CurrentPlaylist == null)
            {
                // Exit method
                return;
            }

            // Get playlist song from playlist
            PlaylistSongDTO playlistSong = CurrentPlaylist.Songs.FirstOrDefault(x => x.PlaylistSongId == playlistSongId);

            // Check if the playlist song is null
            if (playlistSong == null)
            {
                return;
            }

            // The song is valid; skip to this song            
            CurrentPlaylist.CurrentSong = playlistSong;

            // Play song
            //PlaySong(CurrentSong, false, 0);
            Play(CurrentPlaylist, playlistSong.PlaylistSongId);
        }

        /// <summary>
        /// Adds a song to the current playlist. By default, the song is added
        /// at the end of the playlist.
        /// </summary>
        /// <param name="songId">Song Identifier</param>
        public void AddSongToPlaylist(Guid songId)
        {
            // Check for null
            if (CurrentPlaylist == null)
            {
                // Exit method
                return;
            }

            AddSongToPlaylist(songId, CurrentPlaylist.Songs.Count);
        }

        /// <summary>
        /// Adds a song to the current playlist, to the specified position passed
        /// in parameter.
        /// </summary>
        /// <param name="songId">Song Identifier</param>
        /// <param name="position">Song Position in Playlist</param>
        public void AddSongToPlaylist(Guid songId, int position)
        {
            // Check for null
            if (CurrentPlaylist == null)
            {
                // Exit method
                return;
            }

            // Get song from database            
            SongDTO song = Library.Songs.FirstOrDefault(x => x.SongId == songId);

            // Check if the song is null
            if (song == null)
            {
                return;
            }

            // Create playlist song
            PlaylistSongDTO playlistSong = new PlaylistSongDTO();
            playlistSong.Song = song;

            // Add playlist song to playlist
            CurrentPlaylist.Songs.Insert(position, playlistSong);
        }

        #endregion

        #region VST plugins

        public void GetVSTPluginList()
        {

        }

        #endregion

        #region Events

        /// <summary>
        /// Fires when an audio block has been delivered to the sound card. This should be every few 
        /// milliseconds. This event is useful to analyse the audio spectrum, to update the Player UI
        /// and more.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void timerPlayer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // Check for nulls
                if (soundSystem == null)
                {
                    return;
                }

                //// Check if channel is playing
                //if (!IsPlaying)
                //{
                //    // Nothing is playing; just return
                //    return;
                //}

                // if true, and pos == 0 then its time to load a new list of file paths...

                // Check if the player is in gapless mode
                if (isGapless)
                {
                    // Gapless mode.
                    uint pos_sentence = 0;
                    uint pos_sentence_ms = 0;

                    try
                    {
                        // If the current channel isn't playing, this will crash. However, when switching gapless sequences,
                        // the isPlaying FMOD method returns false when the channel is actually playing.
                        // Thus a try/catch is used in case the channel isn't playing.
                        MainChannel.baseChannel.getPosition(ref pos_sentence, FMOD.TIMEUNIT.SENTENCE);
                        MainChannel.baseChannel.getPosition(ref pos_sentence_ms, FMOD.TIMEUNIT.SENTENCE_MS);
                    }
                    catch (Exception ex)
                    {
                        // The channel is not playing; just return
                        return;
                    }

                    // Check if the isLastLongPlaying flag has been modified; the last song of the sentence is playing
                    if (!isLastSongPlaying && pos_sentence_ms > 0 && pos_sentence == subSoundsFilePaths.Count - 1)
                    {
                        // Set flag                       
                        isLastSongPlaying = true;
                    }
                    else if (!isNewGaplessSequenceLoading && isLastSongPlaying &&
                             pos_sentence == subSoundsFilePaths.Count &&
                             pos_sentence_ms == 0)
                    {
                        // Reset flag
                        isNewGaplessSequenceLoading = true;
                        isLastSongPlaying = false;

                        // Time to load a new sentence                        
                        if (CurrentPlaylist.Position == CurrentPlaylist.Songs.Count + 1)
                        {
                            // This is the end of the gapless sentence. There are no more songs in the playlist.

                            // Check the repeat type
                            if (RepeatType == MPfm.Library.RepeatType.Off)
                            {
                                // Do nothing: this is the end of the playback.
                            }
                            else if (RepeatType == MPfm.Library.RepeatType.Playlist)
                            {
                                // Go back to the first song of the playlist
                                CurrentPlaylist.Position = 1;
                            }
                            else if (RepeatType == MPfm.Library.RepeatType.Song)
                            {
                                // Is this necessary?
                            }
                        }
                        else
                        {
                            // This is the end of the gapless sentence, but there is more content in this playlist!                            
                            Guid newPlaylistSongId = CurrentPlaylist.Songs[CurrentPlaylist.Position].PlaylistSongId;

                            // Find the next gapless sequence                            
                            int firstSongIndex = CurrentPlaylist.Position;
                            int lastSongIndex = -1;
                            CurrentPlaylist.GetRemainingSongsFromAlbum(ref firstSongIndex, ref lastSongIndex);

                            subSoundsFilePaths = new List<string>();
                            if (firstSongIndex >= 0 && lastSongIndex >= 0)
                            {
                                for (int a = firstSongIndex; a <= lastSongIndex; a++)
                                {
                                    subSoundsFilePaths.Add(CurrentPlaylist.Songs[a].Song.FilePath);
                                }
                            }

                            // Increment playlist position                        
                            CurrentPlaylist.Position++;

                            // Raise song end event (if an event is subscribed)
                            if (OnSongFinished != null)
                            {
                                // Create data
                                SongFinishedData data = new SongFinishedData();
                                data.CurrentSong = CurrentPlaylist.CurrentSong;
                                data.NextSong = CurrentPlaylist.Songs[CurrentPlaylist.Position - 1];

                                // Increment song play count and set last played timestamp            
                                Library.UpdateSongPlayCount(data.CurrentSong.Song.SongId);

                                // Raise event
                                OnSongFinished(data);
                            }

                            // Play new gapless sequence
                            gaplessSequenceData = PlayGaplessSequence(subSoundsFilePaths);

                            // Load current sound
                            CurrentPlaylist.CurrentSong = CurrentPlaylist.Songs[CurrentPlaylist.Position];

                            // Reset flag
                            isNewGaplessSequenceLoading = false;
                        }
                    }
                    // Check if a new song is playing (excluding playing a new gapless sentence)
                    else if (!isNewGaplessSequenceLoading
                        && pos_sentence != currentSentenceIndex
                        && pos_sentence != subSoundsFilePaths.Count)
                    {
                        // Set flag
                        int previousSentenceIndex = currentSentenceIndex;
                        currentSentenceIndex = (int)pos_sentence;

                        // Increment playlist position                        
                        CurrentPlaylist.Position++;

                        // Load current sound                        
                        m_currentSound = new MPfm.Sound.FMODWrapper.Sound(gaplessSequenceData.subSounds[currentSentenceIndex], subSoundsFilePaths[currentSentenceIndex]);
                        currentSongLength = m_currentSound.LengthAbsoluteMilliseconds;
                        CurrentPlaylist.CurrentSong = CurrentPlaylist.Songs[CurrentPlaylist.Position - 1];

                        // Raise song end event (if an event is subscribed)
                        if (OnSongFinished != null)
                        {
                            // Create data
                            SongFinishedData data = new SongFinishedData();
                            data.CurrentSong = CurrentPlaylist.Songs[CurrentPlaylist.Position - 2];
                            data.NextSong = CurrentPlaylist.CurrentSong;

                            // Increment song play count and set last played timestamp            
                            Library.UpdateSongPlayCount(data.CurrentSong.Song.SongId);

                            // Raise event
                            OnSongFinished(data);
                        }
                    }

                    // Check if a loop is set
                    if (CurrentLoop != null)
                    {
                        // Check if the current position exceeds the marker B position,
                        // or if the current position is below the marker A position
                        if (MainChannel.PositionSentencePCM > m_currentLoopMarkerB.PositionPCM ||
                            MainChannel.PositionSentencePCM < m_currentLoopMarkerA.PositionPCM)
                        {
                            // Set marker A position (start loop)
                            MainChannel.SetPosition((uint)m_currentLoopMarkerA.PositionPCM.Value, FMOD.TIMEUNIT.SENTENCE_PCM);
                        }
                    }

                    // Update GUI and call delegate; check if an event is subscribed
                    if (OnTimerElapsed != null && IsPlaying)
                    {
                        // Create event data and fill with position values
                        TimerElapsedData data = new TimerElapsedData();

                        //uint pos_subsound = 0;
                        //uint pos_ms = 0;
                        //soundSystem.MainChannel.baseChannel.getPosition(ref pos_sentence, FMOD.TIMEUNIT.SENTENCE);
                        //soundSystem.MainChannel.baseChannel.getPosition(ref pos_subsound, FMOD.TIMEUNIT.SENTENCE_SUBSOUND);
                        //soundSystem.MainChannel.baseChannel.getPosition(ref pos_ms, FMOD.TIMEUNIT.SENTENCE_MS);

                        // The pos_sentence and pos_subsound change when the song is finished.
                        // The subSoundIndex variable changes when the next song is ready to be buffered.

                        //data.Debug = "Se " + pos_sentence.ToString() + " Sb " + pos_subsound.ToString();
                        //data.Debug2 = "MS " + pos_ms.ToString() + " CSI " + currentSentenceIndex.ToString();

                        data.SongPositionMilliseconds = MainChannel.PositionSentenceAbsoluteMilliseconds;
                        data.SongPosition = Conversion.MillisecondsToTimeString(MainChannel.PositionSentenceAbsoluteMilliseconds);
                        //data.SongLength = soundSystem.MainChannel.CurrentSound.Length;
                        //data.SongLengthPCMBytes = soundSystem.MainChannel.CurrentSound.LengthPCMBytes;
                        data.SongPositionPercentage = ((double)MainChannel.PositionSentenceAbsoluteMilliseconds / (double)currentSongLength) * 100;
                        //data.SongLengthMilliseconds = soundSystem.MainChannel.CurrentSound.LengthAbsoluteMilliseconds;
                        //data.SongPositionPCM = soundSystem.MainChannel.PositionPCM;
                        //data.SongPositionPCMBytes = soundSystem.MainChannel.PositionPCMBytes;
                        data.SongPositionSentencePCMBytes = MainChannel.PositionSentencePCMBytes;

                        // Analyse spectrum
                        //data.Spectrum = soundSystem.MainChannel.GetSpectrum();

                        // Get wave data from master                            
                        data.WaveDataLeft = soundSystem.GetWaveDataLeft();
                        data.WaveDataRight = soundSystem.GetWaveDataRight();

                        // Raise event
                        OnTimerElapsed(data);
                    }
                }

                //else
                //{
                //    // No gapless mode.
                //    // Check if the main channel is initialized and a sound is playing
                //    if (soundSystem.MainChannel.IsInitialized && soundSystem.MainChannel.IsPlaying)
                //    {
                //        // Update GUI and call delegate; check if an event is subscribed
                //        if (OnTimerElapsed != null)
                //        {
                //            // Create event data and fill with position values
                //            TimerElapsedData data = new TimerElapsedData();
                //            data.SongPosition = soundSystem.MainChannel.Position;
                //            data.SongPositionMilliseconds = soundSystem.MainChannel.PositionAbsoluteMilliseconds;
                //            data.SongPositionPercentage = ((double)soundSystem.MainChannel.PositionAbsoluteMilliseconds / (double)soundSystem.MainChannel.CurrentSound.LengthAbsoluteMilliseconds) * 100;
                //            data.SongLengthMilliseconds = soundSystem.MainChannel.CurrentSound.LengthAbsoluteMilliseconds;
                //            data.SongLength = soundSystem.MainChannel.CurrentSound.Length;
                //            data.SongPositionPCM = soundSystem.MainChannel.PositionPCM;

                //            // Analyse spectrum
                //            //data.Spectrum = soundSystem.MainChannel.GetSpectrum();

                //            // Get wave data from master                            
                //            data.WaveDataLeft = soundSystem.GetWaveDataLeft();
                //            data.WaveDataRight = soundSystem.GetWaveDataRight();

                //            // Raise event
                //            OnTimerElapsed(data);
                //        }
                //    }

                //    // Update sound system (necessary to play a sound)                    
                //    soundSystem.Update();
                //}
            }
            catch (Exception ex)
            {
                Tracing.Log("Player -- Timer Elapsed Exception: " + ex.Message + " - " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Fires when the end of a sound file is reached.
        /// </summary>
        /// <param name="args">Arguments</param>
        private void soundSystem_OnSoundEnd(EventArgs args)
        {
            // ISN'T CALLED AT ALL IN GAPLESS MODE.
            if (isGapless)
            {
                // Gapless mode does not call this event.
                return;
            }

            // Keep the end-of-file song for later
            //SongDTO endingSong = CurrentSong;
            PlaylistSongDTO endingSong = CurrentPlaylist.CurrentSong;

            // Determine next song to play. Check if the current playlist is valid
            if (CurrentPlaylist != null)
            {
                // Did the user stop the playback?
                if (IsPlaybackCancelled)
                {
                    // Reset flag and exit event
                    IsPlaybackCancelled = false;
                    return;
                }

                // Is the repeat mode in Song?
                if (RepeatType == RepeatType.Song)
                {
                    // Easy: start the song again. No need to reset CurrentSong.                 
                }
                else
                {
                    // Find the current song in the list
                    int index = CurrentPlaylist.Songs.IndexOf(CurrentPlaylist.CurrentSong);

                    // Was the song found?
                    if (index == -1)
                    {
                        // This is not normal!
                        throw new Exception("Current song was not found in current playlist!");
                    }
                    else
                    {
                        // Go to next song. Is this the last song of the playlist?
                        if (index == CurrentPlaylist.Songs.Count - 1)
                        {
                            // Is the repeat mode in Playlist? 
                            if (RepeatType == RepeatType.Playlist)
                            {
                                // Start from the beginning of the playlist
                                CurrentPlaylist.CurrentSong = CurrentPlaylist.Songs[0];
                            }
                            else if (RepeatType == RepeatType.Off)
                            {
                                // We reached the end of the playlist. Stop playback.                                
                                CurrentPlaylist.CurrentSong = null;
                            }
                        }
                        else
                        {
                            // Increment index and go to next song
                            CurrentPlaylist.CurrentSong = CurrentPlaylist.Songs[index + 1];
                        }
                    }
                }

                // Start next song if valid
                if (CurrentSong != null)
                {
                    // Play song
                    //PlaySong(CurrentSong, false, 0);
                }

                // Increment song play count and set last played timestamp            
                Library.UpdateSongPlayCount(endingSong.Song.SongId);

                // Check if an event is subscribed
                if (OnSongFinished != null)
                {
                    // Create data
                    SongFinishedData data = new SongFinishedData();
                    data.CurrentSong = endingSong;
                    data.NextSong = CurrentPlaylist.CurrentSong;

                    // Raise event
                    OnSongFinished(data);
                }
            }


            //// Add play count and update song in database
            //currentSong.LastPlayed = DateTime.Now;
            //currentSong.PlayCount++;
            //db.UpdateSong(currentSong);

            //// Update UI
            //UpdateSongPlayCount(currentSong.SongId);
            //formPlaylist.UpdateSongPlayCount(currentSong.SongId);
        }

        #endregion
    }

    /// <summary>
    /// Defines the playback modes for the player.     
    /// </summary>
    public enum PlaybackMode
    {
        /// <summary>
        /// This playback mode is set when no audio playing.
        /// </summary>
        None = 0,
        /// <summary>
        /// This playback mode is set when a user opens a file to be played.
        /// </summary>
        OpenFile = 1, 
        /// <summary>
        /// This playback mode is set when a user opens or generates a playlist to be played (gapless mode).
        /// </summary>
        Playlist = 2
    }

    /// <summary>
    /// Defines the repeat types (off, playlist or song).
    /// </summary>
    public enum RepeatType
    {
        Off = 0, Playlist = 1, Song = 2
    }

    /// <summary>
    /// Defines the data structure for the player timer method.
    /// </summary>
    public class TimerElapsedData
    {
        public string SongPosition { get; set; }
        public double SongPositionPercentage { get; set; }
        public uint SongPositionMilliseconds { get; set; }
        //public uint SongPositionPCM { get; set; }
        //public uint SongPositionPCMBytes { get; set; }
        public uint SongPositionSentencePCMBytes { get; set; }

        //public string SongLength { get; set; }
        //public uint SongLengthPCMBytes { get; set; }
        //public uint SongLengthMilliseconds { get; set; }

        public float[] Spectrum { get; set; }
        public float[] WaveDataLeft { get; set; }
        public float[] WaveDataRight { get; set; }

        public string Debug { get; set; }
        public string Debug2 { get; set; }
    }

    /// <summary>
    /// Defines the data structure for the end-of-song event.
    /// </summary>
    public class SongFinishedData
    {
        /// <summary>
        /// The song that has just finished playing
        /// </summary>
        public PlaylistSongDTO CurrentSong { get; set; }
        /// <summary>
        /// The next song that is starting to play
        /// </summary>
        public PlaylistSongDTO NextSong { get; set; }
    }



}
