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

using System.Windows.Input;
using System.Windows.Media;
using MPfm.GenericControls.Controls;
using MPfm.GenericControls.Interaction;
using MPfm.WPF.Classes.Controls.Graphics;
using Control = System.Windows.Controls.Control;

namespace MPfm.WPF.Classes.Controls
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
                    OnFaderValueChanged(sender, args);
            };
            _control.OnInvalidateVisual += InvalidateVisual;
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
            float x = (float) location.X;
            float y = (float) location.Y;
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
            float x = (float) location.X;
            float y = (float) location.Y;
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
