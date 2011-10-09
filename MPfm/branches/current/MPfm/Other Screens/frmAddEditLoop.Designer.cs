namespace MPfm
{
    partial class frmAddEditLoop
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
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.panelEditLoop = new MPfm.WindowsControls.Panel();
            this.lblSongTitle = new MPfm.WindowsControls.Label();
            this.lblAlbumTitle = new MPfm.WindowsControls.Label();
            this.lblArtistName = new MPfm.WindowsControls.Label();
            this.lblSong = new MPfm.WindowsControls.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new MPfm.WindowsControls.Label();
            this.btnSave = new MPfm.WindowsControls.Button();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.lblMarkerA = new MPfm.WindowsControls.Label();
            this.lblMarkerB = new MPfm.WindowsControls.Label();
            this.comboMarkerA = new System.Windows.Forms.ComboBox();
            this.comboMarkerB = new System.Windows.Forms.ComboBox();
            this.lblLoopLengthValue = new MPfm.WindowsControls.Label();
            this.lblLoopLength = new MPfm.WindowsControls.Label();
            this.panelEditLoop.SuspendLayout();
            this.SuspendLayout();
            // 
            // fontCollection
            // 
            customFont1.AssemblyPath = "MPfm.Fonts.dll";
            customFont1.Name = "LeagueGothic";
            customFont1.ResourceName = "MPfm.Fonts.LeagueGothic.ttf";
            customFont2.AssemblyPath = "MPfm.Fonts.dll";
            customFont2.Name = "Junction";
            customFont2.ResourceName = "MPfm.Fonts.Junction.ttf";
            customFont3.AssemblyPath = "MPfm.Fonts.dll";
            customFont3.Name = "Nobile";
            customFont3.ResourceName = "MPfm.Fonts.nobile.ttf";
            customFont4.AssemblyPath = "MPfm.Fonts.dll";
            customFont4.Name = "TitilliumText22L Lt";
            customFont4.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            customFont5.AssemblyPath = "MPfm.Fonts.dll";
            customFont5.Name = "Museo Sans 500";
            customFont5.ResourceName = "MPfm.Fonts.MuseoSans_500.ttf";
            this.fontCollection.Fonts.Add(customFont1);
            this.fontCollection.Fonts.Add(customFont2);
            this.fontCollection.Fonts.Add(customFont3);
            this.fontCollection.Fonts.Add(customFont4);
            this.fontCollection.Fonts.Add(customFont5);
            // 
            // panelEditLoop
            // 
            this.panelEditLoop.AntiAliasingEnabled = true;
            this.panelEditLoop.Controls.Add(this.lblLoopLengthValue);
            this.panelEditLoop.Controls.Add(this.comboMarkerB);
            this.panelEditLoop.Controls.Add(this.lblLoopLength);
            this.panelEditLoop.Controls.Add(this.comboMarkerA);
            this.panelEditLoop.Controls.Add(this.lblMarkerB);
            this.panelEditLoop.Controls.Add(this.lblMarkerA);
            this.panelEditLoop.Controls.Add(this.lblSongTitle);
            this.panelEditLoop.Controls.Add(this.lblAlbumTitle);
            this.panelEditLoop.Controls.Add(this.lblArtistName);
            this.panelEditLoop.Controls.Add(this.lblSong);
            this.panelEditLoop.Controls.Add(this.txtName);
            this.panelEditLoop.Controls.Add(this.lblName);
            this.panelEditLoop.Controls.Add(this.btnSave);
            this.panelEditLoop.Controls.Add(this.btnClose);
            this.panelEditLoop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEditLoop.ExpandedHeight = 200;
            this.panelEditLoop.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelEditLoop.FontCollection = this.fontCollection;
            this.panelEditLoop.GradientColor1 = System.Drawing.Color.Silver;
            this.panelEditLoop.GradientColor2 = System.Drawing.Color.Gray;
            this.panelEditLoop.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelEditLoop.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelEditLoop.HeaderExpandable = false;
            this.panelEditLoop.HeaderExpanded = true;
            this.panelEditLoop.HeaderForeColor = System.Drawing.Color.Black;
            this.panelEditLoop.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelEditLoop.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelEditLoop.HeaderHeight = 30;
            this.panelEditLoop.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelEditLoop.HeaderTitle = "Edit Loop";
            this.panelEditLoop.Location = new System.Drawing.Point(0, 0);
            this.panelEditLoop.Name = "panelEditLoop";
            this.panelEditLoop.Size = new System.Drawing.Size(592, 319);
            this.panelEditLoop.TabIndex = 65;
            // 
            // lblSongTitle
            // 
            this.lblSongTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSongTitle.AntiAliasingEnabled = true;
            this.lblSongTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSongTitle.CustomFontName = "Junction";
            this.lblSongTitle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSongTitle.FontCollection = this.fontCollection;
            this.lblSongTitle.Location = new System.Drawing.Point(3, 89);
            this.lblSongTitle.Name = "lblSongTitle";
            this.lblSongTitle.Size = new System.Drawing.Size(581, 17);
            this.lblSongTitle.TabIndex = 81;
            this.lblSongTitle.Text = "SongTitle";
            // 
            // lblAlbumTitle
            // 
            this.lblAlbumTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAlbumTitle.AntiAliasingEnabled = true;
            this.lblAlbumTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblAlbumTitle.CustomFontName = "Junction";
            this.lblAlbumTitle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlbumTitle.FontCollection = this.fontCollection;
            this.lblAlbumTitle.Location = new System.Drawing.Point(3, 70);
            this.lblAlbumTitle.Name = "lblAlbumTitle";
            this.lblAlbumTitle.Size = new System.Drawing.Size(581, 17);
            this.lblAlbumTitle.TabIndex = 80;
            this.lblAlbumTitle.Text = "AlbumTitle";
            // 
            // lblArtistName
            // 
            this.lblArtistName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblArtistName.AntiAliasingEnabled = true;
            this.lblArtistName.BackColor = System.Drawing.Color.Transparent;
            this.lblArtistName.CustomFontName = "Junction";
            this.lblArtistName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArtistName.FontCollection = this.fontCollection;
            this.lblArtistName.Location = new System.Drawing.Point(3, 52);
            this.lblArtistName.Name = "lblArtistName";
            this.lblArtistName.Size = new System.Drawing.Size(581, 17);
            this.lblArtistName.TabIndex = 79;
            this.lblArtistName.Text = "ArtistName";
            // 
            // lblSong
            // 
            this.lblSong.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSong.AntiAliasingEnabled = true;
            this.lblSong.BackColor = System.Drawing.Color.Transparent;
            this.lblSong.CustomFontName = "Junction";
            this.lblSong.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSong.FontCollection = this.fontCollection;
            this.lblSong.Location = new System.Drawing.Point(3, 34);
            this.lblSong.Name = "lblSong";
            this.lblSong.Size = new System.Drawing.Size(581, 17);
            this.lblSong.TabIndex = 78;
            this.lblSong.Text = "Song (Artist Name / Album Title / Song Title) :";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(6, 132);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(578, 21);
            this.txtName.TabIndex = 68;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblName.AntiAliasingEnabled = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.CustomFontName = "Junction";
            this.lblName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.FontCollection = this.fontCollection;
            this.lblName.Location = new System.Drawing.Point(3, 112);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(586, 17);
            this.lblName.TabIndex = 67;
            this.lblName.Text = "Name :";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AntiAliasingEnabled = true;
            this.btnSave.BorderColor = System.Drawing.Color.DimGray;
            this.btnSave.BorderWidth = 1;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.CustomFontName = "Junction";
            this.btnSave.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnSave.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnSave.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnSave.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSave.Enabled = false;
            this.btnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.FontCollection = this.fontCollection;
            this.btnSave.FontColor = System.Drawing.Color.Black;
            this.btnSave.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnSave.GradientColor2 = System.Drawing.Color.Gray;
            this.btnSave.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnSave.Image = global::MPfm.Properties.Resources.disk;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSave.Location = new System.Drawing.Point(390, 270);
            this.btnSave.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnSave.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnSave.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnSave.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 40);
            this.btnSave.TabIndex = 65;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AntiAliasingEnabled = true;
            this.btnClose.BorderColor = System.Drawing.Color.DimGray;
            this.btnClose.BorderWidth = 1;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.CustomFontName = "Junction";
            this.btnClose.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.FontCollection = this.fontCollection;
            this.btnClose.FontColor = System.Drawing.Color.Black;
            this.btnClose.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnClose.GradientColor2 = System.Drawing.Color.Gray;
            this.btnClose.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnClose.Image = global::MPfm.Properties.Resources.cancel;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnClose.Location = new System.Drawing.Point(488, 270);
            this.btnClose.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnClose.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnClose.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnClose.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 40);
            this.btnClose.TabIndex = 63;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblMarkerA
            // 
            this.lblMarkerA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMarkerA.AntiAliasingEnabled = true;
            this.lblMarkerA.BackColor = System.Drawing.Color.Transparent;
            this.lblMarkerA.CustomFontName = "Junction";
            this.lblMarkerA.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMarkerA.FontCollection = this.fontCollection;
            this.lblMarkerA.Location = new System.Drawing.Point(3, 160);
            this.lblMarkerA.Name = "lblMarkerA";
            this.lblMarkerA.Size = new System.Drawing.Size(586, 17);
            this.lblMarkerA.TabIndex = 82;
            this.lblMarkerA.Text = "Marker A :";
            // 
            // lblMarkerB
            // 
            this.lblMarkerB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMarkerB.AntiAliasingEnabled = true;
            this.lblMarkerB.BackColor = System.Drawing.Color.Transparent;
            this.lblMarkerB.CustomFontName = "Junction";
            this.lblMarkerB.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMarkerB.FontCollection = this.fontCollection;
            this.lblMarkerB.Location = new System.Drawing.Point(3, 210);
            this.lblMarkerB.Name = "lblMarkerB";
            this.lblMarkerB.Size = new System.Drawing.Size(586, 17);
            this.lblMarkerB.TabIndex = 84;
            this.lblMarkerB.Text = "Marker B :";
            // 
            // comboMarkerA
            // 
            this.comboMarkerA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboMarkerA.DisplayMember = "Name";
            this.comboMarkerA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMarkerA.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboMarkerA.FormattingEnabled = true;
            this.comboMarkerA.Location = new System.Drawing.Point(6, 180);
            this.comboMarkerA.Name = "comboMarkerA";
            this.comboMarkerA.Size = new System.Drawing.Size(578, 23);
            this.comboMarkerA.TabIndex = 86;
            this.comboMarkerA.ValueMember = "MarkerId";
            // 
            // comboMarkerB
            // 
            this.comboMarkerB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboMarkerB.DisplayMember = "Name";
            this.comboMarkerB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMarkerB.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboMarkerB.FormattingEnabled = true;
            this.comboMarkerB.Location = new System.Drawing.Point(6, 229);
            this.comboMarkerB.Name = "comboMarkerB";
            this.comboMarkerB.Size = new System.Drawing.Size(578, 23);
            this.comboMarkerB.TabIndex = 87;
            this.comboMarkerB.ValueMember = "MarkerId";
            // 
            // lblLoopLengthValue
            // 
            this.lblLoopLengthValue.AntiAliasingEnabled = true;
            this.lblLoopLengthValue.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthValue.CustomFontName = "";
            this.lblLoopLengthValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthValue.FontCollection = this.fontCollection;
            this.lblLoopLengthValue.Location = new System.Drawing.Point(4, 276);
            this.lblLoopLengthValue.Name = "lblLoopLengthValue";
            this.lblLoopLengthValue.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLengthValue.TabIndex = 75;
            this.lblLoopLengthValue.Text = "0";
            // 
            // lblLoopLength
            // 
            this.lblLoopLength.AntiAliasingEnabled = true;
            this.lblLoopLength.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLength.CustomFontName = "Junction";
            this.lblLoopLength.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLength.FontCollection = this.fontCollection;
            this.lblLoopLength.Location = new System.Drawing.Point(4, 260);
            this.lblLoopLength.Name = "lblLoopLength";
            this.lblLoopLength.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLength.TabIndex = 74;
            this.lblLoopLength.Text = "Loop Length";
            // 
            // frmAddEditLoop
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(592, 319);
            this.ControlBox = false;
            this.Controls.Add(this.panelEditLoop);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(400, 245);
            this.Name = "frmAddEditLoop";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Loop";
            this.panelEditLoop.ResumeLayout(false);
            this.panelEditLoop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MPfm.WindowsControls.Button btnClose;
        private MPfm.WindowsControls.Panel panelEditLoop;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private WindowsControls.Button btnSave;
        private WindowsControls.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private WindowsControls.Label lblSongTitle;
        private WindowsControls.Label lblAlbumTitle;
        private WindowsControls.Label lblArtistName;
        private WindowsControls.Label lblSong;
        private System.Windows.Forms.ComboBox comboMarkerB;
        private System.Windows.Forms.ComboBox comboMarkerA;
        private WindowsControls.Label lblMarkerB;
        private WindowsControls.Label lblMarkerA;
        private WindowsControls.Label lblLoopLengthValue;
        private WindowsControls.Label lblLoopLength;        
    }
}