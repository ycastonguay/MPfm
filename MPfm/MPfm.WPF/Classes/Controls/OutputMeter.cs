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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsControls;
using MPfm.WPF.Classes.Theme;
using Control = System.Windows.Controls.Control;

namespace MPfm.WPF.Classes.Controls
{
    public class OutputMeter : Control
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

        private Color _colorMeter1 = Color.FromRgb(0, 125, 0);
        private Color _colorMeter2 = Color.FromRgb(0, 200, 0);
        private Color _colorMeterB1 = Color.FromRgb(0, 150, 0);
        private Color _colorMeterB2 = Color.FromRgb(0, 225, 0);
        private Color _colorMeterDistortion1 = Color.FromRgb(1, 0, 0);
        private Color _colorMeterDistortion2 = Color.FromRgb(220, 0, 0);
        private Color _color0dBLine = Colors.LightGray;
        private Color _colorPeakLine = Colors.Yellow;

        public OutputMeter()
        {
            Initialize();
        }

        private void Initialize()
        {
            waveDataHistory = new List<WaveDataMinMax>();
            //this.BackgroundColor = UIColor.Black;
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

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(new SolidColorBrush(GlobalTheme.BackgroundColor), new Pen(), new Rect(0, 0, ActualWidth, ActualHeight));

            // If the wave data is empty, skip rendering 
            if (WaveDataHistory == null || WaveDataHistory.Count == 0)
                return;

            float fontSize = ActualWidth < 50 ? 8 : 10;

            // By default, the bar width takes the full width of the control (except for stereo)
            double barWidth = ActualWidth;

            // Set bar width to half width since there's two bars to draw
            barWidth = ActualWidth / 2;

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
            double scaleMultiplier = ActualHeight / dbRangeToDisplay;

            // Get bar height -- If value = -100 then 0. If value = 0 then = 100. if value = 10 then = 110.
            //float barHeight = scaleMultiplier * (maxDB + 100);

            // Draw 0db line
            dc.DrawLine(new Pen(new SolidColorBrush(_color0dBLine), 1), new Point(0, 4), new Point(ActualWidth, 4));

            // -----------------------------------------
            // LEFT CHANNEL
            //

            // Get the VU value from audio tools
            //float vuLeft = AudioTools.GetVUMeterValue(WaveDataHistory, 100, ChannelType.Left);

            // Calculate bar height
            double barHeight = scaleMultiplier * (maxLeftDB + 100);
            double height = barHeight;
            if (height < 1)
                height = 1;

            // Create rectangle for bar
            var rect = new Rect(0, ActualHeight - barHeight, barWidth, height);
            // Check for distortion
            //if (maxLeftDB >= 0.2f)
            //{
            //    gradient = theme.MeterDistortionGradient;
            //}
            dc.DrawRectangle(new LinearGradientBrush(_colorMeter1, _colorMeter2, new Point(0, 0), new Point(ActualWidth / 2, ActualHeight)), new Pen(), rect);

            // Draw peak line
            var pt1 = new Point(0, ActualHeight - (scaleMultiplier*(peakLeftDB + 100)));
            var pt2 = new Point(barWidth, ActualHeight - (scaleMultiplier*(peakLeftDB + 100)));
            dc.DrawLine(new Pen(new SolidColorBrush(_colorPeakLine), 1), pt1, pt2);                        

            // Draw number of db      
            string strDB = peakLeftDB.ToString("00.0").Replace(",", ".");
            if (maxLeftDB == -100.0f)
                strDB = "-inf";

            // Draw text
            var formattedText = new FormattedText(strDB, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Roboto Condensed"), 10, new SolidColorBrush(Color.FromRgb(20, 20, 20)));
            double newX = (barWidth - formattedText.Width) / 2;
            dc.DrawText(formattedText, new Point(newX + 1, ActualHeight - formattedText.Height - 4));
            formattedText.SetForegroundBrush(Brushes.White);
            dc.DrawText(formattedText, new Point(newX, ActualHeight - formattedText.Height - 4 - 1));

            // -----------------------------------------
            // RIGHT CHANNEL
            //

            // Calculate bar height
            barHeight = scaleMultiplier * (maxRightDB + 100);
            height = barHeight;
            if (height < 1)
                height = 1;

            // Create rectangle for bar                
            rect = new Rect(barWidth, ActualHeight - barHeight, barWidth, height);
            // Check for distortion
            //if (maxLeftDB >= 0.2f)
            //    gradient = theme.MeterDistortionGradient;
            dc.DrawRectangle(new LinearGradientBrush(_colorMeterB1, _colorMeterB2, new Point(0, 0), new Point(ActualWidth / 2, ActualHeight)), new Pen(), rect);

            // Draw number of db      
            strDB = peakRightDB.ToString("00.0").Replace(",", ".");
            if (maxRightDB == -100.0f)
                strDB = "-inf";

            // Draw peak line
            pt1 = new Point(barWidth, ActualHeight - (scaleMultiplier * (peakRightDB + 100)));
            pt2 = new Point(barWidth * 2, ActualHeight - (scaleMultiplier * (peakRightDB + 100)));
            dc.DrawLine(new Pen(new SolidColorBrush(_colorPeakLine), 1), pt1, pt2);                        

            // Draw number of decibels (with font shadow to make it easier to read)                
            formattedText = new FormattedText(strDB, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Roboto Condensed"), 10, new SolidColorBrush(Color.FromRgb(20, 20, 20)));
            newX = ((barWidth - formattedText.Width) / 2) + barWidth;
            dc.DrawText(formattedText, new Point(newX + 1, ActualHeight - formattedText.Height - 4));
            formattedText.SetForegroundBrush(Brushes.White);
            dc.DrawText(formattedText, new Point(newX, ActualHeight - formattedText.Height - 4 - 1));
        }   
    }
}
