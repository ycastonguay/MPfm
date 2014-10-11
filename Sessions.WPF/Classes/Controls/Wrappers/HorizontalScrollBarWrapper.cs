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

//using System.Windows.Forms;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Sessions.GenericControls.Wrappers;

namespace Sessions.WPF.Classes.Controls
{
    public class HorizontalScrollBarWrapper : ScrollBar, IHorizontalScrollBarWrapper
    {
        public event ScrollValueChanged OnScrollValueChanged;

        bool IHorizontalScrollBarWrapper.Visible
        {
            get { return Visibility == Visibility.Visible; }
            set { Visibility = value ? Visibility.Visible : Visibility.Hidden; }
        }

        bool IHorizontalScrollBarWrapper.Enabled { get { return IsEnabled; } set { IsEnabled = value; } }
        int IHorizontalScrollBarWrapper.Width { get { return (int)Width; } set { Width = value; } }
        int IHorizontalScrollBarWrapper.Height { get { return (int)Height; } set { Height = value; } }
        int IHorizontalScrollBarWrapper.Value { get { return (int)Value; } set { Value = value; } }
        int IHorizontalScrollBarWrapper.Minimum { get { return (int)Minimum; } set { Minimum = value; } }
        int IHorizontalScrollBarWrapper.Maximum { get { return (int)Maximum; } set { Maximum = value; } }
        int IHorizontalScrollBarWrapper.SmallChange { get { return (int)SmallChange; } set { SmallChange = value; } }
        int IHorizontalScrollBarWrapper.LargeChange { get { return (int)LargeChange; } set { LargeChange = value; } }

        public HorizontalScrollBarWrapper()
        {
            Orientation = Orientation.Horizontal;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            //Console.WriteLine("HorizontalScrollBarWrapper - OnValueChanged - newValue: {0} (min: {1} max: {2})", newValue, Minimum, Maximum);
            base.OnValueChanged(oldValue, newValue);
            if (OnScrollValueChanged != null)
                OnScrollValueChanged(this, new EventArgs());
        }

    }
}
