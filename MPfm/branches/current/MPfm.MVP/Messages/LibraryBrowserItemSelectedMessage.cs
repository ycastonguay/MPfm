//
// LibraryBrowserItemSelectedMessage.cs: Message indicating an item has been 
//                                       selected in the Library Browser.
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

using MPfm.MVP.Models;
using TinyMessenger;

namespace MPfm.MVP.Messages
{
    /// <summary>
    /// Message indicating an item has been selected in the Library Browser.
    /// </summary>
    public class LibraryBrowserItemSelectedMessage : TinyMessageBase
    {
        public LibraryBrowserEntity Item { get; set; }

        public LibraryBrowserItemSelectedMessage(object sender) 
            : base(sender)
        {
        }
    }
}
