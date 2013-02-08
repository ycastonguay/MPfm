using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MPfm.GTK
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static System.Drawing.Image ResizeImage(System.Drawing.Image image, int width, int height)
		{
			// Create image
			Image imageResized = new Bitmap(width, height);
			
			// Create Graphics object
			Graphics graphics = Graphics.FromImage(imageResized);
			
			// Set Graphics object options
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			
			// Create rectangle with new width/height
			Rectangle rect = new Rectangle(0, 0, width, height);
			
			// Draw image
			graphics.DrawImage(image, rect);
			
			// Dispose graphics
			graphics.Dispose();
			
			return imageResized;
		}
	}
}
