//
// WaveFormMarkersLoops.cs: This control displays the wave form of an audio file, extracted using readData. 
//                          Supports background loading of the wave form. This control is used in the 
//                          Loops & Markers panel of MPfm.
//                          The control appearance can be changed using the public properties.
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Sound;
using MPfm.WindowsControls;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This wave form display control takes raw audio data and displays the current wave form of mono or stereo channels.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    public partial class WaveFormMarkersLoops : Control
    {
        #region Private variables

        /// <summary>
        /// Defines the X position of the cursor.
        /// </summary>
        private float m_cursorX = 0.0f;

        /// <summary>
        /// PeakFile instance used for generating and reading peak files.
        /// </summary>
        private PeakFile m_peakFile = null;

        /// <summary>
        /// Timer controlling the refresh for the loading and cursor display.
        /// </summary>
        private MPfm.Sound.Timer m_timer = null;

        // Animation
        private int animResolution = 256;
        private int animZoomCount = -1;
        private int animToolbarCount = -1;

        // Mouse cursor related stuff
        private bool isMouseOverToolbar = false;

        MPfm.WindowsControls.HScrollBar horizontalScrollBar = null;

        // Contextual menu
        private ContextMenuStrip menuStrip = null;
        private ToolStripSeparator menuItemSeparator = null;
        private ToolStripMenuItem menuItemCreateMarkerAtThisPosition = null;
        private ToolStripMenuItem menuItemMouseInteractionTypeZoomIn = null;
        private ToolStripMenuItem menuItemMouseInteractionTypeZoomOut = null;
        private ToolStripMenuItem menuItemMouseInteractionTypePointer = null;
        private ToolStripMenuItem menuItemMouseInteractionTypeSelect = null;
        private ToolStripMenuItem menuItemAutoScrollWithCursor = null;
        private ToolStripMenuItem menuItemDisplayCurrentPosition = null;
        private ToolStripMenuItem menuItemZoom = null;
        private ToolStripMenuItem menuItemZoom100 = null;
        private ToolStripMenuItem menuItemZoom200 = null;
        private ToolStripMenuItem menuItemZoom400 = null;
        private ToolStripMenuItem menuItemZoom800 = null;
        private ToolStripMenuItem menuItemZoom1600 = null;
        private ToolStripMenuItem menuItemZoomCustom = null;
        private ToolStripMenuItem menuItemWaveFormDisplayType = null;
        private ToolStripMenuItem menuItemWaveFormDisplayTypeStereo = null;
        private ToolStripMenuItem menuItemWaveFormDisplayTypeLeftMono = null;
        private ToolStripMenuItem menuItemWaveFormDisplayTypeRight = null;
        private ToolStripMenuItem menuItemWaveFormDisplayTypeMix = null;

        private bool needToRefreshBitmapCache = false;

        // Delegate events
        public delegate void PositionChanged(PositionChangedData data);
        public event PositionChanged OnPositionChanged;

        #endregion

        #region Other Properties
        
        private float m_scrollX = 0;
        public float ScrollX
        {
            get
            {
                return m_scrollX;
            }
        }

        private bool m_autoScrollWithCursor = true;
        /// <summary>
        /// Defines if the cursor needs to automatically scroll the waveform
        /// (when zoom is over 100%).
        /// </summary>
        public bool AutoScrollWithCursor
        {
            get
            {
                return m_autoScrollWithCursor;
            }
            set
            {
                m_autoScrollWithCursor = value;
            }
        }

        private float m_zoom = 100;
        /// <summary>
        /// Defines the zoom for the wave form display (in percentage).
        /// </summary>
        public float Zoom
        {
            get
            {
                return m_zoom;
            }
            set
            {                               
                // Check what is the factor between the two zooms (i.e. 100% to 200% = 2)
                float zoomDelta = value - m_zoom;

                // Check if the zoom level hasn't changed
                if (zoomDelta == 0)
                {
                    // Do nothing
                    return;
                }

                // Check if the new zoom is 100% 
                if (value == 100)
                {
                    // Reset scroll X
                    m_scrollX = 0;
                } 
                // Check if the value is positive
                else if (zoomDelta > 0)
                {
                    // 100% == 2
                    // 200% == 4
                    // 300% == 6
                    // 400% == 8
                    // 500% == 10
                    // 600% == 12
                    // 700% == 14
                    // 800% == 16

                    float factor = (zoomDelta / 100) * 2;

                    // Scale the scroll x
                    m_scrollX = factor * ScrollX;
                }
                // Check if the value is negative
                else if (zoomDelta < 0)
                {
                    // -500% == 0.5
                    // -400% == 0.5
                    // -300% == 0.125
                    // -200% == 0.25
                    // -100% == 0.5

                    // -100% = 2 (half)
                    // -200% = 3 (third)
                    float factors = -(zoomDelta / 100) + 1;

                    // Scale the scroll x
                    m_scrollX = ScrollX / factors;
                }

                // Set zoom value
                m_zoom = value;

                // Set scrollbar value
                horizontalScrollBar.m_value = (int)m_scrollX;
            }
        }

        #endregion

        #region Properties
        
        #region Font Properties

        /// <summary>
        /// Name of the embedded font (as written in the Name property of a CustomFont).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Name of the embedded font (as written in the Name property of a CustomFont).")]
        public string CustomFontName { get; set; }

        /// <summary>
        /// Pointer to the embedded font collection.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Pointer to the embedded font collection.")]
        public FontCollection FontCollection { get; set; }

        #endregion

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

        #region Properties (File / Song Information)

        private string m_peakFileDirectory;
        /// <summary>
        /// Directory where the peak files are located.
        /// Peak Files in current directory by default.
        /// </summary>
        public string PeakFileDirectory
        {
            get
            {
                return m_peakFileDirectory;
            }
            set
            {
                m_peakFileDirectory = value;
            }
        }

        private long m_position = 0;
        /// <summary>
        /// Defines the current audio file position (in bytes).
        /// This needs to be set by the user in order to display the cursor 
        /// (with the PositionTime and Length properties).
        /// </summary>
        public long Position
        {
            get
            {
                return m_position;
            }
        }

        private string m_positionTime = "00:00.000";
        /// <summary>
        /// Defines the current audio file position (in time string, such as 00:00.000).
        /// This needs to be set by the user in order to display the cursor 
        /// (with the Position and Length properties).
        /// </summary>
        public string PositionTime
        {
            get
            {
                return m_positionTime;
            }
        }

        private long m_length = 0;
        /// <summary>
        /// Defines the current audio file length (in bytes).
        /// This needs to be set by the user in order to display the cursor 
        /// (with the Position and PositionTime properties).
        /// </summary>
        public long Length
        {
            get
            {
                return m_length;
            }
            set
            {
                m_length = value;
            }
        }

        //private uint m_currentPositionMS = 0;
        ///// <summary>
        ///// Defines the current position (in milliseconds).
        ///// This needs to be set by the user in order to display the cursor.
        ///// </summary>
        //public uint CurrentPositionMS
        //{
        //    get
        //    {
        //        return m_currentPositionMS;
        //    }
        //    set
        //    {
        //        m_currentPositionMS = value;
        //    }
        //}

        //private uint m_totalMS = 0;
        ///// <summary>
        ///// Defines the total number of milliseconds of the audio file.        
        ///// This needs to be set by the user in order to display the cursor.
        ///// </summary>
        //public uint TotalMS
        //{
        //    get
        //    {
        //        return m_totalMS;
        //    }
        //    set
        //    {
        //        m_totalMS = value;
        //    }
        //}

        #endregion

        #region Properties (Wave Data History)

        private bool m_isLoading = false;
        /// <summary>
        /// Indicates if the wave form history data is currently loading. This will display a shadow over 
        /// the loading waveform and indicate the percentage of data loaded.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return m_isLoading;
            }
        }

        private uint m_bytesRead = 0;
        /// <summary>
        /// Defines the number of bytes read when loading wave data history.        
        /// </summary>
        public uint BytesRead
        {
            get
            {
                return m_bytesRead;
            }
        }

        private float m_percentageDone = 0;
        /// <summary>
        /// Defines the percentage of the wave form generation done.
        /// </summary>
        public float PercentageDone
        {
            get
            {
                return m_percentageDone;
            }
        }

        private List<WaveDataMinMax> m_waveDataHistory = null;
        /// <summary>
        /// Array containing an history of min and max peaks of the audio file.
        /// </summary>
        public List<WaveDataMinMax> WaveDataHistory
        {
            get
            {
                return m_waveDataHistory;
            }
        }

        #endregion

        #region Properties (Wave Form Display)
        
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

        private Color m_cursorColor = Color.White;
        /// <summary>
        /// Color used when drawing the current song position cursor over the wave form.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the current song position cursor over the wave form.")]
        public Color CursorColor
        {
            get
            {
                return m_cursorColor;
            }
            set
            {
                m_cursorColor = value;
            }
        }

        #endregion

        // Wave form cache
        private Bitmap m_bitmapWaveForm = null;
        public Bitmap BitmapWaveForm
        {
            get
            {
                return m_bitmapWaveForm;
            }
        }

        private WaveFormMouseInteractionType m_mouseInteractionType = WaveFormMouseInteractionType.Pointer;
        public WaveFormMouseInteractionType MouseInteractionType
        {
            get
            {
                return m_mouseInteractionType;
            }
        }

        private bool m_displayCurrentPosition = true;
        public bool DisplayCurrentPosition
        {
            get
            {
                return m_displayCurrentPosition;
            }
            set
            {
                m_displayCurrentPosition = value;
            }
        }

        #endregion               

        #region Constructor
        
        /// <summary>
        /// Default constructor for WaveFormMarkersLoops.
        /// </summary>
        public WaveFormMarkersLoops() 
            : base()
        {
            #region Contextual Menu
            
            // Create contextual menu
            menuStrip = new System.Windows.Forms.ContextMenuStrip();

            // Create mouse interaction type menu items
            menuItemMouseInteractionTypePointer = (ToolStripMenuItem)menuStrip.Items.Add("Pointer");            
            menuItemMouseInteractionTypePointer.Checked = true;
            menuItemMouseInteractionTypePointer.Image = MPfm.WindowsControls.Properties.Resources.pointer;
            menuItemMouseInteractionTypePointer.Tag = WaveFormMouseInteractionType.Pointer;
            menuItemMouseInteractionTypePointer.Click += new EventHandler(menuItemMouseInteractionType_Click);

            menuItemMouseInteractionTypeSelect = (ToolStripMenuItem)menuStrip.Items.Add("Select");            
            menuItemMouseInteractionTypeSelect.Checked = false;
            menuItemMouseInteractionTypeSelect.Image = MPfm.WindowsControls.Properties.Resources.select;
            menuItemMouseInteractionTypeSelect.Tag = WaveFormMouseInteractionType.Select;
            menuItemMouseInteractionTypeSelect.Click += new EventHandler(menuItemMouseInteractionType_Click);

            menuItemMouseInteractionTypeZoomIn = (ToolStripMenuItem)menuStrip.Items.Add("Zoom in");
            menuItemMouseInteractionTypeZoomIn.Checked = false;
            menuItemMouseInteractionTypeZoomIn.Image = MPfm.WindowsControls.Properties.Resources.zoom_in;
            menuItemMouseInteractionTypeZoomIn.Tag = WaveFormMouseInteractionType.ZoomIn;
            menuItemMouseInteractionTypeZoomIn.Click += new EventHandler(menuItemMouseInteractionType_Click);

            menuItemMouseInteractionTypeZoomOut = (ToolStripMenuItem)menuStrip.Items.Add("Zoom out");
            menuItemMouseInteractionTypeZoomOut.Checked = false;
            menuItemMouseInteractionTypeZoomOut.Image = MPfm.WindowsControls.Properties.Resources.zoom_out;
            menuItemMouseInteractionTypeZoomOut.Tag = WaveFormMouseInteractionType.ZoomOut;
            menuItemMouseInteractionTypeZoomOut.Click += new EventHandler(menuItemMouseInteractionType_Click);

            // Create separator
            ToolStripSeparator menuItemSeparator = new ToolStripSeparator();
            menuStrip.Items.Add(menuItemSeparator);

            // Create Auto Scroll with Cursor menu item
            menuItemCreateMarkerAtThisPosition = (ToolStripMenuItem)menuStrip.Items.Add("Create marker at this position");
            menuItemCreateMarkerAtThisPosition.Image = MPfm.WindowsControls.Properties.Resources.flag_red;
            menuItemCreateMarkerAtThisPosition.Click += new EventHandler(menuItemCreateMarkerAtThisPosition_Click);

            // Create separator
            menuItemSeparator = new ToolStripSeparator();
            menuStrip.Items.Add(menuItemSeparator);

            // Create Display Current Position menu item
            menuItemDisplayCurrentPosition = (ToolStripMenuItem)menuStrip.Items.Add("Display current time position");
            menuItemDisplayCurrentPosition.CheckOnClick = true;
            menuItemDisplayCurrentPosition.Checked = true;
            menuItemDisplayCurrentPosition.Click += new EventHandler(menuItemDisplayCurrentPosition_Click);

            // Create Auto Scroll with Cursor menu item
            menuItemAutoScrollWithCursor = (ToolStripMenuItem)menuStrip.Items.Add("Auto scroll with cursor");
            menuItemAutoScrollWithCursor.CheckOnClick = true;
            menuItemAutoScrollWithCursor.Checked = true;
            menuItemAutoScrollWithCursor.Click += new EventHandler(menuItemAutoScrollWithCursor_Click);
            
            // Create Zoom menu item
            menuItemZoom = (ToolStripMenuItem)menuStrip.Items.Add("Zoom");
            menuItemZoom.Image = MPfm.WindowsControls.Properties.Resources.zoom;

            menuItemZoom1600 = (ToolStripMenuItem)menuItemZoom.DropDownItems.Add("1600%");
            menuItemZoom1600.Checked = false;
            menuItemZoom1600.Tag = 1600;
            menuItemZoom1600.Click += new EventHandler(menuItemZoom_Click);

            menuItemZoom800 = (ToolStripMenuItem)menuItemZoom.DropDownItems.Add("800%");            
            menuItemZoom800.Checked = false;
            menuItemZoom800.Tag = 800;
            menuItemZoom800.Click += new EventHandler(menuItemZoom_Click);

            menuItemZoom400 = (ToolStripMenuItem)menuItemZoom.DropDownItems.Add("400%");            
            menuItemZoom400.Checked = false;
            menuItemZoom400.Tag = 400;
            menuItemZoom400.Click += new EventHandler(menuItemZoom_Click);

            menuItemZoom200 = (ToolStripMenuItem)menuItemZoom.DropDownItems.Add("200%");            
            menuItemZoom200.Checked = false;
            menuItemZoom200.Tag = 200;
            menuItemZoom200.Click += new EventHandler(menuItemZoom_Click);

            menuItemZoom100 = (ToolStripMenuItem)menuItemZoom.DropDownItems.Add("100%");            
            menuItemZoom100.Checked = true;
            menuItemZoom100.Tag = 100;
            menuItemZoom100.Click += new EventHandler(menuItemZoom_Click);

            menuItemZoomCustom = (ToolStripMenuItem)menuItemZoom.DropDownItems.Add("Custom...");
            menuItemZoomCustom.Checked = false;
            menuItemZoomCustom.Tag = -1;
            menuItemZoomCustom.Click += new EventHandler(menuItemZoom_Click);

            // Create Wave Form Display Type menu item
            menuItemWaveFormDisplayType = (ToolStripMenuItem)menuStrip.Items.Add("Wave Form Display Type");
            menuItemWaveFormDisplayType.Image = MPfm.WindowsControls.Properties.Resources.chart_line;

            menuItemWaveFormDisplayTypeStereo = (ToolStripMenuItem)menuItemWaveFormDisplayType.DropDownItems.Add("Stereo");
            menuItemWaveFormDisplayTypeStereo.Checked = true;
            menuItemWaveFormDisplayTypeStereo.Tag = WaveFormDisplayType.Stereo;
            menuItemWaveFormDisplayTypeStereo.Click += new EventHandler(menuItemWaveFormDisplayType_Click);

            menuItemWaveFormDisplayTypeLeftMono = (ToolStripMenuItem)menuItemWaveFormDisplayType.DropDownItems.Add("Left (Mono)");            
            menuItemWaveFormDisplayTypeLeftMono.Checked = false;
            menuItemWaveFormDisplayTypeLeftMono.Tag = WaveFormDisplayType.LeftChannel;
            menuItemWaveFormDisplayTypeLeftMono.Click += new EventHandler(menuItemWaveFormDisplayType_Click);

            menuItemWaveFormDisplayTypeRight = (ToolStripMenuItem)menuItemWaveFormDisplayType.DropDownItems.Add("Right");
            menuItemWaveFormDisplayTypeRight.Checked = false;
            menuItemWaveFormDisplayTypeRight.Tag = WaveFormDisplayType.RightChannel;
            menuItemWaveFormDisplayTypeRight.Click += new EventHandler(menuItemWaveFormDisplayType_Click);            

            menuItemWaveFormDisplayTypeMix = (ToolStripMenuItem)menuItemWaveFormDisplayType.DropDownItems.Add("Mix (Left/Right)");
            menuItemWaveFormDisplayTypeMix.Checked = false;
            menuItemWaveFormDisplayTypeMix.Tag = WaveFormDisplayType.Mix;
            menuItemWaveFormDisplayTypeMix.Click += new EventHandler(menuItemWaveFormDisplayType_Click);

            this.ContextMenuStrip = menuStrip;

            #endregion

            // Create history
            m_waveDataHistory = new List<WaveDataMinMax>();

            // Set peak file directory
            m_peakFileDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Peak Files\\";

            // Create PeakFile class instance 
            m_peakFile = new PeakFile(1);
            m_peakFile.OnProcessStarted += new PeakFile.ProcessStarted(m_peakFile_OnProcessStarted);
            m_peakFile.OnProcessData += new PeakFile.ProcessData(m_peakFile_OnProcessData);
            m_peakFile.OnProcessDone += new PeakFile.ProcessDone(m_peakFile_OnProcessDone);

            // Create timer for refresh
            m_timer = new Sound.Timer();
            m_timer.Period = 500;
            m_timer.Tick += new EventHandler(m_timer_Tick);

            // Set control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
            ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            // Initialize components (thank you Cpt Obvious!)
            InitializeComponent();

            // Create scrollbar
            horizontalScrollBar = new HScrollBar();
            horizontalScrollBar.Visible = false;
            horizontalScrollBar.Height = 14;
            horizontalScrollBar.Width = Width;
            horizontalScrollBar.Minimum = 0;
            horizontalScrollBar.Location = new Point(0, 0);
            horizontalScrollBar.Cursor = Cursors.Default;
            horizontalScrollBar.OnValueChanged += new HScrollBar.ValueChanged(horizontalScrollBar_OnValueChanged);
            //horizontalScrollBar.Scroll += new ScrollEventHandler(horizontalScrollBar_Scroll);            
            this.Controls.Add(horizontalScrollBar);
        }

        /// <summary>
        /// Clears the wave form data history.
        /// </summary>
        public void Clear()
        {
            // Clear history and refresh
            WaveDataHistory.Clear();
            Refresh();
        }

        #endregion

        #region Contextual Menu Events / Scrollbar

        /// <summary>
        /// Occurs when the user clicks on one of the Wave Form Display Type sub menu items.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public void menuItemWaveFormDisplayType_Click(object sender, EventArgs e)
        {
            // Make sure the sender is the menu item
            if (sender is ToolStripMenuItem)
            {
                // Get the reference to the menu item
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

                // Check off all options
                menuItemWaveFormDisplayTypeStereo.Checked = false;
                menuItemWaveFormDisplayTypeLeftMono.Checked = false;
                menuItemWaveFormDisplayTypeRight.Checked = false;
                menuItemWaveFormDisplayTypeMix.Checked = false;

                // Check the selected menu item
                menuItem.Checked = true;

                // Set display type
                DisplayType = (WaveFormDisplayType)menuItem.Tag;

                // Set to redraw the bitmap wave and refresh control
                needToRefreshBitmapCache = true;
                Refresh();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on one of the Zoom sub menu items.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public void menuItemZoom_Click(object sender, EventArgs e)
        {
            // Make sure the sender is the menu item
            if (sender is ToolStripMenuItem)
            {
                // Get the reference to the menu item
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;                

                // Make sure all menu items are unchecked
                menuItemZoom100.Checked = false;
                menuItemZoom200.Checked = false;
                menuItemZoom400.Checked = false;
                menuItemZoom800.Checked = false;
                menuItemZoom1600.Checked = false;
                menuItemZoomCustom.Checked = false;

                // Check the menu item that needs to be checked
                menuItem.Checked = true;

                // Set the right amount of zoom depending on 
                int newZoom = (int)menuItem.Tag;

                // Check if the option is Custom
                if (newZoom == -1)
                {
                    // Display dialog box
                    MessageBox.Show("Sorry, this feature is not implemeted yet.");
                    Zoom = 100;
                }
                else
                {
                    // Check if the current zoom is the same as the user has chosen
                    if (Zoom != newZoom)
                    {
                        // Set new zoom, bitmap refresh flag and refresh
                        Zoom = newZoom;                        
                        animZoomCount = 0;
                        needToRefreshBitmapCache = true;
                        Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the user clicks on one of the MouseInteractionType sub menu items.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public void menuItemMouseInteractionType_Click(object sender, EventArgs e)
        {
            // Make sure the sender is the menu item
            if (sender is ToolStripMenuItem)
            {
                // Get the reference to the menu item
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

                // Make sure all menu items are unchecked
                menuItemMouseInteractionTypePointer.Checked = false;
                menuItemMouseInteractionTypeSelect.Checked = false;
                menuItemMouseInteractionTypeZoomIn.Checked = false;
                menuItemMouseInteractionTypeZoomOut.Checked = false;

                // Check the menu item that needs to be checked
                menuItem.Checked = true;

                // Set mouse interaction type
                m_mouseInteractionType = (WaveFormMouseInteractionType)menuItem.Tag;

                // Set mouse cursor
                if (m_mouseInteractionType == WaveFormMouseInteractionType.Pointer)
                {
                    this.Cursor = Cursors.Default;
                }
                else if (m_mouseInteractionType == WaveFormMouseInteractionType.Select)
                {
                    this.Cursor = Cursors.Cross;
                }
                else if (m_mouseInteractionType == WaveFormMouseInteractionType.ZoomIn)
                {
                    this.Cursor = Tools.CreateCursor(MPfm.WindowsControls.Properties.Resources.zoom_in, 2, 2);
                }
                else if (m_mouseInteractionType == WaveFormMouseInteractionType.ZoomOut)
                {
                    this.Cursor = Tools.CreateCursor(MPfm.WindowsControls.Properties.Resources.zoom_out, 2, 2);
                }
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Display current time position" menu item.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public void menuItemDisplayCurrentPosition_Click(object sender, EventArgs e)
        {
            DisplayCurrentPosition = menuItemDisplayCurrentPosition.Checked;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Auto scroll with cursor" menu item.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public void menuItemAutoScrollWithCursor_Click(object sender, EventArgs e)
        {
            AutoScrollWithCursor = menuItemAutoScrollWithCursor.Checked;
        }

        /// <summary>            
        /// Occurs when the user clicks on the "Create marker at this position" menu item.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public void menuItemCreateMarkerAtThisPosition_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Load Wave Form / Read+Write Peak Files Methods

        protected void m_peakFile_OnProcessStarted(PeakFileStartedData data)
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Create history 
                m_waveDataHistory = new List<WaveDataMinMax>((int)data.TotalBlocks);
            };

            // Check if invoking is necessary
            if (InvokeRequired)
            {
                BeginInvoke(methodUIUpdate);
            }
            else
            {
                methodUIUpdate.Invoke();
            }
        }

        /// <summary>
        /// This event is called every 20 blocks during peak file generation.
        /// </summary>
        /// <param name="data">Peak file progress data</param>
        protected void m_peakFile_OnProcessData(PeakFileProgressData data)
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Add min/max data to history
                //WaveDataHistory.AddRange(data.MinMax);

                // Collection is being modified, so we can't iterate through it.
                List<WaveDataMinMax> minMaxs = data.MinMax;

                while (true)
                {
                    if (minMaxs.Count == 0)
                    {
                        break;
                    }

                    WaveDataHistory.Add(minMaxs[0]);
                    minMaxs.RemoveAt(0);
                }

                // Set percentage done
                m_percentageDone = data.PercentageDone;

                // Refresh control
                //Refresh();
            };

            // Check if invoking is necessary
            if (InvokeRequired)
            {
                BeginInvoke(methodUIUpdate);
            }
            else
            {
                methodUIUpdate.Invoke();
            }
        }

        /// <summary>
        /// This event is called when the peak file generation is done.
        /// </summary>
        /// <param name="data">Peak file done data</param>
        protected void m_peakFile_OnProcessDone(PeakFileDoneData data)
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Stop timer
                m_timer.Stop();
                m_isLoading = false;

                // Reset scrollbar
                m_scrollX = 0;

                // Do a last refresh
                needToRefreshBitmapCache = true;
                Refresh();
            };

            // Check if invoking is necessary
            if (InvokeRequired)
            {
                BeginInvoke(methodUIUpdate);
            }
            else
            {
                methodUIUpdate.Invoke();
            }
        }

        /// <summary>
        /// Load a new wave form into the control.
        /// </summary>
        /// <param name="filePath">File path of the audio file</param>
        public void LoadWaveForm(string filePath)
        {
            // Reset scroll
            m_scrollX = 0;
            horizontalScrollBar.Value = 0;
            horizontalScrollBar.Visible = false;

            // Check if the peak file exists                            
            m_peakFileDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Peak Files\\";
            
            // Check if the folder for peak files exists
            if (!Directory.Exists(PeakFileDirectory))
            {
                // Create directory
                Directory.CreateDirectory(PeakFileDirectory);
            }

            // Build peak file path
            string peakFilePath = PeakFileDirectory + filePath.Replace(@"\", "_").Replace(":", "_").Replace(".", "_") + ".mpfmPeak";

            // Check if the peak file exists
            if(File.Exists(peakFilePath))
            {
                // Load peaks from file                
                WaveDataHistory.Clear();
                m_waveDataHistory = m_peakFile.ReadPeakFile(peakFilePath);
                needToRefreshBitmapCache = true;
                Refresh();
                return;
            }

            // Set flags
            WaveDataHistory.Clear();
            m_isLoading = true;

            // Generate peak file and start timer for updating progress
            m_peakFile.GeneratePeakFile(filePath, peakFilePath);
            m_timer.Start();
        }

        /// <summary>
        /// Cancels the wave form loading operation.
        /// </summary>
        public void CancelWaveFormLoading()
        {
            // Cancel the operation asynchronously
            //m_workerWaveForm.CancelAsync();
            m_peakFile.Cancel();
        }

        #endregion

        #region Timer Events

        /// <summary>
        /// Occurs when the timer for refresh has expired.
        /// Refreshes the wave form during loading or displays the cursor.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void m_timer_Tick(object sender, EventArgs e)
        {
            // Invoke UI updates
            MethodInvoker methodUIUpdate = delegate
            {
                // Make sure the wave form is loading
                if (m_isLoading)
                {
                    // Refresh the whole control
                    Refresh();
                }
            };

            // Check if invoking is necessary
            if (InvokeRequired)
            {
                BeginInvoke(methodUIUpdate);
            }
            else
            {
                methodUIUpdate.Invoke();
            }
        }

        #endregion

        #region Paint Events

        /// <summary>
        /// Sets the position of the cursor and invalidates the cursor area.
        /// </summary>
        /// <param name="positionBytes">Position (in bytes)</param>
        /// <param name="positionTime">Position (in 00:00.000 string format)</param>
        public void SetPosition(long positionBytes, string positionTime)
        {
            // Set position properties
            m_position = positionBytes;
            m_positionTime = positionTime;

            // Make sure the peak file isn't generating
            if (m_isLoading)
            {
                return;
            }

            // Invalidate part of the control and update
            Rectangle rect = new Rectangle((int)m_cursorX - 50, 0, 100, ClientRectangle.Height);
            Invalidate(rect);
            Update();            
        }

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Declare variables
            int widthAvailable = 0;
            int heightAvailable = 0;
            float x1 = 0;
            float x2 = 0;
            float leftMin = 0;
            float leftMax = 0;
            float rightMin = 0;
            float rightMax = 0;
            float mixMin = 0;
            float mixMax = 0;
            float leftMaxHeight = 0;
            float leftMinHeight = 0;
            float rightMaxHeight = 0;
            float rightMinHeight = 0;
            float mixMaxHeight = 0;
            float mixMinHeight = 0;
            int historyIndex = 0;
            int historyCount = 0;
            float lineWidth = 0;
            float lineWidthPerHistoryItem = 0;
            int nHistoryItemsPerLine = 0;
            float desiredLineWidth = 0.5f;

            Rectangle rect;
            Color color;
            Graphics g = null;
            Pen pen = null;
            SolidBrush brush = null;
            LinearGradientBrush brushGradient = null;            
            WaveDataMinMax[] subset = null;            

            try
            {
                // Set scrollbar width
                if (horizontalScrollBar.Width != Width)
                {
                    horizontalScrollBar.Width = Width;
                }

                // If the wave data history is empty, show something
                if (WaveDataHistory.Count == 0)
                {
                    // Get graphics
                    g = pe.Graphics;

                    // Draw background
                    rect = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
                    brushGradient = new LinearGradientBrush(rect, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 50, 50, 50), 90.0f);
                    g.FillRectangle(brushGradient, rect);
                    brushGradient.Dispose();
                    brushGradient = null;

                    // Measure string
                    string feedback = "No peak information found.";
                    SizeF size = g.MeasureString(feedback, Font);

                    // Draw text in the middle of the control
                    g.DrawString(feedback, Font, Brushes.White, new PointF((Width / 2) - (size.Width / 2), (Height / 2) - (size.Height / 2)));

                    return;
                }

                // Determine the width and height of the wave form display area, and display the scrollbar if needed.
                // If the wave form is loading, do not display the scrollbar
                if (IsLoading)
                {
                    // Set full control area 
                    widthAvailable = Width;
                    heightAvailable = Height;

                    // Makre sure the scrollbar is not visible
                    //m_scrollBarVisible = false;
                    horizontalScrollBar.Visible = false;                    
                }
                // Check if the scrollbar needs to be visible (zoom superior to 100%)
                else if (Zoom > 100)
                {
                    // Set available size
                    widthAvailable = (int)(Width * (Zoom / 100));
                    heightAvailable = Height - 14;

                    // Set scrollbar properties
                    //m_scrollBarVisible = true;
                    horizontalScrollBar.Visible = true;
                    horizontalScrollBar.Location = new Point(0, Height - 14);

                    //// Show the scrollbar if it's not visible already
                    //if (!horizontalScrollBar.Visible)
                    //{
                    //    // Show scrollbar
                    //    //horizontalScrollBar.Visible = true;
                    //    //horizontalScrollBar.Width = Width;                        

                    //    // The bitmap needs to be redrawn because the display region has changed
                    //    needToRefreshBitmapCache = true;
                    //}
                }
                else
                {
                    // Set available size
                    widthAvailable = Width;
                    heightAvailable = Height;

                    // Set scrollbar properties
                    horizontalScrollBar.Visible = false;
                    //m_scrollBarVisible = false;

                    //// Check if the scrollbar is visible
                    //if (horizontalScrollBar.Visible)
                    //{
                    //    // Hide scrollbar and refresh bitmap cache
                    //    horizontalScrollBar.Visible = false;
                    //    needToRefreshBitmapCache = true;
                    //}
                }

                #region Generate Waveform Bitmap
                
                // Check if the wave form bitmap needs to be redrawn
                if (needToRefreshBitmapCache || IsLoading)
                {
                    // Set flag off
                    needToRefreshBitmapCache = false;

                    // Set maximum value (minimum = 0)
                    int maximum = widthAvailable - Width; // we dont want to scroll into black

                    // Set the large change at 1/20 of the maximum value
                    int largeChange = (int)((float)maximum / 20.0f);

                    // Set a minimum for large change
                    if (largeChange < 100)
                    {
                        largeChange = 100;
                    }

                    // Set the small change at 1/100 of the maximum value
                    int smallChange = (int)((float)maximum / 100.0f);

                    // Set a minimum for small change
                    if (smallChange < 10)
                    {
                        smallChange = 10;
                    }

                    // Set scrollbar values
                    horizontalScrollBar.Maximum = maximum;
                    horizontalScrollBar.LargeChange = largeChange;
                    horizontalScrollBar.SmallChange = smallChange;

                    // If another bitmap exists, clean it
                    if (m_bitmapWaveForm != null)
                    {
                        // Dispose of the bitmap and set it to null
                        m_bitmapWaveForm.Dispose();
                        m_bitmapWaveForm = null;
                    }

                    // Create bitmap buffer with the drawing zone size
                    m_bitmapWaveForm = new Bitmap(widthAvailable, heightAvailable);
                    g = Graphics.FromImage(m_bitmapWaveForm);                    

                    // Draw background gradient
                    Rectangle rectBackground = new Rectangle(0, 0, widthAvailable, heightAvailable);
                    brushGradient = new LinearGradientBrush(rectBackground, GradientColor1, GradientColor2, GradientMode);
                    g.FillRectangle(brushGradient, rectBackground);
                    brushGradient.Dispose();
                    brushGradient = null;

                    // Check if history is empty
                    if (WaveDataHistory.Count == 0)
                    {
                        return;
                    }

                    // Get the current history count (for multi-thread use)
                    historyCount = WaveDataHistory.Count;

                    // Find out how many samples are represented by each line of the wave form, depending on its width.
                    // For example, if the history has 45000 items, and the control has a width of 1000px, 45 items will need to be averaged by line.
                    //float historyItemsPerLine = (float)WaveDataHistory.Count / (float)Width;

                    lineWidthPerHistoryItem = (float)widthAvailable / (float)historyCount;

                    // Check if the line width is below the desired line width
                    if (lineWidthPerHistoryItem < desiredLineWidth)
                    {
                        // Try to get a line width around 0.5f so the precision is good enough and no artifacts will be shown.
                        while (lineWidth < desiredLineWidth)
                        {
                            // Increment the number of history items per line
                            nHistoryItemsPerLine++;
                            lineWidth += lineWidthPerHistoryItem;
                        }
                        nHistoryItemsPerLine--;
                        lineWidth -= lineWidthPerHistoryItem;
                    }
                    else
                    {
                        // The lines are larger than 0.5 pixels.
                        lineWidth = lineWidthPerHistoryItem;
                        nHistoryItemsPerLine = 1;
                    }

                    for (float i = 0; i < widthAvailable; i += lineWidth)
                    {
                        // Determine the maximum height of a line (+/-)
                        float heightToRenderLine = 0;
                        if (DisplayType == WaveFormDisplayType.Stereo)
                        {
                            heightToRenderLine = (float)heightAvailable / 4;
                        }
                        else
                        {
                            heightToRenderLine = (float)heightAvailable / 2;
                        }

                        // Determine x position
                        x1 = i;
                        x2 = i + lineWidth;

                        try
                        {
                            // Check if there are multiple history items per line
                            if (nHistoryItemsPerLine > 1)
                            {
                                if (historyIndex + nHistoryItemsPerLine > historyCount)
                                {
                                    // Create subset with remaining data
                                    subset = new WaveDataMinMax[historyCount - historyIndex];
                                    WaveDataHistory.CopyTo(historyIndex, subset, 0, historyCount - historyIndex);
                                }
                                else
                                {
                                    subset = new WaveDataMinMax[nHistoryItemsPerLine];
                                    WaveDataHistory.CopyTo(historyIndex, subset, 0, nHistoryItemsPerLine);
                                }

                                leftMin = AudioTools.GetMinPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Left);
                                leftMax = AudioTools.GetMaxPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Left);
                                rightMin = AudioTools.GetMinPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Right);
                                rightMax = AudioTools.GetMaxPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Right);
                                mixMin = AudioTools.GetMinPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Mix);
                                mixMax = AudioTools.GetMaxPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Mix);
                            }
                            else
                            {
                                leftMin = WaveDataHistory[historyIndex].leftMin;
                                leftMax = WaveDataHistory[historyIndex].leftMax;
                                rightMin = WaveDataHistory[historyIndex].rightMin;
                                rightMax = WaveDataHistory[historyIndex].rightMax;
                                mixMin = WaveDataHistory[historyIndex].mixMin;
                                mixMax = WaveDataHistory[historyIndex].mixMax;
                            }

                            // Increment history count
                            //historyCount += historyItemsPerLine;

                            leftMaxHeight = leftMax * heightToRenderLine;
                            leftMinHeight = leftMin * heightToRenderLine;
                            rightMaxHeight = rightMax * heightToRenderLine;
                            rightMinHeight = rightMin * heightToRenderLine;
                            mixMaxHeight = mixMax * heightToRenderLine;
                            mixMinHeight = mixMin * heightToRenderLine;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        // Create pen
                        pen = new Pen(new SolidBrush(WaveFormColor));

                        // Determine display type
                        if (DisplayType == WaveFormDisplayType.LeftChannel ||
                            DisplayType == WaveFormDisplayType.RightChannel ||
                            DisplayType == WaveFormDisplayType.Mix)
                        {
                            // Calculate min/max line height
                            float minLineHeight = 0;
                            float maxLineHeight = 0;

                            // Set mib/max
                            if (DisplayType == WaveFormDisplayType.LeftChannel)
                            {
                                minLineHeight = leftMinHeight;
                                maxLineHeight = leftMaxHeight;
                            }
                            else if (DisplayType == WaveFormDisplayType.RightChannel)
                            {
                                minLineHeight = rightMinHeight;
                                maxLineHeight = rightMaxHeight;
                            }
                            else if (DisplayType == WaveFormDisplayType.Mix)
                            {
                                minLineHeight = mixMinHeight;
                                maxLineHeight = mixMaxHeight;
                            }

                            // ------------------------
                            // Positive Max Value                   

                            // Draw positive value (y: middle to top)                   
                            g.DrawLine(pen, new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine - maxLineHeight));

                            // ------------------------
                            // Negative Max Value

                            // Draw negative value (y: middle to height)
                            g.DrawLine(pen, new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine + (-minLineHeight)));
                            pen.Dispose();
                        }
                        else if (DisplayType == WaveFormDisplayType.Stereo)
                        {
                            // -----------------------------------------
                            // LEFT Channel - Positive Max Value

                            // Draw positive value (y: middle to top)
                            g.DrawLine(pen, new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine - leftMaxHeight));

                            // -----------------------------------------
                            // LEFT Channel - Negative Max Value

                            // Draw negative value (y: middle to height)
                            g.DrawLine(pen, new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine + (-leftMinHeight)));

                            // -----------------------------------------
                            // RIGHT Channel - Positive Max Value

                            // Multiply by 3 to get the new center line for right channel
                            // Draw positive value (y: middle to top)
                            g.DrawLine(pen, new PointF(x1, (heightToRenderLine * 3)), new PointF(x2, (heightToRenderLine * 3) - rightMaxHeight));

                            // -----------------------------------------
                            // RIGHT Channel - Negative Max Value

                            // Draw negative value (y: middle to height)
                            g.DrawLine(pen, new PointF(x1, (heightToRenderLine * 3)), new PointF(x2, (heightToRenderLine * 3) + (-rightMinHeight)));
                        }

                        // Dispose pen
                        pen.Dispose();
                        pen = null;

                        // Increment the history index; pad the last values if the count is about to exceed
                        if (historyIndex < historyCount - 1)
                        {
                            // Increment by the number of history items per line
                            historyIndex += nHistoryItemsPerLine;
                        }
                    }

                    // Dispose objects
                    g.Dispose();
                    g = null;
                }

                #endregion

                // Get the current position in percentage
                float positionPercentage = (float)Position / (float)Length;
                float xCursor = (positionPercentage * widthAvailable) - ScrollX;

                // Check if auto scroll is enabled
                if (AutoScrollWithCursor && Zoom > 100)
                {
                    // Check if the cursor is near the right corner of the control
                    if (xCursor > Width - 10)
                    {
                        // Adjust scroll value
                        if (m_scrollX == 0)
                        {
                            m_scrollX = xCursor;
                            //horizontalScrollBar.Value = (int)xCursor;
                        }
                        else
                        {
                            m_scrollX = xCursor + Width - 15;
                            //horizontalScrollBar.Value = (int)m_scrollX;
                        }

                        // Recalculate cursor x
                        xCursor = (positionPercentage * widthAvailable) - ScrollX;
                    }
                }

                // Set cursor X
                m_cursorX = xCursor;

                // Draw bitmap for control
                Bitmap bmp = new Bitmap(Bounds.Width, Bounds.Height);
                g = Graphics.FromImage(bmp);                

                // Draw wave form bitmap                
                g.DrawImage(m_bitmapWaveForm, new Rectangle(0, 0, Width, heightAvailable), (int)ScrollX, 0, Width, heightAvailable, GraphicsUnit.Pixel);                

                #region Custom Font / Anti-Aliasing
                
                // Set text anti-aliasing to ClearType (best looking AA)
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Set smoothing mode for paths
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Load custom font
                Font font = this.Font;
                try
                {
                    if (FontCollection != null && CustomFontName.Length > 0)
                    {
                        FontFamily family = FontCollection.GetFontFamily(CustomFontName);

                        if (family != null)
                        {
                            font = new Font(family, Font.Size, Font.Style);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                #endregion

                // Is the wave form loading in a background thread?
                if (IsLoading)
                {
                    #region Loading Overlay
                    
                    // Draw shadow over the whole control
                    Color colorTransparent = Color.FromArgb(90, 0, 0, 0);
                    SolidBrush brushTransparent = new SolidBrush(colorTransparent);
                    g.FillRectangle(brushTransparent, ClientRectangle);
                    brushTransparent.Dispose();
                    brushTransparent = null;

                    // Display loading in the center of the control
                    string loadingText = "Generating wave form (" + PercentageDone.ToString("00.00") + "% done)";

                    // Measure string and calculate x,y position
                    SizeF sizeText = g.MeasureString(loadingText, font);
                    float x = (ClientRectangle.Width / 2) - (sizeText.Width / 2);
                    float y = (ClientRectangle.Height / 2) - (sizeText.Height / 2);

                    // Draw semi transparent background                    
                    color = Color.FromArgb(175, 0, 0, 0);
                    brush = new SolidBrush(color);
                    g.FillRectangle(brush, new RectangleF(x - 3, y - 3, sizeText.Width + 4, sizeText.Height + 4));
                    brush.Dispose();
                    brush = null;

                    // Draw string
                    g.DrawString(loadingText, font, Brushes.White, new PointF(x, y));

                    #endregion
                }
                else
                {
                    // -----------------------------------------------------
                    // Cursor Line

                    #region Cursor Line / Position
                    
                    // The control has finished loading; display position
                   
                    // Draw cursor line
                    pen = new Pen(CursorColor);
                    g.DrawLine(pen, new PointF(xCursor, 0), new PointF(xCursor + 1, Height));
                    pen.Dispose();
                    pen = null;

                    // Check if the time display needs to be drawn
                    if (DisplayCurrentPosition)
                    {
                        // Measure string
                        SizeF sizeText = g.MeasureString(PositionTime, Font);

                        // Check if there's enough space at the left of the cursor to display the time
                        float x = 0;
                        if (xCursor < sizeText.Width)
                        {
                            // Display the time string at the right of the cursor
                            x = xCursor;
                        }
                        else
                        {
                            // Display the time string at the left of the cursor
                            x = xCursor - sizeText.Width - 4;
                        }

                        // Draw position background
                        color = Color.FromArgb(200, CursorColor);
                        brush = new SolidBrush(color);
                        g.FillRectangle(brush, new RectangleF(x, 0, sizeText.Width + 4, sizeText.Height + 4));
                        brush.Dispose();
                        brush = null;

                        // Draw text
                        color = Color.FromArgb(255, Color.White);
                        brush = new SolidBrush(color);
                        g.DrawString(PositionTime, Font, brush, new PointF(x + 2, 2));
                        brush.Dispose();
                        brush = null;
                    }

                    #endregion                    

                    // -----------------------------------------------------
                    // ScrollBar

                    #region ScrollBar
                    
                    //if (ScrollBarVisible)
                    //{
                    //    // Set left handle colors
                    //    Color color1 = Color.FromArgb(75, 75, 75, 75);
                    //    Color color2 = Color.FromArgb(125, 125, 125, 125);
                    //    if (isMouseOverScrollBarLeftHandle)
                    //    {
                    //        color1 = Color.FromArgb(100, 125, 125, 125);
                    //        color2 = Color.FromArgb(200, 175, 175, 175);
                    //    }

                    //    // Render left handle
                    //    rectScrollBarLeftHandle = new Rectangle(0, Height - 14, 14, 14);
                    //    brushGradient = new LinearGradientBrush(rectScrollBarLeftHandle, color1, color2, 90.0f);
                    //    g.FillRectangle(brushGradient, rectScrollBarLeftHandle);
                    //    g.DrawImageUnscaled(MPfm.WindowsControls.Properties.Resources.scrollbar_arrow_left, new Point(-1, Height - 14));
                    //    brushGradient.Dispose();
                    //    brushGradient = null;

                    //    // Set left handle colors
                    //    color1 = Color.FromArgb(75, 75, 75, 75);
                    //    color2 = Color.FromArgb(125, 125, 125, 125);
                    //    if (isMouseOverScrollBarRightHandle)
                    //    {
                    //        color1 = Color.FromArgb(100, 125, 125, 125);
                    //        color2 = Color.FromArgb(200, 175, 175, 175);
                    //    }

                    //    // Render right handle
                    //    rectScrollBarRightHandle = new Rectangle(Width - 14, Height - 14, 14, 14);
                    //    brushGradient = new LinearGradientBrush(rectScrollBarRightHandle, color1, color2, 90.0f);
                    //    g.FillRectangle(brushGradient, rectScrollBarRightHandle);
                    //    g.DrawImageUnscaled(MPfm.WindowsControls.Properties.Resources.scrollbar_arrow_right, new Point(Width - 14 - 1, Height - 14 - 2));
                    //    brushGradient.Dispose();
                    //    brushGradient = null;                       

                    //    // Render scrollbar main background
                    //    rect = new Rectangle(14, Height - 14, Width - 28, 14);
                    //    brushGradient = new LinearGradientBrush(rect, Color.FromArgb(75, 75, 75, 75), Color.FromArgb(200, 100, 100, 100), 90.0f);
                    //    g.FillRectangle(brushGradient, rect);
                    //    brushGradient.Dispose();
                    //    brushGradient = null;

                    //    // Render thumb
                    //    color1 = Color.FromArgb(75, 175, 175, 175);
                    //    color2 = Color.FromArgb(200, 200, 200, 200);
                    //    if (isMouseOverScrollBarThumb)
                    //    {
                    //        color1 = Color.FromArgb(125, 175, 175, 175);
                    //        color2 = Color.FromArgb(255, 200, 200, 200);                            
                    //    }
                    //    float thumbWidth = ((float)Width / (float)widthAvailable);
                    //    thumbWidth = thumbWidth * (Width - 28);
                    //    rectScrollBarThumb = new Rectangle(14, Height - 14, (int)thumbWidth, 14);
                    //    brushGradient = new LinearGradientBrush(rectScrollBarThumb, color1, color2, 90.0f);
                    //    g.FillRectangle(brushGradient, rectScrollBarThumb);
                    //    brushGradient.Dispose();
                    //    brushGradient = null;

                    //}

                    #endregion

                    // -----------------------------------------------------
                    // Zoom overlay animation

                    #region Zoom Overlay
                    
                    // Check if an animation needs to start
                    if (animZoomCount == 0)
                    {
                        // Do the first step
                        animZoomCount = 1;
                    }
                    else if(animZoomCount > 0)
                    {
                        // If the counter is beyond 128, it means the animation is done. (1.28 seconds)
                        if(animZoomCount > 128)
                        {
                            // Stop animation by reseting count
                            animZoomCount = -1;
                        }
                        else
                        {
                            // Variables
                            int alpha = 0;

                            // Measure "Zoom: x%" string
                            string zoomText = "Zoom: " + Zoom.ToString() + "%";
                            SizeF sizeText = g.MeasureString(zoomText, Font);                            

                            // Determine where to display the zoom overlay
                            int zoomTextY = Height - (int)sizeText.Height - 6;
                            if (Zoom > 100)
                            {
                                // Add the scrollbar
                                zoomTextY = zoomTextY - 14;
                            }

                            // Create the overlay rectangle (add 6px padding)
                            Rectangle rectZoomOverlay = new Rectangle(0, zoomTextY, (int)sizeText.Width + 6, (int)sizeText.Height + 6);

                            // Get the transparency colors we need to draw the overlay
                            Color colorOverlay = Color.Empty;
                            Color colorText = Color.Empty;

                            // Fill the colors depending on the current stage of the animation
                            if(animZoomCount > 0 && animZoomCount <= 32)
                            {
                                // Step 1: Start to show the overlay

                                // Create alpha value for background transparency
                                alpha = animZoomCount * 5;
                                if (alpha > 255)
                                {
                                    alpha = 255;
                                }
                                colorOverlay = Color.FromArgb(alpha, 75, 75, 75);

                                // Create alpha value for text transparency
                                alpha = animZoomCount * 8;
                                if (alpha > 255)
                                {
                                    alpha = 255;
                                }
                                colorText = Color.FromArgb(alpha, Color.White);
                            }
                            else if (animZoomCount > 32 && animZoomCount <= 96)
                            {
                                // Step 2: Overlay is shown for 640ms

                                // Create alpha value for background transparency
                                colorOverlay = Color.FromArgb(160, 75, 75, 75);

                                // Create alpha value for text transparency
                                colorText = Color.White;
                            }
                            else if (animZoomCount > 96 && animZoomCount <= 128)
                            {
                                // Step 3: Overlay starts to fade

                                // Create alpha value for background transparency
                                alpha = 160 - ((animZoomCount - 96) * 5);
                                if (alpha < 0)
                                {
                                    alpha = 0;
                                }
                                if (alpha > 0)
                                {
                                    colorOverlay = Color.FromArgb(alpha, 75, 75, 75);
                                }

                                // Create alpha value for text transparency
                                alpha = 255 - ((animZoomCount - 96) * 8);
                                if (alpha < 0)
                                {
                                    alpha = 0;
                                }
                                if (alpha > 0)
                                {
                                    colorText = Color.FromArgb(alpha, Color.White);
                                }
                            }

                            // Display the overlay (x% transparency)                                
                            brush = new SolidBrush(colorOverlay);
                            g.FillRectangle(brush, rectZoomOverlay);
                            brush.Dispose();
                            brush = null;

                            // Display text (0% transparency)
                            brush = new SolidBrush(colorText);
                            g.DrawString(zoomText, Font, brush, new Point(1, zoomTextY + 2));
                            brush.Dispose();
                            brush = null;

                            // Increment counter
                            animZoomCount++;
                        }
                    }

                    #endregion

                    // -----------------------------------------------------
                    // Toolbar / overlay animation                                   

                    Rectangle rectToolbar = new Rectangle(0, 0, 80, 20);

                    if (isMouseOverToolbar)
                    {
                        // Variables
                        int alpha = 0;
                        Color colorOverlay = Color.FromArgb(200, 75, 75, 75);
                        Color colorOverlaySelected = Color.FromArgb(200, 125, 125, 125);
                        Image imgPointer = MPfm.WindowsControls.Properties.Resources.pointer;
                        Image imgSelect = MPfm.WindowsControls.Properties.Resources.select;
                        Image imgZoomIn = MPfm.WindowsControls.Properties.Resources.zoom_in;
                        Image imgZoomOut = MPfm.WindowsControls.Properties.Resources.zoom_out;

                        //// Check if an animation needs to start
                        //if (animToolbarCount == 0)
                        //{
                        //    // Do the first step
                        //    animToolbarCount = 1;
                        //}
                        //else if (animToolbarCount > 0)
                        //{
                        //    // If the counter is beyond 128, it means the animation is done. (1.28 seconds)
                        //    if (animToolbarCount > 32)
                        //    {
                        //        // Stop animation by reseting count
                        //        animToolbarCount = -1;
                        //    }
                        //    else
                        //    {
                        //        // Fill the colors depending on the current stage of the animation
                        //        if (animToolbarCount > 0 && animToolbarCount <= 32)
                        //        {
                        //            // Step 1: Toolbar fades in

                        //            // Create alpha value for background transparency
                        //            alpha = animToolbarCount * 5;
                        //            if (alpha > 255)
                        //            {
                        //                alpha = 255;
                        //            }
                        //            colorOverlay = Color.FromArgb(alpha, 75, 75, 75);
                        //            colorOverlaySelected = Color.FromArgb(alpha, 125, 125, 125);

                        //            // Set image opacity
                        //            imgPointer = Tools.SetImageOpacity(MPfm.WindowsControls.Properties.Resources.pointer, alpha);
                        //            imgSelect = Tools.SetImageOpacity(MPfm.WindowsControls.Properties.Resources.select, alpha);
                        //            imgZoomIn = Tools.SetImageOpacity(MPfm.WindowsControls.Properties.Resources.zoom_in, alpha);
                        //            imgZoomOut = Tools.SetImageOpacity(MPfm.WindowsControls.Properties.Resources.zoom_out, alpha);

                        //        }
                        //        //else if (animToolbarCount > 32 && animToolbarCount <= 96)
                        //        //{
                        //        //    // Step 2: Toolbar is shown
                        //        //}
                        //        //else if (animToolbarCount > 96 && animToolbarCount <= 128)
                        //        //{
                        //        //    // Step 3: If the user does not move the mouse for a long time, the toolbar starts to
                        //        //    //         fade away (to leave space for selecting the wave form)
                        //        //}

                        //        // Increment counter
                        //        animToolbarCount++;
                        //    }
                        //}

                        // Draw toolbar background
                        brush = new SolidBrush(colorOverlay);
                        g.FillRectangle(brush, rectToolbar);
                        brush.Dispose();
                        brush = null;

                        // Check if the background of the button needs to be drawn
                        if (MouseInteractionType == WaveFormMouseInteractionType.Pointer)
                        {
                            // Draw currently selected button background
                            brush = new SolidBrush(colorOverlaySelected);
                            g.FillRectangle(brush, new Rectangle(0, 0, 20, 20));
                            g.DrawRectangle(Pens.Gray, new Rectangle(0, 0, 20, 20));
                            brush.Dispose();
                            brush = null;
                        }

                        // Check if the background of the button needs to be drawn
                        if (MouseInteractionType == WaveFormMouseInteractionType.Select)
                        {
                            // Draw currently selected button background
                            brush = new SolidBrush(colorOverlaySelected);
                            g.FillRectangle(brush, new Rectangle(20, 0, 20, 20));
                            g.DrawRectangle(Pens.Gray, new Rectangle(20, 0, 20, 20));
                            brush.Dispose();
                            brush = null;
                        }

                        // Check if the background of the button needs to be drawn
                        if (MouseInteractionType == WaveFormMouseInteractionType.ZoomIn)
                        {
                            // Draw currently selected button background
                            brush = new SolidBrush(colorOverlaySelected);
                            g.FillRectangle(brush, new Rectangle(40, 0, 20, 20));
                            g.DrawRectangle(Pens.Gray, new Rectangle(40, 0, 20, 20));
                            brush.Dispose();
                            brush = null;
                        }

                        // Check if the background of the button needs to be drawn
                        if (MouseInteractionType == WaveFormMouseInteractionType.ZoomOut)
                        {
                            // Draw currently selected button background
                            brush = new SolidBrush(colorOverlaySelected);
                            g.FillRectangle(brush, new Rectangle(60, 0, 20, 20));
                            g.DrawRectangle(Pens.Gray, new Rectangle(60, 0, 20, 20));
                            brush.Dispose();
                            brush = null;
                        }

                        // Pointer button
                        //bmpPointer = new Bitmap(MPfm.WindowsControls.Properties.Resources.pointer);
                        g.DrawImage(imgPointer, new Rectangle(3, 2, 16, 16));
                        imgPointer.Dispose();
                        imgPointer = null;

                        // Select button
                        //Bitmap bmpSelect = new Bitmap(MPfm.WindowsControls.Properties.Resources.select);
                        g.DrawImage(imgSelect, new Rectangle(20 + 2, 1, 16, 16));
                        imgSelect.Dispose();
                        imgSelect = null;

                        // Zoom button
                        //Bitmap bmpZoomIn = new Bitmap(MPfm.WindowsControls.Properties.Resources.zoom_in);
                        g.DrawImage(imgZoomIn, new Rectangle(40 + 3, 2, 16, 16));
                        imgZoomIn.Dispose();
                        imgZoomIn = null;

                        // Zoom button
                        //Bitmap bmpZoomOut = new Bitmap(MPfm.WindowsControls.Properties.Resources.zoom_out);
                        g.DrawImage(imgZoomOut, new Rectangle(61 + 2, 2, 16, 16));
                        imgZoomOut.Dispose();
                        imgZoomOut = null;
                    }

                    //g.DrawString("ScrollX: " + ScrollX + " / " + horizontalScrollBar.Maximum.ToString(), Font, Brushes.White, new Point(0, (Height / 2) - 6));

                    //g.DrawString(CurrentPosition.ToString() + " / " + TotalBytes.ToString(), Font, Brushes.White, new Point(1, 1));
                    //g.DrawString(positionPercentage.ToString(), Font, Brushes.White, new Point(1, 20));
                }                

                // Dispose custom font if needed
                if (font != null)
                {
                    font.Dispose();
                    font = null;
                }               

                // Draw bitmap on control
                pe.Graphics.DrawImage(bmp, 0, 0, ClientRectangle, GraphicsUnit.Pixel);
                bmp.Dispose();
                bmp = null;
                g.Dispose();
                g = null;                
            }
            catch (Exception ex)
            {
                // Display error in control
                pe.Graphics.Clear(Color.Black);
                pe.Graphics.DrawString("An error has occured: " + ex.Message, Font, Brushes.White, new Point(1, 1));
            }
        }

        #endregion

        #region Mouse Events
        
        /// <summary>
        /// Occurs when the mouse pointer is moving over the control.
        /// Manages the display of mouse on/off visual effects.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Check if the mouse cursor is over the toolbar
            if (e.X <= 100 && e.Y <= 24)
            {
                if (!isMouseOverToolbar)
                {
                    animToolbarCount = 0;
                    isMouseOverToolbar = true;
                    Refresh();
                }
            }
            else
            {
                isMouseOverToolbar = false;
            }

            //// Check if the mouse cursor is over the scrollbar 
            //if (Zoom > 100 && e.Y >= Height - 14)
            //{
            //    bool needToRefresh = false;

            //    // Check if the cursor is on the left handle
            //    if (e.X >= rectScrollBarLeftHandle.X &&
            //        e.Y >= rectScrollBarLeftHandle.Y &&
            //        e.X <= rectScrollBarLeftHandle.X + rectScrollBarLeftHandle.Width &&
            //        e.Y <= rectScrollBarLeftHandle.Y + rectScrollBarLeftHandle.Height)
            //    {
            //        isMouseOverScrollBarLeftHandle = true;
            //        needToRefresh = true;
            //    }

            //    // Check if the cursor is on the left handle
            //    if (e.X >= rectScrollBarRightHandle.X &&
            //        e.Y >= rectScrollBarRightHandle.Y &&
            //        e.X <= rectScrollBarRightHandle.X + rectScrollBarRightHandle.Width &&
            //        e.Y <= rectScrollBarRightHandle.Y + rectScrollBarRightHandle.Height)
            //    {
            //        isMouseOverScrollBarRightHandle = true;
            //        needToRefresh = true;
            //    }

            //    // Check if the cursor is on the thumb
            //    if (e.X >= rectScrollBarThumb.X &&
            //        e.Y >= rectScrollBarThumb.Y &&
            //        e.X <= rectScrollBarThumb.X + rectScrollBarThumb.Width &&
            //        e.Y <= rectScrollBarThumb.Y + rectScrollBarThumb.Height &&
            //        !isMouseOverScrollBarThumb)
            //    {
            //        isMouseOverScrollBarThumb = true;
            //        needToRefresh = true;
            //    }

            //    // Check if the control needs to be refreshed
            //    if (needToRefresh)
            //    {
            //        Refresh();
            //    }
            //}
            //else
            //{
            //    // The mouse isn't over any of these elements
            //    isMouseOverScrollBarThumb = false;
            //    isMouseOverScrollBarLeftHandle = false;
            //    isMouseOverScrollBarRightHandle = false;
            //}
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Occurs when the mouse cursor leaves the control.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            // Check if any element needs to be refreshed
            bool needToRefresh = false;
            if (isMouseOverToolbar)
            {
                isMouseOverToolbar = false;
                needToRefresh = true;
            }
            //if (isMouseOverScrollBarThumb)
            //{
            //    isMouseOverScrollBarThumb = false;
            //    needToRefresh = true;
            //}
            //if (isMouseOverScrollBarLeftHandle)
            //{
            //    isMouseOverScrollBarLeftHandle = false;
            //    needToRefresh = true;
            //}
            //if (isMouseOverScrollBarRightHandle)
            //{
            //    isMouseOverScrollBarRightHandle = false;
            //    needToRefresh = true;
            //}

            // Refresh if needed
            if (needToRefresh)
            {
                Refresh();
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Occurs when the user is pressing down a mouse button.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Occurs when the user releases a mouse button.
        /// Manages the clicking on the different areas of the control.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // If the history is empty, cancel any mouse action
            if (WaveDataHistory.Count == 0)
            {
                return;
            }

            // Check if wave form is generating
            if (IsLoading || Length == 0)
            {
                // No mouse interaction
                return;
            }

            // In any case, we don't handle the right mouse button because there's a contextual menu
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                return;
            }

            // Check if the user has clicked on the toolbar
            if (e.X <= 80 && e.Y <= 20)
            {
                // Reset menu items
                menuItemMouseInteractionTypePointer.Checked = false;
                menuItemMouseInteractionTypeSelect.Checked = false;
                menuItemMouseInteractionTypeZoomIn.Checked = false;
                menuItemMouseInteractionTypeZoomOut.Checked = false;

                if (e.X >= 0 && e.X <= 20)
                {
                    // Set type and default cursor
                    m_mouseInteractionType = WaveFormMouseInteractionType.Pointer;
                    this.Cursor = Cursors.Default;
                    menuItemMouseInteractionTypePointer.Checked = true;
                }
                else if (e.X >= 20 && e.X <= 40)
                {
                    // Set type and cursor
                    m_mouseInteractionType = WaveFormMouseInteractionType.Select;
                    this.Cursor = Cursors.Cross;
                    menuItemMouseInteractionTypeSelect.Checked = true;
                }
                else if (e.X >= 40 && e.X <= 60)
                {
                    // Set type and custom cursor
                    m_mouseInteractionType = WaveFormMouseInteractionType.ZoomIn;
                    this.Cursor = Tools.CreateCursor(MPfm.WindowsControls.Properties.Resources.zoom_in, 2, 2);
                    menuItemMouseInteractionTypeZoomIn.Checked = true;
                }
                else if (e.X >= 60 && e.X <= 80)
                {
                    // Set type and custom cursor
                    m_mouseInteractionType = WaveFormMouseInteractionType.ZoomOut;
                    this.Cursor = Tools.CreateCursor(MPfm.WindowsControls.Properties.Resources.zoom_out, 2, 2);
                    menuItemMouseInteractionTypeZoomOut.Checked = true;
                }

                return;
            }

            //// Check if the user has clicked on the scrollbar
            //if (Zoom > 100 && e.Y >= Height - 14)
            //{
            //    // Check if the cursor is on the left handle
            //    if (e.X >= rectScrollBarLeftHandle.X &&
            //        e.Y >= rectScrollBarLeftHandle.Y &&
            //        e.X <= rectScrollBarLeftHandle.X + rectScrollBarLeftHandle.Width &&
            //        e.Y <= rectScrollBarLeftHandle.Y + rectScrollBarLeftHandle.Height)
            //    {
            //        //isMouseOverScrollBarThumb = true;
            //        //Refresh();                   
            //    }

            //    // Check if the cursor is on the left handle
            //    if (e.X >= rectScrollBarRightHandle.X &&
            //        e.Y >= rectScrollBarRightHandle.Y &&
            //        e.X <= rectScrollBarRightHandle.X + rectScrollBarRightHandle.Width &&
            //        e.Y <= rectScrollBarRightHandle.Y + rectScrollBarRightHandle.Height)
            //    {

            //    }

            //    // Check if the cursor is on the thumb
            //    if (e.X >= rectScrollBarThumb.X &&
            //        e.Y >= rectScrollBarThumb.Y &&
            //        e.X <= rectScrollBarThumb.X + rectScrollBarThumb.Width &&
            //        e.Y <= rectScrollBarThumb.Y + rectScrollBarThumb.Height)
            //    {
            //        //isMouseOverScrollBarThumb = true;
            //        //Refresh();                   
            //    }

            //    return;
            //}
            //else
            //{

            //}

            // -------------------------------------------------------------
            // Pointer

            // Check if the interaction type is Pointer
            if (MouseInteractionType == WaveFormMouseInteractionType.Pointer)
            {
                // The user has clicked the left mouse button to skip position; calculate new position  

                // Make sure the x doesn't go under 0
                int x = e.X;
                if (x < 0)
                {
                    x = 0;
                }

                float ratioCursor = (float)x / (float)Width;
                float ratioWidthAvailable = (float)Width / (float)m_bitmapWaveForm.Size.Width;                    
                float percentageCursor = (ratioCursor * ratioWidthAvailable) + (ScrollX / (float)m_bitmapWaveForm.Size.Width);

                //uint position = (uint)(percentageCursor * (float)TotalPCMBytes);

                // Check if an event is subscribed
                if (OnPositionChanged != null)
                {
                    // Create data
                    PositionChangedData data = new PositionChangedData();
                    //data.Position = position;
                    data.Percentage = percentageCursor * 100;

                    // Raise event
                    OnPositionChanged(data);
                }                               
            }

            // -------------------------------------------------------------
            // Zoom In / Zoom Out

            else if (MouseInteractionType == WaveFormMouseInteractionType.ZoomIn ||
                     MouseInteractionType == WaveFormMouseInteractionType.ZoomOut)
            {
                // Check which mouse button has been clicked
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    // Left button; zoom in or out
                    if (MouseInteractionType == WaveFormMouseInteractionType.ZoomIn)
                    {
                        // Check if zoom is already 1800; we can't zoom in further
                        if (Zoom == 1600)
                        {
                            return;
                        }

                        // Zoom in
                        Zoom = Zoom + 100;
                        if (Zoom > 1600)
                        {
                            Zoom = 1600;
                        }
                    }
                    else
                    {
                        // Check if zoom is already 100; we can't zoom out
                        if (Zoom == 100)
                        {
                            return;
                        }

                        // Zoom out
                        Zoom = Zoom - 100;
                        if (Zoom < 100)
                        {
                            Zoom = 100;
                        }
                    }
                }

                // Make sure all menu items are unchecked
                menuItemZoom100.Checked = false;
                menuItemZoom200.Checked = false;
                menuItemZoom400.Checked = false;
                menuItemZoom800.Checked = false;
                menuItemZoom1600.Checked = false;

                // Check which menu item to check
                if (Zoom == 100)
                {
                    menuItemZoom100.Checked = true;
                }
                else if (Zoom == 200)
                {
                    menuItemZoom200.Checked = true;
                }
                else if (Zoom == 400)
                {
                    menuItemZoom400.Checked = true;
                }
                else if (Zoom == 800)
                {
                    menuItemZoom800.Checked = true;
                }
                else if (Zoom == 1600)
                {
                    menuItemZoom1600.Checked = true;
                }
                else
                {
                    menuItemZoomCustom.Checked = true;                    
                }

                // Reset animation count        
                animZoomCount = 0;

                // Refresh wave form
                needToRefreshBitmapCache = true;
                Refresh();
            }

            base.OnMouseUp(e);
        }

        #endregion

        #region Other Events

        /// <summary>
        /// Triggers when the user resizes the control.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnResize(EventArgs e)
        {
            // Raise flag to refresh bitmap cache
            needToRefreshBitmapCache = true;
            base.OnResize(e);
        }

        /// <summary>
        /// Triggers when the scrollbar value has changed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public void horizontalScrollBar_OnValueChanged(object sender, EventArgs e)
        {
            // Set scroll value
            m_scrollX = horizontalScrollBar.Value;
        }

        #endregion

    }

    /// <summary>
    /// Defines the data structure for the end-of-song event.
    /// </summary>
    public class PositionChangedData
    {
        public uint Position { get; set; }
        public float Percentage { get; set; }
    }

    /// <summary>
    /// Defines the type of mouse interaction with the wave form display control.    
    /// </summary>
    public enum WaveFormMouseInteractionType
    {
        /// <summary>
        /// The user can skip to the position he/she chooses by clicking at the.
        /// The user can also resize loops and move markers.
        /// </summary>
        Pointer = 0, 
        /// <summary>
        /// The user defines a portion of the wave form to zoom in.
        /// </summary>
        ZoomIn = 1,
        /// <summary>
        /// The user defines a portion of the wave form to zoom out.
        /// </summary>
        ZoomOut = 2,
        /// <summary>
        /// The user can select portions of the wave form.
        /// </summary>
        Select = 3
    }

    /// <summary>
    /// Defines the data structure for reporting progress when generating the wave form.    
    /// </summary>
    public class WorkerWaveFormProgress
    {
        public uint BytesRead { get; set; }
        public uint TotalBytes { get; set; }
        public float PercentageDone { get; set; }
        public WaveDataMinMax WaveDataMinMax { get; set; }
    }

    /// <summary>
    /// Defines the data structure passed to the background worker for generating waveforms.
    /// </summary>
    public class WorkerWaveFormArgument
    {
        public string AudioFilePath { get; set; }
        public string PeakFilePath { get; set; }
    }
}
