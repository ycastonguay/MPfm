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
using NUnit.Framework;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services;
using Sessions.Sound.PeakFiles;
using Sessions.Core.TestConfiguration;
using Sessions.Sound.AudioFiles;
using System.Threading;
using Sessions.Sound;
using Sessions.GenericControls.Services.Objects;
using Sessions.GenericControls.Basics;
using System.IO;
using Sessions.Sound.Player;

namespace Sessions.GenericControls.IntegrationTests
{
    [TestFixture]
    public class PeakFileServiceTest
    {
        protected IPeakFileService Service;
        protected TestConfigurationEntity Config { get; private set; }

        public PeakFileServiceTest()
        {
            Config = TestConfigurationHelper.Load();
        }

        protected AudioFile GetTestAudioFile()
        {
            return new AudioFile(Config.AudioFilePaths[0]);       
        }

        protected void InitializeBass()
        {
            Console.WriteLine("Initializing Bass...");
            var player = new SSPPlayer();
            player.Init();
            player.InitDevice(-1, 44100, 1000, 100, true);
        }

        [SetUp]
        public void InitializeTests()
        {
            InitializeBass();
            Service = new PeakFileService();
        }

        [Test]
        public void ShouldGeneratePeakFile()
        {
            Service.OnProcessStarted += (data) => {
                Console.WriteLine("PeakFileServiceTest - Service.OnProcessStarted");
            };
            Service.OnProcessData += (data) => {
                Console.WriteLine("PeakFileServiceTest - Service.OnProcessData - percentageDone: {0} currentBlock: {1}", data.PercentageDone, data.CurrentBlock);
            };
            Service.OnProcessDone += (data) => {
                Console.WriteLine("PeakFileServiceTest - Service.OnProcessDone");
            };

            // Act
            var finished = new ManualResetEvent(false);
            var audioFile = GetTestAudioFile();
            string peakFilePath = PeakFileService.GetPeakFilePathForAudioFileAndCreatePeakFileDirectory(audioFile);

            if(File.Exists(peakFilePath))
                File.Delete(peakFilePath);

            Service.GeneratePeakFile(audioFile.FilePath, peakFilePath);
            Assert.IsFalse(finished.WaitOne(15000));
        }

        [Test]
        public void ShouldCancelGeneratePeakFile()
        {
            // Arrange
            string currentDir = Directory.GetCurrentDirectory();
            var finished = new ManualResetEvent(false);
            var audioFile = GetTestAudioFile();
            string peakFilePath = PeakFileService.GetPeakFilePathForAudioFileAndCreatePeakFileDirectory(audioFile);

            if(File.Exists(peakFilePath))
                File.Delete(peakFilePath);

            Service.OnProcessStarted += (data) => {
                Console.WriteLine("PeakFileServiceTest - Service.OnProcessStarted");
            };
            Service.OnProcessData += (data) => {
                if(data.PercentageDone > 5)
                {
                    Console.WriteLine("PeakFileServiceTest - Percentage done > 5%; Cancelling peak file generation...");
                    Service.Cancel();
                }
                Console.WriteLine("PeakFileServiceTest - Service.OnProcessData - percentageDone: {0} currentBlock: {1}", data.PercentageDone, data.CurrentBlock);
            };
            Service.OnProcessDone += (data) => {
                Console.WriteLine("PeakFileServiceTest - Service.OnProcessDone - cancelled: {0}", data.Cancelled);
                if(data.Cancelled)
                    finished.Reset();
            };

            // Act
            Service.GeneratePeakFile(audioFile.FilePath, peakFilePath);
            bool result = finished.WaitOne(15000);
            Assert.IsTrue(result);
        }
	}
}
