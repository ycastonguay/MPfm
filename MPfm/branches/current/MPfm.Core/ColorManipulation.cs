//
// ColorManipulation.cs: Color manipulation routines.
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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MPfm.Core
{
    /// <summary>
    /// The ColorManipulation class contains static functions related to manipulating colors.
    /// </summary>
    public static class ColorManipulation
    {
        /// <summary>
        /// This method takes a color and applies a brightness level to it.
        /// </summary>
        /// <param name="color">Color object to apply brightness</param>
        /// <param name="brightnessLevel">The level of brightness to apply</param>
        /// <returns>Color object with the brightness applied</returns>
        public static Color Brightness(Color color, int brightnessLevel)
        {
            int colorR, colorG, colorB;

            if (color.R + brightnessLevel > 255)
            {
                colorR = 255;
            }
            else if (color.R + brightnessLevel < 0)
            {
                colorR = 0;
            }
            else
            {
                colorR = color.R + brightnessLevel;
            }

            if (color.G + brightnessLevel > 255)
            {
                colorG = 255;
            }
            else if (color.G + brightnessLevel < 0)
            {
                colorG = 0;
            }
            else
            {
                colorG = color.G + brightnessLevel;
            }

            if (color.B + brightnessLevel > 255)
            {
                colorB = 255;
            }
            else if (color.B + brightnessLevel < 0)
            {
                colorB = 0;
            }
            else
            {
                colorB = color.B + brightnessLevel;
            }

            return Color.FromArgb(colorR, colorG, colorB);
        }
    }
}
