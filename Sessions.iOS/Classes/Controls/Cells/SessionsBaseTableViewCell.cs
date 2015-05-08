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
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using Sessions.iOS.Classes.Objects;
using Sessions.Core;

namespace Sessions.iOS.Classes.Controls.Cells
{
	[Register("SessionsBaseTableViewCell")]
	public class SessionsBaseTableViewCell : UITableViewCell
    {
        protected UIView BackgroundView { get; set; }

        public bool IsTextAnimationEnabled { get; set; }

        public SessionsBaseTableViewCell() : base()
        {
            Initialize();
        }

        public SessionsBaseTableViewCell(RectangleF frame) : base(frame)
        {
            Initialize();
        }

        public SessionsBaseTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            BackgroundView = new UIView(Frame);
            BackgroundView.BackgroundColor = UIColor.Clear;
            AddSubview(BackgroundView);
        }
    }
}
