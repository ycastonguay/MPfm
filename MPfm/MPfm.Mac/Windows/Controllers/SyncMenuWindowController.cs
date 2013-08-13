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

namespace MPfm.Mac
{
    public partial class SyncMenuWindowController : BaseWindowController, ISyncMenuView
    {
        List<SyncMenuItemEntity> _items;

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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            viewTable.Hidden = true;
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            outlineView.WeakDelegate = this;
            outlineView.WeakDataSource = this;

            OnViewReady.Invoke(this);
        }

        [Export ("numberOfRowsInTableView:")]
        public int GetRowCount(NSTableView tableView)
        {
            return _items.Count;
        }

        [Export ("tableView:heightOfRow:")]
        public float GetRowHeight(NSTableView tableView, int row)
        {
            return 20;
        }

        [Export ("tableView:dataCellForTableColumn:row:")]
        public NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            return new NSString();
        }

//        [Export ("tableView:viewForTableColumn:row:")]
//        public NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
        
        [Export("outlineView:viewForTableColumn:item:")]
        public NSView GetViewForItem(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
        {
            //Console.WriteLine("GetViewForItem - tableColumn: {0} row: {1}", tableColumn.Identifier.ToString(), row);
            NSTableCellView view;
            if(tableColumn.Identifier.ToString() == "columnTitle")
            {
                view = (NSTableCellView)outlineView.MakeView("cellTitle", this);
                //view.TextField.StringValue = _items[row].ArtistName;
            }
            else
            {
                view = (NSTableCellView)outlineView.MakeView("cellSelection", this);
                //view.TextField.StringValue = _items[row].Url;
            }
//            //view.TextField.Font = NSFont.FromFontName("Junction", 11);
//
//            if (view.ImageView != null)
//            {
//                string iconName = string.Empty;
//                switch (_items[row].ItemType)
//                {
//                    case SyncMenuItemEntityType.Artist:
//                        iconName = "16_icomoon_android";
//                        break;
//                    case SyncMenuItemEntityType.Album:
//                        iconName = "16_icomoon_android";
//                        break;
//                    case SyncMenuItemEntityType.Song:
//                        iconName = "16_icomoon_android";
//                        break;
//                }
//                view.ImageView.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == iconName);
//            }
            return view;
        }

        [Export ("tableViewSelectionDidChange:")]
        public void SelectionDidChange(NSNotification notification)
        {         
            //btnConnect.Enabled = (tableViewDevices.SelectedRow == -1) ? false : true;
        }

        [Export ("outlineView:numberOfChildrenOfItem:")]
        public int GetChildrenCount(NSOutlineView outlineView, NSObject item)
        {
            return 0;
        }

        [Export ("outlineView:objectValueForTableColumn:byItem:")]
        public NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn forTableColumn, NSObject byItem)
        {
            return new NSObject();
        }

        [Export ("outlineView:child:ofItem:")]
        public NSObject GetChild(NSOutlineView outlineView, int childIndex, NSObject ofItem)
        {
            return new NSObject();
        }

        [Export ("outlineView:isItemExpandable:")]
        public bool ItemExpandable(NSOutlineView outlineView, NSObject item)
        {
            return false;
        }

        #region ISyncMenuView implementation

        public Action<SyncMenuItemEntity> OnExpandItem { get; set; }
        public Action<SyncMenuItemEntity> OnSelectItem { get; set; }
        public Action OnSync { get; set; }
        public Action OnSelectButtonClick { get; set; }

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
            InvokeOnMainThread(delegate {
                viewLoading.Hidden = !isLoading;
                viewTable.Hidden = isLoading;

                if(!isLoading)
                    Console.WriteLine("VIEW TABLE NOT HIDDEN ANYMORE");

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
            Console.WriteLine("REFRESITEMS");
            InvokeOnMainThread(delegate {
                _items = items.ToList();
                outlineView.ReloadData();
            });
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
        }

        public void InsertItems(int index, List<SyncMenuItemEntity> items)
        {
        }

        public void RemoveItems(int index, int count)
        {
        }

        #endregion
    }
}
