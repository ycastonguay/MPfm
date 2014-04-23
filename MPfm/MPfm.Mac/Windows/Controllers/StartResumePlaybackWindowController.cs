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
using MPfm.Mac.Classes.Objects;
using System.Threading.Tasks;

namespace MPfm.Mac
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

            this.Window.Center();
            this.Window.MakeKeyAndOrderFront(this);
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            LoadFontsAndImages();
            OnViewReady(this);
        }

        private void LoadFontsAndImages()
        {
            var headerFont = NSFont.FromFontName("Roboto Bold", 16f);
            var noteFont = NSFont.FromFontName("Roboto", 12f);
            var deviceNameFont = NSFont.FromFontName("Roboto Medium", 14f);
            var playlistNameFont = NSFont.FromFontName("Roboto", 13f);
            var lastUpdatedFont = NSFont.FromFontName("Roboto", 12f);
            var artistNameFont = NSFont.FromFontName("Roboto Bold", 18f);
            var albumTitleFont = NSFont.FromFontName("Roboto Medium", 16f);
            var songTitleFont = NSFont.FromFontName("Roboto", 14f);

            lblTitle.Font = headerFont;
            lblNote.Font = noteFont;
            lblDeviceName.Font = deviceNameFont;
            lblPlaylistName.Font = playlistNameFont;
            lblArtistName.Font = artistNameFont;
            lblAlbumTitle.Font = albumTitleFont;
            lblSongTitle.Font = songTitleFont;
            lblLastUpdated.Font = lastUpdatedFont;

            imageViewDevice.Image = NSImage.ImageNamed("icon_device_android");
            btnOK.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_ok");     
            btnCancel.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_cancel");     
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

                LoadAlbumArt(audioFile);
            });
        }

        #endregion
    }
}

