using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mobile.Util.Controls
{
    public partial class StarRating : Control
    {
        public int SelectedIndex
        {
            get;
            set;
        }

        private int Num;

        public StarRating(int size, bool enabled)
        {
            InitializeComponent();

            Num = size - 1;
            Enabled = enabled;

            BackColor = Color.Azure;
            this.Height = 15;
            this.Width = 115;

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: inserire qui il codice personalizzato

            // Calling the base class OnPaint
            base.OnPaint(pe);

            Bitmap starFilled = Mobile.Properties.Resources._637px_Star_Ouro_svg;
            Bitmap starEmpty = Mobile.Properties.Resources._637px_Star_Prata_svg;

            int offset = 0;
            for (int i = 0; i <= SelectedIndex; i++)
            {
                pe.Graphics.DrawImage(starFilled, new Rectangle(offset, 0, 15, 15), new Rectangle(0, 0, starFilled.Width, starFilled.Height), GraphicsUnit.Pixel);
                offset += 20;
            }
            if (SelectedIndex > Num)
                return;
            for (int i = SelectedIndex + 1; i < Num; i++)
            {
                pe.Graphics.DrawImage(starEmpty, new Rectangle(offset, 0, 15, 15), new Rectangle(0, 0, starEmpty.Width, starEmpty.Height), GraphicsUnit.Pixel);
                offset += 20;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Enabled)
            {
                int index = SelectedIndex;
                SelectedIndex = (int)(e.X / 20);
                if (e.X < 7.5)
                    SelectedIndex = -1;
                if (SelectedIndex != index && SelectedIndex < Num)
                    Refresh();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (Enabled)
            {
                SelectedIndex = (int)(e.X / 20);
                if (e.X < 7.5)
                    SelectedIndex = -1;
                if (SelectedIndex < Num - 1)
                    Refresh();
            }
        }

    }
}
