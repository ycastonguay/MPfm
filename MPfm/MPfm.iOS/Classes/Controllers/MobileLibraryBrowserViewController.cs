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
using System.Collections.Concurrent;
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

            lblArtistName.Font = UIFont.FromName("HelveticaNeue-Medium", 16);
            lblAlbumTitle.Font = UIFont.FromName("HelveticaNeue", 14);
            lblSubtitle1.Font = UIFont.FromName("HelveticaNeue", 12);
            lblSubtitle2.Font = UIFont.FromName("HelveticaNeue", 12);

            //_currentTask = Task.Factory.StartNew (() => { });
            _imageCache = new List<KeyValuePair<string, UIImage>>();

            // Create text attributes for navigation bar button
            UITextAttributes attr = new UITextAttributes();
            attr.Font = UIFont.FromName("HelveticaNeue-Medium", 12);
            attr.TextColor = UIColor.White;
            attr.TextShadowColor = UIColor.DarkGray;
            attr.TextShadowOffset = new UIOffset(0, 0);
            
            // Set back button for navigation bar
            btnBack = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null, null);
            btnBack.SetTitleTextAttributes(attr, UIControlState.Normal);
            this.NavigationItem.BackBarButtonItem = btnBack;

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

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _items.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // Request a recycled cell to save memory
            MPfmTableViewCell cell = (MPfmTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            
            // Set cell style
            var cellStyle = UITableViewCellStyle.Subtitle;
            
            // Create cell if cell could not be recycled
            if (cell == null)
                cell = new MPfmTableViewCell(cellStyle, _cellIdentifier);

            // Set title            
            cell.Tag = indexPath.Row;
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            cell.TextLabel.Text = _items[indexPath.Row].Title;
            cell.DetailTextLabel.Text = _items[indexPath.Row].Subtitle;
            cell.ImageView.AutoresizingMask = UIViewAutoresizing.None;
            cell.ImageView.ClipsToBounds = true;

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
        public Action<string, string> OnRequestAlbumArt { get; set; }

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
                    var audioFile = _items[0].AudioFile;
                    lblArtistName.Text = audioFile.ArtistName;
                    lblAlbumTitle.Text = audioFile.AlbumTitle;
                    lblSubtitle1.Text = _items.Count().ToString() + " songs";

                    // Note: cannot call UIScreen.MainScreen in a background thread!
                    int height = (int)(viewAlbumCover.Bounds.Height * UIScreen.MainScreen.Scale);
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

                    imageViewAlbumCover.BackgroundColor = UIColor.Black;
                    viewAlbumCover.BackgroundColor = GlobalTheme.MainDarkColor;
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

