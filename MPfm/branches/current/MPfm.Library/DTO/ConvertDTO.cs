//
// ConvertDTO.cs: Converts DataTable and DataRow from and to DTOs.
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
using MPfm.Player;
using MPfm.Player.PlayerV4;
using MPfm.Sound;

namespace MPfm.Library
{
    /// <summary>
    /// This static class converts entities from Entity Framework into DTOs (Data
    /// Transfer Objects)
    /// </summary>
    public static class ConvertDTO
    {
        /// <summary>
        /// Converts a DataTable to a list of AudioFiles.
        /// Note: Not all the metadata will be present because not all fields
        /// are saved in the database. You need to call RefreshMetadata to refresh
        /// the metadata from the file.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of AudioFiles</returns>
        public static List<AudioFile> AudioFiles(DataTable table)
        { 
            // Create list
            List<AudioFile> dtos = new List<AudioFile>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Get file path and id
                string filePath = table.Rows[a]["FilePath"].ToString();
                Guid id = new Guid(table.Rows[a]["AudioFileId"].ToString());

                // Create audio file
                AudioFile dto = new AudioFile(filePath, id, false);

                // Assign properties (strings)                
                dto.Title = table.Rows[a]["Title"].ToString();                
                dto.ArtistName = table.Rows[a]["ArtistName"].ToString();
                dto.AlbumTitle = table.Rows[a]["AlbumTitle"].ToString();
                dto.Genre = table.Rows[a]["Genre"].ToString();                
                dto.Lyrics = table.Rows[a]["Lyrics"].ToString();
                dto.Length = table.Rows[a]["Length"].ToString();

                // Assign properties (integers)
                int playCount = 0;
                int.TryParse(table.Rows[a]["PlayCount"].ToString(), out playCount);
                dto.PlayCount = playCount;

                uint year = 1900;
                uint.TryParse(table.Rows[a]["Year"].ToString(), out year);
                dto.Year = year;

                uint discNumber = 0;
                uint.TryParse(table.Rows[a]["DiscNumber"].ToString(), out discNumber);
                dto.DiscNumber = discNumber;

                uint trackNumber = 0;
                uint.TryParse(table.Rows[a]["TrackNumber"].ToString(), out trackNumber);
                dto.TrackNumber = trackNumber;

                uint trackCount = 0;
                uint.TryParse(table.Rows[a]["TrackCount"].ToString(), out trackCount);
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
        /// Converts a DataTable to a list of EQPresets.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of EQPresets</returns>
        public static List<EQPreset> EQPresets(DataTable table)
        {
            // Create list
            List<EQPreset> dtos = new List<EQPreset>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Create DTO
                EQPreset dto = new EQPreset();

                // Assign properties (strings)
                dto.EQPresetId = new Guid(table.Rows[a]["EQPresetId"].ToString());
                dto.Name = table.Rows[a]["Name"].ToString();

                float gain0 = 0.0f;
                float.TryParse(table.Rows[a]["Gain0"].ToString(), out gain0);
                dto.Bands[0].Gain = gain0;

                //float gain77Hz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain77Hz"].ToString(), out gain77Hz);
                //dto.Gain77Hz = gain77Hz;

                //float gain110Hz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain110Hz"].ToString(), out gain110Hz);
                //dto.Gain110Hz = gain110Hz;

                //float gain156Hz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain156Hz"].ToString(), out gain156Hz);
                //dto.Gain156Hz = gain156Hz;

                //float gain220Hz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain220Hz"].ToString(), out gain220Hz);
                //dto.Gain220Hz = gain220Hz;

                //float gain311Hz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain311Hz"].ToString(), out gain311Hz);
                //dto.Gain311Hz = gain311Hz;

                //float gain440Hz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain440Hz"].ToString(), out gain440Hz);
                //dto.Gain440Hz = gain440Hz;

                //float gain622Hz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain622Hz"].ToString(), out gain622Hz);
                //dto.Gain622Hz = gain622Hz;

                //float gain880Hz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain880Hz"].ToString(), out gain880Hz);
                //dto.Gain880Hz = gain880Hz;

                //float gain1_2kHz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain1_2kHz"].ToString(), out gain1_2kHz);
                //dto.Gain1_2kHz = gain1_2kHz;

                //float gain1_8kHz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain1_8kHz"].ToString(), out gain1_8kHz);
                //dto.Gain1_8kHz = gain1_8kHz;

                //float gain2_5kHz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain2_5kHz"].ToString(), out gain2_5kHz);
                //dto.Gain2_5kHz = gain2_5kHz;

                //float gain3_5kHz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain3_5kHz"].ToString(), out gain3_5kHz);
                //dto.Gain3_5kHz = gain3_5kHz;

                //float gain5kHz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain5kHz"].ToString(), out gain5kHz);
                //dto.Gain5kHz = gain5kHz;

                //float gain7kHz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain7kHz"].ToString(), out gain7kHz);
                //dto.Gain7kHz = gain7kHz;

                //float gain10kHz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain10kHz"].ToString(), out gain10kHz);
                //dto.Gain10kHz = gain10kHz;

                //float gain14kHz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain14kHz"].ToString(), out gain14kHz);
                //dto.Gain14kHz = gain14kHz;

                //float gain20kHz = 0.0f;
                //float.TryParse(table.Rows[a]["Gain20kHz"].ToString(), out gain20kHz);
                //dto.Gain20kHz = gain20kHz;

                // Add DTO to list
                dtos.Add(dto);
            }

            // Return DTO
            return dtos;
        }

        /// <summary>
        /// Converts a DataTable to a list of Markers.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of Markers</returns>
        public static List<Marker> Markers(DataTable table)
        {
            // Create list
            List<Marker> dtos = new List<Marker>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Create DTO
                Marker dto = new Marker();

                // Assign properties (strings)
                dto.MarkerId = new Guid(table.Rows[a]["MarkerId"].ToString());
                dto.SongId = new Guid(table.Rows[a]["SongId"].ToString());
                dto.Name = table.Rows[a]["Name"].ToString();
                dto.Comments = table.Rows[a]["Comments"].ToString();

                int positionBytes = 0;
                int.TryParse(table.Rows[a]["PositionBytes"].ToString(), out positionBytes);
                dto.PositionBytes = positionBytes;

                int positionMS = 0;
                int.TryParse(table.Rows[a]["PositionMS"].ToString(), out positionMS);
                dto.PositionMS = positionMS;

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
            if (dto is AudioFile)
            {
                // Convert values
                ToAudioFileRow(ref row, (AudioFile)dto);
            }
            else if (dto is FolderDTO)
            {
                // Convert values
                ToFolderRow(ref row, (FolderDTO)dto);
            }
            else if (dto is EQPreset)
            {
                // Convert values
                ToEQPresetRow(ref row, (EQPreset)dto);
            }
        }

        /// <summary>
        /// Sets the values of a DataRow in a AudioFile DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="dto">AudioFile</param>
        public static void ToAudioFileRow(ref DataRow row, AudioFile dto)
        {
            // Set row data
            row["AudioFileId"] = dto.Id.ToString();
            row["Title"] = dto.Title;
            row["FilePath"] = dto.FilePath;
            row["ArtistName"] = dto.ArtistName;
            row["AlbumTitle"] = dto.AlbumTitle;
            row["Genre"] = dto.Genre;
            row["FileType"] = dto.FileType.ToString();
            row["Lyrics"] = dto.Lyrics;
            row["Length"] = dto.Length;

            AssignRowValue(ref row, "PlayCount", dto.PlayCount);
            AssignRowValue(ref row, "Year", dto.Year);
            AssignRowValue(ref row, "DiscNumber", dto.DiscNumber);
            AssignRowValue(ref row, "TrackNumber", dto.TrackNumber);
            AssignRowValue(ref row, "TrackCount", dto.TrackCount);
            AssignRowValue(ref row, "Rating", dto.Rating);
            AssignRowValue(ref row, "Tempo", dto.Tempo);
            AssignRowValue(ref row, "LastPlayed", dto.LastPlayed);
        }

        /// <summary>
        /// Sets the values of a DataRow in a Folder DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="dto">FolderDTO</param>
        public static void ToFolderRow(ref DataRow row, FolderDTO dto)
        {
            // Set row data
            row["FolderId"] = dto.FolderId.ToString();
            row["FolderPath"] = dto.FolderPath;

            AssignRowValue(ref row, "LastUpdated", dto.LastUpdated);
            AssignRowValue(ref row, "IsRecursive", dto.IsRecursive);
        }

        /// <summary>
        /// Sets the values of a DataRow in a EQPreset DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="dto">EQPreset</param>
        public static void ToEQPresetRow(ref DataRow row, EQPreset dto)
        {
            // Set row data
            row["EQPresetId"] = dto.EQPresetId.ToString();
            row["Name"] = dto.Name;

            AssignRowValue(ref row, "Gain0", dto.Bands[0].Gain);
            //AssignRowValue(ref row, "Gain77Hz", dto.Gain77Hz);
            //AssignRowValue(ref row, "Gain110Hz", dto.Gain110Hz);
            //AssignRowValue(ref row, "Gain156Hz", dto.Gain156Hz);
            //AssignRowValue(ref row, "Gain220Hz", dto.Gain220Hz);
            //AssignRowValue(ref row, "Gain311Hz", dto.Gain311Hz);
            //AssignRowValue(ref row, "Gain440Hz", dto.Gain440Hz);
            //AssignRowValue(ref row, "Gain622Hz", dto.Gain622Hz);
            //AssignRowValue(ref row, "Gain880Hz", dto.Gain880Hz);
            //AssignRowValue(ref row, "Gain1_2kHz", dto.Gain1_2kHz);
            //AssignRowValue(ref row, "Gain1_8kHz", dto.Gain1_8kHz);
            //AssignRowValue(ref row, "Gain2_5kHz", dto.Gain2_5kHz);
            //AssignRowValue(ref row, "Gain3_5kHz", dto.Gain3_5kHz);
            //AssignRowValue(ref row, "Gain5kHz", dto.Gain5kHz);
            //AssignRowValue(ref row, "Gain7kHz", dto.Gain7kHz);
            //AssignRowValue(ref row, "Gain10kHz", dto.Gain10kHz);
            //AssignRowValue(ref row, "Gain14kHz", dto.Gain14kHz);
            //AssignRowValue(ref row, "Gain20kHz", dto.Gain20kHz);
        }

        /// <summary>
        /// Sets the values of a DataRow in a Marker DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="marker">Marker</param>
        public static void ToMarkerRow(ref DataRow row, Marker dto)
        {
            // Set row data
            row["MarkerId"] = dto.MarkerId.ToString();
            row["SongId"] = dto.SongId.ToString();
            row["Name"] = dto.Name;
            row["Comments"] = dto.Comments;

            AssignRowValue(ref row, "PositionBytes", dto.PositionBytes);
            AssignRowValue(ref row, "PositionMS", dto.PositionMS);
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

        ///// <summary>
        ///// Converts a Playlist entity from EF to PlaylistDTO. Also converts the playlist
        ///// songs into a list of PlaylistSongDTO.
        ///// </summary>
        ///// <param name="playlist">Playlist (EF)</param>
        ///// <param name="playlistSongs">Playlist songs (EF)</param>
        ///// <returns>Playlist (DTO)</returns>
        //public static PlaylistDTO ConvertPlaylist(Playlist playlist, List<PlaylistSong> playlistSongs)
        //{
        //    // Create DTO
        //    PlaylistDTO dto = new PlaylistDTO();
        //    dto.PlaylistId = new Guid(playlist.PlaylistId);
        //    dto.PlaylistName = playlist.PlaylistName;
        //    dto.PlaylistType = PlaylistType.Custom;
        //    dto.PlaylistModified = false;
        //    dto.CurrentSong = null;

        //    // Convert playlist songs 
        //    if (playlistSongs != null)
        //    {
        //        // Loop through playlist songs
        //        foreach (PlaylistSong playlistSong in playlistSongs)
        //        {
        //            // Create DTO
        //            PlaylistSongDTO playlistSongDTO = new PlaylistSongDTO();
        //            playlistSongDTO.PlaylistSongId = new Guid(playlistSong.PlaylistSongId);

        //            //// Get song from database                    
        //            ////Song song = DataAccess.SelectSong(new Guid(playlistSong.SongId));
        //            //Song song = DataAccess.SelectSong(new Guid(playlistSong.SongId));
        //            //if (song != null)
        //            //{
        //            //    playlistSongDTO.Song = ConvertSong(song);
        //            //    dto.Songs.Add(playlistSongDTO);
        //            //}
        //        }
        //    }

        //    return dto;
        //}
    }
}
