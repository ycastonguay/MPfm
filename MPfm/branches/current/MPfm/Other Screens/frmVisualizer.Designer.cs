namespace MPfm
{
    partial class frmVisualizer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVisualizer));
            this.waveForm = new MPfm.WindowsControls.WaveForm();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.lblError = new MPfm.WindowsControls.Label();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.SuspendLayout();
            // 
            // waveForm
            // 
            this.waveForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.waveForm.BorderColor = System.Drawing.Color.Black;
            this.waveForm.BorderWidth = 1;
            this.waveForm.DisplayType = MPfm.WindowsControls.WaveFormDisplayType.Stereo;
            this.waveForm.DistortionThreshold = 0.9F;
            this.waveForm.GradientColor1 = System.Drawing.Color.Black;
            this.waveForm.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.waveForm.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.waveForm.Location = new System.Drawing.Point(1, 26);
            this.waveForm.Name = "waveForm";
            this.waveForm.Size = new System.Drawing.Size(683, 385);
            this.waveForm.TabIndex = 61;
            this.waveForm.WaveFormColor = System.Drawing.Color.Yellow;
            this.waveForm.WaveFormDistortionColor = System.Drawing.Color.Red;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // lblError
            // 
            this.lblError.AntiAliasingEnabled = true;
            this.lblError.BackColor = System.Drawing.Color.Transparent;
            this.lblError.CustomFontName = "Junction";
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.FontCollection = this.fontCollection;
            this.lblError.ForeColor = System.Drawing.Color.Black;
            this.lblError.Location = new System.Drawing.Point(-2, 1);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(383, 16);
            this.lblError.TabIndex = 79;
            this.lblError.Text = "Options here (TimerInterval, DisplayType, etc.)";
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
            // frmVisualizer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(683, 411);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.waveForm);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "frmVisualizer";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visualizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmVisualizer_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public MPfm.WindowsControls.WaveForm waveForm;
        private System.Windows.Forms.Timer timerUpdate;
        private MPfm.WindowsControls.Label lblError;
        private MPfm.WindowsControls.FontCollection fontCollection;

    }
}