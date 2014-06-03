// Copyright © 2011-2013 Yanick Castonguay
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass;

namespace MPfm.Player.Services
{
    public class DecodingService : IDecodingService
    {
        private readonly object _locker = new object();
        private bool _shouldExitWorker = false;
        private ByteArrayQueue _dataQueue = null;
        private Channel _decodingChannel = null;
        private List<Task> _tasksDecode = null;
        private CancellationTokenSource _cancellationTokenSource = null;
        private CancellationToken _cancellationToken;
        private int _chunkLength = 20000;
        private int _bufferLength = 1000000;
        private long _decodePosition = 0;
        private long _channelLength = 0;
        private ConcurrentQueue<string> _audioFileQueue;
        private string _currentFilePath;

        public DecodingService(int bufferLength)
        {
            _bufferLength = bufferLength;
            _dataQueue = new ByteArrayQueue(_bufferLength);
            _audioFileQueue = new ConcurrentQueue<string>();

            StartWorker();
        }

        public void StartWorker()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (!_shouldExitWorker)
                {
                    Console.WriteLine("DecodingService - Worker looping...");

                    // Make sure we are done decoding chunks before we add/remove files from the queue
                    lock (_locker)
                    {
                        // Fill buffer until it is completely filled
                        // Check buffer fill state (make sure there is enough space to fill with a chunk)
                        while (!string.IsNullOrEmpty(_currentFilePath) &&
                               _dataQueue.BufferDataLength - _dataQueue.BufferLength < _chunkLength)
                        {
                            Console.WriteLine("DecodingService - Decoding chunk...");
                            int chunkLength = Math.Min(-(_dataQueue.BufferDataLength - _dataQueue.BufferLength), _chunkLength);
                            if (chunkLength == 0)
                                break;

                            int length = DecodeChunk(chunkLength);
                            if (length == 0)
                            {
                                // If there is no more audio to decode, skip to the next file
                                string audioFilePath = string.Empty;
                                bool success = _audioFileQueue.TryDequeue(out audioFilePath);
                                if (success)
                                {
                                    Console.WriteLine("DecodingService - Skipping to next file: {0}...", audioFilePath);
                                    CreateDecodingChannel(_currentFilePath);
                                }
                            }
                        }
                    }

                    // The buffer has been filled; wait until a consumer removes data from the queue 
                    // before trying to fill the buffer again
                    Console.WriteLine("DecodingService - Worker waiting for consumer to remove data from queue...");
                    lock (_locker)
                    {
                        Monitor.Wait(_locker);
                    }

                    //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Loop - requests.Count: {0} numberOfBitmapTasksRunning: {1}", _requests.Count, _numberOfBitmapTasksRunning);
                }
            }));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void StopWorker()
        {
            _shouldExitWorker = true;
        }

        private void ClearAudioFileQueue()
        {
            string temp;
            while (!_audioFileQueue.IsEmpty)
                _audioFileQueue.TryDequeue(out temp);
        }

        public void StopDecoding()
        {
            _currentFilePath = string.Empty;
            _dataQueue.Clear();
            _decodePosition = 0;
        }

        public void StartDecodingFile(string audioFilePath, long startPosition)
        {
            lock (_locker)
            {
                // Are we really sure the decoding is over?
                StopDecoding();
                ClearAudioFileQueue();
                _dataQueue.Clear();
                _decodePosition = startPosition;
                CreateDecodingChannel(audioFilePath);
            }
            _audioFileQueue.Enqueue(audioFilePath);
        }

        public void AddFileToDecodeQueue(string audioFilePath)
        {
            _audioFileQueue.Enqueue(audioFilePath);
            lock (_locker)
            {
                Monitor.Pulse(_locker);
            }
        }

        public void AddFilesToDecodeQueue(List<string> audioFilePaths)
        {
            foreach(var audioFilePath in audioFilePaths)
                _audioFileQueue.Enqueue(audioFilePath);
            lock (_locker)
            {
                Monitor.Pulse(_locker);
            }
        }

        private void CreateDecodingChannel(string audioFilePath)
        {
            _decodingChannel = Channel.CreateFileStreamForDecoding(audioFilePath, true);
            _currentFilePath = audioFilePath;
            _channelLength = _decodingChannel.GetLength();
        }

        private int DecodeChunk(int chunkLength)
        {
            Console.WriteLine("DecodingService - DecodeChunk - chunkLength: {0}", chunkLength);
            byte[] bytes = new byte[chunkLength];
            int dataLength = _decodingChannel.GetData(bytes, chunkLength);
            if (dataLength == -1)
            {
                var error = Bass.BASS_ErrorGetCode();
                if (error == BASSError.BASS_ERROR_ENDED)
                {
                    Console.WriteLine("DecodingService - DecodeChunk - EOF (End of file) reached!");
                    return 0;
                }
                else
                {
                    Console.WriteLine("DecodingService - DecodeChunk - " + error.ToString());
                    throw new Exception(error.ToString());
                }
            }

            // Add to queue
            lock (_locker)
            {
                _dataQueue.Enqueue(bytes);
                _decodePosition += bytes.Length;
            }
            return bytes.Length;
        }

        public byte[] DequeueData(int length)
        {
            Console.WriteLine("DecodingService - DequeueData - length: {0}", length);
            var data = _dataQueue.Dequeue(length);
            lock (_locker)
            {
                Monitor.Pulse(_locker);
            }
            return data;
        }
    }
}
