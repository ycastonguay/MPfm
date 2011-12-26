//
// CustomFontEditor.cs: Custom property grid type editor for custom fonts.
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
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Custom property grid type editor for custom fonts.
    /// </summary>
    public class CustomFontEditor : UITypeEditor
    {
        /// <summary>
        /// Returns the window style.
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>Window style</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
       
        /// <summary>
        /// Triggers the Edit window.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="provider">Provider</param>
        /// <param name="value">Property value</param>
        /// <returns>New value</returns>
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            // Get service info
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            
            // Cast value
            CustomFont font = value as CustomFont;

            // Make sure value is ok
            if (svc != null && font != null)
            {
                // Open form
                using (CustomFontEditorForm form = new CustomFontEditorForm())
                {
                    // Instance a new font (or if the user cancels the form, it will STILL change the font values)
                    CustomFont newFont = new CustomFont();
                    newFont.EmbeddedFontName = font.EmbeddedFontName;
                    newFont.StandardFontName = font.StandardFontName;
                    newFont.Size = font.Size;
                    newFont.IsBold = font.IsBold;
                    newFont.IsItalic = font.IsItalic;
                    newFont.IsUnderline = font.IsUnderline;
                    newFont.UseEmbeddedFont = font.UseEmbeddedFont;

                    // Set form text
                    form.CustomFont = newFont;

                    // Show form
                    DialogResult result = svc.ShowDialog(form);
                    if(result == DialogResult.OK)
                    {
                        // The form has ended with OK; set value
                        font = form.CustomFont;                        
                    }
                }
            }

            return font;
        }
    }
}
