namespace MPfm
{
    partial class frmAddEditMarker
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
            this.panelEditMarker = new MPfm.WindowsControls.Panel();
            this.lblSongTitle = new MPfm.WindowsControls.Label();
            this.lblAlbumTitle = new MPfm.WindowsControls.Label();
            this.lblArtistName = new MPfm.WindowsControls.Label();
            this.lblSong = new MPfm.WindowsControls.Label();
            this.lblPositionPCMBytesValue = new MPfm.WindowsControls.Label();
            this.lblPositionPCMBytes = new MPfm.WindowsControls.Label();
            this.btnGoTo = new MPfm.WindowsControls.Button();
            this.btnPunchIn = new MPfm.WindowsControls.Button();
            this.lblPositionPCMValue = new MPfm.WindowsControls.Label();
            this.lblPositionPCM = new MPfm.WindowsControls.Label();
            this.lblPosition = new MPfm.WindowsControls.Label();
            this.txtPosition = new System.Windows.Forms.MaskedTextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new MPfm.WindowsControls.Label();
            this.btnSave = new MPfm.WindowsControls.Button();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.panelEditMarker.SuspendLayout();
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
            // panelEditMarker
            // 
            this.panelEditMarker.AntiAliasingEnabled = true;
            this.panelEditMarker.Controls.Add(this.lblSongTitle);
            this.panelEditMarker.Controls.Add(this.lblAlbumTitle);
            this.panelEditMarker.Controls.Add(this.lblArtistName);
            this.panelEditMarker.Controls.Add(this.lblSong);
            this.panelEditMarker.Controls.Add(this.lblPositionPCMBytesValue);
            this.panelEditMarker.Controls.Add(this.lblPositionPCMBytes);
            this.panelEditMarker.Controls.Add(this.btnGoTo);
            this.panelEditMarker.Controls.Add(this.btnPunchIn);
            this.panelEditMarker.Controls.Add(this.lblPositionPCMValue);
            this.panelEditMarker.Controls.Add(this.lblPositionPCM);
            this.panelEditMarker.Controls.Add(this.lblPosition);
            this.panelEditMarker.Controls.Add(this.txtPosition);
            this.panelEditMarker.Controls.Add(this.txtName);
            this.panelEditMarker.Controls.Add(this.lblName);
            this.panelEditMarker.Controls.Add(this.btnSave);
            this.panelEditMarker.Controls.Add(this.btnClose);
            this.panelEditMarker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEditMarker.ExpandedHeight = 200;
            this.panelEditMarker.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelEditMarker.FontCollection = this.fontCollection;
            this.panelEditMarker.GradientColor1 = System.Drawing.Color.Silver;
            this.panelEditMarker.GradientColor2 = System.Drawing.Color.Gray;
            this.panelEditMarker.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelEditMarker.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelEditMarker.HeaderExpandable = false;
            this.panelEditMarker.HeaderExpanded = true;
            this.panelEditMarker.HeaderForeColor = System.Drawing.Color.Black;
            this.panelEditMarker.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelEditMarker.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelEditMarker.HeaderHeight = 30;
            this.panelEditMarker.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelEditMarker.HeaderTitle = "Edit Marker";
            this.panelEditMarker.Location = new System.Drawing.Point(0, 0);
            this.panelEditMarker.Name = "panelEditMarker";
            this.panelEditMarker.Size = new System.Drawing.Size(585, 301);
            this.panelEditMarker.TabIndex = 65;
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
            this.lblSongTitle.Size = new System.Drawing.Size(574, 17);
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
            this.lblAlbumTitle.Size = new System.Drawing.Size(574, 17);
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
            this.lblArtistName.Size = new System.Drawing.Size(574, 17);
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
            this.lblSong.Size = new System.Drawing.Size(574, 17);
            this.lblSong.TabIndex = 78;
            this.lblSong.Text = "Song (Artist Name / Album Title / Song Title) :";
            // 
            // lblPositionPCMBytesValue
            // 
            this.lblPositionPCMBytesValue.AntiAliasingEnabled = true;
            this.lblPositionPCMBytesValue.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionPCMBytesValue.CustomFontName = "";
            this.lblPositionPCMBytesValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionPCMBytesValue.FontCollection = this.fontCollection;
            this.lblPositionPCMBytesValue.Location = new System.Drawing.Point(3, 263);
            this.lblPositionPCMBytesValue.Name = "lblPositionPCMBytesValue";
            this.lblPositionPCMBytesValue.Size = new System.Drawing.Size(89, 17);
            this.lblPositionPCMBytesValue.TabIndex = 77;
            this.lblPositionPCMBytesValue.Text = "0";
            // 
            // lblPositionPCMBytes
            // 
            this.lblPositionPCMBytes.AntiAliasingEnabled = true;
            this.lblPositionPCMBytes.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionPCMBytes.CustomFontName = "Junction";
            this.lblPositionPCMBytes.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionPCMBytes.FontCollection = this.fontCollection;
            this.lblPositionPCMBytes.Location = new System.Drawing.Point(3, 247);
            this.lblPositionPCMBytes.Name = "lblPositionPCMBytes";
            this.lblPositionPCMBytes.Size = new System.Drawing.Size(89, 17);
            this.lblPositionPCMBytes.TabIndex = 76;
            this.lblPositionPCMBytes.Text = "PCM (Bytes)";
            // 
            // btnGoTo
            // 
            this.btnGoTo.AntiAliasingEnabled = true;
            this.btnGoTo.BorderColor = System.Drawing.Color.DimGray;
            this.btnGoTo.BorderWidth = 1;
            this.btnGoTo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGoTo.CustomFontName = "Junction";
            this.btnGoTo.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnGoTo.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnGoTo.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnGoTo.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnGoTo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGoTo.FontCollection = this.fontCollection;
            this.btnGoTo.FontColor = System.Drawing.Color.Black;
            this.btnGoTo.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnGoTo.GradientColor2 = System.Drawing.Color.Gray;
            this.btnGoTo.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnGoTo.Image = global::MPfm.Properties.Resources.arrow_right;
            this.btnGoTo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGoTo.Location = new System.Drawing.Point(211, 178);
            this.btnGoTo.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnGoTo.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnGoTo.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnGoTo.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new System.Drawing.Size(60, 30);
            this.btnGoTo.TabIndex = 75;
            this.btnGoTo.Text = "Go to";
            this.btnGoTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGoTo.UseVisualStyleBackColor = true;
            this.btnGoTo.Click += new System.EventHandler(this.btnGoTo_Click);
            // 
            // btnPunchIn
            // 
            this.btnPunchIn.AntiAliasingEnabled = true;
            this.btnPunchIn.BorderColor = System.Drawing.Color.DimGray;
            this.btnPunchIn.BorderWidth = 1;
            this.btnPunchIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPunchIn.CustomFontName = "Junction";
            this.btnPunchIn.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnPunchIn.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnPunchIn.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnPunchIn.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnPunchIn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPunchIn.FontCollection = this.fontCollection;
            this.btnPunchIn.FontColor = System.Drawing.Color.Black;
            this.btnPunchIn.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnPunchIn.GradientColor2 = System.Drawing.Color.Gray;
            this.btnPunchIn.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnPunchIn.Image = global::MPfm.Properties.Resources.time;
            this.btnPunchIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPunchIn.Location = new System.Drawing.Point(129, 178);
            this.btnPunchIn.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnPunchIn.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnPunchIn.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnPunchIn.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnPunchIn.Name = "btnPunchIn";
            this.btnPunchIn.Size = new System.Drawing.Size(76, 30);
            this.btnPunchIn.TabIndex = 74;
            this.btnPunchIn.Text = "Punch in";
            this.btnPunchIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPunchIn.UseVisualStyleBackColor = true;
            this.btnPunchIn.Click += new System.EventHandler(this.btnPunchIn_Click);
            // 
            // lblPositionPCMValue
            // 
            this.lblPositionPCMValue.AntiAliasingEnabled = true;
            this.lblPositionPCMValue.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionPCMValue.CustomFontName = "";
            this.lblPositionPCMValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionPCMValue.FontCollection = this.fontCollection;
            this.lblPositionPCMValue.Location = new System.Drawing.Point(3, 230);
            this.lblPositionPCMValue.Name = "lblPositionPCMValue";
            this.lblPositionPCMValue.Size = new System.Drawing.Size(89, 17);
            this.lblPositionPCMValue.TabIndex = 73;
            this.lblPositionPCMValue.Text = "0";
            // 
            // lblPositionPCM
            // 
            this.lblPositionPCM.AntiAliasingEnabled = true;
            this.lblPositionPCM.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionPCM.CustomFontName = "Junction";
            this.lblPositionPCM.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionPCM.FontCollection = this.fontCollection;
            this.lblPositionPCM.Location = new System.Drawing.Point(3, 214);
            this.lblPositionPCM.Name = "lblPositionPCM";
            this.lblPositionPCM.Size = new System.Drawing.Size(89, 17);
            this.lblPositionPCM.TabIndex = 72;
            this.lblPositionPCM.Text = "PCM";
            // 
            // lblPosition
            // 
            this.lblPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPosition.AntiAliasingEnabled = true;
            this.lblPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblPosition.CustomFontName = "Junction";
            this.lblPosition.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosition.FontCollection = this.fontCollection;
            this.lblPosition.Location = new System.Drawing.Point(3, 158);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(579, 17);
            this.lblPosition.TabIndex = 70;
            this.lblPosition.Text = "Position :";
            // 
            // txtPosition
            // 
            this.txtPosition.Font = new System.Drawing.Font("Droid Sans Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPosition.Location = new System.Drawing.Point(6, 178);
            this.txtPosition.Mask = "0:00.000";
            this.txtPosition.Name = "txtPosition";
            this.txtPosition.Size = new System.Drawing.Size(115, 30);
            this.txtPosition.TabIndex = 69;
            this.txtPosition.Text = "000000";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(6, 132);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(571, 21);
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
            this.lblName.Size = new System.Drawing.Size(579, 17);
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
            this.btnSave.Location = new System.Drawing.Point(383, 252);
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
            this.btnClose.Location = new System.Drawing.Point(481, 252);
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
            // frmAddEditMarker
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(585, 301);
            this.ControlBox = false;
            this.Controls.Add(this.panelEditMarker);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(400, 245);
            this.Name = "frmAddEditMarker";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Marker";
            this.panelEditMarker.ResumeLayout(false);
            this.panelEditMarker.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MPfm.WindowsControls.Button btnClose;
        private MPfm.WindowsControls.Panel panelEditMarker;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private WindowsControls.Button btnSave;
        private WindowsControls.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private WindowsControls.Label lblPosition;
        private System.Windows.Forms.MaskedTextBox txtPosition;
        private WindowsControls.Label lblPositionPCMValue;
        private WindowsControls.Label lblPositionPCM;
        private WindowsControls.Button btnPunchIn;
        private WindowsControls.Button btnGoTo;
        private WindowsControls.Label lblPositionPCMBytesValue;
        private WindowsControls.Label lblPositionPCMBytes;
        private WindowsControls.Label lblSongTitle;
        private WindowsControls.Label lblAlbumTitle;
        private WindowsControls.Label lblArtistName;
        private WindowsControls.Label lblSong;        
    }
}