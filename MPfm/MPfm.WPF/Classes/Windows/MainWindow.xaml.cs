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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MPfm.Core;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsControls;
using MPfm.WPF.Classes.Controls;
using MPfm.WPF.Classes.Helpers;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow, IMainView
    {
        private List<LibraryBrowserEntity> _itemsLibraryBrowser;
        private bool _isPlayerPositionChanging;

        public MainWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            SetLegacyControlTheme();
            ViewIsReady();

            gridViewSongs.DoubleClick += GridViewSongsOnDoubleClick;
        }

        private void SetLegacyControlTheme()
        {
            var fontRow = new CustomFont("Roboto", 8, System.Drawing.Color.FromArgb(255, 0, 0, 0));
            var fontHeader = new CustomFont("Roboto", 8, System.Drawing.Color.FromArgb(255, 255, 255, 255));
            gridViewSongs.Theme.AlbumCoverBackgroundGradient = new BackgroundGradient(System.Drawing.Color.FromArgb(255, 36, 47, 53), System.Drawing.Color.FromArgb(255, 36, 47, 53), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0);
            gridViewSongs.Theme.HeaderHoverTextGradient = new TextGradient(System.Drawing.Color.FromArgb(255, 69, 88, 101), System.Drawing.Color.FromArgb(255, 69, 88, 101), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0, fontHeader);
            gridViewSongs.Theme.HeaderTextGradient = new TextGradient(System.Drawing.Color.FromArgb(255, 69, 88, 101), System.Drawing.Color.FromArgb(255, 69, 88, 101), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0, fontHeader);
            gridViewSongs.Theme.IconNowPlayingGradient = new Gradient(System.Drawing.Color.FromArgb(255, 250, 200, 250), System.Drawing.Color.FromArgb(255, 25, 150, 25), LinearGradientMode.Horizontal);
            gridViewSongs.Theme.RowNowPlayingTextGradient = new TextGradient(System.Drawing.Color.FromArgb(255, 135, 235, 135), System.Drawing.Color.FromArgb(255, 135, 235, 135), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0, fontRow);
            gridViewSongs.Theme.RowTextGradient = new TextGradient(System.Drawing.Color.White, System.Drawing.Color.White, LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0, fontRow);

            faderVolume.FaderHeight = 28;
            faderVolume.FaderWidth = 10;
            faderVolume.Theme.BackgroundGradient = new BackgroundGradient(System.Drawing.Color.FromArgb(255, 36, 47, 53), System.Drawing.Color.FromArgb(255, 36, 47, 53), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0);
            faderVolume.Theme.FaderGradient = new BackgroundGradient(System.Drawing.Color.White, System.Drawing.Color.WhiteSmoke, LinearGradientMode.Vertical, System.Drawing.Color.Gray, 0);
            faderVolume.Theme.FaderShadowGradient = new BackgroundGradient(System.Drawing.Color.FromArgb(255, 188, 188, 188), System.Drawing.Color.Gainsboro, LinearGradientMode.Vertical, System.Drawing.Color.Gray, 0);

            outputMeter.Theme.BackgroundGradient = new BackgroundGradient(System.Drawing.Color.FromArgb(255, 36, 47, 53), System.Drawing.Color.FromArgb(255, 36, 47, 53), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0);

            waveFormDisplay.Theme.BackgroundGradient = new BackgroundGradient(System.Drawing.Color.FromArgb(255, 36, 47, 53), System.Drawing.Color.FromArgb(255, 36, 47, 53), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0);            
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
                    OnAddFilesToLibrary(dialog.FileNames.ToList());
            }
            else if (sender == miFile_AddFolder)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "Please select a folder to add to the music library";
                dialog.ShowNewFolderButton = false;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    OnAddFolderToLibrary(dialog.SelectedPath);
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
            else if (sender == miFile_UpdateLibrary)
            {
                OnUpdateLibrary();
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
            //Tracing.Log("treeViewLibrary_SelectedItemChanged");
            if (e.NewValue == null)
                return;

            var treeViewItem = e.NewValue as TreeViewItem;
            var entity = treeViewItem.Header as LibraryBrowserEntity;
            OnTreeNodeSelected(entity);
        }

        private void treeViewLibrary_OnExpanded(object sender, RoutedEventArgs e)
        {            
            //Tracing.Log("treeViewLibrary_OnExpanded");
            var item = e.OriginalSource as MPfmTreeViewItem;
            if (item != null && item.Items.Count == 1)
            {
                var firstItem = item.Items[0] as MPfmTreeViewItem;
                if (firstItem.IsDummyNode)
                {
                    item.Items.Clear();
                    var entity = item.Header as LibraryBrowserEntity;
                    OnTreeNodeExpanded(entity, item);
                }
            }
            e.Handled = true;
        }

        private void gridViewSongs_OnOnSelectedIndexChanged(SongGridViewSelectedIndexChangedData data)
        {
        //    if (gridViewSongs.SelectedItems.Count == 0)
        //        return;

        //    OnTableRowDoubleClicked(gridViewSongs.Items[0].AudioFile);
        }

        private void GridViewSongsOnDoubleClick(object sender, EventArgs eventArgs)
        {
            if (gridViewSongs.SelectedItems.Count == 0)
                return;

            OnTableRowDoubleClicked(gridViewSongs.SelectedItems[0].AudioFile);
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

        private void txtSearchTerms_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnSearchTerms(txtSearchTerms.Text);
        }

        private void TrackPosition_OnMouseDown(object sender, MouseEventArgs e)
        {
            _isPlayerPositionChanging = true;
        }

        private void TrackPosition_OnMouseUp(object sender, MouseEventArgs e)
        {
            _isPlayerPositionChanging = false;
            OnPlayerSetPosition((float) trackPosition.Value / 10f);
        }

        private void TrackPosition_OnTrackBarValueChanged()
        {
            if (OnPlayerRequestPosition == null || !_isPlayerPositionChanging)
                return;
            
            var position = OnPlayerRequestPosition((float) trackPosition.Value/1000f);
            lblPosition.Content = position.Position;
        }

        private void FaderVolume_OnFaderValueChanged(object sender, EventArgs e)
        {
            OnPlayerSetVolume(faderVolume.Value);
        }

        private void BtnTimeShifting_OnClick(object sender, RoutedEventArgs e)
        {
            ResetHeaderButtonStyles();
            ResetHeaderPanelVisibility();
            btnTimeShifting.Style = System.Windows.Application.Current.Resources["HeaderButtonSelected"] as Style;
            panelTimeShifting.Visibility = Visibility.Visible;            
        }

        private void BtnPitchShifting_OnClick(object sender, RoutedEventArgs e)
        {
            ResetHeaderButtonStyles();
            ResetHeaderPanelVisibility();
            btnPitchShifting.Style = System.Windows.Application.Current.Resources["HeaderButtonSelected"] as Style;
            panelPitchShifting.Visibility = Visibility.Visible;
        }

        private void BtnInfo_OnClick(object sender, RoutedEventArgs e)
        {
            ResetHeaderButtonStyles();
            ResetHeaderPanelVisibility();
            btnInfo.Style = System.Windows.Application.Current.Resources["HeaderButtonSelected"] as Style;
            panelInfo.Visibility = Visibility.Visible;
        }

        private void BtnActions_OnClick(object sender, RoutedEventArgs e)
        {
            ResetHeaderButtonStyles();
            ResetHeaderPanelVisibility();
            btnActions.Style = System.Windows.Application.Current.Resources["HeaderButtonSelected"] as Style;
            panelActions.Visibility = Visibility.Visible;
        }

        private void ResetHeaderPanelVisibility()
        {
            panelTimeShifting.Visibility = Visibility.Hidden;
            panelPitchShifting.Visibility = Visibility.Hidden;
            panelInfo.Visibility = Visibility.Hidden;
            panelActions.Visibility = Visibility.Hidden;
        }

        private void ResetHeaderButtonStyles()
        {
            var res = System.Windows.Application.Current.Resources;
            btnTimeShifting.Style = res["HeaderButton"] as Style;
            btnPitchShifting.Style = res["HeaderButton"] as Style;
            btnInfo.Style = res["HeaderButton"] as Style;
            btnActions.Style = res["HeaderButton"] as Style;
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
                treeViewLibrary.Items.Clear();
                foreach (var entity in entities)
                {
                    var item = new MPfmTreeViewItem();
                    //item.Expanding += (sender, args) => { Console.WriteLine("Expanding"); };
                    item.Header = entity;
                    item.HeaderTemplate = FindResource("TreeViewItemTemplate") as DataTemplate;

                    if (entity.SubItems.Count > 0)
                    {
                        var dummy = new MPfmTreeViewItem();
                        dummy.IsDummyNode = true;
                        item.Items.Add(dummy);
                    }

                    treeViewLibrary.Items.Add(item);
                }
            }));
        }

        public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
        {
            Console.WriteLine("MainWindow - RefreshLibraryBrowserNode - entities.Count: {0}", entities.Count());
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                var item = (MPfmTreeViewItem) userData;
                foreach (var subentity in entities)
                {
                    var subitem = new MPfmTreeViewItem();
                    subitem.Header = subentity;
                    subitem.HeaderTemplate = FindResource("TreeViewItemTemplate") as DataTemplate;

                    if (subentity.SubItems.Count > 0)
                    {
                        var dummy = new MPfmTreeViewItem();
                        dummy.IsDummyNode = true;
                        subitem.Items.Add(dummy);
                    }

                    item.Items.Add(subitem);
                }
            }));
        }

        #endregion

        #region ISongBrowserView implementation

        public Action<AudioFile> OnTableRowDoubleClicked { get; set; }
        public Action<AudioFile> OnSongBrowserEditSongMetadata { get; set; }
        public Action<string> OnSearchTerms { get; set; }

        public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                //    //string orderBy = viewSongs2.OrderByFieldName;
                //    //bool orderByAscending = viewSongs2.OrderByAscending;
                gridViewSongs.ImportAudioFiles(audioFiles.ToList());
            }));
        }

        #endregion

        #region IPlayerView implementation

        public bool IsOutputMeterEnabled { get; private set; }
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
        public Action OnOpenEffects { get; set; }

        public void PlayerError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void PushSubView(IBaseView view)
        {
        }

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                //    trackPosition.Enabled = status == PlayerStatusType.Playing;
                if (status == PlayerStatusType.Playing)
                {
                    imagePlayPause.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Toolbar/pause.png"));
                }
                else
                {
                    imagePlayPause.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Toolbar/play.png"));
                }
            }));
        }

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (!_isPlayerPositionChanging)
                {
                    lblPosition.Content = entity.Position;
                    //miTraySongPosition.Text = string.Format("[ {0} / {1} ]", entity.Position, lblLength.Text);
                    trackPosition.Value = (int)entity.PositionPercentage * 10;
                }

                if (!waveFormDisplay.IsLoading)
                    waveFormDisplay.SetPosition(entity.PositionBytes, entity.Position);
            }));
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                //    btnAddLoop.Enabled = audioFile != null;
                //    btnAddMarker.Enabled = audioFile != null;

                    if (audioFile == null)
                    {
                        lblArtistName.Content = string.Empty;
                        lblAlbumTitle.Content = string.Empty;
                        lblSongTitle.Content = string.Empty;
                        lblFilePath.Content = string.Empty;
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
                    }
                    else
                    {
                        lblArtistName.Content = audioFile.ArtistName;
                        lblAlbumTitle.Content = audioFile.AlbumTitle;
                        lblSongTitle.Content = audioFile.Title;
                        lblFilePath.Content = audioFile.FilePath;
                        lblLength.Content = audioFile.Length;
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

                        gridViewSongs.NowPlayingAudioFileId = audioFile.Id;
                        gridViewSongs.Refresh();

                        // Configure wave form
                        waveFormDisplay.Length = lengthBytes;
                        waveFormDisplay.LoadWaveForm(audioFile.FilePath);

                        int albumWidth = (int) imageAlbum.Width;
                        int albumHeight = (int) imageAlbum.Height;
                        var task = Task<System.Drawing.Image>.Factory.StartNew(() =>
                        {
                            //// Get image from library
                            //Image image = MPfm.Library.Library.GetAlbumArtFromID3OrFolder(songPath);

                            //// Resize image with quality AA
                            //if (image != null)
                            //    image = ImageManipulation.ResizeImage(image, picAlbum.Size.Width, picAlbum.Size.Height);

                            try
                            {
                                var image = AudioFile.ExtractImageForAudioFile(audioFile.FilePath);
                                if (image == null)
                                    return null;

                                image = ImageManipulation.ResizeImage(image, albumWidth, albumHeight);
                                return image;

                                //var imageSource = ImageHelper.ImageToImageSource(image);
                                //Console.WriteLine("imageSource is {0}.{1}", imageSource.Width, imageSource.Height);
                                //double lowestValue = imageSource.Width < imageSource.Height ? imageSource.Width : imageSource.Height;
                                //var imageCropped = new CroppedBitmap(new WriteableBitmap(imageSource), new Int32Rect(0, 0, (int)lowestValue, (int)lowestValue));
                                //imageAlbum.Source = ImageHelper.ImageToImageSource(image);                                
                                //return imageCropped;
                                //return imageSource;
                            }
                            catch (Exception ex)
                            {
                                // Ignore error and return null
                                Console.WriteLine("An error occured while extracing album art in {0}: {1}", audioFile.FilePath, ex);
                            }

                            return null;
                        });
                        //var croppedBitmap = task.Result;
                        //if (croppedBitmap != null)
                        //    imageAlbum.Source = croppedBitmap;
                        var imageResult = task.Result;
                        if (imageResult != null)
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                imageAlbum.Source = ImageHelper.ImageToImageSource(imageResult);
                            }));
                        }
                    }
            }));
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

        public void RefreshActiveMarker(Guid markerId)
        {
        }

        public void RefreshMarkerPosition(Marker marker)
        {
        }

        public void RefreshLoops(IEnumerable<Loop> loops)
        {
        }

        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                lblVolume.Content = entity.VolumeString;
                if (faderVolume.Value != (int)entity.Volume)
                    faderVolume.ValueWithoutEvent = (int) entity.Volume;
            }));
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
        }

        #endregion

        #region IMarkersView implementation

        public Action OnAddMarker { get; set; }
        public Action<MarkerTemplateNameType> OnAddMarkerWithTemplate { get; set; }
        public Action<Marker> OnEditMarker { get; set; }
        public Action<Marker> OnSelectMarker { get; set; }
        public Action<Marker> OnDeleteMarker { get; set; }
        public Action<Marker> OnUpdateMarker { get; set; }
        public Action<Guid> OnPunchInMarker { get; set; }
        public Action<Guid> OnUndoMarker { get; set; }
        public Action<Guid> OnSetActiveMarker { get; set; }
        public Action<Guid, string> OnChangeMarkerName { get; set; }
        public Action<Guid, float> OnChangeMarkerPosition { get; set; }
        public Action<Guid, float> OnSetMarkerPosition { get; set; }

        public void MarkerError(Exception ex)
        {
            ShowErrorDialog(ex);
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

        public void RefreshMarkerPosition(Marker marker, int newIndex)
        {
        }

        #endregion

        #region ILoopsView implementation

        public Action OnAddLoop { get; set; }
        public Action<Loop> OnEditLoop { get; set; }

        public void LoopError(Exception ex)
        {
            ShowErrorDialog(ex);
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
            ShowErrorDialog(ex);
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
            ShowErrorDialog(ex);
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
