namespace MPfm
{
    partial class frmLoadPlaylist
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
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            this.panelLoadPlaylist = new MPfm.WindowsControls.Panel();
            this.lblPercentage = new MPfm.WindowsControls.Label();
            this.btnCancel = new MPfm.WindowsControls.Button();
            this.lblFilePath = new MPfm.WindowsControls.Label();
            this.lblFile = new MPfm.WindowsControls.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.panelLoadPlaylist.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLoadPlaylist
            // 
            this.panelLoadPlaylist.Controls.Add(this.lblPercentage);
            this.panelLoadPlaylist.Controls.Add(this.btnCancel);
            this.panelLoadPlaylist.Controls.Add(this.lblFilePath);
            this.panelLoadPlaylist.Controls.Add(this.lblFile);
            this.panelLoadPlaylist.Controls.Add(this.progressBar);
            customFont5.EmbeddedFontName = "Junction";
            customFont5.IsBold = false;
            customFont5.IsItalic = false;
            customFont5.IsUnderline = false;
            customFont5.Size = 8.25F;
            customFont5.StandardFontName = "Arial";
            customFont5.UseAntiAliasing = true;
            customFont5.UseEmbeddedFont = true;
            this.panelLoadPlaylist.CustomFont = customFont5;
            this.panelLoadPlaylist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLoadPlaylist.ExpandedHeight = 0;
            this.panelLoadPlaylist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLoadPlaylist.GradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelLoadPlaylist.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelLoadPlaylist.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;            
            this.panelLoadPlaylist.HeaderExpandable = false;
            this.panelLoadPlaylist.HeaderExpanded = true;
            this.panelLoadPlaylist.HeaderForeColor = System.Drawing.Color.White;
            this.panelLoadPlaylist.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.panelLoadPlaylist.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelLoadPlaylist.HeaderHeight = 0;
            this.panelLoadPlaylist.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelLoadPlaylist.Location = new System.Drawing.Point(0, 0);
            this.panelLoadPlaylist.Name = "panelLoadPlaylist";
            this.panelLoadPlaylist.Size = new System.Drawing.Size(600, 55);
            this.panelLoadPlaylist.TabIndex = 84;
            // 
            // lblPercentage
            // 
            this.lblPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPercentage.BackColor = System.Drawing.Color.Transparent;
            customFont1.EmbeddedFontName = "Junction";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 8F;
            customFont1.StandardFontName = "Arial";
            customFont1.UseAntiAliasing = true;
            customFont1.UseEmbeddedFont = true;
            this.lblPercentage.CustomFont = customFont1;
            this.lblPercentage.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPercentage.ForeColor = System.Drawing.Color.White;
            this.lblPercentage.Location = new System.Drawing.Point(560, 26);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(37, 17);
            this.lblPercentage.TabIndex = 82;
            this.lblPercentage.Text = "100%";
            this.lblPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(6, 27);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(58, 20);
            this.btnCancel.TabIndex = 84;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblFilePath
            // 
            this.lblFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilePath.BackColor = System.Drawing.Color.Transparent;
            customFont3.EmbeddedFontName = "Junction";
            customFont3.IsBold = true;
            customFont3.IsItalic = false;
            customFont3.IsUnderline = false;
            customFont3.Size = 8F;
            customFont3.StandardFontName = "Arial Unicode MS";
            customFont3.UseAntiAliasing = true;
            customFont3.UseEmbeddedFont = true;
            this.lblFilePath.CustomFont = customFont3;
            this.lblFilePath.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilePath.ForeColor = System.Drawing.Color.White;
            this.lblFilePath.Location = new System.Drawing.Point(34, 4);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(558, 17);
            this.lblFilePath.TabIndex = 85;
            this.lblFilePath.Text = "C:\\MP3\\Unknown Artist\\Unknown Album\\01 Hello World.mp3";
            // 
            // lblFile
            // 
            this.lblFile.BackColor = System.Drawing.Color.Transparent;
            customFont4.EmbeddedFontName = "Junction";
            customFont4.IsBold = false;
            customFont4.IsItalic = false;
            customFont4.IsUnderline = false;
            customFont4.Size = 8F;
            customFont4.StandardFontName = "Arial";
            customFont4.UseAntiAliasing = true;
            customFont4.UseEmbeddedFont = true;
            this.lblFile.CustomFont = customFont4;
            this.lblFile.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFile.ForeColor = System.Drawing.Color.White;
            this.lblFile.Location = new System.Drawing.Point(3, 4);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(37, 17);
            this.lblFile.TabIndex = 84;
            this.lblFile.Text = "File :";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(70, 27);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(487, 20);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 81;
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            // 
            // frmLoadPlaylist
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(600, 55);
            this.ControlBox = false;
            this.Controls.Add(this.panelLoadPlaylist);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmLoadPlaylist";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading playlist";
            this.Load += new System.EventHandler(this.frmLoadPlaylist_Load);
            this.panelLoadPlaylist.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private WindowsControls.Panel panelLoadPlaylist;
        private WindowsControls.Label lblPercentage;
        private WindowsControls.Button btnCancel;
        private WindowsControls.Label lblFilePath;
        private WindowsControls.Label lblFile;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker worker;

    }
}