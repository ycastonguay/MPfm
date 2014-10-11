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
using System.Windows;
using System.Windows.Controls.Primitives;
using Sessions.GenericControls.Wrappers;

namespace Sessions.WPF.Classes.Controls
{
    public class VerticalScrollBarWrapper : ScrollBar, IVerticalScrollBarWrapper
    {
        public event ScrollValueChanged OnScrollValueChanged;

        bool IVerticalScrollBarWrapper.Visible
        {
            get { return this.Visibility == Visibility.Visible; }
            set { this.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
        }

        bool IVerticalScrollBarWrapper.Enabled { get { return IsEnabled; } set { IsEnabled = value; } }
        int IVerticalScrollBarWrapper.Width { get { return (int) Width; } set { Width = value; } }
        int IVerticalScrollBarWrapper.Height { get { return (int) Height; } set { Height = value; } }
        int IVerticalScrollBarWrapper.Value { get { return (int)Value; } set { Value = value; } }
        int IVerticalScrollBarWrapper.Minimum { get { return (int)Minimum; } set { Minimum = value; } }
        int IVerticalScrollBarWrapper.Maximum { get { return (int)Maximum; } set { Maximum = value; } }
        int IVerticalScrollBarWrapper.SmallChange { get { return (int)SmallChange; } set { SmallChange = value; } }
        int IVerticalScrollBarWrapper.LargeChange { get { return (int)LargeChange; } set { LargeChange = value; } }

        public VerticalScrollBarWrapper()
        {
            Orientation = System.Windows.Controls.Orientation.Vertical;            
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            //Console.WriteLine("VerticalScrollBarWrapper - OnValueChanged - newValue: {0} (min: {1} max: {2})", newValue, Minimum, Maximum);
            base.OnValueChanged(oldValue, newValue);
            if(OnScrollValueChanged != null)
                OnScrollValueChanged(this, new EventArgs());
        }
    }
}
