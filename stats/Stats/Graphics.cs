using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace Stats
{
    public partial class Graphics : Form
    {
        private Dictionary<string, int> dic;
        private string field;

        public Graphics(Dictionary<string,int> dic,string field)
        {
            InitializeComponent();
            this.dic = dic;
            this.field = field;

        }

        private void Graphics_Load(object sender, EventArgs e)
        {
            CreateGraph(zg1);
            SetSize();
        }

        private void CreateGraph(ZedGraphControl zgc)
        {
            try
            {
                Dictionary<string, int> map = new Dictionary<string, int>();
                GraphPane myPane = zg1.GraphPane;

                myPane.Title.Text = "Stats";
                myPane.XAxis.Title.Text = "Response Number";
                myPane.YAxis.Title.Text = "Kinds of Responses";

                // Create data points for three BarItems using Random data
                PointPairList list = new PointPairList();
                string[] str;
                List<string> names = new List<string>();
                int h = dic.Count;
                for (int i = 0; i < h; i++)
                {
                    names.Add(dic.First().Key);
                    map.Add(dic.First().Key, dic.First().Value);
                    list.Add(dic.First().Value, i + 1, i / 4.0);
                    dic.Remove(dic.First().Key);
                }

                str = names.ToArray<string>();
                dic = map;
                BarItem bar1 = myPane.AddBar(this.field, list, Color.Blue);
                Color[] colors = { Color.Red, Color.Yellow, Color.Green, Color.Blue, Color.Purple };
                bar1.Bar.Fill = new Fill(colors);
                bar1.Bar.Fill.Type = FillType.GradientByZ;

                bar1.Bar.Fill.RangeMin = 0;
                bar1.Bar.Fill.RangeMax = 2;


                // Set the YAxis labels
                myPane.YAxis.Scale.TextLabels = str;

                // Set the YAxis to Text type
                myPane.YAxis.Type = AxisType.Text;

                myPane.BarSettings.Base = BarBase.Y;
                // Make the bars stack instead of cluster
                myPane.BarSettings.Type = BarType.Stack;
                // Create TextObj's to provide labels for each bar
                BarItem.CreateBarLabels(myPane, true, "f0");

                myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45);
                myPane.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 225), 45);





                // Calculate the Axis Scale Ranges
                zgc.AxisChange();
            }
            catch (Exception exp)
            { MessageBox.Show(exp.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void SetSize()
        {
            zg1.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            zg1.Size = new Size(this.ClientRectangle.Width - 20, this.ClientRectangle.Height - 20);
        }

        private void Graphics_Resize(object sender, EventArgs e)
        {
            SetSize();
        }
    }
}
