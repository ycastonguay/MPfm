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
using System.Text;
using Sessions.Core;
using Sessions.Library.Objects;
using Sessions.MVP.Models;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.OSX.Classes.Helpers;
using MPfm.OSX.Classes.Objects;

namespace MPfm.OSX
{
    public partial class SyncMenuWindowController : BaseWindowController, ISyncMenuView
    {
        List<SyncMenuItem> _items = new List<SyncMenuItem>();
        List<AudioFile> _selection = new List<AudioFile>();

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
            ShowWindowCentered();
        }

        public override void WindowDidLoad()
        {
            // Note: Very weird, this is called before the Initialize method!
            base.WindowDidLoad();

            outlineView.WeakDelegate = this;
            outlineView.WeakDataSource = this;
            tableViewSelection.WeakDelegate = this;
            tableViewSelection.WeakDataSource = this;
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("NSOutlineViewItemDidExpandNotification"), ItemDidExpand, outlineView);

            viewTable.Hidden = true;
            LoadFontsAndImages();

            OnViewReady.Invoke(this);
        }

        public override void Close()
        {
            // TODO: Fix this, it doesn't work very well (i.e. not always called)
            Console.WriteLine("SyncMenuWindowController - Close");
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
            base.Close();
        }

        private void LoadFontsAndImages()
        {
            lblTitle.Font = NSFont.FromFontName("Roboto", 18);
            lblSubtitle.Font = NSFont.FromFontName("Roboto", 12);
            lblSubtitle2.Font = NSFont.FromFontName("Roboto", 12);
            lblLoading.Font = NSFont.FromFontName("Roboto", 13);
            lblTotal.Font = NSFont.FromFontName("Roboto", 12);
            lblFreeSpace.Font = NSFont.FromFontName("Roboto", 12);

            btnSync.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_download");
        }

        partial void actionAdd(NSObject sender)
        {
            if(outlineView.SelectedRow == -1)
                return;

            // Create list of items
            List<uint> indexes = outlineView.SelectedRows.ToList();
            List<SyncMenuItemEntity> items = new List<SyncMenuItemEntity>();
            foreach(uint index in indexes)
            {
                var test = (SyncMenuItem)outlineView.ItemAtRow((int)index);
                items.Add(test.Entity);
                //items.Add(_items[(int)index].Entity);
            }

            OnSelectItems(items);
        }

        partial void actionRemove(NSObject sender)
        {
            if(tableViewSelection.SelectedRow == -1)
                return;

            // Create list of items
            List<uint> indexes = tableViewSelection.SelectedRows.ToList();
            List<AudioFile> items = new List<AudioFile>();
            foreach(uint index in indexes)
                items.Add(_selection[(int)index]);

            OnRemoveItems(items);
        }

        partial void actionAddAll(NSObject sender)
        {
            OnSelectAll();
        }

        partial void actionRemoveAll(NSObject sender)
        {
            OnRemoveAll();
        }

        partial void actionSync(NSObject sender)
        {
            OnSync();
        }

        #region NSTableView delegate / datasource

        [Export ("numberOfRowsInTableView:")]
        public int GetRowCount(NSTableView tableView)
        {
            return _selection.Count;
        }

        [Export ("tableView:dataCellForTableColumn:row:")]
        public NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            return new NSString();
        }

        [Export ("tableView:viewForTableColumn:row:")]
        public NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            NSTableCellView view = (NSTableCellView)tableView.MakeView("cellSelection", this);
            var audioFile = _selection[row];
            string title = string.Format("{0} / {1} / {2}. {3}", audioFile.ArtistName, audioFile.AlbumTitle, audioFile.TrackNumber, audioFile.Title);
            view.TextField.StringValue = title;
            view.TextField.Font = NSFont.FromFontName("Roboto", 11);
            return view;
        }

        #endregion

        #region NSOutlineView delegate / datasource

        [Export ("outlineViewItemDidExpand")]
        public void ItemDidExpand(NSNotification notification)
        {
            Console.WriteLine("SyncMenuWindowController - ItemDidExpand");
            var item = (SyncMenuItem)notification.UserInfo["NSObject"];
            OnExpandItem(item.Entity, null);
        }

        [Export ("outlineView:isItemExpandable:")]
        public bool ItemExpandable(NSOutlineView outlineView, NSObject item)
        {
            //Console.WriteLine("SyncMenuWindowController - ItemExpandable");
            var syncMenuItem = (SyncMenuItem) item;
            if (syncMenuItem.SubItems.Count > 0)
                return true;

            return false;
        }

        [Export ("outlineView:shouldSelectItem:")]
        public bool ShouldSelectItem(NSOutlineView outlineView, NSObject item)
        {
            //Console.WriteLine("SyncMenuWindowController - ShouldSelectItem");
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
            //Console.WriteLine("SyncMenuWindowController - GetViewForItem");
            var syncMenuItem = (SyncMenuItem)item;

            NSTableCellView view = null;
            string tableColumnIdentifier = ((NSString)(tableColumn.Identifier)).ToString();
            if (tableColumnIdentifier == "columnTitle")
            {
                // Create view
                view = (NSTableCellView)outlineView.MakeView("cellTitle", this);
                view.TextField.Font = NSFont.FromFontName("Roboto", 11);

                switch (syncMenuItem.Entity.ItemType)
                {
                    case SyncMenuItemEntityType.Artist:
                        view.TextField.StringValue = syncMenuItem.Entity.ArtistName;
                        view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_user");
                        break;
                    case SyncMenuItemEntityType.Album:
                        view.TextField.StringValue = syncMenuItem.Entity.AlbumTitle;
                        view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_vinyl");
                        break;
                    case SyncMenuItemEntityType.Song:
                        view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_song");
                        if (syncMenuItem.Entity.Song != null)
                            view.TextField.StringValue = syncMenuItem.Entity.Song.Title;
                        break;
                }
            } 

            return view;
        }

        #endregion

        #region ISyncMenuView implementation

        public Action<SyncMenuItemEntity, object> OnExpandItem { get; set; }
        public Action<List<SyncMenuItemEntity>> OnSelectItems { get; set; }
        public Action<List<AudioFile>> OnRemoveItems { get; set; }
        public Action OnSync { get; set; }
        public Action OnSelectButtonClick { get; set; }
        public Action OnSelectAll { get; set; }
        public Action OnRemoveAll { get; set; }

        public void SyncMenuError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in SyncMenu: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void SyncEmptyError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", ex.Message, NSAlertStyle.Warning);
            });
        }

        public void RefreshDevice(SyncDevice device)
        {
            InvokeOnMainThread(delegate {
                lblTitle.StringValue = "Sync Library With " + device.Name;
                Window.Title = "Sync Library With " + device.Name;
            });
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            //Console.WriteLine("SyncMenuWindowController - RefreshLoading - isLoading: {0} progressPercentage: {1}", isLoading, progressPercentage);
            InvokeOnMainThread(delegate {
                progressIndicator.DoubleValue = (double)progressPercentage;
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
            // Not used on Mac.
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
            Console.WriteLine("SyncMenuWindowController - RefreshSelection - selection count: {0}", audioFiles.Count);
            InvokeOnMainThread(delegate {
                _selection = audioFiles.ToList();
                tableViewSelection.ReloadData();
            });
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
            Console.WriteLine("SyncMenuWindowController - RefreshSyncTotal");
            InvokeOnMainThread(delegate {
                lblTotal.StringValue = title;
                lblFreeSpace.StringValue = subtitle;
            });
        }

        public void InsertItems(int index, SyncMenuItemEntity parentItem, List<SyncMenuItemEntity> items, object userData)
        {
            Console.WriteLine("SyncMenuWindowController - InsertItems - index: {0} items.Count: {1}", index, items.Count);
            InvokeOnMainThread(delegate {
                var item = _items.FirstOrDefault(x => x.Entity == parentItem);
                // Try to search in subitems
                if(item == null)
                    foreach(var currentItem in _items)
                        foreach(var subItem in currentItem.SubItems)
                            if(subItem.Entity == parentItem)
                            {
                                item = subItem;
                                break;
                            }
                if(item == null)
                    return;

                // Clear dummy node and add actual items
                item.SubItems.Clear();
                foreach(var entity in items)
                {
                    var newItem = new SyncMenuItem(entity);
                    if(entity.ItemType != SyncMenuItemEntityType.Song)
                        newItem.SubItems.Add(new SyncMenuItem(new SyncMenuItemEntity(){
                            ArtistName = "dummy",
                            AlbumTitle = "dummy"
                        }));
                    item.SubItems.Add(newItem);
                }
                outlineView.ReloadData();
            });
        }

        public void RemoveItems(int index, int count, object userData)
        {
        }

        #endregion

    }
}
