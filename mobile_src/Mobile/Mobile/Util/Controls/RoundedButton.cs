using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Mobile.Util.Controls
{
    public class RoundedButton : RoundedControl
    {
        private Color tempBackColor;
        private Color tempBorderColor;
        private int tempBorderSize;

        private Color backColor;
        private Color borderColor;
        private int borderSize;

        public RoundedButton()
            : base()
        {
        }

        public Color ClickedBackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                this.backColor = value;
                Invalidate();
            }
        }

        public Color ClickedBorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                this.borderColor = value;
                Invalidate();
            }
        }

        public int ClickedBorderSize
        {
            get
            {
                return borderSize;
            }
            set
            {
                this.borderSize = value;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            tempBorderColor = BorderColor;
            tempBorderSize = BorderSize;
            tempBackColor = BackColor;

            BorderColor = borderColor;
            BorderSize = borderSize;
            BackColor = backColor;

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            BorderColor = tempBorderColor;
            BorderSize = tempBorderSize;
            BackColor = tempBackColor;

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            SizeF textSize = e.Graphics.MeasureString(Text, Font);

            using(Brush textBrush = new SolidBrush(ForeColor))
                e.Graphics.DrawString(Text, Font, textBrush, ((float)ClientSize.Width - textSize.Width)/2, ((float)ClientSize.Height - textSize.Height)/2);
        }
    }
}
