using Mobile.Util.Controls;
using System.Windows.Forms;
namespace Mobile
{
    partial class NotificationScreen
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
            this.ignoreButton = new System.Windows.Forms.MenuItem();
            this.fillButton = new System.Windows.Forms.MenuItem();
            this.backgroundPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.contentPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tokenPanel = new System.Windows.Forms.Panel();
            this.token = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.servicePanel = new System.Windows.Forms.Panel();
            this.service = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.usernamePanel = new System.Windows.Forms.Panel();
            this.username = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.typePanel = new System.Windows.Forms.Panel();
            this.type = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.notifiedPanel = new System.Windows.Forms.Panel();
            this.notified = new System.Windows.Forms.Label();
            this.notifiedLabel = new System.Windows.Forms.Label();
            this.fromPanel = new System.Windows.Forms.Panel();
            this.from = new System.Windows.Forms.Label();
            this.fromLabel = new System.Windows.Forms.Label();
            this.idPanel = new System.Windows.Forms.Panel();
            this.name = new System.Windows.Forms.Label();
            this.idLabel = new System.Windows.Forms.Label();
            this.backgroundPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tokenPanel.SuspendLayout();
            this.servicePanel.SuspendLayout();
            this.usernamePanel.SuspendLayout();
            this.typePanel.SuspendLayout();
            this.notifiedPanel.SuspendLayout();
            this.fromPanel.SuspendLayout();
            this.idPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.ignoreButton);
            this.mainMenu1.MenuItems.Add(this.fillButton);
            // 
            // ignoreButton
            // 
            this.ignoreButton.Text = "ignoreButton";
            this.ignoreButton.Click += new System.EventHandler(this.OnIgnoreButtonClick);
            // 
            // fillButton
            // 
            this.fillButton.Text = "fillButton";
            this.fillButton.Click += new System.EventHandler(this.OnFillButtonClick);
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.Controls.Add(this.contentPanel);
            this.backgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundPanel.Location = new System.Drawing.Point(0, 0);
            this.backgroundPanel.Name = "backgroundPanel";
            this.backgroundPanel.Size = new System.Drawing.Size(240, 268);
            // 
            // contentPanel
            // 
            this.contentPanel.AdaptBackground = true;
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.BackColor = System.Drawing.Color.White;
            this.contentPanel.Controls.Add(this.panel4);
            this.contentPanel.FilterColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(0)))), ((int)(((byte)(118)))));
            this.contentPanel.FooterHeight = 5;
            this.contentPanel.FooterTileWidth = 2;
            this.contentPanel.HeaderHeight = 40;
            this.contentPanel.HeaderTileWidth = 2;
            this.contentPanel.Location = new System.Drawing.Point(15, 15);
            this.contentPanel.MarginLeftTileHeight = 2;
            this.contentPanel.MarginLeftWidth = 5;
            this.contentPanel.MarginRightTileHeight = 2;
            this.contentPanel.MarginRightWidth = 5;
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(212, 212);
            this.contentPanel.SizeToBackground = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.tokenPanel);
            this.panel4.Controls.Add(this.servicePanel);
            this.panel4.Controls.Add(this.usernamePanel);
            this.panel4.Controls.Add(this.typePanel);
            this.panel4.Controls.Add(this.notifiedPanel);
            this.panel4.Controls.Add(this.fromPanel);
            this.panel4.Controls.Add(this.idPanel);
            this.panel4.Location = new System.Drawing.Point(7, 60);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(197, 143);
            // 
            // tokenPanel
            // 
            this.tokenPanel.Controls.Add(this.token);
            this.tokenPanel.Controls.Add(this.label5);
            this.tokenPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tokenPanel.Location = new System.Drawing.Point(0, 120);
            this.tokenPanel.Name = "tokenPanel";
            this.tokenPanel.Size = new System.Drawing.Size(197, 20);
            // 
            // token
            // 
            this.token.Dock = System.Windows.Forms.DockStyle.Right;
            this.token.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.token.Location = new System.Drawing.Point(74, 0);
            this.token.Name = "token";
            this.token.Size = new System.Drawing.Size(123, 20);
            this.token.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Left;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 20);
            this.label5.Text = "Token";
            // 
            // servicePanel
            // 
            this.servicePanel.Controls.Add(this.service);
            this.servicePanel.Controls.Add(this.label3);
            this.servicePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.servicePanel.Location = new System.Drawing.Point(0, 100);
            this.servicePanel.Name = "servicePanel";
            this.servicePanel.Size = new System.Drawing.Size(197, 20);
            // 
            // service
            // 
            this.service.Dock = System.Windows.Forms.DockStyle.Right;
            this.service.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.service.Location = new System.Drawing.Point(74, 0);
            this.service.Name = "service";
            this.service.Size = new System.Drawing.Size(123, 20);
            this.service.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.Text = "Service";
            // 
            // usernamePanel
            // 
            this.usernamePanel.Controls.Add(this.username);
            this.usernamePanel.Controls.Add(this.label2);
            this.usernamePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.usernamePanel.Location = new System.Drawing.Point(0, 80);
            this.usernamePanel.Name = "usernamePanel";
            this.usernamePanel.Size = new System.Drawing.Size(197, 20);
            // 
            // username
            // 
            this.username.Dock = System.Windows.Forms.DockStyle.Right;
            this.username.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.username.Location = new System.Drawing.Point(74, 0);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(123, 20);
            this.username.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 20);
            this.label2.Text = "Username";
            // 
            // typePanel
            // 
            this.typePanel.Controls.Add(this.type);
            this.typePanel.Controls.Add(this.label4);
            this.typePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.typePanel.Location = new System.Drawing.Point(0, 60);
            this.typePanel.Name = "typePanel";
            this.typePanel.Size = new System.Drawing.Size(197, 20);
            // 
            // type
            // 
            this.type.Dock = System.Windows.Forms.DockStyle.Right;
            this.type.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.type.Location = new System.Drawing.Point(74, 0);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(123, 20);
            this.type.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Left;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 20);
            this.label4.Text = "Type";
            // 
            // notifiedPanel
            // 
            this.notifiedPanel.Controls.Add(this.notified);
            this.notifiedPanel.Controls.Add(this.notifiedLabel);
            this.notifiedPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.notifiedPanel.Location = new System.Drawing.Point(0, 40);
            this.notifiedPanel.Name = "notifiedPanel";
            this.notifiedPanel.Size = new System.Drawing.Size(197, 20);
            // 
            // notified
            // 
            this.notified.Dock = System.Windows.Forms.DockStyle.Right;
            this.notified.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.notified.Location = new System.Drawing.Point(74, 0);
            this.notified.Name = "notified";
            this.notified.Size = new System.Drawing.Size(123, 20);
            this.notified.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // notifiedLabel
            // 
            this.notifiedLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.notifiedLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.notifiedLabel.Location = new System.Drawing.Point(0, 0);
            this.notifiedLabel.Name = "notifiedLabel";
            this.notifiedLabel.Size = new System.Drawing.Size(65, 20);
            this.notifiedLabel.Text = "Notified";
            // 
            // fromPanel
            // 
            this.fromPanel.Controls.Add(this.from);
            this.fromPanel.Controls.Add(this.fromLabel);
            this.fromPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.fromPanel.Location = new System.Drawing.Point(0, 20);
            this.fromPanel.Name = "fromPanel";
            this.fromPanel.Size = new System.Drawing.Size(197, 20);
            // 
            // from
            // 
            this.from.Dock = System.Windows.Forms.DockStyle.Right;
            this.from.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.from.Location = new System.Drawing.Point(74, 0);
            this.from.Name = "from";
            this.from.Size = new System.Drawing.Size(123, 20);
            this.from.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // fromLabel
            // 
            this.fromLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.fromLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.fromLabel.Location = new System.Drawing.Point(0, 0);
            this.fromLabel.Name = "fromLabel";
            this.fromLabel.Size = new System.Drawing.Size(65, 20);
            this.fromLabel.Text = "From";
            // 
            // idPanel
            // 
            this.idPanel.Controls.Add(this.name);
            this.idPanel.Controls.Add(this.idLabel);
            this.idPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.idPanel.Location = new System.Drawing.Point(0, 0);
            this.idPanel.Name = "idPanel";
            this.idPanel.Size = new System.Drawing.Size(197, 20);
            // 
            // name
            // 
            this.name.Dock = System.Windows.Forms.DockStyle.Right;
            this.name.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.name.Location = new System.Drawing.Point(74, 0);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(123, 20);
            this.name.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // idLabel
            // 
            this.idLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.idLabel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.idLabel.Location = new System.Drawing.Point(0, 0);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(65, 20);
            this.idLabel.Text = "ID";
            // 
            // NotificationScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.backgroundPanel);
            this.Menu = this.mainMenu1;
            this.Name = "NotificationScreen";
            this.Text = "NotificationScreen";
            this.backgroundPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tokenPanel.ResumeLayout(false);
            this.servicePanel.ResumeLayout(false);
            this.usernamePanel.ResumeLayout(false);
            this.typePanel.ResumeLayout(false);
            this.notifiedPanel.ResumeLayout(false);
            this.fromPanel.ResumeLayout(false);
            this.idPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem ignoreButton;
        private System.Windows.Forms.MenuItem fillButton;
        private Mobile.Util.Controls.BackgroundedPanel backgroundPanel;
        private Mobile.Util.Controls.BackgroundedPanel contentPanel;
        private System.Windows.Forms.Panel fromPanel;
        private System.Windows.Forms.Label from;
        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.Panel idPanel;
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.Panel notifiedPanel;
        private System.Windows.Forms.Label notified;
        private System.Windows.Forms.Label notifiedLabel;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel tokenPanel;
        private System.Windows.Forms.Label token;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel servicePanel;
        private System.Windows.Forms.Label service;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel usernamePanel;
        private System.Windows.Forms.Label username;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel typePanel;
        private System.Windows.Forms.Label type;
        private System.Windows.Forms.Label label4;
    }
}