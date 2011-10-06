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
            dto.Year = song.Year;
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

                    // Get song from database                    
                    Song song = DataAccess.SelectSong(new Guid(playlistSong.SongId));
                    if (song != null)
                    {
                        playlistSongDTO.Song = ConvertSong(song);
                        dto.Songs.Add(playlistSongDTO);
                    }
                }
            }

            return dto;
        }
    }
}
