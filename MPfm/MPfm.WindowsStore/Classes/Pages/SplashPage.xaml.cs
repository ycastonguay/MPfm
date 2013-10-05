﻿// Copyright © 2011-2013 Yanick Castonguay
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
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsStore.Classes.Pages.Base;

namespace MPfm.WindowsStore.Classes.Pages
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class SplashPage : BasePage, ISplashView
    {
        public SplashPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
            Debug.WriteLine("SplashPage - LoadState");

            // View is ready
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            greetingOutput.Text = "Hello,, " + nameInput.Text + "!";

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            openPicker.ViewMode = PickerViewMode.List;

            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".flac");

            var file = await openPicker.PickSingleFileAsync();

            if (file == null)
                return;

            try
            {
                AudioFile audioFile = new AudioFile(file.Path);
                string a = audioFile.FilePath;
            }
            catch (Exception ex)
            {                
                throw;
            }            
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
        }

        public void InitDone(bool isAppFirstStart)
        {
        }
        
        #endregion

    }
}