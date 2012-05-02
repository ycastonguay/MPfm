﻿//
// ControlExtensions.cs: This static class adds extension methods to the Control class.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This custom version of TableLayoutPanel adds a gradient properties for the background.
    /// </summary>
    public class TableLayoutPanel : System.Windows.Forms.TableLayoutPanel
    {
        /// <summary>
        /// Private value for the Theme property.
        /// </summary>
        private TableLayoutPanelTheme theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        public TableLayoutPanelTheme Theme
        {
            get
            {
                return theme;
            }
            set
            {
                theme = value;
                Refresh();
            }
        }

        /// <summary>
        /// Default constructor for the FlowLayoutPanel class.
        /// </summary>
        public TableLayoutPanel() 
            : base()
        {
            // Create default theme
            theme = new TableLayoutPanelTheme();
        }

        /// <summary>
        /// Occurs when the control needs to draw the background.
        /// Renders a gradient background as set in the Theme property.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Skip base event to prevent flicker
            //base.OnPaintBackground(e);

            // Check if the gradient background should be used
            if (!theme.IsBackgroundTransparent)
            {
                // Draw background gradient (cover -1 pixel to fix graphic bug) 
                Rectangle rectBackground = new Rectangle(-1, -1, ClientRectangle.Width + 2, ClientRectangle.Height + 2);
                Rectangle rectBorder = new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);                
                PaintHelper.RenderBackgroundGradient(e.Graphics, rectBackground, rectBorder, theme.BackgroundGradient);
            }
            else
            {
                // Call paint background
                base.OnPaintBackground(e); // CPU intensive when transparent
            }
        }

        /// <summary>
        /// Occurs when the control is resized.
        /// Refreshes the background.
        /// </summary>
        /// <param name="eventargs">Event arguments</param>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);            
            Refresh();
        }
    }
}