//
// SongGridViewColumn.cs: Column for the SongGridView control.
//
// Copyright © 2011 Yanick Castonguay
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
using System.Drawing;
using System.Linq;
using System.Text;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Column for the SongGridView control.
    /// </summary>
    public class SongGridViewColumn
    {
        /// <summary>
        /// Column title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Column field name (related to the AudioFile class).
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Indicates if the column is visible or not.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Column order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Column width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Indicates if the column can be moved.
        /// </summary>
        public bool CanBeMoved { get; set; }

        /// <summary>
        /// Indicates if the column can be resized.
        /// </summary>
        public bool CanBeResized { get; set; }
        
        /// <summary>
        /// Indicates if the column can be reordered.
        /// </summary>
        public bool CanBeReordered { get; set; }

        /// <summary>
        /// Indicates if the mouse cursor is currently over the column header.
        /// This property is set automatically by the song grid view control.
        /// </summary>
        public bool IsMouseOverColumnHeader { get; set; }

        /// <summary>
        /// Indicates if the mouse cursor is over the column limit (i.e. for column resizing).
        /// This property is set automatically by the song grid view control.
        /// </summary>
        public bool IsMouseCursorOverColumnLimit { get; set; }

        /// <summary>
        /// Indicates if the user is currently resizing the column.
        /// This property is set automatically by the song grid view control.
        /// </summary>
        public bool IsUserResizingColumn { get; set; }

        /// <summary>
        /// Indicates if the user is currently moving the column.
        /// This property is set automatically by the song grid view control.
        /// </summary>
        public bool IsUserMovingColumn { get; set; }

        /// <summary>
        /// Constructor for the SongGridViewColumn class.
        /// </summary>
        /// <param name="title">Column title</param>
        /// <param name="fieldName">Column field title (AudioFile)</param>
        public SongGridViewColumn(string title, string fieldName)
        {
            // Set properties
            Title = title;
            FieldName = fieldName;
            Visible = false;
            Order = -1;
            Width = 100;
            CanBeResized = true;
            CanBeReordered = true;
            CanBeMoved = true;
            IsMouseOverColumnHeader = false;
            IsMouseCursorOverColumnLimit = false;
            IsUserResizingColumn = false;
            IsUserMovingColumn = false;
        }

        /// <summary>
        /// Constructor for the SongGridViewColumn class.
        /// </summary>
        /// <param name="title">Column title</param>
        /// <param name="fieldName">Column field title (AudioFile)</param>
        /// <param name="visible">Indicates if the column is visible</param>
        /// <param name="order">Column order</param>
        public SongGridViewColumn(string title, string fieldName, bool visible, int order)
        {
            // Set properties
            Title = title;
            FieldName = fieldName;
            Visible = visible;
            Order = order;
            Width = 100;
            CanBeResized = true;
            CanBeReordered = true;
            CanBeMoved = true;
            IsMouseOverColumnHeader = false;
            IsMouseCursorOverColumnLimit = false;
            IsUserResizingColumn = false;
            IsUserMovingColumn = false;
        }
    }
}
