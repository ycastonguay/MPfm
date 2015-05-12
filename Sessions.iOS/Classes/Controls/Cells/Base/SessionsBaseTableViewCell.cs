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
	[Register("SessionsBaseTableViewCell")]
	public abstract class SessionsBaseTableViewCell : UITableViewCell
    {
        protected UIView BackgroundView { get; private set; }
        public UIView ContainerView { get; private set; }
        public UIView ContainerBackgroundView { get; private set; }
        public UIView BehindView { get; private set; }

        public bool IsTextAnimationEnabled { get; set; }
        protected bool IsTextLabelAllowedToChangeFrame { get; set; }

        public abstract bool UseContainerView { get; }

        public SessionsBaseTableViewCell() : base()
        {
            Initialize();
        }

        public SessionsBaseTableViewCell(IntPtr handle) : base(handle)
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
            Accessory = UITableViewCellAccessory.None;
            SelectionStyle = UITableViewCellSelectionStyle.None;
            BackgroundColor = UIColor.Clear;

            IsTextLabelAllowedToChangeFrame = true;
        
            BackgroundView = new UIView(Frame);
            BackgroundView.BackgroundColor = UIColor.Clear;
            AddSubview(BackgroundView);

            if (UseContainerView)
            {
                BehindView = new UIView(Bounds);
                BehindView.BackgroundColor = UIColor.FromRGB(47, 129, 183);
                AddSubview(BehindView);

                ContainerView = new UIView(Bounds);
                ContainerView.BackgroundColor = UIColor.White;
                AddSubview(ContainerView);

                ContainerBackgroundView = new UIView(Bounds);
                ContainerBackgroundView.BackgroundColor = UIColor.Clear;
                ContainerView.AddSubview(ContainerBackgroundView);
            }

            var backViewSelected = new UIView(Frame);
            backViewSelected.BackgroundColor = GlobalTheme.SecondaryColor;
            SelectedBackgroundView = backViewSelected;     
            SelectedBackgroundView.Hidden = true;
            AddView(SelectedBackgroundView);
        }

        public void AddView(UIView view)
        {
            if(UseContainerView)
                ContainerView.AddSubview(view);
            else
                AddSubview(view);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if(BehindView != null)
                BehindView.Frame = Bounds;

            if(ContainerView != null)
                ContainerView.Frame = Bounds;

            BackgroundView.Frame = Bounds;
            SelectedBackgroundView.Frame = Bounds;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            AnimatePress(true);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);
            base.TouchesCancelled(touches, evt);
        }

        protected virtual void AnimatePress(bool on)
        {
            if (!IsTextAnimationEnabled)
                return;

            IsTextLabelAllowedToChangeFrame = !on;

            if (!on)
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    // Ignore when scale is lower; it was done on purpose and will be restored to 1 later.
                    //if(TitleTextLabel.Transform.xx < 0.95f) return; // Do we still need this?

                    SetControlScaleForTouchAnimation(1);
                }, null);
            }
            else
            {
                UIView.Animate(0.1, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                    SetControlScaleForTouchAnimation(0.96f);
                }, null);
            }
        }

        protected abstract void SetControlScaleForTouchAnimation(float scale);
    }
}
