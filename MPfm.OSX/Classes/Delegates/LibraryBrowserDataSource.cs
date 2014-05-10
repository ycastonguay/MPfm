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
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MPfm.MVP;
using MPfm.MVP.Models;
using MPfm.OSX.Classes.Objects;

namespace MPfm.OSX.Classes.Delegates
{
	/// <summary>
	/// Class based on NSOutlineViewDataSource for providing the data for the Library Browser.
	/// </summary>
	public class LibraryBrowserDataSource : NSOutlineViewDataSource
	{
        readonly Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable;
		public List<LibraryBrowserItem> Items { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.OSX.LibraryBrowserDataSource"/> class.
        /// </summary>
        /// <param name='entities'>List of LibraryBrowserEntity</param>
        /// <param name='libraryBrowserPresenter'>Library Browser Presenter (necessary to get additional data for the data source)</param>
		public LibraryBrowserDataSource(IEnumerable<LibraryBrowserEntity> entities, 
                                        Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> onTreeNodeExpandable)
        {
            if(onTreeNodeExpandable == null)
                throw new ArgumentNullException("onTreeNodeExpandable");

            this.OnTreeNodeExpandable = onTreeNodeExpandable;

			// Create list of items
			Items = new List<LibraryBrowserItem>();
			foreach(LibraryBrowserEntity entity in entities)
				Items.Add(new LibraryBrowserItem(entity));
		}

		public override NSObject GetChild(NSOutlineView outlineView, int childIndex, NSObject ofItem)
		{
			// Check if this is a subitem
			if(ofItem != null) 
			{
				LibraryBrowserItem libraryBrowserItem = (LibraryBrowserItem)ofItem;
				return libraryBrowserItem.SubItems[childIndex];
			}

			return Items[childIndex];
		}

		public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
        {
            LibraryBrowserItem libraryBrowserItem = (LibraryBrowserItem)item;
			return (libraryBrowserItem.SubItems.Count > 0) ? true : false;
		}

		public override int GetChildrenCount(NSOutlineView outlineView, NSObject item)
        {
            // Check if this is a subitem
            if (item != null)
            {
                LibraryBrowserItem libraryBrowserItem = (LibraryBrowserItem)item;
                return libraryBrowserItem.SubItems.Count;
            }

            return Items.Count;
		}

		public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn forTableColumn, NSObject byItem)
		{
			// Cast item and return NSString value
            var item = (LibraryBrowserItem)byItem;
            if(forTableColumn.DataCell.Identifier == "colLibraryBrowserText")
                return item.StringValue;

            if(forTableColumn.DataCell.CellType == NSCellType.Text)
                return item.StringValue;

			return null;
		}
	}
}

