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

namespace MPfm.iOS.Classes.Controllers
{
    public partial class MobileLibraryBrowserViewController : BaseViewController, IMobileLibraryBrowserView
    {
        Guid _currentlyPlayingSongId;
        bool _viewHasAlreadyBeenShown = false;
        List<LibraryBrowserEntity> _items;
        string _cellIdentifier = "MobileLibraryBrowserCell";
        NSString _collectionCellIdentifier = new NSString("MobileLibraryBrowserCollectionCell");
        string _navigationBarTitle;
        string _navigationBarSubtitle;
        MobileLibraryBrowserType _browserType;
        List<KeyValuePair<string, UIImage>> _imageCache;
        UIButton _btnDelete;
        int _deleteCellIndex = -1;

        public MobileLibraryBrowserViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "MobileLibraryBrowserViewController_iPhone" : "MobileLibraryBrowserViewController_iPad", null)
        {
            _items = new List<LibraryBrowserEntity>();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Flush image cache and table view items
            _items.Clear();
            _imageCache.Clear();
            tableView.ReloadData();
        }
        
        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;
            collectionView.CollectionViewLayout = new MPfmCollectionViewFlowLayout();
            collectionView.BackgroundColor = GlobalTheme.BackgroundColor;
            collectionView.WeakDataSource = this;
            collectionView.WeakDelegate = this;
            collectionView.Alpha = 1;
            collectionView.ContentSize = new SizeF(160, 160);
            collectionView.RegisterClassForCell(typeof(MPfmCollectionAlbumViewCell), _collectionCellIdentifier);

            imageViewAlbumCover.BackgroundColor = UIColor.Black;
            viewAlbumCover.BackgroundColor = GlobalTheme.MainDarkColor;

            // TODO: Move this to a custom table view instead.
            _btnDelete = new UIButton(UIButtonType.Custom);
            _btnDelete.Alpha = 0;
            _btnDelete.Font = UIFont.FromName("HelveticaNeue-Light", 15);
            _btnDelete.SetTitle("Delete", UIControlState.Normal);
            _btnDelete.BackgroundColor = UIColor.Red;
            _btnDelete.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnDelete.SetTitleColor(UIColor.FromRGB(255, 180, 180), UIControlState.Highlighted);
            _btnDelete.TouchUpInside += (sender, e) => {

                var alertView = new UIAlertView("Delete confirmation", string.Format("Are you sure you wish to delete {0}?", 
                                                                                     _items[_deleteCellIndex].Title), null, "OK", new string[1]{"Cancel"});
                alertView.Clicked += (sender2, e2) => {
                    if(e2.ButtonIndex == 0)
                    {
                        // Remove immediately from table view
                        _items.RemoveAt(_deleteCellIndex);
                        tableView.BeginUpdates();
                        tableView.DeleteRows(new NSIndexPath[1]{ NSIndexPath.FromRowSection(_deleteCellIndex, 0) }, UITableViewRowAnimation.Right);
                        tableView.EndUpdates();

                        OnDeleteItem(_deleteCellIndex);
                        _deleteCellIndex = -1;
                        UIView.Animate(0.2, () => {
                            _btnDelete.Alpha = 0;
                        });
                    }
                    else
                    {
                        UIView.Animate(0.2, () => {
                            var cell = (MPfmTableViewCell)tableView.CellAt(NSIndexPath.FromRowSection(_deleteCellIndex, 0));
                            _btnDelete.Alpha = 0;
                            _btnDelete.Frame = new RectangleF(cell.Frame.Width + 60, _btnDelete.Frame.Y, 60, _btnDelete.Frame.Height);
                            cell.ImageChevron.Frame = new RectangleF(cell.Frame.Width - cell.ImageChevron.Bounds.Width, 0, cell.ImageChevron.Bounds.Width, cell.ImageChevron.Bounds.Height);
                        });
                    }
                };
                alertView.Show();
            };
            tableView.AddSubview(_btnDelete);
            _imageCache = new List<KeyValuePair<string, UIImage>>();
            this.NavigationItem.HidesBackButton = true;

            UISwipeGestureRecognizer swipe = new UISwipeGestureRecognizer(HandleSwipe);
            swipe.Direction = UISwipeGestureRecognizerDirection.Right;
            tableView.AddGestureRecognizer(swipe);

            base.ViewDidLoad();            
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle(_navigationBarTitle, _navigationBarSubtitle);

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

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            var screenSize = UIKitHelper.GetDeviceSize();

            if(_deleteCellIndex >= 0)
            {
                _btnDelete.Frame = new RectangleF(screenSize.Width - 60, _btnDelete.Frame.Y, 60, _btnDelete.Frame.Height);

                var cell = tableView.CellAt(NSIndexPath.FromRowSection(_deleteCellIndex, 0));
                if(cell != null)
                {
                    var customCell = (MPfmTableViewCell)cell;
                    customCell.ImageChevron.Frame = new RectangleF(customCell.Frame.Width - customCell.ImageChevron.Bounds.Width - 60, 0, customCell.ImageChevron.Bounds.Width, customCell.ImageChevron.Bounds.Height);
                }
            }
            else
            {
                _btnDelete.Frame = new RectangleF(screenSize.Width + 60, _btnDelete.Frame.Y, 60, _btnDelete.Frame.Height);
            }
        }

        private void HandleSwipe(UISwipeGestureRecognizer gestureRecognizer)
        {
            var point = gestureRecognizer.LocationInView(tableView);
            var indexPath = tableView.IndexPathForRowAtPoint(point);

            // IndexPath is null when swiping an empty cell
            if (indexPath == null)
                return;

            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);

            Console.WriteLine("MobileLibraryBrowserViewController - HandleSwipe - row: {0} deleteCellIndex: {1}", indexPath.Row, _deleteCellIndex);
            if (_deleteCellIndex == -1)
            {
                // no previous selection; just show button
                if(_btnDelete.Alpha == 0)
                    _btnDelete.Frame = new RectangleF(cell.Frame.Width, cell.Frame.Y, 60, cell.Frame.Height);

                UIView.Animate(0.2, () => {
                    _deleteCellIndex = indexPath.Row;
                    _btnDelete.Alpha = 1;
                    _btnDelete.Frame = new RectangleF(cell.Frame.Width - 60, _btnDelete.Frame.Y, 60, _btnDelete.Frame.Height);
                    cell.ImageChevron.Frame = new RectangleF(cell.Frame.Width - cell.ImageChevron.Bounds.Width - 60, 0, cell.ImageChevron.Bounds.Width, cell.ImageChevron.Bounds.Height);
                });
            }
            else if (_deleteCellIndex == indexPath.Row)
            {
                // same cell; hide button
                UIView.Animate(0.2, () => {
                    _deleteCellIndex = -1;
                    _btnDelete.Alpha = 0;
                    _btnDelete.Frame = new RectangleF(cell.Frame.Width + 60, _btnDelete.Frame.Y, 60, _btnDelete.Frame.Height);
                    cell.ImageChevron.Frame = new RectangleF(cell.Frame.Width - cell.ImageChevron.Bounds.Width, 0, cell.ImageChevron.Bounds.Width, cell.ImageChevron.Bounds.Height);
                });
            }
            else
            {
                // else: hide the button in the other cell.
                UIView.Animate(0.2, () => {
                    // Hide button in previous cell
                    var cellPrevious = (MPfmTableViewCell)tableView.CellAt(NSIndexPath.FromRowSection(_deleteCellIndex, 0));
                    if(cellPrevious != null)
                    {
                        cellPrevious.ImageChevron.Frame = new RectangleF(cellPrevious.Frame.Width - cellPrevious.ImageChevron.Bounds.Width, 0, cellPrevious.ImageChevron.Bounds.Width, cellPrevious.ImageChevron.Bounds.Height);
                        _btnDelete.Frame = new RectangleF(cellPrevious.Frame.Width + 60, _btnDelete.Frame.Y, 60, _btnDelete.Frame.Height);
                        _btnDelete.Alpha = 0;
                    }
                }, () => {
                    // Show button in new cell
                    _btnDelete.Frame = new RectangleF(cell.Frame.Width, cell.Frame.Y, 60, cell.Frame.Height);
                    UIView.Animate(0.2, () => {
                        _deleteCellIndex = indexPath.Row;
                        _btnDelete.Alpha = 1;
                        _btnDelete.Frame = new RectangleF(cell.Frame.Width - 60, _btnDelete.Frame.Y, 60, _btnDelete.Frame.Height);
                        cell.ImageChevron.Frame = new RectangleF(cell.Frame.Width - cell.ImageChevron.Bounds.Width - 60, 0, cell.ImageChevron.Bounds.Width, cell.ImageChevron.Bounds.Height);
                    });
                });
            }
        }

        [Export ("collectionView:cellForItemAtIndexPath:")]
        public UICollectionViewCell CellForItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath)
        {
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

//        [Export ("collectionView:viewForSupplementaryElementOfKind:atIndexPath:")]
//        public UICollectionReusableView ViewForSupplementaryElement(UICollectionView collectionView, string viewForSupplementaryElementOfKind, NSIndexPath indexPath)
//        {
//            return null;
//        }

        #region UITableView DataSource/Delegate

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
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
                cell = new MPfmTableViewCell(UITableViewCellStyle.Subtitle, _cellIdentifier);

            cell.Tag = indexPath.Row;
            cell.Accessory = UITableViewCellAccessory.None;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue", 14);
            cell.TextLabel.Text = _items[indexPath.Row].Title;
            cell.DetailTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 12);
            cell.DetailTextLabel.Text = _items[indexPath.Row].Subtitle;
            cell.ImageView.AutoresizingMask = UIViewAutoresizing.None;
            cell.ImageView.ClipsToBounds = true;
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
            cell.ImageChevron.Hidden = false;

            if (indexPath.Row == _deleteCellIndex)
                cell.RightOffset = 60;
            else
                cell.RightOffset = 0;

            if(String.IsNullOrEmpty(_items[indexPath.Row].Subtitle))
                cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);

            if (_browserType == MobileLibraryBrowserType.Songs)
            {
                cell.IndexTextLabel.Text = _items[indexPath.Row].AudioFile.TrackNumber.ToString();
                if (_currentlyPlayingSongId == _items[indexPath.Row].AudioFile.Id)
                    cell.RightImage.Hidden = false;
                else
                    cell.RightImage.Hidden = true;
            }
            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if(_deleteCellIndex >= 0)
            {
                UIView.Animate(0.2, () => {
                    var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
                    _deleteCellIndex = -1;
                    _btnDelete.Alpha = 0;
                    _btnDelete.Frame = new RectangleF(cell.Frame.Width + 60, _btnDelete.Frame.Y, 60, _btnDelete.Frame.Height);
                    cell.ImageChevron.Frame = new RectangleF(cell.Frame.Width - cell.ImageChevron.Bounds.Width, 0, cell.ImageChevron.Bounds.Width, cell.ImageChevron.Bounds.Height);
                });
            }

            OnItemClick(indexPath.Row);
        }

        [Export ("tableView:didHighlightRowAtIndexPath:")]
        public void DidHighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
            if (cell == null)
                return;

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

        public void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData, object userData)
        {
            // Note: cannot call UIScreen.MainScreen in a background thread!
//            int height = 44;
//            InvokeOnMainThread(() => {
//                height = (int)(44 * UIScreen.MainScreen.Scale);
//            });
            int height = 160;
            InvokeOnMainThread(() => {
                height = (int)(160 * UIScreen.MainScreen.Scale);
            });
            Task<UIImage>.Factory.StartNew(() => {
                using (NSData imageData = NSData.FromArray(albumArtData))
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
                                Console.WriteLine("Error resizing image " + artistName + " - " + albumTitle + ": " + ex.Message);
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

                    // Remove older image from cache if exceeds cache size
                    if(_imageCache.Count > 20)
                        _imageCache.RemoveAt(0);
                    
                    // Add image to cache
                    _imageCache.Add(new KeyValuePair<string, UIImage>(artistName + "_" + albumTitle, image));

                    // Get item from list
                    var item = _items.FirstOrDefault(x => x.Query.ArtistName == artistName && x.Query.AlbumTitle == albumTitle);
                    if (item == null)
                        return;
  
                    // Get cell from item
                    int index = _items.IndexOf(item);
                    var cell = (MPfmCollectionAlbumViewCell)collectionView.VisibleCells.FirstOrDefault(x => x.Tag == index);
                    if (cell == null)
                        return;

                    if(cell.Image != image)
                        cell.SetImage(image);

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
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    
        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle, string navigationBarSubtitle, string breadcrumb, bool isPopBackstack, bool isBackstackEmpty)
        {
            InvokeOnMainThread(() => {
                _items = entities.ToList();
                _browserType = browserType;
                _navigationBarTitle = navigationBarTitle;
                _navigationBarSubtitle = navigationBarSubtitle;

                // Reset scroll bar
                tableView.ScrollRectToVisible(new RectangleF(0, 0, 1, 1), false);

                if(browserType == MobileLibraryBrowserType.Albums)
                {
                    tableView.Hidden = true;
                    if(!collectionView.Hidden)
                    {
                        // Prevent the first useless refresh (this "flashes" the album art image views")
                        //Console.WriteLine("MLBVC - RefreshLibraryBrowser - Refreshing collection view...");
                        collectionView.ReloadData();
                    }
                    collectionView.Hidden = false;
                }
                else
                {
                    tableView.Hidden = false;
                    collectionView.Hidden = true;
                    //Console.WriteLine("MLBVC - RefreshLibraryBrowser - Refreshing table view...");
                    tableView.ReloadData();
                }

                // Hide album cover if not showing songs
                if(browserType != MobileLibraryBrowserType.Songs)
                {
                    viewAlbumCover.Hidden = true;
                    tableView.Frame = this.View.Frame;
                }
                else
                {
                    if(_items.Count == 0)
                        return;

                    var audioFile = _items[0].AudioFile;
                    lblArtistName.Text = audioFile.ArtistName;
                    lblAlbumTitle.Text = audioFile.AlbumTitle;
                    lblSubtitle1.Text = _items.Count().ToString() + " songs";

                    // Note: cannot call UIScreen.MainScreen in a background thread!
                    int height = (int)(viewAlbumCover.Bounds.Height * UIScreen.MainScreen.Scale);
                    imageViewAlbumCover.Image = UIImage.FromBundle("Images/emptyalbumart");
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
                                        Console.WriteLine("Error resizing image " + audioFile.ArtistName + " - " + audioFile.AlbumTitle + ": " + ex.Message);
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
                            imageViewAlbumCover.Image = image;                                                                                               
                        });
                    }, TaskScheduler.FromCurrentSynchronizationContext());
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
