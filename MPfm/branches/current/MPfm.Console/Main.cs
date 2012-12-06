//
// Main.cs: This is the main file for the console version of MPfm.
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using MPfm.MVP;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using Ninject;

namespace MPfm.Console
{
    class MainClass
    {
        static IPlayerService playerService;
        static System.Timers.Timer timerRefreshPosition;

        public static void Main(string[] args)
        {
            //PrintMSDOSChars(); return;
            
            // Check params and extract list of files to open
            string[] files = CheckParams(args);
            if(files == null)
                return;
            
            Initialize();
            
            playerService.Player.PlayFiles(files.ToList());
            
            System.Console.Clear();
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write(ConsoleHelper.GetStringFillingScreenWidthWithSpaces(' '));
            System.Console.SetCursorPosition(0, 0);
            System.Console.Write(ConsoleHelper.GetCenteredString("MPfm: Music Player for Musicians"));

            //System.Console.SetCursorPosition(0, 1);
            //System.Console.Write(ConsoleHelper.GetStringFillingScreenWidthWithSpaces('-'));
            //System.Console.SetCursorPosition(0, 1);
            //System.Console.Write(ConsoleHelper.GetCenteredString(" [Current song] ").Replace(" ", "-"));

            System.Console.BackgroundColor = ConsoleColor.Black;
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine();
            System.Console.WriteLine("Artist Name: ");
            System.Console.WriteLine("Album Title: ");
            System.Console.WriteLine("Song Title: ");
            System.Console.WriteLine("Position/Length: [00:00.000 / 00:00.000]");


            // Set cursor to last line
            System.Console.SetCursorPosition(0, System.Console.WindowHeight - 2);
            ConsoleHelper.Write("F1", ConsoleColor.Blue, ConsoleColor.White);
            ConsoleHelper.Write(" Play             ");
            ConsoleHelper.Write("^X", ConsoleColor.Blue, ConsoleColor.White);
            ConsoleHelper.Write(" Exit             ");
            
            while(true)
            {
                // Read input
                ConsoleKeyInfo key = System.Console.ReadKey();
                
                // Check for CTRL-X
                if(key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.X)
                    break; // Exit app

            }
        }
        
        public static void Initialize()
        {
//            Curses.initscr();
//            Curses.move(2, 2);
//            System.Console.WriteLine("Hello world!");
//            Curses.refresh();
//            Curses.getch();
//            Curses.endwin();
            
            
            System.Console.Clear();
            System.Console.WriteLine("MPfm: Music Player for Musicians (v0.7.0.0) © 2011-2012 Yanick Castonguay");            
            System.Console.WriteLine("BASS audio library © 1999-2012 Un4seen Developments.");
            System.Console.WriteLine("BASS.NET audio library © 2005-2012 radio42.");
            System.Console.WriteLine();
            System.Console.Write("Loading...");
            Debug.WriteLine("DebugWriteLine");
            Trace.WriteLine("TraceWriteLine");
            
            // Initialize app
            IInitializationService initService = Bootstrapper.GetKernel().Get<IInitializationService>();
            initService.Initialize();

            // Initialize player
            Device device = new Device(){
                DriverType = DriverType.DirectSound,
                Id = -1
            };
            playerService = Bootstrapper.GetKernel().Get<IPlayerService>();
            playerService.Initialize(device, 44100, 5000, 100);
            //playerService.Player.OnPlaylistIndexChanged += HandlePlayerOnPlaylistIndexChanged;
            
            timerRefreshPosition = new System.Timers.Timer();
            timerRefreshPosition.Interval = 100;
            timerRefreshPosition.Elapsed += (sender, e) => {
                long position = playerService.Player.GetPosition();
                System.Console.SetCursorPosition(20, 4);
                System.Console.Out.Write(position.ToString());
            };
            timerRefreshPosition.Start();
        }
        
        public static string[] CheckParams(string[] args)
        {
            List<string> options = new List<string>();
            string file;
            
            // Validate parameters
            if(args.Length == 0)               
            {
                PrintHelp();
                return null;           
            }
            
            foreach(string arg in args)
            {
                // Check if the parameter is an option (--)
                if(arg.Length >= 2 && arg.StartsWith("--"))
                    options.Add(arg);
                else
                    file = arg;
            }
                        
            // Split files
            string[] split = file.Split(new char[]{','});
            
            List<string> splitFinal = new List<string>();
            foreach(string thisSplit in split)
                splitFinal.Add(thisSplit.Replace("\"", ""));
            
            // Do the files exist?
            
            // Is this a folder?
            
            return splitFinal.ToArray();
        }
        
        public static void PrintHelp()
        {
            System.Console.WriteLine("MPfm: Music Player for Musicians (v0.7.0.0) © 2011-2012 Yanick Castonguay");            
            System.Console.WriteLine("BASS audio library © 1999-2012 Un4seen Developments.");
            System.Console.WriteLine("BASS.NET audio library © 2005-2012 radio42.");
            System.Console.WriteLine();            
            System.Console.WriteLine("Usage: mpfm.exe [OPTION]... [FILE]...");
            System.Console.WriteLine();
            System.Console.WriteLine("Parameters:");
            System.Console.WriteLine("  --open         Opens one or multiple audio files or playlist files.");
            System.Console.WriteLine("                 This is the default option if none are specified.");
            System.Console.WriteLine();
            System.Console.WriteLine("Examples for playing one, multiple, or all audio files from a directory:");
            System.Console.WriteLine("  mpfm.exe \"/home/username/Documents/Test.OGG\"");
            System.Console.WriteLine("  mpfm.exe \"/home/username/Documents/Test.OGG,/home/username/Documents/Test2.OGG\"");
            System.Console.WriteLine("  mpfm.exe \"/home/username/Documents/ArtistName/\"");
        }
        
        public static void PrintMSDOSChars()
        {
            // Set the window size and title
            System.Console.Title = "Code Page 437: MS-DOS ASCII Characters";
            
            for (byte b = 0; b < byte.MaxValue; b++)
            {
                char c = Encoding.GetEncoding(437).GetChars(new byte[] {b})[0];
                switch (b)
                {
                    case 8: // Backspace
                    case 9: // Tab
                    case 10: // Line feed
                    case 13: // Carriage return
                        c = '.';
                        break;
                }
                
                System.Console.Write("{0:000} {1}   ", b, c);
                
                // 7 is a beep -- Console.Beep() also works
                if (b == 7) System.Console.Write(" ");
                
                if ((b + 1) % 8 == 0)
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
            
        }
    }

}