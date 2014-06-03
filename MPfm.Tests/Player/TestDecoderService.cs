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
using MPfm.Player.Services;
using NUnit.Framework;
using MPfm.Sound.BassNetWrapper;
using MPfm.Sound;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MPfm.Tests
{
    [TestFixture()]
    public class TestDecoderService
    {
        private IDecodingService _decodingService;
        private TestDevice _testDevice;

        static string[] TestAudioFilePaths =          
        { 
            "/Volumes/Mountain Lion Data/MP3/Cut Copy/In Ghost Colours/13 Visions.mp3"
        };

        public TestDecoderService()
        {
            Console.WriteLine("Initializing device...");
            InitializeBass();
            _decodingService = new DecodingService(100000);
        }

        private void InitializeBass()
        {
            Base.Register(BassNetKey.Email, BassNetKey.RegistrationKey);
            _testDevice = new TestDevice(DriverType.DirectSound, -1, 44100);
        }

        [Test, TestCaseSource("TestAudioFilePaths")]
        public void TestDecode(string audioFilePath)
        {
            var taskTest = Task.Factory.StartNew(() =>
            {
                _decodingService.StartDecodingFile(audioFilePath, 0);
                var taskDequeue = Task.Factory.StartNew(() =>
                {
                    while(true)
                    {
                        var data = _decodingService.DequeueData(10000);
                        if(data.Length == 0)
                            break;

                        Thread.Sleep(500);
                    }
                });
                Thread.Sleep(5000);
            });
        }
    }
}
