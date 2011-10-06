//
// WaveForm.cs: This wave form display control takes raw audio data and displays the current wave form of mono or stereo channels.
//              The control appearance can be changed using the public properties.
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

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This wave form display control takes raw audio data and displays the current wave form of mono or stereo channels.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    public partial class WaveForm : Control
    {
        private List<WaveDataMinMax> m_waveDataHistory = null;
        /// <summary>
        /// Array containing an history of min and max peaks over the last 1000ms.
        /// </summary>
        public List<WaveDataMinMax> WaveDataHistory
        {
            get
            {
                return m_waveDataHistory;
            }
        }

        #region Border Properties
        
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("The color of the border")]
        public Color BorderColor { get; set; }
        
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("The width of the border")]
        public int BorderWidth { get; set; }
        #endregion

        #region Background Properties

        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("First color of the background gradient.")]
        public Color GradientColor1 { get; set; }

        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Second color of the background gradient.")]
        public Color GradientColor2 { get; set; }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Gradient mode")]
        public LinearGradientMode GradientMode { get; set; }

        #endregion

        #region Other Properties
        
        private WaveFormDisplayType m_displayType = WaveFormDisplayType.Stereo;
        /// <summary>
        /// Wave form display type (left channel, right channel or stereo).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Wave form display type (left channel, right channel or stereo).")]
        public WaveFormDisplayType DisplayType
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

        private Color m_waveFormColor = Color.Green;
        /// <summary>
        /// Color used when drawing the wave form.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the wave form.")]        
        public Color WaveFormColor
        {
            get
            {
                return m_waveFormColor;
            }
            set
            {
                m_waveFormColor = value;
            }
        }

        private Color m_waveFormDistortionColor = Color.Red;
        /// <summary>
        /// Color used when drawing the wave form and value exceeds distortion threshold.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the wave form and value exceeds distortion threshold.")]        
        public Color WaveFormDistortionColor
        {
            get
            {
                return m_waveFormDistortionColor;
            }
            set
            {
                m_waveFormDistortionColor = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for WaveForm.
        /// </summary>
        public WaveForm() 
            : base()
        {
            // Create history
            m_waveDataHistory = new List<WaveDataMinMax>();

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
            AddToHistory(AudioTools.GetMinMaxFromWaveData(waveDataLeft, waveDataRight, false));
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

                // Trim history larger than the width to render
                if (WaveDataHistory.Count > Width)
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
            Pen pen = null;
            base.OnPaint(pe);

            // Create a bitmap the size of the form.
            Bitmap bmp = new Bitmap(Bounds.Width, Bounds.Height);

            Graphics g = Graphics.FromImage(bmp);

            //Graphics g = pe.Graphics;

            // Set text anti-aliasing to ClearType (best looking AA)
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Set smoothing mode for paths
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background gradient
            LinearGradientBrush brushBackground = new LinearGradientBrush(ClientRectangle, GradientColor1, GradientColor2, GradientMode);
            g.FillRectangle(brushBackground, ClientRectangle);
            brushBackground.Dispose();

            //int barWidth = 1;
            for (int i = 0; i < WaveDataHistory.Count; i++)
            {
                // Determine the maximum height of a line (+/-)
                float heightToRenderLine = 0;                
                if (DisplayType == WaveFormDisplayType.Stereo)
                {
                    heightToRenderLine = (float)Height / 4;
                }
                else
                {
                    heightToRenderLine = (float)Height / 2;
                }                

                // Determine x position (total width - 1 - bar width(1))
                int x = this.Width - 1 - i;

                // Determine line height for each channel
                float leftMaxHeight = WaveDataHistory[i].leftMax * heightToRenderLine;
                float leftMinHeight = WaveDataHistory[i].leftMin * heightToRenderLine;
                float rightMaxHeight = WaveDataHistory[i].rightMax * heightToRenderLine;
                float rightMinHeight = WaveDataHistory[i].rightMin * heightToRenderLine;
                float mixMaxHeight = WaveDataHistory[i].mixMax * heightToRenderLine;
                float mixMinHeight = WaveDataHistory[i].mixMin * heightToRenderLine;

                // Determine display type
                if (DisplayType == WaveFormDisplayType.LeftChannel || 
                    DisplayType == WaveFormDisplayType.RightChannel ||
                    DisplayType == WaveFormDisplayType.Mix)
                {
                    // To prevent copying code multiple times, set min/max depending on selected channel
                    float max = 0;
                    float min = 0;
                    float minLineHeight = 0;
                    float maxLineHeight = 0;

                    // Set mib/max
                    if (DisplayType == WaveFormDisplayType.LeftChannel)
                    {
                        min = WaveDataHistory[i].leftMin;
                        max = WaveDataHistory[i].leftMax;
                        minLineHeight = leftMinHeight;
                        maxLineHeight = leftMaxHeight;
                    }
                    else if (DisplayType == WaveFormDisplayType.RightChannel)
                    {
                        min = WaveDataHistory[i].rightMin;
                        max = WaveDataHistory[i].rightMax;
                        minLineHeight = rightMinHeight;
                        maxLineHeight = rightMaxHeight;
                    }
                    else if (DisplayType == WaveFormDisplayType.Mix)
                    {
                        min = WaveDataHistory[i].mixMin;
                        max = WaveDataHistory[i].mixMax;
                        minLineHeight = mixMinHeight;
                        maxLineHeight = mixMaxHeight;
                    }

                    // ------------------------
                    // POSITIVE MAX VALUE 

                    // Determine if the max peak is distorting (change pen color)               
                    if (max > DistortionThreshold)
                    {
                        pen = new Pen(new SolidBrush(WaveFormDistortionColor));
                    }
                    else
                    {
                        pen = new Pen(new SolidBrush(WaveFormColor));
                    }

                    // Draw positive value (y: middle to top)
                    g.DrawLine(pen, new PointF(x, heightToRenderLine), new PointF(x, heightToRenderLine - maxLineHeight));

                    // ------------------------
                    // NEGATIVE MAX VALUE 

                    // Determine if the min peak is distorting (change pen color)                
                    if (min < -DistortionThreshold)
                    {
                        pen = new Pen(new SolidBrush(WaveFormDistortionColor));
                    }
                    else
                    {
                        pen = new Pen(new SolidBrush(WaveFormColor));
                    }

                    // Draw negative value (y: middle to height)
                    g.DrawLine(pen, new PointF(x, heightToRenderLine), new PointF(x, heightToRenderLine + (-minLineHeight)));
                }
                else if (DisplayType == WaveFormDisplayType.Stereo)
                {
                    // ========================
                    // LEFT CHANNEL

                    // ------------------------
                    // POSITIVE MAX VALUE 

                    // Determine if the max peak is distorting (change pen color)               
                    if (WaveDataHistory[i].leftMax > DistortionThreshold)
                    {
                        pen = new Pen(new SolidBrush(WaveFormDistortionColor));
                    }
                    else
                    {
                        pen = new Pen(new SolidBrush(WaveFormColor));
                    }

                    // Draw positive value (y: middle to top)
                    g.DrawLine(pen, new PointF(x, heightToRenderLine), new PointF(x, heightToRenderLine - leftMaxHeight));

                    // ------------------------
                    // NEGATIVE MAX VALUE 

                    // Determine if the min peak is distorting (change pen color)                
                    if (WaveDataHistory[i].leftMin < -DistortionThreshold)
                    {
                        pen = new Pen(new SolidBrush(WaveFormDistortionColor));
                    }
                    else
                    {
                        pen = new Pen(new SolidBrush(WaveFormColor));
                    }

                    // Draw negative value (y: middle to height)
                    g.DrawLine(pen, new PointF(x, heightToRenderLine), new PointF(x, heightToRenderLine + (-leftMinHeight)));

                    // ========================
                    // RIGHT CHANNEL

                    // ------------------------
                    // POSITIVE MAX VALUE 

                    // Determine if the max peak is distorting (change pen color)               
                    if (WaveDataHistory[i].rightMax > DistortionThreshold)
                    {
                        pen = new Pen(new SolidBrush(WaveFormDistortionColor));
                    }
                    else
                    {
                        pen = new Pen(new SolidBrush(WaveFormColor));
                    }

                    // Multiply by 3 to get the new center line for right channel
                    // Draw positive value (y: middle to top)
                    g.DrawLine(pen, new PointF(x, (heightToRenderLine * 3)), new PointF(x, (heightToRenderLine * 3) - rightMaxHeight));

                    // ------------------------
                    // NEGATIVE MAX VALUE 

                    // Determine if the min peak is distorting (change pen color)                
                    if (WaveDataHistory[i].rightMin < -DistortionThreshold)
                    {
                        pen = new Pen(new SolidBrush(WaveFormDistortionColor));
                    }
                    else
                    {
                        pen = new Pen(new SolidBrush(WaveFormColor));
                    }

                    // Draw negative value (y: middle to height)
                    g.DrawLine(pen, new PointF(x, (heightToRenderLine * 3)), new PointF(x, (heightToRenderLine * 3) + (-rightMinHeight)));
                }
            }

            pe.Graphics.DrawImage(bmp, 0, 0, ClientRectangle, GraphicsUnit.Pixel);
            g.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Defines the wave form display type.
    /// </summary>
    public enum WaveFormDisplayType
    {
        LeftChannel = 0, RightChannel = 1, Stereo = 2, Mix = 3
    }
}
