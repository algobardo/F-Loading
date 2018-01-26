using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Mobile.Util.Controls
{
    public class BackgroundedPanel : Panel, IBackgroundedControl
    {
        private Bitmap background;
        private bool adapt;
        private SizeMode sizeXMode;
        private SizeMode sizeYMode;
        private HorizontalAlignment hAlignment;
        private VerticalAlignment vAlignment;

        private int headerHeight;
        private int footerHeight;
        private int marginLeftWidth;
        private int marginRightWidth;

        private int headerTileWidth;
        private int footerTileWidth;
        private int marginLeftTileHeight;
        private int marginRightTileHeight;

        private Color filterColor;
        private bool sizeToBackground;
        private Bitmap iconimage;

        public Bitmap IconImage
        {
            get
            {
                return iconimage;
            }
            set
            {
                if (iconimage != null)
                    iconimage.Dispose();
                iconimage = value;
            }
        }
        public Bitmap BackgroundImage
        {
            get
            {
                return background;
            }
            set
            {
                if (background != null)
                    background.Dispose();

                background = value;

                if (background != null && !adapt && sizeToBackground && sizeXMode == SizeMode.None && sizeYMode == SizeMode.None)
                    this.Size = background.Size;

                Invalidate();
            }
        }

        public bool AdaptBackground
        {
            get
            {
                return adapt;
            }
            set
            {
                this.adapt = value;

                if (background != null && !adapt && sizeToBackground && sizeXMode == SizeMode.None && sizeYMode == SizeMode.None)
                    this.Size = background.Size;

                Invalidate();
            }
        }

        public SizeMode SizeXMode
        {
            get
            {
                return sizeXMode;
            }
            set
            {
                this.sizeXMode = value;

                if (background != null && !adapt && sizeToBackground && sizeXMode == SizeMode.None && sizeYMode == SizeMode.None)
                    this.Size = background.Size;

                Invalidate();
            }
        }

        public SizeMode SizeYMode
        {
            get
            {
                return sizeYMode;
            }
            set
            {
                this.sizeYMode = value;

                if (sizeToBackground && sizeXMode == SizeMode.None && sizeYMode == SizeMode.None)
                    this.Size = background.Size;

                Invalidate();
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return hAlignment;
            }
            set
            {
                this.hAlignment = value;
                Invalidate();
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return vAlignment;
            }
            set
            {
                this.vAlignment = value;
                Invalidate();
            }
        }

        public int HeaderHeight
        {
            get
            {
                return headerHeight;
            }
            set
            {
                this.headerHeight = value;
                Invalidate();
            }
        }

        public int FooterHeight
        {
            get
            {
                return footerHeight;
            }
            set
            {
                this.footerHeight = value;
                Invalidate();
            }
        }

        public int HeaderTileWidth
        {
            get
            {
                return headerTileWidth;
            }
            set
            {
                this.headerTileWidth = value;
                Invalidate();
            }
        }

        public int FooterTileWidth
        {
            get
            {
                return footerTileWidth;
            }
            set
            {
                this.footerTileWidth = value;
                Invalidate();
            }
        }

        public int MarginLeftWidth
        {
            get
            {
                return marginLeftWidth;
            }
            set
            {
                marginLeftWidth = value;
                Invalidate();
            }
        }

        public int MarginRightWidth
        {
            get
            {
                return marginRightWidth;
            }
            set
            {
                marginRightWidth = value;
                Invalidate();
            }
        }

        public int MarginRightTileHeight
        {
            get
            {
                return marginRightTileHeight;
            }
            set
            {
                marginRightTileHeight = value;
                Invalidate();
            }
        }

        public int MarginLeftTileHeight
        {
            get
            {
                return marginLeftTileHeight;
            }
            set
            {
                marginLeftTileHeight = value;
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

        public bool SizeToBackground
        {
            get
            {
                return sizeToBackground;
            }
            set
            {
                this.sizeToBackground = value;

                if (background != null && !adapt && sizeToBackground && sizeXMode == SizeMode.None && sizeYMode == SizeMode.None)
                    this.Size = background.Size;

                Invalidate();
            }
        }

        public BackgroundedPanel()
            : base()
        {
            filterColor = Color.Transparent;
            sizeXMode = SizeMode.None;
            sizeYMode = SizeMode.None;
            hAlignment = HorizontalAlignment.Center;
            vAlignment = VerticalAlignment.Center;
            sizeToBackground = false;
            adapt = false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.Clear(BackColor);

            if (BackgroundImage != null)
            {
                Bitmap offscreenBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
                Graphics output = Graphics.FromImage(offscreenBitmap);

                EnhancedControlHelper.PaintBackground(
                    output,
                    BackgroundImage,
                    ClientSize,
                    Bounds,
                    HorizontalAlignment,
                    SizeXMode,
                    VerticalAlignment,
                    SizeYMode,
                    AdaptBackground,
                    HeaderHeight,
                    FooterHeight,
                    MarginLeftWidth,
                    MarginRightWidth,
                    HeaderTileWidth,
                    FooterTileWidth,
                    MarginLeftTileHeight,
                    MarginRightTileHeight,
                    Parent,
                    FilterColor);

                pe.Graphics.DrawImage(offscreenBitmap, 0, 0);
                offscreenBitmap.Dispose();
                if (iconimage != null)
                {
                    pe.Graphics.DrawImage(iconimage, (BackgroundImage.Width / 6) - iconimage.Width, (BackgroundImage.Width / 6) - iconimage.Width);
                    pe.Graphics.DrawString(this.Text, new Font("Tahoma", 8F, FontStyle.Bold), new SolidBrush(this.ForeColor), (BackgroundImage.Width / 6) + 5, (BackgroundImage.Width / 6) - iconimage.Width + 5);
                   // pe.Graphics.DrawString(this.Text, new Font("Tahoma", 8F, FontStyle.Bold), new SolidBrush(this.ForeColor), 0,0);
                }
            }

            base.OnPaint(pe);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
