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
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Presenters.Interfaces
{
	/// <summary>
	/// Player presenter interface.
	/// </summary>
    public interface IPlayerPresenter : IBasePresenter<IPlayerView>
	{
        void Dispose();
        
		void Play();
		void Play(IEnumerable<AudioFile> audioFiles);
		void Play(IEnumerable<string> filePaths);
		void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath);
		void Stop();
		void Pause();
		void Next();
		void Previous();
		void RepeatType();
  
        void SetPosition(float percentage);
        void SetVolume(float volume);
        void SetTimeShifting(float timeShifting);
	}
}

