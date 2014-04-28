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

namespace MPfm.Core
{
    /// <summary>
    /// Class holding a byte array buffer with a FIFO queue.
    /// </summary>
    public class ByteArrayQueue
    {
        private readonly object _locker = new object();
        public int BufferReadPosition { get; private set; }
        public int BufferWritePosition { get; private set; }
        public int BufferDataLength { get; private set; }
        public int BufferLength { get; private set; }
        public byte[] BufferData { get; private set; }

        public float BufferFillPercentage
        {
            get
            {
                return (BufferLength - (BufferLength - BufferDataLength) / BufferLength) * 100;
            }
        }
        
        public ByteArrayQueue(int bufferLength)
        {
            this.BufferReadPosition = 0;
            this.BufferWritePosition = 0;
            this.BufferDataLength = 0;
            this.BufferLength = bufferLength;
            this.BufferData = new byte[bufferLength];
        }
        
        public void Enqueue(byte[] bytes)
        {
            // Check if bytes fit the buffer length
            if (bytes.Length > BufferLength)
                throw new ArgumentOutOfRangeException("bytes", "The Enqueue method cannot enqueue more bytes than the buffer length.");
            
            // Check if enough space 
            if (bytes.Length > (BufferLength - BufferDataLength))
                throw new ArgumentOutOfRangeException("bytes", "The buffer does not have enough free space to fit the byte array.");
            
            // Check for overspill
            int overspillLength = 0;
            if (bytes.Length > (BufferLength - BufferWritePosition))
                overspillLength = BufferWritePosition + bytes.Length - BufferLength;
            
            lock (_locker)
            {
                // Write first block
                int blockLength = bytes.Length - overspillLength;
                Array.Copy(bytes, 0, BufferData, BufferWritePosition, blockLength);
                
                // Write overspill
                if (overspillLength > 0)
                    Array.Copy(bytes, blockLength, BufferData, 0, overspillLength);
                
                // Calculate data length
                BufferDataLength += bytes.Length;
                
                // Calculate buffer write position
                if(overspillLength > 0)
                {
                    BufferWritePosition = overspillLength;
                }
                else
                {
                    // Check if adding the block would spill over
                    if(BufferWritePosition + blockLength > BufferLength)
                        BufferWritePosition = blockLength - (BufferLength - BufferWritePosition);
                    else
                        BufferWritePosition += blockLength;
                }
            }
        }
        
        public byte[] Dequeue(int length)
        {
            // Check if bytes fit the buffer length
            if (length > BufferLength)
                throw new ArgumentOutOfRangeException("bytes", "The Dequeue method cannot dequeue more bytes than the buffer length.");
            
            // Check if enough data is available
            if (length > BufferDataLength)
                return new byte[0];
                //throw new ArgumentOutOfRangeException("bytes", "The buffer does not have enough data to fit the byte array.");
            
            // Create array
            byte[] bytes = new byte[length];
            
            // Check for overspill
            int overspillLength = 0;
            if (length > BufferLength - BufferReadPosition)
                overspillLength = length - (BufferLength - BufferReadPosition);
            
            lock (_locker)
            {
                // Write first block
                int blockLength = length - overspillLength;
                Array.Copy(BufferData, BufferReadPosition, bytes, 0, blockLength);
                
                // Write overspill
                if(overspillLength > 0)
                    Array.Copy(BufferData, 0, bytes, blockLength, overspillLength);

                // Calculate data length
                BufferDataLength -= length;
                
                // Calculate buffer write position
                if(overspillLength > 0)
                {
                    BufferReadPosition = overspillLength;
                }
                else
                {
                    // Check if adding the block would spill over
                    if(BufferReadPosition + blockLength > BufferLength)
                        BufferReadPosition = blockLength - (BufferLength - BufferReadPosition);
                    else
                        BufferReadPosition += blockLength;
                }

                //Console.WriteLine(DateTime.Now.ToLongTimeString() + " readPos: " + BufferReadPosition.ToString() + " overspill: " + overspillLength.ToString());
            }
            
            return bytes;
        }
    }
}
