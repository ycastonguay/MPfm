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
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Objects;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;

namespace MPfm.Android.Classes.Fragments
{
    public class MobileLibraryBrowserFragment : BaseListFragment, IMobileLibraryBrowserView
    {
        private View _view;
        private Button _button;
        private EditText _editText;
        private IEnumerable<LibraryBrowserEntity> _entities = new List<LibraryBrowserEntity>();

        // Leave an empty constructor or the application will crash at runtime
        public MobileLibraryBrowserFragment() : base(null)
        {
            Console.WriteLine("MobileLibraryBrowserFragment - Empty constructor");
        }

        public MobileLibraryBrowserFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, global::Android.OS.Bundle savedInstanceState)
        {
            ListAdapter = new MobileLibraryBrowserListAdapter(Activity, _entities.ToList());
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            OnItemClick(position);
        }

        public void OnClick(View v)
        {
            if (v == null)
                return;

            var builder = new AlertDialog.Builder(Activity);
            //builder.SetIconAttribute(Android.Resource.Attribute.icon)
            builder.SetTitle("Yeah");
            //builder.SetMessage("You have typed: " + _editText.Text);
            //builder.SetPositiveButton("Yes");// this);
            var dialog = builder.Create();
            builder.Show();
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

        public override void OnDestroyView()
        {
            Console.WriteLine("MLBFragment - OnDestroyView");
            base.OnDestroyView();
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

        public override void OnSaveInstanceState(global::Android.OS.Bundle outState)
        {
            Console.WriteLine("MLBFragment - OnSaveInstanceState");
            base.OnSaveInstanceState(outState);
        }

        #region IMobileLibraryBrowserView implementation

        public MobileLibraryBrowserType BrowserType { get; set; }
        public string Filter { get; set; }
        public Action<int> OnItemClick { get; set; }
        public Action<int> OnDeleteItem { get; set; }
        public Action<string, string> OnRequestAlbumArt { get; set; }

        public void MobileLibraryBrowserError(Exception ex)
        {
        }

        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle, string navigationBarSubtitle)
        {
            Console.WriteLine("MLBF - RefreshLibraryBrowser - Count: {0}", entities.Count());
            Activity.RunOnUiThread(() => {
                _entities = entities;
                var listAdapter = (MobileLibraryBrowserListAdapter)ListAdapter;

                // Update list adapter only if the view was created
                if (listAdapter != null)
                {
                    // http://stackoverflow.com/questions/6837397/updating-listview-by-notifydatasetchanged-has-to-use-runonuithread
                    Activity.RunOnUiThread(() =>
                    {
                        listAdapter.SetData(entities);
                        listAdapter.NotifyDataSetChanged();
                    });
                }
            });
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
        }

        public void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData)
        {
        }

        #endregion

    }
}
