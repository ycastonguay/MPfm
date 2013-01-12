//
// PlaylistItem.cs: This file contains the class defining a playlist item to 
//                  be used with the Player.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Threading;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass;

namespace MPfm.Sound
{
    /// <summary>
    /// Defines a playlist item to be used with the Player.
    /// </summary>
    public class PlaylistItem
    {
        private System.Timers.Timer timerFillBuffer = null;
        private ByteArrayQueue queue = null;
        private List<Task> tasksDecode = null;
        private CancellationTokenSource cancellationTokenSource = null;
        private CancellationToken cancellationToken;
        private int tempBufferLength = 20000;
        private int dataBufferLength = 1000000;
        private long decodePosition = 0;

        /// <summary>
        /// Private value for the Id property.
        /// </summary>
        private Guid id = Guid.Empty;
        /// <summary>
        /// Unique identifier for the playlist item (there might be the same 
        /// audio file multiple times in the same playlist).
        /// </summary>
        public Guid Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Pointer to the parent Playlist instance.
        /// </summary>
        private Playlist playlist = null;

        /// <summary>
        /// Private value for the SyncProc proprety.
        /// </summary>
        private SYNCPROC syncProc = null;
        /// <summary>
        /// Synchronization callback.
        /// </summary>
        public SYNCPROC SyncProc
        {
            get
            {
                return syncProc;
            }
            set
            {
                syncProc = value;
            }
        }

        /// <summary>
        /// Private value for the SyncProcHandle property.
        /// </summary>
        private int syncProcHandle = 0;
        /// <summary>
        /// Contains the handle to the SYNCPROC.
        /// </summary>
        public int SyncProcHandle
        {
            get
            {
                return syncProcHandle;
            }
            set
            {
                syncProcHandle = value;
            }
        }

        /// <summary>
        /// Private value for the LengthSamples property.
        /// </summary>
        private long lengthSamples = 0;
        /// <summary>
        /// Playlist item length (in samples).
        /// </summary>
        public long LengthSamples
        {
            get
            {
                return lengthSamples;
            }
        }

        /// <summary>
        /// Private value for the LengthBytes property.
        /// </summary>
        private long lengthBytes = 0;
        /// <summary>
        /// Playlist item length (in bytes).
        /// </summary>
        public long LengthBytes
        {
            get
            {
                return lengthBytes;
            }
        }

        /// <summary>
        /// Private value for the LengthMilliseconds property.
        /// </summary>
        private int lengthMilliseconds = 0;
        /// <summary>
        /// Playlist item length (in milliseconds).
        /// </summary>
        public int LengthMilliseconds
        {
            get
            {
                return lengthMilliseconds;
            }
        }

        /// <summary>
        /// Private value for the LengthString property.
        /// </summary>
        private string lengthString = string.Empty;
        /// <summary>
        /// Playlist item length (in 00:00.000 string format).
        /// </summary>
        public string LengthString
        {
            get
            {
                return lengthString;
            }
        }

        /// <summary>
        /// Private value for the Channel property.
        /// </summary>
        private Channel channel = null;
        /// <summary>
        /// BASS.NET channel used for playback decoding.
        /// </summary>
        public Channel Channel
        {
            get
            {
                return channel;
            }
        }

        /// <summary>
        /// Private value for the AudioFile property.
        /// </summary>
        private AudioFile audioFile = null;
        /// <summary>
        /// AudioFile structure containing metadata and other file information.
        /// </summary>
        public AudioFile AudioFile
        {
            get
            {
                return audioFile;
            }            
        }

        /// <summary>
        /// Private value for the IsLoaded property.
        /// </summary>
        private bool isLoaded = false;
        /// <summary>
        /// Indicates if the channel and the audio file metadata have been loaded.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }
        }

        /// <summary>
        /// Default constructor for the PlaylistItem class.
        /// Requires a hook to a Playlist instance.
        /// </summary>
        /// <param name="playlist">Playlist</param>
        /// <param name="audioFile">Audio file metadata</param>
        public PlaylistItem(Playlist playlist, AudioFile audioFile)
        {
            this.id = Guid.NewGuid();
            this.playlist = playlist;
            this.audioFile = audioFile;

            tasksDecode = new List<Task>();
            timerFillBuffer = new System.Timers.Timer();
            timerFillBuffer.Interval = 200;
            timerFillBuffer.Elapsed += HandleTimerFillBufferElapsed;
        }

        /// <summary>
        /// Loads the current channel and audio file metadata.
        /// </summary>
        public void Load()
        {
            // Load audio file metadata
            audioFile.RefreshMetadata();

            // Check if a channel already exists
            if(channel != null)
            {
                // Dispose channel
                Dispose();
            }

            // Load channel
            channel = MPfm.Sound.BassNetWrapper.Channel.CreateFileStreamForDecoding(audioFile.FilePath, true);

            // Load channel length
            lengthBytes = channel.GetLength();

            // Check if the channel is using floating point
            if (channel.IsFloatingPoint)
            {
                // Divide value by 2
                lengthBytes /= 2;
            }

            // Check if this is a FLAC file over 44100Hz
            if (audioFile.FileType == AudioFileFormat.FLAC && audioFile.SampleRate > 44100)
            {
                lengthBytes = (long)((float)lengthBytes * 1.5f);
            }

            lengthSamples = ConvertAudio.ToPCM(lengthBytes, (uint)audioFile.BitsPerSample, 2);
            lengthMilliseconds = (int)ConvertAudio.ToMS(lengthSamples, (uint)audioFile.SampleRate);
            lengthString = Conversion.MillisecondsToTimeString((ulong)lengthMilliseconds);

            // Decode file in another thread
            //Decode(0);

            // Set flag
            isLoaded = true;
        }

        private void HandleTimerFillBufferElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Disable timer 
            timerFillBuffer.Stop();

            // Check buffer status (is there enough space to fill more chunks?)
            if (queue.BufferLength - queue.BufferDataLength < tempBufferLength)
            {
                // Reactivate timer; check for buffer health in 200ms
                timerFillBuffer.Start();
                return;
            }

            // Resume decoding from previous position
            StartDecode(decodePosition);
        }

        public void CancelDecode()
        {
            // Check if the decode task can be cancelled
            if (tasksDecode.Count > 0)
            {
                cancellationTokenSource.Cancel(true);
                try
                {
                    Task.WaitAll(tasksDecode.ToArray());
                }
                catch(AggregateException ex)
                {
                    // This means the task has been canceled successfully
                }
            }

            // Cleanup any previous decodes
            queue = null;

            // Reset token
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
        }

        public void Decode(long startPosition)
        {
            // Create queue and start decoding
            queue = new ByteArrayQueue(dataBufferLength);            
            StartDecode(startPosition);
        }

        private void StartDecode(long startPosition)
        {
            // Cancel any decode task
            //CancelDecode();

            // Create channel and get length
            Channel channel = Channel.CreateFileStreamForDecoding(audioFile.FilePath, true);
            long length = channel.GetLength();
            decodePosition = startPosition;
            
            // Decode file in a different thread
            Task taskDecode = Task.Factory.StartNew(() => {
                try
                {
                    // Set starting position
                    if(startPosition > 0)
                        channel.SetPosition(startPosition);

                    // Loop until file has been 100% decoded
                    while (true)
                    {
                        // Check for cancel
                        cancellationToken.ThrowIfCancellationRequested();

                        // Check buffer status (is there enough space to fill more chunks?)
                        if(queue.BufferLength - queue.BufferDataLength < tempBufferLength)
                        {
                            // Start fill buffer timer
                            timerFillBuffer.Start();
                            break; // No more space, quit loop+thread
                        }

                        // Decode chunk
                        byte[] bytes = new byte[tempBufferLength];
                        int dataLength = 0;
                        try
                        {
                            dataLength = channel.GetData(bytes, tempBufferLength);
                            if(dataLength == -1)
                            {
                                BASSError error = Bass.BASS_ErrorGetCode();
                                if(error == BASSError.BASS_ERROR_ENDED)
                                {
                                    break; // Decode done
                                }
                                else
                                {
                                    Console.WriteLine(error.ToString());
                                    throw new Exception(error.ToString());
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            throw;
                        }

                        // Add to queue
                        decodePosition += bytes.Length;
                        queue.Enqueue(bytes);
                    }
                }
                catch(Exception ex)
                {
                    // Exception in cancallation token
                    Console.WriteLine(ex.Message);
                    //throw; // no need to throw on cancel
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
            tasksDecode.Add(taskDecode);
        }

        public byte[] GetData(int length)
        {
            return queue.Dequeue(length);
        }

        /// <summary>
        /// Disposes the current channel.
        /// </summary>
        public void Dispose()
        {
            // Check if a channel already exists
            if (channel != null)
            {
                // Check if the channel is in use
                if (channel.IsActive() == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    // Stop and free channel                    
                    channel.Stop();
                    channel.Free();
                    channel = null;
                }
            }

            // Set flags
            isLoaded = false;
        }       
    }
}
