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

using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MPfm.GenericControls.Wrappers;

namespace MPfm.WPF.Classes.Controls
{
    public class VerticalScrollBarWrapper : ScrollBar, IVerticalScrollBarWrapper
    {
        public event ScrollValueChanged OnScrollValueChanged;
        bool IVerticalScrollBarWrapper.Visible { get; set; }
        bool IVerticalScrollBarWrapper.Enabled { get; set; }
        int IVerticalScrollBarWrapper.Width { get; set; }
        int IVerticalScrollBarWrapper.Height { get; set; }
        int IVerticalScrollBarWrapper.Value { get; set; }
        int IVerticalScrollBarWrapper.Maximum { get; set; }
        int IVerticalScrollBarWrapper.Minimum { get; set; }
        int IVerticalScrollBarWrapper.SmallChange { get; set; }
        int IVerticalScrollBarWrapper.LargeChange { get; set; }

        public VerticalScrollBarWrapper()
        {
            Orientation = System.Windows.Controls.Orientation.Vertical;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Background = new SolidColorBrush(Colors.PowderBlue);
        }
    }
}
