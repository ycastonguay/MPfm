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
using Android.Graphics;
using MPfm.Android.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Graphics;

namespace MPfm.Android.Classes.Controls.Graphics
{
    public class GraphicsContextWrapper : IGraphicsContext
    {
        private Canvas _canvas;
        private Paint _currentPaint;

        public GraphicsContextWrapper(Canvas canvas, float boundsWidth, float boundsHeight, float density, BasicRectangle dirtyRect)
        {
            _canvas = canvas;
            DirtyRect = dirtyRect;
            BoundsWidth = boundsWidth;
            BoundsHeight = boundsHeight;
            Density = density;
        }

        public BasicRectangle DirtyRect { get; private set; }
        public float Density { get; private set; }
        public float BoundsWidth { get; private set; }
        public float BoundsHeight { get; private set; }

        private void TryToCreatePaint()
        {
            if (_currentPaint != null)
                return;

            _currentPaint = new Paint();
            _currentPaint.AntiAlias = true;
        }

        public void SetStrokeColor(BasicColor color)
        {
            TryToCreatePaint();
            _currentPaint.Color = GenericControlHelper.ToColor(color);
        }

        public void SetLineWidth(float width)
        {
            TryToCreatePaint();
            _currentPaint.StrokeWidth = width;
        }

        public void SetPen(BasicPen pen)
        {
            TryToCreatePaint();
            _currentPaint.Color = GenericControlHelper.ToColor(pen.Brush.Color);
            _currentPaint.StrokeWidth = pen.Thickness;
        }

        public void StrokeLine(BasicPoint point, BasicPoint point2)
        {
            _canvas.DrawLine(point.X, point.Y, point2.X, point2.Y, _currentPaint);
        }

        public void SaveState()
        {
            _canvas.Save();
        }

        public void RestoreState()
        {
            _canvas.Restore();
        }

        public void DrawImage(BasicRectangle rectangle, IDisposable image)
        {
            var paintBitmap = new Paint();
            _canvas.DrawBitmap((Bitmap)image, rectangle.X, rectangle.Y, paintBitmap);
        }

        public void DrawImage(BasicRectangle rectangleDestination, BasicRectangle rectangleSource, IDisposable image)
        {
            var paintBitmap = new Paint();
            var bitmap = (Bitmap)image;
            //var resizedBitmap = Bitmap.CreateBitmap(bitmap, (int)rectangleSource.X, (int)rectangleSource.Y, (int)rectangleSource.Width, (int)rectangleSource.Height);
            //Console.WriteLine("GraphicsContextWrapper - DrawImage - rectDestination: {0} rectSource: {1} bitmap.Width: {2}", rectangleDestination, rectangleSource, bitmap.Width);
            //_canvas.DrawBitmap(resizedBitmap, GenericControlHelper.ToRect(rectangleSource), GenericControlHelper.ToRect(rectangleDestination), paintBitmap);
            _canvas.DrawBitmap(bitmap, GenericControlHelper.ToRect(rectangleSource), GenericControlHelper.ToRect(rectangleDestination), paintBitmap);
            //_canvas.DrawBitmap(bitmap, GenericControlHelper.ToRect(rectangleDestination), GenericControlHelper.ToRect(rectangleDestination), paintBitmap);
        }

        public void DrawEllipsis(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
        {
        }

        public void DrawRectangle(BasicRectangle rectangle, BasicBrush brush, BasicPen pen)
        {
            // TODO: Add outline
            var paint = new Paint
            {
                AntiAlias = true,
                Color = GenericControlHelper.ToColor(brush.Color)
            };
            paint.SetStyle(Paint.Style.Fill);
            if (brush is BasicGradientBrush)
            {
                var gradientBrush = (BasicGradientBrush) brush;
                paint.SetShader(new LinearGradient(gradientBrush.StartPoint.X, gradientBrush.StartPoint.Y, gradientBrush.EndPoint.X, gradientBrush.EndPoint.Y, GenericControlHelper.ToColor(gradientBrush.Color2), GenericControlHelper.ToColor(gradientBrush.Color), Shader.TileMode.Mirror));
            }
            _canvas.DrawRect(GenericControlHelper.ToRect(rectangle), paint);
        }

        public void DrawLine(BasicPoint point, BasicPoint point2, BasicPen pen)
        {
            var paint = new Paint
            {
                AntiAlias = true,
                Color = GenericControlHelper.ToColor(pen.Brush.Color),
                StrokeWidth = pen.Thickness * Density
            };
            paint.SetStyle(Paint.Style.Fill);
            _canvas.DrawLine(point.X, point.Y, point2.X, point2.Y, paint);
        }

        public void DrawPath(BasicPath path, BasicBrush brush, BasicPen pen)
        {
            throw new NotImplementedException();
        }

        public void DrawText(string text, BasicPoint point, BasicColor color, string fontFace, float fontSize)
        {
            var paint = new Paint
            {
                AntiAlias = true,
                Color = GenericControlHelper.ToColor(color),
                TextSize = fontSize * Density,
            };
            var boundsText = new Rect();
            paint.GetTextBounds(text, 0, text.Length, boundsText);
            _canvas.DrawText(text, point.X - boundsText.Left, point.Y - boundsText.Top, paint);
            //_canvas.DrawText(text, point.X, point.Y, paint);
        }

        public void DrawText(string text, BasicRectangle rectangle, BasicColor color, string fontFace, float fontSize)
        {
            throw new NotImplementedException();
        }

        public BasicRectangle MeasureText(string text, BasicRectangle rectangle, string fontFace, float fontSize)
        {
            var paint = new Paint
            {
                AntiAlias = true,
                TextSize = fontSize * Density
            };
            var rectText = new Rect();
            paint.GetTextBounds(text, 0, text.Length, rectText);
            return new BasicRectangle(rectText.Left, rectText.Top, rectText.Width(), rectText.Height());
        }

        public void Close()
        {
        }
    }
}