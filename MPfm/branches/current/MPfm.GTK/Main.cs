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
		static NavigationManager navigationManager;
		
		public static void Main (string[] args)
		{
			// Add view implementations to IoC
			Application.Init();
			Bootstrapper.GetKernel().Bind<NavigationManager>().To<GtkNavigationManager>().InSingletonScope();
			Bootstrapper.GetKernel().Bind<ISplashView>().To<SplashWindow>();
			Bootstrapper.GetKernel().Bind<IMainView>().To<MainWindow>();
			Bootstrapper.GetKernel().Bind<IUpdateLibraryView>().To<UpdateLibraryWindow>();
			Bootstrapper.GetKernel().Bind<IPreferencesView>().To<PreferencesWindow>();
			Bootstrapper.GetKernel().Bind<IEffectsView>().To<EffectsWindow>();
			Bootstrapper.GetKernel().Bind<IPlaylistView>().To<PlaylistWindow>();
			
			// Create and start navigation manager
			navigationManager = Bootstrapper.GetKernel().Get<NavigationManager>();
			
			// Start navigation
			navigationManager.Start();
			Application.Run();
		}
	}
}
