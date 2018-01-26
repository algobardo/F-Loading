namespace Mobile
{
    partial class Menu
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem();
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem();
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Home = new System.Windows.Forms.TabPage();
            this.inboxList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.sentLabel = new System.Windows.Forms.LinkLabel();
            this.inboxLabel = new System.Windows.Forms.LinkLabel();
            this.sentList = new System.Windows.Forms.ListView();
            this.CHeadDescription = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.Settings = new System.Windows.Forms.TabPage();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.tabControl1.SuspendLayout();
            this.Home.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.Home);
            this.tabControl1.Controls.Add(this.Settings);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.None;
            this.tabControl1.Font = new System.Drawing.Font("Aharoni", 8F, System.Drawing.FontStyle.Bold);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(240, 268);
            this.tabControl1.TabIndex = 0;
            // 
            // Home
            // 
            this.Home.BackColor = System.Drawing.Color.White;
            this.Home.Controls.Add(this.sentLabel);
            this.Home.Controls.Add(this.inboxLabel);
            this.Home.Controls.Add(this.sentList);
            this.Home.Controls.Add(this.inboxList);
            this.Home.Location = new System.Drawing.Point(0, 0);
            this.Home.Name = "Home";
            this.Home.Size = new System.Drawing.Size(240, 247);
            this.Home.Tag = "";
            this.Home.Text = "Home";
            // 

            // inboxList
            // 
            this.inboxList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.inboxList.Columns.Add(this.columnHeader1);
            this.inboxList.Columns.Add(this.columnHeader2);
            this.inboxList.FullRowSelect = true;
            this.inboxList.Location = new System.Drawing.Point(58, 0);
            this.inboxList.Name = "inboxList";
            this.inboxList.Size = new System.Drawing.Size(182, 247);
            this.inboxList.TabIndex = 4;
            this.inboxList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Description";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Date";
            this.columnHeader2.Width = 79;
            // 

            // sentLabel
            // 
            this.sentLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.sentLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.sentLabel.Location = new System.Drawing.Point(2, 59);
            this.sentLabel.Name = "sentLabel";
            this.sentLabel.Size = new System.Drawing.Size(56, 20);
            this.sentLabel.TabIndex = 2;
            this.sentLabel.Text = "sent()";
            this.sentLabel.Click += new System.EventHandler(this.linkLabel2_Click);
            // 
            // inboxLabel
            // 
            this.inboxLabel.BackColor = System.Drawing.SystemColors.Control;
            this.inboxLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.inboxLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.inboxLabel.Location = new System.Drawing.Point(2, 39);
            this.inboxLabel.Name = "inboxLabel";
            this.inboxLabel.Size = new System.Drawing.Size(56, 20);
            this.inboxLabel.TabIndex = 1;
            this.inboxLabel.Text = "inbox()";
            this.inboxLabel.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // sentList
            // 
            this.sentList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sentList.Columns.Add(this.CHeadDescription);
            this.sentList.Columns.Add(this.columnHeader4);
            this.sentList.FullRowSelect = true;
            listViewItem1.Text = "quest1";
            listViewItem1.SubItems.Add("dsfsf");
            listViewItem2.Text = "Form2";
            listViewItem3.Text = "Form3";
            this.sentList.Items.Add(listViewItem1);
            this.sentList.Items.Add(listViewItem2);
            this.sentList.Items.Add(listViewItem3);
            this.sentList.Location = new System.Drawing.Point(58, 0);
            this.sentList.Name = "sentList";
            this.sentList.Size = new System.Drawing.Size(182, 247);
            this.sentList.TabIndex = 3;
            this.sentList.View = System.Windows.Forms.View.Details;
            // 
            // CHeadDescription
            // 
            this.CHeadDescription.Text = "Description";
            this.CHeadDescription.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Date";
            this.columnHeader4.Width = 79;
            // 
            // Settings
            // 
            this.Settings.BackColor = System.Drawing.Color.White;
            this.Settings.Location = new System.Drawing.Point(0, 0);
            this.Settings.Name = "Settings";
            this.Settings.Size = new System.Drawing.Size(232, 243);
            this.Settings.Text = "Settings";
            // 
            // imageList1
            // 
            this.imageList1.ImageSize = new System.Drawing.Size(100, 100);
            this.imageList1.Images.Clear();
            this.imageList1.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tabControl1);
            this.Name = "Menu";
            this.Size = new System.Drawing.Size(240, 268);
            this.Resize += new System.EventHandler(this.Menu_Resize);
            this.tabControl1.ResumeLayout(false);
            this.Home.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Home;
        private System.Windows.Forms.TabPage Settings;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.LinkLabel inboxLabel;
        private System.Windows.Forms.LinkLabel sentLabel;
        private System.Windows.Forms.ListView sentList;
        private System.Windows.Forms.ColumnHeader CHeadDescription;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ListView inboxList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
