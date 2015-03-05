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
using Sessions.MVP.Config.Models;
using org.sessionsapp.player;

namespace Sessions.MVP.Views
{
	/// <summary>
    /// Audio preferences view interface.
	/// </summary>
    public interface IAudioPreferencesView : IBaseView
	{        
        Action<AudioAppConfig> OnSetAudioPreferences { get; set; }
        Action<SSPDevice, int> OnSetOutputDeviceAndSampleRate { get; set; }
        Action OnResetAudioSettings { get; set; }
        Func<bool> OnCheckIfPlayerIsPlaying { get; set; } 

        void AudioPreferencesError(Exception ex);
        void RefreshAudioPreferences(AudioAppConfig config);
	    void RefreshAudioDevices(IEnumerable<SSPDevice> devices);
	}
}
