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
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.Android.Classes.Fragments
{
    public class PlayerMetadataFragment : BaseFragment, IPlayerMetadataView
    {        
        private View _view;
        private TextView _lblArtistName;
        private TextView _lblAlbumTitle;
        private TextView _lblSongTitle;

        // Leave an empty constructor or the application will crash at runtime
        public PlayerMetadataFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //Console.WriteLine("PlayerMetadataFragment - OnCreateView");
            _view = inflater.Inflate(Resource.Layout.PlayerMetadata, container, false);
            _lblArtistName = _view.FindViewById<TextView>(Resource.Id.playerMetadata_lblArtistName);
            _lblAlbumTitle = _view.FindViewById<TextView>(Resource.Id.playerMetadata_lblAlbumTitle);
            _lblSongTitle = _view.FindViewById<TextView>(Resource.Id.playerMetadata_lblSongTitle);
            return _view;
        }

        #region IPlayerMetadataView implementation

        public Action OnOpenPlaylist { get; set; }
        public Action OnToggleShuffle { get; set; }
        public Action OnToggleRepeat { get; set; }

        public void RefreshMetadata(AudioFile audioFile, int playlistIndex, int playlistCount)
        {            
            Activity.RunOnUiThread(() => {
                if (audioFile != null)
                {
                    //Console.WriteLine("PlayerMetadataFragment - RefreshAudioFile - {0}", audioFile.FilePath);
                    _lblArtistName.Text = audioFile.ArtistName;
                    _lblAlbumTitle.Text = audioFile.AlbumTitle;
                    _lblSongTitle.Text = audioFile.Title;
                }
                else
                {
                    //Console.WriteLine("PlayerMetadataFragment - RefreshAudioFile (null)");
                    _lblArtistName.Text = string.Empty;
                    _lblAlbumTitle.Text = string.Empty;
                    _lblSongTitle.Text = string.Empty;
                }
            });
        }

        public void RefreshShuffle(bool shuffle)
        {
        }

        public void RefreshRepeat(RepeatType repeatType)
        {
        }

        #endregion

    }
}
