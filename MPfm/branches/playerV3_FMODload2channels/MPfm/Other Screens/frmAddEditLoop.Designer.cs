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
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.panelEditLoop = new MPfm.WindowsControls.Panel();
            this.waveForm = new MPfm.WindowsControls.WaveFormMarkersLoops();
            this.panelWarning = new MPfm.WindowsControls.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblWarning = new MPfm.WindowsControls.Label();
            this.lblMarkerBPosition = new MPfm.WindowsControls.Label();
            this.lblMarkerAPosition = new MPfm.WindowsControls.Label();
            this.lblLoopLengthValue = new MPfm.WindowsControls.Label();
            this.comboMarkerB = new System.Windows.Forms.ComboBox();
            this.lblLoopLength = new MPfm.WindowsControls.Label();
            this.comboMarkerA = new System.Windows.Forms.ComboBox();
            this.lblMarkerB = new MPfm.WindowsControls.Label();
            this.lblMarkerA = new MPfm.WindowsControls.Label();
            this.lblSongTitle = new MPfm.WindowsControls.Label();
            this.lblAlbumTitle = new MPfm.WindowsControls.Label();
            this.lblArtistName = new MPfm.WindowsControls.Label();
            this.lblSong = new MPfm.WindowsControls.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new MPfm.WindowsControls.Label();
            this.btnSave = new MPfm.WindowsControls.Button();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.lblLoopLengthPCMValue = new MPfm.WindowsControls.Label();
            this.lblLoopLengthPCM = new MPfm.WindowsControls.Label();
            this.lblLoopLengthPCMBytesValue = new MPfm.WindowsControls.Label();
            this.lblLoopLengthPCMBytes = new MPfm.WindowsControls.Label();
            this.panelEditLoop.SuspendLayout();
            this.panelWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // fontCollection
            // 
            customFont6.AssemblyPath = "MPfm.Fonts.dll";
            customFont6.Name = "LeagueGothic";
            customFont6.ResourceName = "MPfm.Fonts.LeagueGothic.ttf";
            customFont7.AssemblyPath = "MPfm.Fonts.dll";
            customFont7.Name = "Junction";
            customFont7.ResourceName = "MPfm.Fonts.Junction.ttf";
            customFont8.AssemblyPath = "MPfm.Fonts.dll";
            customFont8.Name = "Nobile";
            customFont8.ResourceName = "MPfm.Fonts.nobile.ttf";
            customFont9.AssemblyPath = "MPfm.Fonts.dll";
            customFont9.Name = "TitilliumText22L Lt";
            customFont9.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            customFont10.AssemblyPath = "MPfm.Fonts.dll";
            customFont10.Name = "Museo Sans 500";
            customFont10.ResourceName = "MPfm.Fonts.MuseoSans_500.ttf";
            this.fontCollection.Fonts.Add(customFont6);
            this.fontCollection.Fonts.Add(customFont7);
            this.fontCollection.Fonts.Add(customFont8);
            this.fontCollection.Fonts.Add(customFont9);
            this.fontCollection.Fonts.Add(customFont10);
            // 
            // panelEditLoop
            // 
            this.panelEditLoop.AntiAliasingEnabled = true;
            this.panelEditLoop.Controls.Add(this.panelWarning);
            this.panelEditLoop.Controls.Add(this.lblLoopLengthPCMBytesValue);
            this.panelEditLoop.Controls.Add(this.lblLoopLengthPCMBytes);
            this.panelEditLoop.Controls.Add(this.lblLoopLengthPCMValue);
            this.panelEditLoop.Controls.Add(this.lblLoopLengthPCM);
            this.panelEditLoop.Controls.Add(this.waveForm);
            this.panelEditLoop.Controls.Add(this.lblMarkerBPosition);
            this.panelEditLoop.Controls.Add(this.lblMarkerAPosition);
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
            this.panelEditLoop.Size = new System.Drawing.Size(592, 391);
            this.panelEditLoop.TabIndex = 65;
            // 
            // waveForm
            // 
            this.waveForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waveForm.AutoScrollWithCursor = true;
            this.waveForm.BorderColor = System.Drawing.Color.Empty;
            this.waveForm.BorderWidth = 0;
            this.waveForm.CurrentPositionMS = ((uint)(0u));
            this.waveForm.CurrentPositionPCMBytes = ((uint)(0u));
            this.waveForm.CursorColor = System.Drawing.Color.RoyalBlue;
            this.waveForm.CustomFontName = "BPmono";
            this.waveForm.DisplayCurrentPosition = true;
            this.waveForm.DisplayType = MPfm.WindowsControls.WaveFormDisplayType.Stereo;
            this.waveForm.Font = new System.Drawing.Font("Arial", 8F);
            this.waveForm.FontCollection = this.fontCollection;
            this.waveForm.GradientColor1 = System.Drawing.Color.Black;
            this.waveForm.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.waveForm.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.waveForm.Location = new System.Drawing.Point(6, 258);
            this.waveForm.Name = "waveForm";
            this.waveForm.PeakFileDirectory = "C:\\Users\\Animal Mother\\AppData\\Local\\Microsoft\\VisualStudio\\10.0\\ProjectAssemblie" +
    "s\\om-0gycd01\\Peak Files\\";
            this.waveForm.Size = new System.Drawing.Size(578, 77);
            this.waveForm.TabIndex = 93;
            this.waveForm.TotalMS = ((uint)(0u));
            this.waveForm.TotalPCMBytes = ((uint)(0u));
            this.waveForm.WaveFormColor = System.Drawing.Color.Yellow;
            this.waveForm.Zoom = 100F;
            // 
            // panelWarning
            // 
            this.panelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelWarning.AntiAliasingEnabled = true;
            this.panelWarning.Controls.Add(this.pictureBox1);
            this.panelWarning.Controls.Add(this.lblWarning);
            this.panelWarning.ExpandedHeight = 200;
            this.panelWarning.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelWarning.FontCollection = this.fontCollection;
            this.panelWarning.GradientColor1 = System.Drawing.Color.LemonChiffon;
            this.panelWarning.GradientColor2 = System.Drawing.Color.Gold;
            this.panelWarning.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelWarning.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelWarning.HeaderExpandable = false;
            this.panelWarning.HeaderExpanded = true;
            this.panelWarning.HeaderForeColor = System.Drawing.Color.Black;
            this.panelWarning.HeaderGradientColor1 = System.Drawing.Color.LemonChiffon;
            this.panelWarning.HeaderGradientColor2 = System.Drawing.Color.Gold;
            this.panelWarning.HeaderHeight = 18;
            this.panelWarning.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelWarning.HeaderTitle = "Warning";
            this.panelWarning.Location = new System.Drawing.Point(6, 341);
            this.panelWarning.Name = "panelWarning";
            this.panelWarning.Size = new System.Drawing.Size(382, 44);
            this.panelWarning.TabIndex = 90;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::MPfm.Properties.Resources.error;
            this.pictureBox1.Location = new System.Drawing.Point(6, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(21, 23);
            this.pictureBox1.TabIndex = 92;
            this.pictureBox1.TabStop = false;
            // 
            // lblWarning
            // 
            this.lblWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWarning.AntiAliasingEnabled = true;
            this.lblWarning.BackColor = System.Drawing.Color.Transparent;
            this.lblWarning.CustomFontName = "Junction";
            this.lblWarning.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.FontCollection = this.fontCollection;
            this.lblWarning.Location = new System.Drawing.Point(24, 20);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(334, 17);
            this.lblWarning.TabIndex = 91;
            this.lblWarning.Text = "The loop length must be positive.";
            // 
            // lblMarkerBPosition
            // 
            this.lblMarkerBPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMarkerBPosition.AntiAliasingEnabled = true;
            this.lblMarkerBPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblMarkerBPosition.CustomFontName = "";
            this.lblMarkerBPosition.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMarkerBPosition.FontCollection = this.fontCollection;
            this.lblMarkerBPosition.Location = new System.Drawing.Point(495, 209);
            this.lblMarkerBPosition.Name = "lblMarkerBPosition";
            this.lblMarkerBPosition.Size = new System.Drawing.Size(89, 17);
            this.lblMarkerBPosition.TabIndex = 89;
            this.lblMarkerBPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMarkerAPosition
            // 
            this.lblMarkerAPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMarkerAPosition.AntiAliasingEnabled = true;
            this.lblMarkerAPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblMarkerAPosition.CustomFontName = "";
            this.lblMarkerAPosition.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMarkerAPosition.FontCollection = this.fontCollection;
            this.lblMarkerAPosition.Location = new System.Drawing.Point(495, 161);
            this.lblMarkerAPosition.Name = "lblMarkerAPosition";
            this.lblMarkerAPosition.Size = new System.Drawing.Size(89, 17);
            this.lblMarkerAPosition.TabIndex = 88;
            this.lblMarkerAPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblLoopLengthValue
            // 
            this.lblLoopLengthValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthValue.AntiAliasingEnabled = true;
            this.lblLoopLengthValue.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthValue.CustomFontName = "";
            this.lblLoopLengthValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthValue.FontCollection = this.fontCollection;
            this.lblLoopLengthValue.Location = new System.Drawing.Point(2, 362);
            this.lblLoopLengthValue.Name = "lblLoopLengthValue";
            this.lblLoopLengthValue.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLengthValue.TabIndex = 75;
            this.lblLoopLengthValue.Text = "0";
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
            this.comboMarkerB.SelectedIndexChanged += new System.EventHandler(this.comboMarkerB_SelectedIndexChanged);
            // 
            // lblLoopLength
            // 
            this.lblLoopLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLength.AntiAliasingEnabled = true;
            this.lblLoopLength.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLength.CustomFontName = "Junction";
            this.lblLoopLength.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLength.FontCollection = this.fontCollection;
            this.lblLoopLength.Location = new System.Drawing.Point(2, 345);
            this.lblLoopLength.Name = "lblLoopLength";
            this.lblLoopLength.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLength.TabIndex = 74;
            this.lblLoopLength.Text = "Loop Length";
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
            this.comboMarkerA.SelectedIndexChanged += new System.EventHandler(this.comboMarkerA_SelectedIndexChanged);
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
            this.btnSave.Location = new System.Drawing.Point(394, 341);
            this.btnSave.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnSave.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnSave.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnSave.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 45);
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
            this.btnClose.Location = new System.Drawing.Point(492, 341);
            this.btnClose.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnClose.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnClose.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnClose.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 45);
            this.btnClose.TabIndex = 63;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblLoopLengthPCMValue
            // 
            this.lblLoopLengthPCMValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthPCMValue.AntiAliasingEnabled = true;
            this.lblLoopLengthPCMValue.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthPCMValue.CustomFontName = "";
            this.lblLoopLengthPCMValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthPCMValue.FontCollection = this.fontCollection;
            this.lblLoopLengthPCMValue.Location = new System.Drawing.Point(107, 362);
            this.lblLoopLengthPCMValue.Name = "lblLoopLengthPCMValue";
            this.lblLoopLengthPCMValue.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLengthPCMValue.TabIndex = 95;
            this.lblLoopLengthPCMValue.Text = "0";
            // 
            // lblLoopLengthPCM
            // 
            this.lblLoopLengthPCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthPCM.AntiAliasingEnabled = true;
            this.lblLoopLengthPCM.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthPCM.CustomFontName = "Junction";
            this.lblLoopLengthPCM.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthPCM.FontCollection = this.fontCollection;
            this.lblLoopLengthPCM.Location = new System.Drawing.Point(107, 345);
            this.lblLoopLengthPCM.Name = "lblLoopLengthPCM";
            this.lblLoopLengthPCM.Size = new System.Drawing.Size(130, 17);
            this.lblLoopLengthPCM.TabIndex = 94;
            this.lblLoopLengthPCM.Text = "Loop Length (samples)";
            // 
            // lblLoopLengthPCMBytesValue
            // 
            this.lblLoopLengthPCMBytesValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthPCMBytesValue.AntiAliasingEnabled = true;
            this.lblLoopLengthPCMBytesValue.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthPCMBytesValue.CustomFontName = "";
            this.lblLoopLengthPCMBytesValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthPCMBytesValue.FontCollection = this.fontCollection;
            this.lblLoopLengthPCMBytesValue.Location = new System.Drawing.Point(255, 362);
            this.lblLoopLengthPCMBytesValue.Name = "lblLoopLengthPCMBytesValue";
            this.lblLoopLengthPCMBytesValue.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLengthPCMBytesValue.TabIndex = 97;
            this.lblLoopLengthPCMBytesValue.Text = "0";
            // 
            // lblLoopLengthPCMBytes
            // 
            this.lblLoopLengthPCMBytes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthPCMBytes.AntiAliasingEnabled = true;
            this.lblLoopLengthPCMBytes.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthPCMBytes.CustomFontName = "Junction";
            this.lblLoopLengthPCMBytes.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthPCMBytes.FontCollection = this.fontCollection;
            this.lblLoopLengthPCMBytes.Location = new System.Drawing.Point(255, 345);
            this.lblLoopLengthPCMBytes.Name = "lblLoopLengthPCMBytes";
            this.lblLoopLengthPCMBytes.Size = new System.Drawing.Size(110, 17);
            this.lblLoopLengthPCMBytes.TabIndex = 96;
            this.lblLoopLengthPCMBytes.Text = "Loop Length (bytes)";
            // 
            // frmAddEditLoop
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(592, 391);
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
            this.panelWarning.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private WindowsControls.Label lblMarkerBPosition;
        private WindowsControls.Label lblMarkerAPosition;
        private WindowsControls.Panel panelWarning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private WindowsControls.Label lblWarning;
        public WindowsControls.WaveFormMarkersLoops waveForm;
        private WindowsControls.Label lblLoopLengthPCMBytesValue;
        private WindowsControls.Label lblLoopLengthPCMBytes;
        private WindowsControls.Label lblLoopLengthPCMValue;
        private WindowsControls.Label lblLoopLengthPCM;        
    }
}