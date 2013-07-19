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
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;

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

        bool _isTest = false;

        // Leave an empty constructor or the application will crash at runtime
        public MobileLibraryBrowserFragment() : base(null)
        {
            Console.WriteLine("MobileLibraryBrowserFragment - Empty ctor");
        }

        public MobileLibraryBrowserFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            Console.WriteLine("MobileLibraryBrowserFragment - OnViewReady ctor");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Console.WriteLine("MLBFragment - OnCreateView - isTest: {0}", _isTest);
            _isTest = true;
            _view = inflater.Inflate(Resource.Layout.MobileLibraryBrowser, container, false);
            _listView = _view.FindViewById<ListView>(Resource.Id.mobileLibraryBrowser_listView);
            _listView.Visibility = ViewStates.Gone;
            _gridView = _view.FindViewById<GridView>(Resource.Id.mobileLibraryBrowser_gridView);
            _gridView.Visibility = ViewStates.Gone;

            _listAdapter = new MobileLibraryBrowserListAdapter(Activity, _entities.ToList());
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;
            _listView.ItemLongClick += ListViewOnItemLongClick;

            _gridAdapter = new MobileLibraryBrowserGridAdapter(Activity, this, _gridView, _entities.ToList());
            _gridView.SetAdapter(_gridAdapter);
            _gridView.ItemClick += GridViewOnItemClick;
            _gridView.ItemLongClick += GridViewOnItemLongClick;            

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
            Console.WriteLine("MLBFragment - OnSaveInstanceState");
            base.OnSaveInstanceState(outState);
        }

        public override void OnResume()
        {
            Console.WriteLine("MLBFragment - OnResume");
            base.OnResume();
        }

        public override void OnStart()
        {
            Console.WriteLine("MLBFragment - OnStart");
            base.OnStart();
        }

        public override void OnStop()
        {
            Console.WriteLine("MLBFragment - OnStop");
            base.OnStop();
        }

        public override void OnPause()
        {
            Console.WriteLine("MLBFragment - OnPause");
            base.OnPause();
        }

        public override void OnDestroy()
        {
            Console.WriteLine("MLBFragment - OnDestroy");
            base.OnDestroy();
        }

        public override void OnDestroyView()
        {
            Console.WriteLine("MLBFragment - OnDestroyView");
            base.OnDestroyView();
        }

        public override void OnDetach()
        {
            Console.WriteLine("MLBFragment - OnDetach");
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
            Console.WriteLine("MLBF - RefreshLibraryBrowser - Count: {0}", entities.Count());
            Activity.RunOnUiThread(() => {
                _entities = entities.ToList();

                switch (browserType)
                {
                    case MobileLibraryBrowserType.Artists:
                        _listView.Visibility = ViewStates.Visible;
                        break;
                    case MobileLibraryBrowserType.Albums:
                        _gridView.Visibility = ViewStates.Visible;
                        break;
                    case MobileLibraryBrowserType.Songs:
                        _listView.Visibility = ViewStates.Visible;
                        break;
                    case MobileLibraryBrowserType.Playlists:
                        _listView.Visibility = ViewStates.Visible;
                        break;
                }

                if (browserType == MobileLibraryBrowserType.Albums)
                {
                    if (_gridView != null)
                    {
                        _gridAdapter.SetData(entities);
                        _gridAdapter.NotifyDataSetChanged();
                    }                                       
                }
                else
                {
                    if (_listView != null)
                    {
                        _listAdapter.SetData(entities);
                        _listAdapter.NotifyDataSetChanged();
                    }
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
