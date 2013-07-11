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
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.Android.Classes.Fragments
{
    public class MarkersFragment : BaseFragment, IMarkersView
    {        
        private View _view;
        private ListView _listView;
        private Button _btnAdd;
        private MarkersListAdapter _listAdapter;
        private List<Marker> _markers; 

        // Leave an empty constructor or the application will crash at runtime
        public MarkersFragment() : base(null) { }

        public MarkersFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Markers, container, false);
            _listView = _view.FindViewById<ListView>(Resource.Id.markers_listView);
            _btnAdd = _view.FindViewById<Button>(Resource.Id.markers_btnAdd);
            _btnAdd.Click += (sender, args) => OnAddMarker(MarkerTemplateNameType.Verse);

            _listAdapter = new MarkersListAdapter(Activity, new List<Marker>());
            _listView.SetAdapter(_listAdapter);            
            _listView.ItemClick += ListViewOnItemClick;
            _listView.ItemLongClick += ListViewOnItemLongClick;
            
            return _view;
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            Console.WriteLine("MarkersFragment - ItemClick - itemPosition: {0}", itemClickEventArgs.Position);
            OnSelectMarker(_markers[itemClickEventArgs.Position]);
        }

        private void ListViewOnItemLongClick(object sender, AdapterView.ItemLongClickEventArgs itemLongClickEventArgs)
        {
            Console.WriteLine("MarkersFragment - ItemLongClick - itemPosition: {0}", itemLongClickEventArgs.Position);
            OnEditMarker(_markers[itemLongClickEventArgs.Position]);
        }

        #region IMarkersView implementation

        public Action<MarkerTemplateNameType> OnAddMarker { get; set; }
        public Action<Marker> OnEditMarker { get; set; }
        public Action<Marker> OnSelectMarker { get; set; }

        public void MarkerError(Exception ex)
        {
            Activity.RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(Activity).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in Markers: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        public void RefreshMarkers(List<Marker> markers)
        {
            Console.WriteLine("#####################>>> MarkersFragment - RefreshMarkers - markers count: {0}", markers.Count);
            Activity.RunOnUiThread(() => {
                _markers = markers;
                _listAdapter.SetData(markers);
            });
        }

        #endregion
    }
}
