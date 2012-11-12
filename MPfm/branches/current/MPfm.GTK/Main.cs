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
		static MainWindow mainWindow;
		static SplashWindow splashWindow;

		public static void Main (string[] args)
		{
			Application.Init();

			// Add view implementations to IoC
			Bootstrapper.GetKernel().Bind<ISplashView>().To<SplashWindow>();
			Bootstrapper.GetKernel().Bind<IMainView>().To<MainWindow>();
			//Bootstrapper.GetKernel().Bind<IPreferencesView>().To<PreferencesWindow>();

			// Create splash view
			splashWindow = (SplashWindow)ViewFactory.CreateSplashView();
			mainWindow = (MainWindow)ViewFactory.CreateMainView();

			// Let Ninject create the MainWindow for us
			//mainWindow = Bootstrapper.GetKernel().Get<MainWindow>();
			mainWindow.Icon = new Gdk.Pixbuf(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/icon48.png");
			mainWindow.ShowAll();
						
			Application.Run();
		}
	}
}
