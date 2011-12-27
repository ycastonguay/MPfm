namespace MPfm
{
    partial class frmUpdateLibraryStatus
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
            MPfm.WindowsControls.CustomFont customFont13 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.EmbeddedFont embeddedFont1 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont embeddedFont2 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont embeddedFont3 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont embeddedFont4 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.CustomFont customFont11 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont12 = new MPfm.WindowsControls.CustomFont();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdateLibraryStatus));
            this.workerUpdateLibrary = new System.ComponentModel.BackgroundWorker();
            this.workerTimer = new System.ComponentModel.BackgroundWorker();
            this.saveLogDialog = new System.Windows.Forms.SaveFileDialog();
            this.panelMain = new MPfm.WindowsControls.Panel();
            this.lblEstimatedTimeLeft = new MPfm.WindowsControls.Label();
            this.lblTimeElapsed = new MPfm.WindowsControls.Label();
            this.linkSaveLog = new MPfm.WindowsControls.LinkLabel();
            this.lblSongTitle = new MPfm.WindowsControls.Label();
            this.lblAlbum = new MPfm.WindowsControls.Label();
            this.lblArtist = new MPfm.WindowsControls.Label();
            this.btnOK = new MPfm.WindowsControls.Button();
            this.btnCancel = new MPfm.WindowsControls.Button();
            this.lblProgress = new MPfm.WindowsControls.Label();
            this.picAlbum = new System.Windows.Forms.PictureBox();
            this.panelLog = new MPfm.WindowsControls.Panel();
            this.lbLog = new MPfm.WindowsControls.ListBox();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblMessage = new MPfm.WindowsControls.Label();
            this.lblTitle = new MPfm.WindowsControls.Label();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlbum)).BeginInit();
            this.panelLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // workerUpdateLibrary
            // 
            this.workerUpdateLibrary.WorkerReportsProgress = true;
            this.workerUpdateLibrary.WorkerSupportsCancellation = true;
            // 
            // workerTimer
            // 
            this.workerTimer.WorkerReportsProgress = true;
            this.workerTimer.WorkerSupportsCancellation = true;
            this.workerTimer.DoWork += new System.ComponentModel.DoWorkEventHandler(this.workerTimer_DoWork);
            this.workerTimer.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.workerTimer_ProgressChanged);
            // 
            // saveLogDialog
            // 
            this.saveLogDialog.DefaultExt = "txt";
            this.saveLogDialog.FileName = "log.txt";
            this.saveLogDialog.Filter = "Text files|*.txt|All files|*.*";
            this.saveLogDialog.Title = "Please choose a filename ";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.lblEstimatedTimeLeft);
            this.panelMain.Controls.Add(this.lblTimeElapsed);
            this.panelMain.Controls.Add(this.linkSaveLog);
            this.panelMain.Controls.Add(this.lblSongTitle);
            this.panelMain.Controls.Add(this.lblAlbum);
            this.panelMain.Controls.Add(this.lblArtist);
            this.panelMain.Controls.Add(this.btnOK);
            this.panelMain.Controls.Add(this.btnCancel);
            this.panelMain.Controls.Add(this.lblProgress);
            this.panelMain.Controls.Add(this.picAlbum);
            this.panelMain.Controls.Add(this.panelLog);
            this.panelMain.Controls.Add(this.progressBar);
            this.panelMain.Controls.Add(this.lblMessage);
            this.panelMain.Controls.Add(this.lblTitle);
            customFont13.EmbeddedFontName = "";
            customFont13.IsBold = false;
            customFont13.IsItalic = false;
            customFont13.IsUnderline = false;
            customFont13.Size = 8F;
            customFont13.StandardFontName = "Arial";
            customFont13.UseAntiAliasing = true;
            customFont13.UseEmbeddedFont = false;
            this.panelMain.CustomFont = customFont13;
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.ExpandedHeight = 188;
            this.panelMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelMain.GradientColor1 = System.Drawing.Color.Black;
            this.panelMain.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelMain.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelMain.HeaderCustomFontName = "NeuzeitS";
            this.panelMain.HeaderExpanded = true;
            this.panelMain.HeaderForeColor = System.Drawing.Color.White;
            this.panelMain.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.panelMain.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.panelMain.HeaderHeight = 22;
            this.panelMain.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelMain.HeaderTitle = "Updating Library";
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(800, 450);
            this.panelMain.TabIndex = 27;
            // 
            // lblEstimatedTimeLeft
            // 
            this.lblEstimatedTimeLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEstimatedTimeLeft.BackColor = System.Drawing.Color.Transparent;
            customFont1.EmbeddedFontName = "Junction";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 8F;
            customFont1.StandardFontName = "Arial";
            customFont1.UseAntiAliasing = true;
            customFont1.UseEmbeddedFont = true;
            this.lblEstimatedTimeLeft.CustomFont = customFont1;
            this.lblEstimatedTimeLeft.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstimatedTimeLeft.ForeColor = System.Drawing.Color.LightGray;
            this.lblEstimatedTimeLeft.Location = new System.Drawing.Point(508, 187);
            this.lblEstimatedTimeLeft.Name = "lblEstimatedTimeLeft";
            this.lblEstimatedTimeLeft.Size = new System.Drawing.Size(287, 23);
            this.lblEstimatedTimeLeft.TabIndex = 67;
            this.lblEstimatedTimeLeft.Text = "Estimated time left :";
            this.lblEstimatedTimeLeft.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTimeElapsed
            // 
            this.lblTimeElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeElapsed.BackColor = System.Drawing.Color.Transparent;
            customFont2.EmbeddedFontName = "Junction";
            customFont2.IsBold = false;
            customFont2.IsItalic = false;
            customFont2.IsUnderline = false;
            customFont2.Size = 8F;
            customFont2.StandardFontName = "Arial";
            customFont2.UseAntiAliasing = true;
            customFont2.UseEmbeddedFont = true;
            this.lblTimeElapsed.CustomFont = customFont2;
            this.lblTimeElapsed.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeElapsed.ForeColor = System.Drawing.Color.LightGray;
            this.lblTimeElapsed.Location = new System.Drawing.Point(15, 187);
            this.lblTimeElapsed.Name = "lblTimeElapsed";
            this.lblTimeElapsed.Size = new System.Drawing.Size(197, 23);
            this.lblTimeElapsed.TabIndex = 66;
            this.lblTimeElapsed.Text = "Time elapsed :";
            this.lblTimeElapsed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkSaveLog
            // 
            this.linkSaveLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkSaveLog.BackColor = System.Drawing.Color.Transparent;
            customFont3.EmbeddedFontName = "Junction";
            customFont3.IsBold = false;
            customFont3.IsItalic = false;
            customFont3.IsUnderline = true;
            customFont3.Size = 10F;
            customFont3.StandardFontName = "Arial";
            customFont3.UseAntiAliasing = true;
            customFont3.UseEmbeddedFont = true;
            this.linkSaveLog.CustomFont = customFont3;
            this.linkSaveLog.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkSaveLog.ForeColor = System.Drawing.Color.Gainsboro;
            this.linkSaveLog.Location = new System.Drawing.Point(484, 410);
            this.linkSaveLog.Name = "linkSaveLog";
            this.linkSaveLog.Size = new System.Drawing.Size(102, 20);
            this.linkSaveLog.TabIndex = 65;
            this.linkSaveLog.TabStop = true;
            this.linkSaveLog.Text = "Save log to file";
            this.linkSaveLog.Visible = false;
            this.linkSaveLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSaveLog_LinkClicked);
            // 
            // lblSongTitle
            // 
            this.lblSongTitle.BackColor = System.Drawing.Color.Transparent;
            customFont4.EmbeddedFontName = "TitilliumText22L Lt";
            customFont4.IsBold = false;
            customFont4.IsItalic = false;
            customFont4.IsUnderline = false;
            customFont4.Size = 11F;
            customFont4.StandardFontName = "Arial";
            customFont4.UseAntiAliasing = true;
            customFont4.UseEmbeddedFont = true;
            this.lblSongTitle.CustomFont = customFont4;
            this.lblSongTitle.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSongTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblSongTitle.Location = new System.Drawing.Point(94, 123);
            this.lblSongTitle.Name = "lblSongTitle";
            this.lblSongTitle.Size = new System.Drawing.Size(687, 22);
            this.lblSongTitle.TabIndex = 64;
            this.lblSongTitle.Text = "Song Title";
            this.lblSongTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAlbum
            // 
            this.lblAlbum.BackColor = System.Drawing.Color.Transparent;
            customFont5.EmbeddedFontName = "TitilliumText22L Lt";
            customFont5.IsBold = false;
            customFont5.IsItalic = false;
            customFont5.IsUnderline = false;
            customFont5.Size = 11F;
            customFont5.StandardFontName = "Arial";
            customFont5.UseAntiAliasing = true;
            customFont5.UseEmbeddedFont = true;
            this.lblAlbum.CustomFont = customFont5;
            this.lblAlbum.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlbum.ForeColor = System.Drawing.Color.LightGray;
            this.lblAlbum.Location = new System.Drawing.Point(94, 102);
            this.lblAlbum.Name = "lblAlbum";
            this.lblAlbum.Size = new System.Drawing.Size(687, 22);
            this.lblAlbum.TabIndex = 63;
            this.lblAlbum.Text = "Album Title";
            this.lblAlbum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblArtist
            // 
            this.lblArtist.BackColor = System.Drawing.Color.Transparent;
            customFont6.EmbeddedFontName = "TitilliumText22L Lt";
            customFont6.IsBold = true;
            customFont6.IsItalic = false;
            customFont6.IsUnderline = false;
            customFont6.Size = 12F;
            customFont6.StandardFontName = "Arial";
            customFont6.UseAntiAliasing = true;
            customFont6.UseEmbeddedFont = true;
            this.lblArtist.CustomFont = customFont6;
            this.lblArtist.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArtist.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.lblArtist.Location = new System.Drawing.Point(93, 80);
            this.lblArtist.Name = "lblArtist";
            this.lblArtist.Size = new System.Drawing.Size(687, 22);
            this.lblArtist.TabIndex = 62;
            this.lblArtist.Text = "Artist Name";
            this.lblArtist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOK
            // 
            this.btnOK.BorderColor = System.Drawing.Color.Black;
            this.btnOK.BorderWidth = 1;
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont7.EmbeddedFontName = "Junction";
            customFont7.IsBold = false;
            customFont7.IsItalic = false;
            customFont7.IsUnderline = false;
            customFont7.Size = 9F;
            customFont7.StandardFontName = "Arial";
            customFont7.UseAntiAliasing = true;
            customFont7.UseEmbeddedFont = true;
            this.btnOK.CustomFont = customFont7;
            this.btnOK.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnOK.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnOK.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnOK.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.FontColor = System.Drawing.Color.Black;
            this.btnOK.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnOK.GradientColor2 = System.Drawing.Color.Gray;
            this.btnOK.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnOK.Image = global::MPfm.Properties.Resources.accept;
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOK.Location = new System.Drawing.Point(594, 400);
            this.btnOK.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnOK.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnOK.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnOK.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(92, 40);
            this.btnOK.TabIndex = 61;
            this.btnOK.Text = "OK";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderColor = System.Drawing.Color.Black;
            this.btnCancel.BorderWidth = 1;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont8.EmbeddedFontName = "Junction";
            customFont8.IsBold = false;
            customFont8.IsItalic = false;
            customFont8.IsUnderline = false;
            customFont8.Size = 9F;
            customFont8.StandardFontName = "Arial";
            customFont8.UseAntiAliasing = true;
            customFont8.UseEmbeddedFont = true;
            this.btnCancel.CustomFont = customFont8;
            this.btnCancel.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnCancel.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnCancel.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnCancel.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnCancel.Enabled = false;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.FontColor = System.Drawing.Color.Black;
            this.btnCancel.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnCancel.GradientColor2 = System.Drawing.Color.Gray;
            this.btnCancel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnCancel.Image = global::MPfm.Properties.Resources.cancel;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancel.Location = new System.Drawing.Point(692, 400);
            this.btnCancel.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnCancel.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnCancel.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnCancel.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 40);
            this.btnCancel.TabIndex = 60;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgress.BackColor = System.Drawing.Color.Transparent;
            customFont9.EmbeddedFontName = "Junction";
            customFont9.IsBold = false;
            customFont9.IsItalic = false;
            customFont9.IsUnderline = false;
            customFont9.Size = 9F;
            customFont9.StandardFontName = "Arial";
            customFont9.UseAntiAliasing = true;
            customFont9.UseEmbeddedFont = true;
            this.lblProgress.CustomFont = customFont9;
            this.lblProgress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.ForeColor = System.Drawing.Color.White;
            this.lblProgress.Location = new System.Drawing.Point(12, 187);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(775, 26);
            this.lblProgress.TabIndex = 59;
            this.lblProgress.Text = "Progress";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picAlbum
            // 
            this.picAlbum.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.picAlbum.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAlbum.ImageLocation = "";
            this.picAlbum.Location = new System.Drawing.Point(18, 80);
            this.picAlbum.Name = "picAlbum";
            this.picAlbum.Size = new System.Drawing.Size(70, 70);
            this.picAlbum.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picAlbum.TabIndex = 58;
            this.picAlbum.TabStop = false;
            // 
            // panelLog
            // 
            this.panelLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLog.Controls.Add(this.lbLog);
            customFont10.EmbeddedFontName = "TitilliumText22L Lt";
            customFont10.IsBold = false;
            customFont10.IsItalic = false;
            customFont10.IsUnderline = false;
            customFont10.Size = 9F;
            customFont10.StandardFontName = "Arial";
            customFont10.UseAntiAliasing = true;
            customFont10.UseEmbeddedFont = true;
            this.panelLog.CustomFont = customFont10;
            this.panelLog.ExpandedHeight = 56;
            this.panelLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLog.GradientColor1 = System.Drawing.Color.Black;
            this.panelLog.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.panelLog.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelLog.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelLog.HeaderExpandable = false;
            this.panelLog.HeaderExpanded = true;
            this.panelLog.HeaderForeColor = System.Drawing.Color.White;
            this.panelLog.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelLog.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelLog.HeaderHeight = 16;
            this.panelLog.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelLog.HeaderTitle = "Log";
            this.panelLog.Location = new System.Drawing.Point(18, 216);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(768, 176);
            this.panelLog.TabIndex = 58;
            // 
            // lbLog
            // 
            this.lbLog.AntiAliasingEnabled = true;
            this.lbLog.BackColor = System.Drawing.Color.Black;
            this.lbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbLog.CustomFontName = "Droid Sans Mono";
            this.lbLog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbLog.Font = new System.Drawing.Font("Courier New", 8F);
            this.lbLog.FontCollection = this.fontCollection;
            this.lbLog.ForeColor = System.Drawing.Color.White;
            this.lbLog.FormattingEnabled = true;
            this.lbLog.ItemHeight = 14;
            this.lbLog.Location = new System.Drawing.Point(3, 21);
            this.lbLog.Name = "lbLog";
            this.lbLog.ScrollAlwaysVisible = true;
            this.lbLog.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbLog.Size = new System.Drawing.Size(762, 154);
            this.lbLog.TabIndex = 0;
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
            embeddedFont4.AssemblyPath = "MPfm.Fonts.dll";
            embeddedFont4.Name = "Droid Sans Mono";
            embeddedFont4.ResourceName = "MPfm.Fonts.DroidSansMono.ttf";
            this.fontCollection.Fonts.Add(embeddedFont1);
            this.fontCollection.Fonts.Add(embeddedFont2);
            this.fontCollection.Fonts.Add(embeddedFont3);
            this.fontCollection.Fonts.Add(embeddedFont4);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 161);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(772, 23);
            this.progressBar.TabIndex = 3;
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;
            customFont11.EmbeddedFontName = "Junction";
            customFont11.IsBold = false;
            customFont11.IsItalic = false;
            customFont11.IsUnderline = false;
            customFont11.Size = 8F;
            customFont11.StandardFontName = "Arial";
            customFont11.UseAntiAliasing = true;
            customFont11.UseEmbeddedFont = true;
            this.lblMessage.CustomFont = customFont11;
            this.lblMessage.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Location = new System.Drawing.Point(13, 54);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(773, 23);
            this.lblMessage.TabIndex = 49;
            this.lblMessage.Text = "Message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            customFont12.EmbeddedFontName = "TitilliumText22L Lt";
            customFont12.IsBold = false;
            customFont12.IsItalic = false;
            customFont12.IsUnderline = false;
            customFont12.Size = 12F;
            customFont12.StandardFontName = "Arial";
            customFont12.UseAntiAliasing = true;
            customFont12.UseEmbeddedFont = true;
            this.lblTitle.CustomFont = customFont12;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.LightGray;
            this.lblTitle.Location = new System.Drawing.Point(12, 33);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(685, 22);
            this.lblTitle.TabIndex = 44;
            this.lblTitle.Text = "Title";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmUpdateLibraryStatus
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.panelMain);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUpdateLibraryStatus";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updating Library";
            this.Shown += new System.EventHandler(this.frmUpdateLibraryStatus_Shown);
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAlbum)).EndInit();
            this.panelLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.ComponentModel.BackgroundWorker workerUpdateLibrary;
        private MPfm.WindowsControls.Panel panelMain;
        private MPfm.WindowsControls.Panel panelLog;
        private MPfm.WindowsControls.Label lblMessage;
        private MPfm.WindowsControls.Label lblTitle;
        private MPfm.WindowsControls.Label lblProgress;
        private MPfm.WindowsControls.Button btnOK;
        private MPfm.WindowsControls.Button btnCancel;
        private System.Windows.Forms.PictureBox picAlbum;
        private MPfm.WindowsControls.Label lblSongTitle;
        private MPfm.WindowsControls.Label lblAlbum;
        private MPfm.WindowsControls.Label lblArtist;
        private MPfm.WindowsControls.ListBox lbLog;
        private MPfm.WindowsControls.Label lblEstimatedTimeLeft;
        private MPfm.WindowsControls.Label lblTimeElapsed;
        private MPfm.WindowsControls.LinkLabel linkSaveLog;
        private System.ComponentModel.BackgroundWorker workerTimer;
        private System.Windows.Forms.SaveFileDialog saveLogDialog;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}