// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

namespace MPfm.Windows.Classes.Forms
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
            MPfm.WindowsControls.FlowLayoutPanelTheme flowLayoutPanelTheme1 = new MPfm.WindowsControls.FlowLayoutPanelTheme();
            MPfm.WindowsControls.BackgroundGradient backgroundGradient1 = new MPfm.WindowsControls.BackgroundGradient();
            MPfm.WindowsControls.ButtonTheme buttonTheme1 = new MPfm.WindowsControls.ButtonTheme();
            MPfm.WindowsControls.TextGradient textGradient1 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient2 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient3 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.ButtonTheme buttonTheme2 = new MPfm.WindowsControls.ButtonTheme();
            MPfm.WindowsControls.TextGradient textGradient4 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient5 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient6 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.PanelTheme panelTheme1 = new MPfm.WindowsControls.PanelTheme();
            MPfm.WindowsControls.BackgroundGradient backgroundGradient2 = new MPfm.WindowsControls.BackgroundGradient();
            MPfm.WindowsControls.TextGradient textGradient8 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme1 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient7 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowToolbar = new MPfm.WindowsControls.FlowLayoutPanel();
            this.btnSave = new MPfm.WindowsControls.Button();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.panelEditSongMetadata = new MPfm.WindowsControls.Panel();
            this.lblEditing = new MPfm.WindowsControls.Label();
            this.propertyGridTags = new System.Windows.Forms.PropertyGrid();
            this.flowToolbar.SuspendLayout();
            this.panelEditSongMetadata.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.BackColor = System.Drawing.Color.DimGray;
            this.toolTip.ForeColor = System.Drawing.Color.White;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip.ToolTipTitle = "Edit Song Metadata";
            // 
            // flowToolbar
            // 
            this.flowToolbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowToolbar.AutoSize = true;
            this.flowToolbar.Controls.Add(this.btnSave);
            this.flowToolbar.Controls.Add(this.btnClose);
            this.flowToolbar.Location = new System.Drawing.Point(0, 0);
            this.flowToolbar.Margin = new System.Windows.Forms.Padding(0);
            this.flowToolbar.Name = "flowToolbar";
            this.flowToolbar.Size = new System.Drawing.Size(633, 26);
            this.flowToolbar.TabIndex = 114;
            backgroundGradient1.BorderColor = System.Drawing.Color.DarkGray;
            backgroundGradient1.BorderWidth = 0;
            backgroundGradient1.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            backgroundGradient1.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            backgroundGradient1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            flowLayoutPanelTheme1.BackgroundGradient = backgroundGradient1;
            flowLayoutPanelTheme1.IsBackgroundTransparent = false;
            this.flowToolbar.Theme = flowLayoutPanelTheme1;
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Image = global::MPfm.Windows.Properties.Resources.disk;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.IsAutoSized = true;
            this.btnSave.Location = new System.Drawing.Point(0, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(63, 26);
            this.btnSave.TabIndex = 64;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            textGradient1.BorderColor = System.Drawing.Color.DarkGray;
            textGradient1.BorderWidth = 0;
            textGradient1.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            textGradient1.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            customFont1.Color = System.Drawing.Color.White;
            customFont1.EmbeddedFontName = "Junction";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 9F;
            customFont1.StandardFontName = "Arial";
            customFont1.UseAntiAliasing = true;
            customFont1.UseEmbeddedFont = true;
            textGradient1.Font = customFont1;
            textGradient1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient1.Padding = 5;
            buttonTheme1.TextGradientDefault = textGradient1;
            textGradient2.BorderColor = System.Drawing.Color.DarkGray;
            textGradient2.BorderWidth = 0;
            textGradient2.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            textGradient2.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            customFont2.Color = System.Drawing.Color.WhiteSmoke;
            customFont2.EmbeddedFontName = "Junction";
            customFont2.IsBold = false;
            customFont2.IsItalic = false;
            customFont2.IsUnderline = false;
            customFont2.Size = 9F;
            customFont2.StandardFontName = "Arial";
            customFont2.UseAntiAliasing = true;
            customFont2.UseEmbeddedFont = true;
            textGradient2.Font = customFont2;
            textGradient2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient2.Padding = 5;
            buttonTheme1.TextGradientDisabled = textGradient2;
            textGradient3.BorderColor = System.Drawing.Color.Gray;
            textGradient3.BorderWidth = 0;
            textGradient3.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(130)))), ((int)(((byte)(146)))));
            textGradient3.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(130)))), ((int)(((byte)(146)))));
            customFont3.Color = System.Drawing.Color.White;
            customFont3.EmbeddedFontName = "Junction";
            customFont3.IsBold = false;
            customFont3.IsItalic = false;
            customFont3.IsUnderline = false;
            customFont3.Size = 9F;
            customFont3.StandardFontName = "Arial";
            customFont3.UseAntiAliasing = true;
            customFont3.UseEmbeddedFont = false;
            textGradient3.Font = customFont3;
            textGradient3.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient3.Padding = 5;
            buttonTheme1.TextGradientMouseOver = textGradient3;
            this.btnSave.Theme = buttonTheme1;
            this.toolTip.SetToolTip(this.btnSave, "Saves the current audio file metadata.");
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Image = global::MPfm.Windows.Properties.Resources.cancel;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.IsAutoSized = true;
            this.btnClose.Location = new System.Drawing.Point(63, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(74, 26);
            this.btnClose.TabIndex = 63;
            this.btnClose.Text = "Cancel";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            textGradient4.BorderColor = System.Drawing.Color.DarkGray;
            textGradient4.BorderWidth = 0;
            textGradient4.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            textGradient4.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            customFont4.Color = System.Drawing.Color.White;
            customFont4.EmbeddedFontName = "Junction";
            customFont4.IsBold = false;
            customFont4.IsItalic = false;
            customFont4.IsUnderline = false;
            customFont4.Size = 9F;
            customFont4.StandardFontName = "Arial";
            customFont4.UseAntiAliasing = true;
            customFont4.UseEmbeddedFont = true;
            textGradient4.Font = customFont4;
            textGradient4.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient4.Padding = 5;
            buttonTheme2.TextGradientDefault = textGradient4;
            textGradient5.BorderColor = System.Drawing.Color.DarkGray;
            textGradient5.BorderWidth = 0;
            textGradient5.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            textGradient5.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            customFont5.Color = System.Drawing.Color.WhiteSmoke;
            customFont5.EmbeddedFontName = "Junction";
            customFont5.IsBold = false;
            customFont5.IsItalic = false;
            customFont5.IsUnderline = false;
            customFont5.Size = 9F;
            customFont5.StandardFontName = "Arial";
            customFont5.UseAntiAliasing = true;
            customFont5.UseEmbeddedFont = true;
            textGradient5.Font = customFont5;
            textGradient5.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient5.Padding = 5;
            buttonTheme2.TextGradientDisabled = textGradient5;
            textGradient6.BorderColor = System.Drawing.Color.Gray;
            textGradient6.BorderWidth = 0;
            textGradient6.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(130)))), ((int)(((byte)(146)))));
            textGradient6.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(130)))), ((int)(((byte)(146)))));
            textGradient6.Font = customFont3;
            textGradient6.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient6.Padding = 5;
            buttonTheme2.TextGradientMouseOver = textGradient6;
            this.btnClose.Theme = buttonTheme2;
            this.toolTip.SetToolTip(this.btnClose, "Closes the window without saving.");
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelEditSongMetadata
            // 
            this.panelEditSongMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelEditSongMetadata.Controls.Add(this.lblEditing);
            this.panelEditSongMetadata.Controls.Add(this.propertyGridTags);
            this.panelEditSongMetadata.ExpandedHeight = 200;
            this.panelEditSongMetadata.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelEditSongMetadata.HeaderAutoSize = true;
            this.panelEditSongMetadata.HeaderExpandable = false;
            this.panelEditSongMetadata.HeaderExpanded = true;
            this.panelEditSongMetadata.HeaderHeight = 30;
            this.panelEditSongMetadata.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelEditSongMetadata.HeaderTitle = "Edit Song Metadata";
            this.panelEditSongMetadata.Location = new System.Drawing.Point(0, 26);
            this.panelEditSongMetadata.Name = "panelEditSongMetadata";
            this.panelEditSongMetadata.Size = new System.Drawing.Size(633, 458);
            this.panelEditSongMetadata.TabIndex = 65;
            this.panelEditSongMetadata.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            backgroundGradient2.BorderColor = System.Drawing.Color.DarkGray;
            backgroundGradient2.BorderWidth = 0;
            backgroundGradient2.Color1 = System.Drawing.Color.White;
            backgroundGradient2.Color2 = System.Drawing.Color.White;
            backgroundGradient2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            panelTheme1.BackgroundGradient = backgroundGradient2;
            textGradient8.BorderColor = System.Drawing.Color.DarkGray;
            textGradient8.BorderWidth = 0;
            textGradient8.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(88)))), ((int)(((byte)(101)))));
            textGradient8.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(88)))), ((int)(((byte)(101)))));
            customFont7.Color = System.Drawing.Color.White;
            customFont7.EmbeddedFontName = "Junction";
            customFont7.IsBold = false;
            customFont7.IsItalic = false;
            customFont7.IsUnderline = false;
            customFont7.Size = 11F;
            customFont7.StandardFontName = "Arial";
            customFont7.UseAntiAliasing = true;
            customFont7.UseEmbeddedFont = true;
            textGradient8.Font = customFont7;
            textGradient8.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient8.Padding = 2;
            panelTheme1.HeaderTextGradient = textGradient8;
            this.panelEditSongMetadata.Theme = panelTheme1;
            // 
            // lblEditing
            // 
            this.lblEditing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEditing.BackColor = System.Drawing.Color.Transparent;
            this.lblEditing.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditing.IsAutoSized = false;
            this.lblEditing.Location = new System.Drawing.Point(0, 32);
            this.lblEditing.Name = "lblEditing";
            this.lblEditing.Size = new System.Drawing.Size(627, 17);
            this.lblEditing.TabIndex = 66;
            this.lblEditing.Text = "Editing";
            this.lblEditing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme1.IsBackgroundTransparent = true;
            textGradient7.BorderColor = System.Drawing.Color.DarkGray;
            textGradient7.BorderWidth = 1;
            textGradient7.Color1 = System.Drawing.Color.LightGray;
            textGradient7.Color2 = System.Drawing.Color.Gray;
            customFont6.Color = System.Drawing.Color.Black;
            customFont6.EmbeddedFontName = "Junction";
            customFont6.IsBold = false;
            customFont6.IsItalic = false;
            customFont6.IsUnderline = false;
            customFont6.Size = 8F;
            customFont6.StandardFontName = "Arial";
            customFont6.UseAntiAliasing = true;
            customFont6.UseEmbeddedFont = true;
            textGradient7.Font = customFont6;
            textGradient7.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient7.Padding = 2;
            labelTheme1.TextGradient = textGradient7;
            this.lblEditing.Theme = labelTheme1;
            // 
            // propertyGridTags
            // 
            this.propertyGridTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridTags.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(88)))), ((int)(((byte)(101)))));
            this.propertyGridTags.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyGridTags.Location = new System.Drawing.Point(0, 52);
            this.propertyGridTags.Name = "propertyGridTags";
            this.propertyGridTags.Size = new System.Drawing.Size(631, 404);
            this.propertyGridTags.TabIndex = 64;
            this.propertyGridTags.ToolbarVisible = false;
            // 
            // frmEditSongMetadata
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(632, 482);
            this.ControlBox = false;
            this.Controls.Add(this.flowToolbar);
            this.Controls.Add(this.panelEditSongMetadata);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "frmEditSongMetadata";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Song Metadata";
            this.flowToolbar.ResumeLayout(false);
            this.panelEditSongMetadata.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PropertyGrid propertyGridTags;
        private MPfm.WindowsControls.Panel panelEditSongMetadata;
        private WindowsControls.Label lblEditing;
        public System.Windows.Forms.ToolTip toolTip;
        private WindowsControls.FlowLayoutPanel flowToolbar;
        private WindowsControls.Button btnSave;
        private WindowsControls.Button btnClose;        
    }
}
