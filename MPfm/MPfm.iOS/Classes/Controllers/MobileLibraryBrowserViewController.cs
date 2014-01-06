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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Controls.Layouts;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.Library.Objects;
using MPfm.Core.Helpers;

namespace MPfm.iOS.Classes.Controllers
{
    public partial class MobileLibraryBrowserViewController : BaseViewController, IMobileLibraryBrowserView
    {
		NSIndexPath _movingIndexPath;
		bool _isTableViewScrolling = false;
		readonly object _locker = new object();
		List<Tuple<SectionIndex, List<LibraryBrowserEntity>>> _items;
        MobileLibraryBrowserType _browserType;
        MobileNavigationTabType _tabType;
        LibraryQuery _query;
        Guid _currentlyPlayingSongId;
        bool _viewHasAlreadyBeenShown = false;
        string _cellIdentifier = "MobileLibraryBrowserCell";
		NSString _headerCellIdentifier = new NSString("MobileLibraryBrowserHeaderCell");
        NSString _collectionCellIdentifier = new NSString("MobileLibraryBrowserCollectionCell");
		NSString _collectionCellHeaderIdentifier = new NSString("MobileLibraryBrowserCollectionHeaderCell");
        string _navigationBarSubtitle;
        List<KeyValuePair<string, UIImage>> _imageCache;
        List<KeyValuePair<string, UIImage>> _thumbnailImageCache;
		MPfmTableViewCell _movingCell = null;
        int _editingRowPosition = -1;
		int _editingRowSection = -1;

        public MobileLibraryBrowserViewController(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query)
            : base (UserInterfaceIdiomIsPhone ? "MobileLibraryBrowserViewController_iPhone" : "MobileLibraryBrowserViewController_iPad", null)
        {
            _tabType = tabType;
            _browserType = browserType;
            _query = query;
			_items = new List<Tuple<SectionIndex, List<LibraryBrowserEntity>>>();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Flush image cache and table view items
			//_items.Clear();
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
			tableView.Alpha = 0;
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;
			tableView.RegisterClassForHeaderFooterViewReuse(typeof(MPfmAlbumHeaderView), _headerCellIdentifier);
            collectionView.BackgroundColor = GlobalTheme.BackgroundColor;
            collectionView.WeakDataSource = this;
            collectionView.WeakDelegate = this;
			collectionView.CollectionViewLayout = new MPfmCollectionViewFlowLayout();
            collectionView.ContentSize = new SizeF(160, 160);
            collectionView.RegisterClassForCell(typeof(MPfmCollectionAlbumViewCell), _collectionCellIdentifier);
			collectionView.RegisterClassForSupplementaryView(typeof(MPfmCollectionHeaderView), UICollectionElementKindSection.Header, _collectionCellHeaderIdentifier);

			var pan = new UIPanGestureRecognizer(PanTableView);			
			pan.WeakDelegate = this;
			pan.MinimumNumberOfTouches = 1;
			pan.MaximumNumberOfTouches = 1;
			//pan.CancelsTouchesInView = false;
			tableView.AddGestureRecognizer(pan);

			//viewAlbumCover.Hidden = _browserType != MobileLibraryBrowserType.Songs;
			collectionView.Alpha = (_browserType == MobileLibraryBrowserType.Songs && _tabType != MobileNavigationTabType.Songs) ? 1 : 0;

			//imageViewAlbumCover.BackgroundColor = UIColor.Black;
			//viewAlbumCover.BackgroundColor = GlobalTheme.MainDarkColor;

            _imageCache = new List<KeyValuePair<string, UIImage>>();
            _thumbnailImageCache = new List<KeyValuePair<string, UIImage>>();
            this.NavigationItem.HidesBackButton = true;

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

		public override void WillMoveToParentViewController(UIViewController parent)
		{
			base.WillMoveToParentViewController(parent);

			//_items = new List<LibraryBrowserEntity>();
			//collectionView.ReloadData();
			//tableView.ReloadData();

			//Tracing.Log("MLBVC - WillMoveToParentViewController - RefreshNavBar");
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
				navTitle = "Songs";
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

			//Tracing.Log("MLBVC - RefreshNavBar - defaultTitle: {0} navTitle: {1} iconName: {2}", defaultTitle, navTitle, iconName);
			MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;

			if(navCtrl != null)
				navCtrl.SetTitle(navTitle, iconName);
		}

		#region UICollectionView DataSource/Delegate

        private void HandleLongPressCollectionCellRow(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
                return;

			Tracing.Log("MobileLibraryBrowserViewController - HandleLongPressCollectionCellRow");
            PointF pt = gestureRecognizer.LocationInView(collectionView);
            NSIndexPath indexPath = collectionView.IndexPathForItemAtPoint(pt);
			if (indexPath != null)
			{
				SetEditingCollectionCellRow(indexPath.Row, indexPath.Section);
				var cell = (MPfmCollectionAlbumViewCell)collectionView.CellForItem(indexPath);
				if (cell != null)
				{
					var newView = new UIView(cell.PlayButton.Frame);
					newView.BackgroundColor = UIColor.Purple;
					//UIView.Animate(0.2, 0, UIViewAnimationOptions.TransitionFlipFromTop
//					UIView.Transition(cell.PlayButton, newView, 0.4, UIViewAnimationOptions.TransitionFlipFromRight, () => {
//
//					});
				}
			}
        }

        private void ResetEditingCollectionCellRow()
        {
			SetEditingCollectionCellRow(-1, -1);
        }

		private void SetEditingCollectionCellRow(int position, int section)
        {
            int oldPosition = _editingRowPosition;
			int oldSection = _editingRowSection;
			_editingRowPosition = position;
			_editingRowSection = section;

            if (oldPosition >= 0)
            {
				var oldCell = (MPfmCollectionAlbumViewCell)collectionView.CellForItem(NSIndexPath.FromRowSection(oldPosition, oldSection));
                if (oldCell != null)
                {
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                        oldCell.PlayButton.Alpha = 0;
                        oldCell.AddButton.Alpha = 0;
                        oldCell.DeleteButton.Alpha = 0;
						oldCell.PlayButton.Frame = new RectangleF(((oldCell.Frame.Width - 44) / 2) - 8, 48, 44, 44);
						oldCell.AddButton.Frame = new RectangleF((oldCell.Frame.Width - 44) / 2 + 44, 48, 44, 44);
						oldCell.DeleteButton.Frame = new RectangleF(((oldCell.Frame.Width - 44) / 2) + 96, 48, 44, 44);
                    }, null);
                }
            }

            if (position >= 0)
            {
				var cell = (MPfmCollectionAlbumViewCell)collectionView.CellForItem(NSIndexPath.FromRowSection(position, section));
                if (cell != null)
                {
                    cell.PlayButton.Alpha = 0;
                    cell.AddButton.Alpha = 0;
                    cell.DeleteButton.Alpha = 0;
					cell.PlayButton.Frame = new RectangleF(((cell.Frame.Width - 44) / 2) - 8, 48, 44, 44);
					cell.AddButton.Frame = new RectangleF((cell.Frame.Width - 44) / 2 + 44, 48, 44, 44);
					cell.DeleteButton.Frame = new RectangleF(((cell.Frame.Width - 44) / 2) + 96, 48, 44, 44);
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                        cell.PlayButton.Alpha = 1;
                        cell.AddButton.Alpha = 1;
                        cell.DeleteButton.Alpha = 1;
						cell.PlayButton.Frame = new RectangleF(((cell.Frame.Width - 44) / 2) - 52, 48, 44, 44);
						cell.AddButton.Frame = new RectangleF((cell.Frame.Width - 44) / 2, 48, 44, 44);
						cell.DeleteButton.Frame = new RectangleF(((cell.Frame.Width - 44) / 2) + 52, 48, 44, 44);
                    }, null);
                }
            }
        }

        [Export ("collectionView:cellForItemAtIndexPath:")]
        public UICollectionViewCell CellForItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
        {
			//Tracing.Log("MobileLibraryBrowserViewController - CellForItemAtIndexPath - indexPath.row: {0} .section: {1}", indexPath.Row, indexPath.Section);
			var item = _items[indexPath.Section].Item2[indexPath.Row];
            var cell = (MPfmCollectionAlbumViewCell)collectionView.DequeueReusableCell(_collectionCellIdentifier, indexPath);
            cell.Tag = indexPath.Row;

            // Do not refresh the cell if the contents are the same.
            if (cell.Title == item.Title && cell.Subtitle == item.Subtitle)
                return cell;

            // Refresh cell contents
            cell.Title = item.Title;
            cell.Subtitle = item.Subtitle;
            if (_browserType == MobileLibraryBrowserType.Albums)
            {
                // Check if album art is cached
				string key = string.Format("{0}_{1}", item.Query.ArtistName, item.Query.AlbumTitle);
                KeyValuePair<string, UIImage> keyPair = _imageCache.FirstOrDefault(x => x.Key == key);
                if (keyPair.Equals(default(KeyValuePair<string, UIImage>)))
                {
                    cell.Image = null;
					OnRequestAlbumArt(item.Query.ArtistName, item.Query.AlbumTitle, indexPath);
                } 
                else
                {
                    cell.SetImage(keyPair.Value);
                }
            } 

			float alpha = _editingRowPosition == indexPath.Row && _editingRowSection == indexPath.Section ? 1 : 0;
			cell.PlayButton.Alpha = alpha;
			cell.AddButton.Alpha = alpha;
			cell.DeleteButton.Alpha = alpha;

            cell.PlayButton.TouchUpInside += HandleCollectionViewPlayTouchUpInside;
            cell.AddButton.TouchUpInside += HandleCollectionViewAddTouchUpInside;
            cell.DeleteButton.TouchUpInside += HandleCollectionViewDeleteTouchUpInside;

            return cell;
        }

        [Export ("collectionView:numberOfItemsInSection:")]
        public int NumberOfItemsInSection(UICollectionView collectionView, int section)
        {
            // Prevent loading table view cells when using a collection view
			return _browserType == MobileLibraryBrowserType.Albums && _items.Count - 1 >= section ? _items[section].Item2.Count : 0;
        }

        [Export ("numberOfSectionsInCollectionView:")]
        public int NumberOfSectionsInCollectionView(UICollectionView collectionView)
        {
			return _browserType == MobileLibraryBrowserType.Albums ? _items.Count : 0;
        }

        [Export ("collectionView:didSelectItemAtIndexPath:")]
        public void CollectionDidSelectItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ResetEditingCollectionCellRow();
			Guid id = _items[indexPath.Section].Item2[indexPath.Row].Id;
			OnItemClick(id);
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
			Guid id = _items[_editingRowSection].Item2[_editingRowPosition].Id;
			OnAddItemToPlaylist(id);
            ResetEditingCollectionCellRow();
        }

        private void HandleCollectionViewDeleteTouchUpInside(object sender, EventArgs e)
        {
            Tracing.Log("HandleCollectionViewDeleteTouchUpInside");

			var item = _items[_editingRowSection].Item2[_editingRowPosition];
            if (item == null)
                return;

            var alertView = new UIAlertView("Delete confirmation", string.Format("Are you sure you wish to delete {0}?", item.Title), null, "OK", new string[1] { "Cancel" });
            alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
                switch(e2.ButtonIndex)
                {
                    case 0:
						OnDeleteItem(item.Id);
                        break;
                }
                ResetEditingCollectionCellRow();
            };
            alertView.Show();
        }

        private void HandleCollectionViewPlayTouchUpInside(object sender, EventArgs e)
        {
			Tracing.Log("MobileLibraryBrowserViewCtrl - HandleCollectionViewPlayTouchUpInside");
			Guid id = _items[_editingRowSection].Item2[_editingRowPosition].Id;
			OnPlayItem(id);
            ResetEditingCollectionCellRow();
        }

        [Export ("collectionView:viewForSupplementaryElementOfKind:atIndexPath:")]
        public UICollectionReusableView ViewForSupplementaryElement(UICollectionView collectionView, string viewForSupplementaryElementOfKind, NSIndexPath indexPath)
        {
			//Tracing.Log("MobileLibraryBrowserViewCtrl - ViewForSupplementaryElement - kind: {0} section: {1} row: {2}", viewForSupplementaryElementOfKind, indexPath.Section, indexPath.Row);

//			// Do not show header when item count is 1
//			if (_itemsWithSections.Count <= 1)
//				return null;

			if (viewForSupplementaryElementOfKind == "UICollectionElementKindSectionHeader")
			{
				var view = (MPfmCollectionHeaderView)collectionView.DequeueReusableSupplementaryView(UICollectionElementKindSection.Header, _collectionCellHeaderIdentifier, indexPath);
				if (indexPath.Section <= _items.Count - 1)
					view.TextLabel.Text = _items[indexPath.Section].Item1.Title;
				return view;
			}
			return null;
        }

		#endregion

        #region UITableView DataSource/Delegate

		[Export ("scrollViewDidScroll:")]
		private void ScrollViewDidScroll(UIScrollView scrollView)
		{
			//Console.WriteLine("MLB >> ScrollViewDidScroll");
			_isTableViewScrolling = true;
		}

		[Export ("scrollViewDidEndScrollingAnimation:")]
		private void ScrollViewDidEndScrollingAnimation(UIScrollView scrollView)
		{
			//Console.WriteLine("MLB >> ScrollViewDidEndScrollingAnimation");
			_isTableViewScrolling = false;
		}

		[Export ("scrollViewDidEndDecelerating:")]
		private void ScrollViewDidEndDecelerating(UIScrollView scrollView)
		{
			//Console.WriteLine("MLB >> scrollViewDidEndDecelerating");
			_isTableViewScrolling = false;
		}

		[Export ("scrollViewDidEndDragging:willDecelerate:")]
		private void ScrollViewDidEndDragging(UIScrollView scrollView, bool willDecelerate)
		{
			//Console.WriteLine("MLB >> ScrollViewDidEndDragging - willDecelerate: {0}", willDecelerate);
			_isTableViewScrolling = false;
		}

		[Export ("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
		private bool ShouldRecognizeSimultaneouslyWithGestureRecognizer(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
		{
			return true;
		}

		private void PanTableView(UIPanGestureRecognizer panGestureRecognizer)
		{
			if (_isTableViewScrolling)
				return;

			var ptLocation = panGestureRecognizer.LocationInView(tableView);
			var ptTranslation = panGestureRecognizer.TranslationInView(tableView);
			//Console.WriteLine("Peter Pan - gesture state: {0} ptLocation: {1} ptTranslation: {2}", panGestureRecognizer.State, ptLocation, ptTranslation);

			// Block scrolling when the user clearly starts to go left or right
			float maxX = (View.Bounds.Width / 2f) + 28f + 14f + 8f;
			if (ptTranslation.X > 20f)
				tableView.ScrollEnabled = false;

			// Check when the cell is slided 
			if (panGestureRecognizer.State == UIGestureRecognizerState.Began)
			{
				var indexPath = tableView.IndexPathForRowAtPoint(ptLocation);
				if (indexPath == null)
					return;

				var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
				if (cell == null)
					return;

				// Cache cell for reuse later
				_movingIndexPath = indexPath;
				_movingCell = cell;

				// Refresh icon/text in case they are not in sync with the queue status
				cell.AddToPlaylistLabel.Text = cell.IsQueued ? "Remove from queue" : "Add to queue";
				cell.AddedToPlaylistLabel.Text = cell.IsQueued ? "Removed from queue!" : "Added to queue!";
				cell.ImageAddToPlaylist.Image = cell.IsQueued ? UIImage.FromBundle("Images/ContextualButtons/trash") : UIImage.FromBundle("Images/ContextualButtons/add");
			}
			else if (panGestureRecognizer.State == UIGestureRecognizerState.Ended)
			{
				float movingX = Math.Min(ptTranslation.X, maxX);
				if(movingX == maxX)
					AnimateCellQueueSuccess(_movingCell, _movingIndexPath.Section, _movingIndexPath.Row);
				else
					AnimateCellQueueCancel(_movingCell);

				tableView.ScrollEnabled = true;
				return;
			}

			AnimateCellQueueMovement(_movingCell, ptTranslation, maxX);
		}

		private void AnimateCellQueueMovement(MPfmTableViewCell cell, PointF ptTranslation, float maxX)
		{
			if (cell == null)
				return;

			var newFrame = _movingCell.ContainerView.Frame;
			newFrame.X = Math.Max(Math.Min(_movingCell.IsQueued ? 4 + ptTranslation.X : ptTranslation.X, maxX), 0);
			_movingCell.ContainerView.Frame = newFrame;

			float alpha = Math.Max(Math.Min(ptTranslation.X / 150f, 1), 0);
			float scale = Math.Max(Math.Min(ptTranslation.X / 150f, 1), 0);
			float scale2 = 0.5f + (scale * 0.5f);

			// Blue: 47, 129, 183
			// Red: 139, 0, 0
			int r1 = 47;
			int g1 = 129;
			int b1 = 183;
			int r2 = 139;
			int g2 = 0;
			int b2 = 0;
			int r = (int)(cell.IsQueued ? r1 + (scale * (r2 - r1)) : r1);
			int g = (int)(cell.IsQueued ? g1 + (scale * (g2 - g1)) : g1);
			int b = (int)(cell.IsQueued ? b1 + (scale * (b2 - b1)) : b1);
			cell.ImageAddToPlaylist.Alpha = alpha;
			cell.ImageAddToPlaylist.Transform = CGAffineTransform.MakeScale(scale2, scale2);
			cell.AddToPlaylistLabel.Alpha = alpha;
			cell.AddToPlaylistLabel.Transform = CGAffineTransform.MakeScale(scale2, scale2);
			cell.BehindView.BackgroundColor = UIColor.FromRGB(r, g, b);
			//Console.WriteLine("alpha: {0} scale: {1} scale2: {2} r: {3} g: {4} b: {5}", alpha, scale, scale2, r, g, b);
		}

		private void AnimateCellQueueSuccess(MPfmTableViewCell cell, int section, int row)
		{
			if (cell == null)
				return;

			UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
				{
					var newImageAddToPlaylistFrame = cell.ImageAddToPlaylist.Frame;
					newImageAddToPlaylistFrame.Y = -cell.Bounds.Height;
					cell.ImageAddToPlaylist.Alpha = 0.25f;
					cell.ImageAddToPlaylist.Frame = newImageAddToPlaylistFrame;
					cell.ImageAddToPlaylist.Transform = CGAffineTransform.MakeScale(1, 1);

					var newAddToPlaylistLabelFrame = cell.AddToPlaylistLabel.Frame;
					newAddToPlaylistLabelFrame.Y = -cell.Bounds.Height;
					cell.AddToPlaylistLabel.Alpha = 0.25f;
					cell.AddToPlaylistLabel.Frame = newAddToPlaylistLabelFrame;
					cell.AddToPlaylistLabel.Transform = CGAffineTransform.MakeScale(1, 1);

					var newAddedToPlaylistLabelFrame = cell.AddedToPlaylistLabel.Frame;
					newAddedToPlaylistLabelFrame.Y = 10;
					cell.AddedToPlaylistLabel.Frame = newAddedToPlaylistLabelFrame;
					cell.AddedToPlaylistLabel.Alpha = 1;

					cell.ImageCheckmark.Alpha = 0;
					cell.ImageCheckmarkConfirm.Alpha = 1;
				}, null);
			UIView.Animate(0.2, 0.75, UIViewAnimationOptions.CurveEaseInOut, () =>
				{
					cell.IsQueued = !cell.IsQueued;
					_items[section].Item2[row].IsQueued = cell.IsQueued;

					var containerFrame = cell.ContainerView.Frame;
					containerFrame.X = 0;
					cell.ContainerView.Frame = containerFrame;

					var containerBackgroundFrame = cell.ContainerView.Frame;
					containerBackgroundFrame.X = cell.IsQueued ? 4 : 0;
					cell.ContainerBackgroundView.Frame = containerBackgroundFrame;

					float alpha = 0.5f;
					//float scale = 0.5f;
					int r = cell.IsQueued ? 47 : 139;
					int g = cell.IsQueued ? 129 : 0;
					int b = cell.IsQueued ? 183 : 0;
					cell.ImageAddToPlaylist.Alpha = alpha;
					//cell.ImageAddToPlaylist.Transform = CGAffineTransform.MakeScale(scale, scale);
					cell.AddToPlaylistLabel.Alpha = alpha;
					//cell.AddToPlaylistLabel.Transform = CGAffineTransform.MakeScale(scale, scale);
					cell.BehindView.BackgroundColor = UIColor.FromRGB(r, g, b);
				}, () => {
					var newImageAddToPlaylistFrame = cell.ImageAddToPlaylist.Frame;
					newImageAddToPlaylistFrame.Y = 14;
					cell.ImageAddToPlaylist.Frame = newImageAddToPlaylistFrame;
					cell.ImageAddToPlaylist.Alpha = 0.1f;

					var newAddToPlaylistLabelFrame = cell.AddToPlaylistLabel.Frame;
					newAddToPlaylistLabelFrame.Y = 10;
					cell.AddToPlaylistLabel.Frame = newAddToPlaylistLabelFrame;
					cell.AddToPlaylistLabel.Alpha = 0.1f;

					var newAddedToPlaylistLabelFrame = cell.AddedToPlaylistLabel.Frame;
					newAddedToPlaylistLabelFrame.Y = 62;
					cell.AddedToPlaylistLabel.Frame = newAddedToPlaylistLabelFrame;
					cell.AddedToPlaylistLabel.Alpha = 0;

					cell.ImageCheckmark.Alpha = 1;
					cell.ImageCheckmarkConfirm.Alpha = 0;
				});
		}

		private void AnimateCellQueueCancel(MPfmTableViewCell cell)
		{
			if (cell == null)
				return;

			UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
				{
					var containerFrame = cell.ContainerView.Frame;
					containerFrame.X = 0;
					cell.ContainerView.Frame = containerFrame;

					var containerBackgroundFrame = cell.ContainerBackgroundView.Frame;
					containerBackgroundFrame.X = cell.IsQueued ? 4 : 0;
					cell.ContainerBackgroundView.Frame = containerBackgroundFrame;

					cell.ImageAddToPlaylist.Alpha = 0.1f;
					cell.ImageAddToPlaylist.Transform = CGAffineTransform.MakeScale(1, 1);
					cell.BehindView.BackgroundColor = UIColor.FromRGB(47, 129, 183);
				}, null);
		}

        private void HandleLongPressTableCellRow(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
                return;

            Tracing.Log("MobileLibraryBrowserViewController - HandleLongPressTableCellRow");
            PointF pt = gestureRecognizer.LocationInView(tableView);
            NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(pt);
			if (_editingRowPosition == indexPath.Row && _editingRowSection == indexPath.Section)
				ResetEditingTableCellRow();
			else
				SetEditingTableCellRow(indexPath.Section, indexPath.Row);
        }

        private void ResetEditingTableCellRow()
        {
			SetEditingTableCellRow(-1, -1);
        }

		private void SetEditingTableCellRow(int section, int row)
        {
			int oldRow = _editingRowPosition;
			int oldSection = _editingRowSection;
			_editingRowPosition = row;
			_editingRowSection = section;

            if (oldRow >= 0)
            {
				var oldCell = (MPfmTableViewCell)tableView.CellAt(NSIndexPath.FromRowSection(oldRow, oldSection));
                if (oldCell != null)
                {
					oldCell.ContainerBackgroundView.BackgroundColor = GlobalTheme.SecondaryColor;
					oldCell.IsDarkBackground = false;
					oldCell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
//						oldCell.PlayButton.Frame = new RectangleF(4, 52, 100, 64);
//						oldCell.AddButton.Frame = new RectangleF(108, 52, 100, 64);
//						oldCell.DeleteButton.Frame = new RectangleF(212, 52, 100, 64);
                        oldCell.TextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                        oldCell.DetailTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                        oldCell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
						oldCell.AlbumCountLabel.Transform = CGAffineTransform.MakeScale(1, 1);
						oldCell.ImageAlbum1.Transform = CGAffineTransform.MakeScale(1, 1);
						oldCell.ImageAlbum2.Transform = CGAffineTransform.MakeScale(1, 1);
						oldCell.ImageAlbum3.Transform = CGAffineTransform.MakeScale(1, 1);
						oldCell.PlayButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
						oldCell.AddButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
						oldCell.DeleteButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);

                        oldCell.PlayButton.Alpha = 0;
                        oldCell.AddButton.Alpha = 0;
                        oldCell.DeleteButton.Alpha = 0;
                        oldCell.AlbumCountLabel.Alpha = 0.75f;
                        oldCell.ImageAlbum1.Alpha = 0.75f;
                        oldCell.ImageAlbum2.Alpha = 0.4f;
                        oldCell.ImageAlbum3.Alpha = 0.2f;

						oldCell.ContainerBackgroundView.BackgroundColor = UIColor.White;
						oldCell.TextLabel.TextColor = UIColor.Black;
						oldCell.DetailTextLabel.TextColor = UIColor.Gray;
						oldCell.AlbumCountLabel.TextColor = UIColor.Black;
						oldCell.IndexTextLabel.TextColor = UIColor.FromRGB(0.5f, 0.5f, 0.5f);
						oldCell.PlayButton.UpdateLayout();
						oldCell.AddButton.UpdateLayout();
						oldCell.DeleteButton.UpdateLayout();
                    }, null);
                }
            }

			if (row >= 0)
            {
				var cell = (MPfmTableViewCell)tableView.CellAt(NSIndexPath.FromRowSection(row, section));
                if (cell != null)
                {
                    cell.PlayButton.Alpha = 0;
                    cell.AddButton.Alpha = 0;
                    cell.DeleteButton.Alpha = 0;
                    cell.AlbumCountLabel.Alpha = 0.75f;
                    cell.ImageAlbum1.Alpha = 0.75f;
                    cell.ImageAlbum2.Alpha = 0.4f;
                    cell.ImageAlbum3.Alpha = 0.2f;
//					cell.PlayButton.Frame = new RectangleF(4, 25, 100, 44);
//					cell.AddButton.Frame = new RectangleF(108, 25, 100, 44);
//					cell.DeleteButton.Frame = new RectangleF(212, 25, 100, 44);

					cell.ContainerBackgroundView.BackgroundColor = GlobalTheme.SecondaryColor;
					cell.IsDarkBackground = true;
					cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_white");
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                        cell.PlayButton.Alpha = 1;
                        cell.AddButton.Alpha = 1;
                        cell.DeleteButton.Alpha = 1;
						cell.AlbumCountLabel.Alpha = 1;
						cell.ImageAlbum1.Alpha = 0.5f;
						cell.ImageAlbum2.Alpha = 0.2f;
						cell.ImageAlbum3.Alpha = 0.075f;

                        cell.TextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                        cell.DetailTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                        cell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
						cell.AlbumCountLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
						cell.ImageAlbum1.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
						cell.ImageAlbum2.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
						cell.ImageAlbum3.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
						cell.PlayButton.Transform = CGAffineTransform.MakeScale(1, 1);
						cell.AddButton.Transform = CGAffineTransform.MakeScale(1, 1);
						cell.DeleteButton.Transform = CGAffineTransform.MakeScale(1, 1);

						cell.ContainerBackgroundView.BackgroundColor = GlobalTheme.BackgroundColor;
						cell.TextLabel.TextColor = UIColor.White;
						cell.DetailTextLabel.TextColor = UIColor.White;
						cell.IndexTextLabel.TextColor = UIColor.White;
						cell.AlbumCountLabel.TextColor = UIColor.White;

//						cell.PlayButton.Frame = new RectangleF(4, 52, 100, 64);
//						cell.AddButton.Frame = new RectangleF(108, 52, 100, 64);
//						cell.DeleteButton.Frame = new RectangleF(212, 52, 100, 64);
						cell.PlayButton.UpdateLayout();
						cell.AddButton.UpdateLayout();
						cell.DeleteButton.UpdateLayout();

                    }, null);
                }
            }

			// Execute animation for new row height (as simple as that!)
			tableView.BeginUpdates();
			tableView.EndUpdates();	
        }

		[Export ("numberOfSectionsInTableView:")]
		public int NumberOfSectionsInTableView(UITableView tableView)
		{
			return _browserType != MobileLibraryBrowserType.Albums ? _items.Count : 0;
		}

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
			// Prevent loading cells when using a collection view
			if (_browserType == MobileLibraryBrowserType.Albums)
				return 0;

			return _items.Count - 1 >= section ? _items[section].Item2.Count : 0;
        }

		[Export ("tableView:viewForHeaderInSection:")]
		public UIView ViewForHeaderInSection(UITableView tableview, int section)
		{
			if (_browserType != MobileLibraryBrowserType.Songs)
				return null;

			// TODO: DequeueReusableHeaderFooterView is supposed to work on iOS 6 but it crashes on iPhone 4S iOS 6.1.2
			MPfmAlbumHeaderView header = null;
			if(UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
				header = (MPfmAlbumHeaderView)tableView.DequeueReusableHeaderFooterView(_headerCellIdentifier);

			if (header == null)
				header = new MPfmAlbumHeaderView();

			var audioFile = _items[section].Item2[0].AudioFile; // there is at least one song per section
			int songCount = _items[section].Item2.Count;

			string totalLength = _items[section].Item1.TotalLength.Substring(0, _items[section].Item1.TotalLength.IndexOf(".", StringComparison.Ordinal));
			header.ArtistNameLabel.Text = audioFile.ArtistName;
			header.AlbumTitleLabel.Text = audioFile.AlbumTitle;
			header.TotalTimeLabel.Text = totalLength;
			header.SongCountLabel.Text = string.Format(songCount == 1 ? "{0} song" : "{0} songs", songCount);

			int height = UserInterfaceIdiomIsPhone ? (int)(84f * UIScreen.MainScreen.Scale) : (int)(96f * UIScreen.MainScreen.Scale); 
			header.AlbumImageView.Image = null;
			Task<UIImage>.Factory.StartNew(() => {
			    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);                        
			    using (NSData imageData = NSData.FromArray(bytesImage))
			    {
			        using (UIImage image = UIImage.LoadFromData(imageData))
			        {
			            if (image != null)
			            {
			                try
			                {
			                    UIImage imageResized = CoreGraphicsHelper.ScaleImage(image, height);
			                    return imageResized;
			                } 
			                catch (Exception ex)
			                {
								Console.WriteLine("MobileLibraryBrowserViewController - ViewForHeaderInSection - section: {0} - Error resizing image {1}/{2}: {3}", section, audioFile.ArtistName, audioFile.AlbumTitle, ex);
			                }
			            }
			        }
			    }
			    
			    return null;
			}).ContinueWith(t => {
			    UIImage image = t.Result;
			    if(image == null)
			        return;

				InvokeOnMainThread(() => {
					header.AlbumImageView.Alpha = 0;
					header.AlbumImageView.Image = image;
					UIView.Animate(0.2, () => header.AlbumImageView.Alpha = 1);
				});
			}, TaskScheduler.FromCurrentSynchronizationContext());

			return header;
		}

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
			var item = _items[indexPath.Section].Item2[indexPath.Row];
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
			cell.TextLabel.HighlightedTextColor = UIColor.White;
            cell.DetailTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 12);
            cell.DetailTextLabel.Text = item.Subtitle;
			cell.DetailTextLabel.HighlightedTextColor = UIColor.White;
			cell.AlbumCountLabel.HighlightedTextColor = UIColor.White;
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

			//var newContainerViewFrame = cell.ContainerView.Frame;
			//newContainerViewFrame.X = item.IsQueued ? 12 : 0;
			cell.IsQueued = item.IsQueued;
			//cell.ContainerView.Frame = newContainerViewFrame;

			bool isEditing = _editingRowPosition == indexPath.Row && _editingRowSection == indexPath.Section;
			cell.ImageChevron.Image = isEditing ? UIImage.FromBundle("Images/Tables/chevron_white") : UIImage.FromBundle("Images/Tables/chevron");
			cell.IsDarkBackground = isEditing;
			cell.ContainerBackgroundView.BackgroundColor = isEditing ? GlobalTheme.BackgroundColor : UIColor.White;
			cell.PlayButton.Alpha = isEditing ? 1 : 0;
			cell.AddButton.Alpha = isEditing ? 1 : 0;
			cell.DeleteButton.Alpha = isEditing ? 1 : 0;
			cell.AlbumCountLabel.Alpha = isEditing ? 1 : 0.75f;
			cell.ImageAlbum1.Alpha = isEditing ? 0.5f : 0.75f;
			cell.ImageAlbum2.Alpha = isEditing ? 0.2f : 0.4f;
			cell.ImageAlbum3.Alpha = isEditing ? 0.075f : 0.2f;
			cell.TextLabel.TextColor = isEditing ? UIColor.White : UIColor.Black;
			cell.DetailTextLabel.TextColor = isEditing ? UIColor.White : UIColor.Gray;
			cell.IndexTextLabel.TextColor = isEditing ? UIColor.White : UIColor.FromRGB(0.5f, 0.5f, 0.5f);
			cell.AlbumCountLabel.TextColor = isEditing ? UIColor.White : UIColor.Black;

			if (isEditing)
            {
                cell.TextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
				cell.DetailTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
				cell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
				cell.AlbumCountLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
				cell.ImageAlbum1.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
				cell.ImageAlbum2.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
				cell.ImageAlbum3.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
				cell.PlayButton.Transform = CGAffineTransform.MakeScale(1, 1);
				cell.AddButton.Transform = CGAffineTransform.MakeScale(1, 1);
				cell.DeleteButton.Transform = CGAffineTransform.MakeScale(1, 1);
            }
            else
            {
                cell.TextLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
                cell.DetailTextLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
                cell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
				cell.AlbumCountLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
				cell.ImageAlbum1.Transform = CGAffineTransform.MakeScale(1f, 1f);
				cell.ImageAlbum2.Transform = CGAffineTransform.MakeScale(1f, 1f);
				cell.ImageAlbum3.Transform = CGAffineTransform.MakeScale(1f, 1f);
				cell.PlayButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
				cell.AddButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
				cell.DeleteButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
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
                albumFetchCount = item.AlbumTitles.Count == 3 ? 3 : albumFetchCount;
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
			if(_editingRowPosition != -1)
            {
                Console.WriteLine("MLBVC - RowSelected - Deselecting row... - row: {0}", indexPath.Row);
                tableView.DeselectRow(indexPath, true);
                ResetEditingTableCellRow();
                return;
            }

            Console.WriteLine("MLBVC - RowSelected - OnItemClick - row: {0}", indexPath.Row);
			Guid id = _items[indexPath.Section].Item2[indexPath.Row].Id;
			OnItemClick(id);
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

			bool isEditing = _editingRowPosition == indexPath.Row && _editingRowSection == indexPath.Section;
			cell.ImageChevron.Image = isEditing ? UIImage.FromBundle("Images/Tables/chevron_white") : UIImage.FromBundle("Images/Tables/chevron");
            cell.RightImage.Image = UIImage.FromBundle("Images/Icons/icon_speaker");
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
			bool isEditing = _editingRowPosition == indexPath.Row && _editingRowSection == indexPath.Section;
			return isEditing ? 126 : 52;
        }

		[Export ("tableView:heightForHeaderInSection:")]
		public float HeightForHeaderInSection(UITableView tableView, int section)
		{
			return _browserType == MobileLibraryBrowserType.Songs ? UserInterfaceIdiomIsPhone ? 84 : 96 : 0;
		}

        private void HandleTableViewAddTouchUpInside(object sender, EventArgs e)
        {
			Guid id = _items[_editingRowSection].Item2[_editingRowPosition].Id;
			OnAddItemToPlaylist(id);
            ResetEditingTableCellRow();
        }

        private void HandleTableViewDeleteTouchUpInside(object sender, EventArgs e)
        {
			var item = _items[_editingRowSection].Item2[_editingRowPosition];
            if (item == null)
                return;

            var alertView = new UIAlertView("Delete confirmation", string.Format("Are you sure you wish to delete {0}?", item.Title), null, "OK", new string[1] { "Cancel" });
            alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
                switch(e2.ButtonIndex)
                {
                    case 0:
						OnDeleteItem(item.Id);
                        break;
                }
                ResetEditingTableCellRow();
            };
            alertView.Show();           
        }

        private void HandleTableViewPlayTouchUpInside(object sender, EventArgs e)
        {
			Guid id = _items[_editingRowSection].Item2[_editingRowPosition].Id;
			OnPlayItem(id);
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
		public Action<Guid> OnItemClick { get; set; }
		public Action<Guid> OnDeleteItem { get; set; }
        public Action<string, string, object> OnRequestAlbumArt { get; set; }
		public Action<Guid> OnPlayItem { get; set; }
        public Func<string, string, byte[]> OnRequestAlbumArtSynchronously { get; set; }
		public Action<Guid> OnAddItemToPlaylist { get; set; }

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
			if (albumArtData != null && albumArtData.Length > 0)
			{
				Task.Factory.StartNew(() =>
				{
					lock (_locker)
					{
						string filePath = Path.Combine(PathHelper.PeakFileDirectory, string.Format("{0}_{1}.png", artistName, albumTitle));
						if(!File.Exists(filePath))
							using (var file = File.Open(filePath, FileMode.OpenOrCreate))
								file.Write(albumArtData, 0, albumArtData.Length);
					}
				});
			}

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
                                return imageResized;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error resizing image " + artistName + " - " + albumTitle + ": " + ex.Message);
                }

                return null;
            });
            
			UIImage image = await task;
            if(image == null)
                return;

			//Console.WriteLine("MLBVC - RefreshAlbumArtCell - ContinueWith - artistName: {0} albumTitle: {1} browserType: {2} userData==null: {3} image==null: {4}", artistName, albumTitle, _browserType, userData == null, image == null);
            InvokeOnMainThread(() => {
                switch (_browserType)
                {
                    case MobileLibraryBrowserType.Artists:
                        // Remove older image from cache if exceeds cache size
                        if(_thumbnailImageCache.Count > 80)
                            _thumbnailImageCache.RemoveAt(0);

                        // Add image to cache
                        _thumbnailImageCache.Add(new KeyValuePair<string, UIImage>(artistName + "_" + albumTitle, image));

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
                        break;
                    case MobileLibraryBrowserType.Albums:
                        // Remove older image from cache if exceeds cache size
                        if(_imageCache.Count > 20)
                            _imageCache.RemoveAt(0);

                        _imageCache.Add(new KeyValuePair<string, UIImage>(artistName + "_" + albumTitle, image));
						var indexPath = (NSIndexPath)userData;
						var cellAlbumTitle = (MPfmCollectionAlbumViewCell)collectionView.CellForItem(indexPath);
						if (cellAlbumTitle == null)
                            return;

                        if(cellAlbumTitle.Image != image)
                            cellAlbumTitle.SetImage(image);
                        break;
                }
            });
        }
    
        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle, string navigationBarSubtitle, string breadcrumb, bool isPopBackstack, bool isBackstackEmpty)
        {
            InvokeOnMainThread(() => {
				_editingRowSection = -1;
				_editingRowPosition = -1;
                _browserType = browserType;
                _navigationBarSubtitle = navigationBarSubtitle;
				RefreshNavigationBar(_navigationBarSubtitle);

                // Reset scroll bar
                tableView.ScrollRectToVisible(new RectangleF(0, 0, 1, 1), false);

				// Generate list of items with sections
				_items.Clear();
				if(browserType == MobileLibraryBrowserType.Playlists || browserType == MobileLibraryBrowserType.Artists || 
				   browserType == MobileLibraryBrowserType.Albums)
				{
					var distinctArtists = entities.Select(x => x.Subtitle).OrderBy(x => x).Distinct().ToList();
					foreach(var artist in distinctArtists)
					{
						var sectionIndex = new SectionIndex(){
							ArtistName = artist,
							Title = artist
						};
						_items.Add(new Tuple<SectionIndex, List<LibraryBrowserEntity>>(sectionIndex, entities.Where(x => x.Subtitle == artist).ToList()));
					}
				}
				else if(browserType == MobileLibraryBrowserType.Songs)
				{
					var distinctArtists = entities.Select(x => x.AudioFile.ArtistName).OrderBy(x => x).Distinct().ToList();
					foreach(var artist in distinctArtists)
					{
						var distinctAlbums = entities.Where(x => x.AudioFile.ArtistName == artist).Select(x => x.AudioFile.AlbumTitle).OrderBy(x => x).Distinct().ToList();
						foreach(var album in distinctAlbums)
						{
							var list = entities.Where(x => x.AudioFile.ArtistName == artist && x.AudioFile.AlbumTitle == album).OrderBy(x => x.AudioFile.TrackNumber).ToList();

							// Calculate total length
							long ms = 0;
							string totalLength = string.Empty;
							foreach(var item in list)
								ms += Conversion.TimeStringToMilliseconds(item.AudioFile.Length);
							totalLength = Conversion.MillisecondsToTimeString((ulong)ms);

							var sectionIndex = new SectionIndex(){
								ArtistName = artist,
								AlbumTitle = album,
								Title = artist,
								TotalLength = totalLength
							};
							_items.Add(new Tuple<SectionIndex, List<LibraryBrowserEntity>>(sectionIndex, list));
						}
					}
				}

				// Fade out loading
				if(viewLoading.Alpha > 0)
				{
					activityIndicator.StopAnimating();
					UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => viewLoading.Alpha = 0, null);
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
						UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => collectionView.Alpha = 1, null);
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
						UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => tableView.Alpha = 1, null);
					}
                }

                // Hide album cover if not showing songs
                if(browserType != MobileLibraryBrowserType.Songs)
                {
					tableView.Frame = new RectangleF(View.Frame.X, View.Frame.Y - 44 - 20, View.Frame.Width, View.Frame.Height);
                }
                else
                {
					if(_items.Count == 0)
                        return;
                }
            });
        }

		public void RefreshCurrentlyPlayingSong(Guid id, AudioFile audioFile)
        {
			//Console.WriteLine("MLBVC - RefreshCurrentlyPlayingSong id: {0} audioFile: {1}", id, audioFile.FilePath);
            _currentlyPlayingSongId = audioFile != null ? audioFile.Id : Guid.Empty;
            InvokeOnMainThread(() => {

				// Only shown on Artists/Songs view types.
				int sectionIndex = -1;
				if(_browserType == MobileLibraryBrowserType.Artists)
					sectionIndex = _items.FindIndex(x => x.Item1.ArtistName == audioFile.ArtistName);
				else if(_browserType == MobileLibraryBrowserType.Songs)
					sectionIndex = _items.FindIndex(x => x.Item1.ArtistName == audioFile.ArtistName && x.Item1.AlbumTitle == audioFile.AlbumTitle);

				if(sectionIndex == -1)
					return;

				int index = _items[sectionIndex].Item2.FindIndex(x => x.Id == id);
				if(index == -1)
					return;

				var cell = (MPfmTableViewCell)tableView.CellAt(NSIndexPath.FromRowSection(index, sectionIndex));
				if(cell == null)
					return;

				if(_items[sectionIndex].Item2[index].AudioFile == null)
					return;

				cell.RightImage.Hidden = id == _items[sectionIndex].Item2[index].AudioFile.Id ? false : true;


//                    if(_items[cell.Tag].AudioFile != null)
//                    {
//                        var id = _items[cell.Tag].AudioFile.Id;
//                        var customCell = (MPfmTableViewCell)cell;
//                        if(id == audioFile.Id)
//                            customCell.RightImage.Hidden = false;
//                        else
//                            customCell.RightImage.Hidden = true;
//                    }
//                }
            });
        }

        public void NotifyNewPlaylistItems(string text)
        {
        }

        #endregion

		public class SectionIndex
		{
			public string Title { get; set; }
			public string ArtistName { get; set; }
			public string AlbumTitle { get; set; }
			public string TotalLength { get; set; }
		}
    }
}
