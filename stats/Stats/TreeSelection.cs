using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Stats
{
    public partial class TreeSelection : Form
    {
        private XmlDocument doc;

        public TreeSelection(XmlDocument doc)
        {
            InitializeComponent();
            this.doc = doc;
        }

        private void TreeSelection_Load(object sender, EventArgs e)
        {
            try
            {
                List<string> nodes = new List<string>();
                TreeNode root = treeView1.Nodes.Add(doc.GetElementsByTagName("Data").Item(0).ChildNodes.Item(0).Name);
                int nComp = doc.GetElementsByTagName("Data").Item(0).ChildNodes.Count;
                //per ogni compilazione
                for (int k = 0; k < nComp; k++)
                {
                    XmlNodeList list = doc.GetElementsByTagName("Data").Item(0).ChildNodes.Item(k).ChildNodes;
                    int nForm = list.Count;
                    int nFields = 0;
                    TreeNode form;
                    XmlNodeList tmp;
                    //per ogni nodo di una compilazione
                    for (int i = 0; i < nForm; i++)
                    {
                        if (!nodes.Contains(list.Item(i).Name))
                        {
                            form = root.Nodes.Add(list.Item(i).Name);
                            nodes.Add(list.Item(i).Name);
                            nFields = list.Item(i).ChildNodes.Count;
                            for (int j = 0; j < nFields; j++)
                            {
                                tmp = list.Item(i).ChildNodes;
                                if (tmp.Item(j).FirstChild.Name.Equals("Value"))
                                    form.Nodes.Add(tmp.Item(j).Name);
                            }
                        }
                    }
                }
                treeView1.ExpandAll();
            }
            catch (Exception exp)
            { MessageBox.Show(exp.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void TreeSelection_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void TreeSelection1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (treeView1.SelectedNode.Nodes.Count > 0)
                    Console.WriteLine("RAMO");
                else
                {
                    Console.WriteLine("FOGLIA");
                    DrawGraph(treeView1.SelectedNode.Text);
                }
            }
            catch (Exception exp)
            { MessageBox.Show(exp.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void DrawGraph(string field)
        {
            try
            {
                Dictionary<string, int> dic = new Dictionary<string, int>();
                XmlNodeList list = doc.GetElementsByTagName(field);
                string st = null;
                foreach (XmlNode node in list)
                {
                    st = node.FirstChild.InnerText;
                    if (dic.ContainsKey(st))
                    {
                        dic[st] = dic[st] + 1;
                    }
                    else
                        dic.Add(st, 1);
                }
                Graphics gr = new Graphics(dic, field);
                gr.Show();
            }
            catch (Exception exp)
            { MessageBox.Show(exp.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void TreeSelection2_DoubleClick(object sender, EventArgs e)
        {
            
        }
    }
}
