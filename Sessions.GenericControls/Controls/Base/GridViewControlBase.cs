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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sessions.GenericControls.Controls.Items;
using Sessions.GenericControls.Wrappers;

namespace Sessions.GenericControls.Controls.Base
{
    /// <summary>
    /// Base class for grid view style controls; adds the concept of columns.
    /// </summary>
    public abstract class GridViewControlBase<T, U> : ListViewControlBase<T> where T : ListViewItem where U : GridViewColumn  //, IControlMouseInteraction, IControlKeyboardInteraction
    {
        protected abstract void ItemsCleared();

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
                ItemsCleared();
                InvalidateVisual();
            }
        }

        private bool _orderByAscending = true;
        /// <summary>
        /// Indicates if the order should be ascending (true) or descending (false).
        /// </summary>
        public bool OrderByAscending
        {
            get
            {
                return _orderByAscending;
            }
            set
            {
                _orderByAscending = value;
            }
        }

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

        private bool _canResizeColumns = true;
        /// <summary>
        /// Indicates if the user can resize the columns or not.
        /// </summary>
        public bool CanResizeColumns
        {
            get
            {
                return _canResizeColumns;
            }
            set
            {
                _canResizeColumns = value;
            }
        }

        private bool _canMoveColumns = true;
        /// <summary>
        /// Indicates if the user can move the columns or not.
        /// </summary>
        public bool CanMoveColumns
        {
            get
            {
                return _canMoveColumns;
            }
            set
            {
                _canMoveColumns = value;
            }
        }

        private bool _canChangeOrderBy = true;
        /// <summary>
        /// Indicates if the user can change the order by or not.
        /// </summary>
        public bool CanChangeOrderBy
        {
            get
            {
                return _canChangeOrderBy;
            }
            set
            {
                _canChangeOrderBy = value;
            }
        }

        public delegate void ColumnClickDelegate(int index);
        public event ColumnClickDelegate OnColumnClick;

        protected GridViewControlBase(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar) 
            : base(horizontalScrollBar, verticalScrollBar)
        {
            _columns = new List<U>();
        }

        protected void ColumnClick(int index)
        {
            if (OnColumnClick != null)
                OnColumnClick(index);
        }
    }
}
