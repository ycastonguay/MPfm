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
using Sessions.Sound.AudioFiles;
using System.Linq;
using System.Text;
using Sessions.Core;

namespace Sessions.Sound.CueFiles
{
    public static class CueFileLoader
    {
        public static IEnumerable<AudioFile> GetAudioFilesFromCueFile(string cueFilePath)
        {
            // Load the whole audio file first
            var audioFiles = new List<AudioFile>();
            string audioFilePath = GetAudioFilePathFromCueFile(cueFilePath);    
            if (audioFilePath == null)
            {
                throw new FileNotFoundException("Could not find the audio file related to the cue file!");
            }
            var wholeAudioFile = new AudioFile(audioFilePath, Guid.NewGuid(), true);

            try
            {
                // Load the cue file
                var cueFile = ReadCueFile(cueFilePath);
                for (int i = 0; i < cueFile.Tracks.Count; i++)
                {
                    var track = cueFile.Tracks [i];
                    var audioFile = new AudioFile();
                    audioFile.AlbumTitle = cueFile.Title;
                    audioFile.ArtistName = cueFile.Performer;
                    audioFile.Title = track.Title;
                    audioFile.FilePath = audioFilePath;
                    audioFile.CueFilePath = cueFilePath;
                    audioFile.StartPosition = track.IndexPosition;
                    // TODO: Calculate length

                    // Determine end position
                    if (i == cueFile.Tracks.Count - 1)
                    {
                        // This is the last track, the end position is the audio file length
                        audioFile.EndPosition = GetPositionFormatForSessions(wholeAudioFile.Length);
                    } 
                    else
                    {
                        // Take the next track start position as the end position for this track
                        audioFile.EndPosition = GetPositionFormatForSessions(cueFile.Tracks [i + 1].IndexPosition);
                    }

                    audioFiles.Add(audioFile);
                }
            } 
            catch (Exception ex)
            {
                Tracing.Log("Could not read cue file (path: {0}): {1}", cueFilePath, ex);
            }

            return audioFiles;
        }

        public static string GetAudioFilePathFromCueFile(string cueFilePath)
        {
            // Find out the actual audio file path
            // 99% of the time the audio file has the same file name but a different extension
            string directoryPath = Path.GetDirectoryName(cueFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(cueFilePath);
            var files = Directory.GetFiles(directoryPath, string.Format("{0}.*", fileNameWithoutExtension), SearchOption.TopDirectoryOnly);
            var filesWithoutCue = files.Where(x => !x.ToUpper().Contains(".CUE")).ToList();
            return filesWithoutCue.Any() ? filesWithoutCue.ElementAt(0) : null;
        }

        public static CueFile ReadCueFile(string cueFilePath)
        {
            CueTrack track = null;
            var cueFile = new CueFile();

            using (var reader = new StreamReader(cueFilePath))
            {
                while (true)
                {
                    // Check for end of file
                    if (reader.EndOfStream)
                    {
                        // Add previous track if it exists
                        if(track != null && !cueFile.Tracks.Contains(track))
                            cueFile.Tracks.Add(track);
                        break;
                    }

                    string line = reader.ReadLine();
                    int level = GetHiearchyLevelForLine(line);
                    var splitLine = line.Trim().Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitLine.Length > 0)
                    {
                        string command = splitLine[0];
                        switch (command)
                        {
                            case "REM":
                                // REM GENRE Symphony
                                // REM DATE 1981
                                // REM DISCID 340B4C04
                                // REM COMMENT "ExactAudioCopy v0.99pb5"
                                // This defines comments at the start of the file, usually metadata                            
                                var metadata = new CueMetadata();
                                metadata.Key = GetSubcommandForLine(line);
                                metadata.Value = GetValueForLine(line);
                                cueFile.Metadata.Add(metadata);
                                Console.WriteLine("==>> READ METADATA: key: {0} value: {1}", metadata.Key, metadata.Value);
                                break;
                            case "FILE":
                                // FILE "Wand, Gunter & Cologne Radio Symphony Orchestra - BRUCKNER - 1891 Vienna Revision by Bruckner himself. Ed. Guenter Brosche [1980].wav" WAVE
                                cueFile.File = GetValueForLine(line);
                                break;
                            case "TRACK":
                                // TRACK 01 AUDIO
                                // This initiates a new track

                                // Add previous track if it exists
                                if(track != null)
                                    cueFile.Tracks.Add(track);

                                track = new CueTrack();
                                track.TrackNumber = GetSubcommandForLine(line);
                                track.TrackType = GetValueForLine(line);
                                break;
                            case "INDEX":
                                // INDEX 01 00:00:00
                                track.Index = GetSubcommandForLine(line);
                                track.IndexPosition = GetValueForLine(line);
                                break;
                            case "TITLE":
                                // TITLE "Symphony No 1 in C minor (Vienna version 1891) - I.Allegro molto moderato"
                                // This can be in multiple levels of hiearchy, defines the title of the album or the song title
                                if (level == 1)
                                {
                                    cueFile.Title = GetValueForLine(line);
                                }
                                else if (level == 2)
                                {
                                    track.Title = GetValueForLine(line);
                                }
                                break;
                            case "PERFORMER":
                                // PERFORMER "Wand, Gunter & Cologne Radio Symphony Orchestra"
                                // This can be in multiple levels of hiearchy, defines the performer of the album or the song performer
                                if (level == 1)
                                {
                                    cueFile.Performer = GetValueForLine(line);
                                }
                                else if (level == 2)
                                {
                                    track.Performer = GetValueForLine(line);
                                }
                                break;
                            case "PREGAP":
                                break;
                            case "POSTGAP":
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return cueFile;
        }

        private static int GetHiearchyLevelForLine(string line)
        {
            // The white space at the start is important and defines the hiearchy of the commands.
            // There can only be 3 levels of hiearchy, so we keep things simple here.
            if (line.StartsWith("  ", StringComparison.Ordinal))
                return 2;
            else if (line.StartsWith("    ", StringComparison.Ordinal))
                return 3;

            return 1;
        }

        private static string GetCommandForLine(string line)
        {
            var splitLine = line.Trim().Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return splitLine.Length > 0 ? splitLine[0] : null;
        }

        private static string GetSubcommandForLine(string line)
        {
            string value = null;
            var splitLine = line.Trim().Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries); 

            if(splitLine.Length > 1)
                value = splitLine[1];

            return value;
        }

        private static string GetValueForLine(string line)
        {
            // The value must have quotes when it contains spaces.
            // However, the value can be found in the second word or third word of the line.
            // i.e. TITLE "Ege Bamyasi"
            //      PERFORMER Can
            // i.e. REM GENRE Symphony
            //      REM COMMENT "Gustav Mahler is awesome"

            string value = null;
            string trimmedLine = line.Trim();

            // If this ends by a quote?
            bool valueContainsQuotes = trimmedLine [trimmedLine.Length - 1] == '\"';
            if (valueContainsQuotes)
            {
                // To find the value, find the first occurence of the quote character
                int indexOfQuote = trimmedLine.IndexOf('\"');
                string valueWithQuotes = trimmedLine.Substring(indexOfQuote, trimmedLine.Length - indexOfQuote);
                string valueWithoutQuotes = valueWithQuotes.Substring(1, valueWithQuotes.Length - 2);
                Console.WriteLine("====>>> GetvalueForLine **WITH QUOTES**: {0}", valueWithoutQuotes);
                return valueWithoutQuotes;
            }

            var splitLine = line.Trim().Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries); 

            if(splitLine.Length > 1)
                value = splitLine[splitLine.Length - 1];

            // Does the value have quotes, if yes, then remove quotes
            if (value.StartsWith("\"", StringComparison.Ordinal))
                return value.Remove(value.Length - 1).Remove(0);


            Console.WriteLine("====>>> GetvalueForLine **WITHOUT QUOTES**: {0}", value);
            return value;
        }

        private static string GetPositionFormatForSessions(string cuePosition)
        {
            // This takes the 00:00:00 format and makes it into 00:00.000
            var sb = new StringBuilder(cuePosition);
            int index = cuePosition.LastIndexOf(':');
            if(index >= 0)
                sb[index] = '.';

            return sb.ToString();
        }
    }
}