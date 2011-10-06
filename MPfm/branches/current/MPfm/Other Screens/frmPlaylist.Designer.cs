namespace MPfm
{
    partial class frmPlaylist
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlaylist));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolPlaylist = new MPfm.WindowsControls.ToolStrip();
            this.btnNewPlaylist = new System.Windows.Forms.ToolStripButton();
            this.btnSavePlaylist = new System.Windows.Forms.ToolStripButton();
            this.btnSavePlaylistAs = new System.Windows.Forms.ToolStripButton();
            this.btnRenamePlaylist = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveSongs = new System.Windows.Forms.ToolStripButton();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.viewSongs = new MPfm.WindowsControls.ReorderListView();
            this.columnPlaylistSongPlayIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlaylistSongTrackNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlaylistSongTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlaylistSongLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlaylistSongArtistName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlaylistSongAlbumTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlaylistSongPlayCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlaylistSongLastPlayed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuPlaylist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miPlaylistPlaySong = new System.Windows.Forms.ToolStripMenuItem();
            this.miPlaylistRemoveSongs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolPlaylist.SuspendLayout();
            this.menuPlaylist.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "bullet_go.png");
            // 
            // toolPlaylist
            // 
            this.toolPlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolPlaylist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewPlaylist,
            this.btnSavePlaylist,
            this.btnSavePlaylistAs,
            this.btnRenamePlaylist,
            this.btnRemoveSongs,
            this.btnClose});
            this.toolPlaylist.Location = new System.Drawing.Point(0, 0);
            this.toolPlaylist.Name = "toolPlaylist";
            this.toolPlaylist.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolPlaylist.Size = new System.Drawing.Size(982, 25);
            this.toolPlaylist.TabIndex = 21;
            this.toolPlaylist.Text = "toolStrip3";
            // 
            // btnNewPlaylist
            // 
            this.btnNewPlaylist.Image = global::MPfm.Properties.Resources.page_white_text;
            this.btnNewPlaylist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewPlaylist.Name = "btnNewPlaylist";
            this.btnNewPlaylist.Size = new System.Drawing.Size(86, 22);
            this.btnNewPlaylist.Text = "New playlist";
            this.btnNewPlaylist.ToolTipText = "New playlist";
            this.btnNewPlaylist.Click += new System.EventHandler(this.btnNewPlaylist_Click);
            // 
            // btnSavePlaylist
            // 
            this.btnSavePlaylist.Image = global::MPfm.Properties.Resources.disk;
            this.btnSavePlaylist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSavePlaylist.Name = "btnSavePlaylist";
            this.btnSavePlaylist.Size = new System.Drawing.Size(88, 22);
            this.btnSavePlaylist.Text = "Save playlist";
            this.btnSavePlaylist.Click += new System.EventHandler(this.btnSavePlaylist_Click);
            // 
            // btnSavePlaylistAs
            // 
            this.btnSavePlaylistAs.Image = global::MPfm.Properties.Resources.disk_multiple;
            this.btnSavePlaylistAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSavePlaylistAs.Name = "btnSavePlaylistAs";
            this.btnSavePlaylistAs.Size = new System.Drawing.Size(112, 22);
            this.btnSavePlaylistAs.Text = "Save playlist as...";
            this.btnSavePlaylistAs.Click += new System.EventHandler(this.btnSavePlaylistAs_Click);
            // 
            // btnRenamePlaylist
            // 
            this.btnRenamePlaylist.Image = global::MPfm.Properties.Resources.textfield_rename;
            this.btnRenamePlaylist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRenamePlaylist.Name = "btnRenamePlaylist";
            this.btnRenamePlaylist.Size = new System.Drawing.Size(102, 22);
            this.btnRenamePlaylist.Text = "Rename playlist";
            this.btnRenamePlaylist.Click += new System.EventHandler(this.btnRenamePlaylist_Click);
            // 
            // btnRemoveSongs
            // 
            this.btnRemoveSongs.Image = global::MPfm.Properties.Resources.delete;
            this.btnRemoveSongs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveSongs.Name = "btnRemoveSongs";
            this.btnRemoveSongs.Size = new System.Drawing.Size(107, 22);
            this.btnRemoveSongs.Text = "Remove song(s)";
            this.btnRemoveSongs.Click += new System.EventHandler(this.btnRemoveSongs_Click);
            // 
            // btnClose
            // 
            this.btnClose.Image = global::MPfm.Properties.Resources.cancel;
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(54, 22);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 476);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(982, 22);
            this.statusStrip1.TabIndex = 22;
            this.statusStrip1.Text = "0 song(s)";
            // 
            // viewSongs
            // 
            this.viewSongs.AllowDrop = true;
            this.viewSongs.AllowRowReorder = true;
            this.viewSongs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.viewSongs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnPlaylistSongPlayIcon,
            this.columnPlaylistSongTrackNumber,
            this.columnPlaylistSongTitle,
            this.columnPlaylistSongLength,
            this.columnPlaylistSongArtistName,
            this.columnPlaylistSongAlbumTitle,
            this.columnPlaylistSongPlayCount,
            this.columnPlaylistSongLastPlayed});
            this.viewSongs.ContextMenuStrip = this.menuPlaylist;
            this.viewSongs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewSongs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewSongs.FontCollection = null;
            this.viewSongs.FullRowSelect = true;
            this.viewSongs.GradientColor1 = System.Drawing.Color.LightGray;
            this.viewSongs.GradientColor2 = System.Drawing.Color.Gray;
            this.viewSongs.GridLines = true;
            this.viewSongs.HeaderForeColor = System.Drawing.Color.Black;
            this.viewSongs.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.viewSongs.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.viewSongs.HeaderHeight = 0;
            this.viewSongs.HideSelection = false;
            this.viewSongs.Location = new System.Drawing.Point(0, 25);
            this.viewSongs.Name = "viewSongs";
            this.viewSongs.SelectedColor = System.Drawing.Color.DarkGray;
            this.viewSongs.Size = new System.Drawing.Size(982, 451);
            this.viewSongs.SmallImageList = this.imageList;
            this.viewSongs.TabIndex = 11;
            this.viewSongs.UseCompatibleStateImageBehavior = false;
            this.viewSongs.View = System.Windows.Forms.View.Details;
            this.viewSongs.ItemsReordered += new MPfm.WindowsControls.ReorderListView.ItemsReorderedHandler(this.viewSongs_ItemsReordered);
            this.viewSongs.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.viewSongs_MouseDoubleClick);
            // 
            // columnPlaylistSongPlayIcon
            // 
            this.columnPlaylistSongPlayIcon.Text = "";
            this.columnPlaylistSongPlayIcon.Width = 21;
            // 
            // columnPlaylistSongTrackNumber
            // 
            this.columnPlaylistSongTrackNumber.Text = "Tr #";
            this.columnPlaylistSongTrackNumber.Width = 35;
            // 
            // columnPlaylistSongTitle
            // 
            this.columnPlaylistSongTitle.Text = "Title";
            this.columnPlaylistSongTitle.Width = 234;
            // 
            // columnPlaylistSongLength
            // 
            this.columnPlaylistSongLength.Text = "Length";
            this.columnPlaylistSongLength.Width = 80;
            // 
            // columnPlaylistSongArtistName
            // 
            this.columnPlaylistSongArtistName.Text = "Artist Name";
            this.columnPlaylistSongArtistName.Width = 198;
            // 
            // columnPlaylistSongAlbumTitle
            // 
            this.columnPlaylistSongAlbumTitle.Text = "Album Title";
            this.columnPlaylistSongAlbumTitle.Width = 216;
            // 
            // columnPlaylistSongPlayCount
            // 
            this.columnPlaylistSongPlayCount.Text = "Play Count";
            this.columnPlaylistSongPlayCount.Width = 68;
            // 
            // columnPlaylistSongLastPlayed
            // 
            this.columnPlaylistSongLastPlayed.Text = "Last Played";
            this.columnPlaylistSongLastPlayed.Width = 161;
            // 
            // menuPlaylist
            // 
            this.menuPlaylist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPlaylistPlaySong,
            this.miPlaylistRemoveSongs});
            this.menuPlaylist.Name = "menuSongBrowser";
            this.menuPlaylist.Size = new System.Drawing.Size(180, 48);
            this.menuPlaylist.Opening += new System.ComponentModel.CancelEventHandler(this.menuPlaylist_Opening);
            // 
            // miPlaylistPlaySong
            // 
            this.miPlaylistPlaySong.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.miPlaylistPlaySong.Image = global::MPfm.Properties.Resources.control_play;
            this.miPlaylistPlaySong.Name = "miPlaylistPlaySong";
            this.miPlaylistPlaySong.Size = new System.Drawing.Size(179, 22);
            this.miPlaylistPlaySong.Text = "Play selected song";
            this.miPlaylistPlaySong.Click += new System.EventHandler(this.miPlaylistPlaySong_Click);
            // 
            // miPlaylistRemoveSongs
            // 
            this.miPlaylistRemoveSongs.Image = global::MPfm.Properties.Resources.delete;
            this.miPlaylistRemoveSongs.Name = "miPlaylistRemoveSongs";
            this.miPlaylistRemoveSongs.Size = new System.Drawing.Size(179, 22);
            this.miPlaylistRemoveSongs.Text = "Remove song(s)";
            this.miPlaylistRemoveSongs.Click += new System.EventHandler(this.btnRemoveSongs_Click);
            // 
            // frmPlaylist
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(982, 498);
            this.Controls.Add(this.viewSongs);
            this.Controls.Add(this.toolPlaylist);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "frmPlaylist";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Playlist";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPlaylist_FormClosing);
            this.Shown += new System.EventHandler(this.frmPlaylist_Shown);
            this.toolPlaylist.ResumeLayout(false);
            this.toolPlaylist.PerformLayout();
            this.menuPlaylist.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnPlaylistSongPlayIcon;
        private System.Windows.Forms.ColumnHeader columnPlaylistSongTitle;
        private System.Windows.Forms.ColumnHeader columnPlaylistSongLength;
        private System.Windows.Forms.ColumnHeader columnPlaylistSongArtistName;
        private System.Windows.Forms.ColumnHeader columnPlaylistSongAlbumTitle;
        private System.Windows.Forms.ColumnHeader columnPlaylistSongPlayCount;
        private System.Windows.Forms.ColumnHeader columnPlaylistSongLastPlayed;
        private MPfm.WindowsControls.ToolStrip toolPlaylist;
        private System.Windows.Forms.ToolStripButton btnSavePlaylistAs;
        private System.Windows.Forms.ToolStripButton btnRemoveSongs;
        private System.Windows.Forms.StatusStrip statusStrip1;
        public MPfm.WindowsControls.ReorderListView viewSongs;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.ToolStripButton btnSavePlaylist;
        private System.Windows.Forms.ContextMenuStrip menuPlaylist;
        private System.Windows.Forms.ToolStripMenuItem miPlaylistPlaySong;
        private System.Windows.Forms.ToolStripMenuItem miPlaylistRemoveSongs;
        private System.Windows.Forms.ToolStripButton btnNewPlaylist;
        private System.Windows.Forms.ToolStripButton btnRenamePlaylist;
        private System.Windows.Forms.ColumnHeader columnPlaylistSongTrackNumber;
    }
}