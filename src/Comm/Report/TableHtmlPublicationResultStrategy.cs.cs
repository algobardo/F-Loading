using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Storage;
using System.Xml;
using System.Xml.Linq;

namespace Comm.Report
{
    /// <summary>
    /// Iterate over all compilation result and build
    /// a string that contains a list of node's names,
    /// its attributes and the text value with html table tags
    /// NOTE: for complete this Strategy is necessary a format of XML document with doc
    /// </summary>  
    public class TableHtmlPublicationResultStrategy : ReportStrategy
    {
        private String openTr = "<tr>";
        private String closeTr = "</tr>";
        private String openTd = "<td>";
        private String closeTd = "</td>";
        private String newLine = "<br>";

      
        /// <summary>
        /// Build a string with the content of the XElement passed ordered
        /// and include html table tags (incluse header and footer)
        /// </summary>  
        /// <param name="doc">XElement for precess</param>
        /// <returns></returns>
        private string buildNode(XElement doc)
        {

            StringBuilder buf = new StringBuilder();
            String title = buf.Append(doc.Name).ToString();
            buf.Append(generateHtmlHeader(title));
            IEnumerable<XAttribute> attributes = doc.Attributes();
            foreach (XAttribute attr in attributes)
            {
                
                buf.Append(openTr).Append(attr.Name).Append(closeTr).Append(newLine).Append(openTr).Append(attr.Value).Append(closeTr);
            }
            buf.Append(newLine);
            foreach (XElement child in doc.Descendants())
                buf.Append(newLine).Append("-").Append(buildNode(child));
            buf.Append(generateHtmlFooter());
            return buf.ToString();
        }

        /// <summary>
        /// Build a string with html header
        /// </summary>  
        /// <param name="title">String rapresentative of title</param>
        /// <returns></returns>
        public static string generateHtmlHeader(String title)
        {

            String header = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\"> <html> <head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\"> <title> Report Floading " + title + " </title> </head> <body> ";
            return header;
        }

        /// <summary>
        /// Build a string with html footer
        /// </summary>  
        public static string generateHtmlFooter()
        {
            String footer = "</body></html>";
            return footer;
        }
          
        #region ReportStrategy Membri di

        public override PublicationResult ConvertOutput(Publication pub, int compilationRequestId)
        {
            StringBuilder buf = new StringBuilder();
            foreach (CompilationRequest req in pub.CompilationRequest)
            {
                if (compilationRequestId != -1 || compilationRequestId == req.compilReqID)
                {
                    foreach (Result data in req.Result)
                        buf.Append(buildNode(data.xmlResult)).Append(newLine);
                    if (compilationRequestId != -1)
                        break;
                }
            }

            PublicationResult res = new PublicationResult();
            res.Text = buf.ToString();
            return res;
        }

        public override string Description
        {
            get { return "text passed"; }
        }

        #endregion
    }
}
