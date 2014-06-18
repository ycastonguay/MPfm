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
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MonoTouch.CoreGraphics;
using MPfm.iOS.Classes.Objects;
using MPfm.Core;

namespace MPfm.iOS.Classes.Controls
{
	[Register("MPfmScaleView")]
	public class MPfmScaleView : UIButton
    {
		public delegate void ScaleViewClicked();
		public event ScaleViewClicked OnScaleViewClicked;

		public MPfmScaleView(IntPtr handle) : base(handle)
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
