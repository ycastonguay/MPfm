//
// ReorderListView.cs: This list view control is based on the System.Windows.Forms.ListView control.
//                     It adds custom flickerless redrawing and reordering.
//
// Copyright � 2011 Yanick Castonguay
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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This list view control is based on the System.Windows.Forms.ListView control.
    /// It adds custom flickerless redrawing and reordering.
    /// 
    /// This code has been taken from http://www.codeproject.com/KB/list/ReorderListView.aspx.        
    /// </summary>
    public partial class ReorderListView : System.Windows.Forms.ListView
    {
        private const string REORDER = "Reorder";

        public delegate void ItemsReorderedHandler(EventArgs args);
        public event ItemsReorderedHandler ItemsReordered;

        public void OnItemsReordered(EventArgs args)
        {
            if (ItemsReordered != null)
            {
                ItemsReordered(args);
            }
        }

        private bool allowRowReorder = true;
        public bool AllowRowReorder
        {
            get
            {
                return this.allowRowReorder;
            }
            set
            {
                this.allowRowReorder = value;
                base.AllowDrop = value;
            }
        }

        public new SortOrder Sorting
        {
            get
            {
                return SortOrder.None;
            }
            set
            {
                base.Sorting = SortOrder.None;
            }
        }

        /// <summary>
        /// ListView based on System.Forms.ListView. Added flickerless
        /// code.
        /// </summary>
        public ReorderListView()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                        /*ControlStyles.UserPaint |*/
                        ControlStyles.AllPaintingInWmPaint, true);

            this.AllowRowReorder = true;
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            bool itemsMoved = false;

            base.OnDragDrop(e);
            if (!this.AllowRowReorder)
            {
                return;
            }
            if (base.SelectedItems.Count == 0)
            {
                return;
            }
            Point cp = base.PointToClient(new Point(e.X, e.Y));
            ListViewItem dragToItem = base.GetItemAt(cp.X, cp.Y);
            if (dragToItem == null)
            {
                return;
            }
            int dropIndex = dragToItem.Index;
            if (dropIndex > base.SelectedItems[0].Index)
            {
                dropIndex++;
            }
            ArrayList insertItems =
                new ArrayList(base.SelectedItems.Count);
            foreach (ListViewItem item in base.SelectedItems)
            {
                itemsMoved = true;
                insertItems.Add(item.Clone());
            }
            for (int i = insertItems.Count - 1; i >= 0; i--)
            {
                ListViewItem insertItem =
                    (ListViewItem)insertItems[i];
                base.Items.Insert(dropIndex, insertItem);
            }
            foreach (ListViewItem removeItem in base.SelectedItems)
            {
                base.Items.Remove(removeItem);
            }

            if (itemsMoved)
            {
                EventArgs args = new EventArgs();
                OnItemsReordered(args);
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (!this.AllowRowReorder)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            if (!e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            Point cp = base.PointToClient(new Point(e.X, e.Y));
            ListViewItem hoverItem = base.GetItemAt(cp.X, cp.Y);
            if (hoverItem == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            foreach (ListViewItem moveItem in base.SelectedItems)
            {
                if (moveItem.Index == hoverItem.Index)
                {
                    e.Effect = DragDropEffects.None;
                    hoverItem.EnsureVisible();
                    return;
                }
            }
            base.OnDragOver(e);
            String text = (String)e.Data.GetData(REORDER.GetType());
            if (text.CompareTo(REORDER) == 0)
            {
                e.Effect = DragDropEffects.Move;
                hoverItem.EnsureVisible();
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            if (!this.AllowRowReorder)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            if (!e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            base.OnDragEnter(e);
            String text = (String)e.Data.GetData(REORDER.GetType());
            if (text.CompareTo(REORDER) == 0)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);
            if (!this.AllowRowReorder)
            {
                return;
            }
            base.DoDragDrop(REORDER, DragDropEffects.Move);
        }
    }
}