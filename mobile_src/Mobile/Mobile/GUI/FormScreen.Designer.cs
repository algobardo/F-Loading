using System.Windows.Forms;

using Mobile.Language;

using Mobile.Util.Controls;

namespace Mobile
{
    partial class FormScreen
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
            this.previousMenuItem = new System.Windows.Forms.MenuItem();
            this.nextMenuItem = new System.Windows.Forms.MenuItem();
            this.backgroundPanel = new Mobile.Util.Controls.BackgroundedPanel();
            this.containerPanel = new Mobile.Util.Controls.RoundedPanel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.progressBar = new Mobile.ProgressBar();
            this.backgroundPanel.SuspendLayout();
            this.containerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.previousMenuItem);
            this.mainMenu1.MenuItems.Add(this.nextMenuItem);
            // 
            // previousMenuItem
            // 
            this.previousMenuItem.Text = "previousMenuItem";
            this.previousMenuItem.Click += new System.EventHandler(this.OnPreviousButtonClick);
            // 
            // nextMenuItem
            // 
            this.nextMenuItem.Text = "nextMenuItem";
            this.nextMenuItem.Click += new System.EventHandler(this.OnNextButtonClick);
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.backgroundPanel.AutoScroll = true;
            this.backgroundPanel.BackColor = System.Drawing.SystemColors.HotTrack;
            this.backgroundPanel.BackgroundImage = null;
            this.backgroundPanel.Controls.Add(this.containerPanel);
            this.backgroundPanel.Location = new System.Drawing.Point(0, 36);
            this.backgroundPanel.Name = "backgroundPanel";
            this.backgroundPanel.Size = new System.Drawing.Size(240, 232);
            // 
            // containerPanel
            // 
            this.containerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.containerPanel.BackColor = System.Drawing.Color.White;
            this.containerPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.containerPanel.BorderSize = 1;
            this.containerPanel.Controls.Add(this.mainPanel);
            this.containerPanel.CornerRadius = 4;
            this.containerPanel.Location = new System.Drawing.Point(10, 12);
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.Size = new System.Drawing.Size(220, 214);
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.mainPanel.Location = new System.Drawing.Point(8, 8);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(204, 134);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BackColor = System.Drawing.Color.Black;
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(240, 36);
            this.progressBar.TabIndex = 1;
            this.progressBar.Text = "progressBar1";
            // 
            // FormScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.backgroundPanel);
            this.Controls.Add(this.progressBar);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "FormScreen";
            this.Text = "FormScreen";
            this.backgroundPanel.ResumeLayout(false);
            this.containerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem previousMenuItem;
        private System.Windows.Forms.MenuItem nextMenuItem;
        private Mobile.Util.Controls.BackgroundedPanel backgroundPanel;
        private RoundedPanel containerPanel;
        private System.Windows.Forms.Panel mainPanel;
        private ProgressBar progressBar;
    }
}