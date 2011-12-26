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
            MPfm.WindowsControls.EmbeddedFont customFont21 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont customFont22 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont customFont23 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont customFont24 = new MPfm.WindowsControls.EmbeddedFont();
            MPfm.WindowsControls.EmbeddedFont customFont25 = new MPfm.WindowsControls.EmbeddedFont();
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.panelEditMarker = new MPfm.WindowsControls.Panel();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.lblComments = new MPfm.WindowsControls.Label();
            this.lblPositionMSValue = new MPfm.WindowsControls.Label();
            this.lblPositionMS = new MPfm.WindowsControls.Label();
            this.waveForm = new MPfm.WindowsControls.WaveFormDisplay();
            this.panelWarning = new MPfm.WindowsControls.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblWarning = new MPfm.WindowsControls.Label();
            this.lblSongValue = new MPfm.WindowsControls.Label();
            this.lblSong = new MPfm.WindowsControls.Label();
            this.lblPositionPCMBytesValue = new MPfm.WindowsControls.Label();
            this.lblPositionPCMBytes = new MPfm.WindowsControls.Label();
            this.btnGoTo = new MPfm.WindowsControls.Button();
            this.btnPunchIn = new MPfm.WindowsControls.Button();
            this.lblPositionPCMValue = new MPfm.WindowsControls.Label();
            this.lblPositionPCM = new MPfm.WindowsControls.Label();
            this.lblPosition = new MPfm.WindowsControls.Label();
            this.txtPosition = new System.Windows.Forms.MaskedTextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new MPfm.WindowsControls.Label();
            this.btnSave = new MPfm.WindowsControls.Button();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.panelEditMarker.SuspendLayout();
            this.panelWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // fontCollection
            // 
            customFont21.AssemblyPath = "MPfm.Fonts.dll";
            customFont21.Name = "LeagueGothic";
            customFont21.ResourceName = "MPfm.Fonts.LeagueGothic.ttf";
            customFont22.AssemblyPath = "MPfm.Fonts.dll";
            customFont22.Name = "Junction";
            customFont22.ResourceName = "MPfm.Fonts.Junction.ttf";
            customFont23.AssemblyPath = "MPfm.Fonts.dll";
            customFont23.Name = "Nobile";
            customFont23.ResourceName = "MPfm.Fonts.nobile.ttf";
            customFont24.AssemblyPath = "MPfm.Fonts.dll";
            customFont24.Name = "TitilliumText22L Lt";
            customFont24.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            customFont25.AssemblyPath = "MPfm.Fonts.dll";
            customFont25.Name = "Museo Sans 500";
            customFont25.ResourceName = "MPfm.Fonts.MuseoSans_500.ttf";
            this.fontCollection.Fonts.Add(customFont21);
            this.fontCollection.Fonts.Add(customFont22);
            this.fontCollection.Fonts.Add(customFont23);
            this.fontCollection.Fonts.Add(customFont24);
            this.fontCollection.Fonts.Add(customFont25);
            // 
            // panelEditMarker
            // 
            this.panelEditMarker.AntiAliasingEnabled = true;
            this.panelEditMarker.Controls.Add(this.lblSong);
            this.panelEditMarker.Controls.Add(this.txtComments);
            this.panelEditMarker.Controls.Add(this.lblComments);
            this.panelEditMarker.Controls.Add(this.lblPositionMSValue);
            this.panelEditMarker.Controls.Add(this.lblPositionMS);
            this.panelEditMarker.Controls.Add(this.waveForm);
            this.panelEditMarker.Controls.Add(this.panelWarning);
            this.panelEditMarker.Controls.Add(this.lblSongValue);
            this.panelEditMarker.Controls.Add(this.lblPositionPCMBytesValue);
            this.panelEditMarker.Controls.Add(this.lblPositionPCMBytes);
            this.panelEditMarker.Controls.Add(this.btnGoTo);
            this.panelEditMarker.Controls.Add(this.btnPunchIn);
            this.panelEditMarker.Controls.Add(this.lblPositionPCMValue);
            this.panelEditMarker.Controls.Add(this.lblPositionPCM);
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
            this.panelEditMarker.Size = new System.Drawing.Size(593, 394);
            this.panelEditMarker.TabIndex = 65;
            // 
            // txtComments
            // 
            this.txtComments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtComments.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtComments.Location = new System.Drawing.Point(6, 274);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(581, 57);
            this.txtComments.TabIndex = 96;
            // 
            // lblComments
            // 
            this.lblComments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblComments.AntiAliasingEnabled = true;
            this.lblComments.BackColor = System.Drawing.Color.Transparent;
            this.lblComments.CustomFontName = "Junction";
            this.lblComments.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComments.Location = new System.Drawing.Point(3, 254);
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size(587, 17);
            this.lblComments.TabIndex = 95;
            this.lblComments.Text = "Comments :";
            // 
            // lblPositionMSValue
            // 
            this.lblPositionMSValue.AntiAliasingEnabled = true;
            this.lblPositionMSValue.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionMSValue.CustomFontName = "";
            this.lblPositionMSValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionMSValue.Location = new System.Drawing.Point(279, 150);
            this.lblPositionMSValue.Name = "lblPositionMSValue";
            this.lblPositionMSValue.Size = new System.Drawing.Size(89, 17);
            this.lblPositionMSValue.TabIndex = 94;
            this.lblPositionMSValue.Text = "0";
            // 
            // lblPositionMS
            // 
            this.lblPositionMS.AntiAliasingEnabled = true;
            this.lblPositionMS.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionMS.CustomFontName = "Junction";
            this.lblPositionMS.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionMS.Location = new System.Drawing.Point(279, 134);
            this.lblPositionMS.Name = "lblPositionMS";
            this.lblPositionMS.Size = new System.Drawing.Size(89, 17);
            this.lblPositionMS.TabIndex = 93;
            this.lblPositionMS.Text = "Milliseconds";
            // 
            // waveForm
            // 
            this.waveForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waveForm.AutoScrollWithCursor = true;
            this.waveForm.BorderColor = System.Drawing.Color.Empty;
            this.waveForm.BorderWidth = 0;
            this.waveForm.CursorColor = System.Drawing.Color.RoyalBlue;
            this.waveForm.CustomFontName = "BPmono";
            this.waveForm.DisplayCurrentPosition = true;
            this.waveForm.DisplayType = MPfm.WindowsControls.WaveFormDisplayType.Stereo;
            this.waveForm.Font = new System.Drawing.Font("Arial", 8F);
            this.waveForm.FontCollection = this.fontCollection;
            this.waveForm.GradientColor1 = System.Drawing.Color.Black;
            this.waveForm.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.waveForm.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.waveForm.Length = ((long)(0));
            this.waveForm.Location = new System.Drawing.Point(6, 172);
            this.waveForm.Name = "waveForm";
            this.waveForm.PeakFileDirectory = "C:\\Users\\Animal Mother\\AppData\\Local\\Microsoft\\VisualStudio\\10.0\\ProjectAssemblie" +
    "s\\om-0gycd01\\Peak Files\\";
            this.waveForm.Size = new System.Drawing.Size(581, 79);
            this.waveForm.TabIndex = 92;
            this.waveForm.WaveFormColor = System.Drawing.Color.Yellow;
            this.waveForm.Zoom = 100F;
            // 
            // panelWarning
            // 
            this.panelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelWarning.AntiAliasingEnabled = true;
            this.panelWarning.Controls.Add(this.pictureBox1);
            this.panelWarning.Controls.Add(this.lblWarning);
            this.panelWarning.ExpandedHeight = 200;
            this.panelWarning.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelWarning.FontCollection = this.fontCollection;
            this.panelWarning.GradientColor1 = System.Drawing.Color.LemonChiffon;
            this.panelWarning.GradientColor2 = System.Drawing.Color.Gold;
            this.panelWarning.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelWarning.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelWarning.HeaderExpandable = false;
            this.panelWarning.HeaderExpanded = true;
            this.panelWarning.HeaderForeColor = System.Drawing.Color.Black;
            this.panelWarning.HeaderGradientColor1 = System.Drawing.Color.LemonChiffon;
            this.panelWarning.HeaderGradientColor2 = System.Drawing.Color.Gold;
            this.panelWarning.HeaderHeight = 18;
            this.panelWarning.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelWarning.HeaderTitle = "Warning";
            this.panelWarning.Location = new System.Drawing.Point(6, 340);
            this.panelWarning.Name = "panelWarning";
            this.panelWarning.Size = new System.Drawing.Size(386, 44);
            this.panelWarning.TabIndex = 91;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::MPfm.Properties.Resources.error;
            this.pictureBox1.Location = new System.Drawing.Point(6, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(21, 23);
            this.pictureBox1.TabIndex = 92;
            this.pictureBox1.TabStop = false;
            // 
            // lblWarning
            // 
            this.lblWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWarning.AntiAliasingEnabled = true;
            this.lblWarning.BackColor = System.Drawing.Color.Transparent;
            this.lblWarning.CustomFontName = "Junction";
            this.lblWarning.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.Location = new System.Drawing.Point(24, 20);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(338, 17);
            this.lblWarning.TabIndex = 91;
            this.lblWarning.Text = "The marker must have a valid name.";
            // 
            // lblSongValue
            // 
            this.lblSongValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSongValue.AntiAliasingEnabled = true;
            this.lblSongValue.BackColor = System.Drawing.Color.Transparent;
            this.lblSongValue.CustomFontName = "Junction";
            this.lblSongValue.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSongValue.Location = new System.Drawing.Point(3, 50);
            this.lblSongValue.Name = "lblSongValue";
            this.lblSongValue.Size = new System.Drawing.Size(534, 17);
            this.lblSongValue.TabIndex = 79;
            this.lblSongValue.Text = "Song";
            // 
            // lblSong
            // 
            this.lblSong.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSong.AntiAliasingEnabled = true;
            this.lblSong.BackColor = System.Drawing.Color.Transparent;
            this.lblSong.CustomFontName = "Junction";
            this.lblSong.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));            
            this.lblSong.Location = new System.Drawing.Point(3, 34);
            this.lblSong.Name = "lblSong";
            this.lblSong.Size = new System.Drawing.Size(37, 19);
            this.lblSong.TabIndex = 78;
            this.lblSong.Text = "Song :";
            // 
            // lblPositionPCMBytesValue
            // 
            this.lblPositionPCMBytesValue.AntiAliasingEnabled = true;
            this.lblPositionPCMBytesValue.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionPCMBytesValue.CustomFontName = "";
            this.lblPositionPCMBytesValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionPCMBytesValue.Location = new System.Drawing.Point(481, 150);
            this.lblPositionPCMBytesValue.Name = "lblPositionPCMBytesValue";
            this.lblPositionPCMBytesValue.Size = new System.Drawing.Size(89, 17);
            this.lblPositionPCMBytesValue.TabIndex = 77;
            this.lblPositionPCMBytesValue.Text = "0";
            // 
            // lblPositionPCMBytes
            // 
            this.lblPositionPCMBytes.AntiAliasingEnabled = true;
            this.lblPositionPCMBytes.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionPCMBytes.CustomFontName = "Junction";
            this.lblPositionPCMBytes.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionPCMBytes.Location = new System.Drawing.Point(481, 134);
            this.lblPositionPCMBytes.Name = "lblPositionPCMBytes";
            this.lblPositionPCMBytes.Size = new System.Drawing.Size(107, 17);
            this.lblPositionPCMBytes.TabIndex = 76;
            this.lblPositionPCMBytes.Text = "PCM (bytes)";
            // 
            // btnGoTo
            // 
            this.btnGoTo.AntiAliasingEnabled = true;
            this.btnGoTo.BorderColor = System.Drawing.Color.DimGray;
            this.btnGoTo.BorderWidth = 1;
            this.btnGoTo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGoTo.CustomFontName = "Junction";
            this.btnGoTo.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnGoTo.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnGoTo.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnGoTo.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnGoTo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGoTo.FontCollection = this.fontCollection;
            this.btnGoTo.FontColor = System.Drawing.Color.Black;
            this.btnGoTo.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnGoTo.GradientColor2 = System.Drawing.Color.Gray;
            this.btnGoTo.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnGoTo.Image = global::MPfm.Properties.Resources.arrow_right;
            this.btnGoTo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGoTo.Location = new System.Drawing.Point(211, 135);
            this.btnGoTo.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnGoTo.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnGoTo.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnGoTo.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new System.Drawing.Size(60, 30);
            this.btnGoTo.TabIndex = 75;
            this.btnGoTo.Text = "Go to";
            this.btnGoTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGoTo.UseVisualStyleBackColor = true;
            this.btnGoTo.Click += new System.EventHandler(this.btnGoTo_Click);
            // 
            // btnPunchIn
            // 
            this.btnPunchIn.AntiAliasingEnabled = true;
            this.btnPunchIn.BorderColor = System.Drawing.Color.DimGray;
            this.btnPunchIn.BorderWidth = 1;
            this.btnPunchIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPunchIn.CustomFontName = "Junction";
            this.btnPunchIn.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnPunchIn.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnPunchIn.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnPunchIn.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnPunchIn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPunchIn.FontCollection = this.fontCollection;
            this.btnPunchIn.FontColor = System.Drawing.Color.Black;
            this.btnPunchIn.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnPunchIn.GradientColor2 = System.Drawing.Color.Gray;
            this.btnPunchIn.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnPunchIn.Image = global::MPfm.Properties.Resources.time;
            this.btnPunchIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPunchIn.Location = new System.Drawing.Point(129, 135);
            this.btnPunchIn.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnPunchIn.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnPunchIn.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnPunchIn.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnPunchIn.Name = "btnPunchIn";
            this.btnPunchIn.Size = new System.Drawing.Size(76, 30);
            this.btnPunchIn.TabIndex = 74;
            this.btnPunchIn.Text = "Punch in";
            this.btnPunchIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPunchIn.UseVisualStyleBackColor = true;
            this.btnPunchIn.Click += new System.EventHandler(this.btnPunchIn_Click);
            // 
            // lblPositionPCMValue
            // 
            this.lblPositionPCMValue.AntiAliasingEnabled = true;
            this.lblPositionPCMValue.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionPCMValue.CustomFontName = "";
            this.lblPositionPCMValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionPCMValue.Location = new System.Drawing.Point(374, 150);
            this.lblPositionPCMValue.Name = "lblPositionPCMValue";
            this.lblPositionPCMValue.Size = new System.Drawing.Size(89, 17);
            this.lblPositionPCMValue.TabIndex = 73;
            this.lblPositionPCMValue.Text = "0";
            // 
            // lblPositionPCM
            // 
            this.lblPositionPCM.AntiAliasingEnabled = true;
            this.lblPositionPCM.BackColor = System.Drawing.Color.Transparent;
            this.lblPositionPCM.CustomFontName = "Junction";
            this.lblPositionPCM.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPositionPCM.Location = new System.Drawing.Point(374, 134);
            this.lblPositionPCM.Name = "lblPositionPCM";
            this.lblPositionPCM.Size = new System.Drawing.Size(101, 17);
            this.lblPositionPCM.TabIndex = 72;
            this.lblPositionPCM.Text = "PCM (samples)";
            // 
            // lblPosition
            // 
            this.lblPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPosition.AntiAliasingEnabled = true;
            this.lblPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblPosition.CustomFontName = "Junction";
            this.lblPosition.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosition.Location = new System.Drawing.Point(3, 115);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(587, 17);
            this.lblPosition.TabIndex = 70;
            this.lblPosition.Text = "Position :";
            // 
            // txtPosition
            // 
            this.txtPosition.Font = new System.Drawing.Font("Droid Sans Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPosition.Location = new System.Drawing.Point(6, 135);
            this.txtPosition.Mask = "0:00.000";
            this.txtPosition.Name = "txtPosition";
            this.txtPosition.Size = new System.Drawing.Size(115, 30);
            this.txtPosition.TabIndex = 69;
            this.txtPosition.Text = "000000";
            this.txtPosition.TextChanged += new System.EventHandler(this.txtPosition_TextChanged);
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(6, 89);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(581, 21);
            this.txtName.TabIndex = 68;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblName.AntiAliasingEnabled = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.CustomFontName = "Junction";
            this.lblName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(3, 69);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(587, 17);
            this.lblName.TabIndex = 67;
            this.lblName.Text = "Name :";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AntiAliasingEnabled = true;
            this.btnSave.BorderColor = System.Drawing.Color.DimGray;
            this.btnSave.BorderWidth = 1;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.CustomFontName = "Junction";
            this.btnSave.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnSave.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnSave.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnSave.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSave.Enabled = false;
            this.btnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.FontCollection = this.fontCollection;
            this.btnSave.FontColor = System.Drawing.Color.Black;
            this.btnSave.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnSave.GradientColor2 = System.Drawing.Color.Gray;
            this.btnSave.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnSave.Image = global::MPfm.Properties.Resources.disk;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSave.Location = new System.Drawing.Point(398, 340);
            this.btnSave.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnSave.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnSave.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnSave.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 45);
            this.btnSave.TabIndex = 65;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AntiAliasingEnabled = true;
            this.btnClose.BorderColor = System.Drawing.Color.DimGray;
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
            this.btnClose.Location = new System.Drawing.Point(495, 340);
            this.btnClose.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnClose.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnClose.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnClose.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 45);
            this.btnClose.TabIndex = 63;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmAddEditMarker
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(593, 394);
            this.ControlBox = false;
            this.Controls.Add(this.panelEditMarker);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(400, 245);
            this.Name = "frmAddEditMarker";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Marker";
            this.panelEditMarker.ResumeLayout(false);
            this.panelEditMarker.PerformLayout();
            this.panelWarning.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private WindowsControls.Label lblPositionPCMValue;
        private WindowsControls.Label lblPositionPCM;
        private WindowsControls.Button btnPunchIn;
        private WindowsControls.Button btnGoTo;
        private WindowsControls.Label lblPositionPCMBytesValue;
        private WindowsControls.Label lblPositionPCMBytes;
        private WindowsControls.Label lblSongValue;
        private WindowsControls.Label lblSong;
        private WindowsControls.Panel panelWarning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private WindowsControls.Label lblWarning;
        public WindowsControls.WaveFormDisplay waveForm;
        private WindowsControls.Label lblPositionMSValue;
        private WindowsControls.Label lblPositionMS;
        private System.Windows.Forms.TextBox txtComments;
        private WindowsControls.Label lblComments;        
    }
}