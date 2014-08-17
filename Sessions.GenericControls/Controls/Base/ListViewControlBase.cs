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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Items;
using Sessions.GenericControls.Controls.Songs;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Interaction;
using Sessions.GenericControls.Wrappers;
using Sessions.WindowsControls;
using Sessions.Core;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;

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

        public enum ContextMenuType
        {
            Item = 0, Header = 1
        }
    }
}
