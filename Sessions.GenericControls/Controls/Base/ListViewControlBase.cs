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
using Sessions.GenericControls.Controls.Items;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Interaction;
using Sessions.GenericControls.Wrappers;

namespace Sessions.GenericControls.Controls.Base
{
    /// <summary>
    /// Base class for list view style controls. Does not have the concept of columns.
    /// </summary>
    public abstract class ListViewControlBase<T> : ControlBase where T : ListViewItem  //, IControlMouseInteraction, IControlKeyboardInteraction
    {
        // Control wrappers
        public IHorizontalScrollBarWrapper HorizontalScrollBar { get; private set; }
        public IVerticalScrollBarWrapper VerticalScrollBar { get; private set; }

        private List<T> _items;
        /// <summary>
        /// List of grid view items (representing songs).
        /// </summary>
        [Browsable(false)]
        public List<T> Items
        {
            get
            {
                return _items;
            }
        }

        /// <summary>
        /// Returns the list of selected items.
        /// </summary>
        [Browsable(false)]
        public List<T> SelectedItems
        {
            get
            {
                if (_items != null)
                    return _items.Where(x => x.IsSelected).ToList();

                return null;
            }
        }

        /// <summary>
        /// Indicates if the user can reorder the items or not.
        /// </summary>
        public bool CanReorderItems { get; set; }

        public int Padding { get; set; }

        public delegate void SelectedIndexChangedDelegate();
        public delegate void ItemDoubleClickDelegate(int index);
        public delegate void ChangeMouseCursorTypeDelegate(MouseCursorType mouseCursorType);
        public delegate void DisplayContextMenuDelegate(ContextMenuType contextMenuType, float x, float y);

        public event SelectedIndexChangedDelegate OnSelectedIndexChanged;
        public event ItemDoubleClickDelegate OnItemDoubleClick;
        public event ChangeMouseCursorTypeDelegate OnChangeMouseCursorType;
        public event DisplayContextMenuDelegate OnDisplayContextMenu;

        protected ListViewControlBase(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar)
        {
            Padding = 6;
            CanReorderItems = true;
            VerticalScrollBar = verticalScrollBar;
            VerticalScrollBar.OnScrollValueChanged += (sender, args) => InvalidateVisual();

            HorizontalScrollBar = horizontalScrollBar;
            HorizontalScrollBar.OnScrollValueChanged += (sender, args) => InvalidateVisual();

            _items = new List<T>();
        }

        protected void SelectedIndexChanged()
        {
            if (OnSelectedIndexChanged != null)
                OnSelectedIndexChanged();
        }

        protected void ItemDoubleClick(int index)
        {
            if (OnItemDoubleClick != null)
                OnItemDoubleClick(index);
        }

        protected void ChangeMouseCursorType(MouseCursorType cursorType)
        {
            if (OnChangeMouseCursorType != null)
                OnChangeMouseCursorType(cursorType);
        }

        protected void DisplayContextMenu(ContextMenuType contextMenuType, float x, float y)
        {
            if (OnDisplayContextMenu != null)
                OnDisplayContextMenu(contextMenuType, x, y);
        }

        public override void Render(IGraphicsContext context)
        {
        }

        /// <summary>
        /// Clears the currently selected items.
        /// </summary>
        public void ClearSelectedItems()
        {
            foreach (var item in Items)
            {
                if (item.IsSelected)
                    item.IsSelected = false;
            }

            InvalidateVisual();
        }

        public void ResetSelection()
        {
            // Reset selection, unless the CTRL key is held (TODO)
            var selectedItems = Items.Where(item => item.IsSelected == true).ToList();
            foreach (var item in selectedItems)
                item.IsSelected = false;
        }

        public Tuple<int, int> GetStartIndexAndEndIndexOfSelectedRows()
        {
            if (Items == null)
                return new Tuple<int, int>(-1, -1);

            // Loop through visible lines to find the original selected items
            int startIndex = -1;
            int endIndex = -1;
            //for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
            for (int a = 0; a < _items.Count; a++)
            {
                // Check if the item is selected
                if (Items[a].IsSelected)
                {
                    // Check if the start index was set
                    if (startIndex == -1)
                        startIndex = a;

                    // Check if the end index is set or if it needs to be updated
                    if (endIndex == -1 || endIndex < a)
                        // Set end index
                        endIndex = a;
                }
            }

            return new Tuple<int, int>(startIndex, endIndex);
        }

        public enum ContextMenuType
        {
            Item = 0, Header = 1
        }
    }
}
