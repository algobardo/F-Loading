using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

namespace Mobile.Util.Controls
{
    public class BackgroundedButton : BackgroundedControl
    {
        private Bitmap clickBitmap;

        private bool mouseDown;
        private int wstring;
        public BackgroundedButton() :
            base()
        {
           
           
        }

     
    
        public Bitmap ClickedBackgroundImage
        {
            get
            {
                return clickBitmap;
            }
            set
            {
                if (clickBitmap != null)
                    clickBitmap.Dispose();

                clickBitmap = value;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.Clear(BackColor);

            Bitmap currentImage = mouseDown ? ClickedBackgroundImage : BackgroundImage;
            if (currentImage != null)
            {
                Bitmap offscreenBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
                Graphics output = Graphics.FromImage(offscreenBitmap);

                EnhancedControlHelper.PaintBackground(
                    output,
                    currentImage,
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
                this.MisureString();
               pe.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), (this.Width - wstring)/2, this.Height - (this.Height / 3));
            }
        }
        private void MisureString()
        {
            this.Font=new Font("Tahoma", 7F, FontStyle.Regular);
            Label test = new Label();
            test.Text = this.Text;
            test.Font = this.Font;
            float count = test.Font.Size;
            do
            {
                wstring = StringMeasure.Measure(test, test.Text, test.ClientRectangle).Width;
                this.Font = test.Font;
                count--;
                test.Font = new Font("Tahoma", count, FontStyle.Regular);

            } while (wstring >= this.Size.Width);
            test.Dispose();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseDown = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseDown = false;
            Invalidate();
            base.OnMouseUp(e);
        }
        public override void Refresh()
        {
            this.Font = new Font("Tahoma", 8F, FontStyle.Regular);
            base.Refresh();
        }
    }
}













