namespace MPfm
{
    partial class frmRenameSavePlaylist
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
            MPfm.WindowsControls.EmbeddedFont customFont6 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont customFont7 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont customFont8 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont customFont9 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont customFont10 = new MPfm.WindowsControls.EmbeddedFont();
            this.lblName = new MPfm.WindowsControls.Label();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnOK = new MPfm.WindowsControls.Button();
            this.btnCancel = new MPfm.WindowsControls.Button();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AntiAliasingEnabled = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.CustomFontName = "Junction";            
            this.lblName.Location = new System.Drawing.Point(3, 3);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(89, 17);
            this.lblName.TabIndex = 66;
            this.lblName.Text = "Name :";
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
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(5, 23);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(321, 20);
            this.txtName.TabIndex = 6;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnOK.BorderWidth = 1;
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.CustomFontName = "Junction";
            this.btnOK.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnOK.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnOK.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnOK.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnOK.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.FontCollection = this.fontCollection;
            this.btnOK.FontColor = System.Drawing.Color.Black;
            this.btnOK.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnOK.GradientColor2 = System.Drawing.Color.Gray;
            this.btnOK.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnOK.Image = global::MPfm.Properties.Resources.accept;
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOK.Location = new System.Drawing.Point(163, 49);
            this.btnOK.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnOK.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnOK.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnOK.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 38);
            this.btnOK.TabIndex = 67;
            this.btnOK.Text = "OK";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnCancel.BorderWidth = 1;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.CustomFontName = "Junction";
            this.btnCancel.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnCancel.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnCancel.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnCancel.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.FontCollection = this.fontCollection;
            this.btnCancel.FontColor = System.Drawing.Color.Black;
            this.btnCancel.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnCancel.GradientColor2 = System.Drawing.Color.Gray;
            this.btnCancel.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnCancel.Image = global::MPfm.Properties.Resources.cancel;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancel.Location = new System.Drawing.Point(246, 49);
            this.btnCancel.MouseOverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnCancel.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnCancel.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnCancel.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 38);
            this.btnCancel.TabIndex = 68;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmRenameSavePlaylist
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(337, 116);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmRenameSavePlaylist";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Save Playlist As";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MPfm.WindowsControls.Label lblName;
        public System.Windows.Forms.TextBox txtName;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private MPfm.WindowsControls.Button btnOK;
        private MPfm.WindowsControls.Button btnCancel;
    }
}