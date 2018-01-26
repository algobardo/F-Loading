using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mobile
{
    public partial class ProgressBar : Control
    {
        private Rectangle[] items;

        private int status = 0;

        private int numcircles;

        private const int diameter = 4;

        private const int space = 8;

        public ProgressBar(int n)
        {
            InitializeComponent();
            numcircles = n;
            status = 0;
        }

        public void Load()
        {
            items = new Rectangle[numcircles];
            int total = diameter*numcircles + space*(numcircles - 1);
            for (int i = 0; i < numcircles; i++)
                items[i] = new Rectangle(this.Size.Width/2 - (total/2 - i*(diameter + space)), this.Size.Height / 2 - diameter / 2, diameter, diameter);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics context = pe.Graphics;
            
            // Calling the base class OnPaint
            base.OnPaint(pe);

            for (int i = 0; i < numcircles; i++)
            {
                if (i == status)
                    context.FillEllipse(new SolidBrush(Color.WhiteSmoke), items[i]);
                else
                    context.FillEllipse(new SolidBrush(Color.Gray), items[i]);
            }
        }

        public void SetProgress(int s)
        {
            status = s;
            Refresh();
        }
    }
}
