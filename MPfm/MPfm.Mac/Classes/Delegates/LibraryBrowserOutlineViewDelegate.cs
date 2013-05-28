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
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MPfm.MVP;
using MPfm.MVP.Models;
using MPfm.Mac.Classes.Objects;

namespace MPfm.Mac.Classes.Delegates
{
    /// <summary>
    /// Library Browser outline view delegate.
    /// </summary>
    public class LibraryBrowserOutlineViewDelegate : NSOutlineViewDelegate
    {
        readonly Action<LibraryBrowserEntity> OnTreeNodeSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.Mac.LibraryBrowserOutlineViewDelegate"/> class.
        /// </summary>
        public LibraryBrowserOutlineViewDelegate(Action<LibraryBrowserEntity> onTreeNodeSelected)
        {
            if(onTreeNodeSelected == null)
                throw new ArgumentNullException("onTreeNodeSelected");

            this.OnTreeNodeSelected = onTreeNodeSelected;
        }

        /// <summary>
        /// Occurs when the selected row index changes.
        /// </summary>
        /// <param name='notification'>Notification</param>
        public override void SelectionDidChange(NSNotification notification)
        {
            // Get selected row and call presenter
            NSOutlineView outlineView = (NSOutlineView)notification.Object;
            LibraryBrowserItem item = (LibraryBrowserItem)outlineView.ItemAtRow(outlineView.SelectedRow);

            // Call presenter if a valid item has been found
            if(item != null)
                OnTreeNodeSelected.Invoke(item.Entity);
        }

        [Export("outlineView:viewForTableColumn:item:")]
        public NSView GetViewForItem(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
        {
            // Cast item
            LibraryBrowserItem libraryBrowserItem = (LibraryBrowserItem)item;

            // Create view
            NSTableCellView view = (NSTableCellView)outlineView.MakeView("cellLibrary", this);
            view.TextField.Font = NSFont.FromFontName("Junction", 11);
            view.TextField.StringValue = libraryBrowserItem.Entity.Title;

            // Check icon
            if (libraryBrowserItem.Entity.Type == LibraryBrowserEntityType.AllSongs)
            {
                view.ImageView.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_cabinet");
            } 
            else if (libraryBrowserItem.Entity.Type == LibraryBrowserEntityType.Artists)
            {
                view.ImageView.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_users");
            } 
            else if (libraryBrowserItem.Entity.Type == LibraryBrowserEntityType.Album ||
                     libraryBrowserItem.Entity.Type == LibraryBrowserEntityType.Albums)
            {
                view.ImageView.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_tango_cd");
            } 
            else if (libraryBrowserItem.Entity.Type == LibraryBrowserEntityType.Artist)
            {
                view.ImageView.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_user");
            } 

            return view;
        }
    }
}
