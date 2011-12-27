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
            MPfm.WindowsControls.EmbeddedFont embeddedFont1 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont embeddedFont2 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont embeddedFont3 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
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
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            customFont1.EmbeddedFontName = "TitilliumText22L Lt";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 9;
            customFont1.StandardFontName = "Arial";
            customFont1.UseEmbeddedFont = true;
            this.lblStatus.CustomFont = customFont1;
            this.lblStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(5, 481);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(608, 22);
            this.lblStatus.TabIndex = 50;
            this.lblStatus.Text = "Initializing tracing...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fontCollection
            // 
            embeddedFont1.AssemblyPath = "MPfm.Fonts.dll";
            embeddedFont1.Name = "Junction";
            embeddedFont1.ResourceName = "MPfm.Fonts.Junction.ttf";
            embeddedFont2.AssemblyPath = "MPfm.Fonts.dll";
            embeddedFont2.Name = "TitilliumText22L Lt";
            embeddedFont2.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            embeddedFont3.AssemblyPath = "MPfm.Fonts.dll";
            embeddedFont3.Name = "Droid Sans Mono";
            embeddedFont3.ResourceName = "MPfm.Fonts.DroidSansMono.ttf";
            this.fontCollection.Fonts.Add(embeddedFont1);
            this.fontCollection.Fonts.Add(embeddedFont2);
            this.fontCollection.Fonts.Add(embeddedFont3);
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            customFont2.EmbeddedFontName = "Droid Sans Mono";
            customFont2.IsBold = false;
            customFont2.IsItalic = false;
            customFont2.IsUnderline = false;
            customFont2.Size = 9;
            customFont2.StandardFontName = "Arial";
            customFont2.UseEmbeddedFont = true;
            this.lblVersion.CustomFont = customFont2;
            this.lblVersion.Font = new System.Drawing.Font("Arial", 9F);
            this.lblVersion.ForeColor = System.Drawing.Color.White;
            this.lblVersion.Location = new System.Drawing.Point(7, 8);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(62, 16);
            this.lblVersion.TabIndex = 55;
            this.lblVersion.Text = "0.0.0.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBackground
            // 
            this.pictureBackground.Image = global::MPfm.Properties.Resources.MPFM_Splash_0_5_0_3;
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