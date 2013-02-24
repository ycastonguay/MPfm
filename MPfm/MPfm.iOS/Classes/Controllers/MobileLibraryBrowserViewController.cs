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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Delegates;
using MPfm.iOS.Classes.Controls;
using MPfm.MVP.Views;
using MPfm.MVP.Models;
using System.Linq;
using MPfm.Sound.AudioFiles;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;

namespace MPfm.iOS.Classes.Controllers
{
    public partial class MobileLibraryBrowserViewController : BaseViewController, IMobileLibraryBrowserView
    {
        private List<LibraryBrowserEntity> _items;
        private string _cellIdentifier = "MobileLibraryBrowserCell";
        private string _navigationBarTitle = string.Empty;
        private MobileLibraryBrowserType _browserType;

        public MobileLibraryBrowserViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "MobileLibraryBrowserViewController_iPhone" : "MobileLibraryBrowserViewController_iPad", null)
        {
        }
        
        public override void ViewDidLoad()
        {
            _items = new List<LibraryBrowserEntity>();
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            lblArtistName.Font = UIFont.FromName("OstrichSans-Black", 20);
            lblAlbumTitle.Font = UIFont.FromName("OstrichSans-Black", 16);
            lblSubtitle1.Font = UIFont.FromName("OstrichSans-Black", 12);
            lblSubtitle2.Font = UIFont.FromName("OstrichSans-Black", 12);


            //lblArtistName.SizeToFit();
            //lblAlbumTitle.SizeToFit();
//            lblArtistName.Font = UIFont.FromName("LeagueGothic-Italic", 26);
//            lblAlbumTitle.Font = UIFont.FromName("LeagueGothic-Italic", 22);
//            lblSubtitle1.Font = UIFont.FromName("LeagueGothic-Regular", 16);
//            lblSubtitle2.Font = UIFont.FromName("LeagueGothic-Regular", 16);

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetSubtitle(_navigationBarTitle);
        }
        
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            tableView.DeselectRow(tableView.IndexPathForSelectedRow, false);
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
            UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
            
            // Set cell style
            var cellStyle = UITableViewCellStyle.Subtitle;
            if (_browserType == MobileLibraryBrowserType.Albums)
                cellStyle = UITableViewCellStyle.Default;
            
            // Create cell if cell could not be recycled
            if (cell == null)
                cell = new UITableViewCell(cellStyle, _cellIdentifier);
            
            // Set title
            cell.TextLabel.Text = _items[indexPath.Row].Title;
            //cell.DetailTextLabel.Text = _items[indexPath.Row].

            if (_browserType == MobileLibraryBrowserType.Albums)
                cell.ImageView.Image = UIImage.FromBundle("Images/icon114");
            
            // Set font
            //cell.TextLabel.Font = UIFont.FromName("Junction", 20);
            cell.TextLabel.Font = UIFont.FromName("OstrichSans-Medium", 20);
            //cell.TextLabel.Font = UIFont.FromName("LeagueGothic-Regular", 26);
            //cell.DetailTextLabel.Font = UIFont.FromName("LeagueGothic-Regular", 18);
            
            // Set chevron
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            
            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            OnItemClick(indexPath.Row);
        }

        #region IMobileLibraryBrowserView implementation
        
        public Action<int> OnItemClick { get; set; }

        public void RefreshLibraryBrowser(IEnumerable<LibraryBrowserEntity> entities, MobileLibraryBrowserType browserType, string navigationBarTitle)
        {
            InvokeOnMainThread(() => {
                _items = entities.ToList();
                _browserType = browserType;
                _navigationBarTitle = navigationBarTitle;
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

                    //CGSize s = [yourString sizeWithFont:[UIFont systemFontOfSize:12] constrainedToSize:CGSizeMake(width, MAXFLOAT) lineBreakMode:UILineBreakModeWordWrap];
                    NSString strArtistName = new NSString(audioFile.ArtistName);
                    SizeF sizeArtistName = strArtistName.StringSize(lblArtistName.Font, new SizeF(lblArtistName.Frame.Width, lblArtistName.Frame.Height), UILineBreakMode.WordWrap);
                    lblArtistName.Frame = new RectangleF(lblArtistName.Frame.X, lblArtistName.Frame.Y, sizeArtistName.Width, sizeArtistName.Height);

                    NSString strAlbumTitle = new NSString(audioFile.AlbumTitle);
                    SizeF sizeAlbumTitle = strAlbumTitle.StringSize(lblAlbumTitle.Font, new SizeF(lblAlbumTitle.Frame.Width, lblAlbumTitle.Frame.Height), UILineBreakMode.WordWrap);
                    lblAlbumTitle.Frame = new RectangleF(lblAlbumTitle.Frame.X, lblAlbumTitle.Frame.Y, sizeAlbumTitle.Width, sizeAlbumTitle.Height);

                    // TODO: Add a memory cache and stop reloading the image from disk every time
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                    NSData imageData = NSData.FromArray(bytesImage);
                    UIImage image = UIImage.LoadFromData(imageData);
                    imageViewAlbumCover.Image = image;
                    imageViewAlbumCover.BackgroundColor = UIColor.Black;

                    CAGradientLayer gradient = new CAGradientLayer();
                    gradient.Frame = viewAlbumCover.Bounds;
                    gradient.Colors = new MonoTouch.CoreGraphics.CGColor[2] { new CGColor(0.1f, 0.1f, 0.1f, 1), new CGColor(0.4f, 0.4f, 0.4f, 1) }; //[NSArray arrayWithObjects:(id)[[UIColor blackColor] CGColor], (id)[[UIColor whiteColor] CGColor], nil];
                    viewAlbumCover.Layer.InsertSublayer(gradient, 0);
                }
            });
        }

        #endregion

    }
}

