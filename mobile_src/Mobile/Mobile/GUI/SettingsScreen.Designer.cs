namespace Mobile.GUI
{
    partial class SettingsScreen 
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.BackButton = new System.Windows.Forms.MenuItem();
            this.SaveButton = new System.Windows.Forms.MenuItem();
            this.backgroundPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lanBox = new System.Windows.Forms.ComboBox();
            this.LanguageLabel = new System.Windows.Forms.Label();
            this.infoPanel = new Mobile.Util.Controls.BackgroundedControl();
            this.contentPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.detectSms = new System.Windows.Forms.CheckBox();
            this.mailsmsLabel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.autoSaveLowCharge = new System.Windows.Forms.CheckBox();
            this.autosaveLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.enhancedPanel2 = new Mobile.Util.Controls.RoundedPanel();
            this.wspTextbox = new System.Windows.Forms.TextBox();
            this.PortLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.enhancedPanel1 = new Mobile.Util.Controls.RoundedPanel();
            this.wsaTextbox = new System.Windows.Forms.TextBox();
            this.hostnameLabel = new System.Windows.Forms.Label();
            this.backgroundPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.infoPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.enhancedPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.enhancedPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.BackButton);
            this.mainMenu1.MenuItems.Add(this.SaveButton);
            // 
            // BackButton
            // 
            this.BackButton.Text = "BackButton";
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Text = "SaveButton";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.AutoScroll = true;
            this.backgroundPanel.BackgroundImage = null;
            this.backgroundPanel.Controls.Add(this.panel4);
            this.backgroundPanel.Controls.Add(this.infoPanel);
            this.backgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundPanel.Location = new System.Drawing.Point(0, 0);
            this.backgroundPanel.Name = "backgroundPanel";
            this.backgroundPanel.Size = new System.Drawing.Size(240, 268);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lanBox);
            this.panel4.Controls.Add(this.LanguageLabel);
            this.panel4.Location = new System.Drawing.Point(18, 182);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(204, 21);
            // 
            // lanBox
            // 
            this.lanBox.Items.Add("en-US");
            this.lanBox.Items.Add("it-IT");
            this.lanBox.Location = new System.Drawing.Point(86, 0);
            this.lanBox.Name = "lanBox";
            this.lanBox.Size = new System.Drawing.Size(116, 22);
            this.lanBox.TabIndex = 3;
            this.lanBox.SelectedIndexChanged += new System.EventHandler(this.lanBox_SelectedIndexChanged);
            // 
            // LanguageLabel
            // 
            this.LanguageLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.LanguageLabel.Location = new System.Drawing.Point(8, 4);
            this.LanguageLabel.Name = "LanguageLabel";
            this.LanguageLabel.Size = new System.Drawing.Size(72, 21);
            this.LanguageLabel.Text = "language";
            // 
            // infoPanel
            // 
            this.infoPanel.AdaptBackground = true;
            this.infoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.infoPanel.BackgroundImage = null;
            this.infoPanel.Controls.Add(this.contentPanel);
            this.infoPanel.FilterColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(0)))), ((int)(((byte)(116)))));
            this.infoPanel.FooterHeight = 5;
            this.infoPanel.FooterTileWidth = 2;
            this.infoPanel.HeaderHeight = 40;
            this.infoPanel.HeaderTileWidth = 2;
            this.infoPanel.Location = new System.Drawing.Point(15, 15);
            this.infoPanel.MarginLeftTileHeight = 2;
            this.infoPanel.MarginLeftWidth = 5;
            this.infoPanel.MarginRightTileHeight = 2;
            this.infoPanel.MarginRightWidth = 5;
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(212, 216);
            this.infoPanel.SizeToBackground = false;
            this.infoPanel.TabIndex = 3;
            this.infoPanel.Text = "infoPanel";
            // 
            // contentPanel
            // 
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.BackColor = System.Drawing.Color.White;
            this.contentPanel.BackgroundImage = null;
            this.contentPanel.Controls.Add(this.panel5);
            this.contentPanel.Controls.Add(this.panel3);
            this.contentPanel.Controls.Add(this.panel2);
            this.contentPanel.Controls.Add(this.panel1);
            this.contentPanel.Location = new System.Drawing.Point(3, 51);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(205, 114);
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.Controls.Add(this.detectSms);
            this.panel5.Controls.Add(this.mailsmsLabel);
            this.panel5.Location = new System.Drawing.Point(1, 82);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(204, 21);
            // 
            // detectSms
            // 
            this.detectSms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.detectSms.Location = new System.Drawing.Point(178, 1);
            this.detectSms.Name = "detectSms";
            this.detectSms.Size = new System.Drawing.Size(19, 19);
            this.detectSms.TabIndex = 2;
            this.detectSms.Text = "checkBox3";
            // 
            // mailsmsLabel
            // 
            this.mailsmsLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.mailsmsLabel.Location = new System.Drawing.Point(8, 4);
            this.mailsmsLabel.Name = "mailsmsLabel";
            this.mailsmsLabel.Size = new System.Drawing.Size(164, 21);
            this.mailsmsLabel.Text = "Detect floading sms/email";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.autoSaveLowCharge);
            this.panel3.Controls.Add(this.autosaveLabel);
            this.panel3.Location = new System.Drawing.Point(0, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(204, 21);
            // 
            // autoSaveLowCharge
            // 
            this.autoSaveLowCharge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.autoSaveLowCharge.Location = new System.Drawing.Point(178, 1);
            this.autoSaveLowCharge.Name = "autoSaveLowCharge";
            this.autoSaveLowCharge.Size = new System.Drawing.Size(19, 19);
            this.autoSaveLowCharge.TabIndex = 2;
            this.autoSaveLowCharge.Text = "checkBox1";
            // 
            // autosaveLabel
            // 
            this.autosaveLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.autosaveLabel.Location = new System.Drawing.Point(8, 4);
            this.autosaveLabel.Name = "autosaveLabel";
            this.autosaveLabel.Size = new System.Drawing.Size(145, 21);
            this.autosaveLabel.Text = "Auto save on low charge";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.enhancedPanel2);
            this.panel2.Controls.Add(this.PortLabel);
            this.panel2.Location = new System.Drawing.Point(0, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(204, 21);
            // 
            // enhancedPanel2
            // 
            this.enhancedPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.enhancedPanel2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.enhancedPanel2.BorderSize = 1;
            this.enhancedPanel2.Controls.Add(this.wspTextbox);
            this.enhancedPanel2.CornerRadius = 4;
            this.enhancedPanel2.Location = new System.Drawing.Point(86, 0);
            this.enhancedPanel2.Name = "enhancedPanel2";
            this.enhancedPanel2.Size = new System.Drawing.Size(112, 21);
            // 
            // wspTextbox
            // 
            this.wspTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wspTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.wspTextbox.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.wspTextbox.Location = new System.Drawing.Point(5, 4);
            this.wspTextbox.Name = "wspTextbox";
            this.wspTextbox.Size = new System.Drawing.Size(97, 19);
            this.wspTextbox.TabIndex = 1;
            // 
            // PortLabel
            // 
            this.PortLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.PortLabel.Location = new System.Drawing.Point(8, 4);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(72, 21);
            this.PortLabel.Text = "WS port";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.enhancedPanel1);
            this.panel1.Controls.Add(this.hostnameLabel);
            this.panel1.Location = new System.Drawing.Point(0, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(204, 21);
            // 
            // enhancedPanel1
            // 
            this.enhancedPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.enhancedPanel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.enhancedPanel1.BorderSize = 1;
            this.enhancedPanel1.Controls.Add(this.wsaTextbox);
            this.enhancedPanel1.CornerRadius = 4;
            this.enhancedPanel1.Location = new System.Drawing.Point(86, 0);
            this.enhancedPanel1.Name = "enhancedPanel1";
            this.enhancedPanel1.Size = new System.Drawing.Size(112, 21);
            // 
            // wsaTextbox
            // 
            this.wsaTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wsaTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.wsaTextbox.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.wsaTextbox.Location = new System.Drawing.Point(5, 4);
            this.wsaTextbox.Name = "wsaTextbox";
            this.wsaTextbox.Size = new System.Drawing.Size(97, 19);
            this.wsaTextbox.TabIndex = 1;
            // 
            // hostnameLabel
            // 
            this.hostnameLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.hostnameLabel.Location = new System.Drawing.Point(8, 4);
            this.hostnameLabel.Name = "hostnameLabel";
            this.hostnameLabel.Size = new System.Drawing.Size(72, 21);
            this.hostnameLabel.Text = "WS address";
            // 
            // SettingsScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.backgroundPanel);
            this.Menu = this.mainMenu1;
            this.Name = "SettingsScreen";
            this.Text = "SettingsScreen";
            this.backgroundPanel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.infoPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.enhancedPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.enhancedPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Mobile.Util.Controls.BackgroundedPanel backgroundPanel;
        private Mobile.Util.Controls.BackgroundedPanel contentPanel;
        private System.Windows.Forms.MenuItem BackButton;
        private System.Windows.Forms.MenuItem SaveButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label hostnameLabel;
        private System.Windows.Forms.TextBox wsaTextbox;
        private Mobile.Util.Controls.RoundedPanel enhancedPanel1;
        private System.Windows.Forms.Panel panel2;
        private Mobile.Util.Controls.RoundedPanel enhancedPanel2;
        private System.Windows.Forms.TextBox wspTextbox;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox autoSaveLowCharge;
        private System.Windows.Forms.Label autosaveLabel;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.CheckBox detectSms;
        private System.Windows.Forms.Label mailsmsLabel;
        private Mobile.Util.Controls.BackgroundedControl infoPanel;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ComboBox lanBox;
        private System.Windows.Forms.Label LanguageLabel;
    }
}