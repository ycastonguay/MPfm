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
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabAudioSettings = new System.Windows.Forms.TabPage();
            this.Panel1 = new MPfm.WindowsControls.Panel();
            this.button2 = new MPfm.WindowsControls.Button();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.lblOutputDriver = new MPfm.WindowsControls.Label();
            this.chkShowTray = new System.Windows.Forms.CheckBox();
            this.chkHideTray = new System.Windows.Forms.CheckBox();
            this.btnTestSound = new MPfm.WindowsControls.Button();
            this.cboOutputDevices = new System.Windows.Forms.ComboBox();
            this.Label1 = new MPfm.WindowsControls.Label();
            this.cboDrivers = new System.Windows.Forms.ComboBox();
            this.tabLibrary = new System.Windows.Forms.TabPage();
            this.Panel2 = new MPfm.WindowsControls.Panel();
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
            this.tabs.SuspendLayout();
            this.tabAudioSettings.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.tabLibrary.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabAudioSettings);
            this.tabs.Controls.Add(this.tabLibrary);
            this.tabs.Location = new System.Drawing.Point(0, 2);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(508, 309);
            this.tabs.TabIndex = 6;
            // 
            // tabAudioSettings
            // 
            this.tabAudioSettings.Controls.Add(this.Panel1);
            this.tabAudioSettings.Location = new System.Drawing.Point(4, 24);
            this.tabAudioSettings.Name = "tabAudioSettings";
            this.tabAudioSettings.Size = new System.Drawing.Size(500, 281);
            this.tabAudioSettings.TabIndex = 2;
            this.tabAudioSettings.Text = "Audio Settings";
            this.tabAudioSettings.UseVisualStyleBackColor = true;
            // 
            // Panel1
            // 
            this.Panel1.AntiAliasingEnabled = true;
            this.Panel1.Controls.Add(this.button2);
            this.Panel1.Controls.Add(this.lblOutputDriver);
            this.Panel1.Controls.Add(this.chkShowTray);
            this.Panel1.Controls.Add(this.chkHideTray);
            this.Panel1.Controls.Add(this.btnTestSound);
            this.Panel1.Controls.Add(this.cboOutputDevices);
            this.Panel1.Controls.Add(this.Label1);
            this.Panel1.Controls.Add(this.cboDrivers);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel1.ExpandedHeight = 200;
            this.Panel1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Panel1.FontCollection = this.fontCollection;
            this.Panel1.GradientColor1 = System.Drawing.Color.Silver;
            this.Panel1.GradientColor2 = System.Drawing.Color.Gray;
            this.Panel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.Panel1.HeaderCustomFontName = "TitilliumText22L Lt";
            this.Panel1.HeaderExpandable = false;
            this.Panel1.HeaderExpanded = true;
            this.Panel1.HeaderForeColor = System.Drawing.Color.Black;
            this.Panel1.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.Panel1.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.Panel1.HeaderHeight = 30;
            this.Panel1.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Panel1.HeaderTitle = "Audio Settings";
            this.Panel1.Location = new System.Drawing.Point(0, 0);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(500, 281);
            this.Panel1.TabIndex = 16;
            // 
            // button2
            // 
            this.button2.AntiAliasingEnabled = true;
            this.button2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.button2.BorderWidth = 1;
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.CustomFontName = "Junction";
            this.button2.DisabledBorderColor = System.Drawing.Color.Gray;
            this.button2.DisabledFontColor = System.Drawing.Color.Silver;
            this.button2.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.button2.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.FontCollection = this.fontCollection;
            this.button2.FontColor = System.Drawing.Color.Black;
            this.button2.GradientColor1 = System.Drawing.Color.LightGray;
            this.button2.GradientColor2 = System.Drawing.Color.Gray;
            this.button2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.button2.Image = global::MPfm.Properties.Resources.sound;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button2.Location = new System.Drawing.Point(286, 180);
            this.button2.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.button2.MouseOverFontColor = System.Drawing.Color.Black;
            this.button2.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.button2.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(131, 40);
            this.button2.TabIndex = 82;
            this.button2.Text = "Stop";
            this.button2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
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
            // lblOutputDriver
            // 
            this.lblOutputDriver.AntiAliasingEnabled = true;
            this.lblOutputDriver.BackColor = System.Drawing.Color.Transparent;
            this.lblOutputDriver.CustomFontName = "Junction";
            this.lblOutputDriver.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutputDriver.FontCollection = this.fontCollection;
            this.lblOutputDriver.Location = new System.Drawing.Point(3, 36);
            this.lblOutputDriver.Name = "lblOutputDriver";
            this.lblOutputDriver.Size = new System.Drawing.Size(89, 17);
            this.lblOutputDriver.TabIndex = 11;
            this.lblOutputDriver.Text = "Output device:";
            // 
            // chkShowTray
            // 
            this.chkShowTray.AutoSize = true;
            this.chkShowTray.BackColor = System.Drawing.Color.Transparent;
            this.chkShowTray.Checked = true;
            this.chkShowTray.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowTray.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowTray.Location = new System.Drawing.Point(5, 180);
            this.chkShowTray.Name = "chkShowTray";
            this.chkShowTray.Size = new System.Drawing.Size(183, 19);
            this.chkShowTray.TabIndex = 10;
            this.chkShowTray.Text = "Show PMP in the system tray";
            this.chkShowTray.UseVisualStyleBackColor = false;
            this.chkShowTray.Visible = false;
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
            this.chkHideTray.Location = new System.Drawing.Point(5, 201);
            this.chkHideTray.Name = "chkHideTray";
            this.chkHideTray.Size = new System.Drawing.Size(251, 19);
            this.chkHideTray.TabIndex = 6;
            this.chkHideTray.Text = "Hide PMP in the system tray when closed";
            this.chkHideTray.UseVisualStyleBackColor = false;
            this.chkHideTray.Visible = false;
            this.chkHideTray.CheckedChanged += new System.EventHandler(this.chkHideTray_CheckedChanged);
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
            this.btnTestSound.Location = new System.Drawing.Point(6, 133);
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
            this.cboOutputDevices.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboOutputDevices.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboOutputDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOutputDevices.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboOutputDevices.FormattingEnabled = true;
            this.cboOutputDevices.Location = new System.Drawing.Point(6, 57);
            this.cboOutputDevices.Name = "cboOutputDevices";
            this.cboOutputDevices.Size = new System.Drawing.Size(480, 23);
            this.cboOutputDevices.TabIndex = 9;
            this.cboOutputDevices.SelectedIndexChanged += new System.EventHandler(this.cboDriverOrOutputType_SelectedIndexChanged);
            // 
            // Label1
            // 
            this.Label1.AntiAliasingEnabled = true;
            this.Label1.BackColor = System.Drawing.Color.Transparent;
            this.Label1.CustomFontName = "Junction";
            this.Label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.FontCollection = this.fontCollection;
            this.Label1.Location = new System.Drawing.Point(3, 84);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(89, 17);
            this.Label1.TabIndex = 13;
            this.Label1.Text = "Driver:";
            // 
            // cboDrivers
            // 
            this.cboDrivers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboDrivers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDrivers.DisplayMember = "Description";
            this.cboDrivers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDrivers.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDrivers.FormattingEnabled = true;
            this.cboDrivers.Location = new System.Drawing.Point(6, 104);
            this.cboDrivers.Name = "cboDrivers";
            this.cboDrivers.Size = new System.Drawing.Size(480, 23);
            this.cboDrivers.TabIndex = 12;
            this.cboDrivers.ValueMember = "FMODOutputType";
            this.cboDrivers.SelectedIndexChanged += new System.EventHandler(this.cboDriverOrOutputType_SelectedIndexChanged);
            // 
            // tabLibrary
            // 
            this.tabLibrary.Controls.Add(this.Panel2);
            this.tabLibrary.Location = new System.Drawing.Point(4, 24);
            this.tabLibrary.Name = "tabLibrary";
            this.tabLibrary.Size = new System.Drawing.Size(500, 281);
            this.tabLibrary.TabIndex = 1;
            this.tabLibrary.Text = "Library";
            this.tabLibrary.UseVisualStyleBackColor = true;
            // 
            // Panel2
            // 
            this.Panel2.AntiAliasingEnabled = true;
            this.Panel2.Controls.Add(this.btnRemoveFolder);
            this.Panel2.Controls.Add(this.viewFolders);
            this.Panel2.Controls.Add(this.btnAddFolder);
            this.Panel2.Controls.Add(this.Button1);
            this.Panel2.Controls.Add(this.lblFoldersTitle);
            this.Panel2.Controls.Add(this.btnResetLibrary);
            this.Panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel2.ExpandedHeight = 200;
            this.Panel2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Panel2.FontCollection = this.fontCollection;
            this.Panel2.GradientColor1 = System.Drawing.Color.Silver;
            this.Panel2.GradientColor2 = System.Drawing.Color.Gray;
            this.Panel2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.Panel2.HeaderCustomFontName = "TitilliumText22L Lt";
            this.Panel2.HeaderExpandable = false;
            this.Panel2.HeaderExpanded = true;
            this.Panel2.HeaderForeColor = System.Drawing.Color.Black;
            this.Panel2.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.Panel2.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.Panel2.HeaderHeight = 30;
            this.Panel2.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Panel2.HeaderTitle = "Library";
            this.Panel2.Location = new System.Drawing.Point(0, 0);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(500, 281);
            this.Panel2.TabIndex = 17;
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
            this.lblFoldersTitle.CustomFontName = "Junction";
            this.lblFoldersTitle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFoldersTitle.FontCollection = this.fontCollection;
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
            this.btnClose.AntiAliasingEnabled = true;
            this.btnClose.BorderColor = System.Drawing.Color.Black;
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
            this.btnClose.Location = new System.Drawing.Point(416, 317);
            this.btnClose.MouseOverBorderColor = System.Drawing.Color.Black;
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
            // frmSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(512, 364);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tabs);
            this.CustomFontName = "NeuzeitS";
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontCollection = this.fontCollection;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.GradientColor1 = System.Drawing.Color.Silver;
            this.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSettings_FormClosing);
            this.Shown += new System.EventHandler(this.frmSettings_Shown);
            this.tabs.ResumeLayout(false);
            this.tabAudioSettings.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.tabLibrary.ResumeLayout(false);
            this.Panel2.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox chkHideTray;
        private MPfm.WindowsControls.Button btnResetLibrary;
        private MPfm.WindowsControls.Button btnClose;
        private MPfm.WindowsControls.Button Button1;
        private System.Windows.Forms.CheckBox chkShowTray;
        private MPfm.WindowsControls.Label lblOutputDriver;
        private MPfm.WindowsControls.Label Label1;
        private System.Windows.Forms.ComboBox cboDrivers;
        private MPfm.WindowsControls.Button btnTestSound;
        private System.Windows.Forms.OpenFileDialog dialogOpenFile;
        private MPfm.WindowsControls.Panel Panel1;
        private MPfm.WindowsControls.Panel Panel2;
        private MPfm.WindowsControls.Button btnRemoveFolder;
        private MPfm.WindowsControls.Button btnAddFolder;
        private WindowsControls.Button button2;
    }
}