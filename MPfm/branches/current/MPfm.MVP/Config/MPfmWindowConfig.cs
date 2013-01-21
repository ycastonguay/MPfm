//
// MPfmWindowConfig.cs: Class containing all settings for a single Window for MPfm.
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
    /// Class containing all settings for a single Window for MPfm.
    /// </summary>
    public class MPfmWindowConfig
    {
        public string Title { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public bool IsMaximized { get; set; }
        public bool IsVisible { get; set; }

        public MPfmWindowConfig()
        {
            // Set defaults
            Width = 640;
            Height = 480;
        }
    }
}
