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
using Moq;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services;
using Sessions.Sound.PeakFiles;
using Sessions.Core.TestConfiguration;
using Sessions.Sound.AudioFiles;
using System.Threading;
using Sessions.Sound.BassNetWrapper;
using Sessions.Sound;

namespace Sessions.GenericControls.IntegrationTests
{
    [TestFixture]
    public abstract class WaveFormRenderingServiceTestBase
    {
        private IMemoryGraphicsContextFactory _memoryGraphicsContextFactory;
        private IPeakFileService _peakFileService;

        protected TestConfigurationEntity Config { get; private set; }
        protected TestDevice TestDevice { get; private set; }
        protected IWaveFormRenderingService RenderingService { get; private set; }

        protected WaveFormRenderingServiceTestBase()
        {
            Config = TestConfigurationHelper.Load();
        }

        protected void SetMemoryGraphicsContextFactory(IMemoryGraphicsContextFactory memoryGraphicsContextFactory)
        {
            // This allows injecting real implementations of graphics rendering to test on multiple platforms
            _memoryGraphicsContextFactory = memoryGraphicsContextFactory;
        }

        protected AudioFile GetTestAudioFile()
        {
            return new AudioFile(Config.AudioFilePaths[0]);       
        }

        protected void InitializeBass()
        {
            Console.WriteLine("Initializing Bass...");
            Base.Register(BassNetKey.Email, BassNetKey.RegistrationKey);
            TestDevice = new TestDevice(DriverType.DirectSound, -1, 44100);
        }

        [SetUp]
        public void InitializeTests()
        {
            if(_memoryGraphicsContextFactory == null)
                throw new NullReferenceException("MemoryGraphicsContextFactory is null!");
                        
            InitializeBass();
            _peakFileService = new PeakFileService();
            RenderingService = new WaveFormRenderingService(_peakFileService, _memoryGraphicsContextFactory);
        }

        [Test]
        public void ShouldGeneratePeakFile()
        {
            // Arrange
            RenderingService.GeneratePeakFileBegunEvent += (sender, e) => {
                Console.WriteLine("GeneratePeakFileBegunEvent");
            };
            RenderingService.GeneratePeakFileProgressEvent += (sender, e) => {
                Console.WriteLine("GeneratePeakFileProgressEvent - percentageDone: {0}", e.PercentageDone);
            };
            RenderingService.GeneratePeakFileEndedEvent += (sender, e) => {
                Console.WriteLine("GeneratePeakFileEndedEvent");
            };

            // Act
            var finished = new ManualResetEvent(false);
            RenderingService.LoadPeakFile(GetTestAudioFile());
            Assert.IsFalse(finished.WaitOne(100000));
        }
	}
}
