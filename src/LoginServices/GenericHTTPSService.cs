using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Storage;

namespace LoginServices
{
    class GenericHTTPSService: HTTPSService
    {
        static string url = StorageManager.getEnvValue("genericHTTPSServiceurl");

        static string description = StorageManager.getEnvValue("genericHTTPSServicedescription");

        public GenericHTTPSService() : base(url, description)
        {
            
        }
    }
}
