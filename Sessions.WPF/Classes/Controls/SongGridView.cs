// Copyright © 2011-2013 Yanick Castonguay
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MPfm.WPF.Classes.Controls.Graphics;
using MPfm.WPF.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Songs;
using Sessions.GenericControls.Graphics;
using Sessions.MVP.Bootstrap;
using Sessions.Sound.AudioFiles;
using ModifierKeys = Sessions.GenericControls.Interaction.ModifierKeys;

namespace MPfm.WPF.Classes.Controls
{
    public class SongGridView : DockPanel
    {
        private SongGridViewControl _control;
        private HorizontalScrollBarWrapper _horizontalScrollBar;
        private VerticalScrollBarWrapper _verticalScrollBar;

        private ContextMenu _contextMenuItems;
        private ContextMenu _contextMenuHeader;

        public delegate void MenuItemClickedDelegate(MenuItemType menuItemType);
        public event EventHandler DoubleClick;
        public event MenuItemClickedDelegate MenuItemClicked;
 
        public List<SongGridViewItem> SelectedItems { get { return _control.SelectedItems; } }
        public Guid NowPlayingAudioFileId { get { return _control.NowPlayingAudioFileId; } set { _control.NowPlayingAudioFileId = value; } }

        public SongGridView()
            : base()
        {
            DoubleClick += (sender, e) => { };
            MenuItemClicked += type => { };
            Focusable = true;

            // Add dummy control so the scrollbar can be placed on the right
            var dummy = new Control();
            DockPanel.SetDock(dummy, Dock.Left);
            Children.Add(dummy);

            // Create wrappers for scrollbars so the generic control can interact with them
            _verticalScrollBar = new VerticalScrollBarWrapper();
            _verticalScrollBar.Width = 16;
            _verticalScrollBar.Height = Double.NaN;
            _verticalScrollBar.Minimum = 1;
            _verticalScrollBar.Maximum = 100;
            _verticalScrollBar.Margin = new Thickness(0, 20, 0, 20);
            DockPanel.SetDock(_verticalScrollBar, Dock.Right);
            Children.Add(_verticalScrollBar);

            _horizontalScrollBar = new HorizontalScrollBarWrapper();
            _horizontalScrollBar.Width = Double.NaN;
            _horizontalScrollBar.Height = 16;
            _horizontalScrollBar.Minimum = 1;
            _horizontalScrollBar.Maximum = 100;
            _horizontalScrollBar.VerticalAlignment = VerticalAlignment.Bottom;                
            DockPanel.SetDock(_horizontalScrollBar, Dock.Bottom);
            Children.Add(_horizontalScrollBar);

            var disposableImageFactory = Bootstrapper.GetContainer().Resolve<IDisposableImageFactory>();
            _control = new SongGridViewControl(_horizontalScrollBar, _verticalScrollBar, disposableImageFactory);
            _control.OnChangeMouseCursorType += GenericControlHelper.ChangeMouseCursor;
            _control.OnItemDoubleClick += (id, index) => DoubleClick(this, new EventArgs());
            _control.OnDisplayContextMenu += (type, x, y) => 
            {            
                // Create contextual menu
                //_menuColumns = new System.Windows.Forms.ContextMenuStrip();

                //// Loop through columns
                //foreach (SongGridViewColumn column in _columns)
                //{
                //    // Add menu item                               
                //    ToolStripMenuItem menuItem = (ToolStripMenuItem)_menuColumns.Items.Add(column.Title);
                //    menuItem.Tag = column.Title;
                //    menuItem.Checked = column.Visible;
                //    menuItem.Click += new EventHandler(menuItemColumns_Click);
                //}

                switch (type)
                {
                    case SongGridViewControl.ContextMenuType.Item:
                        _contextMenuItems.Placement = PlacementMode.MousePoint;
                        _contextMenuItems.PlacementTarget = this;
                        _contextMenuItems.Visibility = Visibility.Visible;
                        _contextMenuItems.IsOpen = true;
                        break;
                    case SongGridViewControl.ContextMenuType.Header:
                        _contextMenuHeader.Placement = PlacementMode.MousePoint;
                        _contextMenuHeader.PlacementTarget = this;
                        _contextMenuHeader.Visibility = Visibility.Visible;
                        _contextMenuHeader.IsOpen = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type");
                }
            };
            _control.OnInvalidateVisual += () => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                //Console.WriteLine("SongGridView - OnInvalidateVisual");
                InvalidateVisual();
            }));
            _control.OnInvalidateVisualInRect += (rect) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                //Console.WriteLine("SongGridView - OnInvalidateVisualInRect");
                InvalidateVisual();
                // TODO: It seems you can't invalidate a specific rect in WPF? What?
                // http://stackoverflow.com/questions/2576599/possible-to-invalidatevisual-on-a-given-region-instead-of-entire-wpf-control                                                                                                                       
            }));

            // Create context menu at the end to add columns from control
            CreateContextualMenu();
        }

        private void CreateContextualMenu()
        {
            _contextMenuItems = new ContextMenu();

            var menuItemPlaySong = new MenuItem();
            menuItemPlaySong.Header = "Play song(s)";
            menuItemPlaySong.Click += MenuItemPlaySongOnClick;
            _contextMenuItems.Items.Add(menuItemPlaySong);

            var menuItemAddToPlaylist = new MenuItem();
            menuItemAddToPlaylist.Header = "Add to playlist";
            menuItemAddToPlaylist.Click += MenuItemAddToPlaylistOnClick;
            _contextMenuItems.Items.Add(menuItemAddToPlaylist);

            _contextMenuHeader = new ContextMenu();
            foreach (var column in _control.Columns)
            {
                var menuItem = new MenuItem();
                menuItem.Header = column.FieldName;
                _contextMenuHeader.Items.Add(menuItem);
            }
        }

        private void MenuItemPlaySongOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItemClicked(MenuItemType.PlaySongs);
        }

        private void MenuItemAddToPlaylistOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItemClicked(MenuItemType.AddToPlaylist);
        }

        public void ImportAudioFiles(List<AudioFile> audioFiles)
        {
            _control.ImportAudioFiles(audioFiles);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Clip drawing to make sure we don't draw outside the control
            dc.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));
            // TODO: Fix this, dirty rects in WPF is still a mystery. It was simple in WinForms, why is it hidden in WPF?
            var dirtyRect = new BasicRectangle(0, 0, (float)ActualWidth, (float)ActualHeight); 
            var wrapper = new GraphicsContextWrapper(dc, (float)ActualWidth, (float)ActualHeight, dirtyRect);
            _control.Render(wrapper);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            GenericControlHelper.MouseDown(e, this, _control);
            if (e.ClickCount == 1)
                GenericControlHelper.MouseClick(e, this, _control);
            else if (e.ClickCount == 2)
                GenericControlHelper.MouseDoubleClick(e, this, _control);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            GenericControlHelper.MouseUp(e, this, _control);
            Focus();
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            GenericControlHelper.MouseMove(e, this, _control);
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            _control.MouseEnter();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            _control.MouseLeave();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
                _control.MouseWheel(2);
            else if(e.Delta < 0)
                _control.MouseWheel(-2);
            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            char key = (char) KeyInterop.VirtualKeyFromKey(e.Key);
            _control.KeyDown(key, GenericControlHelper.GetSpecialKeys(e.Key), ModifierKeys.None, e.IsRepeat);
            e.Handled = true;
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            char key = (char)KeyInterop.VirtualKeyFromKey(e.Key);
            _control.KeyUp(key, GenericControlHelper.GetSpecialKeys(e.Key), ModifierKeys.None, e.IsRepeat);
            base.OnKeyUp(e);
        }

        public enum MenuItemType
        {
            PlaySongs = 0,
            AddToPlaylist = 1
        }
    }
}
