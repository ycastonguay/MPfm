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
    partial class frmLoadPlaylist
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
            MPfm.WindowsControls.PanelTheme panelTheme1 = new MPfm.WindowsControls.PanelTheme();
            MPfm.WindowsControls.BackgroundGradient backgroundGradient1 = new MPfm.WindowsControls.BackgroundGradient();
            MPfm.WindowsControls.TextGradient textGradient7 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme1 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient1 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.ButtonTheme buttonTheme1 = new MPfm.WindowsControls.ButtonTheme();
            MPfm.WindowsControls.TextGradient textGradient2 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient3 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient4 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme2 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient5 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme3 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient6 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            this.panelLoadPlaylist = new MPfm.WindowsControls.Panel();
            this.lblPercentage = new MPfm.WindowsControls.Label();
            this.btnCancel = new MPfm.WindowsControls.Button();
            this.lblFilePath = new MPfm.WindowsControls.Label();
            this.lblFile = new MPfm.WindowsControls.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.panelLoadPlaylist.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLoadPlaylist
            // 
            this.panelLoadPlaylist.Controls.Add(this.lblPercentage);
            this.panelLoadPlaylist.Controls.Add(this.btnCancel);
            this.panelLoadPlaylist.Controls.Add(this.lblFilePath);
            this.panelLoadPlaylist.Controls.Add(this.lblFile);
            this.panelLoadPlaylist.Controls.Add(this.progressBar);
            this.panelLoadPlaylist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLoadPlaylist.ExpandedHeight = 0;
            this.panelLoadPlaylist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLoadPlaylist.HeaderAutoSize = true;
            this.panelLoadPlaylist.HeaderExpandable = false;
            this.panelLoadPlaylist.HeaderExpanded = true;
            this.panelLoadPlaylist.HeaderHeight = 0;
            this.panelLoadPlaylist.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panelLoadPlaylist.Location = new System.Drawing.Point(0, 0);
            this.panelLoadPlaylist.Name = "panelLoadPlaylist";
            this.panelLoadPlaylist.Size = new System.Drawing.Size(600, 55);
            this.panelLoadPlaylist.TabIndex = 84;
            this.panelLoadPlaylist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            backgroundGradient1.BorderColor = System.Drawing.Color.DarkGray;
            backgroundGradient1.BorderWidth = 1;
            backgroundGradient1.Color1 = System.Drawing.Color.LightGray;
            backgroundGradient1.Color2 = System.Drawing.Color.Gray;
            backgroundGradient1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            panelTheme1.BackgroundGradient = backgroundGradient1;
            textGradient7.BorderColor = System.Drawing.Color.DarkGray;
            textGradient7.BorderWidth = 1;
            textGradient7.Color1 = System.Drawing.Color.LightGray;
            textGradient7.Color2 = System.Drawing.Color.Gray;
            customFont7.Color = System.Drawing.Color.Black;
            customFont7.EmbeddedFontName = "Junction";
            customFont7.IsBold = false;
            customFont7.IsItalic = false;
            customFont7.IsUnderline = false;
            customFont7.Size = 8F;
            customFont7.StandardFontName = "Arial";
            customFont7.UseAntiAliasing = true;
            customFont7.UseEmbeddedFont = true;
            textGradient7.Font = customFont7;
            textGradient7.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient7.Padding = 2;
            panelTheme1.HeaderTextGradient = textGradient7;
            this.panelLoadPlaylist.Theme = panelTheme1;
            // 
            // lblPercentage
            // 
            this.lblPercentage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPercentage.BackColor = System.Drawing.Color.Transparent;
            this.lblPercentage.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPercentage.ForeColor = System.Drawing.Color.White;
            this.lblPercentage.IsAutoSized = false;
            this.lblPercentage.Location = new System.Drawing.Point(560, 26);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(37, 17);
            this.lblPercentage.TabIndex = 82;
            this.lblPercentage.Text = "100%";
            this.lblPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            textGradient1.Padding = 2;
            labelTheme1.TextGradient = textGradient1;
            this.lblPercentage.Theme = labelTheme1;
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnCancel.Image = null;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.IsAutoSized = false;
            this.btnCancel.Location = new System.Drawing.Point(6, 27);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(58, 20);
            this.btnCancel.TabIndex = 84;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            textGradient2.BorderColor = System.Drawing.Color.DarkGray;
            textGradient2.BorderWidth = 1;
            textGradient2.Color1 = System.Drawing.Color.LightGray;
            textGradient2.Color2 = System.Drawing.Color.Gray;
            customFont2.Color = System.Drawing.Color.Black;
            customFont2.EmbeddedFontName = "Junction";
            customFont2.IsBold = false;
            customFont2.IsItalic = false;
            customFont2.IsUnderline = false;
            customFont2.Size = 8F;
            customFont2.StandardFontName = "Arial";
            customFont2.UseAntiAliasing = true;
            customFont2.UseEmbeddedFont = true;
            textGradient2.Font = customFont2;
            textGradient2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient2.Padding = 2;
            buttonTheme1.TextGradientDefault = textGradient2;
            textGradient3.BorderColor = System.Drawing.Color.DarkGray;
            textGradient3.BorderWidth = 1;
            textGradient3.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            textGradient3.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            customFont3.Color = System.Drawing.Color.LightGray;
            customFont3.EmbeddedFontName = "Junction";
            customFont3.IsBold = false;
            customFont3.IsItalic = false;
            customFont3.IsUnderline = false;
            customFont3.Size = 8F;
            customFont3.StandardFontName = "Arial";
            customFont3.UseAntiAliasing = true;
            customFont3.UseEmbeddedFont = true;
            textGradient3.Font = customFont3;
            textGradient3.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient3.Padding = 2;
            buttonTheme1.TextGradientDisabled = textGradient3;
            textGradient4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            textGradient4.BorderWidth = 1;
            textGradient4.Color1 = System.Drawing.Color.White;
            textGradient4.Color2 = System.Drawing.Color.LightGray;
            customFont4.Color = System.Drawing.Color.Black;
            customFont4.EmbeddedFontName = "Junction";
            customFont4.IsBold = false;
            customFont4.IsItalic = false;
            customFont4.IsUnderline = false;
            customFont4.Size = 8F;
            customFont4.StandardFontName = "Arial";
            customFont4.UseAntiAliasing = true;
            customFont4.UseEmbeddedFont = true;
            textGradient4.Font = customFont4;
            textGradient4.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient4.Padding = 2;
            buttonTheme1.TextGradientMouseOver = textGradient4;
            this.btnCancel.Theme = buttonTheme1;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblFilePath
            // 
            this.lblFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilePath.BackColor = System.Drawing.Color.Transparent;
            this.lblFilePath.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilePath.ForeColor = System.Drawing.Color.White;
            this.lblFilePath.IsAutoSized = false;
            this.lblFilePath.Location = new System.Drawing.Point(34, 4);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(558, 17);
            this.lblFilePath.TabIndex = 85;
            this.lblFilePath.Text = "C:\\MP3\\Unknown Artist\\Unknown Album\\01 Hello World.mp3";
            this.lblFilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme2.IsBackgroundTransparent = true;
            textGradient5.BorderColor = System.Drawing.Color.DarkGray;
            textGradient5.BorderWidth = 1;
            textGradient5.Color1 = System.Drawing.Color.LightGray;
            textGradient5.Color2 = System.Drawing.Color.Gray;
            customFont5.Color = System.Drawing.Color.Black;
            customFont5.EmbeddedFontName = "Junction";
            customFont5.IsBold = false;
            customFont5.IsItalic = false;
            customFont5.IsUnderline = false;
            customFont5.Size = 8F;
            customFont5.StandardFontName = "Arial";
            customFont5.UseAntiAliasing = true;
            customFont5.UseEmbeddedFont = true;
            textGradient5.Font = customFont5;
            textGradient5.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient5.Padding = 2;
            labelTheme2.TextGradient = textGradient5;
            this.lblFilePath.Theme = labelTheme2;
            // 
            // lblFile
            // 
            this.lblFile.BackColor = System.Drawing.Color.Transparent;
            this.lblFile.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFile.ForeColor = System.Drawing.Color.White;
            this.lblFile.IsAutoSized = false;
            this.lblFile.Location = new System.Drawing.Point(3, 4);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(37, 17);
            this.lblFile.TabIndex = 84;
            this.lblFile.Text = "File :";
            this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme3.IsBackgroundTransparent = true;
            textGradient6.BorderColor = System.Drawing.Color.DarkGray;
            textGradient6.BorderWidth = 1;
            textGradient6.Color1 = System.Drawing.Color.LightGray;
            textGradient6.Color2 = System.Drawing.Color.Gray;
            customFont6.Color = System.Drawing.Color.Black;
            customFont6.EmbeddedFontName = "Junction";
            customFont6.IsBold = false;
            customFont6.IsItalic = false;
            customFont6.IsUnderline = false;
            customFont6.Size = 8F;
            customFont6.StandardFontName = "Arial";
            customFont6.UseAntiAliasing = true;
            customFont6.UseEmbeddedFont = true;
            textGradient6.Font = customFont6;
            textGradient6.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient6.Padding = 2;
            labelTheme3.TextGradient = textGradient6;
            this.lblFile.Theme = labelTheme3;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(70, 27);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(487, 20);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 81;
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            // 
            // frmLoadPlaylist
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(600, 55);
            this.ControlBox = false;
            this.Controls.Add(this.panelLoadPlaylist);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmLoadPlaylist";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading playlist";
            this.Load += new System.EventHandler(this.frmLoadPlaylist_Load);
            this.panelLoadPlaylist.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private WindowsControls.Panel panelLoadPlaylist;
        private WindowsControls.Label lblPercentage;
        private WindowsControls.Button btnCancel;
        private WindowsControls.Label lblFilePath;
        private WindowsControls.Label lblFile;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker worker;

    }
}
