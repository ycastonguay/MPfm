namespace PlaybackEngineV4
{
    partial class frmSettings
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
            this.groupAudio = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtUpdateThreads = new System.Windows.Forms.MaskedTextBox();
            this.lblUpdateThreads = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUpdatePeriod = new System.Windows.Forms.MaskedTextBox();
            this.lblUpdatePeriod = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBufferSize = new System.Windows.Forms.MaskedTextBox();
            this.lblBufferSize = new System.Windows.Forms.Label();
            this.trackUpdateThreads = new System.Windows.Forms.TrackBar();
            this.trackUpdatePeriod = new System.Windows.Forms.TrackBar();
            this.trackBufferSize = new System.Windows.Forms.TrackBar();
            this.groupSoundCard = new System.Windows.Forms.GroupBox();
            this.btnTestAudio = new System.Windows.Forms.Button();
            this.lblDefaultValue = new System.Windows.Forms.Label();
            this.lblDefault = new System.Windows.Forms.Label();
            this.comboOutputDevice = new System.Windows.Forms.ComboBox();
            this.lblOutputDevice = new System.Windows.Forms.Label();
            this.comboDriver = new System.Windows.Forms.ComboBox();
            this.lblDriver = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.groupAudio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackUpdateThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackUpdatePeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBufferSize)).BeginInit();
            this.groupSoundCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupAudio
            // 
            this.groupAudio.Controls.Add(this.label3);
            this.groupAudio.Controls.Add(this.btnReset);
            this.groupAudio.Controls.Add(this.txtUpdateThreads);
            this.groupAudio.Controls.Add(this.lblUpdateThreads);
            this.groupAudio.Controls.Add(this.label2);
            this.groupAudio.Controls.Add(this.txtUpdatePeriod);
            this.groupAudio.Controls.Add(this.lblUpdatePeriod);
            this.groupAudio.Controls.Add(this.label1);
            this.groupAudio.Controls.Add(this.txtBufferSize);
            this.groupAudio.Controls.Add(this.lblBufferSize);
            this.groupAudio.Controls.Add(this.trackUpdateThreads);
            this.groupAudio.Controls.Add(this.trackUpdatePeriod);
            this.groupAudio.Controls.Add(this.trackBufferSize);
            this.groupAudio.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupAudio.Location = new System.Drawing.Point(4, 178);
            this.groupAudio.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupAudio.Name = "groupAudio";
            this.groupAudio.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupAudio.Size = new System.Drawing.Size(417, 135);
            this.groupAudio.TabIndex = 17;
            this.groupAudio.TabStop = false;
            this.groupAudio.Text = "Audio Settings";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(318, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 11);
            this.label3.TabIndex = 56;
            this.label3.Text = "thread(s)";
            // 
            // btnReset
            // 
            this.btnReset.Image = global::PlaybackEngineV4.Properties.Resources.asterisk_orange;
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(8, 99);
            this.btnReset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(209, 26);
            this.btnReset.TabIndex = 50;
            this.btnReset.Text = "Reset to default settings";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtUpdateThreads
            // 
            this.txtUpdateThreads.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUpdateThreads.Location = new System.Drawing.Point(265, 40);
            this.txtUpdateThreads.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUpdateThreads.Mask = "0";
            this.txtUpdateThreads.Name = "txtUpdateThreads";
            this.txtUpdateThreads.ReadOnly = true;
            this.txtUpdateThreads.Size = new System.Drawing.Size(47, 18);
            this.txtUpdateThreads.TabIndex = 55;
            this.txtUpdateThreads.Text = "1";
            // 
            // lblUpdateThreads
            // 
            this.lblUpdateThreads.AutoSize = true;
            this.lblUpdateThreads.Location = new System.Drawing.Point(263, 21);
            this.lblUpdateThreads.Name = "lblUpdateThreads";
            this.lblUpdateThreads.Size = new System.Drawing.Size(110, 11);
            this.lblUpdateThreads.TabIndex = 54;
            this.lblUpdateThreads.Text = "Update threads:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(217, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 11);
            this.label2.TabIndex = 52;
            this.label2.Text = "ms";
            // 
            // txtUpdatePeriod
            // 
            this.txtUpdatePeriod.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUpdatePeriod.Location = new System.Drawing.Point(137, 40);
            this.txtUpdatePeriod.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUpdatePeriod.Mask = "0000";
            this.txtUpdatePeriod.Name = "txtUpdatePeriod";
            this.txtUpdatePeriod.ReadOnly = true;
            this.txtUpdatePeriod.Size = new System.Drawing.Size(77, 18);
            this.txtUpdatePeriod.TabIndex = 51;
            this.txtUpdatePeriod.Text = "0010";
            // 
            // lblUpdatePeriod
            // 
            this.lblUpdatePeriod.AutoSize = true;
            this.lblUpdatePeriod.Location = new System.Drawing.Point(135, 21);
            this.lblUpdatePeriod.Name = "lblUpdatePeriod";
            this.lblUpdatePeriod.Size = new System.Drawing.Size(103, 11);
            this.lblUpdatePeriod.TabIndex = 50;
            this.lblUpdatePeriod.Text = "Update period:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 11);
            this.label1.TabIndex = 48;
            this.label1.Text = "ms";
            // 
            // txtBufferSize
            // 
            this.txtBufferSize.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBufferSize.Location = new System.Drawing.Point(9, 40);
            this.txtBufferSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtBufferSize.Mask = "0000";
            this.txtBufferSize.Name = "txtBufferSize";
            this.txtBufferSize.ReadOnly = true;
            this.txtBufferSize.Size = new System.Drawing.Size(77, 18);
            this.txtBufferSize.TabIndex = 46;
            this.txtBufferSize.Text = "0100";
            // 
            // lblBufferSize
            // 
            this.lblBufferSize.AutoSize = true;
            this.lblBufferSize.Location = new System.Drawing.Point(7, 21);
            this.lblBufferSize.Name = "lblBufferSize";
            this.lblBufferSize.Size = new System.Drawing.Size(89, 11);
            this.lblBufferSize.TabIndex = 4;
            this.lblBufferSize.Text = "Buffer size:";
            // 
            // trackUpdateThreads
            // 
            this.trackUpdateThreads.LargeChange = 2;
            this.trackUpdateThreads.Location = new System.Drawing.Point(265, 65);
            this.trackUpdateThreads.Maximum = 8;
            this.trackUpdateThreads.Minimum = 1;
            this.trackUpdateThreads.Name = "trackUpdateThreads";
            this.trackUpdateThreads.Size = new System.Drawing.Size(115, 45);
            this.trackUpdateThreads.SmallChange = 25;
            this.trackUpdateThreads.TabIndex = 57;
            this.trackUpdateThreads.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackUpdateThreads.Value = 1;
            this.trackUpdateThreads.Scroll += new System.EventHandler(this.trackUpdateThreads_Scroll);
            // 
            // trackUpdatePeriod
            // 
            this.trackUpdatePeriod.LargeChange = 10;
            this.trackUpdatePeriod.Location = new System.Drawing.Point(137, 65);
            this.trackUpdatePeriod.Maximum = 100;
            this.trackUpdatePeriod.Minimum = 5;
            this.trackUpdatePeriod.Name = "trackUpdatePeriod";
            this.trackUpdatePeriod.Size = new System.Drawing.Size(106, 45);
            this.trackUpdatePeriod.SmallChange = 25;
            this.trackUpdatePeriod.TabIndex = 53;
            this.trackUpdatePeriod.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackUpdatePeriod.Value = 10;
            this.trackUpdatePeriod.Scroll += new System.EventHandler(this.trackUpdatePeriod_Scroll);
            // 
            // trackBufferSize
            // 
            this.trackBufferSize.LargeChange = 10;
            this.trackBufferSize.Location = new System.Drawing.Point(9, 65);
            this.trackBufferSize.Maximum = 2000;
            this.trackBufferSize.Minimum = 50;
            this.trackBufferSize.Name = "trackBufferSize";
            this.trackBufferSize.Size = new System.Drawing.Size(106, 45);
            this.trackBufferSize.SmallChange = 25;
            this.trackBufferSize.TabIndex = 49;
            this.trackBufferSize.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBufferSize.Value = 100;
            this.trackBufferSize.Scroll += new System.EventHandler(this.trackBufferSize_Scroll);
            // 
            // groupSoundCard
            // 
            this.groupSoundCard.Controls.Add(this.btnTestAudio);
            this.groupSoundCard.Controls.Add(this.lblDefaultValue);
            this.groupSoundCard.Controls.Add(this.lblDefault);
            this.groupSoundCard.Controls.Add(this.comboOutputDevice);
            this.groupSoundCard.Controls.Add(this.lblOutputDevice);
            this.groupSoundCard.Controls.Add(this.comboDriver);
            this.groupSoundCard.Controls.Add(this.lblDriver);
            this.groupSoundCard.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupSoundCard.Location = new System.Drawing.Point(4, 7);
            this.groupSoundCard.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupSoundCard.Name = "groupSoundCard";
            this.groupSoundCard.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupSoundCard.Size = new System.Drawing.Size(417, 163);
            this.groupSoundCard.TabIndex = 58;
            this.groupSoundCard.TabStop = false;
            this.groupSoundCard.Text = "Sound Card Settings";
            // 
            // btnTestAudio
            // 
            this.btnTestAudio.Image = global::PlaybackEngineV4.Properties.Resources.sound;
            this.btnTestAudio.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTestAudio.Location = new System.Drawing.Point(8, 127);
            this.btnTestAudio.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnTestAudio.Name = "btnTestAudio";
            this.btnTestAudio.Size = new System.Drawing.Size(104, 26);
            this.btnTestAudio.TabIndex = 59;
            this.btnTestAudio.Text = "Test audio";
            this.btnTestAudio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTestAudio.UseVisualStyleBackColor = true;
            this.btnTestAudio.Click += new System.EventHandler(this.btnTestAudio_Click);
            // 
            // lblDefaultValue
            // 
            this.lblDefaultValue.AutoSize = true;
            this.lblDefaultValue.Location = new System.Drawing.Point(76, 109);
            this.lblDefaultValue.Name = "lblDefaultValue";
            this.lblDefaultValue.Size = new System.Drawing.Size(0, 11);
            this.lblDefaultValue.TabIndex = 63;
            // 
            // lblDefault
            // 
            this.lblDefault.AutoSize = true;
            this.lblDefault.Location = new System.Drawing.Point(9, 109);
            this.lblDefault.Name = "lblDefault";
            this.lblDefault.Size = new System.Drawing.Size(61, 11);
            this.lblDefault.TabIndex = 62;
            this.lblDefault.Text = "Default:";
            // 
            // comboOutputDevice
            // 
            this.comboOutputDevice.DisplayMember = "Name";
            this.comboOutputDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOutputDevice.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboOutputDevice.FormattingEnabled = true;
            this.comboOutputDevice.Location = new System.Drawing.Point(9, 82);
            this.comboOutputDevice.Name = "comboOutputDevice";
            this.comboOutputDevice.Size = new System.Drawing.Size(392, 19);
            this.comboOutputDevice.TabIndex = 61;
            this.comboOutputDevice.ValueMember = "Band";
            this.comboOutputDevice.SelectedIndexChanged += new System.EventHandler(this.comboOutputDevice_SelectedIndexChanged);
            // 
            // lblOutputDevice
            // 
            this.lblOutputDevice.AutoSize = true;
            this.lblOutputDevice.Location = new System.Drawing.Point(8, 65);
            this.lblOutputDevice.Name = "lblOutputDevice";
            this.lblOutputDevice.Size = new System.Drawing.Size(103, 11);
            this.lblOutputDevice.TabIndex = 60;
            this.lblOutputDevice.Text = "Output Device:";
            // 
            // comboDriver
            // 
            this.comboDriver.DisplayMember = "Title";
            this.comboDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDriver.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboDriver.FormattingEnabled = true;
            this.comboDriver.Location = new System.Drawing.Point(8, 38);
            this.comboDriver.Name = "comboDriver";
            this.comboDriver.Size = new System.Drawing.Size(392, 19);
            this.comboDriver.TabIndex = 59;
            this.comboDriver.ValueMember = "DriverType";
            this.comboDriver.SelectedIndexChanged += new System.EventHandler(this.comboDriver_SelectedIndexChanged);
            // 
            // lblDriver
            // 
            this.lblDriver.AutoSize = true;
            this.lblDriver.Location = new System.Drawing.Point(7, 21);
            this.lblDriver.Name = "lblDriver";
            this.lblDriver.Size = new System.Drawing.Size(54, 11);
            this.lblDriver.TabIndex = 4;
            this.lblDriver.Text = "Driver:";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.Image = global::PlaybackEngineV4.Properties.Resources.DeleteHS;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btnClose.Location = new System.Drawing.Point(352, 319);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(69, 26);
            this.btnClose.TabIndex = 48;
            this.btnClose.Text = "Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // openFile
            // 
            this.openFile.Filter = "Audio files (*.mp3,*.flac,*.ogg,*.wav)|*.mp3;*.flac;*.ogg;*.wav";
            this.openFile.Title = "Select an audio file to play (*.MP3, *.FLAC, *.OGG, *.WAV).";
            // 
            // frmSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(428, 352);
            this.ControlBox = false;
            this.Controls.Add(this.groupSoundCard);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupAudio);
            this.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "frmSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.groupAudio.ResumeLayout(false);
            this.groupAudio.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackUpdateThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackUpdatePeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBufferSize)).EndInit();
            this.groupSoundCard.ResumeLayout(false);
            this.groupSoundCard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupAudio;
        private System.Windows.Forms.MaskedTextBox txtBufferSize;
        private System.Windows.Forms.Label lblBufferSize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TrackBar trackBufferSize;
        private System.Windows.Forms.TrackBar trackUpdatePeriod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox txtUpdatePeriod;
        private System.Windows.Forms.Label lblUpdatePeriod;
        private System.Windows.Forms.TrackBar trackUpdateThreads;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox txtUpdateThreads;
        private System.Windows.Forms.Label lblUpdateThreads;
        private System.Windows.Forms.GroupBox groupSoundCard;
        private System.Windows.Forms.Label lblDriver;
        private System.Windows.Forms.ComboBox comboDriver;
        private System.Windows.Forms.ComboBox comboOutputDevice;
        private System.Windows.Forms.Label lblOutputDevice;
        private System.Windows.Forms.Label lblDefaultValue;
        private System.Windows.Forms.Label lblDefault;
        private System.Windows.Forms.Button btnTestAudio;
        private System.Windows.Forms.OpenFileDialog openFile;
    }
}