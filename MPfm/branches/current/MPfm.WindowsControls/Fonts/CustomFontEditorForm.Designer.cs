namespace MPfm.WindowsControls
{
    partial class CustomFontEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomFontEditorForm));
            this.lblUseStandardFont = new MPfm.WindowsControls.Label();
            this.radioUseStandardFont = new System.Windows.Forms.RadioButton();
            this.radioUseCustomFont = new System.Windows.Forms.RadioButton();
            this.lblUseCustomFont = new MPfm.WindowsControls.Label();
            this.lblFontSize = new MPfm.WindowsControls.Label();
            this.trackFontSize = new MPfm.WindowsControls.TrackBar();
            this.comboStandardFontName = new System.Windows.Forms.ComboBox();
            this.comboCustomFontName = new System.Windows.Forms.ComboBox();
            this.lblPreview = new MPfm.WindowsControls.Label();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.panelBackground = new MPfm.WindowsControls.Panel();
            this.label4 = new MPfm.WindowsControls.Label();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label3 = new MPfm.WindowsControls.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label2 = new MPfm.WindowsControls.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new MPfm.WindowsControls.Label();
            this.panelBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUseStandardFont
            // 
            this.lblUseStandardFont.AntiAliasingEnabled = true;
            this.lblUseStandardFont.BackColor = System.Drawing.Color.Transparent;
            this.lblUseStandardFont.CustomFontName = "Junction";
            this.lblUseStandardFont.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUseStandardFont.FontCollection = null;
            this.lblUseStandardFont.Location = new System.Drawing.Point(151, 64);
            this.lblUseStandardFont.Name = "lblUseStandardFont";
            this.lblUseStandardFont.Size = new System.Drawing.Size(107, 18);
            this.lblUseStandardFont.TabIndex = 27;
            this.lblUseStandardFont.Text = "Use Standard Font";
            this.lblUseStandardFont.Click += new System.EventHandler(this.lblUseStandardFont_Click);
            // 
            // radioUseStandardFont
            // 
            this.radioUseStandardFont.AutoSize = true;
            this.radioUseStandardFont.BackColor = System.Drawing.Color.Transparent;
            this.radioUseStandardFont.Location = new System.Drawing.Point(138, 66);
            this.radioUseStandardFont.Name = "radioUseStandardFont";
            this.radioUseStandardFont.Size = new System.Drawing.Size(14, 13);
            this.radioUseStandardFont.TabIndex = 26;
            this.radioUseStandardFont.UseVisualStyleBackColor = false;
            this.radioUseStandardFont.CheckedChanged += new System.EventHandler(this.radioUseStandardFont_CheckedChanged);
            // 
            // radioUseCustomFont
            // 
            this.radioUseCustomFont.AutoSize = true;
            this.radioUseCustomFont.BackColor = System.Drawing.Color.Transparent;
            this.radioUseCustomFont.Checked = true;
            this.radioUseCustomFont.Location = new System.Drawing.Point(6, 66);
            this.radioUseCustomFont.Name = "radioUseCustomFont";
            this.radioUseCustomFont.Size = new System.Drawing.Size(14, 13);
            this.radioUseCustomFont.TabIndex = 25;
            this.radioUseCustomFont.TabStop = true;
            this.radioUseCustomFont.UseVisualStyleBackColor = false;
            this.radioUseCustomFont.CheckedChanged += new System.EventHandler(this.radioUseCustomFont_CheckedChanged);
            // 
            // lblUseCustomFont
            // 
            this.lblUseCustomFont.AntiAliasingEnabled = true;
            this.lblUseCustomFont.BackColor = System.Drawing.Color.Transparent;
            this.lblUseCustomFont.CustomFontName = "Junction";
            this.lblUseCustomFont.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUseCustomFont.FontCollection = null;
            this.lblUseCustomFont.Location = new System.Drawing.Point(19, 64);
            this.lblUseCustomFont.Name = "lblUseCustomFont";
            this.lblUseCustomFont.Size = new System.Drawing.Size(100, 18);
            this.lblUseCustomFont.TabIndex = 24;
            this.lblUseCustomFont.Text = "Use Custom Font";
            this.lblUseCustomFont.Click += new System.EventHandler(this.lblUseCustomFont_Click);
            // 
            // lblFontSize
            // 
            this.lblFontSize.AntiAliasingEnabled = true;
            this.lblFontSize.BackColor = System.Drawing.Color.Transparent;
            this.lblFontSize.CustomFontName = "Junction";
            this.lblFontSize.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFontSize.FontCollection = null;
            this.lblFontSize.Location = new System.Drawing.Point(263, 64);
            this.lblFontSize.Name = "lblFontSize";
            this.lblFontSize.Size = new System.Drawing.Size(100, 18);
            this.lblFontSize.TabIndex = 21;
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
            this.trackFontSize.Location = new System.Drawing.Point(266, 85);
            this.trackFontSize.Maximum = 24;
            this.trackFontSize.Minimum = 6;
            this.trackFontSize.Name = "trackFontSize";
            this.trackFontSize.Size = new System.Drawing.Size(105, 21);
            this.trackFontSize.TabIndex = 20;
            this.trackFontSize.Text = "trackBar";
            this.trackFontSize.Value = 8;
            this.trackFontSize.OnTrackBarValueChanged += new MPfm.WindowsControls.TrackBar.TrackBarValueChanged(this.trackFontSize_OnTrackBarValueChanged);
            // 
            // comboStandardFontName
            // 
            this.comboStandardFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStandardFontName.Enabled = false;
            this.comboStandardFontName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboStandardFontName.FormattingEnabled = true;
            this.comboStandardFontName.Location = new System.Drawing.Point(138, 85);
            this.comboStandardFontName.Name = "comboStandardFontName";
            this.comboStandardFontName.Size = new System.Drawing.Size(121, 22);
            this.comboStandardFontName.TabIndex = 22;
            this.comboStandardFontName.SelectedIndexChanged += new System.EventHandler(this.comboStandardFontName_SelectedIndexChanged);
            // 
            // comboCustomFontName
            // 
            this.comboCustomFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCustomFontName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboCustomFontName.FormattingEnabled = true;
            this.comboCustomFontName.Location = new System.Drawing.Point(6, 85);
            this.comboCustomFontName.Name = "comboCustomFontName";
            this.comboCustomFontName.Size = new System.Drawing.Size(121, 22);
            this.comboCustomFontName.TabIndex = 23;
            this.comboCustomFontName.SelectedIndexChanged += new System.EventHandler(this.comboCustomFontName_SelectedIndexChanged);
            // 
            // lblPreview
            // 
            this.lblPreview.AntiAliasingEnabled = true;
            this.lblPreview.BackColor = System.Drawing.Color.Transparent;
            this.lblPreview.CustomFontName = "Junction";
            this.lblPreview.FontCollection = null;
            this.lblPreview.Location = new System.Drawing.Point(3, 19);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(435, 33);
            this.lblPreview.TabIndex = 28;
            this.lblPreview.Text = "The quick brown fox jumps over the lazy dog.";
            // 
            // btnClose
            // 
            this.btnClose.AntiAliasingEnabled = true;
            this.btnClose.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnClose.BorderWidth = 1;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.CustomFontName = "Junction";
            this.btnClose.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.FontCollection = null;
            this.btnClose.FontColor = System.Drawing.Color.Black;
            this.btnClose.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnClose.GradientColor2 = System.Drawing.Color.Gray;
            this.btnClose.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnClose.Location = new System.Drawing.Point(377, 85);
            this.btnClose.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnClose.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnClose.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnClose.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(63, 22);
            this.btnClose.TabIndex = 63;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelBackground
            // 
            this.panelBackground.AntiAliasingEnabled = true;
            this.panelBackground.Controls.Add(this.label4);
            this.panelBackground.Controls.Add(this.checkBox3);
            this.panelBackground.Controls.Add(this.label3);
            this.panelBackground.Controls.Add(this.checkBox2);
            this.panelBackground.Controls.Add(this.label2);
            this.panelBackground.Controls.Add(this.checkBox1);
            this.panelBackground.Controls.Add(this.label1);
            this.panelBackground.Controls.Add(this.btnClose);
            this.panelBackground.Controls.Add(this.lblPreview);
            this.panelBackground.Controls.Add(this.comboCustomFontName);
            this.panelBackground.Controls.Add(this.lblUseStandardFont);
            this.panelBackground.Controls.Add(this.comboStandardFontName);
            this.panelBackground.Controls.Add(this.radioUseStandardFont);
            this.panelBackground.Controls.Add(this.trackFontSize);
            this.panelBackground.Controls.Add(this.radioUseCustomFont);
            this.panelBackground.Controls.Add(this.lblFontSize);
            this.panelBackground.Controls.Add(this.lblUseCustomFont);
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.ExpandedHeight = 200;
            this.panelBackground.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelBackground.FontCollection = null;
            this.panelBackground.GradientColor1 = System.Drawing.Color.Silver;
            this.panelBackground.GradientColor2 = System.Drawing.Color.Gray;
            this.panelBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelBackground.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelBackground.HeaderExpandable = false;
            this.panelBackground.HeaderExpanded = true;
            this.panelBackground.HeaderForeColor = System.Drawing.Color.Black;
            this.panelBackground.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelBackground.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelBackground.HeaderHeight = 0;
            this.panelBackground.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(446, 135);
            this.panelBackground.TabIndex = 64;
            // 
            // label4
            // 
            this.label4.AntiAliasingEnabled = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.CustomFontName = "Junction";
            this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.FontCollection = null;
            this.label4.Location = new System.Drawing.Point(3, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 70;
            this.label4.Text = "Preview:";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.BackColor = System.Drawing.Color.Transparent;
            this.checkBox3.Location = new System.Drawing.Point(154, 113);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(15, 14);
            this.checkBox3.TabIndex = 69;
            this.checkBox3.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AntiAliasingEnabled = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.CustomFontName = "Junction";
            this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.FontCollection = null;
            this.label3.Location = new System.Drawing.Point(169, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 18);
            this.label3.TabIndex = 68;
            this.label3.Text = "Underline";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.BackColor = System.Drawing.Color.Transparent;
            this.checkBox2.Location = new System.Drawing.Point(78, 113);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(15, 14);
            this.checkBox2.TabIndex = 67;
            this.checkBox2.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AntiAliasingEnabled = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.CustomFontName = "Junction";
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.FontCollection = null;
            this.label2.Location = new System.Drawing.Point(93, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 18);
            this.label2.TabIndex = 66;
            this.label2.Text = "Italic";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.Location = new System.Drawing.Point(5, 113);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 65;
            this.checkBox1.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AntiAliasingEnabled = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.CustomFontName = "Junction";
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.FontCollection = null;
            this.label1.Location = new System.Drawing.Point(20, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 18);
            this.label1.TabIndex = 64;
            this.label1.Text = "Bold";
            // 
            // FontEditorForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(446, 135);
            this.Controls.Add(this.panelBackground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FontEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Font";
            this.Load += new System.EventHandler(this.FontEditorForm_Load);
            this.panelBackground.ResumeLayout(false);
            this.panelBackground.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Label lblUseStandardFont;
        private System.Windows.Forms.RadioButton radioUseStandardFont;
        private System.Windows.Forms.RadioButton radioUseCustomFont;
        private Label lblUseCustomFont;
        private Label lblFontSize;
        private TrackBar trackFontSize;
        private System.Windows.Forms.ComboBox comboStandardFontName;
        private System.Windows.Forms.ComboBox comboCustomFontName;
        private Label lblPreview;
        private Button btnClose;
        private Panel panelBackground;
        private System.Windows.Forms.CheckBox checkBox3;
        private Label label3;
        private System.Windows.Forms.CheckBox checkBox2;
        private Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private Label label1;
        private Label label4;
    }
}