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

namespace Sessions.GenericControls.Controls.Items
{
    public class GridViewColumn
    {
        /// <summary>
        /// Column title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Column field name.
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
        /// Indicates if the header title should be visible or not.
        /// </summary>
        public bool IsHeaderTitleVisible { get; set; }

        public GridViewColumn(string title, string fieldName, bool visible, int order)
        {
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
            IsHeaderTitleVisible = true;
        }
    }
}
