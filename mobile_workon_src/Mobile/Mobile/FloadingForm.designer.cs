namespace Mobile
{
    partial class FloadingForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.Exit = new System.Windows.Forms.MenuItem();
            this.OkButton = new System.Windows.Forms.MenuItem();
            this.loginpanel = new Mobile.LoginPanel();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.Exit);
            this.mainMenu1.MenuItems.Add(this.OkButton);
            // 
            // Exit
            // 
            this.Exit.Text = "Exit";
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // OkButton
            // 
            this.OkButton.Text = "Login";
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // loginpanel
            // 
            this.loginpanel.BackColor = System.Drawing.Color.Black;
            this.loginpanel.ForeColor = System.Drawing.Color.Transparent;
            this.loginpanel.Location = new System.Drawing.Point(0, 0);
            this.loginpanel.Name = "loginpanel";
            this.loginpanel.Size = new System.Drawing.Size(240, 268);
            this.loginpanel.TabIndex = 3;
            // 
            // FloadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.loginpanel);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "FloadingForm";
            this.Text = "Floading";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FloadingForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem Exit;
        private System.Windows.Forms.MenuItem OkButton;
        private Mobile.LoginPanel loginpanel;
    }
}

