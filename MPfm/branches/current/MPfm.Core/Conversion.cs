//
// Conversion.cs: Conversion routines
//
// Copyright © 2011 Yanick Castonguay
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace MPfm.Core
{
	/// <summary>
	/// The Conversion class contains static functions that convert objects into different formats. 
	/// </summary>
	public class Conversion
	{
        /// <summary>
        /// The StringToByteArray static function converts a string into a byte array.
        /// </summary>
        /// <param name="input">String to convert</param>
        /// <returns>String converted into byte array </returns>
		public static byte[] StringToByteArray(String input)
		{
			try
			{         
                // Create UnicodeEncoding class
				UnicodeEncoding enc = new UnicodeEncoding();							
				
                // Return byte array
				return enc.GetBytes(input);
			}
			catch(Exception ex)
			{
                throw ex;
			}
			
			return null;
		}

        /// <summary>
        /// The ByteArrayToString static function converts a byte array into a string.
        /// </summary>
        /// <param name="input">Byte array to convert</param>
        /// <returns>Byte array converted into a string</returns>
		public static String ByteArrayToString(byte[] input)
		{			
			try
			{
                // Create UnicodeEncoding class
				UnicodeEncoding enc = new UnicodeEncoding();
							
                // Return string
				return enc.GetString(input);
			}
			catch(Exception ex)
			{
                throw ex;
			}

			return "";
		}

        /// <summary>
        /// The ByteArrayToBitmap static function converts a byte array into a Bitmap object.
        /// </summary>
        /// <param name="input">Byte array to convert</param>
        /// <returns>Byte array converted into a Bitmap object</returns>
        public static Bitmap ByteArrayToBitmap(byte[] input)
        {
            try
            {
                // Create MemoryStream object
                MemoryStream stream = new MemoryStream();

                // Write the byte array into the MemoryStream object
                stream.Write(input, 0, input.Length);

                // Return a new Bitmap object from the MemoryStream
                return new Bitmap(stream);                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// The ImageToByteArray static function converts an Image object into a byte array.
        /// </summary>
        /// <param name="imageToConvert">Image object to convert</param>
        /// <param name="formatOfImage">Format of the Image object (jpg, gif, etc.)</param>
        /// <returns>Image object converted into a byte array</returns>
        public static byte[] ImageToByteArray(Image imageToConvert, ImageFormat formatOfImage)
        {
            byte[] array;

            try
            {
                // Create a MemoryStream object
                MemoryStream ms = new MemoryStream();                

                // Save the Image object into the MemoryStream object, specifying its format
                imageToConvert.Save(ms, formatOfImage);

                // Convert the MemoryStream object into a byte array
                array = ms.ToArray();                
            }
            catch (Exception) 
            { 
                throw; 
            }

            return array;
        }

        /// <summary>
        /// The IntToByteArray static function converts a 32-bit integer into a byte array.
        /// </summary>
        /// <param name="input">Integer to convert</param>
        /// <returns>Integer converted into a byte array</returns>
		public static byte[] IntToByteArray(int input)
		{
            // Create byte array (4 dimensions for 32-bit)
			byte[] b = new byte[4]; 

            // Assign the values
			b[0] = (byte)(input); 
			b[1] = (byte)(input >> 8); 
			b[2] = (byte)(input >> 16); 
			b[3] = (byte)(input >> 24); 

            // Return byte array
			return b;
		}

        /// <summary>
        /// The ByteArrayToInt static function converts a 4 dimension byte array into a 32-bit integer.
        /// </summary>
        /// <param name="b">Byte array to convert</param>
        /// <returns>Byte array converted into an integer</returns>
		public static int ByteArrayToInt(byte[] b)
		{
			return (int)(b[0] | b[1] << 8 | b[2] << 16 | b[3] << 24); 
		}

        /// <summary>
        /// The MillisecondsToTimeString static functions converts a time in milliseconds into a string 
        /// displaying the time in the following format: 0:00.000
        /// </summary>
        /// <param name="time">Milliseconds value to convert</param>
        /// <returns>A string displaying the time (0:00.000)</returns>
        public static string MillisecondsToTimeString(ulong time)
        {
            ulong pos = time;
            int minutes = 0;
            int seconds = 0;
            int milliseconds = 0;
            string timeString = "";

            // Minutes
            if (pos > 60000)
            {
                minutes = (int) pos /  60000;
                pos %= 60000;
            }

            // Seconds
            if (pos > 1000)
            {
                seconds = (int) pos / 1000;
                pos %= 1000;
            }

            // Milliseconds
            milliseconds = Convert.ToInt32(pos);

            // Convert to string and return
            timeString = minutes.ToString("0") + ":" + seconds.ToString("00") + "." + milliseconds.ToString("000"); ;
            return timeString;
        }

        /// <summary>
        /// Converts a TimeSpan to a time string (0:00.000 format).
        /// </summary>
        /// <param name="timeSpan">TimeSpan</param>
        /// <returns>Time string</returns>
        public static string TimeSpanToTimeString(TimeSpan timeSpan)
        {
            return timeSpan.Minutes.ToString("0") + ":" + timeSpan.Seconds.ToString("00") + "." + timeSpan.Milliseconds.ToString("000");
        }

        /// <summary>
        /// Converts a DateTime structure into a UNIX date format (number of milliseconds since 1970).
        /// </summary>
        /// <param name="date">DateTime structure to convert</param>
        /// <returns>DateTime in UNIX date format</returns>
        public static Int64 DateTimeToMillisecondsSince1970(DateTime date)
        {            
            DateTime dateReference = new DateTime(1970, 1, 1).ToUniversalTime();

            TimeSpan span = date.ToUniversalTime().Subtract(dateReference);
            return Convert.ToInt64(span.TotalMilliseconds) + date.Millisecond;
        }

        /// <summary>
        /// Converts milliseconds to a tempo (beats per minute)
        /// </summary>
        /// <param name="milliseconds">Value in milliseconds</param>
        /// <returns>Value in tempo (BPM)</returns>
        public static double MillisecondsToTempo(int milliseconds)
        {
            return ((double)60 / milliseconds) * (double)1000;                        
            
        }

        /// <summary>
        /// Converts a tempo (beats per minute) into milliseconds.
        /// </summary>
        /// <param name="tempo">Value in BPM (tempo)</param>
        /// <returns>Value in milliseconds</returns>
        public static double TempoToMilliseconds(double tempo)
        {
            return (double)60000 / tempo;
        }

        /// <summary>
        /// Returns the fractionary number of minutes from absolute milliseconds.
        /// </summary>
        /// <param name="absolutems">Value in absolute milliseconds</param>
        /// <returns>Number of minutes (in double format)</returns>
        public static double GetMinutes(int absolutems)
        {
            return (double)absolutems / 1000 / 60;
        }

        /// <summary>
        /// Returns the fractionary number of milliseconds from absolute milliseconds.
        /// </summary>
        /// <param name="absolutems">Value in absolute milliseconds</param>
        /// <returns>Number of minutes (in double format)</returns>
        public static double GetMilliseconds(int absolutems)
        {
            return (double)absolutems / 1000 % 100;
        }

        /// <summary>
        /// Returns the fractionary number of seconds from absolute milliseconds.
        /// </summary>
        /// <param name="absolutems">Value in absolute milliseconds</param>
        /// <returns>Number of minutes (in double format)</returns>
        public static double GetSeconds(int absolutems)
        {
            return (double)absolutems / 1000 % 60;
        }

        /// <summary>
        /// Returns the number of seconds (with double precision) since 1970 of a DateTime.
        /// Also called Unix timestamp.
        /// </summary>
        /// <param name="dateTime">DateTime to convert</param>
        /// <returns>Unix timestamp</returns>
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Parses a string and converts the value into a specific type.
        /// Inspired from a post on Stack Overflow: 
        /// http://stackoverflow.com/questions/569249/methodinfo-invoke-with-out-parameter
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="input">Input string</param>
        /// <returns>Value (default value if failed)</returns>
        public static T TryParse<T>(string input)
        {
            // Get the type of generic
            var type = typeof(T);

            // Get the default value
            var temp = default(T);

            // Set the list of parameters for TryParse
            object[] parameters = new object[]
            {
                input,
                temp
            };

            // Get the TryParse method info
            var method = type.GetMethod(
                "TryParse",                
                new[]
            {
                typeof (string),
                Type.GetType(string.Format("{0}&", type.FullName))
            });

            // Invoke the TryParse method
            bool success = (bool)method.Invoke(null, parameters);

            // If successful...
            if (success)
            {
                // Return the value
                return (T)parameters[1];
            }

            // Return the default value
            return default(T);
        }
	}

    [StructLayout(LayoutKind.Explicit)]
    public struct UnionArray
    {
        [FieldOffset(0)]
        public Byte[] Bytes;

        [FieldOffset(0)]
        public float[] Floats;
    }
}
