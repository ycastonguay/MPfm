namespace MPfm
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
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            this.menuSongBrowser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miPlaySong = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditSong = new System.Windows.Forms.ToolStripMenuItem();
            this.miAddSongToPlaylist = new System.Windows.Forms.ToolStripMenuItem();
            this.miRemoveSong = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileAddFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileAddFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.miFileOpenAudioFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.miFileUpdateLibrary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miWindowsPlaylist = new System.Windows.Forms.ToolStripMenuItem();
            this.miWindowsEffects = new System.Windows.Forms.ToolStripMenuItem();
            this.miWindowsVisualizer = new System.Windows.Forms.ToolStripMenuItem();
            this.miWindowsSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelpWebsite = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelpReportBug = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.miHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sacToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.toolStripMain = new MPfm.WindowsControls.ToolStrip();
            this.btnUpdateLibrary = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.btnPause = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnPreviousSong = new System.Windows.Forms.ToolStripButton();
            this.btnNextSong = new System.Windows.Forms.ToolStripButton();
            this.btnRepeat = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPlaylist = new System.Windows.Forms.ToolStripButton();
            this.btnEffects = new System.Windows.Forms.ToolStripButton();
            this.btnVisualizer = new System.Windows.Forms.ToolStripButton();
            this.btnSettings = new System.Windows.Forms.ToolStripButton();
            this.dialogAddFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.splitFirst = new System.Windows.Forms.SplitContainer();
            this.panelLibrary = new MPfm.WindowsControls.Panel();
            this.lblFilterBySoundFormat = new MPfm.WindowsControls.Label();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.treeLibrary = new MPfm.WindowsControls.TreeView();
            this.menuLibrary = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miTreeLibraryPlaySongs = new System.Windows.Forms.ToolStripMenuItem();
            this.miTreeLibraryAddSongsToPlaylist = new System.Windows.Forms.ToolStripMenuItem();
            this.miTreeLibraryRemoveSongsFromLibrary = new System.Windows.Forms.ToolStripMenuItem();
            this.miTreeLibraryDeletePlaylist = new System.Windows.Forms.ToolStripMenuItem();
            this.comboSoundFormat = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panelCurrentSong = new MPfm.WindowsControls.Panel();
            this.panelCurrentSongChild = new MPfm.WindowsControls.Panel();
            this.lblCurrentFilePath = new MPfm.WindowsControls.Label();
            this.panelInformation = new MPfm.WindowsControls.Panel();
            this.lblFrequency = new MPfm.WindowsControls.Label();
            this.lblFrequencyTitle = new MPfm.WindowsControls.Label();
            this.lblBitsPerSample = new MPfm.WindowsControls.Label();
            this.lblBitsPerSampleTitle = new MPfm.WindowsControls.Label();
            this.lblSoundFormat = new MPfm.WindowsControls.Label();
            this.lblSoundFormatTitle = new MPfm.WindowsControls.Label();
            this.panelActions = new MPfm.WindowsControls.Panel();
            this.lblSearchWeb = new MPfm.WindowsControls.Label();
            this.linkEditSongMetadata = new MPfm.WindowsControls.LinkLabel();
            this.linkSearchLyrics = new MPfm.WindowsControls.LinkLabel();
            this.linkSearchBassTabs = new MPfm.WindowsControls.LinkLabel();
            this.linkSearchGuitarTabs = new MPfm.WindowsControls.LinkLabel();
            this.lblCurrentAlbumTitle = new MPfm.WindowsControls.LinkLabel();
            this.lblCurrentArtistName = new MPfm.WindowsControls.LinkLabel();
            this.panelVolume = new MPfm.WindowsControls.Panel();
            this.picDistortionWarning = new System.Windows.Forms.PictureBox();
            this.outputMeter = new MPfm.WindowsControls.OutputMeter();
            this.faderVolume = new MPfm.WindowsControls.VolumeFader();
            this.lblVolume = new MPfm.WindowsControls.Label();
            this.panelTimeShifting = new MPfm.WindowsControls.Panel();
            this.lblTimeShifting = new MPfm.WindowsControls.Label();
            this.linkResetTimeShifting = new MPfm.WindowsControls.LinkLabel();
            this.trackTimeShifting = new MPfm.WindowsControls.TrackBar();
            this.panelSongPosition = new MPfm.WindowsControls.Panel();
            this.lblSongPercentage = new MPfm.WindowsControls.Label();
            this.lblSongPosition = new MPfm.WindowsControls.Label();
            this.trackPosition = new MPfm.WindowsControls.TrackBar();
            this.panelTotalTime = new MPfm.WindowsControls.Panel();
            this.lblTotalTime = new MPfm.WindowsControls.Label();
            this.panelCurrentTime = new MPfm.WindowsControls.Panel();
            this.lblCurrentTime = new MPfm.WindowsControls.Label();
            this.lblCurrentSongTitle = new MPfm.WindowsControls.Label();
            this.picAlbum = new System.Windows.Forms.PictureBox();
            this.splitLoopsMarkersSongBrowser = new System.Windows.Forms.SplitContainer();
            this.panelLoopsMarkers = new MPfm.WindowsControls.Panel();
            this.splitWaveFormLoopsMarkers = new System.Windows.Forms.SplitContainer();
            this.waveFormMarkersLoops = new MPfm.WindowsControls.WaveFormMarkersLoops();
            this.splitLoopsMarkers = new System.Windows.Forms.SplitContainer();
            this.btnStopLoop = new MPfm.WindowsControls.Button();
            this.btnEditLoop = new MPfm.WindowsControls.Button();
            this.btnPlayLoop = new MPfm.WindowsControls.Button();
            this.lblLoops = new MPfm.WindowsControls.Label();
            this.btnRemoveLoop = new MPfm.WindowsControls.Button();
            this.btnAddLoop = new MPfm.WindowsControls.Button();
            this.viewLoops = new MPfm.WindowsControls.ReorderListView();
            this.columnLoopPlayIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLoopName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLoopLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLoopMarkerA = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLoopMarkerB = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnGoToMarker = new MPfm.WindowsControls.Button();
            this.btnRemoveMarker = new MPfm.WindowsControls.Button();
            this.lblMarkers = new MPfm.WindowsControls.Label();
            this.btnEditMarker = new MPfm.WindowsControls.Button();
            this.btnAddMarker = new MPfm.WindowsControls.Button();
            this.viewMarkers = new MPfm.WindowsControls.ReorderListView();
            this.columnMarkerName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnMarkerPosition = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnMarkerComments = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelSongBrowser = new MPfm.WindowsControls.Panel();
            this.panelSongBrowserToolbar = new MPfm.WindowsControls.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new MPfm.WindowsControls.Label();
            this.btnPlaySelectedSong = new MPfm.WindowsControls.Button();
            this.btnAddSongToPlaylist = new MPfm.WindowsControls.Button();
            this.btnEditSongMetadata = new MPfm.WindowsControls.Button();
            this.viewSongs = new MPfm.WindowsControls.ReorderListView();
            this.columnSongPlayIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSongTrackNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSongTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSongLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSongArtistName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSongAlbumTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSongPlayCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSongLastPlayed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageListSongBrowser = new System.Windows.Forms.ImageList(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miTrayArtist = new System.Windows.Forms.ToolStripMenuItem();
            this.miTrayAlbum = new System.Windows.Forms.ToolStripMenuItem();
            this.miTraySongName = new System.Windows.Forms.ToolStripMenuItem();
            this.miTraySongStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.miTrayPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.miTrayPause = new System.Windows.Forms.ToolStripMenuItem();
            this.miTrayStop = new System.Windows.Forms.ToolStripMenuItem();
            this.miTrayPreviousSong = new System.Windows.Forms.ToolStripMenuItem();
            this.miTrayNextSong = new System.Windows.Forms.ToolStripMenuItem();
            this.miTrayRepeat = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.miTrayShowPMP = new System.Windows.Forms.ToolStripMenuItem();
            this.miTrayExitPMP = new System.Windows.Forms.ToolStripMenuItem();
            this.dialogAddFiles = new System.Windows.Forms.OpenFileDialog();
            this.workerTreeLibrary = new System.ComponentModel.BackgroundWorker();
            this.timerUpdateOutputMeter = new System.Windows.Forms.Timer(this.components);
            this.workerAlbumArt = new System.ComponentModel.BackgroundWorker();
            this.dialogOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.menuSongBrowser.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.splitFirst.Panel1.SuspendLayout();
            this.splitFirst.Panel2.SuspendLayout();
            this.splitFirst.SuspendLayout();
            this.panelLibrary.SuspendLayout();
            this.menuLibrary.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panelCurrentSong.SuspendLayout();
            this.panelCurrentSongChild.SuspendLayout();
            this.panelInformation.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelVolume.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDistortionWarning)).BeginInit();
            this.panelTimeShifting.SuspendLayout();
            this.panelSongPosition.SuspendLayout();
            this.panelTotalTime.SuspendLayout();
            this.panelCurrentTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlbum)).BeginInit();
            this.splitLoopsMarkersSongBrowser.Panel1.SuspendLayout();
            this.splitLoopsMarkersSongBrowser.Panel2.SuspendLayout();
            this.splitLoopsMarkersSongBrowser.SuspendLayout();
            this.panelLoopsMarkers.SuspendLayout();
            this.splitWaveFormLoopsMarkers.Panel1.SuspendLayout();
            this.splitWaveFormLoopsMarkers.Panel2.SuspendLayout();
            this.splitWaveFormLoopsMarkers.SuspendLayout();
            this.splitLoopsMarkers.Panel1.SuspendLayout();
            this.splitLoopsMarkers.Panel2.SuspendLayout();
            this.splitLoopsMarkers.SuspendLayout();
            this.panelSongBrowser.SuspendLayout();
            this.panelSongBrowserToolbar.SuspendLayout();
            this.menuTray.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuSongBrowser
            // 
            this.menuSongBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPlaySong,
            this.miEditSong,
            this.miAddSongToPlaylist,
            this.miRemoveSong,
            this.toolStripMenuItem2,
            this.toolStripSeparator8});
            this.menuSongBrowser.Name = "menuSongBrowser";
            this.menuSongBrowser.Size = new System.Drawing.Size(217, 120);
            this.menuSongBrowser.Opening += new System.ComponentModel.CancelEventHandler(this.menuSongBrowser_Opening);
            // 
            // miPlaySong
            // 
            this.miPlaySong.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.miPlaySong.Image = global::MPfm.Properties.Resources.control_play;
            this.miPlaySong.Name = "miPlaySong";
            this.miPlaySong.Size = new System.Drawing.Size(216, 22);
            this.miPlaySong.Text = "Play selected song(s)";
            this.miPlaySong.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // miEditSong
            // 
            this.miEditSong.Image = global::MPfm.Properties.Resources.information;
            this.miEditSong.Name = "miEditSong";
            this.miEditSong.Size = new System.Drawing.Size(216, 22);
            this.miEditSong.Text = "Edit song metadata";
            this.miEditSong.Click += new System.EventHandler(this.btnEditSongMetadata_Click);
            // 
            // miAddSongToPlaylist
            // 
            this.miAddSongToPlaylist.Image = global::MPfm.Properties.Resources.add;
            this.miAddSongToPlaylist.Name = "miAddSongToPlaylist";
            this.miAddSongToPlaylist.Size = new System.Drawing.Size(216, 22);
            this.miAddSongToPlaylist.Text = "Add song(s) to playlist";
            this.miAddSongToPlaylist.Click += new System.EventHandler(this.btnAddSongToPlaylist_Click);
            // 
            // miRemoveSong
            // 
            this.miRemoveSong.Image = global::MPfm.Properties.Resources.delete;
            this.miRemoveSong.Name = "miRemoveSong";
            this.miRemoveSong.Size = new System.Drawing.Size(216, 22);
            this.miRemoveSong.Text = "Remove songs from library";
            this.miRemoveSong.Visible = false;
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.CheckOnClick = true;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(213, 6);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "PMP.ico");
            this.imageList.Images.SetKeyName(1, "control_play_blue.png");
            this.imageList.Images.SetKeyName(2, "cd.png");
            this.imageList.Images.SetKeyName(3, "star.png");
            this.imageList.Images.SetKeyName(4, "table.png");
            this.imageList.Images.SetKeyName(5, "page_white_star.png");
            this.imageList.Images.SetKeyName(6, "tag_blue.png");
            this.imageList.Images.SetKeyName(7, "arrow_rotate_clockwise.png");
            this.imageList.Images.SetKeyName(8, "book_next.png");
            this.imageList.Images.SetKeyName(9, "page_white_text.png");
            this.imageList.Images.SetKeyName(10, "music.png");
            this.imageList.Images.SetKeyName(11, "application_view_detail.png");
            this.imageList.Images.SetKeyName(12, "database.png");
            this.imageList.Images.SetKeyName(13, "user_green.png");
            this.imageList.Images.SetKeyName(14, "user_orange.png");
            this.imageList.Images.SetKeyName(15, "user_red.png");
            this.imageList.Images.SetKeyName(16, "group.png");
            this.imageList.Images.SetKeyName(17, "cd_go.png");
            this.imageList.Images.SetKeyName(18, "calendar.png");
            this.imageList.Images.SetKeyName(19, "asterisk_orange.png");
            this.imageList.Images.SetKeyName(20, "bullet_go.png");
            this.imageList.Images.SetKeyName(21, "arrow_right.png");
            this.imageList.Images.SetKeyName(22, "song_load.gif");
            // 
            // menuMain
            // 
            this.menuMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.menuMain.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.windowsToolStripMenuItem,
            this.helpToolStripMenuItem1});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.menuMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuMain.Size = new System.Drawing.Size(1006, 24);
            this.menuMain.TabIndex = 11;
            this.menuMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileAddFile,
            this.miFileAddFolder,
            this.toolStripSeparator9,
            this.miFileOpenAudioFile,
            this.toolStripSeparator10,
            this.miFileUpdateLibrary,
            this.toolStripSeparator3,
            this.miFileExit});
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem1.Text = "&File";
            // 
            // miFileAddFile
            // 
            this.miFileAddFile.Image = global::MPfm.Properties.Resources.page_white_add;
            this.miFileAddFile.Name = "miFileAddFile";
            this.miFileAddFile.Size = new System.Drawing.Size(187, 22);
            this.miFileAddFile.Text = "Add &file(s) to library...";
            this.miFileAddFile.Click += new System.EventHandler(this.miFileAddFile_Click);
            // 
            // miFileAddFolder
            // 
            this.miFileAddFolder.Image = global::MPfm.Properties.Resources.folder_add;
            this.miFileAddFolder.Name = "miFileAddFolder";
            this.miFileAddFolder.Size = new System.Drawing.Size(187, 22);
            this.miFileAddFolder.Text = "Add a f&older to library...";
            this.miFileAddFolder.Click += new System.EventHandler(this.miFileAddFolder_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(184, 6);
            // 
            // miFileOpenAudioFile
            // 
            this.miFileOpenAudioFile.Image = global::MPfm.Properties.Resources.folder_page;
            this.miFileOpenAudioFile.Name = "miFileOpenAudioFile";
            this.miFileOpenAudioFile.Size = new System.Drawing.Size(187, 22);
            this.miFileOpenAudioFile.Text = "&Open an audio file...";
            this.miFileOpenAudioFile.Click += new System.EventHandler(this.miFileOpenAudioFile_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(184, 6);
            // 
            // miFileUpdateLibrary
            // 
            this.miFileUpdateLibrary.Image = global::MPfm.Properties.Resources.database_gear;
            this.miFileUpdateLibrary.Name = "miFileUpdateLibrary";
            this.miFileUpdateLibrary.Size = new System.Drawing.Size(187, 22);
            this.miFileUpdateLibrary.Text = "&Update Library";
            this.miFileUpdateLibrary.Click += new System.EventHandler(this.btnUpdateLibrary_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(184, 6);
            // 
            // miFileExit
            // 
            this.miFileExit.Image = global::MPfm.Properties.Resources.door_in;
            this.miFileExit.Name = "miFileExit";
            this.miFileExit.Size = new System.Drawing.Size(187, 22);
            this.miFileExit.Text = "&Exit";
            this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miWindowsPlaylist,
            this.miWindowsEffects,
            this.miWindowsVisualizer,
            this.miWindowsSettings});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.windowsToolStripMenuItem.Text = "&Windows";
            // 
            // miWindowsPlaylist
            // 
            this.miWindowsPlaylist.Image = global::MPfm.Properties.Resources.application_view_detail;
            this.miWindowsPlaylist.Name = "miWindowsPlaylist";
            this.miWindowsPlaylist.Size = new System.Drawing.Size(122, 22);
            this.miWindowsPlaylist.Text = "&Playlist";
            this.miWindowsPlaylist.Click += new System.EventHandler(this.btnPlaylist_Click);
            // 
            // miWindowsEffects
            // 
            this.miWindowsEffects.Image = global::MPfm.Properties.Resources.control_equalizer;
            this.miWindowsEffects.Name = "miWindowsEffects";
            this.miWindowsEffects.Size = new System.Drawing.Size(122, 22);
            this.miWindowsEffects.Text = "&Effects";
            this.miWindowsEffects.Click += new System.EventHandler(this.btnEffects_Click);
            // 
            // miWindowsVisualizer
            // 
            this.miWindowsVisualizer.Image = global::MPfm.Properties.Resources.chart_line;
            this.miWindowsVisualizer.Name = "miWindowsVisualizer";
            this.miWindowsVisualizer.Size = new System.Drawing.Size(122, 22);
            this.miWindowsVisualizer.Text = "&Visualizer";
            this.miWindowsVisualizer.Click += new System.EventHandler(this.btnVisualizer_Click);
            // 
            // miWindowsSettings
            // 
            this.miWindowsSettings.Image = global::MPfm.Properties.Resources.wrench;
            this.miWindowsSettings.Name = "miWindowsSettings";
            this.miWindowsSettings.Size = new System.Drawing.Size(122, 22);
            this.miWindowsSettings.Text = "&Settings";
            this.miWindowsSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHelpWebsite,
            this.miHelpReportBug,
            this.toolStripSeparator11,
            this.miHelpAbout});
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem1.Text = "&Help";
            // 
            // miHelpWebsite
            // 
            this.miHelpWebsite.Image = global::MPfm.Properties.Resources.world;
            this.miHelpWebsite.Name = "miHelpWebsite";
            this.miHelpWebsite.Size = new System.Drawing.Size(198, 22);
            this.miHelpWebsite.Text = "Go to the MPfm website...";
            this.miHelpWebsite.Click += new System.EventHandler(this.miHelpWebsite_Click);
            // 
            // miHelpReportBug
            // 
            this.miHelpReportBug.Image = global::MPfm.Properties.Resources.bug;
            this.miHelpReportBug.Name = "miHelpReportBug";
            this.miHelpReportBug.Size = new System.Drawing.Size(198, 22);
            this.miHelpReportBug.Text = "Report a bug...";
            this.miHelpReportBug.Click += new System.EventHandler(this.miHelpReportBug_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(195, 6);
            // 
            // miHelpAbout
            // 
            this.miHelpAbout.Image = global::MPfm.Properties.Resources.vcard;
            this.miHelpAbout.Name = "miHelpAbout";
            this.miHelpAbout.Size = new System.Drawing.Size(198, 22);
            this.miHelpAbout.Text = "&About MPfm...";
            this.miHelpAbout.Click += new System.EventHandler(this.miHelpAbout_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(110, 20);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // sacToolStripMenuItem
            // 
            this.sacToolStripMenuItem.Name = "sacToolStripMenuItem";
            this.sacToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sacToolStripMenuItem.Text = "sac.";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(201, 6);
            // 
            // statusBar
            // 
            this.statusBar.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBar.Location = new System.Drawing.Point(0, 695);
            this.statusBar.Name = "statusBar";
            this.statusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusBar.Size = new System.Drawing.Size(1006, 22);
            this.statusBar.TabIndex = 17;
            this.statusBar.Text = "statusStrip1";
            // 
            // toolStripMain
            // 
            this.toolStripMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.toolStripMain.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUpdateLibrary,
            this.toolStripSeparator5,
            this.btnPlay,
            this.btnPause,
            this.btnStop,
            this.btnPreviousSong,
            this.btnNextSong,
            this.btnRepeat,
            this.toolStripSeparator6,
            this.btnPlaylist,
            this.btnEffects,
            this.btnVisualizer,
            this.btnSettings});
            this.toolStripMain.Location = new System.Drawing.Point(0, 24);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMain.Size = new System.Drawing.Size(1006, 37);
            this.toolStripMain.TabIndex = 18;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // btnUpdateLibrary
            // 
            this.btnUpdateLibrary.Image = global::MPfm.Properties.Resources.database_gear;
            this.btnUpdateLibrary.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUpdateLibrary.Name = "btnUpdateLibrary";
            this.btnUpdateLibrary.Size = new System.Drawing.Size(82, 34);
            this.btnUpdateLibrary.Text = "Update Library";
            this.btnUpdateLibrary.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnUpdateLibrary.Click += new System.EventHandler(this.btnUpdateLibrary_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 37);
            // 
            // btnPlay
            // 
            this.btnPlay.Image = global::MPfm.Properties.Resources.control_play;
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(31, 34);
            this.btnPlay.Text = "Play";
            this.btnPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnPause
            // 
            this.btnPause.Image = global::MPfm.Properties.Resources.control_pause;
            this.btnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(41, 34);
            this.btnPause.Text = "Pause";
            this.btnPause.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = global::MPfm.Properties.Resources.control_stop;
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(33, 34);
            this.btnStop.Text = "Stop";
            this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPreviousSong
            // 
            this.btnPreviousSong.Image = global::MPfm.Properties.Resources.control_start;
            this.btnPreviousSong.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreviousSong.Name = "btnPreviousSong";
            this.btnPreviousSong.Size = new System.Drawing.Size(81, 34);
            this.btnPreviousSong.Text = "Previous Song";
            this.btnPreviousSong.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPreviousSong.Click += new System.EventHandler(this.btnPreviousSong_Click);
            // 
            // btnNextSong
            // 
            this.btnNextSong.Image = global::MPfm.Properties.Resources.control_end;
            this.btnNextSong.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextSong.Name = "btnNextSong";
            this.btnNextSong.Size = new System.Drawing.Size(61, 34);
            this.btnNextSong.Text = "Next Song";
            this.btnNextSong.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNextSong.Click += new System.EventHandler(this.btnNextSong_Click);
            // 
            // btnRepeat
            // 
            this.btnRepeat.Image = global::MPfm.Properties.Resources.control_repeat;
            this.btnRepeat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRepeat.Name = "btnRepeat";
            this.btnRepeat.Size = new System.Drawing.Size(72, 34);
            this.btnRepeat.Text = "Repeat (Off)";
            this.btnRepeat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnRepeat.Click += new System.EventHandler(this.btnRepeat_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 37);
            // 
            // btnPlaylist
            // 
            this.btnPlaylist.Image = global::MPfm.Properties.Resources.application_view_detail;
            this.btnPlaylist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlaylist.Name = "btnPlaylist";
            this.btnPlaylist.Size = new System.Drawing.Size(44, 34);
            this.btnPlaylist.Text = "Playlist";
            this.btnPlaylist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPlaylist.Click += new System.EventHandler(this.btnPlaylist_Click);
            // 
            // btnEffects
            // 
            this.btnEffects.Image = global::MPfm.Properties.Resources.control_equalizer;
            this.btnEffects.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEffects.Name = "btnEffects";
            this.btnEffects.Size = new System.Drawing.Size(46, 34);
            this.btnEffects.Text = "Effects";
            this.btnEffects.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnEffects.Click += new System.EventHandler(this.btnEffects_Click);
            // 
            // btnVisualizer
            // 
            this.btnVisualizer.Image = global::MPfm.Properties.Resources.chart_line;
            this.btnVisualizer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnVisualizer.Name = "btnVisualizer";
            this.btnVisualizer.Size = new System.Drawing.Size(59, 34);
            this.btnVisualizer.Text = "Visualizer";
            this.btnVisualizer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnVisualizer.Click += new System.EventHandler(this.btnVisualizer_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Image = global::MPfm.Properties.Resources.wrench;
            this.btnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(50, 34);
            this.btnSettings.Text = "Settings";
            this.btnSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // dialogAddFolder
            // 
            this.dialogAddFolder.Description = "Please select a folder to add to the music library.";
            this.dialogAddFolder.ShowNewFolderButton = false;
            // 
            // splitFirst
            // 
            this.splitFirst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.splitFirst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitFirst.Location = new System.Drawing.Point(0, 61);
            this.splitFirst.Name = "splitFirst";
            // 
            // splitFirst.Panel1
            // 
            this.splitFirst.Panel1.BackColor = System.Drawing.Color.Gray;
            this.splitFirst.Panel1.Controls.Add(this.panelLibrary);
            // 
            // splitFirst.Panel2
            // 
            this.splitFirst.Panel2.BackColor = System.Drawing.Color.Gray;
            this.splitFirst.Panel2.Controls.Add(this.tableLayoutPanel);
            this.splitFirst.Size = new System.Drawing.Size(1006, 634);
            this.splitFirst.SplitterDistance = 204;
            this.splitFirst.TabIndex = 20;
            // 
            // panelLibrary
            // 
            this.panelLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLibrary.AntiAliasingEnabled = true;
            this.panelLibrary.Controls.Add(this.lblFilterBySoundFormat);
            this.panelLibrary.Controls.Add(this.treeLibrary);
            this.panelLibrary.Controls.Add(this.comboSoundFormat);
            this.panelLibrary.ExpandedHeight = 200;
            this.panelLibrary.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLibrary.FontCollection = this.fontCollection;
            this.panelLibrary.GradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.panelLibrary.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panelLibrary.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelLibrary.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelLibrary.HeaderExpandable = false;
            this.panelLibrary.HeaderExpanded = true;
            this.panelLibrary.HeaderForeColor = System.Drawing.Color.White;
            this.panelLibrary.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.panelLibrary.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.panelLibrary.HeaderHeight = 22;
            this.panelLibrary.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelLibrary.HeaderTitle = "Library";
            this.panelLibrary.Location = new System.Drawing.Point(0, 0);
            this.panelLibrary.Name = "panelLibrary";
            this.panelLibrary.Size = new System.Drawing.Size(202, 634);
            this.panelLibrary.TabIndex = 65;
            // 
            // lblFilterBySoundFormat
            // 
            this.lblFilterBySoundFormat.AntiAliasingEnabled = true;
            this.lblFilterBySoundFormat.BackColor = System.Drawing.Color.Transparent;
            this.lblFilterBySoundFormat.CustomFontName = "Junction";
            this.lblFilterBySoundFormat.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterBySoundFormat.FontCollection = this.fontCollection;
            this.lblFilterBySoundFormat.ForeColor = System.Drawing.Color.White;
            this.lblFilterBySoundFormat.Location = new System.Drawing.Point(2, 29);
            this.lblFilterBySoundFormat.Name = "lblFilterBySoundFormat";
            this.lblFilterBySoundFormat.Size = new System.Drawing.Size(128, 14);
            this.lblFilterBySoundFormat.TabIndex = 61;
            this.lblFilterBySoundFormat.Text = "Filter by Sound Format:";
            this.lblFilterBySoundFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            customFont8.Name = "TitilliumText22L Lt";
            customFont8.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            customFont9.AssemblyPath = "MPfm.Fonts.dll";
            customFont9.Name = "BPmono";
            customFont9.ResourceName = "MPfm.Fonts.BPmono.ttf";
            customFont10.AssemblyPath = "MPfm.Fonts.dll";
            customFont10.Name = "CPmono";
            customFont10.ResourceName = "MPfm.Fonts.CPmono.ttf";
            this.fontCollection.Fonts.Add(customFont6);
            this.fontCollection.Fonts.Add(customFont7);
            this.fontCollection.Fonts.Add(customFont8);
            this.fontCollection.Fonts.Add(customFont9);
            this.fontCollection.Fonts.Add(customFont10);
            // 
            // treeLibrary
            // 
            this.treeLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeLibrary.AntiAliasingEnabled = true;
            this.treeLibrary.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeLibrary.ContextMenuStrip = this.menuLibrary;
            this.treeLibrary.CustomFontName = "Avenir";
            this.treeLibrary.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeLibrary.FontCollection = this.fontCollection;
            this.treeLibrary.GradientColor1 = System.Drawing.Color.LightGray;
            this.treeLibrary.GradientColor2 = System.Drawing.Color.Gray;
            this.treeLibrary.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.treeLibrary.HideSelection = false;
            this.treeLibrary.ImageIndex = 0;
            this.treeLibrary.ImageList = this.imageList;
            this.treeLibrary.Location = new System.Drawing.Point(3, 53);
            this.treeLibrary.Name = "treeLibrary";
            this.treeLibrary.SelectedColor = System.Drawing.Color.DarkGray;
            this.treeLibrary.SelectedImageIndex = 0;
            this.treeLibrary.Size = new System.Drawing.Size(200, 578);
            this.treeLibrary.TabIndex = 3;
            this.treeLibrary.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeLibrary_BeforeExpand);
            this.treeLibrary.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeLibrary_BeforeSelect);
            this.treeLibrary.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeLibrary_AfterSelect);
            this.treeLibrary.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeLibrary_NodeMouseClick);
            this.treeLibrary.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeLibrary_NodeMouseDoubleClick);
            // 
            // menuLibrary
            // 
            this.menuLibrary.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miTreeLibraryPlaySongs,
            this.miTreeLibraryAddSongsToPlaylist,
            this.miTreeLibraryRemoveSongsFromLibrary,
            this.miTreeLibraryDeletePlaylist});
            this.menuLibrary.Name = "menuLibrary";
            this.menuLibrary.Size = new System.Drawing.Size(186, 92);
            this.menuLibrary.Opening += new System.ComponentModel.CancelEventHandler(this.menuLibrary_Opening);
            // 
            // miTreeLibraryPlaySongs
            // 
            this.miTreeLibraryPlaySongs.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.miTreeLibraryPlaySongs.Image = global::MPfm.Properties.Resources.control_play;
            this.miTreeLibraryPlaySongs.Name = "miTreeLibraryPlaySongs";
            this.miTreeLibraryPlaySongs.Size = new System.Drawing.Size(185, 22);
            this.miTreeLibraryPlaySongs.Text = "Play selected songs";
            this.miTreeLibraryPlaySongs.Click += new System.EventHandler(this.miTreeLibraryPlaySongs_Click);
            // 
            // miTreeLibraryAddSongsToPlaylist
            // 
            this.miTreeLibraryAddSongsToPlaylist.Image = global::MPfm.Properties.Resources.add;
            this.miTreeLibraryAddSongsToPlaylist.Name = "miTreeLibraryAddSongsToPlaylist";
            this.miTreeLibraryAddSongsToPlaylist.Size = new System.Drawing.Size(185, 22);
            this.miTreeLibraryAddSongsToPlaylist.Text = "Add songs to playlist";
            this.miTreeLibraryAddSongsToPlaylist.Click += new System.EventHandler(this.miTreeLibraryAddSongsToPlaylist_Click);
            // 
            // miTreeLibraryRemoveSongsFromLibrary
            // 
            this.miTreeLibraryRemoveSongsFromLibrary.Image = global::MPfm.Properties.Resources.delete;
            this.miTreeLibraryRemoveSongsFromLibrary.Name = "miTreeLibraryRemoveSongsFromLibrary";
            this.miTreeLibraryRemoveSongsFromLibrary.Size = new System.Drawing.Size(185, 22);
            this.miTreeLibraryRemoveSongsFromLibrary.Text = "Remove from library";
            this.miTreeLibraryRemoveSongsFromLibrary.Visible = false;
            // 
            // miTreeLibraryDeletePlaylist
            // 
            this.miTreeLibraryDeletePlaylist.Image = global::MPfm.Properties.Resources.delete;
            this.miTreeLibraryDeletePlaylist.Name = "miTreeLibraryDeletePlaylist";
            this.miTreeLibraryDeletePlaylist.Size = new System.Drawing.Size(185, 22);
            this.miTreeLibraryDeletePlaylist.Text = "Delete playlist";
            this.miTreeLibraryDeletePlaylist.Click += new System.EventHandler(this.miTreeLibraryDeletePlaylist_Click);
            // 
            // comboSoundFormat
            // 
            this.comboSoundFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSoundFormat.BackColor = System.Drawing.Color.Gainsboro;
            this.comboSoundFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSoundFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboSoundFormat.ForeColor = System.Drawing.Color.Black;
            this.comboSoundFormat.FormattingEnabled = true;
            this.comboSoundFormat.Items.AddRange(new object[] {
            "MP3",
            "FLAC",
            "OGG"});
            this.comboSoundFormat.Location = new System.Drawing.Point(135, 27);
            this.comboSoundFormat.Name = "comboSoundFormat";
            this.comboSoundFormat.Size = new System.Drawing.Size(64, 21);
            this.comboSoundFormat.TabIndex = 28;
            this.comboSoundFormat.SelectedIndexChanged += new System.EventHandler(this.comboSoundFormat_SelectedIndexChanged);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.panelCurrentSong, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.splitLoopsMarkersSongBrowser, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(798, 634);
            this.tableLayoutPanel.TabIndex = 24;
            // 
            // panelCurrentSong
            // 
            this.panelCurrentSong.AntiAliasingEnabled = true;
            this.panelCurrentSong.BackColor = System.Drawing.Color.Black;
            this.panelCurrentSong.Controls.Add(this.panelCurrentSongChild);
            this.panelCurrentSong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCurrentSong.ExpandedHeight = 178;
            this.panelCurrentSong.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelCurrentSong.FontCollection = this.fontCollection;
            this.panelCurrentSong.GradientColor1 = System.Drawing.Color.Black;
            this.panelCurrentSong.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelCurrentSong.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelCurrentSong.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelCurrentSong.HeaderExpanded = true;
            this.panelCurrentSong.HeaderForeColor = System.Drawing.Color.White;
            this.panelCurrentSong.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.panelCurrentSong.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.panelCurrentSong.HeaderHeight = 22;
            this.panelCurrentSong.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelCurrentSong.HeaderTitle = "Current Song";
            this.panelCurrentSong.Location = new System.Drawing.Point(3, 3);
            this.panelCurrentSong.Name = "panelCurrentSong";
            this.panelCurrentSong.Size = new System.Drawing.Size(792, 178);
            this.panelCurrentSong.TabIndex = 26;
            // 
            // panelCurrentSongChild
            // 
            this.panelCurrentSongChild.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCurrentSongChild.AntiAliasingEnabled = true;
            this.panelCurrentSongChild.Controls.Add(this.lblCurrentFilePath);
            this.panelCurrentSongChild.Controls.Add(this.panelInformation);
            this.panelCurrentSongChild.Controls.Add(this.panelActions);
            this.panelCurrentSongChild.Controls.Add(this.lblCurrentAlbumTitle);
            this.panelCurrentSongChild.Controls.Add(this.lblCurrentArtistName);
            this.panelCurrentSongChild.Controls.Add(this.panelVolume);
            this.panelCurrentSongChild.Controls.Add(this.panelTimeShifting);
            this.panelCurrentSongChild.Controls.Add(this.panelSongPosition);
            this.panelCurrentSongChild.Controls.Add(this.panelTotalTime);
            this.panelCurrentSongChild.Controls.Add(this.panelCurrentTime);
            this.panelCurrentSongChild.Controls.Add(this.lblCurrentSongTitle);
            this.panelCurrentSongChild.Controls.Add(this.picAlbum);
            this.panelCurrentSongChild.ExpandedHeight = 200;
            this.panelCurrentSongChild.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelCurrentSongChild.FontCollection = null;
            this.panelCurrentSongChild.GradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelCurrentSongChild.GradientColor2 = System.Drawing.Color.Black;
            this.panelCurrentSongChild.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelCurrentSongChild.HeaderExpanded = true;
            this.panelCurrentSongChild.HeaderForeColor = System.Drawing.Color.Black;
            this.panelCurrentSongChild.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelCurrentSongChild.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelCurrentSongChild.HeaderHeight = 0;
            this.panelCurrentSongChild.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelCurrentSongChild.Location = new System.Drawing.Point(0, 23);
            this.panelCurrentSongChild.Name = "panelCurrentSongChild";
            this.panelCurrentSongChild.Size = new System.Drawing.Size(793, 162);
            this.panelCurrentSongChild.TabIndex = 16;
            // 
            // lblCurrentFilePath
            // 
            this.lblCurrentFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentFilePath.AntiAliasingEnabled = true;
            this.lblCurrentFilePath.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentFilePath.CustomFontName = "TitilliumText22L Lt";
            this.lblCurrentFilePath.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentFilePath.FontCollection = this.fontCollection;
            this.lblCurrentFilePath.ForeColor = System.Drawing.Color.Gray;
            this.lblCurrentFilePath.Location = new System.Drawing.Point(160, 77);
            this.lblCurrentFilePath.Name = "lblCurrentFilePath";
            this.lblCurrentFilePath.Size = new System.Drawing.Size(351, 27);
            this.lblCurrentFilePath.TabIndex = 59;
            this.lblCurrentFilePath.Text = "File Path";
            this.lblCurrentFilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelInformation
            // 
            this.panelInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInformation.AntiAliasingEnabled = true;
            this.panelInformation.Controls.Add(this.lblFrequency);
            this.panelInformation.Controls.Add(this.lblFrequencyTitle);
            this.panelInformation.Controls.Add(this.lblBitsPerSample);
            this.panelInformation.Controls.Add(this.lblBitsPerSampleTitle);
            this.panelInformation.Controls.Add(this.lblSoundFormat);
            this.panelInformation.Controls.Add(this.lblSoundFormatTitle);
            this.panelInformation.ExpandedHeight = 56;
            this.panelInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelInformation.FontCollection = this.fontCollection;
            this.panelInformation.GradientColor1 = System.Drawing.Color.Black;
            this.panelInformation.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelInformation.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelInformation.HeaderCustomFontName = "Junction";
            this.panelInformation.HeaderExpandable = false;
            this.panelInformation.HeaderExpanded = true;
            this.panelInformation.HeaderForeColor = System.Drawing.Color.White;
            this.panelInformation.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelInformation.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelInformation.HeaderHeight = 16;
            this.panelInformation.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelInformation.HeaderTitle = "Information";
            this.panelInformation.Location = new System.Drawing.Point(613, 1);
            this.panelInformation.Name = "panelInformation";
            this.panelInformation.Size = new System.Drawing.Size(100, 104);
            this.panelInformation.TabIndex = 55;
            // 
            // lblFrequency
            // 
            this.lblFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFrequency.AntiAliasingEnabled = true;
            this.lblFrequency.BackColor = System.Drawing.Color.Transparent;
            this.lblFrequency.CustomFontName = "BPmono";
            this.lblFrequency.Font = new System.Drawing.Font("Arial", 7F);
            this.lblFrequency.FontCollection = this.fontCollection;
            this.lblFrequency.ForeColor = System.Drawing.Color.White;
            this.lblFrequency.Location = new System.Drawing.Point(4, 58);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(82, 8);
            this.lblFrequency.TabIndex = 65;
            this.lblFrequency.Text = "Frequency";
            this.lblFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFrequencyTitle
            // 
            this.lblFrequencyTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFrequencyTitle.AntiAliasingEnabled = true;
            this.lblFrequencyTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblFrequencyTitle.CustomFontName = "Junction";
            this.lblFrequencyTitle.Font = new System.Drawing.Font("Arial", 7F);
            this.lblFrequencyTitle.FontCollection = this.fontCollection;
            this.lblFrequencyTitle.ForeColor = System.Drawing.Color.Silver;
            this.lblFrequencyTitle.Location = new System.Drawing.Point(4, 46);
            this.lblFrequencyTitle.Name = "lblFrequencyTitle";
            this.lblFrequencyTitle.Size = new System.Drawing.Size(82, 10);
            this.lblFrequencyTitle.TabIndex = 64;
            this.lblFrequencyTitle.Text = "Frequency";
            this.lblFrequencyTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBitsPerSample
            // 
            this.lblBitsPerSample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBitsPerSample.AntiAliasingEnabled = true;
            this.lblBitsPerSample.BackColor = System.Drawing.Color.Transparent;
            this.lblBitsPerSample.CustomFontName = "BPmono";
            this.lblBitsPerSample.Font = new System.Drawing.Font("Arial", 7F);
            this.lblBitsPerSample.FontCollection = this.fontCollection;
            this.lblBitsPerSample.ForeColor = System.Drawing.Color.White;
            this.lblBitsPerSample.Location = new System.Drawing.Point(4, 83);
            this.lblBitsPerSample.Name = "lblBitsPerSample";
            this.lblBitsPerSample.Size = new System.Drawing.Size(82, 8);
            this.lblBitsPerSample.TabIndex = 63;
            this.lblBitsPerSample.Text = "Bits per sample";
            this.lblBitsPerSample.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBitsPerSampleTitle
            // 
            this.lblBitsPerSampleTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBitsPerSampleTitle.AntiAliasingEnabled = true;
            this.lblBitsPerSampleTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblBitsPerSampleTitle.CustomFontName = "Junction";
            this.lblBitsPerSampleTitle.Font = new System.Drawing.Font("Arial", 7F);
            this.lblBitsPerSampleTitle.FontCollection = this.fontCollection;
            this.lblBitsPerSampleTitle.ForeColor = System.Drawing.Color.Silver;
            this.lblBitsPerSampleTitle.Location = new System.Drawing.Point(4, 71);
            this.lblBitsPerSampleTitle.Name = "lblBitsPerSampleTitle";
            this.lblBitsPerSampleTitle.Size = new System.Drawing.Size(82, 10);
            this.lblBitsPerSampleTitle.TabIndex = 62;
            this.lblBitsPerSampleTitle.Text = "Bits per sample";
            this.lblBitsPerSampleTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSoundFormat
            // 
            this.lblSoundFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSoundFormat.AntiAliasingEnabled = true;
            this.lblSoundFormat.BackColor = System.Drawing.Color.Transparent;
            this.lblSoundFormat.CustomFontName = "BPmono";
            this.lblSoundFormat.Font = new System.Drawing.Font("Arial", 7F);
            this.lblSoundFormat.FontCollection = this.fontCollection;
            this.lblSoundFormat.ForeColor = System.Drawing.Color.White;
            this.lblSoundFormat.Location = new System.Drawing.Point(4, 33);
            this.lblSoundFormat.Name = "lblSoundFormat";
            this.lblSoundFormat.Size = new System.Drawing.Size(82, 8);
            this.lblSoundFormat.TabIndex = 61;
            this.lblSoundFormat.Text = "Sound Format";
            this.lblSoundFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSoundFormatTitle
            // 
            this.lblSoundFormatTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSoundFormatTitle.AntiAliasingEnabled = true;
            this.lblSoundFormatTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSoundFormatTitle.CustomFontName = "Junction";
            this.lblSoundFormatTitle.Font = new System.Drawing.Font("Arial", 7F);
            this.lblSoundFormatTitle.FontCollection = this.fontCollection;
            this.lblSoundFormatTitle.ForeColor = System.Drawing.Color.Silver;
            this.lblSoundFormatTitle.Location = new System.Drawing.Point(4, 21);
            this.lblSoundFormatTitle.Name = "lblSoundFormatTitle";
            this.lblSoundFormatTitle.Size = new System.Drawing.Size(82, 10);
            this.lblSoundFormatTitle.TabIndex = 60;
            this.lblSoundFormatTitle.Text = "Sound Format";
            this.lblSoundFormatTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelActions
            // 
            this.panelActions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelActions.AntiAliasingEnabled = true;
            this.panelActions.Controls.Add(this.lblSearchWeb);
            this.panelActions.Controls.Add(this.linkEditSongMetadata);
            this.panelActions.Controls.Add(this.linkSearchLyrics);
            this.panelActions.Controls.Add(this.linkSearchBassTabs);
            this.panelActions.Controls.Add(this.linkSearchGuitarTabs);
            this.panelActions.ExpandedHeight = 56;
            this.panelActions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelActions.FontCollection = this.fontCollection;
            this.panelActions.GradientColor1 = System.Drawing.Color.Black;
            this.panelActions.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelActions.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelActions.HeaderCustomFontName = "Junction";
            this.panelActions.HeaderExpandable = false;
            this.panelActions.HeaderExpanded = true;
            this.panelActions.HeaderForeColor = System.Drawing.Color.White;
            this.panelActions.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelActions.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelActions.HeaderHeight = 16;
            this.panelActions.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelActions.HeaderTitle = "Actions";
            this.panelActions.Location = new System.Drawing.Point(512, 1);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(100, 104);
            this.panelActions.TabIndex = 58;
            // 
            // lblSearchWeb
            // 
            this.lblSearchWeb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearchWeb.AntiAliasingEnabled = true;
            this.lblSearchWeb.BackColor = System.Drawing.Color.Transparent;
            this.lblSearchWeb.CustomFontName = "Junction";
            this.lblSearchWeb.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchWeb.FontCollection = this.fontCollection;
            this.lblSearchWeb.ForeColor = System.Drawing.Color.Silver;
            this.lblSearchWeb.Location = new System.Drawing.Point(1, 38);
            this.lblSearchWeb.Name = "lblSearchWeb";
            this.lblSearchWeb.Size = new System.Drawing.Size(96, 14);
            this.lblSearchWeb.TabIndex = 63;
            this.lblSearchWeb.Text = "Search the web for:";
            this.lblSearchWeb.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkEditSongMetadata
            // 
            this.linkEditSongMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkEditSongMetadata.AntiAliasingEnabled = true;
            this.linkEditSongMetadata.BackColor = System.Drawing.Color.Transparent;
            this.linkEditSongMetadata.CustomFontName = "Junction";
            this.linkEditSongMetadata.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkEditSongMetadata.FontCollection = this.fontCollection;
            this.linkEditSongMetadata.ForeColor = System.Drawing.Color.White;
            this.linkEditSongMetadata.Location = new System.Drawing.Point(2, 20);
            this.linkEditSongMetadata.Name = "linkEditSongMetadata";
            this.linkEditSongMetadata.Size = new System.Drawing.Size(96, 15);
            this.linkEditSongMetadata.TabIndex = 60;
            this.linkEditSongMetadata.TabStop = true;
            this.linkEditSongMetadata.Text = "Edit song metadata";
            this.linkEditSongMetadata.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEditSongMetadata_LinkClicked);
            // 
            // linkSearchLyrics
            // 
            this.linkSearchLyrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkSearchLyrics.AntiAliasingEnabled = true;
            this.linkSearchLyrics.BackColor = System.Drawing.Color.Transparent;
            this.linkSearchLyrics.CustomFontName = "Junction";
            this.linkSearchLyrics.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkSearchLyrics.FontCollection = this.fontCollection;
            this.linkSearchLyrics.ForeColor = System.Drawing.Color.White;
            this.linkSearchLyrics.Location = new System.Drawing.Point(2, 85);
            this.linkSearchLyrics.Name = "linkSearchLyrics";
            this.linkSearchLyrics.Size = new System.Drawing.Size(79, 15);
            this.linkSearchLyrics.TabIndex = 59;
            this.linkSearchLyrics.TabStop = true;
            this.linkSearchLyrics.Text = "Lyrics";
            this.linkSearchLyrics.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSearchLyrics_LinkClicked);
            // 
            // linkSearchBassTabs
            // 
            this.linkSearchBassTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkSearchBassTabs.AntiAliasingEnabled = true;
            this.linkSearchBassTabs.BackColor = System.Drawing.Color.Transparent;
            this.linkSearchBassTabs.CustomFontName = "Junction";
            this.linkSearchBassTabs.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkSearchBassTabs.FontCollection = this.fontCollection;
            this.linkSearchBassTabs.ForeColor = System.Drawing.Color.White;
            this.linkSearchBassTabs.Location = new System.Drawing.Point(2, 69);
            this.linkSearchBassTabs.Name = "linkSearchBassTabs";
            this.linkSearchBassTabs.Size = new System.Drawing.Size(79, 15);
            this.linkSearchBassTabs.TabIndex = 58;
            this.linkSearchBassTabs.TabStop = true;
            this.linkSearchBassTabs.Text = "Bass tabs";
            this.linkSearchBassTabs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSearchBassTabs_LinkClicked);
            // 
            // linkSearchGuitarTabs
            // 
            this.linkSearchGuitarTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkSearchGuitarTabs.AntiAliasingEnabled = true;
            this.linkSearchGuitarTabs.BackColor = System.Drawing.Color.Transparent;
            this.linkSearchGuitarTabs.CustomFontName = "Junction";
            this.linkSearchGuitarTabs.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkSearchGuitarTabs.FontCollection = this.fontCollection;
            this.linkSearchGuitarTabs.ForeColor = System.Drawing.Color.White;
            this.linkSearchGuitarTabs.Location = new System.Drawing.Point(2, 53);
            this.linkSearchGuitarTabs.Name = "linkSearchGuitarTabs";
            this.linkSearchGuitarTabs.Size = new System.Drawing.Size(79, 15);
            this.linkSearchGuitarTabs.TabIndex = 57;
            this.linkSearchGuitarTabs.TabStop = true;
            this.linkSearchGuitarTabs.Text = "Guitar tabs";
            this.linkSearchGuitarTabs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSearchGuitarTabs_LinkClicked);
            // 
            // lblCurrentAlbumTitle
            // 
            this.lblCurrentAlbumTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentAlbumTitle.AntiAliasingEnabled = true;
            this.lblCurrentAlbumTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentAlbumTitle.CustomFontName = "TitilliumText22L Lt";
            this.lblCurrentAlbumTitle.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentAlbumTitle.FontCollection = this.fontCollection;
            this.lblCurrentAlbumTitle.ForeColor = System.Drawing.Color.Silver;
            this.lblCurrentAlbumTitle.Location = new System.Drawing.Point(158, 31);
            this.lblCurrentAlbumTitle.Name = "lblCurrentAlbumTitle";
            this.lblCurrentAlbumTitle.Size = new System.Drawing.Size(351, 26);
            this.lblCurrentAlbumTitle.TabIndex = 57;
            this.lblCurrentAlbumTitle.TabStop = true;
            this.lblCurrentAlbumTitle.Text = "Album";
            // 
            // lblCurrentArtistName
            // 
            this.lblCurrentArtistName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentArtistName.AntiAliasingEnabled = true;
            this.lblCurrentArtistName.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentArtistName.CustomFontName = "TitilliumText22L Lt";
            this.lblCurrentArtistName.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentArtistName.FontCollection = this.fontCollection;
            this.lblCurrentArtistName.ForeColor = System.Drawing.Color.White;
            this.lblCurrentArtistName.Location = new System.Drawing.Point(157, 0);
            this.lblCurrentArtistName.Name = "lblCurrentArtistName";
            this.lblCurrentArtistName.Size = new System.Drawing.Size(351, 37);
            this.lblCurrentArtistName.TabIndex = 56;
            this.lblCurrentArtistName.TabStop = true;
            this.lblCurrentArtistName.Text = "Artist";
            // 
            // panelVolume
            // 
            this.panelVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVolume.AntiAliasingEnabled = true;
            this.panelVolume.Controls.Add(this.picDistortionWarning);
            this.panelVolume.Controls.Add(this.outputMeter);
            this.panelVolume.Controls.Add(this.faderVolume);
            this.panelVolume.Controls.Add(this.lblVolume);
            this.panelVolume.ExpandedHeight = 56;
            this.panelVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelVolume.FontCollection = this.fontCollection;
            this.panelVolume.GradientColor1 = System.Drawing.Color.Black;
            this.panelVolume.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelVolume.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelVolume.HeaderCustomFontName = "Junction";
            this.panelVolume.HeaderExpandable = false;
            this.panelVolume.HeaderExpanded = true;
            this.panelVolume.HeaderForeColor = System.Drawing.Color.White;
            this.panelVolume.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelVolume.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelVolume.HeaderHeight = 16;
            this.panelVolume.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelVolume.HeaderTitle = "Volume";
            this.panelVolume.Location = new System.Drawing.Point(714, 1);
            this.panelVolume.Name = "panelVolume";
            this.panelVolume.Size = new System.Drawing.Size(78, 153);
            this.panelVolume.TabIndex = 56;
            // 
            // picDistortionWarning
            // 
            this.picDistortionWarning.BackColor = System.Drawing.Color.Red;
            this.picDistortionWarning.Location = new System.Drawing.Point(29, 19);
            this.picDistortionWarning.Name = "picDistortionWarning";
            this.picDistortionWarning.Size = new System.Drawing.Size(44, 6);
            this.picDistortionWarning.TabIndex = 69;
            this.picDistortionWarning.TabStop = false;
            this.picDistortionWarning.Visible = false;
            this.picDistortionWarning.Click += new System.EventHandler(this.picDistortionWarning_Click);
            // 
            // outputMeter
            // 
            this.outputMeter.BorderColor = System.Drawing.Color.Empty;
            this.outputMeter.BorderWidth = 0;
            this.outputMeter.CustomFontName = "LeagueGothic";
            this.outputMeter.DisplayDecibels = true;
            this.outputMeter.DisplayType = MPfm.WindowsControls.OutputMeterDisplayType.Stereo;
            this.outputMeter.DistortionThreshold = 0.9F;
            this.outputMeter.DrawFloor = 0F;
            this.outputMeter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputMeter.FontCollection = this.fontCollection;
            this.outputMeter.FontColor = System.Drawing.Color.White;
            this.outputMeter.FontShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.outputMeter.GradientColor1 = System.Drawing.Color.Black;
            this.outputMeter.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.outputMeter.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.outputMeter.Location = new System.Drawing.Point(29, 26);
            this.outputMeter.Meter0dbLineColor = System.Drawing.Color.Gray;
            this.outputMeter.MeterDistortionGradientColor1 = System.Drawing.Color.Red;
            this.outputMeter.MeterDistortionGradientColor2 = System.Drawing.Color.DarkRed;
            this.outputMeter.MeterGradientColor1 = System.Drawing.Color.PaleGreen;
            this.outputMeter.MeterGradientColor2 = System.Drawing.Color.DarkGreen;
            this.outputMeter.MeterPeakLineColor = System.Drawing.Color.OliveDrab;
            this.outputMeter.Name = "outputMeter";
            this.outputMeter.Size = new System.Drawing.Size(44, 110);
            this.outputMeter.TabIndex = 60;
            // 
            // faderVolume
            // 
            this.faderVolume.CenterLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.faderVolume.CenterLineShadowColor = System.Drawing.Color.Gray;
            this.faderVolume.CustomFontName = null;
            this.faderVolume.FaderGradientColor1 = System.Drawing.Color.Gainsboro;
            this.faderVolume.FaderGradientColor2 = System.Drawing.Color.Gainsboro;
            this.faderVolume.FaderHeight = 28;
            this.faderVolume.FaderMiddleLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.faderVolume.FaderShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.faderVolume.FaderShadowGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(188)))), ((int)(((byte)(188)))));
            this.faderVolume.FaderShadowGradientColor2 = System.Drawing.Color.Gainsboro;
            this.faderVolume.FaderWidth = 10;
            this.faderVolume.FontCollection = this.fontCollection;
            this.faderVolume.GradientColor1 = System.Drawing.Color.Black;
            this.faderVolume.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.faderVolume.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.faderVolume.Location = new System.Drawing.Point(0, 17);
            this.faderVolume.Maximum = 100;
            this.faderVolume.Name = "faderVolume";
            this.faderVolume.Size = new System.Drawing.Size(24, 124);
            this.faderVolume.StepSize = 10;
            this.faderVolume.TabIndex = 68;
            this.faderVolume.Value = 50;
            this.faderVolume.OnFaderValueChanged += new MPfm.WindowsControls.VolumeFader.FaderValueChanged(this.faderVolume_OnFaderValueChanged);
            // 
            // lblVolume
            // 
            this.lblVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVolume.AntiAliasingEnabled = true;
            this.lblVolume.BackColor = System.Drawing.Color.Transparent;
            this.lblVolume.CustomFontName = "";
            this.lblVolume.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolume.FontCollection = this.fontCollection;
            this.lblVolume.ForeColor = System.Drawing.Color.White;
            this.lblVolume.Location = new System.Drawing.Point(4, 139);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(68, 15);
            this.lblVolume.TabIndex = 30;
            this.lblVolume.Text = "100%";
            this.lblVolume.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // panelTimeShifting
            // 
            this.panelTimeShifting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTimeShifting.AntiAliasingEnabled = true;
            this.panelTimeShifting.Controls.Add(this.lblTimeShifting);
            this.panelTimeShifting.Controls.Add(this.linkResetTimeShifting);
            this.panelTimeShifting.Controls.Add(this.trackTimeShifting);
            this.panelTimeShifting.ExpandedHeight = 200;
            this.panelTimeShifting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelTimeShifting.FontCollection = this.fontCollection;
            this.panelTimeShifting.GradientColor1 = System.Drawing.Color.Black;
            this.panelTimeShifting.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelTimeShifting.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelTimeShifting.HeaderCustomFontName = "Junction";
            this.panelTimeShifting.HeaderExpandable = false;
            this.panelTimeShifting.HeaderExpanded = true;
            this.panelTimeShifting.HeaderForeColor = System.Drawing.Color.White;
            this.panelTimeShifting.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelTimeShifting.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelTimeShifting.HeaderHeight = 16;
            this.panelTimeShifting.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelTimeShifting.HeaderTitle = "Time Shifting";
            this.panelTimeShifting.Location = new System.Drawing.Point(613, 106);
            this.panelTimeShifting.Name = "panelTimeShifting";
            this.panelTimeShifting.Size = new System.Drawing.Size(100, 48);
            this.panelTimeShifting.TabIndex = 53;
            // 
            // lblTimeShifting
            // 
            this.lblTimeShifting.AntiAliasingEnabled = true;
            this.lblTimeShifting.BackColor = System.Drawing.Color.Transparent;
            this.lblTimeShifting.CustomFontName = "";
            this.lblTimeShifting.Font = new System.Drawing.Font("Droid Sans Mono", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeShifting.FontCollection = this.fontCollection;
            this.lblTimeShifting.ForeColor = System.Drawing.Color.White;
            this.lblTimeShifting.Location = new System.Drawing.Point(57, 36);
            this.lblTimeShifting.Name = "lblTimeShifting";
            this.lblTimeShifting.Size = new System.Drawing.Size(42, 12);
            this.lblTimeShifting.TabIndex = 30;
            this.lblTimeShifting.Text = "100 %";
            this.lblTimeShifting.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // linkResetTimeShifting
            // 
            this.linkResetTimeShifting.AntiAliasingEnabled = true;
            this.linkResetTimeShifting.BackColor = System.Drawing.Color.Transparent;
            this.linkResetTimeShifting.CustomFontName = "";
            this.linkResetTimeShifting.Font = new System.Drawing.Font("Droid Sans Mono", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkResetTimeShifting.FontCollection = this.fontCollection;
            this.linkResetTimeShifting.ForeColor = System.Drawing.Color.LightGray;
            this.linkResetTimeShifting.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.linkResetTimeShifting.Location = new System.Drawing.Point(1, 34);
            this.linkResetTimeShifting.Name = "linkResetTimeShifting";
            this.linkResetTimeShifting.Size = new System.Drawing.Size(30, 12);
            this.linkResetTimeShifting.TabIndex = 58;
            this.linkResetTimeShifting.TabStop = true;
            this.linkResetTimeShifting.Text = "Reset";
            this.linkResetTimeShifting.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkResetTimeShifting_LinkClicked);
            // 
            // trackTimeShifting
            // 
            this.trackTimeShifting.CenterLineColor = System.Drawing.Color.Gray;
            this.trackTimeShifting.CenterLineShadowColor = System.Drawing.Color.Black;
            this.trackTimeShifting.CustomFontName = null;
            this.trackTimeShifting.FaderGradientColor1 = System.Drawing.Color.White;
            this.trackTimeShifting.FaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trackTimeShifting.FaderHeight = 8;
            this.trackTimeShifting.FaderShadowGradientColor1 = System.Drawing.Color.Gray;
            this.trackTimeShifting.FaderShadowGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.trackTimeShifting.FaderWidth = 16;
            this.trackTimeShifting.FontCollection = null;
            this.trackTimeShifting.GradientColor1 = System.Drawing.Color.Black;
            this.trackTimeShifting.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.trackTimeShifting.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.trackTimeShifting.Location = new System.Drawing.Point(0, 17);
            this.trackTimeShifting.Maximum = 150;
            this.trackTimeShifting.Minimum = 50;
            this.trackTimeShifting.Name = "trackTimeShifting";
            this.trackTimeShifting.Size = new System.Drawing.Size(100, 20);
            this.trackTimeShifting.StepSize = 5;
            this.trackTimeShifting.TabIndex = 32;
            this.trackTimeShifting.Text = "TrackBar1";
            this.trackTimeShifting.Value = 100;
            this.trackTimeShifting.OnTrackBarValueChanged += new MPfm.WindowsControls.TrackBar.TrackBarValueChanged(this.trackTimeShiftingNew_OnTrackBarValueChanged);
            // 
            // panelSongPosition
            // 
            this.panelSongPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSongPosition.AntiAliasingEnabled = true;
            this.panelSongPosition.Controls.Add(this.lblSongPercentage);
            this.panelSongPosition.Controls.Add(this.lblSongPosition);
            this.panelSongPosition.Controls.Add(this.trackPosition);
            this.panelSongPosition.ExpandedHeight = 200;
            this.panelSongPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelSongPosition.FontCollection = this.fontCollection;
            this.panelSongPosition.GradientColor1 = System.Drawing.Color.Black;
            this.panelSongPosition.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelSongPosition.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelSongPosition.HeaderCustomFontName = "Junction";
            this.panelSongPosition.HeaderExpandable = false;
            this.panelSongPosition.HeaderExpanded = true;
            this.panelSongPosition.HeaderForeColor = System.Drawing.Color.White;
            this.panelSongPosition.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelSongPosition.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelSongPosition.HeaderHeight = 16;
            this.panelSongPosition.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelSongPosition.HeaderTitle = "Song Position";
            this.panelSongPosition.Location = new System.Drawing.Point(365, 106);
            this.panelSongPosition.Name = "panelSongPosition";
            this.panelSongPosition.Size = new System.Drawing.Size(247, 48);
            this.panelSongPosition.TabIndex = 52;
            // 
            // lblSongPercentage
            // 
            this.lblSongPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSongPercentage.AntiAliasingEnabled = true;
            this.lblSongPercentage.BackColor = System.Drawing.Color.Transparent;
            this.lblSongPercentage.CustomFontName = "";
            this.lblSongPercentage.Font = new System.Drawing.Font("Droid Sans Mono", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSongPercentage.FontCollection = this.fontCollection;
            this.lblSongPercentage.ForeColor = System.Drawing.Color.White;
            this.lblSongPercentage.Location = new System.Drawing.Point(201, 36);
            this.lblSongPercentage.Name = "lblSongPercentage";
            this.lblSongPercentage.Size = new System.Drawing.Size(44, 12);
            this.lblSongPercentage.TabIndex = 30;
            this.lblSongPercentage.Text = "0 %";
            this.lblSongPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSongPosition
            // 
            this.lblSongPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSongPosition.AntiAliasingEnabled = true;
            this.lblSongPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblSongPosition.CustomFontName = "";
            this.lblSongPosition.Font = new System.Drawing.Font("Droid Sans Mono", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSongPosition.FontCollection = this.fontCollection;
            this.lblSongPosition.ForeColor = System.Drawing.Color.White;
            this.lblSongPosition.Location = new System.Drawing.Point(4, 36);
            this.lblSongPosition.Name = "lblSongPosition";
            this.lblSongPosition.Size = new System.Drawing.Size(245, 12);
            this.lblSongPosition.TabIndex = 64;
            this.lblSongPosition.Text = "0:00.000";
            this.lblSongPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trackPosition
            // 
            this.trackPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackPosition.CenterLineColor = System.Drawing.Color.Gray;
            this.trackPosition.CenterLineShadowColor = System.Drawing.Color.Black;
            this.trackPosition.CustomFontName = null;
            this.trackPosition.FaderGradientColor1 = System.Drawing.Color.White;
            this.trackPosition.FaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.trackPosition.FaderHeight = 8;
            this.trackPosition.FaderShadowGradientColor1 = System.Drawing.Color.Gray;
            this.trackPosition.FaderShadowGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.trackPosition.FaderWidth = 16;
            this.trackPosition.FontCollection = null;
            this.trackPosition.GradientColor1 = System.Drawing.Color.Black;
            this.trackPosition.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.trackPosition.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.trackPosition.Location = new System.Drawing.Point(0, 17);
            this.trackPosition.Maximum = 999;
            this.trackPosition.Name = "trackPosition";
            this.trackPosition.Size = new System.Drawing.Size(247, 20);
            this.trackPosition.StepSize = 5;
            this.trackPosition.TabIndex = 31;
            this.trackPosition.Text = "TrackBar1";
            this.trackPosition.Value = 5;
            this.trackPosition.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackPosition_MouseDown);
            this.trackPosition.MouseMove += new System.Windows.Forms.MouseEventHandler(this.trackPosition_MouseMove);
            this.trackPosition.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackPosition_MouseUp);
            // 
            // panelTotalTime
            // 
            this.panelTotalTime.AntiAliasingEnabled = true;
            this.panelTotalTime.Controls.Add(this.lblTotalTime);
            this.panelTotalTime.ExpandedHeight = 200;
            this.panelTotalTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelTotalTime.FontCollection = this.fontCollection;
            this.panelTotalTime.GradientColor1 = System.Drawing.Color.Black;
            this.panelTotalTime.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelTotalTime.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelTotalTime.HeaderCustomFontName = "Junction";
            this.panelTotalTime.HeaderExpandable = false;
            this.panelTotalTime.HeaderExpanded = true;
            this.panelTotalTime.HeaderForeColor = System.Drawing.Color.White;
            this.panelTotalTime.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelTotalTime.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelTotalTime.HeaderHeight = 16;
            this.panelTotalTime.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelTotalTime.HeaderTitle = "Total Time";
            this.panelTotalTime.Location = new System.Drawing.Point(261, 106);
            this.panelTotalTime.Name = "panelTotalTime";
            this.panelTotalTime.Size = new System.Drawing.Size(103, 48);
            this.panelTotalTime.TabIndex = 50;
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.AntiAliasingEnabled = true;
            this.lblTotalTime.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalTime.CustomFontName = "";
            this.lblTotalTime.Font = new System.Drawing.Font("Droid Sans Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTime.FontCollection = this.fontCollection;
            this.lblTotalTime.ForeColor = System.Drawing.Color.White;
            this.lblTotalTime.Location = new System.Drawing.Point(1, 22);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(99, 22);
            this.lblTotalTime.TabIndex = 23;
            this.lblTotalTime.Text = "00:00.000";
            this.lblTotalTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelCurrentTime
            // 
            this.panelCurrentTime.AntiAliasingEnabled = true;
            this.panelCurrentTime.Controls.Add(this.lblCurrentTime);
            this.panelCurrentTime.ExpandedHeight = 200;
            this.panelCurrentTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelCurrentTime.FontCollection = this.fontCollection;
            this.panelCurrentTime.GradientColor1 = System.Drawing.Color.Black;
            this.panelCurrentTime.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelCurrentTime.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelCurrentTime.HeaderCustomFontName = "Junction";
            this.panelCurrentTime.HeaderExpandable = false;
            this.panelCurrentTime.HeaderExpanded = true;
            this.panelCurrentTime.HeaderForeColor = System.Drawing.Color.White;
            this.panelCurrentTime.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelCurrentTime.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelCurrentTime.HeaderHeight = 16;
            this.panelCurrentTime.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelCurrentTime.HeaderTitle = "Current Time";
            this.panelCurrentTime.Location = new System.Drawing.Point(157, 106);
            this.panelCurrentTime.Name = "panelCurrentTime";
            this.panelCurrentTime.Size = new System.Drawing.Size(103, 48);
            this.panelCurrentTime.TabIndex = 21;
            // 
            // lblCurrentTime
            // 
            this.lblCurrentTime.AntiAliasingEnabled = true;
            this.lblCurrentTime.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentTime.CustomFontName = "";
            this.lblCurrentTime.Font = new System.Drawing.Font("Droid Sans Mono", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTime.FontCollection = this.fontCollection;
            this.lblCurrentTime.ForeColor = System.Drawing.Color.White;
            this.lblCurrentTime.Location = new System.Drawing.Point(0, 22);
            this.lblCurrentTime.Name = "lblCurrentTime";
            this.lblCurrentTime.Size = new System.Drawing.Size(103, 22);
            this.lblCurrentTime.TabIndex = 23;
            this.lblCurrentTime.Text = "00:00.000";
            this.lblCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurrentSongTitle
            // 
            this.lblCurrentSongTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentSongTitle.AntiAliasingEnabled = true;
            this.lblCurrentSongTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentSongTitle.CustomFontName = "TitilliumText22L Lt";
            this.lblCurrentSongTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentSongTitle.FontCollection = this.fontCollection;
            this.lblCurrentSongTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblCurrentSongTitle.Location = new System.Drawing.Point(158, 55);
            this.lblCurrentSongTitle.Name = "lblCurrentSongTitle";
            this.lblCurrentSongTitle.Size = new System.Drawing.Size(351, 25);
            this.lblCurrentSongTitle.TabIndex = 44;
            this.lblCurrentSongTitle.Text = "Song Title";
            this.lblCurrentSongTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picAlbum
            // 
            this.picAlbum.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.picAlbum.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAlbum.ImageLocation = "";
            this.picAlbum.Location = new System.Drawing.Point(1, -1);
            this.picAlbum.Name = "picAlbum";
            this.picAlbum.Size = new System.Drawing.Size(155, 155);
            this.picAlbum.TabIndex = 27;
            this.picAlbum.TabStop = false;
            this.picAlbum.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picAlbum_MouseClick);
            // 
            // splitLoopsMarkersSongBrowser
            // 
            this.splitLoopsMarkersSongBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitLoopsMarkersSongBrowser.Location = new System.Drawing.Point(3, 187);
            this.splitLoopsMarkersSongBrowser.Name = "splitLoopsMarkersSongBrowser";
            this.splitLoopsMarkersSongBrowser.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitLoopsMarkersSongBrowser.Panel1
            // 
            this.splitLoopsMarkersSongBrowser.Panel1.BackColor = System.Drawing.Color.Gray;
            this.splitLoopsMarkersSongBrowser.Panel1.Controls.Add(this.panelLoopsMarkers);
            this.splitLoopsMarkersSongBrowser.Panel1MinSize = 22;
            // 
            // splitLoopsMarkersSongBrowser.Panel2
            // 
            this.splitLoopsMarkersSongBrowser.Panel2.Controls.Add(this.panelSongBrowser);
            this.splitLoopsMarkersSongBrowser.Panel2MinSize = 22;
            this.splitLoopsMarkersSongBrowser.Size = new System.Drawing.Size(792, 444);
            this.splitLoopsMarkersSongBrowser.SplitterDistance = 222;
            this.splitLoopsMarkersSongBrowser.TabIndex = 27;
            // 
            // panelLoopsMarkers
            // 
            this.panelLoopsMarkers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLoopsMarkers.AntiAliasingEnabled = true;
            this.panelLoopsMarkers.BackColor = System.Drawing.Color.Gray;
            this.panelLoopsMarkers.Controls.Add(this.splitWaveFormLoopsMarkers);
            this.panelLoopsMarkers.ExpandedHeight = 266;
            this.panelLoopsMarkers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLoopsMarkers.FontCollection = this.fontCollection;
            this.panelLoopsMarkers.GradientColor1 = System.Drawing.Color.Black;
            this.panelLoopsMarkers.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelLoopsMarkers.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelLoopsMarkers.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelLoopsMarkers.HeaderExpandable = false;
            this.panelLoopsMarkers.HeaderForeColor = System.Drawing.Color.White;
            this.panelLoopsMarkers.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.panelLoopsMarkers.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.panelLoopsMarkers.HeaderHeight = 22;
            this.panelLoopsMarkers.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelLoopsMarkers.HeaderTitle = "Loops & Markers";
            this.panelLoopsMarkers.Location = new System.Drawing.Point(0, 0);
            this.panelLoopsMarkers.Name = "panelLoopsMarkers";
            this.panelLoopsMarkers.Size = new System.Drawing.Size(792, 222);
            this.panelLoopsMarkers.TabIndex = 23;
            this.panelLoopsMarkers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelLoopsMarkers_MouseDoubleClick);
            // 
            // splitWaveFormLoopsMarkers
            // 
            this.splitWaveFormLoopsMarkers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitWaveFormLoopsMarkers.Location = new System.Drawing.Point(0, 22);
            this.splitWaveFormLoopsMarkers.Name = "splitWaveFormLoopsMarkers";
            this.splitWaveFormLoopsMarkers.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitWaveFormLoopsMarkers.Panel1
            // 
            this.splitWaveFormLoopsMarkers.Panel1.Controls.Add(this.waveFormMarkersLoops);
            // 
            // splitWaveFormLoopsMarkers.Panel2
            // 
            this.splitWaveFormLoopsMarkers.Panel2.Controls.Add(this.splitLoopsMarkers);
            this.splitWaveFormLoopsMarkers.Size = new System.Drawing.Size(792, 200);
            this.splitWaveFormLoopsMarkers.SplitterDistance = 56;
            this.splitWaveFormLoopsMarkers.TabIndex = 76;
            // 
            // waveFormMarkersLoops
            // 
            this.waveFormMarkersLoops.AutoScrollWithCursor = true;
            this.waveFormMarkersLoops.BorderColor = System.Drawing.Color.Empty;
            this.waveFormMarkersLoops.BorderWidth = 0;
            this.waveFormMarkersLoops.CurrentPositionMS = ((uint)(0u));
            this.waveFormMarkersLoops.CurrentPositionPCMBytes = ((uint)(0u));
            this.waveFormMarkersLoops.CursorColor = System.Drawing.Color.RoyalBlue;
            this.waveFormMarkersLoops.CustomFontName = "BPmono";
            this.waveFormMarkersLoops.DisplayCurrentPosition = true;
            this.waveFormMarkersLoops.DisplayType = MPfm.WindowsControls.WaveFormDisplayType.Stereo;
            this.waveFormMarkersLoops.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waveFormMarkersLoops.Font = new System.Drawing.Font("Arial", 8F);
            this.waveFormMarkersLoops.FontCollection = this.fontCollection;
            this.waveFormMarkersLoops.GradientColor1 = System.Drawing.Color.Black;
            this.waveFormMarkersLoops.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.waveFormMarkersLoops.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.waveFormMarkersLoops.Location = new System.Drawing.Point(0, 0);
            this.waveFormMarkersLoops.Name = "waveFormMarkersLoops";
            this.waveFormMarkersLoops.PeakFileDirectory = "C:\\Users\\Animal Mother\\AppData\\Local\\Microsoft\\VisualStudio\\10.0\\ProjectAssemblie" +
    "s\\om-0gycd01\\Peak Files\\";
            this.waveFormMarkersLoops.Size = new System.Drawing.Size(792, 56);
            this.waveFormMarkersLoops.TabIndex = 75;
            this.waveFormMarkersLoops.TotalMS = ((uint)(0u));
            this.waveFormMarkersLoops.TotalPCMBytes = ((uint)(0u));
            this.waveFormMarkersLoops.WaveFormColor = System.Drawing.Color.Yellow;
            this.waveFormMarkersLoops.Zoom = 100F;
            this.waveFormMarkersLoops.OnPositionChanged += new MPfm.WindowsControls.WaveFormMarkersLoops.PositionChanged(this.waveFormMarkersLoops_OnPositionChanged);
            // 
            // splitLoopsMarkers
            // 
            this.splitLoopsMarkers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitLoopsMarkers.Location = new System.Drawing.Point(0, 0);
            this.splitLoopsMarkers.Name = "splitLoopsMarkers";
            // 
            // splitLoopsMarkers.Panel1
            // 
            this.splitLoopsMarkers.Panel1.BackColor = System.Drawing.Color.DarkGray;
            this.splitLoopsMarkers.Panel1.Controls.Add(this.btnStopLoop);
            this.splitLoopsMarkers.Panel1.Controls.Add(this.btnEditLoop);
            this.splitLoopsMarkers.Panel1.Controls.Add(this.btnPlayLoop);
            this.splitLoopsMarkers.Panel1.Controls.Add(this.lblLoops);
            this.splitLoopsMarkers.Panel1.Controls.Add(this.btnRemoveLoop);
            this.splitLoopsMarkers.Panel1.Controls.Add(this.btnAddLoop);
            this.splitLoopsMarkers.Panel1.Controls.Add(this.viewLoops);
            // 
            // splitLoopsMarkers.Panel2
            // 
            this.splitLoopsMarkers.Panel2.BackColor = System.Drawing.Color.DarkGray;
            this.splitLoopsMarkers.Panel2.Controls.Add(this.btnGoToMarker);
            this.splitLoopsMarkers.Panel2.Controls.Add(this.btnRemoveMarker);
            this.splitLoopsMarkers.Panel2.Controls.Add(this.lblMarkers);
            this.splitLoopsMarkers.Panel2.Controls.Add(this.btnEditMarker);
            this.splitLoopsMarkers.Panel2.Controls.Add(this.btnAddMarker);
            this.splitLoopsMarkers.Panel2.Controls.Add(this.viewMarkers);
            this.splitLoopsMarkers.Size = new System.Drawing.Size(792, 140);
            this.splitLoopsMarkers.SplitterDistance = 395;
            this.splitLoopsMarkers.TabIndex = 74;
            // 
            // btnStopLoop
            // 
            this.btnStopLoop.AntiAliasingEnabled = true;
            this.btnStopLoop.BorderColor = System.Drawing.Color.Gray;
            this.btnStopLoop.BorderWidth = 1;
            this.btnStopLoop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStopLoop.CustomFontName = "Junction";
            this.btnStopLoop.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnStopLoop.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnStopLoop.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnStopLoop.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnStopLoop.Enabled = false;
            this.btnStopLoop.Font = new System.Drawing.Font("Arial", 7.5F);
            this.btnStopLoop.FontCollection = this.fontCollection;
            this.btnStopLoop.FontColor = System.Drawing.Color.Black;
            this.btnStopLoop.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnStopLoop.GradientColor2 = System.Drawing.Color.Gray;
            this.btnStopLoop.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnStopLoop.Image = global::MPfm.Properties.Resources.control_stop;
            this.btnStopLoop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStopLoop.Location = new System.Drawing.Point(49, 16);
            this.btnStopLoop.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnStopLoop.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnStopLoop.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnStopLoop.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnStopLoop.Name = "btnStopLoop";
            this.btnStopLoop.Size = new System.Drawing.Size(52, 20);
            this.btnStopLoop.TabIndex = 80;
            this.btnStopLoop.Text = "Stop";
            this.btnStopLoop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStopLoop.UseVisualStyleBackColor = true;
            this.btnStopLoop.Click += new System.EventHandler(this.btnStopLoop_Click);
            // 
            // btnEditLoop
            // 
            this.btnEditLoop.AntiAliasingEnabled = true;
            this.btnEditLoop.BorderColor = System.Drawing.Color.Gray;
            this.btnEditLoop.BorderWidth = 1;
            this.btnEditLoop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditLoop.CustomFontName = "Junction";
            this.btnEditLoop.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnEditLoop.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnEditLoop.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnEditLoop.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnEditLoop.Enabled = false;
            this.btnEditLoop.Font = new System.Drawing.Font("Arial", 7.5F);
            this.btnEditLoop.FontCollection = this.fontCollection;
            this.btnEditLoop.FontColor = System.Drawing.Color.Black;
            this.btnEditLoop.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnEditLoop.GradientColor2 = System.Drawing.Color.Gray;
            this.btnEditLoop.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnEditLoop.Image = global::MPfm.Properties.Resources.pencil;
            this.btnEditLoop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditLoop.Location = new System.Drawing.Point(149, 16);
            this.btnEditLoop.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnEditLoop.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnEditLoop.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnEditLoop.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnEditLoop.Name = "btnEditLoop";
            this.btnEditLoop.Size = new System.Drawing.Size(50, 20);
            this.btnEditLoop.TabIndex = 79;
            this.btnEditLoop.Text = "Edit";
            this.btnEditLoop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEditLoop.UseVisualStyleBackColor = true;
            this.btnEditLoop.Click += new System.EventHandler(this.btnEditLoop_Click);
            // 
            // btnPlayLoop
            // 
            this.btnPlayLoop.AntiAliasingEnabled = true;
            this.btnPlayLoop.BorderColor = System.Drawing.Color.Gray;
            this.btnPlayLoop.BorderWidth = 1;
            this.btnPlayLoop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlayLoop.CustomFontName = "Junction";
            this.btnPlayLoop.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnPlayLoop.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnPlayLoop.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnPlayLoop.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnPlayLoop.Enabled = false;
            this.btnPlayLoop.Font = new System.Drawing.Font("Arial", 7.5F);
            this.btnPlayLoop.FontCollection = this.fontCollection;
            this.btnPlayLoop.FontColor = System.Drawing.Color.Black;
            this.btnPlayLoop.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnPlayLoop.GradientColor2 = System.Drawing.Color.Gray;
            this.btnPlayLoop.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnPlayLoop.Image = global::MPfm.Properties.Resources.control_play;
            this.btnPlayLoop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlayLoop.Location = new System.Drawing.Point(0, 16);
            this.btnPlayLoop.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnPlayLoop.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnPlayLoop.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnPlayLoop.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnPlayLoop.Name = "btnPlayLoop";
            this.btnPlayLoop.Size = new System.Drawing.Size(50, 20);
            this.btnPlayLoop.TabIndex = 78;
            this.btnPlayLoop.Text = "Play";
            this.btnPlayLoop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPlayLoop.UseVisualStyleBackColor = true;
            this.btnPlayLoop.Click += new System.EventHandler(this.btnPlayLoop_Click);
            // 
            // lblLoops
            // 
            this.lblLoops.AntiAliasingEnabled = true;
            this.lblLoops.BackColor = System.Drawing.Color.Transparent;
            this.lblLoops.CustomFontName = "Junction";
            this.lblLoops.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.lblLoops.FontCollection = this.fontCollection;
            this.lblLoops.ForeColor = System.Drawing.Color.Black;
            this.lblLoops.Location = new System.Drawing.Point(0, 0);
            this.lblLoops.Name = "lblLoops";
            this.lblLoops.Size = new System.Drawing.Size(392, 14);
            this.lblLoops.TabIndex = 75;
            this.lblLoops.Text = "Loops :";
            this.lblLoops.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRemoveLoop
            // 
            this.btnRemoveLoop.AntiAliasingEnabled = true;
            this.btnRemoveLoop.BorderColor = System.Drawing.Color.Gray;
            this.btnRemoveLoop.BorderWidth = 1;
            this.btnRemoveLoop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemoveLoop.CustomFontName = "Junction";
            this.btnRemoveLoop.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnRemoveLoop.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnRemoveLoop.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnRemoveLoop.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnRemoveLoop.Enabled = false;
            this.btnRemoveLoop.Font = new System.Drawing.Font("Arial", 7.5F);
            this.btnRemoveLoop.FontCollection = this.fontCollection;
            this.btnRemoveLoop.FontColor = System.Drawing.Color.Black;
            this.btnRemoveLoop.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnRemoveLoop.GradientColor2 = System.Drawing.Color.Gray;
            this.btnRemoveLoop.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnRemoveLoop.Image = global::MPfm.Properties.Resources.delete;
            this.btnRemoveLoop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRemoveLoop.Location = new System.Drawing.Point(198, 16);
            this.btnRemoveLoop.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnRemoveLoop.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnRemoveLoop.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnRemoveLoop.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnRemoveLoop.Name = "btnRemoveLoop";
            this.btnRemoveLoop.Size = new System.Drawing.Size(68, 20);
            this.btnRemoveLoop.TabIndex = 77;
            this.btnRemoveLoop.Text = "Remove";
            this.btnRemoveLoop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRemoveLoop.UseVisualStyleBackColor = true;
            this.btnRemoveLoop.Click += new System.EventHandler(this.btnRemoveLoop_Click);
            // 
            // btnAddLoop
            // 
            this.btnAddLoop.AntiAliasingEnabled = true;
            this.btnAddLoop.BorderColor = System.Drawing.Color.Gray;
            this.btnAddLoop.BorderWidth = 1;
            this.btnAddLoop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddLoop.CustomFontName = "Junction";
            this.btnAddLoop.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnAddLoop.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnAddLoop.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnAddLoop.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnAddLoop.Enabled = false;
            this.btnAddLoop.Font = new System.Drawing.Font("Arial", 7.5F);
            this.btnAddLoop.FontCollection = this.fontCollection;
            this.btnAddLoop.FontColor = System.Drawing.Color.Black;
            this.btnAddLoop.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnAddLoop.GradientColor2 = System.Drawing.Color.Gray;
            this.btnAddLoop.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnAddLoop.Image = global::MPfm.Properties.Resources.add;
            this.btnAddLoop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddLoop.Location = new System.Drawing.Point(100, 16);
            this.btnAddLoop.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnAddLoop.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnAddLoop.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnAddLoop.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnAddLoop.Name = "btnAddLoop";
            this.btnAddLoop.Size = new System.Drawing.Size(50, 20);
            this.btnAddLoop.TabIndex = 76;
            this.btnAddLoop.Text = "Add";
            this.btnAddLoop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddLoop.UseVisualStyleBackColor = true;
            this.btnAddLoop.Click += new System.EventHandler(this.btnAddLoop_Click);
            // 
            // viewLoops
            // 
            this.viewLoops.AllowDrop = true;
            this.viewLoops.AllowRowReorder = true;
            this.viewLoops.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewLoops.AntiAliasingEnabled = true;
            this.viewLoops.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.viewLoops.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLoopPlayIcon,
            this.columnLoopName,
            this.columnLoopLength,
            this.columnLoopMarkerA,
            this.columnLoopMarkerB});
            this.viewLoops.CustomFontName = null;
            this.viewLoops.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewLoops.FontCollection = null;
            this.viewLoops.FullRowSelect = true;
            this.viewLoops.GradientColor1 = System.Drawing.Color.LightGray;
            this.viewLoops.GradientColor2 = System.Drawing.Color.Gray;
            this.viewLoops.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.viewLoops.GridLines = true;
            this.viewLoops.HeaderForeColor = System.Drawing.Color.Black;
            this.viewLoops.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.viewLoops.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.viewLoops.HeaderHeight = 0;
            this.viewLoops.HideSelection = false;
            this.viewLoops.Location = new System.Drawing.Point(0, 37);
            this.viewLoops.Name = "viewLoops";
            this.viewLoops.SelectedColor = System.Drawing.Color.DarkGray;
            this.viewLoops.Size = new System.Drawing.Size(393, 103);
            this.viewLoops.SmallImageList = this.imageList;
            this.viewLoops.TabIndex = 74;
            this.viewLoops.UseCompatibleStateImageBehavior = false;
            this.viewLoops.View = System.Windows.Forms.View.Details;
            this.viewLoops.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.viewLoops_ItemSelectionChanged);
            this.viewLoops.DoubleClick += new System.EventHandler(this.viewLoops_DoubleClick);
            // 
            // columnLoopPlayIcon
            // 
            this.columnLoopPlayIcon.Text = "";
            this.columnLoopPlayIcon.Width = 20;
            // 
            // columnLoopName
            // 
            this.columnLoopName.Text = "Name";
            this.columnLoopName.Width = 149;
            // 
            // columnLoopLength
            // 
            this.columnLoopLength.Text = "Length";
            this.columnLoopLength.Width = 63;
            // 
            // columnLoopMarkerA
            // 
            this.columnLoopMarkerA.Text = "Marker A";
            this.columnLoopMarkerA.Width = 82;
            // 
            // columnLoopMarkerB
            // 
            this.columnLoopMarkerB.Text = "Marker B";
            this.columnLoopMarkerB.Width = 72;
            // 
            // btnGoToMarker
            // 
            this.btnGoToMarker.AntiAliasingEnabled = true;
            this.btnGoToMarker.BorderColor = System.Drawing.Color.Gray;
            this.btnGoToMarker.BorderWidth = 1;
            this.btnGoToMarker.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGoToMarker.CustomFontName = "Junction";
            this.btnGoToMarker.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnGoToMarker.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnGoToMarker.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnGoToMarker.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnGoToMarker.Enabled = false;
            this.btnGoToMarker.Font = new System.Drawing.Font("Arial", 7.5F);
            this.btnGoToMarker.FontCollection = this.fontCollection;
            this.btnGoToMarker.FontColor = System.Drawing.Color.Black;
            this.btnGoToMarker.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnGoToMarker.GradientColor2 = System.Drawing.Color.Gray;
            this.btnGoToMarker.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnGoToMarker.Image = global::MPfm.Properties.Resources.arrow_right;
            this.btnGoToMarker.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGoToMarker.Location = new System.Drawing.Point(163, 16);
            this.btnGoToMarker.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnGoToMarker.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnGoToMarker.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnGoToMarker.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnGoToMarker.Name = "btnGoToMarker";
            this.btnGoToMarker.Size = new System.Drawing.Size(54, 20);
            this.btnGoToMarker.TabIndex = 75;
            this.btnGoToMarker.Text = "Go to";
            this.btnGoToMarker.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGoToMarker.UseVisualStyleBackColor = true;
            this.btnGoToMarker.Click += new System.EventHandler(this.btnGoToMarker_Click);
            // 
            // btnRemoveMarker
            // 
            this.btnRemoveMarker.AntiAliasingEnabled = true;
            this.btnRemoveMarker.BorderColor = System.Drawing.Color.Gray;
            this.btnRemoveMarker.BorderWidth = 1;
            this.btnRemoveMarker.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemoveMarker.CustomFontName = "Junction";
            this.btnRemoveMarker.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnRemoveMarker.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnRemoveMarker.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnRemoveMarker.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnRemoveMarker.Enabled = false;
            this.btnRemoveMarker.Font = new System.Drawing.Font("Arial", 7.5F);
            this.btnRemoveMarker.FontCollection = this.fontCollection;
            this.btnRemoveMarker.FontColor = System.Drawing.Color.Black;
            this.btnRemoveMarker.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnRemoveMarker.GradientColor2 = System.Drawing.Color.Gray;
            this.btnRemoveMarker.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnRemoveMarker.Image = global::MPfm.Properties.Resources.delete;
            this.btnRemoveMarker.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRemoveMarker.Location = new System.Drawing.Point(96, 16);
            this.btnRemoveMarker.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnRemoveMarker.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnRemoveMarker.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnRemoveMarker.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnRemoveMarker.Name = "btnRemoveMarker";
            this.btnRemoveMarker.Size = new System.Drawing.Size(68, 20);
            this.btnRemoveMarker.TabIndex = 74;
            this.btnRemoveMarker.Text = "Remove";
            this.btnRemoveMarker.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRemoveMarker.UseVisualStyleBackColor = true;
            this.btnRemoveMarker.Click += new System.EventHandler(this.btnRemoveMarker_Click);
            // 
            // lblMarkers
            // 
            this.lblMarkers.AntiAliasingEnabled = true;
            this.lblMarkers.BackColor = System.Drawing.Color.Transparent;
            this.lblMarkers.CustomFontName = "Junction";
            this.lblMarkers.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.lblMarkers.FontCollection = this.fontCollection;
            this.lblMarkers.ForeColor = System.Drawing.Color.Black;
            this.lblMarkers.Location = new System.Drawing.Point(0, 0);
            this.lblMarkers.Name = "lblMarkers";
            this.lblMarkers.Size = new System.Drawing.Size(302, 14);
            this.lblMarkers.TabIndex = 71;
            this.lblMarkers.Text = "Markers :";
            this.lblMarkers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnEditMarker
            // 
            this.btnEditMarker.AntiAliasingEnabled = true;
            this.btnEditMarker.BorderColor = System.Drawing.Color.Gray;
            this.btnEditMarker.BorderWidth = 1;
            this.btnEditMarker.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditMarker.CustomFontName = "Junction";
            this.btnEditMarker.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnEditMarker.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnEditMarker.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnEditMarker.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnEditMarker.Enabled = false;
            this.btnEditMarker.Font = new System.Drawing.Font("Arial", 7.5F);
            this.btnEditMarker.FontCollection = this.fontCollection;
            this.btnEditMarker.FontColor = System.Drawing.Color.Black;
            this.btnEditMarker.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnEditMarker.GradientColor2 = System.Drawing.Color.Gray;
            this.btnEditMarker.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnEditMarker.Image = global::MPfm.Properties.Resources.pencil;
            this.btnEditMarker.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditMarker.Location = new System.Drawing.Point(49, 16);
            this.btnEditMarker.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnEditMarker.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnEditMarker.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnEditMarker.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnEditMarker.Name = "btnEditMarker";
            this.btnEditMarker.Size = new System.Drawing.Size(48, 20);
            this.btnEditMarker.TabIndex = 73;
            this.btnEditMarker.Text = "Edit";
            this.btnEditMarker.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEditMarker.UseVisualStyleBackColor = true;
            this.btnEditMarker.Click += new System.EventHandler(this.btnEditMarker_Click);
            // 
            // btnAddMarker
            // 
            this.btnAddMarker.AntiAliasingEnabled = true;
            this.btnAddMarker.BorderColor = System.Drawing.Color.Gray;
            this.btnAddMarker.BorderWidth = 1;
            this.btnAddMarker.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddMarker.CustomFontName = "Junction";
            this.btnAddMarker.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnAddMarker.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnAddMarker.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnAddMarker.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnAddMarker.Enabled = false;
            this.btnAddMarker.Font = new System.Drawing.Font("Arial", 7.5F);
            this.btnAddMarker.FontCollection = this.fontCollection;
            this.btnAddMarker.FontColor = System.Drawing.Color.Black;
            this.btnAddMarker.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnAddMarker.GradientColor2 = System.Drawing.Color.Gray;
            this.btnAddMarker.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnAddMarker.Image = global::MPfm.Properties.Resources.add;
            this.btnAddMarker.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddMarker.Location = new System.Drawing.Point(0, 16);
            this.btnAddMarker.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnAddMarker.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnAddMarker.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnAddMarker.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnAddMarker.Name = "btnAddMarker";
            this.btnAddMarker.Size = new System.Drawing.Size(50, 20);
            this.btnAddMarker.TabIndex = 72;
            this.btnAddMarker.Text = "Add";
            this.btnAddMarker.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddMarker.UseVisualStyleBackColor = true;
            this.btnAddMarker.Click += new System.EventHandler(this.btnAddMarker_Click);
            // 
            // viewMarkers
            // 
            this.viewMarkers.AllowDrop = true;
            this.viewMarkers.AllowRowReorder = true;
            this.viewMarkers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewMarkers.AntiAliasingEnabled = true;
            this.viewMarkers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.viewMarkers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnMarkerName,
            this.columnMarkerPosition,
            this.columnMarkerComments});
            this.viewMarkers.CustomFontName = null;
            this.viewMarkers.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewMarkers.FontCollection = null;
            this.viewMarkers.FullRowSelect = true;
            this.viewMarkers.GradientColor1 = System.Drawing.Color.LightGray;
            this.viewMarkers.GradientColor2 = System.Drawing.Color.Gray;
            this.viewMarkers.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.viewMarkers.GridLines = true;
            this.viewMarkers.HeaderForeColor = System.Drawing.Color.Black;
            this.viewMarkers.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.viewMarkers.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.viewMarkers.HeaderHeight = 0;
            this.viewMarkers.HideSelection = false;
            this.viewMarkers.Location = new System.Drawing.Point(0, 37);
            this.viewMarkers.Name = "viewMarkers";
            this.viewMarkers.SelectedColor = System.Drawing.Color.DarkGray;
            this.viewMarkers.Size = new System.Drawing.Size(393, 103);
            this.viewMarkers.TabIndex = 70;
            this.viewMarkers.UseCompatibleStateImageBehavior = false;
            this.viewMarkers.View = System.Windows.Forms.View.Details;
            this.viewMarkers.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.viewMarkers_ItemSelectionChanged);
            this.viewMarkers.DoubleClick += new System.EventHandler(this.viewMarkers_DoubleClick);
            // 
            // columnMarkerName
            // 
            this.columnMarkerName.Text = "Name";
            this.columnMarkerName.Width = 145;
            // 
            // columnMarkerPosition
            // 
            this.columnMarkerPosition.Text = "Position";
            this.columnMarkerPosition.Width = 64;
            // 
            // columnMarkerComments
            // 
            this.columnMarkerComments.Text = "Comments";
            this.columnMarkerComments.Width = 148;
            // 
            // panelSongBrowser
            // 
            this.panelSongBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSongBrowser.AntiAliasingEnabled = true;
            this.panelSongBrowser.BackColor = System.Drawing.Color.Gray;
            this.panelSongBrowser.Controls.Add(this.panelSongBrowserToolbar);
            this.panelSongBrowser.Controls.Add(this.viewSongs);
            this.panelSongBrowser.ExpandedHeight = 200;
            this.panelSongBrowser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelSongBrowser.FontCollection = this.fontCollection;
            this.panelSongBrowser.GradientColor1 = System.Drawing.Color.Black;
            this.panelSongBrowser.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelSongBrowser.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelSongBrowser.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelSongBrowser.HeaderExpandable = false;
            this.panelSongBrowser.HeaderForeColor = System.Drawing.Color.White;
            this.panelSongBrowser.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.panelSongBrowser.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.panelSongBrowser.HeaderHeight = 22;
            this.panelSongBrowser.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelSongBrowser.HeaderTitle = "Song Browser";
            this.panelSongBrowser.Location = new System.Drawing.Point(0, 0);
            this.panelSongBrowser.Name = "panelSongBrowser";
            this.panelSongBrowser.Size = new System.Drawing.Size(792, 218);
            this.panelSongBrowser.TabIndex = 25;
            // 
            // panelSongBrowserToolbar
            // 
            this.panelSongBrowserToolbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSongBrowserToolbar.AntiAliasingEnabled = true;
            this.panelSongBrowserToolbar.Controls.Add(this.txtSearch);
            this.panelSongBrowserToolbar.Controls.Add(this.label1);
            this.panelSongBrowserToolbar.Controls.Add(this.btnPlaySelectedSong);
            this.panelSongBrowserToolbar.Controls.Add(this.btnAddSongToPlaylist);
            this.panelSongBrowserToolbar.Controls.Add(this.btnEditSongMetadata);
            this.panelSongBrowserToolbar.ExpandedHeight = 25;
            this.panelSongBrowserToolbar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelSongBrowserToolbar.FontCollection = this.fontCollection;
            this.panelSongBrowserToolbar.GradientColor1 = System.Drawing.Color.Silver;
            this.panelSongBrowserToolbar.GradientColor2 = System.Drawing.Color.Gainsboro;
            this.panelSongBrowserToolbar.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelSongBrowserToolbar.HeaderCustomFontName = "Junction";
            this.panelSongBrowserToolbar.HeaderExpandable = false;
            this.panelSongBrowserToolbar.HeaderExpanded = true;
            this.panelSongBrowserToolbar.HeaderForeColor = System.Drawing.Color.White;
            this.panelSongBrowserToolbar.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelSongBrowserToolbar.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelSongBrowserToolbar.HeaderHeight = 0;
            this.panelSongBrowserToolbar.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelSongBrowserToolbar.Location = new System.Drawing.Point(0, 23);
            this.panelSongBrowserToolbar.Name = "panelSongBrowserToolbar";
            this.panelSongBrowserToolbar.Size = new System.Drawing.Size(795, 25);
            this.panelSongBrowserToolbar.TabIndex = 73;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(503, 2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(177, 20);
            this.txtSearch.TabIndex = 74;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AntiAliasingEnabled = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.CustomFontName = "Junction";
            this.label1.Font = new System.Drawing.Font("Arial", 8F);
            this.label1.FontCollection = this.fontCollection;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(430, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 10);
            this.label1.TabIndex = 73;
            this.label1.Text = "Search for :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnPlaySelectedSong
            // 
            this.btnPlaySelectedSong.AntiAliasingEnabled = true;
            this.btnPlaySelectedSong.BorderColor = System.Drawing.Color.DimGray;
            this.btnPlaySelectedSong.BorderWidth = 1;
            this.btnPlaySelectedSong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlaySelectedSong.CustomFontName = "Junction";
            this.btnPlaySelectedSong.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnPlaySelectedSong.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnPlaySelectedSong.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnPlaySelectedSong.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnPlaySelectedSong.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlaySelectedSong.FontCollection = this.fontCollection;
            this.btnPlaySelectedSong.FontColor = System.Drawing.Color.Black;
            this.btnPlaySelectedSong.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnPlaySelectedSong.GradientColor2 = System.Drawing.Color.Gray;
            this.btnPlaySelectedSong.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnPlaySelectedSong.Image = global::MPfm.Properties.Resources.control_play;
            this.btnPlaySelectedSong.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlaySelectedSong.Location = new System.Drawing.Point(0, 0);
            this.btnPlaySelectedSong.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnPlaySelectedSong.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnPlaySelectedSong.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnPlaySelectedSong.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnPlaySelectedSong.Name = "btnPlaySelectedSong";
            this.btnPlaySelectedSong.Size = new System.Drawing.Size(142, 25);
            this.btnPlaySelectedSong.TabIndex = 68;
            this.btnPlaySelectedSong.Text = "Play selected song(s)";
            this.btnPlaySelectedSong.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPlaySelectedSong.UseVisualStyleBackColor = true;
            this.btnPlaySelectedSong.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnAddSongToPlaylist
            // 
            this.btnAddSongToPlaylist.AntiAliasingEnabled = true;
            this.btnAddSongToPlaylist.BorderColor = System.Drawing.Color.DimGray;
            this.btnAddSongToPlaylist.BorderWidth = 1;
            this.btnAddSongToPlaylist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddSongToPlaylist.CustomFontName = "Junction";
            this.btnAddSongToPlaylist.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnAddSongToPlaylist.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnAddSongToPlaylist.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnAddSongToPlaylist.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnAddSongToPlaylist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddSongToPlaylist.FontCollection = this.fontCollection;
            this.btnAddSongToPlaylist.FontColor = System.Drawing.Color.Black;
            this.btnAddSongToPlaylist.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnAddSongToPlaylist.GradientColor2 = System.Drawing.Color.Gray;
            this.btnAddSongToPlaylist.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnAddSongToPlaylist.Image = global::MPfm.Properties.Resources.add;
            this.btnAddSongToPlaylist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddSongToPlaylist.Location = new System.Drawing.Point(275, 0);
            this.btnAddSongToPlaylist.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnAddSongToPlaylist.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnAddSongToPlaylist.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnAddSongToPlaylist.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnAddSongToPlaylist.Name = "btnAddSongToPlaylist";
            this.btnAddSongToPlaylist.Size = new System.Drawing.Size(152, 25);
            this.btnAddSongToPlaylist.TabIndex = 72;
            this.btnAddSongToPlaylist.Text = "Add song(s) to playlist";
            this.btnAddSongToPlaylist.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddSongToPlaylist.UseVisualStyleBackColor = true;
            this.btnAddSongToPlaylist.Click += new System.EventHandler(this.btnAddSongToPlaylist_Click);
            // 
            // btnEditSongMetadata
            // 
            this.btnEditSongMetadata.AntiAliasingEnabled = true;
            this.btnEditSongMetadata.BorderColor = System.Drawing.Color.DimGray;
            this.btnEditSongMetadata.BorderWidth = 1;
            this.btnEditSongMetadata.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditSongMetadata.CustomFontName = "Junction";
            this.btnEditSongMetadata.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnEditSongMetadata.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnEditSongMetadata.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnEditSongMetadata.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnEditSongMetadata.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditSongMetadata.FontCollection = this.fontCollection;
            this.btnEditSongMetadata.FontColor = System.Drawing.Color.Black;
            this.btnEditSongMetadata.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnEditSongMetadata.GradientColor2 = System.Drawing.Color.Gray;
            this.btnEditSongMetadata.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnEditSongMetadata.Image = global::MPfm.Properties.Resources.information;
            this.btnEditSongMetadata.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditSongMetadata.Location = new System.Drawing.Point(141, 0);
            this.btnEditSongMetadata.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnEditSongMetadata.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnEditSongMetadata.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnEditSongMetadata.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnEditSongMetadata.Name = "btnEditSongMetadata";
            this.btnEditSongMetadata.Size = new System.Drawing.Size(135, 25);
            this.btnEditSongMetadata.TabIndex = 71;
            this.btnEditSongMetadata.Text = "Edit song metadata";
            this.btnEditSongMetadata.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEditSongMetadata.UseVisualStyleBackColor = true;
            this.btnEditSongMetadata.Click += new System.EventHandler(this.btnEditSongMetadata_Click);
            // 
            // viewSongs
            // 
            this.viewSongs.AllowDrop = true;
            this.viewSongs.AllowRowReorder = false;
            this.viewSongs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewSongs.AntiAliasingEnabled = true;
            this.viewSongs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.viewSongs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnSongPlayIcon,
            this.columnSongTrackNumber,
            this.columnSongTitle,
            this.columnSongLength,
            this.columnSongArtistName,
            this.columnSongAlbumTitle,
            this.columnSongPlayCount,
            this.columnSongLastPlayed});
            this.viewSongs.ContextMenuStrip = this.menuSongBrowser;
            this.viewSongs.CustomFontName = null;
            this.viewSongs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewSongs.FontCollection = this.fontCollection;
            this.viewSongs.FullRowSelect = true;
            this.viewSongs.GradientColor1 = System.Drawing.Color.LightGray;
            this.viewSongs.GradientColor2 = System.Drawing.Color.Gray;
            this.viewSongs.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.viewSongs.GridLines = true;
            this.viewSongs.HeaderForeColor = System.Drawing.Color.Black;
            this.viewSongs.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.viewSongs.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.viewSongs.HeaderHeight = 0;
            this.viewSongs.HideSelection = false;
            this.viewSongs.Location = new System.Drawing.Point(0, 47);
            this.viewSongs.Name = "viewSongs";
            this.viewSongs.SelectedColor = System.Drawing.Color.DarkGray;
            this.viewSongs.Size = new System.Drawing.Size(792, 171);
            this.viewSongs.SmallImageList = this.imageListSongBrowser;
            this.viewSongs.TabIndex = 22;
            this.viewSongs.UseCompatibleStateImageBehavior = false;
            this.viewSongs.View = System.Windows.Forms.View.Details;
            this.viewSongs.SelectedIndexChanged += new System.EventHandler(this.viewSongs_SelectedIndexChanged);
            this.viewSongs.DoubleClick += new System.EventHandler(this.viewSongs_DoubleClick);
            // 
            // columnSongPlayIcon
            // 
            this.columnSongPlayIcon.Text = "";
            this.columnSongPlayIcon.Width = 21;
            // 
            // columnSongTrackNumber
            // 
            this.columnSongTrackNumber.Text = "Tr #";
            this.columnSongTrackNumber.Width = 35;
            // 
            // columnSongTitle
            // 
            this.columnSongTitle.Text = "Title";
            this.columnSongTitle.Width = 234;
            // 
            // columnSongLength
            // 
            this.columnSongLength.Text = "Length";
            this.columnSongLength.Width = 80;
            // 
            // columnSongArtistName
            // 
            this.columnSongArtistName.Text = "Artist Name";
            this.columnSongArtistName.Width = 198;
            // 
            // columnSongAlbumTitle
            // 
            this.columnSongAlbumTitle.Text = "Album Title";
            this.columnSongAlbumTitle.Width = 216;
            // 
            // columnSongPlayCount
            // 
            this.columnSongPlayCount.Text = "Play Count";
            this.columnSongPlayCount.Width = 68;
            // 
            // columnSongLastPlayed
            // 
            this.columnSongLastPlayed.Text = "Last Played";
            this.columnSongLastPlayed.Width = 161;
            // 
            // imageListSongBrowser
            // 
            this.imageListSongBrowser.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSongBrowser.ImageStream")));
            this.imageListSongBrowser.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSongBrowser.Images.SetKeyName(0, "bullet_go.png");
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "PMP";
            this.notifyIcon.BalloonTipTitle = "PMP";
            this.notifyIcon.ContextMenuStrip = this.menuTray;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "PMP - Pimp Music Player";
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // menuTray
            // 
            this.menuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miTrayArtist,
            this.miTrayAlbum,
            this.miTraySongName,
            this.miTraySongStatus,
            this.toolStripSeparator7,
            this.miTrayPlay,
            this.miTrayPause,
            this.miTrayStop,
            this.miTrayPreviousSong,
            this.miTrayNextSong,
            this.miTrayRepeat,
            this.toolStripSeparator4,
            this.miTrayShowPMP,
            this.miTrayExitPMP});
            this.menuTray.Name = "menuSongBrowser";
            this.menuTray.Size = new System.Drawing.Size(178, 280);
            // 
            // miTrayArtist
            // 
            this.miTrayArtist.Enabled = false;
            this.miTrayArtist.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.miTrayArtist.Image = global::MPfm.Properties.Resources.star;
            this.miTrayArtist.Name = "miTrayArtist";
            this.miTrayArtist.Size = new System.Drawing.Size(177, 22);
            this.miTrayArtist.Text = "Artist";
            // 
            // miTrayAlbum
            // 
            this.miTrayAlbum.Enabled = false;
            this.miTrayAlbum.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.miTrayAlbum.Image = global::MPfm.Properties.Resources.cd;
            this.miTrayAlbum.Name = "miTrayAlbum";
            this.miTrayAlbum.Size = new System.Drawing.Size(177, 22);
            this.miTrayAlbum.Text = "Album";
            // 
            // miTraySongName
            // 
            this.miTraySongName.Enabled = false;
            this.miTraySongName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.miTraySongName.Image = global::MPfm.Properties.Resources.music;
            this.miTraySongName.Name = "miTraySongName";
            this.miTraySongName.Size = new System.Drawing.Size(177, 22);
            this.miTraySongName.Text = "Song";
            // 
            // miTraySongStatus
            // 
            this.miTraySongStatus.Enabled = false;
            this.miTraySongStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.miTraySongStatus.Image = global::MPfm.Properties.Resources.time;
            this.miTraySongStatus.Name = "miTraySongStatus";
            this.miTraySongStatus.Size = new System.Drawing.Size(177, 22);
            this.miTraySongStatus.Text = "[ 0:00.00 / 0:00.00 ]";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(174, 6);
            // 
            // miTrayPlay
            // 
            this.miTrayPlay.Image = global::MPfm.Properties.Resources.control_play;
            this.miTrayPlay.Name = "miTrayPlay";
            this.miTrayPlay.Size = new System.Drawing.Size(177, 22);
            this.miTrayPlay.Text = "Play";
            // 
            // miTrayPause
            // 
            this.miTrayPause.Image = global::MPfm.Properties.Resources.control_pause;
            this.miTrayPause.Name = "miTrayPause";
            this.miTrayPause.Size = new System.Drawing.Size(177, 22);
            this.miTrayPause.Text = "Pause";
            this.miTrayPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // miTrayStop
            // 
            this.miTrayStop.Image = global::MPfm.Properties.Resources.control_stop;
            this.miTrayStop.Name = "miTrayStop";
            this.miTrayStop.Size = new System.Drawing.Size(177, 22);
            this.miTrayStop.Text = "Stop";
            this.miTrayStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // miTrayPreviousSong
            // 
            this.miTrayPreviousSong.Image = global::MPfm.Properties.Resources.control_start;
            this.miTrayPreviousSong.Name = "miTrayPreviousSong";
            this.miTrayPreviousSong.Size = new System.Drawing.Size(177, 22);
            this.miTrayPreviousSong.Text = "Previous Song";
            this.miTrayPreviousSong.Click += new System.EventHandler(this.btnPreviousSong_Click);
            // 
            // miTrayNextSong
            // 
            this.miTrayNextSong.Image = global::MPfm.Properties.Resources.control_end;
            this.miTrayNextSong.Name = "miTrayNextSong";
            this.miTrayNextSong.Size = new System.Drawing.Size(177, 22);
            this.miTrayNextSong.Text = "Next Song";
            this.miTrayNextSong.Click += new System.EventHandler(this.btnNextSong_Click);
            // 
            // miTrayRepeat
            // 
            this.miTrayRepeat.Image = global::MPfm.Properties.Resources.control_repeat;
            this.miTrayRepeat.Name = "miTrayRepeat";
            this.miTrayRepeat.Size = new System.Drawing.Size(177, 22);
            this.miTrayRepeat.Text = "Repeat (Off)";
            this.miTrayRepeat.Click += new System.EventHandler(this.btnRepeat_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(174, 6);
            // 
            // miTrayShowPMP
            // 
            this.miTrayShowPMP.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.miTrayShowPMP.Image = global::MPfm.Properties.Resources.image;
            this.miTrayShowPMP.Name = "miTrayShowPMP";
            this.miTrayShowPMP.Size = new System.Drawing.Size(177, 22);
            this.miTrayShowPMP.Text = "Show PMP";
            this.miTrayShowPMP.Click += new System.EventHandler(this.miTrayShowApplication_Click);
            // 
            // miTrayExitPMP
            // 
            this.miTrayExitPMP.Image = global::MPfm.Properties.Resources.door_in;
            this.miTrayExitPMP.Name = "miTrayExitPMP";
            this.miTrayExitPMP.Size = new System.Drawing.Size(177, 22);
            this.miTrayExitPMP.Text = "&Exit PMP";
            this.miTrayExitPMP.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // dialogAddFiles
            // 
            this.dialogAddFiles.Filter = "Audio files (*.mp3,*.flac,*.ogg)|*.mp3;*.flac;*.ogg";
            this.dialogAddFiles.Multiselect = true;
            this.dialogAddFiles.Title = "Add file(s) to library";
            // 
            // workerTreeLibrary
            // 
            this.workerTreeLibrary.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerTreeLibrary_DoWork);
            this.workerTreeLibrary.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerTreeLibrary_RunWorkerCompleted);
            // 
            // timerUpdateOutputMeter
            // 
            this.timerUpdateOutputMeter.Enabled = true;
            this.timerUpdateOutputMeter.Interval = 10;
            this.timerUpdateOutputMeter.Tick += new System.EventHandler(this.timerUpdateOutputMeter_Tick);
            // 
            // workerAlbumArt
            // 
            this.workerAlbumArt.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerAlbumArt_DoWork);
            this.workerAlbumArt.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.workerAlbumArt_RunWorkerCompleted);
            // 
            // dialogOpenFile
            // 
            this.dialogOpenFile.Filter = "Audio files (*.mp3,*.flac,*.ogg, *.wav)|*.mp3;*.flac;*.ogg,*.wav";
            this.dialogOpenFile.Title = "Select an audio file to play";
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1006, 717);
            this.Controls.Add(this.splitFirst);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuMain);
            this.DoubleBuffered = true;
            this.FontCollection = this.fontCollection;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.MinimumSize = new System.Drawing.Size(790, 520);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MPfm: Music Player for Musicians";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuSongBrowser.ResumeLayout(false);
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.splitFirst.Panel1.ResumeLayout(false);
            this.splitFirst.Panel2.ResumeLayout(false);
            this.splitFirst.ResumeLayout(false);
            this.panelLibrary.ResumeLayout(false);
            this.menuLibrary.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.panelCurrentSong.ResumeLayout(false);
            this.panelCurrentSongChild.ResumeLayout(false);
            this.panelInformation.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.panelVolume.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picDistortionWarning)).EndInit();
            this.panelTimeShifting.ResumeLayout(false);
            this.panelSongPosition.ResumeLayout(false);
            this.panelTotalTime.ResumeLayout(false);
            this.panelCurrentTime.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAlbum)).EndInit();
            this.splitLoopsMarkersSongBrowser.Panel1.ResumeLayout(false);
            this.splitLoopsMarkersSongBrowser.Panel2.ResumeLayout(false);
            this.splitLoopsMarkersSongBrowser.ResumeLayout(false);
            this.panelLoopsMarkers.ResumeLayout(false);
            this.splitWaveFormLoopsMarkers.Panel1.ResumeLayout(false);
            this.splitWaveFormLoopsMarkers.Panel2.ResumeLayout(false);
            this.splitWaveFormLoopsMarkers.ResumeLayout(false);
            this.splitLoopsMarkers.Panel1.ResumeLayout(false);
            this.splitLoopsMarkers.Panel2.ResumeLayout(false);
            this.splitLoopsMarkers.ResumeLayout(false);
            this.panelSongBrowser.ResumeLayout(false);
            this.panelSongBrowserToolbar.ResumeLayout(false);
            this.panelSongBrowserToolbar.PerformLayout();
            this.menuTray.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.MenuStrip menuMain;        
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sacToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1; 
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miHelpAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miFileExit;
        private MPfm.WindowsControls.Panel panelCurrentSongChild;
        private System.Windows.Forms.StatusStrip statusBar;
        private MPfm.WindowsControls.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton btnPause;
        private System.Windows.Forms.ToolStripButton btnNextSong;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.SplitContainer splitFirst;
        private MPfm.WindowsControls.TreeView treeLibrary;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.OpenFileDialog dialogAddFiles;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ContextMenuStrip menuSongBrowser;
        private System.Windows.Forms.ToolStripMenuItem miPlaySong;
        private System.Windows.Forms.ToolStripMenuItem miRemoveSong;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton btnSettings;
        public System.Windows.Forms.FolderBrowserDialog dialogAddFolder;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripButton btnPreviousSong;
        private System.Windows.Forms.ContextMenuStrip menuLibrary;
        private System.Windows.Forms.ToolStripMenuItem miTreeLibraryAddSongsToPlaylist;
        private System.Windows.Forms.ToolStripMenuItem miTreeLibraryRemoveSongsFromLibrary;
        private System.Windows.Forms.ToolStripMenuItem miTreeLibraryPlaySongs;
        private System.Windows.Forms.ContextMenuStrip menuTray;
        private System.Windows.Forms.ToolStripMenuItem miTrayShowPMP;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem miTrayPlay;
        private System.Windows.Forms.ToolStripMenuItem miTrayPause;
        private System.Windows.Forms.ToolStripMenuItem miTrayStop;
        private System.Windows.Forms.ToolStripMenuItem miTrayPreviousSong;
        private System.Windows.Forms.ToolStripMenuItem miTrayNextSong;
        private System.Windows.Forms.ToolStripMenuItem miTrayArtist;
        private System.Windows.Forms.ToolStripMenuItem miTraySongName;
        private System.Windows.Forms.ToolStripMenuItem miTraySongStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton btnRepeat;
        private System.Windows.Forms.ToolStripMenuItem miTrayRepeat;
        private System.Windows.Forms.ToolStripMenuItem miTrayAlbum;
        private System.Windows.Forms.ToolStripMenuItem miTrayExitPMP;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miWindowsEffects;
        private System.Windows.Forms.ToolStripMenuItem miWindowsSettings;
        private System.Windows.Forms.ToolStripMenuItem miWindowsPlaylist;
        public System.Windows.Forms.ToolStripButton btnPlaylist;
        private System.Windows.Forms.PictureBox picAlbum;
        private System.Windows.Forms.ToolStripMenuItem miEditSong;
        private MPfm.WindowsControls.Label lblCurrentTime;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private MPfm.WindowsControls.Label lblTotalTime;
        private MPfm.WindowsControls.Label lblSongPercentage;
        private MPfm.WindowsControls.Label lblTimeShifting;
        private MPfm.WindowsControls.Label lblVolume;
        private MPfm.WindowsControls.Label lblCurrentSongTitle;
        private MPfm.WindowsControls.Panel panelCurrentTime;
        private MPfm.WindowsControls.Panel panelTotalTime;
        private MPfm.WindowsControls.Panel panelSongPosition;
        private MPfm.WindowsControls.Panel panelTimeShifting;
        private MPfm.WindowsControls.Panel panelLoopsMarkers;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private MPfm.WindowsControls.Panel panelCurrentSong;
        private MPfm.WindowsControls.Panel panelVolume;
        private MPfm.WindowsControls.Panel panelInformation;
        private System.Windows.Forms.ToolStripMenuItem miAddSongToPlaylist;
        private MPfm.WindowsControls.LinkLabel lblCurrentArtistName;
        private MPfm.WindowsControls.LinkLabel lblCurrentAlbumTitle;
        private MPfm.WindowsControls.Panel panelActions;
        private MPfm.WindowsControls.LinkLabel linkSearchGuitarTabs;
        private MPfm.WindowsControls.LinkLabel linkSearchLyrics;
        private MPfm.WindowsControls.LinkLabel linkSearchBassTabs;
        private System.Windows.Forms.ToolStripButton btnUpdateLibrary;
        private MPfm.WindowsControls.Label lblSoundFormat;
        private MPfm.WindowsControls.Label lblSoundFormatTitle;
        private MPfm.WindowsControls.Label lblBitsPerSample;
        private MPfm.WindowsControls.Label lblBitsPerSampleTitle;
        private MPfm.WindowsControls.Label lblSearchWeb;
        private MPfm.WindowsControls.LinkLabel linkEditSongMetadata;
        private MPfm.WindowsControls.LinkLabel linkResetTimeShifting;
        private System.Windows.Forms.ComboBox comboSoundFormat;
        private MPfm.WindowsControls.Label lblCurrentFilePath;
        private MPfm.WindowsControls.Label lblFilterBySoundFormat;
        private MPfm.WindowsControls.Panel panelLibrary;
        private System.ComponentModel.BackgroundWorker workerTreeLibrary;
        private System.Windows.Forms.ImageList imageListSongBrowser;
        private MPfm.WindowsControls.OutputMeter outputMeter;
        private System.Windows.Forms.Timer timerUpdateOutputMeter;
        private MPfm.WindowsControls.TrackBar trackPosition;
        private MPfm.WindowsControls.TrackBar trackTimeShifting;
        private MPfm.WindowsControls.Label lblSongPosition;
        private MPfm.WindowsControls.VolumeFader faderVolume;
        private System.ComponentModel.BackgroundWorker workerAlbumArt;
        private System.Windows.Forms.PictureBox picDistortionWarning;
        private System.Windows.Forms.ToolStripMenuItem miWindowsVisualizer;
        private System.Windows.Forms.ToolStripMenuItem miFileAddFile;
        private System.Windows.Forms.ToolStripMenuItem miFileAddFolder;
        private System.Windows.Forms.ToolStripMenuItem miFileUpdateLibrary;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem miFileOpenAudioFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private WindowsControls.Label lblFrequency;
        private WindowsControls.Label lblFrequencyTitle;
        private System.Windows.Forms.ToolStripMenuItem miHelpWebsite;
        private System.Windows.Forms.ToolStripMenuItem miHelpReportBug;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.OpenFileDialog dialogOpenFile;
        public System.Windows.Forms.ToolStripButton btnEffects;
        public System.Windows.Forms.ToolStripButton btnVisualizer;
        private System.Windows.Forms.ToolStripMenuItem miTreeLibraryDeletePlaylist;
        private WindowsControls.Label lblMarkers;
        private MPfm.WindowsControls.ReorderListView viewMarkers;
        private System.Windows.Forms.ColumnHeader columnMarkerName;
        private System.Windows.Forms.ColumnHeader columnMarkerPosition;
        private System.Windows.Forms.ColumnHeader columnMarkerComments;
        private WindowsControls.Button btnAddMarker;
        private WindowsControls.Button btnEditMarker;
        private System.Windows.Forms.SplitContainer splitLoopsMarkers;
        private WindowsControls.Label lblLoops;
        private WindowsControls.Button btnRemoveLoop;
        private WindowsControls.Button btnAddLoop;
        private MPfm.WindowsControls.ReorderListView viewLoops;
        private System.Windows.Forms.ColumnHeader columnLoopPlayIcon;
        private System.Windows.Forms.ColumnHeader columnLoopName;
        private System.Windows.Forms.ColumnHeader columnLoopLength;
        private System.Windows.Forms.ColumnHeader columnLoopMarkerA;
        public WindowsControls.WaveFormMarkersLoops waveFormMarkersLoops;
        private System.Windows.Forms.SplitContainer splitWaveFormLoopsMarkers;
        private WindowsControls.Button btnEditLoop;
        private WindowsControls.Button btnPlayLoop;
        private WindowsControls.Button btnRemoveMarker;
        private WindowsControls.Button btnStopLoop;
        private System.Windows.Forms.ColumnHeader columnLoopMarkerB;
        private WindowsControls.Panel panelSongBrowser;
        private WindowsControls.Panel panelSongBrowserToolbar;
        private System.Windows.Forms.TextBox txtSearch;
        private WindowsControls.Label label1;
        private WindowsControls.Button btnPlaySelectedSong;
        private WindowsControls.Button btnAddSongToPlaylist;
        private WindowsControls.Button btnEditSongMetadata;
        public WindowsControls.ReorderListView viewSongs;
        private System.Windows.Forms.ColumnHeader columnSongPlayIcon;
        private System.Windows.Forms.ColumnHeader columnSongTrackNumber;
        private System.Windows.Forms.ColumnHeader columnSongTitle;
        private System.Windows.Forms.ColumnHeader columnSongLength;
        private System.Windows.Forms.ColumnHeader columnSongArtistName;
        private System.Windows.Forms.ColumnHeader columnSongAlbumTitle;
        private System.Windows.Forms.ColumnHeader columnSongPlayCount;
        private System.Windows.Forms.ColumnHeader columnSongLastPlayed;
        private System.Windows.Forms.SplitContainer splitLoopsMarkersSongBrowser;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private WindowsControls.Button btnGoToMarker;        
    }
}

