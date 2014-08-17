// Copyri3w2qght © 2011-2013 Yanick Castonguay
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Web.UI.WebControls;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Items;
using Sessions.GenericControls.Controls.Songs;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Interaction;
using Sessions.GenericControls.Wrappers;
using Sessions.WindowsControls;
using Sessions.Core;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;

namespace Sessions.GenericControls.Controls.Base
{
    /// <summary>
    /// Base class for grid view style controls; adds the concept of columns.
    /// </summary>
    public abstract class GridViewControlBase<T, U> : ListViewControlBase<T> where T : ListViewItem where U : GridViewColumn  //, IControlMouseInteraction, IControlKeyboardInteraction
    {
        private List<U> _columns;
        [Browsable(false)]
        public List<U> Columns
        {
            get
            {
                return _columns;
            }
        }

        public delegate void ColumnClickDelegate(int index);
        public event ColumnClickDelegate OnColumnClick;

        protected GridViewControlBase(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar) 
            : base(horizontalScrollBar, verticalScrollBar)
        {
            _columns = new List<U>();
        }

        protected void ColumnClick(int index)
        {
            if (OnColumnClick != null)
                OnColumnClick(index);
        }
    }
}
