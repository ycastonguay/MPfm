//
// SongBrowserSource.cs: Class based on NSTableViewSource for providing 
//                       the data and cells for the Song Browser.
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
using MPfm.Sound;
using System.Drawing;

namespace MPfm.Mac
{
    /// <summary>
    /// Class based on NSTableViewSource for providing the data and cells for the Song Browser.
    /// </summary>
	public class SongBrowserSource : NSTableViewSource
	{
		public List<SongBrowserItem> Items { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.Mac.SongBrowserSource"/> class.
        /// </summary>
        /// <param name='audioFiles'>List of AudioFile</param>
		public SongBrowserSource(IEnumerable<AudioFile> audioFiles)
        {
			// Create list of items
			Items = new List<SongBrowserItem>();
			foreach(AudioFile audioFile in audioFiles)
				Items.Add(new SongBrowserItem(audioFile));
		}

        public override float GetRowHeight(NSTableView tableView, int row)
        {
            return 18;
        }

        public override int GetRowCount(NSTableView tableView)
        {
            return Items.Count;
        }

        public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            // Get table column name
            string tableColumnName = ((string)(NSString)(tableColumn.Identifier)).Replace("column", "");

            // TEMP
            if(tableColumnName == "IsPlaying")
            {
                // If something else than NSImage is returned, the application crashes...
                //return new NSImage(new SizeF(16, 16));
                return ImageResources.images16x16[0];
            }

            return Items[row].KeyValues[tableColumnName];
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            MPfmTableCellView view = (MPfmTableCellView)tableView.MakeView("tableCellView", this);
            return view;
        }      

//        public override NSTableRowView GetRowView(NSTableView tableView, int row)
//        {
//            //MPfmTableRowView view = new MPfmTableRowView();
//            //NSTableRowView rowView = (NSTableRowView)tableView.MakeView(NSTableView.RowViewKey, this);
//            MPfmTableRowView rowView = (MPfmTableRowView)tableView.MakeView(NSTableView.RowViewKey, this);
//            return rowView;
//        }
	}
}
