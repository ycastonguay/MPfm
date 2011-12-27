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
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomFontEditorForm));
            this.radioUseStandardFont = new System.Windows.Forms.RadioButton();
            this.radioUseCustomFont = new System.Windows.Forms.RadioButton();
            this.comboStandardFontName = new System.Windows.Forms.ComboBox();
            this.comboCustomFontName = new System.Windows.Forms.ComboBox();
            this.lblPreview = new MPfm.WindowsControls.Label();
            this.chkIsUnderline = new System.Windows.Forms.CheckBox();
            this.chkIsItalic = new System.Windows.Forms.CheckBox();
            this.chkIsBold = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblFontSize = new System.Windows.Forms.Label();
            this.trackFontSize = new System.Windows.Forms.TrackBar();
            this.groupPreview = new System.Windows.Forms.GroupBox();
            this.groupFontStyle = new System.Windows.Forms.GroupBox();
            this.groupFont = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackFontSize)).BeginInit();
            this.groupPreview.SuspendLayout();
            this.groupFontStyle.SuspendLayout();
            this.groupFont.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioUseStandardFont
            // 
            this.radioUseStandardFont.AutoSize = true;
            this.radioUseStandardFont.BackColor = System.Drawing.Color.Transparent;
            this.radioUseStandardFont.Location = new System.Drawing.Point(141, 19);
            this.radioUseStandardFont.Name = "radioUseStandardFont";
            this.radioUseStandardFont.Size = new System.Drawing.Size(109, 17);
            this.radioUseStandardFont.TabIndex = 26;
            this.radioUseStandardFont.Text = "Use standard font";
            this.radioUseStandardFont.UseVisualStyleBackColor = false;
            this.radioUseStandardFont.CheckedChanged += new System.EventHandler(this.radioUseStandardFont_CheckedChanged);
            // 
            // radioUseCustomFont
            // 
            this.radioUseCustomFont.AutoSize = true;
            this.radioUseCustomFont.BackColor = System.Drawing.Color.Transparent;
            this.radioUseCustomFont.Checked = true;
            this.radioUseCustomFont.Location = new System.Drawing.Point(9, 19);
            this.radioUseCustomFont.Name = "radioUseCustomFont";
            this.radioUseCustomFont.Size = new System.Drawing.Size(102, 17);
            this.radioUseCustomFont.TabIndex = 25;
            this.radioUseCustomFont.TabStop = true;
            this.radioUseCustomFont.Text = "Use custom font";
            this.radioUseCustomFont.UseVisualStyleBackColor = false;
            this.radioUseCustomFont.CheckedChanged += new System.EventHandler(this.radioUseCustomFont_CheckedChanged);
            // 
            // comboStandardFontName
            // 
            this.comboStandardFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStandardFontName.Enabled = false;
            this.comboStandardFontName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboStandardFontName.FormattingEnabled = true;
            this.comboStandardFontName.Location = new System.Drawing.Point(141, 38);
            this.comboStandardFontName.Name = "comboStandardFontName";
            this.comboStandardFontName.Size = new System.Drawing.Size(121, 22);
            this.comboStandardFontName.TabIndex = 22;
            this.comboStandardFontName.SelectedIndexChanged += new System.EventHandler(this.comboStandardFontName_SelectedIndexChanged);
            // 
            // comboCustomFontName
            // 
            this.comboCustomFontName.DisplayMember = "Name";
            this.comboCustomFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCustomFontName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboCustomFontName.FormattingEnabled = true;
            this.comboCustomFontName.Location = new System.Drawing.Point(9, 38);
            this.comboCustomFontName.Name = "comboCustomFontName";
            this.comboCustomFontName.Size = new System.Drawing.Size(121, 22);
            this.comboCustomFontName.TabIndex = 23;
            this.comboCustomFontName.ValueMember = "ResourceName";
            this.comboCustomFontName.SelectedIndexChanged += new System.EventHandler(this.comboCustomFontName_SelectedIndexChanged);
            // 
            // lblPreview
            //             
            this.lblPreview.BackColor = System.Drawing.Color.Transparent;
            customFont1.EmbeddedFontName = "";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 8;
            customFont1.StandardFontName = "Arial";
            customFont1.UseEmbeddedFont = false;
            this.lblPreview.CustomFont = customFont1;            
            this.lblPreview.Location = new System.Drawing.Point(6, 20);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(435, 44);
            this.lblPreview.TabIndex = 28;
            this.lblPreview.Text = "The quick brown fox jumps over the lazy dog.";
            // 
            // chkIsUnderline
            // 
            this.chkIsUnderline.AutoSize = true;
            this.chkIsUnderline.BackColor = System.Drawing.Color.Transparent;
            this.chkIsUnderline.Location = new System.Drawing.Point(164, 20);
            this.chkIsUnderline.Name = "chkIsUnderline";
            this.chkIsUnderline.Size = new System.Drawing.Size(71, 17);
            this.chkIsUnderline.TabIndex = 69;
            this.chkIsUnderline.Text = "Underline";
            this.chkIsUnderline.UseVisualStyleBackColor = false;
            this.chkIsUnderline.CheckedChanged += new System.EventHandler(this.chkIsUnderline_CheckedChanged);
            // 
            // chkIsItalic
            // 
            this.chkIsItalic.AutoSize = true;
            this.chkIsItalic.BackColor = System.Drawing.Color.Transparent;
            this.chkIsItalic.Location = new System.Drawing.Point(88, 20);
            this.chkIsItalic.Name = "chkIsItalic";
            this.chkIsItalic.Size = new System.Drawing.Size(48, 17);
            this.chkIsItalic.TabIndex = 67;
            this.chkIsItalic.Text = "Italic";
            this.chkIsItalic.UseVisualStyleBackColor = false;
            this.chkIsItalic.CheckedChanged += new System.EventHandler(this.chkIsItalic_CheckedChanged);
            // 
            // chkIsBold
            // 
            this.chkIsBold.AutoSize = true;
            this.chkIsBold.BackColor = System.Drawing.Color.Transparent;
            this.chkIsBold.Location = new System.Drawing.Point(15, 20);
            this.chkIsBold.Name = "chkIsBold";
            this.chkIsBold.Size = new System.Drawing.Size(47, 17);
            this.chkIsBold.TabIndex = 65;
            this.chkIsBold.Text = "Bold";
            this.chkIsBold.UseVisualStyleBackColor = false;
            this.chkIsBold.CheckedChanged += new System.EventHandler(this.chkIsBold_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(376, 210);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 23);
            this.btnCancel.TabIndex = 71;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblFontSize
            // 
            this.lblFontSize.AutoSize = true;
            this.lblFontSize.Location = new System.Drawing.Point(266, 19);
            this.lblFontSize.Name = "lblFontSize";
            this.lblFontSize.Size = new System.Drawing.Size(75, 13);
            this.lblFontSize.TabIndex = 73;
            this.lblFontSize.Text = "Font Size: 8 pt";
            // 
            // trackFontSize
            // 
            this.trackFontSize.Location = new System.Drawing.Point(268, 38);
            this.trackFontSize.Maximum = 24;
            this.trackFontSize.Minimum = 6;
            this.trackFontSize.Name = "trackFontSize";
            this.trackFontSize.Size = new System.Drawing.Size(104, 45);
            this.trackFontSize.TabIndex = 74;
            this.trackFontSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackFontSize.Value = 8;
            this.trackFontSize.Scroll += new System.EventHandler(this.trackFontSize_Scroll);
            // 
            // groupPreview
            // 
            this.groupPreview.Controls.Add(this.lblPreview);
            this.groupPreview.Location = new System.Drawing.Point(3, 3);
            this.groupPreview.Name = "groupPreview";
            this.groupPreview.Size = new System.Drawing.Size(440, 60);
            this.groupPreview.TabIndex = 75;
            this.groupPreview.TabStop = false;
            this.groupPreview.Text = "Preview";
            // 
            // groupFontStyle
            // 
            this.groupFontStyle.Controls.Add(this.chkIsBold);
            this.groupFontStyle.Controls.Add(this.chkIsItalic);
            this.groupFontStyle.Controls.Add(this.chkIsUnderline);
            this.groupFontStyle.Location = new System.Drawing.Point(4, 145);
            this.groupFontStyle.Name = "groupFontStyle";
            this.groupFontStyle.Size = new System.Drawing.Size(440, 60);
            this.groupFontStyle.TabIndex = 76;
            this.groupFontStyle.TabStop = false;
            this.groupFontStyle.Text = "Font Style";
            // 
            // groupFont
            // 
            this.groupFont.Controls.Add(this.radioUseCustomFont);
            this.groupFont.Controls.Add(this.comboCustomFontName);
            this.groupFont.Controls.Add(this.comboStandardFontName);
            this.groupFont.Controls.Add(this.trackFontSize);
            this.groupFont.Controls.Add(this.radioUseStandardFont);
            this.groupFont.Controls.Add(this.lblFontSize);
            this.groupFont.Location = new System.Drawing.Point(3, 69);
            this.groupFont.Name = "groupFont";
            this.groupFont.Size = new System.Drawing.Size(440, 70);
            this.groupFont.TabIndex = 77;
            this.groupFont.TabStop = false;
            this.groupFont.Text = "Font";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(303, 210);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(67, 23);
            this.btnOK.TabIndex = 78;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // CustomFontEditorForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(449, 239);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupFont);
            this.Controls.Add(this.groupFontStyle);
            this.Controls.Add(this.groupPreview);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomFontEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Font";
            this.Load += new System.EventHandler(this.CustomFontEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackFontSize)).EndInit();
            this.groupPreview.ResumeLayout(false);
            this.groupFontStyle.ResumeLayout(false);
            this.groupFontStyle.PerformLayout();
            this.groupFont.ResumeLayout(false);
            this.groupFont.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioUseStandardFont;
        private System.Windows.Forms.RadioButton radioUseCustomFont;
        private System.Windows.Forms.ComboBox comboStandardFontName;
        private System.Windows.Forms.ComboBox comboCustomFontName;
        private Label lblPreview;
        private System.Windows.Forms.CheckBox chkIsUnderline;
        private System.Windows.Forms.CheckBox chkIsItalic;
        private System.Windows.Forms.CheckBox chkIsBold;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblFontSize;
        private System.Windows.Forms.TrackBar trackFontSize;
        private System.Windows.Forms.GroupBox groupPreview;
        private System.Windows.Forms.GroupBox groupFontStyle;
        private System.Windows.Forms.GroupBox groupFont;
        private System.Windows.Forms.Button btnOK;
    }
}