using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Mobile
{
    public partial class Menu : UserControl
    {
        private int inboxnew=0;
        private int sentnew=0;
        public Menu()
        {
            InitializeComponent();
            InitializeMenuList();
        }
        private void InitializeMenuList()
        {
            ListViewItem item = new ListViewItem();
            item.Text = "ques1";
            item.SubItems.Add(""+new DateTime().Date);
            ListViewItem item1 = new ListViewItem();
            item1.Text = "ques2";
            item1.SubItems.Add("" + new DateTime().Date);
            this.newInboxItem(item1);
            this.newInboxItem(item);


        }
        public void newSentItem(ListViewItem lvi)
        {
            this.sentLabel.Font= new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);

            lvi.BackColor = System.Drawing.SystemColors.Info ;
            //lvi.Text = "sent1";
            //lvi.SubItems.Add("18-18-01");
            this.sentList.Items.Insert(0,lvi);
            sentnew++;
            sentLabel.Text = "sent(" + sentnew + ")";
            //lv.Items.Add(lvi);
        }
        public void newInboxItem(ListViewItem lvi)
        {
            this.inboxLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);

            lvi.BackColor = System.Drawing.SystemColors.Info;
            //lvi.Text = "sent1";
            //lvi.SubItems.Add("18-18-01");
            this.inboxList.Items.Insert(0, lvi);
            inboxnew++;
            inboxLabel.Text = "inbox(" + inboxnew + ")";
            //lv.Items.Add(lvi);
        }
        public TabControl GetTabControl()
        {
            return tabControl1;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
           
        }

        private void label1_ParentChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            
            inboxList.Visible = true;
            inboxLabel.BackColor = System.Drawing.SystemColors.Control;
            sentLabel.BackColor = Color.White;
            sentList.Visible = false;
            
        }

        private void linkLabel2_Click(object sender, EventArgs e)
        {
            inboxList.Visible = false;
            inboxLabel.BackColor =Color.White; 
            sentLabel.BackColor = System.Drawing.SystemColors.Control;
            sentList.Visible = true;
        }

        private void Menu_Resize(object sender, EventArgs e)
        {
           // this.inboxList.Width = Screen.PrimaryScreen.Bounds.Width;
            
            //this.sentList.Width = Screen.PrimaryScreen.Bounds.Width;
        }
    }
}