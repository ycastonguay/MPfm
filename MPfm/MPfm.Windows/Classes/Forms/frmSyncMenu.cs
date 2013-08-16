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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Library.Objects;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;

namespace MPfm.Windows.Classes.Forms
{
    public partial class frmSyncMenu : BaseForm, ISyncMenuView
    {
        public frmSyncMenu(Action<IBaseView> onViewReady)
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            OnSelectButtonClick();
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            OnSync();
        }

        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            Console.WriteLine("BEFORE EXPAND");

            // Detect if the child node is a dummy node (indicating we have to fetch the data)            
            if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Text != "dummy")
                return;

            Console.WriteLine("BEFORE EXPAND 2");
            //e.Cancel = true;
            var item = (SyncMenuItemEntity) e.Node.Tag;
            OnExpandItem(item, e.Node);
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null)
                return;

            OnSelectItems(new List<SyncMenuItemEntity>() {
                (SyncMenuItemEntity)treeView.SelectedNode.Tag
            });
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listViewSelection.SelectedItems.Count == 0)
                return;

            List<AudioFile> items = new List<AudioFile>();
            for (int a = 0; a < listViewSelection.SelectedItems.Count; a++)
                items.Add((AudioFile) listViewSelection.SelectedItems[a].Tag);
            OnRemoveItems(items);
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            OnSelectAll();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            OnRemoveAll();
        }

        #region ISyncMenuView implementation

        public Action<SyncMenuItemEntity, object> OnExpandItem { get; set; }
        public Action<List<SyncMenuItemEntity>> OnSelectItems { get; set; }
        public Action<List<AudioFile>> OnRemoveItems { get; set; }
        public Action OnSync { get; set; }
        public Action OnSelectButtonClick { get; set; }
        public Action OnSelectAll { get; set; }
        public Action OnRemoveAll { get; set; }

        public void SyncMenuError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate {
                MessageBox.Show(string.Format("An error occured in SyncMenu: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void SyncEmptyError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshDevice(SyncDevice device)
        {
            MethodInvoker methodUIUpdate = delegate {
                lblTitle.Text = "Sync Library With " + device.Name;
                this.Text = "Sync Library With " + device.Name;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            MethodInvoker methodUIUpdate = delegate {
                progressBar.Value = progressPercentage;
                panelLoading.Visible = isLoading;
                progressBar.Visible = isLoading;
                lblLoading.Visible = isLoading;

                if (progressPercentage < 100)
                    lblLoading.Text = String.Format("Loading index ({0}%)...", progressPercentage);
                else
                    lblLoading.Text = "Processing index...";
            }; 

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshSelectButton(string text)
        {
        }

        public void RefreshItems(List<SyncMenuItemEntity> items)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                treeView.BeginUpdate();
                treeView.Nodes.Clear();
                foreach (var item in items)
                {
                    string title = string.Empty;
                    switch (item.ItemType)
                    {
                        case SyncMenuItemEntityType.Artist:
                            title = item.ArtistName;
                            break;
                        case SyncMenuItemEntityType.Album:
                            title = item.AlbumTitle;
                            break;
                        case SyncMenuItemEntityType.Song:
                            if(item.Song != null)
                                title = item.Song.Title;
                            break;
                    }
                    var treeNode = new TreeNode(title, 0, 0) {
                        Tag = item
                    };
                    treeView.Nodes.Add(treeNode);
                    treeNode.Nodes.Add(new TreeNode("dummy", 0, 0));
                }
                treeView.EndUpdate();
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
            MethodInvoker methodUIUpdate = delegate {
                lblTotal.Text = title;
                lblFreeSpace.Text = subtitle;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshSelection(List<AudioFile> audioFiles)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                listViewSelection.Items.Clear();
                listViewSelection.BeginUpdate();
                foreach (var audioFile in audioFiles)
                {
                    //var split = audioFile.FilePath.Split(new char[2] {'/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
                    //string title = split[split.Length - 1];
                    string title = string.Format("{0} / {1} / {2}. {3}", audioFile.ArtistName, audioFile.AlbumTitle, audioFile.TrackNumber, audioFile.Title);
                    listViewSelection.Items.Add(new ListViewItem(title, 0) {
                        Tag = audioFile
                    });
                }
                listViewSelection.EndUpdate();
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void InsertItems(int index, SyncMenuItemEntity parentItem, List<SyncMenuItemEntity> items, object userData)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                var node = (TreeNode)userData;
                foreach (var item in items)
                {
                    string title = string.Empty;
                    int iconId = -1;
                    switch (item.ItemType)
                    {
                        case SyncMenuItemEntityType.Artist:
                            title = item.ArtistName;
                            iconId = 0;
                            break;
                        case SyncMenuItemEntityType.Album:
                            title = item.AlbumTitle;
                            iconId = 1;
                            break;
                        case SyncMenuItemEntityType.Song:
                            if (item.Song != null)
                                title = item.Song.Title;
                            iconId = 2;
                            break;
                    }

                    // Add sub node
                    var treeNode = new TreeNode(title, iconId, iconId) {
                        Tag = item
                    };
                    node.Nodes.Add(treeNode);

                    // Add dummy node
                    if (item.ItemType != SyncMenuItemEntityType.Song)
                        treeNode.Nodes.Add(new TreeNode("dummy", -1, -1));
                }

                // Remove dummy node
                foreach (TreeNode subNode in node.Nodes)
                    if (subNode.Text == "dummy")
                        subNode.Remove();
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RemoveItems(int index, int count, object userData)
        {
        }

        #endregion

    }
}
