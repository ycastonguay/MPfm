namespace TestControls
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
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.groupSongGridView = new System.Windows.Forms.GroupBox();
            this.txtSearchArtistName = new System.Windows.Forms.TextBox();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.comboCustomFontName = new System.Windows.Forms.ComboBox();
            this.comboStandardFontName = new System.Windows.Forms.ComboBox();
            this.comboDisplayType = new System.Windows.Forms.ComboBox();
            this.groupGeneral = new System.Windows.Forms.GroupBox();
            this.radioUseStandardFont = new System.Windows.Forms.RadioButton();
            this.radioUseCustomFont = new System.Windows.Forms.RadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblUseStandardFont = new MPfm.WindowsControls.Label();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.label5 = new MPfm.WindowsControls.Label();
            this.label4 = new MPfm.WindowsControls.Label();
            this.lblUseCustomFont = new MPfm.WindowsControls.Label();
            this.lblFontSize = new MPfm.WindowsControls.Label();
            this.trackFontSize = new MPfm.WindowsControls.TrackBar();
            this.lblPadding = new MPfm.WindowsControls.Label();
            this.trackPadding = new MPfm.WindowsControls.TrackBar();
            this.label1 = new MPfm.WindowsControls.Label();
            this.label3 = new MPfm.WindowsControls.Label();
            this.label2 = new MPfm.WindowsControls.Label();
            this.lblDisplayDebugInformation = new MPfm.WindowsControls.Label();
            this.songGridView = new MPfm.WindowsControls.SongGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupSongGridView.SuspendLayout();
            this.groupGeneral.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupSongGridView
            // 
            this.groupSongGridView.Controls.Add(this.lblPadding);
            this.groupSongGridView.Controls.Add(this.trackPadding);
            this.groupSongGridView.Controls.Add(this.label1);
            this.groupSongGridView.Controls.Add(this.txtSearchArtistName);
            this.groupSongGridView.Controls.Add(this.label3);
            this.groupSongGridView.Controls.Add(this.label2);
            this.groupSongGridView.Controls.Add(this.chkDebug);
            this.groupSongGridView.Controls.Add(this.lblDisplayDebugInformation);
            this.groupSongGridView.Location = new System.Drawing.Point(3, 73);
            this.groupSongGridView.Name = "groupSongGridView";
            this.groupSongGridView.Size = new System.Drawing.Size(933, 94);
            this.groupSongGridView.TabIndex = 5;
            this.groupSongGridView.TabStop = false;
            this.groupSongGridView.Text = "111";
            // 
            // txtSearchArtistName
            // 
            this.txtSearchArtistName.Location = new System.Drawing.Point(186, 57);
            this.txtSearchArtistName.Name = "txtSearchArtistName";
            this.txtSearchArtistName.Size = new System.Drawing.Size(150, 20);
            this.txtSearchArtistName.TabIndex = 7;
            this.txtSearchArtistName.TextChanged += new System.EventHandler(this.txtSearchArtistName_TextChanged);
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(7, 73);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(15, 14);
            this.chkDebug.TabIndex = 6;
            this.chkDebug.UseVisualStyleBackColor = true;
            this.chkDebug.CheckedChanged += new System.EventHandler(this.chkDebug_CheckedChanged);
            // 
            // comboCustomFontName
            // 
            this.comboCustomFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCustomFontName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboCustomFontName.FormattingEnabled = true;
            this.comboCustomFontName.Location = new System.Drawing.Point(161, 40);
            this.comboCustomFontName.Name = "comboCustomFontName";
            this.comboCustomFontName.Size = new System.Drawing.Size(121, 22);
            this.comboCustomFontName.TabIndex = 11;
            this.comboCustomFontName.SelectedIndexChanged += new System.EventHandler(this.comboCustomFontName_SelectedIndexChanged);
            // 
            // comboStandardFontName
            // 
            this.comboStandardFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStandardFontName.Enabled = false;
            this.comboStandardFontName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboStandardFontName.FormattingEnabled = true;
            this.comboStandardFontName.Location = new System.Drawing.Point(293, 40);
            this.comboStandardFontName.Name = "comboStandardFontName";
            this.comboStandardFontName.Size = new System.Drawing.Size(121, 22);
            this.comboStandardFontName.TabIndex = 9;
            this.comboStandardFontName.SelectedIndexChanged += new System.EventHandler(this.comboFontName_SelectedIndexChanged);
            // 
            // comboDisplayType
            // 
            this.comboDisplayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDisplayType.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboDisplayType.FormattingEnabled = true;
            this.comboDisplayType.Items.AddRange(new object[] {
            "AlbumView",
            "SongGridView"});
            this.comboDisplayType.Location = new System.Drawing.Point(9, 40);
            this.comboDisplayType.Name = "comboDisplayType";
            this.comboDisplayType.Size = new System.Drawing.Size(145, 22);
            this.comboDisplayType.TabIndex = 7;
            this.comboDisplayType.SelectedIndexChanged += new System.EventHandler(this.comboDisplayType_SelectedIndexChanged);
            // 
            // groupGeneral
            // 
            this.groupGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupGeneral.Controls.Add(this.lblUseStandardFont);
            this.groupGeneral.Controls.Add(this.radioUseStandardFont);
            this.groupGeneral.Controls.Add(this.label5);
            this.groupGeneral.Controls.Add(this.label4);
            this.groupGeneral.Controls.Add(this.radioUseCustomFont);
            this.groupGeneral.Controls.Add(this.comboDisplayType);
            this.groupGeneral.Controls.Add(this.lblUseCustomFont);
            this.groupGeneral.Controls.Add(this.lblFontSize);
            this.groupGeneral.Controls.Add(this.trackFontSize);
            this.groupGeneral.Controls.Add(this.comboStandardFontName);
            this.groupGeneral.Controls.Add(this.comboCustomFontName);
            this.groupGeneral.Location = new System.Drawing.Point(2, 3);
            this.groupGeneral.Name = "groupGeneral";
            this.groupGeneral.Size = new System.Drawing.Size(933, 68);
            this.groupGeneral.TabIndex = 7;
            this.groupGeneral.TabStop = false;
            this.groupGeneral.Text = "_";
            // 
            // radioUseStandardFont
            // 
            this.radioUseStandardFont.AutoSize = true;
            this.radioUseStandardFont.Location = new System.Drawing.Point(293, 21);
            this.radioUseStandardFont.Name = "radioUseStandardFont";
            this.radioUseStandardFont.Size = new System.Drawing.Size(14, 13);
            this.radioUseStandardFont.TabIndex = 18;
            this.radioUseStandardFont.UseVisualStyleBackColor = true;
            this.radioUseStandardFont.CheckedChanged += new System.EventHandler(this.radioUseStandardFont_CheckedChanged);
            // 
            // radioUseCustomFont
            // 
            this.radioUseCustomFont.AutoSize = true;
            this.radioUseCustomFont.Checked = true;
            this.radioUseCustomFont.Location = new System.Drawing.Point(161, 21);
            this.radioUseCustomFont.Name = "radioUseCustomFont";
            this.radioUseCustomFont.Size = new System.Drawing.Size(14, 13);
            this.radioUseCustomFont.TabIndex = 17;
            this.radioUseCustomFont.TabStop = true;
            this.radioUseCustomFont.UseVisualStyleBackColor = true;
            this.radioUseCustomFont.CheckedChanged += new System.EventHandler(this.radioUseCustomFont_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblUseStandardFont
            // 
            this.lblUseStandardFont.AntiAliasingEnabled = true;
            this.lblUseStandardFont.CustomFontName = "Junction";
            this.lblUseStandardFont.FontCollection = this.fontCollection;
            this.lblUseStandardFont.Location = new System.Drawing.Point(306, 19);
            this.lblUseStandardFont.Name = "lblUseStandardFont";
            this.lblUseStandardFont.Size = new System.Drawing.Size(107, 18);
            this.lblUseStandardFont.TabIndex = 19;
            this.lblUseStandardFont.Text = "Use Standard Font";
            this.lblUseStandardFont.Click += new System.EventHandler(this.lblUseStandardFont_Click);
            // 
            // fontCollection
            // 
            customFont1.AssemblyPath = "MPfm.Fonts.dll";
            customFont1.Name = "Junction";
            customFont1.ResourceName = "MPfm.Fonts.Junction.ttf";
            customFont2.AssemblyPath = "MPfm.Fonts.dll";
            customFont2.Name = "LeagueGothic";
            customFont2.ResourceName = "MPfm.Fonts.LeagueGothic.ttf";
            customFont3.AssemblyPath = "MPfm.Fonts.dll";
            customFont3.Name = "TitilliumText22L Lt";
            customFont3.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            customFont4.AssemblyPath = "MPfm.Fonts.dll";
            customFont4.Name = "BPmono";
            customFont4.ResourceName = "MPfm.Fonts.BPmono.ttf";
            customFont5.AssemblyPath = "MPfm.Fonts.dll";
            customFont5.Name = "CPmono";
            customFont5.ResourceName = "MPfm.Fonts.CPmono.ttf";
            this.fontCollection.Fonts.Add(customFont1);
            this.fontCollection.Fonts.Add(customFont2);
            this.fontCollection.Fonts.Add(customFont3);
            this.fontCollection.Fonts.Add(customFont4);
            this.fontCollection.Fonts.Add(customFont5);
            // 
            // label5
            // 
            this.label5.AntiAliasingEnabled = true;
            this.label5.CustomFontName = "Junction";
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.FontCollection = this.fontCollection;
            this.label5.Location = new System.Drawing.Point(6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 18);
            this.label5.TabIndex = 17;
            this.label5.Text = "Global Settings";
            // 
            // label4
            // 
            this.label4.AntiAliasingEnabled = true;
            this.label4.CustomFontName = "Junction";
            this.label4.FontCollection = this.fontCollection;
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 18);
            this.label4.TabIndex = 16;
            this.label4.Text = "Control to display :";
            // 
            // lblUseCustomFont
            // 
            this.lblUseCustomFont.AntiAliasingEnabled = true;
            this.lblUseCustomFont.CustomFontName = "Junction";
            this.lblUseCustomFont.FontCollection = this.fontCollection;
            this.lblUseCustomFont.Location = new System.Drawing.Point(174, 19);
            this.lblUseCustomFont.Name = "lblUseCustomFont";
            this.lblUseCustomFont.Size = new System.Drawing.Size(100, 18);
            this.lblUseCustomFont.TabIndex = 12;
            this.lblUseCustomFont.Text = "Use Custom Font";
            this.lblUseCustomFont.Click += new System.EventHandler(this.lblUseCustomFont_Click);
            // 
            // lblFontSize
            // 
            this.lblFontSize.AntiAliasingEnabled = true;
            this.lblFontSize.CustomFontName = "Junction";
            this.lblFontSize.FontCollection = this.fontCollection;
            this.lblFontSize.Location = new System.Drawing.Point(418, 19);
            this.lblFontSize.Name = "lblFontSize";
            this.lblFontSize.Size = new System.Drawing.Size(100, 18);
            this.lblFontSize.TabIndex = 4;
            this.lblFontSize.Text = "Font Size : 8 pt";
            // 
            // trackFontSize
            // 
            this.trackFontSize.CenterLineColor = System.Drawing.Color.Black;
            this.trackFontSize.CenterLineShadowColor = System.Drawing.Color.DarkGray;
            this.trackFontSize.CustomFontName = null;
            this.trackFontSize.FaderGradientColor1 = System.Drawing.Color.DimGray;
            this.trackFontSize.FaderGradientColor2 = System.Drawing.SystemColors.ControlDark;
            this.trackFontSize.FaderHeight = 12;
            this.trackFontSize.FaderShadowGradientColor1 = System.Drawing.SystemColors.ControlDark;
            this.trackFontSize.FaderShadowGradientColor2 = System.Drawing.SystemColors.ControlDarkDark;
            this.trackFontSize.FontCollection = null;
            this.trackFontSize.GradientColor1 = System.Drawing.SystemColors.Control;
            this.trackFontSize.GradientColor2 = System.Drawing.SystemColors.ControlDark;
            this.trackFontSize.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.trackFontSize.Location = new System.Drawing.Point(421, 40);
            this.trackFontSize.Maximum = 24;
            this.trackFontSize.Minimum = 6;
            this.trackFontSize.Name = "trackFontSize";
            this.trackFontSize.Size = new System.Drawing.Size(146, 21);
            this.trackFontSize.TabIndex = 3;
            this.trackFontSize.Text = "trackBar";
            this.trackFontSize.Value = 8;
            this.trackFontSize.OnTrackBarValueChanged += new MPfm.WindowsControls.TrackBar.TrackBarValueChanged(this.trackFontSize_OnTrackBarValueChanged);
            // 
            // lblPadding
            // 
            this.lblPadding.AntiAliasingEnabled = true;
            this.lblPadding.CustomFontName = "Junction";
            this.lblPadding.FontCollection = this.fontCollection;
            this.lblPadding.Location = new System.Drawing.Point(4, 19);
            this.lblPadding.Name = "lblPadding";
            this.lblPadding.Size = new System.Drawing.Size(100, 18);
            this.lblPadding.TabIndex = 17;
            this.lblPadding.Text = "Padding  : 6 pt";
            // 
            // trackPadding
            // 
            this.trackPadding.CenterLineColor = System.Drawing.Color.Black;
            this.trackPadding.CenterLineShadowColor = System.Drawing.Color.DarkGray;
            this.trackPadding.CustomFontName = null;
            this.trackPadding.FaderGradientColor1 = System.Drawing.SystemColors.ControlDark;
            this.trackPadding.FaderGradientColor2 = System.Drawing.SystemColors.ControlDark;
            this.trackPadding.FaderHeight = 12;
            this.trackPadding.FaderShadowGradientColor1 = System.Drawing.SystemColors.ControlDark;
            this.trackPadding.FaderShadowGradientColor2 = System.Drawing.SystemColors.ControlDarkDark;
            this.trackPadding.FontCollection = null;
            this.trackPadding.GradientColor1 = System.Drawing.SystemColors.Control;
            this.trackPadding.GradientColor2 = System.Drawing.SystemColors.ControlDark;
            this.trackPadding.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.trackPadding.Location = new System.Drawing.Point(7, 40);
            this.trackPadding.Maximum = 24;
            this.trackPadding.Name = "trackPadding";
            this.trackPadding.Size = new System.Drawing.Size(167, 21);
            this.trackPadding.TabIndex = 16;
            this.trackPadding.Text = "trackBar1";
            this.trackPadding.Value = 6;
            this.trackPadding.OnTrackBarValueChanged += new MPfm.WindowsControls.TrackBar.TrackBarValueChanged(this.trackPadding_OnTrackBarValueChanged);
            // 
            // label1
            // 
            this.label1.AntiAliasingEnabled = true;
            this.label1.CustomFontName = "Junction";
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.FontCollection = this.fontCollection;
            this.label1.Location = new System.Drawing.Point(183, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 18);
            this.label1.TabIndex = 15;
            this.label1.Text = "Search";
            // 
            // label3
            // 
            this.label3.AntiAliasingEnabled = true;
            this.label3.CustomFontName = "Junction";
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.FontCollection = this.fontCollection;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "SongGridView Settings";
            // 
            // label2
            // 
            this.label2.AntiAliasingEnabled = true;
            this.label2.CustomFontName = "Junction";
            this.label2.FontCollection = this.fontCollection;
            this.label2.Location = new System.Drawing.Point(183, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 18);
            this.label2.TabIndex = 6;
            this.label2.Text = "Artist Name :";
            // 
            // lblDisplayDebugInformation
            // 
            this.lblDisplayDebugInformation.AntiAliasingEnabled = true;
            this.lblDisplayDebugInformation.CustomFontName = "Junction";
            this.lblDisplayDebugInformation.FontCollection = this.fontCollection;
            this.lblDisplayDebugInformation.Location = new System.Drawing.Point(22, 70);
            this.lblDisplayDebugInformation.Name = "lblDisplayDebugInformation";
            this.lblDisplayDebugInformation.Size = new System.Drawing.Size(153, 18);
            this.lblDisplayDebugInformation.TabIndex = 5;
            this.lblDisplayDebugInformation.Text = "Display debug information";
            this.lblDisplayDebugInformation.Click += new System.EventHandler(this.lblDisplayDebugInformation_Click);
            // 
            // songGridView
            // 
            this.songGridView.AlbumCoverBackgroundColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.songGridView.AlbumCoverBackgroundColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.songGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.songGridView.AntiAliasingEnabled = true;
            this.songGridView.ContextMenuStrip = this.contextMenuStrip1;
            this.songGridView.CustomFontName = "Junction";
            this.songGridView.DisplayDebugInformation = false;
            this.songGridView.FontCollection = this.fontCollection;
            this.songGridView.HeaderColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(165)))), ((int)(((byte)(165)))));
            this.songGridView.HeaderColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(195)))), ((int)(((byte)(195)))));
            this.songGridView.HeaderForeColor = System.Drawing.Color.Black;
            this.songGridView.HeaderHoverColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(145)))), ((int)(((byte)(145)))));
            this.songGridView.HeaderHoverColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.songGridView.IconNowPlayingColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(200)))), ((int)(((byte)(250)))));
            this.songGridView.IconNowPlayingColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(150)))), ((int)(((byte)(25)))));
            this.songGridView.ImageCacheSize = 10;            
            this.songGridView.LineColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(215)))), ((int)(((byte)(215)))));
            this.songGridView.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.songGridView.LineForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.songGridView.LineHoverColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.songGridView.LineHoverColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.songGridView.LineNowPlayingColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(235)))), ((int)(((byte)(135)))));
            this.songGridView.LineNowPlayingColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(255)))), ((int)(((byte)(155)))));
            this.songGridView.LineSelectedColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(165)))), ((int)(((byte)(165)))));
            this.songGridView.LineSelectedColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.songGridView.Location = new System.Drawing.Point(0, 173);
            this.songGridView.Name = "songGridView";            
            this.songGridView.OrderByAscending = true;
            this.songGridView.OrderByFieldName = "";            
            this.songGridView.Size = new System.Drawing.Size(936, 300);
            this.songGridView.TabIndex = 2;
            this.songGridView.Text = "libraryGridView1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 92);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem3.Text = "toolStripMenuItem3";
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(938, 473);
            this.Controls.Add(this.groupGeneral);
            this.Controls.Add(this.groupSongGridView);
            this.Controls.Add(this.songGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "MPfm - Test Controls";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupSongGridView.ResumeLayout(false);
            this.groupSongGridView.PerformLayout();
            this.groupGeneral.ResumeLayout(false);
            this.groupGeneral.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MPfm.WindowsControls.SongGridView songGridView;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private MPfm.WindowsControls.TrackBar trackFontSize;
        private MPfm.WindowsControls.Label lblFontSize;
        private System.Windows.Forms.GroupBox groupSongGridView;
        private System.Windows.Forms.CheckBox chkDebug;
        private MPfm.WindowsControls.Label lblDisplayDebugInformation;
        private System.Windows.Forms.GroupBox groupGeneral;
        private System.Windows.Forms.TextBox txtSearchArtistName;
        private MPfm.WindowsControls.Label label2;
        private System.Windows.Forms.ComboBox comboDisplayType;
        private System.Windows.Forms.ComboBox comboStandardFontName;
        private MPfm.WindowsControls.Label lblUseCustomFont;
        private System.Windows.Forms.ComboBox comboCustomFontName;
        private MPfm.WindowsControls.Label label3;
        private MPfm.WindowsControls.Label label5;
        private MPfm.WindowsControls.Label label4;
        private MPfm.WindowsControls.Label label1;
        private System.Windows.Forms.RadioButton radioUseStandardFont;
        private System.Windows.Forms.RadioButton radioUseCustomFont;
        private MPfm.WindowsControls.Label lblUseStandardFont;
        private MPfm.WindowsControls.Label lblPadding;
        private MPfm.WindowsControls.TrackBar trackPadding;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
    }
}

