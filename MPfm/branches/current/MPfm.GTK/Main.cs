using System;
using System.IO;
using System.Reflection;
using Gtk;
using MPfm.MVP;
using MPfm.MVP.Views;

namespace MPfm.GTK
{
	public class MainClass
	{
		static NavigationManager navigationManager;
		
		public static void Main (string[] args)
		{
			// Add view implementations to IoC
			Application.Init();
			Bootstrapper.GetContainer().Register<NavigationManager, GtkNavigationManager>().AsSingleton();
			Bootstrapper.GetContainer().Register<ISplashView, SplashWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IMainView, MainWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IUpdateLibraryView, UpdateLibraryWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IPreferencesView, PreferencesWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IEffectsView, EffectsWindow>().AsMultiInstance();
			Bootstrapper.GetContainer().Register<IPlaylistView, PlaylistWindow>().AsMultiInstance();
			
			// Create and start navigation manager
			navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
			navigationManager.Start();
			Application.Run();
		}
	}
}
