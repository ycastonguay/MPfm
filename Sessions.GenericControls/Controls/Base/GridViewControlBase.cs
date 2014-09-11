// Copyri3w2qght © 2011-2013 Yanick Castonguay
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Items;
using Sessions.GenericControls.Wrappers;
using Sessions.GenericControls.Interaction;

namespace Sessions.GenericControls.Controls.Base
{
    /// <summary>
    /// Base class for grid view style controls; adds the concept of columns.
    /// </summary>
    public abstract class GridViewControlBase<T, U> : ListViewControlBase<T> where T : ListViewItem where U : GridViewColumn  //, IControlMouseInteraction, IControlKeyboardInteraction
    {
        private const int MinimumColumnWidth = 30;
    
        private List<U> _columns;
        [Browsable(false)]
        public List<U> Columns
        {
            get
            {
                return _columns;
            }
        }

        private string _orderByFieldName = string.Empty;
        /// <summary>
        /// Indicates which field should be used for ordering songs.
        /// </summary>
        public string OrderByFieldName
        {
            get
            {
                return _orderByFieldName;
            }
            set
            {
                _orderByFieldName = value;
                Items.Clear();
                GridCache = null;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Indicates if the order should be ascending (true) or descending (false).
        /// </summary>
        public bool OrderByAscending { get; set; }

        /// <summary>
        /// Indicates if a column is currently moving.
        /// </summary>
        public bool IsColumnMoving
        {
            get
            {
                return Columns.Any(column => column.IsUserMovingColumn);
            }
        }

        /// <summary>
        /// Indicates if a column is currently resizing.
        /// </summary>
        public bool IsColumnResizing
        {
            get
            {
                return Columns.Any(column => column.IsUserResizingColumn);
            }
        }

        /// <summary>
        /// Indicates if the user can resize the columns or not.
        /// </summary>
        public bool CanResizeColumns { get; set; }

        /// <summary>
        /// Indicates if the user can move the columns or not.
        /// </summary>
        public bool CanMoveColumns { get; set; }

        /// <summary>
        /// Indicates if the user can change the order by or not.
        /// </summary>
        public bool CanChangeOrderBy { get; set; }

        /// <summary>
        /// Cache for GridView.
        /// </summary>
        protected GridViewCache<U> GridCache { get; set; }

        protected int DragOriginalColumnWidth { get; set; }
        protected int ColumnMoveMarkerX { get; set; }

        public delegate void ColumnClickDelegate(int index);
        public event ColumnClickDelegate OnColumnClick;

        protected GridViewControlBase(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar) 
            : base(horizontalScrollBar, verticalScrollBar)
        {
            OrderByAscending = true;
            GridCache = null;
            CanResizeColumns = true;
            CanMoveColumns = true;
            CanChangeOrderBy = true;
            _columns = new List<U>();
        }

        protected void ColumnClick(int index)
        {
            if (OnColumnClick != null)
                OnColumnClick(index);
        }

        protected virtual void DetermineVisibleLineIndexes()
        {
            // Calculate how many lines must be skipped because of the scrollbar position
            StartLineNumber = Math.Max((int) Math.Floor((double) VerticalScrollBar.Value/(double) (ListCache.LineHeight)), 0);

            // Check if the total number of lines exceeds the number of icons fitting in height
            NumberOfLinesToDraw = 0;
            if (StartLineNumber + ListCache.NumberOfLinesFittingInControl > Items.Count)
            {
                // There aren't enough lines to fill the screen
                NumberOfLinesToDraw = Items.Count - StartLineNumber;
            }
            else
            {
                // Fill up screen 
                NumberOfLinesToDraw = ListCache.NumberOfLinesFittingInControl;
            }

            // Add one line for overflow; however, make sure we aren't adding a line without content 
            if (StartLineNumber + NumberOfLinesToDraw + 1 <= Items.Count)
                NumberOfLinesToDraw++;
        }

        /// <summary>
        /// Creates a cache of values used for rendering the grid view.
        /// Also sets scrollbar position, height, value, maximum, etc.
        /// </summary>
        public void InvalidateGridViewCache()
        {
            // Check if columns have been created
            if (Columns == null || Columns.Count == 0 || Items == null)
                return;

            InvalidateListViewCache();

            // Create cache
            GridCache = new GridViewCache<U>();

            // Get active columns and order them
            GridCache.ActiveColumns = new List<U>(Columns.Where(x => x.Order >= 0).OrderBy(x => x.Order).ToList());

            //string allChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm!@#$%^&*()";
            //var rectText = context.MeasureText(allChars, new BasicRectangle(0, 0, 1000, 100), "Roboto", 12);
            var rectText = new BasicRectangle(0, 0, 100, 14);

            // Check if the total active columns width exceed the width available in the control
            GridCache.TotalWidth = 0;
            for (int a = 0; a < GridCache.ActiveColumns.Count; a++)
                if (GridCache.ActiveColumns[a].Visible)
                    GridCache.TotalWidth += GridCache.ActiveColumns[a].Width;

            // Check width
            if (GridCache.TotalWidth > Frame.Width - VerticalScrollBar.Width)
            {
                // Set scrollbar values
                HorizontalScrollBar.Maximum = GridCache.TotalWidth;
                HorizontalScrollBar.SmallChange = 5;
                HorizontalScrollBar.LargeChange = (int)Frame.Width;
                HorizontalScrollBar.Visible = true;
            }

            // Check if the horizontal scrollbar needs to be turned off
            if (GridCache.TotalWidth <= Frame.Width - VerticalScrollBar.Width && HorizontalScrollBar.Visible)
                HorizontalScrollBar.Visible = false;
        }

        public override void MouseDown(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            base.MouseDown(x, y, button, keysHeld);

            if (Columns == null || GridCache == null)
                return;

            // Loop through columns
            foreach (var column in GridCache.ActiveColumns)
            {
                // Check for resizing column
                if (column.IsMouseCursorOverColumnLimit && column.CanBeResized && CanResizeColumns)
                {
                    column.IsUserResizingColumn = true;
                    DragOriginalColumnWidth = column.Width;
                }
            }                
        }

        public override void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            base.MouseUp(x, y, button, keysHeld);
        
            bool controlNeedsToBeFullyInvalidated = false;

            if (Columns == null || GridCache == null)
                return;

            // Get reference to the moving column
            U columnMoving = null;
            foreach (var column in GridCache.ActiveColumns)
            {
                column.IsUserResizingColumn = false;
                if (column.IsUserMovingColumn)
                    columnMoving = column;
            }

            // Check if the user is moving a column
            if (columnMoving != null)
            {
                columnMoving.IsUserMovingColumn = false;
                controlNeedsToBeFullyInvalidated = true;

                // Find out on what column the mouse cursor is
                GridViewColumn columnOver = null;
                int currentX = 0;
                bool isPastCurrentlyMovingColumn = false;
                for (int a = 0; a < GridCache.ActiveColumns.Count; a++)
                {
                    var currentColumn = GridCache.ActiveColumns[a];
                    if (currentColumn.FieldName == columnMoving.FieldName)
                        isPastCurrentlyMovingColumn = true;

                    if (currentColumn.Visible)
                    {
                        // Check if the cursor is over the left part of the column
                        if (x >= currentX - HorizontalScrollBar.Value &&
                            x <= currentX + (currentColumn.Width/2) - HorizontalScrollBar.Value)
                        {
                            if (isPastCurrentlyMovingColumn && currentColumn.FieldName != columnMoving.FieldName)
                                columnOver = GridCache.ActiveColumns[a - 1];
                            else
                                columnOver = GridCache.ActiveColumns[a];
                            break;
                        }
                            // Check if the cursor is over the right part of the column
                        else if (x >= currentX + (currentColumn.Width/2) - HorizontalScrollBar.Value &&
                                 x <= currentX + currentColumn.Width - HorizontalScrollBar.Value)
                        {
                            // Check if there is a next item
                            if (a < GridCache.ActiveColumns.Count - 1)
                            {
                                if (isPastCurrentlyMovingColumn)
                                    columnOver = GridCache.ActiveColumns[a];
                                else
                                    columnOver = GridCache.ActiveColumns[a + 1];
                            }
                            break;
                        }

                        // Increment x
                        currentX += currentColumn.Width;
                    }
                }

                //// Check if the column was found (the cursor might be past the last column
                //if (columnOver == null)
                //{
                //    return;
                //}

                // Order columns by their current order
                var columnsOrdered = Columns.OrderBy(q => q.Order).ToList();

                // Move column
                int indexRemove = -1;
                int indexAdd = -1;
                for (int a = 0; a < columnsOrdered.Count; a++)
                {
                    // Find the moving column index
                    if (columnsOrdered[a].FieldName == columnMoving.FieldName)
                        indexRemove = a;

                    // Find the column index with the mouse over
                    if (columnOver != null && columnsOrdered[a].FieldName == columnOver.FieldName)
                        indexAdd = a;
                }

                // Remove column
                columnsOrdered.RemoveAt(indexRemove);

                // Check if the item needs to be inserted at the end
                if (indexAdd == -1)
                    columnsOrdered.Insert(columnsOrdered.Count, columnMoving);
                else
                    columnsOrdered.Insert(indexAdd, columnMoving);

                // Loop through columns to change the order of columnns
                for (int a = 0; a < columnsOrdered.Count; a++)
                    columnsOrdered[a].Order = a;
            }

            if (controlNeedsToBeFullyInvalidated)
            {
                InvalidateGridViewCache();
                InvalidateVisual();
            }
        }

        public override void MouseLeave()
        {
            base.MouseLeave();

            if (Columns == null || GridCache == null)
                return;

            bool controlNeedsToBePartiallyInvalidated = false;
            var partialRect = new BasicRectangle();

            // Reset column flags
            int columnOffsetX2 = 0;
            for (int b = 0; b < GridCache.ActiveColumns.Count; b++)
            {
                if (GridCache.ActiveColumns[b].Visible)
                {
                    if (GridCache.ActiveColumns[b].IsMouseOverColumnHeader)
                    {
                        GridCache.ActiveColumns[b].IsMouseOverColumnHeader = false;
                        var newPartialRect = new BasicRectangle(columnOffsetX2 - HorizontalScrollBar.Value, 0, GridCache.ActiveColumns[b].Width, ListCache.LineHeight);
                        partialRect.Merge(newPartialRect);
                        controlNeedsToBePartiallyInvalidated = true;
                    }

                    columnOffsetX2 += GridCache.ActiveColumns[b].Width;
                }
            }

            if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);        
        }

        public override void MouseClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            base.MouseClick(x, y, button, keysHeld);

            bool controlNeedsToBeFullyInvalidated = false;
            bool controlNeedsToBePartiallyInvalidated = false;
            var partialRect = new BasicRectangle();

            if (Columns == null || GridCache == null)
                return;

            // Show context menu strip if the button click is right and not the album art column
            if (button == MouseButtonType.Right && x > Columns[0].Width && y > ListCache.LineHeight)
                DisplayContextMenu(ContextMenuType.Item, x, y);

            var columnResizing = Columns.FirstOrDefault(col => col.IsUserResizingColumn == true);

            // Check if the user has clicked on the header (for orderBy)
            if (y >= 0 && y <= ListCache.LineHeight &&
                columnResizing == null && !IsColumnMoving)
            {
                // Check on what column the user has clicked
                int offsetX = 0;
                for (int a = 0; a < GridCache.ActiveColumns.Count; a++)
                {
                    var column = GridCache.ActiveColumns[a];
                    if (column.Visible)
                    {
                        // Check if the mouse pointer is over this column
                        if (x >= offsetX - HorizontalScrollBar.Value && x <= offsetX + column.Width - HorizontalScrollBar.Value)
                        {
                            if (button == MouseButtonType.Left && CanChangeOrderBy)
                            {
                                // Check if the column order was already set
                                if (OrderByFieldName == column.FieldName)
                                {
                                    // Reverse ascending/descending
                                    OrderByAscending = !OrderByAscending;
                                }
                                else
                                {
                                    // Set order by field name
                                    OrderByFieldName = column.FieldName;
                                    OrderByAscending = true;
                                }

                                // Raise column click event and invalidate control
                                InvalidateGridViewCache();
                                ColumnClick(a);
                                InvalidateVisual();
                                return;
                            }
                            else if (button == MouseButtonType.Right)
                            {
                                //// Refresh column visibility in menu before opening
                                //foreach (ToolStripMenuItem menuItem in _menuColumns.Items)
                                //{
                                //    GridViewColumn menuItemColumn = Columns.FirstOrDefault(x => x.Title == menuItem.Tag.ToString());
                                //    if (menuItemColumn != null)
                                //        menuItem.Checked = menuItemColumn.Visible;
                                //}

                                DisplayContextMenu(ContextMenuType.Header, x, y);
                            }
                        }

                        offsetX += column.Width;
                    }
                }
            }

            if (controlNeedsToBeFullyInvalidated)
                InvalidateVisual();
            else if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);
        }

        public override void MouseMove(float x, float y, MouseButtonType button)
        {
            base.MouseMove(x, y, button);

            //Console.WriteLine("SongGridViewControl - MouseMove - x: {0} y: {1}", x, y);
            bool controlNeedsToBeFullyInvalidated = false;
            bool controlNeedsToBePartiallyInvalidated = false;
            var partialRect = new BasicRectangle();
            if (Columns == null || GridCache == null)
                return;

            // Calculate album cover art width
            int albumArtCoverWidth = Columns[0].Visible ? Columns[0].Width : 0;

            // Check if the user is currently resizing a column (loop through columns)
            foreach (var column in GridCache.ActiveColumns)
            {
                // Check if the user is currently resizing this column
                if (column.IsUserResizingColumn && column.Visible)
                {
                    // Calculate the new column width
                    int newWidth = DragOriginalColumnWidth - (DragStartX - (int)x);

                    // Make sure the width isn't lower than the minimum width
                    if (newWidth < MinimumColumnWidth)
                        newWidth = MinimumColumnWidth;

                    // Set column width
                    column.Width = newWidth;

                    // Refresh control (invalidate whole control)
                    controlNeedsToBeFullyInvalidated = true;
                    InvalidateGridViewCache();

                    // Auto adjust horizontal scrollbar value if it exceeds the value range (i.e. do not show empty column)
                    if (HorizontalScrollBar.Value > HorizontalScrollBar.Maximum - HorizontalScrollBar.LargeChange)
                    {
                        // Set new value
                        int tempValue = HorizontalScrollBar.Maximum - HorizontalScrollBar.LargeChange;
                        if (tempValue < 0)
                            tempValue = 0;
                        HorizontalScrollBar.Value = tempValue;
                    }
                }

                // Check if the user is moving the column
                if (column.IsMouseOverColumnHeader && column.CanBeMoved && CanMoveColumns && IsUserHoldingLeftMouseButton && !IsColumnResizing)
                {
                    // Check if the X position has changed by at least 2 pixels (i.e. dragging)
                    if (DragStartX >= x + 2 ||
                        DragStartX <= x - 2)
                    {
                        // Set resizing column flag
                        column.IsUserMovingColumn = true;
                    }
                }

                // Check if the user is currently moving this column 
                if (column.IsUserMovingColumn)
                {
                    // Loop through columns
                    int currentX = 0;
                    foreach (GridViewColumn columnOver in GridCache.ActiveColumns)
                    {
                        // Check if column is visible
                        if (columnOver.Visible)
                        {
                            // Check if the cursor is over the left part of the column
                            if (x >= currentX - HorizontalScrollBar.Value && x <= currentX + (columnOver.Width / 2) - HorizontalScrollBar.Value)
                                ColumnMoveMarkerX = (int)x;
                            // Check if the cursor is over the right part of the column
                            else if (x >= currentX + (columnOver.Width / 2) - HorizontalScrollBar.Value && x <= currentX + columnOver.Width - HorizontalScrollBar.Value)
                                ColumnMoveMarkerX = (int)x + columnOver.Width;

                            x += columnOver.Width;
                        }
                    }

                    controlNeedsToBeFullyInvalidated = true;
                }
            }

            if (!IsColumnMoving)
            {
                // Check if the cursor needs to be changed            
                int offsetX = 0;
                bool mousePointerIsOverColumnLimit = false;
                foreach (var column in GridCache.ActiveColumns)
                {
                    if (column.Visible)
                    {
                        // Increment offset by the column width
                        offsetX += column.Width;
                        if (column.CanBeResized)
                        {
                            // Check if the mouse pointer is over a column (add 1 pixel so it's easier to select)
                            if (x >= offsetX - HorizontalScrollBar.Value && x <= offsetX + 1 - HorizontalScrollBar.Value)
                            {
                                mousePointerIsOverColumnLimit = true;
                                column.IsMouseCursorOverColumnLimit = true;
                                ChangeMouseCursorType(MouseCursorType.VSplit);
                            }
                            else
                            {
                                column.IsMouseCursorOverColumnLimit = false;
                            }
                        }
                    }
                }

                // Check if the default cursor needs to be restored
                if (!mousePointerIsOverColumnLimit)
                    ChangeMouseCursorType(MouseCursorType.Default);

                int columnOffsetX2 = 0;
                for (int b = 0; b < GridCache.ActiveColumns.Count; b++)
                {
                    var column = GridCache.ActiveColumns[b];
                    if (column.Visible)
                    {
                        // Was mouse over this column header?
                        if (column.IsMouseOverColumnHeader)
                        {
                            // Invalidate region
                            column.IsMouseOverColumnHeader = false;
                            var newPartialRect = new BasicRectangle(columnOffsetX2 - HorizontalScrollBar.Value, 0, column.Width, ListCache.LineHeight);
                            partialRect.Merge(newPartialRect);
                            controlNeedsToBePartiallyInvalidated = true;
                        }

                        // Increment offset
                        columnOffsetX2 += column.Width;
                    }
                }

                // Check if the mouse pointer is over the header
                if (y >= 0 &&
                    y <= ListCache.LineHeight)
                {
                    // Check on what column the user has clicked
                    int columnOffsetX = 0;
                    for (int a = 0; a < GridCache.ActiveColumns.Count; a++)
                    {
                        var column = GridCache.ActiveColumns[a];
                        if (column.Visible)
                        {
                            // Check if the mouse pointer is over this column
                            if (x >= columnOffsetX - HorizontalScrollBar.Value && x <= columnOffsetX + column.Width - HorizontalScrollBar.Value)
                            {
                                // Invalidate region
                                column.IsMouseOverColumnHeader = true;
                                var newPartialRect = new BasicRectangle(columnOffsetX - HorizontalScrollBar.Value, 0, column.Width, ListCache.LineHeight);
                                partialRect.Merge(newPartialRect);
                                controlNeedsToBePartiallyInvalidated = true;
                                break;
                            }

                            columnOffsetX += column.Width;
                        }
                    }
                }
            }

            if (controlNeedsToBeFullyInvalidated)
                InvalidateVisual();
            else if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);
        }

    }
}
