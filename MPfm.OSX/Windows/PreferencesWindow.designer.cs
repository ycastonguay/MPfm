// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.OSX
{
	[Register ("PreferencesWindowController")]
	partial class PreferencesWindowController
	{
		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnAddFolder { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnBrowseCustomDirectory { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnDeletePeakFiles { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnLoginDropbox { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnRemoveFolder { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnResetAudioSettings { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnResetLibrary { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmTabButton btnTabAudio { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmTabButton btnTabCloud { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmTabButton btnTabGeneral { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmTabButton btnTabLibrary { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnTestAudioSettings { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmCheckBoxView chkEnableLibraryService { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmCheckBoxView chkEnableResumePlayback { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAudioMixer { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAudioOutput { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAudioPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAudioStatus { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblBufferSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblCloudDropbox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblCloudPreferences { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmLabel lblEnableLibraryService { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmLabel lblEnableResumePlayback { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery2 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery3 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery4 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralPeakFiles { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralUpdateFrequency { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblHttpPort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblHz { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryFolders { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryService { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryServiceNote { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibrarySize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMaximumPeakFolderSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMB { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMS { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMS2 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMS3 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMS4 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblOutputDevice { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblOutputMeter { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPeakFileFolderSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblResumePlaybackNote { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStatusDescription { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblUpdateFrequencyWarning { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblUpdatePeriod { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMatrix matrixPeakFiles { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmPopUpButton popupOutputDevice { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmPopUpButton popupSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableFolders { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmTrackBarView trackBufferSize { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmTrackBarView trackMaximumPeakFolderSize { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmTrackBarView trackOutputMeter { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmTrackBarView trackSongPosition { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmTrackBarView trackUpdatePeriod { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtBufferSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtCustomDirectory { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtHttpPort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtMaximumPeakFolderSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtOutputMeter { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtSongPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtUpdatePeriod { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewAudioPreferences { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewAudioPreferencesHeader { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewCloudPreferences { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewCloudPreferencesHeader { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewDropboxHeader { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewFoldersHeader { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewGeneralPreferences { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewGeneralPreferencesHeader { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewLibraryPreferences { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewLibraryPreferencesHeader { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewLibraryServiceHeader { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewMixerHeader { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewOutputHeader { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewPeakFilesHeader { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewStatusHeader { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewUpdateFrequencyHeader { get; set; }

		[Action ("actionEnableLibraryService:")]
		partial void actionEnableLibraryService (MonoMac.Foundation.NSObject sender);

		[Action ("actionEnableResumePlayback:")]
		partial void actionEnableResumePlayback (MonoMac.Foundation.NSObject sender);

		[Action ("actionMatrixPeakFiles:")]
		partial void actionMatrixPeakFiles (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAddFolder != null) {
				btnAddFolder.Dispose ();
				btnAddFolder = null;
			}

			if (btnBrowseCustomDirectory != null) {
				btnBrowseCustomDirectory.Dispose ();
				btnBrowseCustomDirectory = null;
			}

			if (btnDeletePeakFiles != null) {
				btnDeletePeakFiles.Dispose ();
				btnDeletePeakFiles = null;
			}

			if (btnLoginDropbox != null) {
				btnLoginDropbox.Dispose ();
				btnLoginDropbox = null;
			}

			if (btnRemoveFolder != null) {
				btnRemoveFolder.Dispose ();
				btnRemoveFolder = null;
			}

			if (btnResetAudioSettings != null) {
				btnResetAudioSettings.Dispose ();
				btnResetAudioSettings = null;
			}

			if (btnResetLibrary != null) {
				btnResetLibrary.Dispose ();
				btnResetLibrary = null;
			}

			if (btnTabAudio != null) {
				btnTabAudio.Dispose ();
				btnTabAudio = null;
			}

			if (btnTabCloud != null) {
				btnTabCloud.Dispose ();
				btnTabCloud = null;
			}

			if (btnTabGeneral != null) {
				btnTabGeneral.Dispose ();
				btnTabGeneral = null;
			}

			if (btnTabLibrary != null) {
				btnTabLibrary.Dispose ();
				btnTabLibrary = null;
			}

			if (btnTestAudioSettings != null) {
				btnTestAudioSettings.Dispose ();
				btnTestAudioSettings = null;
			}

			if (chkEnableLibraryService != null) {
				chkEnableLibraryService.Dispose ();
				chkEnableLibraryService = null;
			}

			if (lblAudioMixer != null) {
				lblAudioMixer.Dispose ();
				lblAudioMixer = null;
			}

			if (lblAudioOutput != null) {
				lblAudioOutput.Dispose ();
				lblAudioOutput = null;
			}

			if (lblAudioPreferences != null) {
				lblAudioPreferences.Dispose ();
				lblAudioPreferences = null;
			}

			if (lblAudioStatus != null) {
				lblAudioStatus.Dispose ();
				lblAudioStatus = null;
			}

			if (lblBufferSize != null) {
				lblBufferSize.Dispose ();
				lblBufferSize = null;
			}

			if (lblCloudDropbox != null) {
				lblCloudDropbox.Dispose ();
				lblCloudDropbox = null;
			}

			if (lblCloudPreferences != null) {
				lblCloudPreferences.Dispose ();
				lblCloudPreferences = null;
			}

			if (lblEvery != null) {
				lblEvery.Dispose ();
				lblEvery = null;
			}

			if (lblEvery2 != null) {
				lblEvery2.Dispose ();
				lblEvery2 = null;
			}

			if (lblEvery3 != null) {
				lblEvery3.Dispose ();
				lblEvery3 = null;
			}

			if (lblEvery4 != null) {
				lblEvery4.Dispose ();
				lblEvery4 = null;
			}

			if (lblGeneralPeakFiles != null) {
				lblGeneralPeakFiles.Dispose ();
				lblGeneralPeakFiles = null;
			}

			if (lblGeneralPreferences != null) {
				lblGeneralPreferences.Dispose ();
				lblGeneralPreferences = null;
			}

			if (lblGeneralUpdateFrequency != null) {
				lblGeneralUpdateFrequency.Dispose ();
				lblGeneralUpdateFrequency = null;
			}

			if (lblHttpPort != null) {
				lblHttpPort.Dispose ();
				lblHttpPort = null;
			}

			if (lblHz != null) {
				lblHz.Dispose ();
				lblHz = null;
			}

			if (lblLibraryFolders != null) {
				lblLibraryFolders.Dispose ();
				lblLibraryFolders = null;
			}

			if (lblLibraryPreferences != null) {
				lblLibraryPreferences.Dispose ();
				lblLibraryPreferences = null;
			}

			if (lblLibraryService != null) {
				lblLibraryService.Dispose ();
				lblLibraryService = null;
			}

			if (lblLibraryServiceNote != null) {
				lblLibraryServiceNote.Dispose ();
				lblLibraryServiceNote = null;
			}

			if (lblLibrarySize != null) {
				lblLibrarySize.Dispose ();
				lblLibrarySize = null;
			}

			if (lblMaximumPeakFolderSize != null) {
				lblMaximumPeakFolderSize.Dispose ();
				lblMaximumPeakFolderSize = null;
			}

			if (lblMB != null) {
				lblMB.Dispose ();
				lblMB = null;
			}

			if (lblMS != null) {
				lblMS.Dispose ();
				lblMS = null;
			}

			if (lblMS2 != null) {
				lblMS2.Dispose ();
				lblMS2 = null;
			}

			if (lblMS3 != null) {
				lblMS3.Dispose ();
				lblMS3 = null;
			}

			if (lblMS4 != null) {
				lblMS4.Dispose ();
				lblMS4 = null;
			}

			if (lblOutputDevice != null) {
				lblOutputDevice.Dispose ();
				lblOutputDevice = null;
			}

			if (lblOutputMeter != null) {
				lblOutputMeter.Dispose ();
				lblOutputMeter = null;
			}

			if (lblPeakFileFolderSize != null) {
				lblPeakFileFolderSize.Dispose ();
				lblPeakFileFolderSize = null;
			}

			if (lblResumePlaybackNote != null) {
				lblResumePlaybackNote.Dispose ();
				lblResumePlaybackNote = null;
			}

			if (lblSampleRate != null) {
				lblSampleRate.Dispose ();
				lblSampleRate = null;
			}

			if (lblSongPosition != null) {
				lblSongPosition.Dispose ();
				lblSongPosition = null;
			}

			if (lblStatusDescription != null) {
				lblStatusDescription.Dispose ();
				lblStatusDescription = null;
			}

			if (lblUpdateFrequencyWarning != null) {
				lblUpdateFrequencyWarning.Dispose ();
				lblUpdateFrequencyWarning = null;
			}

			if (lblUpdatePeriod != null) {
				lblUpdatePeriod.Dispose ();
				lblUpdatePeriod = null;
			}

			if (matrixPeakFiles != null) {
				matrixPeakFiles.Dispose ();
				matrixPeakFiles = null;
			}

			if (popupOutputDevice != null) {
				popupOutputDevice.Dispose ();
				popupOutputDevice = null;
			}

			if (popupSampleRate != null) {
				popupSampleRate.Dispose ();
				popupSampleRate = null;
			}

			if (tableFolders != null) {
				tableFolders.Dispose ();
				tableFolders = null;
			}

			if (trackBufferSize != null) {
				trackBufferSize.Dispose ();
				trackBufferSize = null;
			}

			if (trackMaximumPeakFolderSize != null) {
				trackMaximumPeakFolderSize.Dispose ();
				trackMaximumPeakFolderSize = null;
			}

			if (trackOutputMeter != null) {
				trackOutputMeter.Dispose ();
				trackOutputMeter = null;
			}

			if (trackSongPosition != null) {
				trackSongPosition.Dispose ();
				trackSongPosition = null;
			}

			if (trackUpdatePeriod != null) {
				trackUpdatePeriod.Dispose ();
				trackUpdatePeriod = null;
			}

			if (txtBufferSize != null) {
				txtBufferSize.Dispose ();
				txtBufferSize = null;
			}

			if (txtCustomDirectory != null) {
				txtCustomDirectory.Dispose ();
				txtCustomDirectory = null;
			}

			if (txtHttpPort != null) {
				txtHttpPort.Dispose ();
				txtHttpPort = null;
			}

			if (chkEnableResumePlayback != null) {
				chkEnableResumePlayback.Dispose ();
				chkEnableResumePlayback = null;
			}

			if (txtMaximumPeakFolderSize != null) {
				txtMaximumPeakFolderSize.Dispose ();
				txtMaximumPeakFolderSize = null;
			}

			if (txtOutputMeter != null) {
				txtOutputMeter.Dispose ();
				txtOutputMeter = null;
			}

			if (txtSongPosition != null) {
				txtSongPosition.Dispose ();
				txtSongPosition = null;
			}

			if (txtUpdatePeriod != null) {
				txtUpdatePeriod.Dispose ();
				txtUpdatePeriod = null;
			}

			if (lblEnableResumePlayback != null) {
				lblEnableResumePlayback.Dispose ();
				lblEnableResumePlayback = null;
			}

			if (viewAudioPreferences != null) {
				viewAudioPreferences.Dispose ();
				viewAudioPreferences = null;
			}

			if (viewAudioPreferencesHeader != null) {
				viewAudioPreferencesHeader.Dispose ();
				viewAudioPreferencesHeader = null;
			}

			if (viewCloudPreferences != null) {
				viewCloudPreferences.Dispose ();
				viewCloudPreferences = null;
			}

			if (viewCloudPreferencesHeader != null) {
				viewCloudPreferencesHeader.Dispose ();
				viewCloudPreferencesHeader = null;
			}

			if (viewDropboxHeader != null) {
				viewDropboxHeader.Dispose ();
				viewDropboxHeader = null;
			}

			if (viewFoldersHeader != null) {
				viewFoldersHeader.Dispose ();
				viewFoldersHeader = null;
			}

			if (viewGeneralPreferences != null) {
				viewGeneralPreferences.Dispose ();
				viewGeneralPreferences = null;
			}

			if (viewGeneralPreferencesHeader != null) {
				viewGeneralPreferencesHeader.Dispose ();
				viewGeneralPreferencesHeader = null;
			}

			if (viewLibraryPreferences != null) {
				viewLibraryPreferences.Dispose ();
				viewLibraryPreferences = null;
			}

			if (viewLibraryPreferencesHeader != null) {
				viewLibraryPreferencesHeader.Dispose ();
				viewLibraryPreferencesHeader = null;
			}

			if (viewLibraryServiceHeader != null) {
				viewLibraryServiceHeader.Dispose ();
				viewLibraryServiceHeader = null;
			}

			if (viewMixerHeader != null) {
				viewMixerHeader.Dispose ();
				viewMixerHeader = null;
			}

			if (viewOutputHeader != null) {
				viewOutputHeader.Dispose ();
				viewOutputHeader = null;
			}

			if (viewPeakFilesHeader != null) {
				viewPeakFilesHeader.Dispose ();
				viewPeakFilesHeader = null;
			}

			if (viewStatusHeader != null) {
				viewStatusHeader.Dispose ();
				viewStatusHeader = null;
			}

			if (viewUpdateFrequencyHeader != null) {
				viewUpdateFrequencyHeader.Dispose ();
				viewUpdateFrequencyHeader = null;
			}

			if (lblEnableLibraryService != null) {
				lblEnableLibraryService.Dispose ();
				lblEnableLibraryService = null;
			}
		}
	}

	[Register ("PreferencesWindow")]
	partial class PreferencesWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
