using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mobile.Util.Controls
{
    public enum SizeMode { None, Repeat, Stretch }
    public enum HorizontalAlignment { Left, Right, Center }
    public enum VerticalAlignment { Top, Bottom, Center }
    
    public interface IBackgroundedControl
    {
        Bitmap BackgroundImage
        {
            get;
            set;
        }

        bool AdaptBackground
        {
            get;
            set;
        }

        SizeMode SizeXMode
        {
            get;
            set;
        }

        SizeMode SizeYMode
        {
            get;
            set;
        }

        HorizontalAlignment HorizontalAlignment
        {
            get;
            set;
        }

        VerticalAlignment VerticalAlignment
        {
            get;
            set;
        }

        int HeaderHeight
        {
            get;
            set;
        }

        int FooterHeight
        {
            get;
            set;
        }

        int HeaderTileWidth
        {
            get;
            set;
        }

        int FooterTileWidth
        {
            get;
            set;
        }

        int MarginLeftWidth
        {
            get;
            set;
        }

        int MarginRightWidth
        {
            get;
            set;
        }

        int MarginRightTileHeight
        {
            get;
            set;
        }

        int MarginLeftTileHeight
        {
            get;
            set;
        }

        Color FilterColor
        {
            get;
            set;
        }

        bool SizeToBackground
        {
            get;
            set;
        }
    }
}
