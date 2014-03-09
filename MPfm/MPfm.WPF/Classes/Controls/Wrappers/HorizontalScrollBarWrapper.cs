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

//using System.Windows.Forms;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MPfm.GenericControls.Wrappers;

namespace MPfm.WPF.Classes.Controls
{
    public class HorizontalScrollBarWrapper : ScrollBar, IHorizontalScrollBarWrapper
    {
        public event ScrollValueChanged OnScrollValueChanged;
        bool IHorizontalScrollBarWrapper.Visible { get; set; }
        bool IHorizontalScrollBarWrapper.Enabled { get; set; }
        int IHorizontalScrollBarWrapper.Width { get; set; }
        int IHorizontalScrollBarWrapper.Height { get; set; }
        int IHorizontalScrollBarWrapper.Value { get; set; }
        int IHorizontalScrollBarWrapper.Maximum { get; set; }
        int IHorizontalScrollBarWrapper.Minimum { get; set; }
        int IHorizontalScrollBarWrapper.SmallChange { get; set; }
        int IHorizontalScrollBarWrapper.LargeChange { get; set; }

        public HorizontalScrollBarWrapper()
        {
            Orientation = Orientation.Horizontal;
            HorizontalAlignment = HorizontalAlignment.Left;
            Background = new SolidColorBrush(Colors.HotPink);
        }
    }
}
