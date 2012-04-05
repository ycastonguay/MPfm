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
            this.components = new System.ComponentModel.Container();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            this.propertyGridTags = new System.Windows.Forms.PropertyGrid();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.panelEditSongMetadata = new MPfm.WindowsControls.Panel();
            this.lblEditing = new MPfm.WindowsControls.Label();
            this.btnSave = new MPfm.WindowsControls.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelEditSongMetadata.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGridTags
            // 
            this.propertyGridTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridTags.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGridTags.Location = new System.Drawing.Point(0, 52);
            this.propertyGridTags.Name = "propertyGridTags";
            this.propertyGridTags.Size = new System.Drawing.Size(632, 372);
            this.propertyGridTags.TabIndex = 64;
            this.propertyGridTags.ToolbarVisible = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            customFont1.EmbeddedFontName = "Junction";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 8F;
            customFont1.StandardFontName = "Arial";
            customFont1.UseAntiAliasing = true;
            customFont1.UseEmbeddedFont = true;
            this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Image = global::MPfm.Properties.Resources.cancel;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnClose.Location = new System.Drawing.Point(530, 433);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 40);
            this.btnClose.TabIndex = 63;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.btnClose, "Closes the window.");
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelEditSongMetadata
            // 
            this.panelEditSongMetadata.Controls.Add(this.lblEditing);
            this.panelEditSongMetadata.Controls.Add(this.btnSave);
            this.panelEditSongMetadata.Controls.Add(this.btnClose);
            this.panelEditSongMetadata.Controls.Add(this.propertyGridTags);
            this.panelEditSongMetadata.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEditSongMetadata.ExpandedHeight = 200;
            this.panelEditSongMetadata.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelEditSongMetadata.HeaderExpandable = false;
            this.panelEditSongMetadata.HeaderExpanded = true;
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
            this.lblEditing.BackColor = System.Drawing.Color.Transparent;
            this.lblEditing.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditing.Location = new System.Drawing.Point(3, 32);
            this.lblEditing.Name = "lblEditing";
            this.lblEditing.Size = new System.Drawing.Size(626, 17);
            this.lblEditing.TabIndex = 66;
            this.lblEditing.Text = "Editing";            
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Image = global::MPfm.Properties.Resources.disk;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSave.Location = new System.Drawing.Point(432, 433);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 40);
            this.btnSave.TabIndex = 65;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip.SetToolTip(this.btnSave, "Saves the song metadata.");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolTip
            // 
            this.toolTip.BackColor = System.Drawing.Color.DimGray;
            this.toolTip.ForeColor = System.Drawing.Color.White;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip.ToolTipTitle = "Edit Song Metadata";
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
        private WindowsControls.Button btnSave;
        private WindowsControls.Label lblEditing;
        public System.Windows.Forms.ToolTip toolTip;        
    }
}