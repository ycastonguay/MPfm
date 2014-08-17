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

namespace Sessions.GenericControls.Controls.Base
{
    /// <summary>
    /// Base class for grid view style controls; adds the concept of columns.
    /// </summary>
    public abstract class GridViewControlBase<T, U> : ListViewControlBase<T> where T : ListViewItem where U : GridViewColumn  //, IControlMouseInteraction, IControlKeyboardInteraction
    {
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
                Cache = null;
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
        protected GridViewCache Cache { get; private set; }

        public delegate void ColumnClickDelegate(int index);
        public event ColumnClickDelegate OnColumnClick;

        protected GridViewControlBase(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar) 
            : base(horizontalScrollBar, verticalScrollBar)
        {
            OrderByAscending = true;
            Cache = null;
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

        /// <summary>
        /// Creates a cache of values used for rendering the grid view.
        /// Also sets scrollbar position, height, value, maximum, etc.
        /// </summary>
        public void InvalidateCache()
        {
            // Check if columns have been created
            if (Columns == null || Columns.Count == 0 || Items == null)
                return;

            // Create cache
            Cache = new GridViewCache();

            // Get active columns and order them
            Cache.ActiveColumns = new List<GridViewColumn>(Columns.Where(x => x.Order >= 0).OrderBy(x => x.Order).ToList());

            //string allChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm!@#$%^&*()";
            //var rectText = context.MeasureText(allChars, new BasicRectangle(0, 0, 1000, 100), "Roboto", 12);
            var rectText = new BasicRectangle(0, 0, 100, 14);

            // Calculate the line height (try to measure the total possible height of characters using the custom or default font)
            Cache.LineHeight = (int)rectText.Height + Padding;
            Cache.TotalHeight = Cache.LineHeight * Items.Count;

            // Check if the total active columns width exceed the width available in the control
            Cache.TotalWidth = 0;
            for (int a = 0; a < Cache.ActiveColumns.Count; a++)
                if (Cache.ActiveColumns[a].Visible)
                    Cache.TotalWidth += Cache.ActiveColumns[a].Width;

            // Calculate the number of lines visible (count out the header, which is one line height)
            Cache.NumberOfLinesFittingInControl = (int)Math.Floor((double)(Frame.Height) / (double)(Cache.LineHeight));

            // Set vertical scrollbar dimensions
            //VerticalScrollBar.Top = _cache.LineHeight;
            //VerticalScrollBar.Left = ClientRectangle.Width - VerticalScrollBar.Width;
            VerticalScrollBar.Minimum = 0;

            // Scrollbar maximum is the number of lines fitting in the screen + the remaining line which might be cut
            // by the control height because it's not a multiple of line height (i.e. the last line is only partly visible)
            int lastLineHeight = (int)(Frame.Height - (Cache.LineHeight * Cache.NumberOfLinesFittingInControl));
            int startLineNumber = Math.Max((int)Math.Floor((double)VerticalScrollBar.Value / (double)(Cache.LineHeight)), 0);

            // Check width
            if (Cache.TotalWidth > Frame.Width - VerticalScrollBar.Width)
            {
                // Set scrollbar values
                HorizontalScrollBar.Maximum = Cache.TotalWidth;
                HorizontalScrollBar.SmallChange = 5;
                HorizontalScrollBar.LargeChange = (int)Frame.Width;
                HorizontalScrollBar.Visible = true;
            }

            // Check if the horizontal scrollbar needs to be turned off
            if (Cache.TotalWidth <= Frame.Width - VerticalScrollBar.Width && HorizontalScrollBar.Visible)
                HorizontalScrollBar.Visible = false;

            // If there are less items than items fitting on screen...            
            if (((Cache.NumberOfLinesFittingInControl - 1) * Cache.LineHeight) - HorizontalScrollBar.Height >= Cache.TotalHeight)
            {
                // Disable the scrollbar
                VerticalScrollBar.Enabled = false;
                VerticalScrollBar.Value = 0;
            }
            else
            {
                VerticalScrollBar.Enabled = true;

                // Calculate the vertical scrollbar maximum
                int vMax = Cache.LineHeight * (Items.Count - Cache.NumberOfLinesFittingInControl + 1) - lastLineHeight;

                // Add the horizontal scrollbar height if visible
                if (HorizontalScrollBar.Visible)
                    vMax += HorizontalScrollBar.Height;

                // Compensate for the header, and for the last line which might be truncated by the control height
                VerticalScrollBar.Maximum = vMax;
                VerticalScrollBar.SmallChange = Cache.LineHeight;
                VerticalScrollBar.LargeChange = 1 + Cache.LineHeight * 5;
            }

            // Calculate the scrollbar offset Y
            Cache.ScrollBarOffsetY = (startLineNumber * Cache.LineHeight) - VerticalScrollBar.Value;

            // If both scrollbars need to be visible, the width and height must be changed
            if (HorizontalScrollBar.Visible && VerticalScrollBar.Visible)
            {
                // Cut 16 pixels
                HorizontalScrollBar.Width = (int)(Frame.Width - 16);
                VerticalScrollBar.Height = Math.Max(0, (int)(Frame.Height - (Cache.LineHeight * 2) - 16));
            }
            else
            {
                VerticalScrollBar.Height = Math.Max(0, (int)(Frame.Height - (Cache.LineHeight * 2)));
            }
        }
    }
}
