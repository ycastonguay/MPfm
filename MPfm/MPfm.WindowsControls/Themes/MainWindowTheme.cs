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
        /// Private value for the PanelTheme property.
        /// </summary>
        private PanelTheme panelTheme = new PanelTheme()
            {
                HeaderTextGradient = new TextGradient(Color.FromArgb(100, 100, 100), Color.FromArgb(60, 60, 60), LinearGradientMode.Vertical,
                                                      Color.FromArgb(60, 60, 60), 0, new CustomFont("TitilliumText22L Lt", 10, Color.White)),
                BackgroundGradient = new BackgroundGradient(Color.FromArgb(20, 20, 20), Color.FromArgb(40, 40, 40), LinearGradientMode.Vertical, Color.Gray, 1)
            };
        /// <summary>
        /// Defines the main panel theme.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel theme.")]
        public PanelTheme PanelTheme
        {
            get
            {
                return panelTheme;
            }
            set
            {
                panelTheme = value;
            }
        }

        /// <summary>
        /// Private value for the PanelFontTitle property.
        /// </summary>
        private CustomFont panelFontTitle = new CustomFont("TitilliumText22L Lt", 18, Color.White);
        /// <summary>
        /// Defines the main panel title font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel title font.")]
        public CustomFont PanelFontTitle
        {
            get
            {
                return panelFontTitle;
            }
            set
            {
                panelFontTitle = value;
            }
        }

        /// <summary>
        /// Private value for the PanelFontTitle2 property.
        /// </summary>
        private CustomFont panelFontTitle2 = new CustomFont("TitilliumText22L Lt", 14, Color.Silver);
        /// <summary>
        /// Defines the main panel title 2 font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel title 2 font.")]
        public CustomFont PanelFontTitle2
        {
            get
            {
                return panelFontTitle2;
            }
            set
            {
                panelFontTitle2 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelFontTitle3 property.
        /// </summary>
        private CustomFont panelFontTitle3 = new CustomFont("TitilliumText22L Lt", 12, Color.Gray);
        /// <summary>
        /// Defines the main panel title 3 font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel title 3 font.")]
        public CustomFont PanelFontTitle3
        {
            get
            {
                return panelFontTitle3;
            }
            set
            {
                panelFontTitle3 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelFontText property.
        /// </summary>
        private CustomFont panelFontText = new CustomFont();
        /// <summary>
        /// Defines the main panel text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel text font.")]
        public CustomFont PanelFontText
        {
            get
            {
                return panelFontText;
            }
            set
            {
                panelFontText = value;
            }
        }

        /// <summary>
        /// Private value for the PanelFontMonospaced property.
        /// </summary>
        private CustomFont panelFontMonospaced = new CustomFont("Droid Sans Mono", 10, Color.White);
        /// <summary>
        /// Defines the main panel monospaced font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel monospaced font.")]
        public CustomFont PanelFontMonospaced
        {
            get
            {
                return panelFontMonospaced;
            }
            set
            {
                panelFontMonospaced = value;
            }
        }

        /// <summary>
        /// Private value for the PanelFontSmallMonospaced property.
        /// </summary>
        private CustomFont panelFontSmallMonospaced = new CustomFont("Droid Sans Mono", 7, Color.White);
        /// <summary>
        /// Defines the main panel small monospaced font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel small monospaced font.")]
        public CustomFont PanelFontSmallMonospaced
        {
            get
            {
                return panelFontSmallMonospaced;
            }
            set
            {
                panelFontSmallMonospaced = value;
            }
        }

        #endregion

        #region Secondary Panel

        /// <summary>
        /// Private value for the SecondaryPanelTheme property.
        /// </summary>
        private PanelTheme secondaryPanelTheme = new PanelTheme()
        {
            HeaderTextGradient = new TextGradient(Color.FromArgb(50, 50, 50), Color.FromArgb(100, 100, 100), LinearGradientMode.Vertical,
                                                  Color.FromArgb(60, 60, 60), 0, new CustomFont("Junction", 8.25f, Color.White)),
            BackgroundGradient = new BackgroundGradient(Color.FromArgb(20, 20, 20), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical, Color.Gray, 1)
        };
        /// <summary>
        /// Defines the secondary panel theme.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel theme.")]
        public PanelTheme SecondaryPanelTheme
        {
            get
            {
                return secondaryPanelTheme;
            }
            set
            {
                secondaryPanelTheme = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelFontLabel property.
        /// </summary>
        private CustomFont secondaryPanelFontLabel = new CustomFont("Junction", 8, Color.Silver);
        /// <summary>
        /// Defines the secondary panel label font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel label font.")]
        public CustomFont SecondaryPanelFontLabel
        {
            get
            {
                return secondaryPanelFontLabel;
            }
            set
            {
                secondaryPanelFontLabel = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelFontLinkLabel property.
        /// </summary>
        private CustomFont secondaryPanelFontLinkLabel = new CustomFont("Junction", 8, Color.White) { IsUnderline = true };
        /// <summary>
        /// Defines the secondary panel link label font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel link label font.")]
        public CustomFont SecondaryPanelFontLinkLabel
        {
            get
            {
                return secondaryPanelFontLinkLabel;
            }
            set
            {
                secondaryPanelFontLinkLabel = value;
            }
        }

        #endregion

        #region Toolbar

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

        /// <summary>
        /// Private value for the ToolbarButtonTheme property.
        /// </summary>
        private ButtonTheme toolbarButtonTheme = new ButtonTheme();
        /// <summary>
        /// Defines the toolbar button theme.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar button theme.")]
        public ButtonTheme ToolbarButtonTheme
        {
            get
            {
                return toolbarButtonTheme;
            }
            set
            {
                toolbarButtonTheme = value;
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
