using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Settings
{
    public class Configuration
    {
        public String Host
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public bool SaveOnLowBattery
        {
            get;
            set;
        }

        public bool AutoDetectMessages
        {
            get;
            set;
        }
        public String Language
        {
            get;
            set;
        }
    }
}
