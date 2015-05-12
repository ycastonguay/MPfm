// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.MVP.Views;
using Sessions.iOS.Classes.Controls;
using Sessions.Sound.AudioFiles;
using System.Collections.Generic;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.Player;
using Sessions.Sound.Player;
using Sessions.iOS.Classes.Controls.Cells;

namespace Sessions.iOS
{
    public partial class PlaylistViewController : BaseViewController, IPlaylistView
    {
        string _cellIdentifier = "PlaylistItemCell";
        bool _isEditingTableView = false;
        Guid _currentlyPlayingSongId;
        Playlist _playlist;
        UIBarButtonItem _btnDone;
        UIBarButtonItem _btnEdit;
        SessionsFlatButton _btnFlatEdit;
        UIBarButtonItem _btnNew;
        UIBarButtonItem _btnShuffle;

        public PlaylistViewController()
            : base (UserInterfaceIdiomIsPhone ? "PlaylistViewController_iPhone" : "PlaylistViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            var btnDone = new SessionsFlatButton();
            btnDone.Label.Text = "Done";
            btnDone.Frame = new RectangleF(0, 0, 70, 44);
            btnDone.OnButtonClick += () => {
                NavigationController.DismissViewController(true, null);
            };
            var btnDoneView = new UIView(new RectangleF(0, 0, 70, 44));
            var rect = new RectangleF(btnDoneView.Bounds.X + 16, btnDoneView.Bounds.Y, btnDoneView.Bounds.Width, btnDoneView.Bounds.Height);
            btnDoneView.Bounds = rect;
            btnDoneView.AddSubview(btnDone);
            _btnDone = new UIBarButtonItem(btnDoneView);

            _btnFlatEdit = new SessionsFlatButton();
            _btnFlatEdit.LabelAlignment = UIControlContentHorizontalAlignment.Right;
            _btnFlatEdit.Label.Text = "Edit";
            _btnFlatEdit.Label.TextAlignment = UITextAlignment.Right;
            _btnFlatEdit.Label.Frame = new RectangleF(0, 0, 44, 44);
            _btnFlatEdit.ImageChevron.Hidden = true;
            _btnFlatEdit.Frame = new RectangleF(0, 0, 60, 44);
            _btnFlatEdit.OnButtonClick += HandleEditTouchUpInside;
            var btnEditView = new UIView(new RectangleF(UIScreen.MainScreen.Bounds.Width - 60, 0, 60, 44));
            var rect2 = new RectangleF(btnEditView.Bounds.X - 16, btnEditView.Bounds.Y, btnEditView.Bounds.Width, btnEditView.Bounds.Height);
            btnEditView.Bounds = rect2;
            btnEditView.AddSubview(_btnFlatEdit);
            _btnEdit = new UIBarButtonItem(btnEditView);           

            NavigationItem.SetLeftBarButtonItem(_btnDone, true);
            NavigationItem.SetRightBarButtonItem(_btnEdit, true);

            var btnNew = new SessionsButton();
            btnNew.SetTitle("New", UIControlState.Normal);
            btnNew.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            btnNew.Frame = new RectangleF(0, 12, 50, 30);
            btnNew.TouchUpInside += HandleNewTouchUpInside;
            _btnNew = new UIBarButtonItem(btnNew);

            var btnShuffle = new SessionsButton();
            btnShuffle.SetTitle("Shuffle", UIControlState.Normal);
            btnShuffle.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            btnShuffle.Frame = new RectangleF(0, 12, 70, 30);
            btnShuffle.TouchUpInside += HandleShuffleTouchUpInside;
            _btnShuffle = new UIBarButtonItem(btnShuffle);

            toolbar.Items = new UIBarButtonItem[2]{ _btnNew, _btnShuffle };

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindPlaylistView(null, this);
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableview, int section)
        {
            return _playlist.Count;
        }

        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (SessionsLibraryTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
                cell = new SessionsLibraryTableViewCell(UITableViewCellStyle.Subtitle, _cellIdentifier);

            var item = _playlist.GetItemAt(indexPath.Row);
            var audioFile = item.AudioFile;
            cell.Tag = indexPath.Row;
            cell.Accessory = UITableViewCellAccessory.None;
            cell.TitleTextLabel.Font = UIFont.FromName("HelveticaNeue", 14);
            cell.TitleTextLabel.Text = audioFile.Title;
            cell.SubtitleTextLabel.Font = UIFont.FromName("HelveticaNeue-Light", 12);
            cell.SubtitleTextLabel.Text = audioFile.ArtistName;
            cell.ImageView.AutoresizingMask = UIViewAutoresizing.None;
            cell.ImageView.ClipsToBounds = true;
            cell.ImageChevron.Hidden = true;
            cell.IndexTextLabel.Text = (indexPath.Row+1).ToString();

            if (_currentlyPlayingSongId == audioFile.Id)
                cell.RightImage.Hidden = false;
            else
                cell.RightImage.Hidden = true;

            return cell;
        }

        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _playlist.GetItemAt(indexPath.Row);
            if (item != null)
            {
                OnSelectPlaylistItem(item.AudioFile.Id);
                tableView.DeselectRow(indexPath, true);
            }
        }

        [Export ("tableView:didHighlightRowAtIndexPath:")]
        public void DidHighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (SessionsLibraryTableViewCell)tableView.CellAt(indexPath);
            cell.RightImage.Image = UIImage.FromBundle("Images/Icons/icon_speaker_white");
        }

        [Export ("tableView:didUnhighlightRowAtIndexPath:")]
        public void DidUnhighlightRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (SessionsLibraryTableViewCell)tableView.CellAt(indexPath);
            cell.RightImage.Image = UIImage.FromBundle("Images/Icons/icon_speaker");
        }

        [Export ("tableView:canMoveRowAtIndexPath:")]
        public bool CanMoveRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        [Export ("tableView:moveRowAtIndexPath:toIndexPath:")]
        public void MoveRowAtIndexPath(UITableView tableView, NSIndexPath fromIndexPath, NSIndexPath toIndexPath)
        {
            Console.WriteLine("PlaylistViewController - Move playlist item from {0} to {1}", fromIndexPath.Row, toIndexPath.Row);
        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 52;
        }

        private void HandleEditTouchUpInside()
        {
            _isEditingTableView = !_isEditingTableView;
            tableView.SetEditing(_isEditingTableView, true);
            if (_isEditingTableView)
                _btnFlatEdit.Label.Text = "Done";
            else
                _btnFlatEdit.Label.Text = "Edit";
        }

        private void HandleNewTouchUpInside(object sender, EventArgs e)
        {
            UIAlertView alertView = new UIAlertView("Playlist will be empty", "Are you sure you wish to create a new playlist?", null, "OK", new string[1] {"Cancel"});
            alertView.Dismissed += (sender2, e2) => {
                if(e2.ButtonIndex == 0)
                    OnNewPlaylist();
            };
            alertView.Show();
        }

        private void HandleShuffleTouchUpInside(object sender, EventArgs e)
        {
            OnShufflePlaylist();
        }

        #region IPlaylistView implementation

        public Action<Guid, int> OnChangePlaylistItemOrder { get; set; }
        public Action<Guid> OnSelectPlaylistItem { get; set; }
        public Action<List<Guid>> OnRemovePlaylistItems { get; set; }
        public Action OnNewPlaylist { get; set; }
        public Action<string> OnLoadPlaylist { get; set; }
        public Action OnSavePlaylist { get; set; }
        public Action OnShufflePlaylist { get; set; }

        public void PlaylistError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshPlaylist(Playlist playlist)
        {
            InvokeOnMainThread(() => {
                _playlist = playlist;
                tableView.ReloadData();
            });
        }

        public void RefreshCurrentlyPlayingSong(int index, AudioFile audioFile)
        {
            Console.WriteLine("PlaylistViewController - RefreshCurrentlyPlayingSong index: {0} audioFile: {1}", index, audioFile.FilePath);

            if (audioFile != null)
                _currentlyPlayingSongId = audioFile.Id;
            else
                _currentlyPlayingSongId = Guid.Empty;

            InvokeOnMainThread(() => {
                foreach(var cell in tableView.VisibleCells)
                {
                    var item = _playlist.GetItemAt(cell.Tag);
                    if(item != null)
                    {
                        var customCell = (SessionsLibraryTableViewCell)cell;
                        if(item.AudioFile.Id == audioFile.Id)
                            customCell.RightImage.Hidden = false;
                        else
                            customCell.RightImage.Hidden = true;
                    }
                }
            });
        }

        #endregion
    }
}
