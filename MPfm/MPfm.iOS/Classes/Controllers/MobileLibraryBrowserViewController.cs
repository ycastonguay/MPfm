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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Controls.Layouts;
using MPfm.iOS.Classes.Delegates;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.Library.Objects;

namespace MPfm.iOS.Classes.Controllers
{
    public partial class MobileLibraryBrowserViewController : BaseViewController, IMobileLibraryBrowserView
    {
        MobileLibraryBrowserType _browserType;
        MobileNavigationTabType _tabType;
        LibraryQuery _query;

        Guid _currentlyPlayingSongId;
        bool _viewHasAlreadyBeenShown = false;
        List<LibraryBrowserEntity> _items;
        string _cellIdentifier = "MobileLibraryBrowserCell";
        NSString _collectionCellIdentifier = new NSString("MobileLibraryBrowserCollectionCell");
        string _navigationBarTitle;
        string _navigationBarSubtitle;
        List<KeyValuePair<string, UIImage>> _imageCache;
        List<KeyValuePair<string, UIImage>> _thumbnailImageCache;
        int _editingTableCellRowPosition = -1;
        int _editingCollectionCellRowPosition = -1;

        public MobileLibraryBrowserViewController(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query)
            : base (UserInterfaceIdiomIsPhone ? "MobileLibraryBrowserViewController_iPhone" : "MobileLibraryBrowserViewController_iPad", null)
        {
            _tabType = tabType;
            _browserType = browserType;
            _query = query;
            _items = new List<LibraryBrowserEntity>();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Flush image cache and table view items
            _items.Clear();
            _imageCache.Clear();
            _thumbnailImageCache.Clear();
            tableView.ReloadData();
        }
        
        public override void ViewDidLoad()
        {
			activityIndicator.StartAnimating();
			//viewLoading.BackgroundColor = UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 0.2f);
			//viewLoading.Layer.CornerRadius = 8;
			viewLoading.BackgroundColor = UIColor.Clear;

			// TODO: Add TopLayoutGuide when Xamarin will explain how it works (or patch it)
			View.BackgroundColor = GlobalTheme.BackgroundColor;
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;
            collectionView.CollectionViewLayout = new MPfmCollectionViewFlowLayout();
            collectionView.BackgroundColor = GlobalTheme.BackgroundColor;
            collectionView.WeakDataSource = this;
            collectionView.WeakDelegate = this;
            collectionView.ContentSize = new SizeF(160, 160);
            collectionView.RegisterClassForCell(typeof(MPfmCollectionAlbumViewCell), _collectionCellIdentifier);

			//viewAlbumCover.Hidden = _browserType != MobileLibraryBrowserType.Songs;
			tableView.Alpha = 0;
			collectionView.Alpha = (_browserType == MobileLibraryBrowserType.Songs && _tabType != MobileNavigationTabType.Songs) ? 1 : 0;

			//imageViewAlbumCover.BackgroundColor = UIColor.Black;
			//viewAlbumCover.BackgroundColor = GlobalTheme.MainDarkColor;

            _imageCache = new List<KeyValuePair<string, UIImage>>();
            _thumbnailImageCache = new List<KeyValuePair<string, UIImage>>();
            this.NavigationItem.HidesBackButton = true;

            UISwipeGestureRecognizer swipe = new UISwipeGestureRecognizer(HandleSwipe);
            swipe.Direction = UISwipeGestureRecognizerDirection.Left;
            tableView.AddGestureRecognizer(swipe);

            UILongPressGestureRecognizer longPressTableView = new UILongPressGestureRecognizer(HandleLongPressTableCellRow);
            longPressTableView.MinimumPressDuration = 0.7f;
            longPressTableView.WeakDelegate = this;
            tableView.AddGestureRecognizer(longPressTableView);

            UILongPressGestureRecognizer longPressCollectionView = new UILongPressGestureRecognizer(HandleLongPressCollectionCellRow);
            longPressCollectionView.MinimumPressDuration = 0.7f;
            longPressCollectionView.WeakDelegate = this;
            collectionView.AddGestureRecognizer(longPressCollectionView);

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                NavigationController.InteractivePopGestureRecognizer.WeakDelegate = this;
                NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            }

            base.ViewDidLoad();            

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindMobileLibraryBrowserView(this, _tabType, _browserType, _query);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

			Tracing.Log("MLBVC - ViewWillAppear - RefreshNavBar");
			RefreshNavigationBar(_navigationBarSubtitle);

            if(_viewHasAlreadyBeenShown)
                ReloadImages();

            _viewHasAlreadyBeenShown = true;
        }
        
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            tableView.DeselectRow(tableView.IndexPathForSelectedRow, false);
            FlushImages();
        }

		public override void DidMoveToParentViewController(UIViewController parent)
		{
			base.DidMoveToParentViewController(parent);

			Tracing.Log("MLBVC - DidMoveToParentViewController");
		} 

		public override void WillMoveToParentViewController(UIViewController parent)
		{
			base.WillMoveToParentViewController(parent);

			//_items = new List<LibraryBrowserEntity>();
			//collectionView.ReloadData();
			//tableView.ReloadData();

			Tracing.Log("MLBVC - WillMoveToParentViewController - RefreshNavBar");
			_navigationBarSubtitle = string.Empty;
			RefreshNavigationBar(string.Empty);
		}

		private void RefreshNavigationBar(string defaultTitle)
		{
			string navTitle = string.Empty;
			if (_browserType == MobileLibraryBrowserType.Artists && _tabType == MobileNavigationTabType.Artists)
				navTitle = "Artists";
			else if (_browserType == MobileLibraryBrowserType.Albums && _tabType == MobileNavigationTabType.Albums)
				navTitle = "Albums";
			else if (_browserType == MobileLibraryBrowserType.Songs && _tabType == MobileNavigationTabType.Songs)
				navTitle = "Albums";
			else
				navTitle = defaultTitle;

			string iconName = string.Empty;
			if (string.IsNullOrEmpty(navTitle))
				iconName = string.Empty;
			else if (_tabType == MobileNavigationTabType.Albums && _browserType == MobileLibraryBrowserType.Albums)
				iconName = "albums";
			else if (_tabType == MobileNavigationTabType.Songs)
				iconName = "song";
			else if (_browserType == MobileLibraryBrowserType.Artists)
				iconName = "artists";
			else if (_browserType == MobileLibraryBrowserType.Albums)
				iconName = "artist";
			else if (_browserType == MobileLibraryBrowserType.Songs)
				iconName = "album";

			Tracing.Log("MLBVC - RefreshNavBar - defaultTitle: {0} navTitle: {1} iconName: {2}", defaultTitle, navTitle, iconName);
			MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;

			if(navCtrl != null)
				navCtrl.SetTitle(navTitle, iconName);
		}

        private void HandleSwipe(UISwipeGestureRecognizer gestureRecognizer)
        {
            //Console.WriteLine("MLBVC - HandleSwipe");
            //var point = gestureRecognizer.LocationInView(tableView);
            //var indexPath = tableView.IndexPathForRowAtPoint(point);

            // IndexPath is null when swiping an empty cell
            //if (indexPath == null)
            //    return;
        }

        private void HandleLongPressCollectionCellRow(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
                return;

            Tracing.Log("MobileLibraryBrowserViewController - HandleLongPressCollectionCellRow");
            PointF pt = gestureRecognizer.LocationInView(collectionView);
            NSIndexPath indexPath = collectionView.IndexPathForItemAtPoint(pt);
            SetEditingCollectionCellRow(indexPath.Row);
        }

        private void ResetEditingCollectionCellRow()
        {
            SetEditingCollectionCellRow(-1);
        }

        private void SetEditingCollectionCellRow(int position)
        {
            int oldPosition = _editingCollectionCellRowPosition;
            _editingCollectionCellRowPosition = position;

            if (oldPosition >= 0)
            {
                var oldItem = _items[oldPosition];
                var oldCell = (MPfmCollectionAlbumViewCell)collectionView.VisibleCells.FirstOrDefault(x => x.Tag == oldPosition);
                if (oldCell != null)
                {
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                        oldCell.PlayButton.Alpha = 0;
                        oldCell.AddButton.Alpha = 0;
                        oldCell.DeleteButton.Alpha = 0;
                        oldCell.PlayButton.Frame = new RectangleF(((oldCell.Frame.Width - 44) / 2) - 8, (oldCell.Frame.Height - 44) / 2, 44, 44);
                        oldCell.AddButton.Frame = new RectangleF((oldCell.Frame.Width - 44) / 2 + 44, (oldCell.Frame.Height - 44) / 2, 44, 44);
                        oldCell.DeleteButton.Frame = new RectangleF(((oldCell.Frame.Width - 44) / 2) + 96, (oldCell.Frame.Height - 44) / 2, 44, 44);
                    }, null);
                }
            }

            if (position >= 0)
            {
                var item = _items[position];
                var cell = (MPfmCollectionAlbumViewCell)collectionView.VisibleCells.FirstOrDefault(x => x.Tag == position);
                if (cell != null)
                {
                    cell.PlayButton.Alpha = 0;
                    cell.AddButton.Alpha = 0;
                    cell.DeleteButton.Alpha = 0;
                    cell.PlayButton.Frame = new RectangleF(((cell.Frame.Width - 44) / 2) - 8, (cell.Frame.Height - 44) / 2, 44, 44);
                    cell.AddButton.Frame = new RectangleF((cell.Frame.Width - 44) / 2 + 44, (cell.Frame.Height - 44) / 2, 44, 44);
                    cell.DeleteButton.Frame = new RectangleF(((cell.Frame.Width - 44) / 2) + 96, (cell.Frame.Height - 44) / 2, 44, 44);
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                        cell.PlayButton.Alpha = 1;
                        cell.AddButton.Alpha = 1;
                        cell.DeleteButton.Alpha = 1;
                        cell.PlayButton.Frame = new RectangleF(((cell.Frame.Width - 44) / 2) - 52, (cell.Frame.Height - 44) / 2, 44, 44);
                        cell.AddButton.Frame = new RectangleF((cell.Frame.Width - 44) / 2, (cell.Frame.Height - 44) / 2, 44, 44);
                        cell.DeleteButton.Frame = new RectangleF(((cell.Frame.Width - 44) / 2) + 52, (cell.Frame.Height - 44) / 2, 44, 44);
                    }, null);
                }
            }
        }

        [Export ("collectionView:cellForItemAtIndexPath:")]
        public UICollectionViewCell CellForItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Tracing.Log("MobileLibraryBrowserViewController - CellForItemAtIndexPath - indexPath.Row: {0}", indexPath.Row);
            var cell = (MPfmCollectionAlbumViewCell)collectionView.DequeueReusableCell(_collectionCellIdentifier, indexPath);
            cell.Tag = indexPath.Row;

            // Do not refresh the cell if the contents are the same.
            if (cell.Title == _items[indexPath.Row].Title && cell.Subtitle == _items[indexPath.Row].Subtitle)
                return cell;

            // Refresh cell contents
            cell.Title = _items[indexPath.Row].Title;
            cell.Subtitle = _items[indexPath.Row].Subtitle;
            if (_browserType == MobileLibraryBrowserType.Albums)
            {
                // Check if album art is cached
                string key = _items[indexPath.Row].Query.ArtistName + "_" + _items[indexPath.Row].Query.AlbumTitle;
                KeyValuePair<string, UIImage> keyPair = _imageCache.FirstOrDefault(x => x.Key == key);
                if (keyPair.Equals(default(KeyValuePair<string, UIImage>)))
                {
                    cell.Image = null;
                    OnRequestAlbumArt(_items[indexPath.Row].Query.ArtistName, _items[indexPath.Row].Query.AlbumTitle, null);
                } 
                else
                {
                    cell.SetImage(keyPair.Value);
                }
            } 

            cell.PlayButton.Alpha = _editingCollectionCellRowPosition == indexPath.Row ? 1 : 0;
            cell.AddButton.Alpha = _editingCollectionCellRowPosition == indexPath.Row ? 1 : 0;
            cell.DeleteButton.Alpha = _editingCollectionCellRowPosition == indexPath.Row ? 1 : 0;

            cell.PlayButton.TouchUpInside += HandleCollectionViewPlayTouchUpInside;
            cell.AddButton.TouchUpInside += HandleCollectionViewAddTouchUpInside;
            cell.DeleteButton.TouchUpInside += HandleCollectionViewDeleteTouchUpInside;

            return cell;
        }

        [Export ("collectionView:numberOfItemsInSection:")]
        public int NumberOfItemsInSection(UICollectionView collectionView, int section)
        {
            // Prevent loading table view cells when using a collection view
            if (_browserType == MobileLibraryBrowserType.Albums)
                return _items.Count;
            else
                return 0;
        }

        [Export ("numberOfSectionsInCollectionView:")]
        public int NumberOfSectionsInCollectionView(UICollectionView collectionView)
        {
            return 1;
        }

        [Export ("collectionView:didSelectItemAtIndexPath:")]
        public void CollectionDidSelectItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ResetEditingCollectionCellRow();
            OnItemClick(indexPath.Row);
        }

//        [Export ("collectionView:didDeselectItemAtIndexPath:")]
//        public void CollectionDidDeselectItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
//        {
//        }

        [Export ("collectionView:didHighlightItemAtIndexPath:")]
        public void CollectionDidHighlightItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (MPfmCollectionAlbumViewCell)collectionView.CellForItem(indexPath);
            cell.SetHighlight(true);
        }

        [Export ("collectionView:didUnhighlightItemAtIndexPath:")]
        public void CollectionDidUnhighlightItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (MPfmCollectionAlbumViewCell)collectionView.CellForItem(indexPath);
            cell.SetHighlight(false);
        }

        [Export ("collectionView:shouldShowMenuForItemAtIndexPath:")]
        public bool CollectionShouldShowMenuForItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

        private void HandleCollectionViewAddTouchUpInside(object sender, EventArgs e)
        {
            Tracing.Log("HandleCollectionViewAddTouchUpInside");
            OnAddItemToPlaylist(_editingCollectionCellRowPosition);
            ResetEditingCollectionCellRow();
        }

        private void HandleCollectionViewDeleteTouchUpInside(object sender, EventArgs e)
        {
            Tracing.Log("HandleCollectionViewDeleteTouchUpInside");

            var item = _items[_editingCollectionCellRowPosition];
            if (item == null)
                return;

            var alertView = new UIAlertView("Delete confirmation", string.Format("Are you sure you wish to delete {0}?", item.Title), null, "OK", new string[1] { "Cancel" });
            alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
                switch(e2.ButtonIndex)
                {
                    case 0:
                        OnDeleteItem(_editingCollectionCellRowPosition);
                        break;
                }
                ResetEditingCollectionCellRow();
            };
            alertView.Show();
        }

        private void HandleCollectionViewPlayTouchUpInside(object sender, EventArgs e)
        {
            Tracing.Log("HandleCollectionViewPlayTouchUpInside");
            OnPlayItem(_editingCollectionCellRowPosition);
            ResetEditingCollectionCellRow();
        }

//        [Export ("collectionView:viewForSupplementaryElementOfKind:atIndexPath:")]
//        public UICollectionReusableView ViewForSupplementaryElement(UICollectionView collectionView, string viewForSupplementaryElementOfKind, NSIndexPath indexPath)
//        {
//            return null;
//        }

        #region UITableView DataSource/Delegate

        private void HandleLongPressTableCellRow(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
                return;

            Tracing.Log("MobileLibraryBrowserViewController - HandleLongPressTableCellRow");
            PointF pt = gestureRecognizer.LocationInView(tableView);
            NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(pt);
            SetEditingTableCellRow(indexPath.Row);
        }

        private void ResetEditingTableCellRow()
        {
            SetEditingTableCellRow(-1);
        }

        private void SetEditingTableCellRow(int position)
        {
            int oldPosition = _editingTableCellRowPosition;
            _editingTableCellRowPosition = position;

            if (oldPosition >= 0)
            {
                var oldItem = _items[oldPosition];
                var oldCell = (MPfmTableViewCell)tableView.VisibleCells.FirstOrDefault(x => x.Tag == oldPosition);
                if (oldCell != null)
                {
                    //oldCell.SecondaryMenuBackground.Alpha = 0;
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                        oldCell.PlayButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 138, 4, 44, 44);
                        oldCell.AddButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 86, 4, 44, 44);
                        oldCell.DeleteButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 34, 4, 44, 44);
                        oldCell.TextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                        oldCell.DetailTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                        oldCell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                        oldCell.PlayButton.Alpha = 0;
                        oldCell.AddButton.Alpha = 0;
                        oldCell.DeleteButton.Alpha = 0;
                        oldCell.AlbumCountLabel.Alpha = 0.75f;
                        oldCell.ImageAlbum1.Alpha = 0.75f;
                        oldCell.ImageAlbum2.Alpha = 0.4f;
                        oldCell.ImageAlbum3.Alpha = 0.2f;
                    }, null);
                }
            }

            if (position >= 0)
            {
                var item = _items[position];
                var cell = (MPfmTableViewCell)tableView.VisibleCells.FirstOrDefault(x => x.Tag == position);
                if (cell != null)
                {
                    cell.PlayButton.Alpha = 0;
                    cell.AddButton.Alpha = 0;
                    cell.DeleteButton.Alpha = 0;
                    cell.AlbumCountLabel.Alpha = 0.75f;
                    cell.ImageAlbum1.Alpha = 0.75f;
                    cell.ImageAlbum2.Alpha = 0.4f;
                    cell.ImageAlbum3.Alpha = 0.2f;
                    cell.PlayButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 138, 4, 44, 44);
                    cell.AddButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 86, 4, 44, 44);
                    cell.DeleteButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 34, 4, 44, 44);
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                        cell.PlayButton.Alpha = 1;
                        cell.AddButton.Alpha = 1;
                        cell.DeleteButton.Alpha = 1;
                        cell.AlbumCountLabel.Alpha = 0;
                        cell.ImageAlbum1.Alpha = 0;
                        cell.ImageAlbum2.Alpha = 0;
                        cell.ImageAlbum3.Alpha = 0;
                        cell.TextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                        cell.DetailTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                        cell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                        cell.PlayButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 182, 4, 44, 44);
                        cell.AddButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 130, 4, 44, 44);
                        cell.DeleteButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 78, 4, 44, 44);
                    }, null);
                }
            }
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            // Prevent loading cells when using a collection view
            if (_browserType == MobileLibraryBrowserType.Albums)
                return 0;
            else
                return _items.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _items[indexPath.Row];
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                cell = new MPfmTableViewCell(UITableViewCellStyle.Subtitle, _cellIdentifier);

                // Register events only once!
                cell.PlayButton.TouchUpInside += HandleTableViewPlayTouchUpInside;
                cell.AddButton.TouchUpInside += HandleTableViewAddTouchUpInside;
                cell.DeleteButton.TouchUpInside += HandleTableViewDeleteTouchUpInside;
            }

            cell.Tag = indexPath.Row;
            cell.Accessory = UITableViewCellAccessory.None;
            cell.IsTextAnimationEnabled = true;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue", 14);
            cell.TextLabel.Text = item.Title;
            cell.DetailTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 12);
            cell.DetailTextLabel.Text = item.Subtitle;
            cell.ImageView.AutoresizingMask = UIViewAutoresizing.None;
            cell.ImageView.ClipsToBounds = true;
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
            cell.ImageChevron.Hidden = false;

            cell.ImageAlbum1.Image = null;
            cell.ImageAlbum2.Image = null;
            cell.ImageAlbum3.Image = null;
            cell.ImageAlbum1.Tag = 1;
            cell.ImageAlbum2.Tag = 2;
            cell.ImageAlbum3.Tag = 3;
            cell.RightOffset = 0;

            // Change title font when the item has a subtitle
            if(String.IsNullOrEmpty(item.Subtitle))
                cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);

            cell.PlayButton.Alpha = _editingTableCellRowPosition == indexPath.Row ? 1 : 0;
            cell.AddButton.Alpha = _editingTableCellRowPosition == indexPath.Row ? 1 : 0;
            cell.DeleteButton.Alpha = _editingTableCellRowPosition == indexPath.Row ? 1 : 0;
            cell.AlbumCountLabel.Alpha = _editingTableCellRowPosition == indexPath.Row ? 0 : 0.75f;
            cell.ImageAlbum1.Alpha = _editingTableCellRowPosition == indexPath.Row ? 0 : 0.75f;
            cell.ImageAlbum2.Alpha = _editingTableCellRowPosition == indexPath.Row ? 0 : 0.4f;
            cell.ImageAlbum3.Alpha = _editingTableCellRowPosition == indexPath.Row ? 0 : 0.2f;
            //cell.SecondaryMenuBackground.Alpha = _editingTableCellRowPosition == indexPath.Row ? 1 : 0;
            if (_editingTableCellRowPosition == indexPath.Row)
            {
                cell.TextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                cell.DetailTextLabel.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
                cell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.9f, 0.9f);
            }
            else
            {
                cell.TextLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
                cell.DetailTextLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
                cell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
            }

            if (_browserType == MobileLibraryBrowserType.Songs)
            {
                cell.IndexTextLabel.Text = item.AudioFile.TrackNumber.ToString();
                cell.ImageAlbum1.Hidden = true;
                cell.ImageAlbum2.Hidden = true;
                cell.ImageAlbum3.Hidden = true;
                cell.AlbumCountLabel.Hidden = true;
                if (_currentlyPlayingSongId == item.AudioFile.Id)
                    cell.RightImage.Hidden = false;
                else
                    cell.RightImage.Hidden = true;
            }
            else if(_browserType == MobileLibraryBrowserType.Artists)
            {
                cell.AlbumCountLabel.Text = string.Format("+{0}", item.AlbumTitles.Count - 2);
                cell.ImageAlbum1.Hidden = item.AlbumTitles.Count >= 1 ? false : true;
                cell.ImageAlbum2.Hidden = item.AlbumTitles.Count >= 2 ? false : true;
                cell.ImageAlbum3.Hidden = item.AlbumTitles.Count >= 3 ? false : true;
                cell.AlbumCountLabel.Hidden = item.AlbumTitles.Count > 3 ? false : true;

                int albumFetchCount = item.AlbumTitles.Count > 3 ? 2 : item.AlbumTitles.Count;
                albumFetchCount = item.AlbumTitles.Count == 3 ? 3 : albumFetchCount; //
                //albumFetchCount = item.AlbumTitles.Count == 3 ? 3 : item.AlbumTitles.Count; // Do not load a third album art when the count is visible!

                //Console.WriteLine("GetCell - title: {0} index: {1} albumFetchCount: {2}", item.Title, indexPath.Row, albumFetchCount);

                int startIndex = 0;
                if (item.AlbumTitles.Count > 3)
                {
                    startIndex = 1;
                    albumFetchCount++;
                }
                for (int a = startIndex; a < albumFetchCount; a++)
                {
                    UIImageView imageAlbum = null;
                    if (a == 0)
                        imageAlbum = cell.ImageAlbum1;
                    else if (a == 1)
                        imageAlbum = cell.ImageAlbum2;
                    else if (a == 2)
                        imageAlbum = cell.ImageAlbum3;

                    // Check if album art is cached
                    string key = item.Query.ArtistName + "_" + item.AlbumTitles[a];
                    KeyValuePair<string, UIImage> keyPair = _thumbnailImageCache.FirstOrDefault(x => x.Key == key);
                    if (keyPair.Equals(default(KeyValuePair<string, UIImage>)))
                    {
                        //Console.WriteLine("MLBVC - GetCell - OnRequestAlbumArt - index: {0} key: {1}", indexPath.Row, key);
                        OnRequestAlbumArt(item.Query.ArtistName, item.AlbumTitles[a], imageAlbum);
                    } 
                    else
                    {
                        //Console.WriteLine("MLBVC - GetCell - Taking image from cache - index: {0} key: {1}", indexPath.Row, key);
                        imageAlbum.Image = keyPair.Value;
                    }
                }
            }

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            Console.WriteLine("MLBVC - RowSelected - row: {0}", indexPath.Row);
            //if (indexPath.Row == _editingTableCellRowPosition)
            if(_editingTableCellRowPosition != -1)
            {
                Console.WriteLine("MLBVC - RowSelected - Deselecting row... - row: {0}", indexPath.Row);
                tableView.DeselectRow(indexPath, true);
                ResetEditingTableCellRow();
                return;
            }

            Console.WriteLine("MLBVC - RowSelected - OnItemClick - row: {0}", indexPath.Row);
            OnItemClick(indexPath.Row);
        }

        [Export ("tableView:didHighlightRowAtIndexPath:")]
        public void DidHighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            Console.WriteLine("MLBVC - DidHighlightRowAtIndexPath - row: {0}", indexPath.Row);
            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
            if (cell == null)
                return;

//            if (indexPath.Row != _editingTableCellRowPosition)
//            {
//                Console.WriteLine("MLBVC - DidHighlightRowAtIndexPath - Removing secondary menu... - row: {0}", indexPath.Row);
//                ResetEditingTableCellRow();
//            }
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_white");
            cell.RightImage.Image = UIImage.FromBundle("Images/Icons/icon_speaker_white");
        }

        [Export ("tableView:didUnhighlightRowAtIndexPath:")]
        public void DidUnhighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
            if (cell == null)
                return;

            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
            cell.RightImage.Image = UIImage.FromBundle("Images/Icons/icon_speaker");
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }

        private void HandleTableViewAddTouchUpInside(object sender, EventArgs e)
        {
            OnAddItemToPlaylist(_editingTableCellRowPosition);
            ResetEditingTableCellRow();
        }

        private void HandleTableViewDeleteTouchUpInside(object sender, EventArgs e)
        {
            var item = _items[_editingTableCellRowPosition];
            if (item == null)
                return;

            var alertView = new UIAlertView("Delete confirmation", string.Format("Are you sure you wish to delete {0}?", item.Title), null, "OK", new string[1] { "Cancel" });
            alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
                switch(e2.ButtonIndex)
                {
                    case 0:
                        OnDeleteItem(_editingTableCellRowPosition);
                        break;
                }
                ResetEditingTableCellRow();
            };
            alertView.Show();           
        }

        private void HandleTableViewPlayTouchUpInside(object sender, EventArgs e)
        {
            OnPlayItem(_editingTableCellRowPosition);
            ResetEditingTableCellRow();
        }

        #endregion

        private void FlushImages()
        {
//            if(imageViewAlbumCover.Image != null)
//            {
//                imageViewAlbumCover.Image.Dispose();
//                imageViewAlbumCover.Image = null;
//            }
//
//            // Flush images in table view
//            for(int section = 0; section < tableView.NumberOfSections(); section++)
//            {
//                for(int row = 0; row < tableView.NumberOfRowsInSection(section); row++)
//                {
//                    NSIndexPath indexPath = NSIndexPath.FromItemSection(row, section);
//                    UITableViewCell cell = tableView.CellAt(indexPath);
//                    if(cell != null && cell.ImageView != null && cell.ImageView.Image != null)
//                    {
//                        cell.ImageView.Image.Dispose();
//                        cell.ImageView.Image = null;
//                    }
//                }
//            }
        }

        private void ReloadImages()
        {
//            foreach(UITableViewCell cell in tableView.VisibleCells)
//            {
//                NSIndexPath indexPath = tableView.IndexPathForCell(cell);
//                OnRequestAlbumArt(_items[indexPath.Row].Query.ArtistName, _items[indexPath.Row].Query.AlbumTitle);
//            }
        }

        #region IMobileLibraryBrowserView implementation

        public Action<MobileLibraryBrowserType> OnChangeBrowserType { get; set; }
        public Action<int> OnItemClick { get; set; }
        public Action<int> OnDeleteItem { get; set; }
        public Action<string, string, object> OnRequestAlbumArt { get; set; }
        public Action<int> OnPlayItem { get; set; }
        public Func<string, string, byte[]> OnRequestAlbumArtSynchronously { get; set; }
        public Action<int> OnAddItemToPlaylist { get; set; }

        public void MobileLibraryBrowserError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("MobileLibraryBrowser Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public async void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData, object userData)
        {
            //Console.WriteLine("MLBVC - RefreshAlbumArtCell - artistName: {0} albumTitle: {1} browserType: {2}", artistName, albumTitle, _browserType);
            // Note: cannot call UIScreen.MainScreen in a background thread!
//            int height = 44;
//            InvokeOnMainThread(() => {
//                height = (int)(44 * UIScreen.MainScreen.Scale);
//            });
            int height = 0;
            switch (_browserType)
            {
                case MobileLibraryBrowserType.Artists:
                    height = 44;
                    break;
                case MobileLibraryBrowserType.Albums:
                    height = 160;
                    break;
            }
            InvokeOnMainThread(() => { // We have to use the main thread to fetch the scale
                height = (int)(height * UIScreen.MainScreen.Scale);
            });

            var task = Task<UIImage>.Factory.StartNew(() => {
                //Console.WriteLine("MLBVC - RefreshAlbumArtCell - artistName: {0} albumTitle: {1} browserType: {2} height: {3}", artistName, albumTitle, _browserType, height);
                try
                {
                    using (NSData imageData = NSData.FromArray(albumArtData))
                    {
                        using (UIImage imageFullSize = UIImage.LoadFromData(imageData))
                        {
                            if (imageFullSize != null)
                            {
                                UIImage imageResized = CoreGraphicsHelper.ScaleImage(imageFullSize, height);
                                //Console.WriteLine("MLBVC - RefreshAlbumArtCell - Image resized!");
                                return imageResized;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error resizing image " + artistName + " - " + albumTitle + ": " + ex.Message);
                }

                //Console.WriteLine("MLBVC - RefreshAlbumArtCell - Returning null");
                return null;
            });

            //}).ContinueWith(t => {

                //UIImage image = t.Result;
            UIImage image = await task;
                //Console.WriteLine("MLBVC - RefreshAlbumArtCell - ContinueWith - artistName: {0} albumTitle: {1} browserType: {2} userData==null: {3} image==null: {4}", artistName, albumTitle, _browserType, userData == null, image == null);
                if(image == null)
                    return;

                InvokeOnMainThread(() => {
                    switch (_browserType)
                    {
                        case MobileLibraryBrowserType.Artists:
                            // Remove older image from cache if exceeds cache size
                            if(_thumbnailImageCache.Count > 80)
                                _thumbnailImageCache.RemoveAt(0);

                            // Add image to cache
                            _thumbnailImageCache.Add(new KeyValuePair<string, UIImage>(artistName + "_" + albumTitle, image));

                            // Get item from list
                            var itemArtistName = _items.FirstOrDefault(x => x.Query.ArtistName == artistName);
                            if (itemArtistName == null)
                                return;

                            // Get cell from item
                            int indexArtistName = _items.IndexOf(itemArtistName);
                            var cellArtistName = (MPfmTableViewCell)tableView.VisibleCells.FirstOrDefault(x => x.Tag == indexArtistName);
                            if (cellArtistName == null)
                                return;

                            if(userData != null)
                            {
                                var imageView = (UIImageView)userData;
                                imageView.Alpha = 0;
                                imageView.Image = image;

                                float alpha = 0.75f;
                                if(imageView.Tag == 3)
                                    //alpha = 0.15f;
                                    alpha = 0.2f;
                                else if(imageView.Tag == 2)
                                    alpha = 0.4f;

                                UIView.Animate(0.2, () => {
                                    imageView.Alpha = alpha;
                                });
                            }

                            //if(cellArtistName.Image != image)
                            //    cellArtistName.SetImage(image);
                            break;
                        case MobileLibraryBrowserType.Albums:
                            // Remove older image from cache if exceeds cache size
                            if(_imageCache.Count > 20)
                                _imageCache.RemoveAt(0);

                            // Add image to cache
                            _imageCache.Add(new KeyValuePair<string, UIImage>(artistName + "_" + albumTitle, image));

                            // Get item from list
                            var itemAlbumTitle = _items.FirstOrDefault(x => x.Query.ArtistName == artistName && x.Query.AlbumTitle == albumTitle);
                            if (itemAlbumTitle == null)
                                return;

                            // Get cell from item
                            int indexAlbumTitle = _items.IndexOf(itemAlbumTitle);
                            var cellAlbumTitle = (MPfmCollectionAlbumViewCell)collectionView.VisibleCells.FirstOrDefault(x => x.Tag == indexAlbumTitle);
                            if (cellAlbumTitle == null)
                                return;

                            if(cellAlbumTitle.Image != image)
                                cellAlbumTitle.SetImage(image);
                            break;
                    }

//                    // Get cell from item
//                    int index = _items.IndexOf(item);
//                    var cell = tableView.VisibleCells.FirstOrDefault(x => x.Tag == index);
//                    if (cell == null)
//                        return;
//
//                    // Make sure cell is available
//                    if(cell != null && cell.ImageView != null)
//                    {
//                        cell.ImageView.Alpha = 0;
//                        cell.ImageView.Image = image;
//                        cell.ImageView.Frame = new RectangleF(0, 0, 44, 44);
//                        UIView.Animate(0.1, () => {
//                            cell.ImageView.Alpha = 1;
//                        });
//                    }
                });
            //}, TaskScheduler.FromCurrentSynchronizationContext());
        }
    
        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle, string navigationBarSubtitle, string breadcrumb, bool isPopBackstack, bool isBackstackEmpty)
        {
            InvokeOnMainThread(() => {
                _editingTableCellRowPosition = -1;
                _editingCollectionCellRowPosition = -1;
                _items = entities.ToList();
                _browserType = browserType;
                _navigationBarTitle = navigationBarTitle;
                _navigationBarSubtitle = navigationBarSubtitle;
				Tracing.Log("MLBVC - RefreshLibraryBrowser - RefreshNavBar - browserType: {0}", browserType);
				RefreshNavigationBar(_navigationBarSubtitle);

                // Reset scroll bar
                tableView.ScrollRectToVisible(new RectangleF(0, 0, 1, 1), false);

				// Fade out loading
				if(viewLoading.Alpha > 0)
				{
					activityIndicator.StopAnimating();
					UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
						viewLoading.Alpha = 0;
					}, null);
				}

                if(browserType == MobileLibraryBrowserType.Albums)
                {
                    tableView.Hidden = true;
					collectionView.Hidden = false;
					if(_browserType == MobileLibraryBrowserType.Albums && _tabType != MobileNavigationTabType.Albums)
					{
						collectionView.Alpha = 1;
						collectionView.ReloadData();
					}
					else
					{
						collectionView.Alpha = 0;
						collectionView.ReloadData();

						UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
							collectionView.Alpha = 1;
						}, null);
					}
                }
                else
                {
					collectionView.Hidden = true;
					tableView.Hidden = false;
					if(_browserType == MobileLibraryBrowserType.Songs && _tabType != MobileNavigationTabType.Songs)
					{
						tableView.Alpha = 1;
						tableView.ReloadData();
					}
					else
					{
						tableView.Alpha = 0;
						tableView.ReloadData();

						UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
							tableView.Alpha = 1;
						}, null);
					}
                }

                // Hide album cover if not showing songs
                if(browserType != MobileLibraryBrowserType.Songs)
                {
					//viewAlbumCover.Hidden = true;
					tableView.Frame = new RectangleF(View.Frame.X, View.Frame.Y - 44 - 20, View.Frame.Width, View.Frame.Height);
                }
                else
                {
					//viewAlbumCover.Hidden = false;
                    if(_items.Count == 0)
                        return;

                    var audioFile = _items[0].AudioFile;
//                    lblArtistName.Text = audioFile.ArtistName;
//                    lblAlbumTitle.Text = audioFile.AlbumTitle;
//                    lblSubtitle1.Text = _items.Count().ToString() + " songs";

//                    // Note: cannot call UIScreen.MainScreen in a background thread!
//                    int height = (int)(viewAlbumCover.Bounds.Height * UIScreen.MainScreen.Scale);
//					imageViewAlbumCover.Image = null;
//                    Task<UIImage>.Factory.StartNew(() => {
//                        byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);                        
//                        using (NSData imageData = NSData.FromArray(bytesImage))
//                        {
//                            using (UIImage image = UIImage.LoadFromData(imageData))
//                            {
//                                if (image != null)
//                                {
//                                    try
//                                    {
//                                        UIImage imageResized = CoreGraphicsHelper.ScaleImage(image, height);
//                                        return imageResized;
//                                    } 
//                                    catch (Exception ex)
//                                    {
//                                        Console.WriteLine("Error resizing image " + audioFile.ArtistName + " - " + audioFile.AlbumTitle + ": " + ex.Message);
//                                    }
//                                }
//                            }
//                        }
//                        
//                        return null;
//                    }).ContinueWith(t => {
//                        UIImage image = t.Result;
//                        if(image == null)
//                            return;
//                        
//                        InvokeOnMainThread(() => {
//                            imageViewAlbumCover.Image = image;                                                                                               
//                        });
//                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            });
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
            Console.WriteLine("MLBVC - RefreshCurrentlyPlayingSong index: {0} audioFile: {1}", index, audioFile.FilePath);

            if (audioFile != null)
                _currentlyPlayingSongId = audioFile.Id;
            else
                _currentlyPlayingSongId = Guid.Empty;

            InvokeOnMainThread(() => {
                foreach(var cell in tableView.VisibleCells)
                {
                    if(_items[cell.Tag].AudioFile != null)
                    {
                        var id = _items[cell.Tag].AudioFile.Id;
                        var customCell = (MPfmTableViewCell)cell;
                        if(id == audioFile.Id)
                            customCell.RightImage.Hidden = false;
                        else
                            customCell.RightImage.Hidden = true;
                    }
                }
            });
        }

        public void NotifyNewPlaylistItems(string text)
        {
        }

        #endregion

    }
}
