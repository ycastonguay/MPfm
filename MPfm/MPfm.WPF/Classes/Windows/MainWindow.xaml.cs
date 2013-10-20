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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MPfm.Core;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow, IMainView
    {
        private List<LibraryBrowserEntity> _itemsLibraryBrowser;

        public MainWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == miFile_AddFiles)
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = "Audio files (*.mp3,*.flac,*.ogg, *.ape)|*.mp3;*.flac;*.ogg;*.ape";
                dialog.Multiselect = true;
                dialog.Title = "Add file(s) to library";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    
                }
            }
            else if (sender == miFile_AddFolder)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "Please select a folder to add to the music library";
                dialog.ShowNewFolderButton = false;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                }
            }
            else if (sender == miFile_OpenAudioFiles)
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = "Playlist/Audio files (*.mp3,*.flac,*.ogg, *.ape, *.wv, *.mpc, *.wav, *.m3u, *.m3u8, *.pls, *.xspf)|*.mp3;*.flac;*.ogg;*.ape;*.wav;*.wv;*.mpc;*.m3u;*.m3u8;*.pls;*.xspf";
                dialog.Multiselect = true;
                dialog.Title = "Select audio files or a playlist file to play";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                }
            }
            else if (sender == miWindows_Sync)
            {
                OnOpenSyncWindow();
            }
            else if (sender == miWindows_SyncCloud)
            {
                OnOpenSyncCloudWindow();
            }
            else if (sender == miWindows_SyncWebBrowser)
            {
                OnOpenSyncWebBrowserWindow();
            }
        }

        private void treeViewLibrary_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Tracing.Log("TreeViewLibraryOnSelectedItemChanged");
        }

        private void BtnToolbar_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender == btnPrevious)
                OnPlayerPrevious();
            else if (sender == btnPlayPause)
                OnPlayerPause();
            else if (sender == btnNext)
                OnPlayerNext();
            else if (sender == btnShuffle)
                OnPlayerShuffle();
            else if (sender == btnRepeat)
                OnPlayerRepeat();
            else if (sender == btnPlaylist)
                OnOpenPlaylistWindow();
            else if (sender == btnEffects)
                OnOpenEffectsWindow();
            else if (sender == btnPreferences)
                OnOpenPreferencesWindow();
            else if (sender == btnResumePlayback)
                OnOpenResumePlayback();
            else if (sender == btnSync)
                OnOpenSyncWindow();
            else if (sender == btnSyncCloud)
                OnOpenSyncCloudWindow();
        }

        #region IMainView implementation

        public Action OnOpenPreferencesWindow { get; set; }
        public Action OnOpenEffectsWindow { get; set; }
        public Action OnOpenPlaylistWindow { get; set; }
        public Action OnOpenSyncWindow { get; set; }
        public Action OnOpenSyncCloudWindow { get; set; }
        public Action OnOpenSyncWebBrowserWindow { get; set; }
        public Action OnOpenResumePlayback { get; set; }
        public Action<List<string>> OnAddFilesToLibrary { get; set; }
        public Action<string> OnAddFolderToLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }

        #endregion

        #region ILibraryBrowserView implementation

        public Action<AudioFileFormat> OnAudioFileFormatFilterChanged { get; set; }
        public Action<LibraryBrowserEntity> OnTreeNodeSelected { get; set; }
        public Action<LibraryBrowserEntity> OnTreeNodeDoubleClicked { get; set; }
        public Action<LibraryBrowserEntity, object> OnTreeNodeExpanded { get; set; }
        public Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable { get; set; }

        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                _itemsLibraryBrowser = entities.ToList();
                treeViewLibrary.ItemsSource = _itemsLibraryBrowser;
                

                //treeViewLibrary
                //treeViewLibrary.Items.Clear();


                //    foreach (var entity in entities)
                //    {
                //        var node = treeViewLibrary.Items.Add(entity.Title);                        
                        

                //        //        var node = new TreeNode(entity.Title);
                //        //        node.Tag = entity;
                //        //        switch (entity.EntityType)
                //        //        {
                //        //            case LibraryBrowserEntityType.AllSongs:
                //        //                node.ImageIndex = 12;
                //        //                node.SelectedImageIndex = 12;
                //        //                break;
                //        //            case LibraryBrowserEntityType.Artists:
                //        //                node.ImageIndex = 26;
                //        //                node.SelectedImageIndex = 26;
                //        //                break;
                //        //            case LibraryBrowserEntityType.Albums:
                //        //                node.ImageIndex = 25;
                //        //                node.SelectedImageIndex = 25;
                //        //                break;
                //        //        }

                //        //        if (entity.EntityType != LibraryBrowserEntityType.AllSongs)
                //        //            node.Nodes.Add("dummy", "dummy");

                //        //        treeLibraryBrowser.Nodes.Add(node);
                //    }

            }));
        }

        public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
        {
            //Console.WriteLine("frmMain - RefreshLibraryBrowserNode - entities.Count: {0}", entities.Count());
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    var node = (TreeNode)userData;
            //    treeLibraryBrowser.BeginUpdate();

            //    node.Nodes.Clear();

            //    foreach (var childEntity in entities)
            //    {
            //        var childNode = new TreeNode(childEntity.Title);
            //        childNode.Tag = childEntity;
            //        switch (childEntity.EntityType)
            //        {
            //            case LibraryBrowserEntityType.Artist:
            //                childNode.ImageIndex = 24;
            //                childNode.SelectedImageIndex = 24;
            //                break;
            //            case LibraryBrowserEntityType.Album:
            //            case LibraryBrowserEntityType.ArtistAlbum:
            //                childNode.ImageIndex = 25;
            //                childNode.SelectedImageIndex = 25;
            //                break;
            //        }

            //        if (childEntity.EntityType != LibraryBrowserEntityType.Album)
            //            childNode.Nodes.Add("dummy", "dummy");

            //        node.Nodes.Add(childNode);
            //    }

            //    treeLibraryBrowser.EndUpdate();
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        #endregion

        #region ISongBrowserView implementation

        public Action<AudioFile> OnTableRowDoubleClicked { get; set; }
        public Action<AudioFile> OnSongBrowserEditSongMetadata { get; set; }
        public Action<string> OnSearchTerms { get; set; }

        public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
        {
            ////Console.WriteLine("frmMain - RefreshSongBrowser - audioFiles.Count: {0}", audioFiles.Count());
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    //string orderBy = viewSongs2.OrderByFieldName;
            //    //bool orderByAscending = viewSongs2.OrderByAscending;
            //    viewSongs2.ImportAudioFiles(audioFiles.ToList());
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        #endregion

        #region IPlayerView implementation

        public Action OnPlayerPlay { get; set; }
        public Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        public Action OnPlayerPause { get; set; }
        public Action OnPlayerStop { get; set; }
        public Action OnPlayerPrevious { get; set; }
        public Action OnPlayerNext { get; set; }
        public Action OnPlayerShuffle { get; set; }
        public Action OnPlayerRepeat { get; set; }
        public Action<float> OnPlayerSetVolume { get; set; }
        public Action<float> OnPlayerSetPitchShifting { get; set; }
        public Action<float> OnPlayerSetTimeShifting { get; set; }
        public Action<float> OnPlayerSetPosition { get; set; }
        public Func<float, PlayerPositionEntity> OnPlayerRequestPosition { get; set; }
        public Action OnEditSongMetadata { get; set; }
        public Action OnOpenPlaylist { get; set; }

        public void PlayerError(Exception ex)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    MessageBox.Show(string.Format("An error occured in Player: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{

            //    trackPosition.Enabled = status == PlayerStatusType.Playing;

            //    if (status == PlayerStatusType.Playing)
            //    {
            //        btnPlay.Text = "Pause";
            //        btnPlay.Image = MPfm.Windows.Properties.Resources.control_pause;
            //    }
            //    else
            //    {
            //        btnPlay.Text = "Play";
            //        btnPlay.Image = MPfm.Windows.Properties.Resources.control_play;
            //    }
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    if (!_isPlayerPositionChanging)
            //    {
            //        lblCurrentPosition.Text = entity.Position;
            //        miTraySongPosition.Text = string.Format("[ {0} / {1} ]", entity.Position, lblLength.Text);
            //        trackPosition.Value = (int)entity.PositionPercentage * 10;
            //    }

            //    if (!waveFormMarkersLoops.IsLoading)
            //        waveFormMarkersLoops.SetPosition(entity.PositionBytes, entity.Position);
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    btnAddLoop.Enabled = audioFile != null;
            //    btnAddMarker.Enabled = audioFile != null;

            //    if (audioFile == null)
            //    {
            //        lblCurrentArtistName.Text = string.Empty;
            //        lblCurrentAlbumTitle.Text = string.Empty;
            //        lblCurrentSongTitle.Text = string.Empty;
            //        lblCurrentFilePath.Text = string.Empty;
            //        lblFrequency.Text = string.Empty;
            //        lblBitrate.Text = string.Empty;
            //        lblBitsPerSample.Text = string.Empty;
            //        lblSoundFormat.Text = string.Empty;
            //        lblYear.Text = string.Empty;
            //        lblMonoStereo.Text = string.Empty;
            //        lblFileSize.Text = string.Empty;
            //        lblGenre.Text = string.Empty;
            //        lblPlayCount.Text = string.Empty;
            //        lblLastPlayed.Text = string.Empty;
            //    }
            //    else
            //    {
            //        lblCurrentArtistName.Text = audioFile.ArtistName;
            //        lblCurrentAlbumTitle.Text = audioFile.AlbumTitle;
            //        lblCurrentSongTitle.Text = audioFile.Title;
            //        lblCurrentFilePath.Text = audioFile.FilePath;
            //        lblLength.Text = audioFile.Length;
            //        lblFrequency.Text = string.Format("{0} Hz", audioFile.SampleRate);
            //        lblBitrate.Text = string.Format("{0} kbps", audioFile.Bitrate);
            //        lblBitsPerSample.Text = string.Format("{0} bits", audioFile.BitsPerSample);
            //        lblSoundFormat.Text = audioFile.FileType.ToString();
            //        lblYear.Text = audioFile.Year.ToString();
            //        lblMonoStereo.Text = audioFile.AudioChannels == 1 ? "Mono" : "Stereo";
            //        lblFileSize.Text = string.Format("{0} bytes", audioFile.FileSize);
            //        lblGenre.Text = string.Format("{0}", audioFile.Genre);
            //        lblPlayCount.Text = string.Format("{0} times played", audioFile.PlayCount);
            //        lblLastPlayed.Text = audioFile.LastPlayed.HasValue ? string.Format("Last played on {0}", audioFile.LastPlayed.Value.ToShortDateString()) : "";

            //        miTrayArtistName.Text = audioFile.ArtistName;
            //        miTrayAlbumTitle.Text = audioFile.AlbumTitle;
            //        miTraySongTitle.Text = audioFile.Title;

            //        viewSongs2.NowPlayingAudioFileId = audioFile.Id;
            //        viewSongs2.Refresh();

            //        try
            //        {
            //            // Update the album art in an another thread
            //            workerAlbumArt.RunWorkerAsync(audioFile.FilePath);
            //        }
            //        catch
            //        {
            //            // Just do nothing if thread is busy
            //        }

            //        // Configure wave form
            //        waveFormMarkersLoops.Length = lengthBytes;
            //        waveFormMarkersLoops.LoadWaveForm(audioFile.FilePath);
            //    }
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshMarkers(IEnumerable<Marker> markers)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    _markers = markers.ToList();
            //    viewMarkers.Items.Clear();
            //    foreach (Marker marker in markers)
            //    {
            //        ListViewItem item = viewMarkers.Items.Add(marker.Name);
            //        item.Tag = marker.MarkerId;
            //        item.SubItems.Add(marker.Position);
            //        item.SubItems.Add(marker.Comments);
            //        item.SubItems.Add(marker.PositionBytes.ToString());
            //    }
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshLoops(IEnumerable<Loop> loops)
        {
        }

        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    lblVolume.Text = entity.VolumeString;
            //    if (faderVolume.Value != (int)entity.Volume)
            //        faderVolume.Value = (int)entity.Volume;
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {
        }

        #endregion

        #region IMarkersView implementation

        public Action OnAddMarker { get; set; }
        public Action<MarkerTemplateNameType> OnAddMarkerWithTemplate { get; set; }
        public Action<Marker> OnEditMarker { get; set; }
        public Action<Marker> OnSelectMarker { get; set; }
        public Action<Marker> OnDeleteMarker { get; set; }

        public void MarkerError(Exception ex)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    MessageBox.Show(string.Format("An error occured in Markers: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshMarkers(List<Marker> markers)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    _markers = markers.ToList();
            //    viewMarkers.Items.Clear();
            //    foreach (Marker marker in markers)
            //    {
            //        ListViewItem item = viewMarkers.Items.Add(marker.Name);
            //        item.Tag = marker.MarkerId;
            //        item.SubItems.Add(marker.Position);
            //        item.SubItems.Add(marker.Comments);
            //        item.SubItems.Add(marker.PositionBytes.ToString());
            //    }
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        #endregion

        #region ILoopsView implementation

        public Action OnAddLoop { get; set; }
        public Action<Loop> OnEditLoop { get; set; }

        public void LoopError(Exception ex)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    MessageBox.Show(string.Format("An error occured in Loops: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshLoops(List<Loop> loops)
        {
        }

        #endregion

        #region ITimeShiftingView implementation

        public Action<float> OnSetTimeShifting { get; set; }
        public Action OnResetTimeShifting { get; set; }
        public Action OnUseDetectedTempo { get; set; }
        public Action OnIncrementTempo { get; set; }
        public Action OnDecrementTempo { get; set; }

        public void TimeShiftingError(Exception ex)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    MessageBox.Show(string.Format("An error occured in TimeShifting: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshTimeShifting(PlayerTimeShiftingEntity entity)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    lblDetectedTempoValue.Text = entity.DetectedTempo;
            //    lblReferenceTempoValue.Text = entity.ReferenceTempo;
            //    lblCurrentTempoValue.Text = entity.CurrentTempo;
            //    trackTempo.SetValueWithoutTriggeringEvent((int)entity.TimeShiftingValue);
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        #endregion

        #region IPitchShiftingView implementation

        public Action<int> OnChangeKey { get; set; }
        public Action<int> OnSetInterval { get; set; }
        public Action OnResetInterval { get; set; }
        public Action OnIncrementInterval { get; set; }
        public Action OnDecrementInterval { get; set; }

        public void PitchShiftingError(Exception ex)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    MessageBox.Show(string.Format("An error occured in PitchShifting: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        public void RefreshKeys(List<Tuple<int, string>> keys)
        {
        }

        public void RefreshPitchShifting(PlayerPitchShiftingEntity entity)
        {
            //MethodInvoker methodUIUpdate = delegate
            //{
            //    lblIntervalValue.Text = entity.Interval;
            //    lblCurrentKeyValue.Text = entity.NewKey.Item2;
            //    lblReferenceKeyValue.Text = entity.ReferenceKey.Item2;
            //    trackPitch.SetValueWithoutTriggeringEvent(entity.IntervalValue);
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        #endregion

    }
}
