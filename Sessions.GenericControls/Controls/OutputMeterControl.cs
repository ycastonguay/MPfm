// Copyright � 2011-2013 Yanick Castonguay
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
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Base;
using Sessions.GenericControls.Controls.Interfaces;
using Sessions.GenericControls.Graphics;
using Sessions.Sound.AudioFiles;

namespace Sessions.GenericControls.Controls
{
    public class OutputMeterControl : ControlBase
    {
		private List<WaveDataMinMax> _waveDataHistory;
        private BasicBrush _brushBackground;
        private BasicGradientBrush _brushBarLeft;
        private BasicGradientBrush _brushBarRight;
        private BasicPen _penTransparent;
        private BasicPen _pen0dBLine;
        private BasicPen _penPeakLine;

        /// <summary>
        /// Output meter display type (left channel, right channel, stereo, or mix).
        /// </summary>
		public OutputMeterDisplayType DisplayType { get; set; }

        /// <summary>
        /// Value used to determine if the signal is distorting. Value range: 0.0f to 1.0f.
        /// </summary>
		public float DistortionThreshold { get; set; }

        /// <summary>
        /// Display the peak (1000ms) in decibels in text at the bottom of each bar.
        /// </summary>
		public bool DisplayDecibels { get; set; }

        /// <summary>
        /// Floor from which the output meter will be drawn. Minimum value: -100 (dB). Max: 0 (dB). Default value: -60 (dB).
        /// </summary>
		public float DrawFloor { get; set; }

		public float FontSize { get; set; }
		public string FontFace { get; set; }
		public BasicColor ColorBackground { get { return new BasicColor(32, 40, 46); } }
        public BasicColor ColorMeter1 { get { return new BasicColor(0, 125, 0); } }
        public BasicColor ColorMeter2 { get { return new BasicColor(0, 200, 0); } }
        public BasicColor ColorMeterB1 { get { return new BasicColor(0, 150, 0); } }
        public BasicColor ColorMeterB2 { get { return new BasicColor(0, 225, 0); } }
        //public BasicColor ColorMeter1 { get { return new BasicColor(255, 5, 255); } }
        //public BasicColor ColorMeter2 { get { return new BasicColor(0, 255, 0); } }
        //public BasicColor ColorMeterB1 { get { return new BasicColor(0, 50, 255); } }
        //public BasicColor ColorMeterB2 { get { return new BasicColor(255, 25, 255); } }
        public BasicColor ColorMeterDistortion1 { get { return new BasicColor(1, 0, 0); } }
        public BasicColor ColorMeterDistortion2 { get { return new BasicColor(220, 0, 0); } }
        public BasicColor Color0dBLine { get { return new BasicColor(225, 225, 225); } }
        public BasicColor ColorPeakLine { get { return new BasicColor(0, 225, 0); } }

        public OutputMeterControl()
        {
            _waveDataHistory = new List<WaveDataMinMax>();
			Initialize();
        }

		private void Initialize()
		{
			DisplayType = OutputMeterDisplayType.Stereo;
			DistortionThreshold = 0.9f;
			DrawFloor = -60f;
			DisplayDecibels = true;
			FontFace = "Roboto Condensed";
			FontSize = 10;

		    CreateDrawingResources();
		}

        private void CreateDrawingResources()
        {
            _brushBackground = new BasicBrush(ColorBackground);
            _penTransparent = new BasicPen();
            _pen0dBLine = new BasicPen(new BasicBrush(Color0dBLine), 1);
            _penPeakLine = new BasicPen(new BasicBrush(ColorPeakLine), 1);
        }

        public void Reset()
        {
            _waveDataHistory.Clear();
            InvalidateVisual();
        }

        /// <summary>
        /// Block of 10ms synchronized with timerelapsed on Player.
        /// </summary>
        /// <param name="waveDataLeft"></param>
        /// <param name="waveDataRight"></param>
        public void AddWaveDataBlock(float[] waveDataLeft, float[] waveDataRight)
        {
            AddToHistory(AudioTools.GetMinMaxFromWaveData(waveDataLeft, waveDataRight, true));
            InvalidateVisual();
        }

        /// <summary>
        /// Adds a min/max wave data structure to the history.
        /// </summary>
        /// <param name="data">Min/max wave data structure</param>
        private void AddToHistory(WaveDataMinMax data)
        {
			if (_waveDataHistory.Count == 0)
            {
				_waveDataHistory.Add(data);
            }
            else
            {
				_waveDataHistory.Insert(0, data);

                // Trim history larger than 1000ms (100 items * 10ms)
				if (_waveDataHistory.Count > 100)
					_waveDataHistory.RemoveAt(_waveDataHistory.Count - 1);
            }
        }

        public override void Render(IGraphicsContext context)
        {
            base.Render(context);

            // Note: creating the gradient brush in advance means the output meter cannot change size or the gradient won't fit the new control size
            _brushBarLeft = new BasicGradientBrush(ColorMeter1, ColorMeter2, new BasicPoint(0, 0), new BasicPoint(context.BoundsWidth / 2, context.BoundsHeight));
            _brushBarRight = new BasicGradientBrush(ColorMeterB1, ColorMeterB2, new BasicPoint(0, 0), new BasicPoint(context.BoundsWidth / 2, context.BoundsHeight));

            context.DrawRectangle(new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight), _brushBackground, _penTransparent);

            // If the wave data is empty, skip rendering 
			if (_waveDataHistory == null || _waveDataHistory.Count == 0)
                return;

            float fontSize = context.BoundsWidth < 50 ? 8 : 10;

            // By default, the bar width takes the full width of the control (except for stereo)
            float barWidth = context.BoundsWidth;

            // Set bar width to half width since there's two bars to draw
            barWidth = context.BoundsWidth / 2;

            // at 10ms refresh, get last value.
			float maxLeftDB = 20.0f * (float)Math.Log10(_waveDataHistory[0].leftMax);
			float maxRightDB = 20.0f * (float)Math.Log10(_waveDataHistory[0].rightMax);
            //float maxLeftDB2 = (float)Base.LevelToDB_16Bit((double)WaveDataHistory[0].leftMax);
            //float maxRightDB2 = (float)Base.LevelToDB_16Bit((double)WaveDataHistory[0].rightMax);

            // Get peak for the last 1000ms
			float peakLeftDB = AudioTools.GetMaxdBPeakFromWaveDataMaxHistory(_waveDataHistory, 100, ChannelType.Left);
			float peakRightDB = AudioTools.GetMaxdBPeakFromWaveDataMaxHistory(_waveDataHistory, 100, ChannelType.Right);

            // Set the dB range to display (-100 to +10dB)
            float dbRangeToDisplay = 110;

            // Get multiplier (110 height to 330 == 3)
            float scaleMultiplier = context.BoundsHeight / dbRangeToDisplay;

            // Get bar height -- If value = -100 then 0. If value = 0 then = 100. if value = 10 then = 110.
            //float barHeight = scaleMultiplier * (maxDB + 100);

            // Draw 0db line
            context.DrawLine(new BasicPoint(0, 4), new BasicPoint(context.BoundsWidth, 4), _pen0dBLine);

            // -----------------------------------------
            // LEFT CHANNEL
            //

            // Get the VU value from audio tools
            //float vuLeft = AudioTools.GetVUMeterValue(WaveDataHistory, 100, ChannelType.Left);

            // Calculate bar height
            float barHeight = scaleMultiplier * (maxLeftDB + 100);
            float height = barHeight;
            if (height < 1)
                height = 1;

            // Create rectangle for bar
            var rect = new BasicRectangle(0, context.BoundsHeight - barHeight, barWidth, height);
            // Check for distortion
            //if (maxLeftDB >= 0.2f)
            //{
            //    gradient = theme.MeterDistortionGradient;
            //}
            context.DrawRectangle(rect, _brushBarLeft, _penTransparent);

            // Draw peak line
            var pt1 = new BasicPoint(0, context.BoundsHeight - (scaleMultiplier * (peakLeftDB + 100)));
            var pt2 = new BasicPoint(barWidth, context.BoundsHeight - (scaleMultiplier * (peakLeftDB + 100)));
            context.DrawLine(pt1, pt2, _penPeakLine);

            // Draw number of db      
            string strDB = peakLeftDB.ToString("00.0").Replace(",", ".");
            if (maxLeftDB == -100.0f)
                strDB = "-inf";

            // Draw text
            // TODO: Measuring text is CPU intensive, all this to center the value... can we draw something centered without measuring?
			var rectText = context.MeasureText(strDB, new BasicRectangle(), FontFace, FontSize);
            float newX = (barWidth - rectText.Width) / 2;
			context.DrawText(strDB, new BasicPoint(newX + 1, context.BoundsHeight - rectText.Height - 4), new BasicColor(20, 20, 20), FontFace, FontSize);
			context.DrawText(strDB, new BasicPoint(newX, context.BoundsHeight - rectText.Height - 4 - 1), new BasicColor(255, 255, 255), FontFace, FontSize);

            // -----------------------------------------
            // RIGHT CHANNEL
            //

            // Calculate bar height
            barHeight = scaleMultiplier * (maxRightDB + 100);
            height = barHeight;
            if (height < 1)
                height = 1;

            // Create rectangle for bar                
            rect = new BasicRectangle(barWidth, context.BoundsHeight - barHeight, barWidth, height);
            // Check for distortion
            //if (maxLeftDB >= 0.2f)
            //    gradient = theme.MeterDistortionGradient;
            context.DrawRectangle(rect, _brushBarRight, _penTransparent);

            // Draw number of db      
            strDB = peakRightDB.ToString("00.0").Replace(",", ".");
            if (maxRightDB == -100.0f)
                strDB = "-inf";

            // Draw peak line
            pt1 = new BasicPoint(barWidth, context.BoundsHeight - (scaleMultiplier * (peakRightDB + 100)));
            pt2 = new BasicPoint(barWidth * 2, context.BoundsHeight - (scaleMultiplier * (peakRightDB + 100)));
            context.DrawLine(pt1, pt2, _penPeakLine);

            // Draw number of decibels (with font shadow to make it easier to read)                
			rectText = context.MeasureText(strDB, new BasicRectangle(), FontFace, FontSize);
            newX = ((barWidth - rectText.Width) / 2) + barWidth;
			context.DrawText(strDB, new BasicPoint(newX + 1, context.BoundsHeight - rectText.Height - 4), new BasicColor(20, 20, 20), FontFace, FontSize);
			context.DrawText(strDB, new BasicPoint(newX, context.BoundsHeight - rectText.Height - 4 - 1), new BasicColor(255, 255, 255), FontFace, FontSize);
        }

        /// <summary>
        /// Defines the display type of the output meter.
        /// </summary>
        public enum OutputMeterDisplayType
        {
            /// <summary>
            /// Displays only the left channel.
            /// </summary>
            LeftChannel = 0,
            /// <summary>
            /// Displays only the right channel.
            /// </summary>
            RightChannel = 1,
            /// <summary>
            /// Displays the left and right channel.
            /// </summary>
            Stereo = 2,
            /// <summary>
            /// Displays only one channel (mix of left/right channels).
            /// </summary>
            Mix = 3
        }
    }
}