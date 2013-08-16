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
using MPfm.MVP.Views;
using MPfm.Library.Objects;
using MPfm.MVP.Models;
using System.Text;
using MPfm.Core;
using MPfm.Mac.Classes.Helpers;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.Mac.Classes.Objects;
using System.Linq;
using MPfm.Sound.AudioFiles;

namespace MPfm.Mac
{
    public partial class SyncMenuWindowController : BaseWindowController, ISyncMenuView
    {
        List<SyncMenuItem> _items = new List<SyncMenuItem>();

        public SyncMenuWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public SyncMenuWindowController(Action<IBaseView> onViewReady)
            : base ("SyncMenuWindow", onViewReady)
        {
            Initialize();
        }

        void Initialize()
        {
            this.Window.Center();
            this.Window.MakeKeyAndOrderFront(this);
        }

        public override void WindowDidLoad()
        {
            // Note: Very weird, this is called before the Initialize method!
            base.WindowDidLoad();

            outlineView.WeakDelegate = this;
            outlineView.WeakDataSource = this;
            viewTable.Hidden = true;
            btnSync.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_download");

            OnViewReady.Invoke(this);
        }

        [Export ("outlineView:isItemExpandable:")]
        public bool ItemExpandable(NSOutlineView outlineView, NSObject item)
        {
            var syncMenuItem = (SyncMenuItem) item;
            if (syncMenuItem.SubItems.Count > 0)
                return true;

            return false;
        }

        [Export ("outlineView:shouldSelectItem:")]
        public bool ShouldSelectItem(NSOutlineView outlineView, NSObject item)
        {
            return true;
        }

        [Export ("outlineView:isGroupItem:")]
        public bool IsGroupItem(NSOutlineView outlineView, NSObject item)
        {
            return false;
        }

        [Export ("outlineView:numberOfChildrenOfItem:")]
        public int GetChildrenCount(NSOutlineView outlineView, NSObject item)
        {
            // Check if this is a subitem
            if (item != null)
            {
                var theItem = (SyncMenuItem)item;
                return theItem.SubItems.Count;
            }

            return _items.Count;
        }

        [Export ("outlineView:objectValueForTableColumn:byItem:")]
        public NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn forTableColumn, NSObject byItem)
        {
            //return byItem;
            var item = (SyncMenuItem)byItem;
            return item.StringValue;
        }

        [Export ("outlineView:child:ofItem:")]
        public NSObject GetChild(NSOutlineView outlineView, int childIndex, NSObject ofItem)
        {
            // Check if this is a subitem
            if(ofItem != null) 
            {
                var item = (SyncMenuItem)ofItem;
                return item.SubItems[childIndex];
            }

            return _items[childIndex];
        }

        [Export("outlineView:viewForTableColumn:item:")]
        public NSView GetViewForItem(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
        {
            Console.WriteLine("SyncMenuWindowController - GetViewForItem");
            var syncMenuItem = (SyncMenuItem)item;

            // Create view
            NSTableCellView view = (NSTableCellView)outlineView.MakeView("cellTitle", this);
            view.TextField.Font = NSFont.FromFontName("Junction", 11);

            string title = string.Empty;
            switch (syncMenuItem.Entity.ItemType)
            {
                case SyncMenuItemEntityType.Artist:
                    view.TextField.StringValue = syncMenuItem.Entity.ArtistName;
                    view.ImageView.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_users");
                    break;
                case SyncMenuItemEntityType.Album:
                    view.TextField.StringValue = syncMenuItem.Entity.AlbumTitle;
                    view.ImageView.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_custom_vinyl");
                    break;
                case SyncMenuItemEntityType.Song:
                    view.ImageView.Image = null;
                    if(syncMenuItem.Entity.Song != null)
                        view.TextField.StringValue = syncMenuItem.Entity.Song.Title;
                    break;
            }

            return view;
        }

        #region ISyncMenuView implementation

        public Action<SyncMenuItemEntity, object> OnExpandItem { get; set; }
        public Action<SyncMenuItemEntity> OnSelectItem { get; set; }
        public Action<AudioFile> OnRemoveItem { get; set; }
        public Action OnSync { get; set; }
        public Action OnSelectButtonClick { get; set; }
        public Action OnSelectAll { get; set; }
        public Action OnRemoveAll { get; set; }

        public void SyncMenuError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                string message = string.Format("An error occured in SyncMenu: {0}", ex);
                Tracing.Log(message);
                CocoaHelper.ShowCriticalAlert(message);
            });
        }

        public void SyncEmptyError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowCriticalAlert(ex.Message);
            });
        }

        public void RefreshDevice(SyncDevice device)
        {
            InvokeOnMainThread(delegate {
                lblTitle.StringValue = "Sync library with " + device.Name;
                Window.Title = "Sync library with " + device.Name;
            });
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            Console.WriteLine("SyncMenuWindowController - RefreshLoading - isLoading: {0} progressPercentage: {1}", isLoading, progressPercentage);
            InvokeOnMainThread(delegate {
                viewLoading.Hidden = !isLoading;
                viewTable.Hidden = isLoading;

                if(progressPercentage < 100)
                    lblLoading.StringValue = String.Format("Loading index ({0}%)...", progressPercentage);
                else
                    lblLoading.StringValue = "Processing index...";
            });
        }

        public void RefreshSelectButton(string text)
        {
        }

        public void RefreshItems(List<SyncMenuItemEntity> items)
        {
            Console.WriteLine("SyncMenuWindowController - RefreshItems - items count: {0}", items.Count);
            InvokeOnMainThread(delegate {
                _items.Clear();
                foreach(var item in items)
                {
                    var syncMenuItem = new SyncMenuItem(item);
                    syncMenuItem.SubItems.Add(new SyncMenuItem(new SyncMenuItemEntity(){
                        ArtistName = "dummy",
                        AlbumTitle = "dummy"
                    }));
                    _items.Add(syncMenuItem);
                }

                outlineView.ReloadData();
            });
        }

        public void RefreshSelection(List<AudioFile> audioFiles)
        {
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
        }

        public void InsertItems(int index, List<SyncMenuItemEntity> items, object userData)
        {
        }

        public void RemoveItems(int index, int count, object userData)
        {
        }

        #endregion

    }
}
