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

using MonoMac.AppKit;
using MPfm.GenericControls.Wrappers;
using System;
using MonoMac.Foundation;

namespace MPfm.Mac.Classes.Controls
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
        int IVerticalScrollBarWrapper.Value { get { 
                Console.WriteLine("IVerticalScrollBarWrapper - Get Value {0}", FloatValue);
                return (int)FloatValue; 
            } set { 
                Console.WriteLine("IVerticalScrollBarWrapper - Set Value to {0}", value);
                FloatValue = value; 
            } }
//        int IHorizontalScrollBarWrapper.Minimum { get { return (int)Minimum; } set { Minimum = value; } }
//        int IHorizontalScrollBarWrapper.Maximum { get { return (int)Maximum; } set { Maximum = value; } }
//        int IHorizontalScrollBarWrapper.SmallChange { get { return (int)SmallChange; } set { SmallChange = value; } }
//        int IHorizontalScrollBarWrapper.LargeChange { get { return (int)LargeChange; } set { LargeChange = value; } }
        int IVerticalScrollBarWrapper.Minimum { get { return 0; } set { } }
        int IVerticalScrollBarWrapper.Maximum { get { return 0; } set { } }
        int IVerticalScrollBarWrapper.SmallChange { get { return 0; } set { } }
        int IVerticalScrollBarWrapper.LargeChange { get { return 0; } set { } }

        public VerticalScrollBarWrapper()
        {
            //Orientation = Orientation.Horizontal;
            Action = new MonoMac.ObjCRuntime.Selector("stuff:");
        }

        [Export ("stuff:")]
        private void Stuff(NSObject sender)
        {
            Console.WriteLine("STUFF");
        }


//        protected override void OnValueChanged(double oldValue, double newValue)
//        {
//            //Console.WriteLine("HorizontalScrollBarWrapper - OnValueChanged - newValue: {0} (min: {1} max: {2})", newValue, Minimum, Maximum);
//            base.OnValueChanged(oldValue, newValue);
//            if (OnScrollValueChanged != null)
//                OnScrollValueChanged(this, new EventArgs());
//        }
    }
}
