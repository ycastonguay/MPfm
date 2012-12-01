using System;
using System.IO;
using System.Reflection;
using Gtk;
using Ninject;
using MPfm.MVP;

namespace MPfm.GTK
{
	public class MainClass
	{
		public static MainWindow mainWindow;
		public static SplashWindow splashWindow;

		public static void Main (string[] args)
		{
			Application.Init();

			// Add view implementations to IoC
			Bootstrapper.GetKernel().Bind<ISplashView>().To<SplashWindow>();
			Bootstrapper.GetKernel().Bind<IMainView>().To<MainWindow>();
			Bootstrapper.GetKernel().Bind<IPreferencesView>().To<PreferencesWindow>();

			// Create splash view
			splashWindow = (SplashWindow)ViewFactory.CreateSplashView();
			splashWindow.Show();
			mainWindow = (MainWindow)ViewFactory.CreateMainView();
			mainWindow.Icon = new Gdk.Pixbuf(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/icon48.png");
			//mainWindow.ShowAll();
						
			Application.Run();
		}
	}
}
