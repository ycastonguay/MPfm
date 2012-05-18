using System;
using System.Reflection;
using Gtk;
using MPfm.MVP;
using Ninject;

namespace MPfm.GTK
{
	public class MainClass
	{
		private static MainWindow mainWindow = null;

		public static void Main (string[] args)
		{
			// Get current directory
			string currentDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			
			IKernel kernel = Bootstrapper.GetServiceLocator();
			
			
			Application.Init ();
			mainWindow = new MainWindow ();
			mainWindow.Icon = new Gdk.Pixbuf(currentDirectory + "/icon48.png");

			mainWindow.ShowAll();
			Application.Run();
		}
	}
}
