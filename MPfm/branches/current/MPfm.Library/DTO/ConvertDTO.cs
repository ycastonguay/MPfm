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

        /// <summary>
        /// Converts a DataTable to a list of FolderDTOs.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of FolderDTO</returns>
        public static List<FolderDTO> Folders(DataTable table)
        {
            // Create list
            List<FolderDTO> dtos = new List<FolderDTO>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Create DTO
                FolderDTO dto = new FolderDTO();

                // Assign properties (strings)
                dto.FolderId = new Guid(table.Rows[a]["FolderId"].ToString());
                dto.FolderPath = table.Rows[a]["FolderPath"].ToString();

                bool isRecursive = false;
                bool.TryParse(table.Rows[a]["IsRecursive"].ToString(), out isRecursive);
                dto.IsRecursive = isRecursive;

                // Assign properties (datetimes)
                DateTime lastUpdated = DateTime.MinValue;
                DateTime.TryParse(table.Rows[a]["LastUpdated"].ToString(), out lastUpdated);
                if (lastUpdated == DateTime.MinValue)
                {
                    dto.LastUpdated = null;
                }
                else
                {
                    dto.LastUpdated = lastUpdated;
                }

                // Add DTO to list
                dtos.Add(dto);
            }

            // Return DTO
            return dtos;
        }

        /// <summary>
        /// Converts a DataTable to a list of EqualizerDTOs.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of EqualizerDTO</returns>
        public static List<EqualizerDTO> Equalizers(DataTable table)
        {
            // Create list
            List<EqualizerDTO> dtos = new List<EqualizerDTO>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Create DTO
                EqualizerDTO dto = new EqualizerDTO();

                // Assign properties (strings)
                dto.EqualizerId = new Guid(table.Rows[a]["EqualizerId"].ToString());
                dto.Name = table.Rows[a]["Name"].ToString();

                float gain55Hz = 0.0f;
                float.TryParse(table.Rows[a]["Gain55Hz"].ToString(), out gain55Hz);
                dto.Gain55Hz = gain55Hz;

                float gain77Hz = 0.0f;
                float.TryParse(table.Rows[a]["Gain77Hz"].ToString(), out gain77Hz);
                dto.Gain77Hz = gain77Hz;

                float gain110Hz = 0.0f;
                float.TryParse(table.Rows[a]["Gain110Hz"].ToString(), out gain110Hz);
                dto.Gain110Hz = gain110Hz;

                float gain156Hz = 0.0f;
                float.TryParse(table.Rows[a]["Gain156Hz"].ToString(), out gain156Hz);
                dto.Gain156Hz = gain156Hz;

                float gain220Hz = 0.0f;
                float.TryParse(table.Rows[a]["Gain220Hz"].ToString(), out gain220Hz);
                dto.Gain220Hz = gain220Hz;

                float gain311Hz = 0.0f;
                float.TryParse(table.Rows[a]["Gain311Hz"].ToString(), out gain311Hz);
                dto.Gain311Hz = gain311Hz;

                float gain440Hz = 0.0f;
                float.TryParse(table.Rows[a]["Gain440Hz"].ToString(), out gain440Hz);
                dto.Gain440Hz = gain440Hz;

                float gain622Hz = 0.0f;
                float.TryParse(table.Rows[a]["Gain622Hz"].ToString(), out gain622Hz);
                dto.Gain622Hz = gain622Hz;

                float gain880Hz = 0.0f;
                float.TryParse(table.Rows[a]["Gain880Hz"].ToString(), out gain880Hz);
                dto.Gain880Hz = gain880Hz;

                float gain1_2kHz = 0.0f;
                float.TryParse(table.Rows[a]["Gain1_2kHz"].ToString(), out gain1_2kHz);
                dto.Gain1_2kHz = gain1_2kHz;

                float gain1_8kHz = 0.0f;
                float.TryParse(table.Rows[a]["Gain1_8kHz"].ToString(), out gain1_8kHz);
                dto.Gain1_8kHz = gain1_8kHz;

                float gain2_5kHz = 0.0f;
                float.TryParse(table.Rows[a]["Gain2_5kHz"].ToString(), out gain2_5kHz);
                dto.Gain2_5kHz = gain2_5kHz;

                float gain3_5kHz = 0.0f;
                float.TryParse(table.Rows[a]["Gain3_5kHz"].ToString(), out gain3_5kHz);
                dto.Gain3_5kHz = gain3_5kHz;

                float gain5kHz = 0.0f;
                float.TryParse(table.Rows[a]["Gain5kHz"].ToString(), out gain5kHz);
                dto.Gain5kHz = gain5kHz;

                float gain7kHz = 0.0f;
                float.TryParse(table.Rows[a]["Gain7kHz"].ToString(), out gain7kHz);
                dto.Gain7kHz = gain7kHz;

                float gain10kHz = 0.0f;
                float.TryParse(table.Rows[a]["Gain10kHz"].ToString(), out gain10kHz);
                dto.Gain10kHz = gain10kHz;

                float gain14kHz = 0.0f;
                float.TryParse(table.Rows[a]["Gain14kHz"].ToString(), out gain14kHz);
                dto.Gain14kHz = gain14kHz;

                float gain20kHz = 0.0f;
                float.TryParse(table.Rows[a]["Gain20kHz"].ToString(), out gain20kHz);
                dto.Gain20kHz = gain20kHz;

                // Add DTO to list
                dtos.Add(dto);
            }

            // Return DTO
            return dtos;
        }

        /// <summary>
        /// Sets the values from a DTO into a DataRow.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="dto">DTO</param>
        public static void ToRow(ref DataRow row, object dto)
        {
            // Check what type of DTO
            if (dto is SongDTO)
            {
                // Convert values
                ToSongRow(ref row, (SongDTO)dto);
            }
            else if (dto is FolderDTO)
            {
                // Convert values
                ToFolderRow(ref row, (FolderDTO)dto);
            }
            else if (dto is EqualizerDTO)
            {
                // Convert values
                ToEqualizerRow(ref row, (EqualizerDTO)dto);
            }
        }

        /// <summary>
        /// Sets the values of a DataRow in a Song DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="song">SongDTO</param>
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

        /// <summary>
        /// Sets the values of a DataRow in a Folder DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="folder">FolderDTO</param>
        public static void ToFolderRow(ref DataRow row, FolderDTO folder)
        {
            // Set row data
            row["FolderId"] = folder.FolderId.ToString();
            row["FolderPath"] = folder.FolderPath;

            AssignRowValue(ref row, "LastUpdated", folder.LastUpdated);
            AssignRowValue(ref row, "IsRecursive", folder.IsRecursive);
        }

        /// <summary>
        /// Sets the values of a DataRow in a Equalizer DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="equalizer">EqualizerDTO</param>
        public static void ToEqualizerRow(ref DataRow row, EqualizerDTO equalizer)
        {
            // Set row data
            row["EqualizerId"] = equalizer.EqualizerId.ToString();
            row["Name"] = equalizer.Name;

            AssignRowValue(ref row, "Gain55Hz", equalizer.Gain55Hz);
            AssignRowValue(ref row, "Gain77Hz", equalizer.Gain77Hz);
            AssignRowValue(ref row, "Gain110Hz", equalizer.Gain110Hz);
            AssignRowValue(ref row, "Gain156Hz", equalizer.Gain156Hz);
            AssignRowValue(ref row, "Gain220Hz", equalizer.Gain220Hz);
            AssignRowValue(ref row, "Gain311Hz", equalizer.Gain311Hz);
            AssignRowValue(ref row, "Gain440Hz", equalizer.Gain440Hz);
            AssignRowValue(ref row, "Gain622Hz", equalizer.Gain622Hz);
            AssignRowValue(ref row, "Gain880Hz", equalizer.Gain880Hz);
            AssignRowValue(ref row, "Gain1_2kHz", equalizer.Gain1_2kHz);
            AssignRowValue(ref row, "Gain1_8kHz", equalizer.Gain1_8kHz);
            AssignRowValue(ref row, "Gain2_5kHz", equalizer.Gain2_5kHz);
            AssignRowValue(ref row, "Gain3_5kHz", equalizer.Gain3_5kHz);
            AssignRowValue(ref row, "Gain5kHz", equalizer.Gain5kHz);
            AssignRowValue(ref row, "Gain7kHz", equalizer.Gain7kHz);
            AssignRowValue(ref row, "Gain10kHz", equalizer.Gain10kHz);
            AssignRowValue(ref row, "Gain14kHz", equalizer.Gain14kHz);
            AssignRowValue(ref row, "Gain20kHz", equalizer.Gain20kHz);
        }

        /// <summary>
        /// Sets the value of a field in a DataRow.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="field">Field name</param>
        /// <param name="value">Value to set</param>
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
