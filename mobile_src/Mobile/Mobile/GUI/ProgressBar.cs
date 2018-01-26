using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Mobile
{
    /// <summary>
    /// The progress bar located at the top of the FormScreen
    /// </summary>
    public partial class ProgressBar : Control
    {

        #region Private

        private Rectangle[] items;
        private int status;
        private int numcircles;
        private Label titleLabel;

        private const int diameter = 4;

        private const int space = 8;

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics context = pe.Graphics;

            // Calling the base class OnPaint
            base.OnPaint(pe);

            for (int i = 0; i < numcircles; i++)
            {
                context.FillEllipse(new SolidBrush(Color.Gray), items[i]);
            }
            if (items.Length > 0 && status > 0 && status <= items.Length)
                context.FillEllipse(new SolidBrush(Color.WhiteSmoke), items[status - 1]);
        }

        #endregion

        /// <summary>
        /// Rappresents the circle that must be selected
        /// </summary>
        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                this.status = value;
                Refresh();
            }
        }

        /// <summary>
        /// The numbers of circles (states)
        /// </summary>
        public int NumCircles
        {
            get
            {
                return numcircles;
            }
            set
            {
                this.numcircles = value;
                Load();
                Refresh();
            }
        }

        /// <summary>
        /// This is the name of each state of the workflow
        /// </summary>
        public string Title
        {
            get
            {
                return titleLabel.Text;
            }
            set
            {
                titleLabel.Text = XmlConvert.DecodeName(value);
            }
        }
        
        /// <summary>
        /// Constructs a new ProgressBar
        /// </summary>
        public ProgressBar()
        {
            InitializeComponent();
            status = 1;
            titleLabel = new Label();
            Load();
        }

        /// <summary>
        /// Creates the layout of the control
        /// </summary>
        public void Load()
        {
            items = new Rectangle[numcircles];
            int total = diameter*numcircles + space*(numcircles - 1);
            for (int i = 0; i < numcircles; i++)
                items[i] = new Rectangle(this.Width/2 - (total/2 - i*(diameter + space)), 10, diameter, diameter);
            titleLabel.Size = new Size(this.Width, 15);
            titleLabel.Location = new Point(0, this.Bottom - 15);
            titleLabel.TextAlign = ContentAlignment.TopCenter;
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Tahoma", 8, FontStyle.Bold);
            this.Controls.Add(titleLabel);
            Refresh();
        }

        /// <summary>
        /// Changes the width property to fill the screen
        /// </summary>
        public void ChangeOrientation()
        {
            Width = Screen.PrimaryScreen.WorkingArea.Width;
            Load();
        }
    }
}
