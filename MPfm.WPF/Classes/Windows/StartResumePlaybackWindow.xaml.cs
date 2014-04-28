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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    public partial class StartResumePlaybackWindow : BaseWindow, IStartResumePlaybackView
    {
        private List<CloudDeviceInfo> _devices;

        public StartResumePlaybackWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            OnResumePlayback();
            Close();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region IStartResumePlaybackView implementation

        public Action OnResumePlayback { get; set; }

        public void StartResumePlaybackError(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in ResumePlayback: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void RefreshCloudDeviceInfo(CloudDeviceInfo info, AudioFile audioFile)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblDeviceName.Content = info.DeviceName;
                lblArtistName.Content = audioFile.ArtistName;
                lblAlbumTitle.Content = audioFile.AlbumTitle;
                lblSongTitle.Content = audioFile.Title;
                lblLastUpdated.Content = string.Format("Last updated: {0}", info.Timestamp);

                imageAlbum.Source = null;
                var task = Task<BitmapImage>.Factory.StartNew(() =>
                {
                    try
                    {
                        var bytes = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                        var stream = new MemoryStream(bytes);
                        stream.Seek(0, SeekOrigin.Begin);

                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        return bitmap;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occured while extracing album art in {0}: {1}", audioFile.FilePath, ex);
                    }

                    return null;
                });

                var imageResult = task.Result;
                if (imageResult != null)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        imageAlbum.Source = imageResult;
                    }));
                }
            }));
        }

        #endregion
    }
}
