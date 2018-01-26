using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Mobile.Util.Controls
{
    public static class EnhancedControlHelper
    {
        public static void PaintBackground(
            Graphics graphics,
            Bitmap background,
            Size size,
            Rectangle bounds,
            HorizontalAlignment hAlignment,
            SizeMode sizeXMode,
            VerticalAlignment vAlignment,
            SizeMode sizeYMode,
            bool adapt,
            int headerHeight,
            int footerHeight,
            int marginLeftWidth,
            int marginRightWidth,
            int headerTileWidth,
            int footerTileWidth,
            int marginLeftTileHeight,
            int marginRightTileHeight,
            Control parent,
            Color filterColor)
        {
            ImageAttributes attributes = new ImageAttributes();
            if (filterColor != Color.Transparent) 
            {
                attributes.SetColorKey(filterColor, filterColor);
                PaintParentBackground(graphics, bounds, parent);
            }
            if (!adapt)
            {
                int startImageX = 0, startImageY = 0;
                int sizeImageX = 0, sizeImageY = 0;
                int startBackgroundX = 0, startBackgroundY = 0;
                int sizeBackgroundX = 0, sizeBackgroundY = 0;

                if (sizeXMode == SizeMode.None)
                {
                    sizeImageX = background.Width;
                    sizeBackgroundX = background.Width;
                    if (hAlignment == HorizontalAlignment.Left)
                    {
                        startBackgroundX = 0;
                        startImageX = 0;
                    }
                    if (hAlignment == HorizontalAlignment.Right)
                    {
                        startBackgroundX = (size.Width - background.Width) > 0 ? size.Width - background.Width : 0;
                        startImageX = (size.Width - background.Width) > 0 ? 0 : background.Width - size.Width;
                    }
                    if (hAlignment == HorizontalAlignment.Center)
                    {
                        startBackgroundX = (size.Width - background.Width) > 0 ? (size.Width - background.Width) / 2 : 0;
                        startImageX = (size.Width - background.Width) > 0 ? 0 : (background.Width - size.Width) / 2;
                    }
                }
                if (sizeXMode == SizeMode.Stretch)
                {
                    sizeImageX = background.Width;
                    sizeBackgroundX = size.Width;
                }
                if (sizeYMode == SizeMode.None)
                {
                    sizeImageY = background.Height;
                    sizeBackgroundY = background.Height;
                    if (vAlignment == VerticalAlignment.Top)
                    {
                        startBackgroundY = 0;
                        startImageY = 0;
                    }
                    if (vAlignment == VerticalAlignment.Bottom)
                    {
                        startBackgroundY = (size.Height - background.Height) > 0 ? size.Height - background.Height : 0;
                        startImageY = (size.Height - background.Height) > 0 ? 0 : background.Height - size.Height;
                    }
                    if (vAlignment == VerticalAlignment.Center)
                    {
                        startBackgroundY = (size.Height - background.Height) > 0 ? (size.Height - background.Height) / 2 : 0;
                        startImageY = (size.Height - background.Height) > 0 ? 0 : (background.Height - size.Height) / 2;
                    }
                }
                if (sizeYMode == SizeMode.Stretch)
                {
                    sizeImageY = background.Height;
                    sizeBackgroundY = size.Height;
                }

                graphics.DrawImage(
                    background,
                    new Rectangle(startBackgroundX, startBackgroundY, sizeBackgroundX, sizeBackgroundY),
                    startImageX,
                    startImageY,
                    sizeImageX,
                    sizeImageY,
                    GraphicsUnit.Pixel,
                    attributes);
            }
            else
            {
                graphics.DrawImage(
                    background,
                    new Rectangle(0, 0, marginLeftWidth, headerHeight),
                    0,
                    0,
                    marginLeftWidth,
                    headerHeight,
                    GraphicsUnit.Pixel,
                    attributes);

                graphics.DrawImage(
                    background,
                    new Rectangle(size.Width - marginRightWidth, 0, marginRightWidth, headerHeight),
                    background.Width - marginRightWidth,
                    0,
                    marginRightWidth,
                    headerHeight,
                    GraphicsUnit.Pixel,
                    attributes);

                graphics.DrawImage(
                    background,
                    new Rectangle(0, size.Height - footerHeight, marginLeftWidth, footerHeight),
                    0,
                    background.Height - footerHeight,
                    marginLeftWidth,
                    footerHeight,
                    GraphicsUnit.Pixel,
                    attributes);

                graphics.DrawImage(
                    background,
                    new Rectangle(size.Width - marginRightWidth, size.Height - footerHeight, marginRightWidth, footerHeight),
                    background.Width - marginRightWidth,
                    background.Height - footerHeight,
                    marginRightWidth,
                    footerHeight,
                    GraphicsUnit.Pixel,
                    attributes);

                //Draw top border
                if (headerTileWidth == 0)
                    headerTileWidth = background.Width - marginLeftWidth - marginRightWidth;
                if (footerTileWidth == 0)
                    footerTileWidth = background.Width - marginLeftWidth - marginRightWidth;
                if (marginLeftTileHeight == 0)
                    marginLeftTileHeight = background.Height - headerHeight - footerHeight;
                if (marginRightTileHeight == 0)
                    marginRightTileHeight = background.Height - headerHeight - footerHeight;

                int offset = marginLeftWidth;
                while (offset < size.Width - marginRightWidth)
                {
                    graphics.DrawImage(
                        background,
                        new Rectangle(offset, 0, headerTileWidth, headerHeight),
                        marginLeftWidth,
                        0,
                        headerTileWidth,
                        headerHeight,
                        GraphicsUnit.Pixel,
                        attributes);

                    offset += headerTileWidth;
                }

                //Draw bottom border
                offset = marginLeftWidth;
                while (offset < size.Width - marginRightWidth)
                {
                    graphics.DrawImage(
                        background,
                        new Rectangle(offset, size.Height - footerHeight, footerTileWidth, footerHeight),
                        marginLeftWidth,
                        background.Height - footerHeight,
                        footerTileWidth,
                        footerHeight,
                        GraphicsUnit.Pixel,
                        attributes);

                    offset += footerTileWidth;
                }

                //Draw left border
                offset = headerHeight;
                while (offset < size.Height - footerHeight)
                {
                    graphics.DrawImage(
                        background,
                        new Rectangle(0, offset, marginLeftWidth, marginRightTileHeight),
                        0,
                        headerHeight,
                        marginLeftWidth,
                        marginLeftTileHeight,
                        GraphicsUnit.Pixel,
                        attributes);

                    offset += marginLeftTileHeight;
                }

                //Draw bottom border
                offset = headerHeight;
                while (offset < size.Height - footerHeight)
                {
                    graphics.DrawImage(
                        background,
                        new Rectangle(size.Width - marginRightWidth, offset, marginRightWidth, marginRightTileHeight),
                        background.Width - marginRightWidth,
                        headerHeight,
                        marginRightWidth,
                        marginRightTileHeight,
                        GraphicsUnit.Pixel,
                        attributes);

                    offset += marginRightTileHeight;
                }

                //Draw the center stretched
                graphics.DrawImage(
                    background,
                    new Rectangle(marginLeftWidth, headerHeight, size.Width - marginLeftWidth - marginRightWidth, size.Height - headerHeight - footerHeight),
                    marginLeftWidth,
                    headerHeight,
                    background.Width - marginLeftWidth - marginRightWidth,
                    background.Height - headerHeight - footerHeight,
                    GraphicsUnit.Pixel,
                    attributes);
            }
        }

        public static void PaintBackground(
            Graphics graphics,
            Bitmap background,
            Size size,
            Rectangle bounds,
            HorizontalAlignment hAlignment,
            SizeMode sizeXMode,
            VerticalAlignment vAlignment,
            SizeMode sizeYMode,
            Control parent,
            Color filterColor)
        {
            PaintBackground(
                graphics,
                background,
                size,
                bounds,
                hAlignment,
                sizeXMode,
                vAlignment,
                sizeYMode,
                false,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                parent,
                filterColor);
        }
           

        public static void PaintParentBackground(
            Graphics graphics,
            Rectangle bounds,
            Control parent)
        {
            if (parent != null)
            {
                if (parent is IBackgroundedControl && ((IBackgroundedControl)parent).BackgroundImage != null)
                {
                    IBackgroundedControl p = parent as IBackgroundedControl;
                    Bitmap offset = new Bitmap(parent.ClientSize.Width, parent.ClientSize.Height);
                    PaintBackground(
                        Graphics.FromImage(offset),
                        p.BackgroundImage,
                        parent.ClientSize,
                        parent.Bounds,
                        p.HorizontalAlignment,
                        p.SizeXMode,
                        p.VerticalAlignment,
                        p.SizeYMode,
                        null,
                        p.FilterColor);

                    graphics.DrawImage(offset.Clip(bounds), 0, 0);
                    offset.Dispose();
                }
                else
                    graphics.Clear(parent.BackColor);
            }
        }

        public static void PaintRoundedBackground (
            Graphics graphics,
            Rectangle rect,
            Rectangle bounds,
            int borderSize,
            Color borderColor,
            int radius,
            bool plainLeft,
            bool plainRight,
            bool plainTop,
            bool plainBottom,
            bool circular,
            Color backColor,
            Control parent,
            Color filterColor)
        {
            if (!circular && (radius == 0 || (plainLeft && plainRight && plainTop && plainBottom)))
            {
                Brush borderBrush = new SolidBrush(borderColor);
                Brush fillBrush = new SolidBrush(backColor);

                //Draw borders
                graphics.FillRectangle(borderBrush, rect.X, rect.Y, rect.Width, borderSize);
                graphics.FillRectangle(borderBrush, rect.Width - borderSize, rect.Y, borderSize, rect.Height);
                graphics.FillRectangle(borderBrush, rect.X, rect.Height - borderSize, rect.Width, borderSize);
                graphics.FillRectangle(borderBrush, 0, 0, borderSize, rect.Height);

                //Fill the center
                if (fillBrush != null)
                {
                    if (backColor == Color.Transparent)
                        PaintParentBackground(graphics, new Rectangle(rect.X + borderSize, rect.Y + borderSize, rect.Width - (2 * borderSize), rect.Height - (2 * borderSize)), parent);
                    else
                        graphics.FillRectangle(fillBrush, rect.X + borderSize, rect.Y + borderSize, rect.Width - (2 * borderSize), rect.Height - (2 * borderSize));
                    
                    fillBrush.Dispose();
                }

                borderBrush.Dispose();
            }
            else
            {
                //Draw the rounded rectangle
                Bitmap maskBitmap = new Bitmap(bounds.Width, bounds.Height);
                Graphics maskGraphics = Graphics.FromImage(maskBitmap);
                maskGraphics.Clear(filterColor);

                Color fillColor = backColor;

                if (!circular)
                    PaintRoundedRectangle(
                       maskGraphics,
                       borderColor,
                       borderSize,
                       fillColor,
                       new Rectangle(rect.X, rect.Y, rect.Width, rect.Height),
                       new Size(radius, radius),
                       plainLeft,
                       plainRight,
                       plainTop,
                       plainBottom);
                else
                    PaintCircle(
                        maskGraphics,
                        borderColor,
                        borderSize,
                        fillColor,
                        new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));

                //Mask the background with the rounded rectangle
                //Draw offsetGraphics on the tiled image
                ImageAttributes maskAttributes = new ImageAttributes();
                maskAttributes.SetColorKey(filterColor, filterColor);
                
                PaintParentBackground(graphics, bounds, parent);
                graphics.DrawImage(
                    maskBitmap, 
                    new Rectangle(0, 0, bounds.Width, bounds.Height), 
                    0, 
                    0, 
                    bounds.Width, 
                    bounds.Height, 
                    GraphicsUnit.Pixel, 
                    maskAttributes);


                maskGraphics.Dispose();
                maskBitmap.Dispose();
            }
        }
        /*
        public static void PaintEnhancedBackground(
            PaintEventArgs e, 
            Rectangle rect, 
            Rectangle Bounds, 
            int BorderSize, 
            Color BorderColor,
            int radius,
            bool plainLeft,
            bool plainRight,
            bool plainTop,
            bool plainBottom,
            bool circular,
            Color BackColor, 
            Bitmap BackgroundImage, 
            Control Parent)
        {
            if (!circular && (radius == 0 || (plainLeft && plainRight && plainTop && plainBottom)))
            {
                Brush borderBrush = new SolidBrush(BorderColor);
                Brush fillBrush = null;

                if (BackgroundImage != null)
                    e.Graphics.DrawImage(BackgroundImage, rect.X, rect.Y);
                else if (BackColor == Color.Transparent)
                {
                    if (Parent is IBackgroundedControl && ((IBackgroundedControl)Parent).BackgroundImage != null)
                        e.Graphics.DrawImage(((IBackgroundedControl)Parent).BackgroundImage.Clip(Bounds), 0, 0);
                    else
                        fillBrush = new SolidBrush(Parent.BackColor);
                }
                else
                    fillBrush = new SolidBrush(BackColor);

                //Draw borders
                e.Graphics.FillRectangle(borderBrush, rect.X, rect.Y, rect.Width, BorderSize);
                e.Graphics.FillRectangle(borderBrush, rect.Width - BorderSize, rect.Y, BorderSize, rect.Height);
                e.Graphics.FillRectangle(borderBrush, rect.X, rect.Height - BorderSize, rect.Width, BorderSize);
                e.Graphics.FillRectangle(borderBrush, 0, 0, BorderSize, rect.Height);

                //Fill the center
                if (fillBrush != null)
                {
                    e.Graphics.FillRectangle(fillBrush, rect.X + BorderSize, rect.Y + BorderSize, rect.Width - (2 * BorderSize), rect.Height - (2 * BorderSize));
                    fillBrush.Dispose();
                }

                borderBrush.Dispose();
            }
            else
            {
                //Rectangle screen = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
                //int width = ClientSize.Width;
                //int height = ClientSize.Height;

                //Draw everything offline, set transparency color key e copy into the screen area
                Bitmap finalBitmap = new Bitmap(Bounds.Width, Bounds.Height);
                Graphics finalGraphics = Graphics.FromImage(finalBitmap);

                //Prepare background with the color of the parent or its background image
                Color parentColor = Parent != null ? Parent.BackColor : Color.Transparent;
                if (Parent is IBackgroundedControl && ((IBackgroundedControl)Parent).BackgroundImage != null)
                    finalGraphics.DrawImage(((IBackgroundedControl)Parent).BackgroundImage.Clip(Bounds), 0, 0);
                else
                    finalGraphics.Clear(parentColor);

                //Draw the tiled background 
                Bitmap backgroundBitmap = null;
                Graphics backgroundGraphics = null;

                if (BackgroundImage != null)
                {
                    backgroundBitmap = new Bitmap(Bounds.Width, Bounds.Height);
                    backgroundGraphics = Graphics.FromImage(backgroundBitmap);
                    backgroundGraphics.DrawImage(BackgroundImage, 0, 0);
                }

                //Draw the rounded rectangle
                Bitmap maskBitmap = new Bitmap(Bounds.Width, Bounds.Height);
                Graphics maskGraphics = Graphics.FromImage(maskBitmap);
                maskGraphics.Clear(Color.Magenta);

                Color fillColor = BackColor;
                if (BackgroundImage != null)
                {
                    fillColor = Color.Magenta;
                    maskGraphics.Clear(Color.Green);
                }
                else if (BackColor == Color.Transparent)
                {
                    if (Parent is IBackgroundedControl && ((IBackgroundedControl)Parent).BackgroundImage != null)
                        fillColor = Color.Magenta;
                    else
                        fillColor = parentColor;
                }

                if (!circular)
                    PaintRoundedRectangle(
                       maskGraphics,
                       BorderColor,
                       BorderSize,
                       fillColor,
                       new Rectangle(rect.X, rect.Y, rect.Width, rect.Height),
                       new Size(radius, radius),
                       plainLeft,
                       plainRight,
                       plainTop,
                       plainBottom);
                else
                    PaintCircle(
                        maskGraphics,
                        BorderColor,
                        BorderSize,
                        fillColor,
                        new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));

                //Mask the background with the rounded rectangle
                //Draw offsetGraphics on the tiled image
                ImageAttributes maskAttributes = new ImageAttributes();
                maskAttributes.SetColorKey(Color.Magenta, Color.Magenta);

                if (BackgroundImage != null)
                {
                    backgroundGraphics.DrawImage(maskBitmap, new Rectangle(0, 0, Bounds.Width, Bounds.Height), 0, 0, Bounds.Width, Bounds.Height, GraphicsUnit.Pixel, maskAttributes);

                    //Mask the final bitmap with the background
                    ImageAttributes backgroundAttributes = new ImageAttributes();
                    backgroundAttributes.SetColorKey(Color.Green, Color.Green);
                    finalGraphics.DrawImage(backgroundBitmap, new Rectangle(0, 0, Bounds.Width, Bounds.Height), 0, 0, Bounds.Width, Bounds.Height, GraphicsUnit.Pixel, backgroundAttributes);

                    backgroundGraphics.Dispose();
                    backgroundBitmap.Dispose();
                }
                else
                    finalGraphics.DrawImage(maskBitmap, new Rectangle(0, 0, Bounds.Width, Bounds.Height), 0, 0, Bounds.Width, Bounds.Height, GraphicsUnit.Pixel, maskAttributes);


                maskGraphics.Dispose();
                maskBitmap.Dispose();

                //Draw the final bitmap on the screen
                e.Graphics.DrawImage(finalBitmap, 0, 0);

                finalGraphics.Dispose();
                finalBitmap.Dispose();
            }
        }*/

        public static void PaintRoundedRectangle(
            Graphics graphics, 
            Color borderColor, 
            int borderSize, 
            Color backgroundColor, 
            Rectangle rectangle, 
            Size radius,
            bool plainLeft,
            bool plainRight,
            bool plainTop,
            bool plainBottom)
        {
            Brush borderBrush = new SolidBrush(borderColor);
            Brush fillBrush = new SolidBrush(backgroundColor);

            Size outerEllipsesSize = new Size(2 * (radius.Width), 2 * (radius.Height));
            Size innerEllipsesSize = new Size(2 * (radius.Width - borderSize), 2 * (radius.Height - borderSize));

            if(!plainTop && !plainLeft)
                graphics.FillEllipse(borderBrush, rectangle.Left, rectangle.Top, outerEllipsesSize.Width, outerEllipsesSize.Height);
            if(!plainTop && ! plainRight)
                graphics.FillEllipse(borderBrush, rectangle.Right - (2 * radius.Width) - 1, rectangle.Top, outerEllipsesSize.Width, outerEllipsesSize.Height);
            if(!plainBottom && !plainLeft)
                graphics.FillEllipse(borderBrush, rectangle.Left, rectangle.Bottom - (2 * radius.Height) - 1, outerEllipsesSize.Width, outerEllipsesSize.Height);
            if(!plainBottom && !plainRight)
                graphics.FillEllipse(borderBrush, rectangle.Right - (2 * radius.Width) - 1, rectangle.Bottom - (2 * radius.Width) - 1, outerEllipsesSize.Width, outerEllipsesSize.Height);

            graphics.FillRectangle(borderBrush, rectangle.Left + radius.Width, rectangle.Top, rectangle.Width - (2 * radius.Width), rectangle.Height);
            graphics.FillRectangle(borderBrush, rectangle.Left, rectangle.Top + radius.Height, rectangle.Width, rectangle.Height - (2 * radius.Height));

            if (plainLeft)
                graphics.FillRectangle(borderBrush, rectangle.Left, rectangle.Top, rectangle.Width - radius.Width, rectangle.Height);
            if (plainRight)
                graphics.FillRectangle(borderBrush, rectangle.Left + radius.Width, rectangle.Top, rectangle.Width - radius.Width, rectangle.Height);
            if (plainTop)
                graphics.FillRectangle(borderBrush, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height - radius.Height);
            if (plainBottom)
                graphics.FillRectangle(borderBrush, rectangle.Left, rectangle.Top + radius.Height, rectangle.Width, rectangle.Height - radius.Height);
            
            if (!plainTop && !plainLeft)
                graphics.FillEllipse(fillBrush, rectangle.Left + borderSize, rectangle.Top + borderSize, innerEllipsesSize.Width, innerEllipsesSize.Height);
            if (!plainTop && !plainRight)
                graphics.FillEllipse(fillBrush, rectangle.Right - (2 * radius.Width) + borderSize - 1, rectangle.Top + borderSize, innerEllipsesSize.Width, innerEllipsesSize.Height);
            if (!plainBottom && !plainLeft)
                graphics.FillEllipse(fillBrush, rectangle.Left + borderSize, rectangle.Bottom - (2 * radius.Height) + borderSize - 1, innerEllipsesSize.Width, innerEllipsesSize.Height);
            if (!plainBottom && !plainRight)
                graphics.FillEllipse(fillBrush, rectangle.Right - (2 * radius.Width) + borderSize - 1, rectangle.Bottom - (2 * radius.Width) + borderSize - 1, innerEllipsesSize.Width, innerEllipsesSize.Height);

            graphics.FillRectangle(fillBrush, rectangle.Left + radius.Width, rectangle.Top + borderSize, rectangle.Width - (2 * radius.Width), rectangle.Height - (2 * borderSize));
            graphics.FillRectangle(fillBrush, rectangle.Left + borderSize, rectangle.Top + radius.Height, rectangle.Width - (2 * borderSize), rectangle.Height - (2 * radius.Height));

            if (plainLeft)
                graphics.FillRectangle(fillBrush, rectangle.Left + borderSize, rectangle.Top + borderSize, rectangle.Width - radius.Width - borderSize, rectangle.Height - (2 * borderSize));
            if (plainRight)
                graphics.FillRectangle(fillBrush, rectangle.Left + radius.Width, rectangle.Top + borderSize, rectangle.Width - radius.Width - borderSize, rectangle.Height - (2 * borderSize));
            if (plainTop)
                graphics.FillRectangle(fillBrush, rectangle.Left + borderSize, rectangle.Top + borderSize, rectangle.Width - (2 * borderSize), rectangle.Height - radius.Height - borderSize);
            if (plainBottom)
                graphics.FillRectangle(fillBrush, rectangle.Left + borderSize, rectangle.Top + radius.Height, rectangle.Width - (2 * borderSize), rectangle.Height - radius.Height - borderSize);
            
            borderBrush.Dispose();
            fillBrush.Dispose();
        }

        public static void PaintCircle(
            Graphics graphics,
            Color borderColor,
            int borderSize,
            Color backgroundColor,
            Rectangle rectangle)
        {
            Brush borderBrush = new SolidBrush(borderColor);
            Brush fillBrush = new SolidBrush(backgroundColor);

            graphics.FillEllipse(borderBrush, rectangle);
            graphics.FillEllipse(fillBrush, new Rectangle(rectangle.X + borderSize, rectangle.Y + borderSize, rectangle.Width - (2 * borderSize), rectangle.Height - (2 * borderSize)));

            borderBrush.Dispose();
            fillBrush.Dispose();
        }

        public static void CreateTiledImage(Graphics graphics, Bitmap image, SizeMode Tiling, Size ClientSize)
        {
            int maxWidth = ClientSize.Width, maxHeight = ClientSize.Height;
            if (Tiling != SizeMode.Repeat)
            {
                maxWidth = image.Width;
                maxHeight = image.Height;
            }

            for (int currentHeight = 0; currentHeight < maxHeight; currentHeight += image.Height)
                for (int currentWidth = 0; currentWidth < maxWidth; currentWidth += image.Width)
                    graphics.DrawImage(image, currentWidth, currentHeight);
        }

        public static Bitmap GetSS(this Graphics grx, Rectangle bounds)
        {
            var res = new Bitmap(bounds.Width, bounds.Height);
            var gxc = Graphics.FromImage(res);
            IntPtr hdc = grx.GetHdc();
            PlatformAPI.BitBlt(gxc.GetHdc(), 0, 0, bounds.Width, bounds.Height, hdc, bounds.Left, bounds.Top, PlatformAPI.SRCCOPY);
            grx.ReleaseHdc(hdc);
            return res;
        }

        public static Bitmap Clip(this Bitmap source, Rectangle bounds)
        {
            var grx = Graphics.FromImage(source);
            return grx.GetSS(bounds);
        }

        internal class PlatformAPI
        {
            [DllImport("coredll.dll")]
            public static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref Rect lpRect, uint uFormat);

            [DllImport("coredll.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("coredll.dll")]
            public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

            public const uint SRCCOPY = 0x00CC0020;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            public int Left, Top, Right, Bottom;
            public static implicit operator Rectangle(Rect value)
            {
                return new Rectangle(value.Left, value.Top, value.Right - value.Left, value.Bottom - value.Top);
            }

            public static implicit operator Rect(Rectangle value)
            {
                return new Rect() { Top = value.Top, Left = value.Left, Bottom = value.Bottom, Right = value.Right };
            }
        }
    }
}
