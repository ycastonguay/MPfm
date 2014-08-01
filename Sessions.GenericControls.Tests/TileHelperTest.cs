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
using Sessions.GenericControls.Helpers;

namespace Sessions.GenericControls.Tests
{
	[TestFixture]
	public class TileHelperTest   
	{
        public const int TileSize = 50;

        public void PrepareTests()
        {
        }            

        [TestFixture]
        public class GetTileIndexAtTest : TileHelperTest
        {
            static object[] TestCases =
            {
                new object[] { 0,   1.0f, 1.0f, 0 },
                new object[] { 40,  1.0f, 1.0f, 0 },
                new object[] { 50,  1.0f, 1.0f, 1 },
                new object[] { 80,  1.0f, 1.0f, 1 },
                new object[] { 100, 1.0f, 1.0f, 2 },

                new object[] { 0,   1.5f, 1.0f, 0 },
                new object[] { 50,  1.5f, 1.0f, 0 },
                new object[] { 75,  1.5f, 1.0f, 1 },
                new object[] { 100, 1.5f, 1.0f, 1 },
                new object[] { 150, 1.5f, 1.0f, 2 },

                new object[] { 0,   2.0f, 1.0f, 0 },
                new object[] { 50,  2.0f, 1.0f, 0 },
                new object[] { 100, 2.0f, 1.0f, 1 },
                new object[] { 150, 2.0f, 1.0f, 1 },
                new object[] { 200, 2.0f, 1.0f, 2 },

                new object[] { 0,   2.0f, 2.0f, 0 },
                new object[] { 50,  2.0f, 2.0f, 1 },
                new object[] { 100, 2.0f, 2.0f, 2 }
            };

            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }

            [Test, TestCaseSource("TestCases")]
            public void ExecuteTestCases(float x, float zoom, float tileZoom, int expectedValue)
            {
                int value = TileHelper.GetTileIndexAt(x, zoom, tileZoom, TileSize);
                Assert.AreEqual(expectedValue, value);
            }  
        }
	}
}
