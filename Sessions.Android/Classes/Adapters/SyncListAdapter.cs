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

using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using Sessions.Library.Objects;

namespace MPfm.Android.Classes.Adapters
{
    public class SyncListAdapter : BaseAdapter<SyncDevice>
    {
        private readonly SyncActivity _context;
        private List<SyncDevice> _devices;

        public SyncListAdapter(SyncActivity context, List<SyncDevice> devices)
        {
            _context = context;
            _devices = devices;
        }

        public void SetData(List<SyncDevice> devices)
        {
            _devices = devices;
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override SyncDevice this[int position]
        {
            get { return _devices[position]; }
        }

        public override int Count
        {
            get { return _devices.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _devices[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.GenericCell, null);

            var title = view.FindViewById<TextView>(Resource.Id.genericcell_title);
            var image = view.FindViewById<ImageView>(Resource.Id.genericcell_image);

            // For some strange reason sometimes the item is null... is view being modified during refresh?
            if (item == null)
                return view;

            title.Text = item.Name;
            switch (item.DeviceType)
            {
                case SyncDeviceType.AndroidPhone:
                    image.SetImageResource(Resource.Drawable.icon_android);
                    break;
                case SyncDeviceType.Linux:
                    image.SetImageResource(Resource.Drawable.icon_linux);
                    break;
                case SyncDeviceType.OSX:
                    image.SetImageResource(Resource.Drawable.icon_osx);
                    break;
                case SyncDeviceType.Windows:
                    image.SetImageResource(Resource.Drawable.icon_windows);
                    break;
                case SyncDeviceType.iPhone:
                    image.SetImageResource(Resource.Drawable.icon_phone);
                    break;
                default:
                    image.SetImageResource(0);
                    break;
            }

            //Animation animation = AnimationUtils.LoadAnimation(_context, Resource.Animation.fade_in);
            //view.StartAnimation(animation);

            return view;
        }
    }
}
