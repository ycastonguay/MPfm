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
using System.Linq;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.Sound;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.iOS.Managers;
using MPfm.iOS.Managers.Events;

namespace MPfm.iOS.Classes.Controls
{
    /// <summary>
    /// This output meter control takes raw audio data and displays the current level of mono or stereo channels.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    [Register("MPfmOutputMeterView")]
    public class MPfmOutputMeterView : UIView
    {
        private List<WaveDataMinMax> waveDataHistory = null;
        /// <summary>
        /// Array containing an history of min and max peaks over the last 1000ms.
        /// </summary>
        public List<WaveDataMinMax> WaveDataHistory
        {
            get
            {
                return waveDataHistory;
            }
        }

        private OutputMeterDisplayType displayType = OutputMeterDisplayType.Stereo;
        /// <summary>
        /// Output meter display type (left channel, right channel, stereo, or mix).
        /// </summary>
        public OutputMeterDisplayType DisplayType
        {
            get
            {
                return displayType;
            }
            set
            {
                displayType = value;
            }
        }

        private float distortionThreshold = 0.9f;
        /// <summary>
        /// Value used to determine if the signal is distorting. Value range: 0.0f to 1.0f.
        /// </summary>
        public float DistortionThreshold
        {
            get
            {
                return distortionThreshold;
            }
            set
            {
                distortionThreshold = value;
            }
        }

        private bool displayDecibels = true;
        /// <summary>
        /// Display the peak (1000ms) in decibels in text at the bottom of each bar.
        /// </summary>
        public bool DisplayDecibels
        {
            get
            {
                return displayDecibels;
            }
            set
            {
                displayDecibels = value;
            }
        }

        private float drawFloor = -60f;
        /// <summary>
        /// Floor from which the output meter will be drawn. Minimum value: -100 (dB). Max: 0 (dB). Default value: -60 (dB).
        /// </summary>
        public float DrawFloor
        {
            get
            {
                return drawFloor;
            }
            set
            {
                drawFloor = value;
            }
        }

        private CGColor _colorMeter1 = new CGColor(0, 0.5f, 0);
        private CGColor _colorMeter2 = new CGColor(0, 0.7f, 0);
        private CGColor _colorMeterB1 = new CGColor(0, 0.525f, 0);
        private CGColor _colorMeterB2 = new CGColor(0, 0.725f, 0);
        private CGColor _colorMeterDistortion1 = new CGColor(1, 0, 0);
        private CGColor _colorMeterDistortion2 = new CGColor(0.8f, 0, 0);
        private CGColor _color0dBLine = UIColor.FromRGBA(0.9059f, 0.2980f, 0.2353f, 0.5f).CGColor;
        private CGColor _colorPeakLine = UIColor.Yellow.CGColor;

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
            waveDataHistory = new List<WaveDataMinMax>();
            this.BackgroundColor = UIColor.Black;
        }

        /// <summary>
        /// Block of 10ms synchronized with timerelapsed on Player.
        /// </summary>
        /// <param name="waveDataLeft"></param>
        /// <param name="waveDataRight"></param>
        public void AddWaveDataBlock(float[] waveDataLeft, float[] waveDataRight)
        {
            AddToHistory(AudioTools.GetMinMaxFromWaveData(waveDataLeft, waveDataRight, true));
        }

        /// <summary>
        /// Adds a min/max wave data structure to the history.
        /// </summary>
        /// <param name="data">Min/max wave data structure</param>
        private void AddToHistory(WaveDataMinMax data)
        {
            // Add history
            if (WaveDataHistory.Count == 0)
            {
                WaveDataHistory.Add(data);
            }
            else
            {
                WaveDataHistory.Insert(0, data);

                // Trim history larger than 1000ms (100 items * 10ms)
                if (WaveDataHistory.Count > 100)
                {
                    WaveDataHistory.RemoveAt(WaveDataHistory.Count - 1);
                }
            }
        }

        public override void Draw(RectangleF rectToDraw)
        {
            var context = UIGraphics.GetCurrentContext();
			CoreGraphicsHelper.FillRect(context, Bounds, BackgroundColor.CGColor);

            // If the wave data is empty, skip rendering 
            if (WaveDataHistory == null || WaveDataHistory.Count == 0)
                return;

            // By default, the bar width takes the full width of the control (except for stereo)
            float barWidth = Bounds.Width;

            // Set bar width to half width since there's two bars to draw
            barWidth = Bounds.Width / 2;

            // at 10ms refresh, get last value.
            float maxLeftDB = 20.0f * (float)Math.Log10(WaveDataHistory[0].leftMax);
            float maxRightDB = 20.0f * (float)Math.Log10(WaveDataHistory[0].rightMax);
            //float maxLeftDB2 = (float)Base.LevelToDB_16Bit((double)WaveDataHistory[0].leftMax);
            //float maxRightDB2 = (float)Base.LevelToDB_16Bit((double)WaveDataHistory[0].rightMax);

            // Get peak for the last 1000ms
            float peakLeftDB = AudioTools.GetMaxdBPeakFromWaveDataMaxHistory(WaveDataHistory, 100, ChannelType.Left);
            float peakRightDB = AudioTools.GetMaxdBPeakFromWaveDataMaxHistory(WaveDataHistory, 100, ChannelType.Right);

            // Set the dB range to display (-100 to +10dB)
            float dbRangeToDisplay = 110;

            // Get multiplier (110 height to 330 == 3)
            float scaleMultiplier = Bounds.Height / dbRangeToDisplay;

            // Get bar height -- If value = -100 then 0. If value = 0 then = 100. if value = 10 then = 110.
            //float barHeight = scaleMultiplier * (maxDB + 100);

            // Draw 0db line
            CoreGraphicsHelper.DrawLine(context, new List<PointF>(){
                new PointF(0, 4), 
                new PointF(Bounds.Width, 4)
            }, _color0dBLine, 1, false, false);

            // -----------------------------------------
            // LEFT CHANNEL
            //

            // Get the VU value from audio tools
            //float vuLeft = AudioTools.GetVUMeterValue(WaveDataHistory, 100, ChannelType.Left);

            // Calculate bar height
            float barHeight = maxLeftDB + 100;
            float height = barHeight;
            if (height < 1)
                height = 1;

            // Create rectangle for bar
            RectangleF rect = new RectangleF(0, Bounds.Height - barHeight, barWidth, height);
            //RectangleF rectGradient = new RectangleF(0, Bounds.Height, barWidth, height);
            //BackgroundGradient gradient = theme.MeterGradient;
            // Check for distortion
            //if (maxLeftDB >= 0.2f)
            //{
            //    gradient = theme.MeterDistortionGradient;
            //}
            //CoreGraphicsHelper.FillRect(context, rect, _colorMeter1);
            CoreGraphicsHelper.FillGradient(context, rect, _colorMeter1, _colorMeter2);

            // Draw peak line
            CoreGraphicsHelper.DrawLine(context, new List<PointF>(){
                new PointF(0, Bounds.Height - (peakLeftDB + 100)), 
                new PointF(barWidth, Bounds.Height - (peakLeftDB + 100))
            }, _colorPeakLine, 1, false, false);

            // Draw number of db      
            string strDB = peakLeftDB.ToString("00.0").Replace(",",".");
            if (maxLeftDB == -100.0f)
                strDB = "-inf";

            // Draw text
            SizeF sizeString = CoreGraphicsHelper.MeasureText(context, strDB, "HelveticaNeue-CondensedBold", 10);
            float newX = (barWidth - sizeString.Width) / 2;
//            RectangleF rectBackgroundText = new RectangleF(newX, Bounds.Height - sizeString.Height - 4, sizeString.Width, sizeString.Height);
//            rectBackgroundText.Inflate(new SizeF(2, 0));
//            CoreGraphicsHelper.FillRect(context, rectBackgroundText, new CGColor(0.1f, 0.1f, 0.1f, 0.25f));
            CoreGraphicsHelper.DrawTextAtPoint(context, new PointF(newX + 1, Bounds.Height - sizeString.Height - 4), strDB, "HelveticaNeue-CondensedBold", 10, new CGColor(0.1f, 0.1f, 0.1f, 0.2f));
            CoreGraphicsHelper.DrawTextAtPoint(context, new PointF(newX, Bounds.Height - sizeString.Height - 4 - 1), strDB, "HelveticaNeue-CondensedBold", 10, new CGColor(1, 1, 1));

            // -----------------------------------------
            // RIGHT CHANNEL
            //

            // Calculate bar height
            barHeight = maxRightDB + 100;
            height = barHeight;
            if (height < 1)
                height = 1;

            // Create rectangle for bar                
            rect = new RectangleF(barWidth, Bounds.Height - barHeight, barWidth, height);
            // Check for distortion
            //if (maxLeftDB >= 0.2f)
            //    gradient = theme.MeterDistortionGradient;
            CoreGraphicsHelper.FillGradient(context, rect, _colorMeter1, _colorMeter2);

            // Draw number of db      
            strDB = peakRightDB.ToString("00.0").Replace(",",".");
            if (maxRightDB == -100.0f)
                strDB = "-inf";

            // Draw peak line
            CoreGraphicsHelper.DrawLine(context, new List<PointF>(){
                new PointF(barWidth, Bounds.Height - (peakRightDB + 100)), 
                new PointF(barWidth * 2, Bounds.Height - (peakRightDB + 100))                
            }, _colorPeakLine, 1, false, false);

            // Draw number of decibels (with font shadow to make it easier to read)                
            sizeString = CoreGraphicsHelper.MeasureText(context, strDB, "HelveticaNeue-Bold", 10);
            newX = ((barWidth - sizeString.Width) / 2) + barWidth;
//            rectBackgroundText = new RectangleF(newX, Bounds.Height - sizeString.Height - 4, sizeString.Width, sizeString.Height);
//            rectBackgroundText.Inflate(new SizeF(2, 0));
//            CoreGraphicsHelper.FillRect(context, rectBackgroundText, new CGColor(0.1f, 0.1f, 0.1f, 0.25f));
            CoreGraphicsHelper.DrawTextAtPoint(context, new PointF(newX + 1, Bounds.Height - sizeString.Height - 4), strDB, "HelveticaNeue-CondensedBold", 10, new CGColor(0.1f, 0.1f, 0.1f, 0.2f));
            CoreGraphicsHelper.DrawTextAtPoint(context, new PointF(newX, Bounds.Height - sizeString.Height - 4 - 1), strDB, "HelveticaNeue-CondensedBold", 10, new CGColor(1, 1, 1));
        }
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
