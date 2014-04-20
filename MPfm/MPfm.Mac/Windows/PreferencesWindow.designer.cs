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
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabAudio { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabCloud { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabGeneral { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabLibrary { get; set; }

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
		MonoMac.AppKit.NSTextField lblGeneralPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGeneralUpdateFrequency { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryFolders { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMS { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMS2 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblOutputDevice { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStatusDescription { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblUpdatePeriod { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewAudioPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewCloudPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewGeneralPreferences { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewLibraryPreferences { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
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

			if (lblAudioPreferences != null) {
				lblAudioPreferences.Dispose ();
				lblAudioPreferences = null;
			}

			if (lblCloudPreferences != null) {
				lblCloudPreferences.Dispose ();
				lblCloudPreferences = null;
			}

			if (lblGeneralPreferences != null) {
				lblGeneralPreferences.Dispose ();
				lblGeneralPreferences = null;
			}

			if (lblLibraryPreferences != null) {
				lblLibraryPreferences.Dispose ();
				lblLibraryPreferences = null;
			}

			if (viewAudioPreferences != null) {
				viewAudioPreferences.Dispose ();
				viewAudioPreferences = null;
			}

			if (viewCloudPreferences != null) {
				viewCloudPreferences.Dispose ();
				viewCloudPreferences = null;
			}

			if (viewGeneralPreferences != null) {
				viewGeneralPreferences.Dispose ();
				viewGeneralPreferences = null;
			}

			if (viewLibraryPreferences != null) {
				viewLibraryPreferences.Dispose ();
				viewLibraryPreferences = null;
			}

			if (lblAudioOutput != null) {
				lblAudioOutput.Dispose ();
				lblAudioOutput = null;
			}

			if (lblAudioMixer != null) {
				lblAudioMixer.Dispose ();
				lblAudioMixer = null;
			}

			if (lblAudioStatus != null) {
				lblAudioStatus.Dispose ();
				lblAudioStatus = null;
			}

			if (lblGeneralUpdateFrequency != null) {
				lblGeneralUpdateFrequency.Dispose ();
				lblGeneralUpdateFrequency = null;
			}

			if (lblOutputDevice != null) {
				lblOutputDevice.Dispose ();
				lblOutputDevice = null;
			}

			if (lblSampleRate != null) {
				lblSampleRate.Dispose ();
				lblSampleRate = null;
			}

			if (lblBufferSize != null) {
				lblBufferSize.Dispose ();
				lblBufferSize = null;
			}

			if (lblUpdatePeriod != null) {
				lblUpdatePeriod.Dispose ();
				lblUpdatePeriod = null;
			}

			if (lblEvery != null) {
				lblEvery.Dispose ();
				lblEvery = null;
			}

			if (lblEvery2 != null) {
				lblEvery2.Dispose ();
				lblEvery2 = null;
			}

			if (lblMS != null) {
				lblMS.Dispose ();
				lblMS = null;
			}

			if (lblMS2 != null) {
				lblMS2.Dispose ();
				lblMS2 = null;
			}

			if (lblStatusDescription != null) {
				lblStatusDescription.Dispose ();
				lblStatusDescription = null;
			}

			if (lblLibraryFolders != null) {
				lblLibraryFolders.Dispose ();
				lblLibraryFolders = null;
			}

			if (lblCloudDropbox != null) {
				lblCloudDropbox.Dispose ();
				lblCloudDropbox = null;
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
