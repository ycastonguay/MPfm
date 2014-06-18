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
using Sessions.MVP;
using Sessions.MVP.Models;

namespace MPfm.OSX.Classes.Objects
{
    /// <summary>
    /// Sync Menu item for the NSOutlineView.
    /// 
    /// Note: In MonoMac, you cannot create NSObjects in a method; you must add them
    /// to a static variable or an object, or the application will eventually crash
    /// when the garbage collector will free the NSObject.
    /// </summary>
    public class SyncMenuItem : NSObject
    {
        public NSString StringValue { get; private set; }
        public SyncMenuItemEntity Entity { get; private set; }
        public List<SyncMenuItem> SubItems { get; private set; }

        public SyncMenuItem(SyncMenuItemEntity entity)
        {
            Entity = entity;
            StringValue = new NSString(entity.ItemType.ToString());
            SubItems = new List<SyncMenuItem>();
        }
    }
}
