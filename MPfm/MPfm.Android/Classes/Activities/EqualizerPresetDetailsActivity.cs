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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Navigation;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using org.sessionsapp.android;

namespace MPfm.Android
{
    [Activity(Label = "Equalizer Preset Details", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize, WindowSoftInputMode = SoftInput.StateHidden)]
    public class EqualizerPresetDetailsActivity : BaseActivity, IEqualizerPresetDetailsView, EditText.IOnEditorActionListener
    {
        private MobileNavigationManager _navigationManager;
        private string _sourceActivityType;
        private EqualizerPresetFadersListAdapter _listAdapter;
        private ListView _listView;
        private EditText _txtPresetName;
        private Button _btnNormalize;
        private Button _btnReset;
        private EQPreset _preset;
        private EqualizerPresetGraphView _equalizerPresetGraph;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("EqualizerPresetDetailsActivity - OnCreate");
            base.OnCreate(bundle);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.EqualizerPresetDetails);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _txtPresetName = FindViewById<EditText>(Resource.Id.equalizerPresetDetails_txtPresetName);
            _btnNormalize = FindViewById<Button>(Resource.Id.equalizerPresetDetails_btnNormalize);
            _btnReset = FindViewById<Button>(Resource.Id.equalizerPresetDetails_btnReset);
            _equalizerPresetGraph = FindViewById<EqualizerPresetGraphView>(Resource.Id.equalizerPresetDetails_graphView);
            _btnNormalize.Click += BtnNormalizeOnClick;
            _btnReset.Click += BtnResetOnClick;

            //_txtPresetName.SetOnEditorActionListener(this);

            _listView = FindViewById<ListView>(Resource.Id.equalizerPresetDetails_listView);
            _listAdapter = new EqualizerPresetFadersListAdapter(this, _listView, new EQPreset());
            _listView.SetAdapter(_listAdapter);

            // Save the source activity type for later (for providing Up navigation)
            _sourceActivityType = Intent.GetStringExtra("sourceActivity");

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            ((AndroidNavigationManager)_navigationManager).SetEqualizerPresetDetailsActivityInstance(this);
        }

        protected override void OnStart()
        {
            Console.WriteLine("EqualizerPresetDetailsActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("EqualizerPresetDetailsActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("EqualizerPresetDetailsActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("EqualizerPresetDetailsActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("EqualizerPresetDetailsActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("EqualizerPresetDetailsActivity - OnDestroy");
            base.OnDestroy();
        }

        private void BtnResetOnClick(object sender, EventArgs eventArgs)
        {
            AlertDialog ad = new AlertDialog.Builder(this)
               .SetIconAttribute(global::Android.Resource.Attribute.AlertDialogIcon)
               .SetTitle("Equalizer preset will be reset")
               .SetMessage("Are you sure you wish to reset this equalizer preset?")
               .SetCancelable(true)
               .SetPositiveButton("OK", (sender2, args) => OnResetPreset())
               .SetNegativeButton("Cancel", (sender2, args) => { })
               .Create();
            ad.Show();
        }

        private void BtnNormalizeOnClick(object sender, EventArgs eventArgs)
        {
            AlertDialog ad = new AlertDialog.Builder(this)
               .SetIconAttribute(global::Android.Resource.Attribute.AlertDialogIcon)
               .SetTitle("Equalizer preset will be normalized")
               .SetMessage("Are you sure you wish to normalize this equalizer preset?")
               .SetCancelable(true)
               .SetPositiveButton("OK", (sender2, args) => OnNormalizePreset())
               .SetNegativeButton("Cancel", (sender2, args) => { })
               .Create();
            ad.Show();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.equalizerpresetdetails_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    if (_listAdapter.HasPresetChanged)
                    {
                        ConfirmExitActivity();
                        return true;
                    }

                    var type = Type.GetType(_sourceActivityType);
                    var intent = new Intent(this, type);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop); 
                    this.StartActivity(intent);
                    this.Finish();
                    return true;
                    break;
                case Resource.Id.equalizerPresetDetailsMenu_item_save:
                    Console.WriteLine("EqualizerPresetDetailsActivity - Menu item click - Saving preset...");
                    OnSavePreset(_txtPresetName.Text);
                    _listAdapter.HasPresetChanged = false;
                    return true;
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
                    break;
            }
        }

        public override void OnBackPressed()
        {
            if (_listAdapter.HasPresetChanged)
            {
                ConfirmExitActivity();
                return;
            }

            base.OnBackPressed();
        }

        private void ConfirmExitActivity()
        {
            AlertDialog ad = new AlertDialog.Builder(this)
                .SetIconAttribute(global::Android.Resource.Attribute.AlertDialogIcon)
                .SetTitle("Equalizer preset has been modified")
                .SetMessage("Are you sure you wish to exit this screen without saving?")
                .SetCancelable(true)
                .SetPositiveButton("OK", (sender, args) => {
                    OnRevertPreset();
                    Finish();
                })
                .SetNegativeButton("Cancel", (sender, args) => {})
                .Create();
            ad.Show();
        }

        public void UpdatePreset(EQPreset preset)
        {
            //Console.WriteLine("EQDA - UPDATE PRESET");
            _preset = preset;
            _equalizerPresetGraph.SetPreset(preset);
            _txtPresetName.Text = preset.Name;
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            //if (e.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            if(actionId == ImeAction.Done)
            {
                //var imm = (InputMethodManager)ApplicationContext.GetSystemService(Context.InputMethodService);
                //imm.HideSoftInputFromInputMethod(v.WindowToken, 0);
                Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
                return true;
            }

            return false;
        }

        #region IEqualizerPresetDetailsView implementation

        public Action OnResetPreset { get; set; }
        public Action OnNormalizePreset { get; set; }
        public Action OnRevertPreset { get; set; }
        public Action<string> OnSavePreset { get; set; }
        public Action<string, float> OnSetFaderGain { get; set; }

        public void EqualizerPresetDetailsError(Exception ex)
        {
            RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in EqualizerPresetDetails: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        public void ShowMessage(string title, string message)
        {
            RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetTitle(title);
                ad.SetMessage(message);
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        public void RefreshPreset(EQPreset preset)
        {
            //Console.WriteLine("EQDA - REFRESH PRESET");
            RunOnUiThread(() => {
                UpdatePreset(preset);
                _listAdapter.SetData(preset);
            });
        }

        #endregion

    }
}
