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
        private ListView _listViewArtists;
        private ListView _listViewSongs;
        private GridView _gridViewAlbums;
        private MobileLibraryBrowserListAdapter _listAdapterArtists;
        private MobileLibraryBrowserListAdapter _listAdapterSongs;
        private MobileLibraryBrowserGridAdapter _gridAdapter;
        private List<LibraryBrowserEntity> _entities = new List<LibraryBrowserEntity>();

        SquareImageView _imageAlbum;
        LinearLayout _layoutAlbum;
        LinearLayout _layoutSongs;
        TextView _lblBreadcrumb;
        TextView _lblArtistName;
        TextView _lblAlbumTitle;
        TextView _lblAlbumLength;
        TextView _lblAlbumSongCount;
        ViewFlipper _viewFlipper;
        ListView _listViewPlaylists;

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
            //Console.WriteLine("MLBFragment - OnCreateView");
            _view = inflater.Inflate(Resource.Layout.MobileLibraryBrowser, container, false);

            // Get screen size
            Point size = new Point();
            Activity.WindowManager.DefaultDisplay.GetSize(size);

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 8;
            BitmapCache = new BitmapCache(Activity, cacheSize, size.X / 2, size.X / 2); // Max size = half the screen (grid has 2 columns)

            _viewFlipper = _view.FindViewById<ViewFlipper>(Resource.Id.mobileLibraryBrowser_viewFlipper);            
            _imageAlbum = _view.FindViewById<SquareImageView>(Resource.Id.mobileLibraryBrowser_imageAlbum);
            _lblBreadcrumb = _view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowser_lblBreadcrumb);
            _layoutAlbum = _view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowser_layoutAlbum);
            _layoutSongs = _view.FindViewById<LinearLayout>(Resource.Id.mobileLibraryBrowser_layoutSongs);
            _lblArtistName = _view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowser_lblArtistName);
            _lblAlbumTitle = _view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowser_lblAlbumTitle);
            _lblAlbumLength = _view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowser_lblAlbumLength);
            _lblAlbumSongCount = _view.FindViewById<TextView>(Resource.Id.mobileLibraryBrowser_lblAlbumSongCount);
            _listViewArtists = _view.FindViewById<ListView>(Resource.Id.mobileLibraryBrowser_listViewArtists);
            _listViewSongs = _view.FindViewById<ListView>(Resource.Id.mobileLibraryBrowser_listViewSongs);
            _listViewPlaylists = _view.FindViewById<ListView>(Resource.Id.mobileLibraryBrowser_listViewPlaylists);
            _gridViewAlbums = _view.FindViewById<GridView>(Resource.Id.mobileLibraryBrowser_gridViewAlbums);
            //_listView.Visibility = ViewStates.Gone;
            //_gridView.Visibility = ViewStates.Gone;

            _listAdapterArtists = new MobileLibraryBrowserListAdapter(Activity, this, _listViewArtists, _entities.ToList());
            _listViewArtists.SetAdapter(_listAdapterArtists);
            _listViewArtists.ItemClick += ListViewOnItemClick;
            _listViewArtists.ItemLongClick += ListViewOnItemLongClick;

            _listAdapterSongs = new MobileLibraryBrowserListAdapter(Activity, this, _listViewSongs, _entities.ToList());
            _listViewSongs.SetAdapter(_listAdapterSongs);
            _listViewSongs.ItemClick += ListViewOnItemClick;
            _listViewSongs.ItemLongClick += ListViewOnItemLongClick;

            _gridAdapter = new MobileLibraryBrowserGridAdapter(Activity, this, _gridViewAlbums, _entities.ToList());
            _gridViewAlbums.SetAdapter(_gridAdapter);
            _gridViewAlbums.ItemClick += GridViewOnItemClick;
            _gridViewAlbums.ItemLongClick += GridViewOnItemLongClick;

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
            var listAdapter = (MobileLibraryBrowserListAdapter)((ListView) sender).Adapter;
            listAdapter.SetEditingRow(itemLongClickEventArgs.Position);
        }

        private void GridViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            // Reset long press buttons
            Console.WriteLine("MobileLibraryBrowserFragment - GridViewOnItemClick - position: {0}", itemClickEventArgs.Position);
            var gridAdapter = (MobileLibraryBrowserGridAdapter)((GridView)sender).Adapter;
            gridAdapter.ResetEditingRow();

            OnItemClick(itemClickEventArgs.Position);
        }

        private void GridViewOnItemLongClick(object sender, AdapterView.ItemLongClickEventArgs itemLongClickEventArgs)
        {
            Console.WriteLine("MobileLibraryBrowserFragment - GridViewOnItemLongClick - position: {0}", itemLongClickEventArgs.Position);
            var gridAdapter = (MobileLibraryBrowserGridAdapter)((GridView)sender).Adapter;
            gridAdapter.SetEditingRow(itemLongClickEventArgs.Position);
        }

        //public override void OnResume()
        //{
        //    //Console.WriteLine("MLBFragment - OnResume");
        //    base.OnResume();
        //}

        //public override void OnStart()
        //{
        //    //Console.WriteLine("MLBFragment - OnStart");
        //    base.OnStart();
        //}

        //public override void OnStop()
        //{
        //    //Console.WriteLine("MLBFragment - OnStop");
        //    base.OnStop();
        //}

        //public override void OnPause()
        //{
        //    //Console.WriteLine("MLBFragment - OnPause");
        //    base.OnPause();
        //}

        //public override void OnDestroy()
        //{
        //    //Console.WriteLine("MLBFragment - OnDestroy");            
        //    base.OnDestroy();
        //}

        public override void OnDestroyView()
        {
            Console.WriteLine("MLBFragment - OnDestroyView");
            BitmapCache.Clear();
            base.OnDestroyView();
        }

        //public override void OnDetach()
        //{
        //    //Console.WriteLine("MLBFragment - OnDetach");
        //    base.OnDetach();
        //}

        #region IMobileLibraryBrowserView implementation

        public Action<MobileLibraryBrowserType> OnChangeBrowserType { get; set; }
        public Action<int> OnItemClick { get; set; }
        public Action<int> OnDeleteItem { get; set; }
        public Action<int> OnPlayItem { get; set; }
        public Action<int> OnAddItemToPlaylist { get; set; }
        public Action<string, string> OnRequestAlbumArt { get; set; }
        public Func<string, string, byte[]> OnRequestAlbumArtSynchronously { get; set; }

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

        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle, string navigationBarSubtitle, string breadcrumb, bool isPopBackstack, bool isBackstackEmpty)
        {
            Console.WriteLine("MLBF - RefreshLibraryBrowser - Count: {0} browserType: {1}", entities.Count(), browserType.ToString());
            Activity.RunOnUiThread(() => {
                _entities = entities.ToList();
                _lblBreadcrumb.Text = breadcrumb;

                if (isPopBackstack)
                {
                    _viewFlipper.SetInAnimation(Activity, Resource.Animation.flipper_back_slide_in);
                    _viewFlipper.SetOutAnimation(Activity, Resource.Animation.flipper_back_slide_out);
                }
                else if (isBackstackEmpty)
                {
                    _viewFlipper.SetInAnimation(Activity, Resource.Animation.flipper_changetab_in);
                    _viewFlipper.SetOutAnimation(Activity, Resource.Animation.flipper_changetab_out);                    
                }
                else
                {
                    _viewFlipper.SetInAnimation(Activity, Resource.Animation.flipper_slide_in);
                    _viewFlipper.SetOutAnimation(Activity, Resource.Animation.flipper_slide_out);
                }

                switch (browserType)
                {
                    case MobileLibraryBrowserType.Artists:
                        int index = _viewFlipper.IndexOfChild(_listViewArtists);
                        _viewFlipper.DisplayedChild = index;
                        //_layoutAlbum.Visibility = ViewStates.Gone;
                        //_listViewArtists.Visibility = ViewStates.Visible;
                        //_gridViewAlbums.Visibility = ViewStates.Gone;

                        //Animation animation = AnimationUtils.LoadAnimation(Activity, Resource.Animation.fade_in);
                        //_listView.StartAnimation(animation);

                        if (_listViewArtists != null)
                        {
                            _listAdapterArtists.SetData(_entities);
                            _listViewArtists.SetSelection(0);
                        }
                        break;
                    case MobileLibraryBrowserType.Albums:
                        int index2 = _viewFlipper.IndexOfChild(_gridViewAlbums);
                        _viewFlipper.DisplayedChild = index2;
                        //_layoutAlbum.Visibility = ViewStates.Gone;
                        //_listViewArtists.Visibility = ViewStates.Gone;
                        //_gridViewAlbums.Visibility = ViewStates.Visible;

                        //Animation animation2 = AnimationUtils.LoadAnimation(Activity, Resource.Animation.fade_in);
                        //_gridView.StartAnimation(animation2);

                        if (_gridViewAlbums != null)
                            _gridAdapter.SetData(entities);
                        break;
                    case MobileLibraryBrowserType.Songs:
                        int index3 = _viewFlipper.IndexOfChild(_layoutSongs);
                        _viewFlipper.DisplayedChild = index3;
                        //_layoutAlbum.Visibility = ViewStates.Visible;
                        //_listViewArtists.Visibility = ViewStates.Visible;
                        //_gridViewAlbums.Visibility = ViewStates.Gone;                            

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
                                //Console.WriteLine("MobileLibraryFragment - Album art - key: {0}", key);
                                if (_imageAlbum.Tag == null || _imageAlbum.Tag.ToString().ToUpper() != key.ToUpper())
                                {
                                    //Console.WriteLine("MobileLibraryFragment - Album art - key: {0} is different than tag {1} - Fetching album art...", key, (_imageAlbum.Tag == null) ? "null" : _imageAlbum.Tag.ToString());
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

                        if (_listViewSongs != null)
                        {
                            _listAdapterSongs.SetData(_entities);
                            _listViewSongs.SetSelection(0);
                        }
                        break;
                    case MobileLibraryBrowserType.Playlists:
                        int index4 = _viewFlipper.IndexOfChild(_listViewPlaylists);
                        _viewFlipper.DisplayedChild = index4;
                        //_layoutAlbum.Visibility = ViewStates.Gone;
                        //_listViewArtists.Visibility = ViewStates.Visible;
                        //_gridViewAlbums.Visibility = ViewStates.Gone;
                        break;
                }

                //if (browserType != MobileLibraryBrowserType.Albums)
                //{
                //    if (_listViewArtists != null)
                //    {
                //        _listAdapter.SetData(_entities);
                //        _listViewArtists.SetSelection(0);
                //    }
                //}
            });
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
            Activity.RunOnUiThread(() => {
                _listAdapterSongs.SetNowPlayingRow(index, audioFile);
            });
        }

        public void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData)
        {
            Activity.RunOnUiThread(() => {
                _gridAdapter.RefreshAlbumArtCell(artistName, albumTitle, albumArtData);
            });
        }

        public void NotifyNewPlaylistItems(string text)
        {
            Activity.RunOnUiThread(() =>
            {
                var mainActivity = (MainActivity) Activity;
                mainActivity.ShowMiniPlayerSlide(1);
                Toast toast = Toast.MakeText(Activity, text, ToastLength.Short);
                toast.Show();             
            });            
        }

        #endregion

    }
}
