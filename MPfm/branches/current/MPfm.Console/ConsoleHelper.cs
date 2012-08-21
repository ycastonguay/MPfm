//
// ConsoleHelper.cs: This is an helper class for reading and writing to the console.
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
using System.Collections.Generic;
using System.Timers;
using MPfm.Player;
using System.Threading;
using System.Text;

namespace MPfm.Console
{
    /// <summary>
    /// This is an helper class for reading and writing to the console.
    /// </summary>
    public static class ConsoleHelper
    {
        public static void ReportProgress(string message, int index, int count)
        {   
            int percent = (100 * (index + 1)) / count;   
            System.Console.Write("\r{0}{1}% complete", message, percent);   
            if (index == count-1)   
            {   
                System.Console.WriteLine(Environment.NewLine);   
            }   
        }

        public static void ReportProgress(string message)
        {
            System.Console.Write("\r{0}", message); 
        }

        public static void Log(string log)
        {
            System.Console.Out.Write(log);
        }

        public static void Write(string message)
        {
            System.Console.Write(message);
        }

        public static void Write(string message, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            // Get previous color
            ConsoleColor previousBackgroundColor = System.Console.BackgroundColor;
            ConsoleColor previousForegroundColor = System.Console.ForegroundColor;

            // Set new color
            System.Console.BackgroundColor = backgroundColor;
            System.Console.ForegroundColor = foregroundColor;

            // Print message
            System.Console.Write(message);

            // Set previous color
            System.Console.BackgroundColor = previousBackgroundColor;
            System.Console.ForegroundColor = previousForegroundColor;
        }

        public static string GetCenteredString(string message)
        {
            return String.Format("\r{0," + (System.Console.WindowWidth + message.Length) / 2 + "}", message);
        }

        public static string GetStringFillingScreenWidthWithSpaces(char character)
        {
            string spaces = string.Empty;
            for(int a = 0; a < System.Console.WindowWidth; a++)
                spaces += character;

            return spaces;
        }
    }
}
