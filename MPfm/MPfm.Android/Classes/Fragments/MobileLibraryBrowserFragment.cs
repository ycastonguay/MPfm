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
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Objects;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class MobileLibraryBrowserFragment : BaseListFragment, IMobileLibraryBrowserView
    {
        private View _view;
        private Button _button;
        private EditText _editText;
        private List<GenericListItem> _items;

        public MobileLibraryBrowserType BrowserType { get; set; }
        public string Filter { get; set; }
        public Action<int> OnItemClick { get; set; }

        public MobileLibraryBrowserFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
        }

        public override void OnStart()
        {
            base.OnStart();

            _items = new List<GenericListItem>()
                {
                    new GenericListItem()
                        {
                            Title = "Item 1"
                        },
                    new GenericListItem()
                        {
                            Title = "Item 2"
                        },
                    new GenericListItem()
                        {
                            Title = "Item 3"
                        }
                };
            ListAdapter = new GenericListAdapter(Activity, _items);

            //ListView.SetDrawSelectorOnTop(false);
            //ListView.Selector = Resources.GetDrawable(Resource.Drawable.channels_bap_selected);
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

    }
}
