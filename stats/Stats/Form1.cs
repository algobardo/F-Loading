using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.IO;
using System.Xml;

namespace Stats
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();


                openFileDialog1.Filter = "Xml Document (*.xml)|*.xml";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                string filename = null;
                string path = null;
                string xml = null;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filename = System.IO.Path.GetFileName(openFileDialog1.FileName);
                    path = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                    path = path + "\\" + filename;
                    StreamReader sr = new StreamReader(path);

                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        xml += line + "\n";
                    }


                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    TreeSelection ts = new TreeSelection(doc);
                    ts.Show();
                }
            }
            catch (Exception exp)
            { MessageBox.Show(exp.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        private void Drag_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) Console.WriteLine(file);

        }

        private void Drag_Enter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) Console.WriteLine(file);
        }

        private void Drag_Over(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) Console.WriteLine(file);
        }
    }
}
