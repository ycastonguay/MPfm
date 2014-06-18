// Copyright © 2011-2013 Yanick Castonguay
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Sessions.WPF.Classes.Controls.Graphics;
using Sessions.WPF.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls;
using Control = System.Windows.Controls.Control;

namespace Sessions.WPF.Classes.Controls
{
    /// <summary>
    /// The Fader control is a vertical track bar with the appearance of a fader.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    public class Fader : Control
    {
        private FaderControl _control;

        // TODO: Move to ifaderproperties
        public int Minimum { get { return _control.Minimum; } set { _control.Minimum = value; } }
        public int Maximum { get { return _control.Maximum; } set { _control.Maximum = value; } }
        public int Value { get { return _control.Value; } set { _control.Value = value; } }
        public int ValueWithoutEvent { get { return _control.ValueWithoutEvent; } set { _control.ValueWithoutEvent = value; } }

        public event FaderControl.FaderValueChanged OnFaderValueChanged;

        public Fader()
            : base()
        {
            _control = new FaderControl();
            _control.OnFaderValueChanged += (sender, args) =>
            {
                if (OnFaderValueChanged != null)
                    OnFaderValueChanged(this, args);
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
            // TODO: Fix this, dirty rects in WPF is still a mystery. It was simple in WinForms, why is it hidden in WPF?
            var dirtyRect = new BasicRectangle(0, 0, (float)ActualWidth, (float)ActualHeight); 
            var wrapper = new GraphicsContextWrapper(dc, (float)ActualWidth, (float)ActualHeight, dirtyRect);
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

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                _control.MouseWheel(2);
            else if (e.Delta < 0)
                _control.MouseWheel(-2);
            base.OnMouseWheel(e);
        }
    }
}
