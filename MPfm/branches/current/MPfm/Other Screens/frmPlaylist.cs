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
using System.Text;
using System.Windows.Forms;
using MPfm.Library;
using MPfm.Player.PlayerV4;
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
            //this.viewSongs.ItemsReordered += new MPfm.WindowsControls.ReorderListView.ItemsReorderedHandler(viewSongs_ItemsReordered);

            //formRenameSavePlaylist = new frmRenameSavePlaylist(this, RenameSavePlaylistWindowMode.RenamePlaylist);
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
            this.Text = Main.Player.Playlist.Name;

            //// Set form title depending on playlist type
            //if (Main.Player.CurrentPlaylist.PlaylistType == PlaylistType.Custom)
            //{
            //    this.Text = "[Custom] - " + Main.Player.CurrentPlaylist.PlaylistName;
            //}
            //else
            //{
            //    this.Text = "[Auto] - " + Main.Player.CurrentPlaylist.PlaylistName;
            //}

            //if (Main.Player.CurrentPlaylist.PlaylistModified)
            //{
            //    Text = "*" + Text;
            //}
        }

        /// <summary>
        /// Refreshes the list view showing the songs in the current playlist.
        /// </summary>
        public void RefreshPlaylist()
        {
            // If the form isn't visible, just don't refresh
            if (!Visible)
            {
                return;
            }

            //// Remove all items from grid
            //viewSongs.Items.Clear();

            // Make sure the playlist is valid
            if (Main.Player.Playlist == null)
            {
                return;
            }

            // Import songs into the control
            viewSongs2.ImportPlaylist(Main.Player.Playlist);

            //// Make sure the list of songs is valid and non empty
            //if (Main.Player.CurrentPlaylist.Songs == null)
            //{
            //    return;
            //}

            //// Create array
            //ListViewItem[] lvItems = new ListViewItem[Main.Player.CurrentPlaylist.Songs.Count];
            //int a = 0;

            //// Loop through playlist songs
            //foreach (PlaylistSongDTO playlistSong in Main.Player.CurrentPlaylist.Songs)
            //{
            //    // Create item
            //    ListViewItem item = new ListViewItem(playlistSong.PlaylistSongId.ToString());
            //    item.Tag = playlistSong.PlaylistSongId.ToString();

            //    // Format the track number with the disc number if available
            //    if (playlistSong.Song.DiscNumber == null || playlistSong.Song.DiscNumber.Value == 0)
            //    {
            //        // Display the track number
            //        item.SubItems.Add(playlistSong.Song.TrackNumber.ToString());
            //    }
            //    else
            //    {
            //        // Display the track number with the disc number (disc.track: i.e. 1.1, 1.2, 2.1, 2.2, etc.)
            //        item.SubItems.Add(playlistSong.Song.DiscNumber.ToString() + "." + playlistSong.Song.TrackNumber.ToString());
            //    }

            //    item.SubItems.Add(playlistSong.Song.Title);
            //    item.SubItems.Add(playlistSong.Song.Time);
            //    item.SubItems.Add(playlistSong.Song.ArtistName);
            //    item.SubItems.Add(playlistSong.Song.AlbumTitle);

            //    // Play count
            //    if (playlistSong.Song.PlayCount == 0)
            //    {
            //        item.SubItems.Add("");
            //    }
            //    else
            //    {
            //        item.SubItems.Add(playlistSong.Song.PlayCount.ToString());
            //    }

            //    // Last played
            //    if (playlistSong.Song.LastPlayed == DateTime.MinValue)
            //    {
            //        item.SubItems.Add("");
            //    }
            //    else
            //    {
            //        item.SubItems.Add(playlistSong.Song.LastPlayed.ToString());
            //    }

            //    // Check if a play icon needs to be shown
            //    if (Main.Player.CurrentPlaylist != null && 
            //        Main.Player.CurrentPlaylist.CurrentSong != null &&
            //        Main.Player.CurrentPlaylist.CurrentSong.PlaylistSongId == playlistSong.PlaylistSongId)
            //    {
            //        item.Selected = true;
            //        item.ImageIndex = 0;
            //    }

            //    //viewSongs.Items.Add(item);
            //    lvItems[a] = item;
            //    a++;
            //}

            //// Update list view
            //viewSongs.BeginUpdate();
            //viewSongs.Items.AddRange(lvItems);
            //viewSongs.EndUpdate();

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
        /// Occurs when the user clicks on the "Save playlist" button.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnSavePlaylist_Click(object sender, EventArgs e)
        {
            //// Save playlist
            //Cursor.Current = Cursors.WaitCursor;
            //Main.Player.Library.SavePlaylist(Main.Player.CurrentPlaylist);            

            //// Set modified flag
            //Main.Player.CurrentPlaylist.PlaylistModified = false;

            //// Refresh the window title
            //RefreshTitle();

            //// Refresh the playlists node in the Artist/Album browser
            //Main.RefreshTreeLibraryPlaylists();

            //// Reset cursor
            //Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save playlist as" button.
        /// Display a modal dialog to the user requesting the playlist name.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        private void btnSavePlaylistAs_Click(object sender, EventArgs e)
        {
            //// Popup window                
            //formRenameSavePlaylist = new frmRenameSavePlaylist(Main, RenameSavePlaylistWindowMode.SavePlaylist);

            //// Set window location
            //formRenameSavePlaylist.Location = new Point(this.Location.X + 50, this.Location.Y + 50);
            //formRenameSavePlaylist.txtName.Text = Main.Player.CurrentPlaylist.PlaylistName;

            //// Show Save Playlist dialog
            //if (formRenameSavePlaylist.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            //{
            //    // First, change all ids to make sure items are unique
            //    Cursor.Current = Cursors.WaitCursor;

            //    // Change the playlist Id
            //    Main.Player.CurrentPlaylist.PlaylistId = Guid.NewGuid();

            //    // Change the playlist songs ids
            //    foreach (PlaylistSongDTO playlistSong in Main.Player.CurrentPlaylist.Songs)
            //    {
            //        playlistSong.PlaylistSongId = Guid.NewGuid();
            //    }

            //    // Save playlist
            //    Main.Player.Library.SavePlaylist(Main.Player.CurrentPlaylist);
            //}
            //else
            //{
            //    // The user has cancelled
            //    return;
            //}

            //// Set modified flag
            //Main.Player.CurrentPlaylist.PlaylistModified = false;

            //// Refresh the window title
            //RefreshTitle();

            //// Refresh the playlists node in the Artist/Album browser
            //Main.RefreshTreeLibraryPlaylists();

            //// Reset cursor
            //Cursor.Current = Cursors.Default;
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