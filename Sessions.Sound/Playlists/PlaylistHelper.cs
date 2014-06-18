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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Sessions.Sound.AudioFiles;

namespace Sessions.Sound.Playlists
{
    /// <summary>
    /// This static class contains useful methods for loading and saving playlists.
    /// </summary>
    public static class PlaylistHelper
    {
        /// <summary>
        /// Loads a playlist (from any of the following formats: M3U, M3U8, PLS and XSPF).
        /// Note: long playlists may take a while to load using this method!
        /// </summary>
        /// <param name="filePath">Playlist file path</param>
        /// <returns>Playlist</returns>
        public static Playlist LoadPlaylist(string filePath)
        {
            var playlist = new Playlist();
            var files = new List<string>();

            if (filePath.ToUpper().Contains(".M3U"))
            {
                playlist.Format = PlaylistFileFormat.M3U;
                files = PlaylistHelper.LoadM3UPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".M3U8"))
            {
                playlist.Format = PlaylistFileFormat.M3U8;
                files = PlaylistHelper.LoadM3UPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".PLS"))
            {
                playlist.Format = PlaylistFileFormat.PLS;
                files = PlaylistHelper.LoadPLSPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".XSPF"))
            {
                playlist.Format = PlaylistFileFormat.XSPF;
                files = PlaylistHelper.LoadXSPFPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".ASX"))
            {
                playlist.Format = PlaylistFileFormat.ASX;
            }

            if (files == null || files.Count == 0)
                throw new Exception("Error: The playlist is empty or does not contain any valid audio file paths!");

            playlist.Clear();
            playlist.FilePath = filePath;
            playlist.AddItems(files);
            playlist.First();
            return playlist;
        }


        /// <summary>
        /// Returns the list of audio files from any of the following formats: M3U, M3U8, PLS and XSPF.
        /// </summary>
        /// <param name="filePath">Playlist file path</param>
        /// <returns>List of audio file paths</returns>
        public static List<string> GetPlaylistFileNames(string filePath)
        {
            var files = new List<string>();

            // Determine the playlist format
            if (filePath.ToUpper().Contains(".M3U"))
                files = PlaylistHelper.LoadM3UPlaylist(filePath);
            else if (filePath.ToUpper().Contains(".M3U8"))
                files = PlaylistHelper.LoadM3UPlaylist(filePath);
            else if (filePath.ToUpper().Contains(".PLS"))
                files = PlaylistHelper.LoadPLSPlaylist(filePath);
            else if (filePath.ToUpper().Contains(".XSPF"))
                files = PlaylistHelper.LoadXSPFPlaylist(filePath);
            else if (filePath.ToUpper().Contains(".ASX"))
                throw new NotImplementedException();

            return files;
        }

        #region M3U/M3U8
        
        /// <summary>
        /// Returns the list of audio files to play from a M3U or M3U8 playlist.
        /// </summary>
        /// <param name="filePath">Playlist file path</param>        
        /// <returns>List of audio file paths</returns>
        public static List<string> LoadM3UPlaylist(string filePath)
        {
            StreamReader reader = null;
            var files = new List<string>();
            string playlistDirectory = Path.GetDirectoryName(filePath);

            #if WINDOWSSTORE
            #else

            try
            {
                string line = string.Empty;
                reader = new StreamReader(filePath);                
                while ((line = reader.ReadLine()) != null)
                {
                    // Make sure the line isn't empty
                    if (!String.IsNullOrEmpty(line))
                    {
                        // Make sure the line isn't a comment (begins by the pound # sign)
                        if (line[0] != '#')
                        {
                            string directoryPath = string.Empty;

                            // Check for a media file with absolute path
                            if (File.Exists(line))
                                files.Add(line);
                            // Check for a media file with relative path (without backslash)
                            else if (File.Exists(playlistDirectory + line))
                                files.Add(playlistDirectory + line);
                            // Check for a media file with relative path (with backslash)
                            else if (File.Exists(playlistDirectory + "\\" + line))
                                files.Add(playlistDirectory + "\\" + line);
                            // Check for a directory with absolute path
                            else if (Directory.Exists(line))
                                directoryPath = line;
                            // Check for a directory with relative path (without backslash)
                            else if (Directory.Exists(playlistDirectory + line))
                                directoryPath = playlistDirectory + line;
                            // Check for a directory with relative path (with backslash)
                            else if (Directory.Exists(playlistDirectory + "\\" + line))
                                directoryPath = playlistDirectory + "\\" + line;

                            // Check if the line is a directory
                            if (!String.IsNullOrEmpty(directoryPath))
                            {
                                // The line is a directory! We gotta search recursively through this directory.
                                var moreFiles = AudioTools.SearchAudioFilesRecursive(directoryPath, "MP3;FLAC;OGG;WAV;WV;APE");
                                if (moreFiles != null || moreFiles.Count == 0)
                                    files.AddRange(moreFiles);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading M3U playlist!", ex);
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }
            #endif

            return files;
        }

        /// <summary>
        /// Saves a playlist to a specific path using the M3U/M3U8 playlist format.
        /// </summary>        
        /// <param name="playlistFilePath">Playlist file path</param>
        /// <param name="playlist">Playlist object</param>
        /// <param name="useUTF8Encoding">Use UTF8 encoding (instead of ASCII)</param>
        /// <param name="useRelativePaths">Use relative paths</param>
        public static void SaveM3UPlaylist(string playlistFilePath, Playlist playlist, bool useRelativePaths, bool useUTF8Encoding)
        {
            StreamWriter writer = null;

            #if WINDOWSSTORE
            #else

            try
            {
                string playlistPath = Path.GetDirectoryName(playlistFilePath);

                // Check for UTF8
                if (useUTF8Encoding)
                    writer = new StreamWriter(playlistFilePath, false, Encoding.UTF8);
                #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
                else
                    writer = new StreamWriter(playlistFilePath, false, Encoding.ASCII);
                #endif

                writer.WriteLine("#EXTM3U");
                foreach (PlaylistItem item in playlist.Items)
                {
                    if (useRelativePaths)
                        writer.WriteLine(item.AudioFile.FilePath.Replace(playlistPath + "\\", ""));
                    else
                        writer.WriteLine(item.AudioFile.FilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving M3U playlist!", ex);
            }
            finally
            {
                if (writer != null) writer.Close();
            }

            #endif
        }

        #endregion

        #region PLS
        
        /// <summary>
        /// Returns the list of audio files to play from a PLS playlist.
        /// </summary>
        /// <param name="filePath">Playlist file path</param>
        /// <returns>List of audio file paths</returns>
        public static List<string> LoadPLSPlaylist(string filePath)
        {
            // Example:
            // [playlist]
            // File1=http://streamexample.com:80
            // Title1=My Favorite Online Radio
            // Length1=-1
            // File2=http://example.com/song.mp3
            // Title2=Remote MP3
            // Length2=286
            // File3=/home/myaccount/album.flac
            // Title3=Local album
            // Length3=3487
            // NumberOfEntries=3
            // Version=2

            StreamReader reader = null;
            var files = new List<string>();
            string playlistDirectory = Path.GetDirectoryName(filePath);

            #if WINDOWSSTORE
            #else

            try
            {
                reader = new StreamReader(filePath);

                // Read the first line; should be [playlist]
                string firstLine = reader.ReadLine();

                // Check if the first line could be read or is valid (should begin by [playlist])
                if (String.IsNullOrEmpty(firstLine) || firstLine.ToUpper() != "[PLAYLIST]")
                    throw new Exception("Error reading PLS playlist: the header isn't valid!");

                string line = string.Empty;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        // Remove whitespace, make upper
                        string lineValue = line.Trim().ToUpper();

                        // Determine what type of line (FILE, TITLE, LENGTH, NUMBEROFENTRIES or VERSION)
                        if (lineValue.StartsWith("FILE"))
                        {
                            lineNumber++;

                            // Fetch file name
                            string fileName = lineValue.Replace("FILE" + lineNumber.ToString() + "=", "");

                            #if WINDOWSSTORE
                            #else

                            // Check for a media file with absolute path
                            if (File.Exists(fileName))
                                files.Add(fileName);
                            // Check for a media file with relative path (without backslash)
                            else if (File.Exists(playlistDirectory + fileName))
                                files.Add(playlistDirectory + fileName);
                            // Check for a media file with relative path (with backslash)
                            else if (File.Exists(playlistDirectory + "\\" + fileName))
                                files.Add(playlistDirectory + "\\" + fileName);
                            // Check for a media file with relative path (with slash)
                            else if (File.Exists(playlistDirectory + "/" + fileName))
                                files.Add(playlistDirectory + "/" + fileName);

                            #endif
                        }
                        else if (lineValue.StartsWith("TITLE"))
                        {
                            // Ignore title
                        }
                        else if (lineValue.StartsWith("LENGTH"))
                        {
                            // Ignore length
                        }
                        else if (lineValue.StartsWith("NUMBEROFENTRIES"))
                        {
                            // This is the start of the footer; ignore for now
                        }
                        else if (lineValue.StartsWith("VERSION"))
                        {
                            // This is the end of the footer; ignore for now
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading PLS playlist!", ex);
            }
            finally
            {
                if (reader != null) reader.Dispose();
            }

            #endif

            return files;
        }

        /// <summary>
        /// Saves a playlist to a specific path using the PLS playlist format.
        /// </summary>        
        /// <param name="playlistFilePath">Playlist file path</param>
        /// <param name="playlist">Playlist object</param>
        /// <param name="useRelativePaths">Use relative paths</param>
        public static void SavePLSPlaylist(string playlistFilePath, Playlist playlist, bool useRelativePaths)
        {
            StreamWriter writer = null;
            
            #if WINDOWSSTORE
            #else            

            try
            {
                string playlistPath = Path.GetDirectoryName(playlistFilePath);
                writer = new StreamWriter(playlistFilePath);

                // Write header
                writer.WriteLine("[playlist]");
                writer.WriteLine();

                for(int a = 0; a < playlist.Items.Count; a++)
                {                    
                    if (useRelativePaths)
                        writer.WriteLine("File" + (a + 1).ToString() + "=" + playlist.Items[a].AudioFile.FilePath.Replace(playlistPath + "\\", ""));                        
                    else
                        writer.WriteLine("File" + (a + 1).ToString() + "=" + playlist.Items[a].AudioFile.FilePath);                        

                    // Write title
                    writer.WriteLine("Title" + (a + 1).ToString() + "=" + playlist.Items[a].AudioFile.Title);
                    
                    // Write spacer
                    writer.WriteLine();
                }

                // Write fotter
                writer.WriteLine("NumberOfEntries=" + playlist.Items.Count.ToString());
                writer.WriteLine("Version=2");
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving PLS playlist!", ex);
            }
            finally
            {
                if (writer != null) writer.Close();
            }
            #endif
        }

        #endregion

        #region XSPF

        /// <summary>
        /// Returns the list of audio files to play from a XSPF playlist.
        /// </summary>
        /// <param name="filePath">Playlist file path</param>
        /// <returns>List of audio file paths</returns>
        public static List<string> LoadXSPFPlaylist(string filePath)
        {
            // Example:
            // <?xml version="1.0" encoding="UTF-8"?>
            // <playlist version="1" xmlns="http://xspf.org/ns/0/">
            //   <trackList>
            //     <track>
            //       <title>Internal Example</title>
            //       <location>file:///C:/music/foo.mp3</location>
            //     </track>
            //     <track>
            //       <title>External Example</title>
            //       <location>http://www.example.com/music/bar.ogg</location>
            //     </track>
            //   </trackList>
            // </playlist>

            var files = new List<string>();
            XDocument doc = null;

            #if WINDOWSSTORE
            #else

            string playlistDirectory = Path.GetDirectoryName(filePath);

            try
            {
                doc = XDocument.Load(filePath);
                XNamespace ns = "http://xspf.org/ns/0/";

                // Read playlist element
                XElement elementPlaylist = doc.Element(ns + "playlist");
                if (elementPlaylist == null)
                {
                    throw new Exception("The playlist node could not be read!");
                }

                // Read trackList element
                XElement elementTrackList = elementPlaylist.Element(ns + "trackList");
                if (elementTrackList == null)
                {
                    throw new Exception("The trackList node could not be read!");
                }

                // Read track elements                
                List<XElement> elementsTracks = elementTrackList.Elements(ns + "track").ToList();
                for (int a = 0; a < elementsTracks.Count; a++)
                {                    
                    // Get location element (ignore other elements for now)
                    XElement elementLocation = elementsTracks[a].Element(ns + "location");
                    if (elementLocation != null)
                    {
                        // The file format is always URI.
                        // file:///mp3s/song_1.mp3
                        // file:///C:/mp3s/song_1.mp3

                        try
                        {
                            // Try to get URI
                            Uri uri = new Uri(elementLocation.Value);
                            string audioFilePath = uri.LocalPath.Replace("/", "\\");

                            // Check if the path is valid
                            if (!string.IsNullOrEmpty(audioFilePath))
                            {
                                // Check for a media file with absolute path
                                if (File.Exists(audioFilePath))
                                    files.Add(audioFilePath);
                                // Check for a media file with relative path (without slash)
                                else if (File.Exists(playlistDirectory + audioFilePath))
                                    files.Add(playlistDirectory + audioFilePath);
                                // Check for a media file with relative path (with slash)
                                else if (File.Exists(playlistDirectory + "\\" + audioFilePath))
                                    files.Add(playlistDirectory + "\\" + audioFilePath);
                            }
                        }
                        catch
                        {
                            // Skip file
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading XSPF playlist!", ex);
            }
            finally
            {
                doc = null;            
            }

            #endif

            return files;
        }

        /// <summary>
        /// Saves a playlist to a specific path using the XSPF playlist format.
        /// </summary>        
        /// <param name="playlistFilePath">Playlist file path</param>
        /// <param name="playlist">Playlist object</param>
        /// <param name="useRelativePaths">Use relative paths</param>
        public static void SaveXSPFPlaylist(string playlistFilePath, Playlist playlist, bool useRelativePaths)
        {
            XDocument doc = null;
            string playlistPath = Path.GetDirectoryName(playlistFilePath);

            try
            {
                doc = new XDocument();
                XNamespace ns = "http://xspf.org/ns/0/";

                // Create basic elements
                XElement elementPlaylist = new XElement(ns + "playlist");                
                XElement elementTrackList = new XElement(ns + "trackList");

                // Set playlist attributes                                                
                XAttribute attributePlaylistVersion = new XAttribute("version", 1);                
                elementPlaylist.Add(attributePlaylistVersion);                

                for (int a = 0; a < playlist.Items.Count; a++)
                {
                    // Create elements
                    XElement elementTrack = new XElement(ns + "track");                    
                    XElement elementTitle = new XElement(ns + "title", playlist.Items[a].AudioFile.Title);

                    // Check if paths are relative
                    XElement elementLocation = null;
                    if (useRelativePaths)
                    {
                        // Write relative file path                        
                        elementLocation = new XElement(ns + "location", "file:///" + playlist.Items[a].AudioFile.FilePath.Replace(playlistPath + "\\", "").Replace("\\","/"));
                    }
                    else
                    {
                        // Write absolute file path                        
                        elementLocation = new XElement(ns + "location", "file:///" + playlist.Items[a].AudioFile.FilePath.Replace("\\", "/"));
                    }                        

                    // Add elements
                    elementTrack.Add(elementTitle);
                    elementTrack.Add(elementLocation);
                    elementTrackList.Add(elementTrack);
                }

                // Add elements
                elementPlaylist.Add(elementTrackList);
                doc.Add(elementPlaylist);

                // Save playlist to file
                #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
                doc.Save(playlistFilePath);
                #endif
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving XSPF playlist!", ex);
            }
            finally
            {
                doc = null;
            }
        }

        #endregion
    }
}
