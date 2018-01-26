namespace Mobile
{
    partial class HelpScreen
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
            this.backgroundPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.infoPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.header = new Mobile.Util.Controls.BackgroundedControl();
            this.backgroundPanel.SuspendLayout();
            this.infoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.BackButton);
            // 
            // BackButton
            // 
            this.BackButton.Text = "Back";
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.BackgroundImage = null;
            this.backgroundPanel.Controls.Add(this.infoPanel);
            this.backgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundPanel.Location = new System.Drawing.Point(0, 0);
            this.backgroundPanel.Name = "backgroundPanel";
            this.backgroundPanel.Size = new System.Drawing.Size(240, 268);
            // 
            // infoPanel
            // 
            this.infoPanel.AdaptBackground = true;
            this.infoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.infoPanel.BackgroundImage = null;
            this.infoPanel.Controls.Add(this.header);
            this.infoPanel.FilterColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(0)))), ((int)(((byte)(118)))));
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
            this.infoPanel.Size = new System.Drawing.Size(212, 233);
            // 
            // header
            // 
            this.header.BackgroundImage = null;
            this.header.FilterColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(0)))), ((int)(((byte)(118)))));
            this.header.Location = new System.Drawing.Point(0, 0);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(87, 34);
            this.header.SizeToBackground = true;
            this.header.TabIndex = 0;
            this.header.Text = "enhancedControl1";
            // 
            // HelpScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.backgroundPanel);
            this.Menu = this.mainMenu1;
            this.Name = "HelpScreen";
            this.Text = "HelpScreen";
            this.backgroundPanel.ResumeLayout(false);
            this.infoPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem BackButton;
        private Mobile.Util.Controls.BackgroundedPanel backgroundPanel;
        private Mobile.Util.Controls.BackgroundedControl header;
        private Mobile.Util.Controls.BackgroundedPanel infoPanel;
    }
}