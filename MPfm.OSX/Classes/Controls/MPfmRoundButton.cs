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
using System.IO;
using System.Reflection;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using MPfm.Mac.Classes.Helpers;
using MPfm.Mac.Classes.Objects;
using MonoMac.CoreImage;
using MonoMac.CoreAnimation;

namespace MPfm.Mac.Classes.Controls
{
    [Register("MPfmRoundButton")]
    public class MPfmRoundButton : NSButton
    {
        bool _isAnimating = false;
        bool _isMouseDown = false;
        bool _isMouseOver = false;
        CAShapeLayer _layerCircle;

        public CGColor TextColor { get; set; }
        public CGColor BackgroundColor { get; set; }
        public CGColor BackgroundMouseDownColor { get; set; }
        public CGColor BackgroundMouseOverColor { get; set; }
        public CGColor BorderColor { get; set; }
        public CGColor MouseDownBorderColor { get; set; }
        public CGColor MouseOverBorderColor { get; set; }
        public NSImageView ImageView { get; private set; }

        public delegate void ButtonSelected(MPfmRoundButton button);
        public event ButtonSelected OnButtonSelected;

        [Export("init")]
        public MPfmRoundButton() : base(NSObjectFlag.Empty)
        {
            Initialize();
        }

        // Called when created from unmanaged code
        public MPfmRoundButton(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            //Layer.CornerRadius = 8; // Crashes the app. Bug in MonoMac?
            //BezelStyle = NSBezelStyle.Rounded;
            //Cell.BezelStyle = NSBezelStyle.Rounded;
            TextColor = GlobalTheme.ButtonTextColor;
            BackgroundColor = GlobalTheme.ButtonMainToolbarBackgroundColor;
            BackgroundMouseDownColor = GlobalTheme.ButtonMainToolbarBackgroundMouseDownColor;
            BackgroundMouseOverColor = GlobalTheme.ButtonMainToolbarBackgroundMouseOverColor;
            MouseDownBorderColor = GlobalTheme.ButtonMainToolbarMouseDownBorderColor;
            MouseOverBorderColor = GlobalTheme.ButtonMainToolbarMouseOverBorderColor;
            BorderColor = GlobalTheme.ButtonMainToolbarBorderColor;

            // On OS X, you need to create the layer manually (unlike iOS)
            var layer = new CALayer();
            this.Layer = layer;
            WantsLayer = true;
            //Layer.BackgroundColor = new CGColor(0, 1, 0);
            Layer.BackgroundColor = new CGColor(0, 0);

            float padding = 2; // If the circle stays at (0,0) the curves lines get slightly cut
            float radius = (Bounds.Width / 2);// - (Bounds.Width / 8);
            _layerCircle = new CAShapeLayer();
            //_layerCircle.AllowsEdgeAntialiasing = true;
            _layerCircle.EdgeAntialiasingMask = CAEdgeAntialiasingMask.All;
            _layerCircle.Bounds = Bounds;
            //_layerCircle.Path = NSBezierPath.FromRoundedRect(new RectangleF(0, 0, 2f * radius, 2f * radius), radius).CGPath;
            _layerCircle.Path = new CGPath();
            _layerCircle.Path.AddElipseInRect(new RectangleF(padding, padding, (2f * radius) - (padding * 2), (2f * radius) - (padding * 2))); 
            _layerCircle.Position = new PointF(Bounds.Width / 2, Bounds.Height / 2);
            _layerCircle.FillColor = BackgroundColor;
            _layerCircle.StrokeColor = BorderColor;
            _layerCircle.LineWidth = 2f;
            Layer.AddSublayer(_layerCircle);

            float imageSize = 50; // TODO: Maybe make a property out of this? The issue is that we might want to keep the same image size but vary button size
            ImageView = new NSImageView();
            ImageView.Frame = new RectangleF((Frame.Width - imageSize) / 2, (Frame.Height - imageSize) / 2, imageSize, imageSize);
            AddSubview(ImageView);

            // This allows MouseEntered and MouseExit to work
            AddTrackingRect(Bounds, this, IntPtr.Zero, false);
        }

        [Export("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            _isMouseDown = true;
            _layerCircle.FillColor = BackgroundMouseDownColor;
            _layerCircle.StrokeColor = MouseDownBorderColor;
            //AnimatePress(true);
            base.MouseDown(theEvent);
            this.MouseUp(theEvent);
        }

        [Export("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            _isMouseDown = false;
            _layerCircle.FillColor = BackgroundMouseOverColor;
            _layerCircle.StrokeColor = MouseOverBorderColor;
            SetNeedsDisplay();
            if(OnButtonSelected != null)
                OnButtonSelected(this);
        }

        [Export("mouseEntered:")]
        public override void MouseEntered(NSEvent theEvent)
        {
            base.MouseEntered(theEvent);
            _isMouseOver = true;
            _layerCircle.FillColor = BackgroundMouseOverColor;
            _layerCircle.StrokeColor = MouseOverBorderColor;
            SetNeedsDisplay();
        }

        [Export("mouseExited:")]
        public override void MouseExited(NSEvent theEvent)
        {
            base.MouseExited(theEvent);
            _isMouseOver = false;
            _layerCircle.FillColor = BackgroundColor;
            _layerCircle.StrokeColor = BorderColor;
            SetNeedsDisplay();
        }

//        private void AnimatePress(bool on)
//        {
//            NSAnimationContext.BeginGrouping();
//            NSAnimationContext.CurrentContext.Duration = 2;
//            //ImageView.Animator.AlphaValue = 0;
//            //var stuff = ImageView.Animator;
//            //var stuff2 = stuff as NSImageView;
//            var imageViewAnimator = ImageView.Animator as NSImageView;
//            imageViewAnimator.AlphaValue = 0;
//            //ImageView.Layer.AffineTransform = CGAffineTransform.MakeScale(0.8f, 0.8f);
//            //imageViewAnimator.Layer.AffineTransform = CGAffineTransform.MakeScale(0.8f, 0.8f);
//            NSAnimationContext.EndGrouping();
//
//            CATransaction.Begin();
//            CATransaction.AnimationDuration = 1;
//            ImageView.Layer.Transform = CATransform3D.MakeScale(0.8f, 0.8f, 0);
//            CATransaction.Commit();
//
//
////            NSAnimationContext.RunAnimation((context) =>
////                {
////                    context.Duration = 2;
////                    _isAnimating = true;
////                    ImageView.Layer.AffineTransform = CGAffineTransform.MakeScale(0.8f, 0.8f);
////                    //ImageView.Alpha = 0.7f;
////                }, () =>
////                {
////                    _isAnimating = false;
////                });
//            return;
//
        //            }
        //}
    }
}
