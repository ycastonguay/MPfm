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

namespace MPfm.Mac
{
    public partial class SyncWindowController : BaseWindowController, ISyncView
    {
        bool _isDiscovering;
        List<SyncDevice> _items = new List<SyncDevice>();

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

            tableViewDevices.RowHeight = 28;
            tableViewDevices.WeakDelegate = this;
            tableViewDevices.WeakDataSource = this;

            imageViewDeviceType.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "tablet_android_large");

            LoadFontsAndImages();
            OnViewReady.Invoke(this);
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

            lblDeviceName.Font = NSFont.FromFontName("Roboto", 13f);
            lblDeviceUrl.Font = NSFont.FromFontName("Roboto Light", 12f);
            lblPlayerStatus.Font = NSFont.FromFontName("Roboto Light", 11f);

            lblArtistName.Font = NSFont.FromFontName("Roboto Light", 15f);
            lblAlbumTitle.Font = NSFont.FromFontName("Roboto", 14f);
            lblSongTitle.Font = NSFont.FromFontName("Roboto", 13f);
            lblPosition.Font = NSFont.FromFontName("Roboto Light", 11f);

            btnRefreshDevices.StringValue = "Cancel refresh";

            btnSyncLibrary.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_library");
            btnResumePlayback.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_play");
            btnConnectManual.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_add");
            btnRefreshDevices.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_refresh");

            btnPlayPause.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_play");
            btnPrevious.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_previous");
            btnNext.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_next");
            btnRepeat.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_repeat");
            btnShuffle.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_shuffle");
        }

        private void RefreshDeviceListButton()
        {
            if (_isDiscovering)
            {
                btnRefreshDevices.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_cancel");
                btnRefreshDevices.Title = "Cancel refresh";
            }
            else
            {
                btnRefreshDevices.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_refresh");
                btnRefreshDevices.Title = "Refresh devices";
            }
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
            // Show dialog box
            //OnConnectDeviceManually();
        }

        partial void actionRefreshDevices(NSObject sender)
        {
            if (_isDiscovering)
                OnCancelDiscovery();
            else
                OnStartDiscovery();
        }

        [Export ("numberOfRowsInTableView:")]
        public int GetRowCount(NSTableView tableView)
        {
            return _items.Count;
        }

        [Export ("tableView:dataCellForTableColumn:row:")]
        public NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            return new NSString();
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
                        iconName = "icon_linux";
                        break;
                    case SyncDeviceType.OSX:
                        iconName = "icon_osx";
                        break;
                    case SyncDeviceType.Windows:
                        iconName = "icon_windows";
                        break;
                    case SyncDeviceType.iOS:
                        iconName = "icon_phone";
                        break;
                    case SyncDeviceType.Android:
                        iconName = "icon_android";
                        break;
                }
                var frameImageView = view.ImageView.Frame;
                frameImageView.Height = 24;
                frameImageView.Width = 24;
                frameImageView.Y -= 6;
                view.ImageView.Frame = frameImageView;
                view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "phone_iphone");// iconName);
                //view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == iconName);
            }

            var frame = view.TextField.Frame;
            frame.X = tableColumn.Identifier == "columnDeviceName" ? 30 : -2;
            frame.Y = view.Frame.Height - 19;
            view.TextField.Frame = frame;
            view.TextField.Font = NSFont.FromFontName("Roboto", 12);

            return view;
        }

        [Export ("tableView:rowViewForRow:")]
        private NSTableRowView GetRowView(NSTableView tableView, int row)
        {
            //Console.WriteLine("=======> GetRowView - row: {0}", row);
            var rowView = (MPfmTableRowView)tableView.MakeView(NSTableView.RowViewKey, this);
            return rowView;
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
                    RefreshDeviceListButton();
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
                RefreshDeviceListButton();
            });
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion
    }
}

