// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Reflection;
using System.Text;

namespace Sessions.Core
{
	/// <summary>
	/// The Conversion class contains static functions that convert objects into different formats. 
	/// </summary>
	public static class Conversion
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
			catch
			{
				throw;
			}
		}

        ///// <summary>
        ///// The ByteArrayToString static function converts a byte array into a string.
        ///// </summary>
        ///// <param name="input">Byte array to convert</param>
        ///// <returns>Byte array converted into a string</returns>
        //public static String ByteArrayToString(byte[] input)
        //{			
        //    try
        //    {
        //        // Create UnicodeEncoding class
        //        UnicodeEncoding enc = new UnicodeEncoding();
							
        //        // Return string
        //        return enc.GetString(input, );
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

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
#if WINDOWSSTORE
            var method = type.GetRuntimeMethod(
#else
            var method = type.GetMethod(
#endif
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

//        public static short HighWord(int dWord)
//        {
//            return (short)(dWord >> 16 & 65535);
//        }

//        public static int LowWord(int number)
//        { 
//            return number & 0x0000FFFF; 
//        }
//
//        public static int LowWord(int number, int newValue)
//        { 
//            return (int)((number & 0xFFFF0000) + (newValue & 0x0000FFFF)); 
//        }
//
//        public static int HighWord(int number)
//        { 
//            return (int)(number & 0xFFFF0000); 
//        }
//
//        public static int HighWord(int number, int newValue)
//        { 
//            return (number & 0x0000FFFF) + (newValue << 16); 
//        }

        public static short LowWord(int value)
        {
            return unchecked((short)(uint)value);
        }

        public static short HighWord(int value)
        {
            return unchecked((short)((uint)value >> 16));
        }

        public static char IndexToLetter(int index)
        {
            int indexModulo = index % 26;
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            return alpha[indexModulo];
        }
	}
}
