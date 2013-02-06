using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectSync
{
	public static class SolutionFileReader
	{
        public static List<string> ExtractProjectFilePaths(string solutionFilePath)
        {
            // Read solution file as an array of strings
            string[] solutionFileLines = File.ReadAllLines(solutionFilePath);
            string baseDirectory = Path.GetDirectoryName(solutionFilePath);

            // Read lines
            List<string> listProjectFilePaths = new List<string>();
            foreach (string line in solutionFileLines)
            {
                // We only are intered in CSPROJ file paths, so these lines always start with "Project"
                if(line.ToUpper().StartsWith("PROJECT"))
                {
                    // Try to find the CSPROJ file path
                    string[] parts = line.Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string part in parts)
                    {
                        if (part.ToUpper().Contains(".CSPROJ"))
                        {
                            // Make sure the directory separator is right on every platform (the solution file contains '\')
                            string combinedPath = Path.GetFullPath(Path.Combine(baseDirectory, part));
                            combinedPath = combinedPath.Replace('\\', Path.DirectorySeparatorChar);
                            bool combinedPathExists = File.Exists(combinedPath);
                            listProjectFilePaths.Add(combinedPath);
                        }
                    }
                }
            }

            return listProjectFilePaths;
        }
    }
}