//
// OutputMeter.cs: This output meter control takes raw audio data and displays the current level of mono or stereo channels.
//                 The control appearance can be changed using the public properties.
//
// Copyright © 2011 Yanick Castonguay
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.WindowsControls;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This output meter control takes raw audio data and displays the current level of mono or stereo channels.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    public partial class OutputMeter : Control
    {
        /// <summary>
        /// Private value for the WaveDataHistory property.
        /// </summary>
        private List<WaveDataMinMax> m_waveDataHistory = null;
        /// <summary>
        /// Array containing an history of min and max peaks over the last 1000ms.
        /// </summary>
        [Browsable(false)]
        public List<WaveDataMinMax> WaveDataHistory
        {
            get
            {
                return m_waveDataHistory;
            }
        }

        /// <summary>
        /// Private value for the Theme property.
        /// </summary>
        private OutputMeterTheme m_theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        public OutputMeterTheme Theme
        {
            get
            {
                return m_theme;
            }
            set
            {
                m_theme = value;
            }
        }

        #region Other Properties

        /// <summary>
        /// Private value for the DisplayType property.
        /// </summary>
        private OutputMeterDisplayType m_displayType = OutputMeterDisplayType.Stereo;
        /// <summary>
        /// Output meter display type (left channel, right channel, stereo, or mix).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Output meter display type (left channel, right channel, stereo, or mix).")]
        public OutputMeterDisplayType DisplayType
        {
            get
            {
                return m_displayType;
            }
            set
            {
                m_displayType = value;
            }
        }

        /// <summary>
        /// Private value for the DistortionThreshold property.
        /// </summary>
        private float m_distortionThreshold = 0.9f;
        /// <summary>
        /// Value used to determine if the signal is distorting. Value range: 0.0f to 1.0f.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Value used to determine if the signal is distorting. Value range: 0.0f to 1.0f.")]
        public float DistortionThreshold
        {
            get
            {
                return m_distortionThreshold;
            }
            set
            {
                m_distortionThreshold = value;
            }
        }

        /// <summary>
        /// Private value for the DisplayDecibels property.
        /// </summary>
        private bool m_displayDecibels = true;
        /// <summary>
        /// Display the peak (1000ms) in decibels in text at the bottom of each bar.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Display the peak (1000ms) in decibels in text at the bottom of each bar.")]
        public bool DisplayDecibels
        {
            get
            {
                return m_displayDecibels;
            }
            set
            {
                m_displayDecibels = value;
            }
        }

        /// <summary>
        /// Private value for the DrawFloor property.
        /// </summary>
        private float m_drawFloor = -60f;
        /// <summary>
        /// Floor from which the output meter will be drawn. Minimum value: -100 (dB). Max: 0 (dB). Default value: -60 (dB).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(-60)]
        [Category("Display"), Browsable(true), Description("Floor from which the output meter will be drawn. Minimum value: -100 (dB). Max: 0 (dB). Default value: -60 (dB).")]
        public float DrawFloor
        {
            get
            {
                return m_drawFloor;
            }
            set
            {
                m_drawFloor = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for OutputMeter.
        /// </summary>
        public OutputMeter()
        {
            // Create history
            m_waveDataHistory = new List<WaveDataMinMax>();

            // Create default theme
            m_theme = new OutputMeterTheme();

            // Set control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
            ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            // Initialize components (thank you Cpt Obvious!)
            InitializeComponent();
        }

        #region Wave Data History
        
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

        #endregion

        #region Paint Events

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Declare variables
            Pen pen = null;

            base.OnPaint(pe);

            // Create a bitmap the size of the form.
            Bitmap bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Graphics g = Graphics.FromImage(bmp);

            // Use anti-aliasing?
            if (m_theme.CustomFont.UseAntiAliasing)
            {
                // Set text anti-aliasing to ClearType (best looking AA)
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Set smoothing mode for paths
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }

            // Create custom font
            Font font = null;

            // Make sure the embedded font name needs to be loaded and is valid
            if (m_theme.CustomFont.UseEmbeddedFont && !String.IsNullOrEmpty(m_theme.CustomFont.EmbeddedFontName))
            {
                try
                {
                    // Get embedded font collection
                    EmbeddedFontCollection embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();

                    // Get embedded font
                    font = Tools.LoadEmbeddedFont(embeddedFonts, m_theme.CustomFont.EmbeddedFontName, m_theme.CustomFont.Size, m_theme.CustomFont.ToFontStyle());
                }
                catch (Exception ex)
                {
                    // Use default font instead
                    font = this.Font;
                }
            }

            // Check if font is null
            if (font == null)
            {
                try
                {
                    // Try to get standard font
                    font = new Font(m_theme.CustomFont.StandardFontName, m_theme.CustomFont.Size, m_theme.CustomFont.ToFontStyle());
                }
                catch (Exception ex)
                {
                    // Use default font instead
                    font = this.Font;
                }
            }
            
            // Draw background gradient (cover -1 pixel for some refresh bug)
            Rectangle rectBody = new Rectangle(-1, -1, Width + 1, Height + 1);
            LinearGradientBrush brushBackground = new LinearGradientBrush(rectBody, m_theme.GradientColor1, m_theme.GradientColor2, m_theme.GradientMode);
            g.FillRectangle(brushBackground, rectBody);
            brushBackground.Dispose();
            brushBackground = null;

            // If the wave data is empty, skip rendering 
            if (WaveDataHistory == null || WaveDataHistory.Count == 0)
            {
                // Draw bitmap on control
                pe.Graphics.DrawImage(bmp, 0, 0, ClientRectangle, GraphicsUnit.Pixel);
                
                // Dispose stuff
                g.Dispose();
                g = null;

                return;
            }

            // By default, the bar width takes the full width of the control (except for stereo)
            float barWidth = Width;

            // Check display type
            if (DisplayType == OutputMeterDisplayType.Stereo)
            {
                // Set bar width to half width since there's two bars to draw
                barWidth = Width / 2;

                // at 10ms refresh, get last value.
                float maxLeftDB = 20.0f * (float)Math.Log10(WaveDataHistory[0].leftMax);
                float maxRightDB = 20.0f * (float)Math.Log10(WaveDataHistory[0].rightMax);
                //float maxLeftDB2 = (float)Base.LevelToDB_16Bit((double)WaveDataHistory[0].leftMax);
                //float maxRightDB2 = (float)Base.LevelToDB_16Bit((double)WaveDataHistory[0].rightMax);

                // Get peak for the last 1000ms
                float peakLeftDB = AudioTools.GetPeak(WaveDataHistory, 100, MPfm.Sound.ChannelType.Left);
                float peakRightDB = AudioTools.GetPeak(WaveDataHistory, 100, MPfm.Sound.ChannelType.Right);

                // Set the dB range to display (-100 to +10dB)
                float dbRangeToDisplay = 110;

                // Get the related shits (+10, makes 110 possible height)
                //float dbDisplay = maxDB + 10;

                // Get multiplier (110 height to 330 == 3)
                float scaleMultiplier = ClientRectangle.Height / dbRangeToDisplay;

                // Get bar height -- If value = -100 then 0. If value = 0 then = 100. if value = 10 then = 110.
                //float barHeight = scaleMultiplier * (maxDB + 100);

                // Create brushes for displaying volume in decibels
                SolidBrush brushFontColor = new SolidBrush(m_theme.FontColor);
                SolidBrush brushFontShadowColor = new SolidBrush(m_theme.FontShadowColor);

                // Draw 0 dB line
                pen = new Pen(m_theme.Meter0dbLineColor);
                g.DrawLine(pen, new Point(0, 10), new Point(Width, 10));
                pen.Dispose();
                pen = null;

                // -----------------------------------------
                // LEFT CHANNEL
                //

                // Get the VU value from audio tools
                float vuLeft = AudioTools.GetVUMeterValue(WaveDataHistory, 100, MPfm.Sound.ChannelType.Left);

                // Calculate bar height
                float barHeight = maxLeftDB + 100;
                //float barHeight = vuLeft + 100;
                float height = barHeight;
                if (height == 0)
                {
                    // LinearBrush doesnt like 0 
                    height = 1;
                }

                // Create rectangle for bar
                RectangleF rect = new RectangleF(0, 110 - barHeight, barWidth, height);

                // Create linear gradient brush covering the bar
                RectangleF rectGrad = new RectangleF(0, 110, barWidth, height);
                LinearGradientBrush brushBar = new LinearGradientBrush(rectGrad, m_theme.MeterGradientColor1, m_theme.MeterGradientColor2, LinearGradientMode.Vertical);
                //if (maxLeftDB >= 0.2f)
                //{
                //    brushBar = new LinearGradientBrush(rectGrad, MeterDistortionGradientColor1, MeterDistortionGradientColor2, LinearGradientMode.Vertical);
                //}

                // Draw rectangle and dispose objects
                g.FillRectangle(brushBar, rect);
                brushBar.Dispose();
                brushBar = null;

                // Draw peak line
                pen = new Pen(m_theme.MeterPeakLineColor);
                g.DrawLine(pen, new PointF(0, 110 - (peakLeftDB + 100)), new PointF(barWidth, 110 - (peakLeftDB + 100)));
                pen.Dispose();
                pen = null;

                // Draw number of db      
                string strDB = peakLeftDB.ToString("00.0");                
                //string strDB = vuLeft.ToString("00.0");
                if (maxLeftDB == -100.0f)
                {
                    strDB = "-inf";
                }
               
                // barWidth - stringSize.Width = portion restante sans texte. prendre la moitie de ca
                SizeF stringSize = g.MeasureString(strDB, font);
                float newX = (barWidth - stringSize.Width) / 2;               

                // Draw number of decibels (with font shadow to make it easier to read)
                g.DrawString(strDB, font, brushFontShadowColor, new PointF(newX + 1, Height - stringSize.Height));
                g.DrawString(strDB, font, brushFontColor, new PointF(newX, Height - stringSize.Height - 1));

                // -----------------------------------------
                // RIGHT CHANNEL
                //

                // Calculate bar height
                barHeight = maxRightDB + 100;
                height = barHeight;
                if (height == 0)
                {
                    // LinearBrush doesnt like 0 
                    height = 1;
                }

                // Create rectangle for bar
                rect = new RectangleF(barWidth, Height - barHeight, barWidth, height);

                // Create linear gradient brush covering the bar
                rectGrad = new RectangleF(barWidth, 110, barWidth, height);
                brushBar = new LinearGradientBrush(rectGrad, m_theme.MeterGradientColor1, m_theme.MeterGradientColor2, LinearGradientMode.Vertical);
                //if (maxRightDB >= 0.2f)
                //{
                //    brushBar = new LinearGradientBrush(rectGrad, MeterDistortionGradientColor1, MeterDistortionGradientColor2, LinearGradientMode.Vertical);
                //}

                // Draw rectangle and dispose objects
                g.FillRectangle(brushBar, rect);
                brushBar.Dispose();
                brushBar = null;

                // Draw number of db      
                strDB = maxRightDB.ToString("00.0");
                if (maxRightDB == -100.0f)
                {
                    strDB = "-inf";
                }

                // Draw peak line
                pen = new Pen(m_theme.MeterPeakLineColor);
                g.DrawLine(pen, new PointF(barWidth, 110 - (peakRightDB + 100)), new PointF(barWidth * 2, 110 - (peakRightDB + 100)));
                pen.Dispose();
                pen = null;

                // Draw number of db      
                strDB = peakRightDB.ToString("00.0");

                // barWidth - stringSize.Width = portion restante sans texte. prendre la moitie de ca
                stringSize = g.MeasureString(strDB, font);
                newX = ((barWidth - stringSize.Width) / 2) + barWidth;
                
                // Draw number of decibels (with font shadow to make it easier to read)                
                g.DrawString(strDB, font, brushFontShadowColor, new PointF(newX + 1, Height - stringSize.Height));                
                g.DrawString(strDB, font, brushFontColor, new PointF(newX, Height - stringSize.Height - 1));

                // Dispose the font brushes
                brushFontColor.Dispose();
                brushFontColor = null;
                brushFontShadowColor.Dispose();
                brushFontShadowColor = null;
            }
            else
            {
                float max = 0.0f;
                if (DisplayType == OutputMeterDisplayType.LeftChannel)
                {
                    max = WaveDataHistory[0].leftMax;
                }
                else if (DisplayType == OutputMeterDisplayType.RightChannel)
                {
                    max = WaveDataHistory[0].rightMax;
                }
                else if (DisplayType == OutputMeterDisplayType.Mix)
                {
                    max = WaveDataHistory[0].mixMax;
                }

                // at 10ms refresh, get last value.
                float maxDB = 20.0f * (float)Math.Log10(max);

                // Set the dB range to display (-100 to +10dB)
                float dbRangeToDisplay = 110;

                // Get the related shits (+10, makes 110 possible height)
                //float dbDisplay = maxDB + 10;

                // Get multiplier (110 height to 330 == 3)
                float scaleMultiplier = ClientRectangle.Height / dbRangeToDisplay;

                // Get bar height -- If value = -100 then 0. If value = 0 then = 100. if value = 10 then = 110.
                //float barHeight = scaleMultiplier * (maxDB + 100);
                float barHeight = maxDB + 100;

                float height = barHeight;
                if (height == 0)
                {
                    // LinearBrush doesnt like 0 
                    height = 1;
                }

                RectangleF rect = new RectangleF(0, Height - barHeight, barWidth, height);
                RectangleF rectGrad = new RectangleF(0, 110, barWidth, height);
                LinearGradientBrush brushBar = new LinearGradientBrush(rectGrad, Color.LightGreen, Color.DarkGreen, LinearGradientMode.Vertical);
                if (maxDB >= 0)
                {
                    brushBar = new LinearGradientBrush(rectGrad, Color.Red, Color.DarkRed, LinearGradientMode.Vertical);
                }
                g.FillRectangle(brushBar, rect);
                brushBar.Dispose();
                brushBar = null;

                g.DrawString(maxDB.ToString("0.0"), Font, Brushes.White, new PointF(0, 0));
            }

            // Dispose font if necessary
            if (font != null)
            {
                font.Dispose();
                font = null;
            }

            // Draw bitmap on control
            pe.Graphics.DrawImage(bmp, 0, 0, ClientRectangle, GraphicsUnit.Pixel);
            
            // Dispose graphics and bitmap
            bmp.Dispose();
            bmp = null;
            g.Dispose();
            g = null;
        }

        #endregion
    }

    /// <summary>
    /// Defines the display type of the output meter.
    /// </summary>
    public enum OutputMeterDisplayType
    {
        LeftChannel = 0, RightChannel = 1, Stereo = 2, Mix = 3
    }
}
 