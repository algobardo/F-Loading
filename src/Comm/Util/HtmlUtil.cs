using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comm.Util
{
    class HtmlUtil
    {
        public static string generateHtmlHeader(String title)
        {

            String header = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\"> <html> <head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\"> <title> Report Floading " + title + " </title> </head> <body> ";
            return header;
        }

        public static string generateHtmlFooter()
        {
            String footer = "</body></html>";
            return footer;
        }
    }
}
