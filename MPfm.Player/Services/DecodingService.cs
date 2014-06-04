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
        private bool _useFloatingPoint = false;
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

        public DecodingService(int bufferLength, bool useFloatingPoint)
        {
            _bufferLength = bufferLength;
            _useFloatingPoint = useFloatingPoint;
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
                    // Make sure we are done decoding chunks before we add/remove files from the queue
                    lock (_locker)
                    {
                        // Fill buffer until it is completely filled
                        // Check buffer fill state (make sure there is enough space to fill with a chunk)
                        while (!string.IsNullOrEmpty(_currentFilePath) &&
                               _dataQueue.BufferDataLength - _dataQueue.BufferLength < _chunkLength)
                        {
                            // Size the chunk to try to fill the missing data into the queue
                            int chunkLength = Math.Min(-(_dataQueue.BufferDataLength - _dataQueue.BufferLength), _chunkLength);
                            if (chunkLength == 0)
                            {
                                Console.WriteLine("DecodingService - Worker - No chunk to decode");
                                break;
                            }

                            // Make sure the chunk length doesn't exceed the remaining data in the current file
                            if (_channelLength - _decodePosition < chunkLength)
                                chunkLength = (int) (_channelLength - _decodePosition);

                            bool skipToNextFile = chunkLength == 0;
                            if (!skipToNextFile)
                            {
                                int length = DecodeChunk(chunkLength);
                                skipToNextFile = length == 0;
                            }

                            if (skipToNextFile)
                            {                                
                                string audioFilePath = string.Empty;
                                bool success = _audioFileQueue.TryDequeue(out audioFilePath);
                                if (success)
                                {
                                    Console.WriteLine("DecodingService - Worker - Skipping to next file: {0}...", audioFilePath);
                                    CreateDecodingChannel(_currentFilePath);
                                }
                            }
                        }
                    }

                    // The buffer has been filled; wait until a consumer removes data from the queue 
                    // before trying to fill the buffer again
                    Console.WriteLine("DecodingService - Worker - Waiting for consumer to remove data from queue...");
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
            _decodingChannel = Channel.CreateFileStreamForDecoding(audioFilePath, _useFloatingPoint);
            _currentFilePath = audioFilePath;
            _channelLength = _decodingChannel.GetLength();
        }

        private int DecodeChunk(int chunkLength)
        {
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
                Console.WriteLine("DecodingService - DecodeChunk - Requested {0} bytes; decoded {1} bytes - decodePosition: {2}/{3} - queue size: {4}/{5} ({6}%)", chunkLength, dataLength, _decodePosition, _channelLength, _dataQueue.BufferDataLength, _dataQueue.BufferLength, _dataQueue.BufferFillPercentage);
            }
            return bytes.Length;
        }

        public byte[] DequeueData(int length)
        {
            Console.WriteLine("DecodingService - DequeueData - length: {0} bufferDataLength: {1} bufferFillPercentage: {2}", length, _dataQueue.BufferDataLength, _dataQueue.BufferFillPercentage);
            var data = _dataQueue.Dequeue(length);
            lock (_locker)
            {
                Monitor.Pulse(_locker);
            }
            return data;
        }
    }
}
