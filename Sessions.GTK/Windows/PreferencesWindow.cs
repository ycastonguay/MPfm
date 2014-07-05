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
using Sessions.MVP;
using Sessions.MVP.Views;
using System.Collections.Generic;
using Sessions.Sound.BassNetWrapper;
using Sessions.MVP.Config.Models;
using Sessions.MVP.Models;
using Sessions.Library.Objects;

namespace MPfm.GTK.Windows
{
	/// <summary>
	/// Settings window.
	/// </summary>
	public partial class PreferencesWindow : BaseWindow, IDesktopPreferencesView
	{		
		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.GTK.PreferencesWindow"/> class.
		/// </summary>
		/// <param name='main'>Reference to the main window.</param>
		public PreferencesWindow (Action<IBaseView> onViewReady) : 
				base(Gtk.WindowType.Toplevel, onViewReady)
		{
			this.Build();
			onViewReady(this);
            this.Center();
			this.Show();
		}
		
		/// <summary>
		/// Raises the delete event (when the form is closing).
		/// Prevents the form from closing by hiding it instead.
		/// </summary>
		/// <param name='o'>Object</param>
		/// <param name='args'>Event arguments</param>
		protected void OnDeleteEvent(object o, Gtk.DeleteEventArgs args)
		{
			// Prevent window from closing
			//args.RetVal = true;
			
			// Hide window instead
			//this.Hi
			Console.WriteLine("PreferencesWindow - OnDeleteEvent");
		}

        #region ILibraryPreferencesView implementation

        public System.Action OnSelectFolders { get; set; }
        public System.Action<string, bool> OnAddFolder { get; set; }
        public System.Action<IEnumerable<Folder>, bool> OnRemoveFolders { get; set; }
        public System.Action OnResetLibrary { get; set; }
        public System.Action OnUpdateLibrary { get; set; }
        public System.Action<bool> OnEnableSyncListener { get; set; }
        public Action<int> OnSetSyncListenerPort { get; set; }
        public Action<LibraryAppConfig> OnSetLibraryPreferences { get; set; }

        public void LibraryPreferencesError(Exception ex)
        {
        }

        public void RefreshLibraryPreferences(LibraryAppConfig config, string librarySize)
        {
        }

        #endregion

        #region IGeneralPreferencesView implementation

        public Action<GeneralAppConfig> OnSetGeneralPreferences { get; set; }
        public System.Action OnDeletePeakFiles { get; set; }

        public void GeneralPreferencesError(Exception ex)
        {
        }

        public void RefreshGeneralPreferences(GeneralAppConfig config, string peakFolderSize)
        {
        }

        #endregion

        #region ICloudPreferencesView implementation

        public Action<CloudAppConfig> OnSetCloudPreferences { get; set; }
        public System.Action OnDropboxLoginLogout { get; set; }

        public void CloudPreferencesError(Exception ex)
        {
        }

        public void RefreshCloudPreferences(CloudAppConfig config)
        {
        }

        public void RefreshCloudPreferencesState(CloudPreferencesStateEntity entity)
        {
        }

        #endregion

        #region IAudioPreferencesView implementation

        public Action<AudioAppConfig> OnSetAudioPreferences { get; set; }		
        public Action<Device, int> OnSetOutputDeviceAndSampleRate { get; set; }
        public Action OnResetAudioSettings { get; set; }
        public Func<bool> OnCheckIfPlayerIsPlaying { get; set; }

        public void AudioPreferencesError(Exception ex)
        {
        }

        public void RefreshAudioPreferences(AudioAppConfig config)
        {
        }
		
		public void RefreshAudioDevices(IEnumerable<Device> devices)
        {
        }

        #endregion
	}
}
