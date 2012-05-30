using System;
using System.IO;
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
			Application.Init();
			
			// Let Ninject create the MainWindow for us
			mainWindow = Bootstrapper.GetKernel().Get<MainWindow>();						
			mainWindow.Icon = new Gdk.Pixbuf(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/icon48.png");
			mainWindow.ShowAll();
						
			Application.Run();
		}
	}
}
