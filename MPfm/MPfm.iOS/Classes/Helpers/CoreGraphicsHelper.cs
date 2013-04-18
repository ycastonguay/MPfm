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
using System.IO;
using System.Reflection;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MPfm.iOS.Helpers
{
    /// <summary>
    /// Helper static class for Core Graphics.
    /// </summary>
    public static class CoreGraphicsHelper
    {
        // TODO: Cannot use NSAttributedString in iOS5, only iOS6+
//        public static RectangleF MeasureString(SizeF sizeConstraint, string text, string fontName, float fontSize)
//        {
//            NSMutableDictionary dict = new NSMutableDictionary();
//            dict.Add(NSAttributedString.FontAttributeName, NSFont.FromFontName(fontName, fontSize));
//            NSString nsstr = new NSString(text);
//            RectangleF rect = nsstr.BoundingRectWithSize(sizeConstraint, NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin, dict);
//            return rect;
//        }
        
        public static float MeasureStringWidth(CGContext context, string text, string fontName, float fontSize)
        {
            context.SaveState();
            PointF pos = context.TextPosition;
            context.SelectFont(fontName, fontSize, CGTextEncoding.MacRoman);
            context.TextMatrix = CGAffineTransform.MakeScale(1.0f, -1.0f);
            //context.TranslateCTM(0, 20);
            context.ScaleCTM(1, -1);
            context.SetTextDrawingMode(CGTextDrawingMode.Invisible);
            context.ShowTextAtPoint(pos.X, pos.Y, text);
            PointF pos2 = context.TextPosition;
            context.RestoreState();
            
            return pos2.X - pos.X;
        }
        
        public static void FillRect(CGContext context, RectangleF rect, CGColor color)
        {
            context.SaveState();
            context.AddRect(rect);
            context.Clip();
            context.SetFillColor(color);
            context.FillRect(rect);
            context.RestoreState();
        }
        
        public static void DrawEllipsis(CGContext context, RectangleF rect, CGColor color, float lineWidth)
        {
            context.SaveState();
            context.SetStrokeColor(color);
            context.SetLineWidth(lineWidth);
            context.AddEllipseInRect(rect);
            context.StrokePath();
            context.RestoreState();
        }
        
        public static void FillEllipsis(CGContext context, RectangleF rect, CGColor color)
        {
            context.SaveState();
            context.SetFillColor(color);
            context.FillEllipseInRect(rect);
            context.FillPath();
            context.RestoreState();
        }
        
        public static void FillPath(CGContext context, CGPath path, CGColor color)
        {
            context.SaveState();
            context.SetFillColor(color);
            context.AddPath(path);
            context.FillPath();
            context.RestoreState();
        }
        
        public static void EOFillPath(CGContext context, CGPath path, CGColor color)
        {
            context.SaveState();
            context.SetFillColor(color);
            context.AddPath(path);
            context.EOFillPath();
            context.RestoreState();
        }
        
        public static void FillGradient(CGContext context, RectangleF rect, CGColor color1, CGColor color2)
        {
            CGGradient gradientBackground;
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            
            float[] locationListBackground = new float[] { 1.0f, 0.0f };
            List<float> colorListBackground = new List<float>();
            colorListBackground.AddRange(color1.Components);
            colorListBackground.AddRange(color2.Components);
            gradientBackground = new CGGradient(colorSpace, colorListBackground.ToArray(), locationListBackground);
            
            context.SaveState();
            context.AddRect(rect);
            context.Clip();
            //context.ScaleCTM(1, -1);
            context.DrawLinearGradient(gradientBackground, new PointF(0, 0), new PointF(0, rect.Height), CGGradientDrawingOptions.DrawsBeforeStartLocation);
            context.RestoreState();
        }       
        
        public static void DrawText(CGContext context, string text, string fontName, float fontSize, float translateHeight, float x, float y)
        {
            context.SaveState();
            context.SelectFont(fontName, fontSize, CGTextEncoding.MacRoman);
            context.SetTextDrawingMode(CGTextDrawingMode.Fill);
            context.SetFillColor(new CGColor(1, 1));
            context.SetStrokeColor(new CGColor(1.0f, 1.0f));
            //context.AddRect(rectText);
            //context.Clip();
            context.TextMatrix = CGAffineTransform.MakeScale(1.0f, -1.0f);
            context.TranslateCTM(0, translateHeight);
            context.ScaleCTM(1, -1);
            context.ShowTextAtPoint(x, y, text);
            context.RestoreState();
        }

        // TODO: Cannot use NSAttributedString in iOS5, only iOS6+
//        public static void DrawText(RectangleF rect, float x, float y, string text, string fontName, float fontSize, NSColor fontColor)
//        {
//            NSMutableDictionary dict = new NSMutableDictionary();
//            dict.Add(NSAttributedString.FontAttributeName, NSFont.FromFontName(fontName, fontSize));
//            dict.Add(NSAttributedString.ForegroundColorAttributeName, fontColor);
//            NSString nsstr = new NSString(text);
//            RectangleF rectBounds = nsstr.BoundingRectWithSize(new SizeF(rect.Width, rect.Height), NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin, dict);
//            rectBounds.X = rect.X + x;
//            rectBounds.Y = rect.Y + y;
//            nsstr.DrawString(rectBounds, NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin, dict);
//        }

        public static UIImage ScaleImage(UIImage image, int maxSize)
        {           
            UIImage newImage;
            using (CGImage imageRef = image.CGImage)
            {
                CGImageAlphaInfo alphaInfo = imageRef.AlphaInfo;
                CGColorSpace colorSpaceInfo = CGColorSpace.CreateDeviceRGB();
                if (alphaInfo == CGImageAlphaInfo.None)
                    alphaInfo = CGImageAlphaInfo.NoneSkipLast;
  
                int width = maxSize;
                int height = maxSize;

//                int width = imageRef.Width;
//                int height = imageRef.Height;
                
//                if (height >= width)
//                {
//                    width = (int)Math.Floor((double)width * ((double)maxSize / (double)height));
//                    height = maxSize;
//                }
//                else
//                {
//                    height = (int)Math.Floor((double)height * ((double)maxSize / (double)width));
//                    width = maxSize;
//                }
                
                CGBitmapContext bitmap;
                if (image.Orientation == UIImageOrientation.Up || image.Orientation == UIImageOrientation.Down)
                    //bitmap = new CGBitmapContext(IntPtr.Zero, width, height, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, alphaInfo);
                    bitmap = new CGBitmapContext(IntPtr.Zero, width, height, imageRef.BitsPerComponent, 0, colorSpaceInfo, alphaInfo);
                else
                    bitmap = new CGBitmapContext(IntPtr.Zero, height, width, imageRef.BitsPerComponent, 0, colorSpaceInfo, alphaInfo);
                
                switch (image.Orientation)
                {
                    case UIImageOrientation.Left:
                        bitmap.RotateCTM((float)Math.PI / 2);
                        bitmap.TranslateCTM(0, -height);
                        break;
                    case UIImageOrientation.Right:
                        bitmap.RotateCTM(-((float)Math.PI / 2));
                        bitmap.TranslateCTM(-width, 0);
                        break;
                    case UIImageOrientation.Up:
                        break;
                    case UIImageOrientation.Down:
                        bitmap.TranslateCTM(width, height);
                        bitmap.RotateCTM(-(float)Math.PI);
                        break;
                }
                bitmap.DrawImage(new Rectangle(0, 0, width, height), imageRef);
                newImage = UIImage.FromImage(bitmap.ToImage());
                bitmap.Dispose();
                bitmap = null;
            }
            
            return newImage;
        }
    }
}

