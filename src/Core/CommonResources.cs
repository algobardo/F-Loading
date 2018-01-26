using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace Core
{
    public static class CommonResources
    {
        private static void handler(object sender, EventArgs e)
        {
        }
        public static XmlSchema BaseTypes
        {
            get
            {
                return XmlSchema.Read(new StringReader(TestResources.BaseTypes), handler);
                     
            }
        }

        public static XmlNamespaceManager AllNamespaces { get; set; }
    }
}
