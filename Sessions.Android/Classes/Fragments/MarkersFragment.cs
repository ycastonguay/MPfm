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
using Android.OS;
using Android.Views;
using Android.Widget;
using Sessions.Android.Classes.Adapters;
using Sessions.Android.Classes.Fragments.Base;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters;
using Sessions.MVP.Views;
using Sessions.Player.Objects;

namespace Sessions.Android.Classes.Fragments
{
    public class MarkersFragment : BaseFragment, IMarkersView
    {        
        private View _view;
        private ListView _listView;
        private Button _btnAdd;
        private MarkersListAdapter _listAdapter;
        private List<Marker> _markers; 

        // Leave an empty constructor or the application will crash at runtime
        public MarkersFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Markers, container, false);
            _listView = _view.FindViewById<ListView>(Resource.Id.markers_listView);
            _btnAdd = _view.FindViewById<Button>(Resource.Id.markers_btnAdd);
            _btnAdd.Click += (sender, args) => OnAddMarker();

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
            _listView.ClearFocus();
        }

        private void ListViewOnItemLongClick(object sender, AdapterView.ItemLongClickEventArgs itemLongClickEventArgs)
        {
            Console.WriteLine("MarkersFragment - ItemLongClick - itemPosition: {0}", itemLongClickEventArgs.Position);
            OnEditMarker(_markers[itemLongClickEventArgs.Position]);
        }

        public override void OnResume()
        {
            Console.WriteLine("MarkersFragment - OnResume");
            base.OnResume();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindMarkersView(this);
        }

        public override void OnStart()
        {
            Console.WriteLine("MarkersFragment - OnStart");
            base.OnStart();
        }

        public override void OnStop()
        {
            Console.WriteLine("MarkersFragment - OnStop");
            base.OnStop();
        }

        public override void OnPause()
        {
            Console.WriteLine("MarkersFragment - OnPause");
            base.OnPause();
        }

        public override void OnDestroy()
        {
            Console.WriteLine("MarkersFragment - OnDestroy");            
            base.OnDestroy();
        }

        public override void OnDestroyView()
        {
            Console.WriteLine("MarkersFragment - OnDestroyView");
            base.OnDestroyView();
        }

        public override void OnDetach()
        {
            Console.WriteLine("MarkersFragment - OnDetach");
            base.OnDetach();
        }

        #region IMarkersView implementation

        public Action OnAddMarker { get; set; }
        public Action<MarkerTemplateNameType> OnAddMarkerWithTemplate { get; set; }
        public Action<Marker> OnEditMarker { get; set; }
        public Action<Marker> OnSelectMarker { get; set; }
        public Action<Marker> OnDeleteMarker { get; set; }
        public Action<Marker> OnUpdateMarker { get; set; }
        public Action<Guid> OnPunchInMarker { get; set; }
        public Action<Guid> OnUndoMarker { get; set; }
        public Action<Guid> OnSetActiveMarker { get; set; }
        public Action<Guid, string> OnChangeMarkerName { get; set; }
        public Action<Guid, float> OnChangeMarkerPosition { get; set; }
        public Action<Guid, float> OnSetMarkerPosition { get; set; }

        public void MarkerError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshMarkers(List<Marker> markers)
        {
            Console.WriteLine("MarkersFragment - RefreshMarkers - markers count: {0}", markers.Count);
            Activity.RunOnUiThread(() => {
                _markers = markers;
                _listAdapter.SetData(markers);
            });
        }

        public void RefreshMarkerPosition(Marker marker, int newIndex)
        {
        }

        #endregion
    }
}
