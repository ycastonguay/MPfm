using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Xml.Linq;

namespace ProjectSync
{
    public class MainClass
    {
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

            Log("Listening to file changes of the following directory: " + baseSolutionPath);
            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.Path = baseSolutionPath;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                             NotifyFilters.DirectoryName;
            //fileSystemWatcher.Filter = "*.csproj";

            fileSystemWatcher.Changed += HandleFileWatcherChanged;
            fileSystemWatcher.Created += HandleFileWatcherCreated;
            fileSystemWatcher.Deleted += HandleFileWatcherDeleted;
            fileSystemWatcher.Renamed += HandleFileWatcherRenamed;

            fileSystemWatcher.EnableRaisingEvents = true;
            Log("Press \'q\' to exit the application.");
            Log("");
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Q)
                    break;
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
                        // Read XML content
                        string fileContents = File.ReadAllText(projectFilePath);
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
                        foreach (string compile in compiles)
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

                        // Save new project file
                        //xdoc.Save(directoryPath + "\\" + "test.xml");
                        Log("Writing project file: " + projectFilePath);
                        xdoc.Save(projectFilePath);
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

        private static void Log(string message)
        {
            if(!string.IsNullOrEmpty(message))
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + message);
        }
    }
}
