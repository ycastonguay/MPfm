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
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP.Views;
using MPfm.Library.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.OSX.Classes.Objects;
using System.Threading.Tasks;
using System.Drawing;

namespace MPfm.OSX
{
    public partial class StartResumePlaybackWindowController : BaseWindowController, IStartResumePlaybackView
    {
        public StartResumePlaybackWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public StartResumePlaybackWindowController(Action<IBaseView> onViewReady)
            : base ("StartResumePlaybackWindow", onViewReady)
        {
            Initialize();
        }

        private void Initialize()
        {
            btnOK.OnButtonSelected += (button) => {
                OnResumePlayback();
                Close();
            };
            btnCancel.OnButtonSelected += (button) => {
                Close();
            };

            ShowWindowCentered();
        }

        public override void WindowDidLoad()
        {
            LoadFontsAndImages();
            base.WindowDidLoad();
            OnViewReady(this);
        }

        private void LoadFontsAndImages()
        {
            viewHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;

            lblTitle.Font = NSFont.FromFontName("Roboto Light", 16f);
            lblNote.Font = NSFont.FromFontName("Roboto", 12f);
            lblDeviceName.Font = NSFont.FromFontName("Roboto", 14f);
            lblPlaylistName.Font = NSFont.FromFontName("Roboto Light", 13f);
            lblLastUpdated.Font = NSFont.FromFontName("Roboto", 12f);

            lblArtistName.Font = NSFont.FromFontName("Roboto Light", 16f);
            lblAlbumTitle.Font = NSFont.FromFontName("Roboto", 15f);
            lblSongTitle.Font = NSFont.FromFontName("Roboto", 14f);

            imageViewDevice.Image = NSImage.ImageNamed("icon_device_android");
            btnOK.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_ok");     
            btnCancel.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_cancel");     
        }

        private void LoadDeviceIcon(SyncDeviceType deviceType)
        {
            string iconName = string.Empty;
            switch (deviceType)
            {
                case SyncDeviceType.Linux:
                    iconName = "pc_linux_large";
                    break;
                case SyncDeviceType.OSX:
                    iconName = "pc_mac_large";
                    break;
                case SyncDeviceType.Windows:
                    iconName = "pc_windows_large";
                    break;
                case SyncDeviceType.iPhone:
                    iconName = "phone_iphone_large";
                    break;
                case SyncDeviceType.iPad:
                    iconName = "tablet_ipad_large";
                    break;
                case SyncDeviceType.AndroidPhone:
                    iconName = "phone_android_large";
                    break;
                case SyncDeviceType.AndroidTablet:
                    iconName = "tablet_android_large";
                    break;
                case SyncDeviceType.WindowsPhone:
                    iconName = "phone_windows_large";
                    break;
                case SyncDeviceType.WindowsStore:
                    iconName = "tablet_windows_large";
                    break;
            }
            imageViewDevice.Image = ImageResources.Images.FirstOrDefault(x => x.Name == iconName);
        }

        private async void LoadAlbumArt(AudioFile audioFile)
        {
            var task = Task<NSImage>.Factory.StartNew(() => {
                try
                {
                    NSImage image = null;
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);                        
                    using (NSData imageData = NSData.FromArray(bytesImage))
                    {
                        InvokeOnMainThread(() => {
                            image = new NSImage(imageData);
                        });
                    }
                    return image;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to process image: {0}", ex);
                }

                return null;
            });

            NSImage imageFromTask = await task;
            if(imageFromTask == null)
                return;

            InvokeOnMainThread(() => {
                try
                {
                    imageViewAlbum.Image = imageFromTask;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to set image after processing: {0}", ex);
                }
            });
        }

        #region IStartResumePlaybackView implementation

        public Action OnResumePlayback { get; set; }

        public void StartResumePlaybackError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshCloudDeviceInfo(CloudDeviceInfo info, AudioFile audioFile)
        {
            InvokeOnMainThread(() =>
            {
                lblDeviceName.StringValue = info.DeviceName;
                lblArtistName.StringValue = audioFile.ArtistName;
                lblAlbumTitle.StringValue = audioFile.AlbumTitle;
                lblSongTitle.StringValue = audioFile.Title;
                lblLastUpdated.StringValue = string.Format("Last updated: {0}", info.Timestamp);

                SyncDeviceType deviceType = SyncDeviceType.Unknown;
                Enum.TryParse<SyncDeviceType>(info.DeviceType, out deviceType);
                LoadDeviceIcon(deviceType);
                LoadAlbumArt(audioFile);
            });
        }

        #endregion
    }
}

