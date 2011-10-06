namespace MPfm
{
    partial class frmSplash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSplash));
            this.timerUpdateGUI = new System.Windows.Forms.Timer(this.components);
            this.timerFading = new System.Windows.Forms.Timer(this.components);
            this.lblStatus = new MPfm.WindowsControls.Label();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.lblVersion = new MPfm.WindowsControls.Label();
            this.pictureBackground = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // timerUpdateGUI
            // 
            this.timerUpdateGUI.Enabled = true;
            this.timerUpdateGUI.Interval = 50;
            this.timerUpdateGUI.Tick += new System.EventHandler(this.timerUpdateGUI_Tick);
            // 
            // timerFading
            // 
            this.timerFading.Enabled = true;
            this.timerFading.Interval = 5;
            this.timerFading.Tick += new System.EventHandler(this.timerFading_Tick);
            // 
            // lblStatus
            // 
            this.lblStatus.AntiAliasingEnabled = true;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(4)))), ((int)(((byte)(4)))));
            this.lblStatus.CustomFontName = "TitilliumText22L Lt";
            this.lblStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.FontCollection = this.fontCollection;
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(8, 481);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(620, 22);
            this.lblStatus.TabIndex = 50;
            this.lblStatus.Text = "Initializing tracing...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fontCollection
            // 
            customFont1.AssemblyPath = "MPfm.Fonts.dll";
            customFont1.Name = "Junction";
            customFont1.ResourceName = "MPfm.Fonts.Junction.ttf";
            customFont2.AssemblyPath = "MPfm.Fonts.dll";
            customFont2.Name = "TitilliumText22L Lt";
            customFont2.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            customFont3.AssemblyPath = "MPfm.Fonts.dll";
            customFont3.Name = "BPmono";
            customFont3.ResourceName = "MPfm.Fonts.BPmono.ttf";
            this.fontCollection.Fonts.Add(customFont1);
            this.fontCollection.Fonts.Add(customFont2);
            this.fontCollection.Fonts.Add(customFont3);
            // 
            // lblVersion
            // 
            this.lblVersion.AntiAliasingEnabled = true;
            this.lblVersion.BackColor = System.Drawing.Color.Black;
            this.lblVersion.CustomFontName = "BPMono";
            this.lblVersion.Font = new System.Drawing.Font("Arial", 9F);
            this.lblVersion.FontCollection = this.fontCollection;
            this.lblVersion.ForeColor = System.Drawing.Color.White;
            this.lblVersion.Location = new System.Drawing.Point(7, 8);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(58, 16);
            this.lblVersion.TabIndex = 55;
            this.lblVersion.Text = "0.0.0.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBackground
            // 
            this.pictureBackground.Image = global::MPfm.Properties.Resources.MPFM_Splash_0_4;
            this.pictureBackground.Location = new System.Drawing.Point(0, 0);
            this.pictureBackground.Name = "pictureBackground";
            this.pictureBackground.Size = new System.Drawing.Size(640, 504);
            this.pictureBackground.TabIndex = 0;
            this.pictureBackground.TabStop = false;
            this.pictureBackground.Click += new System.EventHandler(this.pictureBackground_Click);
            this.pictureBackground.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBackground_MouseMove);
            // 
            // frmSplash
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(640, 504);
            this.ControlBox = false;
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pictureBackground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.GradientColor1 = System.Drawing.Color.Black;
            this.GradientColor2 = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSplash";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSplash";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBackground)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBackground;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private MPfm.WindowsControls.Label lblStatus;
        private System.Windows.Forms.Timer timerUpdateGUI;
        private System.Windows.Forms.Timer timerFading;
        private MPfm.WindowsControls.Label lblVersion;
    }
}