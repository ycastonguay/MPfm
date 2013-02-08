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
using MPfm.Player;
using System.Threading;

namespace MPfm.Console
{
    /// <summary>
    /// This class is for testing the player using a console.
    /// </summary>
    public class PlayerTest
    {
        public MPfm.Player.Player player = null;
        private List<string> audioFilePaths = null;
        private System.Timers.Timer timer = null;

        public PlayerTest()
        {
            player = new MPfm.Player.Player();
            player.OnPlaylistIndexChanged += HandleOnPlaylistIndexChanged;

            audioFilePaths = new List<string>(){
                "/Users/animal/Music/Flac/Washed Out/Within and Without (2011)/01. Eyes Be Closed.flac",
                "/Users/animal/Music/Flac/Washed Out/Within and Without (2011)/02. Echoes.flac",
                "/Users/animal/Music/Flac/Washed Out/Within and Without (2011)/03. Amor Fati.flac",
                "/Users/animal/Music/Flac/Washed Out/Within and Without (2011)/04. Soft.flac",
                "/Users/animal/Music/Flac/Washed Out/Within and Without (2011)/05. Far Away.flac",
                "/Users/animal/Music/Flac/Washed Out/Within and Without (2011)/06. Before.flac",
                "/Users/animal/Music/Flac/Washed Out/Within and Without (2011)/07. You and I.flac",
                "/Users/animal/Music/Flac/Washed Out/Within and Without (2011)/08. Within and Without.flac",
                "/Users/animal/Music/Flac/Washed Out/Within and Without (2011)/09. A Dedication.flac"
            };

            timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.Elapsed += HandleElapsed;
            timer.Start();
        }

        public void HandleOnPlaylistIndexChanged(PlayerPlaylistIndexChangedData data)
        {
            if (data.AudioFileEnded != null)
            {
                ConsoleHelper.Log("A song has just ended: " + data.AudioFileEnded.ArtistName + " | " + data.AudioFileEnded.AlbumTitle + " | " + data.AudioFileEnded.Title + "\n");
            }

            if (data.AudioFileStarted != null)
            {
                ConsoleHelper.Log("A song has just started: " + data.AudioFileStarted.ArtistName + " | " + data.AudioFileStarted.AlbumTitle + " | " + data.AudioFileStarted.Title + "\n");
            }
        }

        public void HandleElapsed(object sender, ElapsedEventArgs e)
        {
            string positionString = "Error";
            try
            {
                long position = player.GetPosition();
                positionString = position.ToString();

            }
            catch
            {
           
            }

            System.Console.SetCursorPosition(0, 5);
            System.Console.Write("\rPosition/Length: [{0}]", positionString);

        }

        public void ClearAndAddFiles()
        {
            player.Playlist.Clear();
            player.Playlist.AddItems(audioFilePaths);
        }
    }
}
