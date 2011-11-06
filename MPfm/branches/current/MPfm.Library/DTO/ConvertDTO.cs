//
// ConvertDTO.cs: Converts entities from Entity Framework into DTOs.
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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Objects;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using MPfm.Library.Data;

namespace MPfm.Library
{
    /// <summary>
    /// This static class converts entities from Entity Framework into DTOs (Data
    /// Transfer Objects)
    /// </summary>
    public static class ConvertDTO
    {
        /// <summary>
        /// Converts a DataTable to a list of SongDTOs.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of SongDTO</returns>
        public static List<SongDTO> Songs(DataTable table)
        {
            // Create list
            List<SongDTO> dtos = new List<SongDTO>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Create DTO
                SongDTO dto = new SongDTO();

                // Assign properties (strings)
                dto.SongId = new Guid(table.Rows[a]["SongId"].ToString());
                dto.Title = table.Rows[a]["Title"].ToString();
                dto.FilePath = table.Rows[a]["FilePath"].ToString();
                dto.ArtistName = table.Rows[a]["ArtistName"].ToString();
                dto.AlbumTitle = table.Rows[a]["AlbumTitle"].ToString();
                dto.Genre = table.Rows[a]["Genre"].ToString();
                dto.SoundFormat = table.Rows[a]["SoundFormat"].ToString();
                dto.Lyrics = table.Rows[a]["Lyrics"].ToString();
                dto.Time = table.Rows[a]["Time"].ToString();

                // Assign properties (integers)
                int playCount = 0;
                int.TryParse(table.Rows[a]["PlayCount"].ToString(), out playCount);
                dto.PlayCount = playCount;

                int year = 1900;
                int.TryParse(table.Rows[a]["Year"].ToString(), out year);
                dto.Year = year;

                int discNumber = 0;
                int.TryParse(table.Rows[a]["DiscNumber"].ToString(), out discNumber);
                dto.DiscNumber = discNumber;

                int trackNumber = 0;
                int.TryParse(table.Rows[a]["TrackNumber"].ToString(), out trackNumber);
                dto.TrackNumber = trackNumber;

                int trackCount = 0;
                int.TryParse(table.Rows[a]["TrackCount"].ToString(), out trackCount);
                dto.TrackCount = trackCount;

                int rating = 0;
                int.TryParse(table.Rows[a]["Rating"].ToString(), out rating);
                dto.Rating = rating;

                int tempo = 0;
                int.TryParse(table.Rows[a]["Tempo"].ToString(), out tempo);
                dto.Tempo = tempo;

                // Assign properties (datetimes)
                DateTime lastPlayed = DateTime.MinValue;
                DateTime.TryParse(table.Rows[a]["LastPlayed"].ToString(), out lastPlayed);
                if(lastPlayed == DateTime.MinValue)
                {
                    dto.LastPlayed = null;
                }
                else
                {
                    dto.LastPlayed = lastPlayed;
                }                                

                // Add DTO to list
                dtos.Add(dto);

            }

            // Return DTO
            return dtos;
        }

        public static void ToRow(ref DataRow row, object dto)
        {
            if (dto is SongDTO)
            {
                ToSongRow(ref row, (SongDTO)dto);
            }
        }

        public static void ToSongRow(ref DataRow row, SongDTO song)
        {
            // Set row data
            row["SongId"] = song.SongId.ToString();
            row["Title"] = song.Title;
            row["FilePath"] = song.FilePath;
            row["ArtistName"] = song.ArtistName;
            row["AlbumTitle"] = song.AlbumTitle;
            row["Genre"] = song.Genre;
            row["SoundFormat"] = song.SoundFormat;
            row["Lyrics"] = song.Lyrics;
            row["Time"] = song.Time;

            AssignRowValue(ref row, "PlayCount", song.PlayCount);
            AssignRowValue(ref row, "Year", song.Year);
            AssignRowValue(ref row, "DiscNumber", song.DiscNumber);
            AssignRowValue(ref row, "TrackNumber", song.TrackNumber);
            AssignRowValue(ref row, "TrackCount", song.TrackCount);
            AssignRowValue(ref row, "Rating", song.Rating);
            AssignRowValue(ref row, "Tempo", song.Tempo);
            AssignRowValue(ref row, "LastPlayed", song.LastPlayed);
        }

        public static void AssignRowValue(ref DataRow row, string field, object value)
        {
            if (value == null)
            {
                row[field] = DBNull.Value;
            }
            else
            {
                row[field] = value;
            }
        }

        /// <summary>
        /// Converts a Song entity from EF to SongDTO.
        /// </summary>
        /// <param name="song">Song (EF)</param>
        /// <returns>Song (DTO)</returns>
        public static SongDTO ConvertSong(Song song)
        {
            SongDTO dto = new SongDTO();

            dto.SongId = new Guid(song.SongId);
            dto.Title = song.Title;
            dto.FilePath = song.FilePath;
            dto.ArtistName = song.ArtistName;
            dto.AlbumTitle = song.AlbumTitle;
            dto.PlayCount = song.PlayCount;
            dto.LastPlayed = song.LastPlayed;
            int year = 0;
            int.TryParse(song.Year, out year);
            dto.Year = year;
            dto.TrackNumber = song.TrackNumber;
            dto.DiscNumber = song.DiscNumber;
            dto.TrackCount = song.TrackCount;
            dto.Rating = song.Rating;
            dto.Time = song.Time;
            dto.Genre = song.Genre;
            dto.Tempo = song.Tempo;
            dto.Lyrics = song.Lyrics;
            dto.SoundFormat = song.SoundFormat;

            return dto;
        }

        /// <summary>
        /// Converts a list of Song entities from EF to a list of SongDTO.
        /// </summary>
        /// <param name="songs">List of songs (EF)</param>
        /// <returns>List of songs (DTO)</returns>
        public static List<SongDTO> ConvertSongs(List<Song> songs)
        {
            List<SongDTO> dtos = new List<SongDTO>();

            foreach (Song song in songs)
            {
                dtos.Add(ConvertSong(song));
            }

            return dtos;
        }

        /// <summary>
        /// Converts a Playlist entity from EF to PlaylistDTO. Also converts the playlist
        /// songs into a list of PlaylistSongDTO.
        /// </summary>
        /// <param name="playlist">Playlist (EF)</param>
        /// <param name="playlistSongs">Playlist songs (EF)</param>
        /// <returns>Playlist (DTO)</returns>
        public static PlaylistDTO ConvertPlaylist(Playlist playlist, List<PlaylistSong> playlistSongs)
        {
            // Create DTO
            PlaylistDTO dto = new PlaylistDTO();
            dto.PlaylistId = new Guid(playlist.PlaylistId);
            dto.PlaylistName = playlist.PlaylistName;
            dto.PlaylistType = PlaylistType.Custom;
            dto.PlaylistModified = false;
            dto.CurrentSong = null;

            // Convert playlist songs 
            if (playlistSongs != null)
            {
                // Loop through playlist songs
                foreach (PlaylistSong playlistSong in playlistSongs)
                {
                    // Create DTO
                    PlaylistSongDTO playlistSongDTO = new PlaylistSongDTO();
                    playlistSongDTO.PlaylistSongId = new Guid(playlistSong.PlaylistSongId);

                    //// Get song from database                    
                    ////Song song = DataAccess.SelectSong(new Guid(playlistSong.SongId));
                    //Song song = DataAccess.SelectSong(new Guid(playlistSong.SongId));
                    //if (song != null)
                    //{
                    //    playlistSongDTO.Song = ConvertSong(song);
                    //    dto.Songs.Add(playlistSongDTO);
                    //}
                }
            }

            return dto;
        }
    }
}
