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
using System.IO;
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
        private List<Marker> _segmentMarkers;
        private List<Loop> _loops;
        private Marker _currentMarker;
        private Loop _currentLoop;
        private Segment _currentSegment;
        private bool _isPlayerPositionChanging;
        private bool _isScrollViewWaveFormChangingSecondaryPosition;
        private int _selectedMarkerIndex = -1;
        private int _selectedLoopIndex = -1;
        private int _selectedSegmentIndex = -1;
        private AudioFile _currentAudioFile;
        private string _currentAlbumArtKey;

        public MainWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            Initialize();
            ViewIsReady();
        }

        private void Initialize()
        {
            panelUpdateLibrary.Visibility = Visibility.Collapsed;
            gridViewSongsNew.DoubleClick += GridViewSongsNewOnDoubleClick;
            gridViewSongsNew.MenuItemClicked += GridViewSongsNewOnMenuItemClicked;
            scrollViewWaveForm.OnChangePosition += ScrollViewWaveForm_OnChangePosition;
            scrollViewWaveForm.OnChangeSecondaryPosition += ScrollViewWaveForm_OnChangeSecondaryPosition;

            comboSoundFormat.Items.Add(AudioFileFormat.All);
            comboSoundFormat.Items.Add(AudioFileFormat.APE);
            comboSoundFormat.Items.Add(AudioFileFormat.FLAC);
            comboSoundFormat.Items.Add(AudioFileFormat.MP3);
            comboSoundFormat.Items.Add(AudioFileFormat.MPC);
            comboSoundFormat.Items.Add(AudioFileFormat.OGG);
            comboSoundFormat.Items.Add(AudioFileFormat.WMA);
            comboSoundFormat.Items.Add(AudioFileFormat.WV);
            comboSoundFormat.SelectedIndex = 0;

            EnableMarkerButtons(false);
            EnableLoopButtons(false);
            EnableSegmentButtons(false);
            RefreshSongInformation(null, 0, 0, 0);
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
            else if (sender == miFile_Exit)
            {
                Application.Current.Shutdown();
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
            else if (sender == miHelp_About)
            {
                OnOpenAboutWindow();
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

        private void GridViewSongsNewOnDoubleClick(object sender, EventArgs eventArgs)
        {
            if (gridViewSongsNew.SelectedItems.Count == 0)
                return;

            OnTableRowDoubleClicked(gridViewSongsNew.SelectedItems[0].AudioFile);
        }

        private void GridViewSongsNewOnMenuItemClicked(SongGridView.MenuItemType menuItemType)
        {
            if (gridViewSongsNew.SelectedItems.Count == 0)
                return;

            switch (menuItemType)
            {
                case SongGridView.MenuItemType.PlaySongs:
                    AudioFile audioFile = gridViewSongsNew.SelectedItems[0].AudioFile;
                    OnTableRowDoubleClicked(audioFile);
                    break;
                case SongGridView.MenuItemType.AddToPlaylist:
                    var audioFiles = new List<AudioFile>();
                    foreach (var item in gridViewSongsNew.SelectedItems)
                        audioFiles.Add(item.AudioFile);
                    OnSongBrowserAddToPlaylist(audioFiles);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
            scrollViewWaveForm.ShowSecondaryPosition(true);
        }

        private void TrackPosition_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isPlayerPositionChanging = false;
            OnPlayerSetPosition((float) trackPosition.Value / 10f);
            scrollViewWaveForm.ShowSecondaryPosition(false);
        }

        private void TrackPosition_OnTrackBarValueChanged()
        {
            if (OnPlayerRequestPosition == null || !_isPlayerPositionChanging || _isScrollViewWaveFormChangingSecondaryPosition)
                return;

            var position = OnPlayerRequestPosition((float) trackPosition.Value/1000f);
            lblPosition.Content = position.Position;
            scrollViewWaveForm.SetSecondaryPosition(position.PositionBytes);
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
            var res = Application.Current.Resources;
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

        private void TrackTimeShifting_OnTrackBarValueChanged()
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            if (OnSetTimeShifting != null)
                OnSetTimeShifting(trackTimeShifting.Value);
        }

        private void TrackPitchShifting_OnTrackBarValueChanged()
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            if (OnSetInterval != null)
                OnSetInterval((int)trackPitchShifting.Value);
        }

        private void TrackMarkerPosition_OnTrackBarValueChanged()
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            if (OnChangePositionMarkerDetails != null)
                OnChangePositionMarkerDetails((float)trackMarkerPosition.Value / 1000f);
        }

        private void BtnGoToMarker_OnClick(object sender, RoutedEventArgs e)
        {
            var marker = listViewMarkers.SelectedItem as Marker;
            OnSelectMarker(marker);
        }

        private void BtnBackMarker_OnClick(object sender, RoutedEventArgs e)
        {
            gridMarkers.Visibility = Visibility.Visible;
            gridMarkerDetails.Visibility = Visibility.Hidden;

            _currentMarker.Name = txtMarkerName.Text;
            OnUpdateMarkerDetails(_currentMarker);

            _currentMarker = null;
            scrollViewWaveForm.SetActiveMarker(Guid.Empty);
        }

        private void BtnPunchInMarker_OnClick(object sender, RoutedEventArgs e)
        {
            OnPunchInMarkerDetails();
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

        private void ListViewMarkers_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableMarkerButtons(listViewMarkers.SelectedIndex >= 0);

            if (listViewMarkers.SelectedIndex >= 0)
                _selectedMarkerIndex = listViewMarkers.SelectedIndex;
        }

        private void ListViewMarkers_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;
            var gridView = listView.View as GridView;
            gridView.Columns[0].Width = listView.ActualWidth;
        }

        private void ListViewMarkers_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIHelper.ListView_PreviewMouseDown_RemoveSelectionIfNotClickingOnAListViewItem(listViewMarkers, e);
        }

        private void ListViewMarkers_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listViewMarkers.SelectedIndex < 0 || listViewMarkers.SelectedIndex >= _loops.Count)
                return;

            OnSelectMarker(_markers[listViewMarkers.SelectedIndex]);
        }

        private void EnableMarkerButtons(bool enabled)
        {
            btnGoToMarker.Enabled = enabled;
            btnEditMarker.Enabled = enabled;
            btnRemoveMarker.Enabled = enabled;
        }

        private void AddMarker(MarkerTemplateNameType template)
        {
            OnAddMarkerWithTemplate(template);
        }

        private void BtnEditMarker_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewMarkers.SelectedIndex < 0)
                return;

            OnEditMarker(_markers[listViewMarkers.SelectedIndex]);

            gridMarkers.Visibility = Visibility.Hidden;
            gridMarkerDetails.Visibility = Visibility.Visible;
        }

        private void BtnRemoveMarker_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewMarkers.SelectedIndex < 0 || listViewMarkers.SelectedIndex >= _loops.Count)
                return;

            var result = MessageBox.Show("Are you sure you wish to remove this marker?", "Remove marker", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                OnDeleteMarker(_markers[listViewMarkers.SelectedIndex]);
                EnableMarkerButtons(false);
            }
        }

        private void ListViewLoops_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableLoopButtons(listViewLoops.SelectedIndex >= 0);

            if (listViewLoops.SelectedIndex >= 0)
                _selectedLoopIndex = listViewLoops.SelectedIndex;
        }

        private void ListViewLoops_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditLoop();
        }

        private void ListViewLoops_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIHelper.ListView_PreviewMouseDown_RemoveSelectionIfNotClickingOnAListViewItem(listViewLoops, e);
        }

        private void BtnBackLoopDetails_OnClick(object sender, RoutedEventArgs e)
        {
            gridLoops.Visibility = Visibility.Visible;
            gridLoopDetails.Visibility = Visibility.Hidden;
            gridLoopPlayback.Visibility = Visibility.Hidden;
            gridSegmentDetails.Visibility = Visibility.Hidden;

            _currentLoop.Name = txtLoopName.Text;
            OnUpdateLoopDetails(_currentLoop);
            _currentLoop = null;
            scrollViewWaveForm.SetLoop(null);
        }

        private void BtnPlayLoop_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnAddLoop_OnClick(object sender, RoutedEventArgs e)
        {
            OnAddLoop();
            gridLoops.Visibility = Visibility.Hidden;
            gridLoopDetails.Visibility = Visibility.Visible;
            gridLoopPlayback.Visibility = Visibility.Hidden;
            gridSegmentDetails.Visibility = Visibility.Hidden;
        }

        private void BtnEditLoop_OnClick(object sender, RoutedEventArgs e)
        {
            EditLoop();
        }

        private void EditLoop()
        {
            if (listViewLoops.SelectedIndex < 0 || listViewLoops.SelectedIndex >= _loops.Count)
                return;

            OnEditLoop(_loops[listViewLoops.SelectedIndex]);
            gridLoops.Visibility = Visibility.Hidden;
            gridLoopDetails.Visibility = Visibility.Visible;
            gridLoopPlayback.Visibility = Visibility.Hidden;
            gridSegmentDetails.Visibility = Visibility.Hidden;
        }

        private void BtnRemoveLoop_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewLoops.SelectedIndex < 0 || listViewLoops.SelectedIndex >= _loops.Count)
                return;

            var result = MessageBox.Show("Are you sure you wish to remove this loop?", "Remove loop", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                OnDeleteMarker(_markers[listViewMarkers.SelectedIndex]);
                EnableLoopButtons(false);
            }
        }

        private void EnableLoopButtons(bool enabled)
        {
            btnPlayLoop.Enabled = enabled;
            btnEditLoop.Enabled = enabled;
            btnRemoveLoop.Enabled = enabled;
        }

        private void ListViewSegments_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableSegmentButtons(listViewLoops.SelectedIndex >= 0);

            if (listViewLoops.SelectedIndex >= 0)
                _selectedLoopIndex = listViewLoops.SelectedIndex;
        }

        private void ListViewSegments_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSegment();
        }

        private void ListViewSegments_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIHelper.ListView_PreviewMouseDown_RemoveSelectionIfNotClickingOnAListViewItem(listViewSegments, e);
        }

        private void BtnAddSegment_OnClick(object sender, RoutedEventArgs e)
        {
            OnAddSegment();
            gridLoops.Visibility = Visibility.Hidden;
            gridLoopDetails.Visibility = Visibility.Hidden;
            gridLoopPlayback.Visibility = Visibility.Hidden;
            gridSegmentDetails.Visibility = Visibility.Visible;
        }

        private void BtnEditSegment_OnClick(object sender, RoutedEventArgs e)
        {
            EditSegment();
        }

        private void EditSegment()
        {
            if (listViewSegments.SelectedIndex < 0 || listViewSegments.SelectedIndex >= _currentLoop.Segments.Count)
                return;

            OnEditSegment(_currentLoop.Segments[listViewSegments.SelectedIndex]);
            gridLoops.Visibility = Visibility.Hidden;
            gridLoopDetails.Visibility = Visibility.Hidden;
            gridLoopPlayback.Visibility = Visibility.Hidden;
            gridSegmentDetails.Visibility = Visibility.Visible;
        }

        private void BtnRemoveSegment_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewSegments.SelectedIndex < 0 || listViewSegments.SelectedIndex >= _currentLoop.Segments.Count)
                return;

            var result = MessageBox.Show("Are you sure you wish to remove this segment?", "Remove segment", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                OnDeleteSegment(_currentLoop.Segments[listViewSegments.SelectedIndex]);
                EnableSegmentButtons(false);
            }
        }

        private void EnableSegmentButtons(bool enabled)
        {
            btnEditSegment.Enabled = enabled;
            btnRemoveSegment.Enabled = enabled;
        }

        private void BtnBackSegmentDetails_OnClick(object sender, RoutedEventArgs e)
        {
            gridLoops.Visibility = Visibility.Hidden;
            gridLoopDetails.Visibility = Visibility.Visible;
            gridLoopPlayback.Visibility = Visibility.Hidden;
            gridSegmentDetails.Visibility = Visibility.Hidden;

            _currentSegment.MarkerId = Guid.Empty;
            if (chkSegmentLinkToMarker.IsChecked.Value && comboSegmentMarker.SelectedIndex >= 0)
                _currentSegment.MarkerId = _segmentMarkers[comboSegmentMarker.SelectedIndex].MarkerId;

            OnUpdateSegmentDetails(_currentSegment);
            _currentSegment = null;
        }

        private void ComboSegmentMarker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSegmentLinkedMarker();
        }

        private void ChkSegmentLinkToMarker_OnChecked(object sender, RoutedEventArgs e)
        {
            SetSegmentLinkedMarker();
        }

        private void SetSegmentLinkedMarker()
        {
            if (comboSegmentMarker.SelectedIndex == -1)
                return;

            if (_segmentMarkers.Count == 0)
            {
                chkSegmentLinkToMarker.IsChecked = false;
                MessageBox.Show("There are no markers to link to this segment.", "Cannot link to marker", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            comboSegmentMarker.Visibility = chkSegmentLinkToMarker.IsChecked.Value ? Visibility.Visible : Visibility.Hidden; //.Hidden = !chkSegmentLinkToMarker.Value;
            if (chkSegmentLinkToMarker.IsChecked.Value)
            {
                var marker = _segmentMarkers[comboSegmentMarker.SelectedIndex];
                OnLinkToMarkerSegmentDetails(marker.MarkerId);
            }
            else
            {
                OnLinkToMarkerSegmentDetails(Guid.Empty);
            }
        }

        private void BtnPunchInSegment_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentSegment == null)
                return;

            chkSegmentLinkToMarker.IsChecked = false;
            comboSegmentMarker.Visibility = Visibility.Hidden;
            OnPunchInPositionSegmentDetails();
        }

        private void TrackSegmentPosition_OnOnTrackBarValueChanged()
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            chkSegmentLinkToMarker.IsChecked = false;
            comboSegmentMarker.Visibility = Visibility.Hidden;
            if (OnChangePositionSegmentDetails != null)
                OnChangePositionSegmentDetails((float)trackSegmentPosition.Value / 1000f);
        }

        private void BtnBackLoopPlayback_OnClick(object sender, RoutedEventArgs e)
        {
            gridLoops.Visibility = Visibility.Visible;
            gridLoopDetails.Visibility = Visibility.Hidden;
            gridLoopPlayback.Visibility = Visibility.Hidden;
            gridSegmentDetails.Visibility = Visibility.Hidden;
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

        private void StartPlaybackOfSelectedLibraryBrowserTreeViewItem()
        {
            var value = (MPfmTreeViewItem)treeViewLibrary.SelectedValue;
            var entity = value.Entity;
            if (entity != null)
                OnTreeNodeDoubleClicked(entity);            
        }

        private void TreeViewLibrary_OnTreeViewItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            StartPlaybackOfSelectedLibraryBrowserTreeViewItem();
        }

        private void MenuItemLibraryBrowserPlay_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            StartPlaybackOfSelectedLibraryBrowserTreeViewItem();
        }

        private void MenuItemLibraryBrowserAddToPlaylist_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var value = (MPfmTreeViewItem)treeViewLibrary.SelectedValue;
            var entity = value.Entity;
            if (entity != null)
                OnAddToPlaylist(entity);
        }

        private void MenuItemLibraryBrowserRemoveFromLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (MessageBox.Show("Are you sure you wish to remove these audio files from your library?\nThis does not delete the audio files from your hard disk.", "Audio files will be removed from library", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            var value = (MPfmTreeViewItem)treeViewLibrary.SelectedValue;
            var entity = value.Entity;
            if (entity != null)
                OnRemoveFromLibrary(entity);
        }

        private void MenuItemLibraryBrowserDeleteFromHardDisk_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (MessageBox.Show("Are you sure you wish to delete these audio files from your hard disk?\nWARNING: This operation CANNOT be undone!", "Audio files will be deleted from hard disk", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            var value = (MPfmTreeViewItem)treeViewLibrary.SelectedValue;
            var entity = value.Entity;
            if (entity != null)
                OnDeleteFromHardDisk(entity);
        }

        private void TreeViewLibrary_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Select the item if it isn't selected yet
            var item = UIHelper.VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }

        private void ComboSoundFormat_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = (AudioFileFormat)comboSoundFormat.SelectedValue;     
            if(OnAudioFileFormatFilterChanged != null)
                OnAudioFileFormatFilterChanged(value);
        }

        private void ScrollViewWaveForm_OnChangePosition(float position)
        {
            //Console.WriteLine("MainWindow - ScrollViewWaveForm_OnChangePosition - position: {0}", position);
            _isScrollViewWaveFormChangingSecondaryPosition = false;
            OnPlayerSetPosition(position*100);
        }

        private void ScrollViewWaveForm_OnChangeSecondaryPosition(float position)
        {
            //Console.WriteLine("MainWindow - ScrollViewWaveForm_OnChangeSecondaryPosition - position: {0}", position);
            _isScrollViewWaveFormChangingSecondaryPosition = true;
            var requestedPosition = OnPlayerRequestPosition(position);
            trackPosition.Value = (int)(position * 1000);
            lblPosition.Content = requestedPosition.Position;
        }

        private void ScrollViewWaveForm_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentAudioFile == null)
                return;

            //contextMenuWaveForm.Placement = PlacementMode.MousePoint;
            //contextMenuWaveForm.PlacementTarget = scrollViewWaveForm;
            //contextMenuWaveForm.Visibility = Visibility.Visible;
            //contextMenuWaveForm.IsOpen = true;
            e.Handled = true;
        }

        private void MenuItemZoomIn_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItemZoomOut_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItemCreateMarker_OnClick(object sender, RoutedEventArgs e)
        {
        }

        #region IMainView implementation

        public Action OnOpenAboutWindow { get; set; }
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
        public Action<LibraryBrowserEntity> OnAddToPlaylist { get; set; }
        public Action<LibraryBrowserEntity> OnRemoveFromLibrary { get; set; }
        public Action<LibraryBrowserEntity> OnDeleteFromHardDisk { get; set; }

        public void LibraryBrowserError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

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
                    item.Entity = entity;
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
                    subitem.Entity = subentity;
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

        public void RefreshLibraryBrowserSelectedNode(LibraryBrowserEntity entity)
        {
        }

        public void NotifyLibraryBrowserNewNode(int position, LibraryBrowserEntity entity)
        {
        }

        public void NotifyLibraryBrowserRemovedNode(int position)
        {
        }

        #endregion

        #region ISongBrowserView implementation

        public Action<AudioFile> OnTableRowDoubleClicked { get; set; }
        public Action<AudioFile> OnSongBrowserEditSongMetadata { get; set; }
        public Action<IEnumerable<AudioFile>> OnSongBrowserAddToPlaylist { get; set; }
        public Action<string> OnSearchTerms { get; set; }

        public void SongBrowserError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                //    //string orderBy = viewSongs2.OrderByFieldName;
                //    //bool orderByAscending = viewSongs2.OrderByAscending;
                gridViewSongsNew.ImportAudioFiles(audioFiles.ToList());
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
                trackPosition.IsEnabled = status == PlayerStatusType.Playing;
                if (status == PlayerStatusType.Playing)
                    imagePlayPause.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Toolbar/pause.png"));
                else
                    imagePlayPause.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Toolbar/play.png"));
            }));
        }

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            if (_isPlayerPositionChanging || _isScrollViewWaveFormChangingSecondaryPosition)
                return;

            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                lblPosition.Content = entity.Position;
                trackPosition.Value = (int)(entity.PositionPercentage * 10);
                scrollViewWaveForm.SetPosition(entity.PositionBytes);
                //Console.WriteLine("Player position: {0} {1} slider: {2} min: {3} max: {4}", entity.Position, entity.PositionPercentage, entity.PositionBytes, trackPosition.Minimum, trackPosition.Maximum);
            }));
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            _selectedMarkerIndex = -1;
            _currentAudioFile = audioFile;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
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
                    lblYear.Content = audioFile.Year == 0 ? "No year specified" : audioFile.Year.ToString();
                    lblMonoStereo.Content = audioFile.AudioChannels == 1 ? "Mono" : "Stereo";
                    lblFileSize.Content = string.Format("{0} bytes", audioFile.FileSize);
                    lblGenre.Content = string.IsNullOrEmpty(audioFile.Genre) ? "No genre specified" : string.Format("{0}", audioFile.Genre);
                    lblPlayCount.Content = string.Format("{0} times played", audioFile.PlayCount);
                    lblLastPlayed.Content = audioFile.LastPlayed.HasValue ? string.Format("Last played on {0}", audioFile.LastPlayed.Value.ToShortDateString()) : "";

            //        miTrayArtistName.Text = audioFile.ArtistName;
            //        miTrayAlbumTitle.Text = audioFile.AlbumTitle;
            //        miTraySongTitle.Text = audioFile.Title;

                    gridViewSongsNew.NowPlayingAudioFileId = audioFile.Id;

                    scrollViewWaveForm.SetWaveFormLength(lengthBytes);
                    scrollViewWaveForm.LoadPeakFile(audioFile);

                    string key = audioFile.ArtistName.ToUpper() + "_" + audioFile.AlbumTitle.ToUpper();
                    if (_currentAlbumArtKey != key)
                    {
                        imageAlbum.Source = null;
                        var task = Task<BitmapImage>.Factory.StartNew(() =>
                        {
                            try
                            {
                                var bytes = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                                var stream = new MemoryStream(bytes);
                                stream.Seek(0, SeekOrigin.Begin);

                                var bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.StreamSource = stream;
                                bitmap.EndInit();
                                bitmap.Freeze();

                                return bitmap;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occured while extracing album art in {0}: {1}",
                                    audioFile.FilePath, ex);
                            }

                            return null;
                        });

                        var imageResult = task.Result;
                        if (imageResult != null)
                        {
                            _currentAlbumArtKey = key;
                            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                imageAlbum.Source = imageResult;
                            }));
                        }
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
                scrollViewWaveForm.SetMarkers(_markers);

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
        public Action<Loop> OnDeleteLoop { get; set; }
        public Action<Loop> OnPlayLoop { get; set; }

        public void LoopError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshLoops(List<Loop> loops)
        {
            _loops = loops.ToList();
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                listViewLoops.ItemsSource = _loops;
                listViewLoops.SelectedIndex = _selectedLoopIndex;
            }));
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
                trackTimeShifting.Value = (int) entity.TimeShiftingValue;
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
                trackPitchShifting.Value = (int)entity.IntervalValue;
            }));
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

        #region IMarkerDetailsView implementation

        public Action<float> OnChangePositionMarkerDetails { get; set; }
        public Action<Marker> OnUpdateMarkerDetails { get; set; }
        public Action OnDeleteMarkerDetails { get; set; }
        public Action OnPunchInMarkerDetails { get; set; }

        public void MarkerDetailsError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void DismissMarkerDetailsView()
        {
        }

        public void RefreshMarker(Marker marker, AudioFile audioFile)
        {
            _currentMarker = marker;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                txtMarkerName.Text = marker.Name;
                lblMarkerPosition.Content = marker.Position;
                trackMarkerPosition.ValueWithoutEvent = (int)(marker.PositionPercentage * 10);
                scrollViewWaveForm.SetActiveMarker(marker.MarkerId);
            }));
        }

        public void RefreshMarkerPosition(string position, float positionPercentage)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblMarkerPosition.Content = position;
                trackMarkerPosition.ValueWithoutEvent = (int)(positionPercentage * 10);
                _currentMarker.Position = position;
                _currentMarker.PositionPercentage = positionPercentage;
                scrollViewWaveForm.SetMarkerPosition(_currentMarker);
            }));
        }

        #endregion

        #region ILoopDetailsView implementation

        public Action OnAddSegment { get; set; }
        public Action<Guid, int> OnAddSegmentFromMarker { get; set; }
        public Action<Segment> OnEditSegment { get; set; }
        public Action<Segment> OnDeleteSegment { get; set; }
        public Action<Loop> OnUpdateLoopDetails { get; set; }
        public Action<Segment, int> OnChangeSegmentOrder { get; set; }
        public Action<Segment, Guid> OnLinkSegmentToMarker { get; set; }

        public void LoopDetailsError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshLoopDetails(Loop loop, AudioFile audioFile)
        {
            _currentLoop = loop;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                txtLoopName.Text = loop.Name;
                scrollViewWaveForm.SetLoop(loop);
                //scrollViewWaveForm.FocusZoomOnLoop(_currentLoop);

                listViewSegments.ItemsSource = _currentLoop.Segments;
                listViewSegments.SelectedIndex = _selectedSegmentIndex;
            }));
        }

        #endregion

        #region ILoopPlaybackView implementation

        public Action OnPreviousLoop { get; set; }
        public Action OnNextLoop { get; set; }
        public Action OnPreviousSegment { get; set; }
        public Action OnNextSegment { get; set; }

        public void LoopPlaybackError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshLoopPlayback(LoopPlaybackEntity entity)
        {
        }

        #endregion

        #region ISegmentDetailsView implementation

        public Action<float> OnChangeStartPositionSegmentDetails { get; set; }
        public Action<float> OnChangeEndPositionSegmentDetails { get; set; }
        public Action OnPunchInStartPositionSegmentDetails { get; set; }
        public Action OnPunchInEndPositionSegmentDetails { get; set; }
        public Action<float> OnChangePositionSegmentDetails { get; set; }
        public Action OnPunchInPositionSegmentDetails { get; set; }
        public Action<Segment> OnUpdateSegmentDetails { get; set; }
        public Action<Guid> OnLinkToMarkerSegmentDetails { get; set; }

        public void SegmentDetailsError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshSegmentDetails(Segment segment, long audioFileLength)
        {
            //Console.WriteLine("RefreshSegmentDetails - position: {0}", segment.Position);
            _currentSegment = segment;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                //waveFormScrollView.SetSegment(segment);

                chkSegmentLinkToMarker.IsChecked = segment.MarkerId != Guid.Empty;
                comboSegmentMarker.Visibility = segment.MarkerId == Guid.Empty ? Visibility.Hidden : Visibility.Visible;
                int index = _segmentMarkers.FindIndex(x => x.MarkerId == segment.MarkerId);
                if (index >= 0)
                    comboSegmentMarker.SelectedIndex = index;

                float positionPercentage = (float)segment.PositionBytes / (float)audioFileLength;
                trackSegmentPosition.ValueWithoutEvent = (int)(positionPercentage * 10);

                lblSegmentPosition.Content = segment.Position;
            }));
        }

        public void RefreshSegmentPosition(string position, float positionPercentage)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                lblSegmentPosition.Content = position;
                trackSegmentPosition.ValueWithoutEvent = (int)(positionPercentage * 10);

                if (_currentSegment != null)
                {
                    _currentSegment.Position = position;
                    //waveFormScrollView.SetSegment(_currentSegment);
                }
            }));
        }

        public void RefreshSegmentMarkers(IEnumerable<Marker> markers)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                _segmentMarkers = markers.ToList();
                comboSegmentMarker.Items.Clear();
                foreach (var marker in markers)
                    comboSegmentMarker.Items.Add(marker.Name);
            }));
        }

        #endregion

        #region IQueueView implementation

        public Action OnQueueStartPlayback { get; set; }
        public Action OnQueueRemoveAll { get; set; }

        public void QueueError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshQueue(int songCount, string totalLength)
        {
        }

        #endregion

    }
}
