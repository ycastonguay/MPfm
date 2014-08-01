// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Sessions.iOS.Helpers
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
			if (string.IsNullOrEmpty(text))
				return 0;

            //context.SaveState();
            PointF pos = context.TextPosition;
            context.SelectFont(fontName, fontSize, CGTextEncoding.MacRoman);
            context.TextMatrix = CGAffineTransform.MakeScale(1.0f, -1.0f);
            //context.TranslateCTM(0, 20);
            context.ScaleCTM(1, -1);
            context.SetTextDrawingMode(CGTextDrawingMode.Invisible);
            context.ShowTextAtPoint(pos.X, pos.Y, text);
            PointF pos2 = context.TextPosition;
            //context.RestoreState();
            
            return pos2.X - pos.X;
        }
        
        public static void FillRect(CGContext context, RectangleF rect, CGColor color)
        {
            //context.SaveState();
            context.AddRect(rect);
            //context.Clip();
            context.SetFillColor(color);
            context.FillRect(rect);
            //context.RestoreState();
        }
        
        public static void DrawEllipsis(CGContext context, RectangleF rect, CGColor color, float lineWidth)
        {
            //context.SaveState();
            context.SetStrokeColor(color);
            context.SetLineWidth(lineWidth);
            context.AddEllipseInRect(rect);
            context.StrokePath();
            //context.RestoreState();
        }

        public static void DrawLine(CGContext context, List<PointF> points, CGColor color, float lineWidth, bool closePath, bool dashed)
        {
            if (points == null)
                throw new NullReferenceException();

            if (points.Count == 0)
                throw new ArgumentException("The line must have at least one point.");

            //context.SaveState();
            context.SetStrokeColor(color);
            context.SetLineWidth(lineWidth);
            context.MoveTo(points[0].X, points[0].Y);
            for(int a = 1; a < points.Count; a++)
                context.AddLineToPoint(points[a].X, points[a].Y);
            if (dashed)
                context.SetLineDash(0, new float[2] { 1, 2 }, 2);
            if (closePath)
                context.ClosePath();
            context.StrokePath();
            //context.RestoreState();
        }

        public static void DrawRoundedLine(CGContext context, List<PointF> points, CGColor color, float lineWidth, bool closePath, bool dashed)
        {
            if (points == null)
                throw new NullReferenceException();

            if (points.Count == 0)
                throw new ArgumentException("The line must have at least one point.");

            //context.SaveState();
            context.SetStrokeColor(color);
            context.SetLineWidth(lineWidth);
            context.SetLineCap(CGLineCap.Round);
            context.SetLineJoin(CGLineJoin.Round);
            context.MoveTo(points[0].X, points[0].Y);
            for(int a = 1; a < points.Count; a++)
                context.AddLineToPoint(points[a].X, points[a].Y);
            if (dashed)
                context.SetLineDash(0, new float[2] { 1, 2 }, 2);
            if (closePath)
                context.ClosePath();
            context.StrokePath();
            //context.RestoreState();
        }

        public static void FillEllipsis(CGContext context, RectangleF rect, CGColor color)
        {
            //context.SaveState();
            context.SetFillColor(color);
            context.FillEllipseInRect(rect);
            context.FillPath();
            //context.RestoreState();
        }
        
        public static void FillPath(CGContext context, CGPath path, CGColor color)
        {
            //context.SaveState();
            context.SetFillColor(color);
            context.AddPath(path);
            context.FillPath();
            //context.RestoreState();
        }
        
        public static void EOFillPath(CGContext context, CGPath path, CGColor color)
        {
            //context.SaveState();
            context.SetFillColor(color);
            context.AddPath(path);
            context.EOFillPath();
            //context.RestoreState();
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
            
            //context.SaveState();
            context.AddRect(rect);
            context.Clip();
            //context.ScaleCTM(1, -1);
            context.DrawLinearGradient(gradientBackground, new PointF(rect.X, rect.Y), new PointF(rect.X + rect.Width, rect.Y + rect.Height), CGGradientDrawingOptions.DrawsBeforeStartLocation);
            //context.RestoreState();
        }       
        
        public static SizeF DrawTextAtPoint(CGContext context, PointF pt, string text, string fontName, float fontSize, CGColor fontColor)
        {
            //context.SaveState();
            context.SetFillColor(fontColor);
            NSString str = new NSString(text);
            SizeF size = str.DrawString(pt, UIFont.FromName(fontName, fontSize));
            //context.RestoreState();
            return size;
        }

        public static SizeF DrawTextInRect(CGContext context, RectangleF rect, string text, string fontName, float fontSize, CGColor fontColor, UILineBreakMode breakMode, UITextAlignment alignment)
        {
            //context.SaveState();
            context.SetFillColor(fontColor);
            NSString str = new NSString(text);
            SizeF size = str.DrawString(rect, UIFont.FromName(fontName, fontSize), breakMode, alignment);
            //context.RestoreState();
            return size;
        }

        public static SizeF MeasureText(CGContext context, string text, string fontName, float fontSize)
        {
            NSString str = new NSString(text);
            SizeF size = str.StringSize(UIFont.FromName(fontName, fontSize));
            return size;
        }

        public static SizeF MeasureTextWithConstraint(CGContext context, string text, string fontName, float fontSize, UILineBreakMode breakMode, SizeF constraint)
        {
            NSString str = new NSString(text);
            SizeF size = str.StringSize(UIFont.FromName(fontName, fontSize), constraint, breakMode);
            return size;
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

