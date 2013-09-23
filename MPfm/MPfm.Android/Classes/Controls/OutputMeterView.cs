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
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MPfm.Sound.AudioFiles;

namespace org.sessionsapp.android
{
    public class OutputMeterView : View
    {
        private List<WaveDataMinMax> _waveDataHistory = null;
        /// <summary>
        /// Array containing an history of min and max peaks over the last 1000ms.
        /// </summary>
        public List<WaveDataMinMax> WaveDataHistory
        {
            get
            {
                return _waveDataHistory;
            }
        }

        private OutputMeterDisplayType _displayType = OutputMeterDisplayType.Stereo;
        /// <summary>
        /// Output meter display type (left channel, right channel, stereo, or mix).
        /// </summary>
        public OutputMeterDisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }

        private float _distortionThreshold = 0.9f;
        /// <summary>
        /// Value used to determine if the signal is distorting. Value range: 0.0f to 1.0f.
        /// </summary>
        public float DistortionThreshold
        {
            get
            {
                return _distortionThreshold;
            }
            set
            {
                _distortionThreshold = value;
            }
        }

        private bool _displayDecibels = true;
        /// <summary>
        /// Display the peak (1000ms) in decibels in text at the bottom of each bar.
        /// </summary>
        public bool DisplayDecibels
        {
            get
            {
                return _displayDecibels;
            }
            set
            {
                _displayDecibels = value;
            }
        }

        private float _drawFloor = -60f;
        /// <summary>
        /// Floor from which the output meter will be drawn. Minimum value: -100 (dB). Max: 0 (dB). Default value: -60 (dB).
        /// </summary>
        public float DrawFloor
        {
            get
            {
                return _drawFloor;
            }
            set
            {
                _drawFloor = value;
            }
        }

        private Color _colorMeter1 = new Color(0, 125, 0);
        private Color _colorMeter2 = new Color(0, 200, 0);
        private Color _colorMeterB1 = new Color(0, 150, 0);
        private Color _colorMeterB2 = new Color(0, 225, 0);
        private Color _colorMeterDistortion1 = new Color(1, 0, 0);
        private Color _colorMeterDistortion2 = new Color(220, 0, 0);
        private Color _color0dBLine = Color.LightGray;
        private Color _colorPeakLine = Color.Yellow;

        public OutputMeterView(Context context) : base(context)
        {
            Initialize();
        }

        public OutputMeterView(Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Initialize();
        }

        public OutputMeterView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        public OutputMeterView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            Initialize();
        }

        private void Initialize()
        {
            _waveDataHistory = new List<WaveDataMinMax>();

            if (!IsInEditMode)
                SetLayerType(LayerType.Hardware, null); // Use GPU instead of CPU (except in IDE such as Eclipse)
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
                    WaveDataHistory.RemoveAt(WaveDataHistory.Count - 1);
            }
        }

        public override void Draw(global::Android.Graphics.Canvas canvas)
        {
            float density = Resources.DisplayMetrics.Density;

            //Console.WriteLine("OutputMeterView - Draw");
            //base.Draw(canvas);

            //var context = UIGraphics.GetCurrentContext();
            //CoreGraphicsHelper.FillRect(context, Bounds, new CGColor(0.1f, 0.1f, 0.1f));

            // If the wave data is empty, skip rendering 
            if (WaveDataHistory == null || WaveDataHistory.Count == 0)
                return;

            // By default, the bar width takes the full width of the control (except for stereo)
            int barWidth = Width / 2;

            // at 10ms refresh, get last value.
            float maxLeftDB = 20.0f * (float)Math.Log10(WaveDataHistory[0].leftMax);
            float maxRightDB = 20.0f * (float)Math.Log10(WaveDataHistory[0].rightMax);
            //float maxLeftDB2 = (float)Base.LevelToDB_16Bit((double)WaveDataHistory[0].leftMax);
            //float maxRightDB2 = (float)Base.LevelToDB_16Bit((double)WaveDataHistory[0].rightMax);

            // Get peak for the last 1000ms
            float peakLeftDB = AudioTools.GetMaxdBPeakFromWaveDataMaxHistory(WaveDataHistory, 100, ChannelType.Left);
            float peakRightDB = AudioTools.GetMaxdBPeakFromWaveDataMaxHistory(WaveDataHistory, 100, ChannelType.Right);

            // Set the dB range to display (-100 to +10dB)
            //float dbRangeToDisplay = 110;
            float dbRangeToDisplay = 100; // (-100 to 0)

            // Get multiplier (110 height to 330 == 3)
            float scaleMultiplier = Height / dbRangeToDisplay;

            // Draw 0db line
            var paintLine = new Paint {
                AntiAlias = true,
                Color = _color0dBLine
            };
            paintLine.SetStyle(Paint.Style.Fill);
            canvas.DrawLine(0, 4, Width, 4, paintLine);

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
            Rect rect = new Rect(0, Height - (int)barHeight, barWidth, Height);

            //Console.WriteLine("OutputMeterView - DRAW LEFT BAR - ControlHeight: {0} height: {1} barHeight: {2} maxLeftDB: {3} leftMax: {4}", Height, height, barHeight, maxLeftDB, leftMax);

            //RectangleF rectGradient = new RectangleF(0, Bounds.Height, barWidth, height);
            //BackgroundGradient gradient = theme.MeterGradient;
            // Check for distortion
            //if (maxLeftDB >= 0.2f)
            //{
            //    gradient = theme.MeterDistortionGradient;
            //}
            //CoreGraphicsHelper.FillRect(context, rect, _colorMeter1);

            //CoreGraphicsHelper.FillGradient(context, rect, _colorMeter1, _colorMeter2);
            var paintMeter = new Paint {
                AntiAlias = true,
                Color = _colorMeter1
            };
            paintMeter.SetStyle(Paint.Style.Fill);    
            canvas.DrawRect(rect, paintMeter);

            // Draw peak line
            var paintPeakLine = new Paint
            {
                AntiAlias = true,
                Color = _colorPeakLine
            };
            paintPeakLine.SetStyle(Paint.Style.Fill);
            float leftPeakHeight = Height - (scaleMultiplier * (peakLeftDB + 100));
            canvas.DrawLine(0, leftPeakHeight, barWidth, leftPeakHeight, paintPeakLine);

            // Draw decibel value in text
            string strDB = peakLeftDB.ToString("00.0").Replace(",", ".");
            if (maxLeftDB == -100.0f)
                strDB = "-inf";

            var paintText = new Paint
            {
                AntiAlias = true,
                Color = Color.White,
                TextSize = 12 * density
            }; 
            Rect rectText = new Rect();
            paintText.GetTextBounds(strDB, 0, strDB.Length, rectText);
            int newX = (barWidth - rectText.Width()) / 2;
            paintText.Color = new Color(30, 30, 30);
            canvas.DrawText(strDB, newX + 1, Height - rectText.Height() - 4, paintText);
            paintText.Color = Color.White;
            canvas.DrawText(strDB, newX, Height - rectText.Height() - 4 - 1, paintText);

            // -----------------------------------------
            // RIGHT CHANNEL
            //

            // Calculate bar height
            barHeight = scaleMultiplier * (maxRightDB + 100);
            height = barHeight;
            if (height < 1)
                height = 1;

            // Create rectangle for bar                
            rect = new Rect(barWidth, Height - (int)barHeight, barWidth * 2, Height);

            //Console.WriteLine("OutputMeterView - DRAW RIGHT BAR - ControlHeight: {0} height: {1} barHeight: {2} maxRightDB: {3}", Height, height, barHeight, maxRightDB);
            // Check for distortion
            //if (maxLeftDB >= 0.2f)
            //    gradient = theme.MeterDistortionGradient;
            //CoreGraphicsHelper.FillGradient(context, rect, _colorMeter1, _colorMeter2);
            canvas.DrawRect(rect, paintMeter);

            // Draw peak line
            float rightPeakHeight = Height - (scaleMultiplier * (peakRightDB + 100));
            canvas.DrawLine(barWidth, rightPeakHeight, barWidth * 2, rightPeakHeight, paintPeakLine);

            // Draw number of db      
            strDB = peakRightDB.ToString("00.0").Replace(",", ".");
            if (maxRightDB == -100.0f)
                strDB = "-inf";

            rectText = new Rect();
            paintText.GetTextBounds(strDB, 0, strDB.Length, rectText);
            newX = ((barWidth - rectText.Width()) / 2) + barWidth;
            paintText.Color = new Color(30, 30, 30);
            canvas.DrawText(strDB, newX + 1, Height - rectText.Height() - 4, paintText);
            paintText.Color = Color.White;
            canvas.DrawText(strDB, newX, Height - rectText.Height() - 4 - 1, paintText);
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