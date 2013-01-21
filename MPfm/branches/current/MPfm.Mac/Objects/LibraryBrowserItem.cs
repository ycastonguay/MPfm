//
// LibraryBrowserItem.cs: Library Browser item for the NSOutlineView.
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
using MPfm.MVP.Models;

namespace MPfm.Mac
{
    /// <summary>
    /// Library Browser item for the NSOutlineView.
    /// 
    /// Note: In MonoMac, you cannot create NSObjects in a method; you must add them
    /// to a static variable or an object, or the application will eventually crash
    /// when the garbage collector will free the NSObject.
    /// </summary>
    public class LibraryBrowserItem : NSObject
    {
        /// <summary>
        /// List of NSStrings for the Library Browser labels.
        /// In MonoMac, you cannot create NSStrings in a method; you must add them
        /// to a static variable or an object, or the application will eventually crash
        /// when the garbage collector will free the NSString.
        /// </summary>
        public NSString StringValue { get; private set; }
        public LibraryBrowserEntity Entity { get; private set; }
        public List<LibraryBrowserItem> SubItems { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.Mac.LibraryBrowserItem"/> class.
        /// </summary>
        /// <param name='entity'>Library Browser entity</param>
        public LibraryBrowserItem(LibraryBrowserEntity entity)
        {
            // Set entity and NSString value
            this.Entity = entity;
            this.StringValue = new NSString(entity.Title);

            // Create empty list of subitems
            SubItems = new List<LibraryBrowserItem>();
            foreach(LibraryBrowserEntity subEntity in entity.SubItems)
                SubItems.Add(new LibraryBrowserItem(subEntity));
        }
    }
}

