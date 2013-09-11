// Copyright © 2011-2013 Yanick Castonguay
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
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Timers;

namespace ProjectSync
{
    public class MainClass
    {
        private static Timer _timer = null;
        private static DateTime _dateTimeRefresh = DateTime.Now;
        private static List<string> _options = new List<string>();
        private static List<string> _projectFiles = new List<string>();
        private static readonly FileSystemWatcher _fileSystemWatcher = null;        

        public static void Main(string[] args)
        {
            // Validate params
            if (args.Length == 0)
            {
                LogWithoutTimestamp("Error: The first parameter must be the solution file path!");
                LogWithoutTimestamp("");
                PrintHelp();
                return;
            }

            // Make sure the solution file exists
            if (!File.Exists(args[0]))
            {
                LogWithoutTimestamp("Error: The solution file doesn't exist (" + args[0] + ")");
                LogWithoutTimestamp("");
                PrintHelp();
                return;
            }

            // Check for options
            foreach (string t in args)
                if (t.StartsWith("--"))
                    _options.Add(t.ToUpper());

            try
            {
                Log("Reading solution file...");
                _projectFiles = SolutionFileReader.ExtractProjectFilePaths(args[0]);
            }
            catch (Exception ex)
            {
                Log("An error occured while reading the solution file: " + ex.Message + "\n" + ex.StackTrace);
                return;
            }

            // Check for manual project file refresh
            if (_options.Contains("--REFRESH"))
            {
                // Update project files
                foreach(string projectFile in _projectFiles)
                    UpdateProjectFiles(projectFile);

                // Exit the app
                Log("The manual project file refresh was completed succesfully.");
                return;
            }

            // Default mode: watch solution file for changes
            WatchSolutionFile(args[0]);
        }

        public static void PrintHelp()
        {
            LogWithoutTimestamp("");
            LogWithoutTimestamp("Project File Synchronization Tool v1.0 (C) 2013 Yanick Castonguay");
            LogWithoutTimestamp("\n");
            LogWithoutTimestamp("This tool synchronizes Visual Studio/MonoDevelop project files. It works by watching changes in all project files listed in a solution. It then synchronizes file additions/deletion between project files located in the same directory. You can use this tool to synchronize class library project files between .NET/Mono/MonoTouch/Mono for Android projects.");
            LogWithoutTimestamp("\n");
            LogWithoutTimestamp("Option list:");
            LogWithoutTimestamp("--refresh: Force refreshing all project files from a solution.");
            LogWithoutTimestamp("\n");
            LogWithoutTimestamp("Usage:     ProjectSync.exe [SolutionFilePath] [--refresh]");
            LogWithoutTimestamp("Examples:  ProjectSync.exe MPfm.sln");
            LogWithoutTimestamp("           ProjectSync.exe C:\\Code\\MPfm\\MPfm.sln");
            LogWithoutTimestamp("           ProjectSync.exe ~/Projects/MPfm/MPfm.sln --refresh");
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void WatchSolutionFile(string solutionFilePath)
        {
            Log("Press \'q\' to exit the application.");
            Log("");

            _timer = new Timer(5000);
            _timer.Elapsed += (object sender, ElapsedEventArgs e) => {
                Log("Checking for changes in files...");
                DateTime dateTimeCurrentRefresh = DateTime.Now; // Keep a timestamp from the moment we are checking files to make sure we don't miss an update
                CheckForChanges(_projectFiles);
                _dateTimeRefresh = dateTimeCurrentRefresh;
            };
            _timer.Start();

            // File system watcher doesn't work well on Mac and Linux with Mono :-(
//            Log("Listening to file changes of the following directory: " + baseSolutionPath);
//            fileSystemWatcher = new FileSystemWatcher(baseSolutionPath, "*");
//            fileSystemWatcher.IncludeSubdirectories = true;
//            fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
//                                             NotifyFilters.DirectoryName;
//            //fileSystemWatcher.Filter = "*.csproj";
//
//            fileSystemWatcher.Changed += HandleFileWatcherChanged;
//            fileSystemWatcher.Created += HandleFileWatcherCreated;
//            fileSystemWatcher.Deleted += HandleFileWatcherDeleted;
//            fileSystemWatcher.Renamed += HandleFileWatcherRenamed;
//
//            fileSystemWatcher.EnableRaisingEvents = true;
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Q)
                {
                    _timer.Stop();
                    break;
                }
            }
        }

        private static void CheckForChanges(IEnumerable<string> filePaths)
        {
            // Check for changed files
            List<string> changedFiles = new List<string>();
            foreach (string filePath in filePaths)
            {
                DateTime dateTimeWrite = File.GetLastWriteTime(filePath);
                if (dateTimeWrite >= _dateTimeRefresh)
                {
                    changedFiles.Add(filePath);
                }
            }

            // Loop through modified CSPROJ files
            foreach (string filePath in changedFiles)
                UpdateProjectFiles(filePath);
        }

        private static void UpdateProjectFiles(string masterProjectFilePath)
        {
            // Fetch all CSPROJ files from the same directory
            Log("Updating project files from master file: " + masterProjectFilePath);
            string directoryPath = Path.GetDirectoryName(masterProjectFilePath);
            string[] csProjFiles = Directory.GetFiles(directoryPath, "*.csproj");
                
            // Make a list of project files to update
            List<string> projectFilesToUpdate = csProjFiles.ToList();
            projectFilesToUpdate.Remove(masterProjectFilePath);
            List<string> compiles = ProjectFileReader.ExtractProjectCompileFilePaths(masterProjectFilePath);
                
            // Update other CSPROJ files
            foreach (string projectFilePath in projectFilesToUpdate)
            {
                Log("Updating project file " + projectFilePath);
                ProjectFileWriter.UpdateProjectFileCompileList(projectFilePath, compiles);
            }
        }

        private static void HandleFileWatcherChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.FullPath.ToUpper().Contains(".CSPROJ"))
                {
                    Log("A project file has changed: " + e.FullPath);

                    // Deactivate file watcher while editing other CSPROJ files (or this will trigger an endless loop!)
                    //_fileSystemWatcher.EnableRaisingEvents = false;

                    // Fetch all CSPROJ files from the same directory
                    string directoryPath = Path.GetDirectoryName(e.FullPath);
                    string[] csProjFiles = Directory.GetFiles(directoryPath, "*.csproj");

                    // Make a list of project files to update
                    List<string> projectFilesToUpdate = csProjFiles.ToList();
                    projectFilesToUpdate.Remove(e.FullPath);
                    List<string> compiles = ProjectFileReader.ExtractProjectCompileFilePaths(e.FullPath);

                    // Update other CSPROJ files
                    foreach (string projectFilePath in projectFilesToUpdate)
                    {
                        Log("Writing project file changes to: " + projectFilePath);
                        ProjectFileWriter.UpdateProjectFileCompileList(projectFilePath, compiles);
                    }

                    // Reactive file watcher
                    //_fileSystemWatcher.EnableRaisingEvents = true;
                }
            }
            catch (IOException ex)
            {
                Log("An error occured while processing file change (is the file currently in use?): " + ex.Message + "\n" + ex.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                Log("An error occured while processing file change: " + ex.Message + "\n" + ex.StackTrace);
                throw;
            }
        }

        private static void HandleFileWatcherCreated(object sender, FileSystemEventArgs e)
        {
            Log("A file has been created: " + e.FullPath);
        }

        private static void HandleFileWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            Log("A file has been deleted: " + e.FullPath);
        }

        private static void HandleFileWatcherRenamed(object sender, RenamedEventArgs e)
        {
            Log("A file has been renamed: " + e.FullPath);
        }

        private static void Log(string message)
        {
            if(!string.IsNullOrEmpty(message))
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }

        private static void LogWithoutTimestamp(string message)
        {
            if (!string.IsNullOrEmpty(message))
                Console.WriteLine(message);
        }    
    }
}
