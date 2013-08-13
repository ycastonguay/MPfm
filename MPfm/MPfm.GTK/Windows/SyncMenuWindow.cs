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
using MPfm.MVP.Views;
using System.Collections.Generic;
using MPfm.MVP.Models;
using MPfm.GTK.Windows;
using MPfm.Library.Objects;
using System.Reflection;
using System.Text;
using Gtk;

namespace MPfm.GTK
{
    public partial class SyncMenuWindow : BaseWindow, ISyncMenuView
    {
        Gtk.TreeStore _storeItems;

        public SyncMenuWindow(Action<IBaseView> onViewReady) : 
            base(Gtk.WindowType.Toplevel, onViewReady)
        {
            this.Build();

            Title = "Sync Library With";
            InitializeTreeView();

            onViewReady(this);
            btnSelectAll.GrabFocus(); // the list view changes color when focused by default. it annoys me!
            this.Center();
            this.Show();
        }

        private void InitializeTreeView()
        {
            _storeItems = new Gtk.TreeStore(typeof(SyncMenuItemEntity));
            treeView.HeadersVisible = false;

            // Create title column
            Gtk.TreeViewColumn colTitle = new Gtk.TreeViewColumn();
            Gtk.CellRendererText cellTitle = new Gtk.CellRendererText();    
            colTitle.Data.Add("Property", "Title");
            colTitle.PackStart(cellTitle, true);
            colTitle.SetCellDataFunc(cellTitle, new Gtk.TreeCellDataFunc(RenderCell));
            treeView.AppendColumn(colTitle);
        }

        private void RenderCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            Console.WriteLine("SyncMenuWindow - RenderCell");
            SyncMenuItemEntity entity = (SyncMenuItemEntity)model.GetValue(iter, 0);

            switch (entity.ItemType)
            {
                case SyncMenuItemEntityType.Artist:
                    (cell as Gtk.CellRendererText).Text = entity.ArtistName;
                    break;
                case SyncMenuItemEntityType.Album:
                    (cell as Gtk.CellRendererText).Text = entity.AlbumTitle;
                    break;
                case SyncMenuItemEntityType.Song:
                    (cell as Gtk.CellRendererText).Text = entity.Song.Title;
                    break;
            }
        }

        #region ISyncMenuView implementation

        public Action<SyncMenuItemEntity> OnExpandItem { get; set; }
        public Action<SyncMenuItemEntity> OnSelectItem { get; set; }
        public System.Action OnSync { get; set; }
        public System.Action OnSelectButtonClick { get; set; }

        public void SyncMenuError(Exception ex)
        {
            Gtk.Application.Invoke(delegate{            
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the SyncMenu component:");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);                                                               
                MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, sb.ToString());
                md.Run();
                md.Destroy();
            });
        }

        public void SyncEmptyError(Exception ex)
        {
            Gtk.Application.Invoke(delegate{            
                MessageDialog md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, ex.Message);
                md.Run();
                md.Destroy();
            });
        }

        public void RefreshDevice(SyncDevice device)
        {
            Gtk.Application.Invoke(delegate{
                Title = "Sync library with " + device.Name;
                lblTitle.Text = "Sync library with " + device.Name;
            });
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            Gtk.Application.Invoke(delegate{
                progressBar.Visible = isLoading;
                lblLoading.Visible = isLoading;

                if(progressPercentage < 100)
                    lblLoading.Text = String.Format("Loading index ({0}%)...", progressPercentage);
                else
                    lblLoading.Text = "Processing index...";            
            });
        }

        public void RefreshSelectButton(string text)
        {
            Gtk.Application.Invoke(delegate{
                btnSelectAll.Label = text;
            });
        }

        public void RefreshItems(List<SyncMenuItemEntity> items)
        {
            Gtk.Application.Invoke(delegate{
                Console.WriteLine("SyncMenuWindow - RefreshItems");
                _storeItems.Clear();
                
                foreach(SyncMenuItemEntity item in items)
                    _storeItems.AppendValues(item);
                            
                treeView.Model = _storeItems;
            });
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
            Gtk.Application.Invoke(delegate{
                lblTotal.Text = title;
                lblFreeSpace.Text = subtitle;
            });
        }

        public void InsertItems(int index, List<SyncMenuItemEntity> items)
        {
        }

        public void RemoveItems(int index, int count)
        {
        }

        #endregion

    }
}
