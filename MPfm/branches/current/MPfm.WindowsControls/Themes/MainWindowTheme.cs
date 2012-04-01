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

        #region Panel

        #region Panel Header
        
        /// <summary>
        /// Private value for the PanelHeaderBackgroundColor1 property.
        /// </summary>
        private Color panelHeaderBackgroundColor1 = Color.FromArgb(100, 100, 100);
        /// <summary>
        /// Defines the main panel header background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel - Header"), Browsable(true), Description("Main panel header background gradient (first color).")]
        public Color PanelHeaderBackgroundColor1
        {
            get
            {
                return panelHeaderBackgroundColor1;
            }
            set
            {
                panelHeaderBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelHeaderBackgroundColor2 property.
        /// </summary>
        private Color panelHeaderBackgroundColor2 = Color.FromArgb(60, 60, 60);
        /// <summary>
        /// Defines the main panel header background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel - Header"), Browsable(true), Description("Main panel header background gradient (second color).")]
        public Color PanelHeaderBackgroundColor2
        {
            get
            {
                return panelHeaderBackgroundColor2;
            }
            set
            {
                panelHeaderBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelHeaderTextColor property.
        /// </summary>
        private Color panelHeaderTextColor = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// Defines the main panel header text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel - Header"), Browsable(true), Description("Main panel header text fore color.")]
        public Color PanelHeaderTextColor
        {
            get
            {
                return panelHeaderTextColor;
            }
            set
            {
                panelHeaderTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelHeaderTextFont property.
        /// </summary>
        private CustomFont panelHeaderTextFont = new CustomFont("TitilliumText22L Lt", 10);
        /// <summary>
        /// Defines the main panel header text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel - Header"), Browsable(true), Description("Main panel header text font.")]
        public CustomFont PanelHeaderTextFont
        {
            get
            {
                return panelHeaderTextFont;
            }
            set
            {
                panelHeaderTextFont = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the PanelBackgroundColor1 property.
        /// </summary>
        private Color panelBackgroundColor1 = Color.FromArgb(20, 20, 20);
        /// <summary>
        /// Defines the main panel background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel background gradient (first color).")]
        public Color PanelBackgroundColor1
        {
            get
            {
                return panelBackgroundColor1;
            }
            set
            {
                panelBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelBackgroundColor2 property.
        /// </summary>
        private Color panelBackgroundColor2 = Color.FromArgb(40, 40, 40);
        /// <summary>
        /// Defines the main panel background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel background gradient (second color).")]
        public Color PanelBackgroundColor2
        {
            get
            {
                return panelBackgroundColor2;
            }
            set
            {
                panelBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTitleColor property.
        /// </summary>
        private Color panelTitleColor = Color.White;
        /// <summary>
        /// Defines the main panel title fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel title fore color.")]
        public Color PanelTitleColor
        {
            get
            {
                return panelTitleColor;
            }
            set
            {
                panelTitleColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTitleFont property.
        /// </summary>
        private CustomFont panelTitleFont = new CustomFont("TitilliumText22L Lt", 18);
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
        /// Private value for the PanelSubtitleColor property.
        /// </summary>
        private Color panelSubtitleColor = Color.Silver;
        /// <summary>
        /// Defines the main panel subtitle fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel subtitle fore color.")]
        public Color PanelSubtitleColor
        {
            get
            {
                return panelSubtitleColor;
            }
            set
            {
                panelSubtitleColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSubtitleFont property.
        /// </summary>
        private CustomFont panelSubtitleFont = new CustomFont("TitilliumText22L Lt", 14);
        /// <summary>
        /// Defines the main panel subtitle font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel subtitle font.")]
        public CustomFont PanelSubtitleFont
        {
            get
            {
                return panelSubtitleFont;
            }
            set
            {
                panelSubtitleFont = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSubtitle2Color property.
        /// </summary>
        private Color panelSubtitle2Color = Color.Gray;
        /// <summary>
        /// Defines the main panel subtitle 2 fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel subtitle 2 fore color.")]
        public Color PanelSubtitle2Color
        {
            get
            {
                return panelSubtitle2Color;
            }
            set
            {
                panelSubtitle2Color = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSubtitle2Font property.
        /// </summary>
        private CustomFont panelSubtitle2Font = new CustomFont("TitilliumText22L Lt", 12);
        /// <summary>
        /// Defines the main panel subtitle 2 font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel subtitle 2 font.")]
        public CustomFont PanelSubtitle2Font
        {
            get
            {
                return panelSubtitle2Font;
            }
            set
            {
                panelSubtitle2Font = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTextColor property.
        /// </summary>
        private Color panelTextColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel text fore color.")]
        public Color PanelTextColor
        {
            get
            {
                return panelTextColor;
            }
            set
            {
                panelTextColor = value;
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
        /// Private value for the PanelTimeDisplayColor property.
        /// </summary>
        private Color panelTimeDisplayColor = Color.White;
        /// <summary>
        /// Defines the main panel time display fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel time display fore color.")]
        public Color PanelTimeDisplayColor
        {
            get
            {
                return panelTimeDisplayColor;
            }
            set
            {
                panelTimeDisplayColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTimeDisplayFont property.
        /// </summary>
        private CustomFont panelTimeDisplayFont = new CustomFont("Droid Sans Mono", 10);
        /// <summary>
        /// Defines the main panel time display font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel time display font.")]
        public CustomFont PanelTimeDisplayFont
        {
            get
            {
                return panelTimeDisplayFont;
            }
            set
            {
                panelTimeDisplayFont = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSmallTimeDisplayColor property.
        /// </summary>
        private Color panelSmallTimeDisplayColor = Color.White;
        /// <summary>
        /// Defines the main panel small time display fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel small time display fore color.")]
        public Color PanelSmallTimeDisplayColor
        {
            get
            {
                return panelSmallTimeDisplayColor;
            }
            set
            {
                panelSmallTimeDisplayColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSmallTimeDisplayFont property.
        /// </summary>
        private CustomFont panelSmallTimeDisplayFont = new CustomFont("Droid Sans Mono", 7);
        /// <summary>
        /// Defines the main panel small time display font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel small time display font.")]
        public CustomFont PanelSmallTimeDisplayFont
        {
            get
            {
                return panelSmallTimeDisplayFont;
            }
            set
            {
                panelSmallTimeDisplayFont = value;
            }
        }

        #endregion

        #region Secondary Panel

        #region Secondary Panel Header

        /// <summary>
        /// Private value for the SecondaryPanelHeaderBackgroundColor1 property.
        /// </summary>
        private Color secondaryPanelHeaderBackgroundColor1 = Color.FromArgb(50, 50, 50);
        /// <summary>
        /// Defines the secondary panel header background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel - Header"), Browsable(true), Description("Secondary panel header background gradient (first color).")]
        public Color SecondaryPanelHeaderBackgroundColor1
        {
            get
            {
                return secondaryPanelHeaderBackgroundColor1;
            }
            set
            {
                secondaryPanelHeaderBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelHeaderBackgroundColor2 property.
        /// </summary>
        private Color secondaryPanelHeaderBackgroundColor2 = Color.FromArgb(100, 100, 100);
        /// <summary>
        /// Defines the secondary panel header background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel - Header"), Browsable(true), Description("Secondary panel header background gradient (second color).")]
        public Color SecondaryPanelHeaderBackgroundColor2
        {
            get
            {
                return secondaryPanelHeaderBackgroundColor2;
            }
            set
            {
                secondaryPanelHeaderBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelHeaderTextColor property.
        /// </summary>
        private Color secondaryPanelHeaderTextColor = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// Defines the secondary panel header text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel - Header"), Browsable(true), Description("Secondary panel header text fore color.")]
        public Color SecondaryPanelHeaderTextColor
        {
            get
            {
                return secondaryPanelHeaderTextColor;
            }
            set
            {
                secondaryPanelHeaderTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelHeaderTextFont property.
        /// </summary>
        private CustomFont secondaryPanelHeaderTextFont = new CustomFont("Junction", 8.25f);
        /// <summary>
        /// Defines the secondary panel header text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel - Header"), Browsable(true), Description("Secondary panel header text font.")]
        public CustomFont SecondaryPanelHeaderTextFont
        {
            get
            {
                return secondaryPanelHeaderTextFont;
            }
            set
            {
                secondaryPanelHeaderTextFont = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the SecondaryPanelBackgroundColor1 property.
        /// </summary>
        private Color secondaryPanelBackgroundColor1 = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// Defines the secondary panel background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel background gradient (first color).")]
        public Color SecondaryPanelBackgroundColor1
        {
            get
            {
                return secondaryPanelBackgroundColor1;
            }
            set
            {
                secondaryPanelBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelBackgroundColor2 property.
        /// </summary>
        private Color secondaryPanelBackgroundColor2 = Color.FromArgb(47, 47, 47);
        /// <summary>
        /// Defines the secondary panel background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel background gradient (second color).")]
        public Color SecondaryPanelBackgroundColor2
        {
            get
            {
                return secondaryPanelBackgroundColor2;
            }
            set
            {
                secondaryPanelBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelLabelColor property.
        /// </summary>
        private Color secondaryPanelLabelColor = Color.Silver;
        /// <summary>
        /// Defines the secondary panel label fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel label fore color.")]
        public Color SecondaryPanelLabelColor
        {
            get
            {
                return secondaryPanelLabelColor;
            }
            set
            {
                secondaryPanelLabelColor = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelLabelFont property.
        /// </summary>
        private CustomFont secondaryPanelLabelFont = new CustomFont("Junction", 7);
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

        /// <summary>
        /// Private value for the SecondaryPanelTextColor property.
        /// </summary>
        private Color secondaryPanelTextColor = Color.White;
        /// <summary>
        /// Defines the secondary panel text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel text fore color.")]
        public Color SecondaryPanelTextColor
        {
            get
            {
                return secondaryPanelTextColor;
            }
            set
            {
                secondaryPanelTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelTextFont property.
        /// </summary>
        private CustomFont secondaryPanelTextFont = new CustomFont("Junction", 7);
        /// <summary>
        /// Defines the secondary panel text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel text font.")]
        public CustomFont SecondaryPanelTextFont
        {
            get
            {
                return secondaryPanelTextFont;
            }
            set
            {
                secondaryPanelTextFont = value;
            }
        }

        #endregion

        #region Toolbar

        #region Toolbar Buttons

        /// <summary>
        /// Private value for the ToolbarButtonBackgroundColor1 property.
        /// </summary>
        private Color toolbarButtonBackgroundColor1 = Color.LightGray;
        /// <summary>
        /// Defines the toolbar button background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (first color).")]
        public Color ToolbarButtonBackgroundColor1
        {
            get
            {
                return toolbarButtonBackgroundColor1;
            }
            set
            {
                toolbarButtonBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonBackgroundColor2 property.
        /// </summary>
        private Color toolbarButtonBackgroundColor2 = Color.Gray;
        /// <summary>
        /// Defines the toolbar button background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (second color).")]
        public Color ToolbarButtonBackgroundColor2
        {
            get
            {
                return toolbarButtonBackgroundColor2;
            }
            set
            {
                toolbarButtonBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonBorderColor property.
        /// </summary>
        private Color toolbarButtonBorderColor = Color.DimGray;
        /// <summary>
        /// Defines the toolbar button border color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button border color.")]
        public Color ToolbarButtonBorderColor
        {
            get
            {
                return toolbarButtonBorderColor;
            }
            set
            {
                toolbarButtonBorderColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonTextColor property.
        /// </summary>
        private Color toolbarButtonTextColor = Color.Black;
        /// <summary>
        /// Defines the toolbar button text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button text fore color.")]
        public Color ToolbarButtonTextColor
        {
            get
            {
                return toolbarButtonTextColor;
            }
            set
            {
                toolbarButtonTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonOverBackgroundColor1 property.
        /// </summary>
        private Color toolbarButtonMouseOverBackgroundColor1 = Color.White;
        /// <summary>
        /// Defines the toolbar button background gradient (first color), when the mouse is over.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (first color), when the mouse is over.")]
        public Color ToolbarButtonMouseOverBackgroundColor1
        {
            get
            {
                return toolbarButtonMouseOverBackgroundColor1;
            }
            set
            {
                toolbarButtonMouseOverBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonOverBackgroundColor2 property.
        /// </summary>
        private Color toolbarButtonMouseOverBackgroundColor2 = Color.DarkGray;
        /// <summary>
        /// Defines the toolbar button background gradient (second color), when the mouse is over.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (second color), when the mouse is over.")]
        public Color ToolbarButtonMouseOverBackgroundColor2
        {
            get
            {
                return toolbarButtonMouseOverBackgroundColor2;
            }
            set
            {
                toolbarButtonMouseOverBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonOverBackgroundColor2 property.
        /// </summary>
        private Color toolbarButtonMouseOverBorderColor = Color.DimGray;
        /// <summary>
        /// Defines the toolbar button border color, when the mouse is over.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button border color, when the mouse is over.")]
        public Color ToolbarButtonMouseOverBorderColor
        {
            get
            {
                return toolbarButtonMouseOverBorderColor;
            }
            set
            {
                toolbarButtonMouseOverBorderColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonMouseOverTextColor property.
        /// </summary>
        private Color toolbarButtonMouseOverTextColor = Color.Black;
        /// <summary>
        /// Defines the toolbar button text fore color, when the mouse is over.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button text fore color, when the mouse is over.")]
        public Color ToolbarButtonMouseOverTextColor
        {
            get
            {
                return toolbarButtonMouseOverTextColor;
            }
            set
            {
                toolbarButtonMouseOverTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonDisabledBackgroundColor1 property.
        /// </summary>
        private Color toolbarButtonDisabledBackgroundColor1 = Color.Gray;
        /// <summary>
        /// Defines the toolbar button background gradient (first color), when the control is disabled.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (first color), when the control is disabled.")]
        public Color ToolbarButtonDisabledBackgroundColor1
        {
            get
            {
                return toolbarButtonDisabledBackgroundColor1;
            }
            set
            {
                toolbarButtonDisabledBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonDisabledBackgroundColor2 property.
        /// </summary>
        private Color toolbarButtonDisabledBackgroundColor2 = Color.FromArgb(35, 35, 35);
        /// <summary>
        /// Defines the toolbar button background gradient (second color), when the control is disabled.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (second color), when the control is disabled.")]
        public Color ToolbarButtonDisabledBackgroundColor2
        {
            get
            {
                return toolbarButtonDisabledBackgroundColor2;
            }
            set
            {
                toolbarButtonDisabledBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonDisabledBorderColor property.
        /// </summary>
        private Color toolbarButtonDisabledBorderColor = Color.Gray;
        /// <summary>
        /// Defines the toolbar button border color, when the control is disabled.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button border color, when the control is disabled.")]
        public Color ToolbarButtonDisabledBorderColor
        {
            get
            {
                return toolbarButtonDisabledBorderColor;
            }
            set
            {
                toolbarButtonDisabledBorderColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarDisabledButtonTextColor property.
        /// </summary>
        private Color toolbarButtonDisabledTextColor = Color.Silver;
        /// <summary>
        /// Defines the toolbar button text fore color, when the control is disabled.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button text fore color, when the control is disabled.")]
        public Color ToolbarButtonDisabledTextColor
        {
            get
            {
                return toolbarButtonDisabledTextColor;
            }
            set
            {
                toolbarButtonDisabledTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonTextFont property.
        /// </summary>
        private CustomFont toolbarButtonTextFont = new CustomFont();
        /// <summary>
        /// Defines the toolbar button text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button text font.")]
        public CustomFont ToolbarButtonTextFont
        {
            get
            {
                return toolbarButtonTextFont;
            }
            set
            {
                toolbarButtonTextFont = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the ToolbarBackgroundColor1 property.
        /// </summary>
        private Color toolbarBackgroundColor1 = Color.Silver;
        /// <summary>
        /// Defines the toolbar background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar background gradient (first color).")]
        public Color ToolbarBackgroundColor1
        {
            get
            {
                return toolbarBackgroundColor1;
            }
            set
            {
                toolbarBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarBackgroundColor2 property.
        /// </summary>
        private Color toolbarBackgroundColor2 = Color.Gainsboro;
        /// <summary>
        /// Defines the toolbar background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar background gradient (second color).")]
        public Color ToolbarBackgroundColor2
        {
            get
            {
                return toolbarBackgroundColor2;
            }
            set
            {
                toolbarBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarTextColor property.
        /// </summary>
        private Color toolbarTextColor = Color.Black;
        /// <summary>
        /// Defines the toolbar text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar text fore color.")]
        public Color ToolbarTextColor
        {
            get
            {
                return toolbarTextColor;
            }
            set
            {
                toolbarTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarTextFont property.
        /// </summary>
        private CustomFont toolbarTextFont = new CustomFont();
        /// <summary>
        /// Defines the toolbar text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar text font.")]
        public CustomFont ToolbarTextFont
        {
            get
            {
                return toolbarTextFont;
            }
            set
            {
                toolbarTextFont = value;
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
