// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Sessions.Core;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Config;
using Sessions.MVP.Services.Interfaces;
using Sessions.Sound.Playlists;
using Sessions.WPF.Classes.Controls;
using Sessions.WPF.Classes.Helpers;
using Sessions.WPF.Classes.Windows.Base;
using Sessions.Core.Helpers;
using Sessions.Library.Objects;
using Sessions.MVP.Messages;
using Sessions.MVP.Models;
using Sessions.MVP.Presenters;
using Sessions.MVP.Views;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace Sessions.WPF.Classes.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow, IMainView
    {
        private readonly IDownloadImageService _downloadImageService;
        private List<Marker> _markers;
        private List<Marker> _loopMarkers;
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
        private LibraryBrowserEntity _selectedLibraryNode;
        private static NotifyIcon _playerNotifyIcon;
        private Segment _startSegment;
        private Segment _endSegment;
        private bool _isPlayingLoop;

        public MainWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            _downloadImageService = Bootstrapper.GetContainer().Resolve<IDownloadImageService>();

            InitializeComponent();
            Initialize();
            ViewIsReady();
        }

        private void Initialize()
        {
            panelUpdateLibrary.Visibility = Visibility.Collapsed;
            songGridView.DoubleClick += SongGridViewOnDoubleClick;
            songGridView.MenuItemClicked += SongGridViewOnMenuItemClicked;
            scrollViewWaveForm.OnChangePosition += ScrollViewWaveForm_OnChangePosition;
            scrollViewWaveForm.OnChangeSecondaryPosition += ScrollViewWaveForm_OnChangeSecondaryPosition;
            scrollViewWaveForm.OnChangingSegmentPosition += ScrollViewWaveForm_OnChangingSegmentPosition;
            scrollViewWaveForm.OnChangedSegmentPosition += ScrollViewWaveForm_OnChangedSegmentPosition;

            InitializeComboBoxes();
            EnableMarkerButtons(false);
            EnableLoopButtons(false);
            RefreshSongInformation(null, 0, 0, 0);
            CreatePlayerNotifyIcon(() =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;                
            });
        }

        public static void CreatePlayerNotifyIcon(Action doubleClickAction)
        {
            var stream = Application.GetResourceStream(new Uri("pack://application:,,,/Sessions.WPF;component/Resources/Icon.ico")).Stream;
            _playerNotifyIcon = new NotifyIcon();
            _playerNotifyIcon.Icon = new Icon(stream);
            _playerNotifyIcon.Visible = AppConfigManager.Instance.Root.General.ShowAppInSystemTray;
            _playerNotifyIcon.DoubleClick += (sender, args) => doubleClickAction();
        }

        public static void EnablePlayerNotifyIcon(bool enable)
        {
            if (enable)
            {
                var stream = Application.GetResourceStream(new Uri("pack://application:,,,/Sessions.WPF;component/Resources/Icon.ico")).Stream;
                _playerNotifyIcon.Icon = new Icon(stream);
                _playerNotifyIcon.Visible = true;
            }
            else
            {
                _playerNotifyIcon.Visible = false;
                _playerNotifyIcon.Icon = null;
            }
        }

        public static void DisposePlayerNotifyIcon()
        {
            if (_playerNotifyIcon == null)
                return;

            _playerNotifyIcon.Visible = false;
            _playerNotifyIcon.Icon = null;
            _playerNotifyIcon.Dispose();
            _playerNotifyIcon = null;
        }

        private void InitializeComboBoxes()
        {
            comboSoundFormat.Items.Add(AudioFileFormat.All);
            comboSoundFormat.Items.Add(AudioFileFormat.APE);
            comboSoundFormat.Items.Add(AudioFileFormat.FLAC);
            comboSoundFormat.Items.Add(AudioFileFormat.MP3);
            comboSoundFormat.Items.Add(AudioFileFormat.MPC);
            comboSoundFormat.Items.Add(AudioFileFormat.OGG);
            comboSoundFormat.Items.Add(AudioFileFormat.WMA);
            comboSoundFormat.Items.Add(AudioFileFormat.WV);
            comboSoundFormat.SelectedIndex = 0;
        }

        private void EnableUIForPlayerStatus(PlayerStatusType status)
        {
            bool enabled = status != PlayerStatusType.Stopped && status != PlayerStatusType.Initialized;
            trackPosition.IsEnabled = enabled;
            trackTimeShifting.IsEnabled = enabled;
            trackPitchShifting.IsEnabled = enabled;
            btnAddLoop.IsEnabled = enabled;
            btnAddLoop.Enabled = enabled;
            btnAddMarker.IsEnabled = enabled;
            btnAddMarker.Enabled = enabled;
            btnUseThisTempo.IsEnabled = enabled;
            btnChangeKey.IsEnabled = enabled;
            btnEditSongMetadata.IsEnabled = enabled;
            btnSearchGuitarTabs.IsEnabled = enabled;
            btnSearchBassTabs.IsEnabled = enabled;
            btnSearchLyrics.IsEnabled = enabled;
            btnIncrementPitch.IsEnabled = enabled;
            btnDecrementPitch.IsEnabled = enabled;
            btnResetPitch.IsEnabled = enabled;
            btnIncrementTime.IsEnabled = enabled;
            btnDecrementTime.IsEnabled = enabled;
            btnResetTime.IsEnabled = enabled;
        }

        protected override void OnClosed(EventArgs e)
        {
            // We must close all windows of the application before leaving
            for(int i = App.Current.Windows.Count - 1; i >= 0; i--)
                App.Current.Windows[i].Close();

            DisposePlayerNotifyIcon();
            base.OnClosed(e);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (!AppConfigManager.Instance.Root.General.MinimizeAppInSystemTray)
                return;

            if(WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
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
            if (panelUpdateLibrary.Visibility == Visibility.Visible && show)
                return;

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
            var item = e.OriginalSource as LibraryTreeViewItem;
            if (item != null && item.Items.Count == 1)
            {
                var firstItem = item.Items[0] as LibraryTreeViewItem;
                if (firstItem.IsDummyNode)
                {
                    item.Items.Clear();
                    var entity = item.Header as LibraryBrowserEntity;
                    OnTreeNodeExpanded(entity, item);
                }
            }
            e.Handled = true;
        }

        private void SongGridViewOnDoubleClick(object sender, EventArgs eventArgs)
        {
            if (songGridView.SelectedAudioFiles.Count == 0)
                return;

            OnTableRowDoubleClicked(songGridView.SelectedAudioFiles[0]);
        }

        private void SongGridViewOnMenuItemClicked(SongGridView.MenuItemType menuItemType)
        {
            if (songGridView.SelectedAudioFiles.Count == 0)
                return;

            switch (menuItemType)
            {
                case SongGridView.MenuItemType.PlaySongs:
                    AudioFile audioFile = songGridView.SelectedAudioFiles[0];
                    OnTableRowDoubleClicked(audioFile);
                    break;
                case SongGridView.MenuItemType.AddToPlaylist:
                    var audioFiles = new List<AudioFile>();
                    foreach (var item in songGridView.SelectedAudioFiles)
                        audioFiles.Add(item);
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

        private void ResetLoopHeaderPanelVisibility()
        {
            panelLoopStartPosition.Visibility = Visibility.Hidden;
            panelLoopEndPosition.Visibility = Visibility.Hidden;            
        }

        private void ResetLoopHeaderButtonStyles()
        {
            var res = Application.Current.Resources;
            btnLoopStartPosition.Style = res["SmallHeaderButton"] as Style;
            btnLoopEndPosition.Style = res["SmallHeaderButton"] as Style;
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
            if (listViewMarkers.SelectedIndex < 0 || listViewMarkers.SelectedIndex >= _markers.Count)
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
            {
                _selectedLoopIndex = listViewLoops.SelectedIndex;
                scrollViewWaveForm.SetLoop(_loops[_selectedLoopIndex]);
                _currentLoop = _loops[_selectedLoopIndex];
                OnSelectLoop(_currentLoop);
            }
            else
            {
                scrollViewWaveForm.SetLoop(null);
            }
        }

        private void ListViewLoops_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Block double click on the textbox and image buttons. Cannot use these controls as a type
            if (e.OriginalSource.GetType().Name == "TextBoxView" ||
                e.OriginalSource.GetType().Name == "Image")
                return;

            //EditLoop();
            if(_currentLoop != null && _selectedLoopIndex >= 0)
                PlayOrStopCurrentLoop();
        }

        private void ListViewLoops_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIHelper.ListView_PreviewMouseDown_RemoveSelectionIfNotClickingOnAListViewItem(listViewLoops, e);
        }

        private void lblLoopName_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowLoopNameTextBox(sender);
        }

        private void TxtLoopName_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                HideLoopNameTextBoxFromTextBox(sender, true);
            }
            else if (e.Key == Key.Escape)
            {
                HideLoopNameTextBoxFromTextBox(sender, false);
            }
        }

        private void BtnLoopName_OK_OnClick(object sender, RoutedEventArgs e)
        {
            HideLoopNameTextBoxFromButton(sender, true);
        }

        private void BtnLoopName_Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            HideLoopNameTextBoxFromButton(sender, false);
        }

        private void ShowLoopNameTextBox(object sender)
        {
            var lblLoopName = sender as TextBlock;
            var grid = lblLoopName.Parent as Grid;
            var gridLoopName = UIHelper.FindByName("gridLoopName", grid) as Grid;
            var txtLoopName = UIHelper.FindByName("txtLoopName", gridLoopName) as TextBox;
            //_previousLoopName = lblLoopName.Text;
            txtLoopName.Text = lblLoopName.Text;

            gridLoopName.Visibility = Visibility.Visible;
            lblLoopName.Visibility = Visibility.Collapsed;
        }

        private void HideLoopNameTextBoxFromButton(object sender, bool saveLoopName)
        {
            var btnLoopNameOK = sender as Button;
            var gridLoopName = btnLoopNameOK.Parent as Grid;
            var grid = gridLoopName.Parent as Grid;
            var lblLoopName = UIHelper.FindByName("lblLoopName", grid) as TextBlock;
            var loop = btnLoopNameOK.DataContext as Loop;
            var txtLoopName = UIHelper.FindByName("txtLoopName", gridLoopName) as TextBox;

            gridLoopName.Visibility = Visibility.Collapsed;
            lblLoopName.Visibility = Visibility.Visible;

            if (saveLoopName)
            {
                loop.Name = txtLoopName.Text;
                lblLoopName.Text = txtLoopName.Text;
                OnUpdateLoop(loop);
            }
            else
            {
                loop.Name = lblLoopName.Text;
                //lblLoopName.Text = _previousLoopName;
                //loop.Name = _previousLoopName;
            }
        }

        private void HideLoopNameTextBoxFromTextBox(object sender, bool saveLoopName)
        {
            var txtLoopName = sender as TextBox;
            var gridLoopNameTextbox = txtLoopName.Parent as Grid;
            var gridLoopName = gridLoopNameTextbox.Parent as Grid;
            var grid = gridLoopName.Parent as Grid;
            var lblLoopName = UIHelper.FindByName("lblLoopName", grid) as TextBlock;
            var loop = txtLoopName.DataContext as Loop;

            gridLoopName.Visibility = Visibility.Collapsed;
            lblLoopName.Visibility = Visibility.Visible;

            if (saveLoopName)
            {
                loop.Name = txtLoopName.Text;
                lblLoopName.Text = txtLoopName.Text;
                OnUpdateLoop(loop);
            }
            else
            {
                loop.Name = lblLoopName.Text;
                //lblLoopName.Text = _previousLoopName;
                //loop.Name = _previousLoopName;
            }
        }

        private void BtnLoopStartPositionPunchIn_OnClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                var loop = frameworkElement.DataContext as Loop;
                OnSelectLoop(loop);
                if (loop != null) OnPunchInLoopSegment(loop.GetStartSegment());
            }
        }

        private void BtnLoopEndPositionPunchIn_OnClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                var loop = frameworkElement.DataContext as Loop;
                OnSelectLoop(loop);
                if (loop != null) OnPunchInLoopSegment(loop.GetEndSegment());
            }
        }

        private void BtnLoopStartPositionPunchIn_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Empty because we want to block the double click action on this button so the loop does not start playing
        }

        private void BtnLoopEndPositionPunchIn_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Empty because we want to block the double click action on this button so the loop does not start playing
        }

        private void BtnBackLoopDetails_OnClick(object sender, RoutedEventArgs e)
        {
            gridLoops.Visibility = Visibility.Visible;
            gridLoopDetails.Visibility = Visibility.Hidden;
            gridLoopPlayback.Visibility = Visibility.Hidden;

            //_currentSegment.MarkerId = Guid.Empty;
            //if (chkSegmentLinkToMarker.IsChecked.Value && comboSegmentMarker.SelectedIndex >= 0)
            //    _currentSegment.MarkerId = _segmentMarkers[comboSegmentMarker.SelectedIndex].MarkerId;

            //OnUpdateSegmentDetails(_currentSegment);
            //_currentSegment = null;

            _currentLoop.Name = txtLoopName.Text;
            OnUpdateLoopDetails(_currentLoop);
            _currentLoop = null;
            scrollViewWaveForm.SetLoop(null);
        }

        private void BtnPlayLoop_OnClick(object sender, RoutedEventArgs e)
        {
            PlayOrStopCurrentLoop();
        }

        private void PlayOrStopCurrentLoop()
        {
            OnPlayLoop(_loops[_selectedLoopIndex]);
            _isPlayingLoop = !_isPlayingLoop;
            SetPlayLoopButtonState(_isPlayingLoop);
        }

        private void SetPlayLoopButtonState(bool isPlayingLoop)
        {
            if (isPlayingLoop)
                btnPlayLoop.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Buttons/stop.png"));
            else
                btnPlayLoop.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Buttons/play.png"));                       
        }

        private void BtnAddLoop_OnClick(object sender, RoutedEventArgs e)
        {
            OnAddLoop();
            //gridLoops.Visibility = Visibility.Hidden;
            //gridLoopDetails.Visibility = Visibility.Visible;
            //gridLoopPlayback.Visibility = Visibility.Hidden;
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
        }

        private void BtnRemoveLoop_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewLoops.SelectedIndex < 0 || listViewLoops.SelectedIndex >= _loops.Count)
                return;

            var result = MessageBox.Show("Are you sure you wish to remove this loop?", "Remove loop", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                OnDeleteLoop(_loops[listViewLoops.SelectedIndex]);
                EnableLoopButtons(false);
            }
        }

        private void EnableLoopButtons(bool enabled)
        {
            btnPlayLoop.Enabled = enabled;
            //btnEditLoop.Enabled = enabled;
            btnRemoveLoop.Enabled = enabled;
        }

        private void BtnLoopStartPosition_OnClick(object sender, RoutedEventArgs e)
        {
            ShowLoopStartPositionTab();
        }

        private void ShowLoopStartPositionTab()
        {
            if (panelLoopStartPosition.Visibility == Visibility.Visible)
                return;

            ResetLoopHeaderButtonStyles();
            ResetLoopHeaderPanelVisibility();
            btnLoopStartPosition.Style = System.Windows.Application.Current.Resources["SmallHeaderButtonSelected"] as Style;
            panelLoopStartPosition.Visibility = Visibility.Visible;                        
        }

        private void BtnLoopEndPosition_OnClick(object sender, RoutedEventArgs e)
        {
            ShowLoopEndPositionTab();
        }

        private void ShowLoopEndPositionTab()
        {
            if (panelLoopEndPosition.Visibility == Visibility.Visible)
                return;

            ResetLoopHeaderButtonStyles();
            ResetLoopHeaderPanelVisibility();
            btnLoopEndPosition.Style = System.Windows.Application.Current.Resources["SmallHeaderButtonSelected"] as Style;
            panelLoopEndPosition.Visibility = Visibility.Visible;
        }

        private void TxtLoopName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _currentLoop.Name = txtLoopName.Text;
            scrollViewWaveForm.SetLoop(_currentLoop);
        }

        private void TrackStartSegmentPosition_OnTrackBarValueChanged()
        {
            ChangeStartSegment(trackStartSegmentPosition.Value / 1000f, false);
        }

        private void TrackStartSegmentPosition_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangeStartSegment(trackStartSegmentPosition.Value / 1000f, true);
        }

        private void TrackEndSegmentPosition_OnTrackBarValueChanged()
        {
            ChangeEndSegment(trackEndSegmentPosition.Value / 1000f, false);
        }

        private void TrackEndSegmentPosition_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangeEndSegment(trackEndSegmentPosition.Value / 1000f, true);
        }

        private void ChkStartSegmentLinkToMarker_OnChecked(object sender, RoutedEventArgs e)
        {
            SetStartSegmentLinkedMarker();
        }

        private void ChkEndSegmentLinkToMarker_OnChecked(object sender, RoutedEventArgs e)
        {
            SetEndSegmentLinkedMarker();
        }

        private void SetStartSegmentLinkedMarker()
        {
            if (_loopMarkers.Count == 0)
            {
                chkStartSegmentLinkToMarker.Unchecked -= ChkStartSegmentLinkToMarker_OnChecked;
                chkStartSegmentLinkToMarker.IsChecked = false;
                chkStartSegmentLinkToMarker.Unchecked += ChkStartSegmentLinkToMarker_OnChecked;
                MessageBox.Show("There are no markers to link to this segment.", "Cannot link to marker", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            comboStartSegmentMarker.Visibility = chkStartSegmentLinkToMarker.IsChecked.Value ? Visibility.Visible : Visibility.Hidden; //.Hidden = !chkSegmentLinkToMarker.Value;
            if (chkStartSegmentLinkToMarker.IsChecked.Value && comboStartSegmentMarker.SelectedIndex >= 0)
            {
                var marker = _loopMarkers[comboStartSegmentMarker.SelectedIndex];
                OnLinkSegmentToMarker(_startSegment, marker.MarkerId);
            }
            else
            {
                //OnLinkSegmentToMarker(_startSegment, Guid.Empty);
                comboStartSegmentMarker.SelectedIndex = 0;
                var marker = _loopMarkers[0];
                OnLinkSegmentToMarker(_startSegment, marker.MarkerId);
            }
        }

        private void SetEndSegmentLinkedMarker()
        {
            if (_loopMarkers.Count == 0)
            {
                chkEndSegmentLinkToMarker.Unchecked -= ChkEndSegmentLinkToMarker_OnChecked;
                chkEndSegmentLinkToMarker.IsChecked = false;
                chkEndSegmentLinkToMarker.Unchecked += ChkEndSegmentLinkToMarker_OnChecked;
                MessageBox.Show("There are no markers to link to this segment.", "Cannot link to marker", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            comboEndSegmentMarker.Visibility = chkEndSegmentLinkToMarker.IsChecked.Value ? Visibility.Visible : Visibility.Hidden; //.Hidden = !chkSegmentLinkToMarker.Value;
            if (chkEndSegmentLinkToMarker.IsChecked.Value && comboEndSegmentMarker.SelectedIndex >= 0)
            {
                var marker = _loopMarkers[comboEndSegmentMarker.SelectedIndex];
                OnLinkSegmentToMarker(_endSegment, marker.MarkerId);
            }
            else
            {
                OnLinkSegmentToMarker(_endSegment, Guid.Empty);
            }
        }

        private void BtnPunchInStartSegment_OnClick(object sender, RoutedEventArgs e)
        {
            if (_startSegment == null)
                return;

            chkStartSegmentLinkToMarker.IsChecked = false;
            comboStartSegmentMarker.Visibility = Visibility.Hidden;
            OnPunchInSegment(_startSegment);
        }

        private void BtnPunchInEndSegment_OnClick(object sender, RoutedEventArgs e)
        {
            if (_endSegment == null)
                return;

            chkEndSegmentLinkToMarker.IsChecked = false;
            comboEndSegmentMarker.Visibility = Visibility.Hidden;
            OnPunchInSegment(_endSegment);
        }

        private void ScrollViewWaveForm_OnChangingSegmentPosition(Segment segment, float positionPercentage)
        {
            if (gridLoopDetails.Visibility == Visibility.Visible)
            {
                OnChangingSegmentPosition(segment, positionPercentage);
            }
            else
            {
                OnChangingLoopSegmentPosition(segment, positionPercentage);
            }

            // Select the correct segment tab
            var startSegment = _currentLoop.GetStartSegment();
            var endSegment = _currentLoop.GetEndSegment();
            if (startSegment.SegmentId == segment.SegmentId)
                ShowLoopStartPositionTab();
            else if (endSegment.SegmentId == segment.SegmentId)
                ShowLoopEndPositionTab();
        }

        private void ScrollViewWaveForm_OnChangedSegmentPosition(Segment segment, float positionPercentage)
        {
            if (gridLoopDetails.Visibility == Visibility.Visible)
            {
                OnChangedSegmentPosition(segment, positionPercentage);
            }
            else
            {
                OnChangedLoopSegmentPosition(segment, positionPercentage);
            }
        }

        private void ChangeStartSegment(float percentage, bool mouseUp)
        {
            chkStartSegmentLinkToMarker.IsChecked = false;
            comboStartSegmentMarker.Visibility = Visibility.Hidden;
            //trackSegmentPosition.ValueWithoutEvent = (int) (percentage*1000f);

            if (mouseUp)
            {
                if (OnChangedSegmentPosition != null)
                    OnChangedSegmentPosition(_startSegment, percentage);
            }
            else
            {                
                if (OnChangingSegmentPosition != null)
                    OnChangingSegmentPosition(_startSegment, percentage);
            }
        }

        private void ChangeEndSegment(float percentage, bool mouseUp)
        {
            chkEndSegmentLinkToMarker.IsChecked = false;
            comboEndSegmentMarker.Visibility = Visibility.Hidden;
            //trackSegmentPosition.ValueWithoutEvent = (int) (percentage*1000f);

            if (mouseUp)
            {
                if (OnChangedSegmentPosition != null)
                    OnChangedSegmentPosition(_endSegment, percentage);
            }
            else
            {
                if (OnChangingSegmentPosition != null)
                    OnChangingSegmentPosition(_endSegment, percentage);                
            }
        }

        private void BtnBackLoopPlayback_OnClick(object sender, RoutedEventArgs e)
        {
            gridLoops.Visibility = Visibility.Visible;
            gridLoopDetails.Visibility = Visibility.Hidden;
            gridLoopPlayback.Visibility = Visibility.Hidden;
            //gridSegmentDetails.Visibility = Visibility.Hidden;
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
            var value = (LibraryTreeViewItem)treeViewLibrary.SelectedValue;
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
            var value = (LibraryTreeViewItem)treeViewLibrary.SelectedValue;
            var entity = value.Entity;
            if (entity != null)
                OnAddToPlaylist(entity);
        }

        private void MenuItemLibraryBrowserRemoveFromLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (MessageBox.Show("Are you sure you wish to remove these audio files from your library?\nThis does not delete the audio files from your hard disk.", "Audio files will be removed from library", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            var value = (LibraryTreeViewItem)treeViewLibrary.SelectedValue;
            var entity = value.Entity;
            if (entity != null)
                OnRemoveFromLibrary(entity);
        }

        private void MenuItemLibraryBrowserDeleteFromHardDisk_OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (MessageBox.Show("Are you sure you wish to delete these audio files from your hard disk?\nWARNING: This operation CANNOT be undone!", "Audio files will be deleted from hard disk", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            var value = (LibraryTreeViewItem)treeViewLibrary.SelectedValue;
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

        private void BtnHideUpdateLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            ShowUpdateLibraryPanel(false, () => panelUpdateLibrary.Visibility = Visibility.Collapsed);
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

        private void BtnApplyAlbumArt_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnChooseAlbumArt_OnClick(object sender, RoutedEventArgs e)
        {
            OnOpenSelectAlbumArt();
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
            //Console.WriteLine("MainWindow - RefreshLibraryBrowser - entities.Count: {0}", entities.Count());
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                treeViewLibrary.Items.Clear();
                foreach (var entity in entities)
                {
                    var item = new LibraryTreeViewItem();
                    //item.Expanding += (sender, args) => { Console.WriteLine("Expanding"); };
                    item.Entity = entity;
                    item.Header = entity;
                    item.HeaderTemplate = FindResource("TreeViewItemTemplate") as DataTemplate;

                    if (entity.SubItems.Count > 0)
                    {
                        var dummy = new LibraryTreeViewItem();
                        dummy.IsDummyNode = true;
                        item.Items.Add(dummy);
                    }

                    treeViewLibrary.Items.Add(item);
                }
            }));
        }

        public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
        {
            //Console.WriteLine("MainWindow - RefreshLibraryBrowserNode - entities.Count: {0}", entities.Count());
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                var item = (LibraryTreeViewItem) userData;
                foreach (var subentity in entities)
                {
                    var subitem = new LibraryTreeViewItem();
                    subitem.Entity = subentity;
                    subitem.Header = subentity;
                    subitem.HeaderTemplate = FindResource("TreeViewItemTemplate") as DataTemplate;

                    if (subentity.SubItems.Count > 0)
                    {
                        var dummy = new LibraryTreeViewItem();                        
                        dummy.IsDummyNode = true;
                        subitem.Items.Add(dummy);
                    }

                    item.Items.Add(subitem);
                }

                if (_selectedLibraryNode != null)
                {
                    switch (entity.EntityType)
                    {
                        case LibraryBrowserEntityType.Artists:
                            var artistNode = GetLibraryTreeViewItemByArtistName(item.Items, _selectedLibraryNode.Query.ArtistName);
                            if (artistNode != null)
                            {
                                if (_selectedLibraryNode.EntityType == LibraryBrowserEntityType.Artist)
                                {
                                    artistNode.IsSelected = true;
                                    artistNode.BringIntoView();
                                    _selectedLibraryNode = null;
                                }
                                else if (_selectedLibraryNode.EntityType == LibraryBrowserEntityType.ArtistAlbum)
                                {
                                    artistNode.IsExpanded = true;
                                    artistNode.BringIntoView();
                                }
                            }
                            break;
                        case LibraryBrowserEntityType.Artist:
                            var artistAlbumNode = GetLibraryTreeViewItemByAlbumTitle(item.Items, _selectedLibraryNode.Query.AlbumTitle);
                            if (artistAlbumNode != null)
                            {
                                artistAlbumNode.IsSelected = true;
                                artistAlbumNode.BringIntoView(); //new Rect(0, treeViewLibrary.ScrollViewer.VerticalOffset, 10, 10));
                            }
                            _selectedLibraryNode = null;
                            break;
                        case LibraryBrowserEntityType.Albums:
                            var albumNode = GetLibraryTreeViewItemByAlbumTitle(item.Items, _selectedLibraryNode.Query.AlbumTitle);
                            if (albumNode != null)
                            {
                                albumNode.IsSelected = true;
                                albumNode.BringIntoView();
                            }
                            _selectedLibraryNode = null;
                            break;
                    }
                }

                // When calling BringIntoView, the horizontal offset is not always zero. 
                // This is a workaround, BringIntoView(Rect) doesn't seem to work...
                treeViewLibrary.ScrollViewer.ScrollToHorizontalOffset(0);
            }));
        }

        // Note: these helper methods only exist because you cannot use LINQ on ItemCollection :-(
        public static LibraryTreeViewItem GetLibraryTreeViewItem(ItemCollection items, LibraryBrowserEntityType entityType)
        {        
            foreach (var treeViewItem in items)
            {
                var item = treeViewItem as LibraryTreeViewItem;
                if (item.Entity.EntityType == entityType)
                {
                    return item;
                }
            }

            return null;
        }

        public static LibraryTreeViewItem GetLibraryTreeViewItemByArtistName(ItemCollection items, string artistName)
        {
            foreach (var treeViewItem in items)
            {
                var item = treeViewItem as LibraryTreeViewItem;
                if (string.Compare(item.Entity.Query.ArtistName, artistName, true) == 0)
                {
                    return item;
                }
            }

            return null;
        }

        public static LibraryTreeViewItem GetLibraryTreeViewItemByAlbumTitle(ItemCollection items, string albumTitle)
        {
            foreach (var treeViewItem in items)
            {
                var item = treeViewItem as LibraryTreeViewItem;
                if (string.Compare(item.Entity.Query.AlbumTitle, albumTitle, true) == 0)
                {
                    return item;
                }
            }

            return null;
        }

        public void RefreshLibraryBrowserSelectedNode(LibraryBrowserEntity entity)
        {
            _selectedLibraryNode = entity;
            Console.WriteLine("MainWindow - RefreshLibraryBrowserSelectedNode - isnull: {0} title: {1}", entity == null, entity == null ? string.Empty : entity.Title);
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                LibraryTreeViewItem item = null;
                switch (entity.EntityType)
                {
                    case LibraryBrowserEntityType.Artists:
                        item = GetLibraryTreeViewItem(treeViewLibrary.Items, LibraryBrowserEntityType.Artists);
                        item.IsSelected = true;
                        break;
                    case LibraryBrowserEntityType.Albums:
                        item = GetLibraryTreeViewItem(treeViewLibrary.Items, LibraryBrowserEntityType.Albums);
                        item.IsSelected = true;
                        break;
                    case LibraryBrowserEntityType.ArtistAlbum:
                    case LibraryBrowserEntityType.Artist:
                        item = GetLibraryTreeViewItem(treeViewLibrary.Items, LibraryBrowserEntityType.Artists);
                        item.IsExpanded = true;
                        item.BringIntoView();
                        break;
                    case LibraryBrowserEntityType.Album:
                        item = GetLibraryTreeViewItem(treeViewLibrary.Items, LibraryBrowserEntityType.Albums);
                        item.IsExpanded = true;
                        item.BringIntoView();
                        break;
                }
            }));
        }

        public void NotifyLibraryBrowserNewNode(int position, LibraryBrowserEntity entity)
        {
            //Console.WriteLine("===========>>>> MainWindow - RefreshLibraryBrowserSelectedNode - isnull: {0} title: {1} position: {2}", entity == null, entity == null ? string.Reset : entity.Title, position);
        }

        public void NotifyLibraryBrowserRemovedNode(int position)
        {
            //Console.WriteLine("===========>>>> MainWindow - RefreshLibraryBrowserSelectedNode - position: {0}", position);
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
                songGridView.SetAudioFiles(audioFiles.ToList());
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
        public Func<float, PlayerPosition> OnPlayerRequestPosition { get; set; }
        public Action OnEditSongMetadata { get; set; }
        public Action OnOpenPlaylist { get; set; }
        public Action OnOpenEffects { get; set; }
        public Action OnOpenSelectAlbumArt { get; set; }
        public Action OnPlayerViewAppeared { get; set; }
        public Action<byte[]> OnApplyAlbumArtToSong { get; set; }
        public Action<byte[]> OnApplyAlbumArtToAlbum { get; set; }

        public void PlayerError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void PushSubView(IBaseView view)
        {
        }

        public void RefreshPlayerStatus(PlayerStatusType status, RepeatType repeatType, bool isShuffleEnabled)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (status == PlayerStatusType.Playing)
                    imagePlayPause.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Toolbar/pause.png"));
                else
                    imagePlayPause.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Toolbar/play.png"));

                switch (repeatType)
                {
                    case RepeatType.Off:
                        imageRepeat.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Toolbar/repeat_off.png"));
                        break;
                    case RepeatType.Playlist:
                        imageRepeat.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Toolbar/repeat_on.png"));
                        break;
                    case RepeatType.Song:
                        imageRepeat.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Toolbar/repeat_single.png"));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                string imageName = isShuffleEnabled ? "shuffle_on" : "shuffle_off";
                imageShuffle.Source = new BitmapImage(new Uri(string.Format("pack://application:,,,/Resources/Images/Toolbar/{0}.png", imageName)));

                EnableUIForPlayerStatus(status);
            }));
        }

        public void RefreshPlayerPosition(PlayerPosition entity)
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

        public void RefreshPlaylist(Playlist playlist)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                playlistListView.SetPlaylist(playlist);
            }));
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            _selectedMarkerIndex = -1;
            _currentAudioFile = audioFile;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
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
                    lblPosition.Content = "0:00.000";
                    lblLength.Content = "0:00.000";
                    _currentAlbumArtKey = string.Empty;
                    imageAlbum.Source = null;
                    scrollViewWaveForm.Reset();   
                    outputMeter.Reset();
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

                    songGridView.NowPlayingAudioFileId = audioFile.Id;

                    scrollViewWaveForm.SetWaveFormLength(lengthBytes);
                    scrollViewWaveForm.LoadPeakFile(audioFile);

                    string key = audioFile.ArtistName.ToUpper() + "_" + audioFile.AlbumTitle.ToUpper();
                    if (_currentAlbumArtKey != key)
                    {
                        //panelImageDownloaded.Visibility = Visibility.Hidden;
                        UIHelper.FadeElement(panelImageDownloading, false, 1, null);
                        UIHelper.FadeElement(panelImageDownloaded, false, 1, null);
                        UIHelper.FadeElement(panelImageDownloadError, false, 1, null);
                        UIHelper.FadeElement(imageAlbum, false, 200, () =>
                        {
                            TryToDownloadAlbumArtFromFileOrInternet(audioFile, key);                            
                        });
                    }
                }

                SetPlayLoopButtonState(false);
            }));
        }

        private async void TryToDownloadAlbumArtFromFileOrInternet(AudioFile audioFile, string key)
        {
            imageAlbum.Source = null;
            var task = Task<BitmapImage>.Factory.StartNew(() =>
            {
                try
                {
                    var bitmap = ImageHelper.GetAlbumArtFromAudioFile(audioFile.FilePath);
                    return bitmap;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occured while extracing album art in {0}: {1}",
                        audioFile.FilePath, ex);
                }

                return null;
            });

            var imageResult = await task;
            if (imageResult != null)
            {
                _currentAlbumArtKey = key;
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    imageAlbum.Opacity = 0;
                    imageAlbum.Source = imageResult;
                    UIHelper.FadeElement(imageAlbum, true, 200, null);
                }));
            }
            else
            {
                TryToDownloadAlbumArtFromInternet(audioFile, key);
            }            
        }

        private async void TryToDownloadAlbumArtFromInternet(AudioFile audioFile, string key)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UIHelper.FadeElement(panelImageDownloading, true, 200, null)));

            var taskDownload = _downloadImageService.DownloadAlbumArt(audioFile);
            taskDownload.Start();            
            var result = await taskDownload;
            if (result != null)
            {
                var task = Task<BitmapImage>.Factory.StartNew(() =>
                {
                    try
                    {
                        if (result.ImageData == null)
                            return null;

                        var bitmap = ImageHelper.GetBitmapImageFromBytes(result.ImageData);
                        return bitmap;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occured while extracing album art in {0}: {1}",
                            audioFile.FilePath, ex);
                    }

                    return null;
                });

                var imageResult = await task;
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    UIHelper.FadeElement(panelImageDownloading, false, 200, () =>
                    {                                            
                        if (imageResult != null)
                        {
                            // Show new album art with overloay
                            _currentAlbumArtKey = key;
                            imageAlbum.Opacity = 0;
                            imageAlbum.Source = imageResult;
                            UIHelper.FadeElement(imageAlbum, true, 200, () => {
                                UIHelper.FadeElement(panelImageDownloaded, true, 400, 200, null);
                            });
                        }
                        else
                        {
                            // Indicate error
                            UIHelper.FadeElement(panelImageDownloadError, true, 200, null);
                        }
                    });
                }));
            }
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

        public void RefreshPlayerVolume(PlayerVolume entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblVolume.Content = entity.VolumeString;
                if (faderVolume.Value != (int)entity.Volume)
                    faderVolume.ValueWithoutEvent = (int) entity.Volume;
            }));
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShifting entity)
        {
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
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
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                _markers = markers.ToList();
                listViewMarkers.Items.Clear();
                foreach (var marker in markers)
                    listViewMarkers.Items.Add(marker);                
                listViewMarkers.SelectedIndex = _selectedMarkerIndex;
                scrollViewWaveForm.SetMarkers(_markers);
            }));
        }

        public void RefreshMarkerPosition(Marker marker, int newIndex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
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
        public Action<Loop> OnSelectLoop { get; set; }
        public Action<Loop> OnDeleteLoop { get; set; }
        public Action<Loop> OnPlayLoop { get; set; }
        public Action<Loop> OnUpdateLoop { get; set; }

        public Action<Segment> OnPunchInLoopSegment { get; set; }
        public Action<Segment, float> OnChangingLoopSegmentPosition { get; set; }
        public Action<Segment, float> OnChangedLoopSegmentPosition { get; set; }

        public void LoopError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshLoops(List<Loop> loops)
        {
            _loops = loops.ToList();
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                listViewLoops.ItemsSource = _loops;
                listViewLoops.SelectedIndex = _selectedLoopIndex;
            }));
        }

        public void RefreshLoopSegment(Segment segment, long audioFileLength)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                listViewLoops.Items.Refresh();
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

        public void RefreshTimeShifting(PlayerTimeShifting entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblDetectedTempo.Content = entity.DetectedTempo;
                lblReferenceTempo.Content = entity.ReferenceTempo;
                lblCurrentTempo.Content = entity.CurrentTempo;
                //sliderTimeShifting.SetValueWithoutTriggeringEvent((int)entity.TimeShiftingValue);
                trackTimeShifting.ValueWithoutEvent = (int) entity.TimeShiftingValue;
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

        public void RefreshPitchShifting(PlayerPitchShifting entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblInterval.Content = entity.Interval;
                lblCurrentKey.Content = entity.NewKey.Item2;
                lblReferenceKey.Content = entity.ReferenceKey.Item2;
                //    trackPitch.SetValueWithoutTriggeringEvent(entity.IntervalValue);
                trackPitchShifting.ValueWithoutEvent = (int)entity.IntervalValue;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ShowUpdateLibraryPanel(true, null);
            }));
        }

        public void ProcessEnded(bool canceled)
        {
            if (panelUpdateLibrary.Visibility == Visibility.Collapsed)
                return;

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
                trackMarkerPosition.ValueWithoutEvent = (int)(marker.PositionPercentage * 1000);
                scrollViewWaveForm.SetActiveMarker(marker.MarkerId);
            }));
        }

        public void RefreshMarkerPosition(string position, float positionPercentage)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblMarkerPosition.Content = position;
                trackMarkerPosition.ValueWithoutEvent = (int)(positionPercentage * 1000);
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
        public Action<Segment> OnPunchInSegment { get; set; }
        public Action<Segment, int> OnChangeSegmentOrder { get; set; }
        public Action<Segment, Guid> OnLinkSegmentToMarker { get; set; }
        public Action<Segment, float> OnChangingSegmentPosition { get; set; }
        public Action<Segment, float> OnChangedSegmentPosition { get; set; }

        public void LoopDetailsError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshLoopDetails(Loop loop, AudioFile audioFile, long audioFileLength)
        {
            //_currentLoop = loop;
            //Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            //    txtLoopName.Text = loop.Name;
            //    scrollViewWaveForm.SetLoop(loop);
            //    //scrollViewWaveForm.FocusZoomOnLoop(_currentLoop);

            //    _startSegment = _currentLoop.GetStartSegment();
            //    if (_startSegment != null)
            //    {
            //        lblLoopStartPosition.Content = _startSegment.Position;
            //        float positionPercentage = (float)_startSegment.PositionBytes / (float)audioFileLength;
            //        trackStartSegmentPosition.ValueWithoutEvent = (int)(positionPercentage * 1000);
            //    }

            //    _endSegment = _currentLoop.GetEndSegment();
            //    if (_endSegment != null)
            //    {
            //        lblLoopEndPosition.Content = _endSegment.Position;
            //        float positionPercentage = (float)_endSegment.PositionBytes / (float)audioFileLength;
            //        trackEndSegmentPosition.ValueWithoutEvent = (int)(positionPercentage * 1000);
            //    }

            //    lblLoopLength.Content = _currentLoop.TotalLength;
            //}));
        }

        public void RefreshLoopDetailsSegment(Segment segment, long audioFileLength)
        {
            //Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            //    var startSegment = _currentLoop.GetStartSegment();
            //    var endSegment = _currentLoop.GetEndSegment();

            //    if (startSegment == null || endSegment == null)
            //        return;

            //    if (startSegment.SegmentId == segment.SegmentId)
            //    {
            //        lblLoopStartPosition.Content = segment.Position;
            //        float positionPercentage = (float)_startSegment.PositionBytes / (float)audioFileLength;
            //        trackStartSegmentPosition.ValueWithoutEvent = (int)(positionPercentage * 1000);
            //    }
            //    else if (endSegment.SegmentId == segment.SegmentId)
            //    {
            //        lblLoopEndPosition.Content = segment.Position;
            //        float positionPercentage = (float)_endSegment.PositionBytes / (float)audioFileLength;
            //        trackEndSegmentPosition.ValueWithoutEvent = (int)(positionPercentage * 1000);
            //    }

            //    lblLoopLength.Content = _currentLoop.TotalLength;
            //    scrollViewWaveForm.SetLoop(_currentLoop);
            //}));        
        }

        public void RefreshLoopMarkers(IEnumerable<Marker> markers)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                _loopMarkers = markers.ToList();
                comboStartSegmentMarker.Items.Clear();
                comboEndSegmentMarker.Items.Clear();
                foreach (var marker in markers)
                {
                    comboStartSegmentMarker.Items.Add(marker.Name);
                    comboEndSegmentMarker.Items.Add(marker.Name);
                }
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
            ////Console.WriteLine("RefreshSegmentDetails - position: {0}", segment.Position);
            //_currentSegment = segment;
            //Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            //    //waveFormScrollView.SetSegment(segment);
            //    chkSegmentLinkToMarker.IsChecked = segment.MarkerId != Guid.Empty;
            //    comboSegmentMarker.Visibility = segment.MarkerId == Guid.Empty ? Visibility.Hidden : Visibility.Visible;
            //    int index = _segmentMarkers.FindIndex(x => x.MarkerId == segment.MarkerId);
            //    if (index >= 0)
            //        comboSegmentMarker.SelectedIndex = index;

            //    float positionPercentage = (float)segment.PositionBytes / (float)audioFileLength;
            //    trackSegmentPosition.ValueWithoutEvent = (int)(positionPercentage * 10);
            //    lblSegmentPosition.Content = segment.Position;
            //}));
        }

        public void RefreshSegmentPosition(string position, float positionPercentage)
        {
            //Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            //    lblSegmentPosition.Content = position;
            //    trackSegmentPosition.ValueWithoutEvent = (int)(positionPercentage * 10);

            //    if (_currentSegment != null)
            //    {
            //        _currentSegment.Position = position;
            //        scrollViewWaveForm.SetSegment(_currentSegment);
            //    }
            //}));
        }

        public void RefreshSegmentMarkers(IEnumerable<Marker> markers)
        {
            //Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            //    _segmentMarkers = markers.ToList();
            //    comboSegmentMarker.Items.Clear();
            //    foreach (var marker in markers)
            //        comboSegmentMarker.Items.Add(marker.Name);
            //}));
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
