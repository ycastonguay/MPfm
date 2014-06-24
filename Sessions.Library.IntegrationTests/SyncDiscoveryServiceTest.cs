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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sessions.Library.Services;
using Moq;
using Sessions.Core.Network;
using Sessions.Library.Services.Interfaces;
using Sessions.Library.Objects;
using Sessions.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Sessions.Library.IntegrationTests
{
    [TestFixture()]
    public class SyncDiscoveryServiceTest
    {
        private Dictionary<string, string> _devices;
        private ISyncDiscoveryService _discoveryService;
        
        [SetUp]
        public void InitializeTests()
        {
            PrepareExpectedResults();
            
            // Mock HttpService with expected results
            var httpServiceMock = new Mock<IHttpService>();
            
            // Only one setup is possible per method.           
            httpServiceMock.Setup(m => m.DownloadString(It.IsAny<string>()))
                .Returns<string>(s => {
                    var key = _devices.Keys.FirstOrDefault(x => s.Contains(x));
                    return key == null ? null : _devices[key];
                });
            
            // Mock HttpServiceFactory to inject the mocked service
            var httpServiceFactoryMock = new Mock<IHttpServiceFactory>();
            httpServiceFactoryMock.Setup(m => m.CreateService(It.IsAny<int>()))
                .Returns(() => httpServiceMock.Object);
            
            _discoveryService = new SyncDiscoveryService(httpServiceFactoryMock.Object);
        }
        
        private void PrepareExpectedResults()
        {
            _devices = new Dictionary<string, string>();
            _devices.Add("192.168.1.1", null);
            _devices.Add("192.168.1.2", GenerateTestDeviceXML("TestDevice1"));
            _devices.Add("192.168.1.3", null);
            _devices.Add("192.168.1.4", GenerateTestDeviceXML("TestDevice2"));
        }
        
        private string GenerateTestDeviceXML(string deviceName)
        {
            var testDevice = new SyncDevice();
            testDevice.Name = deviceName;
            testDevice.SyncVersionId = SyncListenerService.SyncVersionId;
            return XmlSerialization.Serialize<SyncDevice>(testDevice);
        }            

        [Test]
        public void ShouldReturnExpectedResults()
        {
            // Arrange
            var finished = new ManualResetEvent(false);
            _discoveryService.OnDeviceFound += (device) => {
                //Console.WriteLine("Device found: {0} - {1}", device.Name, device.Url);
                Assert.IsNotNull(_devices.Keys.FirstOrDefault(x => device.Url.Contains(x)));
            };
            _discoveryService.OnDiscoveryEnded += () => {
                finished.Set();
            };

            // Act
            _discoveryService.Start();
            _discoveryService.AddDevicesToSearchList(_devices.Keys);

            Assert.IsTrue(finished.WaitOne(1000));
        }        
    }
}
