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

using System.Collections.Generic;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Services.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.GenericControls.Services.Interfaces
{
    public interface IWaveFormCacheService
    {
        event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileBegunEvent;
        event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileProgressEvent;
        event WaveFormRenderingService.GeneratePeakFileEventHandler GeneratePeakFileEndedEvent;
        event WaveFormRenderingService.LoadPeakFileEventHandler LoadedPeakFileSuccessfullyEvent;
        event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapBegunEvent;
        event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;
        bool IsEmpty { get; }

        void FlushCache();
        void LoadPeakFile(AudioFile audioFile);
        WaveFormTile GetTile(float x, float height, float waveFormWidth, float zoom);
        List<WaveFormTile> GetTiles(int startTile, int endTile, int tileSize, BasicRectangle boundsWaveForm, float zoom);
    }
}