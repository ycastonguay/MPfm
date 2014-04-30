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
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.Mac.Classes.Objects;
using MPfm.Mac.Classes.Helpers;
using MPfm.Mac.Classes.Controls;
using System.Timers;
using System.Threading.Tasks;

namespace MPfm.Mac
{
    public partial class SyncWindowController : BaseWindowController, ISyncView
    {
        private readonly object _locker = new object();
        private bool _isDiscovering;
        private List<SyncDevice> _items = new List<SyncDevice>();
        private Timer _timer;

        // Called when created from unmanaged code
        public SyncWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public SyncWindowController(Action<IBaseView> onViewReady)
            : base ("SyncWindow", onViewReady)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
            ShowWindowCentered();
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            _timer = new Timer(250);
            _timer.Elapsed += (sender, e) => {
                InvokeOnMainThread(() => {
                    double currentValue = progressIndicator.DoubleValue;
                    double newValue = currentValue + 1;
                    if(newValue > 100)
                        newValue = 0;
                    progressIndicator.DoubleValue = newValue;
                });
            };
            _timer.Start();

            tableViewDevices.RowHeight = 28;
            tableViewDevices.WeakDelegate = this;
            tableViewDevices.WeakDataSource = this;

            lblStatus.StringValue = "Loading...";
            LoadFontsAndImages();
            ResetFields();
            OnViewReady(this);
        }

        private void LoadFontsAndImages()
        {
            viewTitleHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewTitleHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewDetails.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewDetails.BackgroundColor2 = GlobalTheme.PanelBackgroundColor2;

            viewDeviceDetailsHeader.BackgroundColor1 = GlobalTheme.AlbumCoverBackgroundColor1;
            viewDeviceDetailsHeader.BackgroundColor2 = GlobalTheme.AlbumCoverBackgroundColor2;
            viewRemotePlayerHeader.BackgroundColor1 = GlobalTheme.AlbumCoverBackgroundColor1;
            viewRemotePlayerHeader.BackgroundColor2 = GlobalTheme.AlbumCoverBackgroundColor2;

            viewSubtitleHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewSubtitleHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewConnectManualHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewConnectManualHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;

            var headerFont = NSFont.FromFontName("Roboto Light", 16f);
            lblTitle.Font = headerFont;

            var subtitleFont = NSFont.FromFontName("Roboto Light", 13f);
            lblSubtitle.Font = subtitleFont;
            lblConnectManual.Font = subtitleFont;
            lblDeviceDetails.Font = subtitleFont;
            lblRemotePlayer.Font = subtitleFont;

            var textFont = NSFont.FromFontName("Roboto", 12f);
            var textColor = NSColor.FromDeviceRgba(0.85f, 0.85f, 0.85f, 1);
            lblConnectManualUrl.Font = textFont;
            lblConnectManualUrl.TextColor = textColor;
            lblConnectManualPort.Font = textFont;
            lblConnectManualPort.TextColor = textColor;

            var textBoxFont = NSFont.FromFontName("Roboto", 12f);
            txtConnectManualUrl.Font = textBoxFont;
            txtConnectManualPort.Font = textBoxFont;

            var noteFont = NSFont.FromFontName("Roboto", 11f);
            var noteColor = NSColor.FromDeviceRgba(0.7f, 0.7f, 0.7f, 1);
            lblLibraryUrl.Font = noteFont;
            lblLibraryUrl.TextColor = noteColor;           
            lblLastUpdated.Font = noteFont;
            lblLastUpdated.TextColor = noteColor;

            lblStatus.Font = NSFont.FromFontName("Roboto Light", 12f);
            lblDeviceName.Font = NSFont.FromFontName("Roboto", 13f);
            lblDeviceUrl.Font = NSFont.FromFontName("Roboto Light", 12f);
            lblPlayerStatus.Font = NSFont.FromFontName("Roboto Light", 11f);

            lblArtistName.Font = NSFont.FromFontName("Roboto Light", 15f);
            lblAlbumTitle.Font = NSFont.FromFontName("Roboto", 14f);
            lblSongTitle.Font = NSFont.FromFontName("Roboto", 13f);
            lblPosition.Font = NSFont.FromFontName("Roboto Light", 11f);
            lblPlaylist.Font = NSFont.FromFontName("Roboto Light", 12f);

            btnSyncLibrary.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_library");
            btnResumePlayback.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_play");
            btnConnectManual.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_add");

            btnPlayPause.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_play");
            btnPrevious.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_previous");
            btnNext.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_next");
            btnRepeat.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_repeat");
            btnShuffle.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_shuffle");
        }

        private void ResetFields()
        {
            lblDeviceName.StringValue = string.Empty;
            lblDeviceUrl.StringValue = string.Empty;
            lblArtistName.StringValue = string.Empty;
            lblAlbumTitle.StringValue = string.Empty;
            lblSongTitle.StringValue = string.Empty;
            lblPlaylist.StringValue = string.Empty;
            lblPosition.StringValue = string.Empty;
            lblLastUpdated.StringValue = string.Empty;
            lblPlayerStatus.StringValue = string.Empty;

            imageViewAlbum.Image = null;
            imageViewDeviceType.Image = null;
        }

//        partial void actionConnect(NSObject sender)
//        {
//            if(tableViewDevices.SelectedRow == -1)
//                return;
//
//            OnConnectDevice(_items[tableViewDevices.SelectedRow]);
//        }

        partial void actionConnectManual(NSObject sender)
        {
            //OnConnectDeviceManually();
        }

        [Export ("numberOfRowsInTableView:")]
        public int GetRowCount(NSTableView tableView)
        {
            return _items.Count;
        }

        [Export ("tableView:dataCellForTableColumn:row:")]
        public NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            return new NSString(_items[row].Name);
            //return new NSString();
        }

        [Export ("tableViewSelectionDidChange:")]
        public void SelectionDidChange(NSNotification notification)
        {
            //Console.WriteLine("SelectionDidChange");
            if (tableViewDevices.SelectedRow < 0)
            {
                ResetFields();
                return;
            }

            RefreshDeviceDetails(_items[tableViewDevices.SelectedRow]);
        }

        [Export ("tableView:viewForTableColumn:row:")]
        public NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            NSTableCellView view;
            if(tableColumn.Identifier.ToString() == "columnDeviceName")
            {
                view = (NSTableCellView)tableView.MakeView("cellDeviceName", this);
                view.TextField.StringValue = _items[row].Name;
            }
            else
            {
                view = (NSTableCellView)tableView.MakeView("cellDeviceDescription", this);
                view.TextField.StringValue = "Unavailable"; //_items[row].Url;
            }

            if (view.ImageView != null)
            {
                string iconName = string.Empty;
                switch (_items[row].DeviceType)
                {
                    case SyncDeviceType.Linux:
                        iconName = "pc_linux";
                        break;
                    case SyncDeviceType.OSX:
                        iconName = "pc_mac";
                        break;
                    case SyncDeviceType.Windows:
                        iconName = "pc_windows";
                        break;
                    case SyncDeviceType.iPhone:
                        iconName = "phone_iphone";
                        break;
                    case SyncDeviceType.iPad:
                        iconName = "tablet_ipad";
                        break;
                    case SyncDeviceType.AndroidPhone:
                        iconName = "phone_android";
                        break;
                    case SyncDeviceType.AndroidTablet:
                        iconName = "tablet_android";
                        break;
                    case SyncDeviceType.WindowsPhone:
                        iconName = "phone_windows";
                        break;
                    case SyncDeviceType.WindowsStore:
                        iconName = "tablet_windows";
                        break;
                }
                var frameImageView = view.ImageView.Frame;
                frameImageView.Height = 24;
                frameImageView.Width = 24;
                frameImageView.Y = view.Frame.Height - 26;
                view.ImageView.Frame = frameImageView;
                view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == iconName);
            }

            var frame = view.TextField.Frame;
            frame.X = tableColumn.Identifier == "columnDeviceName" ? 30 : -2;
            frame.Y = view.Frame.Height - 19;
            view.TextField.Frame = frame;
            view.TextField.Font = NSFont.FromFontName("Roboto", 12);

            return view;
        }

//        [Export ("tableView:rowViewForRow:")]
//        private NSTableRowView GetRowView(NSTableView tableView, int row)
//        {
//            //Console.WriteLine("=======> GetRowView - row: {0}", row);
//            var rowView = (MPfmTableRowView)tableView.MakeView(NSTableView.RowViewKey, this);
//            return rowView;
//        }

        private void RefreshDeviceDetails(SyncDevice device)
        {
            string iconName = string.Empty;
            switch (device.DeviceType)
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

            lblDeviceName.StringValue = device.Name;
            lblDeviceUrl.StringValue = string.IsNullOrEmpty(device.Url) ? "Unknown" : device.Url;
            lblPlayerStatus.StringValue = "Unavailable";
            imageViewDeviceType.Image = ImageResources.Images.FirstOrDefault(x => x.Name == iconName);
            lblLastUpdated.StringValue = string.Format("Last updated: {0}", device.LastUpdated);

            if (device.PlayerMetadata != null)
            {
                lblArtistName.StringValue = device.PlayerMetadata.CurrentAudioFile.ArtistName;
                lblAlbumTitle.StringValue = device.PlayerMetadata.CurrentAudioFile.AlbumTitle;
                lblSongTitle.StringValue = device.PlayerMetadata.CurrentAudioFile.Title;
                lblPosition.StringValue = string.Format("{0} / {1}", device.PlayerMetadata.Position, device.PlayerMetadata.Length);
                lblPlaylist.StringValue = string.Format("On-the-fly Playlist ({0}/{1})", device.PlayerMetadata.PlaylistIndex, device.PlayerMetadata.PlaylistCount);
                LoadAlbumArt(device.PlayerMetadata.AlbumArt);
            } 
            else
            {
                lblArtistName.StringValue = string.Empty;
                lblAlbumTitle.StringValue = string.Empty;
                lblSongTitle.StringValue = string.Empty;
                lblPosition.StringValue = string.Empty;
                lblPlaylist.StringValue = string.Empty;
            }
        }

        private async void LoadAlbumArt(byte[] bytesImage)
        {
            if (bytesImage == null)
            {
                imageViewAlbum.Image = null;
                return;
            }

            var task = Task<NSImage>.Factory.StartNew(() => {
                try
                {
                    NSImage image = null;
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
                    //Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to process image: {0}", ex);
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
                    //Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to set image after processing: {0}", ex);
                }
            });
        }

        #region ISyncView implementation

        public Action<SyncDevice> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }
        public Action OnStartDiscovery { get; set; }
        public Action OnCancelDiscovery { get; set; }
        public Action OnOpenConnectDevice { get; set; }

        public void SyncError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in Sync: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshIPAddress(string address)
        {
            InvokeOnMainThread(() => {
                lblLibraryUrl.StringValue = address;
            });
        }

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
            InvokeOnMainThread(() => {
                progressIndicator.DoubleValue = (double)percentageDone;
                if (!_isDiscovering)
                {
                    _isDiscovering = true;
                    progressIndicator.Hidden = false;
                }
            });
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
            InvokeOnMainThread(() => {
                Console.WriteLine("SyncWindowCtrl - RefreshDevices");
                _items = devices.ToList();
                tableViewDevices.ReloadData();
            });
        }

        public void RefreshDevicesEnded()
        {
            InvokeOnMainThread(() => {
                Console.WriteLine("SyncWindowCtrl - RefreshDevicesEnded");
                progressIndicator.Hidden = true;
                _isDiscovering = false;
            });
        }

        public void RefreshStatus(string status)
        {
            InvokeOnMainThread(() => {
                lblStatus.StringValue = status;
            });
        }

        public void NotifyAddedDevice(SyncDevice device)
        {
            // We need an approach where the whole list isn't refreshed at once or we will lose selection
            lock (_locker)
            {
                _items.Add(device);
            }

            InvokeOnMainThread(() => {
                //tableViewDevices.ReloadData(NSIndexSet.FromIndex(_items.IndexOf(device)), NSIndexSet.FromIndex(0));
                tableViewDevices.ReloadData();
            });
        }

        public void NotifyRemovedDevice(SyncDevice device)
        {
            lock (_locker)
            {
                _items.Remove(device);
            }

            InvokeOnMainThread(() => {
                tableViewDevices.ReloadData();
            });
        }

        public void NotifyUpdatedDevice(SyncDevice device)
        {
            // Should be the same instance... do we need to update the list?
            InvokeOnMainThread(() => {
                //tableViewDevices.ReloadData(NSIndexSet.FromIndex(_items.IndexOf(device)), NSIndexSet.FromIndex(0));
                int row = tableViewDevices.SelectedRow;
                tableViewDevices.ReloadData();
                if(row >= 0)
                {
                    tableViewDevices.SelectRow(row, false);

                    if(_items[row] == device)
                        RefreshDeviceDetails(device);
                }
            });
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion
    }
}

