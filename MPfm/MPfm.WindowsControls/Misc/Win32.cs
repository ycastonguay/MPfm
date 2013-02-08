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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This static class contains useful methods for Win32 operation.
    /// </summary>
    public static class Win32
    {
        #region Mouse Events
        
        /// <summary>
        /// Defines the mouse wheel event identifier.
        /// </summary>
        public const int WM_MOUSEWHEEL = 0x020A;

        /// <summary>
        /// Returns the mouse wheel delta value from parameter.
        /// </summary>
        /// <param name="wparam">Mouse event parameter</param>
        /// <returns>Mouse wheel delta</returns>
        public static int GetWheelDeltaWParam(int wparam) 
        { 
            return HighWord(wparam); 
        }

        /// <summary>
        /// Returns the low 16-bit value of a 32-bit word.
        /// </summary>
        /// <param name="word">32-bit word</param>
        /// <returns>Low 16-bit value</returns>
        public static int LowWord(int word) 
        { 
            return word & 0xFFFF; 
        }

        /// <summary>
        /// Returns the high 16-bit value of a 32-bit word.
        /// </summary>
        /// <param name="word">32-bit word</param>
        /// <returns>High 16-bit value</returns>
        public static int HighWord(int word) 
        { 
            return word >> 16; 
        }

        #endregion

        #region Icons

        /// <summary>
        /// Defines the structure of an icon.
        /// </summary>
        public struct IconInfo
        {
            /// <summary>
            /// If true, the icon is valid.
            /// </summary>
            public bool fIcon;

            /// <summary>
            /// Hot spot (x).
            /// </summary>
            public int xHotspot;

            /// <summary>
            /// Hot spot (y).
            /// </summary>
            public int yHotspot;

            /// <summary>
            /// Icon mask.
            /// </summary>
            public IntPtr hbmMask;

            /// <summary>
            /// Icon color.
            /// </summary>
            public IntPtr hbmColor;
        }

        /// <summary>
        /// Create an icon.
        /// </summary>
        /// <param name="icon">Icon</param>
        /// <returns>Icon pointer</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        /// <summary>
        /// Returns information about an icon.
        /// </summary>
        /// <param name="hIcon">Icon pointer</param>
        /// <param name="pIconInfo">Icon information (ref)</param>
        /// <returns>True if successful</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        #endregion
    }
}
