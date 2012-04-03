//
// ConvertLibrary.cs: Converts DataTable and DataRow from and to objects.
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
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using MPfm.Player;
using MPfm.Sound;

namespace MPfm.Library
{
    /// <summary>
    /// This static class converts objects from different libraries into DataTables and back into objects.    
    /// </summary>
    public static class ConvertLibrary
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

                int sampleRate = 0;
                int.TryParse(table.Rows[a]["SampleRate"].ToString(), out sampleRate);
                dto.SampleRate = sampleRate;

                int bitrate = 0;
                int.TryParse(table.Rows[a]["Bitrate"].ToString(), out bitrate);
                dto.Bitrate = bitrate;

                // Assign properties (datetimes)
                DateTime lastPlayed = DateTime.MinValue;
                DateTime.TryParse(table.Rows[a]["LastPlayedDateTime"].ToString(), out lastPlayed);
                if(lastPlayed == DateTime.MinValue)
                {
                    dto.LastPlayed = null;
                }
                else
                {
                    dto.LastPlayed = lastPlayed;
                }                               
 
                // Load metadata
                // Extract file extension
                string ext = Path.GetExtension(dto.FilePath).ToUpper().Replace(".", "");
                AudioFileFormat format = AudioFileFormat.Unknown;
                Enum.TryParse<AudioFileFormat>(ext, out format);
                dto.FileType = format;

                // Add DTO to list
                dtos.Add(dto);
            }

            // Return DTO
            return dtos;
        }

        /// <summary>
        /// Converts a DataTable to a list of PlaylistFiles.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of PlaylistFiles</returns>
        public static List<PlaylistFile> PlaylistFiles(DataTable table)
        {
            // Create list
            List<PlaylistFile> dtos = new List<PlaylistFile>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Set file path
                PlaylistFile dto = new PlaylistFile();
                dto.FilePath = table.Rows[a]["FilePath"].ToString();

                // Set format
                PlaylistFileFormat format = PlaylistFileFormat.Unknown;
                Enum.TryParse<PlaylistFileFormat>(table.Rows[a]["FilePath"].ToString(), out format);
                dto.Format = format;

                // Add DTO to list
                dtos.Add(dto);
            }

            return dtos;
        }

        /// <summary>
        /// Converts a DataTable to a list of Folders.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of Folders</returns>
        public static List<Folder> Folders(DataTable table)
        {
            // Create list
            List<Folder> dtos = new List<Folder>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Create DTO
                Folder dto = new Folder();

                // Assign properties (strings)
                dto.FolderId = new Guid(table.Rows[a]["FolderId"].ToString());
                dto.FolderPath = table.Rows[a]["FolderPath"].ToString();

                // Bug fix: for some reason, SQLite returns 0 or 1 instead of true/false for binary fields.                
                int isRecursive = 0;
                int.TryParse(table.Rows[a]["IsRecursive"].ToString(), out isRecursive);
                dto.IsRecursive = (isRecursive == 1) ? true : false;

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

                // Loop through 18 bands
                for (int b = 0; b < 18; b++)
                {
                    float gain = 0.0f;
                    float.TryParse(table.Rows[a]["Gain" + b.ToString()].ToString(), out gain);
                    dto.Bands[b].Gain = gain;
                }

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
                dto.AudioFileId = new Guid(table.Rows[a]["AudioFileId"].ToString());
                dto.Name = table.Rows[a]["Name"].ToString();
                dto.Comments = table.Rows[a]["Comments"].ToString();
                dto.Position = table.Rows[a]["Position"].ToString();

                int positionBytes = 0;
                int.TryParse(table.Rows[a]["PositionBytes"].ToString(), out positionBytes);
                dto.PositionBytes = positionBytes;

                uint positionSamples = 0;
                uint.TryParse(table.Rows[a]["PositionSamples"].ToString(), out positionSamples);
                dto.PositionSamples = positionSamples;

                // Add DTO to list
                dtos.Add(dto);
            }

            // Return DTO
            return dtos;
        }

        /// <summary>
        /// Converts a DataTable to a list of Loops.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of Loops</returns>
        public static List<Loop> Loops(DataTable table)
        {
            // Create list
            List<Loop> dtos = new List<Loop>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Create DTO
                Loop dto = new Loop();

                // Assign properties (strings)
                dto.LoopId = new Guid(table.Rows[a]["LoopId"].ToString());
                dto.AudioFileId = new Guid(table.Rows[a]["AudioFileId"].ToString());
                dto.Name = table.Rows[a]["Name"].ToString();
                dto.Comments = table.Rows[a]["Comments"].ToString();
                dto.Length = table.Rows[a]["Length"].ToString();
                dto.StartPosition = table.Rows[a]["StartPosition"].ToString();
                dto.EndPosition = table.Rows[a]["EndPosition"].ToString();

                uint lengthBytes = 0;
                uint.TryParse(table.Rows[a]["LengthBytes"].ToString(), out lengthBytes);
                dto.LengthBytes = lengthBytes;

                uint lengthSamples = 0;
                uint.TryParse(table.Rows[a]["LengthSamples"].ToString(), out lengthSamples);
                dto.LengthSamples = lengthSamples;

                uint startPositionBytes = 0;
                uint.TryParse(table.Rows[a]["StartPositionBytes"].ToString(), out startPositionBytes);
                dto.StartPositionBytes = startPositionBytes;

                uint startPositionSamples = 0;
                uint.TryParse(table.Rows[a]["StartPositionSamples"].ToString(), out startPositionSamples);
                dto.StartPositionSamples = startPositionSamples;

                uint endPositionBytes = 0;
                uint.TryParse(table.Rows[a]["EndPositionBytes"].ToString(), out endPositionBytes);
                dto.EndPositionBytes = endPositionBytes;

                uint endPositionSamples = 0;
                uint.TryParse(table.Rows[a]["EndPositionSamples"].ToString(), out endPositionSamples);
                dto.EndPositionSamples = endPositionSamples;

                // Add DTO to list
                dtos.Add(dto);
            }

            // Return DTO
            return dtos;
        }

        /// <summary>
        /// Converts a DataTable to a list of Settings.
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns>List of Settings</returns>
        public static List<Setting> Settings(DataTable table)
        {
            // Create list
            List<Setting> dtos = new List<Setting>();

            // Loop through rows
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Create DTO
                Setting dto = new Setting();

                // Assign properties (strings)
                dto.SettingId = new Guid(table.Rows[a]["SettingId"].ToString());
                dto.SettingName = table.Rows[a]["SettingName"].ToString();
                dto.SettingValue = table.Rows[a]["SettingValue"].ToString();

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
            else if (dto is PlaylistFile)
            {
                // Convert values
                ToPlaylistFileRow(ref row, (PlaylistFile)dto);
            }
            else if (dto is Folder)
            {
                // Convert values
                ToFolderRow(ref row, (Folder)dto);
            }
            else if (dto is EQPreset)
            {
                // Convert values
                ToEQPresetRow(ref row, (EQPreset)dto);
            }
            else if (dto is Marker)
            {
                // Convert values
                ToMarkerRow(ref row, (Marker)dto);
            }
            else if (dto is Loop)
            {
                // Convert values
                ToLoopRow(ref row, (Loop)dto);
            }
            else if (dto is Setting)
            {
                // Convert values
                ToSettingRow(ref row, (Setting)dto);
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
            AssignRowValue(ref row, "SampleRate", dto.SampleRate);
            AssignRowValue(ref row, "Bitrate", dto.Bitrate);
            AssignRowValue(ref row, "LastPlayedDateTime", dto.LastPlayed);
        }

        /// <summary>
        /// Sets the values of a DataRow in a PlaylistFile DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="dto">PlaylistFile</param>
        public static void ToPlaylistFileRow(ref DataRow row, PlaylistFile dto)
        {
            // Set row data
            row["FilePath"] = dto.FilePath;
            row["Format"] = dto.Format.ToString();
        }

        /// <summary>
        /// Sets the values of a DataRow in a Folder DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="dto">Folder</param>
        public static void ToFolderRow(ref DataRow row, Folder dto)
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

            // Loop through bands
            for (int a = 0; a < 18; a++)
            {
                // Assign row value
                AssignRowValue(ref row, "Gain" + a.ToString(), dto.Bands[a].Gain);
            }                
        }

        /// <summary>
        /// Sets the values of a DataRow in a Marker DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="dto">Marker</param>
        public static void ToMarkerRow(ref DataRow row, Marker dto)
        {
            // Set row data
            row["MarkerId"] = dto.MarkerId.ToString();
            row["AudioFileId"] = dto.AudioFileId.ToString();
            row["Name"] = dto.Name;
            row["Comments"] = dto.Comments;
            row["Position"] = dto.Position;

            AssignRowValue(ref row, "PositionBytes", dto.PositionBytes);
            AssignRowValue(ref row, "PositionSamples", dto.PositionSamples);
        }

        /// <summary>
        /// Sets the values of a DataRow in a Loop DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="dto">Loop</param>
        public static void ToLoopRow(ref DataRow row, Loop dto)
        {
            // Set row data
            row["LoopId"] = dto.LoopId.ToString();
            row["AudioFileId"] = dto.AudioFileId.ToString();
            row["Name"] = dto.Name;
            row["Comments"] = dto.Comments;
            row["Length"] = dto.Length;
            row["StartPosition"] = dto.StartPosition;
            row["EndPosition"] = dto.EndPosition;

            AssignRowValue(ref row, "LengthBytes", dto.LengthBytes);
            AssignRowValue(ref row, "LengthSamples", dto.LengthSamples);
            AssignRowValue(ref row, "StartPositionBytes", dto.StartPositionBytes);
            AssignRowValue(ref row, "StartPositionSamples", dto.StartPositionSamples);
            AssignRowValue(ref row, "EndPositionBytes", dto.EndPositionBytes);
            AssignRowValue(ref row, "EndPositionSamples", dto.EndPositionSamples);
        }

        /// <summary>
        /// Sets the values of a DataRow in a Settings DataTable.
        /// </summary>
        /// <param name="row">DataRow to set</param>
        /// <param name="dto">Setting</param>
        public static void ToSettingRow(ref DataRow row, Setting dto)
        {
            // Set row data
            row["SettingId"] = dto.SettingId.ToString();
            row["SettingName"] = dto.SettingName;
            row["SettingValue"] = dto.SettingValue;
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
    }
}
