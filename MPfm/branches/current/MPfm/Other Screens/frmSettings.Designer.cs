namespace MPfm
{
    partial class frmSettings
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
            MPfm.WindowsControls.EmbeddedFont embeddedFont1 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont embeddedFont2 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont embeddedFont3 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.SongGridViewTheme songGridViewTheme1 = new MPfm.WindowsControls.SongGridViewTheme();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont11 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont12 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont13 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont14 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont15 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont16 = new MPfm.WindowsControls.CustomFont();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.panelGeneralSettings = new MPfm.WindowsControls.Panel();
            this.lblTest = new MPfm.WindowsControls.Label();
            this.btnStopPeak = new MPfm.WindowsControls.Button();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.btnTestPeak = new MPfm.WindowsControls.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblHideTray = new MPfm.WindowsControls.Label();
            this.lblShowTray = new MPfm.WindowsControls.Label();
            this.chkShowTray = new System.Windows.Forms.CheckBox();
            this.chkHideTray = new System.Windows.Forms.CheckBox();
            this.tabTheme = new System.Windows.Forms.TabPage();
            this.panelTheme = new MPfm.WindowsControls.Panel();
            this.comboThemeControl = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.lblFilterBySoundFormat = new MPfm.WindowsControls.Label();
            this.label2 = new MPfm.WindowsControls.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new MPfm.WindowsControls.Label();
            this.button4 = new MPfm.WindowsControls.Button();
            this.button3 = new MPfm.WindowsControls.Button();
            this.button2 = new MPfm.WindowsControls.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.songBrowser = new MPfm.WindowsControls.SongGridView();
            this.propertyGridTheme = new System.Windows.Forms.PropertyGrid();
            this.tabAudioSettings = new System.Windows.Forms.TabPage();
            this.panelAudioSettings = new MPfm.WindowsControls.Panel();
            this.lblMixerSampleRateUnit = new MPfm.WindowsControls.Label();
            this.txtMixerSampleRate = new System.Windows.Forms.NumericUpDown();
            this.lblBufferSizeUnit = new MPfm.WindowsControls.Label();
            this.txtBufferSize = new System.Windows.Forms.NumericUpDown();
            this.lblUpdatePeriodUnit = new MPfm.WindowsControls.Label();
            this.txtUpdatePeriod = new System.Windows.Forms.NumericUpDown();
            this.lblUpdatePeriod = new MPfm.WindowsControls.Label();
            this.lblBufferSize = new MPfm.WindowsControls.Label();
            this.lblMixerSampleRate = new MPfm.WindowsControls.Label();
            this.lblOutputDriver = new MPfm.WindowsControls.Label();
            this.btnTestSound = new MPfm.WindowsControls.Button();
            this.cboOutputDevices = new System.Windows.Forms.ComboBox();
            this.lblDriver = new MPfm.WindowsControls.Label();
            this.cboDrivers = new System.Windows.Forms.ComboBox();
            this.tabLibrary = new System.Windows.Forms.TabPage();
            this.panelLibrarySettings = new MPfm.WindowsControls.Panel();
            this.btnRemoveFolder = new MPfm.WindowsControls.Button();
            this.viewFolders = new MPfm.WindowsControls.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddFolder = new MPfm.WindowsControls.Button();
            this.Button1 = new MPfm.WindowsControls.Button();
            this.lblFoldersTitle = new MPfm.WindowsControls.Label();
            this.btnResetLibrary = new MPfm.WindowsControls.Button();
            this.dialogAddFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.dialogOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.panelBackground = new MPfm.WindowsControls.Panel();
            this.tabs.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.panelGeneralSettings.SuspendLayout();
            this.tabTheme.SuspendLayout();
            this.panelTheme.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabAudioSettings.SuspendLayout();
            this.panelAudioSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMixerSampleRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBufferSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUpdatePeriod)).BeginInit();
            this.tabLibrary.SuspendLayout();
            this.panelLibrarySettings.SuspendLayout();
            this.panelBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabGeneral);
            this.tabs.Controls.Add(this.tabTheme);
            this.tabs.Controls.Add(this.tabAudioSettings);
            this.tabs.Controls.Add(this.tabLibrary);
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(639, 378);
            this.tabs.TabIndex = 6;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.panelGeneralSettings);
            this.tabGeneral.Location = new System.Drawing.Point(4, 24);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(631, 350);
            this.tabGeneral.TabIndex = 3;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // panelGeneralSettings
            // 
            this.panelGeneralSettings.AntiAliasingEnabled = true;
            this.panelGeneralSettings.Controls.Add(this.lblTest);
            this.panelGeneralSettings.Controls.Add(this.btnStopPeak);
            this.panelGeneralSettings.Controls.Add(this.btnTestPeak);
            this.panelGeneralSettings.Controls.Add(this.txtPath);
            this.panelGeneralSettings.Controls.Add(this.lblHideTray);
            this.panelGeneralSettings.Controls.Add(this.lblShowTray);
            this.panelGeneralSettings.Controls.Add(this.chkShowTray);
            this.panelGeneralSettings.Controls.Add(this.chkHideTray);
            this.panelGeneralSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGeneralSettings.ExpandedHeight = 200;
            this.panelGeneralSettings.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelGeneralSettings.FontCollection = this.fontCollection;
            this.panelGeneralSettings.GradientColor1 = System.Drawing.Color.Silver;
            this.panelGeneralSettings.GradientColor2 = System.Drawing.Color.Gray;
            this.panelGeneralSettings.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelGeneralSettings.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelGeneralSettings.HeaderExpandable = false;
            this.panelGeneralSettings.HeaderExpanded = true;
            this.panelGeneralSettings.HeaderForeColor = System.Drawing.Color.Black;
            this.panelGeneralSettings.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelGeneralSettings.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelGeneralSettings.HeaderHeight = 30;
            this.panelGeneralSettings.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelGeneralSettings.HeaderTitle = "General Settings";
            this.panelGeneralSettings.Location = new System.Drawing.Point(0, 0);
            this.panelGeneralSettings.Name = "panelGeneralSettings";
            this.panelGeneralSettings.Size = new System.Drawing.Size(631, 350);
            this.panelGeneralSettings.TabIndex = 17;
            // 
            // lblTest
            // 
            this.lblTest.AntiAliasingEnabled = true;
            this.lblTest.BackColor = System.Drawing.Color.Transparent;
            customFont1.EmbeddedFontName = "";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 8;
            customFont1.StandardFontName = "Arial";
            customFont1.UseEmbeddedFont = false;
            this.lblTest.CustomFont = customFont1;
            this.lblTest.CustomFontName = "Junction";
            this.lblTest.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTest.Location = new System.Drawing.Point(8, 229);
            this.lblTest.Name = "lblTest";
            this.lblTest.Size = new System.Drawing.Size(254, 17);
            this.lblTest.TabIndex = 96;
            this.lblTest.Text = "Audio file directory for peak file generation:";
            this.lblTest.Visible = false;
            // 
            // btnStopPeak
            // 
            this.btnStopPeak.AntiAliasingEnabled = true;
            this.btnStopPeak.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnStopPeak.BorderWidth = 1;
            this.btnStopPeak.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStopPeak.CustomFontName = "Junction";
            this.btnStopPeak.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnStopPeak.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnStopPeak.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnStopPeak.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnStopPeak.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopPeak.FontCollection = this.fontCollection;
            this.btnStopPeak.FontColor = System.Drawing.Color.Black;
            this.btnStopPeak.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnStopPeak.GradientColor2 = System.Drawing.Color.Gray;
            this.btnStopPeak.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnStopPeak.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnStopPeak.Location = new System.Drawing.Point(351, 249);
            this.btnStopPeak.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnStopPeak.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnStopPeak.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnStopPeak.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnStopPeak.Name = "btnStopPeak";
            this.btnStopPeak.Size = new System.Drawing.Size(77, 23);
            this.btnStopPeak.TabIndex = 95;
            this.btnStopPeak.Text = "Stop Peak";
            this.btnStopPeak.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStopPeak.UseVisualStyleBackColor = true;
            this.btnStopPeak.Visible = false;
            // 
            // fontCollection
            // 
            embeddedFont1.AssemblyPath = "MPfm.Fonts.dll";
            embeddedFont1.Name = "LeagueGothic";
            embeddedFont1.ResourceName = "MPfm.Fonts.LeagueGothic.ttf";
            embeddedFont2.AssemblyPath = "MPfm.Fonts.dll";
            embeddedFont2.Name = "Junction";
            embeddedFont2.ResourceName = "MPfm.Fonts.Junction.ttf";
            embeddedFont3.AssemblyPath = "MPfm.Fonts.dll";
            embeddedFont3.Name = "TitilliumText22L Lt";
            embeddedFont3.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            this.fontCollection.Fonts.Add(embeddedFont1);
            this.fontCollection.Fonts.Add(embeddedFont2);
            this.fontCollection.Fonts.Add(embeddedFont3);
            // 
            // btnTestPeak
            // 
            this.btnTestPeak.AntiAliasingEnabled = true;
            this.btnTestPeak.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnTestPeak.BorderWidth = 1;
            this.btnTestPeak.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestPeak.CustomFontName = "Junction";
            this.btnTestPeak.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnTestPeak.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnTestPeak.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnTestPeak.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnTestPeak.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestPeak.FontCollection = this.fontCollection;
            this.btnTestPeak.FontColor = System.Drawing.Color.Black;
            this.btnTestPeak.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnTestPeak.GradientColor2 = System.Drawing.Color.Gray;
            this.btnTestPeak.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnTestPeak.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnTestPeak.Location = new System.Drawing.Point(268, 249);
            this.btnTestPeak.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnTestPeak.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnTestPeak.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnTestPeak.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnTestPeak.Name = "btnTestPeak";
            this.btnTestPeak.Size = new System.Drawing.Size(77, 23);
            this.btnTestPeak.TabIndex = 94;
            this.btnTestPeak.Text = "Test Peak";
            this.btnTestPeak.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnTestPeak.UseVisualStyleBackColor = true;
            this.btnTestPeak.Visible = false;
            // 
            // txtPath
            // 
            this.txtPath.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPath.Location = new System.Drawing.Point(11, 251);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(251, 21);
            this.txtPath.TabIndex = 93;
            this.txtPath.Text = "E:\\Mp3\\Bob Marley\\Exodus\\";
            this.txtPath.Visible = false;
            // 
            // lblHideTray
            // 
            this.lblHideTray.AntiAliasingEnabled = true;
            this.lblHideTray.BackColor = System.Drawing.Color.Transparent;
            customFont2.EmbeddedFontName = "Junction";
            customFont2.IsBold = false;
            customFont2.IsItalic = false;
            customFont2.IsUnderline = false;
            customFont2.Size = 9;
            customFont2.StandardFontName = "Arial";
            customFont2.UseEmbeddedFont = true;
            this.lblHideTray.CustomFont = customFont2;
            this.lblHideTray.CustomFontName = "Junction";
            this.lblHideTray.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHideTray.Location = new System.Drawing.Point(20, 57);
            this.lblHideTray.Name = "lblHideTray";
            this.lblHideTray.Size = new System.Drawing.Size(254, 17);
            this.lblHideTray.TabIndex = 92;
            this.lblHideTray.Text = "Hide MPfm in the system tray when closed";
            this.lblHideTray.Click += new System.EventHandler(this.lblHideTray_Click);
            // 
            // lblShowTray
            // 
            this.lblShowTray.AntiAliasingEnabled = true;
            this.lblShowTray.BackColor = System.Drawing.Color.Transparent;
            customFont3.EmbeddedFontName = "Junction";
            customFont3.IsBold = false;
            customFont3.IsItalic = false;
            customFont3.IsUnderline = false;
            customFont3.Size = 9;
            customFont3.StandardFontName = "Arial";
            customFont3.UseEmbeddedFont = true;
            this.lblShowTray.CustomFont = customFont3;
            this.lblShowTray.CustomFontName = "Junction";
            this.lblShowTray.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShowTray.Location = new System.Drawing.Point(20, 36);
            this.lblShowTray.Name = "lblShowTray";
            this.lblShowTray.Size = new System.Drawing.Size(208, 17);
            this.lblShowTray.TabIndex = 91;
            this.lblShowTray.Text = "Show MPfm in the system tray";
            this.lblShowTray.Click += new System.EventHandler(this.lblShowTray_Click);
            // 
            // chkShowTray
            // 
            this.chkShowTray.AutoSize = true;
            this.chkShowTray.BackColor = System.Drawing.Color.Transparent;
            this.chkShowTray.Checked = true;
            this.chkShowTray.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowTray.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowTray.Location = new System.Drawing.Point(5, 40);
            this.chkShowTray.Name = "chkShowTray";
            this.chkShowTray.Size = new System.Drawing.Size(15, 14);
            this.chkShowTray.TabIndex = 90;
            this.chkShowTray.UseVisualStyleBackColor = false;
            this.chkShowTray.CheckedChanged += new System.EventHandler(this.chkShowTray_CheckedChanged);
            // 
            // chkHideTray
            // 
            this.chkHideTray.AutoSize = true;
            this.chkHideTray.BackColor = System.Drawing.Color.Transparent;
            this.chkHideTray.Checked = true;
            this.chkHideTray.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHideTray.Enabled = false;
            this.chkHideTray.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHideTray.Location = new System.Drawing.Point(5, 61);
            this.chkHideTray.Name = "chkHideTray";
            this.chkHideTray.Size = new System.Drawing.Size(15, 14);
            this.chkHideTray.TabIndex = 89;
            this.chkHideTray.UseVisualStyleBackColor = false;
            this.chkHideTray.CheckedChanged += new System.EventHandler(this.chkHideTray_CheckedChanged);
            // 
            // tabTheme
            // 
            this.tabTheme.Controls.Add(this.panelTheme);
            this.tabTheme.Location = new System.Drawing.Point(4, 24);
            this.tabTheme.Name = "tabTheme";
            this.tabTheme.Size = new System.Drawing.Size(631, 350);
            this.tabTheme.TabIndex = 4;
            this.tabTheme.Text = "Theme";
            this.tabTheme.UseVisualStyleBackColor = true;
            // 
            // panelTheme
            // 
            this.panelTheme.AntiAliasingEnabled = true;
            this.panelTheme.Controls.Add(this.comboThemeControl);
            this.panelTheme.Controls.Add(this.textBox2);
            this.panelTheme.Controls.Add(this.lblFilterBySoundFormat);
            this.panelTheme.Controls.Add(this.label2);
            this.panelTheme.Controls.Add(this.textBox1);
            this.panelTheme.Controls.Add(this.label1);
            this.panelTheme.Controls.Add(this.button4);
            this.panelTheme.Controls.Add(this.button3);
            this.panelTheme.Controls.Add(this.button2);
            this.panelTheme.Controls.Add(this.splitContainer1);
            this.panelTheme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTheme.ExpandedHeight = 200;
            this.panelTheme.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelTheme.FontCollection = this.fontCollection;
            this.panelTheme.GradientColor1 = System.Drawing.Color.Silver;
            this.panelTheme.GradientColor2 = System.Drawing.Color.Gray;
            this.panelTheme.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelTheme.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelTheme.HeaderExpandable = false;
            this.panelTheme.HeaderExpanded = true;
            this.panelTheme.HeaderForeColor = System.Drawing.Color.Black;
            this.panelTheme.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelTheme.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelTheme.HeaderHeight = 30;
            this.panelTheme.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelTheme.HeaderTitle = "Theme";
            this.panelTheme.Location = new System.Drawing.Point(0, 0);
            this.panelTheme.Name = "panelTheme";
            this.panelTheme.Size = new System.Drawing.Size(631, 350);
            this.panelTheme.TabIndex = 18;
            // 
            // comboThemeControl
            // 
            this.comboThemeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboThemeControl.BackColor = System.Drawing.Color.Gainsboro;
            this.comboThemeControl.DisplayMember = "Main.SongBrowser";
            this.comboThemeControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboThemeControl.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboThemeControl.ForeColor = System.Drawing.Color.Black;
            this.comboThemeControl.FormattingEnabled = true;
            this.comboThemeControl.Items.AddRange(new object[] {
            "Main.SongBrowser",
            "Playlist.SongBrowser"});
            this.comboThemeControl.Location = new System.Drawing.Point(333, 56);
            this.comboThemeControl.Name = "comboThemeControl";
            this.comboThemeControl.Size = new System.Drawing.Size(216, 22);
            this.comboThemeControl.TabIndex = 99;
            this.comboThemeControl.ValueMember = "Main.SongBrowser";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(342, 32);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(160, 22);
            this.textBox2.TabIndex = 107;
            this.textBox2.Visible = false;
            // 
            // lblFilterBySoundFormat
            // 
            this.lblFilterBySoundFormat.AntiAliasingEnabled = true;
            this.lblFilterBySoundFormat.BackColor = System.Drawing.Color.Transparent;
            customFont4.EmbeddedFontName = "";
            customFont4.IsBold = false;
            customFont4.IsItalic = false;
            customFont4.IsUnderline = false;
            customFont4.Size = 8;
            customFont4.StandardFontName = "Arial";
            customFont4.UseEmbeddedFont = false;
            this.lblFilterBySoundFormat.CustomFont = customFont4;
            this.lblFilterBySoundFormat.CustomFontName = "Junction";
            this.lblFilterBySoundFormat.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilterBySoundFormat.ForeColor = System.Drawing.Color.Black;
            this.lblFilterBySoundFormat.Location = new System.Drawing.Point(278, 59);
            this.lblFilterBySoundFormat.Name = "lblFilterBySoundFormat";
            this.lblFilterBySoundFormat.Size = new System.Drawing.Size(56, 14);
            this.lblFilterBySoundFormat.TabIndex = 100;
            this.lblFilterBySoundFormat.Text = "Control :";
            this.lblFilterBySoundFormat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AntiAliasingEnabled = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            customFont5.EmbeddedFontName = "";
            customFont5.IsBold = false;
            customFont5.IsItalic = false;
            customFont5.IsUnderline = false;
            customFont5.Size = 8;
            customFont5.StandardFontName = "Arial";
            customFont5.UseEmbeddedFont = false;
            this.label2.CustomFont = customFont5;
            this.label2.CustomFontName = "Junction";
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(289, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 14);
            this.label2.TabIndex = 106;
            this.label2.Text = "Author :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(48, 56);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(227, 22);
            this.textBox1.TabIndex = 105;
            // 
            // label1
            // 
            this.label1.AntiAliasingEnabled = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            customFont6.EmbeddedFontName = "";
            customFont6.IsBold = false;
            customFont6.IsItalic = false;
            customFont6.IsUnderline = false;
            customFont6.Size = 8;
            customFont6.StandardFontName = "Arial";
            customFont6.UseEmbeddedFont = false;
            this.label1.CustomFont = customFont6;
            this.label1.CustomFontName = "Junction";
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(1, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 14);
            this.label1.TabIndex = 104;
            this.label1.Text = "Name :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button4
            // 
            this.button4.AntiAliasingEnabled = true;
            this.button4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.button4.BorderWidth = 1;
            this.button4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button4.CustomFontName = "Junction";
            this.button4.DisabledBorderColor = System.Drawing.Color.Gray;
            this.button4.DisabledFontColor = System.Drawing.Color.Gray;
            this.button4.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.button4.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.button4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.FontCollection = this.fontCollection;
            this.button4.FontColor = System.Drawing.Color.Black;
            this.button4.GradientColor1 = System.Drawing.Color.LightGray;
            this.button4.GradientColor2 = System.Drawing.Color.Gray;
            this.button4.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.button4.Image = global::MPfm.Properties.Resources.folder_page;
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.Location = new System.Drawing.Point(88, 30);
            this.button4.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.button4.MouseOverFontColor = System.Drawing.Color.Black;
            this.button4.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.button4.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(93, 24);
            this.button4.TabIndex = 103;
            this.button4.Text = "Load theme";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.AntiAliasingEnabled = true;
            this.button3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.button3.BorderWidth = 1;
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.CustomFontName = "Junction";
            this.button3.DisabledBorderColor = System.Drawing.Color.Gray;
            this.button3.DisabledFontColor = System.Drawing.Color.Gray;
            this.button3.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.button3.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.button3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.FontCollection = this.fontCollection;
            this.button3.FontColor = System.Drawing.Color.Black;
            this.button3.GradientColor1 = System.Drawing.Color.LightGray;
            this.button3.GradientColor2 = System.Drawing.Color.Gray;
            this.button3.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.button3.Image = global::MPfm.Properties.Resources.disk;
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(180, 30);
            this.button3.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.button3.MouseOverFontColor = System.Drawing.Color.Black;
            this.button3.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.button3.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(93, 24);
            this.button3.TabIndex = 102;
            this.button3.Text = "Save theme";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.AntiAliasingEnabled = true;
            this.button2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.button2.BorderWidth = 1;
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.CustomFontName = "Junction";
            this.button2.DisabledBorderColor = System.Drawing.Color.Gray;
            this.button2.DisabledFontColor = System.Drawing.Color.Gray;
            this.button2.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.button2.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.button2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.FontCollection = this.fontCollection;
            this.button2.FontColor = System.Drawing.Color.Black;
            this.button2.GradientColor1 = System.Drawing.Color.LightGray;
            this.button2.GradientColor2 = System.Drawing.Color.Gray;
            this.button2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.button2.Image = global::MPfm.Properties.Resources.page_white_text;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(0, 30);
            this.button2.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.button2.MouseOverFontColor = System.Drawing.Color.Black;
            this.button2.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.button2.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(89, 24);
            this.button2.TabIndex = 63;
            this.button2.Text = "New theme";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 84);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.songBrowser);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGridTheme);
            this.splitContainer1.Size = new System.Drawing.Size(631, 263);
            this.splitContainer1.SplitterDistance = 276;
            this.splitContainer1.TabIndex = 101;
            // 
            // songBrowser
            // 
            this.songBrowser.AntiAliasingEnabled = true;
            this.songBrowser.CanChangeOrderBy = false;
            this.songBrowser.CanMoveColumns = false;
            this.songBrowser.CanReorderItems = true;
            this.songBrowser.CanResizeColumns = true;
            this.songBrowser.CustomFontName = "Junction";
            this.songBrowser.DisplayDebugInformation = false;
            this.songBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.songBrowser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.songBrowser.FontCollection = this.fontCollection;
            this.songBrowser.ImageCacheSize = 10;
            this.songBrowser.Location = new System.Drawing.Point(0, 0);
            this.songBrowser.Name = "songBrowser";
            this.songBrowser.NowPlayingAudioFileId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.songBrowser.NowPlayingPlaylistItemId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.songBrowser.OrderByAscending = true;
            this.songBrowser.OrderByFieldName = "";
            this.songBrowser.Size = new System.Drawing.Size(276, 263);
            this.songBrowser.TabIndex = 98;
            this.songBrowser.Text = "songGridView1";
            songGridViewTheme1.AlbumCoverBackgroundColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            songGridViewTheme1.AlbumCoverBackgroundColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            customFont7.EmbeddedFontName = "";
            customFont7.IsBold = false;
            customFont7.IsItalic = false;
            customFont7.IsUnderline = false;
            customFont7.Size = 8;
            customFont7.StandardFontName = "Arial";
            customFont7.UseEmbeddedFont = false;
            songGridViewTheme1.Font = customFont7;
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
            this.songBrowser.Theme = songGridViewTheme1;
            // 
            // propertyGridTheme
            // 
            this.propertyGridTheme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridTheme.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGridTheme.Location = new System.Drawing.Point(0, 0);
            this.propertyGridTheme.Name = "propertyGridTheme";
            this.propertyGridTheme.Size = new System.Drawing.Size(351, 263);
            this.propertyGridTheme.TabIndex = 97;
            this.propertyGridTheme.ToolbarVisible = false;
            this.propertyGridTheme.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridTheme_PropertyValueChanged);
            // 
            // tabAudioSettings
            // 
            this.tabAudioSettings.Controls.Add(this.panelAudioSettings);
            this.tabAudioSettings.Location = new System.Drawing.Point(4, 24);
            this.tabAudioSettings.Name = "tabAudioSettings";
            this.tabAudioSettings.Size = new System.Drawing.Size(631, 350);
            this.tabAudioSettings.TabIndex = 2;
            this.tabAudioSettings.Text = "Audio";
            this.tabAudioSettings.UseVisualStyleBackColor = true;
            // 
            // panelAudioSettings
            // 
            this.panelAudioSettings.AntiAliasingEnabled = true;
            this.panelAudioSettings.Controls.Add(this.lblMixerSampleRateUnit);
            this.panelAudioSettings.Controls.Add(this.txtMixerSampleRate);
            this.panelAudioSettings.Controls.Add(this.lblBufferSizeUnit);
            this.panelAudioSettings.Controls.Add(this.txtBufferSize);
            this.panelAudioSettings.Controls.Add(this.lblUpdatePeriodUnit);
            this.panelAudioSettings.Controls.Add(this.txtUpdatePeriod);
            this.panelAudioSettings.Controls.Add(this.lblUpdatePeriod);
            this.panelAudioSettings.Controls.Add(this.lblBufferSize);
            this.panelAudioSettings.Controls.Add(this.lblMixerSampleRate);
            this.panelAudioSettings.Controls.Add(this.lblOutputDriver);
            this.panelAudioSettings.Controls.Add(this.btnTestSound);
            this.panelAudioSettings.Controls.Add(this.cboOutputDevices);
            this.panelAudioSettings.Controls.Add(this.lblDriver);
            this.panelAudioSettings.Controls.Add(this.cboDrivers);
            this.panelAudioSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAudioSettings.ExpandedHeight = 200;
            this.panelAudioSettings.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelAudioSettings.FontCollection = this.fontCollection;
            this.panelAudioSettings.GradientColor1 = System.Drawing.Color.Silver;
            this.panelAudioSettings.GradientColor2 = System.Drawing.Color.Gray;
            this.panelAudioSettings.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelAudioSettings.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelAudioSettings.HeaderExpandable = false;
            this.panelAudioSettings.HeaderExpanded = true;
            this.panelAudioSettings.HeaderForeColor = System.Drawing.Color.Black;
            this.panelAudioSettings.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelAudioSettings.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelAudioSettings.HeaderHeight = 30;
            this.panelAudioSettings.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelAudioSettings.HeaderTitle = "Audio Settings";
            this.panelAudioSettings.Location = new System.Drawing.Point(0, 0);
            this.panelAudioSettings.Name = "panelAudioSettings";
            this.panelAudioSettings.Size = new System.Drawing.Size(631, 350);
            this.panelAudioSettings.TabIndex = 16;
            // 
            // lblMixerSampleRateUnit
            // 
            this.lblMixerSampleRateUnit.AntiAliasingEnabled = true;
            this.lblMixerSampleRateUnit.BackColor = System.Drawing.Color.Transparent;
            customFont8.EmbeddedFontName = "";
            customFont8.IsBold = false;
            customFont8.IsItalic = false;
            customFont8.IsUnderline = false;
            customFont8.Size = 8;
            customFont8.StandardFontName = "Arial";
            customFont8.UseEmbeddedFont = false;
            this.lblMixerSampleRateUnit.CustomFont = customFont8;
            this.lblMixerSampleRateUnit.CustomFontName = "Junction";
            this.lblMixerSampleRateUnit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMixerSampleRateUnit.Location = new System.Drawing.Point(70, 147);
            this.lblMixerSampleRateUnit.Name = "lblMixerSampleRateUnit";
            this.lblMixerSampleRateUnit.Size = new System.Drawing.Size(39, 17);
            this.lblMixerSampleRateUnit.TabIndex = 93;
            this.lblMixerSampleRateUnit.Text = "Hz";
            // 
            // txtMixerSampleRate
            // 
            this.txtMixerSampleRate.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMixerSampleRate.Location = new System.Drawing.Point(6, 147);
            this.txtMixerSampleRate.Maximum = new decimal(new int[] {
            96000,
            0,
            0,
            0});
            this.txtMixerSampleRate.Minimum = new decimal(new int[] {
            44100,
            0,
            0,
            0});
            this.txtMixerSampleRate.Name = "txtMixerSampleRate";
            this.txtMixerSampleRate.Size = new System.Drawing.Size(62, 23);
            this.txtMixerSampleRate.TabIndex = 92;
            this.txtMixerSampleRate.Value = new decimal(new int[] {
            44100,
            0,
            0,
            0});
            this.txtMixerSampleRate.ValueChanged += new System.EventHandler(this.txtMixerSampleRate_ValueChanged);
            // 
            // lblBufferSizeUnit
            // 
            this.lblBufferSizeUnit.AntiAliasingEnabled = true;
            this.lblBufferSizeUnit.BackColor = System.Drawing.Color.Transparent;
            customFont9.EmbeddedFontName = "";
            customFont9.IsBold = false;
            customFont9.IsItalic = false;
            customFont9.IsUnderline = false;
            customFont9.Size = 8;
            customFont9.StandardFontName = "Arial";
            customFont9.UseEmbeddedFont = false;
            this.lblBufferSizeUnit.CustomFont = customFont9;
            this.lblBufferSizeUnit.CustomFontName = "Junction";
            this.lblBufferSizeUnit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBufferSizeUnit.Location = new System.Drawing.Point(175, 147);
            this.lblBufferSizeUnit.Name = "lblBufferSizeUnit";
            this.lblBufferSizeUnit.Size = new System.Drawing.Size(39, 17);
            this.lblBufferSizeUnit.TabIndex = 91;
            this.lblBufferSizeUnit.Text = "ms";
            // 
            // txtBufferSize
            // 
            this.txtBufferSize.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBufferSize.Location = new System.Drawing.Point(125, 147);
            this.txtBufferSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtBufferSize.Name = "txtBufferSize";
            this.txtBufferSize.Size = new System.Drawing.Size(48, 23);
            this.txtBufferSize.TabIndex = 90;
            this.txtBufferSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtBufferSize.ValueChanged += new System.EventHandler(this.txtBufferSize_ValueChanged);
            // 
            // lblUpdatePeriodUnit
            // 
            this.lblUpdatePeriodUnit.AntiAliasingEnabled = true;
            this.lblUpdatePeriodUnit.BackColor = System.Drawing.Color.Transparent;
            customFont10.EmbeddedFontName = "";
            customFont10.IsBold = false;
            customFont10.IsItalic = false;
            customFont10.IsUnderline = false;
            customFont10.Size = 8;
            customFont10.StandardFontName = "Arial";
            customFont10.UseEmbeddedFont = false;
            this.lblUpdatePeriodUnit.CustomFont = customFont10;
            this.lblUpdatePeriodUnit.CustomFontName = "Junction";
            this.lblUpdatePeriodUnit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdatePeriodUnit.Location = new System.Drawing.Point(269, 147);
            this.lblUpdatePeriodUnit.Name = "lblUpdatePeriodUnit";
            this.lblUpdatePeriodUnit.Size = new System.Drawing.Size(39, 17);
            this.lblUpdatePeriodUnit.TabIndex = 89;
            this.lblUpdatePeriodUnit.Text = "ms";
            // 
            // txtUpdatePeriod
            // 
            this.txtUpdatePeriod.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUpdatePeriod.Location = new System.Drawing.Point(226, 147);
            this.txtUpdatePeriod.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtUpdatePeriod.Name = "txtUpdatePeriod";
            this.txtUpdatePeriod.Size = new System.Drawing.Size(41, 23);
            this.txtUpdatePeriod.TabIndex = 88;
            this.txtUpdatePeriod.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtUpdatePeriod.ValueChanged += new System.EventHandler(this.txtUpdatePeriod_ValueChanged);
            // 
            // lblUpdatePeriod
            // 
            this.lblUpdatePeriod.AntiAliasingEnabled = true;
            this.lblUpdatePeriod.BackColor = System.Drawing.Color.Transparent;
            customFont11.EmbeddedFontName = "";
            customFont11.IsBold = false;
            customFont11.IsItalic = false;
            customFont11.IsUnderline = false;
            customFont11.Size = 8;
            customFont11.StandardFontName = "Arial";
            customFont11.UseEmbeddedFont = false;
            this.lblUpdatePeriod.CustomFont = customFont11;
            this.lblUpdatePeriod.CustomFontName = "Junction";
            this.lblUpdatePeriod.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdatePeriod.Location = new System.Drawing.Point(223, 127);
            this.lblUpdatePeriod.Name = "lblUpdatePeriod";
            this.lblUpdatePeriod.Size = new System.Drawing.Size(106, 17);
            this.lblUpdatePeriod.TabIndex = 86;
            this.lblUpdatePeriod.Text = "Update period:";
            // 
            // lblBufferSize
            // 
            this.lblBufferSize.AntiAliasingEnabled = true;
            this.lblBufferSize.BackColor = System.Drawing.Color.Transparent;
            customFont12.EmbeddedFontName = "";
            customFont12.IsBold = false;
            customFont12.IsItalic = false;
            customFont12.IsUnderline = false;
            customFont12.Size = 8;
            customFont12.StandardFontName = "Arial";
            customFont12.UseEmbeddedFont = false;
            this.lblBufferSize.CustomFont = customFont12;
            this.lblBufferSize.CustomFontName = "Junction";
            this.lblBufferSize.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBufferSize.Location = new System.Drawing.Point(122, 127);
            this.lblBufferSize.Name = "lblBufferSize";
            this.lblBufferSize.Size = new System.Drawing.Size(106, 17);
            this.lblBufferSize.TabIndex = 84;
            this.lblBufferSize.Text = "Buffer size:";
            // 
            // lblMixerSampleRate
            // 
            this.lblMixerSampleRate.AntiAliasingEnabled = true;
            this.lblMixerSampleRate.BackColor = System.Drawing.Color.Transparent;
            customFont13.EmbeddedFontName = "";
            customFont13.IsBold = false;
            customFont13.IsItalic = false;
            customFont13.IsUnderline = false;
            customFont13.Size = 8;
            customFont13.StandardFontName = "Arial";
            customFont13.UseEmbeddedFont = false;
            this.lblMixerSampleRate.CustomFont = customFont13;
            this.lblMixerSampleRate.CustomFontName = "Junction";
            this.lblMixerSampleRate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMixerSampleRate.Location = new System.Drawing.Point(3, 127);
            this.lblMixerSampleRate.Name = "lblMixerSampleRate";
            this.lblMixerSampleRate.Size = new System.Drawing.Size(106, 17);
            this.lblMixerSampleRate.TabIndex = 82;
            this.lblMixerSampleRate.Text = "Mixer sample rate:";
            // 
            // lblOutputDriver
            // 
            this.lblOutputDriver.AntiAliasingEnabled = true;
            this.lblOutputDriver.BackColor = System.Drawing.Color.Transparent;
            customFont14.EmbeddedFontName = "";
            customFont14.IsBold = false;
            customFont14.IsItalic = false;
            customFont14.IsUnderline = false;
            customFont14.Size = 8;
            customFont14.StandardFontName = "Arial";
            customFont14.UseEmbeddedFont = false;
            this.lblOutputDriver.CustomFont = customFont14;
            this.lblOutputDriver.CustomFontName = "Junction";
            this.lblOutputDriver.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutputDriver.Location = new System.Drawing.Point(3, 80);
            this.lblOutputDriver.Name = "lblOutputDriver";
            this.lblOutputDriver.Size = new System.Drawing.Size(89, 17);
            this.lblOutputDriver.TabIndex = 11;
            this.lblOutputDriver.Text = "Output device:";
            // 
            // btnTestSound
            // 
            this.btnTestSound.AntiAliasingEnabled = true;
            this.btnTestSound.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnTestSound.BorderWidth = 1;
            this.btnTestSound.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestSound.CustomFontName = "Junction";
            this.btnTestSound.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnTestSound.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnTestSound.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnTestSound.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnTestSound.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestSound.FontCollection = this.fontCollection;
            this.btnTestSound.FontColor = System.Drawing.Color.Black;
            this.btnTestSound.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnTestSound.GradientColor2 = System.Drawing.Color.Gray;
            this.btnTestSound.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnTestSound.Image = global::MPfm.Properties.Resources.sound;
            this.btnTestSound.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnTestSound.Location = new System.Drawing.Point(6, 179);
            this.btnTestSound.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnTestSound.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnTestSound.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnTestSound.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnTestSound.Name = "btnTestSound";
            this.btnTestSound.Size = new System.Drawing.Size(131, 40);
            this.btnTestSound.TabIndex = 81;
            this.btnTestSound.Text = "Test audio settings";
            this.btnTestSound.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnTestSound.UseVisualStyleBackColor = true;
            this.btnTestSound.Click += new System.EventHandler(this.btnTestSound_Click);
            // 
            // cboOutputDevices
            // 
            this.cboOutputDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOutputDevices.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboOutputDevices.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboOutputDevices.DisplayMember = "Name";
            this.cboOutputDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOutputDevices.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboOutputDevices.FormattingEnabled = true;
            this.cboOutputDevices.Location = new System.Drawing.Point(6, 101);
            this.cboOutputDevices.Name = "cboOutputDevices";
            this.cboOutputDevices.Size = new System.Drawing.Size(611, 23);
            this.cboOutputDevices.TabIndex = 9;
            this.cboOutputDevices.ValueMember = "Id";
            this.cboOutputDevices.SelectedIndexChanged += new System.EventHandler(this.cboDriverOrOutputType_SelectedIndexChanged);
            // 
            // lblDriver
            // 
            this.lblDriver.AntiAliasingEnabled = true;
            this.lblDriver.BackColor = System.Drawing.Color.Transparent;
            customFont15.EmbeddedFontName = "";
            customFont15.IsBold = false;
            customFont15.IsItalic = false;
            customFont15.IsUnderline = false;
            customFont15.Size = 8;
            customFont15.StandardFontName = "Arial";
            customFont15.UseEmbeddedFont = false;
            this.lblDriver.CustomFont = customFont15;
            this.lblDriver.CustomFontName = "Junction";
            this.lblDriver.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDriver.Location = new System.Drawing.Point(3, 33);
            this.lblDriver.Name = "lblDriver";
            this.lblDriver.Size = new System.Drawing.Size(89, 17);
            this.lblDriver.TabIndex = 13;
            this.lblDriver.Text = "Driver:";
            // 
            // cboDrivers
            // 
            this.cboDrivers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDrivers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboDrivers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDrivers.DisplayMember = "Title";
            this.cboDrivers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDrivers.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDrivers.FormattingEnabled = true;
            this.cboDrivers.Location = new System.Drawing.Point(6, 53);
            this.cboDrivers.Name = "cboDrivers";
            this.cboDrivers.Size = new System.Drawing.Size(611, 23);
            this.cboDrivers.TabIndex = 12;
            this.cboDrivers.ValueMember = "DriverType";
            this.cboDrivers.SelectedIndexChanged += new System.EventHandler(this.cboDriverOrOutputType_SelectedIndexChanged);
            // 
            // tabLibrary
            // 
            this.tabLibrary.Controls.Add(this.panelLibrarySettings);
            this.tabLibrary.Location = new System.Drawing.Point(4, 24);
            this.tabLibrary.Name = "tabLibrary";
            this.tabLibrary.Size = new System.Drawing.Size(631, 350);
            this.tabLibrary.TabIndex = 1;
            this.tabLibrary.Text = "Library";
            this.tabLibrary.UseVisualStyleBackColor = true;
            // 
            // panelLibrarySettings
            // 
            this.panelLibrarySettings.AntiAliasingEnabled = true;
            this.panelLibrarySettings.Controls.Add(this.btnRemoveFolder);
            this.panelLibrarySettings.Controls.Add(this.viewFolders);
            this.panelLibrarySettings.Controls.Add(this.btnAddFolder);
            this.panelLibrarySettings.Controls.Add(this.Button1);
            this.panelLibrarySettings.Controls.Add(this.lblFoldersTitle);
            this.panelLibrarySettings.Controls.Add(this.btnResetLibrary);
            this.panelLibrarySettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLibrarySettings.ExpandedHeight = 200;
            this.panelLibrarySettings.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLibrarySettings.FontCollection = this.fontCollection;
            this.panelLibrarySettings.GradientColor1 = System.Drawing.Color.Silver;
            this.panelLibrarySettings.GradientColor2 = System.Drawing.Color.Gray;
            this.panelLibrarySettings.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelLibrarySettings.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelLibrarySettings.HeaderExpandable = false;
            this.panelLibrarySettings.HeaderExpanded = true;
            this.panelLibrarySettings.HeaderForeColor = System.Drawing.Color.Black;
            this.panelLibrarySettings.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelLibrarySettings.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelLibrarySettings.HeaderHeight = 30;
            this.panelLibrarySettings.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelLibrarySettings.HeaderTitle = "Library Settings";
            this.panelLibrarySettings.Location = new System.Drawing.Point(0, 0);
            this.panelLibrarySettings.Name = "panelLibrarySettings";
            this.panelLibrarySettings.Size = new System.Drawing.Size(501, 281);
            this.panelLibrarySettings.TabIndex = 17;
            // 
            // btnRemoveFolder
            // 
            this.btnRemoveFolder.AntiAliasingEnabled = true;
            this.btnRemoveFolder.BorderColor = System.Drawing.Color.DimGray;
            this.btnRemoveFolder.BorderWidth = 1;
            this.btnRemoveFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemoveFolder.CustomFontName = "Junction";
            this.btnRemoveFolder.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnRemoveFolder.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnRemoveFolder.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnRemoveFolder.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnRemoveFolder.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveFolder.FontCollection = this.fontCollection;
            this.btnRemoveFolder.FontColor = System.Drawing.Color.Black;
            this.btnRemoveFolder.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnRemoveFolder.GradientColor2 = System.Drawing.Color.Gray;
            this.btnRemoveFolder.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnRemoveFolder.Image = global::MPfm.Properties.Resources.delete;
            this.btnRemoveFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRemoveFolder.Location = new System.Drawing.Point(104, 56);
            this.btnRemoveFolder.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnRemoveFolder.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnRemoveFolder.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnRemoveFolder.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnRemoveFolder.Name = "btnRemoveFolder";
            this.btnRemoveFolder.Size = new System.Drawing.Size(122, 25);
            this.btnRemoveFolder.TabIndex = 67;
            this.btnRemoveFolder.Text = "Remove Folder(s)";
            this.btnRemoveFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRemoveFolder.UseVisualStyleBackColor = true;
            this.btnRemoveFolder.Click += new System.EventHandler(this.btnRemoveFolder_Click);
            // 
            // viewFolders
            // 
            this.viewFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewFolders.AntiAliasingEnabled = true;
            this.viewFolders.BackColor = System.Drawing.Color.White;
            this.viewFolders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.viewFolders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.viewFolders.CustomFontName = "NeuzeitS";
            this.viewFolders.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.viewFolders.FontCollection = this.fontCollection;
            this.viewFolders.FullRowSelect = true;
            this.viewFolders.GradientColor1 = System.Drawing.Color.Gainsboro;
            this.viewFolders.GradientColor2 = System.Drawing.Color.Silver;
            this.viewFolders.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.viewFolders.GridLines = true;
            this.viewFolders.HeaderCustomFontName = "Avenir";
            this.viewFolders.HeaderForeColor = System.Drawing.Color.Black;
            this.viewFolders.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.viewFolders.HeaderGradientColor2 = System.Drawing.Color.DarkGray;
            this.viewFolders.HeaderHeight = 0;
            this.viewFolders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.viewFolders.Location = new System.Drawing.Point(7, 87);
            this.viewFolders.Name = "viewFolders";
            this.viewFolders.OwnerDraw = true;
            this.viewFolders.SelectedColor = System.Drawing.Color.Gray;
            this.viewFolders.Size = new System.Drawing.Size(488, 130);
            this.viewFolders.TabIndex = 23;
            this.viewFolders.UseCompatibleStateImageBehavior = false;
            this.viewFolders.View = System.Windows.Forms.View.Details;
            this.viewFolders.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.viewFolders_ColumnWidthChanging);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Path";
            this.columnHeader1.Width = 359;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Recursive";
            this.columnHeader2.Width = 117;
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.AntiAliasingEnabled = true;
            this.btnAddFolder.BorderColor = System.Drawing.Color.DimGray;
            this.btnAddFolder.BorderWidth = 1;
            this.btnAddFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddFolder.CustomFontName = "Junction";
            this.btnAddFolder.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnAddFolder.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnAddFolder.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnAddFolder.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnAddFolder.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddFolder.FontCollection = this.fontCollection;
            this.btnAddFolder.FontColor = System.Drawing.Color.Black;
            this.btnAddFolder.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnAddFolder.GradientColor2 = System.Drawing.Color.Gray;
            this.btnAddFolder.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnAddFolder.Image = global::MPfm.Properties.Resources.add;
            this.btnAddFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddFolder.Location = new System.Drawing.Point(7, 56);
            this.btnAddFolder.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnAddFolder.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnAddFolder.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnAddFolder.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Size = new System.Drawing.Size(91, 25);
            this.btnAddFolder.TabIndex = 66;
            this.btnAddFolder.Text = "Add Folder";
            this.btnAddFolder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddFolder.UseVisualStyleBackColor = true;
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // Button1
            // 
            this.Button1.AntiAliasingEnabled = true;
            this.Button1.BorderColor = System.Drawing.Color.DimGray;
            this.Button1.BorderWidth = 1;
            this.Button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button1.CustomFontName = "Junction";
            this.Button1.DisabledBorderColor = System.Drawing.Color.Gray;
            this.Button1.DisabledFontColor = System.Drawing.Color.Silver;
            this.Button1.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.Button1.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.Button1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button1.FontCollection = this.fontCollection;
            this.Button1.FontColor = System.Drawing.Color.Black;
            this.Button1.GradientColor1 = System.Drawing.Color.LightGray;
            this.Button1.GradientColor2 = System.Drawing.Color.Gray;
            this.Button1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.Button1.Image = global::MPfm.Properties.Resources.arrow_refresh;
            this.Button1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Button1.Location = new System.Drawing.Point(113, 223);
            this.Button1.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.Button1.MouseOverFontColor = System.Drawing.Color.Black;
            this.Button1.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.Button1.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(99, 42);
            this.Button1.TabIndex = 65;
            this.Button1.Text = "Update Library";
            this.Button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Visible = false;
            this.Button1.Click += new System.EventHandler(this.btnUpdateLibrary_Click);
            // 
            // lblFoldersTitle
            // 
            this.lblFoldersTitle.AntiAliasingEnabled = true;
            this.lblFoldersTitle.BackColor = System.Drawing.Color.Transparent;
            customFont16.EmbeddedFontName = "";
            customFont16.IsBold = false;
            customFont16.IsItalic = false;
            customFont16.IsUnderline = false;
            customFont16.Size = 8;
            customFont16.StandardFontName = "Arial";
            customFont16.UseEmbeddedFont = false;
            this.lblFoldersTitle.CustomFont = customFont16;
            this.lblFoldersTitle.CustomFontName = "Junction";
            this.lblFoldersTitle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFoldersTitle.Location = new System.Drawing.Point(3, 36);
            this.lblFoldersTitle.Name = "lblFoldersTitle";
            this.lblFoldersTitle.Size = new System.Drawing.Size(69, 17);
            this.lblFoldersTitle.TabIndex = 6;
            this.lblFoldersTitle.Text = "Folders :";
            // 
            // btnResetLibrary
            // 
            this.btnResetLibrary.AntiAliasingEnabled = true;
            this.btnResetLibrary.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnResetLibrary.BorderWidth = 1;
            this.btnResetLibrary.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResetLibrary.CustomFontName = "Junction";
            this.btnResetLibrary.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnResetLibrary.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnResetLibrary.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnResetLibrary.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnResetLibrary.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetLibrary.FontCollection = this.fontCollection;
            this.btnResetLibrary.FontColor = System.Drawing.Color.Black;
            this.btnResetLibrary.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnResetLibrary.GradientColor2 = System.Drawing.Color.Gray;
            this.btnResetLibrary.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnResetLibrary.Image = global::MPfm.Properties.Resources.exclamation;
            this.btnResetLibrary.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnResetLibrary.Location = new System.Drawing.Point(7, 223);
            this.btnResetLibrary.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnResetLibrary.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnResetLibrary.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnResetLibrary.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnResetLibrary.Name = "btnResetLibrary";
            this.btnResetLibrary.Size = new System.Drawing.Size(99, 42);
            this.btnResetLibrary.TabIndex = 64;
            this.btnResetLibrary.Text = "Reset Library";
            this.btnResetLibrary.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnResetLibrary.UseVisualStyleBackColor = true;
            this.btnResetLibrary.Click += new System.EventHandler(this.btnResetLibrary_Click);
            // 
            // dialogAddFolder
            // 
            this.dialogAddFolder.Description = "Please select a folder to add to the music library.";
            this.dialogAddFolder.ShowNewFolderButton = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AntiAliasingEnabled = true;
            this.btnClose.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnClose.BorderWidth = 1;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.CustomFontName = "Junction";
            this.btnClose.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.FontCollection = this.fontCollection;
            this.btnClose.FontColor = System.Drawing.Color.Black;
            this.btnClose.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnClose.GradientColor2 = System.Drawing.Color.Gray;
            this.btnClose.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnClose.Image = global::MPfm.Properties.Resources.cancel;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnClose.Location = new System.Drawing.Point(543, 384);
            this.btnClose.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnClose.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnClose.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnClose.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 40);
            this.btnClose.TabIndex = 62;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dialogOpenFile
            // 
            this.dialogOpenFile.Filter = "Audio files (*.mp3,*.flac,*.ogg, *.wav)|*.mp3;*.flac;*.ogg,*.wav";
            this.dialogOpenFile.Title = "Please select an audio file to play";
            // 
            // panelBackground
            // 
            this.panelBackground.AntiAliasingEnabled = true;
            this.panelBackground.Controls.Add(this.btnClose);
            this.panelBackground.Controls.Add(this.tabs);
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.ExpandedHeight = 200;
            this.panelBackground.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelBackground.FontCollection = this.fontCollection;
            this.panelBackground.GradientColor1 = System.Drawing.Color.Silver;
            this.panelBackground.GradientColor2 = System.Drawing.Color.Gray;
            this.panelBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelBackground.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelBackground.HeaderExpandable = false;
            this.panelBackground.HeaderExpanded = true;
            this.panelBackground.HeaderForeColor = System.Drawing.Color.Black;
            this.panelBackground.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelBackground.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelBackground.HeaderHeight = 0;
            this.panelBackground.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(639, 431);
            this.panelBackground.TabIndex = 63;
            // 
            // frmSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(639, 431);
            this.Controls.Add(this.panelBackground);
            this.CustomFontName = "NeuzeitS";
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontCollection = this.fontCollection;
            this.GradientColor1 = System.Drawing.Color.Silver;
            this.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(525, 400);
            this.Name = "frmSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSettings_FormClosing);
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.Shown += new System.EventHandler(this.frmSettings_Shown);
            this.tabs.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.panelGeneralSettings.ResumeLayout(false);
            this.panelGeneralSettings.PerformLayout();
            this.tabTheme.ResumeLayout(false);
            this.panelTheme.ResumeLayout(false);
            this.panelTheme.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabAudioSettings.ResumeLayout(false);
            this.panelAudioSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMixerSampleRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBufferSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUpdatePeriod)).EndInit();
            this.tabLibrary.ResumeLayout(false);
            this.panelLibrarySettings.ResumeLayout(false);
            this.panelBackground.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabLibrary;
        private System.Windows.Forms.TabPage tabAudioSettings;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private MPfm.WindowsControls.Label lblFoldersTitle;
        public System.Windows.Forms.FolderBrowserDialog dialogAddFolder;        
        private MPfm.WindowsControls.ListView viewFolders;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ComboBox cboOutputDevices;
        private MPfm.WindowsControls.Button btnResetLibrary;
        private MPfm.WindowsControls.Button btnClose;
        private MPfm.WindowsControls.Button Button1;
        private MPfm.WindowsControls.Label lblOutputDriver;
        private MPfm.WindowsControls.Label lblDriver;
        private System.Windows.Forms.ComboBox cboDrivers;
        private MPfm.WindowsControls.Button btnTestSound;
        private System.Windows.Forms.OpenFileDialog dialogOpenFile;
        private MPfm.WindowsControls.Panel panelAudioSettings;
        private MPfm.WindowsControls.Panel panelLibrarySettings;
        private MPfm.WindowsControls.Button btnRemoveFolder;
        private MPfm.WindowsControls.Button btnAddFolder;
        private WindowsControls.Panel panelBackground;
        private System.Windows.Forms.TabPage tabGeneral;
        private WindowsControls.Panel panelGeneralSettings;
        private WindowsControls.Label lblTest;
        private WindowsControls.Button btnStopPeak;
        private WindowsControls.Button btnTestPeak;
        private System.Windows.Forms.TextBox txtPath;
        private WindowsControls.Label lblHideTray;
        private WindowsControls.Label lblShowTray;
        private System.Windows.Forms.CheckBox chkShowTray;
        private System.Windows.Forms.CheckBox chkHideTray;
        private WindowsControls.Label lblMixerSampleRate;
        private WindowsControls.Label lblBufferSize;
        private WindowsControls.Label lblUpdatePeriod;
        private System.Windows.Forms.NumericUpDown txtUpdatePeriod;
        private WindowsControls.Label lblUpdatePeriodUnit;
        private WindowsControls.Label lblMixerSampleRateUnit;
        private System.Windows.Forms.NumericUpDown txtMixerSampleRate;
        private WindowsControls.Label lblBufferSizeUnit;
        private System.Windows.Forms.NumericUpDown txtBufferSize;
        private System.Windows.Forms.TabPage tabTheme;
        private WindowsControls.Panel panelTheme;
        private WindowsControls.SongGridView songBrowser;
        public System.Windows.Forms.PropertyGrid propertyGridTheme;
        private System.Windows.Forms.ComboBox comboThemeControl;
        private WindowsControls.Label lblFilterBySoundFormat;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private WindowsControls.Button button2;
        private WindowsControls.Button button4;
        private WindowsControls.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private WindowsControls.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private WindowsControls.Label label2;
    }
}