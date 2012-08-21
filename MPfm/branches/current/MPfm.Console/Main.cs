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
using System.Timers;
using System.Threading;
using MPfm.Player;
using MPfm.MVP;
using Ninject;

namespace MPfm.Console
{
    class MainClass
    {
        public static MainApp App = null;

        public static void Main(string[] args)
        {
            Curses.initscr();
            Curses.move(2, 2);
            System.Console.WriteLine("Hello world!");
            Curses.refresh();
            Curses.getch();
            Curses.endwin();


            // Let Ninject create the application for us
            //App = Bootstrapper.GetKernel().Get<MainApp>();

//            System.Console.Clear();
//            System.Console.BackgroundColor = ConsoleColor.Blue;
//            System.Console.ForegroundColor = ConsoleColor.White;
//            System.Console.Write(ConsoleHelper.GetStringFillingScreenWidthWithSpaces(' '));
//            System.Console.SetCursorPosition(0, 0);
//            System.Console.Write(ConsoleHelper.GetCenteredString("MPfm: Music Player for Musicians - 0.6.0.1 ALPHA"));
//
//            System.Console.SetCursorPosition(0, 1);
//            System.Console.Write(ConsoleHelper.GetStringFillingScreenWidthWithSpaces('-'));
//            System.Console.SetCursorPosition(0, 1);
//            System.Console.Write(ConsoleHelper.GetCenteredString(" [Current song] ").Replace(" ", "-"));
//
//            System.Console.BackgroundColor = ConsoleColor.Black;
//            System.Console.ForegroundColor = ConsoleColor.Gray;
//            System.Console.WriteLine();
//            System.Console.WriteLine("Artist Name: ");
//            System.Console.WriteLine("Album Title: ");
//            System.Console.WriteLine("Song Title: ");
//            System.Console.WriteLine("Position/Length: [00:00.000 / 00:00.000]");
//
//
//            // Set cursor to last line
//            System.Console.SetCursorPosition(0, System.Console.WindowHeight - 1);
//            ConsoleHelper.Write("F1", ConsoleColor.Blue, ConsoleColor.White);
//            ConsoleHelper.Write(" Get Help         ");
//            ConsoleHelper.Write("F2", ConsoleColor.Blue, ConsoleColor.White);
//            ConsoleHelper.Write(" Open file        ");
//            ConsoleHelper.Write("F3", ConsoleColor.Blue, ConsoleColor.White);
//            ConsoleHelper.Write(" Play             ");
//            ConsoleHelper.Write("^X", ConsoleColor.Blue, ConsoleColor.White);
//            ConsoleHelper.Write(" Exit             ");
//            System.Console.SetCursorPosition(System.Console.WindowWidth - 1, System.Console.WindowHeight - 1);

            //System.Console.WriteLine("Usage: ");


//            for (int i = 0; i < 100; i++)   
//            {   
//                ConsoleHelper.ReportProgress("Processing...", i, 100);   
//                Thread.Sleep(100);   
//            }   

            //PlayerTest test = new PlayerTest();
            //test.ClearAndAddFiles();
            //test.player.Play();

            //System.Console.In.Read();

            //test.player.Stop();
        }
    }

    public class MainApp : IPlayerView
    {
        private IPlayerPresenter playerPresenter = null;

        public MainApp(IPlayerPresenter playerPresenter)
        {
            this.playerPresenter = playerPresenter;
        }

        public void Play()
        {
            playerPresenter.Play();
        }

        public void Pause()
        {
            playerPresenter.Pause();
        }

        public void Next()
        {
            playerPresenter.Next();
        }

        public void Previous()
        {
            playerPresenter.Previous();
        }


        #region IPlayerView implementation

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {

        }

        public void RefreshSongInformation(SongInformationEntity entity)
        {

        }

        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
        {

        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {

        }

        public void PlayerError(Exception ex)
        {

        }

        #endregion

    }
}