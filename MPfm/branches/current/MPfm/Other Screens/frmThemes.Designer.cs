namespace MPfm
{
    partial class frmThemes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmThemes));
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont13 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont12 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont11 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.SongGridViewTheme songGridViewTheme1 = new MPfm.WindowsControls.SongGridViewTheme();
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.OutputMeterTheme outputMeterTheme1 = new MPfm.WindowsControls.OutputMeterTheme();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont14 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont15 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont16 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont17 = new MPfm.WindowsControls.CustomFont();
            this.dialogAddFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.dialogOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.dialogOpenTheme = new System.Windows.Forms.OpenFileDialog();
            this.dialogSaveTheme = new System.Windows.Forms.SaveFileDialog();
            this.dialogBrowsePeakFileDirectory = new System.Windows.Forms.FolderBrowserDialog();
            this.btnSaveTheme = new MPfm.WindowsControls.Button();
            this.btnApplyTheme = new MPfm.WindowsControls.Button();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.btnNewTheme = new MPfm.WindowsControls.Button();
            this.lblCurrentThemeTitle = new MPfm.WindowsControls.Label();
            this.panelThemeProperties = new MPfm.WindowsControls.Panel();
            this.lblThemeAuthor = new MPfm.WindowsControls.Label();
            this.txtThemeAuthor = new System.Windows.Forms.TextBox();
            this.txtThemeName = new System.Windows.Forms.TextBox();
            this.lblThemeName = new MPfm.WindowsControls.Label();
            this.splitTheme = new System.Windows.Forms.SplitContainer();
            this.propertyGridTheme = new System.Windows.Forms.PropertyGrid();
            this.lblProperties = new MPfm.WindowsControls.Label();
            this.lblPreview = new MPfm.WindowsControls.Label();
            this.previewSongGridView = new MPfm.WindowsControls.SongGridView();
            this.previewOutputMeter = new MPfm.WindowsControls.OutputMeter();
            this.lblPreviewPane = new MPfm.WindowsControls.Label();
            this.comboPreviewPane = new System.Windows.Forms.ComboBox();
            this.btnSaveThemeAs = new MPfm.WindowsControls.Button();
            this.lblThemeTitle = new MPfm.WindowsControls.Label();
            this.lblCurrentTheme = new MPfm.WindowsControls.Label();
            this.cboTheme = new System.Windows.Forms.ComboBox();
            this.panelBackground = new MPfm.WindowsControls.Panel();
            this.panelThemeProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTheme)).BeginInit();
            this.splitTheme.Panel1.SuspendLayout();
            this.splitTheme.Panel2.SuspendLayout();
            this.splitTheme.SuspendLayout();
            this.panelBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // dialogAddFolder
            // 
            this.dialogAddFolder.Description = "Please select a folder to add to the music library.";
            this.dialogAddFolder.ShowNewFolderButton = false;
            // 
            // dialogOpenFile
            // 
            this.dialogOpenFile.Filter = "Audio files (*.mp3,*.flac,*.ogg, *.wav, *.ape, *.wv, *.mpc)|*.mp3;*.flac;*.ogg,*." +
    "wav;*.wv;*.ape;*.mpc";
            this.dialogOpenFile.Title = "Please select an audio file to play";
            // 
            // dialogOpenTheme
            // 
            this.dialogOpenTheme.Filter = "Theme files (*.mpfmTheme)|*.mpfmTheme";
            this.dialogOpenTheme.Title = "Please select a theme to load";
            // 
            // dialogSaveTheme
            // 
            this.dialogSaveTheme.Filter = "Theme files (*.mpfmTheme)|*.mpfmTheme";
            this.dialogSaveTheme.Title = "Please select a file name for the theme";
            // 
            // dialogBrowsePeakFileDirectory
            // 
            this.dialogBrowsePeakFileDirectory.Description = "Please select a folder for peak files.";
            // 
            // btnSaveTheme
            // 
            this.btnSaveTheme.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.btnSaveTheme.BorderWidth = 1;
            this.btnSaveTheme.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont1.EmbeddedFontName = "Junction";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 8F;
            customFont1.StandardFontName = "Arial";
            customFont1.UseAntiAliasing = true;
            customFont1.UseEmbeddedFont = true;
            this.btnSaveTheme.CustomFont = customFont1;
            this.btnSaveTheme.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnSaveTheme.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnSaveTheme.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnSaveTheme.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSaveTheme.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveTheme.FontColor = System.Drawing.Color.Black;
            this.btnSaveTheme.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnSaveTheme.GradientColor2 = System.Drawing.Color.Gray;
            this.btnSaveTheme.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnSaveTheme.Image = global::MPfm.Properties.Resources.disk;
            this.btnSaveTheme.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveTheme.Location = new System.Drawing.Point(88, 31);
            this.btnSaveTheme.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnSaveTheme.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnSaveTheme.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnSaveTheme.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnSaveTheme.Name = "btnSaveTheme";
            this.btnSaveTheme.Size = new System.Drawing.Size(93, 24);
            this.btnSaveTheme.TabIndex = 102;
            this.btnSaveTheme.Text = "Save theme";
            this.btnSaveTheme.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSaveTheme.UseVisualStyleBackColor = true;
            this.btnSaveTheme.Click += new System.EventHandler(this.btnSaveTheme_Click);
            // 
            // btnApplyTheme
            // 
            this.btnApplyTheme.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.btnApplyTheme.BorderWidth = 1;
            this.btnApplyTheme.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont2.EmbeddedFontName = "Junction";
            customFont2.IsBold = false;
            customFont2.IsItalic = false;
            customFont2.IsUnderline = false;
            customFont2.Size = 8F;
            customFont2.StandardFontName = "Arial";
            customFont2.UseAntiAliasing = true;
            customFont2.UseEmbeddedFont = true;
            this.btnApplyTheme.CustomFont = customFont2;
            this.btnApplyTheme.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnApplyTheme.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnApplyTheme.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnApplyTheme.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnApplyTheme.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplyTheme.FontColor = System.Drawing.Color.Black;
            this.btnApplyTheme.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnApplyTheme.GradientColor2 = System.Drawing.Color.Gray;
            this.btnApplyTheme.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnApplyTheme.Image = global::MPfm.Properties.Resources.accept;
            this.btnApplyTheme.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnApplyTheme.Location = new System.Drawing.Point(286, 31);
            this.btnApplyTheme.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnApplyTheme.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnApplyTheme.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnApplyTheme.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnApplyTheme.Name = "btnApplyTheme";
            this.btnApplyTheme.Size = new System.Drawing.Size(96, 24);
            this.btnApplyTheme.TabIndex = 109;
            this.btnApplyTheme.Text = "Apply theme";
            this.btnApplyTheme.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnApplyTheme.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(100)))));
            this.btnClose.BorderWidth = 1;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont3.EmbeddedFontName = "Junction";
            customFont3.IsBold = false;
            customFont3.IsItalic = false;
            customFont3.IsUnderline = false;
            customFont3.Size = 8F;
            customFont3.StandardFontName = "Arial";
            customFont3.UseAntiAliasing = true;
            customFont3.UseEmbeddedFont = true;
            this.btnClose.CustomFont = customFont3;
            this.btnClose.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.FontColor = System.Drawing.Color.Black;
            this.btnClose.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnClose.GradientColor2 = System.Drawing.Color.Gray;
            this.btnClose.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnClose.Image = global::MPfm.Properties.Resources.cancel;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(381, 31);
            this.btnClose.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnClose.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnClose.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnClose.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(61, 24);
            this.btnClose.TabIndex = 62;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNewTheme
            // 
            this.btnNewTheme.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.btnNewTheme.BorderWidth = 1;
            this.btnNewTheme.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont4.EmbeddedFontName = "Junction";
            customFont4.IsBold = false;
            customFont4.IsItalic = false;
            customFont4.IsUnderline = false;
            customFont4.Size = 8F;
            customFont4.StandardFontName = "Arial";
            customFont4.UseAntiAliasing = true;
            customFont4.UseEmbeddedFont = true;
            this.btnNewTheme.CustomFont = customFont4;
            this.btnNewTheme.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnNewTheme.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnNewTheme.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnNewTheme.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnNewTheme.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewTheme.FontColor = System.Drawing.Color.Black;
            this.btnNewTheme.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnNewTheme.GradientColor2 = System.Drawing.Color.Gray;
            this.btnNewTheme.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnNewTheme.Image = ((System.Drawing.Image)(resources.GetObject("btnNewTheme.Image")));
            this.btnNewTheme.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNewTheme.Location = new System.Drawing.Point(0, 31);
            this.btnNewTheme.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnNewTheme.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnNewTheme.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnNewTheme.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnNewTheme.Name = "btnNewTheme";
            this.btnNewTheme.Size = new System.Drawing.Size(89, 24);
            this.btnNewTheme.TabIndex = 63;
            this.btnNewTheme.Text = "New theme";
            this.btnNewTheme.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNewTheme.UseVisualStyleBackColor = true;
            this.btnNewTheme.Click += new System.EventHandler(this.btnNewTheme_Click);
            // 
            // lblCurrentThemeTitle
            // 
            this.lblCurrentThemeTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentThemeTitle.BackgroundGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblCurrentThemeTitle.BackgroundGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblCurrentThemeTitle.BackgroundGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            customFont5.EmbeddedFontName = "Junction";
            customFont5.IsBold = false;
            customFont5.IsItalic = false;
            customFont5.IsUnderline = false;
            customFont5.Size = 8F;
            customFont5.StandardFontName = "Arial";
            customFont5.UseAntiAliasing = true;
            customFont5.UseEmbeddedFont = true;
            this.lblCurrentThemeTitle.CustomFont = customFont5;
            this.lblCurrentThemeTitle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentThemeTitle.Location = new System.Drawing.Point(4, 59);
            this.lblCurrentThemeTitle.Name = "lblCurrentThemeTitle";
            this.lblCurrentThemeTitle.Size = new System.Drawing.Size(97, 18);
            this.lblCurrentThemeTitle.TabIndex = 111;
            this.lblCurrentThemeTitle.Text = "Current theme:";
            this.lblCurrentThemeTitle.UseBackgroundGradient = false;
            // 
            // panelThemeProperties
            // 
            this.panelThemeProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelThemeProperties.Controls.Add(this.comboPreviewPane);
            this.panelThemeProperties.Controls.Add(this.lblPreviewPane);
            this.panelThemeProperties.Controls.Add(this.splitTheme);
            this.panelThemeProperties.Controls.Add(this.lblThemeName);
            this.panelThemeProperties.Controls.Add(this.txtThemeName);
            this.panelThemeProperties.Controls.Add(this.txtThemeAuthor);
            this.panelThemeProperties.Controls.Add(this.lblThemeAuthor);
            customFont13.EmbeddedFontName = "TitilliumText22L Lt";
            customFont13.IsBold = true;
            customFont13.IsItalic = false;
            customFont13.IsUnderline = false;
            customFont13.Size = 10F;
            customFont13.StandardFontName = "Arial";
            customFont13.UseAntiAliasing = true;
            customFont13.UseEmbeddedFont = true;
            this.panelThemeProperties.CustomFont = customFont13;
            this.panelThemeProperties.ExpandedHeight = 200;
            this.panelThemeProperties.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelThemeProperties.GradientColor1 = System.Drawing.Color.Silver;
            this.panelThemeProperties.GradientColor2 = System.Drawing.Color.Gray;
            this.panelThemeProperties.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelThemeProperties.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelThemeProperties.HeaderExpandable = false;
            this.panelThemeProperties.HeaderExpanded = true;
            this.panelThemeProperties.HeaderForeColor = System.Drawing.Color.Black;
            this.panelThemeProperties.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelThemeProperties.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelThemeProperties.HeaderHeight = 20;
            this.panelThemeProperties.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelThemeProperties.HeaderTitle = "Theme Properties";
            this.panelThemeProperties.Location = new System.Drawing.Point(8, 111);
            this.panelThemeProperties.Name = "panelThemeProperties";
            this.panelThemeProperties.Size = new System.Drawing.Size(622, 309);
            this.panelThemeProperties.TabIndex = 110;
            // 
            // lblThemeAuthor
            // 
            this.lblThemeAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblThemeAuthor.BackColor = System.Drawing.Color.Transparent;
            this.lblThemeAuthor.BackgroundGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblThemeAuthor.BackgroundGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblThemeAuthor.BackgroundGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            customFont12.EmbeddedFontName = "Junction";
            customFont12.IsBold = false;
            customFont12.IsItalic = false;
            customFont12.IsUnderline = false;
            customFont12.Size = 8F;
            customFont12.StandardFontName = "Arial";
            customFont12.UseAntiAliasing = true;
            customFont12.UseEmbeddedFont = true;
            this.lblThemeAuthor.CustomFont = customFont12;
            this.lblThemeAuthor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThemeAuthor.ForeColor = System.Drawing.Color.Black;
            this.lblThemeAuthor.Location = new System.Drawing.Point(373, 29);
            this.lblThemeAuthor.Name = "lblThemeAuthor";
            this.lblThemeAuthor.Size = new System.Drawing.Size(46, 14);
            this.lblThemeAuthor.TabIndex = 106;
            this.lblThemeAuthor.Text = "Author:";
            this.lblThemeAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblThemeAuthor.UseBackgroundGradient = false;
            // 
            // txtThemeAuthor
            // 
            this.txtThemeAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtThemeAuthor.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtThemeAuthor.Location = new System.Drawing.Point(426, 26);
            this.txtThemeAuthor.Name = "txtThemeAuthor";
            this.txtThemeAuthor.Size = new System.Drawing.Size(184, 22);
            this.txtThemeAuthor.TabIndex = 107;
            // 
            // txtThemeName
            // 
            this.txtThemeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtThemeName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtThemeName.Location = new System.Drawing.Point(53, 26);
            this.txtThemeName.Name = "txtThemeName";
            this.txtThemeName.Size = new System.Drawing.Size(314, 22);
            this.txtThemeName.TabIndex = 105;
            // 
            // lblThemeName
            // 
            this.lblThemeName.BackColor = System.Drawing.Color.Transparent;
            this.lblThemeName.BackgroundGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblThemeName.BackgroundGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblThemeName.BackgroundGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            customFont11.EmbeddedFontName = "Junction";
            customFont11.IsBold = false;
            customFont11.IsItalic = false;
            customFont11.IsUnderline = false;
            customFont11.Size = 8F;
            customFont11.StandardFontName = "Arial";
            customFont11.UseAntiAliasing = true;
            customFont11.UseEmbeddedFont = true;
            this.lblThemeName.CustomFont = customFont11;
            this.lblThemeName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThemeName.ForeColor = System.Drawing.Color.Black;
            this.lblThemeName.Location = new System.Drawing.Point(6, 29);
            this.lblThemeName.Name = "lblThemeName";
            this.lblThemeName.Size = new System.Drawing.Size(46, 14);
            this.lblThemeName.TabIndex = 104;
            this.lblThemeName.Text = "Name:";
            this.lblThemeName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblThemeName.UseBackgroundGradient = false;
            // 
            // splitTheme
            // 
            this.splitTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitTheme.Location = new System.Drawing.Point(7, 83);
            this.splitTheme.Name = "splitTheme";
            // 
            // splitTheme.Panel1
            // 
            this.splitTheme.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.splitTheme.Panel1.Controls.Add(this.previewOutputMeter);
            this.splitTheme.Panel1.Controls.Add(this.previewSongGridView);
            this.splitTheme.Panel1.Controls.Add(this.lblPreview);
            // 
            // splitTheme.Panel2
            // 
            this.splitTheme.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.splitTheme.Panel2.Controls.Add(this.lblProperties);
            this.splitTheme.Panel2.Controls.Add(this.propertyGridTheme);
            this.splitTheme.Size = new System.Drawing.Size(603, 226);
            this.splitTheme.SplitterDistance = 262;
            this.splitTheme.TabIndex = 101;
            // 
            // propertyGridTheme
            // 
            this.propertyGridTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridTheme.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGridTheme.Location = new System.Drawing.Point(0, 19);
            this.propertyGridTheme.Name = "propertyGridTheme";
            this.propertyGridTheme.Size = new System.Drawing.Size(337, 204);
            this.propertyGridTheme.TabIndex = 97;
            this.propertyGridTheme.ToolbarVisible = false;
            this.propertyGridTheme.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridTheme_PropertyValueChanged);
            // 
            // lblProperties
            // 
            this.lblProperties.BackColor = System.Drawing.Color.Transparent;
            this.lblProperties.BackgroundGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblProperties.BackgroundGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblProperties.BackgroundGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            customFont10.EmbeddedFontName = "TitilliumText22L Lt";
            customFont10.IsBold = true;
            customFont10.IsItalic = false;
            customFont10.IsUnderline = false;
            customFont10.Size = 8F;
            customFont10.StandardFontName = "Arial";
            customFont10.UseAntiAliasing = true;
            customFont10.UseEmbeddedFont = true;
            this.lblProperties.CustomFont = customFont10;
            this.lblProperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProperties.ForeColor = System.Drawing.Color.White;
            this.lblProperties.Location = new System.Drawing.Point(0, 2);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(94, 14);
            this.lblProperties.TabIndex = 109;
            this.lblProperties.Text = "Properties";
            this.lblProperties.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblProperties.UseBackgroundGradient = false;
            // 
            // lblPreview
            // 
            this.lblPreview.BackColor = System.Drawing.Color.Transparent;
            this.lblPreview.BackgroundGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblPreview.BackgroundGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblPreview.BackgroundGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            customFont9.EmbeddedFontName = "TitilliumText22L Lt";
            customFont9.IsBold = true;
            customFont9.IsItalic = false;
            customFont9.IsUnderline = false;
            customFont9.Size = 8F;
            customFont9.StandardFontName = "Arial";
            customFont9.UseAntiAliasing = true;
            customFont9.UseEmbeddedFont = true;
            this.lblPreview.CustomFont = customFont9;
            this.lblPreview.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreview.ForeColor = System.Drawing.Color.White;
            this.lblPreview.Location = new System.Drawing.Point(0, 2);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(94, 14);
            this.lblPreview.TabIndex = 110;
            this.lblPreview.Text = "Preview";
            this.lblPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPreview.UseBackgroundGradient = false;
            // 
            // previewSongGridView
            // 
            this.previewSongGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewSongGridView.CanChangeOrderBy = false;
            this.previewSongGridView.CanMoveColumns = false;
            this.previewSongGridView.CanReorderItems = true;
            this.previewSongGridView.CanResizeColumns = true;
            this.previewSongGridView.DisplayDebugInformation = false;
            this.previewSongGridView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewSongGridView.ImageCacheSize = 10;
            this.previewSongGridView.Location = new System.Drawing.Point(0, 19);
            this.previewSongGridView.Name = "previewSongGridView";
            this.previewSongGridView.NowPlayingAudioFileId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.previewSongGridView.NowPlayingPlaylistItemId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.previewSongGridView.OrderByAscending = true;
            this.previewSongGridView.OrderByFieldName = "";
            this.previewSongGridView.Size = new System.Drawing.Size(260, 204);
            this.previewSongGridView.TabIndex = 98;
            this.previewSongGridView.Text = "songGridView1";
            songGridViewTheme1.AlbumCoverBackgroundColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            songGridViewTheme1.AlbumCoverBackgroundColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            customFont8.EmbeddedFontName = "";
            customFont8.IsBold = false;
            customFont8.IsItalic = false;
            customFont8.IsUnderline = false;
            customFont8.Size = 8F;
            customFont8.StandardFontName = "Arial";
            customFont8.UseAntiAliasing = true;
            customFont8.UseEmbeddedFont = false;
            songGridViewTheme1.Font = customFont8;
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
            this.previewSongGridView.Theme = songGridViewTheme1;
            // 
            // previewOutputMeter
            // 
            this.previewOutputMeter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewOutputMeter.DisplayDecibels = true;
            this.previewOutputMeter.DisplayType = MPfm.WindowsControls.OutputMeterDisplayType.Stereo;
            this.previewOutputMeter.DistortionThreshold = 0.9F;
            this.previewOutputMeter.DrawFloor = 0F;
            this.previewOutputMeter.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewOutputMeter.Location = new System.Drawing.Point(88, 19);
            this.previewOutputMeter.Name = "previewOutputMeter";
            this.previewOutputMeter.Size = new System.Drawing.Size(89, 204);
            this.previewOutputMeter.TabIndex = 108;
            customFont7.EmbeddedFontName = "LeagueGothic";
            customFont7.IsBold = false;
            customFont7.IsItalic = false;
            customFont7.IsUnderline = false;
            customFont7.Size = 9F;
            customFont7.StandardFontName = "Arial";
            customFont7.UseAntiAliasing = true;
            customFont7.UseEmbeddedFont = true;
            outputMeterTheme1.CustomFont = customFont7;
            outputMeterTheme1.FontColor = System.Drawing.Color.White;
            outputMeterTheme1.FontShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            outputMeterTheme1.GradientColor1 = System.Drawing.Color.Black;
            outputMeterTheme1.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            outputMeterTheme1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            outputMeterTheme1.Meter0dbLineColor = System.Drawing.Color.Gray;
            outputMeterTheme1.MeterDistortionGradientColor1 = System.Drawing.Color.Red;
            outputMeterTheme1.MeterDistortionGradientColor2 = System.Drawing.Color.DarkRed;
            outputMeterTheme1.MeterGradientColor1 = System.Drawing.Color.PaleGreen;
            outputMeterTheme1.MeterGradientColor2 = System.Drawing.Color.DarkGreen;
            outputMeterTheme1.MeterPeakLineColor = System.Drawing.Color.OliveDrab;
            this.previewOutputMeter.Theme = outputMeterTheme1;
            // 
            // lblPreviewPane
            // 
            this.lblPreviewPane.BackColor = System.Drawing.Color.Transparent;
            this.lblPreviewPane.BackgroundGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblPreviewPane.BackgroundGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblPreviewPane.BackgroundGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            customFont6.EmbeddedFontName = "Junction";
            customFont6.IsBold = false;
            customFont6.IsItalic = false;
            customFont6.IsUnderline = false;
            customFont6.Size = 8F;
            customFont6.StandardFontName = "Arial";
            customFont6.UseAntiAliasing = true;
            customFont6.UseEmbeddedFont = true;
            this.lblPreviewPane.CustomFont = customFont6;
            this.lblPreviewPane.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreviewPane.ForeColor = System.Drawing.Color.Black;
            this.lblPreviewPane.Location = new System.Drawing.Point(7, 56);
            this.lblPreviewPane.Name = "lblPreviewPane";
            this.lblPreviewPane.Size = new System.Drawing.Size(133, 16);
            this.lblPreviewPane.TabIndex = 108;
            this.lblPreviewPane.Text = "Showing preview pane:";
            this.lblPreviewPane.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPreviewPane.UseBackgroundGradient = false;
            // 
            // comboPreviewPane
            // 
            this.comboPreviewPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPreviewPane.BackColor = System.Drawing.Color.Gainsboro;
            this.comboPreviewPane.DisplayMember = "Title";
            this.comboPreviewPane.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPreviewPane.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboPreviewPane.ForeColor = System.Drawing.Color.Black;
            this.comboPreviewPane.FormattingEnabled = true;
            this.comboPreviewPane.Location = new System.Drawing.Point(146, 54);
            this.comboPreviewPane.Name = "comboPreviewPane";
            this.comboPreviewPane.Size = new System.Drawing.Size(464, 22);
            this.comboPreviewPane.TabIndex = 99;
            this.comboPreviewPane.ValueMember = "ClassName";
            this.comboPreviewPane.SelectedIndexChanged += new System.EventHandler(this.comboThemeControl_SelectedIndexChanged);
            // 
            // btnSaveThemeAs
            // 
            this.btnSaveThemeAs.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.btnSaveThemeAs.BorderWidth = 1;
            this.btnSaveThemeAs.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont14.EmbeddedFontName = "Junction";
            customFont14.IsBold = false;
            customFont14.IsItalic = false;
            customFont14.IsUnderline = false;
            customFont14.Size = 8F;
            customFont14.StandardFontName = "Arial";
            customFont14.UseAntiAliasing = true;
            customFont14.UseEmbeddedFont = true;
            this.btnSaveThemeAs.CustomFont = customFont14;
            this.btnSaveThemeAs.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnSaveThemeAs.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnSaveThemeAs.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnSaveThemeAs.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSaveThemeAs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveThemeAs.FontColor = System.Drawing.Color.Black;
            this.btnSaveThemeAs.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnSaveThemeAs.GradientColor2 = System.Drawing.Color.Gray;
            this.btnSaveThemeAs.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnSaveThemeAs.Image = global::MPfm.Properties.Resources.disk;
            this.btnSaveThemeAs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveThemeAs.Location = new System.Drawing.Point(180, 31);
            this.btnSaveThemeAs.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnSaveThemeAs.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnSaveThemeAs.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnSaveThemeAs.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnSaveThemeAs.Name = "btnSaveThemeAs";
            this.btnSaveThemeAs.Size = new System.Drawing.Size(107, 24);
            this.btnSaveThemeAs.TabIndex = 115;
            this.btnSaveThemeAs.Text = "Save theme as";
            this.btnSaveThemeAs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSaveThemeAs.UseVisualStyleBackColor = true;
            // 
            // lblThemeTitle
            // 
            this.lblThemeTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblThemeTitle.BackgroundGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblThemeTitle.BackgroundGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblThemeTitle.BackgroundGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            customFont15.EmbeddedFontName = "Junction";
            customFont15.IsBold = false;
            customFont15.IsItalic = false;
            customFont15.IsUnderline = false;
            customFont15.Size = 8F;
            customFont15.StandardFontName = "Arial";
            customFont15.UseAntiAliasing = true;
            customFont15.UseEmbeddedFont = true;
            this.lblThemeTitle.CustomFont = customFont15;
            this.lblThemeTitle.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThemeTitle.Location = new System.Drawing.Point(4, 84);
            this.lblThemeTitle.Name = "lblThemeTitle";
            this.lblThemeTitle.Size = new System.Drawing.Size(48, 18);
            this.lblThemeTitle.TabIndex = 116;
            this.lblThemeTitle.Text = "Theme:";
            this.lblThemeTitle.UseBackgroundGradient = false;
            // 
            // lblCurrentTheme
            // 
            this.lblCurrentTheme.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentTheme.BackgroundGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblCurrentTheme.BackgroundGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblCurrentTheme.BackgroundGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            customFont16.EmbeddedFontName = "Junction";
            customFont16.IsBold = true;
            customFont16.IsItalic = false;
            customFont16.IsUnderline = false;
            customFont16.Size = 8F;
            customFont16.StandardFontName = "Arial";
            customFont16.UseAntiAliasing = true;
            customFont16.UseEmbeddedFont = true;
            this.lblCurrentTheme.CustomFont = customFont16;
            this.lblCurrentTheme.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTheme.Location = new System.Drawing.Point(93, 59);
            this.lblCurrentTheme.Name = "lblCurrentTheme";
            this.lblCurrentTheme.Size = new System.Drawing.Size(113, 18);
            this.lblCurrentTheme.TabIndex = 114;
            this.lblCurrentTheme.Text = "Default Theme";
            this.lblCurrentTheme.UseBackgroundGradient = false;
            // 
            // cboTheme
            // 
            this.cboTheme.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTheme.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboTheme.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboTheme.DisplayMember = "Title";
            this.cboTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTheme.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTheme.FormattingEnabled = true;
            this.cboTheme.Location = new System.Drawing.Point(53, 82);
            this.cboTheme.Name = "cboTheme";
            this.cboTheme.Size = new System.Drawing.Size(576, 22);
            this.cboTheme.TabIndex = 112;
            this.cboTheme.ValueMember = "DriverType";
            // 
            // panelBackground
            // 
            this.panelBackground.Controls.Add(this.cboTheme);
            this.panelBackground.Controls.Add(this.lblCurrentTheme);
            this.panelBackground.Controls.Add(this.lblThemeTitle);
            this.panelBackground.Controls.Add(this.btnSaveThemeAs);
            this.panelBackground.Controls.Add(this.panelThemeProperties);
            this.panelBackground.Controls.Add(this.lblCurrentThemeTitle);
            this.panelBackground.Controls.Add(this.btnNewTheme);
            this.panelBackground.Controls.Add(this.btnClose);
            this.panelBackground.Controls.Add(this.btnApplyTheme);
            this.panelBackground.Controls.Add(this.btnSaveTheme);
            customFont17.EmbeddedFontName = "TitilliumText22L Lt";
            customFont17.IsBold = true;
            customFont17.IsItalic = false;
            customFont17.IsUnderline = false;
            customFont17.Size = 12F;
            customFont17.StandardFontName = "Arial";
            customFont17.UseAntiAliasing = true;
            customFont17.UseEmbeddedFont = true;
            this.panelBackground.CustomFont = customFont17;
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.ExpandedHeight = 200;
            this.panelBackground.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelBackground.GradientColor1 = System.Drawing.Color.Silver;
            this.panelBackground.GradientColor2 = System.Drawing.Color.Gray;
            this.panelBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelBackground.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelBackground.HeaderExpandable = false;
            this.panelBackground.HeaderExpanded = true;
            this.panelBackground.HeaderForeColor = System.Drawing.Color.Black;
            this.panelBackground.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelBackground.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelBackground.HeaderHeight = 30;
            this.panelBackground.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelBackground.HeaderTitle = "Themes";
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(639, 431);
            this.panelBackground.TabIndex = 109;
            // 
            // frmThemes
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(639, 431);
            this.Controls.Add(this.panelBackground);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(525, 400);
            this.Name = "frmThemes";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Themes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSettings_FormClosing);
            this.Load += new System.EventHandler(this.frmThemes_Load);
            this.Shown += new System.EventHandler(this.frmThemes_Shown);
            this.panelThemeProperties.ResumeLayout(false);
            this.panelThemeProperties.PerformLayout();
            this.splitTheme.Panel1.ResumeLayout(false);
            this.splitTheme.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitTheme)).EndInit();
            this.splitTheme.ResumeLayout(false);
            this.panelBackground.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.FolderBrowserDialog dialogAddFolder;
        private System.Windows.Forms.OpenFileDialog dialogOpenFile;
        private System.Windows.Forms.OpenFileDialog dialogOpenTheme;
        private System.Windows.Forms.SaveFileDialog dialogSaveTheme;
        private System.Windows.Forms.FolderBrowserDialog dialogBrowsePeakFileDirectory;
        private WindowsControls.Button btnSaveTheme;
        private WindowsControls.Button btnApplyTheme;
        private WindowsControls.Button btnClose;
        private WindowsControls.Button btnNewTheme;
        private WindowsControls.Label lblCurrentThemeTitle;
        private WindowsControls.Panel panelThemeProperties;
        private System.Windows.Forms.ComboBox comboPreviewPane;
        private WindowsControls.Label lblPreviewPane;
        private System.Windows.Forms.SplitContainer splitTheme;
        private WindowsControls.OutputMeter previewOutputMeter;
        private WindowsControls.SongGridView previewSongGridView;
        private WindowsControls.Label lblPreview;
        private WindowsControls.Label lblProperties;
        public System.Windows.Forms.PropertyGrid propertyGridTheme;
        private WindowsControls.Label lblThemeName;
        private System.Windows.Forms.TextBox txtThemeName;
        private System.Windows.Forms.TextBox txtThemeAuthor;
        private WindowsControls.Label lblThemeAuthor;
        private WindowsControls.Button btnSaveThemeAs;
        private WindowsControls.Label lblThemeTitle;
        private WindowsControls.Label lblCurrentTheme;
        private System.Windows.Forms.ComboBox cboTheme;
        private WindowsControls.Panel panelBackground;
    }
}