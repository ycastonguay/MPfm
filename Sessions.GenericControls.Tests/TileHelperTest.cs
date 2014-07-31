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
                new object[] { 0,   1, 0 },
                new object[] { 50,  1, 1 },
                new object[] { 100, 1, 2 },

                new object[] { 0,   2, 0 },
                new object[] { 50,  2, 1 },
                new object[] { 100, 2, 2 }
            };

            [SetUp]
            public void PrepareTest()
            {
                PrepareTests();
            }

            [Test, TestCaseSource("TestCases")]
            public void ExecuteTestCases(float x, float zoom, int expectedValue)
            {
                int value = TileHelper.GetTileIndexAt(x, zoom, TileSize);
                Assert.AreEqual(expectedValue, value);
            }  
        }

//        [TestFixture]
//        public class GetStartDirtyTileTest : TileHelperTest
//        {
//            static object[] TestCases =
//            {
//                new object[] { 0,   0,  1, 0 },
//                new object[] { 50,  0,  1, 1 },
//                new object[] { 100, 0,  1, 2 },
//
//                new object[] { 0,   50, 1, 1 },
//                new object[] { 50,  50, 1, 2 },
//                new object[] { 100, 50, 1, 3 },
//
//                new object[] { 0,   0,  2, 0 },
//                new object[] { 50,  0,  2, 1 },
//                new object[] { 100, 0,  2, 2 },
//
//                new object[] { 0,   50, 2, 1 },
//                new object[] { 50,  50, 2, 2 },
//                new object[] { 100, 50, 2, 3 }
//            };
//
//            [SetUp]
//            public void PrepareTest()
//            {
//                PrepareTests();
//            }
//
//            [Test, TestCaseSource("TestCases")]
//            public void ExecuteTestCases(float offsetX, float dirtyRectX, float zoom, int expectedValue)
//            {
//                int value = TileHelper.GetStartDirtyTile(offsetX, dirtyRectX, zoom, TileSize);
//                Assert.AreEqual(expectedValue, value);
//            }  
//        }
//
//        // Is this necessary to test when we know the only difference between this and GetTileAt is an addition?
//        [TestFixture]
//        public class GetEndDirtyTileTest : TileHelperTest
//        {
//            static object[] TestCases =
//            {
//                new object[] { 0,   0,  100, 1, 2 },
//                new object[] { 50,  0,  100, 1, 3 },
//                new object[] { 100, 0,  100, 1, 4 },
//
//                new object[] { 0,   0,  100, 2, 2 },
//                new object[] { 50,  0,  100, 2, 3 },
//                new object[] { 100, 0,  100, 2, 4 },
//
//                new object[] { 0,   0, 450, 2, 9 }
//            };
//
//            [SetUp]
//            public void PrepareTest()
//            {
//                PrepareTests();
//            }
//
//            [Test, TestCaseSource("TestCases")]
//            public void ExecuteTestCases(float offsetX, float dirtyRectX, float dirtyRectWidth, float zoom, int expectedValue)
//            {
//                int value = TileHelper.GetEndDirtyTile(offsetX, dirtyRectX, dirtyRectWidth, zoom, TileSize);
//                Assert.AreEqual(expectedValue, value);
//            }  
//        }
	}
}
