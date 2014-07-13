// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace Sessions.OSX
{
	[Register ("PreferencesWindowController")]
	partial class PreferencesWindowController
	{
		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnAddFolder { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnBrowseCustomDirectory { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnDeletePeakFiles { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnLoginDropbox { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnRemoveFolder { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnResetAudioSettings { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnResetLibrary { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTabButton btnTabAudio { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTabButton btnTabCloud { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTabButton btnTabGeneral { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTabButton btnTabLibrary { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnUpdateLibrary { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsCheckBoxView chkEnableLibraryService { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsCheckBoxView chkEnableResumePlayback { get; set; }

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
		MonoMac.AppKit.NSTextField lblBufferSizeValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblCloudDropbox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblCloudPreferences { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsLabel lblEnableLibraryService { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsLabel lblEnableResumePlayback { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery2 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery3 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery4 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralPeakFileDeletion { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralPeakFiles { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralUpdateFrequency { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblHttpPort { get; set; }

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
		MonoMac.AppKit.NSTextField lblMaximumPeakFolderSizeValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMB { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMixerNote { get; set; }

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
		MonoMac.AppKit.NSTextField lblOutputMeterValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPeakFileFolderSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblResumePlaybackNote { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongPositionValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblUpdateFrequencyWarning { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblUpdatePeriod { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblUpdatePeriodValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMatrix matrixPeakFiles { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsPopUpButton popupOutputDevice { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsPopUpButton popupSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableFolders { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackBufferSize { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackMaximumPeakFolderSize { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackOutputMeter { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackSongPosition { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackUpdatePeriod { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtCustomDirectory { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtHttpPort { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewAudioPreferences { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewAudioPreferencesHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewCloudPreferences { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewCloudPreferencesHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewDropboxHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewFoldersHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewGeneralPreferences { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewGeneralPreferencesHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLibraryPreferences { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLibraryPreferencesHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLibraryServiceHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewMixerHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewOutputHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewPeakFileDeletionHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewPeakFilesHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewStatusHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewUpdateFrequencyHeader { get; set; }

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

			if (chkEnableLibraryService != null) {
				chkEnableLibraryService.Dispose ();
				chkEnableLibraryService = null;
			}

			if (chkEnableResumePlayback != null) {
				chkEnableResumePlayback.Dispose ();
				chkEnableResumePlayback = null;
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

			if (lblEnableLibraryService != null) {
				lblEnableLibraryService.Dispose ();
				lblEnableLibraryService = null;
			}

			if (lblEnableResumePlayback != null) {
				lblEnableResumePlayback.Dispose ();
				lblEnableResumePlayback = null;
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

			if (lblSongPositionValue != null) {
				lblSongPositionValue.Dispose ();
				lblSongPositionValue = null;
			}

			if (lblOutputMeterValue != null) {
				lblOutputMeterValue.Dispose ();
				lblOutputMeterValue = null;
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

			if (btnUpdateLibrary != null) {
				btnUpdateLibrary.Dispose ();
				btnUpdateLibrary = null;
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

			if (lblMixerNote != null) {
				lblMixerNote.Dispose ();
				lblMixerNote = null;
			}

			if (txtCustomDirectory != null) {
				txtCustomDirectory.Dispose ();
				txtCustomDirectory = null;
			}

			if (txtHttpPort != null) {
				txtHttpPort.Dispose ();
				txtHttpPort = null;
			}

			if (lblMaximumPeakFolderSizeValue != null) {
				lblMaximumPeakFolderSizeValue.Dispose ();
				lblMaximumPeakFolderSizeValue = null;
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

			if (viewPeakFileDeletionHeader != null) {
				viewPeakFileDeletionHeader.Dispose ();
				viewPeakFileDeletionHeader = null;
			}

			if (viewUpdateFrequencyHeader != null) {
				viewUpdateFrequencyHeader.Dispose ();
				viewUpdateFrequencyHeader = null;
			}

			if (lblUpdatePeriodValue != null) {
				lblUpdatePeriodValue.Dispose ();
				lblUpdatePeriodValue = null;
			}

			if (lblGeneralPeakFileDeletion != null) {
				lblGeneralPeakFileDeletion.Dispose ();
				lblGeneralPeakFileDeletion = null;
			}

			if (lblBufferSizeValue != null) {
				lblBufferSizeValue.Dispose ();
				lblBufferSizeValue = null;
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
