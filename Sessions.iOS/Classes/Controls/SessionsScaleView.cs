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
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using CoreGraphics;
using Sessions.iOS.Classes.Objects;
using Sessions.Core;

namespace Sessions.iOS.Classes.Controls
{
	[Register("SessionsScaleView")]
	public class SessionsScaleView : UIButton
    {
		public delegate void ScaleViewClicked();
		public event ScaleViewClicked OnScaleViewClicked;

		public SessionsScaleView(IntPtr handle) : base(handle)
        {
			var tap = new UITapGestureRecognizer(() =>
			{
				AnimatePress(false);
				if (OnScaleViewClicked != null)
					OnScaleViewClicked();
			});
			AddGestureRecognizer(tap);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
			AnimatePress(true);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
			AnimatePress(false);
            base.TouchesMoved(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
			AnimatePress(false);
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
        }

		private void AnimatePress(bool on)
		{
			if (!on)
			{
				UIView.Animate(0.1, () => {
					Alpha = 1f;
					Transform = CGAffineTransform.MakeScale(1, 1);
				});
			}
			else
			{
				UIView.Animate(0.1, () => {
					Alpha = 0.7f;
					Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
				});
			}
		}
    }
}
