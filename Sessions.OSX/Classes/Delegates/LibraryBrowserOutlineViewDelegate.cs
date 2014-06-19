// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Sessions.MVP.Models;
using Sessions.OSX.Classes.Objects;

namespace Sessions.OSX.Classes.Delegates
{
    /// <summary>
    /// Library Browser outline view delegate.
    /// </summary>
    public class LibraryBrowserOutlineViewDelegate : NSOutlineViewDelegate
    {
        readonly Action<LibraryBrowserEntity> OnTreeNodeSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sessions.OSX.LibraryBrowserOutlineViewDelegate"/> class.
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
            var outlineView = (NSOutlineView)notification.Object;
            var item = (LibraryBrowserItem)outlineView.ItemAtRow(outlineView.SelectedRow);
            if (item != null)
                OnTreeNodeSelected(item.Entity);
            else
                OnTreeNodeSelected(null);
        }

        public override void ItemDidExpand(NSNotification notification)
        {
            // TODO: Move the code for ItemExpandable here (i.e. the code that fetches the artist's album)
        }

        [Export("outlineView:viewForTableColumn:item:")]
        public NSView GetViewForItem(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
        {
            var libraryBrowserItem = (LibraryBrowserItem)item;
            var view = (NSTableCellView)outlineView.MakeView("cellLibrary", this);
            view.TextField.Font = NSFont.FromFontName("Roboto", 11);
            view.TextField.StringValue = libraryBrowserItem.Entity.Title;

            switch (libraryBrowserItem.Entity.EntityType)
            {
                case LibraryBrowserEntityType.AllSongs:
                    view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_artists");
                    break;
                case LibraryBrowserEntityType.Artists:
                    view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_artists");
                    break;
                case LibraryBrowserEntityType.Album:
                case LibraryBrowserEntityType.Albums:
                case LibraryBrowserEntityType.ArtistAlbum:
                    view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_vinyl");
                    break;
                case LibraryBrowserEntityType.Artist:
                    view.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_user");
                    break;
            } 

            return view;
        }
    }
}
