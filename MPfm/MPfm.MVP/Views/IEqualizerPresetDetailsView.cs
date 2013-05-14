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
using MPfm.Player.Objects;

namespace MPfm.MVP.Views
{
	public interface IEqualizerPresetDetailsView : IBaseView
    {
        Action OnResetPreset { get; set; }
        Action OnNormalizePreset { get; set; }
        Action<EQPreset> OnSavePreset { get; set; }
        Action<string, float> OnSetFaderValue { get; set; }

        void EqualizerPresetDetailsError(Exception ex);
        void RefreshFaders(List<KeyValuePair<string, float>> values);
	}
}
