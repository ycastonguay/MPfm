﻿//
// PlaylistTools.cs: This static class contains useful methods for loading and 
//                   saving playlists.
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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MPfm.Sound
{
    /// <summary>
    /// This static class contains useful methods for loading and saving playlists.
    /// </summary>
    public static class PlaylistTools
    {
        #region M3U/M3U8
        
        /// <summary>
        /// Returns the list of audio files to play from a M3U playlist.
        /// </summary>
        /// <param name="filePath">Playlist file path</param>
        /// <returns>List of audio file paths</returns>
        public static List<string> LoadM3UPlaylist(string filePath)
        {
            // Declare variables
            List<string> files = new List<string>();
            StreamReader reader = null;

            try
            {
                // Open reader
                reader = new StreamReader(filePath);

                // Loop through lines
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    // Make sure the line isn't empty
                    if (!String.IsNullOrEmpty(line))
                    {
                        // Make sure the line isn't a comment (begins by the pound # sign)
                        if (line[0] != '#')
                        {
                            // Make sure the file exists
                            if (File.Exists(line))
                            {
                                // Add file to list
                                files.Add(line);
                            }
                            else if (Directory.Exists(line))
                            {
                                // The line is a directory! We gotta search recursively through this directory.
                                List<string> moreFiles = AudioTools.SearchAudioFilesRecursive(line, "MP3;FLAC;OGG;WAV;WV;APE");

                                // Check if the list is empty
                                if (moreFiles != null || moreFiles.Count == 0)
                                {
                                    // Add files to list
                                    files.AddRange(moreFiles);
                                }
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
                // Close reader
                reader.Dispose();
            }

            return files;
        }

        /// <summary>
        /// Saves a playlist to a specific path using the M3U playlist format.
        /// </summary>        
        /// <param name="playlistFilePath">Playlist file path</param>
        /// <param name="playlist">Playlist object</param>
        public static void SaveM3UPlaylist(string playlistFilePath, Playlist playlist)
        {
            // Declare variables
            StreamWriter writer = null;

            try
            {
                // Open writer
                writer = new StreamWriter(playlistFilePath);

                // Write MPfm header
                writer.WriteLine("# " + playlist.Name);
                writer.WriteLine("# Saved using MPfm: Music Player for Musicians (http://www.mp4m.org)");

                // Loop through files
                foreach (PlaylistItem item in playlist.Items)
                {
                    // Write file path
                    writer.WriteLine(item.AudioFile.FilePath);
                }

                // Dispose writer
                writer.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving M3U playlist!", ex);
            }
            finally
            {
                // Close writer
                writer.Dispose();
            }
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

            // Declare variables
            List<string> files = new List<string>();
            StreamReader reader = null;

            try
            {
                // Open reader
                reader = new StreamReader(filePath);

                // Read the first line; should be [playlist]
                string firstLine = reader.ReadLine();

                // Check if the first line could be read or is valid (should begin by [playlist])
                if (String.IsNullOrEmpty(firstLine) || firstLine.ToUpper() != "[PLAYLIST]")
                {
                    throw new Exception("Error reading PLS playlist: the header isn't valid!");
                }

                // Loop through lines
                string line = string.Empty;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    // Make sure the line isn't empty
                    if (!String.IsNullOrEmpty(line))
                    {
                        // Remove whitespace, make upper
                        string lineValue = line.Trim().ToUpper();

                        // Determine what type of line (FILE, TITLE, LENGTH, NUMBEROFENTRIES or VERSION)
                        if (lineValue.StartsWith("FILE"))
                        {
                            // Increment line number
                            lineNumber++;

                            // Fetch file name
                            string fileName = lineValue.Replace("FILE" + lineNumber.ToString() + "=", "");

                            // Add to list
                            files.Add(fileName);
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
                // Close reader
                reader.Dispose();
            }

            return files;
        }

        /// <summary>
        /// Saves a playlist to a specific path using the PLS playlist format.
        /// </summary>        
        /// <param name="playlistFilePath">Playlist file path</param>
        /// <param name="playlist">Playlist object</param>
        public static void SavePLSPlaylist(string playlistFilePath, Playlist playlist)
        {
            // Declare variables
            StreamWriter writer = null;

            try
            {
                // Open writer
                writer = new StreamWriter(playlistFilePath);

                // Write header
                writer.WriteLine("[playlist]");
                writer.WriteLine();

                // Loop through files
                for(int a = 0; a < playlist.Items.Count; a++)
                {
                    // Write file path and title
                    writer.WriteLine("File" + (a+1).ToString() + "=" + playlist.Items[a].AudioFile.FilePath);
                    writer.WriteLine("Title" + (a + 1).ToString() + "=" + playlist.Items[a].AudioFile.Title);
                    
                    // Write spacer
                    writer.WriteLine();
                }

                // Write fotter
                writer.WriteLine("NumberOfEntries=" + playlist.Items.Count.ToString());
                writer.WriteLine("Version=2");

                // Dispose writer
                writer.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving PLS playlist!", ex);
            }
            finally
            {
                // Close writer
                writer.Dispose();
            }
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

            // Declare variables
            List<string> files = new List<string>();
            XDocument doc = null;

            try
            {
                // Load XML document
                doc = XDocument.Load(filePath);

                // Create namespace
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

                // Loop through elements
                for (int a = 0; a < elementsTracks.Count; a++)
                {                    
                    // Get location element (ignore other elements for now)
                    XElement elementLocation = elementsTracks[a].Element(ns + "location");
                    if (elementLocation != null)
                    {
                        // Add to list
                        files.Add(elementLocation.Value);
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

            return files;
        }

        /// <summary>
        /// Saves a playlist to a specific path using the XSPF playlist format.
        /// </summary>        
        /// <param name="playlistFilePath">Playlist file path</param>
        /// <param name="playlist">Playlist object</param>
        public static void SaveXSPFPlaylist(string playlistFilePath, Playlist playlist)
        {
            // Declare variables
            XDocument doc = null;

            try
            {
                // Create document
                doc = new XDocument();

                // Create namespace
                XNamespace ns = "http://xspf.org/ns/0/";

                // Create basic elements
                XElement elementPlaylist = new XElement(ns + "playlist");                
                XElement elementTrackList = new XElement(ns + "trackList");

                // Set playlist attributes                                                
                XAttribute attributePlaylistVersion = new XAttribute("version", 1);                
                elementPlaylist.Add(attributePlaylistVersion);                

                // Loop through files
                for (int a = 0; a < playlist.Items.Count; a++)
                {
                    // Create elements
                    XElement elementTrack = new XElement(ns + "track");                    
                    XElement elementTitle = new XElement(ns + "title", playlist.Items[a].AudioFile.Title);
                    XElement elementLocation = new XElement(ns + "location", playlist.Items[a].AudioFile.FilePath);

                    // Add elements
                    elementTrack.Add(elementTitle);
                    elementTrack.Add(elementLocation);
                    elementTrackList.Add(elementTrack);
                }

                // Add elements
                elementPlaylist.Add(elementTrackList);
                doc.Add(elementPlaylist);

                // Save playlist to file
                doc.Save(playlistFilePath);
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
