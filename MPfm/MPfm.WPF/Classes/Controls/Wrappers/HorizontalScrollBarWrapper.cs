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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MPfm.GenericControls.Controls;
using MPfm.GenericControls.Interaction;
using MPfm.GenericControls.Wrappers;
using MPfm.WPF.Classes.Controls.Graphics;
using MPfm.WPF.Classes.Controls.Helpers;
using MPfm.WPF.Classes.Extensions;
using Control = System.Windows.Controls.Control;

namespace MPfm.WPF.Classes.Controls
{
    public class HorizontalScrollBarWrapper : Control, IHorizontalScrollBarWrapper
    {
        public event ScrollValueChanged OnScrollValueChanged;
        public bool Visible { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Value { get; set; }
        public int Maximum { get; set; }
        public int LargeChange { get; set; }
    }
}
