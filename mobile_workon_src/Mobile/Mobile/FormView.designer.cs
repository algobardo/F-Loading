namespace Mobile
{
    partial class FormView
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
            this.Back = new System.Windows.Forms.MenuItem();
            this.Next = new System.Windows.Forms.MenuItem();
            this.Panel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.transPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.Back);
            this.mainMenu1.MenuItems.Add(this.Next);
            // 
            // Back
            // 
            this.Back.Text = "Back";
            this.Back.Click += new System.EventHandler(this.Back_Click);
            // 
            // Next
            // 
            this.Next.Text = "Next";
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // Panel
            // 
            this.Panel.AutoScroll = true;
            this.Panel.BackColor = System.Drawing.Color.Silver;
            this.Panel.Location = new System.Drawing.Point(0, 56);
            this.Panel.Name = "Panel";
            this.Panel.Size = new System.Drawing.Size(240, 212);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(11, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 19);
            this.label1.Text = "Form Title";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // transPanel
            // 
            this.transPanel.BackColor = System.Drawing.Color.Silver;
            this.transPanel.Location = new System.Drawing.Point(0, 56);
            this.transPanel.Name = "transPanel";
            this.transPanel.Size = new System.Drawing.Size(240, 212);
            this.transPanel.Visible = false;
            // 
            // FormView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.transPanel);
            this.Controls.Add(this.Panel);
            this.Menu = this.mainMenu1;
            this.Name = "FormView";
            this.Text = "FormView";
            this.Load += new System.EventHandler(this.FormView_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem Back;
        private System.Windows.Forms.MenuItem Next;
        private System.Windows.Forms.Panel Panel;
        private ProgressBar progressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel transPanel;
    }
}