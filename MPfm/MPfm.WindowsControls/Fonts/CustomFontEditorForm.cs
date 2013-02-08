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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Custom property grid type editor form for custom fonts.
    /// </summary>
    public partial class CustomFontEditorForm : Form
    {
        // Private variables
        private bool formLoaded = false;
        private InstalledFontCollection fonts = null;
        private EmbeddedFontCollection embeddedFonts = null;

        /// <summary>
        /// Private value for the CustomFont property.
        /// </summary>
        private CustomFont customFont = null;
        /// <summary>
        /// Defines the font to be modified.
        /// </summary>
        public CustomFont CustomFont
        {
            get
            {
                return customFont;
            }
            set
            {
                customFont = value;
            }
        }

        /// <summary>
        /// Default constructor for the FontEditorForm class.
        /// </summary>
        public CustomFontEditorForm()
        {
            // Initialize components
            InitializeComponent();

            // Create default font
            customFont = new WindowsControls.CustomFont();         
        }

        /// <summary>
        /// Occurs when the form is ready to load.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void CustomFontEditorForm_Load(object sender, EventArgs e)
        {
            // Get list of embedded fonts
            embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();
            comboCustomFontName.DataSource = embeddedFonts;

            // Get list of standard fonts
            fonts = new InstalledFontCollection();
            for (int a = 0; a < fonts.Families.Length; a++)
            {
                // Make sure the regular style is available
                if (fonts.Families[a].IsStyleAvailable(FontStyle.Regular))
                {
                    // Add font
                    int index = comboStandardFontName.Items.Add(fonts.Families[a].Name);

                    // Check if the font name matches
                    if (CustomFont.StandardFontName.ToUpper() == fonts.Families[a].Name.ToUpper())
                    {
                        // Set selected index
                        comboStandardFontName.SelectedIndex = index;
                    }
                    
                }
            }            

            // Loop through embedded fonts
            foreach (EmbeddedFont embeddedFont in embeddedFonts)
            {
                // Check if the name matches
                if (embeddedFont.Name.ToUpper() == CustomFont.EmbeddedFontName.ToUpper())
                {
                    // Set combo box item
                    comboCustomFontName.SelectedItem = embeddedFont;
                    break;
                }
            }            

            // Set initial values
            radioUseCustomFont.Checked = CustomFont.UseEmbeddedFont;
            lblFontSize.Text = "Font Size: " + CustomFont.Size.ToString() + " pt";
            trackFontSize.Value = (int)CustomFont.Size;
            chkIsBold.Checked = CustomFont.IsBold;
            chkIsItalic.Checked = CustomFont.IsItalic;
            chkIsUnderline.Checked = CustomFont.IsUnderline;

            // Refresh preview
            lblPreview.Theme.TextGradient.Font = CustomFont;
            lblPreview.Refresh();

            // Set text
            Text = "Edit font";

            // Set flags
            formLoaded = true;
        }

        /// <summary>
        /// Occurs when the user clicks on the OK button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Set result and close form
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Occurs when the user clicks on the Cancel button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Close window
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Occurs when the user changes the embedded font name in the combo box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void comboCustomFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the form is loading
            if (!formLoaded)
            {
                return;
            }

            // Get embedded font
            EmbeddedFont font = (EmbeddedFont)comboCustomFontName.SelectedItem;

            // Set preview font            
            CustomFont.EmbeddedFontName = font.Name;
            CustomFont.UseEmbeddedFont = true;
            lblPreview.Theme.TextGradient.Font = CustomFont;
            lblPreview.Refresh();
        }

        /// <summary>
        /// Occurs when the user changes the standard font name in the combo box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void comboStandardFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the form is loading
            if (!formLoaded)
            {
                return;
            }

            // Set preview font
            string font = (comboStandardFontName.Items.Count > 0) ? comboStandardFontName.Items[comboStandardFontName.SelectedIndex].ToString() : "";

            // Make sure font isn't empty
            if (!String.IsNullOrEmpty(font))
            {
                // Set font properties
                CustomFont.StandardFontName = font;
                CustomFont.UseEmbeddedFont = false;
                lblPreview.Theme.TextGradient.Font = CustomFont;
                lblPreview.Refresh();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the Use custom font radio button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void radioUseCustomFont_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the form is loading
            if (!formLoaded)
            {
                return;
            }

            // Set control enable
            radioUseStandardFont.Checked = !radioUseCustomFont.Checked;
            comboStandardFontName.Enabled = radioUseStandardFont.Checked;
            comboCustomFontName.Enabled = radioUseCustomFont.Checked;

            // Set font properties
            CustomFont.UseEmbeddedFont = radioUseCustomFont.Checked;
            lblPreview.Theme.TextGradient.Font = CustomFont;
            lblPreview.Refresh();
        }

        /// <summary>
        /// Occurs when the user clicks on the Use standard font radio button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void radioUseStandardFont_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the form is loading
            if (!formLoaded)
            {
                return;
            }

            // Set control enable
            radioUseCustomFont.Checked = !radioUseStandardFont.Checked;
            comboStandardFontName.Enabled = radioUseStandardFont.Checked;
            comboCustomFontName.Enabled = radioUseCustomFont.Checked;

            // Set font properties
            CustomFont.UseEmbeddedFont = radioUseCustomFont.Checked;
            lblPreview.Theme.TextGradient.Font = CustomFont;
            lblPreview.Refresh();
        }

        /// <summary>
        /// Occurs when the user changes the font size using the track bar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void trackFontSize_Scroll(object sender, EventArgs e)
        {
            // Check if the form is loading
            if (!formLoaded)
            {
                return;
            }

            // Update label
            lblFontSize.Text = "Font Size: " + trackFontSize.Value.ToString() + " pt";

            // Update font            
            CustomFont.Size = trackFontSize.Value;
            lblPreview.Theme.TextGradient.Font = CustomFont;
            lblPreview.Refresh();
        }

        /// <summary>
        /// Occurs when the user clicks on the Bold check box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void chkIsBold_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the form is loading
            if (!formLoaded)
            {
                return;
            }

            // Set bold
            CustomFont.IsBold = chkIsBold.Checked;
            lblPreview.Theme.TextGradient.Font = CustomFont;
            lblPreview.Refresh();
        }

        /// <summary>
        /// Occurs when the user clicks on the Italic check box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void chkIsItalic_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the form is loading
            if (!formLoaded)
            {
                return;
            }

            // Set italic
            CustomFont.IsItalic = chkIsItalic.Checked;
            lblPreview.Theme.TextGradient.Font = CustomFont;
            lblPreview.Refresh();
        }

        /// <summary>
        /// Occurs when the user clicks on the Underline check box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void chkIsUnderline_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the form is loading
            if (!formLoaded)
            {
                return;
            }

            // Set underline
            CustomFont.IsUnderline = chkIsUnderline.Checked;
            lblPreview.Theme.TextGradient.Font = CustomFont;
            lblPreview.Refresh();
        }
    }
}