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

using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Sessions.MVP.Models;

namespace Sessions.Android.Classes.Adapters
{
    public class FolderListAdapter : BaseAdapter<FolderEntity>
    {
        readonly Activity _context;
        readonly ListView _listView;
        List<FolderEntity> _folders;

        public FolderListAdapter(Activity context, ListView listView, List<FolderEntity> folders)
        {
            _context = context;
            _listView = listView;
            _folders = folders;
        }

        public void SetData(List<FolderEntity> folders)
        {
            _folders = folders;
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override FolderEntity this[int position]
        {
            get { return _folders[position]; }
        }

        public override int Count
        {
            get { return _folders.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _folders[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.FolderCell, null);

            var title = view.FindViewById<TextView>(Resource.Id.foldercell_title);
            title.Text = _folders[position].Path;

            return view;
        }
    }
}
