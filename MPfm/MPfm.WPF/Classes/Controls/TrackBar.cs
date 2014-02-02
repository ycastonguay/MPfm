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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MPfm.GenericControls.Controls;
using MPfm.GenericControls.Interaction;
using MPfm.WPF.Classes.Controls.Graphics;
using MPfm.WPF.Classes.Extensions;
using Control = System.Windows.Controls.Control;

namespace MPfm.WPF.Classes.Controls
{
    public class TrackBar : Control
    {
        private TrackBarControl _control;

        // TODO: Move to ifaderproperties
        public int Minimum { get { return _control.Minimum; } set { _control.Minimum = value; } }
        public int Maximum { get { return _control.Maximum; } set { _control.Maximum = value; } }
        public int Value { get { return _control.Value; } set { _control.Value = value; } }
        public int ValueWithoutEvent { get { return _control.ValueWithoutEvent; } set { _control.ValueWithoutEvent = value; } }

        public event TrackBarControl.TrackBarValueChanged OnTrackBarValueChanged;
        public TrackBar()
            : base()
        {
            _control = new TrackBarControl();
            _control.OnTrackBarValueChanged += () =>
            {
                if (OnTrackBarValueChanged != null)
                    OnTrackBarValueChanged();
            };
            _control.OnInvalidateVisual += () => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(InvalidateVisual));
            _control.OnInvalidateVisualInRect += (rect) => Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                InvalidateVisual();
                // TODO: It seems you can't invalidate a specific rect in WPF? What?
                // http://stackoverflow.com/questions/2576599/possible-to-invalidatevisual-on-a-given-region-instead-of-entire-wpf-control                                                                                                                       
            }));
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            var wrapper = new GraphicsContextWrapper(dc, (float)ActualWidth, (float)ActualHeight);
            _control.Render(wrapper);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var location = e.GetPosition(this);
            float x = (float)location.X;
            float y = (float)location.Y;
            CaptureMouse();
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    _control.MouseDown(x, y, MouseButtonType.Left);
                    break;
                case MouseButton.Middle:
                    _control.MouseDown(x, y, MouseButtonType.Middle);
                    break;
                case MouseButton.Right:
                    _control.MouseDown(x, y, MouseButtonType.Right);
                    break;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            var location = e.GetPosition(this);
            float x = (float)location.X;
            float y = (float)location.Y;
            ReleaseMouseCapture();
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    _control.MouseUp(x, y, MouseButtonType.Left);
                    break;
                case MouseButton.Middle:
                    _control.MouseUp(x, y, MouseButtonType.Middle);
                    break;
                case MouseButton.Right:
                    _control.MouseUp(x, y, MouseButtonType.Right);
                    break;
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var location = e.GetPosition(this);
            float x = (float)location.X;
            float y = (float)location.Y;
            if (e.LeftButton == MouseButtonState.Pressed)
                _control.MouseMove(x, y, MouseButtonType.Left);
            else if (e.MiddleButton == MouseButtonState.Pressed)
                _control.MouseMove(x, y, MouseButtonType.Middle);
            else if (e.RightButton == MouseButtonState.Pressed)
                _control.MouseMove(x, y, MouseButtonType.Right);
            base.OnMouseMove(e);
        }
    }
}
