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
using System.Timers;
using System.Threading;
using System.Text;
using Mono;
using Mono.Terminal;

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
            return String.Format("{0,-" + System.Console.WindowWidth.ToString() + "}", String.Format("{0," + ((System.Console.WindowWidth + message.Length) / 2).ToString() +  "}", message));
        }

        public static string FillString(string message, int width)
        {
            if(width <= 0)
                return string.Empty;

            string str = string.Format("{0,-" + width.ToString() + "}", message);
            if (str.Length > width && width > 4)

                str = str.Substring(0, width - 4) + "(..)";
            return str;
        }

        public static string GetStringFillingScreenWidthWithSpaces(char character)
        {
            string spaces = string.Empty;
            for(int a = 0; a < System.Console.WindowWidth; a++)
                spaces += character;

            return spaces;
        }

        public static void PrintWindow(string title, int width, int height, int x, int y)
        {
            // Write background
            int line = y;
            Curses.attron(Curses.ColorPair(4));
            Curses.move(line, 0);
            Curses.addch(Curses.ACS_ULCORNER);
            Curses.addch(Curses.ACS_HLINE);
            Curses.addch(Curses.ACS_HLINE);
            string titleWithBrackets = "[ " + title + " ]";
            Curses.addstr(titleWithBrackets);
            for (int z = 0; z < width - 4 - titleWithBrackets.Length; z++)
                Curses.addch(Curses.ACS_HLINE);
            Curses.addch(Curses.ACS_URCORNER);
            line++;
            
            for (int z = line; z < y + height - 1; z++)
            {
                Curses.move(z, 0);
                Curses.addch(Curses.ACS_VLINE);
                Curses.addstr(new string(' ', width - 2));
                Curses.addch(Curses.ACS_VLINE);
                line++;
            }

            Curses.move(line, 0);
            Curses.addch(Curses.ACS_LLCORNER);
            Curses.addch(Curses.ACS_HLINE);
            Curses.addch(Curses.ACS_HLINE);
            for (int z = 0; z < width - 4; z++)
                Curses.addch(Curses.ACS_HLINE);
            Curses.addch(Curses.ACS_LRCORNER);
            Curses.attroff(Curses.ColorPair(4));
        }
    }
}
