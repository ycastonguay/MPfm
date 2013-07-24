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
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Cache;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.Android.Classes.Helpers;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using org.sessionsapp.android;
using Exception = System.Exception;

namespace MPfm.Android.Classes.Fragments
{
    public class MobileLibraryBrowserFragment : BaseFragment, IMobileLibraryBrowserView
    {
        private View _view;
        private ListView _listView;
        private GridView _gridView;
        private MobileLibraryBrowserListAdapter _listAdapter;
        private MobileLibraryBrowserGridAdapter _gridAdapter;
        private List<LibraryBrowserEntity> _entities = new List<LibraryBrowserEntity>();

        SquareImageView _imageAlbum;
        LinearLayout _layoutAlbum;
        TextView _lblArtistName;
        TextView _lblAlbumTitle;
        TextView _lblAlbumLength;
        TextView _lblAlbumSongCount;
        public BitmapCache BitmapCache { get; set; }

        // Leave an empty constructor or the application will crash at runtime
        public MobileLibraryBrowserFragment() : base(null)
        {
            //Console.WriteLine("MobileLibraryBrowserFragment - Empty ctor");
        }

        public MobileLibraryBrowserFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            //Console.WriteLine("MobileLibraryBrowserFragment - OnViewReady ctor");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Console.WriteLine("MLBFragment - OnCreateView");
            _view = inflater.Inflate(Resource.Layout.MobileLibraryBrowser, container, false);

            // Get screen size
            Point size = new Point();
            Activity.WindowManager.DefaultDisplay.GetSize(size);

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 8;
            BitmapCache = new BitmapCache(Activity, cacheSize, size.X / 2, size.X / 2); // Max size = half the screen (grid has 2 columns)

            _imageAlbum = _view.FindViewById<SquareImageView>(Resource.Id.mobileLibraryBrowser_imageAlbum);
            _layoutAlbum = _view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowser_layoutAlbum);
            _lblArtistName = _view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowser_lblArtistName);
            _lblAlbumTitle = _view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowser_lblAlbumTitle);
            _lblAlbumLength = _view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowser_lblAlbumLength);
            _lblAlbumSongCount = _view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowser_lblAlbumSongCount);
            _listView = _view.FindViewById<ListView>(Resource.Id.mobileLibraryBrowser_listView);
            _gridView = _view.FindViewById<GridView>(Resource.Id.mobileLibraryBrowser_gridView);
            _listView.Visibility = ViewStates.Gone;
            _gridView.Visibility = ViewStates.Gone;

            _listAdapter = new MobileLibraryBrowserListAdapter(Activity, _entities.ToList());
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;
            _listView.ItemLongClick += ListViewOnItemLongClick;

            _gridAdapter = new MobileLibraryBrowserGridAdapter(Activity, this, _gridView, _entities.ToList());
            _gridView.SetAdapter(_gridAdapter);
            _gridView.ItemClick += GridViewOnItemClick;
            _gridView.ItemLongClick += GridViewOnItemLongClick;

            //this.RetainInstance = true;

            //if (savedInstanceState != null)
            //{
            //    string saved = savedInstanceState.GetString("Test");
            //    Console.WriteLine("MLBFRAGMENT - ONCREATEVIEW - STATE: {0}", saved);
            //}

            return _view;
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            OnItemClick(itemClickEventArgs.Position);
        }

        private void ListViewOnItemLongClick(object sender, AdapterView.ItemLongClickEventArgs itemLongClickEventArgs)
        {
        }

        private void GridViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            OnItemClick(itemClickEventArgs.Position);
        }

        private void GridViewOnItemLongClick(object sender, AdapterView.ItemLongClickEventArgs itemLongClickEventArgs)
        {
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            //Console.WriteLine("MLBFRAGMENT - ON SAVE INSTANCE STATE");
            outState.PutString("Test", DateTime.Now.ToLongTimeString());
            base.OnSaveInstanceState(outState);
        }

        public override void OnResume()
        {
            //Console.WriteLine("MLBFragment - OnResume");
            base.OnResume();
        }

        public override void OnStart()
        {
            //Console.WriteLine("MLBFragment - OnStart");
            base.OnStart();
        }

        public override void OnStop()
        {
            //Console.WriteLine("MLBFragment - OnStop");
            base.OnStop();
        }

        public override void OnPause()
        {
            //Console.WriteLine("MLBFragment - OnPause");
            base.OnPause();
        }

        public override void OnDestroy()
        {
            //Console.WriteLine("MLBFragment - OnDestroy");            
            base.OnDestroy();
        }

        public override void OnDestroyView()
        {
            Console.WriteLine("MLBFragment - OnDestroyView");
            BitmapCache.Clear();
            base.OnDestroyView();
        }

        public override void OnDetach()
        {
            //Console.WriteLine("MLBFragment - OnDetach");
            base.OnDetach();
        }

        #region IMobileLibraryBrowserView implementation

        public MobileLibraryBrowserType BrowserType { get; set; }
        public string Filter { get; set; }
        public Action<int> OnItemClick { get; set; }
        public Action<int> OnDeleteItem { get; set; }
        public Action<string, string> OnRequestAlbumArt { get; set; }

        public void MobileLibraryBrowserError(Exception ex)
        {
            Activity.RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(Activity).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in MobileLibraryBrowser: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle, string navigationBarSubtitle)
        {
            Console.WriteLine("MLBF - RefreshLibraryBrowser - Count: {0} browserType: {1}", entities.Count(), browserType.ToString());
            Activity.RunOnUiThread(() => {
                _entities = entities.ToList();

                switch (browserType)
                {
                    case MobileLibraryBrowserType.Artists:
                        _layoutAlbum.Visibility = ViewStates.Gone;
                        _listView.Visibility = ViewStates.Visible;
                        _gridView.Visibility = ViewStates.Gone;

                        //Animation animation = AnimationUtils.LoadAnimation(Activity, Resource.Animation.fade_in);
                        //_listView.StartAnimation(animation);
                        break;
                    case MobileLibraryBrowserType.Albums:
                        _layoutAlbum.Visibility = ViewStates.Gone;
                        _listView.Visibility = ViewStates.Gone;
                        _gridView.Visibility = ViewStates.Visible;

                        //Animation animation2 = AnimationUtils.LoadAnimation(Activity, Resource.Animation.fade_in);
                        //_gridView.StartAnimation(animation2);

                        if (_gridView != null)
                            _gridAdapter.SetData(entities);
                        break;
                    case MobileLibraryBrowserType.Songs:
                        _layoutAlbum.Visibility = ViewStates.Visible;                        
                        _listView.Visibility = ViewStates.Visible;
                        _gridView.Visibility = ViewStates.Gone;                            

                        if (_entities.Count > 0)
                        {                            
                            var audioFile = _entities[0].AudioFile;
                            _lblArtistName.Text = audioFile.ArtistName;
                            _lblAlbumTitle.Text = audioFile.AlbumTitle;
                            _lblAlbumSongCount.Text = _entities.Count.ToString() + " songs";
                            _imageAlbum.SetImageBitmap(null);

                            Task.Factory.StartNew(() =>
                            {
                                string key = audioFile.ArtistName + "_" + audioFile.AlbumTitle;
                                Console.WriteLine("MobileLibraryFragment - Album art - key: {0}", key);
                                if (_imageAlbum.Tag == null || _imageAlbum.Tag.ToString().ToUpper() != key.ToUpper())
                                {
                                    Console.WriteLine("MobileLibraryFragment - Album art - key: {0} is different than tag {1} - Fetching album art...", key, (_imageAlbum.Tag == null) ? "null" : _imageAlbum.Tag.ToString());
                                    _imageAlbum.Tag = key;
                                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                                    if (bytesImage.Length == 0)
                                        _imageAlbum.SetImageBitmap(null);
                                    else
                                        //((MainActivity)Activity).BitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);
                                        BitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);
                                }
                            });
                        }
                        break;
                    case MobileLibraryBrowserType.Playlists:
                        _layoutAlbum.Visibility = ViewStates.Gone;
                        _listView.Visibility = ViewStates.Visible;
                        _gridView.Visibility = ViewStates.Gone;
                        break;
                }

                if (browserType != MobileLibraryBrowserType.Albums)
                {
                    if (_listView != null)
                        _listAdapter.SetData(_entities);
                }
            });
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
        }

        public void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData)
        {
            Activity.RunOnUiThread(() => {
                _gridAdapter.RefreshAlbumArtCell(artistName, albumTitle, albumArtData);
            });
        }

        #endregion

    }
}
