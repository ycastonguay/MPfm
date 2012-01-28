//
// ImageManipulation.cs: Image manipulation routines
//
// Copyright � 2011-2012 Yanick Castonguay
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace MPfm.Core
{
	/// <summary>
	/// The ImageManipulation class contains static functions for manipulating images.
	/// </summary>
	public class ImageManipulation
	{
        /// <summary>
        /// The ResizeImage static function resizes images to a desired width and height, with the highest quality possible.
        /// </summary>
        /// <param name="image">The Image object to resize</param>
        /// <param name="width">Resized image width</param>
        /// <param name="height">Resized image height</param>
        /// <returns></returns>
		public static System.Drawing.Image ResizeImage(System.Drawing.Image image, int width, int height)
		{
			System.Drawing.Image imageResized = new Bitmap(width, height);

			// Create Graphics Object
			Graphics graphics = Graphics.FromImage(imageResized);

			// Set Graphics Object Options
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

			Rectangle rect = new Rectangle(0, 0, width, height);

			graphics.DrawImage(image, rect);

			return imageResized;
		}
	}
}
