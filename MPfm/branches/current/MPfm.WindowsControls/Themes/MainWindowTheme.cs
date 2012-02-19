//
// MainWindowTheme.cs: Defines a theme object for the MPfm main window.
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Defines a theme object for the MPfm main window.
    /// </summary>
    public class MainWindowTheme
    {

        #region Header
        
        /// <summary>
        /// Private value for the HeaderBackgroundColor1 property.
        /// </summary>
        private Color m_headerBackgroundColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("First color of the header background gradient.")]
        public Color HeaderBackgroundColor1
        {
            get
            {
                return m_headerBackgroundColor1;
            }
            set
            {
                m_headerBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderBackgroundColor2 property.
        /// </summary>
        private Color m_headerBackgroundColor2 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("Second color of the header background gradient.")]
        public Color HeaderBackgroundColor2
        {
            get
            {
                return m_headerBackgroundColor2;
            }
            set
            {
                m_headerBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderForeColor property.
        /// </summary>
        private Color m_headerForeColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Header fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("Header fore color.")]
        public Color HeaderForeColor
        {
            get
            {
                return m_headerForeColor;
            }
            set
            {
                m_headerForeColor = value;
            }
        }

        /// <summary>
        /// Private value for the Font property.
        /// </summary>
        private CustomFont m_headerFont = new CustomFont();
        /// <summary>
        /// Defines the font used for the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("Font used for the header.")]
        public CustomFont HeaderFont
        {
            get
            {
                return m_headerFont;
            }
            set
            {
                m_headerFont = value;
            }
        }

        #endregion

        #region Panel

        #region Panel Header

        /// <summary>
        /// Private value for the PanelHeaderBackgroundColor1 property.
        /// </summary>
        private Color m_panelHeaderBackgroundColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Panel"), Browsable(true), Description("First color of the panelHeader background gradient.")]
        public Color PanelHeaderBackgroundColor1
        {
            get
            {
                return m_panelHeaderBackgroundColor1;
            }
            set
            {
                m_panelHeaderBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelHeaderBackgroundColor2 property.
        /// </summary>
        private Color m_panelHeaderBackgroundColor2 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Panel"), Browsable(true), Description("Second color of the panelHeader background gradient.")]
        public Color PanelHeaderBackgroundColor2
        {
            get
            {
                return m_panelHeaderBackgroundColor2;
            }
            set
            {
                m_panelHeaderBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelHeaderForeColor property.
        /// </summary>
        private Color m_panelHeaderForeColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// PanelHeader fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Panel"), Browsable(true), Description("PanelHeader fore color.")]
        public Color PanelHeaderForeColor
        {
            get
            {
                return m_panelHeaderForeColor;
            }
            set
            {
                m_panelHeaderForeColor = value;
            }
        }

        /// <summary>
        /// Private value for the Font property.
        /// </summary>
        private CustomFont m_panelHeaderFont = new CustomFont();
        /// <summary>
        /// Defines the font used for the panelHeader.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Panel"), Browsable(true), Description("Font used for the panelHeader.")]
        public CustomFont PanelHeaderFont
        {
            get
            {
                return m_panelHeaderFont;
            }
            set
            {
                m_panelHeaderFont = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the PanelBackgroundColor1 property.
        /// </summary>
        private Color m_panelBackgroundColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Panel"), Browsable(true), Description("First color of the panel background gradient.")]
        public Color PanelBackgroundColor1
        {
            get
            {
                return m_panelBackgroundColor1;
            }
            set
            {
                m_panelBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelBackgroundColor2 property.
        /// </summary>
        private Color m_panelBackgroundColor2 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Panel"), Browsable(true), Description("Second color of the panel background gradient.")]
        public Color PanelBackgroundColor2
        {
            get
            {
                return m_panelBackgroundColor2;
            }
            set
            {
                m_panelBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelForeColor property.
        /// </summary>
        private Color m_panelForeColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Panel fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Panel"), Browsable(true), Description("Panel fore color.")]
        public Color PanelForeColor
        {
            get
            {
                return m_panelForeColor;
            }
            set
            {
                m_panelForeColor = value;
            }
        }

        /// <summary>
        /// Private value for the Font property.
        /// </summary>
        private CustomFont m_panelFont = new CustomFont();
        /// <summary>
        /// Defines the font used for the panel.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Panel"), Browsable(true), Description("Font used for the panel.")]
        public CustomFont PanelFont
        {
            get
            {
                return m_panelFont;
            }
            set
            {
                m_panelFont = value;
            }
        }

        #endregion

        // x Header color1/2
        // x Header fore color
        // x Header font

        // Background color1/2
        // Content fore color
        // Content font
        // x Panel header color 1/2
        // x Panel header fore color
        // x Panel header font
        // x Panel color 1/2
        // x Panel text fore color        
        // x Panel text font
        // Panel link color
        // Panel link font

        // Toolbar border
        // Toolbar color1/2
        // Toolbar fore color
        // Toolbar font  
        // Toolbar button color 1/2 and so on


        /// <summary>
        /// Default constructor for the MainWindowTheme class.
        /// </summary>
        public MainWindowTheme()
        {

        }
    }
}
