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
using NUnit.Framework;
using Sessions.Core;

namespace Sessions.Core.Tests
{
    [TestFixture()]
    public class TestByteArrayQueue
    {
        private const int QueueCapacity = 10000;
        private readonly Random _random;
        private ByteArrayQueue _queue;

        public TestByteArrayQueue()
        {
            _queue = new ByteArrayQueue(QueueCapacity);
            _random = new Random();            
        }

        private void InitializeQueue()
        {
            _queue.Clear();
        }

        [Test]
        public void CannotEnqueueLargerThanBufferLength()
        {
            InitializeQueue();

            // Enqueue a block larger than queue capacity
            var data = new byte[QueueCapacity + 1];
            _random.NextBytes(data);

            Assert.Throws<ArgumentOutOfRangeException>(() => _queue.Enqueue(data));
        }

        [Test]
        public void CannotEnqueueNotEnoughFreeSpace()
        {
            InitializeQueue();

            // Enqueue first block of data, filling almost all the space
            var data = new byte[QueueCapacity - 10];
            _random.NextBytes(data);
            _queue.Enqueue(data);

            // Enqueue a new block that is larger than the free space left
            var data2 = new byte[QueueCapacity];
            _random.NextBytes(data2);

            Assert.Throws<ArgumentOutOfRangeException>(() => _queue.Enqueue(data2));
        }

        [Test]
        public void CannotDequeueLargerThanAvailable()
        {
            InitializeQueue();

            // Enqueue a block that fills half of the available space
            const int chunkLength = QueueCapacity/2;
            var data = new byte[chunkLength];
            _random.NextBytes(data);
            _queue.Enqueue(data);

            Assert.Throws<ArgumentOutOfRangeException>(() => _queue.Dequeue(QueueCapacity));
        }

        [Test]
        public void TestEnqueueThenDequeueWithOverspill()
        {
            InitializeQueue();

            // Use a chunk that doesn't fit perfectly with the capacity
            const int chunkLength = 777;

            // Loop several times to fill the buffer multiple times its capacity
            // to test overspill (byte array that spills from the end of the array up to the start)
            for (int a = 0; a < 10; a++)
            {
                var listData = new List<byte[]>();

                // Generate random data and enqueue 
                for (int i = 0; i < 10; i++)
                {
                    var data = new byte[chunkLength];
                    _random.NextBytes(data);
                    listData.Add(data);

                    _queue.Enqueue(data);
                }

                // Dequeue data and validate that the chunks are the same
                for (int i = 0; i < 10; i++)
                {
                    var data = _queue.Dequeue(chunkLength);
                    Assert.AreEqual(data, listData[i]);
                }                
            }
        }
    }
}
