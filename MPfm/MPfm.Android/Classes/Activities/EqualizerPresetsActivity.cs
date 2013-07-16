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
using System.Reflection;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Views;
using Android.OS;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Navigation;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.Android
{
    [Activity(Label = "Equalizer Presets", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class EqualizerPresetsActivity : BaseActivity, IEqualizerPresetsView
    {
        private MobileNavigationManager _navigationManager;
        private string _sourceActivityType;
        private SeekBar _seekBarVolume;
        private ToggleButton _btnBypass;
        private ListView _listView;
        private EqualizerPresetsListAdapter _listAdapter;
        private List<EQPreset> _presets;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("EqualizerPresetsActivity - OnCreate");
            base.OnCreate(bundle);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.EqualizerPresets);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _seekBarVolume = FindViewById<SeekBar>(Resource.Id.equalizerPresets_seekBarVolume);
            _seekBarVolume.ProgressChanged += (sender, args) => OnSetVolume(1);

            _btnBypass = FindViewById<ToggleButton>(Resource.Id.equalizerPresets_btnBypass);
            _btnBypass.Click += (sender, args) => OnBypassEqualizer();

            _listView = FindViewById<ListView>(Resource.Id.equalizerPresets_listView);
            _listAdapter = new EqualizerPresetsListAdapter(this, new List<EQPreset>());
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;
            _listView.ItemLongClick += ListViewOnItemLongClick;

            // Save the source activity type for later (for providing Up navigation)
            _sourceActivityType = Intent.GetStringExtra("sourceActivity");

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            ((AndroidNavigationManager)_navigationManager).SetEqualizerPresetsActivityInstance(this);
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            OnLoadPreset(_presets[itemClickEventArgs.Position].EQPresetId);
        }

        private void ListViewOnItemLongClick(object sender, AdapterView.ItemLongClickEventArgs itemLongClickEventArgs)
        {
            OnEditPreset(_presets[itemLongClickEventArgs.Position].EQPresetId);
        }

        protected override void OnStart()
        {
            Console.WriteLine("EqualizerPresetsActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("EqualizerPresetsActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("EqualizerPresetsActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("EqualizerPresetsActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("EqualizerPresetsActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("EqualizerPresetsActivity - OnDestroy");
            base.OnDestroy();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.equalizerpresets_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    var type = Type.GetType(_sourceActivityType);
                    var intent = new Intent(this, type);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop); 
                    this.StartActivity(intent);
                    this.Finish();
                    return true;
                    break;
                case Resource.Id.equalizerPresetsMenu_item_add:
                    Console.WriteLine("EqualizerPresetsActivity - Menu item click - Showing equalizer preset details view...");
                    OnAddPreset();
                    return true;
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
                    break;
            }
        }

        #region IEqualizerPresetsView implementation

        public Action OnBypassEqualizer { get; set; }
        public Action<float> OnSetVolume { get; set; }
        public Action OnAddPreset { get; set; }
        public Action<Guid> OnLoadPreset { get; set; }
        public Action<Guid> OnEditPreset { get; set; }
        public Action<Guid> OnDeletePreset { get; set; }

        public void EqualizerPresetsError(Exception ex)
        {
            RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in EqualizerPresets: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        public void RefreshPresets(IEnumerable<EQPreset> presets, Guid selectedPresetId, bool isEQBypassed)
        {
            RunOnUiThread(() => {
                _btnBypass.Checked = isEQBypassed;
                _presets = presets.ToList();
                _listAdapter.SetData(_presets);
                _listAdapter.SetSelected(selectedPresetId);
            });
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
        }

        public void RefreshVolume(float volume)
        {
        }

        #endregion

    }
}
