using System;
using System.IO;
using System.Security.Permissions;

namespace ProjectSync
{
	public class MainClass
	{
		public static void Main(string[] args)
        {
            // Validate params
            if (args.Length == 0 || args.Length > 1)
            {
                Console.WriteLine("Error: The first parameter must be the solution file path!");
                Console.WriteLine();
                PrintHelp();
                return;
            }

            // Make sure the solution file exists
            if (!File.Exists(args [0]))
            {
                Console.WriteLine("Error: The solution file doesn't exist (" + args[0] + ")");
                Console.WriteLine();
                PrintHelp();
                return;
            }

            // All things validated; run the app!
			Run();
		}

        public static void PrintHelp()
        {
            Console.WriteLine("Project File Synchronization Tool v1.0");
            Console.WriteLine("(C) 2013 Yanick Castonguay");
            Console.WriteLine();
            Console.WriteLine("Usage:   ProjectSync.exe [SolutionFilePath]");
            Console.WriteLine("Example: ProjectSync.exe MPfm.sln");
        }

		[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
		public static void Run()
		{
			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path = "";
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Filter = "*.csproj";

			watcher.Changed += HandleFileWatcherChanged;
			watcher.Created += HandleFileWatcherCreated;
			watcher.Deleted += HandleFileWatcherDeleted;
			watcher.Renamed += HandleFileWatcherRenamed;
		}

		static void HandleFileWatcherChanged(object sender, FileSystemEventArgs e)
		{

		}

        static void HandleFileWatcherCreated(object sender, FileSystemEventArgs e)
        {
            
        }

        static void HandleFileWatcherDeleted(object sender, FileSystemEventArgs e)
        {
            
        }

        static void HandleFileWatcherRenamed(object sender, RenamedEventArgs e)
        {
            
        }
	}
}