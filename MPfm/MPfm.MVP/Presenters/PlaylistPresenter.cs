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

using System.Collections.Generic;
using MPfm.Core;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;
using MPfm.MVP.Messages;
using System;
using MPfm.MVP.Services.Interfaces;

namespace MPfm.MVP.Presenters
{
	public class PlaylistPresenter : BasePresenter<IPlaylistView>, IPlaylistPresenter
	{
        readonly ITinyMessengerHub _messageHub;
        readonly IPlayerService _playerService;

        public PlaylistPresenter(ITinyMessengerHub messageHub, IPlayerService playerService)
		{
            _messageHub = messageHub;
            _playerService = playerService;

            _messageHub.Subscribe<PlayerPlaylistUpdatedMessage>((PlayerPlaylistUpdatedMessage m) => {
                Tracing.Log("PlaylistPresenter - PlayerPlaylistUpdated - Refreshing items...");
                RefreshItems();
            });
            _messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>(PlayerPlaylistIndexChanged);
		}
 
        public override void BindView(IPlaylistView view)
        {
            view.OnNewPlaylist = NewPlaylist;
            view.OnLoadPlaylist = LoadPlaylist;
            view.OnSavePlaylist = SavePlaylist;
            view.OnShufflePlaylist = ShufflePlaylist;
            view.OnChangePlaylistItemOrder = ChangePlaylistItemOrder;
            view.OnRemovePlaylistItems = RemovePlaylistItems;
            view.OnSelectPlaylistItem = SelectPlaylistItem;

            base.BindView(view);
            Initialize();
        }

        private void Initialize()
        {
            RefreshItems();

            if(_playerService.IsPlaying)
                View.RefreshCurrentlyPlayingSong(_playerService.CurrentPlaylist.CurrentItemIndex, _playerService.CurrentPlaylist.CurrentItem.AudioFile);
        }

        private void PlayerPlaylistIndexChanged(PlayerPlaylistIndexChangedMessage message)
        {
            View.RefreshCurrentlyPlayingSong(_playerService.CurrentPlaylist.CurrentItemIndex, _playerService.CurrentPlaylist.CurrentItem.AudioFile);
        }

        private void RefreshItems()
        {
            try
            {
                View.RefreshPlaylist(_playerService.CurrentPlaylist);
            }
            catch(Exception ex)
            {
                Tracing.Log("PlaylistPresenter - RefreshItems - Exception: {0}", ex);
                View.PlaylistError(ex);
            }        
        }

        private void NewPlaylist()
        {
            try
            {
                if(_playerService.IsPlaying)
                    _playerService.Stop();

                _playerService.CurrentPlaylist.Clear();
            }
            catch(Exception ex)
            {
                Tracing.Log("PlaylistPresenter - NewPlaylist - Exception: {0}", ex);
                View.PlaylistError(ex);
            }        
        }

        private void LoadPlaylist(string filePath)
        {
        }

        private void SavePlaylist()
        {
        }

        private void ShufflePlaylist()
        {
        }

        private void ChangePlaylistItemOrder(Guid playlistItemId, int newIndex)
        {
        }

        private void SelectPlaylistItem(Guid playlistItemId)
        {
            try
            {
                _playerService.GoTo(playlistItemId);
            }
            catch(Exception ex)
            {
                Tracing.Log("PlaylistPresenter - SelectPlaylistItem - Exception: {0}", ex);
                View.PlaylistError(ex);
            }
        }

        private void RemovePlaylistItems(List<Guid> playlistItemIds)
        {
            try
            {
                _playerService.CurrentPlaylist.RemoveItems(playlistItemIds);
                View.RefreshPlaylist(_playerService.CurrentPlaylist);
            }
            catch (Exception ex)
            {
                Tracing.Log("PlaylistPresenter - SelectPlaylistItem - Exception: {0}", ex);
                View.PlaylistError(ex);
            }
        }

        ///// <summary>
        ///// Loads a playlist.
        ///// </summary>
        ///// <param name="playlistFilePath">Playlist file path</param>
        //public void LoadPlaylist(string playlistFilePath)
        //{
        //    //try
        //    //{
        //    //    // Create window
        //    //    formLoadPlaylist = new frmLoadPlaylist(Main, playlistFilePath);

        //    //    // Show Load playlist dialog (progress bar)
        //    //    DialogResult dialogResult = formLoadPlaylist.ShowDialog(this);
        //    //    if (dialogResult == System.Windows.Forms.DialogResult.OK)
        //    //    {
        //    //        // Get audio files
        //    //        List<AudioFile> audioFiles = formLoadPlaylist.AudioFiles;

        //    //        // Check if any audio files have failed loading
        //    //        List<string> failedAudioFilePaths = formLoadPlaylist.FailedAudioFilePaths;

        //    //        // Clear player playlist
        //    //        Main.Player.Playlist.Clear();
        //    //        Main.Player.Playlist.FilePath = playlistFilePath;

        //    //        // Make sure there are audio files to add to the player playlist
        //    //        if (audioFiles.Count > 0)
        //    //        {
        //    //            // Add audio files                        
        //    //            Main.Player.Playlist.AddItems(audioFiles);
        //    //            Main.Player.Playlist.First();
        //    //        }

        //    //        // Refresh song browser
        //    //        RefreshTitle();
        //    //        RefreshPlaylist();                    
        //    //        RefreshPlaylistPlayIcon(Guid.Empty);

        //    //        // Check if any files could not be loaded
        //    //        if (failedAudioFilePaths != null && failedAudioFilePaths.Count > 0)
        //    //        {
        //    //            // Check if the user wants to see the list of files
        //    //            if (MessageBox.Show("Some files in the playlist could not be loaded. Do you wish to see the list of files in a text editor?", "Some files could not be loaded", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
        //    //            {
        //    //                // Get temp file path
        //    //                string tempFilePath = Path.GetTempPath() + "MPfm_PlaylistLog_" + Conversion.DateTimeToUnixTimestamp(DateTime.Now).ToString("0.0000000").Replace(".", "") + ".txt";

        //    //                // Create temporary file
        //    //                TextWriter tw = null;
        //    //                try
        //    //                {
        //    //                    // Open text writer
        //    //                    tw = new StreamWriter(tempFilePath);
        //    //                    foreach (string item in failedAudioFilePaths)
        //    //                    {
        //    //                        tw.WriteLine(item);
        //    //                    }                                
        //    //                }
        //    //                catch (Exception ex)
        //    //                {
        //    //                    // Display error
        //    //                    MessageBox.Show("Failed to save the file to " + tempFilePath + "!\n\nException:\n" + ex.Message + "\n" + ex.StackTrace, "Failed to save the file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //                }
        //    //                finally
        //    //                {
        //    //                    tw.Close();
        //    //                }

        //    //                // Start notepad
        //    //                Process.Start(tempFilePath);
        //    //            }
        //    //        }

        //    //        // Check if the playlist is empty
        //    //        if (audioFiles.Count == 0)
        //    //        {
        //    //            // Warn user that the playlist is empty
        //    //            MessageBox.Show("The playlist file is empty or does not contain any valid file!", "Playlist is empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    //        }
        //    //    }

        //    //    // Dispose form
        //    //    formLoadPlaylist.Dispose();
        //    //    formLoadPlaylist = null;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    MessageBox.Show(ex.Message, "Error loading playlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //}
        //}

        ///// <summary>
        ///// Saves the playlist in the specified file path.
        ///// </summary>
        ///// <param name="playlistFilePath">Playlist file path</param>        
        //public void SavePlaylist(string playlistFilePath)
        //{
        //    //bool relativePath = false;

        //    //// Change cursor
        //    //Cursor.Current = Cursors.WaitCursor;

        //    //// Set playlist file path
        //    //Main.Player.Playlist.FilePath = playlistFilePath;

        //    //// Check if the path is in the same 
        //    //DialogResult dialogResult = MessageBox.Show("Do you wish to use relative paths instead of absolute paths when possible?\n\nA full path or absolute path is a path that points to the same location on one file system regardless of the working directory or combined paths.\n\nA relative path is a path relative to the working directory of the user or application, so the full absolute path will not have to be given.", "Use relative paths", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

        //    //// Check result
        //    //if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
        //    //{
        //    //    // Cancel
        //    //    return;
        //    //}
        //    //else if (dialogResult == System.Windows.Forms.DialogResult.Yes)
        //    //{
        //    //    // Save playlist
        //    //    //SavePlaylist(dialogSavePlaylist.FileName, true);
        //    //    relativePath = true;
        //    //}
        //    //else if (dialogResult == System.Windows.Forms.DialogResult.No)
        //    //{
        //    //    // Save playlist
        //    //    //SavePlaylist(dialogSavePlaylist.FileName, false);
        //    //    relativePath = false;
        //    //}

        //    //// Determine what format the user has chosen
        //    //if (dialogSavePlaylist.FileName.ToUpper().Contains(".M3U8"))
        //    //{
        //    //    // Save playlist
        //    //    PlaylistTools.SaveM3UPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist, relativePath, true);
        //    //}
        //    //else if (dialogSavePlaylist.FileName.ToUpper().Contains(".M3U"))
        //    //{
        //    //    // Save playlist
        //    //    PlaylistTools.SaveM3UPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist, relativePath, false);
        //    //}
        //    //else if (dialogSavePlaylist.FileName.ToUpper().Contains(".PLS"))
        //    //{
        //    //    // Save playlist
        //    //    PlaylistTools.SavePLSPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist, relativePath);
        //    //}
        //    //else if (dialogSavePlaylist.FileName.ToUpper().Contains(".XSPF"))
        //    //{
        //    //    // Save playlist
        //    //    PlaylistTools.SaveXSPFPlaylist(dialogSavePlaylist.FileName, Main.Player.Playlist, relativePath);
        //    //}

        //    //// Change cursor
        //    //Cursor.Current = Cursors.Default;

        //    //// Refresh title
        //    //RefreshTitle();
        //}

        //// Check if playlist file is valid
        //if (!String.IsNullOrEmpty(Main.Player.Playlist.FilePath))
        //{
        //    // Check if file exists
        //    if (File.Exists(Main.Player.Playlist.FilePath))
        //    {
        //        // Warn user is he wants to save
        //        if(MessageBox.Show("Warning: The playlist file already exists. Do you wish to overwrite this file?", "The playlist file already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
        //        {
        //            // The user wants to save
        //            SavePlaylist(Main.Player.Playlist.FilePath);
        //        }
        //    }
        //}
        //else
        //{
        //    // Ask user for file path
        //    SavePlaylistAs();
        //}
	}
}
