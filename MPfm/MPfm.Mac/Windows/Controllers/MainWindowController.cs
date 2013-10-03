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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.CoreText;
using MonoMac.Foundation;
using MPfm.Core;
using MPfm.Library;
using MPfm.MVP;
using MPfm.Sound;
using MPfm.MVP.Views;
using MPfm.MVP.Models;
using MPfm.Sound.AudioFiles;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Messages;
using MPfm.Mac.Classes.Objects;
using MPfm.Mac.Classes.Helpers;
using MPfm.Mac.Classes.Delegates;
using MPfm.Player.Objects;
using MPfm.MVP.Presenters;
using MPfm.Mac.Classes.Controls;

namespace MPfm.Mac
{
    /// <summary>
    /// Main window controller.
    /// </summary>
	public partial class MainWindowController : BaseWindowController, IMainView
	{
        List<Marker> _markers = new List<Marker>();
        LibraryBrowserOutlineViewDelegate _libraryBrowserOutlineViewDelegate = null;
		LibraryBrowserDataSource _libraryBrowserDataSource = null;
        SongBrowserTableViewDelegate _songBrowserOutlineViewDelegate = null;
        SongBrowserSource _songBrowserSource = null;
        AlbumCoverSource _albumCoverSource = null;
        AlbumCoverCacheService _albumCoverCacheService = null;

		public MainWindowController(IntPtr handle) 
            : base (handle)
		{
		}

		public MainWindowController(Action<IBaseView> onViewReady) : base ("MainWindow", onViewReady)
        {
            this._albumCoverCacheService = new AlbumCoverCacheService();
            this.Window.AlphaValue = 0;
            this.Window.MakeKeyAndOrderFront(this);

            // Fade in main window
            NSMutableDictionary dict = new NSMutableDictionary();
            dict.Add(NSViewAnimation.TargetKey, Window);
            dict.Add(NSViewAnimation.EffectKey, NSViewAnimation.FadeInEffect);
            NSViewAnimation anim = new NSViewAnimation(new List<NSMutableDictionary>(){ dict }.ToArray());
            anim.Duration = 0.4f;
            anim.StartAnimation();
		}		

		public override void WindowDidLoad()
		{
            base.WindowDidLoad();

            Tracing.Log("MainWindowController.WindowDidLoad -- Initializing user interface...");
            //this.Window.Title = "Sessions " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA";

            splitMain.Delegate = new MainSplitViewDelegate();

            cboSoundFormat.RemoveAllItems();
            cboSoundFormat.AddItem("All");
            cboSoundFormat.AddItem("FLAC");
            cboSoundFormat.AddItem("OGG");
            cboSoundFormat.AddItem("MP3");
            cboSoundFormat.AddItem("MPC");
            cboSoundFormat.AddItem("WAV");
            cboSoundFormat.AddItem("WV");

            _libraryBrowserOutlineViewDelegate = new LibraryBrowserOutlineViewDelegate((entity) => { OnTreeNodeSelected(entity); });
            outlineLibraryBrowser.Delegate = _libraryBrowserOutlineViewDelegate;
            outlineLibraryBrowser.AllowsMultipleSelection = false;
            outlineLibraryBrowser.DoubleClick += HandleLibraryBrowserDoubleClick;

            _songBrowserOutlineViewDelegate = new SongBrowserTableViewDelegate();
            tableSongBrowser.Delegate = _songBrowserOutlineViewDelegate;
            tableSongBrowser.AllowsMultipleSelection = true;
            tableSongBrowser.DoubleClick += HandleSongBrowserDoubleClick;

            tableMarkers.WeakDelegate = this;
            tableMarkers.WeakDataSource = this;
            tableMarkers.DoubleClick += HandleMarkersDoubleClick;

            LoadImages();
            SetTheme();

            tableAlbumCovers.FocusRingType = NSFocusRingType.None;
            tableSongBrowser.FocusRingType = NSFocusRingType.None;
            scrollViewSongBrowser.BorderType = NSBorderType.NoBorder;
            scrollViewAlbumCovers.BorderType = NSBorderType.NoBorder;
            scrollViewAlbumCovers.HasHorizontalScroller = false;
            scrollViewAlbumCovers.HasVerticalScroller = false;

            scrollViewAlbumCovers.SetSynchronizedScrollView(scrollViewSongBrowser);
            scrollViewSongBrowser.SetSynchronizedScrollView(scrollViewAlbumCovers);

            btnTabTimeShifting.IsSelected = true;
            btnTabTimeShifting.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabPitchShifting.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabInfo.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabActions.OnTabButtonSelected += HandleOnTabButtonSelected;

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("NSOutlineViewItemDidExpandNotification"), ItemDidExpand, outlineLibraryBrowser);
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("NSControlTextDidChangeNotification"), SearchTextDidChange, searchSongBrowser);

            OnViewReady.Invoke(this);
		}

        private void HandleOnTabButtonSelected(MPfmTabButton button)
        {
            Console.WriteLine("Test: {0}", button.Title);

            btnTabTimeShifting.IsSelected = button == btnTabTimeShifting;
            btnTabPitchShifting.IsSelected = button == btnTabPitchShifting;
            btnTabInfo.IsSelected = button == btnTabInfo;
            btnTabActions.IsSelected = button == btnTabActions;

            viewTimeShifting.Hidden = button != btnTabTimeShifting;
            viewPitchShifting.Hidden = button != btnTabPitchShifting;
            viewInformation.Hidden = button != btnTabInfo;
            viewActions.Hidden = button != btnTabActions;

//            if (button == btnTabTimeShifting)
//            {
//
//            } 
//            else if (button == btnTabPitchShifting)
//            {
//            }
//            else if (button == btnTabInfo)
//            {
//            } 
//            else if (button == btnTabActions)
//            {
//            }
        }

        private void SetTheme()
        {
            viewLeftHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewLeftHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewRightHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewRightHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewLoopsHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewLoopsHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewMarkersHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewMarkersHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewSongBrowserHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewSongBrowserHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;

            btnPlayLoop.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnPlayLoop.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnPlayLoop.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnPlayLoop.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
            //btnStopLoop.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            //btnStopLoop.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            //btnStopLoop.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            //btnStopLoop.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
            btnAddLoop.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnAddLoop.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnAddLoop.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
            btnAddLoop.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnEditLoop.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnEditLoop.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnEditLoop.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnEditLoop.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
            btnRemoveLoop.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnRemoveLoop.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnRemoveLoop.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnRemoveLoop.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
            btnGoToMarker.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnGoToMarker.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnGoToMarker.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnGoToMarker.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
            btnAddMarker.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnAddMarker.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnAddMarker.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnAddMarker.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
            btnEditMarker.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnEditMarker.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnEditMarker.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnEditMarker.BorderColor = GlobalTheme.ButtonToolbarBorderColor;
            btnRemoveMarker.BackgroundColor = GlobalTheme.ButtonToolbarBackgroundColor;
            btnRemoveMarker.BackgroundMouseOverColor = GlobalTheme.ButtonToolbarBackgroundMouseOverColor;
            btnRemoveMarker.BackgroundMouseDownColor = GlobalTheme.ButtonToolbarBackgroundMouseDownColor;
            btnRemoveMarker.BorderColor = GlobalTheme.ButtonToolbarBorderColor;

            lblAlbumTitle.TextColor = NSColor.FromDeviceRgba(196f/255f, 213f/255f, 225f/255f, 1);
            lblSongTitle.TextColor = NSColor.FromDeviceRgba(171f/255f, 186f/255f, 196f/255f, 1);
            lblSongPath.TextColor = NSColor.FromDeviceRgba(97f/255f, 122f/255f, 140f/255f, 1);

            //viewInformation.IsHeaderVisible = true;
            viewSongPosition.IsHeaderVisible = true;
            viewVolume.IsHeaderVisible = true;
            //viewTimeShifting.IsHeaderVisible = true;
            //viewPitchShifting.IsHeaderVisible = true;           

            lblArtistName.Font = NSFont.FromFontName("TitilliumText25L-800wt", 24);
            lblAlbumTitle.Font = NSFont.FromFontName("TitilliumText25L-600wt", 20);
            lblSongTitle.Font = NSFont.FromFontName("TitilliumText25L-600wt", 17);
            lblSongPath.Font = NSFont.FromFontName("TitilliumText25L-400wt", 12);

            lblSampleRate.Font = NSFont.FromFontName("Junction", 11f);
            lblBitrate.Font = NSFont.FromFontName("Junction", 11f);
            lblFileType.Font = NSFont.FromFontName("Junction", 11f);
            lblBitsPerSample.Font = NSFont.FromFontName("Junction", 11f);
            lblFilterBySoundFormat.Font = NSFont.FromFontName("Junction", 11f);

            lblTitleLibraryBrowser.Font = NSFont.FromFontName("TitilliumText25L-800wt", 14);
            lblTitleCurrentSong.Font = NSFont.FromFontName("TitilliumText25L-800wt", 14);
            lblTitleLoops.Font = NSFont.FromFontName("TitilliumText25L-800wt", 14);
            lblTitleMarkers.Font = NSFont.FromFontName("TitilliumText25L-800wt", 14);
            lblTitleSongBrowser.Font = NSFont.FromFontName("TitilliumText25L-800wt", 14);

            lblSubtitleSongPosition.Font = NSFont.FromFontName("TitilliumText25L-800wt", 12);
            //lblSubtitleTimeShifting.Font = NSFont.FromFontName("TitilliumText25L-800wt", 12);
            lblSubtitleVolume.Font = NSFont.FromFontName("TitilliumText25L-800wt", 12);
            //lblSubtitleInformation.Font = NSFont.FromFontName("TitilliumText25L-800wt", 12);
            //lblSubtitlePitchShifting.Font = NSFont.FromFontName("TitilliumText25L-800wt", 12);

            lblPosition.Font = NSFont.FromFontName("DroidSansMono", 15f);
            lblLength.Font = NSFont.FromFontName("DroidSansMono", 15f);
            lblVolume.Font = NSFont.FromFontName("DroidSansMono", 11f);
            txtPitchShiftingValue.Font = NSFont.FromFontName("DroidSansMono", 10f);
            lblDetectedTempoValue.Font = NSFont.FromFontName("DroidSansMono", 10f);
            lblReferenceTempoValue.Font = NSFont.FromFontName("DroidSansMono", 10f);
            txtCurrentTempoValue.Font = NSFont.FromFontName("DroidSansMono", 10f);

            lblDetectedTempo.Font = NSFont.FromFontName("Junction", 11);
            lblCurrentTempo.Font = NSFont.FromFontName("Junction", 11);
            lblReferenceTempo.Font = NSFont.FromFontName("Junction", 11);
            lblSemitones.Font = NSFont.FromFontName("Junction", 11);

            cboSoundFormat.Font = NSFont.FromFontName("Junction", 11);
            searchSongBrowser.Font = NSFont.FromFontName("Junction", 12);

            // Set cell fonts for Library Browser
            NSTableColumn columnText = outlineLibraryBrowser.FindTableColumn(new NSString("columnText"));
            columnText.DataCell.Font = NSFont.FromFontName("Junction", 11f);

            // Set cell fonts for Song Browser
            NSTableColumn columnTrackNumber = tableSongBrowser.FindTableColumn(new NSString("columnTrackNumber"));
            NSTableColumn columnTitle = tableSongBrowser.FindTableColumn(new NSString("columnTitle"));
            NSTableColumn columnLength = tableSongBrowser.FindTableColumn(new NSString("columnLength"));
            NSTableColumn columnArtistName = tableSongBrowser.FindTableColumn(new NSString("columnArtistName"));
            NSTableColumn columnAlbumTitle = tableSongBrowser.FindTableColumn(new NSString("columnAlbumTitle"));
            columnTrackNumber.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnTrackNumber.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnTitle.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnTitle.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLength.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLength.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnArtistName.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnArtistName.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnAlbumTitle.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnAlbumTitle.DataCell.Font = NSFont.FromFontName("Junction", 11f);

            // Set cell fonts for Loops
            NSTableColumn columnLoopName = tableLoops.FindTableColumn(new NSString("columnLoopName"));
            NSTableColumn columnLoopLength = tableLoops.FindTableColumn(new NSString("columnLoopLength"));
            NSTableColumn columnLoopStartPosition = tableLoops.FindTableColumn(new NSString("columnLoopStartPosition"));
            NSTableColumn columnLoopEndPosition = tableLoops.FindTableColumn(new NSString("columnLoopEndPosition"));
            columnLoopName.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopName.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopLength.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopLength.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopStartPosition.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopStartPosition.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopEndPosition.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnLoopEndPosition.DataCell.Font = NSFont.FromFontName("Junction", 11f);

            // Set cell fonts for Markers
            NSTableColumn columnMarkerName = tableMarkers.FindTableColumn(new NSString("columnMarkerName"));
            NSTableColumn columnMarkerPosition = tableMarkers.FindTableColumn(new NSString("columnMarkerPosition"));
            NSTableColumn columnMarkerComments = tableMarkers.FindTableColumn(new NSString("columnMarkerComments"));
            columnMarkerName.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerName.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerPosition.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerPosition.DataCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerComments.HeaderCell.Font = NSFont.FromFontName("Junction", 11f);
            columnMarkerComments.DataCell.Font = NSFont.FromFontName("Junction", 11f);

            btnDetectTempo.Font = NSFont.FromFontName("Junction", 11f);
            btnPlayLoop.Font = NSFont.FromFontName("Junction", 11f);
            //btnStopLoop.Font = NSFont.FromFontName("Junction", 11f);
            btnAddLoop.Font = NSFont.FromFontName("Junction", 11f);
            btnEditLoop.Font = NSFont.FromFontName("Junction", 11f);
            btnRemoveLoop.Font = NSFont.FromFontName("Junction", 11f);
            btnGoToMarker.Font = NSFont.FromFontName("Junction", 11f);
            btnAddMarker.Font = NSFont.FromFontName("Junction", 11f);
            btnEditMarker.Font = NSFont.FromFontName("Junction", 11f);
            btnRemoveMarker.Font = NSFont.FromFontName("Junction", 11f);
        }

        /// <summary>
        /// Loads the image resources in all controls.
        /// </summary>
        private void LoadImages()
        {
            // Load images in toolbar
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarOpen").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_folder-open");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarUpdateLibrary").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_update");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlay").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_play");
            //toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPause").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_pause");
            //toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarStop").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_stop");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPrevious").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_previous");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarNext").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_next");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarRepeat").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_repeat");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarShuffle").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_repeat");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarEffects").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_equalizer");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlaylist").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_playlist");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPreferences").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_settings");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarSync").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_sync");

            // Load button images
            cboSoundFormat.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_plus");
            btnAddLoop.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_add");
            btnAddMarker.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_add");
            btnAddSongToPlaylist.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_add");
            btnEditLoop.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_edit");
            btnEditMarker.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_edit");
            btnEditSongMetadata.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_edit");
            btnRemoveLoop.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_delete");
            btnRemoveMarker.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_delete");
            btnPlayLoop.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_play");
            btnPlaySelectedSong.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_play");
            //btnStopLoop.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_stop");
            btnGoToMarker.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_goto");
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
                openPanel.AllowedFileTypes = new string[]{ "FLAC", "MP3", "OGG", "WAV", "MPC", "WV" };
                openPanel.Title = "Please select audio files to add to the library";
				openPanel.Prompt = "Add files to library";
				openPanel.RunModal();
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
				openPanel.RunModal();
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
				openPanel.RunModal();
				filePaths = openPanel.Urls.Select(x => x.Path);
			}

			if(filePaths != null && filePaths.Count() > 0)
                OnPlayerPlayFiles(filePaths);
		}

		partial void actionUpdateLibrary(NSObject sender)
		{
            OnUpdateLibrary();
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
		}

        partial void actionShuffle(NSObject sender)
        {
        }

        partial void actionOpenMainWindow(NSObject sender)
        {
            this.Window.MakeKeyAndOrderFront(this);
        }

		partial void actionOpenPlaylistWindow(NSObject sender)
		{
            OnOpenPlaylistWindow();
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

        partial void actionAddSongToPlaylist(NSObject sender)
        {
        }

        partial void actionEditSongMetadata(NSObject sender)
        {

        }

        partial void actionPlaySelectedSong(NSObject sender)
        {

        }

        partial void actionChangeTimeShifting(NSObject sender)
        {
            OnPlayerSetTimeShifting(sliderTimeShifting.FloatValue);
        }

        partial void actionChangeSongPosition(NSObject sender)
        {

        }

        partial void actionChangeVolume(NSObject sender)
        {
            OnPlayerSetVolume(sliderVolume.FloatValue);
        }

        partial void actionPlayLoop(NSObject sender)
        {
        }

        partial void actionStopLoop(NSObject sender)
        {
        }

        partial void actionAddLoop(NSObject sender)
        {
        }

        partial void actionEditLoop(NSObject sender)
        {           
        }

        partial void actionRemoveLoop(NSObject sender)
        {
        }

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
        }

        partial void actionRemoveMarker(NSObject sender)
        {
        }

        partial void actionContextualMenuPlay(NSObject sender)
        {
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
                return _markers.Count;

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
            NSTableCellView view;           
            view = (NSTableCellView)tableView.MakeView(tableColumn.Identifier.ToString().Replace("column", "cell"), this);
            view.TextField.Font = NSFont.FromFontName("Junction", 11);

            if (tableView.Identifier == "tableMarkers")
            {
                if (tableColumn.Identifier.ToString() == "columnMarkerName")
                    view.TextField.StringValue = _markers[row].Name;
                else if (tableColumn.Identifier.ToString() == "columnMarkerPosition")
                    view.TextField.StringValue = _markers[row].Position;
                else
                    view.TextField.StringValue = string.Empty;
            }

            return view;
        }

        [Export ("outlineViewItemDidExpand")]
        public void ItemDidExpand(NSNotification notification)
        {
            // Check for dummy nodes
            var item = (LibraryBrowserItem)notification.UserInfo["NSObject"];
            if (item.SubItems.Count > 0 && item.SubItems[0].Entity.Type == LibraryBrowserEntityType.Dummy)
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
        
        protected void HandleLibraryBrowserDoubleClick(object sender, EventArgs e)
        {
            if(outlineLibraryBrowser.SelectedRow == -1)
                return;

            LibraryBrowserItem item = (LibraryBrowserItem)outlineLibraryBrowser.ItemAtRow(outlineLibraryBrowser.SelectedRow);
            if(OnTreeNodeDoubleClicked != null)
                OnTreeNodeDoubleClicked.Invoke(item.Entity);
        }

        protected void HandleSongBrowserDoubleClick(object sender, EventArgs e)
        {
            if (tableSongBrowser.SelectedRow == -1)
                return;

            AudioFile audioFile = _songBrowserSource.Items[tableSongBrowser.SelectedRow].AudioFile;
            if(OnTableRowDoubleClicked != null)
                OnTableRowDoubleClicked.Invoke(audioFile);
        }

        public void RefreshAll()
        {
            OnAudioFileFormatFilterChanged(AudioFileFormat.All);
        }

        #region IPlayerView implementation

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
        public Func<float, PlayerPositionEntity> OnPlayerRequestPosition { get; set; }
        public Action OnEditSongMetadata { get; set; }        
        public Action OnPlayerShuffle { get; set; }
        public Action OnPlayerRepeat { get; set; }
        public Action OnOpenPlaylist { get; set; }

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
            InvokeOnMainThread(() => {
                switch (status)
                {
                    case PlayerStatusType.Initialized:
                        goto case PlayerStatusType.Paused;
                    case PlayerStatusType.Stopped:
                        goto case PlayerStatusType.Paused;
                    case PlayerStatusType.Paused:
                        toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlay").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_play");
                        toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlay").Label = "Play";
                        break;
                    case PlayerStatusType.Playing:
                        toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlay").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_icomoon_pause");
                        toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlay").Label = "Pause";
                        break;
                }
            });
        }

		public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            // When setting .StringValue, use a autorelease pool to keep the warnings away
            // http://mono.1490590.n4.nabble.com/Memory-Leak-td3206211.html
            using (NSAutoreleasePool pool = new NSAutoreleasePool())
            {
                // TODO: Bug CPU hit when updating label...
                lblPosition.StringValue = entity.Position;
                sliderPosition.SetPosition(entity.PositionPercentage * 100);
            };
		}
		
        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            InvokeOnMainThread(() => {
                lblArtistName.StringValue = audioFile.ArtistName;
                lblAlbumTitle.StringValue = audioFile.AlbumTitle;
                lblSongTitle.StringValue = audioFile.Title;
                lblSongPath.StringValue = audioFile.FilePath;
                //lblPosition.StringValue = audioFile.Position;
                lblLength.StringValue = audioFile.Length;
                lblFileType.StringValue = audioFile.FileType.ToString();
                lblBitrate.StringValue = audioFile.Bitrate.ToString() + " kbit/s";
                lblBitsPerSample.StringValue = audioFile.BitsPerSample.ToString() + " bits";
                lblSampleRate.StringValue = audioFile.SampleRate.ToString() + " Hz";

                // Set album cover
                if (!String.IsNullOrEmpty(audioFile.FilePath))
                {
                    NSImage image = AlbumCoverHelper.GetAlbumCover(audioFile.FilePath);
                    if (image != null)
                        imageAlbumCover.Image = image;
                    else
                        imageAlbumCover.Image = new NSImage();
                } 
                else
                {
                    imageAlbumCover.Image = new NSImage();
                }

                if(_songBrowserSource != null)
                    _songBrowserSource.RefreshIsPlaying(tableSongBrowser, audioFile.FilePath);
            });
		}

        public void RefreshMarkers(IEnumerable<Marker> markers)
        {
        }

        public void RefreshLoops(IEnumerable<Loop> loops)
        {
        }

        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
        {
            InvokeOnMainThread(() => {
                lblVolume.StringValue = entity.VolumeString;
                if(sliderVolume.FloatValue != entity.Volume)
                    sliderVolume.FloatValue = entity.Volume;
            });
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {
            InvokeOnMainThread(() => {
//                lblTimeShifting.StingValue = entity.TimeShiftingString;
//                if(sliderTimeShifting.FloatValue != entity.TimeShifting)
//                    sliderTimeShifting.FloatValue = entity.TimeShifting;
            });
        }

        public void PlayerError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in the Player component: {0}", ex), NSAlertStyle.Critical);
            });
        }

        #endregion

		#region ISongBrowserView implementation

        public Action<AudioFile> OnTableRowDoubleClicked { get; set; }
        public Action<AudioFile> OnSongBrowserEditSongMetadata { get; set; }   
        public Action<string> OnSearchTerms { get; set; }

		public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
        {
            InvokeOnMainThread(() => {
                _songBrowserSource = new SongBrowserSource(audioFiles);
                tableSongBrowser.Source = _songBrowserSource;
                _albumCoverSource = new AlbumCoverSource(_albumCoverCacheService, audioFiles);
                tableAlbumCovers.Source = _albumCoverSource;
            });
		}

		#endregion

		#region ILibraryBrowserView implementation

        public System.Action<AudioFileFormat> OnAudioFileFormatFilterChanged { get; set; }
        public System.Action<LibraryBrowserEntity> OnTreeNodeSelected { get; set; }
        public System.Action<LibraryBrowserEntity, object> OnTreeNodeExpanded { get; set; }     
        public System.Action<LibraryBrowserEntity> OnTreeNodeDoubleClicked { get; set; }
        public Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable { get; set; }

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

		#endregion

        #region IMainView implementation

        public System.Action OnOpenPreferencesWindow { get; set; }
        public System.Action OnOpenEffectsWindow { get; set; }
        public System.Action OnOpenPlaylistWindow { get; set; }
        public System.Action OnOpenSyncWindow { get; set; }
        public Action<List<string>> OnAddFilesToLibrary { get; set; }
        public Action<string> OnAddFolderToLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }

        #endregion

        #region IPitchShiftingView implementation

        public Action<int> OnChangeKey { get; set; }
        public Action<int> OnSetInterval { get; set; }
        public Action OnResetInterval { get; set; }
        public Action OnIncrementInterval { get; set; }
        public Action OnDecrementInterval { get; set; }

        public void PitchShiftingError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in the PitchShifting component: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshKeys(List<Tuple<int, string>> keys)
        {
        }

        public void RefreshPitchShifting(PlayerPitchShiftingEntity entity)
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
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in the TimeShifting component: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshTimeShifting(PlayerTimeShiftingEntity entity)
        {
        }

        #endregion

        #region ILoopsView implementation

        public Action OnAddLoop { get; set; }
        public Action<Loop> OnEditLoop { get; set; }

        public void LoopError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in the Loops component: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshLoops(List<Loop> loops)
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
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in the Markers component: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshMarkers(List<Marker> markers)
        {
            InvokeOnMainThread(delegate {
                _markers = markers.ToList();
                tableMarkers.ReloadData();
            });
        }

        #endregion

	}
}
