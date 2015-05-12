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

namespace Sessions.iOS.Classes.Controls.Cells.Base
{
    [Register("SessionsBaseExpandableTableViewCell")]
    public abstract class SessionsBaseExpandableTableViewCell : SessionsBaseTableViewCell
    {
        public bool IsExpanded { get; protected set; }
    
        public SessionsBaseExpandableTableViewCell() : base()
        {
        }

        public SessionsBaseExpandableTableViewCell(IntPtr handle) : base(handle)
        {
        }

        public SessionsBaseExpandableTableViewCell(RectangleF frame) : base(frame)
        {
        }

        public SessionsBaseExpandableTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void ExpandCell(bool animated)
        {
            if (animated)
            {
                UIView.Animate(0.2, ExpandCell, () => IsExpanded = true);
            }
            else
            {
                ExpandCell();
                IsExpanded = true;
            }
        }

        protected virtual void ExpandCell()
        {
            SetControlVisibilityForExpand(true);
        }

        public void CollapseCell(bool animated)
        {
            if (animated)
            {
                IsTextLabelAllowedToChangeFrame = false;
                UIView.Animate(0.2, CollapseCell, () => {
                    IsExpanded = false;
                    IsTextLabelAllowedToChangeFrame = true;
                });
            }
            else
            {
                CollapseCell();
                IsExpanded = false;
            }
        }

        protected virtual void CollapseCell()
        {
            SetControlVisibilityForExpand(false);
        }

        protected abstract void SetControlVisibilityForExpand(bool isExpanded);
    }
}
