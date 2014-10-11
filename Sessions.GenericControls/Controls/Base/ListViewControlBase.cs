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
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Themes;
using System.Text;
using System.Collections.ObjectModel;

namespace Sessions.GenericControls.Controls.Base
{
    /// <summary>
    /// Base class for list view style controls. Does not have the concept of columns.
    /// </summary>
    public abstract class ListViewControlBase<T> : ControlBase, IControlMouseInteraction, IControlKeyboardInteraction where T : ListViewItem 
    {
        // Control wrappers
        public IHorizontalScrollBarWrapper HorizontalScrollBar { get; private set; }    
        public IVerticalScrollBarWrapper VerticalScrollBar { get; private set; }

        /// <summary>
        /// Indicates if the user can reorder the items or not.
        /// </summary>
        public bool CanReorderItems { get; set; }

        /// <summary>
        /// Cache for GridView.
        /// </summary>
        protected ListViewCache ListCache { get; set; }

        public ListViewTheme Theme { get; set; }
        public int Padding { get; set; }
        public bool DisplayDebugInformation { get; set; }

//        public int SelectedIndex
//        {
//            get
//            {
//                return SelectedIndexes.Count > 0 ? SelectedIndexes[0] : -1;
//            }
//            set
//            {
//                SelectedIndexes.Clear();
//                SelectedIndexes.Add(value);
//            }
//        }

        public ObservableCollection<int> SelectedIndexes { get; protected set; }

        protected int MouseOverRowIndex { get; set; }
        protected int StartLineNumber { get; set; }
        protected int NumberOfLinesToDraw { get; set; }
        protected bool IsMouseOverControl { get; set; }
        protected bool IsUserHoldingLeftMouseButton { get; set; }
        protected int DragStartX { get; set; }
        protected int LastItemIndexClicked { get; set; }

        public delegate void SelectedIndexChangedDelegate();
        public delegate void ItemDoubleClickDelegate(int index);
        public delegate void ChangeMouseCursorTypeDelegate(MouseCursorType mouseCursorType);
        public delegate void DisplayContextMenuDelegate(ContextMenuType contextMenuType, float x, float y);

        public event SelectedIndexChangedDelegate OnSelectedIndexChanged;
        public event ItemDoubleClickDelegate OnItemDoubleClick;
        public event ChangeMouseCursorTypeDelegate OnChangeMouseCursorType;
        public event DisplayContextMenuDelegate OnDisplayContextMenu;

        protected abstract string GetCellContent(int row, int col, string fieldName);
        protected abstract int GetRowCount();
        protected abstract int GetRowHeight();
        protected abstract bool ShouldDrawHeader();
        protected abstract bool IsRowSelectable(int row);
        protected abstract bool IsRowEmpty(int row);

        protected ListViewControlBase(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar)
        {
            Padding = 6;
            CanReorderItems = true;
            MouseOverRowIndex = -1;
            Theme = new ListViewTheme();
            SelectedIndexes = new ObservableCollection<int>();
            SelectedIndexes.CollectionChanged += (sender, e) => {
                InvalidateVisual();
                SelectedIndexChanged();
            };

            HorizontalScrollBar = horizontalScrollBar;
            HorizontalScrollBar.OnScrollValueChanged += (sender, args) => InvalidateVisual();
            VerticalScrollBar = verticalScrollBar;
            VerticalScrollBar.OnScrollValueChanged += (sender, args) => InvalidateVisual();
        }

        public void ScrollToRow(int row)
        {
            float offsetY = (row * ListCache.LineHeight);
            Console.WriteLine("ListViewControlBase - ScrollToRow - row: {0} - offsetY: {1}", row, offsetY);
            VerticalScrollBar.Value = (int)offsetY;
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

        protected virtual void DetermineVisibleLineIndexes()
        {
            // Calculate how many lines must be skipped because of the scrollbar position
            StartLineNumber = Math.Max((int) Math.Floor((double) VerticalScrollBar.Value/(double) (ListCache.LineHeight)), 0);

            // Check if the total number of lines exceeds the number of icons fitting in height
            NumberOfLinesToDraw = 0;
            if (StartLineNumber + ListCache.NumberOfLinesFittingInControl > GetRowCount())
            {
                // There aren't enough lines to fill the screen
                NumberOfLinesToDraw = GetRowCount() - StartLineNumber;
            }
            else
            {
                // Fill up screen 
                NumberOfLinesToDraw = ListCache.NumberOfLinesFittingInControl;
            }

            // Add one line for overflow; however, make sure we aren't adding a line without content 
            if (StartLineNumber + NumberOfLinesToDraw + 1 <= GetRowCount())
                NumberOfLinesToDraw++;
        }

        public override void Render(IGraphicsContext context)
        {
//            //var stopwatch = new Stopwatch();
//            //stopwatch.Start();

            // If frame doesn't match, refresh frame and song cache
            if (Frame.Width != context.BoundsWidth || Frame.Height != context.BoundsHeight || ListCache == null)
            {
                Frame = new BasicRectangle(context.BoundsWidth, context.BoundsHeight);
                InvalidateCache();
            }            

            // Draw background
            context.DrawRectangle(Frame, new BasicBrush(Theme.BackgroundColor), new BasicPen());

            DrawRows(context);
            DrawHeader(context);
            DrawDebugInformation(context);

            if (HorizontalScrollBar.Visible && VerticalScrollBar.Visible)
            {
                // Draw a bit of control color over the 16x16 area in the lower right corner
                var brush = new BasicBrush(new BasicColor(200, 200, 200));
                context.DrawRectangle(new BasicRectangle(Frame.Width - 16, Frame.Height - 16, 16, 16), brush, new BasicPen());
            }

//            //stopwatch.Stop();
//            //Console.WriteLine("SongGridViewControl - Render - Completed in {0} - frame: {1} numberOfLinesToDraw: {2}", stopwatch.Elapsed, Frame, _numberOfLinesToDraw);
        }     

        protected virtual BasicColor GetRowBackgroundColor(int row)
        {   
            var color = Theme.CellBackgroundColor;
            if(SelectedIndexes.Contains(row))
            {
                color = Theme.SelectedBackgroundColor;

//                    // Use darker color
//                    byte diff = 4;
//                    colorBackground1 = new BasicColor(255,
//                        (byte)((colorBackground1.R - diff < 0) ? 0 : colorBackground1.R - diff),
//                        (byte)((colorBackground1.G - diff < 0) ? 0 : colorBackground1.G - diff),
//                        (byte)((colorBackground1.B - diff < 0) ? 0 : colorBackground1.B - diff));
//                    colorBackground2 = new BasicColor(255,
//                        (byte)((colorBackground2.R - diff < 0) ? 0 : colorBackground2.R - diff),
//                        (byte)((colorBackground2.G - diff < 0) ? 0 : colorBackground2.G - diff),
//                        (byte)((colorBackground2.B - diff < 0) ? 0 : colorBackground2.B - diff));
            }

            //// Check if mouse is over item
            //if (items[a].IsMouseOverItem)
            //{
            //    // Use lighter color
            //    int diff = 20;
            //    colorBackground1 = Color.FromArgb(255,
            //        (colorBackground1.R + diff > 255) ? 255 : colorBackground1.R + diff,
            //        (colorBackground1.G + diff > 255) ? 255 : colorBackground1.G + diff,
            //        (colorBackground1.B + diff > 255) ? 255 : colorBackground1.B + diff);
            //    colorBackground2 = Color.FromArgb(255,
            //        (colorBackground2.R + diff > 255) ? 255 : colorBackground2.R + diff,
            //        (colorBackground2.G + diff > 255) ? 255 : colorBackground2.G + diff,
            //        (colorBackground2.B + diff > 255) ? 255 : colorBackground2.B + diff);
            //}

            return color;
        }

        protected virtual void DrawRowBackground(IGraphicsContext context, int row, float offsetY)
        {
            var penTransparent = new BasicPen();

            //int albumArtColumnWidth = Columns[0].Visible ? Columns[0].Width : 0;
            //int lineBackgroundWidth = (int)(Frame.Width + HorizontalScrollBar.Value - albumArtColumnWidth);
            int lineBackgroundWidth = (int)(Frame.Width + HorizontalScrollBar.Value);
            if (VerticalScrollBar.Visible)
                lineBackgroundWidth -= VerticalScrollBar.Width;

            // Draw row background
            //var rectBackground = new BasicRectangle(albumArtColumnWidth - HorizontalScrollBar.Value, offsetY, lineBackgroundWidth, ListCache.LineHeight + 1);
            var color = GetRowBackgroundColor(row);
            var rectBackground = new BasicRectangle(HorizontalScrollBar.Value, offsetY, lineBackgroundWidth, ListCache.LineHeight + 1);
            //var brushGradient = new BasicGradientBrush(color, color, 90);
            var brush = new BasicBrush(color);
            context.DrawRectangle(rectBackground, brush, penTransparent);
        }

        protected virtual void DrawRows(IGraphicsContext context)
        {
            DetermineVisibleLineIndexes();

            // Loop through lines
            for (int a = StartLineNumber; a < StartLineNumber + NumberOfLinesToDraw; a++)
            {                
                const float offsetX = 0;
                float offsetY = (a * ListCache.LineHeight) - VerticalScrollBar.Value;
                if(ShouldDrawHeader())
                    offsetY += ListCache.LineHeight;

                DrawRowBackground(context, a, offsetY);
                DrawCells(context, a, offsetX, offsetY);
            }
        }

        protected virtual void DrawCells(IGraphicsContext context, int row, float offsetX, float offsetY)
        {
            // Only one cell in a list view
            DrawCell(context, row, 0, offsetX, offsetY);
        }

        protected virtual void DrawCell(IGraphicsContext context, int row, int col, float offsetX, float offsetY)
        {
            //var rect = new BasicRectangle(offsetX - HorizontalScrollBar.Value + 2, offsetY + (Theme.Padding / 2), GridCache.ActiveColumns[col].Width, ListCache.LineHeight - Theme.Padding + 2);
            var rect = new BasicRectangle(offsetX - HorizontalScrollBar.Value + 2, offsetY + (Theme.Padding / 2), Frame.Width, ListCache.LineHeight - Theme.Padding + 2);
            var value = GetCellContent(row, col, string.Empty);
            context.DrawText(value, rect, Theme.TextColor, Theme.FontName, Theme.FontSize);
        }
            
        protected virtual void DrawHeader(IGraphicsContext context)
        {
            if(!ShouldDrawHeader())
                return;

            var penTransparent = new BasicPen();
            var brushGradient = new BasicGradientBrush(Theme.HeaderBackgroundColor, Theme.HeaderBackgroundColor, 90);

            // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)
            var rectBackgroundHeader = new BasicRectangle(0, -1, Frame.Width, ListCache.LineHeight + 1);
            context.DrawRectangle(rectBackgroundHeader, brushGradient, penTransparent);
        }

        protected virtual void DrawDebugInformation(IGraphicsContext context)
        {
            if (DisplayDebugInformation)
            {
                // Build debug string
                var sbDebug = new StringBuilder();
                sbDebug.AppendLine("Line Count: " + GetRowCount().ToString());
                sbDebug.AppendLine("Line Height: " + ListCache.LineHeight.ToString());
                sbDebug.AppendLine("Lines Fit In Height: " + ListCache.NumberOfLinesFittingInControl.ToString());
                //sbDebug.AppendLine("Total Width: " + GridCache.TotalWidth);
                sbDebug.AppendLine("Total Height: " + ListCache.TotalHeight);
                sbDebug.AppendLine("Scrollbar Offset Y: " + ListCache.ScrollBarOffsetY);
                sbDebug.AppendLine("HScrollbar Maximum: " + HorizontalScrollBar.Maximum.ToString());
                sbDebug.AppendLine("HScrollbar LargeChange: " + HorizontalScrollBar.LargeChange.ToString());
                sbDebug.AppendLine("HScrollbar Value: " + HorizontalScrollBar.Value.ToString());
                sbDebug.AppendLine("VScrollbar Maximum: " + VerticalScrollBar.Maximum.ToString());
                sbDebug.AppendLine("VScrollbar LargeChange: " + VerticalScrollBar.LargeChange.ToString());
                sbDebug.AppendLine("VScrollbar Value: " + VerticalScrollBar.Value.ToString());

                // Measure string
                var sizeDebugText = context.MeasureText(sbDebug.ToString(), new BasicRectangle(0, 0, 200, 40), Theme.FontName, Theme.FontSize);
                var rect = new BasicRectangle(0, 0, 200, sizeDebugText.Height);

                // Draw background
                var brush = new BasicBrush(new BasicColor(200, 0, 0));
                var penTransparent = new BasicPen();
                context.DrawRectangle(rect, brush, penTransparent);

                // Draw string
                context.DrawText(sbDebug.ToString(), rect, new BasicColor(255, 255, 255), Theme.FontName, Theme.FontSize);
            }
        }

        public void ResetSelection()
        {
//            // Reset selection, unless the CTRL key is held (TODO)
//            var selectedItems = Items.Where(item => item.IsSelected == true).ToList();
//            foreach (var item in selectedItems)
//                item.IsSelected = false;
            SelectedIndexes.Clear();
        }

        public Tuple<int, int> GetStartIndexAndEndIndexOfSelectedRows()
        {
            // Loop through visible lines to find the original selected items
            int startIndex = -1;
            int endIndex = -1;
            //for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
            for (int a = 0; a < GetRowCount(); a++)
            {
                // Check if the item is selected
                if (SelectedIndexes.Contains(a))
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

        protected bool IsLineVisible(int lineIndex)
        {
            return lineIndex < StartLineNumber || lineIndex > StartLineNumber + NumberOfLinesToDraw;// + PreloadLinesAlbumCover;
        }

        public virtual void ReloadData()
        {
            InvalidateCache();
            VerticalScrollBar.Value = 0;
            InvalidateVisual();
        }

        public virtual void ReloadRow(int row)
        {
            int offsetY = (row * ListCache.LineHeight) - VerticalScrollBar.Value + ListCache.LineHeight;
            InvalidateVisualInRect(new BasicRectangle(HorizontalScrollBar.Value, offsetY, Frame.Width - HorizontalScrollBar.Value, ListCache.LineHeight));
        }

        public virtual void InvalidateCache()
        {
            InvalidateListViewCache();
        }

        private void InvalidateListViewCache()
        {
            ListCache = new ListViewCache();

            // Calculate the line height (try to measure the total possible height of characters using the custom or default font)
            ListCache.LineHeight = GetRowHeight() + Padding;
            ListCache.TotalHeight = ListCache.LineHeight * GetRowCount();

            // Calculate the number of lines visible (count out the header, which is one line height)
            ListCache.NumberOfLinesFittingInControl = (int)Math.Floor((double)(Frame.Height) / (double)(ListCache.LineHeight));

            // Make sure the horizontal scroll bar isn't visible on a list view
            HorizontalScrollBar.Visible = false;

            // Set vertical scrollbar dimensions
            //VerticalScrollBar.Top = _cache.LineHeight;
            //VerticalScrollBar.Left = ClientRectangle.Width - VerticalScrollBar.Width;
            VerticalScrollBar.Minimum = 0;

            // Scrollbar maximum is the number of lines fitting in the screen + the remaining line which might be cut
            // by the control height because it's not a multiple of line height (i.e. the last line is only partly visible)
            int lastLineHeight = (int)(Frame.Height - (ListCache.LineHeight * ListCache.NumberOfLinesFittingInControl));
            int startLineNumber = Math.Max((int)Math.Floor((double)VerticalScrollBar.Value / (double)(ListCache.LineHeight)), 0);

            // If there are less items than items fitting on screen...            
            if (((ListCache.NumberOfLinesFittingInControl - 1) * ListCache.LineHeight) - HorizontalScrollBar.Height >= ListCache.TotalHeight)
            {
                // Disable the scrollbar
                VerticalScrollBar.Enabled = false;
                VerticalScrollBar.Value = 0;
            }
            else
            {
                VerticalScrollBar.Enabled = true;

                // Calculate the vertical scrollbar maximum
                int vMax = ListCache.LineHeight * (GetRowCount() - ListCache.NumberOfLinesFittingInControl + 1) - lastLineHeight;

                // Add the horizontal scrollbar height if visible
                if (HorizontalScrollBar.Visible)
                    vMax += HorizontalScrollBar.Height;

                // Compensate for the header, and for the last line which might be truncated by the control height
                VerticalScrollBar.Maximum = vMax;
                VerticalScrollBar.SmallChange = ListCache.LineHeight;
                VerticalScrollBar.LargeChange = 1 + ListCache.LineHeight * 5;
            }

            // Calculate the scrollbar offset Y
            ListCache.ScrollBarOffsetY = (startLineNumber * ListCache.LineHeight) - VerticalScrollBar.Value;

            // If both scrollbars need to be visible, the width and height must be changed
            int verticalScrollBarCompensation = ListCache.LineHeight;
            if(ShouldDrawHeader())
                verticalScrollBarCompensation -= ListCache.LineHeight;
                    
            if (HorizontalScrollBar.Visible && VerticalScrollBar.Visible)
            {
                // Cut 16 pixels (size of scrollbar)
                const int scrollBarSize = 16;
                HorizontalScrollBar.Width = (int)(Frame.Width - scrollBarSize);
                VerticalScrollBar.Height = Math.Max(0, (int)(Frame.Height - verticalScrollBarCompensation - scrollBarSize));
            }
            else
            {
                VerticalScrollBar.Height = Math.Max(0, (int)(Frame.Height - verticalScrollBarCompensation));
            }
        }

        #region IControlKeyboardInteraction implementation

        public virtual void KeyDown(char key, SpecialKeys specialKeys, ModifierKeys modifierKeys, bool isRepeat)
        {           
            if (ListCache == null)
                return;

            int selectedIndex = -1;
            int scrollbarOffsetY = (StartLineNumber * ListCache.LineHeight) - VerticalScrollBar.Value;
            var startEndIndexes = GetStartIndexAndEndIndexOfSelectedRows();

            if (specialKeys == SpecialKeys.Enter)
            {
                ItemDoubleClick(startEndIndexes.Item1);
                return;
            }

            switch (specialKeys)
            {
                case SpecialKeys.Down:
                    if (startEndIndexes.Item1 < GetRowCount() - 1)
                    {
                        selectedIndex = startEndIndexes.Item1;
                        while (selectedIndex >= 0 && selectedIndex <= GetRowCount() - 1)
                        {
                            selectedIndex++;
                            if (!IsRowEmpty(selectedIndex))
                                break;
                        }
                    }
                    break;
                case SpecialKeys.Up:
                    if (startEndIndexes.Item1 > 0)
                    {
                        selectedIndex = startEndIndexes.Item1;
                        while (selectedIndex >= 0 && selectedIndex <= GetRowCount() - 1)
                        {
                            selectedIndex--;
                            if (!IsRowEmpty(selectedIndex))
                                break;
                        }
                    }
                    break;
                case SpecialKeys.PageDown:
                    selectedIndex = startEndIndexes.Item1 + ListCache.NumberOfLinesFittingInControl - 2; // 2 is header + scrollbar height
                    selectedIndex = Math.Min(selectedIndex, GetRowCount() - 1);

                    if (selectedIndex == GetRowCount() - 1)
                    {
                        // If we are to select the last item, make sure the item we're selecting is NOT an empty row
                        for (int a = selectedIndex; a >= 0; a--)
                        {
                            if (!IsRowEmpty(a))
                            {
                                selectedIndex = a;
                                break;
                            }
                        }
                    } 
                    else
                    {
                        // Continue to interate until we find a selectable row
                        while (selectedIndex >= 0 && selectedIndex <= GetRowCount() - 1)
                        {
                            selectedIndex++;
                            if (!IsRowEmpty(selectedIndex))
                                break;
                        }
                    }
                    break;
                case SpecialKeys.PageUp:
                    selectedIndex = startEndIndexes.Item1 - ListCache.NumberOfLinesFittingInControl + 2; 
                    selectedIndex = Math.Max(selectedIndex, 0);

                    if (selectedIndex > 0)
                    {
                        while (selectedIndex >= 0 && selectedIndex <= GetRowCount() - 1)
                        {
                            selectedIndex--;
                            if (!IsRowEmpty(selectedIndex))
                                break;
                        }
                    }
                    break;
                case SpecialKeys.Home:
                    selectedIndex = 0; // First item cannot be empty
                    break;
                case SpecialKeys.End:
                    for (int a = GetRowCount() - 1; a >= 0; a--)
                    {
                        if (!IsRowEmpty(a))
                        {
                            selectedIndex = a;
                            break;
                        }
                    }
                    break;
            }

            if (selectedIndex == -1)
                return;

            SelectedIndexes.Clear();
            SelectedIndexes.Add(selectedIndex);

            // Check if new selection is out of bounds of visible area
            float y = ((selectedIndex - StartLineNumber + 1)*ListCache.LineHeight) + scrollbarOffsetY;
            //Console.WriteLine("SongGridViewControl - KeyDown - y: {0} scrollbarOffsetY: {1} VerticalScrollBar.Value: {2}", y, scrollbarOffsetY, VerticalScrollBar.Value);

            int newVerticalScrollBarValue = VerticalScrollBar.Value;
            switch (specialKeys)
            {
                case SpecialKeys.Down:
                    // Check for out of bounds
                    if (y > Frame.Height - HorizontalScrollBar.Height - ListCache.LineHeight)
                        newVerticalScrollBarValue = VerticalScrollBar.Value + ListCache.LineHeight;
                    break;
                case SpecialKeys.Up:
                    // Check for out of bounds
                    if (y < ListCache.LineHeight)
                        newVerticalScrollBarValue = VerticalScrollBar.Value - ListCache.LineHeight;
                    break;
                case SpecialKeys.PageDown:
                    int heightToScrollDown = ((startEndIndexes.Item1 - StartLineNumber) * ListCache.LineHeight) + scrollbarOffsetY;
                    newVerticalScrollBarValue = VerticalScrollBar.Value + heightToScrollDown;
                    break;
                case SpecialKeys.PageUp:
                    int heightToScrollUp = ((StartLineNumber + ListCache.NumberOfLinesFittingInControl - startEndIndexes.Item1 - 2) * ListCache.LineHeight) - scrollbarOffsetY;
                    newVerticalScrollBarValue = VerticalScrollBar.Value - heightToScrollUp;
                    break;
                case SpecialKeys.Home:
                    newVerticalScrollBarValue = 0;
                    break;
                case SpecialKeys.End:
                    newVerticalScrollBarValue = VerticalScrollBar.Maximum;
                    break;
            }

            // Make sure we don't scroll out of bounds
            if (newVerticalScrollBarValue > VerticalScrollBar.Maximum)
                newVerticalScrollBarValue = VerticalScrollBar.Maximum;
            if (newVerticalScrollBarValue < 0)
                newVerticalScrollBarValue = 0;
            VerticalScrollBar.Value = newVerticalScrollBarValue;

            // Is this necessary when scrolling the whole area? it will refresh all anyway
            //OnInvalidateVisualInRect(new BasicRectangle(albumArtCoverWidth - HorizontalScrollBar.Value, y, Frame.Width - albumArtCoverWidth + HorizontalScrollBar.Value, Cache.LineHeight));
            InvalidateVisual();
        }

        public virtual void KeyUp(char key, SpecialKeys specialKeys, ModifierKeys modifierKeys, bool isRepeat)
        {
        }

        #endregion

        #region IControlMouseInteraction implementation

        public virtual void MouseDown(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            if(button == MouseButtonType.Left)
                IsUserHoldingLeftMouseButton = true;

            DragStartX = (int)x;                
        }

        public virtual void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            IsUserHoldingLeftMouseButton = false;       
            DragStartX = -1;
        }

        public virtual void MouseClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {            
            bool controlNeedsToBeFullyInvalidated = false;
            bool controlNeedsToBePartiallyInvalidated = false;
            var partialRect = new BasicRectangle();

            if (ListCache == null)
                return;

            // Loop through visible lines to find the original selected items
            var tuple = GetStartIndexAndEndIndexOfSelectedRows();
            int startIndex = tuple.Item1;
            int endIndex = tuple.Item2;
            int scrollbarOffsetY = (StartLineNumber * ListCache.LineHeight) - VerticalScrollBar.Value;

            // Make sure the indexes are set
            if (startIndex > -1 && endIndex > -1)
            {
                // Invalidate the original selected lines
                int startY = ((startIndex - StartLineNumber + 1) * ListCache.LineHeight) + scrollbarOffsetY;
                int endY = ((endIndex - StartLineNumber + 2) * ListCache.LineHeight) + scrollbarOffsetY;
                var newPartialRect = new BasicRectangle(HorizontalScrollBar.Value, startY, Frame.Width + HorizontalScrollBar.Value, endY - startY);
                partialRect.Merge(newPartialRect);
                controlNeedsToBePartiallyInvalidated = true;
            }

            // Reset selection (make sure SHIFT or CTRL isn't held down)
            if (!keysHeld.IsShiftKeyHeld && !keysHeld.IsCtrlKeyHeld)
            {
                // Make sure the mouse is over at least one item
                if(MouseOverRowIndex >= 0)
                    ResetSelection();
            }

            // Loop through visible lines to update the new selected items
            bool invalidatedNewSelection = false;
            for (int a = StartLineNumber; a < StartLineNumber + NumberOfLinesToDraw; a++)
            {
                // Check if mouse is over this item
                if(MouseOverRowIndex == a)
                {
                    invalidatedNewSelection = true;

                    // Check if SHIFT is held
                    if (keysHeld.IsShiftKeyHeld)
                    {
                        // Find the start index of the selection
                        int startIndexSelection = LastItemIndexClicked;
                        if (a < startIndexSelection)
                            startIndexSelection = a;
                        if (startIndexSelection < 0)
                            startIndexSelection = 0;

                        // Find the end index of the selection
                        int endIndexSelection = LastItemIndexClicked;
                        if (a > endIndexSelection)
                            endIndexSelection = a + 1;

                        // Loop through items to selected
                        for (int b = startIndexSelection; b < endIndexSelection; b++)
                            SelectedIndexes.Add(b);

                        controlNeedsToBeFullyInvalidated = true;
                    }                
                    // Check if CTRL is held
                    else if(keysHeld.IsCtrlKeyHeld)
                    {
                        // Invert selection
                        SelectedIndexes.Remove(a);
                        var newPartialRect = new BasicRectangle(HorizontalScrollBar.Value, ((a - StartLineNumber + 1) * ListCache.LineHeight) + scrollbarOffsetY, Frame.Width + HorizontalScrollBar.Value, ListCache.LineHeight);
                        partialRect.Merge(newPartialRect);
                        controlNeedsToBePartiallyInvalidated = true;
                    }
                    else
                    {
                        // Set this item as the new selected item
                        SelectedIndexes.Add(a);
                        var newPartialRect = new BasicRectangle(HorizontalScrollBar.Value, ((a - StartLineNumber + 1) * ListCache.LineHeight) + scrollbarOffsetY, Frame.Width + HorizontalScrollBar.Value, ListCache.LineHeight);
                        partialRect.Merge(newPartialRect);
                        controlNeedsToBePartiallyInvalidated = true;
                    }

                    // Set the last item clicked index
                    LastItemIndexClicked = a;
                    break;
                }
            }

            // Raise selected item changed event (if an event is subscribed)
            if (invalidatedNewSelection)
            {
                if(SelectedIndexes.Count > 0)
                    SelectedIndexChanged();
                else
                    SelectedIndexChanged();
            }

            if (controlNeedsToBeFullyInvalidated)
                InvalidateVisual();
            else if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);
        }

        public virtual void MouseDoubleClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
        }

        public virtual void MouseMove(float x, float y, MouseButtonType button)
        {
            //Console.WriteLine("SongGridViewControl - MouseMove - x: {0} y: {1}", x, y);
            if (ListCache == null)
                return;

            // Check if the mouse cursor is over a line (loop through lines)                        
            int offsetY = 0;
            //int scrollbarOffsetY = (_startLineNumber * Cache.LineHeight) - VerticalScrollBar.Value;

            // Check if there's at least one item
            if (GetRowCount() > 0)
            {
                // Reset mouse over item flags
                for (int b = StartLineNumber; b < StartLineNumber + NumberOfLinesToDraw; b++)
                {
                    //Console.WriteLine("SongGridViewControl - MouseMove - Checking for resetting mouse over flag for line {0}", b);
                    // Check if the mouse was over this item
                    if(MouseOverRowIndex == b)
                    {
                        // Reset flag and invalidate region
                        //Console.WriteLine("SongGridViewControl - MouseMove - Resetting mouse over flag for line {0}", b);
                        MouseOverRowIndex = -1;
                        break;
                    }
                }

                // Put new mouse over flag
                for (int a = StartLineNumber; a < StartLineNumber + NumberOfLinesToDraw; a++)
                {
                    // Calculate offset
                    int headerHeight = ShouldDrawHeader() ? ListCache.LineHeight : 0;
                    offsetY = (a * ListCache.LineHeight) - VerticalScrollBar.Value + headerHeight;
                    //Console.WriteLine("SongGridViewControl - MouseMove - Checking for setting mouse over flag for line {0} - offsetY: {1}", a, offsetY);

                    // Check if the mouse cursor is over this line (and not already mouse over)
                    if (x >= HorizontalScrollBar.Value &&
                        y >= offsetY &&
                        y <= offsetY + ListCache.LineHeight &&
                        !IsRowEmpty(a) &&
                        MouseOverRowIndex != a)
                    {
                        // Set item as mouse over
                        //Console.WriteLine("SongGridViewControl - MouseMove - Mouse is over item {0} {1}/{2}/{3}", a, Items[a].AudioFile.ArtistName, Items[a].AudioFile.AlbumTitle, Items[a].AudioFile.Title);
                        MouseOverRowIndex = a;
                        break;
                    }
                }
            }
        }

        public virtual void MouseLeave()
        {
            IsMouseOverControl = false;   

            if (ListCache == null)
                return;

            bool controlNeedsToBePartiallyInvalidated = false;
            var partialRect = new BasicRectangle();

            int scrollbarOffsetY = (StartLineNumber * ListCache.LineHeight) - VerticalScrollBar.Value;
            if (GetRowCount() > 0)
            {
                for (int b = StartLineNumber; b < StartLineNumber + NumberOfLinesToDraw; b++)
                {
                    if(MouseOverRowIndex == b)
                    {
                        MouseOverRowIndex = -1;
                        var newPartialRect = new BasicRectangle(HorizontalScrollBar.Value, ((b - StartLineNumber + 1) * ListCache.LineHeight) + scrollbarOffsetY, Frame.Width + HorizontalScrollBar.Value, ListCache.LineHeight);
                        partialRect.Merge(newPartialRect);
                        controlNeedsToBePartiallyInvalidated = true;
                        break;
                    }
                }
            }

            if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);             
        }

        public virtual void MouseEnter()
        {
            IsMouseOverControl = true;
        }

        public virtual void MouseWheel(float delta)
        {
            if (ListCache == null)
                return;

            // Make sure the mouse cursor is over the control, and that the vertical scrollbar is enabled
            if (!IsMouseOverControl || !VerticalScrollBar.Enabled)
                return;

            // Get relative value
            //int value = delta / SystemInformation.MouseWheelScrollDelta;

            int newValue = (int) (VerticalScrollBar.Value + (-delta * ListCache.LineHeight));
            //Console.WriteLine("SongGridViewControl - MouseWheel - delta: {0} VerticalScrollBar.Value: {1} lineHeight: {2} newValue: {3}", delta, VerticalScrollBar.Value, Cache.LineHeight, newValue);

            newValue = Math.Min(newValue, VerticalScrollBar.Maximum - VerticalScrollBar.LargeChange);
            newValue = Math.Max(newValue, 0);
            
            VerticalScrollBar.Value = newValue;
            InvalidateVisual();
        }

        #endregion

        public enum ContextMenuType
        {
            Item = 0, Header = 1
        }
    }
}
