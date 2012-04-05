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
            MPfm.WindowsControls.CustomFont customFont11 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdateLibraryStatus));
            this.workerUpdateLibrary = new System.ComponentModel.BackgroundWorker();
            this.workerTimer = new System.ComponentModel.BackgroundWorker();
            this.saveLogDialog = new System.Windows.Forms.SaveFileDialog();
            this.panelMain = new MPfm.WindowsControls.Panel();
            this.lblEstimatedTimeLeft = new MPfm.WindowsControls.Label();
            this.lblTimeElapsed = new MPfm.WindowsControls.Label();
            this.linkSaveLog = new MPfm.WindowsControls.LinkLabel();
            this.btnOK = new MPfm.WindowsControls.Button();
            this.btnCancel = new MPfm.WindowsControls.Button();
            this.lblProgress = new MPfm.WindowsControls.Label();
            this.panelLog = new MPfm.WindowsControls.Panel();
            this.lbLog = new MPfm.WindowsControls.ListBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblMessage = new MPfm.WindowsControls.Label();
            this.lblTitle = new MPfm.WindowsControls.Label();
            this.panelMain.SuspendLayout();
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
            this.panelMain.Controls.Add(this.btnOK);
            this.panelMain.Controls.Add(this.btnCancel);
            this.panelMain.Controls.Add(this.lblProgress);
            this.panelMain.Controls.Add(this.panelLog);
            this.panelMain.Controls.Add(this.progressBar);
            this.panelMain.Controls.Add(this.lblMessage);
            this.panelMain.Controls.Add(this.lblTitle);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.ExpandedHeight = 188;
            this.panelMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelMain.HeaderExpanded = true;
            this.panelMain.HeaderHeight = 22;
            this.panelMain.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelMain.HeaderTitle = "Updating Library";
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(800, 373);
            this.panelMain.TabIndex = 27;
            // 
            // lblEstimatedTimeLeft
            // 
            this.lblEstimatedTimeLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEstimatedTimeLeft.BackColor = System.Drawing.Color.Transparent;

            this.lblEstimatedTimeLeft.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstimatedTimeLeft.ForeColor = System.Drawing.Color.LightGray;
            this.lblEstimatedTimeLeft.Location = new System.Drawing.Point(508, 110);
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

            this.lblTimeElapsed.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeElapsed.ForeColor = System.Drawing.Color.LightGray;
            this.lblTimeElapsed.Location = new System.Drawing.Point(15, 110);
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
            this.linkSaveLog.Location = new System.Drawing.Point(484, 333);
            this.linkSaveLog.Name = "linkSaveLog";
            this.linkSaveLog.Size = new System.Drawing.Size(102, 20);
            this.linkSaveLog.TabIndex = 65;
            this.linkSaveLog.TabStop = true;
            this.linkSaveLog.Text = "Save log to file";
            this.linkSaveLog.Visible = false;
            this.linkSaveLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSaveLog_LinkClicked);
            // 
            // btnOK
            // 
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Image = global::MPfm.Properties.Resources.accept;
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOK.Location = new System.Drawing.Point(594, 323);
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
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Enabled = false;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Image = global::MPfm.Properties.Resources.cancel;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancel.Location = new System.Drawing.Point(692, 323);
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

            this.lblProgress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.ForeColor = System.Drawing.Color.White;
            this.lblProgress.Location = new System.Drawing.Point(12, 110);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(775, 26);
            this.lblProgress.TabIndex = 59;
            this.lblProgress.Text = "Progress";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelLog
            // 
            this.panelLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLog.Controls.Add(this.lbLog);
            this.panelLog.ExpandedHeight = 56;
            this.panelLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLog.HeaderExpandable = false;
            this.panelLog.HeaderExpanded = true;
            this.panelLog.HeaderHeight = 16;
            this.panelLog.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelLog.HeaderTitle = "Error Log";
            this.panelLog.Location = new System.Drawing.Point(18, 139);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(768, 176);
            this.panelLog.TabIndex = 58;
            // 
            // lbLog
            // 
            this.lbLog.BackColor = System.Drawing.Color.Black;
            this.lbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            customFont7.EmbeddedFontName = "Droid Sans Mono";
            customFont7.IsBold = false;
            customFont7.IsItalic = false;
            customFont7.IsUnderline = false;
            customFont7.Size = 8F;
            customFont7.StandardFontName = "Courier New";
            customFont7.UseAntiAliasing = true;
            customFont7.UseEmbeddedFont = true;
            this.lbLog.CustomFont = customFont7;
            this.lbLog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbLog.Font = new System.Drawing.Font("Courier New", 8F);
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
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 84);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(772, 23);
            this.progressBar.TabIndex = 3;
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.BackColor = System.Drawing.Color.Transparent;

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
            this.ClientSize = new System.Drawing.Size(800, 373);
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
        private MPfm.WindowsControls.ListBox lbLog;
        private MPfm.WindowsControls.Label lblEstimatedTimeLeft;
        private MPfm.WindowsControls.Label lblTimeElapsed;
        private MPfm.WindowsControls.LinkLabel linkSaveLog;
        private System.ComponentModel.BackgroundWorker workerTimer;
        private System.Windows.Forms.SaveFileDialog saveLogDialog;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}