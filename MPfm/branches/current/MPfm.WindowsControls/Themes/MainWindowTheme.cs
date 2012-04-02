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
using System.Xml.Serialization;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Defines a theme object for the MPfm main window.
    /// </summary>
    public class MainWindowTheme
    {
        #region Panel

        /// <summary>
        /// Private value for the PanelHeaderBackground property.
        /// </summary>
        private TextGradient panelHeaderBackground = new TextGradient(Color.FromArgb(100, 100, 100), Color.FromArgb(60, 60, 60), LinearGradientMode.Vertical,
                                                                      Color.FromArgb(60, 60, 60), 0, new CustomFont("TitilliumText22L Lt", 10, Color.White));
        /// <summary>
        /// Defines the main panel header background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel header background gradient.")]
        public TextGradient PanelHeaderBackground
        {
            get
            {
                return panelHeaderBackground;
            }
            set
            {
                panelHeaderBackground = value;
            }
        }

        /// <summary>
        /// Private value for the PanelBackground property.
        /// </summary>
        private Gradient panelBackground = new Gradient(Color.FromArgb(20, 20, 20), Color.FromArgb(40, 40, 40), LinearGradientMode.Vertical);
        /// <summary>
        /// Defines the main panel header background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel background gradient.")]
        public Gradient PanelBackground
        {
            get
            {
                return panelBackground;
            }
            set
            {
                panelBackground = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTitleFont property.
        /// </summary>
        private CustomFont panelTitleFont = new CustomFont("TitilliumText22L Lt", 18, Color.White);
        /// <summary>
        /// Defines the main panel title font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel title font.")]
        public CustomFont PanelTitleFont
        {
            get
            {
                return panelTitleFont;
            }
            set
            {
                panelTitleFont = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTitle2Font property.
        /// </summary>
        private CustomFont panelTitle2Font = new CustomFont("TitilliumText22L Lt", 14, Color.Silver);
        /// <summary>
        /// Defines the main panel title 2 font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel title 2 font.")]
        public CustomFont PanelTitle2Font
        {
            get
            {
                return panelTitle2Font;
            }
            set
            {
                panelTitle2Font = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTitle3Font property.
        /// </summary>
        private CustomFont panelTitle3Font = new CustomFont("TitilliumText22L Lt", 12, Color.Gray);
        /// <summary>
        /// Defines the main panel title 3 font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel title 3 font.")]
        public CustomFont PanelTitle3Font
        {
            get
            {
                return panelTitle3Font;
            }
            set
            {
                panelTitle3Font = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTextFont property.
        /// </summary>
        private CustomFont panelTextFont = new CustomFont();
        /// <summary>
        /// Defines the main panel text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel text font.")]
        public CustomFont PanelTextFont
        {
            get
            {
                return panelTextFont;
            }
            set
            {
                panelTextFont = value;
            }
        }

        /// <summary>
        /// Private value for the PanelMonospacedFont property.
        /// </summary>
        private CustomFont panelMonospacedFont = new CustomFont("Droid Sans Mono", 10, Color.White);
        /// <summary>
        /// Defines the main panel monospaced font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel monospaced font.")]
        public CustomFont PanelMonospacedFont
        {
            get
            {
                return panelMonospacedFont;
            }
            set
            {
                panelMonospacedFont = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSmallMonospacedFont property.
        /// </summary>
        private CustomFont panelSmallMonospacedFont = new CustomFont("Droid Sans Mono", 7, Color.White);
        /// <summary>
        /// Defines the main panel small monospaced font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel small monospaced font.")]
        public CustomFont PanelSmallMonospacedFont
        {
            get
            {
                return panelSmallMonospacedFont;
            }
            set
            {
                panelSmallMonospacedFont = value;
            }
        }

        #endregion

        #region Secondary Panel

        /// <summary>
        /// Private value for the SecondaryPanelHeaderBackground property.
        /// </summary>
        private TextGradient secondaryPanelHeaderBackground = new TextGradient(Color.FromArgb(50, 50, 50), Color.FromArgb(100, 100, 100), LinearGradientMode.Vertical,
                                                                      Color.FromArgb(60, 60, 60), 0, new CustomFont("Junction", 8.25f, Color.White));
        /// <summary>
        /// Defines the secondary panel header background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel header background gradient.")]
        public TextGradient SecondaryPanelHeaderBackground
        {
            get
            {
                return secondaryPanelHeaderBackground;
            }
            set
            {
                secondaryPanelHeaderBackground = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelBackground property.
        /// </summary>
        private TextGradient secondaryPanelBackground = new TextGradient(Color.FromArgb(20, 20, 20), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical, 
                                                                         Color.Gray, 0, new CustomFont("Junction", 7.0f, Color.White));
        /// <summary>
        /// Defines the secondary panel background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel background gradient.")]
        public TextGradient SecondaryPanelBackground
        {
            get
            {
                return secondaryPanelBackground;
            }
            set
            {
                secondaryPanelBackground = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelLabelFont property.
        /// </summary>
        private CustomFont secondaryPanelLabelFont = new CustomFont("Junction", 7, Color.Silver);
        /// <summary>
        /// Defines the secondary panel label font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel label font.")]
        public CustomFont SecondaryPanelLabelFont
        {
            get
            {
                return secondaryPanelLabelFont;
            }
            set
            {
                secondaryPanelLabelFont = value;
            }
        }

        #endregion

        #region Toolbar

        #region Toolbar Buttons

        /// <summary>
        /// Private value for the ToolbarButtonBackground property.
        /// </summary>
        private TextGradient toolbarButtonBackground = new TextGradient(Color.LightGray, Color.Gray, LinearGradientMode.Vertical, Color.Gray, 
                                                                        1, new CustomFont("Junction", 8.0f, Color.Black));
        /// <summary>
        /// Defines the toolbar button background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar button background gradient.")]
        public TextGradient ToolbarButtonBackground
        {
            get
            {
                return toolbarButtonBackground;
            }
            set
            {
                toolbarButtonBackground = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonMouseOverBackground property.
        /// </summary>
        private TextGradient toolbarButtonMouseOverBackground = new TextGradient(Color.White, Color.LightGray, LinearGradientMode.Vertical, 
                                                                                 Color.Gray, 1, new CustomFont("Junction", 8.0f, Color.Black));
        /// <summary>
        /// Defines the toolbar button background gradient (when the mouse cursor is over).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar button background gradient (when the mouse cursor is over).")]
        public TextGradient ToolbarButtonMouseOverBackground
        {
            get
            {
                return toolbarButtonMouseOverBackground;
            }
            set
            {
                toolbarButtonMouseOverBackground = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonDisabledBackground property.
        /// </summary>
        private TextGradient toolbarButtonDisabledBackground = new TextGradient(Color.White, Color.LightGray, LinearGradientMode.Vertical, 
                                                                                Color.Gray, 1, new CustomFont("Junction", 8.0f, Color.Black));
        /// <summary>
        /// Defines the toolbar button background gradient (when the mouse cursor is over).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar button background gradient (when the mouse cursor is over).")]
        public TextGradient ToolbarButtonDisabledBackground
        {
            get
            {
                return toolbarButtonDisabledBackground;
            }
            set
            {
                toolbarButtonDisabledBackground = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the ToolbarBackground property.
        /// </summary>
        private TextGradient toolbarBackground = new TextGradient(Color.Silver, Color.Gainsboro, LinearGradientMode.Vertical, 
                                                                  Color.Silver, 0, new CustomFont("Junction", 8.0f, Color.Black));
        /// <summary>
        /// Defines the toolbar background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar background gradient.")]
        public TextGradient ToolbarBackground
        {
            get
            {
                return toolbarBackground;
            }
            set
            {
                toolbarBackground = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for the MainWindowTheme class.
        /// </summary>
        public MainWindowTheme()
        {
        }
    }
}
