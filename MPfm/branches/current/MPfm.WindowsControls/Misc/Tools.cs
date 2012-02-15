//
// Tools.cs: This class contains miscelleanous methods for Windows controls.
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
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This class contains miscelleanous methods for Windows controls.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Checks if the current process is in "design-time" (i.e. in Visual Studio).
        /// </summary>
        /// <returns>True if in Visual Studio</returns>
        public static bool IsDesignTime()
        {
            // Check if the process is devenv
            // http://stackoverflow.com/questions/282014/net-windows-forms-design-time-rules
            return (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
        }

        /// <summary>  
        /// Method for changing the opacity of an image.
        /// http://stackoverflow.com/questions/4779027/changing-the-opacity-of-a-bitmap-image
        /// </summary>  
        /// <param name="image">image to set opacity on</param>  
        /// <param name="opacity">percentage of opacity</param>  
        /// <returns>Image</returns>  
        public static Image SetImageOpacity(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        } 

        /// <summary>
        /// Adds a font to a private font collection.
        /// </summary>
        /// <param name="resourceName">Name of the resource</param>
        /// <param name="pfc">Collection</param>
        public static void AddFontToPrivateFontCollection(string resourceName, PrivateFontCollection pfc)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream fontStream = assembly.GetManifestResourceStream(resourceName);

            byte[] fontdata = new byte[fontStream.Length];
            fontStream.Read(fontdata, 0, (int)fontStream.Length);
            fontStream.Close();

            unsafe
            {
                fixed (byte* pFontData = fontdata)
                {
                    pfc.AddMemoryFont((System.IntPtr)pFontData, fontdata.Length);
                }
            }
        }

        /// <summary>
        /// Loads a custom font from a custom font collection, using the font name, size and style
        /// passed in parameter.
        /// </summary>
        /// <param name="fontCollection">Custom font collection</param>
        /// <param name="customFontName">Custom font name</param>
        /// <param name="fontSize">Font size</param>
        /// <param name="fontStyle">Font style</param>
        /// <returns></returns>
        public static Font LoadEmbeddedFont(EmbeddedFontCollection fontCollection, string customFontName, float fontSize, FontStyle fontStyle)
        {
            // Declare variables
            Font font = null;

            try
            {
                // Check if the parameters are valid
                if (fontCollection != null && customFontName.Length > 0)
                {
                    // Get custom font family
                    FontFamily family = fontCollection.GetFontFamily(customFontName);

                    // Check if family is valid
                    if (family != null)
                    {
                        // Create font
                        font = new Font(family, fontSize, fontStyle);
                    }
                }
            }
            catch
            {
                // Do nothing
            }

            return font;
        }

        /// <summary>
        /// Gets a rectangle with rounded corners based on the rectangle passed in parameter.
        /// </summary>
        /// <param name="baseRect">Base Rectangle</param>
        /// <param name="radius">Radio (for rounded corners)</param>
        /// <returns>Rectangle (with rounded corners, GraphicsPath)</returns>
        public static GraphicsPath GetRoundedRect(RectangleF baseRect, float radius) 
        {
            // if corner radius is less than or equal to zero, 
            // return the original rectangle s
            if( radius<=0.0F ) 
            { 
                GraphicsPath mPath = new GraphicsPath(); 
                mPath.AddRectangle(baseRect); 
                mPath.CloseFigure(); 
                return mPath;
            }

            // if the corner radius is greater than or equal to 
            // half the width, or height (whichever is shorter) 
            // then return a capsule instead of a lozenge 
            if( radius>=(Math.Min(baseRect.Width, baseRect.Height))/2.0) 
              return GetCapsule( baseRect ); 

            // create the arc for the rectangle sides and declare 
            // a graphics path object for the drawing 
            float diameter = radius * 2.0F; 
            SizeF sizeF = new SizeF( diameter, diameter );
            RectangleF arc = new RectangleF( baseRect.Location, sizeF ); 
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath(); 

            // top left arc 
            path.AddArc( arc, 180, 90 ); 

            // top right arc 
            arc.X = baseRect.Right-diameter; 
            path.AddArc( arc, 270, 90 ); 

            // bottom right arc 
            arc.Y = baseRect.Bottom-diameter; 
            path.AddArc( arc, 0, 90 ); 

            // bottom left arc
            arc.X = baseRect.Left;     
            path.AddArc( arc, 90, 90 );     

            path.CloseFigure(); 
            return path; 
        }         

        /// <summary>
        /// Returns the capsule (GraphicsPath) of a rectangle.
        /// </summary>
        /// <param name="baseRect">Rectangle</param>
        /// <returns>Capsule (GraphicsPath)</returns>
        public static GraphicsPath GetCapsule( RectangleF baseRect ) 
        { 
            float diameter; 
            RectangleF arc; 
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath(); 
            try 
            { 
                if( baseRect.Width>baseRect.Height ) 
                { 
                    // return horizontal capsule 
                    diameter = baseRect.Height; 
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF( baseRect.Location, sizeF ); 
                    path.AddArc( arc, 90, 180); 
                    arc.X = baseRect.Right-diameter; 
                    path.AddArc( arc, 270, 180); 
                } 
                else if( baseRect.Width < baseRect.Height ) 
                { 
                    // return vertical capsule 
                    diameter = baseRect.Width;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF( baseRect.Location, sizeF ); 
                    path.AddArc( arc, 180, 180 ); 
                    arc.Y = baseRect.Bottom-diameter; 
                    path.AddArc( arc, 0, 180 ); 
                } 
                else
                { 
                    // return circle 
                    path.AddEllipse( baseRect ); 
                }
            } 
            catch
            {
                path.AddEllipse( baseRect ); 
            } 
            finally 
            { 
                path.CloseFigure(); 
            } 
            return path; 
        }

        /// <summary>
        /// Creates a cursor from bitmap.
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="xHotSpot">Hot spot (x)</param>
        /// <param name="yHotSpot">Hot spot (y)</param>
        /// <returns>Cursor</returns>
        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            Win32.IconInfo tmp = new Win32.IconInfo();
            Win32.GetIconInfo(bmp.GetHicon(), ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            return new Cursor(Win32.CreateIconIndirect(ref tmp));
        }
    }
}
