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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using Mono;
using Mono.Terminal;
using Sessions.MVP.Services.Interfaces;
using Sessions.Sound.AudioFiles;
using Sessions.MVP.Bootstrap;
using Sessions.Sound.BassNetWrapper;
using Sessions.Core;
using Sessions.MVP.Services;
using Sessions.Library;
using Sessions.Library.Services.Interfaces;
using Sessions.Library.Services;
using Sessions.Sound;
using Sessions.MVP.Config.Providers;

namespace Sessions.Console
{
    class MainClass
    {
        static IPlayerService playerService;
        static System.Timers.Timer timerRefreshPosition;
        static Window win;

        public static void Main(string[] args)
        {
            //PrintMSDOSChars(); return;
            bool continueRunning = true;
            
            // Check params and extract list of files to open
            string[] files = CheckParams(args);
            if (files == null)
                return;

            // Initialize ncurses
            // TODO: Add animation \|/-                        
            try
            {
                InitializeApp();
                InitializePlayer();

                Player.Player.CurrentPlayer.PlayFiles(files.ToList());
                Player.Player.CurrentPlayer.OnPlaylistIndexChanged += (data) => {
                    if(data.IsPlaybackStopped)
                    {
                        PrintSong(null);
                    }
                    else
                    {
                        PrintSong(data.AudioFileStarted);
                    }
                };
                PrintMain();
                PrintSong(Player.Player.CurrentPlayer.Playlist.Items[0].AudioFile);
            } 
            catch (Exception ex)
            {
                Curses.clear();
                Curses.addstr("An error occured: " + ex.Message + "\n" + ex.StackTrace + "\n");
                Curses.refresh();
                Curses.endwin();
                return;
            }

            // NOTE: This is weird, when the player is init in another thread, the playback is fine in debug mode. I
            //       If the player is init in main thread, the playback always stops after 1 second in debug.

//            // Initialize player in another thread
//            Task.Factory.StartNew(() => {
//                InitializePlayer();
//            }).ContinueWith((a) => {
//                
//                try
//                {
//                    PrintMain();
//                    Player.Player.CurrentPlayer.PlayFiles(files.ToList());
//                }
//                catch(Exception ex)
//                {
//                    Curses.clear();
//                    Curses.addstr("An error occured: " + ex.Message + "\n" + ex.StackTrace + "\n");
//                    Curses.refresh();
//                    //Curses.endwin();
//                    continueRunning = false; // TODO: Find a better way to exit app
//                }
//            });

            // Main loop
            while(continueRunning)
            {
                long pos;
                int ch = Curses.getch();
                switch(ch)
                {
                    case Curses.KeyF1: // Player screen
                        Player.Player.CurrentPlayer.Play();
                        break;
                    case Curses.KeyF2: // Playlist screen
                        Player.Player.CurrentPlayer.Pause();
                        break;
                    case Curses.KeyF3: // Effects screen
                        Player.Player.CurrentPlayer.Stop();
                        break;
                    case Curses.KeyF4: // 
                        break;
                    case Curses.KeyF5:
                        break;
                    case Curses.KeyF10:
                        ExitApp();
                        continueRunning = false;
                        break;
                    case Curses.KeyResize:
                        PrintMain();
                        if(Player.Player.CurrentPlayer.Playlist.CurrentItem != null)
                            PrintSong(Player.Player.CurrentPlayer.Playlist.CurrentItem.AudioFile);
                        break;
                    case Curses.KeyLeft:
                        pos = Player.Player.CurrentPlayer.GetPosition() - 50000;
                        if(pos < 0)
                            pos = 0;
                        Player.Player.CurrentPlayer.SetPosition(pos);
                        break;
                    case Curses.KeyRight:
                        pos = Player.Player.CurrentPlayer.GetPosition() + 50000;
                        if(pos > Player.Player.CurrentPlayer.Playlist.CurrentItem.LengthBytes)
                            pos = Player.Player.CurrentPlayer.Playlist.CurrentItem.LengthBytes - 50000;
                        Player.Player.CurrentPlayer.SetPosition(pos);
                        break;
                    case Curses.KeyUp:
                        Player.Player.CurrentPlayer.Previous();
                        break;
                    case Curses.KeyDown:
                        Player.Player.CurrentPlayer.Next();
                        break;
                }
            }
        }
        
        public static void InitializeApp()
        {
            // Build loading screen text
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Sessions: Music Player for Musicians (v0.7.0.0) © 2011-2012 Yanick Castonguay");
            sb.AppendLine("BASS audio library © 1999-2012 Un4seen Developments.");
            sb.AppendLine("BASS.NET audio library © 2005-2012 radio42.");
            sb.AppendLine();
            sb.Append("Loading...");

            win = Curses.initscr(); // Start curses mode
            Curses.raw();
            win.keypad(true);
            Curses.noecho();  // Dont output chars while getch
            Curses.cbreak();  // Take input chars one at a time
            Curses.start_color();
            Curses.use_default_colors();
            Curses.init_pair(1, Curses.COLOR_WHITE, Curses.COLOR_BLUE);
            Curses.init_pair(2, Curses.COLOR_WHITE, Curses.COLOR_BLACK);
            Curses.init_pair(3, Curses.COLOR_BLACK, Curses.COLOR_WHITE);
            Curses.init_pair(4, Curses.COLOR_YELLOW, Curses.COLOR_BLUE);
            Curses.addstr(sb.ToString());
            Curses.refresh();

            Base.Register(BassNetKey.Email, BassNetKey.RegistrationKey);

            var container = TinyIoC.TinyIoCContainer.Current;
            container.Register<ISyncDeviceSpecifications, SyncDeviceSpecifications>().AsSingleton();
            container.Register<ICloudService, DropboxCoreService>().AsSingleton();
            container.Register<IAppConfigProvider, XmlAppConfigProvider>().AsSingleton();
        }

        public static void ExitApp()
        {
            Curses.endwin();
        }

        public static void InitializePlayer()
        {           
            // Initialize app
            IInitializationService initService = Bootstrapper.GetContainer().Resolve<IInitializationService>();
            initService.Initialize();

            // Initialize player
            Device device = new Device(){
                DriverType = DriverType.DirectSound,
                Id = -1
            };
            playerService = Bootstrapper.GetContainer().Resolve<IPlayerService>();
            playerService.Initialize(device, 44100, 5000, 100);
            Player.Player.CurrentPlayer.Volume = 0.9f;
            //Player.Player.CurrentPlayer.OnPlaylistIndexChanged += HandlePlayerOnPlaylistIndexChanged;
            
            timerRefreshPosition = new System.Timers.Timer();
            timerRefreshPosition.Interval = 100;
            timerRefreshPosition.Elapsed += (sender, e) => {
                try
                {
                    long bytes = Player.Player.CurrentPlayer.GetPosition();
                    long samples = ConvertAudio.ToPCM(bytes, (uint)Player.Player.CurrentPlayer.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
                    long ms = ConvertAudio.ToMS(samples, (uint)Player.Player.CurrentPlayer.Playlist.CurrentItem.AudioFile.SampleRate);
                    string pos = Conversion.MillisecondsToTimeString((ulong)ms);

                    //int dataAvailable = Player.Player.CurrentPlayer.MixerChannel.GetMixerDataAvailable();
                    //int dataAvailable = Player.Player.CurrentPlayer.MixerChannel.GetDataAvailable();

                    Curses.move(6, 19);
                    Curses.attron(Curses.ColorPair(4));
                    //Curses.addstr(pos + " / " + Player.Player.CurrentPlayer.Playlist.CurrentItem.LengthString + " / " + dataAvailable.ToString());
                    Curses.attroff(Curses.ColorPair(4));
                    Curses.move(12, 0);
                    Curses.refresh();
                }
                catch(Exception ex)
                {
                    Curses.addstr("Error while fetching song position: " + ex.Message);
                }
            };
            timerRefreshPosition.Start();
        }
        
        public static string[] CheckParams(string[] args)
        {
            List<string> options = new List<string>();
            string file = null;
            
            // Validate parameters
            if (args.Length == 0)
            {
                PrintHelp();
                return null;           
            }
            
            foreach (string arg in args)
            {
                // Check if the parameter is an option (--)
                if (arg.Length >= 2 && arg.StartsWith("--"))
                    options.Add(arg);
                else
                    file = arg;
            }
                        
            // Split files
            string[] split = file.Split(new char[]{','});
            
            List<string> splitFinal = new List<string>();
            foreach (string thisSplit in split)
            {
                if(Directory.Exists(thisSplit))
                {
                    // Recursive
                    string[] directories = Directory.GetDirectories(thisSplit);
                    string[] files = Directory.GetFiles(thisSplit);
                    foreach(string thisFile in files)
                    {
                        string ext = Path.GetExtension(thisFile);

                        if(ext.ToUpper() == ".FLAC" ||
                           ext.ToUpper() == ".MP3")
                            splitFinal.Add(thisFile);
                    }
                }
                else if(File.Exists(thisSplit))
                {
                    splitFinal.Add(thisSplit.Replace("\"", ""));
                }
                else
                {
                    // File not found
                }
                
            }
            
            // Do the files exist?
            
            // Is this a folder?
            
            return splitFinal.ToArray();
        }
        
        public static void PrintHelp()
        {
            System.Console.WriteLine("Sessions: Music Player for Musicians (v0.7.0.0) © 2011-2012 Yanick Castonguay");            
            System.Console.WriteLine("BASS audio library © 1999-2012 Un4seen Developments.");
            System.Console.WriteLine("BASS.NET audio library © 2005-2012 radio42.");
            System.Console.WriteLine();            
            System.Console.WriteLine("Usage: Sessions.exe [OPTION]... [FILE]...");
            System.Console.WriteLine();
            System.Console.WriteLine("Parameters:");
            System.Console.WriteLine("  --open         Opens one or multiple audio files or playlist files.");
            System.Console.WriteLine("                 This is the default option if none are specified.");
            System.Console.WriteLine();
            System.Console.WriteLine("Examples for playing one, multiple, or all audio files from a directory:");
            System.Console.WriteLine("  Sessions.exe \"/home/username/Documents/Test.OGG\"");
            System.Console.WriteLine("  Sessions.exe \"/home/username/Documents/Test.OGG,/home/username/Documents/Test2.OGG\"");
            System.Console.WriteLine("  Sessions.exe \"/home/username/Documents/ArtistName/\"");
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

        public static void PrintMain()
        {
            Curses.clear();

            // Write title
            Curses.attron(Curses.ColorPair(3));
            Curses.addstr(ConsoleHelper.GetCenteredString("Sessions: Music Player for Musicians"));
            Curses.attroff(Curses.ColorPair(3));

            // Print windows
            ConsoleHelper.PrintWindow("Current Song", System.Console.WindowWidth, 7, 0, 1);
            ConsoleHelper.PrintWindow("Player Properties", System.Console.WindowWidth, System.Console.WindowHeight - 12, 0, 8);
            ConsoleHelper.PrintWindow("Player Controls", System.Console.WindowWidth, 3, 0, System.Console.WindowHeight - 4);

            // Print player properties
            Curses.attron(Curses.ColorPair(1));
            Curses.move(9, 2);           
            Curses.addstr("Volume:");
            Curses.move(10, 2);
            Curses.addstr("Time Shifting:");
            Curses.move(11, 2);           
            Curses.addstr("Pitch Shifting:");
            Curses.attroff(Curses.ColorPair(1));

            // Print player controls
            Curses.attron(Curses.ColorPair(1));
            Curses.move(System.Console.WindowHeight - 3, 2);           
            Curses.addstr("[SPACE] Play/Pause   [LEFT] Prev. song   [RIGHT] Next song   [UP] -1sec   [DOWN] +1sec");
            Curses.attroff(Curses.ColorPair(1));

//            // Write background
//            Curses.attron(Curses.ColorPair(4));
//            Curses.move(1, 0);
//            Curses.addch(Curses.ACS_ULCORNER);
//            Curses.addch(Curses.ACS_HLINE);
//            Curses.addch(Curses.ACS_HLINE);
//            string currentSong = "[ Current Song ]";
//            Curses.addstr(currentSong);
//            for (int z = 0; z < System.Console.WindowWidth - 4 - currentSong.Length; z++)
//                Curses.addch(Curses.ACS_HLINE);
//            Curses.addch(Curses.ACS_URCORNER);
//
//            for (int z = 2; z < 10; z++)
//            {
//                Curses.move(z, 0);
//                Curses.addch(Curses.ACS_VLINE);
//                Curses.addstr(new string(' ', System.Console.WindowWidth - 2));
//                Curses.addch(Curses.ACS_VLINE);
//            }
//
//            Curses.move(10, 0);
//            Curses.addch(Curses.ACS_LLCORNER);
//            Curses.addch(Curses.ACS_HLINE);
//            Curses.addch(Curses.ACS_HLINE);
//            for (int z = 0; z < System.Console.WindowWidth - 4; z++)
//                Curses.addch(Curses.ACS_HLINE);
//            Curses.addch(Curses.ACS_LRCORNER);
//            Curses.attroff(Curses.ColorPair(4));
            
            // Write current song properties
            Curses.attron(Curses.ColorPair(1));
            Curses.move(2, 2);
            Curses.addstr("Artist Name: ");
            Curses.move(3, 2);
            Curses.addstr("Album Title: ");
            Curses.move(4, 2);
            Curses.addstr("Song Title: ");
            Curses.move(5, 2);
            Curses.addstr("File Path: ");
            Curses.move(6, 2);
            Curses.addstr("Position/Length: ");
            Curses.attroff(Curses.ColorPair(1));

            // Write menu 
            Curses.move(System.Console.WindowHeight - 1, 0);
            Dictionary<string, string> dictOptions = new Dictionary<string, string>();
            dictOptions.Add("F1", "Player");
            dictOptions.Add("F2", "Playlist");
            dictOptions.Add("F3", "Effects");
            //dictOptions.Add("F4", "Previous");
            //dictOptions.Add("F5", "Next");
            dictOptions.Add("F10", "Exit");

            // Calculate item width
            double itemWidth = System.Console.WindowWidth / dictOptions.Count;
            itemWidth = Math.Round(itemWidth);
            foreach (KeyValuePair<string, string> kvp in dictOptions)
            {
                Curses.attron(Curses.ColorPair(2));// | Curses.A_DIM);
                Curses.addstr(kvp.Key);
                Curses.attroff(Curses.ColorPair(2));// | Curses.A_DIM);
                Curses.attron(Curses.ColorPair(3));
                Curses.addstr(ConsoleHelper.FillString(kvp.Value, (int)itemWidth - kvp.Key.Length));
                Curses.attroff(Curses.ColorPair(3));// | Curses.A_DIM);
            }

            Curses.refresh();
        }

        public static void PrintSong(AudioFile audioFile)
        {
            // Clear any info
            int availableWidth = System.Console.WindowWidth - 21;
            if(audioFile != null)
            {
                // TODO: Make sure the song position refresh doesn't conflict with printing song (i.e. Curses.move).
                uint trackCount = audioFile.TrackCount;
                if(trackCount == 0)
                    trackCount = (uint)Player.Player.CurrentPlayer.Playlist.Items.Count;
                Curses.attron(Curses.ColorPair(4));
                Curses.move(2, 19);
                Curses.addstr(ConsoleHelper.FillString(audioFile.ArtistName, availableWidth));
                Curses.move(3, 19);
                Curses.addstr(ConsoleHelper.FillString(audioFile.AlbumTitle, availableWidth));
                Curses.move(4, 19);
                Curses.addstr(ConsoleHelper.FillString("(" + audioFile.TrackNumber.ToString("00") + "/" + trackCount.ToString("00") + ") " + audioFile.Title, availableWidth));
                Curses.move(5, 19);
                Curses.addstr(ConsoleHelper.FillString(audioFile.FilePath, availableWidth));
                Curses.attroff(Curses.ColorPair(4));
            }

            Curses.move(12, 0);
        }
    }
}

