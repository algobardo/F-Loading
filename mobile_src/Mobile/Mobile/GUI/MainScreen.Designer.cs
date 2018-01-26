using Mobile.Language;
namespace Mobile
{
    partial class MainScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu BottomButtons;

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
            this.BottomButtons = new System.Windows.Forms.MainMenu();
            this.exitButton = new System.Windows.Forms.MenuItem();
            this.backgroundPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.checkNewButton = new Mobile.Util.Controls.BackgroundedControl();
            this.privateFormButton = new Mobile.Util.Controls.BackgroundedButton();
            this.warningButton = new Mobile.Util.Controls.BackgroundedControl();
            this.helpButton = new Mobile.Util.Controls.BackgroundedControl();
            this.settingsButton = new Mobile.Util.Controls.BackgroundedControl();
            this.resultsButton = new Mobile.Util.Controls.BackgroundedButton();
            this.publicFormButton = new Mobile.Util.Controls.BackgroundedButton();
            this.backgroundPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomButtons
            // 
            this.BottomButtons.MenuItems.Add(this.exitButton);
            // 
            // exitButton
            // 
            this.exitButton.Text = "exitButton";
            this.exitButton.Click += new System.EventHandler(this.OnExitButtonClick);
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.Controls.Add(this.privateFormButton);
            this.backgroundPanel.Controls.Add(this.resultsButton);
            this.backgroundPanel.Controls.Add(this.publicFormButton);
            this.backgroundPanel.Controls.Add(this.checkNewButton);
            this.backgroundPanel.Controls.Add(this.warningButton);
            this.backgroundPanel.Controls.Add(this.helpButton);
            this.backgroundPanel.Controls.Add(this.settingsButton);
            this.backgroundPanel.Location = new System.Drawing.Point(0, 0);
            this.backgroundPanel.Name = "backgroundPanel";
            this.backgroundPanel.Size = new System.Drawing.Size(240, 268);
            // 
            // checkNewButton
            // 
            this.checkNewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkNewButton.Location = new System.Drawing.Point(107, 7);
            this.checkNewButton.Name = "checkNewButton";
            this.checkNewButton.Size = new System.Drawing.Size(31, 31);
            this.checkNewButton.SizeToBackground = true;
            this.checkNewButton.TabIndex = 7;
            this.checkNewButton.Text = "enhancedControl1";
            this.checkNewButton.Click += new System.EventHandler(this.OnCheckNowButtonClick);
            // 
            // privateFormButton
            // 
            this.privateFormButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.privateFormButton.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.privateFormButton.Location = new System.Drawing.Point(16, 170);
            this.privateFormButton.Name = "privateFormButton";
            this.privateFormButton.Size = new System.Drawing.Size(70, 90);
            this.privateFormButton.SizeToBackground = true;
            this.privateFormButton.TabIndex = 6;
            this.privateFormButton.Text = "privateFormButton";
            this.privateFormButton.Click += new System.EventHandler(this.OnPrivateFormButtonClick);
            // 
            // warningButton
            // 
            this.warningButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.warningButton.Location = new System.Drawing.Point(138, 7);
            this.warningButton.Name = "warningButton";
            this.warningButton.Size = new System.Drawing.Size(31, 31);
            this.warningButton.SizeToBackground = true;
            this.warningButton.TabIndex = 5;
            this.warningButton.Text = "enhancedControl1";
            this.warningButton.Click += new System.EventHandler(this.OnWarningButtonClick);
            // 
            // helpButton
            // 
            this.helpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpButton.Location = new System.Drawing.Point(200, 7);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(31, 31);
            this.helpButton.SizeToBackground = true;
            this.helpButton.TabIndex = 4;
            this.helpButton.Text = "enhancedControl1";
            this.helpButton.Click += new System.EventHandler(this.OnHelpButtonClick);
            // 
            // settingsButton
            // 
            this.settingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsButton.Location = new System.Drawing.Point(169, 7);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(31, 31);
            this.settingsButton.SizeToBackground = true;
            this.settingsButton.TabIndex = 3;
            this.settingsButton.Text = "enhancedControl1";
            this.settingsButton.Click += new System.EventHandler(this.OnSettingsButtonClick);
            // 
            // resultsButton
            // 
            this.resultsButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.resultsButton.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.resultsButton.Location = new System.Drawing.Point(156, 170);
            this.resultsButton.Name = "resultsButton";
            this.resultsButton.Size = new System.Drawing.Size(74, 90);
            this.resultsButton.SizeToBackground = true;
            this.resultsButton.TabIndex = 2;
            this.resultsButton.Text = "resultsButton";
            this.resultsButton.Click += new System.EventHandler(this.OnResultsButtonClick);
            // 
            // publicFormButton
            // 
            this.publicFormButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.publicFormButton.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular);
            this.publicFormButton.Location = new System.Drawing.Point(86, 170);
            this.publicFormButton.Name = "publicFormButton";
            this.publicFormButton.Size = new System.Drawing.Size(70, 90);
            this.publicFormButton.SizeToBackground = true;
            this.publicFormButton.TabIndex = 1;
            this.publicFormButton.Text = "publicFormButton";
            this.publicFormButton.Click += new System.EventHandler(this.OnPublicFormButtonClick);
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.backgroundPanel);
            this.KeyPreview = true;
            this.Menu = this.BottomButtons;
            this.MinimizeBox = false;
            this.Name = "MainScreen";
            this.Text = "Floading";
            this.backgroundPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Mobile.Util.Controls.BackgroundedPanel backgroundPanel;
        private Mobile.Util.Controls.BackgroundedButton resultsButton;
        private Mobile.Util.Controls.BackgroundedControl settingsButton;
        private Mobile.Util.Controls.BackgroundedControl warningButton;
        private Mobile.Util.Controls.BackgroundedControl helpButton;
        private System.Windows.Forms.MenuItem exitButton;
        private Mobile.Util.Controls.BackgroundedButton privateFormButton;
        private Mobile.Util.Controls.BackgroundedButton publicFormButton;
        private Mobile.Util.Controls.BackgroundedControl checkNewButton;
    }
}