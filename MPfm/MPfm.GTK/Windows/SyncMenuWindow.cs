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
            Gtk.CellRendererPixbuf cellPixbuf = new Gtk.CellRendererPixbuf();
            Gtk.CellRendererToggle cellToggle = new Gtk.CellRendererToggle();
            cellToggle.Activatable = true;
            cellToggle.Toggled += (o, args) => {
                var cellRendererToggle = (Gtk.CellRendererToggle)o;
                //cellRendererToggle.Active = !cellRendererToggle.Active;
                Console.WriteLine("******* Toggle {0}", cellRendererToggle.Data["itemType"]);
            };
            colTitle.PackStart(cellPixbuf, false);
            colTitle.PackStart(cellTitle, true);
            colTitle.PackStart(cellToggle, false);
            colTitle.SetCellDataFunc(cellPixbuf, new Gtk.TreeCellDataFunc(RenderCell));
            colTitle.SetCellDataFunc(cellTitle, new Gtk.TreeCellDataFunc(RenderCell));
            colTitle.SetCellDataFunc(cellToggle, new Gtk.TreeCellDataFunc(RenderCell));
            treeView.AppendColumn(colTitle);
        }

        private void RenderCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            //Console.WriteLine("SyncMenuWindow - RenderCell");
            SyncMenuItemEntity entity = (SyncMenuItemEntity)model.GetValue(iter, 0);

            if (cell is Gtk.CellRendererText)
            {
                string title = string.Empty;
                switch (entity.ItemType)
                {
                    case SyncMenuItemEntityType.Artist:
                        title = entity.ArtistName;
                        break;
                    case SyncMenuItemEntityType.Album:
                        title = entity.AlbumTitle;
                        break;
                    case SyncMenuItemEntityType.Song:
                        if(entity.Song != null)
                            title = entity.Song.Title;
                        break;
                }       
                if(title == "dummy") title = string.Empty;
                (cell as Gtk.CellRendererText).Text = title;
            }
            else if (cell is Gtk.CellRendererPixbuf)
            {
                var cellPixbuf = (Gtk.CellRendererPixbuf)cell;
                var pixbuf = new Gdk.Pixbuf("android.png");
                cellPixbuf.Pixbuf = pixbuf;
            }
            else if (cell is Gtk.CellRendererToggle)
            {
                var cellToggle = (Gtk.CellRendererToggle)cell;
                cellToggle.Data.Clear();
                cellToggle.Data.Add("itemType", entity.ItemType.ToString());
            }
        }

        protected void OnTreeViewRowExpanded(object o, RowExpandedArgs args)
        {
            Console.WriteLine(">>>>> SyncMenuWindow - OnTreeViewRowExpanded");
            Gtk.TreeIter iter;
            SyncMenuItemEntity entity = (SyncMenuItemEntity)_storeItems.GetValue(args.Iter, 0);                     

            // Check for dummy node     
            _storeItems.IterChildren(out iter, args.Iter);
            SyncMenuItemEntity entityChild = (SyncMenuItemEntity)_storeItems.GetValue(iter, 0);          
            if (entityChild.ArtistName == "dummy" && entityChild.AlbumTitle == "dummy")
            {
                Console.WriteLine("SyncMenuWindow - OnTreeViewRowExpanded -- Expanding node...");
                OnExpandItem(entity);
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
            Console.WriteLine("SyncMenuWindow - RefreshLoading - isLoading: {0} progressPercentage: {1}", isLoading, progressPercentage);
            Gtk.Application.Invoke(delegate{
                progressBar.Visible = isLoading;
                lblLoading.Visible = isLoading;

                if(progressPercentage < 100)
                    lblLoading.Text = String.Format("Loading index ({0}%)..." + DateTime.Now.ToLongTimeString(), progressPercentage);
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
                {
                    Gtk.TreeIter iter = _storeItems.AppendValues(item);

                    // Add dummy cell
                    _storeItems.AppendValues(iter, new SyncMenuItemEntity(){
                        ItemType = SyncMenuItemEntityType.Album,
                        ArtistName = "dummy",
                        AlbumTitle = "dummy"
                    });
                }
                            
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
            Gtk.Application.Invoke(delegate{
                Console.WriteLine("SyncMenuWindow - InsertItems - index {0}", index);
                TreeIter treeIter;
                Gtk.TreePath treePath = new TreePath((index-1).ToString());
                _storeItems.GetIter(out treeIter, treePath);
                object stuff = _storeItems.GetValue(treeIter, 0);
                //_storeItems.IterNthChild(out treeIter, index-1);
                TreeIter treeIterDummy;
                _storeItems.IterNthChild(out treeIterDummy, treeIter, 0);
                object stuff2 = _storeItems.GetValue(treeIterDummy, 0);

                // Add new children
                foreach(var item in items)
                {
                    TreeIter treeIterChild = _storeItems.AppendValues(treeIter, item);
                    if(item.ItemType == SyncMenuItemEntityType.Album)
                    {
                        // Add dummy cell
                        _storeItems.AppendValues(treeIterChild, new SyncMenuItemEntity(){
                            ItemType = SyncMenuItemEntityType.Song,
                            ArtistName = "dummy",
                            AlbumTitle = "dummy"
                        });
                    }
                }

                // Remove dummy node
                _storeItems.Remove(ref treeIterDummy);
            });
        }

        public void RemoveItems(int index, int count)
        {
        }

        #endregion
    }
}
