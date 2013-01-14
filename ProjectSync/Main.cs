using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Timers;
using System.Xml.Linq;

namespace ProjectSync
{
    public class MainClass
    {
        private static Timer timer = null;
        private static DateTime dateTimeRefresh = DateTime.Now;
        public static List<string> listProjectFiles = new List<string>();
        public static FileSystemWatcher fileSystemWatcher = null;

        public static void Main(string[] args)
        {
            // Validate params
            if (args.Length == 0 || args.Length > 1)
            {
                Log("Error: The first parameter must be the solution file path!");
                Log("");
                PrintHelp();
                return;
            }

            // Make sure the solution file exists
            if (!File.Exists(args[0]))
            {
                Log("Error: The solution file doesn't exist (" + args[0] + ")");
                Log("");
                PrintHelp();
                return;
            }

            // All things validated; run the app!
            Run(args[0]);
        }

        public static void PrintHelp()
        {
            Log("Project File Synchronization Tool v1.0");
            Log("(C) 2013 Yanick Castonguay");
            Log("");
            Log("Usage:    ProjectSync.exe [SolutionFilePath]");
            Log("Examples: ProjectSync.exe MPfm.sln");
            Log("          ProjectSync.exe C:\\Code\\MPfm\\MPfm.sln");
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run(string solutionFilePath)
        {
            string baseSolutionPath = Path.GetDirectoryName(solutionFilePath);
            Log("Reading solution file...");
            try
            {
                listProjectFiles = SolutionFileReader.ExtractProjectFilePaths(solutionFilePath);
            }
            catch (Exception ex)
            {
                Log("An error occured while reading the solution file: " + ex.Message + "\n" + ex.StackTrace);
                return;
            }

            Log("Press \'q\' to exit the application.");
            Log("");

            timer = new Timer(5000);
            timer.Elapsed += (object sender, ElapsedEventArgs e) => {
                Log("Checking for changes in files...");
                DateTime dateTimeCurrentRefresh = DateTime.Now; // Keep a timestamp from the moment we are checking files to make sure we don't miss an update
                CheckForChanges(listProjectFiles);
                dateTimeRefresh = dateTimeCurrentRefresh;
            };
            timer.Start();

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
                    timer.Stop();
                    break;
                }
            }
        }

        private static void CheckForChanges(List<string> filePaths)
        {
            // Check for changed files
            List<string> changedFiles = new List<string>();
            foreach (string filePath in filePaths)
            {
                DateTime dateTimeWrite = File.GetLastWriteTime(filePath);
                //Log("Checking project file for changes: " + filePath);
                //Log("File timestamp: " + dateTimeWrite.ToLongDateString() + " " + dateTimeWrite.ToLongTimeString());
                if (dateTimeWrite >= dateTimeRefresh)
                {
                    changedFiles.Add(filePath);
                }
            }

            // Loop through modified CSPROJ files
            foreach (string filePath in changedFiles)
            {
                // Fetch all CSPROJ files from the same directory
                Log("Changed file: " + filePath);
                string directoryPath = Path.GetDirectoryName(filePath);
                string[] csProjFiles = Directory.GetFiles(directoryPath, "*.csproj");
                
                // Make a list of project files to update
                List<string> projectFilesToUpdate = csProjFiles.ToList();
                projectFilesToUpdate.Remove(filePath);
                List<string> compiles = ProjectFileReader.ExtractProjectCompileFilePaths(filePath);
                
                // Update other CSPROJ files
                foreach (string projectFilePath in projectFilesToUpdate)
                {
                    Log("Updating project file " + projectFilePath);
                    UpdateProjectFile(projectFilePath, compiles);
                }
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
                    fileSystemWatcher.EnableRaisingEvents = false;

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
                        UpdateProjectFile(projectFilePath, compiles);
                    }

                    // Reactive file watcher
                    fileSystemWatcher.EnableRaisingEvents = true;
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

        private static void UpdateProjectFile(string filePath, List<string> compileList)
        {
            // Read XML content
            string fileContents = File.ReadAllText(filePath);
            XDocument xdoc = XDocument.Parse(fileContents);
            XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
            XElement elProject = xdoc.Element(ns + "Project");
            List<XElement> elGlobalCompiles = xdoc.Descendants(ns + "Compile").ToList();
            if (elProject == null)
                throw new Exception("Could not find the Project xml node.");
            
            // Remove Compile nodes
            foreach (XElement elCompile in elGlobalCompiles)
                elCompile.Remove();
            
            // Add a ItemGroup with all compiles
            XElement elGlobalItemGroup = new XElement(ns + "ItemGroup");
            foreach (string compile in compileList)
            {
                XElement elCompile = new XElement(ns + "Compile");
                XAttribute attrInclude = new XAttribute("Include", compile);
                elCompile.Add(attrInclude);
                elGlobalItemGroup.Add(elCompile);
            }
            elProject.Add(elGlobalItemGroup);
            
            // Remove empty ItemGroup nodes
            List<XElement> elItemGroups = xdoc.Descendants(ns + "ItemGroup").ToList();
            foreach (XElement elItemGroup in elItemGroups)
            {
                if (!elItemGroup.HasElements)
                {
                    elItemGroup.Remove();
                }
            }
            
            // Save project file
            Log("Writing project file: " + filePath);
            xdoc.Save(filePath);
        }

        private static void Log(string message)
        {
            if(!string.IsNullOrEmpty(message))
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }
    }
}
