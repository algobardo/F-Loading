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
    public class EnhancedGroupBox : RoundedControl
    {
        private Color headerColor;
        private Color headerBackColor;
        private String headerText;
        private float headerOffset;
        private int headerHeight;
        private bool headerInline;
        private bool headerBordered;
        private Font headerFont;
        private int padding = 4;

        private Panel stackPanel;
        
        public Color HeaderColor
        {
            get
            {
                return headerColor;
            }
            set
            {
                headerColor = value;
                Invalidate();
            }
        }

        public Color HeaderBackColor
        {
            get
            {
                return headerBackColor;
            }
            set
            {
                headerBackColor = value;
                Invalidate();
            }
        }

        public Font HeaderFont
        {
            get
            {
                return headerFont;
            }
            set
            {
                headerFont = value;
                Invalidate();
            }
        }

        public String HeaderText
        {
            get
            {
                return headerText;
            }
            set
            {
                headerText = value;
                Invalidate();
            }
        }

        public float HeaderOffset
        {
            get
            {
                return headerOffset;
            }
            set
            {
                headerOffset = value;
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
                headerHeight = value;
                Invalidate();
            }
        }

        public bool InlineHeader
        {
            get
            {
                return headerInline;
            }
            set
            {
                headerInline = value;
                Invalidate();
            }
        }

        public bool BorderedHeader
        {
            get
            {
                return headerBordered;
            }
            set
            {
                headerBordered = value;
                Invalidate();
            }
        }

        public EnhancedGroupBox() 
            : base()
        {
            headerOffset = 0f;
            headerText = "GROUP";
            headerColor = Color.White;
            headerBackColor = Color.Black;
            headerInline = false;
            headerBordered = true;
            headerHeight = 20;
            headerFont = new Font("Tahoma", 8, FontStyle.Regular);

            stackPanel = new Panel()
            {
                Height = 0,
                Width = this.Width - (2 * padding),
                Left = padding,
                Top = HeaderHeight + (padding / 2)
            };
            this.Controls.Add(stackPanel);
        }

        public void AddElement(Control element)
        {
            Control container = CreateControllerContainer(element);
            stackPanel.Controls.Add(container);
            stackPanel.Height += container.Height;
            
            this.Height = stackPanel.Top + stackPanel.Height + padding;
         }

        private Control CreateControllerContainer(Control controller)
        {
            Control container = new Control()
            {
                Anchor = AnchorStyles.Right | AnchorStyles.Left,// | AnchorStyles.Top,
                //Top = padding,
                //Dock = DockStyle.Top,
                Width = stackPanel.Width,
                Height = controller.Height + (2 * padding),
                Top = padding
            };

            controller.Dock = DockStyle.Top ;

            container.Dock = DockStyle.Bottom;
            container.Controls.Add(controller);

            return container;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            stackPanel.Width = this.Width - (2 * padding);
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SizeF headerSize = e.Graphics.MeasureString(HeaderText, headerFont);
            if (!InlineHeader)
            {
                EnhancedControlHelper.PaintRoundedBackground(
                    e.Graphics,
                    new Rectangle(0, HeaderHeight, (int)Math.Ceiling(ClientSize.Width), ClientSize.Height - HeaderHeight),
                    Bounds,
                    BorderSize,
                    BorderColor,
                    CornerRadius,
                    false,
                    false,
                    false,
                    false,
                    false,
                    BackColor,
                    Parent,
                    FilterColor);

                Color fillColor = BackColor;
                if (BackColor == Color.Transparent)
                    fillColor = Parent.BackColor;

                Brush fillBrush = new SolidBrush(fillColor);

                RectangleF headerRectangle = new RectangleF(
                    Math.Max(HeaderOffset * ClientSize.Width, CornerRadius + BorderSize),
                    0,
                    Math.Min(ClientSize.Width - BorderSize - Math.Max(HeaderOffset * headerSize.Width, CornerRadius + BorderSize), headerSize.Width + 8),
                    HeaderHeight);

                if (HeaderOffset == 0)
                    headerRectangle.X = 0;

                if (HeaderOffset == 1)
                    headerRectangle.X = ClientSize.Width - headerRectangle.Width;

                RectangleF textRectangle = new RectangleF(
                    headerRectangle.X + (headerRectangle.Width - headerSize.Width) / 2,
                    headerRectangle.Y + (HeaderHeight - headerSize.Height) / 2,
                    headerRectangle.Width,
                    headerRectangle.Height);

                e.Graphics.FillRectangle(
                    fillBrush,
                    (int)Math.Ceiling(headerRectangle.X),
                    (int)Math.Ceiling(HeaderHeight),
                    (int)Math.Ceiling(headerRectangle.Width),
                    (int)Math.Ceiling((headerOffset != 0 && headerOffset != 1) ? BorderSize : BorderSize + CornerRadius));

                Brush borderBursh = new SolidBrush(BorderColor);

                if (headerOffset == 0)
                    e.Graphics.FillRectangle(borderBursh, new Rectangle(0, (int)Math.Ceiling(HeaderHeight), BorderSize, BorderSize + CornerRadius));
                if (headerOffset == 1)
                    e.Graphics.FillRectangle(borderBursh, new Rectangle(ClientSize.Width - BorderSize, (int)Math.Ceiling(headerSize.Height + HeaderHeight), BorderSize, BorderSize + CornerRadius));

                EnhancedControlHelper.PaintRoundedRectangle(
                    e.Graphics,
                    BorderColor,
                    BorderSize,
                    HeaderBackColor,
                    new Rectangle((int)Math.Ceiling(headerRectangle.X), (int)Math.Ceiling(headerRectangle.Y), (int)Math.Ceiling(headerRectangle.Width), (int)headerRectangle.Height + BorderSize),
                    new Size(CornerRadius, CornerRadius),
                    false,
                    false,
                    false,
                    true);

                Brush textBrush = new SolidBrush(HeaderColor);
                e.Graphics.DrawString(HeaderText, headerFont, textBrush, textRectangle);

                borderBursh.Dispose();
                fillBrush.Dispose();
            }
            else
            {
                EnhancedControlHelper.PaintRoundedBackground(
                    e.Graphics,
                    new Rectangle(0, (int)(headerSize.Height / 2), (int)Math.Ceiling(ClientSize.Width), ClientSize.Height - (int)(headerSize.Height / 2)),
                    Bounds,
                    BorderSize,
                    BorderColor,
                    CornerRadius,
                    false,
                    false,
                    false,
                    false,
                    false,
                    BackColor,
                    Parent,
                    FilterColor);

                RectangleF headerRectangle = new RectangleF(
                    Math.Max(HeaderOffset * ClientSize.Width, CornerRadius + BorderSize),
                    0,
                    Math.Min(ClientSize.Width - BorderSize - Math.Max(HeaderOffset * headerSize.Width, CornerRadius + BorderSize), HeaderHeight),
                    headerSize.Height);

                RectangleF textRectangle = new RectangleF(
                    headerRectangle.X + (headerRectangle.Width - headerSize.Width) / 2,
                    (headerRectangle.Height - headerSize.Height) / 2,
                    headerRectangle.Width,
                    headerSize.Height);

                Color fillColor = BackColor;
                if (BackColor == Color.Transparent)
                    fillColor = Parent.BackColor;

                if (HeaderOffset == 0)
                    headerRectangle.X = 0;

                if (HeaderOffset == 1)
                    headerRectangle.X = ClientSize.Width - headerRectangle.Width;

                Brush textBrush = new SolidBrush(HeaderColor);
                e.Graphics.DrawString(HeaderText, headerFont, textBrush, textRectangle);

                textBrush.Dispose();
            }
        }


        
        /*Size headerSize = StringMeasure.Measure(this, HeaderText, this.ClientRectangle);
            Size headerSize = new Size(0, 20);
            int offset = headerSize.Height + (2 * HeaderVerticalMargin) + Padding[1];
            for (int i = 0; i < Controls.Count; i++)
            {
                Controls[i].Resize -= OnResizeChild;
                Controls[i].Resize += OnResizeChild;

                Controls[i].Left = Padding[0];
                Controls[i].Top = offset;
                offset += Controls[i].Height + ControlsPadding;
            }

            this.Height = offset - ControlsPadding + Padding[3];
        }

        private void OnResizeChild(object source, EventArgs args)
        {
            PerformControlsLayout();
            Invalidate();
        }*/
    }
}
