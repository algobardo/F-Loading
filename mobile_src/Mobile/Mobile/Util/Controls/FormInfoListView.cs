using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mobile.Communication;
using System.Collections;
using System.Drawing.Imaging;

namespace Mobile.Util.Controls
{
    public class FormListView : OwnerDrawnList
    {
        const int Column1Left = 5;
        const int Column2Left = 25;

        public FormListView()
        {
            // We need a total of 5 rows, so the height is 180/5=36
            Graphics g = this.CreateGraphics();
            Font font = new Font(this.Font.Name, 9, FontStyle.Bold);
            // Calc line height to be height of letter A plus 10%
            int fontHeight = (int)(g.MeasureString("A", font).Height * 1.1);
            this.ItemHeight = fontHeight * 2;
            g.Dispose();
        }

        // Check if the user presses the action key
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    MessageBox.Show("You selected item " + this.SelectedIndex.ToString(),
                        "Item selected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk,
                        MessageBoxDefaultButton.Button1);
                    break;
            }

            base.OnKeyDown(e);
        }


        /// <summary>
        /// Custom OnPaint. This paints the listview items
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Declare vars
            Font font;
            Color fontColor;

            // Get graphics object from bitmap.
            Graphics gOffScreen = Graphics.FromImage(this.OffScreen);

            // Set background color
            gOffScreen.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);

            // Set the y pos of the current item
            int itemTop = 0;

            // Draw the visible items.
            for (int n = this.VScrollBar.Value; n <= this.VScrollBar.Value + DrawCount; n++)
            {
                // Draw the selected item to appear highlighted
                if (n == this.SelectedIndex)
                {
                    gOffScreen.FillRectangle(new SolidBrush(SystemColors.Highlight),
                        0,
                        itemTop,
                        // If the scroll bar is visible, subtract the scrollbar width
                        this.ClientSize.Width - (this.VScrollBar.Visible ? this.VScrollBar.Width : 0),
                        this.ItemHeight);
                    fontColor = CalcTextColor(SystemColors.Highlight);
                }
                else
                    fontColor = this.ForeColor;

                // Draw a gray separator for each item
                gOffScreen.DrawLine(new Pen(Color.DarkGray),
                    1,
                    itemTop + this.ItemHeight,
                    this.ClientSize.Width - (this.VScrollBar.Visible ? this.VScrollBar.Width : 2),
                    itemTop + this.ItemHeight);

                // Get the current MailItem
                FormInfo info = (FormInfo)this.Items[n];

                // Set font and image pending mail state
                Bitmap bmp = null;
                font = new Font(this.Font.Name, 9, FontStyle.Bold);
                if (info.Status == FormStatus.Notified)
                    bmp = Mobile.Properties.Resources.FormNotifiedIcon;
                if (info.Status == FormStatus.Downloaded)
                    bmp = Mobile.Properties.Resources.FormDownloadedIcon;
                if (info.Status == FormStatus.Opened)
                    bmp = Mobile.Properties.Resources.FormOpenedIcon;

                ImageAttributes ia = new ImageAttributes();
                //ia.SetColorKey(Color.Red, Color.Red);

                // Set the image rectangle
                Rectangle imgRect = new Rectangle(Column1Left, itemTop + 3, bmp.Width, bmp.Height);

                // Draw the image
                gOffScreen.DrawImage(bmp, imgRect, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
                // Draw the mail sender
                gOffScreen.DrawString(info.Source, font, new SolidBrush(fontColor), Column2Left, itemTop);
                // Draw the mail subject
                gOffScreen.DrawString(info.Name, font, new SolidBrush(fontColor), Column2Left, itemTop + (ItemHeight / 2));

                // Cleanup
                font.Dispose();
                bmp.Dispose();

                // Set the next item top to move down the item height
                itemTop += this.ItemHeight;
            }

            // Now draw the visible list box
            e.Graphics.DrawImage(this.OffScreen, 0, 0);

            gOffScreen.Dispose();
        }

        // Draws the external border around the control.    
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
    }

    // Base custom control for OwnerDrawnList
    public class OwnerDrawnList : Control
    {
        int scrollWidth;
        int itemHeight = -1;
        int selectedIndex = -1;

        Bitmap offScreen;
        VScrollBar vs;
        ArrayList items;

        public OwnerDrawnList()
        {
            this.vs = new VScrollBar();
            scrollWidth = this.vs.Width;
            this.vs.Parent = this;
            this.vs.Visible = false;
            this.vs.SmallChange = 1;
            this.vs.ValueChanged += new EventHandler(this.ScrollValueChanged);

            // Items to draw
            this.items = new ArrayList();
        }

        public ArrayList Items
        {
            get { return this.items; }
        }

        protected Bitmap OffScreen
        {
            get { return this.offScreen; }
        }

        protected VScrollBar VScrollBar
        {
            get { return this.vs; }
        }

        public event EventHandler SelectedIndexChanged;

        // Raise the SelectedIndexChanged event
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(this, e);
        }

        // Get or set index of selected item.
        public int SelectedIndex
        {
            get { return this.selectedIndex; }

            set
            {
                this.selectedIndex = value;
                if (this.SelectedIndexChanged != null)
                    this.SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        protected void ScrollValueChanged(object o, EventArgs e)
        {
            this.Refresh();
        }

        protected virtual int ItemHeight
        {
            get { return this.itemHeight; }
            set { this.itemHeight = value; }
        }

        // If the requested index is before the first visible index then set the
        // first item to be the requested index. If it is after the last visible
        // index, then set the last visible index to be the requested index.
        public void EnsureVisible(int index)
        {
            if (index < this.vs.Value)
            {
                this.vs.Value = index;
                this.Refresh();
            }
            else if (index >= this.vs.Value + this.DrawCount)
            {
                this.vs.Value = index - this.DrawCount + 1;
                this.Refresh();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }
        // Selected item moves when you use the keyboard up/down keys.
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (this.SelectedIndex < this.vs.Maximum)
                    {
                        EnsureVisible(++this.SelectedIndex);
                        this.Refresh();
                    }
                    break;
                case Keys.Up:
                    if (this.SelectedIndex > this.vs.Minimum)
                    {
                        EnsureVisible(--this.SelectedIndex);
                        this.Refresh();
                    }
                    break;
            }

            base.OnKeyDown(e);
        }

        // Calculate how many items we can draw given the height of the control.
        protected int DrawCount
        {
            get
            {
                if (this.vs.Value + this.vs.LargeChange > this.vs.Maximum)
                    return this.vs.Maximum - this.vs.Value + 1;
                else
                    return this.vs.LargeChange;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            int viewableItemCount = this.ClientSize.Height / this.ItemHeight;

            this.vs.Bounds = new Rectangle(this.ClientSize.Width - scrollWidth,
                0,
                scrollWidth,
                this.ClientSize.Height);


            // Determine if scrollbars are needed
            if (this.items.Count > viewableItemCount)
            {
                this.vs.Visible = true;
                this.vs.LargeChange = viewableItemCount;
                this.offScreen = new Bitmap(this.ClientSize.Width - scrollWidth, this.ClientSize.Height);
            }
            else
            {
                this.vs.Visible = false;
                this.vs.LargeChange = this.items.Count;
                this.offScreen = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            }

            this.vs.Maximum = this.items.Count - 1;
        }

        // Determine what the text color should be
        // for the selected item drawn as highlighted
        protected Color CalcTextColor(Color backgroundColor)
        {
            if (backgroundColor.Equals(Color.Empty))
                return Color.Black;

            int sum = backgroundColor.R + backgroundColor.G + backgroundColor.B;

            if (sum > 256)
                return Color.Black;
            else
                return Color.White;
        }

    }
}
