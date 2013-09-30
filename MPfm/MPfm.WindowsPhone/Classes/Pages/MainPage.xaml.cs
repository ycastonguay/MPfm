// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.Storage;
using MPfm.Library.Services;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsPhone.Classes.Controls;
using MPfm.WindowsPhone.Classes.Helpers;
using MPfm.WindowsPhone.Classes.Navigation;
using MPfm.WindowsPhone.Classes.Pages.Base;

namespace MPfm.WindowsPhone.Classes.Pages
{
    public partial class MainPage : BasePage, IMobileOptionsMenuView
    {
        private SyncDiscoveryService _syncDiscoveryService;
        private WindowsPhoneNavigationManager _navigationManager;
        private List<KeyValuePair<MobileOptionsMenuType, string>> _menuOptions;

        public MainPage()
        {
            InitializeComponent();
            SetTheme(LayoutRoot);

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            //_syncDiscoveryService = Bootstrapper.GetContainer().Resolve<SyncDiscoveryService>();
            //_syncDiscoveryService.SearchForDevices("192.168.1");

            CreateDummyData();

            Debug.WriteLine("MainPage - Ctor - Starting navigation manager...");
            _navigationManager = (WindowsPhoneNavigationManager) Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            _navigationManager.BindOptionsMenuView(this);
            _navigationManager.Start();
        }

        private void CreateDummyData()
        {
            List<string> list = new List<string>();
            list.Add("Arcade Fire");
            list.Add("Bob Marley & The Wailers");
            list.Add("Can");
            list.Add("David Bowie");
            list.Add("James Blake");
            list.Add("John Coltrane");
            list.Add("Miles Davis");
            list.Add("Neil Young");
            list.Add("Pink Floyd");
            list.Add("The Beatles");
            list.Add("The Clash");
            list.Add("The Fall");
            list.Add("The Jesus and Mary Chain");
            list.Add("The Vibrators");
            list.Add("Zappa Frank");
            listArtists.ItemsSource = list;

            //List<string> list2 = new List<string>();
            //list2.Add("sync library (other devices)");
            //list2.Add("sync library (cloud services)");
            //list2.Add("sync library (web browser)");
            //list2.Add("equalizer presets");
            //list2.Add("preferences");
            //list2.Add("about sessions");
            //listMore.ItemsSource = list2;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            list.Add("test");
            list.Add("test2");
            listAlbums.ItemsSource = list;
            return;

            var folder = ApplicationData.Current.LocalFolder;
            var audioFolder = await folder.GetFolderAsync("Audio");
            var files = await audioFolder.GetFilesAsync();
            foreach (var file in files)
            {
                if (file.Path.Contains(".mp3"))
                {
                    Debug.WriteLine("Definitely a mp3: " + file.Path);
                    AudioFile audioFile = new AudioFile(file.Path);
                    txtStuff.Text = audioFile.Title;
                }
                else
                {
                    Debug.WriteLine("Not mp3: " + file.Path);
                }                
            }

            //CreateDummyFile();
            //AudioFile audioFile = new AudioFile();
        }

        private async Task WriteToFile()
        {
            // Get the text data from the textbox. 
            string data = "hello world";
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(data.ToCharArray());

            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create dummy data file
            var dataFolder = await local.CreateFolderAsync("Audio", CreationCollisionOption.OpenIfExists);
            var file = await dataFolder.CreateFileAsync("DataFile.txt", CreationCollisionOption.ReplaceExisting);
            using (var s = await file.OpenStreamForWriteAsync())
                s.Write(fileBytes, 0, fileBytes.Length);
        }

        private void listArtists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get item of LongListSelector. 
            List<UserControl> listItems = new List<UserControl>();
            XamlHelper.GetItemsRecursive<UserControl>(listArtists, ref listItems);

            // Selected
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
                foreach (UserControl userControl in listItems)
                    if (e.AddedItems[0].Equals(userControl.DataContext))
                        VisualStateManager.GoToState(userControl, "Selected", true);
            // Unselected 
            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] != null)
                foreach (UserControl userControl in listItems)
                    if (e.RemovedItems[0].Equals(userControl.DataContext))
                        VisualStateManager.GoToState(userControl, "Normal", true);

            //RootFrame.Navigate(new Uri("/Classes/Pages/MainPage.xaml", UriKind.Relative));
            //var mobileNavigationManager = (WindowsPhoneNavigationManager) Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            //mobileNavigationManager.CreateSyncView();
            //e.AddedItems[]
        }

        private void listMore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get item of LongListSelector. 
            List<UserControl> userControlList = new List<UserControl>();
            XamlHelper.GetItemsRecursive<UserControl>(listMore, ref userControlList);

            // Selected
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
                foreach (UserControl userControl in userControlList)
                    if (e.AddedItems[0].Equals(userControl.DataContext))
                        VisualStateManager.GoToState(userControl, "Selected", true);
            // Unselected
            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] != null)
                foreach (UserControl userControl in userControlList)
                    if (e.RemovedItems[0].Equals(userControl.DataContext))
                        VisualStateManager.GoToState(userControl, "Normal", true);

            var addedItem = e.AddedItems.Count > 0 ? e.AddedItems[0] : null;
            if (addedItem == null)
                return;

            var item = _menuOptions.FirstOrDefault(x => x.Value.ToLower() == (string)addedItem);
            if (!item.Equals(default(KeyValuePair<MobileOptionsMenuType, string>)))
                OnItemClick(item.Key);
        } 

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        #region IMobileOptionsMenuView implementation

        public Action<MobileOptionsMenuType> OnItemClick { get; set; }

        public void RefreshMenu(List<KeyValuePair<MobileOptionsMenuType, string>> options)
        {
            _menuOptions = options;
            Dispatcher.BeginInvoke(() =>
            {
                var list = _menuOptions.Select(x => x.Value.ToLower()).ToList();
                listMore.ItemsSource = list;                
            });
        }

        #endregion

    }
}