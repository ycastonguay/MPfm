// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.BassNetWrapper;
#if WINDOWSSTORE
using Windows.UI.Xaml;
#elif WINDOWS_PHONE
using System.Windows.Threading;
#endif
using Sessions.Core;

namespace Sessions.Sound.Playlists
{
    /// <summary>
    /// Defines a playlist item to be used with the Player.
    /// </summary>
    public class PlaylistItem
    {
        private readonly Guid _id = Guid.Empty;
        /// <summary>
        /// Unique identifier for the playlist item (there might be the same 
        /// audio file multiple times in the same playlist).
        /// </summary>
        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        private long _lengthSamples = 0;
        /// <summary>
        /// Playlist item length (in samples).
        /// </summary>
        public long LengthSamples
        {
            get
            {
                return _lengthSamples;
            }
        }

        private long _lengthBytes = 0;
        /// <summary>
        /// Playlist item length (in bytes).
        /// </summary>
        public long LengthBytes
        {
            get
            {
                return _lengthBytes;
            }
        }

        private long _lengthMilliseconds = 0;
        /// <summary>
        /// Playlist item length (in milliseconds).
        /// </summary>
        public long LengthMilliseconds
        {
            get
            {
                return _lengthMilliseconds;
            }
        }

        private string _lengthString = string.Empty;
        /// <summary>
        /// Playlist item length (in 00:00.000 string format).
        /// </summary>
        public string LengthString
        {
            get
            {
                return _lengthString;
            }
        }

        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE

        // TODO: Consider adding a wrapper around to make this not BASS-related
        private Channel _channel = null;
        /// <summary>
        /// BASS.NET channel used for playback decoding.
        /// </summary>
        public Channel Channel
        {
            get
            {
                return _channel;
            }
        }

        #endif

        private readonly AudioFile _audioFile = null;
        /// <summary>
        /// AudioFile structure containing metadata and other file information.
        /// </summary>
        public AudioFile AudioFile
        {
            get
            {
                return _audioFile;
            }            
        }

        private bool _isLoaded = false;
        /// <summary>
        /// Indicates if the channel and the audio file metadata have been loaded.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }

        /// <summary>
        /// Default constructor for the PlaylistItem class.
        /// Requires a hook to a Playlist instance.
        /// </summary>
        /// <param name="audioFile">Audio file metadata</param>
        public PlaylistItem(AudioFile audioFile)
        {
            _id = Guid.NewGuid();
            _audioFile = audioFile;
        }

        /// <summary>
        /// Loads the current channel and audio file metadata.
        /// </summary>
        public void Load(bool useFloatingPoint)
        {
            // Load audio file metadata
            _audioFile.RefreshMetadata();

            // Check if a channel already exists
            if (_channel != null)
                Dispose();

            // Load channel
            _channel = Channel.CreateFileStreamForDecoding(_audioFile.FilePath, useFloatingPoint);

            // Check if this file should be started at a specific point (i.e. CUE file)
            int startPositionMS = -1;
            int startPositionSamples = -1;
            long startPositionBytes = -1;
            if (!string.IsNullOrEmpty(_audioFile.StartPosition))
            {
                startPositionMS = ConvertAudio.ToMS(_audioFile.StartPosition);
                startPositionSamples = ConvertAudio.ToPCM(startPositionMS, _audioFile.SampleRate);
                startPositionBytes = ConvertAudio.ToPCMBytes(startPositionSamples, _audioFile.BitsPerSample, _audioFile.AudioChannels);

                if(_channel.IsFloatingPoint)
                    startPositionBytes *= 2;
            }

            // Check if this file should end at a specific point (i.e. CUE file)
            int endPositionMS = -1;
            int endPositionSamples = -1;
            long endPositionBytes = -1;
            if (!string.IsNullOrEmpty(_audioFile.EndPosition))
            {
                endPositionMS = ConvertAudio.ToMS(_audioFile.EndPosition);
                endPositionSamples = ConvertAudio.ToPCM(endPositionMS, _audioFile.SampleRate);
                endPositionBytes = ConvertAudio.ToPCMBytes(endPositionSamples, _audioFile.BitsPerSample, _audioFile.AudioChannels);

                if(_channel.IsFloatingPoint)
                    endPositionBytes *= 2;
            }

            // Did we find a range to play?
            if (startPositionBytes >= 0 && endPositionBytes >= 0)
            {
                Console.WriteLine("--> PlaylistItem - Setting channel position to {0} ({1} ms/{2} bytes)", _audioFile.StartPosition, startPositionMS, startPositionBytes);
                _channel.SetPosition(startPositionBytes);
                _lengthBytes = endPositionBytes - startPositionBytes;
            }
            else
            {
                _lengthBytes = _channel.GetLength();
            }

            // Divide length by 2 if using floating point
            if (_channel.IsFloatingPoint)
                _lengthBytes /= 2;

            // Check if this is a FLAC file over 44100Hz
            if (_audioFile.FileType == AudioFileFormat.FLAC && _audioFile.SampleRate > 44100)
                _lengthBytes = (long)((float)_lengthBytes * 1.5f);

            _lengthSamples = ConvertAudio.ToPCM(_lengthBytes, _audioFile.BitsPerSample, _audioFile.AudioChannels);
            _lengthMilliseconds = ConvertAudio.ToMS(_lengthSamples, _audioFile.SampleRate);
            _lengthString = ConvertAudio.ToTimeString(_lengthMilliseconds);

            _isLoaded = true;
        }

        /// <summary>
        /// Disposes the current channel.
        /// </summary>
        public void Dispose()
        {
            #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE

            // Check if a channel already exists
            if (_channel != null)
            {
                try
                {
                    // Stop and free channel
                    Console.WriteLine("Freeing channel " + AudioFile.FilePath + "...");
                    _channel.Stop();
                    _channel.Free();
                    _channel = null;
                }
                catch(Exception ex)
                {
                    if(_audioFile != null)
                        Console.WriteLine("Could not dispose channel " + AudioFile.FilePath + ": " + ex.Message);
                }

//                // Check if the channel is in use
//                if (channel.IsActive() == BASSActive.BASS_ACTIVE_PLAYING)
//                {
//                    // Stop and free channel                    
//                    channel.Stop();
//                    channel.Free();
//                    channel = null;
//                }                
            }

            #endif

            _isLoaded = false;
        }       
    }
}
