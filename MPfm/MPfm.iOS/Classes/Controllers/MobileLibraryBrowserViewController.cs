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
using MPfm.iOS.Classes.Delegates;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;

namespace MPfm.iOS.Classes.Controllers
{
    public partial class MobileLibraryBrowserViewController : BaseViewController, IMobileLibraryBrowserView
    {
        private bool _viewHasAlreadyBeenShown = false;
        private List<LibraryBrowserEntity> _items;
        private string _cellIdentifier = "MobileLibraryBrowserCell";
        private string _navigationBarTitle;
        private string _navigationBarSubtitle;
        private MobileLibraryBrowserType _browserType;
        private List<KeyValuePair<string, UIImage>> _imageCache;
        private UIBarButtonItem btnBack;

        private UIButton _btnDelete;
        private int _deleteCellIndex = -1;

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

            imageViewAlbumCover.BackgroundColor = UIColor.Black;
            viewAlbumCover.BackgroundColor = GlobalTheme.MainDarkColor;

            lblArtistName.Font = UIFont.FromName("HelveticaNeue-Medium", 16);
            lblAlbumTitle.Font = UIFont.FromName("HelveticaNeue", 14);
            lblSubtitle1.Font = UIFont.FromName("HelveticaNeue", 12);
            lblSubtitle2.Font = UIFont.FromName("HelveticaNeue", 12);

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

            // Create text attributes for navigation bar button
            UITextAttributes attr = new UITextAttributes();
            attr.Font = UIFont.FromName("HelveticaNeue-Medium", 12);
            attr.TextColor = UIColor.White;
            attr.TextShadowColor = UIColor.DarkGray;
            attr.TextShadowOffset = new UIOffset(0, 0);
            
            btnBack = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null, null);
            btnBack.SetTitleTextAttributes(attr, UIControlState.Normal);
            this.NavigationItem.BackBarButtonItem = btnBack;

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

        private void HandleSwipe(UISwipeGestureRecognizer gestureRecognizer)
        {
            var point = gestureRecognizer.LocationInView(tableView);
            var indexPath = tableView.IndexPathForRowAtPoint(point);
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

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _items.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
                cell = new MPfmTableViewCell(UITableViewCellStyle.Subtitle, _cellIdentifier);

            // Set title            
            cell.Tag = indexPath.Row;
            cell.Accessory = UITableViewCellAccessory.None;
            cell.TextLabel.Font = UIFont.FromName("HelveticaNeue", 14);
            cell.TextLabel.Text = _items[indexPath.Row].Title;
            cell.DetailTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 12);
            cell.DetailTextLabel.Text = _items[indexPath.Row].Subtitle;
            cell.ImageView.AutoresizingMask = UIViewAutoresizing.None;
            cell.ImageView.ClipsToBounds = true;
            cell.ImageChevron.Hidden = false;

            if(indexPath.Row == _deleteCellIndex)
                cell.ImageChevron.Frame = new RectangleF(cell.Frame.Width - cell.ImageChevron.Bounds.Width - 60, 0, cell.ImageChevron.Bounds.Width, cell.ImageChevron.Bounds.Height);
            else
                cell.ImageChevron.Frame = new RectangleF(cell.Frame.Width - cell.ImageChevron.Bounds.Width, 0, cell.ImageChevron.Bounds.Width, cell.ImageChevron.Bounds.Height);

            if(String.IsNullOrEmpty(_items[indexPath.Row].Subtitle))
                cell.TextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 16);

            if (_browserType == MobileLibraryBrowserType.Albums)
            {
                // Check if album art is cached
                string key = _items[indexPath.Row].Query.ArtistName + "_" + _items[indexPath.Row].Query.AlbumTitle;
                KeyValuePair<string, UIImage> keyPair = _imageCache.FirstOrDefault(x => x.Key == key);
                if (keyPair.Equals(default(KeyValuePair<string, UIImage>)))
                {
                    cell.ImageView.Image = UIImage.FromBundle("Images/emptyalbumart");
                    OnRequestAlbumArt(_items[indexPath.Row].Query.ArtistName, _items[indexPath.Row].Query.AlbumTitle);
                } 
                else
                {
                    cell.ImageView.Image = keyPair.Value;
                }
            } 
            else if (_browserType == MobileLibraryBrowserType.Songs)
            {
                cell.IndexTextLabel.Text = _items[indexPath.Row].AudioFile.TrackNumber.ToString();
            }            
            
            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if(indexPath.Row == _deleteCellIndex)            
                UIView.Animate(0.2, () => {
                    var cell = (MPfmTableViewCell)tableView.CellAt(indexPath);
                    _deleteCellIndex = -1;
                    _btnDelete.Alpha = 0;
                    _btnDelete.Frame = new RectangleF(cell.Frame.Width + 60, _btnDelete.Frame.Y, 60, _btnDelete.Frame.Height);
                    cell.ImageChevron.Frame = new RectangleF(cell.Frame.Width - cell.ImageChevron.Bounds.Width, 0, cell.ImageChevron.Bounds.Width, cell.ImageChevron.Bounds.Height);
                });

            OnItemClick(indexPath.Row);
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 44;
        }

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
        
        public Action<int> OnItemClick { get; set; }
        public Action<int> OnDeleteItem { get; set; }
        public Action<string, string> OnRequestAlbumArt { get; set; }

        public void MobileLibraryBrowserError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("MobileLibraryBrowser Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshAlbumArtCell(string artistName, string albumTitle, byte[] albumArtData)
        {
            // Note: cannot call UIScreen.MainScreen in a background thread!
            int height = 44;
            InvokeOnMainThread(() => {
                height = (int)(44 * UIScreen.MainScreen.Scale);
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
                    var cell = tableView.VisibleCells.FirstOrDefault(x => x.Tag == index);
                    if (cell == null)
                        return;

                    // Make sure cell is available
                    if(cell != null && cell.ImageView != null)
                    {
                        cell.ImageView.Alpha = 0;
                        cell.ImageView.Image = image;
                        cell.ImageView.Frame = new RectangleF(0, 0, 44, 44);
                        UIView.Animate(0.1, () => {
                            cell.ImageView.Alpha = 1;
                        });
                    }
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    
        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle, string navigationBarSubtitle)
        {
            InvokeOnMainThread(() => {
                _items = entities.ToList();
                _browserType = browserType;
                _navigationBarTitle = navigationBarTitle;
                _navigationBarSubtitle = navigationBarSubtitle;
                tableView.ReloadData();

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
            InvokeOnMainThread(() => {
                MPfmTableViewCell cell = (MPfmTableViewCell)tableView.CellAt(NSIndexPath.FromRowSection(index, 0));
                if(cell != null)
                {

                }
            });
        }

        #endregion

    }
}

