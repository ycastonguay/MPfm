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
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Classes.Services;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.Sound.AudioFiles;
using System.Threading.Tasks;
using MPfm.iOS.Helpers;
using MPfm.MVP.Models;

namespace MPfm.iOS
{
    public partial class ResumePlaybackViewController : BaseViewController, IResumePlaybackView
    {
        string _cellIdentifier = "ResumePlaybackCell";
        List<ResumePlaybackEntity> _devices = new List<ResumePlaybackEntity>();

        public ResumePlaybackViewController()
			: base (UserInterfaceIdiomIsPhone ? "ResumePlaybackViewController_iPhone" : "ResumePlaybackViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            viewLoading.BackgroundColor = GlobalTheme.BackgroundColor;
            viewTable.BackgroundColor = GlobalTheme.BackgroundColor;
            viewAppNotLinked.BackgroundColor = GlobalTheme.BackgroundColor;
            viewLoading.Alpha = 1;
            viewTable.Alpha = 0;
            viewAppNotLinked.Alpha = 0;

            activityIndicator.StartAnimating();
            View.BackgroundColor = GlobalTheme.BackgroundColor;
            viewAppNotLinked.BackgroundColor = GlobalTheme.BackgroundColor;
            btnOpenCloudPreferences.SetImage(UIImage.FromBundle("Images/Buttons/cloud"));

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindResumePlaybackView(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Resume Playback");

            OnCheckCloudLoginStatus();
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _devices.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var entity = _devices[indexPath.Row];
            MPfmResumePlaybackTableViewCell cell = (MPfmResumePlaybackTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new MPfmResumePlaybackTableViewCell(cellStyle, _cellIdentifier);
            }

            if (cell.TextLabel.Text == entity.DeviceInfo.DeviceName)
                return cell;

            cell.ImageIcon.Image = UIImage.FromBundle("/Images/Icons/android");
            cell.TextLabel.Text = entity.DeviceInfo.DeviceName;
            cell.DetailTextLabel.Text = "On-the-fly Playlist";
            cell.LabelLastUpdated.Text = string.Format("Last updated on {0}", entity.DeviceInfo.Timestamp);
            cell.LabelArtistName.Text = entity.DeviceInfo.ArtistName;
            cell.LabelAlbumTitle.Text = entity.DeviceInfo.AlbumTitle;
            cell.LabelSongTitle.Text = entity.DeviceInfo.SongTitle;

            cell.UserInteractionEnabled = entity.CanResumePlayback;
            //cell.BackgroundView.BackgroundColor = entity.CanResumePlayback ? UIColor.White : UIColor.FromRGB(0.85f, 0.85f, 0.85f);
            cell.LabelCellDisabled.Alpha = entity.CanResumePlayback ? 0 : 1;
            cell.ViewOverlay.Alpha = entity.CanResumePlayback ? 0 : 1;

            cell.ImageAlbum.Image = null;
            LoadAlbumArt(cell.ImageAlbum, entity.LocalAudioFilePath);

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            OnResumePlayback(_devices[indexPath.Row]);
        }

        [Export ("tableView:didHighlightRowAtIndexPath:")]
        public void DidHighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (MPfmResumePlaybackTableViewCell)tableView.CellAt(indexPath);
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron_white");
        }

        [Export ("tableView:didUnhighlightRowAtIndexPath:")]
        public void DidUnhighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (MPfmResumePlaybackTableViewCell)tableView.CellAt(indexPath);
            cell.ImageChevron.Image = UIImage.FromBundle("Images/Tables/chevron");
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 130;
        }

        partial void actionOpenCloudPreferences(NSObject sender)
        {
            OnOpenPreferencesView();
        }

        public async void LoadAlbumArt(UIImageView imageView, string audioFilePath)
        {
            int height = 108;

            // Load album art + resize in another thread
            var task = Task<UIImage>.Factory.StartNew(() => {
                try
                {
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFilePath);                        
                    using (NSData imageData = NSData.FromArray(bytesImage))
                    {
                        using (UIImage imageFullSize = UIImage.LoadFromData(imageData))
                        {
                            if (imageFullSize != null)
                            {
                                try
                                {
                                    UIImage imageResized = CoreGraphicsHelper.ScaleImage(imageFullSize, height);
                                    return imageResized;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error resizing image {0}: {1}", audioFilePath, ex);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to process image: {0}", ex);
                }

                return null;
            });
            //}).ContinueWith(t => {
            UIImage image = await task;
            if(image == null)
                return;

            InvokeOnMainThread(() => {
                try
                {
                    imageView.Alpha = 0;
                    imageView.Image = image;              

                    UIView.Animate(0.3, () => {
                        imageView.Alpha = 1;
                    });
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Failed to set image after processing: {0}", ex);
                }
            });
            //}, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #region IResumePlaybackView implementation

        public Action<ResumePlaybackEntity> OnResumePlayback { get; set; }
        public Action OnOpenPreferencesView { get; set; }
        public Action OnCheckCloudLoginStatus { get; set; }

        public void ResumePlaybackError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void AudioFilesNotFoundError(string title, string message)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView(title, message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshDevices(IEnumerable<ResumePlaybackEntity> entities)
        {
            Console.WriteLine("ResumePlaybackViewController - RefreshDevices - devices.Count: {0}", entities.Count());
            InvokeOnMainThread(() => {
                if(activityIndicator.IsAnimating)
                {
                    viewTable.Alpha = 0;
                    activityIndicator.StopAnimating();

                    UIView.Animate(0.2, () => {
                        viewTable.Alpha = 1;
                        viewLoading.Alpha = 0;
                    });
                }

                _devices = entities.ToList();
                tableView.ReloadData();
            });
        }

        public void RefreshAppLinkedStatus(bool isAppLinked)
        {
            Console.WriteLine("ResumePlaybackViewController - RefreshAppLinkedStatus - isAppLinked: {0}", isAppLinked);
            InvokeOnMainThread(() => {
                activityIndicator.StopAnimating();

                UIView.Animate(0.2, () => {
                    viewLoading.Alpha = 0;
                    viewTable.Alpha = isAppLinked ? 1 : 0;
                    viewAppNotLinked.Alpha = isAppLinked ? 0 : 1;
                });
            });
        }

        #endregion
    }
}
