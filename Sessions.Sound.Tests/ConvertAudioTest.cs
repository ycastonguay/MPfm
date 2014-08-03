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

using NUnit.Framework;
using Sessions.Sound.AudioFiles;

namespace Sessions.Sound.Tests
{
    [TestFixture()]
    public class ConvertAudioTest
    {        
        [TestFixture]
        public class ToMSWithTimeStringTest : ConvertAudioTest
        {           
            static object[] TestCases =
            {
                new object[] { 0,       "0:00.000" },
                new object[] { 30000,   "0:30.000" },
                new object[] { 60000,   "1:00.000" },
                new object[] { 600000,  "10:00.000" },
                new object[] { 3600000, "1:00:00.000" }
            };

            [Test, TestCaseSource("TestCases")]
            public void ExecuteTestCases(int expectedValue, string timeString)
            {
                var value = ConvertAudio.ToMS(timeString);
                Assert.AreEqual(expectedValue, value);
            } 
        }

        [TestFixture]
        public class ToTimeStringWithMillisecondsTest : ConvertAudioTest
        {            
            static object[] TestCases =
            {
                new object[] { 0,       "0:00.000" },
                new object[] { 30000,   "0:30.000" },
                new object[] { 60000,   "1:00.000" },
                new object[] { 600000,  "10:00.000" },
                new object[] { 3600000, "1:00:00.000" }
            };

            [Test, TestCaseSource("TestCases")]
            public void ExecuteTestCases(int milliseconds, string expectedValue)
            {
                var value = ConvertAudio.ToTimeString(milliseconds);
                Assert.AreEqual(expectedValue, value);
            }
        }

    }
}
