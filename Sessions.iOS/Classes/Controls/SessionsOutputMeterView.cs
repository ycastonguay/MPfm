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
using CoreGraphics;
using CoreGraphics;
using Foundation;
using UIKit;
using Sessions.GenericControls.Controls;
using Sessions.iOS.Classes.Controls.Graphics;
using Sessions.iOS.Classes.Controls.Helpers;

namespace Sessions.iOS.Classes.Controls
{
    /// <summary>
    /// This output meter control takes raw audio data and displays the current level of mono or stereo channels.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    [Register("SessionsOutputMeterView")]
    public class SessionsOutputMeterView : UIView
    {
		private OutputMeterControl _control;

        public SessionsOutputMeterView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public SessionsOutputMeterView(CGRect frame) 
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

		public override void Draw(CGRect rect)
		{
            var context = UIGraphics.GetCurrentContext();
            context.SaveState();
			var wrapper = new GraphicsContextWrapper(context, Bounds.Width, Bounds.Height, GenericControlHelper.ToBasicRect(rect));
			_control.FontSize = Bounds.Width < 50 ? 8 : 10;
			_control.Render(wrapper);
            context.RestoreState();
		}
    }
}
