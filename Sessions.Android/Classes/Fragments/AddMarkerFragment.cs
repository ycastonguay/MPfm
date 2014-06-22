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
using System.Runtime.InteropServices;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Sessions.Android.Classes.Adapters;
using Sessions.Android.Classes.Fragments.Base;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters;
using Sessions.MVP.Views;

namespace Sessions.Android.Classes.Fragments
{
    public class AddMarkerFragment : BaseDialogFragment, IAddMarkerView
    {
        private View _view;
        private RadioButton _radioVerse;
        private RadioButton _radioChorus;
        private RadioButton _radioBridge;
        private RadioButton _radioSolo;
        private Button _btnCancel;
        private Button _btnCreate;

        public AddMarkerFragment() : base() {}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Select marker template");
            _view = inflater.Inflate(Resource.Layout.AddMarker, container, false);

            _btnCancel = _view.FindViewById<Button>(Resource.Id.addMarker_btnCancel);
            _btnCreate = _view.FindViewById<Button>(Resource.Id.addMarker_btnCreate);
            _radioVerse = _view.FindViewById<RadioButton>(Resource.Id.addMarker_radioVerse);
            _radioChorus = _view.FindViewById<RadioButton>(Resource.Id.addMarker_radioChorus);
            _radioBridge = _view.FindViewById<RadioButton>(Resource.Id.addMarker_radioBridge);
            _radioSolo = _view.FindViewById<RadioButton>(Resource.Id.addMarker_radioSolo);
            _btnCancel.Click += (sender, args) => Dismiss();
            _btnCreate.Click += (sender, args) =>
            {
                MarkerTemplateNameType template = MarkerTemplateNameType.None;
                if(_radioVerse.Checked)
                    template = MarkerTemplateNameType.Verse;
                else if (_radioChorus.Checked)
                    template = MarkerTemplateNameType.Chorus;
                else if (_radioBridge.Checked)
                    template = MarkerTemplateNameType.Bridge;
                else if (_radioSolo.Checked)
                    template = MarkerTemplateNameType.Solo;

                OnAddMarker(template);
                Dismiss();
            };
            _radioVerse.Checked = true;

            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);            

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindAddMarkerView(this);
        }

        #region IAddMarkerView implementation

        public Action<MarkerTemplateNameType> OnAddMarker { get; set; }

        public void AddMarkerError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        #endregion

    }
}
