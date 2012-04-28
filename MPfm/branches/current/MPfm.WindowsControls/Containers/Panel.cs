//
// Panel.cs: This panel control is based on the System.Windows.Forms.Panel control.
//           It adds custom drawing, gradient backgrounds and other features.
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
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel.Design;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This panel control is based on the System.Windows.Forms.Panel control.
    /// It adds custom drawing, gradient backgrounds and other features.
    /// </summary>
    public class Panel : System.Windows.Forms.Panel
    {
        /// <summary>
        /// Embedded font collection used for drawing.
        /// </summary>
        protected EmbeddedFontCollection embeddedFonts = null;

        /// <summary>
        /// Private value for the Theme property.
        /// </summary>
        private PanelTheme theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Theme object for this control.")]
        public PanelTheme Theme
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
        /// Private value for the TextAlign property.
        /// </summary>
        private ContentAlignment textAlign = ContentAlignment.MiddleLeft;
        /// <summary>
        /// Defines the text alignment used in the header text gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the text alignement used in the header text gradient.")]
        public ContentAlignment TextAlign
        {
            get
            {
                return textAlign;
            }
            set
            {
                textAlign = value;
            }
        }

        #region Header Properties

        /// <summary>
        /// Private value for the HeaderTitle property.
        /// </summary>
        private string headerTitle = "";
        /// <summary>
        /// Title displayed in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Title displayed in the header.")]
        public string HeaderTitle
        {
            get
            {
                return headerTitle;
            }
            set
            {
                headerTitle = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderAutoSize property.
        /// </summary>
        private bool headerAutoSize = true;
        /// <summary>
        /// Defines if the header height is auto sized depending on the font size.
        /// </summary>
        public bool HeaderAutoSize
        {
            get
            {
                return headerAutoSize;
            }
            set
            {
                headerAutoSize = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderHeight property.
        /// </summary>
        private int headerHeight = 0;
        /// <summary>
        /// Height of the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Height of the header.")]
        public int HeaderHeight
        {
            get
            {
                return headerHeight;
            }
            set
            {
                // Make sure the header height is not 0
                if (headerHeight > 0)
                {
                    // Calculate diff and change position of child controls
                    int diff = value - headerHeight;
                    foreach (System.Windows.Forms.Control ctrl in Controls)
                    {
                        ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y + diff);
                    }
                }

                // Set value and refresh control
                headerHeight = value;
                Refresh();
            }
        }

        /// <summary>
        /// Private value for the HeaderTextAlign property.
        /// </summary>
        private ContentAlignment headerTextAlign = ContentAlignment.MiddleLeft;
        /// <summary>
        /// Alignment of the text in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Alignment of the text in the header.")]
        public ContentAlignment HeaderTextAlign
        {
            get
            {
                return headerTextAlign;
            }
            set
            {
                headerTextAlign = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderExpandable property.
        /// </summary>
        private bool headerExpandable = true;
        /// <summary>
        /// Defines if the header is expandable or not.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(true)]
        [Category("Header"), Browsable(true), Description("Defines if the header is expandable or not.")]
        public bool HeaderExpandable
        {
            get
            {
                return headerExpandable;
            }
            set
            {
                headerExpandable = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderExpanded property.
        /// </summary>
        private bool headerExpanded = false;
        /// <summary>
        /// Defines if the header is expanded.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(false)]
        [Category("Header"), Browsable(true), Description("Defines if the header is expanded.")]
        public bool HeaderExpanded
        {
            get
            {
                return headerExpanded;
            }
            set
            {
                headerExpanded = value;
            }
        }

        /// <summary>
        /// Private value for the ExpandedHeight property.
        /// </summary>
        private int expandedHeight = 200;
        /// <summary>
        /// Defines the header height when expanded.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(false)]
        [Category("Header"), Browsable(true), Description("Defines the header height when expanded.")]
        public int ExpandedHeight
        {
            get
            {
                return expandedHeight;
            }
            set
            {
                expandedHeight = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for Panel.
        /// </summary>
        public Panel()
        {
            // Set control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            // Create default theme
            theme = new PanelTheme();
        }

        
        /// <summary>
        /// Triggered when the control is created.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            LoadEmbeddedFonts();
        }

        /// <summary>
        /// Loads the embedded fonts for rendering.
        /// </summary>
        protected void LoadEmbeddedFonts()
        {
            // Check if design time or run time            
            if (Tools.IsDesignTime())
            {
                // This only exists when running in design time and cannot be run in the constructor                
                ITypeResolutionService typeResService = GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
                string path = string.Empty;
                if (typeResService != null)
                {
                    // Get path
                    path = typeResService.GetPathOfAssembly(Assembly.GetExecutingAssembly().GetName());
                }

                // Example path: D:\Code\MPfm\Branches\Current\MPfm.WindowsControls\obj\Debug\MPfm.WindowsControls.dll
                // We want to get the path for MPfm.Fonts.dll.
                string fontsPath = path.Replace("MPfm.WindowsControls", "MPfm.Fonts").Replace("MPfm.Fonts.dll", "");

                // Get embedded font collection
                embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts(fontsPath);
            }
            else
            {
                // Get embedded font collection
                embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();
            }
        }

        #region Expand Methods

        /// <summary>
        /// Expands or hides the content panel.
        /// </summary>
        public void Expand()
        {
            if (headerExpandable)
            {
                headerExpanded = !headerExpanded;

                if (headerExpanded)
                {
                    Height = expandedHeight;
                }
                else
                {
                    Height = headerHeight;
                }
            }
        }

        /// <summary>
        /// Expands or hides the content panel, based on the value passed in parameter.
        /// </summary>
        /// <param name="value">Expand or hide the panel</param>
        public void Expand(bool value)
        {
            if (headerExpandable)
            {
                headerExpanded = value;

                if (headerExpanded)
                {
                    Height = expandedHeight;
                }
                else
                {
                    Height = headerHeight;
                }
            }
        }

        #endregion

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Get graphics from event
            Graphics g = pe.Graphics;

            // Use anti-aliasing?
            if (theme.HeaderTextGradient.Font.UseAntiAliasing)
            {
                // Set anti-aliasing
                PaintHelper.SetAntiAliasing(g);
            }

            // Get font
            Font font = PaintHelper.LoadFont(embeddedFonts, theme.HeaderTextGradient.Font);

            // If the embedded font could not be loaded, get the default font
            if (font == null)
            {
                // Use default Font instead
                font = this.Font;
            }

            // Draw background gradient (cover -1 pixel to fix graphic bug) 
            Rectangle rectBackground = new Rectangle(-1, -1, ClientRectangle.Width + 2, ClientRectangle.Height + 2);
            if (headerExpanded)
            {
                rectBackground.Height -= headerHeight - 1;
                rectBackground.Y = headerHeight - 1;
            }            
            Rectangle rectBorder = new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            PaintHelper.RenderBackgroundGradient(g, rectBackground, rectBorder, theme.BackgroundGradient);            

            // Draw header gradient
            if (headerHeight > 0)
            {
                Rectangle rectHeader = new Rectangle(-1, -1, ClientRectangle.Width + 1, headerHeight + 1);
                PaintHelper.RenderBackgroundGradient(g, rectHeader, rectBorder, theme.HeaderTextGradient);                                
                PaintHelper.RenderTextWithAlignment(g, new RectangleF(0, 0, ClientRectangle.Width, headerHeight), font, HeaderTitle, TextAlign, theme.HeaderTextGradient.Font.Color, theme.HeaderTextGradient.Padding);
            }

            // Dispose font if necessary
            if (font != null && font != this.Font)
            {
                // Dispose font
                font.Dispose();
                font = null;
            }
        }

        #endregion

        #region Mouse Events
        
        /// <summary>
        /// Occurs when the user double clicks on the control.
        /// Expands the header if the cursor is in the header area.
        /// </summary>
        /// <param name="e">Mouse Event arguments</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (headerExpandable)
            {
                if (e.Y >= 0 && e.Y <= headerHeight)
                {
                    Expand();
                }
            }
        }

        #endregion

    }
}
