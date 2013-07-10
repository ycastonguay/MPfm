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
using MPfm.Core;
using MPfm.Player.Objects;

namespace MPfm.Android.Classes.Adapters
{
    public class EqualizerPresetsListAdapter : BaseAdapter<EQPreset>
    {
        private readonly EqualizerPresetsActivity _context;
        private List<EQPreset> _presets;

        public bool HasPresetChanged { get; private set; }

        public EqualizerPresetsListAdapter(EqualizerPresetsActivity context, List<EQPreset> presets)
        {
            _context = context;
            _presets = presets;
        }

        public void SetData(List<EQPreset> presets)
        {
            _presets = presets;
            NotifyDataSetChanged();
        }

        public void SetSelected(Guid presetId)
        {
            
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override EQPreset this[int position]
        {
            get { return _presets[position]; }
        }

        public override int Count
        {
            get { return _presets.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _presets[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.EqualizerPresetCell, null);

            var lblName = view.FindViewById<TextView>(Resource.Id.equalizerPresetCell_lblName);
            lblName.Text = _presets[position].Name;

            return view;
        }
    }
}
