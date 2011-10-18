namespace MPfm
{
    partial class frmEditSongMetadata
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
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            this.propertyGridTags = new System.Windows.Forms.PropertyGrid();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.panelEditSongMetadata = new MPfm.WindowsControls.Panel();
            this.lblEditing = new MPfm.WindowsControls.Label();
            this.btnSave = new MPfm.WindowsControls.Button();
            this.panelEditSongMetadata.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGridTags
            // 
            this.propertyGridTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridTags.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGridTags.Location = new System.Drawing.Point(0, 52);
            this.propertyGridTags.Name = "propertyGridTags";
            this.propertyGridTags.Size = new System.Drawing.Size(632, 372);
            this.propertyGridTags.TabIndex = 64;
            this.propertyGridTags.ToolbarVisible = false;
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
            // panelEditSongMetadata
            // 
            this.panelEditSongMetadata.AntiAliasingEnabled = true;
            this.panelEditSongMetadata.Controls.Add(this.lblEditing);
            this.panelEditSongMetadata.Controls.Add(this.btnSave);
            this.panelEditSongMetadata.Controls.Add(this.btnClose);
            this.panelEditSongMetadata.Controls.Add(this.propertyGridTags);
            this.panelEditSongMetadata.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEditSongMetadata.ExpandedHeight = 200;
            this.panelEditSongMetadata.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelEditSongMetadata.FontCollection = this.fontCollection;
            this.panelEditSongMetadata.GradientColor1 = System.Drawing.Color.Silver;
            this.panelEditSongMetadata.GradientColor2 = System.Drawing.Color.Gray;
            this.panelEditSongMetadata.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelEditSongMetadata.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelEditSongMetadata.HeaderExpandable = false;
            this.panelEditSongMetadata.HeaderExpanded = true;
            this.panelEditSongMetadata.HeaderForeColor = System.Drawing.Color.Black;
            this.panelEditSongMetadata.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelEditSongMetadata.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelEditSongMetadata.HeaderHeight = 30;
            this.panelEditSongMetadata.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelEditSongMetadata.HeaderTitle = "Edit Song Metadata";
            this.panelEditSongMetadata.Location = new System.Drawing.Point(0, 0);
            this.panelEditSongMetadata.Name = "panelEditSongMetadata";
            this.panelEditSongMetadata.Size = new System.Drawing.Size(632, 482);
            this.panelEditSongMetadata.TabIndex = 65;
            // 
            // lblEditing
            // 
            this.lblEditing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEditing.AntiAliasingEnabled = true;
            this.lblEditing.BackColor = System.Drawing.Color.Transparent;
            this.lblEditing.CustomFontName = "Junction";
            this.lblEditing.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditing.FontCollection = this.fontCollection;
            this.lblEditing.Location = new System.Drawing.Point(3, 32);
            this.lblEditing.Name = "lblEditing";
            this.lblEditing.Size = new System.Drawing.Size(626, 17);
            this.lblEditing.TabIndex = 66;
            this.lblEditing.Text = "Editing";
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
            // frmEditSongMetadata
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(632, 482);
            this.ControlBox = false;
            this.Controls.Add(this.panelEditSongMetadata);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "frmEditSongMetadata";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Song Metadata";
            this.panelEditSongMetadata.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MPfm.WindowsControls.Button btnClose;
        public System.Windows.Forms.PropertyGrid propertyGridTags;
        private MPfm.WindowsControls.Panel panelEditSongMetadata;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private WindowsControls.Button btnSave;
        private WindowsControls.Label lblEditing;        
    }
}