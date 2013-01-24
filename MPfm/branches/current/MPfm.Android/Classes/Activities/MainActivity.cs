using System;
using Android.App;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.OS;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Listeners;

namespace MPfm.Android
{
    [Activity(Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //NavigationManager navigationManager;
        private ViewPager _viewPager;
        private TabPagerAdapter _tabPagerAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Main);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            
            _viewPager = FindViewById<ViewPager>(Resource.Id.main_pager);
            _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _viewPager, ActionBar);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            var playerTab = ActionBar.NewTab();
            playerTab.SetTabListener(_tabPagerAdapter);
            playerTab.SetText("Player");
            ActionBar.AddTab(playerTab);
            var playlistsTab = ActionBar.NewTab();
            playlistsTab.SetTabListener(_tabPagerAdapter);
            playlistsTab.SetText("Playlists");
            ActionBar.AddTab(playlistsTab);
            var artistsTab = ActionBar.NewTab();
            artistsTab.SetTabListener(_tabPagerAdapter);
            artistsTab.SetText("Artists");
            ActionBar.AddTab(artistsTab);
            var albumsTab = ActionBar.NewTab();
            albumsTab.SetTabListener(_tabPagerAdapter);
            albumsTab.SetText("Albums");
            ActionBar.AddTab(albumsTab);
            var songsTab = ActionBar.NewTab();
            songsTab.SetTabListener(_tabPagerAdapter);
            songsTab.SetText("Songs");
            ActionBar.AddTab(songsTab);

            // Create and start navigation manager
            //navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
            //navigationManager.Start();

            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            string text = menuItem.TitleFormatted.ToString();
            return base.OnOptionsItemSelected(menuItem);
        }
    }
}

