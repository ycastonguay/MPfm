// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.Mac
{
	[Register ("PreferencesWindowController")]
	partial class PreferencesWindowController
	{
		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnAddFolder { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnLoginDropbox { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnRemoveFolder { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnResetAudioSettings { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnResetLibrary { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabAudio { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabCloud { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabGeneral { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabLibrary { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnTestAudioSettings { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton checkEnableResumePlayback { get; set; }

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
		MonoMac.AppKit.NSTextField lblEvery { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery2 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery3 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblEvery4 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralUpdateFrequency { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblHz { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryFolders { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryPreferences { get; set; }

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
		MonoMac.AppKit.NSTextField lblResumePlaybackNote { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStatusDescription { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblUpdatePeriod { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmPopUpButton popupOutputDevice { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmPopUpButton popupSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableFolders { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTrackBarView trackBufferSize { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTrackBarView trackOutputMeter { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTrackBarView trackSongPosition { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTrackBarView trackUpdatePeriod { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtBufferSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtOutputMeter { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtSongPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtUpdatePeriod { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewAudioPreferences { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewAudioPreferencesHeader { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewCloudPreferences { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewCloudPreferencesHeader { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewGeneralPreferences { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewGeneralPreferencesHeader { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewLibraryPreferences { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewLibraryPreferencesHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewMixerHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewOutputHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewStatusHeader { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAddFolder != null) {
				btnAddFolder.Dispose ();
				btnAddFolder = null;
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

			if (checkEnableResumePlayback != null) {
				checkEnableResumePlayback.Dispose ();
				checkEnableResumePlayback = null;
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

			if (lblGeneralPreferences != null) {
				lblGeneralPreferences.Dispose ();
				lblGeneralPreferences = null;
			}

			if (lblGeneralUpdateFrequency != null) {
				lblGeneralUpdateFrequency.Dispose ();
				lblGeneralUpdateFrequency = null;
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

			if (lblUpdatePeriod != null) {
				lblUpdatePeriod.Dispose ();
				lblUpdatePeriod = null;
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

			if (viewOutputHeader != null) {
				viewOutputHeader.Dispose ();
				viewOutputHeader = null;
			}

			if (viewMixerHeader != null) {
				viewMixerHeader.Dispose ();
				viewMixerHeader = null;
			}

			if (viewStatusHeader != null) {
				viewStatusHeader.Dispose ();
				viewStatusHeader = null;
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
