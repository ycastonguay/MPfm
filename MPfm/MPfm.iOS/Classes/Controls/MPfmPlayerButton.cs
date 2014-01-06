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
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MonoTouch.CoreAnimation;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmPlayerButton")]
    public class MPfmPlayerButton : UIButton
    {
		private bool _isAnimating;
        private CAShapeLayer _layerCircle;

        public UIImageView GlyphImageView { get; private set; }
		public float GlyphAlpha { get; private set; }
		public float GlyphAlphaOn { get; private set; }
		public UIColor FillColor { get; private set; }
		public UIColor FillColorOn { get; private set; }
		public UIColor StrokeColor { get; private set; }
		public UIColor StrokeColorOn { get; private set; }

        public delegate void ButtonClick();
        public event ButtonClick OnButtonClick;

		public MPfmPlayerButton() 
            : base()
        {
            Initialize();
        }

		public MPfmPlayerButton(IntPtr handle) 
            : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
			SetDefaultTheme();
            TintColor = UIColor.White;
			Layer.BackgroundColor = UIColor.Clear.CGColor;

            float radius = Bounds.Width / 2;
            _layerCircle = new CAShapeLayer();
            _layerCircle.AllowsEdgeAntialiasing = true;
            _layerCircle.Bounds = Bounds;
            _layerCircle.Path = UIBezierPath.FromRoundedRect(new RectangleF(0, 0, 2f * radius, 2f * radius), radius).CGPath;
            _layerCircle.Position = new PointF(Bounds.Width / 2, Bounds.Height / 2);
			_layerCircle.FillColor = FillColor.CGColor;
			_layerCircle.StrokeColor = StrokeColor.CGColor;
            _layerCircle.LineWidth = 1f;
            Layer.AddSublayer(_layerCircle);

            GlyphImageView = new UIImageView();
            GlyphImageView.BackgroundColor = UIColor.Clear;
            GlyphImageView.Layer.AnchorPoint = new PointF(0.5f, 0.5f);
            GlyphImageView.Frame = new RectangleF((Frame.Width - 50) / 2, (Frame.Height - 50) / 2, 50, 50);
			GlyphImageView.Alpha = GlyphAlpha;

            TitleLabel.Alpha = 0;
            TitleLabel.Hidden = true;
            TitleLabel.Text = string.Empty;

            AddSubview(GlyphImageView);
        }

		private void SetDefaultTheme()
		{
			SetTheme(GlobalTheme.BackgroundColor, GlobalTheme.MainLightColor, GlobalTheme.MainColor, GlobalTheme.LightColor.ColorWithAlpha(0.5f), 0.7f, 1);
		}

		public void SetTheme(UIColor fillColor, UIColor strokeColor, UIColor fillColorOn, UIColor strokeColorOn, float glyphAlpha, float glyphAlphaOn)
		{
			FillColor = fillColor;
			StrokeColor = strokeColor;
			FillColorOn = fillColorOn;
			StrokeColorOn = strokeColorOn;
			GlyphAlpha = glyphAlpha;
			GlyphAlphaOn = glyphAlphaOn;

			if (_layerCircle == null)
				return;

			_layerCircle.FillColor = FillColor.CGColor;
			_layerCircle.StrokeColor = StrokeColor.CGColor;
			GlyphImageView.Alpha = GlyphAlpha;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			float radius = Bounds.Width / 2;
			_layerCircle.Path = UIBezierPath.FromRoundedRect(new RectangleF(0, 0, 2f * radius, 2f * radius), radius).CGPath;

			//if(!_isAnimating)
			//	GlyphImageView.Frame = new RectangleF((Frame.Width - 50) / 2, (Frame.Height - 50) / 2, 50, 50);
		}

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            AnimatePress(true);
            base.TouchesBegan(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);

            if (OnButtonClick != null)
                OnButtonClick();

            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            AnimatePress(false);
            base.TouchesCancelled(touches, evt);
        }

        private void AnimatePress(bool on)
        {
            if (!on)
            {
                UIView.Animate(0.1, () => {
					_isAnimating = true;
                    GlyphImageView.Transform = CGAffineTransform.MakeScale(1, 1);
                    GlyphImageView.Alpha = 0.7f;
				}, () => _isAnimating = false);
                UIView.Animate(0.05, () => {
					_layerCircle.FillColor = FillColor.CGColor;
					_layerCircle.StrokeColor = StrokeColor.CGColor;
                    _layerCircle.LineWidth = 1f;
                });
            }
            else
            {
                UIView.Animate(0.05, () => {
					_isAnimating = true;
                    GlyphImageView.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                    GlyphImageView.Alpha = 1f;
				}, () => _isAnimating = false);
                UIView.Animate(0.025, () => {
					_layerCircle.FillColor = FillColorOn.CGColor;
					_layerCircle.StrokeColor = StrokeColorOn.CGColor;
                    _layerCircle.LineWidth = 1.5f;
                });
            }
        }
    }
}
