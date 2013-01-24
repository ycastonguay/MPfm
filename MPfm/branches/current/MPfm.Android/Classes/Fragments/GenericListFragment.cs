using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Objects;

namespace MPfm.Android.Classes.Fragments
{
    public class GenericListFragment : ListFragment
    {
        private View _view;
        private Button _button;
        private EditText _editText;
        private List<GenericListItem> _items;

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

            FragmentTransaction ft = FragmentManager.BeginTransaction();
            ft.SetTransition(FragmentTransit.FragmentFade);
            ft.AddToBackStack(null);
            //ft.SetCustomAnimations(Resource.Animation.slide_in_left, Resource.Animation.slide_in_right);


            //NowPlayingFragment fragment = NowPlayingFragment.NewInstance(_entities.ElementAt(position), null);
            //ft.Replace(Resource.Id.test_layout, fragment, "nowPlayingFragment");
            //ft.Commit();

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

        //public void RefreshChannelList(IEnumerable<ChannelEntity> items)
        //{
        //    Activity.RunOnUiThread(() =>
        //    {
        //        this._items = entities;
        //        ListAdapter = new ChannelListAdapter(Activity, entities.ToList());
        //    });
        //}
    }
}