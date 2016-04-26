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
using UIKit;

namespace Sessions.iOS.Classes.Objects
{
    public class PreferenceCellItem
    {
        public string Id { get; set; }
        public PreferenceCellType CellType { get; set; }
        public string Title { get; set; }
        public string HeaderTitle { get; set; }
        public string FooterTitle { get; set; }
        public string Description { get; set; }
        public string ScaleName { get; set; }
        public string IconName { get; set; }
        public bool Enabled { get; set; }

        public object Value { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
		public Func<string, bool> ValidateValueDelegate { get; set; }
		public string ValidateFailErrorMessage { get; set; }

        public PreferenceCellItem()
        {
            Enabled = true;
        }
    }
}
