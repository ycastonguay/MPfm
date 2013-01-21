//
// MPfmTableViewColumnConfig.cs: Class containing settings for a single table 
//                               view column for MPfm.
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

namespace MPfm.MVP.Config
{
    /// <summary>
    /// Class containing settings for a single table view column for MPfm.
    /// </summary>
    public class MPfmTableViewColumnConfig
    {
        public string FieldName { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public int Width { get; set; }
        public bool IsVisible { get; set; }

        public MPfmTableViewColumnConfig()
        {
            // Set defaults
            Width = 50;
            IsVisible = true;
        }
    }
}
