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

namespace Sessions.Sound.AudioFiles
{
    public static class CueFileLoader
    {
        public static IEnumerable<AudioFile> FromCue(string filePath)
        {
            var audioFiles = new List<AudioFile>();




            return audioFiles;
        }

        public static void ReadCue(string filePath)
        {
            string albumTitle = string.Empty;
            string albumPerformer = string.Empty;
            string songTitle = string.Empty;
            string songPerformer = string.Empty;

            using (var reader = new StringReader(filePath))
            {
                while (true)
                {
                    string line = reader.ReadLine();

                    // Check for end of file
                    if (line == null)
                        break;

//                    // Remove white space at the start and end
//                    line = line.Trim();

                    int level = GetHiearchyLevelForLine(line);

                    var splitLine = line.Trim().Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitLine.Length > 0)
                    {
                        string command = splitLine[0];
                        switch (command)
                        {
                            case "REM":
                                // This defines comments at the start of the file, usually metadata                            
                                break;
                            case "PERFORMER":
                                // This can be in multiple levels of hiearchy, defines the performer of the album or the song performer
                                break;
                            case "TITLE":
                                // This can be in multiple levels of hiearchy, defines the title of the album or the song title
                                if (level == 1)
                                {
                                    //albumTitle = 
                                }
                                else if (level == 3)
                                {
                                }
                                break;
                            case "FILE":
                                break;
                            case "TRACK":
                                break;
                            case "INDEX":
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
            if (splitLine.Length > 0)
                return splitLine[0];

            return null;
        }

        private static string GetREMKeywordForLine(string line)
        {
            string value = null;
            var splitLine = line.Trim().Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries); 

            return value;
        }

        private static string GetValueForLine(string line)
        {
            // Usually follows immediately the command
            // i.e. TITLE "Ege Bamyasi"
            //      PERFORMER Can

            // The metadata field follows immediately the command, and then the value is defined
            // The value doesn't use quotes for single words, but uses quotes when it uses spaces
            // i.e. REM GENRE Symphony
            //      REM COMMENT "Gustav Mahler is awesome"

            string value = null;
            var splitLine = line.Trim().Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries); 

            if(splitLine.Length > 1)
                value = splitLine[splitLine.Length - 1];

            // Does the value have quotes, if yes, then remove quotes
            if (value.StartsWith("\"", StringComparison.Ordinal))
                return value.Remove(value.Length - 1).Remove(0);

            return value;
        }
    }
}
