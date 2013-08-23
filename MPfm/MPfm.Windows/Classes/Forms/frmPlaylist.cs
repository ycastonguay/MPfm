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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;

namespace MPfm.Windows.Classes.Forms
{
    /// <summary>
    /// Playlist window. This is where the user can view and manipulate playlists. They can
    /// also save the playlists into the library.
    /// </summary>
    public partial class frmPlaylist : BaseForm, IPlaylistView
    {
        // Private variables        
        private frmLoadPlaylist formLoadPlaylist = null;

        public frmPlaylist(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        /// <summary>
        /// Refreshes the "Load playlist"/"Library playlists" menu item with library playlists.
        /// </summary>
        public void RefreshLibraryPlaylistsMenu()
        {
            // Clear sub items
            miLoadPlaylistLibrary.DropDownItems.Clear();

            //// Fetch list of playlists from database
            //List<PlaylistFile> playlistFiles = Main.Library.Gateway.SelectPlaylistFiles();

            //// Add items to menu
            //foreach (PlaylistFile playlistFile in playlistFiles)
            //{
            //    // Extract file name without extension
            //    string fileName = Path.GetFileNameWithoutExtension(playlistFile.FilePath);

            //    // Add item
            //    ToolStripItem item = miLoadPlaylistLibrary.DropDownItems.Add(fileName);
            //    item.Tag = playlistFile.FilePath;
            //    item.ToolTipText = playlistFile.FilePath;
            //    item.Click += new EventHandler(miLoadPlaylistLibraryItem_Click);
            //}
        }

        /// <summary>
        /// Occurs when the user clicks on the "New playlist" button in the toolbar.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnNewPlaylist_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you wish to create a new playlist? You will lose the current playlist if not saved.", "Playlist will be lost", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                OnNewPlaylist();
        }

        /// <summary>
        /// Occurs when the user clicks on the "Open playlist" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnOpenPlaylist_Click(object sender, EventArgs e)
        {
            menuLoadPlaylist.Show(btnOpenPlaylist, new Point(0, btnOpenPlaylist.Height));
        }

        /// <summary>
        /// Occurs when the user clicks on the "Save playlist" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSavePlaylist_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Occurs when the user clicks on the "Save playlist as" button.
        /// Display a modal dialog to the user requesting the playlist name.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnSavePlaylistAs_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Occurs when the user clicks on the "Remove songs" button.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void btnRemoveSongs_Click(object sender, EventArgs e)
        {
            if (viewSongs2.SelectedItems.Count == 0)
                return;

            var ids = viewSongs2.SelectedItems.Select(x => x.PlaylistItemId).ToList();

            var ids2 = new List<Guid>();
            foreach (var sel in viewSongs2.SelectedItems)
            {
                ids2.Add(sel.PlaylistItemId);
            }
            OnRemovePlaylistItems(ids);
        }

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
            if (viewSongs2.SelectedItems.Count == 0)
                return;

            OnSelectPlaylistItem(viewSongs2.SelectedItems[0].AudioFile.Id);
        }

        /// <summary>
        /// Occurs when the user clicks on the "Load playlist" button and the "Browse" menu item.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void miLoadPlaylistBrowse_Click(object sender, EventArgs e)
        {
            //// Display the open playlist file dialog
            //if (dialogLoadPlaylist.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            //{
            //    // The user has canceled the operation
            //    return;
            //}

            //// Check if the player is playing
            //if (Main.Player.IsPlaying)
            //{
            //    // Warn user
            //    if (MessageBox.Show("Loading a new playlist will stop playback. Are you sure you wish to do this?", "Load a new playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            //    {
            //        // The user said no; exit method
            //        return;
            //    }

            //    // Stop playback
            //    Main.Stop();
            //}

            //// Load playlist
            //LoadPlaylist(dialogLoadPlaylist.FileName);
        }

        /// <summary>
        /// Occurs when the user clicks on one of the library playlists in the "Load playlist"/"Library playlists" menu.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void miLoadPlaylistLibraryItem_Click(object sender, EventArgs e)
        {
            //// Cast sender
            //ToolStripItem item = (ToolStripItem)sender;

            //// Check if the player is playing
            //if (Main.Player.IsPlaying)
            //{
            //    // Warn user
            //    if (MessageBox.Show("Loading a new playlist will stop playback. Are you sure you wish to do this?", "Load a new playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            //    {
            //        // The user said no; exit method
            //        return;
            //    }

            //    // Stop playback
            //    Main.Stop();
            //}

            //// Load playlist
            //LoadPlaylist(item.Tag.ToString());
        }

        /// <summary>
        /// Occurs when the user double clicks on one of the item in the playlist song view.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void viewSongs2_DoubleClick(object sender, EventArgs e)
        {
            if (viewSongs2.SelectedItems.Count == 0)
                return;

            OnSelectPlaylistItem(viewSongs2.SelectedItems[0].AudioFile.Id);
        }

        #region IPlaylistView implementation

        public Action<Guid, int> OnChangePlaylistItemOrder { get; set; }
        public Action<Guid> OnSelectPlaylistItem { get; set; }
        public Action<List<Guid>> OnRemovePlaylistItems { get; set; }
        public Action OnNewPlaylist { get; set; }
        public Action<string> OnLoadPlaylist { get; set; }
        public Action OnSavePlaylist { get; set; }
        public Action OnShufflePlaylist { get; set; }

        public void PlaylistError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                MessageBox.Show(string.Format("An error occured in Playlist: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshPlaylist(Playlist playlist)
        {
            MethodInvoker methodUIUpdate = delegate {
                viewSongs2.ImportAudioFiles(playlist.Items.Select(x => x.AudioFile).ToList());
                this.Text = !String.IsNullOrEmpty(playlist.FilePath) ? playlist.FilePath : "Unsaved playlist";
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                viewSongs2.NowPlayingPlaylistItemId = audioFile.Id;
                viewSongs2.Refresh();
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        #endregion

    }
}
