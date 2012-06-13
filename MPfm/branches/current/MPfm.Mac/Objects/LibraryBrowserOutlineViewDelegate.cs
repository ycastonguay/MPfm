//
// LibraryBrowserOutlineViewDelegate.cs: Library Browser outline view delegate.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using Ninject;

namespace MPfm.Mac
{
    /// <summary>
    /// Library Browser outline view delegate.
    /// </summary>
    public class LibraryBrowserOutlineViewDelegate : NSOutlineViewDelegate
    {
        private ILibraryBrowserPresenter libraryBrowserPresenter = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.Mac.LibraryBrowserOutlineViewDelegate"/> class.
        /// </summary>
        public LibraryBrowserOutlineViewDelegate(ILibraryBrowserPresenter libraryBrowserPresenter)
        {
            this.libraryBrowserPresenter = libraryBrowserPresenter;
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
                libraryBrowserPresenter.TreeNodeSelected(item.Entity);
        }
    }
}