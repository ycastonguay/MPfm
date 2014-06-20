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
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sessions.Core;
using Sessions.Player.Services;
using Sessions.Sound;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.BassNetWrapper;

namespace Sessions.Player.Tests
{
    [TestFixture()]
    public class TestDecodingService
    {
        private const int QueueCapacity = 2000000;
        private IDecodingService _decodingService;
        private TestDevice _testDevice;

        static string[] TestAudioFilePaths =          
        { 
            //"/Volumes/Mountain Lion Data/MP3/Cut Copy/In Ghost Colours/13 Visions.mp3"
            @"z:\Data1\Mp3\Bob Marley\Kaya\02 Kaya.mp3"
        };

        public TestDecodingService()
        {
            InitializeBass();
            _decodingService = new DecodingService(QueueCapacity, true);
        }

        private void InitializeBass()
        {
            Console.WriteLine("Initializing Bass...");
            Base.Register(BassNetKey.Email, BassNetKey.RegistrationKey);
            _testDevice = new TestDevice(DriverType.DirectSound, -1, 44100);
        }

        //[Test, TestCaseSource("TestAudioFilePaths")]
        //public void TestDecodeWithExcessivelyLargeChunks(string audioFilePath)
        //{
        //    const int chunkSize = 200000;
        //    bool shouldLoop = true;
        //    _decodingService.OnNoAudioFileToDecode += () =>
        //    {
        //        Console.WriteLine("[Consumer] Finished decoding file!");
        //        shouldLoop = false;
        //    };

        //    Task.Factory.StartNew(() =>
        //    {
        //        Console.WriteLine("[Producer] Starting to decode file...");
        //        _decodingService.StartDecodingFile(audioFilePath, 0);
        //    });

        //    while (shouldLoop)
        //    {
        //        int length = Math.Min(_decodingService.BufferDataLength, chunkSize);
        //        Console.WriteLine("[Consumer] - Dequeing data (length: {0})...", length);
        //        var data = _decodingService.DequeueData(length);
        //        if (data.Length == 0)
        //        {
        //            Console.WriteLine("[Consumer] - No data to dequeue!");
        //            //break;
        //        }

        //        Thread.Sleep(50);
        //    }
        //}

        [Test, TestCaseSource("TestAudioFilePaths")]
        public void TestDecodeWithNormalChunks(string audioFilePath)
        {
            //TestDecode(audioFilePath, false);
        }

        [Test, TestCaseSource("TestAudioFilePaths")]
        public void TestDecodeWithBusyDisk(string audioFilePath)
        {
            TestDecode(audioFilePath, true);
        }

        private void TestDecode(string audioFilePath, bool isBusyDisk)
        {
            // Get byte length for 100ms worth of 44100Hz, 16-bit, Stereo
            int chunkSize = GetByteArrayLengthForMS(100);
            var queue = new ByteArrayQueue(chunkSize * 10);
            bool shouldLoop = true;
            _decodingService.OnNoAudioFileToDecode += () =>
            {
                Console.WriteLine("Finished decoding file!");
                shouldLoop = false;
            };

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Starting to decode file...");
                _decodingService.StartDecodingFile(audioFilePath, 0);
            });

            Task.Factory.StartNew(() =>
            {
                while (shouldLoop)
                {
                    try
                    {
                        // Make sure the chunk length doesn't exceed the remaining data in the current file
                        int chunkSizeToDequeue = Math.Min(queue.BufferLength - queue.BufferDataLength, Math.Max(queue.BufferDataLength, chunkSize));
                        //int chunkSizeToDequeue = Math.Min(queue.BufferLength - queue.BufferDataLength, chunkSize);
                        Console.WriteLine("[>>>>>>] Dequeuing data (chunkSize: {0} chunkSizeToDequeue: {1} decodingServiceBuffer: {2}/{3})...", chunkSize, chunkSizeToDequeue, queue.BufferDataLength, queue.BufferLength);

                        if (chunkSizeToDequeue > 0)
                            queue.Dequeue(chunkSizeToDequeue);

                        Thread.Sleep(20);
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("Failed: {0}", ex);
                    }
                }
            });

            byte counter = 0;
            while (shouldLoop)
            {
                try
                {
                    // Make sure we don't dequeue more data than available in the decoder service, or not more data than fits in the audio queue
                    int chunkSizeToEnqueue = Math.Min(_decodingService.BufferDataLength, chunkSize);
                    chunkSizeToEnqueue = Math.Min(queue.BufferLength - queue.BufferDataLength, chunkSizeToEnqueue);
                    Console.WriteLine("[<<<<<<] Enqueuing data (length: {0})...", chunkSizeToEnqueue);

                    var data = _decodingService.DequeueData(chunkSizeToEnqueue);
                    if (data.Length > 0)
                        queue.Enqueue(data);
                    //else
                    //Console.WriteLine("[Consumer] - No data to dequeue!");

                    // Simulate busy disk every 10 cycles
                    if (data.Length > 0 && isBusyDisk && counter % 10 == 0)
                    {
                        Console.WriteLine("!!!!!!!!!! SIMULATE BUSY DISK! !!!!!!!!!!!!!!!!");
                        Thread.Sleep(20);
                        counter = 0;
                    }

                    if(data.Length > 0)
                        counter++;

                    Thread.Sleep(20);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed: {0}", ex);
                }
            }
        }

        private int GetByteArrayLengthForMS(uint milliseconds)
        {
            uint samples = ConvertAudio.ToPCM(100, 44100);
            uint bytes = ConvertAudio.ToPCMBytes(samples, 16, 2);
            return (int) bytes;
        }
    }
}
