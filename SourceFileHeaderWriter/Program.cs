using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SourceFileHeaderWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Validate params (must have 4 params)
            if (args.Length != 4)
            {
                Console.WriteLine("Error: Parameters are missing!");
                Console.WriteLine("");
                PrintHelp();
                return;
            }

            // Get parameter values
            string folderPath = args[0];
            string appTitle = args[1];
            string copyrightStartYear = args[2];
            string copyrightHolder = args[3];

            // Get list of source files to update
            Console.WriteLine("Finding source files to update...");
            string[] extensions = new string[] { ".cs" };
            List<string> filePaths = FindFilesRecursively(extensions, folderPath);

            if (filePaths.Count == 0)
            {
                Console.WriteLine("Error: There are no source files to update in the specified folder path: " + folderPath);
                return;
            }

            // Update source files
            string headerContent = GetHeader(appTitle, copyrightStartYear, copyrightHolder);
            foreach (string filePath in filePaths)
            {
                Console.WriteLine("Writing " + filePath);
                WriteHeader(filePath, headerContent);
            }

            // Success!
            Console.WriteLine("The tool has rewritten source file headers successfully.");
        }

        public static void PrintHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("Source File Header Writer Tool v1.0 (C) 2013 Yanick Castonguay");
            Console.WriteLine("\n");
            Console.WriteLine("This tool rewrites the header of C# source files for the GPLv3 license. It replaces any comments located in the header of the file. It rewrites all the .CS files recursively from the specified folder path.");
            Console.WriteLine("\n");
            Console.WriteLine("Usage:     SourceFileHeaderWriter.exe [FolderPath] [AppTitle] [CopyrightStartYear] [CopyrightHolder]");
            Console.WriteLine("Examples:  SourceFileHeaderWriter.exe \"C:\\Code\\MPfm\" \"MPfm\" \"2011\" \"Yanick Castonguay\"");
            Console.WriteLine("           SourceFileHeaderWriter.exe \"~/Projects/MPfm\" \"MPfm\" \"2011\" \"Yanick Castonguay\"");
        }

        public static List<string> FindFilesRecursively(IEnumerable<string> extensions, string folderPath)
        {
            List<string> files = new List<string>();
            string[] directories = Directory.GetDirectories(folderPath);
            foreach (string directory in directories)
            {
                files.AddRange(FindFilesRecursively(extensions, directory));
            }

            // Do not use a search pattern or this won't work on Linux and OS X
            string[] filePaths = Directory.GetFiles(folderPath);
            foreach (string file in filePaths)
            {
                string extension = Path.GetExtension(file);
                string match = extensions.FirstOrDefault(x => x.ToUpper() == extension.ToUpper());
                if (!string.IsNullOrEmpty(match))
                    files.Add(file);
            }
            return files;
        }

        public static string GetHeader(string softwareTitle, string copyrightStartYear, string copyrightHolder)
        {
            string copyrightYear = copyrightStartYear + "-" + DateTime.Now.Year.ToString();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Copyright © " + copyrightYear + " " + copyrightHolder);
            sb.AppendLine("//");
            sb.AppendLine("// This file is part of " + softwareTitle + ".");
            sb.AppendLine("//");
            sb.AppendLine("// " + softwareTitle + " is free software: you can redistribute it and/or modify");
            sb.AppendLine("// it under the terms of the GNU General Public License as published by");
            sb.AppendLine("// the Free Software Foundation, either version 3 of the License, or");
            sb.AppendLine("// (at your option) any later version.");
            sb.AppendLine("//");
            sb.AppendLine("// " + softwareTitle + " is distributed in the hope that it will be useful,");
            sb.AppendLine("// but WITHOUT ANY WARRANTY; without even the implied warranty of");
            sb.AppendLine("// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the");
            sb.AppendLine("// GNU General Public License for more details.");
            sb.AppendLine("//");
            sb.AppendLine("// You should have received a copy of the GNU General Public License");
            sb.AppendLine("// along with " + softwareTitle + ". If not, see <http://www.gnu.org/licenses/>.");
            return sb.ToString();
        }

        public static void WriteHeader(string filePath, string headerContent)
        {
            // Read all lines of the file
            List<string> lines = File.ReadLines(filePath).ToList();

            // Try to find the first line of code in the file
            int firstLineOfCodeIndex = -1;
            for (int index = 0; index < lines.Count; index++)
            {
                string line = lines[index];
                if (!line.StartsWith("//"))
                {
                    // Is this an empty line?
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        firstLineOfCodeIndex = index;
                        break;
                    }
                }
            }

            // Check code was found in the file (in fact, anything that doesn't start with //)
            if (firstLineOfCodeIndex == -1)
            {
                Console.WriteLine("==> Warning: Could not find any code in " + filePath);
                return;
            }

            // Delete all comment lines at the start of the file
            lines.RemoveRange(0, firstLineOfCodeIndex);

            // Rewrite the file
            using (var textWriter = new StreamWriter(filePath + ".baktest"))
            {
                // Add header
                textWriter.Write(headerContent);
                textWriter.WriteLine(); // Add an empty line after comments

                // Write the rest of file
                foreach (string line in lines)
                {
                    textWriter.WriteLine(line);
                }
            }

        }
    }

}
