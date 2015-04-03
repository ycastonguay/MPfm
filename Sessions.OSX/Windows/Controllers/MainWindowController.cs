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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Sessions.Core;
using Sessions.Core.Helpers;
using Sessions.Library.Objects;
using Sessions.MVP.Models;
using Sessions.MVP.Presenters;
using Sessions.MVP.Views;
using Sessions.Player;
using Sessions.Sound.AudioFiles;
using org.sessionsapp.player;
using Sessions.OSX.Classes.Controls;
using Sessions.OSX.Classes.Delegates;
using Sessions.OSX.Classes.Helpers;
using Sessions.OSX.Classes.Objects;
using Sessions.Sound.Objects;
using Sessions.Sound.Player;

namespace Sessions.OSX
{
    /// <summary>
    /// Main window controller.
    /// </summary>
	public partial class MainWindowController : BaseWindowController, IMainView
	{
        private LibraryBrowserEntity _currentLibraryBrowserEntity;
        private string _currentAlbumArtKey;
        private bool _isPlayerPositionChanging = false;
        private bool _isScrollViewWaveFormChangingSecondaryPosition = false;
        private List<Marker> _markers = new List<Marker>();
        private List<SSPLoop> _loops = new List<SSPLoop>();
        private List<Marker> _segmentMarkers = new List<Marker>();
        private Marker _currentMarker;
        private SSPLoop _currentLoop;
        //private Segment _currentSegment;
        private SongInformationEntity _currentSongInfo;
        private SSPPlayerState _playerStatus;
        private LibraryBrowserOutlineViewDelegate _libraryBrowserOutlineViewDelegate = null;
        private LibraryBrowserDataSource _libraryBrowserDataSource = null;
        private NSObject _eventMonitor;

		public MainWindowController(IntPtr handle) 
            : base (handle)
		{
		}

		public MainWindowController(Action<IBaseView> onViewReady) : base ("MainWindow", onViewReady)
        {
            Window.AlphaValue = 0;
            ShowWindowCentered();

            // Fade in main window
            var dict = new NSMutableDictionary();
            dict.Add(NSViewAnimation.TargetKey, Window);
            dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeInEffect);
            var anim = new NSViewAnimation(new List<NSMutableDictionary>(){ dict }.ToArray());
            anim.Duration = 0.25f;
            anim.StartAnimation();
        }

		public override void WindowDidLoad()
		{
            base.WindowDidLoad();
            Tracing.Log("MainWindowController.WindowDidLoad -- Initializing user interface...");
            //this.Window.Title = "Sessions " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA";

            LoadButtons();
            LoadComboBoxes();
            LoadCheckBoxes();
            LoadTableViews();
            LoadTrackBars();
            LoadTreeViews();
            LoadImages();
            HidePanels();
            SetTheme();
            EnableLoopButtons(false);
            EnableMarkerButtons(false);
            EnableSegmentButtons(false);
            RefreshSongInformation(new SongInformationEntity());
            SetupLocalEventMonitorForKeys();

            // Make sure the text field doesn't trigger local event monitor
            searchSongBrowser.EditingBegan += (sender, e) => RemoveLocalEventMonitorForKeys();
            searchSongBrowser.EditingEnded += (sender, e) => SetupLocalEventMonitorForKeys();

            splitMain.Delegate = new MainSplitViewDelegate();
            splitMain.PostsBoundsChangedNotifications = true;
            splitMain.PostsFrameChangedNotifications = true;
            NSNotificationCenter.DefaultCenter.AddObserver(NSView.FrameChangedNotification, SplitViewFrameDidChangeNotification, splitMain);

            viewUpdateLibrary.Hidden = true;
            waveFormScrollView.OnChangePosition += ScrollViewWaveForm_OnChangePosition;
            waveFormScrollView.OnChangeSecondaryPosition += ScrollViewWaveForm_OnChangeSecondaryPosition;
//            waveFormScrollView.OnChangingSegmentPosition += ScrollViewWaveForm_OnChangingSegmentPosition;
//            waveFormScrollView.OnChangedSegmentPosition += ScrollViewWaveForm_OnChangedSegmentPosition;
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("NSControlTextDidChangeNotification"), SearchTextDidChange, searchSongBrowser);

            viewAlbumArt.OnButtonSelected += HandleOnSelectAlbumArtButtonSelected;

            OnViewReady(this);
		}

        private void SetupLocalEventMonitorForKeys()
        {
            var localEventHandler = new LocalEventHandler((ev) => {
                // Make sure this is a key down, and this comes from this window
                if(ev.Type == NSEventType.KeyDown && ev.Window == this.Window)
                {
                    //Console.WriteLine("LocalEventMonitor -- keyDown keycode: {0} locationInWindow: {1}", ev.KeyCode, ev.LocationInWindow);

                    // Check for space bar
                    if(ev.KeyCode == 49)
                    {
                        if(_playerStatus == SSPPlayerState.Stopped)
                            OnPlayerPlay();
                        else if(_playerStatus != SSPPlayerState.Initialized)
                            OnPlayerPause();
                    } 
                }
                return ev; // this processes the event instead of ignoring it
            });

            _eventMonitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.KeyDown, localEventHandler);
        }

        private void RemoveLocalEventMonitorForKeys()
        {
            NSEvent.RemoveMonitor(_eventMonitor);
        }

        private void HidePanels()
        {
            viewMarkerDetails.Hidden = true;
            viewLoopDetails.Hidden = true;
            viewLoopPlayback.Hidden = true;
            viewUpdateLibrary.Hidden = true;
            viewQueue.Hidden = true;

            ShowSegmentDetails(false);
        }

        private void ShowSegmentDetails(bool show)
        {
            viewSegmentDetails.Hidden = !show;
            trackBarSegmentPosition.Enabled = show;
        }

        private void LoadComboBoxes()
        {
            cboSoundFormat.RemoveAllItems();
            cboSoundFormat.AddItem("All");
            cboSoundFormat.AddItem("APE");
            cboSoundFormat.AddItem("FLAC");
            cboSoundFormat.AddItem("OGG");
            cboSoundFormat.AddItem("MP3");
            cboSoundFormat.AddItem("MPC");
            cboSoundFormat.AddItem("WAV");
            cboSoundFormat.AddItem("WV");
        }

        private void LoadCheckBoxes()
        {
            chkSegmentLinkToMarker.OnValueChanged += HandleSegmentLinkToMarkerOnValueChanged;
            lblSegmentLinkToMarker.OnLabelClicked += (label) => {
                chkSegmentLinkToMarker.Value = !chkSegmentLinkToMarker.Value;
                SetSegmentLinkedMarker();
            };
        }

        private void LoadTrackBars()
        {
            faderVolume.Minimum = 0;
            faderVolume.Maximum = 100;
            faderVolume.OnFaderValueChanged += HandleOnFaderValueChanged;
            faderVolume.SetNeedsDisplayInRect(faderVolume.Bounds);

            trackBarPosition.Minimum = 0;
            trackBarPosition.Maximum = 1000;
            trackBarPosition.WheelStepSize = 20;
            trackBarPosition.BlockValueChangeWhenDraggingMouse = true;
            trackBarPosition.OnTrackBarValueChanged += HandleOnTrackBarValueChanged;
            trackBarPosition.OnTrackBarMouseDown += HandleOnTrackBarMouseDown;
            trackBarPosition.OnTrackBarMouseUp += HandleOnTrackBarMouseUp;
            trackBarPosition.SetNeedsDisplayInRect(trackBarPosition.Bounds);

            trackBarTimeShifting.Minimum = 50;
            trackBarTimeShifting.Maximum = 150;
            trackBarTimeShifting.Value = 100;
            trackBarTimeShifting.BlockValueChangeWhenDraggingMouse = true;
            trackBarTimeShifting.OnTrackBarValueChanged += HandleOnTrackBarTimeShiftingValueChanged;
            trackBarTimeShifting.SetNeedsDisplayInRect(trackBarTimeShifting.Bounds);

            trackBarPitchShifting.Minimum = -12;
            trackBarPitchShifting.Maximum = 12;
            trackBarPitchShifting.Value = 0;
            trackBarPitchShifting.BlockValueChangeWhenDraggingMouse = true;
            trackBarPitchShifting.OnTrackBarValueChanged += HandleOnTrackBarPitchShiftingValueChanged;
            trackBarPitchShifting.SetNeedsDisplayInRect(trackBarPitchShifting.Bounds);

            trackBarMarkerPosition.Minimum = 0;
            trackBarMarkerPosition.Maximum = 1000;
            trackBarMarkerPosition.BlockValueChangeWhenDraggingMouse = true;
            trackBarMarkerPosition.OnTrackBarValueChanged += HandleOnTrackBarMarkerPositionValueChanged;
            trackBarMarkerPosition.SetNeedsDisplayInRect(trackBarMarkerPosition.Bounds);

            trackBarSegmentPosition.Minimum = 0;
            trackBarSegmentPosition.Maximum = 1000;
            trackBarSegmentPosition.BlockValueChangeWhenDraggingMouse = true;
            trackBarSegmentPosition.OnTrackBarValueChanged += HandleOnTrackBarSegmentPositionValueChanged;
            trackBarSegmentPosition.SetNeedsDisplayInRect(trackBarSegmentPosition.Bounds);
        }

        private void LoadTreeViews()
        {
            //_libraryBrowserOutlineViewDelegate = new LibraryBrowserOutlineViewDelegate((entity) => { OnTreeNodeSelected(entity); });
            _libraryBrowserOutlineViewDelegate = new LibraryBrowserOutlineViewDelegate(LibraryBrowserTreeNodeSelected);
            outlineLibraryBrowser.Delegate = _libraryBrowserOutlineViewDelegate;
            outlineLibraryBrowser.AllowsMultipleSelection = false;
            outlineLibraryBrowser.DoubleClick += HandleLibraryBrowserDoubleClick;
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("NSOutlineViewItemDidExpandNotification"), ItemDidExpand, outlineLibraryBrowser);
        }

        private void LoadTableViews()
        {
            songGridView.DoubleClick += HandleSongBrowserDoubleClick;
            songGridView.MenuItemClicked += HandleSongGridViewMenuItemClicked;

            playlistView.DoubleClick += HandlePlaylistDoubleClick;
            playlistView.MenuItemClicked += HandlePlaylistMenuItemClicked;

            tableMarkers.WeakDelegate = this;
            tableMarkers.WeakDataSource = this;
            tableMarkers.DoubleClick += HandleMarkersDoubleClick;

            tableLoops.WeakDelegate = this;
            tableLoops.WeakDataSource = this;
            tableLoops.DoubleClick += HandleLoopsDoubleClick;

            tableSegments.WeakDelegate = this;
            tableSegments.WeakDataSource = this;
            tableSegments.RegisterForDraggedTypes(new string[2] { "Segment", "Marker" });
            tableSegments.DoubleClick += HandleSegmentsDoubleClick;
        }

        private void LoadButtons()
        {
            btnTabTimeShifting.IsSelected = true;
            btnTabTimeShifting.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabPitchShifting.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabInfo.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabActions.OnTabButtonSelected += HandleOnTabButtonSelected;
        }

        private void SetTheme()
        {
            viewToolbar.BackgroundColor1 = GlobalTheme.PanelBackgroundColor1;
            viewToolbar.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewLeftHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewLeftHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewRightHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewRightHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewLoopsHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewLoopsHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewLoopDetailsHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewLoopDetailsHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewLoopPlaybackHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewLoopPlaybackHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewSegmentsHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewSegmentsHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewSegmentDetailsHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewSegmentDetailsHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewMarkersHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewMarkersHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewMarkerDetailsHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewMarkerDetailsHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewSongBrowserHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewSongBrowserHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewPlaylistHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewPlaylistHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewUpdateLibraryHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewUpdateLibraryHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewQueueHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewQueueHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;

            btnPlayLoop.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnAddLoop.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnEditLoop.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnRemoveLoop.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnGoToMarker.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnAddMarker.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnEditMarker.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnRemoveMarker.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnAddSegment.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnEditSegment.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnRemoveSegment.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnBackLoop.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnBackLoopPlayback.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnBackSegment.Theme = SessionsButton.ButtonTheme.Toolbar;
            btnBackMarker.Theme = SessionsButton.ButtonTheme.Toolbar;

            lblAlbumTitle.TextColor = NSColor.FromDeviceRgba(196f/255f, 213f/255f, 225f/255f, 1);
            lblSongTitle.TextColor = NSColor.FromDeviceRgba(171f/255f, 186f/255f, 196f/255f, 1);
            lblSongPath.TextColor = NSColor.FromDeviceRgba(97f/255f, 122f/255f, 140f/255f, 1);

            viewSongPosition.IsHeaderVisible = true;
            viewSongPosition.HeaderHeight = 26;
            viewVolume.IsHeaderVisible = true;

            lblArtistName.Font = NSFont.FromFontName("Roboto Thin", 24);
            lblAlbumTitle.Font = NSFont.FromFontName("Roboto Light", 20);
            lblSongTitle.Font = NSFont.FromFontName("Roboto Light", 17);
            lblSongPath.Font = NSFont.FromFontName("Roboto Light", 12);

            var textFont = NSFont.FromFontName("Roboto", 11f);
            lblSampleRate.Font = textFont;
            lblBitrate.Font = textFont;
            lblMonoStereo.Font = textFont;
            lblFileType.Font = textFont;
            lblBitsPerSample.Font = textFont;
            lblFilterBySoundFormat.Font = textFont;
            lblYear.Font = textFont;
            lblGenre.Font = textFont;
            lblFileSize.Font = textFont;
            lblPlayCount.Font = textFont;
            lblLastPlayed.Font = textFont;
            lblQueueDetails.Font = textFont;
            lblSegmentPosition.Font = textFont;

            var titleFont = NSFont.FromFontName("Roboto", 13f);
            lblTitleLibraryBrowser.Font = titleFont;
            lblTitleCurrentSong.Font = titleFont;
            lblTitleLoops.Font = titleFont;
            lblTitleLoopDetails.Font = titleFont;
            lblTitleLoopPlayback.Font = titleFont;
            lblTitleSegments.Font = titleFont;
            lblTitleSegmentDetails.Font = titleFont;
            lblTitleMarkers.Font = titleFont;
            lblTitleMarkerDetails.Font = titleFont;
            lblTitleSongBrowser.Font = titleFont;
            lblTitlePlaylist.Font = titleFont;
            lblTitleUpdateLibrary.Font = titleFont;
            lblTitleQueue.Font = titleFont;

            var secondaryTextFont = NSFont.FromFontName("Roboto", 12f);
            lblUpdateLibraryStatus.Font = secondaryTextFont;
            lblSearchWeb.Font = secondaryTextFont;
            lblSubtitleSongPosition.Font = secondaryTextFont;
            lblSubtitleVolume.Font = secondaryTextFont;
            lblCurrentLoop.Font = secondaryTextFont;
            lblCurrentSegment.Font = secondaryTextFont;
            lblDetectedTempoValue.Font = secondaryTextFont;
            lblReferenceTempoValue.Font = secondaryTextFont;
            txtCurrentTempoValue.Font = secondaryTextFont;
            lblReferenceKeyValue.Font = secondaryTextFont;
            lblNewKeyValue.Font = secondaryTextFont;
            txtIntervalValue.Font = secondaryTextFont;
            lblSegmentLinkToMarker.Font = secondaryTextFont;

            var valueFont = NSFont.FromFontName("Roboto Light", 12f);
            lblVolume.Font = valueFont;
            lblMarkerName.Font = valueFont;
            lblMarkerPosition.Font = valueFont;
            lblSegmentPosition.Font = valueFont;
            lblDetectedTempo.Font = valueFont;
            lblCurrentTempo.Font = valueFont;
            lblReferenceTempo.Font = valueFont;
            lblReferenceKey.Font = valueFont;
            lblInterval.Font = valueFont;
            lblNewKey.Font = valueFont;
            lblPlaylistIndexCount.Font = valueFont;

            var largePositionFont = NSFont.FromFontName("Roboto Light", 15f);
            lblPosition.Font = largePositionFont;
            lblLength.Font = largePositionFont;
            lblLoopPosition.Font = largePositionFont;

            var textBoxFont = NSFont.FromFontName("Roboto", 12f);
            txtMarkerName.Font = textBoxFont;
            searchSongBrowser.Font = textBoxFont;
            txtLoopName.Font = textBoxFont;

            lblMarkerPositionValue.Font = NSFont.FromFontName("Roboto Light", 14f);
            lblSegmentPositionValue.Font = NSFont.FromFontName("Roboto Light", 14f);
            lblLoopName.Font = NSFont.FromFontName("Roboto Light", 11);
            cboSoundFormat.Font = NSFont.FromFontName("Roboto", 11);

            // Set cell fonts for Library Browser
            NSTableColumn columnText = outlineLibraryBrowser.FindTableColumn(new NSString("columnText"));
            columnText.DataCell.Font = NSFont.FromFontName("Roboto", 11f);
        }

        /// <summary>
        /// Loads the image resources in all controls.
        /// </summary>
        private void LoadImages()
        {
            btnAddLoop.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_add");
            btnAddMarker.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_add");
            btnAddSegment.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_add");
            btnEditLoop.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_edit");
            btnEditMarker.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_edit");
            btnEditSegment.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_edit");
            btnRemoveLoop.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_delete");
            btnRemoveMarker.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_delete");
            btnRemoveSegment.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_delete");
            btnPlayLoop.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_play");
            btnGoToMarker.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_goto");
            btnBackLoop.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_back");
            btnBackLoopPlayback.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_back");
            btnBackMarker.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_back");
            btnBackSegment.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_back");
            btnPunchInMarker.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_punch_in");
            btnPunchInSegment.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_punch_in");

            btnToolbarPlayPause.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_play");
            btnToolbarPrevious.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_previous");
            btnToolbarNext.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_next");
            btnToolbarRepeat.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_repeat_off");
            btnToolbarShuffle.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_shuffle_off");
            btnToolbarEffects.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_equalizer");
            btnToolbarSync.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_devices");
            btnToolbarSettings.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_preferences");

            btnIncrementTimeShifting.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_roundbutton_add");
            btnDecrementTimeShifting.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_roundbutton_minus");
            btnResetTimeShifting.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_roundbutton_reset");
            btnIncrementPitchShifting.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_roundbutton_add");
            btnDecrementPitchShifting.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_roundbutton_minus");
            btnResetPitchShifting.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_roundbutton_reset");
        }

        private void EnableMarkerButtons(bool enabled)
        {
            btnGoToMarker.Enabled = enabled;
            btnEditMarker.Enabled = enabled;
            btnRemoveMarker.Enabled = enabled;
        }

        private void EnableLoopButtons(bool enabled)
        {
            btnPlayLoop.Enabled = enabled;
            btnEditLoop.Enabled = enabled;
            btnRemoveLoop.Enabled = enabled;
        }

        private void EnableSegmentButtons(bool enabled)
        {
            btnEditSegment.Enabled = enabled;
            btnRemoveSegment.Enabled = enabled;
        }

        private void SplitViewFrameDidChangeNotification(NSNotification notification)
        {
            const float headerHeight = 59;
            var frame = scrollViewLibraryBrowser.Frame;
            frame.Y = 0;
            frame.Height = splitMain.Frame.Height - headerHeight;
            scrollViewLibraryBrowser.Frame = frame;
            //Console.WriteLine("MainWindow - SplitViewFrameDidChangeNotification - splitMain.Frame: {0} viewLibraryBrowser.Frame: {1} scrollViewLibraryBrowser.Frame: {2}", splitMain.Frame, viewLibraryBrowser.Frame, scrollViewLibraryBrowser.Frame);
        }

        private void HandleOnTabButtonSelected(SessionsTabButton button)
        {
            btnTabTimeShifting.IsSelected = button == btnTabTimeShifting;
            btnTabPitchShifting.IsSelected = button == btnTabPitchShifting;
            btnTabInfo.IsSelected = button == btnTabInfo;
            btnTabActions.IsSelected = button == btnTabActions;

            viewTimeShifting.Hidden = button != btnTabTimeShifting;
            viewPitchShifting.Hidden = button != btnTabPitchShifting;
            viewInformation.Hidden = button != btnTabInfo;
            viewActions.Hidden = button != btnTabActions;
        }

        private void HandleSongGridViewMenuItemClicked(SessionsSongGridView.MenuItemType menuItemType)
        {
            if (songGridView.SelectedAudioFiles.Count == 0)
                return;

            switch (menuItemType)
            {
                case SessionsSongGridView.MenuItemType.PlaySongs:
                    var audioFile = songGridView.SelectedAudioFiles[0];
                    OnTableRowDoubleClicked(audioFile);
                    break;
                case SessionsSongGridView.MenuItemType.AddToPlaylist:
                    var audioFiles = new List<AudioFile>();
                    foreach (var item in songGridView.SelectedAudioFiles)
                        audioFiles.Add(item);
                    OnSongBrowserAddToPlaylist(audioFiles);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandlePlaylistMenuItemClicked(SessionsPlaylistListView.MenuItemType menuItemType)
        {
            if (playlistView.SelectedAudioFiles.Count == 0)
                return;

            switch (menuItemType)
            {
                case SessionsPlaylistListView.MenuItemType.RemoveSongs:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandlePlaylistDoubleClick(object sender, EventArgs e)
        {
            if (playlistView.SelectedAudioFiles.Count == 0)
                return;

            var audioFile = playlistView.SelectedAudioFiles[0];
            OnTableRowDoubleClicked(audioFile);
        }

        private void LibraryBrowserTreeNodeSelected(LibraryBrowserEntity entity)
        {
            _currentLibraryBrowserEntity = entity;

            menuPlay.Enabled = entity != null;
            menuAddToPlaylist.Enabled = entity != null;
            menuAddToQueue.Enabled = entity != null;
            menuRemoveFromLibrary.Enabled = entity != null;
            menuDeleteFromHardDisk.Enabled = entity != null;

            if(entity != null)
                OnTreeNodeSelected(entity);
        }

		partial void actionAddFilesToLibrary(NSObject sender)
		{
			IEnumerable<string> filePaths = null;
			using(NSOpenPanel openPanel = new NSOpenPanel())
			{
				openPanel.CanChooseDirectories = false;
				openPanel.CanChooseFiles = true;
				openPanel.ReleasedWhenClosed = true;
				openPanel.AllowsMultipleSelection = true;
                openPanel.AllowedFileTypes = new string[]{ "FLAC", "MP3", "OGG", "WAV", "MPC", "WV", "APE" };
                openPanel.Title = "Please select audio files to add to the library";
				openPanel.Prompt = "Add files to library";

                // Check for cancel
                if(openPanel.RunModal() == 0)
                    return;

				filePaths = openPanel.Urls.Select(x => x.Path);
			}

			if(filePaths != null && filePaths.Count() > 0)
                OnAddFilesToLibrary(filePaths.ToList());
		}

		partial void actionAddFolderLibrary(NSObject sender)
		{
			string folderPath = string.Empty;
			using(NSOpenPanel openPanel = new NSOpenPanel())
			{
				openPanel.CanChooseDirectories = true;
				openPanel.CanChooseFiles = false;
				openPanel.ReleasedWhenClosed = true;
				openPanel.AllowsMultipleSelection = false;
				openPanel.Title = "Please select a folder to add to the library";
				openPanel.Prompt = "Add folder to library";	

                // Check for cancel
                if(openPanel.RunModal() == 0)
                    return;

				folderPath = openPanel.Url.Path;
			}

			if(!String.IsNullOrEmpty(folderPath))
                OnAddFolderToLibrary(folderPath);
		}

		partial void actionOpenAudioFiles(NSObject sender)
		{
			IEnumerable<string> filePaths = null;
			using(NSOpenPanel openPanel = new NSOpenPanel())
			{
				openPanel.CanChooseDirectories = false;
				openPanel.CanChooseFiles = true;
				openPanel.ReleasedWhenClosed = true;
				openPanel.AllowsMultipleSelection = true;
				openPanel.AllowedFileTypes = new string[]{ "FLAC", "MP3", "OGG", "WAV", "MPC", "WV" };
				openPanel.Title = "Please select audio files to play";
				openPanel.Prompt = "Add to playlist";	

                // Check for cancel
                if(openPanel.RunModal() == 0)
                    return;

				filePaths = openPanel.Urls.Select(x => x.Path);
			}

			if(filePaths != null && filePaths.Count() > 0)
                OnPlayerPlayFiles(filePaths);
		}            

        partial void actionSoundFormatChanged(NSObject sender)
        {
            AudioFileFormat format;
            Enum.TryParse<AudioFileFormat>(cboSoundFormat.TitleOfSelectedItem, out format);
            OnAudioFileFormatFilterChanged(format);
        }

		partial void actionPlay(NSObject sender)
		{
            OnPlayerPause();
		}

		partial void actionPrevious(NSObject sender)
		{
            OnPlayerPrevious();
        }

		partial void actionNext(NSObject sender)
		{
            OnPlayerNext();
        }

		partial void actionRepeatType(NSObject sender)
		{
            OnPlayerRepeat();
		}

        partial void actionShuffle(NSObject sender)
        {
            OnPlayerShuffle();        
        }

		partial void actionOpenEffectsWindow(NSObject sender)
		{
            OnOpenEffectsWindow();
        }

		partial void actionOpenPreferencesWindow(NSObject sender)
		{
            OnOpenPreferencesWindow();
        }

        partial void actionOpenSyncWindow(NSObject sender)
        {
            OnOpenSyncWindow();
        }

        partial void actionEditSongMetadata(NSObject sender)
        {
            OnEditSongMetadata();
        }

        private void HandleOnFaderValueChanged(object sender, EventArgs e)
        {
            OnPlayerSetVolume(faderVolume.Value);
        }       

        private void HandleOnTrackBarTimeShiftingValueChanged()
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            if (OnSetTimeShifting != null)
                OnSetTimeShifting(trackBarTimeShifting.Value);
        }

        private void HandleOnTrackBarPitchShiftingValueChanged()
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            if (OnSetInterval != null)
                OnSetInterval(trackBarPitchShifting.Value);
        }

        private void HandleOnTrackBarMarkerPositionValueChanged()
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            if (OnChangePositionMarkerDetails != null)
                OnChangePositionMarkerDetails((float)trackBarMarkerPosition.Value / 1000f);
        }

        private void HandleOnTrackBarSegmentPositionValueChanged()
        {
            // The value of the slider is changed at the startup of the app and the view is not ready
            ChangePositionSegmentDetails((float)trackBarSegmentPosition.Value / 1000f);
        }

        private void ChangePositionSegmentDetails(float percentage)
        {
            chkSegmentLinkToMarker.Value = false;
            comboSegmentMarker.Hidden = true;
//            if (OnChangePositionSegmentDetails != null)
//                OnChangePositionSegmentDetails(percentage);
        }

        partial void actionPlayLoop(NSObject sender)
        {
            viewLoops.Hidden = true;
            viewLoopPlayback.Hidden = false;
        }

        partial void actionAddLoop(NSObject sender)
        {
            OnAddLoop();
            viewLoopDetails.Hidden = false;
            viewLoops.Hidden = true;
            ShowSegmentDetails(false);
        }

        partial void actionEditLoop(NSObject sender)
        {           
            EditLoop();
        }

        private void EditLoop()
        {
            if(tableLoops.SelectedRow < 0 || tableLoops.SelectedRow >= _loops.Count)
                return;

            OnEditLoop(_loops[tableLoops.SelectedRow]);
            viewLoopDetails.Hidden = false;
            viewLoopPlayback.Hidden = true;
            viewSegmentDetails.Hidden = true;
            viewLoops.Hidden = true;
            ShowSegmentDetails(false);
        }

        partial void actionRemoveLoop(NSObject sender)
        {
            if(tableLoops.SelectedRow < 0 || tableLoops.SelectedRow >= _loops.Count)
                return;

            using(var alert = new NSAlert())
            {
                alert.MessageText = "Loop will be removed";
                alert.InformativeText = "Are you sure you wish to remove this loop?";
                alert.AlertStyle = NSAlertStyle.Warning;
                var btnOK = alert.AddButton("OK");
                btnOK.Activated += (sender2, e2) => {
                    NSApplication.SharedApplication.StopModal();
                    OnDeleteLoop(_loops[tableLoops.SelectedRow]);
                    EnableLoopButtons(false);
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => {
                    NSApplication.SharedApplication.StopModal();
                };
                alert.RunModal();
            }
        }

        partial void actionBackLoopPlayback(NSObject sender)
        {
            viewLoops.Hidden = false;
            viewLoopPlayback.Hidden = true;
        }

        partial void actionPreviousLoop(NSObject sender)
        {
        }

        partial void actionNextLoop(NSObject sender)
        {
        }

        partial void actionPreviousSegment(NSObject sender)
        {
        }

        partial void actionNextSegment(NSObject sender)
        {
        }

        partial void actionAddSegment(NSObject sender)
        {
            OnAddSegment();
            viewLoopDetails.Hidden = true;
            viewLoops.Hidden = true;
            ShowSegmentDetails(true);
        }

        partial void actionEditSegment(NSObject sender)
        {
            EditSegment();
        }

        private void EditSegment()
        {
//            if(tableSegments.SelectedRow < 0 || tableSegments.SelectedRow >= _currentLoop.Segments.Count)
//                return;
//
//            OnEditSegment(_currentLoop.Segments[tableSegments.SelectedRow]);
//            viewLoopDetails.Hidden = true;
//            viewLoops.Hidden = true;
//            ShowSegmentDetails(true);
        }

        partial void actionRemoveSegment(NSObject sender)
        {
//            if(tableSegments.SelectedRow < 0 || tableSegments.SelectedRow >= _currentLoop.Segments.Count)
//                return;
//
//            using(var alert = new NSAlert())
//            {
//                alert.MessageText = "Segment will be removed";
//                alert.InformativeText = "Are you sure you wish to remove this segment?";
//                alert.AlertStyle = NSAlertStyle.Warning;
//                var btnOK = alert.AddButton("OK");
//                btnOK.Activated += (sender2, e2) => {
//                    NSApplication.SharedApplication.StopModal();
//                    OnDeleteSegment(_currentLoop.Segments[tableSegments.SelectedRow]);
//                    EnableSegmentButtons(false);
//                };
//                var btnCancel = alert.AddButton("Cancel");
//                btnCancel.Activated += (sender3, e3) => {
//                    NSApplication.SharedApplication.StopModal();
//                };
//                alert.RunModal();
//            }
        }

        partial void actionBackSegmentDetails(NSObject sender)
        {
//            viewLoopDetails.Hidden = false;
//            ShowSegmentDetails(false);
//            viewLoops.Hidden = true;
//
//            _currentSegment.MarkerId = Guid.Empty;
//            if(chkSegmentLinkToMarker.Value && comboSegmentMarker.IndexOfSelectedItem >= 0)
//                _currentSegment.MarkerId = _segmentMarkers[comboSegmentMarker.IndexOfSelectedItem].MarkerId;
//
//            OnUpdateSegmentDetails(_currentSegment);
//            _currentSegment = null;
        }

        partial void actionPunchInSegment(NSObject sender)
        {
//            if(_currentSegment == null)
//                return;
//
//            chkSegmentLinkToMarker.Value = false;
//            comboSegmentMarker.Hidden = true;
//            OnPunchInPositionSegmentDetails();
        }

        partial void actionSegmentMarker(NSObject sender)
        {
            SetSegmentLinkedMarker();
        }

        private void HandleSegmentLinkToMarkerOnValueChanged(SessionsCheckBoxView checkBox)
        {
            SetSegmentLinkedMarker();
        }

        private void SetSegmentLinkedMarker()
        {
//            if (_segmentMarkers.Count == 0)
//            {
//                chkSegmentLinkToMarker.Value = false;
//                CocoaHelper.ShowAlert("Cannot link to marker", "There are no markers to link to this segment.", NSAlertStyle.Critical);
//                return;
//            }
//
//            comboSegmentMarker.Hidden = !chkSegmentLinkToMarker.Value;
//            if (chkSegmentLinkToMarker.Value)
//            {
//                var marker = _segmentMarkers[comboSegmentMarker.IndexOfSelectedItem];
//                OnLinkToMarkerSegmentDetails(marker.MarkerId);
//            }
//            else
//            {
//                OnLinkToMarkerSegmentDetails(Guid.Empty);
//            }
        }

//        private void ScrollViewWaveForm_OnChangingSegmentPosition(Segment segment, float positionPercentage)
//        {
//            if (!viewLoopDetails.Hidden)
//            {
//                OnChangingSegmentPosition(segment, positionPercentage);
//            }
//            else if (!viewSegmentDetails.Hidden)
//            {
//                if (segment.SegmentId == _currentSegment.SegmentId)
//                    ChangePositionSegmentDetails(positionPercentage);
//            }
//        }
//
//        private void ScrollViewWaveForm_OnChangedSegmentPosition(Segment segment, float positionPercentage)
//        {
//            if (!viewLoopDetails.Hidden)
//            {
//                OnChangedSegmentPosition(segment, positionPercentage);
//            }
//            else if (!viewSegmentDetails.Hidden)
//            {
//                if(segment.SegmentId == _currentSegment.SegmentId)
//                    ChangePositionSegmentDetails(positionPercentage);
//            }
//        }

        partial void actionGoToMarker(NSObject sender)
        {
            if(tableMarkers.SelectedRow == -1)
                return;

            OnSelectMarker(_markers[tableMarkers.SelectedRow]);
        }

        partial void actionAddMarker(NSObject sender)
        {
            OnAddMarkerWithTemplate(MarkerTemplateNameType.Verse);
        }

        partial void actionEditMarker(NSObject sender)
        {
            if(tableMarkers.SelectedRow == -1)
                return;

            OnEditMarker(_markers[tableMarkers.SelectedRow]);
            viewMarkerDetails.Hidden = false;
            viewMarkers.Hidden = true;
        }

        partial void actionRemoveMarker(NSObject sender)
        {
            if(tableMarkers.SelectedRow < 0)
                return;

            using(var alert = new NSAlert())
            {
                alert.MessageText = "Marker will be removed";
                alert.InformativeText = "Are you sure you wish to remove this marker?";
                alert.AlertStyle = NSAlertStyle.Warning;
                var btnOK = alert.AddButton("OK");
                btnOK.Activated += (sender2, e2) => {
                    NSApplication.SharedApplication.StopModal();
                    OnDeleteMarker(_markers[tableMarkers.SelectedRow]);
                    EnableMarkerButtons(false);
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => {
                    NSApplication.SharedApplication.StopModal();
                };
                alert.RunModal();
            }
        }

        partial void actionBackLoopDetails(NSObject sender)
        {
            viewLoopDetails.Hidden = true;
            viewLoops.Hidden = false;
            ShowSegmentDetails(false);

            _currentLoop.Name = txtLoopName.StringValue;
            OnUpdateLoopDetails(_currentLoop);
            _currentLoop = null;
            waveFormScrollView.SetLoop(null);
        }

        partial void actionBackMarkerDetails(NSObject sender)
        {
            viewMarkerDetails.Hidden = true;
            viewMarkers.Hidden = false;

            _currentMarker.Name = txtMarkerName.StringValue;
            OnUpdateMarkerDetails(_currentMarker);

            _currentMarker = null;
            waveFormScrollView.SetActiveMarker(Guid.Empty);
        }

        partial void actionPunchInMarker(NSObject sender)
        {
            if(_currentMarker == null)
                return;

            OnPunchInMarkerDetails();
        }

        partial void actionContextualMenuPlay(NSObject sender)
        {
            if(outlineLibraryBrowser.SelectedRow < 0 || _currentLibraryBrowserEntity == null)
                return;

            OnTreeNodeDoubleClicked(_currentLibraryBrowserEntity);
        }

        partial void actionAddToPlaylist(NSObject sender)
        {
            if(outlineLibraryBrowser.SelectedRow < 0 || _currentLibraryBrowserEntity == null)
                return;

            OnAddToPlaylist(_currentLibraryBrowserEntity);
        }

        partial void actionRemoveFromLibrary(NSObject sender)
        {
            if(outlineLibraryBrowser.SelectedRow < 0 || _currentLibraryBrowserEntity == null)
                return;

            using(var alert = new NSAlert())
            {
                alert.MessageText = "Audio files will be removed from library";
                alert.InformativeText = "Are you sure you wish to remove these audio files from your library?\nThis does not delete the audio files from your hard disk.";
                alert.AlertStyle = NSAlertStyle.Warning;
                var btnOK = alert.AddButton("OK");
                btnOK.Activated += (sender2, e2) => {
                    NSApplication.SharedApplication.StopModal();
                    OnRemoveFromLibrary(_currentLibraryBrowserEntity);
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => {
                    NSApplication.SharedApplication.StopModal();
                };
                alert.RunModal();
            }
        }

        partial void actionDeleteFromHardDisk(NSObject sender)
        {
            if(outlineLibraryBrowser.SelectedRow < 0 || _currentLibraryBrowserEntity == null)
                return;

            using(var alert = new NSAlert())
            {
                alert.MessageText = "Audio files will be deleted from hard disk";
                alert.InformativeText = "Are you sure you wish to delete these audio files from your hard disk?\nWARNING: This operation CANNOT be undone!";
                alert.AlertStyle = NSAlertStyle.Warning;
                var btnOK = alert.AddButton("OK");
                btnOK.Activated += (sender2, e2) => {
                    NSApplication.SharedApplication.StopModal();
                    OnDeleteFromHardDisk(_currentLibraryBrowserEntity);
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => {
                    NSApplication.SharedApplication.StopModal();
                };
                alert.RunModal();
            }
        }

        partial void actionTabActions(NSObject sender)
        {
        }

        partial void actionTabInfo(NSObject sender)
        {
        }

        partial void actionTabPitchShifting(NSObject sender)
        {
        }

        partial void actionTabTimeShifting(NSObject sender)
        {
        }

        partial void actionUseTempo(NSObject sender)
        {
            OnUseDetectedTempo();
        }

        partial void actionDecrementTimeShifting(NSObject sender)
        {
            OnDecrementTempo();
        }

        partial void actionIncrementTimeShifting(NSObject sender)
        {
            OnIncrementTempo();
        }

        partial void actionResetTimeShifting(NSObject sender)
        {
            OnResetTimeShifting();
        }

        partial void actionChangeKey(NSObject sender)
        {
            OnChangeKey(trackBarPitchShifting.Value);
        }

        partial void actionDecrementPitchShifting(NSObject sender)
        {
            OnDecrementInterval();
        }

        partial void actionIncrementPitchShfiting(NSObject sender)
        {
            OnIncrementInterval();
        } 

        partial void actionResetPitchShifting(NSObject sender)
        {
            OnResetInterval();
        }

        partial void actionSearchGuitarTabs(NSObject sender)
        {
            try
            {
                if(!string.IsNullOrEmpty(lblArtistName.StringValue))
                    NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(lblArtistName.StringValue) + "+" + HttpUtility.UrlEncode(lblSongTitle.StringValue) + "+guitar+tab"));
            }
            catch (Exception ex)
            {
                CocoaHelper.ShowAlert("Error searching for guitar tabs", string.Format("An error occured while searching for guitar tabs: {0}", ex), NSAlertStyle.Critical);
            }
        }

        partial void actionSearchBassTabs(NSObject sender)
        {           
            try
            {
                if(!string.IsNullOrEmpty(lblArtistName.StringValue))
                    NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(lblArtistName.StringValue) + "+" + HttpUtility.UrlEncode(lblSongTitle.StringValue) + "+bass+tab"));
            }
            catch (Exception ex)
            {
                CocoaHelper.ShowAlert("Error searching for bass tabs", string.Format("An error occured while searching for bass tabs: {0}", ex), NSAlertStyle.Critical);
            }
        }

        partial void actionSearchLyrics(NSObject sender)
        {
            try
            {
                if(!string.IsNullOrEmpty(lblArtistName.StringValue))
                    NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl("http://www.google.ca/search?q=" + HttpUtility.UrlEncode(lblArtistName.StringValue) + "+" + HttpUtility.UrlEncode(lblSongTitle.StringValue) + "+lyrics"));
            }
            catch (Exception ex)
            {
                CocoaHelper.ShowAlert("Error searching for lyrics", string.Format("An error occured while searching for lyrics: {0}", ex), NSAlertStyle.Critical);
            }
        }

        partial void actionAddToQueue(NSObject sender)
        {
        }

        partial void actionDeleteQueue(NSObject sender)
        {
        }

        partial void actionPlayQueue(NSObject sender)
        {
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
            trackBarPosition.Value = (int)(position * 1000);
            lblPosition.StringValue = requestedPosition.Str;
        }

        private void HandleOnTrackBarValueChanged()
        {
            if (OnPlayerRequestPosition == null || !_isPlayerPositionChanging || _isScrollViewWaveFormChangingSecondaryPosition)
                return;

            var position = OnPlayerRequestPosition((float)trackBarPosition.Value/1000f);
            //Console.WriteLine("HandleOnTrackBarValueChanged - trackBarPosition.Value: {0} position: {1}", trackBarPosition.Value, position.Position);
            lblPosition.StringValue = position.Str;
            waveFormScrollView.SetSecondaryPosition(position.Bytes);
        }

        private void HandleOnTrackBarMouseDown()
        {
            _isPlayerPositionChanging = true;
            waveFormScrollView.ShowSecondaryPosition(true);
        }

        private void HandleOnTrackBarMouseUp()
        {
            _isPlayerPositionChanging = false;
            OnPlayerSetPosition((float) trackBarPosition.Value / 10f);
            waveFormScrollView.ShowSecondaryPosition(false);
        }

        private void HandleOnSelectAlbumArtButtonSelected(SessionsRoundButton button)
        {
            OnOpenSelectAlbumArt();
        }

        [Export ("controlTextDidChange")]
        private void SearchTextDidChange(NSNotification notification)
        {
            OnSearchTerms(searchSongBrowser.StringValue);
        }

        [Export ("numberOfRowsInTableView:")]
        public int GetRowCount(NSTableView tableView)
        {
            if(tableView.Identifier == "tableMarkers")
                return _markers == null ? 0 : _markers.Count;
            else if(tableView.Identifier == "tableLoops")
                return _loops == null ? 0 : _loops.Count;
//            else if(tableView.Identifier == "tableSegments")
//                return _currentLoop == null ? 0 : _currentLoop.Segments.Count;

            return 0;
        }

        [Export ("tableView:dataCellForTableColumn:row:")]
        public NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            return new NSString();
        }

        [Export ("tableView:viewForTableColumn:row:")]
        public NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            SessionsTableCellView view;           
            view = (SessionsTableCellView)tableView.MakeView(tableColumn.Identifier.ToString().Replace("column", "cell"), this);
            view.TextField.Font = NSFont.FromFontName("Roboto", 11);

            bool adjustXPadding = false;
            if (tableView.Identifier == "tableMarkers")
            {
                adjustXPadding = tableMarkers.FindColumn(new NSString(tableColumn.Identifier)) > 0;

                if (tableColumn.Identifier.ToString() == "columnMarkerIndex")
                {
                    view.IndexLabel.StringValue = string.Format("{0}", Conversion.IndexToLetter(row));
                    view.IndexBackground.BackgroundColor1 = new CGColor(1, 0, 0);
                    view.IndexBackground.BackgroundColor2 = new CGColor(1, 0, 0);
                }
                else if (tableColumn.Identifier.ToString() == "columnMarkerName")
                    view.TextField.StringValue = _markers[row].Name;
                else if (tableColumn.Identifier.ToString() == "columnMarkerPosition")
                    view.TextField.StringValue = _markers[row].Position;
                else
                    view.TextField.StringValue = string.Empty;
            } 
            else if (tableView.Identifier == "tableLoops")
            {
                adjustXPadding = tableLoops.FindColumn(new NSString(tableColumn.Identifier)) > 0;

                if (tableColumn.Identifier.ToString() == "columnLoopIndex")
                {
                    view.IndexLabel.StringValue = string.Format("{0}", Conversion.IndexToLetter(row));
                    view.IndexBackground.BackgroundColor1 = new CGColor(0, 0, 1);
                    view.IndexBackground.BackgroundColor2 = new CGColor(0, 0, 1);
                }
                else if (tableColumn.Identifier.ToString() == "columnLoopName")
                    view.TextField.StringValue = _loops[row].Name;
//                else if (tableColumn.Identifier.ToString() == "columnLoopSegments")
//                    view.TextField.StringValue = _loops[row].Segments.Count.ToString();
                else if (tableColumn.Identifier.ToString() == "columnLoopTotalLength")
                    view.TextField.StringValue = "0:00";
                else
                    view.TextField.StringValue = string.Empty;
            }
            else if (tableView.Identifier == "tableSegments")
            {
                adjustXPadding = tableSegments.FindColumn(new NSString(tableColumn.Identifier)) > 0;

                if (tableColumn.Identifier.ToString() == "columnSegmentIndex")
                {               
                    view.IndexLabel.StringValue = string.Format("{0}", row + 1);
                    view.IndexBackground.BackgroundColor1 = new CGColor(0, 0, 1);
                    view.IndexBackground.BackgroundColor2 = new CGColor(0, 0, 1);
                }
                else if (tableColumn.Identifier.ToString() == "columnSegmentMarker")
                {
//                    string markerName = string.Empty;
//                    if (_currentLoop.Segments[row].MarkerId != Guid.Empty)
//                    {
//                        var marker = _markers.FirstOrDefault(x => x.MarkerId == _currentLoop.Segments[row].MarkerId);
//                        if (marker != null)
//                            markerName = marker.Name;
//                    }
//                    view.TextField.StringValue = markerName;
                }
//                else if (tableColumn.Identifier.ToString() == "columnSegmentPosition")
//                    view.TextField.StringValue = _currentLoop.Segments[row].Position;
//                else
//                    view.TextField.StringValue = string.Empty;
            }

            view.TextField.Frame = new RectangleF(adjustXPadding ? -2 : 0, -2, view.Frame.Width, view.Frame.Height);

            if (tableColumn.Identifier.ToString() == "columnMarkerIndex" ||
                tableColumn.Identifier.ToString() == "columnLoopIndex" ||
                tableColumn.Identifier.ToString() == "columnSegmentIndex")
            {
                view.SetTheme(SessionsTableCellView.CellTheme.Index);
            }
            else
            {
                view.SetTheme(SessionsTableCellView.CellTheme.Normal);
            }
                
            return view;
        }

        [Export ("tableViewSelectionDidChange:")]
        public void SelectionDidChange(NSNotification notification)
        {
            if(notification.Object == tableMarkers)
                EnableMarkerButtons(tableMarkers.SelectedRow >= 0);
            else if(notification.Object == tableLoops)
                EnableLoopButtons(tableLoops.SelectedRow >= 0);
            else if(notification.Object == tableSegments)
                EnableSegmentButtons(tableSegments.SelectedRow >= 0);
        }

        [Export ("tableView:acceptDrop:row:dropOperation:")]
        public bool AcceptDropForRow(NSTableView tableView, NSDraggingInfo info, int row, NSTableViewDropOperation operation)
        {
            NSData data = null;
            string dataType = string.Empty;
            if (info.DraggingSource == tableSegments)
                dataType = "Segment";
            else if (info.DraggingSource == tableMarkers)
                dataType = "Marker";

            data = info.DraggingPasteboard.GetDataForType(dataType);
            byte[] dataBytes = data.ToArray();
            byte originRow = dataBytes[0];

            Console.WriteLine(">>> AcceptDropForRow - originRow: {0} newRow: {1}", originRow, row);
            if (operation == NSTableViewDropOperation.Above)
            {
//                if (info.DraggingSource == tableSegments)
//                    OnChangeSegmentOrder(_currentLoop.Segments[originRow], row);
//                else if (info.DraggingSource == tableMarkers)
//                    OnAddSegmentFromMarker(_markers[originRow].MarkerId, row);
            }
            else if (operation == NSTableViewDropOperation.On)
            {
//                if (info.DraggingSource == tableMarkers)
//                    OnLinkSegmentToMarker(_currentLoop.Segments[row], _markers[originRow].MarkerId);
            }

            return true;
        }

        [Export ("tableView:validateDrop:proposedRow:proposedDropOperation:")]
        public NSDragOperation ValidateDropForRow(NSTableView tableView, NSDraggingInfo info, int row, NSTableViewDropOperation operation)
        {
            Console.WriteLine(">>> ValidateDropForRow - row: {0} operation: {1}", row, operation);
            if (info.DraggingSource == tableSegments)
            {
                // Do not allow dragging on segments, only allow reordering of rows
                return operation == NSTableViewDropOperation.Above ? NSDragOperation.All : NSDragOperation.None;
            }
            else if (info.DraggingSource == tableMarkers)
            {
                // Allow dragging on segments to link an existing segment to a marker
                // Allow dragging above segments to create a new segment from a marke
                return NSDragOperation.All;
            }

            return NSDragOperation.None;
        }

        [Export ("tableView:writeRowsWithIndexes:toPasteboard:")]
        public bool WriteRowsWithIndexesToPasteboard(NSTableView tableView, NSIndexSet rowIndexes, NSPasteboard pboard)
        {
            //Console.WriteLine(">>> WriteRowsWithIndexesToPasteboard");
            string dataType = string.Empty;
            if (tableView == tableSegments)
                dataType = "Segment";
            else if (tableView == tableMarkers)
                dataType = "Marker";

            pboard.DeclareTypes(new string[1] { dataType }, this);
            byte index = (byte)rowIndexes.Last();
            var data = NSData.FromArray(new byte[1] { index });
            pboard.SetDataForType(data, dataType);
            return true;
        }

        [Export ("outlineViewItemDidExpand")]
        public void ItemDidExpand(NSNotification notification)
        {
            // Check for dummy nodes
            var item = (LibraryBrowserItem)notification.UserInfo["NSObject"];
            if (item.SubItems.Count > 0 && item.SubItems[0].Entity.EntityType == LibraryBrowserEntityType.Dummy)
            {
                IEnumerable<LibraryBrowserEntity> entities = OnTreeNodeExpandable(item.Entity);
                //Console.WriteLine("MainWindowController - ItemDidExpand - dummy node - entities.Count: {0}", entities.Count());

                // Clear subitems (dummy node) and fill with actual nodes
                item.SubItems.Clear();
                foreach (LibraryBrowserEntity entity in entities)
                    item.SubItems.Add(new LibraryBrowserItem(entity));

                outlineLibraryBrowser.ReloadData();
            }
        }

        private void HandleMarkersDoubleClick(object sender, EventArgs e)
        {
            if (tableMarkers.SelectedRow == -1)
                return;

            OnSelectMarker(_markers[tableMarkers.SelectedRow]);
        }

        private void HandleLoopsDoubleClick(object sender, EventArgs e)
        {
            if (tableLoops.SelectedRow == -1)
                return;

            EditLoop();
        }

        private void HandleSegmentsDoubleClick(object sender, EventArgs e)
        {
            if (tableSegments.SelectedRow == -1)
                return;

            EditSegment();
        }
                        
        protected void HandleLibraryBrowserDoubleClick(object sender, EventArgs e)
        {
            if(outlineLibraryBrowser.SelectedRow == -1)
                return;

            var item = (LibraryBrowserItem)outlineLibraryBrowser.ItemAtRow(outlineLibraryBrowser.SelectedRow);
            if(OnTreeNodeDoubleClicked != null)
                OnTreeNodeDoubleClicked.Invoke(item.Entity);
        }

        protected void HandleSongBrowserDoubleClick(object sender, EventArgs e)
        {
            if (songGridView.SelectedAudioFiles.Count == 0)
                return;

            var audioFile = songGridView.SelectedAudioFiles[0];
            OnTableRowDoubleClicked(audioFile);
        }

        public void RefreshAll()
        {
            OnAudioFileFormatFilterChanged(AudioFileFormat.All);
        }

        private void ShowUpdateLibraryView(bool show)
        {
            float viewHeight = 85;
            var frame = scrollViewLibraryBrowser.Frame;
            frame.Height = show ? frame.Height - viewHeight : frame.Height + viewHeight;
            frame.Y = show ? viewHeight : 0;
            scrollViewLibraryBrowser.Frame = frame;
            viewUpdateLibrary.Hidden = !show;
        }

        #region IPlayerView implementation

        public bool IsOutputMeterEnabled { get { return true; } }
        public bool IsPlayerPerformanceEnabled { get { return true; } }

        public Action OnPlayerPlay { get; set; }
        public Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        public Action OnPlayerPause { get; set; }
        public Action OnPlayerStop { get; set; }
        public Action OnPlayerPrevious { get; set; }
        public Action OnPlayerNext { get; set; } 
        public Action<float> OnPlayerSetPitchShifting { get; set; }
        public Action<float> OnPlayerSetTimeShifting { get; set; }
        public Action<float> OnPlayerSetVolume { get; set; }
        public Action<float> OnPlayerSetPosition { get; set; }
        public Func<float, SSPPosition> OnPlayerRequestPosition { get; set; }
        public Action OnEditSongMetadata { get; set; }        
        public Action OnPlayerShuffle { get; set; }
        public Action OnPlayerRepeat { get; set; }
        public Action OnOpenPlaylist { get; set; }
        public Action OnOpenEffects { get; set; }
        public Action OnOpenResumePlayback { get; set; }
        public Action OnOpenSelectAlbumArt { get; set; }
        public Action OnPlayerViewAppeared { get; set; }
        public Action<byte[]> OnApplyAlbumArtToSong { get; set; }
        public Action<byte[]> OnApplyAlbumArtToAlbum { get; set; }

        public void PlayerError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshPlaylist(Playlist playlist)
        {
            //InvokeOnMainThread(() => playlistView.SetPlaylist(playlist));
        }

        public void RefreshPlayerState(SSPPlayerState status, SSPRepeatType repeatType, bool isShuffleEnabled)
        {
            Console.WriteLine("RefreshPlayerStatus - Status: {0} - RepeatType: {1} - IsShuffleEnabled: {2}", status, repeatType, isShuffleEnabled);
            _playerStatus = status;
            InvokeOnMainThread(() => {
                switch (status)
                {
                    case SSPPlayerState.Initialized:
                    case SSPPlayerState.Stopped:
                    case SSPPlayerState.Paused:
//                    case SSPPlayerState.StartPaused:
//                    case SSPPlayerState.WaitingToStart:
                        btnToolbarPlayPause.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_play");
                        break;
                    case SSPPlayerState.Playing:
                        btnToolbarPlayPause.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_pause");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (repeatType)
                {
                    case SSPRepeatType.Off:
                        btnToolbarRepeat.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_repeat_off");
                        break;
                    case SSPRepeatType.Playlist:
                        btnToolbarRepeat.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_repeat_on");
                        break;
                    case SSPRepeatType.Song:
                        btnToolbarRepeat.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "toolbar_repeat_single");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                string imageName = isShuffleEnabled ? "toolbar_shuffle_on" : "toolbar_shuffle_off";
                btnToolbarShuffle.ImageView.Image = ImageResources.Images.FirstOrDefault(x => x.Name == imageName);
            });
        }

        public void RefreshPlayerPosition(SSPPosition position)
        {
            InvokeOnMainThread(() => {

                if (_isPlayerPositionChanging || _isScrollViewWaveFormChangingSecondaryPosition)
                return;

                if(_currentSongInfo == null || _currentSongInfo.AudioFile == null)
                    return;

                // The wave form scroll view isn't aware of floating point
                long positionBytes = position.Bytes;
                if(_currentSongInfo.UseFloatingPoint)
                    positionBytes /= 2;

                lblPosition.StringValue = position.Str;
                waveFormScrollView.SetPosition(positionBytes);

                long length = _currentSongInfo.AudioFile.LengthBytes;
                if(length > 0)
                {
                    trackBarPosition.ValueWithoutEvent = (int)(((float)position.Bytes / (float)length) * 1000);
                }
            });
		}

        public void RefreshPlayerPerformance(float cpu, UInt32 bufferDataAvailable)
        {
            Console.WriteLine("[PlayerPerformance] CPU: {0:0.0} - Buffer: {1}", cpu, bufferDataAvailable);
            InvokeOnMainThread(() => {
                string perf = string.Format("CPU: {0:0.0}% - Buffer: {1:0.0}kb", cpu, bufferDataAvailable / 1000f);
                lblSubtitleSongPosition.StringValue = perf;
            });
        }
		
        public void RefreshSongInformation(SongInformationEntity entity)
        {
            InvokeOnMainThread(() => {
                _currentSongInfo = entity;
                if (entity == null || entity.AudioFile == null)
                {
                    lblArtistName.StringValue = string.Empty;
                    lblAlbumTitle.StringValue = string.Empty;
                    lblSongTitle.StringValue = string.Empty;
                    lblSongPath.StringValue = string.Empty;
                    lblSampleRate.StringValue = string.Empty;
                    lblBitrate.StringValue = string.Empty;
                    lblBitsPerSample.StringValue = string.Empty;
                    lblFileType.StringValue = string.Empty;
                    lblYear.StringValue = string.Empty;
                    lblMonoStereo.StringValue = string.Empty;
                    lblFileSize.StringValue = string.Empty;
                    lblGenre.StringValue = string.Empty;
                    lblPlayCount.StringValue = string.Empty;
                    lblLastPlayed.StringValue = string.Empty;
                    lblPlaylistIndexCount.StringValue = string.Empty;
                    lblPosition.StringValue = "0:00.000";
                    lblLength.StringValue = "0:00.000";
                    _currentAlbumArtKey = string.Empty;
                    viewAlbumArt.SetImage(null);
                    waveFormScrollView.Reset();   
                    outputMeter.Reset();
                }
                else
                {
                    var audioFile = entity.AudioFile;
                    lblArtistName.StringValue = audioFile.ArtistName;
                    lblAlbumTitle.StringValue = audioFile.AlbumTitle;
                    lblSongTitle.StringValue = audioFile.Title;
                    lblSongPath.StringValue = audioFile.FilePath;
                    lblLength.StringValue = audioFile.Length;
                    lblSampleRate.StringValue = string.Format("{0} Hz", audioFile.SampleRate);
                    lblBitrate.StringValue = string.Format("{0} kbps", audioFile.Bitrate);
                    lblBitsPerSample.StringValue = string.Format("{0} bits", audioFile.BitsPerSample);
                    lblFileType.StringValue = audioFile.FileType.ToString();
                    lblYear.StringValue = audioFile.Year == 0 ? "No year specified" : audioFile.Year.ToString();
                    lblMonoStereo.StringValue = audioFile.AudioChannels == 1 ? "Mono" : "Stereo";
                    lblFileSize.StringValue = string.Format("{0} bytes", audioFile.FileSize);
                    lblGenre.StringValue = string.IsNullOrEmpty(audioFile.Genre) ? "No genre specified" : string.Format("{0}", audioFile.Genre);
                    lblPlayCount.StringValue = string.Format("{0} times played", audioFile.PlayCount);
                    lblLastPlayed.StringValue = audioFile.LastPlayed.HasValue ? string.Format("Last played on {0}", audioFile.LastPlayed.Value.ToShortDateString()) : "";
                    lblPlaylistIndexCount.StringValue = string.Format("{0} / {1}", entity.PlaylistIndex+1, entity.PlaylistCount);

                    songGridView.NowPlayingAudioFileId = audioFile.Id;
                    //playlistView.NowPlayingPlaylistItemId = playlistItemId;

                    LoadAlbumArt(audioFile);

                    // The wave form scroll view isn't aware of floating point
                    long lengthWaveForm = entity.AudioFile.LengthBytes;
                    if(entity.UseFloatingPoint)
                        lengthWaveForm /= 2;

                    waveFormScrollView.SetWaveFormLength(lengthWaveForm);
                    waveFormScrollView.LoadPeakFile(audioFile);
                }
            });
		}
        
        private async void LoadAlbumArt(AudioFile audioFile)
        {
            string key = audioFile.ArtistName.ToUpper() + "_" + audioFile.AlbumTitle.ToUpper();
            if (_currentAlbumArtKey == key)
                return;

            int imageSize = (int)viewAlbumArt.Frame.Height;
            var task = Task<NSImage>.Factory.StartNew(() => {
                try
                {
                    NSImage image = null;
                    NSImage scaledImage = null;
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);                        
                    using (NSData imageData = NSData.FromArray(bytesImage))
                    {
                        InvokeOnMainThread(() => {
                            image = new NSImage(imageData);
                            scaledImage = CoreGraphicsHelper.ScaleImageSquare(image, imageSize);
                        });
                    }
                    return scaledImage;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to process image: {0}", ex);
                }
                
                return null;
            });

            NSImage imageFromTask = await task;
            if(imageFromTask == null)
                return;
            
            InvokeOnMainThread(() => {
                try
                {
                    _currentAlbumArtKey = key;
                    viewAlbumArt.SetImage(imageFromTask);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("PlayerViewController - RefreshSongInformation - Failed to set image after processing: {0}", ex);
                }
            });
        }

        public void RefreshMarkers(IEnumerable<Marker> markers)
        {
        }

        public void RefreshLoops(IEnumerable<SSPLoop> loops)
        {
        }

        public void RefreshPlayerVolume(PlayerVolume entity)
        {
            InvokeOnMainThread(() => {
                lblVolume.StringValue = entity.VolumeString;
                if(faderVolume.Value != (int)entity.Volume)
                    faderVolume.ValueWithoutEvent = (int)entity.Volume;
            });
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShifting entity)
        {
            InvokeOnMainThread(() => {
//                lblTimeShifting.StingValue = entity.TimeShiftingString;
//                if(sliderTimeShifting.FloatValue != entity.TimeShifting)
//                    sliderTimeShifting.FloatValue = entity.TimeShifting;
            });
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
            InvokeOnMainThread(() => {
                outputMeter.AddWaveDataBlock(dataLeft, dataRight);
            });
        }

        #endregion

		#region ISongBrowserView implementation

        public Action<AudioFile> OnTableRowDoubleClicked { get; set; }
        public Action<AudioFile> OnSongBrowserEditSongMetadata { get; set; }   
        public Action<IEnumerable<AudioFile>> OnSongBrowserAddToPlaylist { get; set; }
        public Action<string> OnSearchTerms { get; set; }

        public void SongBrowserError(Exception ex)
        {
            ShowError(ex);
        }

		public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
        {
            InvokeOnMainThread(() => songGridView.SetAudioFiles(audioFiles.ToList()));
		}

		#endregion

		#region ILibraryBrowserView implementation

        public Action<AudioFileFormat> OnAudioFileFormatFilterChanged { get; set; }
        public Action<LibraryBrowserEntity> OnTreeNodeSelected { get; set; }
        public Action<LibraryBrowserEntity, object> OnTreeNodeExpanded { get; set; }     
        public Action<LibraryBrowserEntity> OnTreeNodeDoubleClicked { get; set; }
        public Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable { get; set; }
        public Action<LibraryBrowserEntity> OnAddToPlaylist { get; set; }
        public Action<LibraryBrowserEntity> OnRemoveFromLibrary { get; set; }
        public Action<LibraryBrowserEntity> OnDeleteFromHardDisk { get; set; }

        public void LibraryBrowserError(Exception ex)
        {
            ShowError(ex);
        }

		public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities)
		{
            InvokeOnMainThread(() => {
                _libraryBrowserDataSource = new LibraryBrowserDataSource(entities, (entity) => { return this.OnTreeNodeExpandable(entity); });
    			outlineLibraryBrowser.DataSource = _libraryBrowserDataSource;
            });
		}

		public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
		{
		    // Not used in Cocoa.
		}
        
        public void RefreshLibraryBrowserSelectedNode(LibraryBrowserEntity entity)
        {
            InvokeOnMainThread(() => {
                if(entity.EntityType == LibraryBrowserEntityType.Artist ||
                   entity.EntityType == LibraryBrowserEntityType.ArtistAlbum)
                {
                    var artistsNode = _libraryBrowserDataSource.Items.FirstOrDefault(x => x.Entity.EntityType == LibraryBrowserEntityType.Artists);
                    outlineLibraryBrowser.ExpandItem(artistsNode);
                    var artistNode = artistsNode.SubItems.FirstOrDefault(x => string.Compare(x.Entity.Query.ArtistName, entity.Query.ArtistName, true) == 0);
                    
                    if(entity.EntityType == LibraryBrowserEntityType.Artist)
                    {
                        int row = outlineLibraryBrowser.RowForItem(artistNode);
                        outlineLibraryBrowser.SelectRow(row, false);
                        outlineLibraryBrowser.ScrollRowToVisible(row);
                    }
                    else if(entity.EntityType == LibraryBrowserEntityType.ArtistAlbum)
                    {
                        outlineLibraryBrowser.ExpandItem(artistNode);
                        var artistAlbumNode = artistNode.SubItems.FirstOrDefault(x => string.Compare(x.Entity.Query.AlbumTitle, entity.Query.AlbumTitle, true) == 0);
                        int row = outlineLibraryBrowser.RowForItem(artistAlbumNode);
                        outlineLibraryBrowser.SelectRow(row, false);
                        outlineLibraryBrowser.ScrollRowToVisible(row);
                    }
                }
                else if(entity.EntityType == LibraryBrowserEntityType.Album)
                {
                    var albumsNode = _libraryBrowserDataSource.Items.FirstOrDefault(x => x.Entity.EntityType == LibraryBrowserEntityType.Albums);
                    outlineLibraryBrowser.ExpandItem(albumsNode);
                    
                    var albumNode = albumsNode.SubItems.FirstOrDefault(x => string.Compare(x.Entity.Query.AlbumTitle, entity.Query.AlbumTitle, true) == 0);
                    int row = outlineLibraryBrowser.RowForItem(albumNode);
                    outlineLibraryBrowser.SelectRow(row, false);
                    outlineLibraryBrowser.ScrollRowToVisible(row);
                }
            });
        }

        public void NotifyLibraryBrowserNewNode(int position, LibraryBrowserEntity entity)
        {
            InvokeOnMainThread(() => {
                outlineLibraryBrowser.ReloadData();
            });
        }

        public void NotifyLibraryBrowserRemovedNode(int position)
        {
            InvokeOnMainThread(() => {
                outlineLibraryBrowser.ReloadData();
            });
        }

        private void SelectItem(NSObject item)
        {
            int itemIndex = outlineLibraryBrowser.RowForItem(item);
            if (itemIndex < 0)
            {
                ExpandParentsOfItem(item);
                itemIndex = outlineLibraryBrowser.RowForItem(item);
                if(itemIndex < 0)
                    return;
            }
            
            outlineLibraryBrowser.SelectRow(itemIndex, false);
        }
        
        private void ExpandParentsOfItem(NSObject item)
        {
            while (item != null)
            {
                var parent = outlineLibraryBrowser.GetParent(item);
                if(!outlineLibraryBrowser.IsExpandable(parent))
                    break;
                if(!outlineLibraryBrowser.IsItemExpanded(parent))
                    outlineLibraryBrowser.ExpandItem(parent);
                item = parent;
            }
        }

		#endregion

        #region IMainView implementation

        public Action OnOpenAboutWindow { get; set; }
        public Action OnOpenPreferencesWindow { get; set; }
        public Action OnOpenEffectsWindow { get; set; }
        public Action OnOpenPlaylistWindow { get; set; }
        public Action OnOpenSyncWindow { get; set; }
        public Action OnOpenSyncCloudWindow { get; set; }
        public Action OnOpenSyncWebBrowserWindow { get; set; }
        public Action OnUpdateLibrary { get; set; }

        public void PushSubView(IBaseView view)
        {
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
            ShowError(ex);
        }

        public void RefreshKeys(List<Tuple<int, string>> keys)
        {
        }

        public void RefreshPitchShifting(PlayerPitchShifting entity)
        {
            InvokeOnMainThread(() =>{
                txtIntervalValue.StringValue = entity.Interval;
                lblNewKeyValue.StringValue = entity.NewKey.Item2;
                lblReferenceKeyValue.StringValue = entity.ReferenceKey.Item2;
                trackBarPitchShifting.Value = (int)entity.IntervalValue;
            });
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
            ShowError(ex);
        }

        public void RefreshTimeShifting(PlayerTimeShifting entity)
        {
            InvokeOnMainThread(() =>{
                lblDetectedTempoValue.StringValue = entity.DetectedTempo;
                lblReferenceTempoValue.StringValue = entity.ReferenceTempo;
                txtCurrentTempoValue.StringValue = entity.CurrentTempo;
                trackBarTimeShifting.Value = (int) entity.TimeShiftingValue;
            });
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
            ShowError(ex);
        }

        public void RefreshMarkers(List<Marker> markers)
        {
            InvokeOnMainThread(delegate {
                _markers = markers.ToList();
                waveFormScrollView.SetMarkers(_markers);

                int row = tableMarkers.SelectedRow;
                var selectedMarker = row >= 0 && row <= _markers.Count - 1 ? _markers[row] : null;
                tableMarkers.ReloadData();
                if(selectedMarker != null)
                {
                    int newRow = _markers.IndexOf(selectedMarker);
                    if(newRow >= 0)
                        tableMarkers.SelectRow(newRow, false);
                }
            });
        }

        public void RefreshActiveMarker(Guid markerId)
        {
        }

        public void RefreshMarkerPosition(Marker marker)
        {
        }

        public void RefreshMarkerPosition(Marker marker, int newIndex)
        {
        }

        #endregion

        #region IMarkerDetailsView implementation

        public Action<float> OnChangePositionMarkerDetails { get; set; }
        public Action<Marker> OnUpdateMarkerDetails { get; set; }
        public Action OnDeleteMarkerDetails { get; set; }
        public Action OnPunchInMarkerDetails { get; set; }

        public void MarkerDetailsError(Exception ex)
        {
            ShowError(ex);
        }

        public void DismissMarkerDetailsView()
        {
        }

        public void RefreshMarker(Marker marker, AudioFile audioFile)
        {
            InvokeOnMainThread(delegate {
                _currentMarker = marker;
                txtMarkerName.StringValue = marker.Name;
                lblMarkerPositionValue.StringValue = marker.Position;
                trackBarMarkerPosition.ValueWithoutEvent = (int)(marker.PositionPercentage * 10);
                waveFormScrollView.SetActiveMarker(marker.MarkerId);
            });
        }

        public void RefreshMarkerPosition(string position, float positionPercentage)
        {
            InvokeOnMainThread(delegate {
                lblMarkerPositionValue.StringValue = position;
                trackBarMarkerPosition.ValueWithoutEvent = (int)(positionPercentage * 10);
                _currentMarker.Position = position;
                _currentMarker.PositionPercentage = positionPercentage;
                waveFormScrollView.SetMarkerPosition(_currentMarker);
            });
        }

        #endregion

        #region ILoopsView implementation

        public Action OnAddLoop { get; set; }
        public Action<SSPLoop> OnEditLoop { get; set; }
        public Action<SSPLoop> OnSelectLoop { get; set; }
        public Action<SSPLoop> OnDeleteLoop { get; set; }
        public Action<SSPLoop> OnUpdateLoop { get; set; }
        public Action<SSPLoop> OnPlayLoop { get; set; }

        public Action<SSPLoopSegmentType> OnPunchInLoopSegment { get; set; }
        public Action<SSPLoopSegmentType, float> OnChangingLoopSegmentPosition { get; set; }
        public Action<SSPLoopSegmentType, float> OnChangedLoopSegmentPosition { get; set; }

        public void LoopError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshLoops(List<SSPLoop> loops)
        {
            InvokeOnMainThread(delegate {
                _loops = loops.ToList();

                int row = tableLoops.SelectedRow;
                var selectedLoop = row >= 0 && row <= _loops.Count - 1 ? _loops[row] : null;
                tableLoops.ReloadData();
                if(selectedLoop != null)
                {
                    int newRow = _loops.IndexOf(selectedLoop);
                    if(newRow >= 0)
                        tableLoops.SelectRow(newRow, false);
                }
            });
        }

//        public void RefreshLoopSegment(Loop loop, Segment segment, long audioFileLength)
//        {
//            InvokeOnMainThread(delegate {
//                
//                //tableLoops.ReloadData(NSIndexSet.FromIndex(
//            });
//        }

        public void RefreshCurrentlyPlayingLoop(SSPLoop loop)
        {
        }

        #endregion

        #region ILoopDetailsView implementation

        public Action OnAddSegment { get; set; }
        public Action<Guid, int> OnAddSegmentFromMarker { get; set; }
//        public Action<Segment> OnEditSegment { get; set; }
//        public Action<Segment> OnDeleteSegment { get; set; }
        public Action<SSPLoop> OnUpdateLoopDetails { get; set; }
//        public Action<Segment, int> OnChangeSegmentOrder { get; set; }
//        public Action<Segment, Guid> OnLinkSegmentToMarker { get; set; }
//		public Action<Segment, float> OnChangingSegmentPosition { get; set; }
//        public Action<Segment, float> OnChangedSegmentPosition { get; set; }
//		public Action<Segment> OnPunchInSegment { get; set; }
        
        public void LoopDetailsError(Exception ex)
        {
            ShowError(ex);
        }
        
        public void RefreshLoopDetails(SSPLoop loop, AudioFile audioFile, long audioFileLength)
        {
//            InvokeOnMainThread(delegate {
//                _currentLoop = loop;
//                txtLoopName.StringValue = loop.Name;
//                waveFormScrollView.SetLoop(loop);
//                waveFormScrollView.FocusZoomOnLoop(_currentLoop);
//
//                int row = tableSegments.SelectedRow;
//                var selectedSegment = row >= 0 && row <= _currentLoop.Segments.Count - 1 ? _currentLoop.Segments[row] : null;
//                tableSegments.ReloadData();
//                if(selectedSegment != null)
//                {
//                    int newRow = _currentLoop.Segments.IndexOf(selectedSegment);
//                    if(newRow >= 0)
//                        tableSegments.SelectRow(newRow, false);
//                }
//            });
        }
		
//		public void RefreshLoopDetailsSegment(Segment segment, long audioFileLength)
//        {
//        }
		
		public void RefreshLoopMarkers(IEnumerable<Marker> markers)
		{
		}

        #endregion

        #region ILoopPlaybackView implementation

        public Action OnPreviousLoop { get; set; }
        public Action OnNextLoop { get; set; }
        public Action OnPreviousSegment { get; set; }
        public Action OnNextSegment { get; set; }

        public void LoopPlaybackError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshLoopPlayback(LoopPlaybackEntity entity)
        {
            InvokeOnMainThread(delegate {
            });
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
            //Console.WriteLine("IUpdateLibraryView - RefreshStatus - status: {0} progress: {1}", entity.Title, entity.PercentageDone);
            InvokeOnMainThread(delegate {
                lblUpdateLibraryStatus.StringValue = entity.Title;
                progressBarUpdateLibrary.Value = Math.Min(1, entity.PercentageDone);
            });
        }

        public void AddToLog(string entry)
        {
        }

        public void ProcessStarted()
        {
            //Console.WriteLine("IUpdateLibraryView - ProcessStarted");
            InvokeOnMainThread(delegate {
                ShowUpdateLibraryView(true);
            });
        }

        public void ProcessEnded(bool canceled)
        {
            //Console.WriteLine("IUpdateLibraryView - ProcessEnded");
            InvokeOnMainThread(delegate {
                lblUpdateLibraryStatus.StringValue = "Update library successful.";
                progressBarUpdateLibrary.Value = 1;
            });

            // Delay before closing update library panel
            var task = TaskHelper.DelayTask(1500);
            task.ContinueWith((a) =>
                {
                    InvokeOnMainThread(delegate {
                        ShowUpdateLibraryView(false);
                    });
                });
        }

        #endregion

        #region IQueueView implementation

        public Action OnQueueStartPlayback { get; set; }
        public Action OnQueueRemoveAll { get; set; }

        public void QueueError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshQueue(int songCount, string totalLength)
        {
        }

        #endregion
	}
}
