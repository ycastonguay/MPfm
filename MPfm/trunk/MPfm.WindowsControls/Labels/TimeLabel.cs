//
// Label.cs: This label control is based on the System.Windows.Forms.Label control.
//           It adds custom drawing, supports embedded fonts and other features.
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
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This label control is based on the System.Windows.Forms.Label control.
    /// It adds custom drawing, supports embedded fonts and other features.
    /// </summary>
    public class TimeLabel : System.Windows.Forms.Label
    {
        /// <summary>
        /// Default constructor for Label.
        /// </summary>
        public TimeLabel()
        {            
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);        
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            //base.OnPaintBackground(pe);

            System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file = thisExe.GetManifestResourceStream("MPfm.WindowsControls.Resources.1.png");
            Image image = Image.FromStream(file);

            Rectangle rect1 = new Rectangle(0, 0, 19, 30);
            g.DrawImage(image, rect1);

            Rectangle rect2 = new Rectangle(19, 0, 19, 30);
            g.DrawImage(image, rect2);

        }

        #endregion
    }
}
