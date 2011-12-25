//
// frmPlaylist.cs: Playlist window. This is where the user can view and manipulate playlists. 
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
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
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
        public frmRenameSavePlaylist formRenameSavePlaylist = null;

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
        /// Constructor for the Playlist form. Requires a hook to the main form.
        /// </summary>
        /// <param name="main">Hook to main form</param>
        public frmPlaylist(frmMain main)
        {
            InitializeComponent();
            m_main = main;
        }

        /// <summary>
        /// Occurs when the form is shown.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void frmPlaylist_Shown(object sender, EventArgs e)
        {
            RefreshPlaylist();
        }

        #region Close Events

        /// <summary>
        /// Occurs when the user tries to close the form, using the X button or the
        /// Close button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
        /// <param name="args">Event Arguments</param>
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
        /// Occurs when the user doubles clicks on an item of the ListView displaying the songs of the current playlist.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void viewSongs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Play the selected song
            PlaySelectedSong();
        }

        /// <summary>
        /// Plays the selected song in the ListView displaying the songs in the current playlist.
        /// </summary>
        public void PlaySelectedSong()
        {
            //// Check if there is a selection
            //if (viewSongs.SelectedItems.Count > 0)
            //{               
            //    // Get playlist song id
            //    Guid playlistSongId = new Guid(viewSongs.SelectedItems[0].Tag.ToString());

            //    // Skip to song
            //    Main.Player.SkipToSong(playlistSongId);

            //    // Refresh controls                
            //    Main.RefreshSongControls(false);
            //    RefreshPlaylistPlayIcon(playlistSongId);
            //}
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
                this.Text = Main.Player.Playlist.Name + " (" + Main.Player.Playlist.FilePath + ")";
            }
            else
            {
                this.Text = Main.Player.Playlist.Name;
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

        #endregion

        #region Toolbar Buttons Events

        /// <summary>
        /// Occurs when the user clicks on the "New playlist" button in the toolbar.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnLoadPlaylist_Click(object sender, EventArgs e)
        {
            // Display the open playlist file dialog
            if (dialogLoadPlaylist.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                // The user has canceled the operation
                return;
            }

            // Check if the player is playing
            if(Main.Player.IsPlaying)
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
        /// Loads a playlist.
        /// </summary>
        /// <param name="playlistFilePath">Playlist file path</param>
        public void LoadPlaylist(string playlistFilePath)
        {
            // Clear current playlist
            Main.Player.Playlist.LoadPlaylist(playlistFilePath);

            // Refresh view
            RefreshPlaylist();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save playlist" button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnSavePlaylistAs_Click(object sender, EventArgs e)
        {
            SavePlaylistAs();
        }

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

        public void SavePlaylist(string playlistFilePath)
        {
            // Change cursor
            Cursor.Current = Cursors.WaitCursor;

            // Set playlist file path
            Main.Player.Playlist.FilePath = playlistFilePath;

            // Determine what format the user has chosen
            if (dialogSavePlaylist.FileName.ToUpper().Contains(".M3U"))
            {
                // Save playlist
                PlaylistTools.SaveM3UPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist);
            }
            else if (dialogSavePlaylist.FileName.ToUpper().Contains(".M3U8"))
            {
                // Save playlist
                PlaylistTools.SaveM3UPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist);
            }
            else if (dialogSavePlaylist.FileName.ToUpper().Contains(".PLS"))
            {
                // Save playlist
                PlaylistTools.SavePLSPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist);
            }
            else if (dialogSavePlaylist.FileName.ToUpper().Contains(".XSPF"))
            {
                // Save playlist
                PlaylistTools.SaveXSPFPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist);
            }

            // Change cursor
            Cursor.Current = Cursors.Default;

            // Refresh title
            RefreshTitle();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Rename playlist" button.
        /// Display a modal dialog to the user requesting the new playlist name.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnRenamePlaylist_Click(object sender, EventArgs e)
        {
            // Create window
            formRenameSavePlaylist = new frmRenameSavePlaylist(Main, RenameSavePlaylistWindowMode.RenamePlaylist);

            // Set window location
            formRenameSavePlaylist.Location = new Point(this.Location.X + 50, this.Location.Y + 50);
            formRenameSavePlaylist.txtName.Text = Main.Player.Playlist.Name;

            // Show Save Playlist dialog
            if (formRenameSavePlaylist.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                // Set playlist type 
                //Main.Player.Playlist.PlaylistType = PlaylistType.Custom;

                // Refresh the window title
                RefreshTitle();

                // Refresh the playlists node in the Artist/Album browser
                Main.RefreshTreeLibraryPlaylists();
            }
        }

        /// <summary>
        /// Occurs when the user clicks on the "Remove songs" button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
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

        private void menuPlaylist_Opening(object sender, CancelEventArgs e)
        {
            // Check if at least one item is selected
            if (viewSongs2.SelectedItems.Count == 0)
            {
                // Do not display the contextual menu
                e.Cancel = true;
            }
        }

        private void miPlaylistPlaySong_Click(object sender, EventArgs e)
        {
            PlaySelectedSong();
        }

        #endregion

        /// <summary>
        /// Occurs when the user double clicks on one of the item in the playlist song view.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void viewSongs2_DoubleClick(object sender, EventArgs e)
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
    }
}