//
// CustomFontEditorForm.cs: Custom property grid type editor form for custom fonts.
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
        private FontCollection m_fontCollection = null;
        public FontCollection FontCollection
        {
            get
            {
                return m_fontCollection;
            }
            set
            {
                m_fontCollection = value;
            }
        }

        /// <summary>
        /// Default constructor for the FontEditorForm class.
        /// </summary>
        public CustomFontEditorForm()
        {
            // Initialize components
            InitializeComponent();

            // Create font collection
            m_fontCollection = new FontCollection();

            // Add fonts
            EmbeddedFont font = new EmbeddedFont();
            font.AssemblyPath = "MPfm.Fonts.dll";
            font.Name = "LeagueGothic";
            font.ResourceName = "MPfm.Fonts.LeagueGothic.ttf";
            m_fontCollection.Fonts.Add(font);

            font = new EmbeddedFont();
            font.AssemblyPath = "MPfm.Fonts.dll";
            font.Name = "Junction";
            font.ResourceName = "MPfm.Fonts.Junction.ttf";
            m_fontCollection.Fonts.Add(font);
        }

        private void FontEditorForm_Load(object sender, EventArgs e)
        {
            // Set font collection
            lblPreview.FontCollection = m_fontCollection;
            lblPreview.CustomFontName = "Junction";

            // Get list of installed fonts
            InstalledFontCollection fonts = new InstalledFontCollection();
            for (int a = 0; a < fonts.Families.Length; a++)
            {
                comboStandardFontName.Items.Add(fonts.Families[a].Name);
            }

            // Set text
            Text = "Edit font";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboCustomFontName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboStandardFontName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblUseCustomFont_Click(object sender, EventArgs e)
        {
            radioUseCustomFont.Checked = true;
        }

        private void lblUseStandardFont_Click(object sender, EventArgs e)
        {
            radioUseStandardFont.Checked = true;
        }

        private void radioUseCustomFont_CheckedChanged(object sender, EventArgs e)
        {
            radioUseStandardFont.Checked = !radioUseCustomFont.Checked;
            comboStandardFontName.Enabled = radioUseStandardFont.Checked;
            comboCustomFontName.Enabled = radioUseCustomFont.Checked;
        }

        private void radioUseStandardFont_CheckedChanged(object sender, EventArgs e)
        {
            radioUseCustomFont.Checked = !radioUseStandardFont.Checked;
            comboStandardFontName.Enabled = radioUseStandardFont.Checked;
            comboCustomFontName.Enabled = radioUseCustomFont.Checked;
        }

        private void trackFontSize_OnTrackBarValueChanged()
        {
            lblFontSize.Text = "Font Size: " + trackFontSize.Value.ToString() + " pt";
        }
    }
}
