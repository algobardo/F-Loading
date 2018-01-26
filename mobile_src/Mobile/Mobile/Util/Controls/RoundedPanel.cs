using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace Mobile.Util.Controls
{
    public partial class RoundedPanel : Panel, IRoundedControl
    {
        private int cornerRadius;
        private bool plainLeft;
        private bool plainRight;
        private bool plainTop;
        private bool plainBottom;
        private Color borderColor;
        private int borderSize;

        private Color filterColor;

        public RoundedPanel()
            : base()
        {
            cornerRadius = 0;
            borderSize = 0;
            filterColor = Color.Magenta;
        }

        public int CornerRadius
        {
            get
            {
                return cornerRadius;
            }
            set
            {
                this.cornerRadius = value;
                Invalidate();
            }
        }

        public bool PlainLeft
        {
            get
            {
                return plainLeft;
            }
            set
            {
                this.plainLeft = value;
                Invalidate();
            }
        }

        public bool PlainRight
        {
            get
            {
                return plainRight;
            }
            set
            {
                this.plainRight = value;
                Invalidate();
            }
        }

        public bool PlainTop
        {
            get
            {
                return plainTop;
            }
            set
            {
                this.plainTop = value;
                Invalidate();
            }
        }

        public bool PlainBottom
        {
            get
            {
                return plainBottom;
            }
            set
            {
                this.plainBottom = value;
                Invalidate();
            }
        }

        public Color BorderColor
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

        public int BorderSize
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

        public Color FilterColor
        {
            get
            {
                return filterColor;
            }
            set
            {
                this.filterColor = value;
                Invalidate();
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap offscreenBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics output = Graphics.FromImage(offscreenBitmap);

            EnhancedControlHelper.PaintRoundedBackground(
                output,
                ClientRectangle,
                Bounds,
                BorderSize,
                BorderColor,
                CornerRadius,
                plainLeft,
                plainRight,
                plainTop,
                plainBottom,
                false,
                BackColor,
                Parent,
                FilterColor);

            e.Graphics.DrawImage(offscreenBitmap, 0, 0);
            offscreenBitmap.Dispose();

            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
    }
}
