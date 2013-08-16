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
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Mono.Unix;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;

namespace MPfm.Mac.Classes.Helpers
{
    /// <summary>
    /// Helper static class for miscellaneous helper methods.
    /// </summary>
    public static class CocoaHelper
    {
        public static void ShowCriticalAlert(string title, string text)
        {
            using(NSAlert alert = new NSAlert())
            {
                alert.MessageText = title;
                alert.InformativeText = text;
                alert.AlertStyle = NSAlertStyle.Critical;
                alert.RunModal();
            }
        }

        public static RectangleF MeasureString(SizeF sizeConstraint, string text, string fontName, float fontSize)
        {
            NSMutableDictionary dict = new NSMutableDictionary();
            dict.Add(NSAttributedString.FontAttributeName, NSFont.FromFontName(fontName, fontSize));
            NSString nsstr = new NSString(text);
            RectangleF rect = nsstr.BoundingRectWithSize(sizeConstraint, NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin, dict);
            return rect;
        }

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

        public static void DrawRect(CGContext context, RectangleF rect, CGColor color)
        {
            context.SaveState();
            context.AddRect(rect);
            context.Clip();
            context.SetLineWidth(2);
            context.SetStrokeColor(color);
            context.StrokeRect(rect);
            context.RestoreState();
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

        public static void DrawText(RectangleF rect, float x, float y, string text, string fontName, float fontSize, NSColor fontColor)
        {
            NSMutableDictionary dict = new NSMutableDictionary();
            dict.Add(NSAttributedString.FontAttributeName, NSFont.FromFontName(fontName, fontSize));
            dict.Add(NSAttributedString.ForegroundColorAttributeName, fontColor);
            NSString nsstr = new NSString(text);
            RectangleF rectBounds = nsstr.BoundingRectWithSize(new SizeF(rect.Width, rect.Height), NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin, dict);
            rectBounds.X = rect.X + x;
            rectBounds.Y = rect.Y + y;
            nsstr.DrawString(rectBounds, NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin, dict);
        }
    }
}

