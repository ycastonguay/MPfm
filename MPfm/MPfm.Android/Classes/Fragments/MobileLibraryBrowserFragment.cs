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
            Console.WriteLine("OnListItemClick");
            base.OnListItemClick(l, v, position, id);
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