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
using Sessions.GenericControls.Controls;
using MPfm.iOS.Classes.Controls.Graphics;
using MPfm.iOS.Classes.Controls.Helpers;

namespace MPfm.iOS.Classes.Controls
{
    /// <summary>
    /// This output meter control takes raw audio data and displays the current level of mono or stereo channels.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    [Register("MPfmOutputMeterView")]
    public class MPfmOutputMeterView : UIView
    {
		private OutputMeterControl _control;

        public MPfmOutputMeterView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public MPfmOutputMeterView(RectangleF frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
			_control = new OutputMeterControl();
			_control.FontFace = "HelveticaNeue-CondensedBold";
			_control.OnInvalidateVisual += () => InvokeOnMainThread(SetNeedsDisplay);
			_control.OnInvalidateVisualInRect += (rect) => InvokeOnMainThread(() => SetNeedsDisplayInRect(GenericControlHelper.ToRect(rect)));
            BackgroundColor = UIColor.Black;
        }

        public void AddWaveDataBlock(float[] waveDataLeft, float[] waveDataRight)
        {
			_control.AddWaveDataBlock(waveDataLeft, waveDataRight);
        }

		public override void Draw(RectangleF rect)
		{
            var context = UIGraphics.GetCurrentContext();
			var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height, GenericControlHelper.ToBasicRect(rect));
			_control.FontSize = Bounds.Width < 50 ? 8 : 10;
			_control.Render(wrapper);
		}
    }
}
