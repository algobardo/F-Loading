using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Util
{
    class TabIndexGenerator
    {
        private static int nextTabIndex = 0;

        public static int NextTabIndex()
        {
            return nextTabIndex++;
        }
    }
}
