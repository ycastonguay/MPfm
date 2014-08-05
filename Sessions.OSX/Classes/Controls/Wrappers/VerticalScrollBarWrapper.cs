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

using MonoMac.AppKit;
using Sessions.GenericControls.Wrappers;
using System;
using MonoMac.Foundation;

namespace Sessions.OSX.Classes.Controls
{
    public class VerticalScrollBarWrapper : NSScroller, IVerticalScrollBarWrapper
    {
        public event ScrollValueChanged OnScrollValueChanged;

        bool IVerticalScrollBarWrapper.Visible { get; set; }
        bool IVerticalScrollBarWrapper.Enabled { get { return Enabled; } set { Enabled = value; } }
        int IVerticalScrollBarWrapper.Width { get { return (int)Frame.Width; } set { 
                var frame = Frame;
                frame.Width = value; 
                Frame = frame;
            } }
        int IVerticalScrollBarWrapper.Height { get { return (int)Frame.Height; } set { 
                var frame = Frame;
                frame.Height = value; 
                Frame = frame;
            } }

        private float _lastValue = 0;
        private float _value = 0;
        int IVerticalScrollBarWrapper.Value { get { 
                int newValue = (int)(FloatValue * _maximum);
                //Console.WriteLine("Get Value - value: {0} newValue: {1} _value: {2}", FloatValue, newValue, _value);

                // Can't find any other way to get the value change, unfortunately :(
                if(FloatValue != _value)
                {
                    _value = FloatValue;
                    //Console.WriteLine("VALUE CHANGED! {0}", _value);
                }
                return newValue;
            } set { 
                float newValue = (float)value / (float)_maximum;
                //Console.WriteLine("IVerticalScrollBarWrapper - Set Value to {0} ({1}) -- min: {2} max: {3}", value, newValue, _minimum, _maximum);
                FloatValue = newValue; 
            } }

        private int _minimum = 0;
        int IVerticalScrollBarWrapper.Minimum { get { return _minimum; } set { _minimum = value; } }

        private int _maximum = 100;
        int IVerticalScrollBarWrapper.Maximum { get { return _maximum; } set { _maximum = value; } }

        // Doesn't matter
        int IVerticalScrollBarWrapper.SmallChange { get { return 5; } set { } }
        int IVerticalScrollBarWrapper.LargeChange { get { return 10; } set { } }

        public VerticalScrollBarWrapper()
        {
            //Orientation = Orientation.Horizontal;
            Continuous = true;
            Action = new MonoMac.ObjCRuntime.Selector("scrollAction:");
        }

        public override void DrawKnob()
        {
            base.DrawKnob();

            // I tried multiple events but this is the only reliable way to notify scroll bar value change
            if (_lastValue != FloatValue)
            {
                _lastValue = FloatValue;

                if (OnScrollValueChanged != null)
                    OnScrollValueChanged(this, new EventArgs());
            }
        }
    }
}
