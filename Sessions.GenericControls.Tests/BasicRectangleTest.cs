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
using Sessions.GenericControls.Basics;

namespace Sessions.GenericControls.Tests
{
	[TestFixture()]
	public class BasicRectangleTest
	{
        static MergeTestRectangle[] MergeTestRectangles =
        { 
            new MergeTestRectangle() {
                A = new BasicRectangle(0, 100, 100, 200), 
                B = new BasicRectangle(50, 150, 300, 350), 
                Expected = new BasicRectangle(0, 100, 350, 400)
            },
            new MergeTestRectangle() {
                A = new BasicRectangle(250, 0, 200, 20), 
                B = new BasicRectangle(0, 0, 200, 20), 
                Expected = new BasicRectangle(0, 0, 450, 20)
            }
        };

		[Test]
		public void Center_ShouldBeCentered()
		{
            float size = 200;
            var rect = new BasicRectangle(0, 0, size, size);
            var point = rect.Center();

            Assert.AreEqual(point.X, size / 2f);
            Assert.AreEqual(point.Y, size / 2f);
		}

        [Test, TestCaseSource("MergeTestRectangles")]
        public void Merge_ShouldBeMerged(MergeTestRectangle testCase)
        {
            var rectMerged = BasicRectangle.Merge(testCase.A, testCase.B);

            Assert.AreEqual(rectMerged.X, testCase.Expected.X);
            Assert.AreEqual(rectMerged.Y, testCase.Expected.Y);
            Assert.AreEqual(rectMerged.Width, testCase.Expected.Width);
            Assert.AreEqual(rectMerged.Height, testCase.Expected.Height);
        }

        [Test]
        public void Equals_ShouldBeEqual()
        {
            var rectA = new BasicRectangle(0, 50, 100, 150);
            var rectB = new BasicRectangle(0, 50, 100, 150);

            Assert.IsTrue(rectA.Equals(rectB));
        }

        [Test]
        public void Equals_ShouldNotBeEqual()
        {
            var rectA = new BasicRectangle(0, 50, 100, 150);
            var rectB = new BasicRectangle(50, 100, 150, 200);

            Assert.IsFalse(rectA.Equals(rectB));
        }

        public class MergeTestRectangle
        {
            public BasicRectangle A { get; set; }
            public BasicRectangle B { get; set; }
            public BasicRectangle Expected { get; set; }
        }
	}
}
