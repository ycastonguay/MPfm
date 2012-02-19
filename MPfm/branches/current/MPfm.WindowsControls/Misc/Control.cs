//
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

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This control adds a method for loading embedded fonts.
    /// </summary>
    public class Control : System.Windows.Forms.Control
    {
        /// <summary>
        /// Embedded font collection used for drawing.
        /// </summary>
        private EmbeddedFontCollection m_embeddedFonts = null;

        /// <summary>
        /// Default constructor for the Control class.
        /// </summary>
        public Control() 
            : base()
        {
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
                m_embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts(fontsPath);
            }
            else
            {
                // Get embedded font collection
                m_embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();
            }
        }
    }
}
