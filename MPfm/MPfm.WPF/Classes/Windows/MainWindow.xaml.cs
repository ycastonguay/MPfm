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
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MPfm.Core.Helpers;
using MPfm.Library.Objects;
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
        private List<Marker> _markers;
        private bool _isPlayerPositionChanging;
        private int _selectedMarkerIndex = -1;

        public MainWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            Initialize();
            SetLegacyControlTheme();
            ViewIsReady();
        }

        private void Initialize()
        {
            panelUpdateLibrary.Visibility = Visibility.Collapsed;
            gridViewSongs.DoubleClick += GridViewSongsOnDoubleClick;
            EnableMarkerButtons(false);
            EnableLoopButtons(false);
            RefreshSongInformation(null, 0, 0, 0);
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
            //waveFormDisplay.Theme.BackgroundGradient = new BackgroundGradient(System.Drawing.Color.FromArgb(255, 36, 47, 53), System.Drawing.Color.FromArgb(255, 36, 47, 53), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0);            
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
                    ShowUpdateLibraryPanel(true, () => OnAddFilesToLibrary(dialog.FileNames.ToList()));                    
                }
            }
            else if (sender == miFile_AddFolder)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "Please select a folder to add to the music library";
                dialog.ShowNewFolderButton = false;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ShowUpdateLibraryPanel(true, () => OnAddFolderToLibrary(dialog.SelectedPath));                    
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
                    ShowUpdateLibraryPanel(true, null);
                }
            }
            else if (sender == miFile_UpdateLibrary)
            {
                ShowUpdateLibraryPanel(true, () => OnStartUpdateLibrary());                
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

        private void ShowUpdateLibraryPanel(bool show, Action onAnimationCompleted)
        {
            // Reset values
            if (show)
            {
                lblUpdateLibraryTitle.Text = "Loading...";                
                progressBarUpdateLibrary.Value = 0;
            }

            //panelUpdateLibrary.Opacity = 0;
            panelUpdateLibrary.Height = show ? 0 : 1;
            panelUpdateLibrary.Visibility = Visibility.Visible;

            //var animOpacity = new DoubleAnimation();
            //animOpacity.From = 0.0;
            //animOpacity.To = 1.0;
            //animOpacity.Duration = TimeSpan.FromMilliseconds(200);

            var animHeight = new DoubleAnimation();
            animHeight.From = show ? 0.0 : 80.0;
            animHeight.To = show ? 80.0 : 0.0;
            animHeight.Duration = TimeSpan.FromMilliseconds(200);

            var storyboard = new Storyboard();
            //storyboard.Children.Add(animOpacity);                    
            //Storyboard.SetTarget(animOpacity, panelUpdateLibrary);
            //Storyboard.SetTargetProperty(animOpacity, new PropertyPath(OpacityProperty));
            storyboard.Completed += (sender, args) =>
            {
                if(onAnimationCompleted != null)
                    onAnimationCompleted();
            };
            storyboard.Children.Add(animHeight);
            Storyboard.SetTarget(animHeight, panelUpdateLibrary);
            Storyboard.SetTargetProperty(animHeight, new PropertyPath(HeightProperty));
            storyboard.Begin();
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

        private void TrackPosition_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isPlayerPositionChanging = true;
        }

        private void TrackPosition_OnMouseUp(object sender, MouseButtonEventArgs e)
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

        private void BtnUseThisTempo_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnIncrementTime_OnClick(object sender, RoutedEventArgs e)
        {
            OnIncrementTempo();
        }

        private void BtnDecrementTime_OnClick(object sender, RoutedEventArgs e)
        {
            OnDecrementTempo();
        }

        private void BtnResetTime_OnClick(object sender, RoutedEventArgs e)
        {
            OnResetTimeShifting();
        }

        private void BtnChangeKey_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnIncrementPitch_OnClick(object sender, RoutedEventArgs e)
        {
            OnIncrementInterval();
        }

        private void BtnDecrementPitch_OnClick(object sender, RoutedEventArgs e)
        {
            OnDecrementInterval();
        }

        private void BtnResetPitch_OnClick(object sender, RoutedEventArgs e)
        {
            OnResetInterval();
        }

        private void BtnEditSongMetadata_OnClick(object sender, RoutedEventArgs e)
        {
            OnEditSongMetadata();
        }

        private void BtnSearchGuitarTabs_OnClick(object sender, RoutedEventArgs e)
        {
            SearchWebForSong("guitar+tab");
        }

        private void BtnSearchBassTabs_OnClick(object sender, RoutedEventArgs e)
        {
            SearchWebForSong("bass+tab");
        }

        private void BtnSearchLyrics_OnClick(object sender, RoutedEventArgs e)
        {
            SearchWebForSong("lyrics");
        }

        private void SearchWebForSong(string querySuffix)
        {
            try
            {
                if (!string.IsNullOrEmpty(lblArtistName.Content.ToString()))
                    Process.Start(string.Format("http://www.google.ca/search?q={0}+{1}+{2}", HttpUtility.UrlEncode(lblArtistName.Content.ToString()), HttpUtility.UrlEncode(lblSongTitle.Content.ToString()), querySuffix));
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex);
            }
        }

        private void SliderTimeShifting_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            if(OnSetTimeShifting != null)
                OnSetTimeShifting((float)sliderTimeShifting.Value);
        }

        private void SliderPitchShifting_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            if (OnSetInterval != null)
                OnSetInterval((int)sliderPitchShifting.Value);
        }


        private void BtnGoToMarker_OnClick(object sender, RoutedEventArgs e)
        {
            var marker = listViewMarkers.SelectedItem as Marker;
            OnSelectMarker(marker);
        }

        private void BtnAddMarker_OnClick(object sender, RoutedEventArgs e)
        {            
            contextMenuAddMarker.Placement = PlacementMode.Bottom;
            contextMenuAddMarker.PlacementTarget = btnAddMarker;
            contextMenuAddMarker.Visibility = Visibility.Visible;
            contextMenuAddMarker.IsOpen = true;
            e.Handled = true;
        }

        private void BtnAddMarker_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            contextMenuAddMarker.Visibility = Visibility.Collapsed;
        }

        private void MenuItemVerse_OnClick(object sender, RoutedEventArgs e)
        {
            AddMarker(MarkerTemplateNameType.Verse);
        }

        private void MenuItemChorus_OnClick(object sender, RoutedEventArgs e)
        {
            AddMarker(MarkerTemplateNameType.Chorus);
        }

        private void MenuItemBridge_OnClick(object sender, RoutedEventArgs e)
        {
            AddMarker(MarkerTemplateNameType.Bridge);
        }

        private void MenuItemSolo_OnClick(object sender, RoutedEventArgs e)
        {
            AddMarker(MarkerTemplateNameType.Solo);
        }

        private void AddMarker(MarkerTemplateNameType template)
        {
            OnAddMarkerWithTemplate(template);
        }

        private void BtnEditMarker_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnRemoveMarker_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to remove this marker?", "Remove marker", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
                OnDeleteMarker(_markers[listViewMarkers.SelectedIndex]);
        }

        private void BtnPlayLoop_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnAddLoop_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnEditLoop_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnRemoveLoop_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void ListViewMarkers_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                var marker = e.RemovedItems[0] as Marker;
                int index = _markers.FindIndex(x => x.MarkerId == marker.MarkerId);
                ChangeMarkerCellPanelVisibility(index, false);    
            }

            EnableMarkerButtons(listViewMarkers.SelectedIndex >= 0);

            if (listViewMarkers.SelectedIndex >= 0)
            {
                _selectedMarkerIndex = listViewMarkers.SelectedIndex;
                ChangeMarkerCellPanelVisibility(listViewMarkers.SelectedIndex, true);
            }
        }

        private void ChangeMarkerCellPanelVisibility(int cellIndex, bool selected)
        {
            try
            {
                var item = listViewMarkers.ItemContainerGenerator.ContainerFromIndex(cellIndex) as ListViewItem;
                var panelMarkerCollapsed = UIHelper.FindByName("panelMarkerCollapsed", item) as Grid;
                panelMarkerCollapsed.Visibility = selected ? Visibility.Collapsed : Visibility.Visible;
                var panelMarkerExtended = UIHelper.FindByName("panelMarkerExtended", item) as Grid;
                panelMarkerExtended.Visibility = selected ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to change marker cell panel visiblity: {0}", ex);
            }
        }

        private void ListViewMarkers_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;
            var gridView = listView.View as GridView;
            gridView.Columns[0].Width = listView.ActualWidth;
        }

        private void HandleMarkerItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var marker = ((ListViewItem)sender).Content as Marker;
            OnSelectMarker(marker);
        }

        private void EnableMarkerButtons(bool enabled)
        {
            //btnGoToMarker.Enabled = enabled;
            //btnEditMarker.Enabled = enabled;
            //btnRemoveMarker.Enabled = enabled;
        }

        private void EnableLoopButtons(bool enabled)
        {
            btnPlayLoop.Enabled = enabled;
            btnEditLoop.Enabled = enabled;
            btnRemoveLoop.Enabled = enabled;
        }

        private void SliderMarkerPosition_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Console.WriteLine("index: {0} value: {1} id: {2}", listViewMarkers.SelectedIndex, e.NewValue, _markers[listViewMarkers.SelectedIndex].MarkerId);
            OnChangeMarkerPosition(_markers[_selectedMarkerIndex].MarkerId, (float)e.NewValue);
        }

        private void SliderMarkerPosition_OnLostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            OnUpdateMarker(_markers[_selectedMarkerIndex]);
        }

        private void TxtMarkerName_OnLostFocus(object sender, RoutedEventArgs e)
        {
            OnUpdateMarker(_markers[_selectedMarkerIndex]);
        }

        #region IMainView implementation

        public Action OnOpenPreferencesWindow { get; set; }
        public Action OnOpenEffectsWindow { get; set; }
        public Action OnOpenPlaylistWindow { get; set; }
        public Action OnOpenSyncWindow { get; set; }
        public Action OnOpenSyncCloudWindow { get; set; }
        public Action OnOpenSyncWebBrowserWindow { get; set; }
        public Action OnOpenResumePlayback { get; set; }

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

        public bool IsOutputMeterEnabled { get { return true; } }
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

                //if (!waveFormDisplay.IsLoading)
                //    waveFormDisplay.SetPosition(entity.PositionBytes, entity.Position);
            }));
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            _selectedMarkerIndex = -1;
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
                        lblSampleRate.Content = string.Empty;
                        lblBitrate.Content = string.Empty;
                        lblBitsPerSample.Content = string.Empty;
                        lblSoundFormat.Content = string.Empty;
                        lblYear.Content = string.Empty;
                        lblMonoStereo.Content = string.Empty;
                        lblFileSize.Content = string.Empty;
                        lblGenre.Content = string.Empty;
                        lblPlayCount.Content = string.Empty;
                        lblLastPlayed.Content = string.Empty;
                    }
                    else
                    {
                        lblArtistName.Content = audioFile.ArtistName;
                        lblAlbumTitle.Content = audioFile.AlbumTitle;
                        lblSongTitle.Content = audioFile.Title;
                        lblFilePath.Content = audioFile.FilePath;
                        lblLength.Content = audioFile.Length;
                        lblSampleRate.Content = string.Format("{0} Hz", audioFile.SampleRate);
                        lblBitrate.Content = string.Format("{0} kbps", audioFile.Bitrate);
                        lblBitsPerSample.Content = string.Format("{0} bits", audioFile.BitsPerSample);
                        lblSoundFormat.Content = audioFile.FileType.ToString();
                        lblYear.Content = audioFile.Year.ToString();
                        lblMonoStereo.Content = audioFile.AudioChannels == 1 ? "Mono" : "Stereo";
                        lblFileSize.Content = string.Format("{0} bytes", audioFile.FileSize);
                        lblGenre.Content = string.Format("{0}", audioFile.Genre);
                        lblPlayCount.Content = string.Format("{0} times played", audioFile.PlayCount);
                        lblLastPlayed.Content = audioFile.LastPlayed.HasValue ? string.Format("Last played on {0}", audioFile.LastPlayed.Value.ToShortDateString()) : "";

                //        miTrayArtistName.Text = audioFile.ArtistName;
                //        miTrayAlbumTitle.Text = audioFile.AlbumTitle;
                //        miTraySongTitle.Text = audioFile.Title;

                        gridViewSongs.NowPlayingAudioFileId = audioFile.Id;
                        gridViewSongs.Refresh();

                        //// Configure wave form
                        //waveFormDisplay.Length = lengthBytes;
                        //waveFormDisplay.LoadWaveForm(audioFile.FilePath);

                        scrollViewWaveForm.SetWaveFormLength(lengthBytes);
                        scrollViewWaveForm.LoadPeakFile(audioFile);

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
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                outputMeter.AddWaveDataBlock(dataLeft, dataRight);
                outputMeter.InvalidateVisual();
            }));
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
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                _markers = markers.ToList();
                listViewMarkers.Items.Clear();
                foreach (var marker in markers)
                    listViewMarkers.Items.Add(marker);                
                listViewMarkers.SelectedIndex = _selectedMarkerIndex;

                // Does not open because it cannot find the ListViewItem yet... 
                // Not needed, setting the selectedindex triggers the event already
                //ChangeMarkerCellPanelVisibility(_selectedMarkerIndex, true);
            }));
        }

        public void RefreshMarkerPosition(Marker marker, int newIndex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                //Console.WriteLine("index: {0} value: {1} newIndex: {2}", listViewMarkers.SelectedIndex, marker.Position, newIndex);
                _markers[_selectedMarkerIndex].Position = marker.Position;
                _markers[_selectedMarkerIndex].PositionBytes = marker.PositionBytes;
                _markers[_selectedMarkerIndex].PositionPercentage = marker.PositionPercentage;
                _markers[_selectedMarkerIndex].PositionSamples = marker.PositionSamples;

                var item = listViewMarkers.ItemContainerGenerator.ContainerFromIndex(_selectedMarkerIndex) as ListViewItem;
                var lblMarkerPosition = UIHelper.FindByName("lblMarkerPosition", item) as TextBlock;
                lblMarkerPosition.Text = marker.Position;
            }));
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
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                lblDetectedTempo.Content = entity.DetectedTempo;
                lblReferenceTempo.Content = entity.ReferenceTempo;
                lblCurrentTempo.Content = entity.CurrentTempo;
                //sliderTimeShifting.SetValueWithoutTriggeringEvent((int)entity.TimeShiftingValue);
                sliderTimeShifting.Value = (int) entity.TimeShiftingValue;
            }));
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
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                lblInterval.Content = entity.Interval;
                lblCurrentKey.Content = entity.NewKey.Item2;
                lblReferenceKey.Content = entity.ReferenceKey.Item2;
                //    trackPitch.SetValueWithoutTriggeringEvent(entity.IntervalValue);
                sliderPitchShifting.Value = (int)entity.IntervalValue;
            }));
            //MethodInvoker methodUIUpdate = delegate
            //{
            //};

            //if (InvokeRequired)
            //    BeginInvoke(methodUIUpdate);
            //else
            //    methodUIUpdate.Invoke();
        }

        #endregion

        #region IUpdateLibraryView implementation

        public Action<List<string>> OnAddFilesToLibrary { get; set; }
        public Action<string> OnAddFolderToLibrary { get; set; }
        public Action OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }
        public Action<string> OnSaveLog { get; set; }

        public void RefreshStatus(UpdateLibraryEntity entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblUpdateLibraryTitle.Text = entity.Title;
                progressBarUpdateLibrary.Value = entity.PercentageDone * 100;
            }));
        }

        public void AddToLog(string entry)
        {
        }

        public void ProcessStarted()
        {
        }

        public void ProcessEnded(bool canceled)
        {
            // Show finish state
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblUpdateLibraryTitle.Text = "Update library successful.";
                progressBarUpdateLibrary.Value = 100;
            }));

            // Delay before closing update library panel
            var task = TaskHelper.DelayTask(1500);
            task.ContinueWith((a) =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    ShowUpdateLibraryPanel(false, () => panelUpdateLibrary.Visibility = Visibility.Collapsed);
                }));
            });
        }

        #endregion

    }
}
