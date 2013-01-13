using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectSync
{
	public static class SolutionFileReader
	{
        public static void Read(string filePath)
        {
            // Read solution file as an array of strings
            string[] solutionFileLines = File.ReadAllLines(filePath);

            // Read lines
            List<string> listProjectFilePaths = new List<string>();
            foreach (string line in solutionFileLines)
            {
                // We only are intered in CSPROJ file paths, so these lines always start with "Project"
                if(line.ToUpper().StartsWith("PROJECT"))
                {

                }
            }
        }
    }
}