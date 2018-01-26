using System.Windows.Forms;
using Mobile.Language;
namespace Mobile
{
    partial class FormFetchScreen
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
            this.getFormButton = new System.Windows.Forms.MenuItem();
            this.backgroundPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.contentPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Description = new System.Windows.Forms.Label();
            this.servicePanel = new Mobile.Util.Controls.RoundedPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.service = new System.Windows.Forms.ComboBox();
            this.tokenPanel = new Mobile.Util.Controls.RoundedPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.roundedControl3 = new Mobile.Util.Controls.RoundedControl();
            this.token = new System.Windows.Forms.TextBox();
            this.usernamePanel = new Mobile.Util.Controls.RoundedPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.roundedControl2 = new Mobile.Util.Controls.RoundedControl();
            this.username = new System.Windows.Forms.TextBox();
            this.idPanel = new Mobile.Util.Controls.RoundedPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.roundedControl1 = new Mobile.Util.Controls.RoundedControl();
            this.id = new System.Windows.Forms.TextBox();
            this.backgroundPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.servicePanel.SuspendLayout();
            this.tokenPanel.SuspendLayout();
            this.roundedControl3.SuspendLayout();
            this.usernamePanel.SuspendLayout();
            this.roundedControl2.SuspendLayout();
            this.idPanel.SuspendLayout();
            this.roundedControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.backButton);
            this.mainMenu1.MenuItems.Add(this.getFormButton);
            // 
            // backButton
            // 
            this.backButton.Text = "backButton";
            this.backButton.Click += new System.EventHandler(this.OnBackButtonClick);
            // 
            // getFormButton
            // 
            this.getFormButton.Text = "getFormButton";
            this.getFormButton.Click += new System.EventHandler(this.OnGetFormButtonClick);
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.AutoScroll = true;
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
            this.contentPanel.Controls.Add(this.panel1);
            this.contentPanel.Controls.Add(this.Description);
            this.contentPanel.Controls.Add(this.servicePanel);
            this.contentPanel.Controls.Add(this.tokenPanel);
            this.contentPanel.Controls.Add(this.usernamePanel);
            this.contentPanel.Controls.Add(this.idPanel);
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
            this.contentPanel.Size = new System.Drawing.Size(212, 219);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.panel1.Location = new System.Drawing.Point(8, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(196, 1);
            // 
            // Description
            // 
            this.Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Description.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.Description.Location = new System.Drawing.Point(8, 48);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(196, 47);
            this.Description.Text = "Please, fill all the informations shown below. You should have received all the i" +
                "nformations by email/sms";
            // 
            // servicePanel
            // 
            this.servicePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.servicePanel.Controls.Add(this.label5);
            this.servicePanel.Controls.Add(this.service);
            this.servicePanel.Location = new System.Drawing.Point(8, 187);
            this.servicePanel.Name = "servicePanel";
            this.servicePanel.Size = new System.Drawing.Size(196, 20);
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Left;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 20);
            this.label5.Text = "Service";
            // 
            // service
            // 
            this.service.Dock = System.Windows.Forms.DockStyle.Right;
            this.service.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.service.Items.Add("Google");
            this.service.Items.Add("Facebook");
            this.service.Items.Add("Windows live");
            this.service.Location = new System.Drawing.Point(90, 0);
            this.service.Name = "service";
            this.service.Size = new System.Drawing.Size(106, 20);
            this.service.TabIndex = 0;
            // 
            // tokenPanel
            // 
            this.tokenPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tokenPanel.Controls.Add(this.label4);
            this.tokenPanel.Controls.Add(this.roundedControl3);
            this.tokenPanel.Location = new System.Drawing.Point(8, 160);
            this.tokenPanel.Name = "tokenPanel";
            this.tokenPanel.Size = new System.Drawing.Size(196, 20);
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Left;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 20);
            this.label4.Text = "Token";
            // 
            // roundedControl3
            // 
            this.roundedControl3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.roundedControl3.BorderSize = 1;
            this.roundedControl3.Controls.Add(this.token);
            this.roundedControl3.CornerRadius = 4;
            this.roundedControl3.Dock = System.Windows.Forms.DockStyle.Right;
            this.roundedControl3.Location = new System.Drawing.Point(89, 0);
            this.roundedControl3.Name = "roundedControl3";
            this.roundedControl3.Size = new System.Drawing.Size(107, 20);
            this.roundedControl3.TabIndex = 1;
            this.roundedControl3.Text = "roundedControl3";
            // 
            // token
            // 
            this.token.BackColor = System.Drawing.Color.White;
            this.token.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.token.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.token.Location = new System.Drawing.Point(7, 3);
            this.token.Name = "token";
            this.token.Size = new System.Drawing.Size(95, 19);
            this.token.TabIndex = 0;
            // 
            // usernamePanel
            // 
            this.usernamePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.usernamePanel.Controls.Add(this.label3);
            this.usernamePanel.Controls.Add(this.roundedControl2);
            this.usernamePanel.Location = new System.Drawing.Point(8, 133);
            this.usernamePanel.Name = "usernamePanel";
            this.usernamePanel.Size = new System.Drawing.Size(196, 20);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 20);
            this.label3.Text = "Username";
            // 
            // roundedControl2
            // 
            this.roundedControl2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.roundedControl2.BorderSize = 1;
            this.roundedControl2.Controls.Add(this.username);
            this.roundedControl2.CornerRadius = 4;
            this.roundedControl2.Dock = System.Windows.Forms.DockStyle.Right;
            this.roundedControl2.Location = new System.Drawing.Point(89, 0);
            this.roundedControl2.Name = "roundedControl2";
            this.roundedControl2.Size = new System.Drawing.Size(107, 20);
            this.roundedControl2.TabIndex = 1;
            this.roundedControl2.Text = "roundedControl2";
            // 
            // username
            // 
            this.username.BackColor = System.Drawing.Color.White;
            this.username.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.username.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.username.Location = new System.Drawing.Point(7, 3);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(95, 19);
            this.username.TabIndex = 0;
            // 
            // idPanel
            // 
            this.idPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.idPanel.Controls.Add(this.label2);
            this.idPanel.Controls.Add(this.roundedControl1);
            this.idPanel.Location = new System.Drawing.Point(8, 106);
            this.idPanel.Name = "idPanel";
            this.idPanel.Size = new System.Drawing.Size(196, 20);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 20);
            this.label2.Text = "ID";
            // 
            // roundedControl1
            // 
            this.roundedControl1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
            this.roundedControl1.BorderSize = 1;
            this.roundedControl1.Controls.Add(this.id);
            this.roundedControl1.CornerRadius = 4;
            this.roundedControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.roundedControl1.Location = new System.Drawing.Point(89, 0);
            this.roundedControl1.Name = "roundedControl1";
            this.roundedControl1.Size = new System.Drawing.Size(107, 20);
            this.roundedControl1.TabIndex = 1;
            this.roundedControl1.Text = "roundedControl1";
            // 
            // id
            // 
            this.id.BackColor = System.Drawing.Color.White;
            this.id.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.id.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.id.Location = new System.Drawing.Point(7, 3);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(95, 19);
            this.id.TabIndex = 0;
            // 
            // FormFetchScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.backgroundPanel);
            this.Menu = this.mainMenu1;
            this.Name = "FormFetchScreen";
            this.Text = "LoginScreen";
            this.backgroundPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.servicePanel.ResumeLayout(false);
            this.tokenPanel.ResumeLayout(false);
            this.roundedControl3.ResumeLayout(false);
            this.usernamePanel.ResumeLayout(false);
            this.roundedControl2.ResumeLayout(false);
            this.idPanel.ResumeLayout(false);
            this.roundedControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Mobile.Util.Controls.BackgroundedPanel backgroundPanel;
        private System.Windows.Forms.MenuItem backButton;
        private System.Windows.Forms.MenuItem getFormButton;
        private System.Windows.Forms.TextBox id;
        private Mobile.Util.Controls.BackgroundedPanel contentPanel;
        private Mobile.Util.Controls.RoundedPanel idPanel;
        private Mobile.Util.Controls.RoundedControl roundedControl1;
        private Mobile.Util.Controls.RoundedPanel tokenPanel;
        private Mobile.Util.Controls.RoundedControl roundedControl3;
        private System.Windows.Forms.TextBox token;
        private Mobile.Util.Controls.RoundedPanel usernamePanel;
        private Mobile.Util.Controls.RoundedControl roundedControl2;
        private System.Windows.Forms.TextBox username;
        private Mobile.Util.Controls.RoundedPanel servicePanel;
        private System.Windows.Forms.ComboBox service;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
    }
}