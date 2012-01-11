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
            MPfm.WindowsControls.CustomFont customFont17 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont11 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont12 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont13 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont14 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont15 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont16 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.SongGridViewTheme songGridViewTheme2 = new MPfm.WindowsControls.SongGridViewTheme();
            MPfm.WindowsControls.CustomFont customFont18 = new MPfm.WindowsControls.CustomFont();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.menuPlaylist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miPlaylistPlaySong = new System.Windows.Forms.ToolStripMenuItem();
            this.miPlaylistRemoveSongs = new System.Windows.Forms.ToolStripMenuItem();
            this.panelToolbar = new MPfm.WindowsControls.Panel();
            this.btnLoadPlaylist = new MPfm.WindowsControls.Button();
            this.btnClose = new MPfm.WindowsControls.Button();
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
            this.panelToolbar.Controls.Add(this.btnLoadPlaylist);
            this.panelToolbar.Controls.Add(this.btnClose);
            this.panelToolbar.Controls.Add(this.btnRemoveSongs);
            this.panelToolbar.Controls.Add(this.btnRenamePlaylist);
            this.panelToolbar.Controls.Add(this.btnSavePlaylistAs);
            this.panelToolbar.Controls.Add(this.btnSavePlaylist);
            this.panelToolbar.Controls.Add(this.btnNewPlaylist);
            customFont17.EmbeddedFontName = "";
            customFont17.IsBold = false;
            customFont17.IsItalic = false;
            customFont17.IsUnderline = false;
            customFont17.Size = 8F;
            customFont17.StandardFontName = "Arial";
            customFont17.UseAntiAliasing = true;
            customFont17.UseEmbeddedFont = false;
            this.panelToolbar.CustomFont = customFont17;
            this.panelToolbar.ExpandedHeight = 25;
            this.panelToolbar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelToolbar.GradientColor1 = System.Drawing.Color.Silver;
            this.panelToolbar.GradientColor2 = System.Drawing.Color.Gainsboro;
            this.panelToolbar.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelToolbar.HeaderCustomFontName = "Junction";
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
            // btnLoadPlaylist
            // 
            this.btnLoadPlaylist.BorderColor = System.Drawing.Color.DimGray;
            this.btnLoadPlaylist.BorderWidth = 1;
            this.btnLoadPlaylist.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont10.EmbeddedFontName = "Junction";
            customFont10.IsBold = false;
            customFont10.IsItalic = false;
            customFont10.IsUnderline = false;
            customFont10.Size = 8F;
            customFont10.StandardFontName = "Arial";
            customFont10.UseAntiAliasing = true;
            customFont10.UseEmbeddedFont = true;
            this.btnLoadPlaylist.CustomFont = customFont10;
            this.btnLoadPlaylist.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnLoadPlaylist.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnLoadPlaylist.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnLoadPlaylist.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnLoadPlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadPlaylist.FontColor = System.Drawing.Color.Black;
            this.btnLoadPlaylist.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnLoadPlaylist.GradientColor2 = System.Drawing.Color.Gray;
            this.btnLoadPlaylist.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnLoadPlaylist.Image = global::MPfm.Properties.Resources.folder_page;
            this.btnLoadPlaylist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadPlaylist.Location = new System.Drawing.Point(92, 0);
            this.btnLoadPlaylist.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnLoadPlaylist.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnLoadPlaylist.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnLoadPlaylist.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnLoadPlaylist.Name = "btnLoadPlaylist";
            this.btnLoadPlaylist.Size = new System.Drawing.Size(95, 25);
            this.btnLoadPlaylist.TabIndex = 74;
            this.btnLoadPlaylist.Text = "Load playlist";
            this.btnLoadPlaylist.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoadPlaylist.UseVisualStyleBackColor = true;
            this.btnLoadPlaylist.Click += new System.EventHandler(this.btnLoadPlaylist_Click);
            // 
            // btnClose
            // 
            this.btnClose.BorderColor = System.Drawing.Color.DimGray;
            this.btnClose.BorderWidth = 1;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont11.EmbeddedFontName = "Junction";
            customFont11.IsBold = false;
            customFont11.IsItalic = false;
            customFont11.IsUnderline = false;
            customFont11.Size = 8F;
            customFont11.StandardFontName = "Arial";
            customFont11.UseAntiAliasing = true;
            customFont11.UseEmbeddedFont = true;
            this.btnClose.CustomFont = customFont11;
            this.btnClose.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnClose.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.FontColor = System.Drawing.Color.Black;
            this.btnClose.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnClose.GradientColor2 = System.Drawing.Color.Gray;
            this.btnClose.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnClose.Image = global::MPfm.Properties.Resources.cancel;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(511, 0);
            this.btnClose.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnClose.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnClose.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnClose.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(58, 25);
            this.btnClose.TabIndex = 73;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRemoveSongs
            // 
            this.btnRemoveSongs.BorderColor = System.Drawing.Color.DimGray;
            this.btnRemoveSongs.BorderWidth = 1;
            this.btnRemoveSongs.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont12.EmbeddedFontName = "Junction";
            customFont12.IsBold = false;
            customFont12.IsItalic = false;
            customFont12.IsUnderline = false;
            customFont12.Size = 8F;
            customFont12.StandardFontName = "Arial";
            customFont12.UseAntiAliasing = true;
            customFont12.UseEmbeddedFont = true;
            this.btnRemoveSongs.CustomFont = customFont12;
            this.btnRemoveSongs.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnRemoveSongs.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnRemoveSongs.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnRemoveSongs.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnRemoveSongs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveSongs.FontColor = System.Drawing.Color.Black;
            this.btnRemoveSongs.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnRemoveSongs.GradientColor2 = System.Drawing.Color.Gray;
            this.btnRemoveSongs.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnRemoveSongs.Image = global::MPfm.Properties.Resources.delete;
            this.btnRemoveSongs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRemoveSongs.Location = new System.Drawing.Point(399, 0);
            this.btnRemoveSongs.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnRemoveSongs.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnRemoveSongs.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnRemoveSongs.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnRemoveSongs.Name = "btnRemoveSongs";
            this.btnRemoveSongs.Size = new System.Drawing.Size(113, 25);
            this.btnRemoveSongs.TabIndex = 72;
            this.btnRemoveSongs.Text = "Remove song(s)";
            this.btnRemoveSongs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRemoveSongs.UseVisualStyleBackColor = true;
            this.btnRemoveSongs.Click += new System.EventHandler(this.btnRemoveSongs_Click);
            // 
            // btnRenamePlaylist
            // 
            this.btnRenamePlaylist.BorderColor = System.Drawing.Color.DimGray;
            this.btnRenamePlaylist.BorderWidth = 1;
            this.btnRenamePlaylist.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont13.EmbeddedFontName = "Junction";
            customFont13.IsBold = false;
            customFont13.IsItalic = false;
            customFont13.IsUnderline = false;
            customFont13.Size = 8F;
            customFont13.StandardFontName = "Arial";
            customFont13.UseAntiAliasing = true;
            customFont13.UseEmbeddedFont = true;
            this.btnRenamePlaylist.CustomFont = customFont13;
            this.btnRenamePlaylist.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnRenamePlaylist.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnRenamePlaylist.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnRenamePlaylist.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnRenamePlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRenamePlaylist.FontColor = System.Drawing.Color.Black;
            this.btnRenamePlaylist.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnRenamePlaylist.GradientColor2 = System.Drawing.Color.Gray;
            this.btnRenamePlaylist.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnRenamePlaylist.Image = global::MPfm.Properties.Resources.textfield_rename;
            this.btnRenamePlaylist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRenamePlaylist.Location = new System.Drawing.Point(691, 0);
            this.btnRenamePlaylist.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnRenamePlaylist.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnRenamePlaylist.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnRenamePlaylist.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
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
            this.btnSavePlaylistAs.BorderColor = System.Drawing.Color.DimGray;
            this.btnSavePlaylistAs.BorderWidth = 1;
            this.btnSavePlaylistAs.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont14.EmbeddedFontName = "Junction";
            customFont14.IsBold = false;
            customFont14.IsItalic = false;
            customFont14.IsUnderline = false;
            customFont14.Size = 8F;
            customFont14.StandardFontName = "Arial";
            customFont14.UseAntiAliasing = true;
            customFont14.UseEmbeddedFont = true;
            this.btnSavePlaylistAs.CustomFont = customFont14;
            this.btnSavePlaylistAs.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnSavePlaylistAs.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnSavePlaylistAs.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnSavePlaylistAs.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSavePlaylistAs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePlaylistAs.FontColor = System.Drawing.Color.Black;
            this.btnSavePlaylistAs.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnSavePlaylistAs.GradientColor2 = System.Drawing.Color.Gray;
            this.btnSavePlaylistAs.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnSavePlaylistAs.Image = global::MPfm.Properties.Resources.disk;
            this.btnSavePlaylistAs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSavePlaylistAs.Location = new System.Drawing.Point(280, 0);
            this.btnSavePlaylistAs.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnSavePlaylistAs.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnSavePlaylistAs.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnSavePlaylistAs.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnSavePlaylistAs.Name = "btnSavePlaylistAs";
            this.btnSavePlaylistAs.Size = new System.Drawing.Size(120, 25);
            this.btnSavePlaylistAs.TabIndex = 70;
            this.btnSavePlaylistAs.Text = "Save playlist as...";
            this.btnSavePlaylistAs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSavePlaylistAs.UseVisualStyleBackColor = true;
            this.btnSavePlaylistAs.Click += new System.EventHandler(this.btnSavePlaylistAs_Click);
            // 
            // btnSavePlaylist
            // 
            this.btnSavePlaylist.BorderColor = System.Drawing.Color.DimGray;
            this.btnSavePlaylist.BorderWidth = 1;
            this.btnSavePlaylist.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont15.EmbeddedFontName = "Junction";
            customFont15.IsBold = false;
            customFont15.IsItalic = false;
            customFont15.IsUnderline = false;
            customFont15.Size = 8F;
            customFont15.StandardFontName = "Arial";
            customFont15.UseAntiAliasing = true;
            customFont15.UseEmbeddedFont = true;
            this.btnSavePlaylist.CustomFont = customFont15;
            this.btnSavePlaylist.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnSavePlaylist.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnSavePlaylist.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnSavePlaylist.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSavePlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePlaylist.FontColor = System.Drawing.Color.Black;
            this.btnSavePlaylist.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnSavePlaylist.GradientColor2 = System.Drawing.Color.Gray;
            this.btnSavePlaylist.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnSavePlaylist.Image = global::MPfm.Properties.Resources.disk;
            this.btnSavePlaylist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSavePlaylist.Location = new System.Drawing.Point(186, 0);
            this.btnSavePlaylist.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnSavePlaylist.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnSavePlaylist.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnSavePlaylist.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnSavePlaylist.Name = "btnSavePlaylist";
            this.btnSavePlaylist.Size = new System.Drawing.Size(95, 25);
            this.btnSavePlaylist.TabIndex = 69;
            this.btnSavePlaylist.Text = "Save playlist";
            this.btnSavePlaylist.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSavePlaylist.UseVisualStyleBackColor = true;
            this.btnSavePlaylist.Click += new System.EventHandler(this.btnSavePlaylist_Click);
            // 
            // btnNewPlaylist
            // 
            this.btnNewPlaylist.BorderColor = System.Drawing.Color.DimGray;
            this.btnNewPlaylist.BorderWidth = 1;
            this.btnNewPlaylist.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont16.EmbeddedFontName = "Junction";
            customFont16.IsBold = false;
            customFont16.IsItalic = false;
            customFont16.IsUnderline = false;
            customFont16.Size = 8F;
            customFont16.StandardFontName = "Arial";
            customFont16.UseAntiAliasing = true;
            customFont16.UseEmbeddedFont = true;
            this.btnNewPlaylist.CustomFont = customFont16;
            this.btnNewPlaylist.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnNewPlaylist.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnNewPlaylist.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnNewPlaylist.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnNewPlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewPlaylist.FontColor = System.Drawing.Color.Black;
            this.btnNewPlaylist.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnNewPlaylist.GradientColor2 = System.Drawing.Color.Gray;
            this.btnNewPlaylist.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnNewPlaylist.Image = global::MPfm.Properties.Resources.page_white_text;
            this.btnNewPlaylist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNewPlaylist.Location = new System.Drawing.Point(0, 0);
            this.btnNewPlaylist.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnNewPlaylist.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnNewPlaylist.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnNewPlaylist.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnNewPlaylist.Name = "btnNewPlaylist";
            this.btnNewPlaylist.Size = new System.Drawing.Size(93, 25);
            this.btnNewPlaylist.TabIndex = 68;
            this.btnNewPlaylist.Text = "New playlist";
            this.btnNewPlaylist.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNewPlaylist.UseVisualStyleBackColor = true;
            this.btnNewPlaylist.Click += new System.EventHandler(this.btnNewPlaylist_Click);
            // 
            // viewSongs2
            // 
            this.viewSongs2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewSongs2.CanChangeOrderBy = false;
            this.viewSongs2.CanMoveColumns = false;
            this.viewSongs2.CanReorderItems = true;
            this.viewSongs2.CanResizeColumns = true;
            this.viewSongs2.ContextMenuStrip = this.menuPlaylist;
            this.viewSongs2.DisplayDebugInformation = false;
            this.viewSongs2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewSongs2.ImageCacheSize = 10;
            this.viewSongs2.Location = new System.Drawing.Point(0, 25);
            this.viewSongs2.Name = "viewSongs2";
            this.viewSongs2.NowPlayingAudioFileId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.viewSongs2.NowPlayingPlaylistItemId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.viewSongs2.OrderByAscending = true;
            this.viewSongs2.OrderByFieldName = "";
            this.viewSongs2.Size = new System.Drawing.Size(807, 329);
            this.viewSongs2.TabIndex = 75;
            this.viewSongs2.Text = "songGridView1";
            songGridViewTheme2.AlbumCoverBackgroundColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            songGridViewTheme2.AlbumCoverBackgroundColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            customFont18.EmbeddedFontName = "Junction";
            customFont18.IsBold = false;
            customFont18.IsItalic = false;
            customFont18.IsUnderline = false;
            customFont18.Size = 8F;
            customFont18.StandardFontName = "Arial";
            customFont18.UseAntiAliasing = true;
            customFont18.UseEmbeddedFont = true;
            songGridViewTheme2.Font = customFont18;
            songGridViewTheme2.HeaderColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(165)))), ((int)(((byte)(165)))));
            songGridViewTheme2.HeaderColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(195)))), ((int)(((byte)(195)))));
            songGridViewTheme2.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            songGridViewTheme2.HeaderHoverColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(145)))), ((int)(((byte)(145)))));
            songGridViewTheme2.HeaderHoverColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            songGridViewTheme2.IconNowPlayingColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(200)))), ((int)(((byte)(250)))));
            songGridViewTheme2.IconNowPlayingColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(150)))), ((int)(((byte)(25)))));
            songGridViewTheme2.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            songGridViewTheme2.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            songGridViewTheme2.LineForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            songGridViewTheme2.LineNowPlayingColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(235)))), ((int)(((byte)(135)))));
            songGridViewTheme2.LineNowPlayingColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(255)))), ((int)(((byte)(155)))));
            songGridViewTheme2.Padding = 6;
            this.viewSongs2.Theme = songGridViewTheme2;
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
        private WindowsControls.Button btnClose;
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
    }
}