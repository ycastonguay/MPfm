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

namespace MPfm.Mac
{
    /// <summary>
    /// Main window controller.
    /// </summary>
    //[Register("NSWindow")]
	public partial class MainWindowController : BaseWindowController, IMainView
	{
        #region View Actions
        
        public System.Action OnOpenPreferencesWindow { get; set; }
        public System.Action OnOpenEffectsWindow { get; set; }
        public System.Action OnOpenPlaylistWindow { get; set; }
        public System.Action OnOpenSyncWindow { get; set; }
        public System.Action OnPlayerPlay { get; set; }
        public System.Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        public System.Action OnPlayerPause { get; set; }
        public System.Action OnPlayerStop { get; set; }
        public System.Action OnPlayerPrevious { get; set; }
        public System.Action OnPlayerNext { get; set; } 
        public System.Action<float> OnPlayerSetPitchShifting { get; set; }
        public System.Action<float> OnPlayerSetTimeShifting { get; set; }
        public System.Action<float> OnPlayerSetVolume { get; set; }
        public System.Action<float> OnPlayerSetPosition { get; set; }
        public Func<float, PlayerPositionEntity> OnPlayerRequestPosition { get; set; }
        
        public System.Action<AudioFileFormat> OnAudioFileFormatFilterChanged { get; set; }
        public System.Action<LibraryBrowserEntity> OnTreeNodeSelected { get; set; }
        public System.Action<LibraryBrowserEntity, object> OnTreeNodeExpanded { get; set; }     
        public System.Action<LibraryBrowserEntity> OnTreeNodeDoubleClicked { get; set; }
        public Func<LibraryBrowserEntity, IEnumerable<LibraryBrowserEntity>> OnTreeNodeExpandable { get; set; }
        
        public System.Action<AudioFile> OnTableRowDoubleClicked { get; set; }
        
        #endregion

		UpdateLibraryWindowController updateLibraryWindowController = null;
        LibraryBrowserOutlineViewDelegate libraryBrowserOutlineViewDelegate = null;
		LibraryBrowserDataSource libraryBrowserDataSource = null;
        SongBrowserTableViewDelegate songBrowserOutlineViewDelegate = null;
        SongBrowserSource songBrowserSource = null;
        AlbumCoverSource albumCoverSource = null;
        AlbumCoverCacheService albumCoverCacheService = null;

		//strongly typed window accessor00
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
		
		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindowController(IntPtr handle) 
            : base (handle)
		{
		}

		// Call to load from the XIB/NIB file
		public MainWindowController(Action<IBaseView> onViewReady) : base ("MainWindow", onViewReady)
        {
            // Set properties
            this.albumCoverCacheService = new AlbumCoverCacheService();

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
		}

		public override void AwakeFromNib()
        {
            // Set main window title
            this.Window.Title = "MPfm: Music Player for Musicians - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA";
            Tracing.Log("MainWindowController.AwakeFromNib -- Initializing user interface...");

            // Create split delegate
            splitMain.Delegate = new MainSplitViewDelegate();

            // Add items to Sound Format combo box
            cboSoundFormat.RemoveAllItems();
            cboSoundFormat.AddItem("All");
            cboSoundFormat.AddItem("FLAC");
            cboSoundFormat.AddItem("OGG");
            cboSoundFormat.AddItem("MP3");
            cboSoundFormat.AddItem("MPC");
            cboSoundFormat.AddItem("WAV");
            cboSoundFormat.AddItem("WV");

            // Initialize and configure Library Browser
            libraryBrowserOutlineViewDelegate = new LibraryBrowserOutlineViewDelegate((entity) => { OnTreeNodeSelected(entity); });
            outlineLibraryBrowser.Delegate = libraryBrowserOutlineViewDelegate;
            outlineLibraryBrowser.AllowsMultipleSelection = false;
            outlineLibraryBrowser.DoubleClick += HandleLibraryBrowserDoubleClick;

            // Initialize and configure Song Browser
            songBrowserOutlineViewDelegate = new SongBrowserTableViewDelegate();
            tableSongBrowser.Delegate = songBrowserOutlineViewDelegate;
            tableSongBrowser.AllowsMultipleSelection = true;
            tableSongBrowser.DoubleClick += HandleSongBrowserDoubleClick;

            // Load images and set theme
            LoadImages();
            SetTheme();

            // Create controllers
            //playlistWindowController = new PlaylistWindowController();
            //effectsWindowController = new EffectsWindowController();
            //preferencesWindowController = new PreferencesWindowController();

            tableAlbumCovers.FocusRingType = NSFocusRingType.None;
            tableSongBrowser.FocusRingType = NSFocusRingType.None;
            scrollViewSongBrowser.BorderType = NSBorderType.NoBorder;
            scrollViewAlbumCovers.BorderType = NSBorderType.NoBorder;
            scrollViewAlbumCovers.HasHorizontalScroller = false;
            scrollViewAlbumCovers.HasVerticalScroller = false;

            scrollViewAlbumCovers.SetSynchronizedScrollView(scrollViewSongBrowser);
            scrollViewSongBrowser.SetSynchronizedScrollView(scrollViewAlbumCovers);

            // Set view as ready
            OnViewReady.Invoke(this);
		}

        private void SetTheme()
        {
            //BackgroundColor = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewLeftHeader.GradientColor1 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewLeftHeader.GradientColor2 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewRightHeader.GradientColor1 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewRightHeader.GradientColor2 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewLoopsHeader.GradientColor1 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewLoopsHeader.GradientColor2 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewMarkersHeader.GradientColor1 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewMarkersHeader.GradientColor2 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewSongBrowserHeader.GradientColor1 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);
            viewSongBrowserHeader.GradientColor2 = new CGColor(62f/255f, 79f/255f, 91f/255f, 1);

            lblAlbumTitle.TextColor = NSColor.FromDeviceRgba(196f/255f, 213f/255f, 225f/255f, 1);
            //lblAlbumTitle.TextColor = NSColor.FromDeviceRgba(175f/255f, 206f/255f, 227f/255f, 1);
            //lblSongTitle.TextColor = NSColor.FromDeviceRgba(175f/255f, 206f/255f, 227f/255f, 1);
            //lblSongTitle.TextColor = NSColor.FromDeviceRgba(136f/255f, 174f/255f, 200f/255f, 1);
            lblSongTitle.TextColor = NSColor.FromDeviceRgba(171f/255f, 186f/255f, 196f/255f, 1);
            lblSongPath.TextColor = NSColor.FromDeviceRgba(97f/255f, 122f/255f, 140f/255f, 1);
//            viewLeftHeader.GradientColor1 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
//            viewLeftHeader.GradientColor2 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
//            viewRightHeader.GradientColor1 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
//            viewRightHeader.GradientColor2 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
//            viewLoopsHeader.GradientColor1 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
//            viewLoopsHeader.GradientColor2 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
//            viewMarkersHeader.GradientColor1 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
//            viewMarkersHeader.GradientColor2 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
//            viewSongBrowserHeader.GradientColor1 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);
//            viewSongBrowserHeader.GradientColor2 = new CGColor(0.2745f, 0.3490f, 0.4f, 1);

//            viewLeftHeader.GradientColor1 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            viewLeftHeader.GradientColor2 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            viewRightHeader.GradientColor1 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            viewRightHeader.GradientColor2 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            viewLoopsHeader.GradientColor1 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            viewLoopsHeader.GradientColor2 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            viewMarkersHeader.GradientColor1 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            viewMarkersHeader.GradientColor2 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            viewSongBrowserHeader.GradientColor1 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            viewSongBrowserHeader.GradientColor2 = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);

//            btnPlayLoop.BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            btnPlayLoop.BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnPlayLoop.BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnStopLoop.BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            btnStopLoop.BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnStopLoop.BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnAddLoop.BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            btnAddLoop.BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnAddLoop.BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnEditLoop.BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            btnEditLoop.BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnEditLoop.BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnRemoveLoop.BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            btnRemoveLoop.BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnRemoveLoop.BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//
//            btnGoToMarker.BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            btnGoToMarker.BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnGoToMarker.BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnAddMarker.BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            btnAddMarker.BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnAddMarker.BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnEditMarker.BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            btnEditMarker.BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnEditMarker.BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnRemoveMarker.BackgroundColor = new CGColor(0.9059f, 0.2980f, 0.2353f, 1);
//            btnRemoveMarker.BackgroundOverColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);
//            btnRemoveMarker.BorderColor = new CGColor(0.9559f, 0.3480f, 0.2853f, 1);

            viewInformation.IsHeaderVisible = true;
            viewSongPosition.IsHeaderVisible = true;
            viewVolume.IsHeaderVisible = true;
            viewTimeShifting.IsHeaderVisible = true;
            viewPitchShifting.IsHeaderVisible = true;           

            // Set label fonts
//            lblArtistName.Font = NSFont.FromFontName("TitilliumText25L-800wt", 22);
//            lblAlbumTitle.Font = NSFont.FromFontName("TitilliumText25L-600wt", 19);
//            lblSongTitle.Font = NSFont.FromFontName("TitilliumText25L-600wt", 16);
//            lblSongPath.Font = NSFont.FromFontName("TitilliumText25L-400wt", 12);
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
            lblSubtitleTimeShifting.Font = NSFont.FromFontName("TitilliumText25L-800wt", 12);
            lblSubtitleVolume.Font = NSFont.FromFontName("TitilliumText25L-800wt", 12);
            lblSubtitleInformation.Font = NSFont.FromFontName("TitilliumText25L-800wt", 12);
            lblSubtitlePitchShifting.Font = NSFont.FromFontName("TitilliumText25L-800wt", 12);

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
            btnStopLoop.Font = NSFont.FromFontName("Junction", 11f);
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
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarOpen").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_document-open");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarUpdateLibrary").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_view-refresh");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlay").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_media-playback-start");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPause").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_media-playback-pause");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarStop").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_media-playback-stop");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPrevious").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_media-skip-backward");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarNext").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_media-skip-forward");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarRepeat").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_view-refresh");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarEffects").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_preferences-desktop");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPlaylist").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_audio-x-generic");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarPreferences").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_preferences-system");
            toolbarMain.Items.FirstOrDefault(x => x.Identifier == "toolbarSync").Image = ImageResources.images32x32.FirstOrDefault(x => x.Name == "32_tango_network-wireless");

            // Load button images
            cboSoundFormat.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_plus");
            btnAddLoop.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_plus");
            btnAddMarker.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_plus");
            btnAddSongToPlaylist.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_plus");
            btnEditLoop.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_quill");
            btnEditMarker.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_quill");
            btnEditSongMetadata.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_quill");
            btnRemoveLoop.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_close");
            btnRemoveMarker.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_close");
            btnPlayLoop.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_play");
            btnPlaySelectedSong.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_play");
            btnStopLoop.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_stop");
            btnGoToMarker.Image = ImageResources.images16x16.FirstOrDefault(x => x.Name == "16_icomoon_arrow-right");
        }

		#endregion

		partial void actionAddFilesToLibrary(NSObject sender)
		{
			// Open panel to choose audio files
			IEnumerable<string> filePaths = null;
			using(NSOpenPanel openPanel = new NSOpenPanel())
			{
				openPanel.CanChooseDirectories = false;
				openPanel.CanChooseFiles = true;
				openPanel.ReleasedWhenClosed = true;
				openPanel.AllowsMultipleSelection = true;
                openPanel.AllowedFileTypes = new string[]{ "FLAC", "MP3", "OGG", "WAV", "MPC", "WV" };
                openPanel.Title = "Please select audio files to add to the library";
				openPanel.Prompt = "Add to library";
				openPanel.RunModal();

				filePaths = openPanel.Urls.Select(x => x.Path);
			}

			// Check if files were found
			if(filePaths != null && filePaths.Count() > 0)
			{
				StartUpdateLibrary(UpdateLibraryMode.SpecificFiles, filePaths.ToList(), null);
			}
		}

		partial void actionAddFolderLibrary(NSObject sender)
		{
			// Open panel to choose folder
			string folderPath = string.Empty;
			using(NSOpenPanel openPanel = new NSOpenPanel())
			{
				openPanel.CanChooseDirectories = true;
				openPanel.CanChooseFiles = false;
				openPanel.ReleasedWhenClosed = true;
				openPanel.AllowsMultipleSelection = false;
				openPanel.Title = "Please select a folder to add to the library";
				openPanel.Prompt = "Add to library";	
				openPanel.RunModal();

				folderPath = openPanel.Url.Path;
			}

			// Check if the folder is valid
			if(!String.IsNullOrEmpty(folderPath))
			{
				StartUpdateLibrary(UpdateLibraryMode.SpecificFolder, null, folderPath);
			}
		}

		partial void actionOpenAudioFiles(NSObject sender)
		{
			// Open panel to choose audio files to play
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

			// Check if files were found
			if(filePaths != null && filePaths.Count() > 0)
			{
				////playerPresenter.Player.Playlist.Clear();
				////playerPresenter.Player.Playlist.AddItems(filePaths.ToList());
				//playerPresenter.Play(filePaths);
                OnPlayerPlayFiles(filePaths);
			}
		}

		partial void actionUpdateLibrary(NSObject sender)
		{
			StartUpdateLibrary(UpdateLibraryMode.WholeLibrary, null, null);
		}

        partial void actionSoundFormatChanged(NSObject sender)
        {
            AudioFileFormat format;
            Enum.TryParse<AudioFileFormat>(cboSoundFormat.TitleOfSelectedItem, out format);
            OnAudioFileFormatFilterChanged(format);
        }

		partial void actionPlay(NSObject sender)
		{
            OnPlayerPlay();
		}

		partial void actionPause(NSObject sender)
		{
            OnPlayerPause();
		}

		partial void actionStop(NSObject sender)
		{
            OnPlayerStop();
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
			//playerPresenter.RepeatType();
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
            if(OnPlayerSetTimeShifting != null)
                OnPlayerSetTimeShifting.Invoke(sliderTimeShifting.FloatValue);

        }

        partial void actionChangeSongPosition(NSObject sender)
        {

        }

        partial void actionChangeVolume(NSObject sender)
        {
            if(OnPlayerSetVolume != null)
                OnPlayerSetVolume.Invoke(sliderVolume.FloatValue);
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
        }

        partial void actionAddMarker(NSObject sender)
        {
        }

        partial void actionEditMarker(NSObject sender)
        {
        }

        partial void actionRemoveMarker(NSObject sender)
        {
        }
        
        protected void HandleLibraryBrowserDoubleClick(object sender, EventArgs e)
        {
            // Check for selection
            if(outlineLibraryBrowser.SelectedRow == -1)
            {
                return;
            }

            try
            {
                // Get selected item and start playback
                Tracing.Log("MainWindowController.HandleLibraryBrowserDoubleClick -- Getting library browser item...");
                LibraryBrowserItem item = (LibraryBrowserItem)outlineLibraryBrowser.ItemAtRow(outlineLibraryBrowser.SelectedRow);
                Tracing.Log("MainWindowController.HandleLibraryBrowserDoubleClick -- Calling LibraryBrowserPresenter.TreeNodeDoubleClicked...");
                if(OnTreeNodeDoubleClicked != null)
                    OnTreeNodeDoubleClicked.Invoke(item.Entity);
            } 
            catch (Exception ex)
            {
                // Build text
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the Main Window Controller component (when double-clicking on an item in the Library Browser):");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);

                // Show alert
                Tracing.Log(sb.ToString());
                CocoaHelper.ShowCriticalAlert(sb.ToString());
            }
        }

        protected void HandleSongBrowserDoubleClick(object sender, EventArgs e)
        {
            // Check for selection
            if (tableSongBrowser.SelectedRow == -1)
            {
                return;
            }

            try
            {
                // Get selected item and start playback
                AudioFile audioFile = songBrowserSource.Items[tableSongBrowser.SelectedRow].AudioFile;
                if(OnTableRowDoubleClicked != null)
                    OnTableRowDoubleClicked.Invoke(audioFile);
            } 
            catch (Exception ex)
            {
                // Build text
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the Main Window Controller component (when double-clicking on an item in the Song Browser):");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);

                // Show alert
                Tracing.Log(sb.ToString());
                CocoaHelper.ShowCriticalAlert(sb.ToString());
            }
        }

        void StartUpdateLibrary(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
        {
            // Create window and start process
            if(updateLibraryWindowController != null) {
                updateLibraryWindowController.Dispose();
            }

            updateLibraryWindowController = new UpdateLibraryWindowController(this, null);
            updateLibraryWindowController.Window.MakeKeyAndOrderFront(this);
            updateLibraryWindowController.StartProcess(mode, filePaths, folderPath);
        }

        public void RefreshAll()
        {
            //libraryBrowserPresenter.AudioFileFormatFilterChanged(AudioFileFormat.All);
            //songBrowserPresenter.ChangeQuery(songBrowserPresenter.Query);

            if(OnAudioFileFormatFilterChanged != null)
                OnAudioFileFormatFilterChanged(AudioFileFormat.All);
        }

        #region IPlayerView implementation

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
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
            InvokeOnMainThread(delegate {
                // Set labels
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
                        imageAlbumCover.Image = null;//NSImage.ImageNamed("NSUser");
                } 
                else
                {
                    imageAlbumCover.Image = null;//NSImage.ImageNamed("NSUser");
                }

                // Refresh which song is playing in the Song Browser
                if(songBrowserSource != null)
                    songBrowserSource.RefreshIsPlaying(tableSongBrowser, audioFile.FilePath);
            });
		}

        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
        {
            InvokeOnMainThread(delegate {
                lblVolume.StringValue = entity.VolumeString;
                if(sliderVolume.FloatValue != entity.Volume)
                    sliderVolume.FloatValue = entity.Volume;
            });
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {
            InvokeOnMainThread(delegate {
//                lblTimeShifting.StringValue = entity.TimeShiftingString;
//                if(sliderTimeShifting.FloatValue != entity.TimeShifting)
//                    sliderTimeShifting.FloatValue = entity.TimeShifting;
            });
        }

        public void PlayerError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                // Build text
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the Player component:");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);

                // Show alert
                Tracing.Log(sb.ToString());
                CocoaHelper.ShowCriticalAlert(sb.ToString());
            });
        }

        #endregion

		#region ISongBrowserView implementation

		public void RefreshSongBrowser(IEnumerable<AudioFile> audioFiles)
        {
            InvokeOnMainThread(delegate {
                // Set data source
                songBrowserSource = new SongBrowserSource(audioFiles);
                tableSongBrowser.Source = songBrowserSource;
                albumCoverSource = new AlbumCoverSource(albumCoverCacheService, audioFiles);
                tableAlbumCovers.Source = albumCoverSource;
            });
		}

//        public void RefreshCurrentlyPlayingSong(AudioFile audioFile)
//        {
//            songBrowserSource.RefreshIsPlaying(tableSongBrowser, audioFile);
//        }

		#endregion

		#region ILibraryBrowserView implementation

		public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities)
		{
            InvokeOnMainThread(delegate {
                libraryBrowserDataSource = new LibraryBrowserDataSource(entities, (entity) => { return this.OnTreeNodeExpandable(entity); });
    			outlineLibraryBrowser.DataSource = libraryBrowserDataSource;
            });
		}

		public void RefreshLibraryBrowserNode(LibraryBrowserEntity entity, IEnumerable<LibraryBrowserEntity> entities, object userData)
		{
		    // Not used in Cocoa.
		}

		#endregion

	}
}

