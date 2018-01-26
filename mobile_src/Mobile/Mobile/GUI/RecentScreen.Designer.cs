using Mobile.Language;
namespace Mobile
{
    partial class RecentScreen
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
            this.backButton = new System.Windows.Forms.MenuItem();
            this.optionButton = new System.Windows.Forms.MenuItem();
            this.fillButton = new System.Windows.Forms.MenuItem();
            this.Delete = new System.Windows.Forms.MenuItem();
            this.backgroundPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.contentPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.infoPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Description = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundPanel.SuspendLayout();
            this.infoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.backButton);
            this.mainMenu1.MenuItems.Add(this.optionButton);
            // 
            // backButton
            // 
            this.backButton.Text = "backButton";
            this.backButton.Click += new System.EventHandler(this.OnCloseAdviceButtonClick);
            // 
            // optionButton
            // 
            this.optionButton.MenuItems.Add(this.fillButton);
            this.optionButton.MenuItems.Add(this.Delete);
            this.optionButton.Text = "optionButton";
            // 
            // fillButton
            // 
            this.fillButton.Text = "fillButton";
            this.fillButton.Click += new System.EventHandler(this.OnFillButtonClick);
            // 
            // Delete
            // 
            this.Delete.Text = "Delete";
            this.Delete.Click += new System.EventHandler(this.OnDeleteButtonClick);
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.AutoScroll = true;
            this.backgroundPanel.Controls.Add(this.contentPanel);
            this.backgroundPanel.Controls.Add(this.infoPanel);
            this.backgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundPanel.Location = new System.Drawing.Point(0, 0);
            this.backgroundPanel.Name = "backgroundPanel";
            this.backgroundPanel.Size = new System.Drawing.Size(240, 268);
            this.backgroundPanel.SizeToBackground = false;
            // 
            // contentPanel
            // 
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.AutoScroll = true;
            this.contentPanel.Location = new System.Drawing.Point(23, 128);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(196, 120);
            this.contentPanel.SizeToBackground = true;
            // 
            // infoPanel
            // 
            this.infoPanel.AdaptBackground = true;
            this.infoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.infoPanel.Controls.Add(this.panel1);
            this.infoPanel.Controls.Add(this.Description);
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
            this.infoPanel.Size = new System.Drawing.Size(212, 241);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.panel1.Location = new System.Drawing.Point(8, 107);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(196, 1);
            // 
            // Description
            // 
            this.Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Description.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.Description.Location = new System.Drawing.Point(8, 47);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(196, 57);
            this.Description.Text = "It seems there are some forms you have not finished to fill or that you have not " +
                "submitted. These forms are listed below";
            
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(8, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 57);
            this.label1.Text = "It seems there are some forms you have not finished to fill or that you have not " +
                "submitted. These forms are listed below";
            // 
            // RecentScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.backgroundPanel);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "RecentScreen";
            this.Text = "WarningScreen";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RecentScreen_KeyDown);
            this.backgroundPanel.ResumeLayout(false);
            this.infoPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Mobile.Util.Controls.BackgroundedPanel backgroundPanel;
        private System.Windows.Forms.MenuItem backButton;
        private System.Windows.Forms.MenuItem optionButton;
        private Mobile.Util.Controls.BackgroundedPanel contentPanel;
        private System.Windows.Forms.MenuItem fillButton;
        private System.Windows.Forms.MenuItem Delete;
        private Mobile.Util.Controls.BackgroundedPanel infoPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.Label label1;
    }
}