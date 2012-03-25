//
// frmPlaylist.cs: Playlist window. This is where the user can view and manipulate playlists. 
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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Playlist window. This is where the user can view and manipulate playlists. They can
    /// also save the playlists into the library.
    /// </summary>
    public partial class frmPlaylist : MPfm.WindowsControls.Form
    {
        // Private variables        
        private frmRenameSavePlaylist formRenameSavePlaylist = null;
        private frmLoadPlaylist formLoadPlaylist = null;

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
        /// Constructor for the Playlist form. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmPlaylist(frmMain main)
        {
            InitializeComponent();
            this.main = main;
        }

        /// <summary>
        /// Occurs when the form is first shown.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmPlaylist_Shown(object sender, EventArgs e)
        {
            // Refresh playlist
            RefreshPlaylist();

            // Refresh playlist menu
            RefreshLibraryPlaylistsMenu();
        }

        /// <summary>
        /// Occurs when the form is shown.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmPlaylist_VisibleChanged(object sender, EventArgs e)
        {
            // Check if form is visible
            if (Visible)
            {
                // Refresh playlist menu
                RefreshLibraryPlaylistsMenu();
            }
        }

        #region Close Events

        /// <summary>
        /// Occurs when the user tries to close the form, using the X button or the
        /// Close button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void frmPlaylist_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall || e.CloseReason == CloseReason.WindowsShutDown)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                this.Hide();
                Main.btnPlaylist.Checked = false;
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the Close button on the toolbar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Main.BringToFront();
            Main.Focus();
        }

        #endregion

        /// <summary>
        /// Occurs when the user changes the order of the playlist songs in the ListView.
        /// </summary>
        /// <param name="args">Event arguments</param>
        public void viewSongs_ItemsReordered(EventArgs args)
        {
            //// Copy the original list of songs (we have to create a new list or the objects will stay as a reference)
            //List<PlaylistSongDTO> playlistSongs = new List<PlaylistSongDTO>();
            //foreach (PlaylistSongDTO playlistSong in Main.Player.CurrentPlaylist.Songs)
            //{
            //    // Perform a deep clone of the object
            //    PlaylistSongDTO newPlaylistSong = new PlaylistSongDTO();
            //    newPlaylistSong.PlaylistSongId = playlistSong.PlaylistSongId;
            //    newPlaylistSong.Song = playlistSong.Song;
            //    playlistSongs.Add(newPlaylistSong);
            //}           

            //// Loop through list view items            
            //for (int a = 0; a < viewSongs.Items.Count; a++ )
            //{
            //    // Get the playlist song id
            //    Guid playlistSongId = new Guid(viewSongs.Items[a].Tag.ToString());

            //    // Check if the playlist song id matches
            //    if (playlistSongs[a].PlaylistSongId != playlistSongId)
            //    {
            //        // No match: we have to update this song                   

            //        // We can't just update the properties of the PlaylistSongDTO or it will
            //        // change the values in the CurrentSong property. Thus we will create a new
            //        // PlaylistSongDTO instance with the same id.
            //        PlaylistSongDTO newPlaylistSong = new PlaylistSongDTO();

            //        // Update the playlist song id
            //        // Note; This changes the id of the CurrentSong too! it must not do that!
            //        newPlaylistSong.PlaylistSongId = playlistSongId;

            //        // Find the SongDTO in the list of playlist songs
            //        foreach (PlaylistSongDTO playlistSong in playlistSongs)
            //        {
            //            // Is there a match?
            //            if (playlistSong.PlaylistSongId == playlistSongId)
            //            {
            //                // Update the SongDTO item
            //                newPlaylistSong.Song = playlistSong.Song;

            //                // Get out of the loop
            //                break;
            //            }

            //        }

            //        // Replace object in array
            //        Main.Player.CurrentPlaylist.Songs[a] = newPlaylistSong;
            //    }
            //}            
        }

        /// <summary>
        /// Plays the selected song in the ListView displaying the songs in the current playlist.
        /// </summary>
        public void PlaySelectedSong()
        {
            // Make sure there is a selected item
            if (viewSongs2.SelectedItems.Count == 0)
            {
                return;
            }

            // Get the playlist item identifier from the selected item
            Guid playlistItemId = viewSongs2.SelectedItems[0].PlaylistItemId;
            PlaylistItem item = Main.Player.Playlist.Items.FirstOrDefault(x => x.Id == playlistItemId);
            int index = Main.Player.Playlist.Items.IndexOf(item);

            // Check if the player is playing
            if (Main.Player.IsPlaying)
            {
                // Skip to new song
                Main.Player.GoTo(index);
            }
            else
            {
                // Set playlist index
                Main.Player.Playlist.GoTo(index);

                // Start playback
                Main.Play();
            }
        }

        #region Refresh Methods

        /// <summary>
        /// Updates the title of the window with the playlist name.
        /// </summary>
        public void RefreshTitle()
        {
            // Display playlist file path in form title if available
            if (!String.IsNullOrEmpty(Main.Player.Playlist.FilePath))
            {                
                this.Text = Main.Player.Playlist.FilePath;
            }
            else
            {                
                this.Text = "Unsaved playlist";
            }
        }

        /// <summary>
        /// Refreshes the list view showing the songs in the current playlist.
        /// </summary>
        public void RefreshPlaylist()
        {
            // Make sure the playlist is valid
            if (Main.Player.Playlist == null)
            {
                return;
            }

            // Import songs into the control
            viewSongs2.ImportPlaylist(Main.Player.Playlist);

            // Refresh window title
            RefreshTitle();
        }

        /// <summary>
        /// Refreshes the play icon in the SongGridView displaying playlist items.
        /// </summary>
        /// <param name="newPlaylistItemId">Playlist item identifier</param>
        public void RefreshPlaylistPlayIcon(Guid newPlaylistItemId)
        {
            // Set currently playing song
            viewSongs2.NowPlayingPlaylistItemId = newPlaylistItemId;
            viewSongs2.Refresh();
        }

        /// <summary>
        /// Refreshes the "Load playlist"/"Library playlists" menu item with library playlists.
        /// </summary>
        public void RefreshLibraryPlaylistsMenu()
        {
            // Clear sub items
            miLoadPlaylistLibrary.DropDownItems.Clear();

            // Fetch list of playlists from database
            List<PlaylistFile> playlistFiles = Main.Library.Gateway.SelectPlaylistFiles();

            // Add items to menu
            foreach(PlaylistFile playlistFile in playlistFiles)
            {
                // Extract file name without extension
                string fileName = Path.GetFileNameWithoutExtension(playlistFile.FilePath);

                // Add item
                ToolStripItem item = miLoadPlaylistLibrary.DropDownItems.Add(fileName);
                item.Tag = playlistFile.FilePath;
                item.ToolTipText = playlistFile.FilePath;
                item.Click += new EventHandler(miLoadPlaylistLibraryItem_Click);               
            }
        }

        #endregion

        #region Toolbar Buttons Events

        /// <summary>
        /// Occurs when the user clicks on the "New playlist" button in the toolbar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnNewPlaylist_Click(object sender, EventArgs e)
        {
            // Check if the playlist has at least one item
            if (viewSongs2.Items.Count > 0)
            {
                // Warn user
                if (MessageBox.Show("Are you sure you wish to create a new playlist?\nYou will lose the contents of the current playlist. This will also stop playback.", "Create a new playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    // The user said no; exit method
                    return;
                }
            }

            // Is the player running?
            if (Main.Player.IsPlaying)
            {
                // Stop playback
                Main.Stop();
            }

            // Empty current playlist
            Main.Player.Playlist.Clear();            

            // Refresh playlist
            RefreshPlaylist();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Load playlist" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnLoadPlaylist_Click(object sender, EventArgs e)
        {
            // Display contextual menu
            menuLoadPlaylist.Show(btnLoadPlaylist, new Point(0, btnLoadPlaylist.Height));
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save playlist" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSavePlaylist_Click(object sender, EventArgs e)
        {
            // Check if playlist file is valid
            if (!String.IsNullOrEmpty(Main.Player.Playlist.FilePath))
            {
                // Check if file exists
                if (File.Exists(Main.Player.Playlist.FilePath))
                {
                    // Warn user is he wants to save
                    if(MessageBox.Show("Warning: The playlist file already exists. Do you wish to overwrite this file?", "The playlist file already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                    {
                        // The user wants to save
                        SavePlaylist(Main.Player.Playlist.FilePath);
                    }
                }
            }
            else
            {
                // Ask user for file path
                SavePlaylistAs();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save playlist as" button.
        /// Display a modal dialog to the user requesting the playlist name.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSavePlaylistAs_Click(object sender, EventArgs e)
        {
            SavePlaylistAs();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Rename playlist" button.
        /// Display a modal dialog to the user requesting the new playlist name.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnRenamePlaylist_Click(object sender, EventArgs e)
        {
            // Create window
            formRenameSavePlaylist = new frmRenameSavePlaylist(Main, RenameSavePlaylistWindowMode.RenamePlaylist);

            // Set window location
            formRenameSavePlaylist.Location = new Point(this.Location.X + 50, this.Location.Y + 50);
            //formRenameSavePlaylist.txtName.Text = Main.Player.Playlist.Name;

            // Show Save Playlist dialog
            if (formRenameSavePlaylist.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                // Refresh the window title
                RefreshTitle();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Remove songs" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnRemoveSongs_Click(object sender, EventArgs e)
        {
            // Create a list of songs to remove
            //List<Guid> removedSongs = new List<Guid>();

            // Check if there is at least one item selected
            if (viewSongs2.SelectedItems.Count > 0)
            {
                // Go through list view items (use a while loop so we can remove items from a collection we are iterating through)
                while (true)
                {
                    // Are there selected items left?
                    if (viewSongs2.SelectedItems.Count == 0)
                    {
                        // Exit loop
                        break;
                    }

                    // Get item
                    SongGridViewItem item = viewSongs2.SelectedItems[0];

                    // Check if the selected song is playing
                    if (item.PlaylistItemId == Main.Player.Playlist.CurrentItem.Id)
                    {
                        // Warn the user
                        MessageBox.Show("You cannot remove the current song from the playlist!", "Error removing song from playlist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }

                    // Remove playlist item
                    PlaylistItem playlistItem = Main.Player.Playlist.Items.FirstOrDefault(x => x.Id == item.PlaylistItemId);
                    Main.Player.Playlist.Items.Remove(playlistItem);

                    // Remove grid view item
                    viewSongs2.Items.Remove(item);
                }

                // Set playlist as modified
                //Main.Player.CurrentPlaylist.PlaylistModified = true;
                //Main.Player.CurrentPlaylist.PlaylistType = PlaylistType.Custom;

                // Refresh the window title
                RefreshTitle();

                // Refresh grid view
                viewSongs2.InvalidateSongCache();
                viewSongs2.Refresh();
            }
        }

        #endregion

        #region Contextual Menu Events

        /// <summary>
        /// Occurs when the user right clicks on the Song Browser.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void menuPlaylist_Opening(object sender, CancelEventArgs e)
        {
            // Check if at least one item is selected
            if (viewSongs2.SelectedItems.Count == 0)
            {
                // Do not display the contextual menu
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Occurs when the user click on the "Play selected songs" menu item in the Playlist contextual menu.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miPlaylistPlaySong_Click(object sender, EventArgs e)
        {
            PlaySelectedSong();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Load playlist" button and the "Browse" menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miLoadPlaylistBrowse_Click(object sender, EventArgs e)
        {
            // Display the open playlist file dialog
            if (dialogLoadPlaylist.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                // The user has canceled the operation
                return;
            }

            // Check if the player is playing
            if (Main.Player.IsPlaying)
            {
                // Warn user
                if (MessageBox.Show("Loading a new playlist will stop playback. Are you sure you wish to do this?", "Load a new playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    // The user said no; exit method
                    return;
                }

                // Stop playback
                Main.Stop();
            }

            // Load playlist
            LoadPlaylist(dialogLoadPlaylist.FileName);
        }

        /// <summary>
        /// Occurs when the user clicks on one of the library playlists in the "Load playlist"/"Library playlists" menu.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void miLoadPlaylistLibraryItem_Click(object sender, EventArgs e)
        {
            // Cast sender
            ToolStripItem item = (ToolStripItem)sender;

            // Check if the player is playing
            if (Main.Player.IsPlaying)
            {
                // Warn user
                if (MessageBox.Show("Loading a new playlist will stop playback. Are you sure you wish to do this?", "Load a new playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    // The user said no; exit method
                    return;
                }

                // Stop playback
                Main.Stop();
            }

            // Load playlist
            LoadPlaylist(item.Tag.ToString());
        }

        #endregion

        #region Load/Save Playlists

        /// <summary>
        /// Loads a playlist.
        /// </summary>
        /// <param name="playlistFilePath">Playlist file path</param>
        public void LoadPlaylist(string playlistFilePath)
        {
            try
            {
                // Create window
                formLoadPlaylist = new frmLoadPlaylist(Main, playlistFilePath);

                // Show Load playlist dialog (progress bar)
                DialogResult dialogResult = formLoadPlaylist.ShowDialog(this);
                if (dialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    // Get audio files
                    List<AudioFile> audioFiles = formLoadPlaylist.AudioFiles;

                    // Check if any audio files have failed loading
                    List<string> failedAudioFilePaths = formLoadPlaylist.FailedAudioFilePaths;

                    // Clear player playlist
                    Main.Player.Playlist.Clear();
                    Main.Player.Playlist.FilePath = playlistFilePath;

                    // Make sure there are audio files to add to the player playlist
                    if (audioFiles.Count > 0)
                    {
                        // Add audio files                        
                        Main.Player.Playlist.AddItems(audioFiles);
                        Main.Player.Playlist.First();
                    }
                                       
                    // Refresh song browser
                    RefreshTitle();
                    RefreshPlaylist();                    
                    RefreshPlaylistPlayIcon(Guid.Empty);

                    // Check if any files could not be loaded
                    if (failedAudioFilePaths != null && failedAudioFilePaths.Count > 0)
                    {
                        // Check if the user wants to see the list of files
                        if (MessageBox.Show("Some files in the playlist could not be loaded. Do you wish to see the list of files in a text editor?", "Some files could not be loaded", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                        {
                            // Get temp file path
                            string tempFilePath = Path.GetTempPath() + "MPfm_PlaylistLog_" + Conversion.DateTimeToUnixTimestamp(DateTime.Now).ToString("0.0000000").Replace(".", "") + ".txt";

                            // Create temporary file
                            TextWriter tw = null;
                            try
                            {
                                // Open text writer
                                tw = new StreamWriter(tempFilePath);
                                foreach (string item in failedAudioFilePaths)
                                {
                                    tw.WriteLine(item);
                                }                                
                            }
                            catch (Exception ex)
                            {
                                // Display error
                                MessageBox.Show("Failed to save the file to " + tempFilePath + "!\n\nException:\n" + ex.Message + "\n" + ex.StackTrace, "Failed to save the file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            finally
                            {
                                tw.Close();
                            }

                            // Start notepad
                            Process.Start(tempFilePath);
                        }
                    }
                    
                    // Check if the playlist is empty
                    if (audioFiles.Count == 0)
                    {
                        // Warn user that the playlist is empty
                        MessageBox.Show("The playlist file is empty or does not contain any valid file!", "Playlist is empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                // Dispose form
                formLoadPlaylist.Dispose();
                formLoadPlaylist = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading playlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the playlist under a different file path.
        /// </summary>
        public void SavePlaylistAs()
        {
            // Check if the playlist is empty
            if (Main.Player.Playlist.Items.Count == 0)
            {
                // Display error
                MessageBox.Show("Error: You cannot save an empty playlist!", "Error saving playlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Display dialog
            if (dialogSavePlaylist.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                // The user has cancelled the operation
                return;
            }

            // Save playlist
            SavePlaylist(dialogSavePlaylist.FileName);
        }

        /// <summary>
        /// Saves the playlist in the specified file path.
        /// </summary>
        /// <param name="playlistFilePath">Playlist file path</param>        
        public void SavePlaylist(string playlistFilePath)
        {
            bool relativePath = false;

            // Change cursor
            Cursor.Current = Cursors.WaitCursor;

            // Set playlist file path
            Main.Player.Playlist.FilePath = playlistFilePath;

            // Check if the path is in the same 
            DialogResult dialogResult = MessageBox.Show("Do you wish to use relative paths instead of absolute paths when possible?\n\nA full path or absolute path is a path that points to the same location on one file system regardless of the working directory or combined paths.\n\nA relative path is a path relative to the working directory of the user or application, so the full absolute path will not have to be given.", "Use relative paths", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            // Check result
            if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                // Cancel
                return;
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                // Save playlist
                //SavePlaylist(dialogSavePlaylist.FileName, true);
                relativePath = true;
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                // Save playlist
                //SavePlaylist(dialogSavePlaylist.FileName, false);
                relativePath = false;
            }

            // Determine what format the user has chosen
            if (dialogSavePlaylist.FileName.ToUpper().Contains(".M3U8"))
            {
                // Save playlist
                PlaylistTools.SaveM3UPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist, relativePath, true);
            }
            else if (dialogSavePlaylist.FileName.ToUpper().Contains(".M3U"))
            {
                // Save playlist
                PlaylistTools.SaveM3UPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist, relativePath, false);
            }
            else if (dialogSavePlaylist.FileName.ToUpper().Contains(".PLS"))
            {
                // Save playlist
                PlaylistTools.SavePLSPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist, relativePath);
            }
            else if (dialogSavePlaylist.FileName.ToUpper().Contains(".XSPF"))
            {
                // Save playlist
                PlaylistTools.SaveXSPFPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist, relativePath);
            }

            // Change cursor
            Cursor.Current = Cursors.Default;

            // Refresh title
            RefreshTitle();
        }

        #endregion

        /// <summary>
        /// Occurs when the user double clicks on one of the item in the playlist song view.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void viewSongs2_DoubleClick(object sender, EventArgs e)
        {
            PlaySelectedSong();
        }
    }

}