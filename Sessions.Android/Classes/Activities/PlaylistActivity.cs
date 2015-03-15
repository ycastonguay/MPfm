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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Android.Widget;
using Sessions.Android.Classes.Adapters;
using org.sessionsapp.android;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;
using Sessions.Sound.Player;

namespace Sessions.Android
{
    [Activity(Label = "Playlist", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class PlaylistActivity : BaseActivity, IPlaylistView
    {
        private MobileNavigationManager _navigationManager;
        Button _btnNew;
        Button _btnShuffle;
        CustomListView _listView;
        PlaylistItemListAdapter _itemListAdapter;
        Playlist _playlist;
        string _sourceActivityType;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("PlaylistActivity - OnCreate");
            base.OnCreate(bundle);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.Playlist);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _btnNew = FindViewById<Button>(Resource.Id.playlist_btnNew);
            _btnNew.Click += BtnNewOnClick;
            _btnShuffle = FindViewById<Button>(Resource.Id.playlist_btnNew);
            _btnShuffle.Click += BtnShuffleOnClick;
            
            _listView = FindViewById<CustomListView>(Resource.Id.playlist_listView);
            _itemListAdapter = new PlaylistItemListAdapter(this, _listView, new Playlist());
            _listView.SetAdapter(_itemListAdapter);
            _listView.ItemClick += ListViewOnItemClick;
            _listView.ItemLongClick += ListViewOnItemLongClick;

            // Save the source activity type for later (for providing Up navigation)
            _sourceActivityType = Intent.GetStringExtra("sourceActivity");

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            //((AndroidNavigationManager)_navigationManager).SetPlaylistActivityInstance(this);
            _navigationManager.BindPlaylistView(null, this);
        }

        //public override void OnAttachedToWindow()
        //{
        //    Console.WriteLine("PlaylistActivity - OnAttachedToWindow");
        //    var window = this.Window;
        //    window.AddFlags(WindowManagerFlags.ShowWhenLocked);
        //    window.SetWindowAnimations(0);
        //}

        private void BtnNewOnClick(object sender, EventArgs eventArgs)
        {
            OnNewPlaylist();
        }

        private void BtnShuffleOnClick(object sender, EventArgs eventArgs)
        {
            OnShufflePlaylist();
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            var item = _playlist.GetItemAt(itemClickEventArgs.Position);
            if (item != null)
            {
                //OnSelectPlaylistItem(item.Id);
            }
        }

        private void ListViewOnItemLongClick(object sender, AdapterView.ItemLongClickEventArgs itemLongClickEventArgs)
        {
            var listAdapter = (PlaylistItemListAdapter)((ListView)sender).Adapter;
            listAdapter.SetEditingRow(itemLongClickEventArgs.Position);
        }

        protected override void OnStart()
        {
            Console.WriteLine("PlaylistActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("PlaylistActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("PlaylistActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("PlaylistActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("PlaylistActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("PlaylistActivity - OnDestroy");
            base.OnDestroy();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    var type = Type.GetType(_sourceActivityType);
                    var intent = new Intent(this, type);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop); 
                    this.StartActivity(intent);
                    this.Finish();
                    return true;
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
                    break;
            }
        }

        #region IPlaylistView implementation

        public Action<Guid, int> OnChangePlaylistItemOrder { get; set; }
        public Action<Guid> OnSelectPlaylistItem { get; set; }
        public Action<List<Guid>> OnRemovePlaylistItems { get; set; }
        public Action OnNewPlaylist { get; set; }
        public Action<string> OnLoadPlaylist { get; set; }
        public Action OnSavePlaylist { get; set; }
        public Action OnShufflePlaylist { get; set; }

        public void PlaylistError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshPlaylist(Playlist playlist)
        {
            RunOnUiThread(() => {
                _playlist = playlist;
                _itemListAdapter.SetData(_playlist);
            });
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
            Console.WriteLine("PlaylistActivity - RefreshCurrentlyPlayingSong index: {0} audioFile: {1}", index, audioFile.FilePath);
            RunOnUiThread(() => {
                _itemListAdapter.SetNowPlayingRow(index, audioFile);
            });
        }

        #endregion

    }
}
