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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using Sessions.Player.Objects;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Helpers;
using Sessions.MVP.Bootstrap;
using Sessions.GenericControls.Basics;
using org.sessionsapp.player;

namespace Sessions.iOS
{
    public partial class EqualizerPresetsViewController : BaseViewController, IEqualizerPresetsView
    {
        private bool _isViewVisible;
        private MPVolumeView _volumeView;
        private UIBarButtonItem _btnDone;
        private UIBarButtonItem _btnAdd;
        private string _cellIdentifier = "EqualizerPresetCell";
        private List<SSPEQPreset> _presets = new List<SSPEQPreset>();
        private Guid _selectedPresetId;
        private int _editingRowPosition = -1;
        private int _editingRowSection = -1;

        public EqualizerPresetsViewController()
            : base (UserInterfaceIdiomIsPhone ? "EqualizerPresetsViewController_iPhone" : "EqualizerPresetsViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            tableView.WeakDataSource = this;
            tableView.WeakDelegate = this;

            var longPress = new UILongPressGestureRecognizer(HandleLongPress);
            longPress.MinimumPressDuration = 0.7f;
            longPress.WeakDelegate = this;
            tableView.AddGestureRecognizer(longPress);

            viewOptions.BackgroundColor = GlobalTheme.BackgroundColor;
            switchBypass.OnTintColor = GlobalTheme.SecondaryColor;
            switchBypass.On = false;
            switchBypass.ValueChanged += HandleSwitchBypassValueChanged;

            //sliderMasterVolume.SetThumbImage(UIImage.FromBundle("Images/Sliders/thumb"), UIControlState.Normal);
            //sliderMasterVolume.SetMinTrackImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            //sliderMasterVolume.SetMaxTrackImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            //sliderMasterVolume.ValueChanged += HandleSliderMasterVolumeValueChanged;

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

            var btnAdd = new SessionsFlatButton();
            btnAdd.LabelAlignment = UIControlContentHorizontalAlignment.Right;
            btnAdd.Label.Text = "Add";
            btnAdd.Label.TextAlignment = UITextAlignment.Right;
            btnAdd.Label.Frame = new RectangleF(0, 0, 44, 44);
            btnAdd.ImageChevron.Image = UIImage.FromBundle("Images/Tables/plus_blue");
            btnAdd.ImageChevron.Frame = new RectangleF(70 - 22, 0, 22, 44);
            btnAdd.Frame = new RectangleF(0, 0, 70, 44);
            btnAdd.OnButtonClick += HandleButtonAddTouchUpInside;
            var btnAddView = new UIView(new RectangleF(UIScreen.MainScreen.Bounds.Width - 70, 0, 70, 44));
            var rect2 = new RectangleF(btnAddView.Bounds.X - 16, btnAddView.Bounds.Y, btnAddView.Bounds.Width, btnAddView.Bounds.Height);
            btnAddView.Bounds = rect2;
            btnAddView.AddSubview(btnAdd);
            _btnAdd = new UIBarButtonItem(btnAddView);

            NavigationItem.SetLeftBarButtonItem(_btnDone, true);
            NavigationItem.SetRightBarButtonItem(_btnAdd, true);

            var navCtrl = (SessionsNavigationController)NavigationController;
            navCtrl.SetBackButtonVisible(false);

            // Create MPVolumeView (only visible on physical iOS device)
            RectangleF rectVolume;
            if (UserInterfaceIdiomIsPhone)
                rectVolume = new RectangleF(74, 25, 236, 46);
            else
                rectVolume = new RectangleF(74, 25, 236, 46);
            _volumeView = new MPVolumeView(rectVolume);
            _volumeView.SetMinimumVolumeSliderImage(UIImage.FromBundle("Images/Sliders/slider2").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            _volumeView.SetMaximumVolumeSliderImage(UIImage.FromBundle("Images/Sliders/slider").CreateResizableImage(new UIEdgeInsets(0, 8, 0, 8), UIImageResizingMode.Tile), UIControlState.Normal);
            _volumeView.SetVolumeThumbImage(UIImage.FromBundle("Images/Sliders/thumbbig"), UIControlState.Normal);
            this.View.AddSubview(_volumeView);

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindEqualizerPresetsView(null, this);
        }

//        private void HandleSliderMasterVolumeValueChanged(object sender, EventArgs e)
//        {
//            lblMasterVolumeValue.Text = sliderMasterVolume.Value.ToString("0") + " %";
//            OnSetVolume(sliderMasterVolume.Value / 100);
//        }

        private void HandleButtonAddTouchUpInside()
        {
            foreach (var visibleCell in tableView.VisibleCells)
                visibleCell.Accessory = UITableViewCellAccessory.None;

            OnAddPreset();
        }

        private void HandleSwitchBypassValueChanged(object sender, EventArgs e)
        {
            OnBypassEqualizer();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            var navCtrl = (SessionsNavigationController)this.NavigationController;
            navCtrl.SetTitle("Equalizer Presets");
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			_isViewVisible = true;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			_isViewVisible = false;
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			var screenSize = UIKitHelper.GetDeviceSize();
			if (UserInterfaceIdiomIsPhone)
				_volumeView.Frame = new RectangleF(74, 25, 236, 46);
			else
				_volumeView.Frame = new RectangleF(100, 58, screenSize.Width - 112, 46);
		}

        private void HandleLongPress(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
                return;

//            PointF pt = gestureRecognizer.LocationInView(tableView);
//            NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(pt);
//            if (indexPath != null)
//            {
//                //SetCheckmarkCell(indexPath);
//                //OnEditPreset(_presets[indexPath.Row].EQPresetId);
//
//                if (_editingRowPosition == indexPath.Row && _editingRowSection == indexPath.Section)
//                    ResetEditingTableCellRow();
//                else
//                    SetEditingTableCellRow(indexPath.Section, indexPath.Row);
//            }
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
                var oldCell = (SessionsEqualizerTableViewCell)tableView.CellAt(NSIndexPath.FromRowSection(oldRow, oldSection));
                if (oldCell != null)
                {
                    //oldCell.ContainerBackgroundView.BackgroundColor = UIColor.Blue; //GlobalTheme.SecondaryColor;
                    oldCell.IsDarkBackground = false;
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
//                      oldCell.PlayButton.Frame = new RectangleF(4, 52, 100, 64);
//                      oldCell.AddButton.Frame = new RectangleF(108, 52, 100, 64);
//                      oldCell.DeleteButton.Frame = new RectangleF(212, 52, 100, 64);
                        oldCell.TitleTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                        oldCell.SubtitleTextLabel.Transform = CGAffineTransform.MakeScale(1, 1);
                        oldCell.EditButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                        oldCell.DuplicateButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                        oldCell.DeleteButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);

                        oldCell.GraphView.ColorMainLine = BasicColors.Black;
                        oldCell.GraphView.SetNeedsDisplay();

                        oldCell.EditButton.Alpha = 0;
                        oldCell.DuplicateButton.Alpha = 0;
                        oldCell.DeleteButton.Alpha = 0;

                        oldCell.ContainerBackgroundView.BackgroundColor = UIColor.White;
                        oldCell.TitleTextLabel.TextColor = UIColor.Black;
                        oldCell.SubtitleTextLabel.TextColor = UIColor.Gray;
                        oldCell.EditButton.UpdateLayout();
                        oldCell.DuplicateButton.UpdateLayout();
                        oldCell.DeleteButton.UpdateLayout();
                    }, null);
                }
            }

            if (row >= 0)
            {
                var cell = (SessionsEqualizerTableViewCell)tableView.CellAt(NSIndexPath.FromRowSection(row, section));
                if (cell != null)
                {
                    cell.EditButton.Alpha = 0;
                    cell.DuplicateButton.Alpha = 0;
                    cell.DeleteButton.Alpha = 0;
//                  cell.PlayButton.Frame = new RectangleF(4, 25, 100, 44);
//                  cell.AddButton.Frame = new RectangleF(108, 25, 100, 44);
//                  cell.DeleteButton.Frame = new RectangleF(212, 25, 100, 44);

                    cell.ContainerBackgroundView.BackgroundColor = GlobalTheme.SecondaryColor;
                    cell.IsDarkBackground = true;
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseIn, () => {
                        cell.EditButton.Alpha = 1;
                        cell.DuplicateButton.Alpha = 1;
                        cell.DeleteButton.Alpha = 1;

                        cell.TitleTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                        cell.SubtitleTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                        cell.EditButton.Transform = CGAffineTransform.MakeScale(1, 1);
                        cell.DuplicateButton.Transform = CGAffineTransform.MakeScale(1, 1);
                        cell.DeleteButton.Transform = CGAffineTransform.MakeScale(1, 1);

                        cell.GraphView.ColorMainLine = BasicColors.White;
                        cell.GraphView.SetNeedsDisplay();

                        cell.ContainerBackgroundView.BackgroundColor = GlobalTheme.BackgroundColor;
                        cell.TitleTextLabel.TextColor = UIColor.White;
                        cell.SubtitleTextLabel.TextColor = UIColor.White;

//                      cell.PlayButton.Frame = new RectangleF(4, 52, 100, 64);
//                      cell.AddButton.Frame = new RectangleF(108, 52, 100, 64);
//                      cell.DeleteButton.Frame = new RectangleF(212, 52, 100, 64);
                        cell.EditButton.UpdateLayout();
                        cell.DuplicateButton.UpdateLayout();
                        cell.DeleteButton.UpdateLayout();

                    }, null);
                }
            }

            // Execute animation for new row height (as simple as that!)
            tableView.BeginUpdates();
            tableView.EndUpdates(); 
        }

        [Export ("tableView:commitEditingStyle:forRowAtIndexPath:")]
        public void CommitEditingStyleForRowAtIndexPath(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            if(editingStyle == UITableViewCellEditingStyle.Delete)
            {
                OnDeletePreset(_presets[indexPath.Row].EQPresetId);
            }
        }

        [Export ("tableView:canEditRowAtIndexPath:")]
        public bool CanEditRowAtIndexPath(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        [Export ("tableView:numberOfRowsInSection:")]
        public int RowsInSection(UITableView tableView, int section)
        {
            return _presets.Count;
        }
        
        [Export ("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // Request a recycled cell to save memory
            var cell = (SessionsEqualizerTableViewCell)tableView.DequeueReusableCell(_cellIdentifier);
            if (cell == null)
            {
                var cellStyle = UITableViewCellStyle.Subtitle;
                cell = new SessionsEqualizerTableViewCell(cellStyle, _cellIdentifier);

                // Register events only once!
                cell.EditButton.TouchUpInside += HandleTableViewEditTouchUpInside;
                cell.DuplicateButton.TouchUpInside += HandleTableViewDuplicateTouchUpInside;
                cell.DeleteButton.TouchUpInside += HandleTableViewDeleteTouchUpInside;
            }

            cell.Tag = indexPath.Row;
            cell.GraphView.Preset = _presets[indexPath.Row];
            cell.TitleTextLabel.Text = _presets[indexPath.Row].Name;
            cell.Accessory = UITableViewCellAccessory.None;
            //cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;

            cell.CheckmarkImageView.Hidden = _presets[indexPath.Row].EQPresetId != _selectedPresetId;
            //if (_presets[indexPath.Row].EQPresetId == _selectedPresetId)
//                cell.Accessory = UITableViewCellAccessory.Checkmark;
            
            bool isEditing = _editingRowPosition == indexPath.Row && _editingRowSection == indexPath.Section;
            cell.IsDarkBackground = isEditing;
            //cell.ContainerBackgroundView.BackgroundColor = isEditing ? UIColor.Green : UIColor.Purple; //GlobalTheme.BackgroundColor;// : UIColor.Green;
            cell.ContainerBackgroundView.BackgroundColor = isEditing ? GlobalTheme.BackgroundColor : UIColor.White;
            cell.EditButton.Alpha = isEditing ? 1 : 0;
            cell.DuplicateButton.Alpha = isEditing ? 1 : 0;
            cell.DeleteButton.Alpha = isEditing ? 1 : 0;
            cell.TitleTextLabel.TextColor = isEditing ? UIColor.White : UIColor.Black;
            cell.SubtitleTextLabel.TextColor = isEditing ? UIColor.White : UIColor.Gray;
            //cell.IndexTextLabel.TextColor = isEditing ? UIColor.White : UIColor.FromRGB(0.5f, 0.5f, 0.5f);

            if (isEditing)
            {
                cell.TitleTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                cell.SubtitleTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
//                cell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(0.86f, 0.86f);
                cell.EditButton.Transform = CGAffineTransform.MakeScale(1, 1);
                cell.DuplicateButton.Transform = CGAffineTransform.MakeScale(1, 1);
                cell.DeleteButton.Transform = CGAffineTransform.MakeScale(1, 1);
            }
            else
            {
                cell.TitleTextLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
                cell.SubtitleTextLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
//                cell.IndexTextLabel.Transform = CGAffineTransform.MakeScale(1f, 1f);
                cell.EditButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                cell.DuplicateButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                cell.DeleteButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
            }

//            UIView viewBackgroundSelected = new UIView();
//            viewBackgroundSelected.BackgroundColor = GlobalTheme.SecondaryColor;
//            cell.SelectedBackgroundView = viewBackgroundSelected;
            
            return cell;
        }
        
        [Export ("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            SetCheckmarkCell(indexPath);
            OnLoadPreset(_presets[indexPath.Row].EQPresetId);
            tableView.DeselectRow(indexPath, true);

            if (_editingRowPosition == indexPath.Row && _editingRowSection == indexPath.Section)
                ResetEditingTableCellRow();
            else
                SetEditingTableCellRow(indexPath.Section, indexPath.Row);

        }

        [Export ("tableView:heightForRowAtIndexPath:")]
        public float HeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            bool isEditing = _editingRowPosition == indexPath.Row && _editingRowSection == indexPath.Section;
            return isEditing ? SessionsEqualizerTableViewCell.ExpandedCellHeight : SessionsEqualizerTableViewCell.StandardCellHeight;
        }

        private void SetCheckmarkCell(NSIndexPath indexPath)
        {
            // _selectedPresetId = _presets[indexPath.Row].EQPresetId;
            var presetId = _presets[indexPath.Row].EQPresetId;

            // Reset checkmarks
            foreach (var visibleCell in tableView.VisibleCells)
            {
                //visibleCell.Accessory = UITableViewCellAccessory.None;
                var cellToRemove = visibleCell as SessionsEqualizerTableViewCell;
                cellToRemove.IsPresetSelected = false;
            }

            // If selecting a cell that's already checked, then uncheck this cell
            if (presetId == _selectedPresetId)
            {
                _selectedPresetId = Guid.Empty;
                return;
            }

            // Set new checkmark (force reload row or the checkmark isn't always visible)
            _selectedPresetId = presetId;
            var cell = tableView.CellAt(indexPath);
            if (cell != null)
            {
                var cellToAdd = cell as SessionsEqualizerTableViewCell;
                cellToAdd.IsPresetSelected = true;
            }
                //tableView.ReloadRows(new NSIndexPath[1] { indexPath }, UITableViewRowAnimation.None);
        }

        private void HandleTableViewEditTouchUpInside(object sender, EventArgs e)
        {
            SetCheckmarkCell(NSIndexPath.FromRowSection(_editingRowPosition, _editingRowSection));
            OnEditPreset(_presets[_editingRowPosition].EQPresetId);
            ResetEditingTableCellRow();
        }

        private void HandleTableViewDeleteTouchUpInside(object sender, EventArgs e)
        {
            var item = _presets[_editingRowPosition];
            if (item == null)
                return;

            var alertView = new UIAlertView("Delete confirmation", string.Format("Are you sure you wish to delete {0}?", item.Name), null, "OK", new string[1] { "Cancel" });
            alertView.Clicked += (object sender2, UIButtonEventArgs e2) => {
                switch(e2.ButtonIndex)
                {
                    case 0:
                        OnDeletePreset(item.EQPresetId);
                        break;
                }
                ResetEditingTableCellRow();
            };
            alertView.Show();           
        }

        private void HandleTableViewDuplicateTouchUpInside(object sender, EventArgs e)
        {
            //SetCheckmarkCell(_editingRowPosition);
            //OnEditPreset(_presets[_editingRowPosition].EQPresetId);
            ResetEditingTableCellRow();
        }

        #region IEqualizerPresetsView implementation

        public Action OnBypassEqualizer { get; set; }
        public Action<float> OnSetVolume { get; set; }
        public Action OnAddPreset { get; set; }
        public Action<Guid> OnLoadPreset { get; set; }
        public Action<Guid> OnEditPreset { get; set; }
        public Action<Guid> OnDeletePreset { get; set; }
		public Action<Guid> OnDuplicatePreset { get; set; }
		public Action<Guid, string> OnExportPreset { get; set; }

        public void EqualizerPresetsError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshPresets(IEnumerable<SSPEQPreset> presets, Guid selectedPresetId, bool isEQBypassed)
        {
            InvokeOnMainThread(() => {
                _selectedPresetId = selectedPresetId;
                _presets = presets.ToList();
                switchBypass.On = isEQBypassed;

                tableView.ReloadData();
                int index = _presets.FindIndex(x => x.EQPresetId == _selectedPresetId);
                if(index >= 0) {
                    SetCheckmarkCell(NSIndexPath.FromRowSection(index, 0));
                }
            });
        }

        public void RefreshOutputMeter(float[] dataLeft, float[] dataRight)
        {
            InvokeOnMainThread(() => {
				if(_isViewVisible)
				{
		            outputMeter.AddWaveDataBlock(dataLeft, dataRight);
		            outputMeter.SetNeedsDisplay();
				}
            });
        }

        public void RefreshVolume(float volume)
        {
            InvokeOnMainThread(() => {
                //sliderMasterVolume.Value = volume * 100;
                //lblMasterVolumeValue.Text = (volume * 100).ToString("0") + " %";
            });
        }

        #endregion
    }
}
