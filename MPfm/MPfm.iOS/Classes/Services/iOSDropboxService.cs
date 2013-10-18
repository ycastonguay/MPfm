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

using System.Reflection;
using System.Diagnostics;
using MonoTouch.Foundation;
using MPfm.Library.Services.Interfaces;

namespace MPfm.iOS.Classes.Services
{
	public class iOSDropboxService : IDropboxService
	{
        public bool HasLinkedAccount
        {
            get
            {
                return false;
            }
        }

        public event DropboxDataChanged OnDropboxDataChanged;

        public iOSDropboxService()
        {
        }

        private void Initialize()
        {
        }

        public void LinkApp(object view)
        {
        }

        public void UnlinkApp()
        {
        }

        public void PushStuff()
        {
        }

        public string PullStuff()
        {
            return string.Empty;
        }

        public void DeleteStuff()
        {
        }

        public string PushNowPlaying(MPfm.Sound.AudioFiles.AudioFile audioFile, long positionBytes, string position)
        {
            return string.Empty;
        }

        public string PullNowPlaying()
        {
            return string.Empty;
        }

        public void DeleteNowPlaying()
        {
        }
	}
}
