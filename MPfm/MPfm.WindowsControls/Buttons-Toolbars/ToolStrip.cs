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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This toolstrip control is based on System.Windows.Forms.ToolStrip.
    /// It fixes an inactive window button click bug (see WndProc for more information).
    /// </summary>
    public class ToolStrip : System.Windows.Forms.ToolStrip
    {
        /// <summary>
        /// Default constructor for ToolStrip.
        /// </summary>
        public ToolStrip()
        {      
        }

        /// <summary>
        /// Pass the mouse events event when window is inactive
        /// Source: http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/d3b47e33-4fbc-45b9-9c57-e1697d6530ec/
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0021) // WM_MOUSEACTIVATE
            {
                base.WndProc(ref m);
                // Force result to be MA_ACTIVATE rather than MA_ACTIVATEANDEAT or MA_NOACTIVATE
                m.Result = (IntPtr)1; // MA_ACTIVATE
                return;
            }
            base.WndProc(ref m);
        }
    }
}
