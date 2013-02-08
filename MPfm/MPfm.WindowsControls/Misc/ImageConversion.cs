//
// ImageConversion.cs: Conversion routines for images.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// The Conversion class contains static functions that convert objects into different formats. 
    /// </summary>
    public class ImageConversion
    {
        /// <summary>
        /// The ByteArrayToBitmap static function converts a byte array into a Bitmap object.
        /// </summary>
        /// <param name="input">Byte array to convert</param>
        /// <returns>Byte array converted into a Bitmap object</returns>
        public static Bitmap ByteArrayToBitmap(byte[] input)
        {
            // Declare variables
            MemoryStream stream = null;

            try
            {
                // Create MemoryStream object
                stream = new MemoryStream();

                // Write the byte array into the MemoryStream object
                stream.Write(input, 0, input.Length);
            }
            finally
            {
                // Dispose stream
                stream.Dispose();
            }

            // Return a new Bitmap object from the MemoryStream
            return new Bitmap(stream);
        }

        /// <summary>
        /// The ImageToByteArray static function converts an Image object into a byte array.
        /// </summary>
        /// <param name="imageToConvert">Image object to convert</param>
        /// <param name="formatOfImage">Format of the Image object (jpg, gif, etc.)</param>
        /// <returns>Image object converted into a byte array</returns>
        public static byte[] ImageToByteArray(Image imageToConvert, ImageFormat formatOfImage)
        {
            // Declare variables
            byte[] array = null;
            MemoryStream ms = null;

            try
            {
                // Create a MemoryStream object
                ms = new MemoryStream();

                // Save the Image object into the MemoryStream object, specifying its format
                imageToConvert.Save(ms, formatOfImage);

                // Convert the MemoryStream object into a byte array
                array = ms.ToArray();
            }
            finally
            {
                ms.Dispose();
            }

            return array;
        }
    }
}