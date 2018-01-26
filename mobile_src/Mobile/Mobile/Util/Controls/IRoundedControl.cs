using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mobile.Util.Controls
{
    public interface IRoundedControl
    {
        int CornerRadius
        {
            get;
            set;
        }

        Color BorderColor
        {
            get;
            set;
        }

        int BorderSize
        {
            get;
            set;
        }

        bool PlainTop
        {
            get;
            set;
        }

        bool PlainBottom
        {
            get;
            set;
        }

        bool PlainLeft
        {
            get;
            set;
        }

        bool PlainRight
        {
            get;
            set;
        }

        Color FilterColor
        {
            get;
            set;
        }
    }
}
