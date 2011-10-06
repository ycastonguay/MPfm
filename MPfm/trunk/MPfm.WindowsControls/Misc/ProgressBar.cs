//
// ProgressBar.cs: This progress bar control is based on the System.Windows.Forms.ProgressBar control.
//                 It adds custom drawing and other features.
//
// Copyright © 2011 Yanick Castonguay
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
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This progress bar control is based on the System.Windows.Forms.ProgressBar control.
    /// It adds custom drawing and other features.
    /// </summary>
    public class ProgressBar : System.Windows.Forms.ProgressBar
    {
        /// <summary>
        /// Default constructor for ProgressBar.
        /// </summary>
        public ProgressBar()
        {
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);            
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //Region iRegion = new Region(e.ClipRectangle);
            //e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);

            base.OnPaint(e);
        }

        #endregion
    }
}
