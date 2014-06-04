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
using MPfm.Player.Services;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using NUnit.Framework;

namespace MPfm.Tests.Player
{
    [TestFixture()]
    public class TestDecoderService
    {
        private IDecodingService _decodingService;
        private TestDevice _testDevice;

        static string[] TestAudioFilePaths =          
        { 
            //"/Volumes/Mountain Lion Data/MP3/Cut Copy/In Ghost Colours/13 Visions.mp3"
            @"z:\Data1\Mp3\Bob Marley\Kaya\02 Kaya.mp3"
        };

        public TestDecoderService()
        {
            InitializeBass();
            _decodingService = new DecodingService(2000000, true);
        }

        private void InitializeBass()
        {
            Console.WriteLine("Registering BASS.NET...");
            Base.Register(BassNetKey.Email, BassNetKey.RegistrationKey);
            Console.WriteLine("Initializing device...");
            _testDevice = new TestDevice(DriverType.DirectSound, -1, 44100);
        }

        [Test, TestCaseSource("TestAudioFilePaths")]
        public void TestDecode(string audioFilePath)
        {
            var taskTest = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("[Producer Thread] Starting to decode file...");
                _decodingService.StartDecodingFile(audioFilePath, 0);
                //Thread.Sleep(5000);
            });

            while (true)
            {
                Console.WriteLine("[Consumer Thread] - Dequeing data...");
                var data = _decodingService.DequeueData(200000);
                if (data.Length == 0)
                {
                    Console.WriteLine("[Consumer Thread] - No data to dequeue!");
                    //break;
                }

                Thread.Sleep(50);
            }
        }
    }
}
