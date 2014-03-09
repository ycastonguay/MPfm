﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MPfm.GenericControls.Controls.Songs;
using MPfm.Sound.AudioFiles;
using MPfm.WPF.Classes.Controls.Graphics;
using MPfm.WPF.Classes.Controls.Helpers;

namespace MPfm.WPF.Classes.Controls
{
    public class SongGridView : DockPanel
    {
        private SongGridViewControl _control;
        private HorizontalScrollBarWrapper _horizontalScrollBar;
        private VerticalScrollBarWrapper _verticalScrollBar;
        private WindowsFormsHost _hostHorizontalScrollBar;
        private WindowsFormsHost _hostVerticalScrollBar;

        public SongGridView()
            : base()
        {
            // Create wrappers for scrollbars so the generic control can interact with them
            _verticalScrollBar = new VerticalScrollBarWrapper();
            _verticalScrollBar.Width = 40;
            _verticalScrollBar.Height = 200;
            _verticalScrollBar.Minimum = 1;
            _verticalScrollBar.Maximum = 100;
            _verticalScrollBar.Value = 50;
            DockPanel.SetDock(_verticalScrollBar, Dock.Right);
            Children.Add(_verticalScrollBar);

            _horizontalScrollBar = new HorizontalScrollBarWrapper();
            _horizontalScrollBar.Width = 200;
            _horizontalScrollBar.Height = 40;
            _horizontalScrollBar.Minimum = 1;
            _horizontalScrollBar.Maximum = 100;
            _horizontalScrollBar.Value = 50;
            DockPanel.SetDock(_horizontalScrollBar, Dock.Bottom);
            Children.Add(_horizontalScrollBar);

            _control = new SongGridViewControl(_horizontalScrollBar, _verticalScrollBar);
            _control.OnInvalidateVisual += () => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(InvalidateVisual));
            _control.OnInvalidateVisualInRect += (rect) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                InvalidateVisual();
                // TODO: It seems you can't invalidate a specific rect in WPF? What?
                // http://stackoverflow.com/questions/2576599/possible-to-invalidatevisual-on-a-given-region-instead-of-entire-wpf-control                                                                                                                       
            }));
        }

        public void ImportAudioFiles(List<AudioFile> audioFiles)
        {
            _control.ImportAudioFiles(audioFiles);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            var wrapper = new GraphicsContextWrapper(dc, (float)ActualWidth, (float)ActualHeight);
            _control.Render(wrapper);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            GenericControlHelper.MouseDown(e, this, _control);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            GenericControlHelper.MouseUp(e, this, _control);
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

        //protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        //{
        //    GenericControlHelper.MouseDoubleClick(e, this, _control);
        //    base.OnMouseDoubleClick(e);
        //}
    }
}
