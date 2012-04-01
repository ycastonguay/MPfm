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
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.SongGridViewTheme songGridViewTheme1 = new MPfm.WindowsControls.SongGridViewTheme();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.menuPlaylist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miPlaylistPlaySong = new System.Windows.Forms.ToolStripMenuItem();
            this.miPlaylistRemoveSongs = new System.Windows.Forms.ToolStripMenuItem();
            this.panelToolbar = new MPfm.WindowsControls.Panel();
            this.button1 = new MPfm.WindowsControls.Button();
            this.btnLoadPlaylist = new MPfm.WindowsControls.Button();
            this.btnRemoveSongs = new MPfm.WindowsControls.Button();
            this.btnRenamePlaylist = new MPfm.WindowsControls.Button();
            this.btnSavePlaylistAs = new MPfm.WindowsControls.Button();
            this.btnSavePlaylist = new MPfm.WindowsControls.Button();
            this.btnNewPlaylist = new MPfm.WindowsControls.Button();
            this.viewSongs2 = new MPfm.WindowsControls.SongGridView();
            this.dialogLoadPlaylist = new System.Windows.Forms.OpenFileDialog();
            this.dialogSavePlaylist = new System.Windows.Forms.SaveFileDialog();
            this.menuLoadPlaylist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miLoadPlaylistBrowse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.recentPlaylistsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fgfgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miLoadPlaylistLibrary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuPlaylist.SuspendLayout();
            this.panelToolbar.SuspendLayout();
            this.menuLoadPlaylist.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "bullet_go.png");
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
            // panelToolbar
            // 
            this.panelToolbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelToolbar.Controls.Add(this.button1);
            this.panelToolbar.Controls.Add(this.btnLoadPlaylist);
            this.panelToolbar.Controls.Add(this.btnRemoveSongs);
            this.panelToolbar.Controls.Add(this.btnRenamePlaylist);
            this.panelToolbar.Controls.Add(this.btnSavePlaylistAs);
            this.panelToolbar.Controls.Add(this.btnSavePlaylist);
            this.panelToolbar.Controls.Add(this.btnNewPlaylist);
            customFont8.EmbeddedFontName = "";
            customFont8.IsBold = false;
            customFont8.IsItalic = false;
            customFont8.IsUnderline = false;
            customFont8.Size = 8F;
            customFont8.StandardFontName = "Arial";
            customFont8.UseAntiAliasing = true;
            customFont8.UseEmbeddedFont = false;
            this.panelToolbar.CustomFont = customFont8;
            this.panelToolbar.ExpandedHeight = 25;
            this.panelToolbar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelToolbar.GradientColor1 = System.Drawing.Color.Silver;
            this.panelToolbar.GradientColor2 = System.Drawing.Color.Gainsboro;
            this.panelToolbar.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelToolbar.HeaderExpandable = false;
            this.panelToolbar.HeaderExpanded = true;
            this.panelToolbar.HeaderForeColor = System.Drawing.Color.White;
            this.panelToolbar.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelToolbar.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelToolbar.HeaderHeight = 0;
            this.panelToolbar.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(807, 25);
            this.panelToolbar.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = global::MPfm.Properties.Resources.arrow_switch;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(511, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 25);
            this.button1.TabIndex = 75;
            this.button1.Text = "Shuffle playlist";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.button1, "Shuffles the playlist items.");
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnLoadPlaylist
            // 
            this.btnLoadPlaylist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadPlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadPlaylist.Image = global::MPfm.Properties.Resources.folder_page;
            this.btnLoadPlaylist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadPlaylist.Location = new System.Drawing.Point(92, 0);
            this.btnLoadPlaylist.Name = "btnLoadPlaylist";
            this.btnLoadPlaylist.Size = new System.Drawing.Size(95, 25);
            this.btnLoadPlaylist.TabIndex = 74;
            this.btnLoadPlaylist.Text = "Load playlist";
            this.btnLoadPlaylist.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnLoadPlaylist, "Loads a playlist file (M3U, M3U8, PLS, XSPF).");
            this.btnLoadPlaylist.UseVisualStyleBackColor = true;
            this.btnLoadPlaylist.Click += new System.EventHandler(this.btnLoadPlaylist_Click);
            // 
            // btnRemoveSongs
            // 
            this.btnRemoveSongs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemoveSongs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveSongs.Image = global::MPfm.Properties.Resources.delete;
            this.btnRemoveSongs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRemoveSongs.Location = new System.Drawing.Point(399, 0);
            this.btnRemoveSongs.Name = "btnRemoveSongs";
            this.btnRemoveSongs.Size = new System.Drawing.Size(113, 25);
            this.btnRemoveSongs.TabIndex = 72;
            this.btnRemoveSongs.Text = "Remove song(s)  ";
            this.btnRemoveSongs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnRemoveSongs, "Removes the selected songs from the playlist.");
            this.btnRemoveSongs.UseVisualStyleBackColor = true;
            this.btnRemoveSongs.Click += new System.EventHandler(this.btnRemoveSongs_Click);
            // 
            // btnRenamePlaylist
            // 
            this.btnRenamePlaylist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRenamePlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRenamePlaylist.Image = global::MPfm.Properties.Resources.textfield_rename;
            this.btnRenamePlaylist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRenamePlaylist.Location = new System.Drawing.Point(691, 0);
            this.btnRenamePlaylist.Name = "btnRenamePlaylist";
            this.btnRenamePlaylist.Size = new System.Drawing.Size(116, 25);
            this.btnRenamePlaylist.TabIndex = 71;
            this.btnRenamePlaylist.Text = "Rename playlist";
            this.btnRenamePlaylist.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRenamePlaylist.UseVisualStyleBackColor = true;
            this.btnRenamePlaylist.Visible = false;
            this.btnRenamePlaylist.Click += new System.EventHandler(this.btnRenamePlaylist_Click);
            // 
            // btnSavePlaylistAs
            // 
            this.btnSavePlaylistAs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSavePlaylistAs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePlaylistAs.Image = global::MPfm.Properties.Resources.disk;
            this.btnSavePlaylistAs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSavePlaylistAs.Location = new System.Drawing.Point(280, 0);
            this.btnSavePlaylistAs.Name = "btnSavePlaylistAs";
            this.btnSavePlaylistAs.Size = new System.Drawing.Size(120, 25);
            this.btnSavePlaylistAs.TabIndex = 70;
            this.btnSavePlaylistAs.Text = "Save playlist as...";
            this.btnSavePlaylistAs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnSavePlaylistAs, "Saves the current playlist under a different format/file name.");
            this.btnSavePlaylistAs.UseVisualStyleBackColor = true;
            this.btnSavePlaylistAs.Click += new System.EventHandler(this.btnSavePlaylistAs_Click);
            // 
            // btnSavePlaylist
            // 
            this.btnSavePlaylist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSavePlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePlaylist.Image = global::MPfm.Properties.Resources.disk;
            this.btnSavePlaylist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSavePlaylist.Location = new System.Drawing.Point(186, 0);
            this.btnSavePlaylist.Name = "btnSavePlaylist";
            this.btnSavePlaylist.Size = new System.Drawing.Size(95, 25);
            this.btnSavePlaylist.TabIndex = 69;
            this.btnSavePlaylist.Text = "Save playlist";
            this.btnSavePlaylist.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnSavePlaylist, "Saves the current playlist.");
            this.btnSavePlaylist.UseVisualStyleBackColor = true;
            this.btnSavePlaylist.Click += new System.EventHandler(this.btnSavePlaylist_Click);
            // 
            // btnNewPlaylist
            // 
            this.btnNewPlaylist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewPlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewPlaylist.Image = global::MPfm.Properties.Resources.page_white_text;
            this.btnNewPlaylist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNewPlaylist.Location = new System.Drawing.Point(0, 0);
            this.btnNewPlaylist.Name = "btnNewPlaylist";
            this.btnNewPlaylist.Size = new System.Drawing.Size(93, 25);
            this.btnNewPlaylist.TabIndex = 68;
            this.btnNewPlaylist.Text = "New playlist";
            this.btnNewPlaylist.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnNewPlaylist, "Creates a new empty playlist.");
            this.btnNewPlaylist.UseVisualStyleBackColor = true;
            this.btnNewPlaylist.Click += new System.EventHandler(this.btnNewPlaylist_Click);
            // 
            // viewSongs2
            // 
            this.viewSongs2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewSongs2.CanChangeOrderBy = false;
            this.viewSongs2.CanMoveColumns = true;
            this.viewSongs2.CanReorderItems = true;
            this.viewSongs2.CanResizeColumns = true;
            this.viewSongs2.ContextMenuStrip = this.menuPlaylist;
            this.viewSongs2.DisplayDebugInformation = false;
            this.viewSongs2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewSongs2.ImageCacheSize = 10;
            this.viewSongs2.Location = new System.Drawing.Point(-1, 25);
            this.viewSongs2.Name = "viewSongs2";
            this.viewSongs2.NowPlayingAudioFileId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.viewSongs2.NowPlayingPlaylistItemId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.viewSongs2.OrderByAscending = true;
            this.viewSongs2.OrderByFieldName = "";
            this.viewSongs2.Size = new System.Drawing.Size(807, 329);
            this.viewSongs2.TabIndex = 75;
            this.viewSongs2.Text = "songGridView1";
            songGridViewTheme1.AlbumCoverBackgroundColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            songGridViewTheme1.AlbumCoverBackgroundColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            customFont9.EmbeddedFontName = "Junction";
            customFont9.IsBold = false;
            customFont9.IsItalic = false;
            customFont9.IsUnderline = false;
            customFont9.Size = 8F;
            customFont9.StandardFontName = "Arial";
            customFont9.UseAntiAliasing = true;
            customFont9.UseEmbeddedFont = true;
            songGridViewTheme1.Font = customFont9;
            songGridViewTheme1.HeaderColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(165)))), ((int)(((byte)(165)))));
            songGridViewTheme1.HeaderColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(195)))), ((int)(((byte)(195)))));
            songGridViewTheme1.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            songGridViewTheme1.HeaderHoverColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(145)))), ((int)(((byte)(145)))));
            songGridViewTheme1.HeaderHoverColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            songGridViewTheme1.IconNowPlayingColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(200)))), ((int)(((byte)(250)))));
            songGridViewTheme1.IconNowPlayingColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(150)))), ((int)(((byte)(25)))));
            songGridViewTheme1.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            songGridViewTheme1.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            songGridViewTheme1.LineForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            songGridViewTheme1.LineNowPlayingColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(235)))), ((int)(((byte)(135)))));
            songGridViewTheme1.LineNowPlayingColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(255)))), ((int)(((byte)(155)))));
            songGridViewTheme1.Padding = 6;
            this.viewSongs2.Theme = songGridViewTheme1;
            this.viewSongs2.DoubleClick += new System.EventHandler(this.viewSongs2_DoubleClick);
            // 
            // dialogLoadPlaylist
            // 
            this.dialogLoadPlaylist.Filter = "Playlist files (*.m3u,*.m3u8,*.pls,*.xspf)|*.m3u;*.m3u8;*.pls;*.xspf";
            this.dialogLoadPlaylist.Title = "Please select the playlist to load";
            // 
            // dialogSavePlaylist
            // 
            this.dialogSavePlaylist.Filter = "M3U Playlist file (*.m3u)|*.m3u|M3U8 Playlist file (*.m3u8)|*.m3u8|PLS Playlist f" +
    "ile (*.pls)|*.pls|XSPF Playlist file (*.xspf)|*.xspf";
            this.dialogSavePlaylist.Title = "Please select a file path for the playlist";
            // 
            // menuLoadPlaylist
            // 
            this.menuLoadPlaylist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLoadPlaylistBrowse,
            this.toolStripSeparator1,
            this.recentPlaylistsToolStripMenuItem,
            this.miLoadPlaylistLibrary});
            this.menuLoadPlaylist.Name = "menuLoadPlaylist";
            this.menuLoadPlaylist.Size = new System.Drawing.Size(208, 76);
            // 
            // miLoadPlaylistBrowse
            // 
            this.miLoadPlaylistBrowse.Image = global::MPfm.Properties.Resources.folder_page;
            this.miLoadPlaylistBrowse.Name = "miLoadPlaylistBrowse";
            this.miLoadPlaylistBrowse.Size = new System.Drawing.Size(207, 22);
            this.miLoadPlaylistBrowse.Text = "Browse...";
            this.miLoadPlaylistBrowse.Click += new System.EventHandler(this.miLoadPlaylistBrowse_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(204, 6);
            // 
            // recentPlaylistsToolStripMenuItem
            // 
            this.recentPlaylistsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fgfgToolStripMenuItem});
            this.recentPlaylistsToolStripMenuItem.Name = "recentPlaylistsToolStripMenuItem";
            this.recentPlaylistsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.recentPlaylistsToolStripMenuItem.Text = "Recently opened playlists";
            this.recentPlaylistsToolStripMenuItem.Visible = false;
            // 
            // fgfgToolStripMenuItem
            // 
            this.fgfgToolStripMenuItem.Name = "fgfgToolStripMenuItem";
            this.fgfgToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.fgfgToolStripMenuItem.Text = "fgfg";
            // 
            // miLoadPlaylistLibrary
            // 
            this.miLoadPlaylistLibrary.Image = global::MPfm.Properties.Resources.database;
            this.miLoadPlaylistLibrary.Name = "miLoadPlaylistLibrary";
            this.miLoadPlaylistLibrary.Size = new System.Drawing.Size(207, 22);
            this.miLoadPlaylistLibrary.Text = "Library playlists";
            // 
            // toolTip
            // 
            this.toolTip.BackColor = System.Drawing.Color.DimGray;
            this.toolTip.ForeColor = System.Drawing.Color.White;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip.ToolTipTitle = "Playlist";
            // 
            // frmPlaylist
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(807, 354);
            this.Controls.Add(this.panelToolbar);
            this.Controls.Add(this.viewSongs2);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "frmPlaylist";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Playlist";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPlaylist_FormClosing);
            this.Shown += new System.EventHandler(this.frmPlaylist_Shown);
            this.VisibleChanged += new System.EventHandler(this.frmPlaylist_VisibleChanged);
            this.menuPlaylist.ResumeLayout(false);
            this.panelToolbar.ResumeLayout(false);
            this.menuLoadPlaylist.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ContextMenuStrip menuPlaylist;
        private System.Windows.Forms.ToolStripMenuItem miPlaylistPlaySong;
        private System.Windows.Forms.ToolStripMenuItem miPlaylistRemoveSongs;
        private WindowsControls.Panel panelToolbar;
        private WindowsControls.Button btnNewPlaylist;
        private WindowsControls.Button btnRemoveSongs;
        private WindowsControls.Button btnRenamePlaylist;
        private WindowsControls.Button btnSavePlaylistAs;
        private WindowsControls.Button btnSavePlaylist;
        private WindowsControls.Button btnLoadPlaylist;
        private System.Windows.Forms.OpenFileDialog dialogLoadPlaylist;
        private System.Windows.Forms.SaveFileDialog dialogSavePlaylist;
        private System.Windows.Forms.ContextMenuStrip menuLoadPlaylist;
        private System.Windows.Forms.ToolStripMenuItem miLoadPlaylistBrowse;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miLoadPlaylistLibrary;
        private System.Windows.Forms.ToolStripMenuItem recentPlaylistsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fgfgToolStripMenuItem;
        public WindowsControls.SongGridView viewSongs2;
        private WindowsControls.Button button1;
        public System.Windows.Forms.ToolTip toolTip;
    }
}