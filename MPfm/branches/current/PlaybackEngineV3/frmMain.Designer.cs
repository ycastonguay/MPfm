namespace PlaybackEngineV3
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.label1 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCurrentArtist = new System.Windows.Forms.Label();
            this.lblCurrentAlbum = new System.Windows.Forms.Label();
            this.lblCurrentTitle = new System.Windows.Forms.Label();
            this.lblCurrentPath = new System.Windows.Forms.Label();
            this.listBoxPlaylist = new System.Windows.Forms.ListBox();
            this.groupPlaylist = new System.Windows.Forms.GroupBox();
            this.groupCurrentlyPlaying = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.lblCurrentLengthPCM = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblCurrentLength = new System.Windows.Forms.Label();
            this.lblCurrentPositionPCM = new System.Windows.Forms.Label();
            this.lblCurrentPosition = new System.Windows.Forms.Label();
            this.trackPosition = new System.Windows.Forms.TrackBar();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.dialogFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timerUpdateSoundSystem = new System.Windows.Forms.Timer(this.components);
            this.trackVolume = new System.Windows.Forms.TrackBar();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lblVolumeValue = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupOptions.SuspendLayout();
            this.groupPlaylist.SuspendLayout();
            this.groupCurrentlyPlaying.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(243, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "Copyright © 2011 Yanick Castonguay";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(62, 27);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(54, 12);
            this.lblVersion.TabIndex = 17;
            this.lblVersion.Text = "Version";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Lucida Console", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(60, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(307, 15);
            this.lblTitle.TabIndex = 14;
            this.lblTitle.Text = "MPfm - Playback Engine V4 Demo";
            // 
            // groupOptions
            // 
            this.groupOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupOptions.Controls.Add(this.lblPath);
            this.groupOptions.Controls.Add(this.btnBrowse);
            this.groupOptions.Controls.Add(this.txtPath);
            this.groupOptions.Location = new System.Drawing.Point(5, 61);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(814, 62);
            this.groupOptions.TabIndex = 15;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Options";
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(6, 18);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(320, 12);
            this.lblPath.TabIndex = 3;
            this.lblPath.Text = "Select a folder containing an album to play :";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.Image")));
            this.btnBrowse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowse.Location = new System.Drawing.Point(703, 30);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(107, 27);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse ...";
            this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPath.Location = new System.Drawing.Point(7, 34);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(690, 20);
            this.txtPath.TabIndex = 0;
            this.txtPath.Text = "C:\\";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(130, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 9);
            this.label2.TabIndex = 21;
            this.label2.Text = "Album";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(130, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(245, 9);
            this.label3.TabIndex = 20;
            this.label3.Text = "Artist";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Gray;
            this.label4.Location = new System.Drawing.Point(130, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 9);
            this.label4.TabIndex = 22;
            this.label4.Text = "Title";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Gray;
            this.label5.Location = new System.Drawing.Point(130, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 9);
            this.label5.TabIndex = 23;
            this.label5.Text = "Path";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentArtist
            // 
            this.lblCurrentArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentArtist.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentArtist.ForeColor = System.Drawing.Color.White;
            this.lblCurrentArtist.Location = new System.Drawing.Point(130, 15);
            this.lblCurrentArtist.Name = "lblCurrentArtist";
            this.lblCurrentArtist.Size = new System.Drawing.Size(394, 11);
            this.lblCurrentArtist.TabIndex = 24;
            this.lblCurrentArtist.Text = "[Artist]";
            this.lblCurrentArtist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentAlbum
            // 
            this.lblCurrentAlbum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentAlbum.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentAlbum.ForeColor = System.Drawing.Color.White;
            this.lblCurrentAlbum.Location = new System.Drawing.Point(130, 41);
            this.lblCurrentAlbum.Name = "lblCurrentAlbum";
            this.lblCurrentAlbum.Size = new System.Drawing.Size(400, 11);
            this.lblCurrentAlbum.TabIndex = 25;
            this.lblCurrentAlbum.Text = "[Album]";
            this.lblCurrentAlbum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentTitle
            // 
            this.lblCurrentTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentTitle.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTitle.ForeColor = System.Drawing.Color.White;
            this.lblCurrentTitle.Location = new System.Drawing.Point(130, 67);
            this.lblCurrentTitle.Name = "lblCurrentTitle";
            this.lblCurrentTitle.Size = new System.Drawing.Size(394, 11);
            this.lblCurrentTitle.TabIndex = 26;
            this.lblCurrentTitle.Text = "[Title]";
            this.lblCurrentTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentPath
            // 
            this.lblCurrentPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentPath.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentPath.ForeColor = System.Drawing.Color.White;
            this.lblCurrentPath.Location = new System.Drawing.Point(130, 93);
            this.lblCurrentPath.Name = "lblCurrentPath";
            this.lblCurrentPath.Size = new System.Drawing.Size(400, 11);
            this.lblCurrentPath.TabIndex = 27;
            this.lblCurrentPath.Text = "[Path]";
            this.lblCurrentPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listBoxPlaylist
            // 
            this.listBoxPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxPlaylist.FormattingEnabled = true;
            this.listBoxPlaylist.ItemHeight = 12;
            this.listBoxPlaylist.Location = new System.Drawing.Point(6, 16);
            this.listBoxPlaylist.Name = "listBoxPlaylist";
            this.listBoxPlaylist.Size = new System.Drawing.Size(801, 100);
            this.listBoxPlaylist.TabIndex = 28;
            // 
            // groupPlaylist
            // 
            this.groupPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPlaylist.Controls.Add(this.listBoxPlaylist);
            this.groupPlaylist.Location = new System.Drawing.Point(5, 264);
            this.groupPlaylist.Name = "groupPlaylist";
            this.groupPlaylist.Size = new System.Drawing.Size(814, 133);
            this.groupPlaylist.TabIndex = 16;
            this.groupPlaylist.TabStop = false;
            this.groupPlaylist.Text = "Playlist";
            // 
            // groupCurrentlyPlaying
            // 
            this.groupCurrentlyPlaying.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupCurrentlyPlaying.Controls.Add(this.panel1);
            this.groupCurrentlyPlaying.Location = new System.Drawing.Point(5, 129);
            this.groupCurrentlyPlaying.Name = "groupCurrentlyPlaying";
            this.groupCurrentlyPlaying.Size = new System.Drawing.Size(814, 129);
            this.groupCurrentlyPlaying.TabIndex = 29;
            this.groupCurrentlyPlaying.TabStop = false;
            this.groupCurrentlyPlaying.Text = "Currently playing";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.lblCurrentLengthPCM);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.lblCurrentLength);
            this.panel1.Controls.Add(this.lblCurrentPositionPCM);
            this.panel1.Controls.Add(this.lblCurrentPosition);
            this.panel1.Controls.Add(this.trackPosition);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lblCurrentPath);
            this.panel1.Controls.Add(this.lblCurrentArtist);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.lblCurrentTitle);
            this.panel1.Controls.Add(this.lblCurrentAlbum);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Location = new System.Drawing.Point(6, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(801, 111);
            this.panel1.TabIndex = 28;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Silver;
            this.label10.Location = new System.Drawing.Point(654, 76);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 9);
            this.label10.TabIndex = 37;
            this.label10.Text = "PCM";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurrentLengthPCM
            // 
            this.lblCurrentLengthPCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentLengthPCM.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentLengthPCM.ForeColor = System.Drawing.Color.White;
            this.lblCurrentLengthPCM.Location = new System.Drawing.Point(698, 75);
            this.lblCurrentLengthPCM.Name = "lblCurrentLengthPCM";
            this.lblCurrentLengthPCM.Size = new System.Drawing.Size(92, 12);
            this.lblCurrentLengthPCM.TabIndex = 38;
            this.lblCurrentLengthPCM.Text = "[Length]";
            this.lblCurrentLengthPCM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Gray;
            this.label8.Location = new System.Drawing.Point(666, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 9);
            this.label8.TabIndex = 35;
            this.label8.Text = "Length";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Gray;
            this.label7.Location = new System.Drawing.Point(536, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(124, 9);
            this.label7.TabIndex = 34;
            this.label7.Text = "Position";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Silver;
            this.label6.Location = new System.Drawing.Point(536, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(221, 9);
            this.label6.TabIndex = 33;
            this.label6.Text = "Audio File Position";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentLength
            // 
            this.lblCurrentLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentLength.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentLength.ForeColor = System.Drawing.Color.White;
            this.lblCurrentLength.Location = new System.Drawing.Point(676, 59);
            this.lblCurrentLength.Name = "lblCurrentLength";
            this.lblCurrentLength.Size = new System.Drawing.Size(114, 12);
            this.lblCurrentLength.TabIndex = 32;
            this.lblCurrentLength.Text = "[Length]";
            this.lblCurrentLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCurrentPositionPCM
            // 
            this.lblCurrentPositionPCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentPositionPCM.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentPositionPCM.ForeColor = System.Drawing.Color.White;
            this.lblCurrentPositionPCM.Location = new System.Drawing.Point(536, 75);
            this.lblCurrentPositionPCM.Name = "lblCurrentPositionPCM";
            this.lblCurrentPositionPCM.Size = new System.Drawing.Size(115, 12);
            this.lblCurrentPositionPCM.TabIndex = 31;
            this.lblCurrentPositionPCM.Text = "[Position]";
            this.lblCurrentPositionPCM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentPosition
            // 
            this.lblCurrentPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentPosition.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentPosition.ForeColor = System.Drawing.Color.White;
            this.lblCurrentPosition.Location = new System.Drawing.Point(536, 59);
            this.lblCurrentPosition.Name = "lblCurrentPosition";
            this.lblCurrentPosition.Size = new System.Drawing.Size(147, 12);
            this.lblCurrentPosition.TabIndex = 29;
            this.lblCurrentPosition.Text = "[Position]";
            this.lblCurrentPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trackPosition
            // 
            this.trackPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackPosition.LargeChange = 50;
            this.trackPosition.Location = new System.Drawing.Point(530, 19);
            this.trackPosition.Maximum = 1000;
            this.trackPosition.Name = "trackPosition";
            this.trackPosition.Size = new System.Drawing.Size(268, 45);
            this.trackPosition.SmallChange = 25;
            this.trackPosition.TabIndex = 30;
            this.trackPosition.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackPosition.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackPosition_MouseDown);
            this.trackPosition.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackPosition_MouseUp);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(-58, -72);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(203, 170);
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            // 
            // dialogFolderBrowser
            // 
            this.dialogFolderBrowser.Description = "Select a folder with MP3 files to play.";
            this.dialogFolderBrowser.SelectedPath = "F:\\Flac\\Nine Inch Nails";
            // 
            // btnPause
            // 
            this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPause.Enabled = false;
            this.btnPause.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPause.Location = new System.Drawing.Point(612, 5);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(69, 27);
            this.btnPause.TabIndex = 31;
            this.btnPause.Text = "Pause";
            this.btnPause.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(756, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(63, 27);
            this.btnExit.TabIndex = 30;
            this.btnExit.Text = "Exit";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.Location = new System.Drawing.Point(687, 5);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(63, 27);
            this.btnStop.TabIndex = 29;
            this.btnStop.Text = "Stop";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlay.Enabled = false;
            this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
            this.btnPlay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlay.Location = new System.Drawing.Point(541, 5);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(63, 27);
            this.btnPlay.TabIndex = 4;
            this.btnPlay.Text = "Play";
            this.btnPlay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(5, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(60, 50);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // timerUpdateSoundSystem
            // 
            this.timerUpdateSoundSystem.Enabled = true;
            this.timerUpdateSoundSystem.Interval = 10;
            this.timerUpdateSoundSystem.Tick += new System.EventHandler(this.timerUpdateSoundSystem_Tick);
            // 
            // trackVolume
            // 
            this.trackVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackVolume.LargeChange = 10;
            this.trackVolume.Location = new System.Drawing.Point(662, 38);
            this.trackVolume.Maximum = 100;
            this.trackVolume.Name = "trackVolume";
            this.trackVolume.Size = new System.Drawing.Size(157, 45);
            this.trackVolume.SmallChange = 25;
            this.trackVolume.TabIndex = 32;
            this.trackVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackVolume.Value = 90;
            this.trackVolume.Scroll += new System.EventHandler(this.trackVolume_Scroll);
            // 
            // lblVolume
            // 
            this.lblVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVolume.AutoSize = true;
            this.lblVolume.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolume.Location = new System.Drawing.Point(608, 36);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(54, 12);
            this.lblVolume.TabIndex = 33;
            this.lblVolume.Text = "Volume:";
            // 
            // lblVolumeValue
            // 
            this.lblVolumeValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVolumeValue.AutoSize = true;
            this.lblVolumeValue.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolumeValue.Location = new System.Drawing.Point(632, 49);
            this.lblVolumeValue.Name = "lblVolumeValue";
            this.lblVolumeValue.Size = new System.Drawing.Size(29, 11);
            this.lblVolumeValue.TabIndex = 34;
            this.lblVolumeValue.Text = "90%";
            this.lblVolumeValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 400);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(825, 22);
            this.statusStrip.TabIndex = 35;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(825, 422);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.lblVolumeValue);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.trackVolume);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.groupCurrentlyPlaying);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.groupPlaylist);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "MPfm - Playback Engine V3 Demo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupOptions.ResumeLayout(false);
            this.groupOptions.PerformLayout();
            this.groupPlaylist.ResumeLayout(false);
            this.groupCurrentlyPlaying.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupOptions;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCurrentArtist;
        private System.Windows.Forms.Label lblCurrentAlbum;
        private System.Windows.Forms.Label lblCurrentTitle;
        private System.Windows.Forms.Label lblCurrentPath;
        private System.Windows.Forms.ListBox listBoxPlaylist;
        private System.Windows.Forms.GroupBox groupPlaylist;
        private System.Windows.Forms.GroupBox groupCurrentlyPlaying;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblCurrentPosition;
        private System.Windows.Forms.FolderBrowserDialog dialogFolderBrowser;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TrackBar trackPosition;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Timer timerUpdateSoundSystem;
        private System.Windows.Forms.Label lblCurrentPositionPCM;
        private System.Windows.Forms.TrackBar trackVolume;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.Label lblVolumeValue;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Label lblCurrentLength;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblCurrentLengthPCM;
    }
}

