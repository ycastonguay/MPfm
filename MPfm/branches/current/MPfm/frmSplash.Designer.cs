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
            MPfm.WindowsControls.LabelTheme labelTheme1 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient1 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme2 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient2 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSplash));
            this.timerUpdateGUI = new System.Windows.Forms.Timer(this.components);
            this.timerFading = new System.Windows.Forms.Timer(this.components);
            this.lblStatus = new MPfm.WindowsControls.Label();
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
            this.lblStatus.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(5, 481);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(608, 22);
            this.lblStatus.TabIndex = 50;
            this.lblStatus.Text = "Initializing tracing...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme1.IsBackgroundTransparent = true;
            textGradient1.BorderColor = System.Drawing.Color.DarkGray;
            textGradient1.BorderWidth = 1;
            textGradient1.Color1 = System.Drawing.Color.LightGray;
            textGradient1.Color2 = System.Drawing.Color.Gray;
            customFont1.Color = System.Drawing.Color.Black;
            customFont1.EmbeddedFontName = "Junction";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 8F;
            customFont1.StandardFontName = "Arial";
            customFont1.UseAntiAliasing = true;
            customFont1.UseEmbeddedFont = true;
            textGradient1.Font = customFont1;
            textGradient1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            labelTheme1.TextGradient = textGradient1;
            this.lblStatus.Theme = labelTheme1;
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Arial", 9F);
            this.lblVersion.ForeColor = System.Drawing.Color.White;
            this.lblVersion.Location = new System.Drawing.Point(7, 8);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(62, 16);
            this.lblVersion.TabIndex = 55;
            this.lblVersion.Text = "0.0.0.0";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme2.IsBackgroundTransparent = true;
            textGradient2.BorderColor = System.Drawing.Color.DarkGray;
            textGradient2.BorderWidth = 1;
            textGradient2.Color1 = System.Drawing.Color.LightGray;
            textGradient2.Color2 = System.Drawing.Color.Gray;
            customFont2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            customFont2.EmbeddedFontName = "Droid Sans Mono";
            customFont2.IsBold = false;
            customFont2.IsItalic = false;
            customFont2.IsUnderline = false;
            customFont2.Size = 9F;
            customFont2.StandardFontName = "Arial";
            customFont2.UseAntiAliasing = true;
            customFont2.UseEmbeddedFont = true;
            textGradient2.Font = customFont2;
            textGradient2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            labelTheme2.TextGradient = textGradient2;
            this.lblVersion.Theme = labelTheme2;
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
        private MPfm.WindowsControls.Label lblStatus;
        private System.Windows.Forms.Timer timerUpdateGUI;
        private System.Windows.Forms.Timer timerFading;
        private MPfm.WindowsControls.Label lblVersion;
    }
}