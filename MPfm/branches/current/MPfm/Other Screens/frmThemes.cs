//
// frmThemes.cs: Themes window. This is where the user can create and edit themes.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Library;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Themes window. This is where the user can create and edit themes.   
    /// </summary>
    public partial class frmThemes : MPfm.WindowsControls.Form
    {
        // Private variables
        private MainWindowTheme mainWindowTheme = null;
        private SecondaryWindowTheme secondaryWindowTheme = null;
        private string filePath = string.Empty;            
                
        private frmMain m_main = null;
        /// <summary>
        /// Hook to the main form.
        /// </summary>
        public frmMain Main
        {
            get
            {
                return m_main;
            }
        }

        /// <summary>
        /// Constructor for Themes window. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmThemes(frmMain main)
        {
            InitializeComponent();
            m_main = main;
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
            mainWindowTheme = new MainWindowTheme();
            secondaryWindowTheme = new SecondaryWindowTheme();
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

        /// <summary>
        /// Occurs when the user clicks on the Close button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Hide form            
            Main.BringToFront();
            Main.Focus();
            this.Close();
        }

        #endregion

        #region Theme Tab Events

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
        }

        #endregion

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
                mainWindowTheme = (MainWindowTheme)propertyGridTheme.SelectedObject;

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
                propertyGridTheme.SelectedObject = mainWindowTheme;
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
            if(mainWindowTheme == null)
            {
                return;
            }

            // Main panels
            panelCurrentSong.CustomFont = mainWindowTheme.PanelHeaderTextFont;
            panelCurrentSong.ForeColor = mainWindowTheme.PanelHeaderTextColor;
            panelCurrentSong.GradientColor1 = mainWindowTheme.PanelBackgroundColor1;
            panelCurrentSong.GradientColor2 = mainWindowTheme.PanelBackgroundColor2;
            panelCurrentSong.HeaderGradientColor1 = mainWindowTheme.PanelHeaderBackgroundColor1;
            panelCurrentSong.HeaderGradientColor2 = mainWindowTheme.PanelHeaderBackgroundColor2;
            panelCurrentSong.HeaderForeColor = mainWindowTheme.PanelHeaderTextColor;            
            // header font to add
            panelCurrentSong.Refresh();

            // Set main panel labels
            lblCurrentArtistName.CustomFont = mainWindowTheme.PanelTitleFont;
            lblCurrentArtistName.ForeColor = mainWindowTheme.PanelTitleColor;
            lblCurrentAlbumTitle.CustomFont = mainWindowTheme.PanelSubtitleFont;
            lblCurrentAlbumTitle.ForeColor = mainWindowTheme.PanelSubtitleColor;
            lblCurrentSongTitle.CustomFont = mainWindowTheme.PanelSubtitle2Font;
            lblCurrentSongTitle.ForeColor = mainWindowTheme.PanelSubtitle2Color;
            lblCurrentFilePath.CustomFont = mainWindowTheme.PanelTextFont;
            lblCurrentFilePath.ForeColor = mainWindowTheme.PanelTextColor;

            // Secondary panels            
            panelInformation.GradientColor1 = mainWindowTheme.SecondaryPanelBackgroundColor1;
            panelInformation.GradientColor2 = mainWindowTheme.SecondaryPanelBackgroundColor2;            
            panelInformation.CustomFont = mainWindowTheme.SecondaryPanelHeaderTextFont;
            panelInformation.ForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            panelInformation.HeaderGradientColor1 = mainWindowTheme.SecondaryPanelHeaderBackgroundColor1;
            panelInformation.HeaderGradientColor2 = mainWindowTheme.SecondaryPanelHeaderBackgroundColor2;
            panelInformation.HeaderForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            // header font to add
            panelInformation.Refresh();
            panelActions.GradientColor1 = mainWindowTheme.SecondaryPanelBackgroundColor1;
            panelActions.GradientColor2 = mainWindowTheme.SecondaryPanelBackgroundColor2;
            panelActions.CustomFont = mainWindowTheme.SecondaryPanelHeaderTextFont;
            panelActions.ForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            panelActions.HeaderGradientColor1 = mainWindowTheme.SecondaryPanelHeaderBackgroundColor1;
            panelActions.HeaderGradientColor2 = mainWindowTheme.SecondaryPanelHeaderBackgroundColor2;
            panelActions.HeaderForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            // header font to add
            panelActions.Refresh();
            panelCurrentPosition.GradientColor1 = mainWindowTheme.SecondaryPanelBackgroundColor1;
            panelCurrentPosition.GradientColor2 = mainWindowTheme.SecondaryPanelBackgroundColor2;
            panelCurrentPosition.CustomFont = mainWindowTheme.SecondaryPanelHeaderTextFont;
            panelCurrentPosition.ForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            panelCurrentPosition.HeaderGradientColor1 = mainWindowTheme.SecondaryPanelHeaderBackgroundColor1;
            panelCurrentPosition.HeaderGradientColor2 = mainWindowTheme.SecondaryPanelHeaderBackgroundColor2;
            panelCurrentPosition.HeaderForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            // header font to add
            panelCurrentPosition.Refresh();
            panelTimeShifting.GradientColor1 = mainWindowTheme.SecondaryPanelBackgroundColor1;
            panelTimeShifting.GradientColor2 = mainWindowTheme.SecondaryPanelBackgroundColor2;
            panelTimeShifting.CustomFont = mainWindowTheme.SecondaryPanelHeaderTextFont;
            panelTimeShifting.ForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            panelTimeShifting.HeaderGradientColor1 = mainWindowTheme.SecondaryPanelHeaderBackgroundColor1;
            panelTimeShifting.HeaderGradientColor2 = mainWindowTheme.SecondaryPanelHeaderBackgroundColor2;
            panelTimeShifting.HeaderForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            // header font to add
            panelTimeShifting.Refresh();

            // Set secondary panels labels
            lblCurrentPosition.CustomFont = mainWindowTheme.PanelTimeDisplayFont;
            lblCurrentPosition.ForeColor = mainWindowTheme.PanelTimeDisplayColor;
            lblTimeShifting.CustomFont = mainWindowTheme.PanelSmallTimeDisplayFont;
            lblTimeShifting.ForeColor = mainWindowTheme.PanelSmallTimeDisplayColor;
            linkResetTimeShifting.CustomFont = mainWindowTheme.PanelSmallTimeDisplayFont;
            linkResetTimeShifting.ForeColor = mainWindowTheme.PanelSmallTimeDisplayColor;

            lblSoundFormatTitle.CustomFont = mainWindowTheme.SecondaryPanelLabelFont;
            lblSoundFormatTitle.ForeColor = mainWindowTheme.SecondaryPanelLabelColor;
            lblSoundFormat.CustomFont = mainWindowTheme.SecondaryPanelTextFont;
            lblSoundFormat.ForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            lblFrequencyTitle.CustomFont = mainWindowTheme.SecondaryPanelLabelFont;
            lblFrequencyTitle.ForeColor = mainWindowTheme.SecondaryPanelLabelColor;
            lblFrequency.CustomFont = mainWindowTheme.SecondaryPanelTextFont;
            lblFrequency.ForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;
            lblBitrateTitle.CustomFont = mainWindowTheme.SecondaryPanelLabelFont;
            lblBitrateTitle.ForeColor = mainWindowTheme.SecondaryPanelLabelColor;
            lblBitrate.CustomFont = mainWindowTheme.SecondaryPanelTextFont;
            lblBitrate.ForeColor = mainWindowTheme.SecondaryPanelHeaderTextColor;

            linkEditSongMetadata.CustomFont = mainWindowTheme.SecondaryPanelTextFont;
            linkEditSongMetadata.ForeColor = mainWindowTheme.SecondaryPanelTextColor;
            linkSearchLyrics.CustomFont = mainWindowTheme.SecondaryPanelTextFont;
            linkSearchLyrics.ForeColor = mainWindowTheme.SecondaryPanelTextColor;
            linkSearchBassTabs.CustomFont = mainWindowTheme.SecondaryPanelTextFont;
            linkSearchBassTabs.ForeColor = mainWindowTheme.SecondaryPanelTextColor;
            linkSearchGuitarTabs.CustomFont = mainWindowTheme.SecondaryPanelTextFont;
            linkSearchGuitarTabs.ForeColor = mainWindowTheme.SecondaryPanelTextColor;
            lblSearchWeb.CustomFont = mainWindowTheme.SecondaryPanelLabelFont;
            lblSearchWeb.ForeColor = mainWindowTheme.SecondaryPanelLabelColor;
            
            trackTimeShifting.GradientColor1 = mainWindowTheme.SecondaryPanelBackgroundColor1;
            trackTimeShifting.GradientColor2 = mainWindowTheme.SecondaryPanelBackgroundColor2;

            // Toolbar
            panelSongBrowserToolbar.GradientColor1 = mainWindowTheme.ToolbarBackgroundColor1;
            panelSongBrowserToolbar.GradientColor2 = mainWindowTheme.ToolbarBackgroundColor2;

            btnPlaySelectedSong.GradientColor1 = mainWindowTheme.ToolbarButtonBackgroundColor1;
            btnPlaySelectedSong.GradientColor2 = mainWindowTheme.ToolbarButtonBackgroundColor2;
            btnPlaySelectedSong.BorderColor = mainWindowTheme.ToolbarButtonBorderColor;
            btnPlaySelectedSong.MouseOverGradientColor1 = mainWindowTheme.ToolbarButtonMouseOverBackgroundColor1;
            btnPlaySelectedSong.MouseOverGradientColor2 = mainWindowTheme.ToolbarButtonMouseOverBackgroundColor2;
            btnPlaySelectedSong.MouseOverBorderColor = mainWindowTheme.ToolbarButtonMouseOverBorderColor;
            btnPlaySelectedSong.DisabledGradientColor1 = mainWindowTheme.ToolbarButtonDisabledBackgroundColor1;
            btnPlaySelectedSong.DisabledGradientColor2 = mainWindowTheme.ToolbarButtonDisabledBackgroundColor2;
            btnPlaySelectedSong.DisabledBorderColor = mainWindowTheme.ToolbarButtonDisabledBorderColor;            
            btnPlaySelectedSong.CustomFont = mainWindowTheme.ToolbarButtonTextFont;
            btnPlaySelectedSong.DisabledFontColor = mainWindowTheme.ToolbarButtonDisabledTextColor;
            btnPlaySelectedSong.MouseOverFontColor = mainWindowTheme.ToolbarButtonMouseOverTextColor;
            btnPlaySelectedSong.FontColor = mainWindowTheme.ToolbarButtonTextColor;

            lblSearchFor.CustomFont = mainWindowTheme.ToolbarTextFont;
            lblSearchFor.ForeColor = mainWindowTheme.ToolbarTextColor;
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
        private string m_title = string.Empty;
        /// <summary>
        /// Title of the combo box item (ex: Song Browser).
        /// </summary>
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
            }
        }

        /// <summary>
        /// Private value for the ClassName property.
        /// </summary>
        private string m_className = string.Empty;
        /// <summary>
        /// Name of the class that contains the theme properties (ex: OutputMeterTheme).
        /// </summary>
        public string ClassName
        {
            get
            {
                return m_className;
            }
            set
            {
                m_className = value;
            }
        }

        /// <summary>
        /// Default constructor for the ThemePreviewPaneComboBoxItem class.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="className">Theme class name</param>
        public ThemePreviewPaneComboBoxItem(string title, string className)
        {
            m_title = title;
            m_className = className;                
        }
    }

}