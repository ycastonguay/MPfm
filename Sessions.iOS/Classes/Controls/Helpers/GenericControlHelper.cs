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

using CoreGraphics;
using UIKit;
using Sessions.GenericControls.Basics;

namespace Sessions.iOS.Classes.Controls.Helpers
{
	public static class GenericControlHelper
	{
		public static BasicRectangle ToBasicRect(CGRect rectangle)
		{
            return new BasicRectangle((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
		}

		public static CGRect ToRect(BasicRectangle rectangle)
		{
			return new CGRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static CGPoint ToPoint(BasicPoint point)
		{
			return new CGPoint(point.X, point.Y);
		}

		public static UIColor ToColor(BasicColor color)
		{
            return UIColor.FromRGBA(color.R/255f, color.G/255f, color.B/255f, color.A/255f);
		}

		// No brushes/pens in iOS, using colors directly with CoreGraphicsHelper class
	}
}
