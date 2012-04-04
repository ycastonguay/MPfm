using System;
using System.Reflection;
using Gtk;

namespace MPfm
{
	public class MainClass
	{
		private static MainWindow mainWindow = null;

		public static void Main (string[] args)
		{
			// Get current directory
			string currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			Application.Init ();
			mainWindow = new MainWindow ();
			mainWindow.Icon = new Gdk.Pixbuf(currentDirectory + "/icon48.png");

			mainWindow.ShowAll();
			Application.Run();
		}
	}
}
