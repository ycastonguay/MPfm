namespace MPfm.Windows.Classes.Forms
{
    partial class frmSync
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSync));
            MPfm.WindowsControls.PanelTheme panelTheme1 = new MPfm.WindowsControls.PanelTheme();
            MPfm.WindowsControls.BackgroundGradient backgroundGradient1 = new MPfm.WindowsControls.BackgroundGradient();
            MPfm.WindowsControls.TextGradient textGradient12 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont12 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme1 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient1 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont1 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.LabelTheme labelTheme2 = new MPfm.WindowsControls.LabelTheme();
            MPfm.WindowsControls.TextGradient textGradient2 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont2 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.ButtonTheme buttonTheme1 = new MPfm.WindowsControls.ButtonTheme();
            MPfm.WindowsControls.TextGradient textGradient3 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont3 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient4 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont4 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient5 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont5 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.ButtonTheme buttonTheme2 = new MPfm.WindowsControls.ButtonTheme();
            MPfm.WindowsControls.TextGradient textGradient6 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont6 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient7 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont7 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient8 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont8 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.ButtonTheme buttonTheme3 = new MPfm.WindowsControls.ButtonTheme();
            MPfm.WindowsControls.TextGradient textGradient9 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont9 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient10 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont10 = new MPfm.WindowsControls.CustomFont();
            MPfm.WindowsControls.TextGradient textGradient11 = new MPfm.WindowsControls.TextGradient();
            MPfm.WindowsControls.CustomFont customFont11 = new MPfm.WindowsControls.CustomFont();
            this.listView = new System.Windows.Forms.ListView();
            this.columnDevice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageListIcons = new System.Windows.Forms.ImageList(this.components);
            this.panelBackground = new MPfm.WindowsControls.Panel();
            this.lblTitle = new MPfm.WindowsControls.Label();
            this.lblSubtitle = new MPfm.WindowsControls.Label();
            this.btnConnectManual = new MPfm.WindowsControls.Button();
            this.btnRefreshDevices = new MPfm.WindowsControls.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnConnect = new MPfm.WindowsControls.Button();
            this.panelBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDevice});
            this.listView.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(13, 53);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(602, 152);
            this.listView.SmallImageList = this.imageListIcons;
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // columnDevice
            // 
            this.columnDevice.Text = "Device";
            this.columnDevice.Width = 554;
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
            this.panelBackground.Controls.Add(this.lblTitle);
            this.panelBackground.Controls.Add(this.lblSubtitle);
            this.panelBackground.Controls.Add(this.btnConnectManual);
            this.panelBackground.Controls.Add(this.btnRefreshDevices);
            this.panelBackground.Controls.Add(this.progressBar);
            this.panelBackground.Controls.Add(this.listView);
            this.panelBackground.Controls.Add(this.btnConnect);
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
            this.panelBackground.Size = new System.Drawing.Size(627, 250);
            this.panelBackground.TabIndex = 103;
            this.panelBackground.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            backgroundGradient1.BorderColor = System.Drawing.Color.DarkGray;
            backgroundGradient1.BorderWidth = 0;
            backgroundGradient1.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            backgroundGradient1.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(69)))), ((int)(((byte)(79)))));
            backgroundGradient1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            panelTheme1.BackgroundGradient = backgroundGradient1;
            textGradient12.BorderColor = System.Drawing.Color.DarkGray;
            textGradient12.BorderWidth = 0;
            textGradient12.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(40)))), ((int)(((byte)(46)))));
            textGradient12.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(40)))), ((int)(((byte)(46)))));
            customFont12.Color = System.Drawing.Color.Black;
            customFont12.EmbeddedFontName = "Junction";
            customFont12.IsBold = false;
            customFont12.IsItalic = false;
            customFont12.IsUnderline = false;
            customFont12.Size = 9F;
            customFont12.StandardFontName = "Arial";
            customFont12.UseAntiAliasing = true;
            customFont12.UseEmbeddedFont = true;
            textGradient12.Font = customFont12;
            textGradient12.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient12.Padding = 2;
            panelTheme1.HeaderTextGradient = textGradient12;
            this.panelBackground.Theme = panelTheme1;
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
            this.lblTitle.Size = new System.Drawing.Size(233, 22);
            this.lblTitle.TabIndex = 108;
            this.lblTitle.Text = "Sync Library With Other Devices";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelTheme1.IsBackgroundTransparent = true;
            textGradient1.BorderColor = System.Drawing.Color.DarkGray;
            textGradient1.BorderWidth = 1;
            textGradient1.Color1 = System.Drawing.Color.LightGray;
            textGradient1.Color2 = System.Drawing.Color.Gray;
            customFont1.Color = System.Drawing.Color.White;
            customFont1.EmbeddedFontName = "TitilliumText22L Lt";
            customFont1.IsBold = true;
            customFont1.IsItalic = false;
            customFont1.IsUnderline = false;
            customFont1.Size = 11F;
            customFont1.StandardFontName = "Arial";
            customFont1.UseAntiAliasing = true;
            customFont1.UseEmbeddedFont = true;
            textGradient1.Font = customFont1;
            textGradient1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient1.Padding = 2;
            labelTheme1.TextGradient = textGradient1;
            this.lblTitle.Theme = labelTheme1;
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
            customFont2.Size = 8F;
            customFont2.StandardFontName = "Arial";
            customFont2.UseAntiAliasing = true;
            customFont2.UseEmbeddedFont = true;
            textGradient2.Font = customFont2;
            textGradient2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient2.Padding = 2;
            labelTheme2.TextGradient = textGradient2;
            this.lblSubtitle.Theme = labelTheme2;
            // 
            // btnConnectManual
            // 
            this.btnConnectManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnectManual.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnectManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnectManual.Image = global::MPfm.Windows.Properties.Resources.icon_button_connect_16;
            this.btnConnectManual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnectManual.IsAutoSized = true;
            this.btnConnectManual.Location = new System.Drawing.Point(291, 213);
            this.btnConnectManual.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnConnectManual.Name = "btnConnectManual";
            this.btnConnectManual.Size = new System.Drawing.Size(190, 28);
            this.btnConnectManual.TabIndex = 96;
            this.btnConnectManual.Text = "Connect manually to a device";
            this.btnConnectManual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            textGradient3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient3.BorderWidth = 1;
            textGradient3.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            textGradient3.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            customFont3.Color = System.Drawing.Color.White;
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
            textGradient3.Padding = 6;
            buttonTheme1.TextGradientDefault = textGradient3;
            textGradient4.BorderColor = System.Drawing.Color.DarkGray;
            textGradient4.BorderWidth = 1;
            textGradient4.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient4.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            customFont4.Color = System.Drawing.Color.LightGray;
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
            textGradient4.Padding = 6;
            buttonTheme1.TextGradientDisabled = textGradient4;
            textGradient5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient5.BorderWidth = 1;
            textGradient5.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(119)))), ((int)(((byte)(106)))));
            textGradient5.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(119)))), ((int)(((byte)(106)))));
            customFont5.Color = System.Drawing.Color.White;
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
            textGradient5.Padding = 6;
            buttonTheme1.TextGradientMouseOver = textGradient5;
            this.btnConnectManual.Theme = buttonTheme1;
            this.btnConnectManual.Click += new System.EventHandler(this.btnConnectManual_Click);
            // 
            // btnRefreshDevices
            // 
            this.btnRefreshDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefreshDevices.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshDevices.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshDevices.Image = global::MPfm.Windows.Properties.Resources.icon_button_cancel_16;
            this.btnRefreshDevices.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefreshDevices.IsAutoSized = true;
            this.btnRefreshDevices.Location = new System.Drawing.Point(13, 213);
            this.btnRefreshDevices.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnRefreshDevices.Name = "btnRefreshDevices";
            this.btnRefreshDevices.Size = new System.Drawing.Size(113, 28);
            this.btnRefreshDevices.TabIndex = 81;
            this.btnRefreshDevices.Text = "Cancel refresh";
            this.btnRefreshDevices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            textGradient6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient6.BorderWidth = 1;
            textGradient6.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            textGradient6.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            customFont6.Color = System.Drawing.Color.White;
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
            textGradient6.Padding = 6;
            buttonTheme2.TextGradientDefault = textGradient6;
            textGradient7.BorderColor = System.Drawing.Color.DarkGray;
            textGradient7.BorderWidth = 1;
            textGradient7.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient7.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
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
            textGradient7.Padding = 6;
            buttonTheme2.TextGradientDisabled = textGradient7;
            textGradient8.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient8.BorderWidth = 1;
            textGradient8.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(119)))), ((int)(((byte)(106)))));
            textGradient8.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(119)))), ((int)(((byte)(106)))));
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
            buttonTheme2.TextGradientMouseOver = textGradient8;
            this.btnRefreshDevices.Theme = buttonTheme2;
            this.btnRefreshDevices.Click += new System.EventHandler(this.btnRefreshDevices_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(474, 25);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(141, 18);
            this.progressBar.TabIndex = 95;
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Image = global::MPfm.Windows.Properties.Resources.icon_button_connect_16;
            this.btnConnect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.IsAutoSized = true;
            this.btnConnect.Location = new System.Drawing.Point(484, 213);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(131, 28);
            this.btnConnect.TabIndex = 94;
            this.btnConnect.Text = "Connect to device";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            textGradient9.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient9.BorderWidth = 1;
            textGradient9.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            textGradient9.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            customFont9.Color = System.Drawing.Color.White;
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
            buttonTheme3.TextGradientDefault = textGradient9;
            textGradient10.BorderColor = System.Drawing.Color.DarkGray;
            textGradient10.BorderWidth = 1;
            textGradient10.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient10.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            customFont10.Color = System.Drawing.Color.LightGray;
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
            buttonTheme3.TextGradientDisabled = textGradient10;
            textGradient11.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(43)))), ((int)(((byte)(30)))));
            textGradient11.BorderWidth = 1;
            textGradient11.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(119)))), ((int)(((byte)(106)))));
            textGradient11.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(119)))), ((int)(((byte)(106)))));
            customFont11.Color = System.Drawing.Color.White;
            customFont11.EmbeddedFontName = "Junction";
            customFont11.IsBold = false;
            customFont11.IsItalic = false;
            customFont11.IsUnderline = false;
            customFont11.Size = 8F;
            customFont11.StandardFontName = "Arial";
            customFont11.UseAntiAliasing = true;
            customFont11.UseEmbeddedFont = true;
            textGradient11.Font = customFont11;
            textGradient11.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            textGradient11.Padding = 6;
            buttonTheme3.TextGradientMouseOver = textGradient11;
            this.btnConnect.Theme = buttonTheme3;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // frmSync
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(627, 250);
            this.Controls.Add(this.panelBackground);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSync";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sync Library With Other Devices";
            this.panelBackground.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnDevice;
        private WindowsControls.Panel panelBackground;
        private System.Windows.Forms.ProgressBar progressBar;
        private WindowsControls.Button btnRefreshDevices;
        private WindowsControls.Button btnConnect;
        private WindowsControls.Button btnConnectManual;
        private WindowsControls.Label lblTitle;
        private WindowsControls.Label lblSubtitle;
        private System.Windows.Forms.ImageList imageListIcons;
    }
}