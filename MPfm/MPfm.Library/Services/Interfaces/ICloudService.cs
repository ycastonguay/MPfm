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
using System.Collections;
using System.Collections.Generic;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using MPfm.Library.Objects;

namespace MPfm.Library.Services.Interfaces
{
    public delegate void CloudAuthenticationStatusChanged(CloudAuthenticationStatusType statusType);
    public delegate void CloudAuthenticationFailed();
    public delegate void CloudDataChanged(string path, string data);

    /// <summary>
    /// Interface for the cloud service implementations.
    /// </summary>
    public interface ICloudService
    {
        event CloudAuthenticationStatusChanged OnCloudAuthenticationStatusChanged;
        event CloudAuthenticationFailed OnCloudAuthenticationFailed;
        event CloudDataChanged OnCloudDataChanged;

        bool HasLinkedAccount { get; }

        void LinkApp(object view);
        void ContinueLinkApp();
        void UnlinkApp();

        void CreateFolder(string path);
        List<string> ListFiles(string path); 
        void WatchFile(string path);
        byte[] DownloadFile(string path);
        void UploadFile(string path, byte[] data);
    }

    public enum CloudAuthenticationStatusType
    {
        GetRequestToken = 0,
        OpenWebBrowser = 1,
        RequestAccessToken = 2,
        ConnectToDropbox = 3,
        ConnectedToDropbox = 4
    }
}
