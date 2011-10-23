namespace PlaybackEngineV4
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
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupLoadPlaylist = new System.Windows.Forms.GroupBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblCurrentAlbumTitle = new System.Windows.Forms.Label();
            this.lblCurrentArtistTitle = new System.Windows.Forms.Label();
            this.lblCurrentTitleTitle = new System.Windows.Forms.Label();
            this.lblCurrentPathTitle = new System.Windows.Forms.Label();
            this.lblCurrentArtist = new System.Windows.Forms.Label();
            this.lblCurrentAlbum = new System.Windows.Forms.Label();
            this.lblCurrentTitle = new System.Windows.Forms.Label();
            this.lblCurrentPath = new System.Windows.Forms.Label();
            this.listBoxPlaylist = new System.Windows.Forms.ListBox();
            this.groupPlaylist = new System.Windows.Forms.GroupBox();
            this.groupCurrentlyPlaying = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPCM = new System.Windows.Forms.Label();
            this.lblCurrentLengthPCM = new System.Windows.Forms.Label();
            this.lblLength = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.lblAudioFilePosition = new System.Windows.Forms.Label();
            this.lblCurrentLength = new System.Windows.Forms.Label();
            this.lblCurrentPositionPCM = new System.Windows.Forms.Label();
            this.lblCurrentPosition = new System.Windows.Forms.Label();
            this.trackPosition = new System.Windows.Forms.TrackBar();
            this.dialogFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.timerUpdateSoundSystem = new System.Windows.Forms.Timer(this.components);
            this.trackVolume = new System.Windows.Forms.TrackBar();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lblVolumeValue = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTimeShiftingValue = new System.Windows.Forms.Label();
            this.lblTimeShifting = new System.Windows.Forms.Label();
            this.trackTimeShifting = new System.Windows.Forms.TrackBar();
            this.linkResetTimeShifting = new System.Windows.Forms.LinkLabel();
            this.groupLoop = new System.Windows.Forms.GroupBox();
            this.txtLoopEnd = new System.Windows.Forms.MaskedTextBox();
            this.txtLoopStart = new System.Windows.Forms.MaskedTextBox();
            this.lblLoopEnd = new System.Windows.Forms.Label();
            this.lblLoopStart = new System.Windows.Forms.Label();
            this.groupEQ = new System.Windows.Forms.GroupBox();
            this.txtEQBandwidth = new System.Windows.Forms.MaskedTextBox();
            this.lblEQBandwidth = new System.Windows.Forms.Label();
            this.txtEQQ = new System.Windows.Forms.MaskedTextBox();
            this.lblEQQ = new System.Windows.Forms.Label();
            this.comboEQBands = new System.Windows.Forms.ComboBox();
            this.txtEQGain = new System.Windows.Forms.MaskedTextBox();
            this.lblEQGain = new System.Windows.Forms.Label();
            this.lblEQBand = new System.Windows.Forms.Label();
            this.btnResetEQ = new System.Windows.Forms.Button();
            this.btnSetEQ = new System.Windows.Forms.Button();
            this.btnStopLoop = new System.Windows.Forms.Button();
            this.btnPlayLoop = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnRepeat = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnPlay = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupLoadPlaylist.SuspendLayout();
            this.groupPlaylist.SuspendLayout();
            this.groupCurrentlyPlaying.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).BeginInit();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTimeShifting)).BeginInit();
            this.groupLoop.SuspendLayout();
            this.groupEQ.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright.Location = new System.Drawing.Point(62, 41);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(243, 12);
            this.lblCopyright.TabIndex = 18;
            this.lblCopyright.Text = "Copyright © 2011 Yanick Castonguay";
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
            // groupLoadPlaylist
            // 
            this.groupLoadPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupLoadPlaylist.Controls.Add(this.lblPath);
            this.groupLoadPlaylist.Controls.Add(this.btnBrowse);
            this.groupLoadPlaylist.Controls.Add(this.txtPath);
            this.groupLoadPlaylist.Location = new System.Drawing.Point(5, 69);
            this.groupLoadPlaylist.Name = "groupLoadPlaylist";
            this.groupLoadPlaylist.Size = new System.Drawing.Size(379, 85);
            this.groupLoadPlaylist.TabIndex = 15;
            this.groupLoadPlaylist.TabStop = false;
            this.groupLoadPlaylist.Text = "Load Playlist";
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(6, 18);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(341, 12);
            this.lblPath.TabIndex = 3;
            this.lblPath.Text = "Select a folder containing audio files to play :";
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPath.Location = new System.Drawing.Point(7, 34);
            this.txtPath.Multiline = true;
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(298, 44);
            this.txtPath.TabIndex = 0;
            this.txtPath.Text = "C:\\";
            // 
            // lblCurrentAlbumTitle
            // 
            this.lblCurrentAlbumTitle.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentAlbumTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblCurrentAlbumTitle.Location = new System.Drawing.Point(136, 29);
            this.lblCurrentAlbumTitle.Name = "lblCurrentAlbumTitle";
            this.lblCurrentAlbumTitle.Size = new System.Drawing.Size(71, 9);
            this.lblCurrentAlbumTitle.TabIndex = 21;
            this.lblCurrentAlbumTitle.Text = "Album";
            this.lblCurrentAlbumTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentArtistTitle
            // 
            this.lblCurrentArtistTitle.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentArtistTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblCurrentArtistTitle.Location = new System.Drawing.Point(136, 3);
            this.lblCurrentArtistTitle.Name = "lblCurrentArtistTitle";
            this.lblCurrentArtistTitle.Size = new System.Drawing.Size(245, 9);
            this.lblCurrentArtistTitle.TabIndex = 20;
            this.lblCurrentArtistTitle.Text = "Artist";
            this.lblCurrentArtistTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentTitleTitle
            // 
            this.lblCurrentTitleTitle.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTitleTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblCurrentTitleTitle.Location = new System.Drawing.Point(136, 55);
            this.lblCurrentTitleTitle.Name = "lblCurrentTitleTitle";
            this.lblCurrentTitleTitle.Size = new System.Drawing.Size(71, 9);
            this.lblCurrentTitleTitle.TabIndex = 22;
            this.lblCurrentTitleTitle.Text = "Title";
            this.lblCurrentTitleTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentPathTitle
            // 
            this.lblCurrentPathTitle.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentPathTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblCurrentPathTitle.Location = new System.Drawing.Point(136, 81);
            this.lblCurrentPathTitle.Name = "lblCurrentPathTitle";
            this.lblCurrentPathTitle.Size = new System.Drawing.Size(71, 9);
            this.lblCurrentPathTitle.TabIndex = 23;
            this.lblCurrentPathTitle.Text = "Path";
            this.lblCurrentPathTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentArtist
            // 
            this.lblCurrentArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentArtist.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentArtist.ForeColor = System.Drawing.Color.White;
            this.lblCurrentArtist.Location = new System.Drawing.Point(136, 15);
            this.lblCurrentArtist.Name = "lblCurrentArtist";
            this.lblCurrentArtist.Size = new System.Drawing.Size(503, 11);
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
            this.lblCurrentAlbum.Location = new System.Drawing.Point(136, 41);
            this.lblCurrentAlbum.Name = "lblCurrentAlbum";
            this.lblCurrentAlbum.Size = new System.Drawing.Size(509, 11);
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
            this.lblCurrentTitle.Location = new System.Drawing.Point(136, 67);
            this.lblCurrentTitle.Name = "lblCurrentTitle";
            this.lblCurrentTitle.Size = new System.Drawing.Size(503, 11);
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
            this.lblCurrentPath.Location = new System.Drawing.Point(136, 93);
            this.lblCurrentPath.Name = "lblCurrentPath";
            this.lblCurrentPath.Size = new System.Drawing.Size(509, 11);
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
            this.listBoxPlaylist.Size = new System.Drawing.Size(910, 124);
            this.listBoxPlaylist.TabIndex = 28;
            this.listBoxPlaylist.DoubleClick += new System.EventHandler(this.listBoxPlaylist_DoubleClick);
            // 
            // groupPlaylist
            // 
            this.groupPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPlaylist.Controls.Add(this.listBoxPlaylist);
            this.groupPlaylist.Location = new System.Drawing.Point(5, 295);
            this.groupPlaylist.Name = "groupPlaylist";
            this.groupPlaylist.Size = new System.Drawing.Size(923, 147);
            this.groupPlaylist.TabIndex = 16;
            this.groupPlaylist.TabStop = false;
            this.groupPlaylist.Text = "Playlist";
            // 
            // groupCurrentlyPlaying
            // 
            this.groupCurrentlyPlaying.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupCurrentlyPlaying.Controls.Add(this.panel1);
            this.groupCurrentlyPlaying.Location = new System.Drawing.Point(5, 160);
            this.groupCurrentlyPlaying.Name = "groupCurrentlyPlaying";
            this.groupCurrentlyPlaying.Size = new System.Drawing.Size(923, 129);
            this.groupCurrentlyPlaying.TabIndex = 29;
            this.groupCurrentlyPlaying.TabStop = false;
            this.groupCurrentlyPlaying.Text = "Currently playing";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.lblPCM);
            this.panel1.Controls.Add(this.lblCurrentLengthPCM);
            this.panel1.Controls.Add(this.lblLength);
            this.panel1.Controls.Add(this.lblPosition);
            this.panel1.Controls.Add(this.lblAudioFilePosition);
            this.panel1.Controls.Add(this.lblCurrentLength);
            this.panel1.Controls.Add(this.lblCurrentPositionPCM);
            this.panel1.Controls.Add(this.lblCurrentPosition);
            this.panel1.Controls.Add(this.trackPosition);
            this.panel1.Controls.Add(this.lblCurrentArtistTitle);
            this.panel1.Controls.Add(this.lblCurrentPath);
            this.panel1.Controls.Add(this.lblCurrentArtist);
            this.panel1.Controls.Add(this.lblCurrentAlbumTitle);
            this.panel1.Controls.Add(this.lblCurrentPathTitle);
            this.panel1.Controls.Add(this.lblCurrentTitle);
            this.panel1.Controls.Add(this.lblCurrentAlbum);
            this.panel1.Controls.Add(this.lblCurrentTitleTitle);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Location = new System.Drawing.Point(6, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(910, 111);
            this.panel1.TabIndex = 28;
            // 
            // lblPCM
            // 
            this.lblPCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPCM.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPCM.ForeColor = System.Drawing.Color.Silver;
            this.lblPCM.Location = new System.Drawing.Point(763, 76);
            this.lblPCM.Name = "lblPCM";
            this.lblPCM.Size = new System.Drawing.Size(32, 9);
            this.lblPCM.TabIndex = 37;
            this.lblPCM.Text = "PCM";
            this.lblPCM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurrentLengthPCM
            // 
            this.lblCurrentLengthPCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentLengthPCM.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentLengthPCM.ForeColor = System.Drawing.Color.White;
            this.lblCurrentLengthPCM.Location = new System.Drawing.Point(807, 75);
            this.lblCurrentLengthPCM.Name = "lblCurrentLengthPCM";
            this.lblCurrentLengthPCM.Size = new System.Drawing.Size(92, 12);
            this.lblCurrentLengthPCM.TabIndex = 38;
            this.lblCurrentLengthPCM.Text = "[Length]";
            this.lblCurrentLengthPCM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblLength
            // 
            this.lblLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLength.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLength.ForeColor = System.Drawing.Color.Gray;
            this.lblLength.Location = new System.Drawing.Point(775, 45);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(124, 9);
            this.lblLength.TabIndex = 35;
            this.lblLength.Text = "Length";
            this.lblLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPosition
            // 
            this.lblPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPosition.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosition.ForeColor = System.Drawing.Color.Gray;
            this.lblPosition.Location = new System.Drawing.Point(645, 45);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(124, 9);
            this.lblPosition.TabIndex = 34;
            this.lblPosition.Text = "Position";
            this.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAudioFilePosition
            // 
            this.lblAudioFilePosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAudioFilePosition.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAudioFilePosition.ForeColor = System.Drawing.Color.Silver;
            this.lblAudioFilePosition.Location = new System.Drawing.Point(645, 4);
            this.lblAudioFilePosition.Name = "lblAudioFilePosition";
            this.lblAudioFilePosition.Size = new System.Drawing.Size(221, 9);
            this.lblAudioFilePosition.TabIndex = 33;
            this.lblAudioFilePosition.Text = "Audio File Position";
            this.lblAudioFilePosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurrentLength
            // 
            this.lblCurrentLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentLength.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentLength.ForeColor = System.Drawing.Color.White;
            this.lblCurrentLength.Location = new System.Drawing.Point(785, 59);
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
            this.lblCurrentPositionPCM.Location = new System.Drawing.Point(645, 75);
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
            this.lblCurrentPosition.Location = new System.Drawing.Point(645, 59);
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
            this.trackPosition.Location = new System.Drawing.Point(639, 19);
            this.trackPosition.Maximum = 1000;
            this.trackPosition.Name = "trackPosition";
            this.trackPosition.Size = new System.Drawing.Size(268, 45);
            this.trackPosition.SmallChange = 25;
            this.trackPosition.TabIndex = 30;
            this.trackPosition.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackPosition.MouseCaptureChanged += new System.EventHandler(this.trackPosition_MouseCaptureChanged);
            this.trackPosition.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackPosition_MouseDown);
            this.trackPosition.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackPosition_MouseUp);
            // 
            // dialogFolderBrowser
            // 
            this.dialogFolderBrowser.Description = "Select a folder with audio files to play (*.MP3, *.FLAC, *.OGG, *.WAV).";
            this.dialogFolderBrowser.SelectedPath = "F:\\Flac\\Nine Inch Nails";
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
            this.trackVolume.Location = new System.Drawing.Point(771, 38);
            this.trackVolume.Maximum = 100;
            this.trackVolume.Name = "trackVolume";
            this.trackVolume.Size = new System.Drawing.Size(157, 45);
            this.trackVolume.SmallChange = 25;
            this.trackVolume.TabIndex = 32;
            this.trackVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackVolume.Value = 100;
            this.trackVolume.Scroll += new System.EventHandler(this.trackVolume_Scroll);
            // 
            // lblVolume
            // 
            this.lblVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVolume.AutoSize = true;
            this.lblVolume.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolume.Location = new System.Drawing.Point(717, 36);
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
            this.lblVolumeValue.Location = new System.Drawing.Point(732, 49);
            this.lblVolumeValue.Name = "lblVolumeValue";
            this.lblVolumeValue.Size = new System.Drawing.Size(37, 11);
            this.lblVolumeValue.TabIndex = 34;
            this.lblVolumeValue.Text = "100%";
            this.lblVolumeValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 445);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(934, 22);
            this.statusStrip.TabIndex = 35;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // lblTimeShiftingValue
            // 
            this.lblTimeShiftingValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeShiftingValue.AutoSize = true;
            this.lblTimeShiftingValue.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeShiftingValue.Location = new System.Drawing.Point(514, 49);
            this.lblTimeShiftingValue.Name = "lblTimeShiftingValue";
            this.lblTimeShiftingValue.Size = new System.Drawing.Size(21, 11);
            this.lblTimeShiftingValue.TabIndex = 41;
            this.lblTimeShiftingValue.Text = "0%";
            this.lblTimeShiftingValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTimeShifting
            // 
            this.lblTimeShifting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeShifting.AutoSize = true;
            this.lblTimeShifting.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeShifting.Location = new System.Drawing.Point(450, 36);
            this.lblTimeShifting.Name = "lblTimeShifting";
            this.lblTimeShifting.Size = new System.Drawing.Size(103, 12);
            this.lblTimeShifting.TabIndex = 40;
            this.lblTimeShifting.Text = "Time Shifting:";
            // 
            // trackTimeShifting
            // 
            this.trackTimeShifting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackTimeShifting.LargeChange = 10;
            this.trackTimeShifting.Location = new System.Drawing.Point(554, 38);
            this.trackTimeShifting.Maximum = 100;
            this.trackTimeShifting.Minimum = -100;
            this.trackTimeShifting.Name = "trackTimeShifting";
            this.trackTimeShifting.Size = new System.Drawing.Size(157, 45);
            this.trackTimeShifting.SmallChange = 25;
            this.trackTimeShifting.TabIndex = 39;
            this.trackTimeShifting.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackTimeShifting.Scroll += new System.EventHandler(this.trackTimeShifting_Scroll);
            // 
            // linkResetTimeShifting
            // 
            this.linkResetTimeShifting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkResetTimeShifting.AutoSize = true;
            this.linkResetTimeShifting.Location = new System.Drawing.Point(450, 50);
            this.linkResetTimeShifting.Name = "linkResetTimeShifting";
            this.linkResetTimeShifting.Size = new System.Drawing.Size(40, 12);
            this.linkResetTimeShifting.TabIndex = 42;
            this.linkResetTimeShifting.TabStop = true;
            this.linkResetTimeShifting.Text = "Reset";
            this.linkResetTimeShifting.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkResetTimeShifting_LinkClicked);
            // 
            // groupLoop
            // 
            this.groupLoop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupLoop.Controls.Add(this.txtLoopEnd);
            this.groupLoop.Controls.Add(this.txtLoopStart);
            this.groupLoop.Controls.Add(this.lblLoopEnd);
            this.groupLoop.Controls.Add(this.lblLoopStart);
            this.groupLoop.Controls.Add(this.btnStopLoop);
            this.groupLoop.Controls.Add(this.btnPlayLoop);
            this.groupLoop.Location = new System.Drawing.Point(718, 69);
            this.groupLoop.Name = "groupLoop";
            this.groupLoop.Size = new System.Drawing.Size(210, 85);
            this.groupLoop.TabIndex = 16;
            this.groupLoop.TabStop = false;
            this.groupLoop.Text = "Loop";
            // 
            // txtLoopEnd
            // 
            this.txtLoopEnd.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLoopEnd.Location = new System.Drawing.Point(110, 31);
            this.txtLoopEnd.Mask = "00000000";
            this.txtLoopEnd.Name = "txtLoopEnd";
            this.txtLoopEnd.Size = new System.Drawing.Size(94, 18);
            this.txtLoopEnd.TabIndex = 47;
            this.txtLoopEnd.Text = "00050000";
            // 
            // txtLoopStart
            // 
            this.txtLoopStart.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLoopStart.Location = new System.Drawing.Point(8, 31);
            this.txtLoopStart.Mask = "00000000";
            this.txtLoopStart.Name = "txtLoopStart";
            this.txtLoopStart.Size = new System.Drawing.Size(94, 18);
            this.txtLoopStart.TabIndex = 46;
            this.txtLoopStart.Text = "00000000";
            // 
            // lblLoopEnd
            // 
            this.lblLoopEnd.AutoSize = true;
            this.lblLoopEnd.Location = new System.Drawing.Point(108, 15);
            this.lblLoopEnd.Name = "lblLoopEnd";
            this.lblLoopEnd.Size = new System.Drawing.Size(33, 12);
            this.lblLoopEnd.TabIndex = 45;
            this.lblLoopEnd.Text = "End:";
            // 
            // lblLoopStart
            // 
            this.lblLoopStart.AutoSize = true;
            this.lblLoopStart.Location = new System.Drawing.Point(6, 16);
            this.lblLoopStart.Name = "lblLoopStart";
            this.lblLoopStart.Size = new System.Drawing.Size(47, 12);
            this.lblLoopStart.TabIndex = 4;
            this.lblLoopStart.Text = "Start:";
            // 
            // groupEQ
            // 
            this.groupEQ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupEQ.Controls.Add(this.btnResetEQ);
            this.groupEQ.Controls.Add(this.txtEQBandwidth);
            this.groupEQ.Controls.Add(this.lblEQBandwidth);
            this.groupEQ.Controls.Add(this.txtEQQ);
            this.groupEQ.Controls.Add(this.lblEQQ);
            this.groupEQ.Controls.Add(this.btnSetEQ);
            this.groupEQ.Controls.Add(this.comboEQBands);
            this.groupEQ.Controls.Add(this.txtEQGain);
            this.groupEQ.Controls.Add(this.lblEQGain);
            this.groupEQ.Controls.Add(this.lblEQBand);
            this.groupEQ.Location = new System.Drawing.Point(390, 69);
            this.groupEQ.Name = "groupEQ";
            this.groupEQ.Size = new System.Drawing.Size(323, 85);
            this.groupEQ.TabIndex = 48;
            this.groupEQ.TabStop = false;
            this.groupEQ.Text = "EQ";
            // 
            // txtEQBandwidth
            // 
            this.txtEQBandwidth.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEQBandwidth.Location = new System.Drawing.Point(234, 33);
            this.txtEQBandwidth.Mask = "00.00";
            this.txtEQBandwidth.Name = "txtEQBandwidth";
            this.txtEQBandwidth.Size = new System.Drawing.Size(73, 18);
            this.txtEQBandwidth.TabIndex = 56;
            this.txtEQBandwidth.Text = "0250";
            // 
            // lblEQBandwidth
            // 
            this.lblEQBandwidth.AutoSize = true;
            this.lblEQBandwidth.Location = new System.Drawing.Point(232, 18);
            this.lblEQBandwidth.Name = "lblEQBandwidth";
            this.lblEQBandwidth.Size = new System.Drawing.Size(75, 12);
            this.lblEQBandwidth.TabIndex = 55;
            this.lblEQBandwidth.Text = "Bandwidth:";
            // 
            // txtEQQ
            // 
            this.txtEQQ.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEQQ.Location = new System.Drawing.Point(180, 33);
            this.txtEQQ.Mask = "00.00";
            this.txtEQQ.Name = "txtEQQ";
            this.txtEQQ.Size = new System.Drawing.Size(50, 18);
            this.txtEQQ.TabIndex = 54;
            this.txtEQQ.Text = "0100";
            // 
            // lblEQQ
            // 
            this.lblEQQ.AutoSize = true;
            this.lblEQQ.Location = new System.Drawing.Point(178, 18);
            this.lblEQQ.Name = "lblEQQ";
            this.lblEQQ.Size = new System.Drawing.Size(19, 12);
            this.lblEQQ.TabIndex = 53;
            this.lblEQQ.Text = "Q:";
            // 
            // comboEQBands
            // 
            this.comboEQBands.DisplayMember = "Text";
            this.comboEQBands.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEQBands.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboEQBands.FormattingEnabled = true;
            this.comboEQBands.Location = new System.Drawing.Point(8, 32);
            this.comboEQBands.Name = "comboEQBands";
            this.comboEQBands.Size = new System.Drawing.Size(113, 19);
            this.comboEQBands.TabIndex = 52;
            this.comboEQBands.ValueMember = "Band";
            this.comboEQBands.SelectedIndexChanged += new System.EventHandler(this.comboEQBands_SelectedIndexChanged);
            // 
            // txtEQGain
            // 
            this.txtEQGain.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEQGain.Location = new System.Drawing.Point(126, 33);
            this.txtEQGain.Mask = "00.00";
            this.txtEQGain.Name = "txtEQGain";
            this.txtEQGain.Size = new System.Drawing.Size(50, 18);
            this.txtEQGain.TabIndex = 51;
            this.txtEQGain.Text = "0600";
            // 
            // lblEQGain
            // 
            this.lblEQGain.AutoSize = true;
            this.lblEQGain.Location = new System.Drawing.Point(124, 18);
            this.lblEQGain.Name = "lblEQGain";
            this.lblEQGain.Size = new System.Drawing.Size(40, 12);
            this.lblEQGain.TabIndex = 50;
            this.lblEQGain.Text = "Gain:";
            // 
            // lblEQBand
            // 
            this.lblEQBand.AutoSize = true;
            this.lblEQBand.Location = new System.Drawing.Point(6, 18);
            this.lblEQBand.Name = "lblEQBand";
            this.lblEQBand.Size = new System.Drawing.Size(40, 12);
            this.lblEQBand.TabIndex = 48;
            this.lblEQBand.Text = "Band:";
            // 
            // btnResetEQ
            // 
            this.btnResetEQ.Image = global::PlaybackEngineV4.Properties.Resources.chart_bar_error;
            this.btnResetEQ.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnResetEQ.Location = new System.Drawing.Point(91, 55);
            this.btnResetEQ.Name = "btnResetEQ";
            this.btnResetEQ.Size = new System.Drawing.Size(91, 23);
            this.btnResetEQ.TabIndex = 57;
            this.btnResetEQ.Text = "Reset EQ";
            this.btnResetEQ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnResetEQ.UseVisualStyleBackColor = true;
            this.btnResetEQ.Click += new System.EventHandler(this.btnResetEQ_Click);
            // 
            // btnSetEQ
            // 
            this.btnSetEQ.Image = global::PlaybackEngineV4.Properties.Resources.chart_bar_edit;
            this.btnSetEQ.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnSetEQ.Location = new System.Drawing.Point(8, 55);
            this.btnSetEQ.Name = "btnSetEQ";
            this.btnSetEQ.Size = new System.Drawing.Size(77, 23);
            this.btnSetEQ.TabIndex = 48;
            this.btnSetEQ.Text = "Set EQ";
            this.btnSetEQ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSetEQ.UseVisualStyleBackColor = true;
            this.btnSetEQ.Click += new System.EventHandler(this.btnSetEQ_Click);
            // 
            // btnStopLoop
            // 
            this.btnStopLoop.Enabled = false;
            this.btnStopLoop.Image = ((System.Drawing.Image)(resources.GetObject("btnStopLoop.Image")));
            this.btnStopLoop.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnStopLoop.Location = new System.Drawing.Point(110, 55);
            this.btnStopLoop.Name = "btnStopLoop";
            this.btnStopLoop.Size = new System.Drawing.Size(94, 23);
            this.btnStopLoop.TabIndex = 43;
            this.btnStopLoop.Text = "Stop Loop";
            this.btnStopLoop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStopLoop.UseVisualStyleBackColor = true;
            this.btnStopLoop.Click += new System.EventHandler(this.btnStopLoop_Click);
            // 
            // btnPlayLoop
            // 
            this.btnPlayLoop.Enabled = false;
            this.btnPlayLoop.Image = ((System.Drawing.Image)(resources.GetObject("btnPlayLoop.Image")));
            this.btnPlayLoop.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnPlayLoop.Location = new System.Drawing.Point(8, 55);
            this.btnPlayLoop.Name = "btnPlayLoop";
            this.btnPlayLoop.Size = new System.Drawing.Size(94, 23);
            this.btnPlayLoop.TabIndex = 43;
            this.btnPlayLoop.Text = "Play Loop";
            this.btnPlayLoop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPlayLoop.UseVisualStyleBackColor = true;
            this.btnPlayLoop.Click += new System.EventHandler(this.btnPlayLoop_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.Image")));
            this.btnBrowse.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBrowse.Location = new System.Drawing.Point(310, 33);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(62, 45);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnRepeat
            // 
            this.btnRepeat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRepeat.Image = global::PlaybackEngineV4.Properties.Resources.RepeatHS;
            this.btnRepeat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRepeat.Location = new System.Drawing.Point(786, 5);
            this.btnRepeat.Name = "btnRepeat";
            this.btnRepeat.Size = new System.Drawing.Size(74, 27);
            this.btnRepeat.TabIndex = 38;
            this.btnRepeat.Text = "Repeat";
            this.btnRepeat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRepeat.UseVisualStyleBackColor = true;
            this.btnRepeat.Click += new System.EventHandler(this.btnRepeat_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrev.Enabled = false;
            this.btnPrev.Image = global::PlaybackEngineV4.Properties.Resources.DataContainer_MoveFirstHS;
            this.btnPrev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrev.Location = new System.Drawing.Point(648, 5);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(63, 27);
            this.btnPrev.TabIndex = 37;
            this.btnPrev.Text = "Prev";
            this.btnPrev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Enabled = false;
            this.btnNext.Image = global::PlaybackEngineV4.Properties.Resources.DataContainer_MoveLastHS;
            this.btnNext.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNext.Location = new System.Drawing.Point(717, 5);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(63, 27);
            this.btnNext.TabIndex = 36;
            this.btnNext.Text = "Next";
            this.btnNext.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPause
            // 
            this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPause.Enabled = false;
            this.btnPause.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPause.Location = new System.Drawing.Point(503, 5);
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
            this.btnExit.Location = new System.Drawing.Point(865, 5);
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
            this.btnStop.Location = new System.Drawing.Point(578, 5);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(63, 27);
            this.btnStop.TabIndex = 29;
            this.btnStop.Text = "Stop";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(-56, -72);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(203, 170);
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlay.Enabled = false;
            this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
            this.btnPlay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlay.Location = new System.Drawing.Point(434, 5);
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
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(934, 467);
            this.Controls.Add(this.groupEQ);
            this.Controls.Add(this.groupLoop);
            this.Controls.Add(this.groupLoadPlaylist);
            this.Controls.Add(this.linkResetTimeShifting);
            this.Controls.Add(this.lblTimeShiftingValue);
            this.Controls.Add(this.lblTimeShifting);
            this.Controls.Add(this.trackTimeShifting);
            this.Controls.Add(this.btnRepeat);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.lblVolumeValue);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.trackVolume);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.groupCurrentlyPlaying);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.groupPlaylist);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(900, 400);
            this.Name = "frmMain";
            this.Text = "MPfm - Playback Engine V4 Demo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupLoadPlaylist.ResumeLayout(false);
            this.groupLoadPlaylist.PerformLayout();
            this.groupPlaylist.ResumeLayout(false);
            this.groupCurrentlyPlaying.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTimeShifting)).EndInit();
            this.groupLoop.ResumeLayout(false);
            this.groupLoop.PerformLayout();
            this.groupEQ.ResumeLayout(false);
            this.groupEQ.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupLoadPlaylist;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblCurrentAlbumTitle;
        private System.Windows.Forms.Label lblCurrentArtistTitle;
        private System.Windows.Forms.Label lblCurrentTitleTitle;
        private System.Windows.Forms.Label lblCurrentPathTitle;
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
        private System.Windows.Forms.Label lblAudioFilePosition;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.Label lblPCM;
        private System.Windows.Forms.Label lblCurrentLengthPCM;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnRepeat;
        private System.Windows.Forms.Label lblTimeShiftingValue;
        private System.Windows.Forms.Label lblTimeShifting;
        private System.Windows.Forms.TrackBar trackTimeShifting;
        private System.Windows.Forms.LinkLabel linkResetTimeShifting;
        private System.Windows.Forms.GroupBox groupLoop;
        private System.Windows.Forms.Label lblLoopEnd;
        private System.Windows.Forms.Label lblLoopStart;
        private System.Windows.Forms.Button btnStopLoop;
        private System.Windows.Forms.Button btnPlayLoop;
        private System.Windows.Forms.MaskedTextBox txtLoopStart;
        private System.Windows.Forms.MaskedTextBox txtLoopEnd;
        private System.Windows.Forms.GroupBox groupEQ;
        private System.Windows.Forms.ComboBox comboEQBands;
        private System.Windows.Forms.MaskedTextBox txtEQGain;
        private System.Windows.Forms.Label lblEQGain;
        private System.Windows.Forms.Label lblEQBand;
        private System.Windows.Forms.Button btnSetEQ;
        private System.Windows.Forms.MaskedTextBox txtEQQ;
        private System.Windows.Forms.Label lblEQQ;
        private System.Windows.Forms.MaskedTextBox txtEQBandwidth;
        private System.Windows.Forms.Label lblEQBandwidth;
        private System.Windows.Forms.Button btnResetEQ;
    }
}

