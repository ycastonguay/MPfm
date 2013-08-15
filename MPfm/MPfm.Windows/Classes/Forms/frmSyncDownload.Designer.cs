namespace MPfm.Windows.Classes.Forms
{
    partial class frmSyncDownload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSyncDownload));
            MPfm.WindowsControls.PanelTheme panelTheme1 = new MPfm.WindowsControls.PanelTheme();
            MPfm.WindowsControls.BackgroundGradient backgroundGradient1 = new MPfm.WindowsControls.BackgroundGradient();
            MPfm.WindowsControls.TextGradient textGradient11 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont11 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme1 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient1 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme2 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient2 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme3 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient3 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme4 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient4 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme5 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient5 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme6 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient6 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme7 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient7 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.ButtonTheme buttonTheme1 = new MPfm.WindowsControls.ButtonTheme();
            MPfm.WindowsControls.TextGradient textGradient8 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient9 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient10 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            this.imageListIcons = new System.Windows.Forms.ImageList(this.components);
            this.panelBackground = new MPfm.WindowsControls.Panel();
            this.label4 = new MPfm.WindowsControls.Label();
            this.label5 = new MPfm.WindowsControls.Label();
            this.lblDownloadSpeedValue = new MPfm.WindowsControls.Label();
            this.label2 = new MPfm.WindowsControls.Label();
            this.lblStatus = new MPfm.WindowsControls.Label();
            this.lblTitle = new MPfm.WindowsControls.Label();
            this.lblSubtitle = new MPfm.WindowsControls.Label();
            this.btnCancel = new MPfm.WindowsControls.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.panelBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListIcons
            // 
            this.imageListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcons.ImageStream")));
            this.imageListIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListIcons.Images.SetKeyName(0, "icon_tablet_16.png");
            this.imageListIcons.Images.SetKeyName(1, "icon_linux_16.png");
            this.imageListIcons.Images.SetKeyName(2, "icon_osx_16.png");
            this.imageListIcons.Images.SetKeyName(3, "icon_windows_16.png");
            this.imageListIcons.Images.SetKeyName(4, "icon_phone_16.png");
            this.imageListIcons.Images.SetKeyName(5, "icon_android_16.png");
            // 
            // panelBackground
            // 
            this.panelBackground.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelBackground.Controls.Add(this.label4);
            this.panelBackground.Controls.Add(this.label5);
            this.panelBackground.Controls.Add(this.lblDownloadSpeedValue);
            this.panelBackground.Controls.Add(this.label2);
            this.panelBackground.Controls.Add(this.lblStatus);
            this.panelBackground.Controls.Add(this.lblTitle);
            this.panelBackground.Controls.Add(this.lblSubtitle);
            this.panelBackground.Controls.Add(this.btnCancel);
            this.panelBackground.Controls.Add(this.progressBar);
            this.panelBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBackground.ExpandedHeight = 200;
            this.panelBackground.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelBackground.HeaderAutoSize = true;
            this.panelBackground.HeaderExpandable = false;
            this.panelBackground.HeaderHeight = 0;
            this.panelBackground.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelBackground.HeaderTitle = "Devices";
            this.panelBackground.Location = new System.Drawing.Point(0, 0);
            this.panelBackground.Margin = new System.Windows.Forms.Padding(6, 6, 6, 0);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(398, 288);
            this.panelBackground.TabIndex = 104;
            this.panelBackground.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            backgroundGradient1.BorderColor = System.Drawing.Color.DarkGray;
            backgroundGradient1.BorderWidth = 0;
            backgroundGradient1.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            backgroundGradient1.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            backgroundGradient1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            panelTheme1.BackgroundGradient = backgroundGradient1;
            textGradient11.BorderColor = System.Drawing.Color.DarkGray;
            textGradient11.BorderWidth = 0;
            textGradient11.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(40)))), ((int)(((byte)(46)))));
            textGradient11.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(40)))), ((int)(((byte)(46)))));
            customFont11.Color = System.Drawing.Color.Black;
            customFont11.EmbeddedFontName = "Junction";
            customFont11.IsBold = false;
            customFont11.IsItalic = false;
            customFont11.IsUnderline = false;
            customFont11.Size = 9F;
            customFont11.StandardFontName = "Arial";
            customFont11.UseAntiAliasing = true;
            customFont11.UseEmbeddedFont = true;
            textGradient11.Font = customFont11;
            textGradient11.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient11.Padding = 2;
            panelTheme1.HeaderTextGradient = textGradient11;
            this.panelBackground.Theme = panelTheme1;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.IsAutoSized = false;
            this.label4.Location = new System.Drawing.Point(137, 146);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 22);
            this.label4.TabIndex = 113;
            this.label4.Text = "547";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme1.IsBackgroundTransparent = true;
            textGradient1.BorderColor = System.Drawing.Color.DarkGray;
            textGradient1.BorderWidth = 1;
            textGradient1.Color1 = System.Drawing.Color.LightGray;
            textGradient1.Color2 = System.Drawing.Color.Gray;
            customFont1.Color = System.Drawing.Color.White;
            customFont1.EmbeddedFontName = "Junction";
            customFont1.IsBold = false;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 13F;
            customFont1.StandardFontName = "Arial";
            customFont1.UseAntiAliasing = true;
            customFont1.UseEmbeddedFont = true;
            textGradient1.Font = customFont1;
            textGradient1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient1.Padding = 2;
            labelTheme1.TextGradient = textGradient1;
            this.label4.Theme = labelTheme1;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.IsAutoSized = false;
            this.label5.Location = new System.Drawing.Point(137, 166);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 22);
            this.label5.TabIndex = 112;
            this.label5.Text = "files to download";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme2.IsBackgroundTransparent = true;
            textGradient2.BorderColor = System.Drawing.Color.DarkGray;
            textGradient2.BorderWidth = 1;
            textGradient2.Color1 = System.Drawing.Color.LightGray;
            textGradient2.Color2 = System.Drawing.Color.Gray;
            customFont2.Color = System.Drawing.Color.LightGray;
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
            textGradient2.Padding = 2;
            labelTheme2.TextGradient = textGradient2;
            this.label5.Theme = labelTheme2;
            // 
            // lblDownloadSpeedValue
            // 
            this.lblDownloadSpeedValue.BackColor = System.Drawing.Color.Transparent;
            this.lblDownloadSpeedValue.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDownloadSpeedValue.IsAutoSized = false;
            this.lblDownloadSpeedValue.Location = new System.Drawing.Point(10, 146);
            this.lblDownloadSpeedValue.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblDownloadSpeedValue.Name = "lblDownloadSpeedValue";
            this.lblDownloadSpeedValue.Size = new System.Drawing.Size(124, 22);
            this.lblDownloadSpeedValue.TabIndex = 111;
            this.lblDownloadSpeedValue.Text = "4302 kb/s";
            this.lblDownloadSpeedValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme3.IsBackgroundTransparent = true;
            textGradient3.BorderColor = System.Drawing.Color.DarkGray;
            textGradient3.BorderWidth = 1;
            textGradient3.Color1 = System.Drawing.Color.LightGray;
            textGradient3.Color2 = System.Drawing.Color.Gray;
            customFont3.Color = System.Drawing.Color.White;
            customFont3.EmbeddedFontName = "Junction";
            customFont3.IsBold = false;
            customFont3.IsItalic = false;
            customFont3.IsUnderline = false;
            customFont3.Size = 13F;
            customFont3.StandardFontName = "Arial";
            customFont3.UseAntiAliasing = true;
            customFont3.UseEmbeddedFont = true;
            textGradient3.Font = customFont3;
            textGradient3.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient3.Padding = 2;
            labelTheme3.TextGradient = textGradient3;
            this.lblDownloadSpeedValue.Theme = labelTheme3;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.IsAutoSized = false;
            this.label2.Location = new System.Drawing.Point(10, 166);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 22);
            this.label2.TabIndex = 110;
            this.label2.Text = "download speed";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme4.IsBackgroundTransparent = true;
            textGradient4.BorderColor = System.Drawing.Color.DarkGray;
            textGradient4.BorderWidth = 1;
            textGradient4.Color1 = System.Drawing.Color.LightGray;
            textGradient4.Color2 = System.Drawing.Color.Gray;
            customFont4.Color = System.Drawing.Color.LightGray;
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
            textGradient4.Padding = 2;
            labelTheme4.TextGradient = textGradient4;
            this.label2.Theme = labelTheme4;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.IsAutoSized = false;
            this.lblStatus.Location = new System.Drawing.Point(10, 79);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(233, 22);
            this.lblStatus.TabIndex = 109;
            this.lblStatus.Text = "Downloading files...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme5.IsBackgroundTransparent = true;
            textGradient5.BorderColor = System.Drawing.Color.DarkGray;
            textGradient5.BorderWidth = 1;
            textGradient5.Color1 = System.Drawing.Color.LightGray;
            textGradient5.Color2 = System.Drawing.Color.Gray;
            customFont5.Color = System.Drawing.Color.White;
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
            textGradient5.Padding = 2;
            labelTheme5.TextGradient = textGradient5;
            this.lblStatus.Theme = labelTheme5;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.IsAutoSized = false;
            this.lblTitle.Location = new System.Drawing.Point(10, 6);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(675, 22);
            this.lblTitle.TabIndex = 108;
            this.lblTitle.Text = "Syncing Library With";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme6.IsBackgroundTransparent = true;
            textGradient6.BorderColor = System.Drawing.Color.DarkGray;
            textGradient6.BorderWidth = 1;
            textGradient6.Color1 = System.Drawing.Color.LightGray;
            textGradient6.Color2 = System.Drawing.Color.Gray;
            customFont6.Color = System.Drawing.Color.White;
            customFont6.EmbeddedFontName = "TitilliumText22L Lt";
            customFont6.IsBold = true;
            customFont6.IsItalic = false;
            customFont6.IsUnderline = false;
            customFont6.Size = 11F;
            customFont6.StandardFontName = "Arial";
            customFont6.UseAntiAliasing = true;
            customFont6.UseEmbeddedFont = true;
            textGradient6.Font = customFont6;
            textGradient6.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient6.Padding = 2;
            labelTheme6.TextGradient = textGradient6;
            this.lblTitle.Theme = labelTheme6;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.IsAutoSized = false;
            this.lblSubtitle.Location = new System.Drawing.Point(10, 25);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(233, 22);
            this.lblSubtitle.TabIndex = 107;
            this.lblSubtitle.Text = "My IP address is:";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme7.IsBackgroundTransparent = true;
            textGradient7.BorderColor = System.Drawing.Color.DarkGray;
            textGradient7.BorderWidth = 1;
            textGradient7.Color1 = System.Drawing.Color.LightGray;
            textGradient7.Color2 = System.Drawing.Color.Gray;
            customFont7.Color = System.Drawing.Color.LightGray;
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
            labelTheme7.TextGradient = textGradient7;
            this.lblSubtitle.Theme = labelTheme7;
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Image = global::MPfm.Windows.Properties.Resources.icon_button_cancel_16;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.IsAutoSized = true;
            this.btnCancel.Location = new System.Drawing.Point(130, 235);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(126, 28);
            this.btnCancel.TabIndex = 81;
            this.btnCancel.Text = "Cancel download";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            textGradient8.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient8.BorderWidth = 1;
            textGradient8.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            textGradient8.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            customFont8.Color = System.Drawing.Color.White;
            customFont8.EmbeddedFontName = "Junction";
            customFont8.IsBold = false;
            customFont8.IsItalic = false;
            customFont8.IsUnderline = false;
            customFont8.Size = 8F;
            customFont8.StandardFontName = "Arial";
            customFont8.UseAntiAliasing = true;
            customFont8.UseEmbeddedFont = true;
            textGradient8.Font = customFont8;
            textGradient8.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient8.Padding = 6;
            buttonTheme1.TextGradientDefault = textGradient8;
            textGradient9.BorderColor = System.Drawing.Color.DarkGray;
            textGradient9.BorderWidth = 1;
            textGradient9.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient9.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            customFont9.Color = System.Drawing.Color.LightGray;
            customFont9.EmbeddedFontName = "Junction";
            customFont9.IsBold = false;
            customFont9.IsItalic = false;
            customFont9.IsUnderline = false;
            customFont9.Size = 8F;
            customFont9.StandardFontName = "Arial";
            customFont9.UseAntiAliasing = true;
            customFont9.UseEmbeddedFont = true;
            textGradient9.Font = customFont9;
            textGradient9.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient9.Padding = 6;
            buttonTheme1.TextGradientDisabled = textGradient9;
            textGradient10.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient10.BorderWidth = 1;
            textGradient10.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(119)))), ((int)(((byte)(106)))));
            textGradient10.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(119)))), ((int)(((byte)(106)))));
            customFont10.Color = System.Drawing.Color.White;
            customFont10.EmbeddedFontName = "Junction";
            customFont10.IsBold = false;
            customFont10.IsItalic = false;
            customFont10.IsUnderline = false;
            customFont10.Size = 8F;
            customFont10.StandardFontName = "Arial";
            customFont10.UseAntiAliasing = true;
            customFont10.UseEmbeddedFont = true;
            textGradient10.Font = customFont10;
            textGradient10.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient10.Padding = 6;
            buttonTheme1.TextGradientMouseOver = textGradient10;
            this.btnCancel.Theme = buttonTheme1;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(13, 107);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(373, 18);
            this.progressBar.TabIndex = 95;
            // 
            // frmSyncDownload
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(398, 288);
            this.Controls.Add(this.panelBackground);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSyncDownload";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Syncing Library With";
            this.panelBackground.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageListIcons;
        private WindowsControls.Panel panelBackground;
        private WindowsControls.Label label4;
        private WindowsControls.Label label5;
        private WindowsControls.Label lblDownloadSpeedValue;
        private WindowsControls.Label label2;
        private WindowsControls.Label lblStatus;
        private WindowsControls.Label lblTitle;
        private WindowsControls.Label lblSubtitle;
        private WindowsControls.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}