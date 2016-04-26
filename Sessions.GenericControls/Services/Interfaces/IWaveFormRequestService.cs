// Copyright � 2011-2013 Yanick Castonguay
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

using System.Threading.Tasks;
using Sessions.GenericControls.Basics;

namespace Sessions.GenericControls.Services.Interfaces
{
    public interface IWaveFormRequestService
    {
        int Count { get; }
        event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;

        void Flush();
        Task RequestBitmap(BasicRectangle boundsBitmap, BasicRectangle boundsWaveForm, float zoom, WaveFormDisplayType displayType);
    }
}