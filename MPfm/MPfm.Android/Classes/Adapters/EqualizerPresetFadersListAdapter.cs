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
    public class EqualizerPresetFadersListAdapter : BaseAdapter<EQPresetBand>
    {
        private readonly EqualizerPresetDetailsActivity _context;
        private EQPreset _preset;

        public bool HasPresetChanged { get; private set; }

        public EqualizerPresetFadersListAdapter(EqualizerPresetDetailsActivity context, EQPreset preset)
        {
            _context = context;
            _preset = preset;
        }

        public void SetData(EQPreset preset)
        {
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override EQPresetBand this[int position]
        {
            get { return _preset.Bands[position]; }
        }

        public override int Count
        {
            get { return _preset.Bands.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _preset.Bands[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = _context.LayoutInflater.Inflate(Resource.Layout.EqualizerPresetFaderCell, null);

            var lblFrequency = view.FindViewById<TextView>(Resource.Id.equalizerPresetFaderCell_lblFrequency);
            var lblValue = view.FindViewById<TextView>(Resource.Id.equalizerPresetFaderCell_lblValue);
            var seekBar = view.FindViewById<SeekBar>(Resource.Id.equalizerPresetFaderCell_seekBar);

            int progress = (int) ((_preset.Bands[position].Gain + 6f)*10f);
            lblFrequency.Text = _preset.Bands[position].CenterString;
            lblValue.Text = GetGainString(_preset.Bands[position].Gain);
            seekBar.Progress = progress;
            seekBar.ProgressChanged += (sender, args) => {
                HasPresetChanged = true;
                float gain = (((float)seekBar.Progress) / 10f) - 6f;
                lblValue.Text = GetGainString(gain);
                _preset.Bands[position].Gain = gain;
                _context.OnSetFaderGain(_preset.Bands[position].CenterString, gain);
            };

            return view;
        }

        private string GetGainString(float gain)
        {
            if (gain > 0)
                return "+" + gain.ToString("0.0").Replace(",", ".") + " dB";
            else
                return gain.ToString("0.0").Replace(",", ".") + " dB";
        }
    }
}
