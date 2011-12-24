namespace MPfm
{
    partial class frmAddEditLoop
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
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.panelEditLoop = new MPfm.WindowsControls.Panel();
            this.btnEndPositionGoTo = new MPfm.WindowsControls.Button();
            this.btnEndPositionPunchIn = new MPfm.WindowsControls.Button();
            this.label1 = new MPfm.WindowsControls.Label();
            this.txtEndPosition = new System.Windows.Forms.MaskedTextBox();
            this.comboEndPositionMarker = new System.Windows.Forms.ComboBox();
            this.lblEndPositionMarker = new MPfm.WindowsControls.Label();
            this.btnStartPositionGoTo = new MPfm.WindowsControls.Button();
            this.btnStartPositionPunchIn = new MPfm.WindowsControls.Button();
            this.lblPosition = new MPfm.WindowsControls.Label();
            this.txtStartPosition = new System.Windows.Forms.MaskedTextBox();
            this.panelWarning = new MPfm.WindowsControls.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblWarning = new MPfm.WindowsControls.Label();
            this.lblLoopLengthPCMBytesValue = new MPfm.WindowsControls.Label();
            this.lblLoopLengthPCMBytes = new MPfm.WindowsControls.Label();
            this.lblLoopLengthPCMValue = new MPfm.WindowsControls.Label();
            this.lblLoopLengthPCM = new MPfm.WindowsControls.Label();
            this.waveForm = new MPfm.WindowsControls.WaveFormDisplay();
            this.lblLoopLengthValue = new MPfm.WindowsControls.Label();
            this.lblLoopLength = new MPfm.WindowsControls.Label();
            this.comboStartPositionMarker = new System.Windows.Forms.ComboBox();
            this.lblStartPositionMarker = new MPfm.WindowsControls.Label();
            this.lblSongValue = new MPfm.WindowsControls.Label();
            this.lblSong = new MPfm.WindowsControls.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new MPfm.WindowsControls.Label();
            this.btnSave = new MPfm.WindowsControls.Button();
            this.btnClose = new MPfm.WindowsControls.Button();
            this.panelEditLoop.SuspendLayout();
            this.panelWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
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
            // panelEditLoop
            // 
            this.panelEditLoop.AntiAliasingEnabled = true;
            this.panelEditLoop.Controls.Add(this.btnEndPositionGoTo);
            this.panelEditLoop.Controls.Add(this.btnEndPositionPunchIn);
            this.panelEditLoop.Controls.Add(this.label1);
            this.panelEditLoop.Controls.Add(this.txtEndPosition);
            this.panelEditLoop.Controls.Add(this.comboEndPositionMarker);
            this.panelEditLoop.Controls.Add(this.lblEndPositionMarker);
            this.panelEditLoop.Controls.Add(this.btnStartPositionGoTo);
            this.panelEditLoop.Controls.Add(this.btnStartPositionPunchIn);
            this.panelEditLoop.Controls.Add(this.lblPosition);
            this.panelEditLoop.Controls.Add(this.txtStartPosition);
            this.panelEditLoop.Controls.Add(this.panelWarning);
            this.panelEditLoop.Controls.Add(this.lblLoopLengthPCMBytesValue);
            this.panelEditLoop.Controls.Add(this.lblLoopLengthPCMBytes);
            this.panelEditLoop.Controls.Add(this.lblLoopLengthPCMValue);
            this.panelEditLoop.Controls.Add(this.lblLoopLengthPCM);
            this.panelEditLoop.Controls.Add(this.waveForm);
            this.panelEditLoop.Controls.Add(this.lblLoopLengthValue);
            this.panelEditLoop.Controls.Add(this.lblLoopLength);
            this.panelEditLoop.Controls.Add(this.comboStartPositionMarker);
            this.panelEditLoop.Controls.Add(this.lblStartPositionMarker);
            this.panelEditLoop.Controls.Add(this.lblSongValue);
            this.panelEditLoop.Controls.Add(this.lblSong);
            this.panelEditLoop.Controls.Add(this.txtName);
            this.panelEditLoop.Controls.Add(this.lblName);
            this.panelEditLoop.Controls.Add(this.btnSave);
            this.panelEditLoop.Controls.Add(this.btnClose);
            this.panelEditLoop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEditLoop.ExpandedHeight = 200;
            this.panelEditLoop.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelEditLoop.FontCollection = this.fontCollection;
            this.panelEditLoop.GradientColor1 = System.Drawing.Color.Silver;
            this.panelEditLoop.GradientColor2 = System.Drawing.Color.Gray;
            this.panelEditLoop.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelEditLoop.HeaderCustomFontName = "TitilliumText22L Lt";
            this.panelEditLoop.HeaderExpandable = false;
            this.panelEditLoop.HeaderExpanded = true;
            this.panelEditLoop.HeaderForeColor = System.Drawing.Color.Black;
            this.panelEditLoop.HeaderGradientColor1 = System.Drawing.Color.LightGray;
            this.panelEditLoop.HeaderGradientColor2 = System.Drawing.Color.Gray;
            this.panelEditLoop.HeaderHeight = 30;
            this.panelEditLoop.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelEditLoop.HeaderTitle = "Edit Loop";
            this.panelEditLoop.Location = new System.Drawing.Point(0, 0);
            this.panelEditLoop.Name = "panelEditLoop";
            this.panelEditLoop.Size = new System.Drawing.Size(592, 373);
            this.panelEditLoop.TabIndex = 65;
            // 
            // btnEndPositionGoTo
            // 
            this.btnEndPositionGoTo.AntiAliasingEnabled = true;
            this.btnEndPositionGoTo.BorderColor = System.Drawing.Color.DimGray;
            this.btnEndPositionGoTo.BorderWidth = 1;
            this.btnEndPositionGoTo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEndPositionGoTo.CustomFontName = "Junction";
            this.btnEndPositionGoTo.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnEndPositionGoTo.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnEndPositionGoTo.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnEndPositionGoTo.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnEndPositionGoTo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEndPositionGoTo.FontCollection = this.fontCollection;
            this.btnEndPositionGoTo.FontColor = System.Drawing.Color.Black;
            this.btnEndPositionGoTo.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnEndPositionGoTo.GradientColor2 = System.Drawing.Color.Gray;
            this.btnEndPositionGoTo.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnEndPositionGoTo.Image = global::MPfm.Properties.Resources.arrow_right;
            this.btnEndPositionGoTo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEndPositionGoTo.Location = new System.Drawing.Point(211, 189);
            this.btnEndPositionGoTo.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnEndPositionGoTo.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnEndPositionGoTo.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnEndPositionGoTo.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnEndPositionGoTo.Name = "btnEndPositionGoTo";
            this.btnEndPositionGoTo.Size = new System.Drawing.Size(60, 30);
            this.btnEndPositionGoTo.TabIndex = 110;
            this.btnEndPositionGoTo.Text = "Go to";
            this.btnEndPositionGoTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEndPositionGoTo.UseVisualStyleBackColor = true;
            this.btnEndPositionGoTo.Click += new System.EventHandler(this.btnEndPositionGoTo_Click);
            // 
            // btnEndPositionPunchIn
            // 
            this.btnEndPositionPunchIn.AntiAliasingEnabled = true;
            this.btnEndPositionPunchIn.BorderColor = System.Drawing.Color.DimGray;
            this.btnEndPositionPunchIn.BorderWidth = 1;
            this.btnEndPositionPunchIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEndPositionPunchIn.CustomFontName = "Junction";
            this.btnEndPositionPunchIn.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnEndPositionPunchIn.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnEndPositionPunchIn.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnEndPositionPunchIn.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnEndPositionPunchIn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEndPositionPunchIn.FontCollection = this.fontCollection;
            this.btnEndPositionPunchIn.FontColor = System.Drawing.Color.Black;
            this.btnEndPositionPunchIn.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnEndPositionPunchIn.GradientColor2 = System.Drawing.Color.Gray;
            this.btnEndPositionPunchIn.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnEndPositionPunchIn.Image = global::MPfm.Properties.Resources.time;
            this.btnEndPositionPunchIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEndPositionPunchIn.Location = new System.Drawing.Point(129, 189);
            this.btnEndPositionPunchIn.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnEndPositionPunchIn.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnEndPositionPunchIn.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnEndPositionPunchIn.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnEndPositionPunchIn.Name = "btnEndPositionPunchIn";
            this.btnEndPositionPunchIn.Size = new System.Drawing.Size(76, 30);
            this.btnEndPositionPunchIn.TabIndex = 109;
            this.btnEndPositionPunchIn.Text = "Punch in";
            this.btnEndPositionPunchIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEndPositionPunchIn.UseVisualStyleBackColor = true;
            this.btnEndPositionPunchIn.Click += new System.EventHandler(this.btnEndPositionPunchIn_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AntiAliasingEnabled = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.CustomFontName = "Junction";
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.FontCollection = this.fontCollection;
            this.label1.Location = new System.Drawing.Point(3, 169);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 17);
            this.label1.TabIndex = 108;
            this.label1.Text = "End Position :";
            // 
            // txtEndPosition
            // 
            this.txtEndPosition.Font = new System.Drawing.Font("Droid Sans Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEndPosition.Location = new System.Drawing.Point(6, 189);
            this.txtEndPosition.Mask = "0:00.000";
            this.txtEndPosition.Name = "txtEndPosition";
            this.txtEndPosition.Size = new System.Drawing.Size(115, 30);
            this.txtEndPosition.TabIndex = 107;
            this.txtEndPosition.Text = "000000";
            this.txtEndPosition.TextChanged += new System.EventHandler(this.txtEndPosition_TextChanged);
            // 
            // comboEndPositionMarker
            // 
            this.comboEndPositionMarker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboEndPositionMarker.DisplayMember = "Name";
            this.comboEndPositionMarker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEndPositionMarker.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboEndPositionMarker.FormattingEnabled = true;
            this.comboEndPositionMarker.Location = new System.Drawing.Point(371, 192);
            this.comboEndPositionMarker.Name = "comboEndPositionMarker";
            this.comboEndPositionMarker.Size = new System.Drawing.Size(213, 23);
            this.comboEndPositionMarker.TabIndex = 105;
            this.comboEndPositionMarker.ValueMember = "MarkerId";
            this.comboEndPositionMarker.SelectedIndexChanged += new System.EventHandler(this.comboEndPositionMarker_SelectedIndexChanged);
            // 
            // lblEndPositionMarker
            // 
            this.lblEndPositionMarker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEndPositionMarker.AntiAliasingEnabled = true;
            this.lblEndPositionMarker.BackColor = System.Drawing.Color.Transparent;
            this.lblEndPositionMarker.CustomFontName = "Junction";
            this.lblEndPositionMarker.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEndPositionMarker.FontCollection = this.fontCollection;
            this.lblEndPositionMarker.Location = new System.Drawing.Point(275, 194);
            this.lblEndPositionMarker.Name = "lblEndPositionMarker";
            this.lblEndPositionMarker.Size = new System.Drawing.Size(93, 17);
            this.lblEndPositionMarker.TabIndex = 104;
            this.lblEndPositionMarker.Text = "Related marker :";
            // 
            // btnStartPositionGoTo
            // 
            this.btnStartPositionGoTo.AntiAliasingEnabled = true;
            this.btnStartPositionGoTo.BorderColor = System.Drawing.Color.DimGray;
            this.btnStartPositionGoTo.BorderWidth = 1;
            this.btnStartPositionGoTo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartPositionGoTo.CustomFontName = "Junction";
            this.btnStartPositionGoTo.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnStartPositionGoTo.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnStartPositionGoTo.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnStartPositionGoTo.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnStartPositionGoTo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartPositionGoTo.FontCollection = this.fontCollection;
            this.btnStartPositionGoTo.FontColor = System.Drawing.Color.Black;
            this.btnStartPositionGoTo.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnStartPositionGoTo.GradientColor2 = System.Drawing.Color.Gray;
            this.btnStartPositionGoTo.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnStartPositionGoTo.Image = global::MPfm.Properties.Resources.arrow_right;
            this.btnStartPositionGoTo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartPositionGoTo.Location = new System.Drawing.Point(211, 135);
            this.btnStartPositionGoTo.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnStartPositionGoTo.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnStartPositionGoTo.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnStartPositionGoTo.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnStartPositionGoTo.Name = "btnStartPositionGoTo";
            this.btnStartPositionGoTo.Size = new System.Drawing.Size(60, 30);
            this.btnStartPositionGoTo.TabIndex = 103;
            this.btnStartPositionGoTo.Text = "Go to";
            this.btnStartPositionGoTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStartPositionGoTo.UseVisualStyleBackColor = true;
            this.btnStartPositionGoTo.Click += new System.EventHandler(this.btnStartPositionGoTo_Click);
            // 
            // btnStartPositionPunchIn
            // 
            this.btnStartPositionPunchIn.AntiAliasingEnabled = true;
            this.btnStartPositionPunchIn.BorderColor = System.Drawing.Color.DimGray;
            this.btnStartPositionPunchIn.BorderWidth = 1;
            this.btnStartPositionPunchIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartPositionPunchIn.CustomFontName = "Junction";
            this.btnStartPositionPunchIn.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnStartPositionPunchIn.DisabledFontColor = System.Drawing.Color.Gray;
            this.btnStartPositionPunchIn.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnStartPositionPunchIn.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnStartPositionPunchIn.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartPositionPunchIn.FontCollection = this.fontCollection;
            this.btnStartPositionPunchIn.FontColor = System.Drawing.Color.Black;
            this.btnStartPositionPunchIn.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnStartPositionPunchIn.GradientColor2 = System.Drawing.Color.Gray;
            this.btnStartPositionPunchIn.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnStartPositionPunchIn.Image = global::MPfm.Properties.Resources.time;
            this.btnStartPositionPunchIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartPositionPunchIn.Location = new System.Drawing.Point(129, 135);
            this.btnStartPositionPunchIn.MouseOverBorderColor = System.Drawing.Color.DimGray;
            this.btnStartPositionPunchIn.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnStartPositionPunchIn.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnStartPositionPunchIn.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnStartPositionPunchIn.Name = "btnStartPositionPunchIn";
            this.btnStartPositionPunchIn.Size = new System.Drawing.Size(76, 30);
            this.btnStartPositionPunchIn.TabIndex = 102;
            this.btnStartPositionPunchIn.Text = "Punch in";
            this.btnStartPositionPunchIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStartPositionPunchIn.UseVisualStyleBackColor = true;
            this.btnStartPositionPunchIn.Click += new System.EventHandler(this.btnStartPositionPunchIn_Click);
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
            this.lblPosition.Location = new System.Drawing.Point(3, 115);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(118, 17);
            this.lblPosition.TabIndex = 99;
            this.lblPosition.Text = "Start Position :";
            // 
            // txtStartPosition
            // 
            this.txtStartPosition.Font = new System.Drawing.Font("Droid Sans Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStartPosition.Location = new System.Drawing.Point(6, 135);
            this.txtStartPosition.Mask = "0:00.000";
            this.txtStartPosition.Name = "txtStartPosition";
            this.txtStartPosition.Size = new System.Drawing.Size(115, 30);
            this.txtStartPosition.TabIndex = 98;
            this.txtStartPosition.Text = "000000";
            this.txtStartPosition.TextChanged += new System.EventHandler(this.txtStartPosition_TextChanged);
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
            this.panelWarning.Location = new System.Drawing.Point(6, 323);
            this.panelWarning.Name = "panelWarning";
            this.panelWarning.Size = new System.Drawing.Size(382, 44);
            this.panelWarning.TabIndex = 90;
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
            this.lblWarning.FontCollection = this.fontCollection;
            this.lblWarning.Location = new System.Drawing.Point(24, 20);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(334, 17);
            this.lblWarning.TabIndex = 91;
            this.lblWarning.Text = "The loop length must be positive.";
            // 
            // lblLoopLengthPCMBytesValue
            // 
            this.lblLoopLengthPCMBytesValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthPCMBytesValue.AntiAliasingEnabled = true;
            this.lblLoopLengthPCMBytesValue.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthPCMBytesValue.CustomFontName = "";
            this.lblLoopLengthPCMBytesValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthPCMBytesValue.FontCollection = this.fontCollection;
            this.lblLoopLengthPCMBytesValue.Location = new System.Drawing.Point(255, 344);
            this.lblLoopLengthPCMBytesValue.Name = "lblLoopLengthPCMBytesValue";
            this.lblLoopLengthPCMBytesValue.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLengthPCMBytesValue.TabIndex = 97;
            this.lblLoopLengthPCMBytesValue.Text = "0";
            // 
            // lblLoopLengthPCMBytes
            // 
            this.lblLoopLengthPCMBytes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthPCMBytes.AntiAliasingEnabled = true;
            this.lblLoopLengthPCMBytes.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthPCMBytes.CustomFontName = "Junction";
            this.lblLoopLengthPCMBytes.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthPCMBytes.FontCollection = this.fontCollection;
            this.lblLoopLengthPCMBytes.Location = new System.Drawing.Point(255, 327);
            this.lblLoopLengthPCMBytes.Name = "lblLoopLengthPCMBytes";
            this.lblLoopLengthPCMBytes.Size = new System.Drawing.Size(110, 17);
            this.lblLoopLengthPCMBytes.TabIndex = 96;
            this.lblLoopLengthPCMBytes.Text = "Loop Length (bytes)";
            // 
            // lblLoopLengthPCMValue
            // 
            this.lblLoopLengthPCMValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthPCMValue.AntiAliasingEnabled = true;
            this.lblLoopLengthPCMValue.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthPCMValue.CustomFontName = "";
            this.lblLoopLengthPCMValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthPCMValue.FontCollection = this.fontCollection;
            this.lblLoopLengthPCMValue.Location = new System.Drawing.Point(107, 344);
            this.lblLoopLengthPCMValue.Name = "lblLoopLengthPCMValue";
            this.lblLoopLengthPCMValue.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLengthPCMValue.TabIndex = 95;
            this.lblLoopLengthPCMValue.Text = "0";
            // 
            // lblLoopLengthPCM
            // 
            this.lblLoopLengthPCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthPCM.AntiAliasingEnabled = true;
            this.lblLoopLengthPCM.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthPCM.CustomFontName = "Junction";
            this.lblLoopLengthPCM.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthPCM.FontCollection = this.fontCollection;
            this.lblLoopLengthPCM.Location = new System.Drawing.Point(107, 327);
            this.lblLoopLengthPCM.Name = "lblLoopLengthPCM";
            this.lblLoopLengthPCM.Size = new System.Drawing.Size(130, 17);
            this.lblLoopLengthPCM.TabIndex = 94;
            this.lblLoopLengthPCM.Text = "Loop Length (samples)";
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
            this.waveForm.Location = new System.Drawing.Point(6, 225);
            this.waveForm.Name = "waveForm";
            this.waveForm.PeakFileDirectory = "C:\\Users\\Animal Mother\\AppData\\Local\\Microsoft\\VisualStudio\\10.0\\ProjectAssemblie" +
    "s\\om-0gycd01\\Peak Files\\";
            this.waveForm.Size = new System.Drawing.Size(578, 92);
            this.waveForm.TabIndex = 93;
            this.waveForm.WaveFormColor = System.Drawing.Color.Yellow;
            this.waveForm.Zoom = 100F;
            // 
            // lblLoopLengthValue
            // 
            this.lblLoopLengthValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLengthValue.AntiAliasingEnabled = true;
            this.lblLoopLengthValue.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLengthValue.CustomFontName = "";
            this.lblLoopLengthValue.Font = new System.Drawing.Font("Droid Sans Mono", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLengthValue.FontCollection = this.fontCollection;
            this.lblLoopLengthValue.Location = new System.Drawing.Point(2, 344);
            this.lblLoopLengthValue.Name = "lblLoopLengthValue";
            this.lblLoopLengthValue.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLengthValue.TabIndex = 75;
            this.lblLoopLengthValue.Text = "0";
            // 
            // lblLoopLength
            // 
            this.lblLoopLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLoopLength.AntiAliasingEnabled = true;
            this.lblLoopLength.BackColor = System.Drawing.Color.Transparent;
            this.lblLoopLength.CustomFontName = "Junction";
            this.lblLoopLength.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoopLength.FontCollection = this.fontCollection;
            this.lblLoopLength.Location = new System.Drawing.Point(2, 327);
            this.lblLoopLength.Name = "lblLoopLength";
            this.lblLoopLength.Size = new System.Drawing.Size(89, 17);
            this.lblLoopLength.TabIndex = 74;
            this.lblLoopLength.Text = "Loop Length";
            // 
            // comboStartPositionMarker
            // 
            this.comboStartPositionMarker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboStartPositionMarker.DisplayMember = "Name";
            this.comboStartPositionMarker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStartPositionMarker.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboStartPositionMarker.FormattingEnabled = true;
            this.comboStartPositionMarker.Location = new System.Drawing.Point(371, 138);
            this.comboStartPositionMarker.Name = "comboStartPositionMarker";
            this.comboStartPositionMarker.Size = new System.Drawing.Size(213, 23);
            this.comboStartPositionMarker.TabIndex = 86;
            this.comboStartPositionMarker.ValueMember = "MarkerId";
            this.comboStartPositionMarker.SelectedIndexChanged += new System.EventHandler(this.comboStartPositionMarker_SelectedIndexChanged);
            // 
            // lblStartPositionMarker
            // 
            this.lblStartPositionMarker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStartPositionMarker.AntiAliasingEnabled = true;
            this.lblStartPositionMarker.BackColor = System.Drawing.Color.Transparent;
            this.lblStartPositionMarker.CustomFontName = "Junction";
            this.lblStartPositionMarker.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStartPositionMarker.FontCollection = this.fontCollection;
            this.lblStartPositionMarker.Location = new System.Drawing.Point(275, 140);
            this.lblStartPositionMarker.Name = "lblStartPositionMarker";
            this.lblStartPositionMarker.Size = new System.Drawing.Size(93, 17);
            this.lblStartPositionMarker.TabIndex = 82;
            this.lblStartPositionMarker.Text = "Related marker :";
            // 
            // lblSongValue
            // 
            this.lblSongValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSongValue.AntiAliasingEnabled = true;
            this.lblSongValue.BackColor = System.Drawing.Color.Transparent;
            this.lblSongValue.CustomFontName = "Junction";
            this.lblSongValue.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSongValue.FontCollection = this.fontCollection;
            this.lblSongValue.Location = new System.Drawing.Point(3, 52);
            this.lblSongValue.Name = "lblSongValue";
            this.lblSongValue.Size = new System.Drawing.Size(581, 17);
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
            this.lblSong.FontCollection = this.fontCollection;
            this.lblSong.Location = new System.Drawing.Point(3, 34);
            this.lblSong.Name = "lblSong";
            this.lblSong.Size = new System.Drawing.Size(581, 17);
            this.lblSong.TabIndex = 78;
            this.lblSong.Text = "Song :";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(6, 91);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(578, 21);
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
            this.lblName.FontCollection = this.fontCollection;
            this.lblName.Location = new System.Drawing.Point(3, 71);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(586, 17);
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
            this.btnSave.Location = new System.Drawing.Point(394, 323);
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
            this.btnClose.Location = new System.Drawing.Point(492, 323);
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
            // frmAddEditLoop
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(592, 373);
            this.ControlBox = false;
            this.Controls.Add(this.panelEditLoop);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(400, 245);
            this.Name = "frmAddEditLoop";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Loop";
            this.panelEditLoop.ResumeLayout(false);
            this.panelEditLoop.PerformLayout();
            this.panelWarning.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MPfm.WindowsControls.Button btnClose;
        private MPfm.WindowsControls.Panel panelEditLoop;
        private MPfm.WindowsControls.FontCollection fontCollection;
        private WindowsControls.Button btnSave;
        private WindowsControls.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private WindowsControls.Label lblSongValue;
        private WindowsControls.Label lblSong;
        private System.Windows.Forms.ComboBox comboStartPositionMarker;
        private WindowsControls.Label lblStartPositionMarker;
        private WindowsControls.Label lblLoopLengthValue;
        private WindowsControls.Label lblLoopLength;
        private WindowsControls.Panel panelWarning;
        private System.Windows.Forms.PictureBox pictureBox1;
        private WindowsControls.Label lblWarning;
        public WindowsControls.WaveFormDisplay waveForm;
        private WindowsControls.Label lblLoopLengthPCMBytesValue;
        private WindowsControls.Label lblLoopLengthPCMBytes;
        private WindowsControls.Label lblLoopLengthPCMValue;
        private WindowsControls.Label lblLoopLengthPCM;
        private WindowsControls.Button btnStartPositionGoTo;
        private WindowsControls.Button btnStartPositionPunchIn;
        private WindowsControls.Label lblPosition;
        private System.Windows.Forms.MaskedTextBox txtStartPosition;
        private WindowsControls.Button btnEndPositionGoTo;
        private WindowsControls.Button btnEndPositionPunchIn;
        private WindowsControls.Label label1;
        private System.Windows.Forms.MaskedTextBox txtEndPosition;
        private System.Windows.Forms.ComboBox comboEndPositionMarker;
        private WindowsControls.Label lblEndPositionMarker;        
    }
}