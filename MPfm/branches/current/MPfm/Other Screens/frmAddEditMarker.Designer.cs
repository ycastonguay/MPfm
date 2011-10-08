namespace MPfm
{
    partial class frmAddEditMarker
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
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.panelEditMarker = new MPfm.WindowsControls.Panel();
            this.lblName = new MPfm.WindowsControls.Label();
            this.btnSave = new MPfm.WindowsControls.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtPosition = new System.Windows.Forms.MaskedTextBox();
            this.lblPosition = new MPfm.WindowsControls.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.panelEditMarker.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AntiAliasingEnabled = true;
            this.btnClose.BorderColor = System.Drawing.Color.Black;
            this.btnClose.BorderWidth = 1;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.CustomFontName = "Junction";
            this.btnClose.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnClose.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.FontCollection = this.fontCollection;
            this.btnClose.FontColor = System.Drawing.Color.Black;
            this.btnClose.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnClose.GradientColor2 = System.Drawing.Color.Gray;
            this.btnClose.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnClose.Image = global::MPfm.Properties.Resources.cancel;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnClose.Location = new System.Drawing.Point(528, 433);
            this.btnClose.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnClose.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnClose.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnClose.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 40);
            this.btnClose.TabIndex = 63;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // fontCollection
            // 
            customFont6.AssemblyPath = "MPfm.Fonts.dll";
            customFont6.Name = "LeagueGothic";
            customFont6.ResourceName = "MPfm.Fonts.LeagueGothic.ttf";
            customFont7.AssemblyPath = "MPfm.Fonts.dll";
            customFont7.Name = "Junction";
            customFont7.ResourceName = "MPfm.Fonts.Junction.ttf";
            customFont8.AssemblyPath = "MPfm.Fonts.dll";
            customFont8.Name = "Nobile";
            customFont8.ResourceName = "MPfm.Fonts.nobile.ttf";
            customFont9.AssemblyPath = "MPfm.Fonts.dll";
            customFont9.Name = "TitilliumText22L Lt";
            customFont9.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            customFont10.AssemblyPath = "MPfm.Fonts.dll";
            customFont10.Name = "Museo Sans 500";
            customFont10.ResourceName = "MPfm.Fonts.MuseoSans_500.ttf";
            this.fontCollection.Fonts.Add(customFont6);
            this.fontCollection.Fonts.Add(customFont7);
            this.fontCollection.Fonts.Add(customFont8);
            this.fontCollection.Fonts.Add(customFont9);
            this.fontCollection.Fonts.Add(customFont10);
            // 
            // panelEditMarker
            // 
            this.panelEditMarker.AntiAliasingEnabled = true;
            this.panelEditMarker.Controls.Add(this.numericUpDown1);
            this.panelEditMarker.Controls.Add(this.lblPosition);
            this.panelEditMarker.Controls.Add(this.txtPosition);
            this.panelEditMarker.Controls.Add(this.txtName);
            this.panelEditMarker.Controls.Add(this.lblName);
            this.panelEditMarker.Controls.Add(this.btnSave);
            this.panelEditMarker.Controls.Add(this.btnClose);
            this.panelEditMarker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEditMarker.ExpandedHeight = 200;
            this.panelEditMarker.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelEditMarker.FontCollection = this.fontCollection;
            this.panelEditMarker.GradientColor1 = System.Drawing.Color.Silver;
            this.panelEditMarker.GradientColor2 = System.Drawing.Color.Gray;
            this.panelEditMarker.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelEditMarker.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelEditMarker.HeaderExpandable = false;
            this.panelEditMarker.HeaderExpanded = true;
            this.panelEditMarker.HeaderForeColor = System.Drawing.Color.Black;
            this.panelEditMarker.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelEditMarker.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelEditMarker.HeaderHeight = 30;
            this.panelEditMarker.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelEditMarker.HeaderTitle = "Edit Marker";
            this.panelEditMarker.Location = new System.Drawing.Point(0, 0);
            this.panelEditMarker.Name = "panelEditMarker";
            this.panelEditMarker.Size = new System.Drawing.Size(632, 482);
            this.panelEditMarker.TabIndex = 65;
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblName.AntiAliasingEnabled = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.CustomFontName = "Junction";
            this.lblName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.FontCollection = this.fontCollection;
            this.lblName.Location = new System.Drawing.Point(3, 32);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(626, 17);
            this.lblName.TabIndex = 67;
            this.lblName.Text = "Name :";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AntiAliasingEnabled = true;
            this.btnSave.BorderColor = System.Drawing.Color.Black;
            this.btnSave.BorderWidth = 1;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.CustomFontName = "Junction";
            this.btnSave.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnSave.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnSave.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnSave.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.FontCollection = this.fontCollection;
            this.btnSave.FontColor = System.Drawing.Color.Black;
            this.btnSave.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnSave.GradientColor2 = System.Drawing.Color.Gray;
            this.btnSave.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnSave.Image = global::MPfm.Properties.Resources.disk;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSave.Location = new System.Drawing.Point(430, 433);
            this.btnSave.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnSave.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnSave.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnSave.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 40);
            this.btnSave.TabIndex = 65;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(6, 52);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(618, 21);
            this.txtName.TabIndex = 68;
            // 
            // txtPosition
            // 
            this.txtPosition.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPosition.Location = new System.Drawing.Point(6, 98);
            this.txtPosition.Mask = "00:00:00";
            this.txtPosition.Name = "txtPosition";
            this.txtPosition.Size = new System.Drawing.Size(100, 21);
            this.txtPosition.TabIndex = 69;
            // 
            // lblPosition
            // 
            this.lblPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPosition.AntiAliasingEnabled = true;
            this.lblPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblPosition.CustomFontName = "Junction";
            this.lblPosition.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosition.FontCollection = this.fontCollection;
            this.lblPosition.Location = new System.Drawing.Point(3, 78);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(626, 17);
            this.lblPosition.TabIndex = 70;
            this.lblPosition.Text = "Position :";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Location = new System.Drawing.Point(6, 125);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(41, 22);
            this.numericUpDown1.TabIndex = 71;
            // 
            // frmAddEditMarker
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(632, 482);
            this.ControlBox = false;
            this.Controls.Add(this.panelEditMarker);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "frmAddEditMarker";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Marker";
            this.panelEditMarker.ResumeLayout(false);
            this.panelEditMarker.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MPfm.WindowsControls.Button btnClose;
        private MPfm.WindowsControls.Panel panelEditMarker;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private WindowsControls.Button btnSave;
        private WindowsControls.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private WindowsControls.Label lblPosition;
        private System.Windows.Forms.MaskedTextBox txtPosition;
        private System.Windows.Forms.NumericUpDown numericUpDown1;        
    }
}