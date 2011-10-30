namespace MPfm
{
    partial class frmFirstRun
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFirstRun));
            this.fontCollection = new MPfm.WindowsControls.FontCollection();
            this.lblName = new MPfm.WindowsControls.Label();
            this.Label1 = new MPfm.WindowsControls.Label();
            this.panelWelcome = new MPfm.WindowsControls.Panel();
            this.label11 = new MPfm.WindowsControls.Label();
            this.label12 = new MPfm.WindowsControls.Label();
            this.label2 = new MPfm.WindowsControls.Label();
            this.label3 = new MPfm.WindowsControls.Label();
            this.label10 = new MPfm.WindowsControls.Label();
            this.Label9 = new MPfm.WindowsControls.Label();
            this.btnTestAudioSettings = new MPfm.WindowsControls.Button();
            this.Panel1 = new MPfm.WindowsControls.Panel();
            this.Label5 = new MPfm.WindowsControls.Label();
            this.Label6 = new MPfm.WindowsControls.Label();
            this.Label7 = new MPfm.WindowsControls.Label();
            this.Label8 = new MPfm.WindowsControls.Label();
            this.Label4 = new MPfm.WindowsControls.Label();
            this.cboOutputDevices = new System.Windows.Forms.ComboBox();
            this.cboDrivers = new System.Windows.Forms.ComboBox();
            this.panelError = new MPfm.WindowsControls.Panel();
            this.btnErrorCopyToClipboard = new MPfm.WindowsControls.Button();
            this.btnErrorSendEmail = new MPfm.WindowsControls.Button();
            this.btnErrorExitPMP = new MPfm.WindowsControls.Button();
            this.txtError = new System.Windows.Forms.RichTextBox();
            this.lblError = new MPfm.WindowsControls.Label();
            this.btnNext = new MPfm.WindowsControls.Button();
            this.btnCancelWizard = new MPfm.WindowsControls.Button();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.panelWelcome.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.panelError.SuspendLayout();
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
            this.fontCollection.Fonts.Add(customFont1);
            this.fontCollection.Fonts.Add(customFont2);
            this.fontCollection.Fonts.Add(customFont3);
            this.fontCollection.Fonts.Add(customFont4);
            // 
            // lblName
            // 
            this.lblName.AntiAliasingEnabled = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.CustomFontName = "TitilliumText22L Lt";
            this.lblName.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.FontCollection = this.fontCollection;
            this.lblName.Location = new System.Drawing.Point(12, 32);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(264, 32);
            this.lblName.TabIndex = 74;
            this.lblName.Text = "Welcome to MPfm!";
            // 
            // Label1
            // 
            this.Label1.AntiAliasingEnabled = true;
            this.Label1.BackColor = System.Drawing.Color.Transparent;
            this.Label1.CustomFontName = "Junction";
            this.Label1.Font = new System.Drawing.Font("MingLiU", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.FontCollection = this.fontCollection;
            this.Label1.ForeColor = System.Drawing.Color.Gainsboro;
            this.Label1.Location = new System.Drawing.Point(13, 64);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(469, 26);
            this.Label1.TabIndex = 75;
            this.Label1.Text = "This wizard will help you set your initial configuration.";
            // 
            // panelWelcome
            // 
            this.panelWelcome.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelWelcome.AntiAliasingEnabled = true;
            this.panelWelcome.Controls.Add(this.label11);
            this.panelWelcome.Controls.Add(this.label12);
            this.panelWelcome.Controls.Add(this.label2);
            this.panelWelcome.Controls.Add(this.label3);
            this.panelWelcome.Controls.Add(this.label10);
            this.panelWelcome.Controls.Add(this.Label9);
            this.panelWelcome.Controls.Add(this.btnTestAudioSettings);
            this.panelWelcome.Controls.Add(this.Panel1);
            this.panelWelcome.Controls.Add(this.Label8);
            this.panelWelcome.Controls.Add(this.Label4);
            this.panelWelcome.Controls.Add(this.cboOutputDevices);
            this.panelWelcome.Controls.Add(this.cboDrivers);
            this.panelWelcome.Controls.Add(this.Label1);
            this.panelWelcome.Controls.Add(this.lblName);
            this.panelWelcome.ExpandedHeight = 200;
            this.panelWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelWelcome.FontCollection = this.fontCollection;
            this.panelWelcome.ForeColor = System.Drawing.Color.White;
            this.panelWelcome.GradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.panelWelcome.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panelWelcome.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelWelcome.HeaderCustomFontName = "Junction";
            this.panelWelcome.HeaderExpandable = false;
            this.panelWelcome.HeaderExpanded = true;
            this.panelWelcome.HeaderForeColor = System.Drawing.Color.White;
            this.panelWelcome.HeaderGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.panelWelcome.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.panelWelcome.HeaderHeight = 22;
            this.panelWelcome.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelWelcome.HeaderTitle = "MPfm Initial Configuration Wizard";
            this.panelWelcome.Location = new System.Drawing.Point(0, 0);
            this.panelWelcome.Name = "panelWelcome";
            this.panelWelcome.Size = new System.Drawing.Size(620, 430);
            this.panelWelcome.TabIndex = 77;
            this.panelWelcome.Text = "CustomTextBox Disabled";
            // 
            // label11
            // 
            this.label11.AntiAliasingEnabled = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.CustomFontName = "TitilliumText22L Lt";
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.FontCollection = this.fontCollection;
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(13, 301);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(18, 20);
            this.label11.TabIndex = 93;
            this.label11.Text = "2.";
            // 
            // label12
            // 
            this.label12.AntiAliasingEnabled = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.CustomFontName = "Junction";
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.FontCollection = this.fontCollection;
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(28, 301);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(257, 20);
            this.label12.TabIndex = 92;
            this.label12.Text = "Please select an output device :";
            // 
            // label2
            // 
            this.label2.AntiAliasingEnabled = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.CustomFontName = "TitilliumText22L Lt";
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.FontCollection = this.fontCollection;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(14, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 20);
            this.label2.TabIndex = 91;
            this.label2.Text = "1.";
            // 
            // label3
            // 
            this.label3.AntiAliasingEnabled = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.CustomFontName = "Junction";
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.FontCollection = this.fontCollection;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(29, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(195, 20);
            this.label3.TabIndex = 90;
            this.label3.Text = "Please select a driver :";
            // 
            // label10
            // 
            this.label10.AntiAliasingEnabled = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.CustomFontName = "TitilliumText22L Lt";
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.FontCollection = this.fontCollection;
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(14, 353);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(18, 20);
            this.label10.TabIndex = 89;
            this.label10.Text = "3.";
            // 
            // Label9
            // 
            this.Label9.AntiAliasingEnabled = true;
            this.Label9.BackColor = System.Drawing.Color.Transparent;
            this.Label9.CustomFontName = "Junction";
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.FontCollection = this.fontCollection;
            this.Label9.ForeColor = System.Drawing.Color.White;
            this.Label9.Location = new System.Drawing.Point(29, 353);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(416, 20);
            this.Label9.TabIndex = 88;
            this.Label9.Text = "Test the selected audio settings by clicking this button :";
            // 
            // btnTestAudioSettings
            // 
            this.btnTestAudioSettings.AntiAliasingEnabled = true;
            this.btnTestAudioSettings.BorderColor = System.Drawing.Color.Black;
            this.btnTestAudioSettings.BorderWidth = 1;
            this.btnTestAudioSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestAudioSettings.CustomFontName = "Junction";
            this.btnTestAudioSettings.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnTestAudioSettings.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnTestAudioSettings.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnTestAudioSettings.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnTestAudioSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestAudioSettings.FontCollection = this.fontCollection;
            this.btnTestAudioSettings.FontColor = System.Drawing.Color.Black;
            this.btnTestAudioSettings.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnTestAudioSettings.GradientColor2 = System.Drawing.Color.Gray;
            this.btnTestAudioSettings.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnTestAudioSettings.Image = global::MPfm.Properties.Resources.sound;
            this.btnTestAudioSettings.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnTestAudioSettings.Location = new System.Drawing.Point(16, 378);
            this.btnTestAudioSettings.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnTestAudioSettings.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnTestAudioSettings.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnTestAudioSettings.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnTestAudioSettings.Name = "btnTestAudioSettings";
            this.btnTestAudioSettings.Size = new System.Drawing.Size(129, 40);
            this.btnTestAudioSettings.TabIndex = 80;
            this.btnTestAudioSettings.Text = "Test audio settings";
            this.btnTestAudioSettings.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnTestAudioSettings.UseVisualStyleBackColor = true;
            this.btnTestAudioSettings.Click += new System.EventHandler(this.btnTestAudioSettings_Click);
            // 
            // Panel1
            // 
            this.Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel1.AntiAliasingEnabled = true;
            this.Panel1.Controls.Add(this.Label5);
            this.Panel1.Controls.Add(this.Label6);
            this.Panel1.Controls.Add(this.Label7);
            this.Panel1.ExpandedHeight = 200;
            this.Panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Panel1.FontCollection = this.fontCollection;
            this.Panel1.ForeColor = System.Drawing.Color.White;
            this.Panel1.GradientColor1 = System.Drawing.Color.Gainsboro;
            this.Panel1.GradientColor2 = System.Drawing.Color.Silver;
            this.Panel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.Panel1.HeaderCustomFontName = "Junction";
            this.Panel1.HeaderExpandable = false;
            this.Panel1.HeaderExpanded = true;
            this.Panel1.HeaderForeColor = System.Drawing.Color.Black;
            this.Panel1.HeaderGradientColor1 = System.Drawing.Color.Silver;
            this.Panel1.HeaderGradientColor2 = System.Drawing.Color.DarkGray;
            this.Panel1.HeaderHeight = 22;
            this.Panel1.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Panel1.HeaderTitle = "Note :";
            this.Panel1.Location = new System.Drawing.Point(16, 198);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(591, 99);
            this.Panel1.TabIndex = 87;
            this.Panel1.Text = "CustomTextBox Disabled";
            // 
            // Label5
            // 
            this.Label5.AntiAliasingEnabled = true;
            this.Label5.BackColor = System.Drawing.Color.Transparent;
            this.Label5.CustomFontName = "Junction";
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.FontCollection = this.fontCollection;
            this.Label5.ForeColor = System.Drawing.Color.Black;
            this.Label5.Location = new System.Drawing.Point(5, 29);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(583, 20);
            this.Label5.TabIndex = 83;
            this.Label5.Text = "The recommended driver for Windows XP, Windows Vista, Windows 7 and Windows 8 is " +
    "DirectSound.";
            // 
            // Label6
            // 
            this.Label6.AntiAliasingEnabled = true;
            this.Label6.BackColor = System.Drawing.Color.Transparent;
            this.Label6.CustomFontName = "Junction";
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.FontCollection = this.fontCollection;
            this.Label6.ForeColor = System.Drawing.Color.Black;
            this.Label6.Location = new System.Drawing.Point(5, 48);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(583, 20);
            this.Label6.TabIndex = 84;
            this.Label6.Text = "The ASIO and WASAPI (Windows Audio Session API) drivers are also available for te" +
    "sting (Beta for now).";
            // 
            // Label7
            // 
            this.Label7.AntiAliasingEnabled = true;
            this.Label7.BackColor = System.Drawing.Color.Transparent;
            this.Label7.CustomFontName = "Junction";
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.FontCollection = this.fontCollection;
            this.Label7.ForeColor = System.Drawing.Color.Black;
            this.Label7.Location = new System.Drawing.Point(6, 67);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(582, 20);
            this.Label7.TabIndex = 85;
            this.Label7.Text = "Note: The ASIO driver will work better if you have a low latency sound card with " +
    "a true ASIO driver.";
            // 
            // Label8
            // 
            this.Label8.AntiAliasingEnabled = true;
            this.Label8.BackColor = System.Drawing.Color.Transparent;
            this.Label8.CustomFontName = "Junction";
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.FontCollection = this.fontCollection;
            this.Label8.ForeColor = System.Drawing.Color.LightGray;
            this.Label8.Location = new System.Drawing.Point(13, 115);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(601, 20);
            this.Label8.TabIndex = 86;
            this.Label8.Text = "You must select a driver and an output device. The default settings for your OS h" +
    "ave been selected.";
            // 
            // Label4
            // 
            this.Label4.AntiAliasingEnabled = true;
            this.Label4.BackColor = System.Drawing.Color.Transparent;
            this.Label4.CustomFontName = "TitilliumText22L Lt";
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.FontCollection = this.fontCollection;
            this.Label4.ForeColor = System.Drawing.Color.White;
            this.Label4.Location = new System.Drawing.Point(13, 92);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(211, 26);
            this.Label4.TabIndex = 82;
            this.Label4.Text = "Audio settings";
            // 
            // cboOutputDevices
            // 
            this.cboOutputDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOutputDevices.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboOutputDevices.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboOutputDevices.DisplayMember = "Name";
            this.cboOutputDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOutputDevices.FormattingEnabled = true;
            this.cboOutputDevices.Location = new System.Drawing.Point(16, 325);
            this.cboOutputDevices.Name = "cboOutputDevices";
            this.cboOutputDevices.Size = new System.Drawing.Size(591, 23);
            this.cboOutputDevices.TabIndex = 81;
            this.cboOutputDevices.ValueMember = "Id";
            this.cboOutputDevices.SelectedIndexChanged += new System.EventHandler(this.cboOutputDevices_SelectedIndexChanged);
            // 
            // cboDrivers
            // 
            this.cboDrivers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboDrivers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboDrivers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDrivers.DisplayMember = "Title";
            this.cboDrivers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDrivers.FormattingEnabled = true;
            this.cboDrivers.Location = new System.Drawing.Point(16, 168);
            this.cboDrivers.Name = "cboDrivers";
            this.cboDrivers.Size = new System.Drawing.Size(591, 23);
            this.cboDrivers.TabIndex = 77;
            this.cboDrivers.ValueMember = "DriverType";
            this.cboDrivers.SelectedIndexChanged += new System.EventHandler(this.cboDrivers_SelectedIndexChanged);
            // 
            // panelError
            // 
            this.panelError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelError.AntiAliasingEnabled = true;
            this.panelError.Controls.Add(this.btnErrorCopyToClipboard);
            this.panelError.Controls.Add(this.btnErrorSendEmail);
            this.panelError.Controls.Add(this.btnErrorExitPMP);
            this.panelError.Controls.Add(this.txtError);
            this.panelError.Controls.Add(this.lblError);
            this.panelError.ExpandedHeight = 200;
            this.panelError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelError.FontCollection = this.fontCollection;
            this.panelError.ForeColor = System.Drawing.Color.White;
            this.panelError.GradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.panelError.GradientColor2 = System.Drawing.Color.Maroon;
            this.panelError.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.panelError.HeaderCustomFontName = "Avenir";
            this.panelError.HeaderExpandable = false;
            this.panelError.HeaderExpanded = true;
            this.panelError.HeaderForeColor = System.Drawing.Color.White;
            this.panelError.HeaderGradientColor1 = System.Drawing.Color.Red;
            this.panelError.HeaderGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.panelError.HeaderHeight = 22;
            this.panelError.HeaderTextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.panelError.HeaderTitle = "Error starting wizard";
            this.panelError.Location = new System.Drawing.Point(645, 451);
            this.panelError.Name = "panelError";
            this.panelError.Size = new System.Drawing.Size(590, 227);
            this.panelError.TabIndex = 79;
            this.panelError.Text = "CustomTextBox Disabled";
            this.panelError.Visible = false;
            // 
            // btnErrorCopyToClipboard
            // 
            this.btnErrorCopyToClipboard.AntiAliasingEnabled = true;
            this.btnErrorCopyToClipboard.BorderColor = System.Drawing.Color.Black;
            this.btnErrorCopyToClipboard.BorderWidth = 1;
            this.btnErrorCopyToClipboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnErrorCopyToClipboard.CustomFontName = "Avenir";
            this.btnErrorCopyToClipboard.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnErrorCopyToClipboard.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnErrorCopyToClipboard.DisabledGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnErrorCopyToClipboard.DisabledGradientColor2 = System.Drawing.Color.Maroon;
            this.btnErrorCopyToClipboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnErrorCopyToClipboard.FontCollection = this.fontCollection;
            this.btnErrorCopyToClipboard.FontColor = System.Drawing.Color.White;
            this.btnErrorCopyToClipboard.GradientColor1 = System.Drawing.Color.DarkRed;
            this.btnErrorCopyToClipboard.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnErrorCopyToClipboard.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnErrorCopyToClipboard.Image = global::MPfm.Properties.Resources.page_copy;
            this.btnErrorCopyToClipboard.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnErrorCopyToClipboard.Location = new System.Drawing.Point(318, 180);
            this.btnErrorCopyToClipboard.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnErrorCopyToClipboard.MouseOverFontColor = System.Drawing.Color.White;
            this.btnErrorCopyToClipboard.MouseOverGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnErrorCopyToClipboard.MouseOverGradientColor2 = System.Drawing.Color.Maroon;
            this.btnErrorCopyToClipboard.Name = "btnErrorCopyToClipboard";
            this.btnErrorCopyToClipboard.Size = new System.Drawing.Size(123, 40);
            this.btnErrorCopyToClipboard.TabIndex = 81;
            this.btnErrorCopyToClipboard.Text = "Copy to clipboard";
            this.btnErrorCopyToClipboard.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnErrorCopyToClipboard.UseVisualStyleBackColor = true;
            this.btnErrorCopyToClipboard.Click += new System.EventHandler(this.btnErrorCopyToClipboard_Click);
            // 
            // btnErrorSendEmail
            // 
            this.btnErrorSendEmail.AntiAliasingEnabled = true;
            this.btnErrorSendEmail.BorderColor = System.Drawing.Color.Black;
            this.btnErrorSendEmail.BorderWidth = 1;
            this.btnErrorSendEmail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnErrorSendEmail.CustomFontName = "Avenir";
            this.btnErrorSendEmail.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnErrorSendEmail.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnErrorSendEmail.DisabledGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnErrorSendEmail.DisabledGradientColor2 = System.Drawing.Color.Maroon;
            this.btnErrorSendEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnErrorSendEmail.FontCollection = this.fontCollection;
            this.btnErrorSendEmail.FontColor = System.Drawing.Color.White;
            this.btnErrorSendEmail.GradientColor1 = System.Drawing.Color.DarkRed;
            this.btnErrorSendEmail.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnErrorSendEmail.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnErrorSendEmail.Image = global::MPfm.Properties.Resources.email;
            this.btnErrorSendEmail.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnErrorSendEmail.Location = new System.Drawing.Point(447, 180);
            this.btnErrorSendEmail.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnErrorSendEmail.MouseOverFontColor = System.Drawing.Color.White;
            this.btnErrorSendEmail.MouseOverGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnErrorSendEmail.MouseOverGradientColor2 = System.Drawing.Color.Maroon;
            this.btnErrorSendEmail.Name = "btnErrorSendEmail";
            this.btnErrorSendEmail.Size = new System.Drawing.Size(123, 40);
            this.btnErrorSendEmail.TabIndex = 80;
            this.btnErrorSendEmail.Text = "Send email to author";
            this.btnErrorSendEmail.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnErrorSendEmail.UseVisualStyleBackColor = true;
            this.btnErrorSendEmail.Click += new System.EventHandler(this.btnErrorSendEmail_Click);
            // 
            // btnErrorExitPMP
            // 
            this.btnErrorExitPMP.AntiAliasingEnabled = true;
            this.btnErrorExitPMP.BorderColor = System.Drawing.Color.Black;
            this.btnErrorExitPMP.BorderWidth = 1;
            this.btnErrorExitPMP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnErrorExitPMP.CustomFontName = "Avenir";
            this.btnErrorExitPMP.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnErrorExitPMP.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnErrorExitPMP.DisabledGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnErrorExitPMP.DisabledGradientColor2 = System.Drawing.Color.Maroon;
            this.btnErrorExitPMP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnErrorExitPMP.FontCollection = this.fontCollection;
            this.btnErrorExitPMP.FontColor = System.Drawing.Color.White;
            this.btnErrorExitPMP.ForeColor = System.Drawing.Color.White;
            this.btnErrorExitPMP.GradientColor1 = System.Drawing.Color.DarkRed;
            this.btnErrorExitPMP.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnErrorExitPMP.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnErrorExitPMP.Image = global::MPfm.Properties.Resources.door_in;
            this.btnErrorExitPMP.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnErrorExitPMP.Location = new System.Drawing.Point(576, 180);
            this.btnErrorExitPMP.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnErrorExitPMP.MouseOverFontColor = System.Drawing.Color.White;
            this.btnErrorExitPMP.MouseOverGradientColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnErrorExitPMP.MouseOverGradientColor2 = System.Drawing.Color.Maroon;
            this.btnErrorExitPMP.Name = "btnErrorExitPMP";
            this.btnErrorExitPMP.Size = new System.Drawing.Size(88, 40);
            this.btnErrorExitPMP.TabIndex = 78;
            this.btnErrorExitPMP.Text = "Exit PMP";
            this.btnErrorExitPMP.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnErrorExitPMP.UseVisualStyleBackColor = true;
            this.btnErrorExitPMP.Click += new System.EventHandler(this.btnErrorExitPMP_Click);
            // 
            // txtError
            // 
            this.txtError.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.txtError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtError.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtError.ForeColor = System.Drawing.Color.White;
            this.txtError.Location = new System.Drawing.Point(6, 54);
            this.txtError.Name = "txtError";
            this.txtError.ReadOnly = true;
            this.txtError.Size = new System.Drawing.Size(658, 120);
            this.txtError.TabIndex = 79;
            this.txtError.Text = "";
            // 
            // lblError
            // 
            this.lblError.AntiAliasingEnabled = true;
            this.lblError.BackColor = System.Drawing.Color.Transparent;
            this.lblError.CustomFontName = "Avenir";
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.FontCollection = this.fontCollection;
            this.lblError.ForeColor = System.Drawing.Color.LightGray;
            this.lblError.Location = new System.Drawing.Point(3, 25);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(383, 26);
            this.lblError.TabIndex = 78;
            this.lblError.Text = "An error has occured while detecting output devices:";
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.AntiAliasingEnabled = true;
            this.btnNext.BorderColor = System.Drawing.Color.Black;
            this.btnNext.BorderWidth = 1;
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.CustomFontName = "Junction";
            this.btnNext.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnNext.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnNext.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnNext.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnNext.Enabled = false;
            this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.FontCollection = this.fontCollection;
            this.btnNext.FontColor = System.Drawing.Color.Black;
            this.btnNext.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnNext.GradientColor2 = System.Drawing.Color.Gray;
            this.btnNext.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnNext.Image = global::MPfm.Properties.Resources.arrow_right;
            this.btnNext.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNext.Location = new System.Drawing.Point(447, 436);
            this.btnNext.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnNext.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnNext.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnNext.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(161, 40);
            this.btnNext.TabIndex = 73;
            this.btnNext.Text = "Save settings and continue";
            this.btnNext.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancelWizard
            // 
            this.btnCancelWizard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelWizard.AntiAliasingEnabled = true;
            this.btnCancelWizard.BorderColor = System.Drawing.Color.Black;
            this.btnCancelWizard.BorderWidth = 1;
            this.btnCancelWizard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelWizard.CustomFontName = "Junction";
            this.btnCancelWizard.DisabledBorderColor = System.Drawing.Color.Gray;
            this.btnCancelWizard.DisabledFontColor = System.Drawing.Color.Silver;
            this.btnCancelWizard.DisabledGradientColor1 = System.Drawing.Color.Gray;
            this.btnCancelWizard.DisabledGradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnCancelWizard.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelWizard.FontCollection = this.fontCollection;
            this.btnCancelWizard.FontColor = System.Drawing.Color.Black;
            this.btnCancelWizard.GradientColor1 = System.Drawing.Color.LightGray;
            this.btnCancelWizard.GradientColor2 = System.Drawing.Color.Gray;
            this.btnCancelWizard.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnCancelWizard.Image = global::MPfm.Properties.Resources.door_in;
            this.btnCancelWizard.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelWizard.Location = new System.Drawing.Point(296, 436);
            this.btnCancelWizard.MouseOverBorderColor = System.Drawing.Color.Black;
            this.btnCancelWizard.MouseOverFontColor = System.Drawing.Color.Black;
            this.btnCancelWizard.MouseOverGradientColor1 = System.Drawing.Color.White;
            this.btnCancelWizard.MouseOverGradientColor2 = System.Drawing.Color.DarkGray;
            this.btnCancelWizard.Name = "btnCancelWizard";
            this.btnCancelWizard.Size = new System.Drawing.Size(145, 40);
            this.btnCancelWizard.TabIndex = 78;
            this.btnCancelWizard.Text = "Cancel wizard and exit";
            this.btnCancelWizard.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelWizard.UseVisualStyleBackColor = true;
            this.btnCancelWizard.Click += new System.EventHandler(this.btnCancelWizard_Click);
            // 
            // openFile
            // 
            this.openFile.Filter = "Audio files (*.mp3, *.flac, *.ogg, *.wav)|*.mp3;*.flac;*.ogg;*.wav";
            this.openFile.Title = "Please select an audio file to play";
            // 
            // frmFirstRun
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(620, 485);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancelWizard);
            this.Controls.Add(this.panelWelcome);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.panelError);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.GradientColor1 = System.Drawing.Color.White;
            this.GradientColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(110)))), ((int)(((byte)(110)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFirstRun";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CustomTextBox Disabled";
            this.TopMost = true;
            this.panelWelcome.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.panelError.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MPfm.WindowsControls.FontCollection fontCollection;
        private MPfm.WindowsControls.Label lblName;
        private MPfm.WindowsControls.Button btnNext;
        private MPfm.WindowsControls.Label Label1;
        private MPfm.WindowsControls.Panel panelWelcome;
        private System.Windows.Forms.ComboBox cboDrivers;
        private MPfm.WindowsControls.Panel panelError;
        private System.Windows.Forms.RichTextBox txtError;
        private MPfm.WindowsControls.Label lblError;
        private MPfm.WindowsControls.Button btnErrorCopyToClipboard;
        private MPfm.WindowsControls.Button btnErrorSendEmail;
        private MPfm.WindowsControls.Button btnErrorExitPMP;
        private MPfm.WindowsControls.Button btnCancelWizard;
        private System.Windows.Forms.ComboBox cboOutputDevices;
        private MPfm.WindowsControls.Panel Panel1;
        private MPfm.WindowsControls.Label Label5;
        private MPfm.WindowsControls.Label Label6;
        private MPfm.WindowsControls.Label Label7;
        private MPfm.WindowsControls.Label Label8;
        private MPfm.WindowsControls.Label Label4;
        private MPfm.WindowsControls.Label Label9;
        private MPfm.WindowsControls.Button btnTestAudioSettings;
        private System.Windows.Forms.OpenFileDialog openFile;
        private MPfm.WindowsControls.Label label10;
        private MPfm.WindowsControls.Label label11;
        private MPfm.WindowsControls.Label label12;
        private MPfm.WindowsControls.Label label2;
        private MPfm.WindowsControls.Label label3;
    }
}