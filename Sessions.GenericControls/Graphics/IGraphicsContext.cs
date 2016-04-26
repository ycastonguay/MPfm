// Copyright � 2011-2013 Yanick Castonguay
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
using Sessions.GenericControls.Basics;
using System.Collections.Generic;

namespace Sessions.GenericControls.Graphics
{
    public interface IGraphicsContext
    {
        BasicRectangle DirtyRect { get; }
        float BoundsWidth { get; }
        float BoundsHeight { get; }
        float Density { get; }

		// Should this be a different kind of graphics context? IPlotGraphicsContext?
		// These methods are used for drawing as quick as possible without reseting the pen and brushes
        void SetPen(BasicPen pen);
		void StrokeLine(BasicPoint point, BasicPoint point2);
		void SaveState();
		void RestoreState();

		// These methods are inteded to be easy to use
        void DrawImage(BasicRectangle rectangle, IDisposable image);
        void DrawImage(BasicRectangle rectangleDestination, BasicRectangle rectangleSource, IDisposable image);
        void DrawEllipsis(BasicRectangle rectangle, BasicBrush brush, BasicPen pen);
        void DrawRectangle(BasicRectangle rectangle, BasicBrush brush, BasicPen pen);
        void DrawLine(BasicPoint point, BasicPoint point2, BasicPen pen);
        void DrawLine(List<BasicPoint> points, BasicPen pen, bool isRounded, bool closePath, bool isDashed);
        void DrawPath(BasicPath path, BasicBrush brush, BasicPen pen);
        void DrawText(string text, BasicPoint point, BasicColor color, string fontFace, float fontSize);
        void DrawText(string text, BasicRectangle rectangle, BasicColor color, string fontFace, float fontSize);
        BasicRectangle MeasureText(string text, BasicRectangle rectangle, string fontFace, float fontSize);

        void Close();
    }
}