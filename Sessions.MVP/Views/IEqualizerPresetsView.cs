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
using Sessions.Player.Objects;
using org.sessionsapp.player;

namespace Sessions.MVP.Views
{
	public interface IEqualizerPresetsView : IBaseView
	{
        Action OnBypassEqualizer { get; set; }
        Action<float> OnSetVolume { get; set; }
        Action OnAddPreset { get; set; }
        Action<Guid> OnLoadPreset { get; set; }
        Action<Guid> OnEditPreset { get; set; }
        Action<Guid> OnDeletePreset { get; set; }
        Action<Guid> OnDuplicatePreset { get; set; }
        Action<Guid, string> OnExportPreset { get; set; }

        void EqualizerPresetsError(Exception ex);
        void RefreshPresets(IEnumerable<SSPEQPreset> presets, Guid selectedPresetId, bool isEQBypassed);
        void RefreshOutputMeter(float[] dataLeft, float[] dataRight);
        void RefreshVolume(float volume);
	}
}
