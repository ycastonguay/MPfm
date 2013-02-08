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
using System.Windows.Forms;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Themes window. This is where the user can create and edit themes.   
    /// </summary>
    public partial class frmThemes : MPfm.WindowsControls.Form
    {
        // Private variables
        private Theme theme = null;
        //private MainWindowTheme mainWindowTheme = null;
        //private SecondaryWindowTheme secondaryWindowTheme = null;
        private string filePath = string.Empty;            
                
        /// <summary>
        /// Private value for the Main property.
        /// </summary>
        private frmMain main = null;
        /// <summary>
        /// Hook to the main form.
        /// </summary>
        public frmMain Main
        {
            get
            {
                return main;
            }
        }

        /// <summary>
        /// Constructor for Themes window. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmThemes(frmMain main)
        {
            InitializeComponent();
            this.main = main;
            this.theme = new Theme();
        }

        #region Form Events

        /// <summary>
        /// Occurs when the form has loaded.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmThemes_Load(object sender, EventArgs e)
        {
            // Add theme sections
            List<ThemePreviewPaneComboBoxItem> themeControls = new List<ThemePreviewPaneComboBoxItem>();
            themeControls.Add(new ThemePreviewPaneComboBoxItem("Main Window", "MainWindowTheme"));
            themeControls.Add(new ThemePreviewPaneComboBoxItem("Output Meter", "OutputMeterTheme"));
            themeControls.Add(new ThemePreviewPaneComboBoxItem("Song Browser", "SongGridViewTheme"));
            themeControls.Add(new ThemePreviewPaneComboBoxItem("Wave Form Display", "WaveFormDisplayTheme"));
            themeControls.Add(new ThemePreviewPaneComboBoxItem("Faders", "FaderTheme"));
            comboPreviewPane.DataSource = themeControls;
            comboPreviewPane.SelectedIndex = 0;

            // Load sample data into grid
            List<AudioFile> audioFiles = new List<AudioFile>();
            for (int a = 0; a < 20; a++)
            {
                // Create audio file
                AudioFile audioFile = new AudioFile(@"file://", Guid.NewGuid(), false);
                audioFile.TrackNumber = (uint)a + 1;
                audioFile.Length = "10:23.450";
                audioFile.ArtistName = "Artist Name";
                audioFile.AlbumTitle = "Album Title";
                audioFile.Title = "Song Title #" + (a + 1).ToString();

                // Add to list
                audioFiles.Add(audioFile);
            }

            // Set now playing song
            previewSongGridView.NowPlayingAudioFileId = audioFiles[0].Id;

            // Load into control
            previewSongGridView.Theme = Main.viewSongs2.Theme;
            previewSongGridView.ImportAudioFiles(audioFiles);

            // Set column widths
            previewSongGridView.Columns[0].Width = 100;

            // Refresh preview
            //mainWindowTheme = new MainWindowTheme();
            //secondaryWindowTheme = new SecondaryWindowTheme();
            RefreshMainWindowPreview();
        }

        /// <summary>
        /// Occurs when the form becomes visible (each time the user presses on the
        /// Themes button on the main form toolbar).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmThemes_Shown(object sender, EventArgs e)
        {
            // Refresh property grid and preview pane.
            RefreshPropertyGrid();
        }

        #endregion

        #region Close Events

        /// <summary>
        /// Occurs when the form is about to close.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                this.Hide();
            }

            Main.btnThemes.Checked = false;
        }

        #endregion

        /// <summary>
        /// Occurs when the user clicks on the New Theme button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnNewTheme_Click(object sender, EventArgs e)
        {
            // Warn user that this will overwrite the current theme
            if (MessageBox.Show("Are you sure you wish to create a new theme? You will lose the current theme properties.", "Create a new theme confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            // Reset themes
            previewOutputMeter.Theme = new OutputMeterTheme();
            previewSongGridView.Theme = new SongGridViewTheme();

            // Refresh controls
            previewOutputMeter.Refresh();
            previewSongGridView.Refresh();
        }

        /// <summary>
        /// Occurs when the user clicks on the Load Theme button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnLoadTheme_Click(object sender, EventArgs e)
        {
            // Display save theme dialog
            if (dialogLoadTheme.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            // Load theme
            theme = ThemeHelper.Load(dialogLoadTheme.FileName);
            RefreshMainWindowPreview();
        }

        /// <summary>
        /// Occurs when the user clicks on the Save Theme button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSaveTheme_Click(object sender, EventArgs e)
        {
            // Display save theme dialog
            if (dialogSaveTheme.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            // Save theme
            ThemeHelper.Save(dialogSaveTheme.FileName, theme);
        }

        /// <summary>
        /// Occurs when the user clicks on the Apply Theme button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnApplyTheme_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Occurs when the user changes a property in the Theme property grid.
        /// </summary>
        /// <param name="s">Object</param>
        /// <param name="e">Event arguments</param>
        private void propertyGridTheme_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // Get item
            ThemePreviewPaneComboBoxItem themeControl = (ThemePreviewPaneComboBoxItem)comboPreviewPane.SelectedItem;

            // Check for theme type
            if (themeControl.ClassName == "SongGridViewTheme")
            {
                // Get theme
                SongGridViewTheme theme = (SongGridViewTheme)propertyGridTheme.SelectedObject;

                // Refresh theme
                previewSongGridView.Theme = theme;
                previewSongGridView.InvalidateSongCache();
                previewSongGridView.Refresh();
            }                        
            else if (themeControl.ClassName == "OutputMeterTheme")
            {
                // Get theme
                OutputMeterTheme theme = (OutputMeterTheme)propertyGridTheme.SelectedObject;

                // Refresh theme
                previewOutputMeter.Theme = theme;                
                previewOutputMeter.Refresh();
            }
            else if (themeControl.ClassName == "MainWindowTheme")
            {
                // Get theme
                theme.MainWindow = (MainWindowTheme)propertyGridTheme.SelectedObject;

                // Refresh theme
                RefreshMainWindowPreview();
            }
        }

        /// <summary>
        /// Occurs when the user changes the current theme control using the combo box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void comboThemeControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Refresh property grid and preview pane
            RefreshPropertyGrid();
        }

        /// <summary>
        /// Refreshes the property grid and preview pane.
        /// </summary>
        public void RefreshPropertyGrid()
        {
            // Get item
            ThemePreviewPaneComboBoxItem themeControl = (ThemePreviewPaneComboBoxItem)comboPreviewPane.SelectedItem;

            // Check for Main Window theme
            if (themeControl.ClassName == "MainWindowTheme")
            {
                // Set visibility
                panelPreviewMainWindow.Visible = true;
                RefreshMainWindowPreview();

                // Set property grid item
                propertyGridTheme.SelectedObject = theme.MainWindow;
            }
            else
            {
                // Set visibility
                panelPreviewMainWindow.Visible = false;
            }

            // Check for Song Browser theme
            if (themeControl.ClassName == "SongGridViewTheme")
            {
                // Set visibility
                previewSongGridView.Visible = true;

                // Set property grid item
                propertyGridTheme.SelectedObject = previewSongGridView.Theme;
            }
            else
            {
                // Set visibility
                previewSongGridView.Visible = false;
            }

            // Check for Output Meter theme
            if (themeControl.ClassName == "OutputMeterTheme")
            {
                // Set visibility
                previewOutputMeter.Visible = true;

                // Set property grid item
                propertyGridTheme.SelectedObject = previewOutputMeter.Theme;
            }
            else
            {
                // Set visibility
                previewOutputMeter.Visible = false;
            }
        }

        /// <summary>
        /// Refreshes the Main Window preview pane.
        /// </summary>
        public void RefreshMainWindowPreview()
        {
            // Check if theme exists
            if(theme == null || theme.MainWindow == null)
            {
                return;
            }

            panelCurrentSong.Theme = theme.MainWindow.PanelTheme;            
            panelActions.Theme = theme.MainWindow.SecondaryPanelTheme;
            panelInformation.Theme = theme.MainWindow.SecondaryPanelTheme;
            panelCurrentPosition.Theme = theme.MainWindow.SecondaryPanelTheme;
            panelTimeShifting.Theme = theme.MainWindow.SecondaryPanelTheme;
        }
    }

    /// <summary>
    /// This class is used to define the items of the Theme Section combo box (in the Settings window).
    /// </summary>
    public class ThemePreviewPaneComboBoxItem
    {
        /// <summary>
        /// Private value for the Title property.
        /// </summary>
        private string title = string.Empty;
        /// <summary>
        /// Title of the combo box item (ex: Song Browser).
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        /// <summary>
        /// Private value for the ClassName property.
        /// </summary>
        private string className = string.Empty;
        /// <summary>
        /// Name of the class that contains the theme properties (ex: OutputMeterTheme).
        /// </summary>
        public string ClassName
        {
            get
            {
                return className;
            }
            set
            {
                className = value;
            }
        }

        /// <summary>
        /// Default constructor for the ThemePreviewPaneComboBoxItem class.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="className">Theme class name</param>
        public ThemePreviewPaneComboBoxItem(string title, string className)
        {
            this.title = title;
            this.className = className;                
        }
    }

}
