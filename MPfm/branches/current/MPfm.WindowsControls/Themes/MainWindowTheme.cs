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
        private Color m_panelHeaderBackgroundColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel header background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel - Header"), Browsable(true), Description("Main panel header background gradient (first color).")]
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
        /// Defines the main panel header background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel - Header"), Browsable(true), Description("Main panel header background gradient (second color).")]
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
        /// Private value for the PanelHeaderTextColor property.
        /// </summary>
        private Color m_panelHeaderTextColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel header text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel - Header"), Browsable(true), Description("Main panel header text fore color.")]
        public Color PanelHeaderTextColor
        {
            get
            {
                return m_panelHeaderTextColor;
            }
            set
            {
                m_panelHeaderTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelHeaderTextFont property.
        /// </summary>
        private CustomFont m_panelHeaderTextFont = new CustomFont();
        /// <summary>
        /// Defines the main panel header text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel - Header"), Browsable(true), Description("Main panel header text font.")]
        public CustomFont PanelHeaderTextFont
        {
            get
            {
                return m_panelHeaderTextFont;
            }
            set
            {
                m_panelHeaderTextFont = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the PanelBackgroundColor1 property.
        /// </summary>
        private Color m_panelBackgroundColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel background gradient (first color).")]
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
        /// Defines the main panel background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel background gradient (second color).")]
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
        /// Private value for the PanelTitleColor property.
        /// </summary>
        private Color m_panelTitleColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel title fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel title fore color.")]
        public Color PanelTitleColor
        {
            get
            {
                return m_panelTitleColor;
            }
            set
            {
                m_panelTitleColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTitleFont property.
        /// </summary>
        private CustomFont m_panelTitleFont = new CustomFont();
        /// <summary>
        /// Defines the main panel title font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel title font.")]
        public CustomFont PanelTitleFont
        {
            get
            {
                return m_panelTitleFont;
            }
            set
            {
                m_panelTitleFont = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSubtitleColor property.
        /// </summary>
        private Color m_panelSubtitleColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel subtitle fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel subtitle fore color.")]
        public Color PanelSubtitleColor
        {
            get
            {
                return m_panelSubtitleColor;
            }
            set
            {
                m_panelSubtitleColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSubtitleFont property.
        /// </summary>
        private CustomFont m_panelSubtitleFont = new CustomFont();
        /// <summary>
        /// Defines the main panel subtitle font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel subtitle font.")]
        public CustomFont PanelSubtitleFont
        {
            get
            {
                return m_panelSubtitleFont;
            }
            set
            {
                m_panelSubtitleFont = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSubtitle2Color property.
        /// </summary>
        private Color m_panelSubtitle2Color = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel subtitle 2 fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel subtitle 2 fore color.")]
        public Color PanelSubtitle2Color
        {
            get
            {
                return m_panelSubtitle2Color;
            }
            set
            {
                m_panelSubtitle2Color = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSubtitle2Font property.
        /// </summary>
        private CustomFont m_panelSubtitle2Font = new CustomFont();
        /// <summary>
        /// Defines the main panel subtitle 2 font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel subtitle 2 font.")]
        public CustomFont PanelSubtitle2Font
        {
            get
            {
                return m_panelSubtitle2Font;
            }
            set
            {
                m_panelSubtitle2Font = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTextColor property.
        /// </summary>
        private Color m_panelTextColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel text fore color.")]
        public Color PanelTextColor
        {
            get
            {
                return m_panelTextColor;
            }
            set
            {
                m_panelTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTextFont property.
        /// </summary>
        private CustomFont m_panelTextFont = new CustomFont();
        /// <summary>
        /// Defines the main panel text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel text font.")]
        public CustomFont PanelTextFont
        {
            get
            {
                return m_panelTextFont;
            }
            set
            {
                m_panelTextFont = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTimeDisplayColor property.
        /// </summary>
        private Color m_panelTimeDisplayColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel time display fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel time display fore color.")]
        public Color PanelTimeDisplayColor
        {
            get
            {
                return m_panelTimeDisplayColor;
            }
            set
            {
                m_panelTimeDisplayColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelTimeDisplayFont property.
        /// </summary>
        private CustomFont m_panelTimeDisplayFont = new CustomFont();
        /// <summary>
        /// Defines the main panel time display font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel time display font.")]
        public CustomFont PanelTimeDisplayFont
        {
            get
            {
                return m_panelTimeDisplayFont;
            }
            set
            {
                m_panelTimeDisplayFont = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSmallTimeDisplayColor property.
        /// </summary>
        private Color m_panelSmallTimeDisplayColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the main panel small time display fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel small time display fore color.")]
        public Color PanelSmallTimeDisplayColor
        {
            get
            {
                return m_panelSmallTimeDisplayColor;
            }
            set
            {
                m_panelSmallTimeDisplayColor = value;
            }
        }

        /// <summary>
        /// Private value for the PanelSmallTimeDisplayFont property.
        /// </summary>
        private CustomFont m_panelSmallTimeDisplayFont = new CustomFont();
        /// <summary>
        /// Defines the main panel small time display font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Main Panel"), Browsable(true), Description("Main panel small time display font.")]
        public CustomFont PanelSmallTimeDisplayFont
        {
            get
            {
                return m_panelSmallTimeDisplayFont;
            }
            set
            {
                m_panelSmallTimeDisplayFont = value;
            }
        }

        #endregion

        #region Secondary Panel

        #region Secondary Panel Header

        /// <summary>
        /// Private value for the SecondaryPanelHeaderBackgroundColor1 property.
        /// </summary>
        private Color m_secondaryPanelHeaderBackgroundColor1 = Color.FromArgb(50, 50, 50);
        /// <summary>
        /// Defines the secondary panel header background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel - Header"), Browsable(true), Description("Secondary panel header background gradient (first color).")]
        public Color SecondaryPanelHeaderBackgroundColor1
        {
            get
            {
                return m_secondaryPanelHeaderBackgroundColor1;
            }
            set
            {
                m_secondaryPanelHeaderBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelHeaderBackgroundColor2 property.
        /// </summary>
        private Color m_secondaryPanelHeaderBackgroundColor2 = Color.FromArgb(100, 100, 100);
        /// <summary>
        /// Defines the secondary panel header background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel - Header"), Browsable(true), Description("Secondary panel header background gradient (second color).")]
        public Color SecondaryPanelHeaderBackgroundColor2
        {
            get
            {
                return m_secondaryPanelHeaderBackgroundColor2;
            }
            set
            {
                m_secondaryPanelHeaderBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelHeaderTextColor property.
        /// </summary>
        private Color m_secondaryPanelHeaderTextColor = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// Defines the secondary panel header text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel - Header"), Browsable(true), Description("Secondary panel header text fore color.")]
        public Color SecondaryPanelHeaderTextColor
        {
            get
            {
                return m_secondaryPanelHeaderTextColor;
            }
            set
            {
                m_secondaryPanelHeaderTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelHeaderTextFont property.
        /// </summary>
        private CustomFont m_secondaryPanelHeaderTextFont = new CustomFont();
        /// <summary>
        /// Defines the secondary panel header text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel - Header"), Browsable(true), Description("Secondary panel header text font.")]
        public CustomFont SecondaryPanelHeaderTextFont
        {
            get
            {
                return m_secondaryPanelHeaderTextFont;
            }
            set
            {
                m_secondaryPanelHeaderTextFont = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the SecondaryPanelBackgroundColor1 property.
        /// </summary>
        private Color m_secondaryPanelBackgroundColor1 = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// Defines the secondary panel background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel background gradient (first color).")]
        public Color SecondaryPanelBackgroundColor1
        {
            get
            {
                return m_secondaryPanelBackgroundColor1;
            }
            set
            {
                m_secondaryPanelBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelBackgroundColor2 property.
        /// </summary>
        private Color m_secondaryPanelBackgroundColor2 = Color.FromArgb(47, 47, 47);
        /// <summary>
        /// Defines the secondary panel background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel background gradient (second color).")]
        public Color SecondaryPanelBackgroundColor2
        {
            get
            {
                return m_secondaryPanelBackgroundColor2;
            }
            set
            {
                m_secondaryPanelBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelLabelColor property.
        /// </summary>
        private Color m_secondaryPanelLabelColor = Color.Silver;
        /// <summary>
        /// Defines the secondary panel label fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel label fore color.")]
        public Color SecondaryPanelLabelColor
        {
            get
            {
                return m_secondaryPanelLabelColor;
            }
            set
            {
                m_secondaryPanelLabelColor = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelLabelFont property.
        /// </summary>
        private CustomFont m_secondaryPanelLabelFont = new CustomFont();
        /// <summary>
        /// Defines the secondary panel label font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel label font.")]
        public CustomFont SecondaryPanelLabelFont
        {
            get
            {
                return m_secondaryPanelLabelFont;
            }
            set
            {
                m_secondaryPanelLabelFont = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelTextColor property.
        /// </summary>
        private Color m_secondaryPanelTextColor = Color.White;
        /// <summary>
        /// Defines the secondary panel text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel text fore color.")]
        public Color SecondaryPanelTextColor
        {
            get
            {
                return m_secondaryPanelTextColor;
            }
            set
            {
                m_secondaryPanelTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryPanelTextFont property.
        /// </summary>
        private CustomFont m_secondaryPanelTextFont = new CustomFont();
        /// <summary>
        /// Defines the secondary panel text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Secondary Panel"), Browsable(true), Description("Secondary panel text font.")]
        public CustomFont SecondaryPanelTextFont
        {
            get
            {
                return m_secondaryPanelTextFont;
            }
            set
            {
                m_secondaryPanelTextFont = value;
            }
        }

        #endregion

        #region Toolbar

        #region Toolbar Buttons

        /// <summary>
        /// Private value for the ToolbarButtonBackgroundColor1 property.
        /// </summary>
        private Color m_toolbarButtonBackgroundColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (first color).")]
        public Color ToolbarButtonBackgroundColor1
        {
            get
            {
                return m_toolbarButtonBackgroundColor1;
            }
            set
            {
                m_toolbarButtonBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonBackgroundColor2 property.
        /// </summary>
        private Color m_toolbarButtonBackgroundColor2 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (second color).")]
        public Color ToolbarButtonBackgroundColor2
        {
            get
            {
                return m_toolbarButtonBackgroundColor2;
            }
            set
            {
                m_toolbarButtonBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonBorderColor property.
        /// </summary>
        private Color m_toolbarButtonBorderColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button border color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button border color.")]
        public Color ToolbarButtonBorderColor
        {
            get
            {
                return m_toolbarButtonBorderColor;
            }
            set
            {
                m_toolbarButtonBorderColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonOverBackgroundColor1 property.
        /// </summary>
        private Color m_toolbarButtonMouseOverBackgroundColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button background gradient (first color), when the mouse is over.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (first color), when the mouse is over.")]
        public Color ToolbarButtonMouseOverBackgroundColor1
        {
            get
            {
                return m_toolbarButtonMouseOverBackgroundColor1;
            }
            set
            {
                m_toolbarButtonMouseOverBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonOverBackgroundColor2 property.
        /// </summary>
        private Color m_toolbarButtonMouseOverBackgroundColor2 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button background gradient (second color), when the mouse is over.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (second color), when the mouse is over.")]
        public Color ToolbarButtonMouseOverBackgroundColor2
        {
            get
            {
                return m_toolbarButtonMouseOverBackgroundColor2;
            }
            set
            {
                m_toolbarButtonMouseOverBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonOverBackgroundColor2 property.
        /// </summary>
        private Color m_toolbarButtonMouseOverBorderColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button border color, when the mouse is over.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button border color, when the mouse is over.")]
        public Color ToolbarButtonMouseOverBorderColor
        {
            get
            {
                return m_toolbarButtonMouseOverBorderColor;
            }
            set
            {
                m_toolbarButtonMouseOverBorderColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonDisabledBackgroundColor1 property.
        /// </summary>
        private Color m_toolbarButtonDisabledBackgroundColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button background gradient (first color), when the control is disabled.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (first color), when the control is disabled.")]
        public Color ToolbarButtonDisabledBackgroundColor1
        {
            get
            {
                return m_toolbarButtonDisabledBackgroundColor1;
            }
            set
            {
                m_toolbarButtonDisabledBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonDisabledBackgroundColor2 property.
        /// </summary>
        private Color m_toolbarButtonDisabledBackgroundColor2 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button background gradient (second color), when the control is disabled.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button background gradient (second color), when the control is disabled.")]
        public Color ToolbarButtonDisabledBackgroundColor2
        {
            get
            {
                return m_toolbarButtonDisabledBackgroundColor2;
            }
            set
            {
                m_toolbarButtonDisabledBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonDisabledBorderColor property.
        /// </summary>
        private Color m_toolbarButtonDisabledBorderColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button border color, when the control is disabled.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button border color, when the control is disabled.")]
        public Color ToolbarButtonDisabledBorderColor
        {
            get
            {
                return m_toolbarButtonDisabledBorderColor;
            }
            set
            {
                m_toolbarButtonDisabledBorderColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonTextColor property.
        /// </summary>
        private Color m_toolbarButtonTextColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar button text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button text fore color.")]
        public Color ToolbarButtonTextColor
        {
            get
            {
                return m_toolbarButtonTextColor;
            }
            set
            {
                m_toolbarButtonTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarButtonTextFont property.
        /// </summary>
        private CustomFont m_toolbarButtonTextFont = new CustomFont();
        /// <summary>
        /// Defines the toolbar button text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar - Buttons"), Browsable(true), Description("Toolbar button text font.")]
        public CustomFont ToolbarButtonTextFont
        {
            get
            {
                return m_toolbarButtonTextFont;
            }
            set
            {
                m_toolbarButtonTextFont = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the ToolbarBackgroundColor1 property.
        /// </summary>
        private Color m_toolbarBackgroundColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar background gradient (first color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar background gradient (first color).")]
        public Color ToolbarBackgroundColor1
        {
            get
            {
                return m_toolbarBackgroundColor1;
            }
            set
            {
                m_toolbarBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarBackgroundColor2 property.
        /// </summary>
        private Color m_toolbarBackgroundColor2 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar background gradient (second color).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar background gradient (second color).")]
        public Color ToolbarBackgroundColor2
        {
            get
            {
                return m_toolbarBackgroundColor2;
            }
            set
            {
                m_toolbarBackgroundColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarTextColor property.
        /// </summary>
        private Color m_toolbarTextColor = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// Defines the toolbar text fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar text fore color.")]
        public Color ToolbarTextColor
        {
            get
            {
                return m_toolbarTextColor;
            }
            set
            {
                m_toolbarTextColor = value;
            }
        }

        /// <summary>
        /// Private value for the ToolbarTextFont property.
        /// </summary>
        private CustomFont m_toolbarTextFont = new CustomFont();
        /// <summary>
        /// Defines the toolbar text font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Toolbar"), Browsable(true), Description("Toolbar text font.")]
        public CustomFont ToolbarTextFont
        {
            get
            {
                return m_toolbarTextFont;
            }
            set
            {
                m_toolbarTextFont = value;
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
